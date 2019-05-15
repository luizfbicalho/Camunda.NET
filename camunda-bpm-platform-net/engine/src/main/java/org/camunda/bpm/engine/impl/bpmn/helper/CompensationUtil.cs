using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.bpmn.helper
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ActivityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;
	using FlowScopeWalker = org.camunda.bpm.engine.impl.tree.FlowScopeWalker;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CompensationUtil
	{

	  /// <summary>
	  /// name of the signal that is thrown when a compensation handler completed
	  /// </summary>
	  public const string SIGNAL_COMPENSATION_DONE = "compensationDone";

	  /// <summary>
	  /// we create a separate execution for each compensation handler invocation.
	  /// </summary>
	  public static void throwCompensationEvent(IList<EventSubscriptionEntity> eventSubscriptions, ActivityExecution execution, bool async)
	  {

		// first spawn the compensating executions
		foreach (EventSubscriptionEntity eventSubscription in eventSubscriptions)
		{
		  // check whether compensating execution is already created
		  // (which is the case when compensating an embedded subprocess,
		  // where the compensating execution is created when leaving the subprocess
		  // and holds snapshot data).
		  ExecutionEntity compensatingExecution = getCompensatingExecution(eventSubscription);
		  if (compensatingExecution != null)
		  {
			if (compensatingExecution.Parent != execution)
			{
			  // move the compensating execution under this execution if this is not the case yet
			  compensatingExecution.Parent = (PvmExecutionImpl) execution;
			}

			compensatingExecution.EventScope = false;
		  }
		  else
		  {
			compensatingExecution = (ExecutionEntity) execution.createExecution();
			eventSubscription.Configuration = compensatingExecution.Id;
		  }
		  compensatingExecution.Concurrent = true;
		}

		// signal compensation events in REVERSE order of their 'created' timestamp
		eventSubscriptions.Sort(new ComparatorAnonymousInnerClass());

		foreach (EventSubscriptionEntity compensateEventSubscriptionEntity in eventSubscriptions)
		{
		  compensateEventSubscriptionEntity.eventReceived(null, async);
		}
	  }

	  private class ComparatorAnonymousInnerClass : IComparer<EventSubscriptionEntity>
	  {
		  public int compare(EventSubscriptionEntity o1, EventSubscriptionEntity o2)
		  {
			return o2.Created.compareTo(o1.Created);
		  }
	  }

	  /// <summary>
	  /// creates an event scope for the given execution:
	  /// 
	  /// create a new event scope execution under the parent of the given execution
	  /// and move all event subscriptions to that execution.
	  /// 
	  /// this allows us to "remember" the event subscriptions after finishing a
	  /// scope
	  /// </summary>
	  public static void createEventScopeExecution(ExecutionEntity execution)
	  {

		// parent execution is a subprocess or a miBody
		ActivityImpl activity = execution.getActivity();
		ExecutionEntity scopeExecution = (ExecutionEntity) execution.findExecutionForFlowScope(activity.FlowScope);

		IList<EventSubscriptionEntity> eventSubscriptions = execution.CompensateEventSubscriptions;

		if (eventSubscriptions.Count > 0 || hasCompensationEventSubprocess(activity))
		{

		  ExecutionEntity eventScopeExecution = scopeExecution.createExecution();
		  eventScopeExecution.setActivity(execution.getActivity());
		  eventScopeExecution.activityInstanceStarting();
		  eventScopeExecution.enterActivityInstance();
		  eventScopeExecution.Active = false;
		  eventScopeExecution.Concurrent = false;
		  eventScopeExecution.EventScope = true;

		  // copy local variables to eventScopeExecution by value. This way,
		  // the eventScopeExecution references a 'snapshot' of the local variables
		  IDictionary<string, object> variables = execution.VariablesLocal;
		  foreach (KeyValuePair<string, object> variable in variables.SetOfKeyValuePairs())
		  {
			eventScopeExecution.setVariableLocal(variable.Key, variable.Value);
		  }

		  // set event subscriptions to the event scope execution:
		  foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
		  {
			EventSubscriptionEntity newSubscription = EventSubscriptionEntity.createAndInsert(eventScopeExecution, EventType.COMPENSATE, eventSubscriptionEntity.Activity);
			newSubscription.Configuration = eventSubscriptionEntity.Configuration;
			// use the original date
			newSubscription.Created = eventSubscriptionEntity.Created;
		  }

		  // set existing event scope executions as children of new event scope execution
		  // (ensuring they don't get removed when 'execution' gets removed)
		  foreach (PvmExecutionImpl childEventScopeExecution in execution.EventScopeExecutions)
		  {
			childEventScopeExecution.Parent = eventScopeExecution;
		  }

		  ActivityImpl compensationHandler = getEventScopeCompensationHandler(execution);
		  EventSubscriptionEntity eventSubscription = EventSubscriptionEntity.createAndInsert(scopeExecution, EventType.COMPENSATE, compensationHandler);
		  eventSubscription.Configuration = eventScopeExecution.Id;

		}
	  }

	  protected internal static bool hasCompensationEventSubprocess(ActivityImpl activity)
	  {
		ActivityImpl compensationHandler = activity.findCompensationHandler();

		return compensationHandler != null && compensationHandler.SubProcessScope && compensationHandler.TriggeredByEvent;
	  }

	  /// <summary>
	  /// In the context when an event scope execution is created (i.e. a scope such as a subprocess has completed),
	  /// this method returns the compensation handler activity that is going to be executed when by the event scope execution.
	  /// 
	  /// This method is not relevant when the scope has a boundary compensation handler.
	  /// </summary>
	  protected internal static ActivityImpl getEventScopeCompensationHandler(ExecutionEntity execution)
	  {
		ActivityImpl activity = execution.getActivity();

		ActivityImpl compensationHandler = activity.findCompensationHandler();
		if (compensationHandler != null && compensationHandler.SubProcessScope)
		{
		  // subprocess with inner compensation event subprocess
		  return compensationHandler;
		}
		else
		{
		  // subprocess without compensation handler or
		  // multi instance activity
		  return activity;
		}
	  }

	  /// <summary>
	  /// Collect all compensate event subscriptions for scope of given execution.
	  /// </summary>
	  public static IList<EventSubscriptionEntity> collectCompensateEventSubscriptionsForScope(ActivityExecution execution)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<org.camunda.bpm.engine.impl.pvm.process.ScopeImpl, org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl> scopeExecutionMapping = execution.createActivityExecutionMapping();
		IDictionary<ScopeImpl, PvmExecutionImpl> scopeExecutionMapping = execution.createActivityExecutionMapping();
		ScopeImpl activity = (ScopeImpl) execution.Activity;

		// <LEGACY>: different flow scopes may have the same scope execution =>
		// collect subscriptions in a set
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity> subscriptions = new java.util.HashSet<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity>();
		ISet<EventSubscriptionEntity> subscriptions = new HashSet<EventSubscriptionEntity>();
		TreeVisitor<ScopeImpl> eventSubscriptionCollector = new TreeVisitorAnonymousInnerClass(execution, scopeExecutionMapping, subscriptions);

		(new FlowScopeWalker(activity)).addPostVisitor(eventSubscriptionCollector).walkUntil(new WalkConditionAnonymousInnerClass());

		return new List<EventSubscriptionEntity>(subscriptions);
	  }

	  private class TreeVisitorAnonymousInnerClass : TreeVisitor<ScopeImpl>
	  {
		  private ActivityExecution execution;
		  private IDictionary<ScopeImpl, PvmExecutionImpl> scopeExecutionMapping;
		  private ISet<EventSubscriptionEntity> subscriptions;

		  public TreeVisitorAnonymousInnerClass(ActivityExecution execution, IDictionary<ScopeImpl, PvmExecutionImpl> scopeExecutionMapping, ISet<EventSubscriptionEntity> subscriptions)
		  {
			  this.execution = execution;
			  this.scopeExecutionMapping = scopeExecutionMapping;
			  this.subscriptions = subscriptions;
		  }

		  public void visit(ScopeImpl obj)
		  {
			PvmExecutionImpl execution = scopeExecutionMapping[obj];
			subscriptions.addAll(((ExecutionEntity) execution).CompensateEventSubscriptions);
		  }
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  public bool isFulfilled(ScopeImpl element)
		  {
			bool? consumesCompensationProperty = (bool?) element.getProperty(BpmnParse.PROPERTYNAME_CONSUMES_COMPENSATION);
			return consumesCompensationProperty == null || consumesCompensationProperty == true;
		  }
	  }

	  /// <summary>
	  /// Collect all compensate event subscriptions for activity on the scope of
	  /// given execution.
	  /// </summary>
	  public static IList<EventSubscriptionEntity> collectCompensateEventSubscriptionsForActivity(ActivityExecution execution, string activityRef)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity> eventSubscriptions = collectCompensateEventSubscriptionsForScope(execution);
		IList<EventSubscriptionEntity> eventSubscriptions = collectCompensateEventSubscriptionsForScope(execution);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subscriptionActivityId = getSubscriptionActivityId(execution, activityRef);
		string subscriptionActivityId = getSubscriptionActivityId(execution, activityRef);

		IList<EventSubscriptionEntity> eventSubscriptionsForActivity = new List<EventSubscriptionEntity>();
		foreach (EventSubscriptionEntity subscription in eventSubscriptions)
		{
		  if (subscriptionActivityId.Equals(subscription.ActivityId))
		  {
			eventSubscriptionsForActivity.Add(subscription);
		  }
		}
		return eventSubscriptionsForActivity;
	  }

	  public static ExecutionEntity getCompensatingExecution(EventSubscriptionEntity eventSubscription)
	  {
		string configuration = eventSubscription.Configuration;
		if (!string.ReferenceEquals(configuration, null))
		{
		  return Context.CommandContext.ExecutionManager.findExecutionById(configuration);
		}
		else
		{
		  return null;
		}
	  }

	  private static string getSubscriptionActivityId(ActivityExecution execution, string activityRef)
	  {
		ActivityImpl activityToCompensate = ((ExecutionEntity) execution).getProcessDefinition().findActivity(activityRef);

		if (activityToCompensate.MultiInstance)
		{

		  ActivityImpl flowScope = (ActivityImpl) activityToCompensate.FlowScope;
		  return flowScope.ActivityId;
		}
		else
		{

		  ActivityImpl compensationHandler = activityToCompensate.findCompensationHandler();
		  if (compensationHandler != null)
		  {
			return compensationHandler.ActivityId;
		  }
		  else
		  {
			// if activityRef = subprocess and subprocess has no compensation handler
			return activityRef;
		  }
		}
	  }

	}

}