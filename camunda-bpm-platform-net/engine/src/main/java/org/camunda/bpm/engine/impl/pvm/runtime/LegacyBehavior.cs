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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil.SIGNAL_COMPENSATION_DONE;


	using BpmnBehaviorLogger = org.camunda.bpm.engine.impl.bpmn.behavior.BpmnBehaviorLogger;
	using CancelBoundaryEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CancelBoundaryEventActivityBehavior;
	using CancelEndEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CancelEndEventActivityBehavior;
	using CompensationEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CompensationEventActivityBehavior;
	using EventSubProcessActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessActivityBehavior;
	using MultiInstanceActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior;
	using ReceiveTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ReceiveTaskActivityBehavior;
	using SequentialMultiInstanceActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.SequentialMultiInstanceActivityBehavior;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.SubProcessActivityBehavior;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using GetActivityInstanceCmd = org.camunda.bpm.engine.impl.cmd.GetActivityInstanceCmd;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using ActivityInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.ActivityInstanceImpl;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ExecutionWalker = org.camunda.bpm.engine.impl.tree.ExecutionWalker;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using WalkCondition = org.camunda.bpm.engine.impl.tree.ReferenceWalker.WalkCondition;

	/// <summary>
	/// This class encapsulates legacy runtime behavior for the process engine.
	/// <para>
	/// Since 7.3 the behavior of certain bpmn elements has changed slightly.
	/// </para>
	/// <para>
	/// 
	/// 1. Some elements which did not used to be scopes are now scopes:
	/// <ul>
	///  <li>Sequential multi instance Embedded Subprocess: is now a scope, used to be non-scope.</li>
	///  <li>Event subprocess: is now a scope, used to be non-scope.</li>
	/// </ul>
	/// 
	/// 2. In certain situations, executions which were both scope and concurrent were created.
	/// This used to be the case if a scope execution already had a single scope child execution
	/// and then concurrency was introduced (by a on interrupting boundary event or
	/// a non-interrupting event subprocess).  In that case the existing scope execution
	/// was made concurrent. Starting from 7.3 this behavior is considered legacy.
	/// The new behavior is that the existing scope execution will not be made concurrent, instead,
	/// a new, concurrent execution will be created and be interleaved between the parent and the
	/// existing scope execution.
	/// </para>
	/// <para>
	/// 
	/// @author Daniel Meyer
	/// @since 7.3
	/// </para>
	/// </summary>
	public class LegacyBehavior
	{

	  private static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  // concurrent scopes ///////////////////////////////////////////

	  /// <summary>
	  /// Prunes a concurrent scope. This can only happen if
	  /// (a) the process instance has been migrated from a previous version to a new version of the process engine
	  /// 
	  /// This is an inverse operation to <seealso cref="#createConcurrentScope(PvmExecutionImpl)"/>.
	  /// 
	  /// See: javadoc of this class for note about concurrent scopes.
	  /// </summary>
	  /// <param name="execution"> </param>
	  public static void pruneConcurrentScope(PvmExecutionImpl execution)
	  {
		ensureConcurrentScope(execution);
		LOG.debugConcurrentScopeIsPruned(execution);
		execution.Concurrent = false;
	  }

	  /// <summary>
	  /// Cancels an execution which is both concurrent and scope. This can only happen if
	  /// (a) the process instance has been migrated from a previous version to a new version of the process engine
	  /// 
	  /// See: javadoc of this class for note about concurrent scopes.
	  /// </summary>
	  /// <param name="execution"> the concurrent scope execution to destroy </param>
	  /// <param name="cancelledScopeActivity"> the activity that cancels the execution; it must hold that
	  ///   cancellingActivity's event scope is the scope the execution is responsible for </param>
	  public static void cancelConcurrentScope(PvmExecutionImpl execution, PvmActivity cancelledScopeActivity)
	  {
		ensureConcurrentScope(execution);
		LOG.debugCancelConcurrentScopeExecution(execution);

		execution.interrupt("Scope " + cancelledScopeActivity + " cancelled.");
		// <!> HACK set to event scope activity and leave activity instance
		execution.setActivity(cancelledScopeActivity);
		execution.leaveActivityInstance();
		execution.interrupt("Scope " + cancelledScopeActivity + " cancelled.");
		execution.destroy();
	  }

	  /// <summary>
	  /// Destroys a concurrent scope Execution. This can only happen if
	  /// (a) the process instance has been migrated from a previous version to a 7.3+ version of the process engine
	  /// 
	  /// See: javadoc of this class for note about concurrent scopes.
	  /// </summary>
	  /// <param name="execution"> the execution to destroy </param>
	  public static void destroyConcurrentScope(PvmExecutionImpl execution)
	  {
		ensureConcurrentScope(execution);
		LOG.destroyConcurrentScopeExecution(execution);
		execution.destroy();
	  }

	  // sequential multi instance /////////////////////////////////

	  public static bool eventSubprocessComplete(ActivityExecution scopeExecution)
	  {
		bool performLegacyBehavior = isLegacyBehaviorRequired(scopeExecution);

		if (performLegacyBehavior)
		{
		  LOG.completeNonScopeEventSubprocess();
		  scopeExecution.end(false);
		}

		return performLegacyBehavior;
	  }

	  public static bool eventSubprocessConcurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {
		bool performLegacyBehavior = isLegacyBehaviorRequired(endedExecution);

		if (performLegacyBehavior)
		{
		  LOG.endConcurrentExecutionInEventSubprocess();
		  // notify the grandparent flow scope in a similar way PvmAtomicOperationAcitivtyEnd does
		  ScopeImpl flowScope = endedExecution.Activity.FlowScope;
		  if (flowScope != null)
		  {
			flowScope = flowScope.FlowScope;

			if (flowScope != null)
			{
			  if (flowScope == endedExecution.Activity.ProcessDefinition)
			  {
				endedExecution.remove();
				scopeExecution.tryPruneLastConcurrentChild();
				scopeExecution.forceUpdate();
			  }
			  else
			  {
				PvmActivity flowScopeActivity = (PvmActivity) flowScope;

				ActivityBehavior activityBehavior = flowScopeActivity.ActivityBehavior;
				if (activityBehavior is CompositeActivityBehavior)
				{
				  ((CompositeActivityBehavior) activityBehavior).concurrentChildExecutionEnded(scopeExecution, endedExecution);
				}
			  }
			}
		  }
		}

		return performLegacyBehavior;
	  }

	  /// <summary>
	  /// Destroy an execution for an activity that was previously not a scope and now is
	  /// (e.g. event subprocess)
	  /// </summary>
	  public static bool destroySecondNonScope(PvmExecutionImpl execution)
	  {
		ensureScope(execution);
		bool performLegacyBehavior = isLegacyBehaviorRequired(execution);

		if (performLegacyBehavior)
		{
		  // legacy behavior is to do nothing
		}

		return performLegacyBehavior;
	  }

	  /// <summary>
	  /// This method </summary>
	  /// <param name="scopeExecution">
	  /// @return </param>
	  protected internal static bool isLegacyBehaviorRequired(ActivityExecution scopeExecution)
	  {
		// legacy behavior is turned off: the current activity was parsed as scope.
		// now we need to check whether a scope execution was correctly created for the
		// event subprocess.

		// first create the mapping:
		IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = scopeExecution.createActivityExecutionMapping();
		// if the scope execution for the current activity is the same as for the parent scope
		// -> we need to perform legacy behavior
		PvmScope activity = scopeExecution.Activity;
		if (!activity.Scope)
		{
		  activity = activity.FlowScope;
		}
		return activityExecutionMapping[activity] == activityExecutionMapping[activity.FlowScope];
	  }

	  /// <summary>
	  /// In case the process instance was migrated from a previous version, activities which are now parsed as scopes
	  /// do not have scope executions. Use the flow scopes of these activities in order to find their execution.
	  /// - For an event subprocess this is the scope execution of the scope in which the event subprocess is embeded in
	  /// - For a multi instance sequential subprocess this is the multi instace scope body.
	  /// </summary>
	  /// <param name="scope"> </param>
	  /// <param name="activityExecutionMapping">
	  /// @return </param>
	  public static PvmExecutionImpl getScopeExecution(ScopeImpl scope, IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
	  {
		ScopeImpl flowScope = scope.FlowScope;
		return activityExecutionMapping[flowScope];
	  }

	  // helpers ////////////////////////////////////////////////

	  protected internal static void ensureConcurrentScope(PvmExecutionImpl execution)
	  {
		ensureScope(execution);
		ensureConcurrent(execution);
	  }

	  protected internal static void ensureConcurrent(PvmExecutionImpl execution)
	  {
		if (!execution.Concurrent)
		{
		  throw new ProcessEngineException("Execution must be concurrent.");
		}
	  }

	  protected internal static void ensureScope(PvmExecutionImpl execution)
	  {
		if (!execution.Scope)
		{
		  throw new ProcessEngineException("Execution must be scope.");
		}
	  }

	  /// <summary>
	  /// Creates an activity execution mapping, when the scope hierarchy and the execution hierarchy are out of sync.
	  /// </summary>
	  /// <param name="scopeExecutions"> </param>
	  /// <param name="scopes">
	  /// @return </param>
	  public static IDictionary<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping(IList<PvmExecutionImpl> scopeExecutions, IList<ScopeImpl> scopes)
	  {
		PvmExecutionImpl deepestExecution = scopeExecutions[0];
		if (isLegacyAsyncAtMultiInstance(deepestExecution))
		{
		  // in case the deepest execution is in fact async at multi-instance, the multi instance body is part
		  // of the list of scopes, however it is not instantiated yet or has already ended. Thus it must be removed.
		  scopes.RemoveAt(0);
		}

		// The trees are out of sync.
		// We are missing executions:
		int numOfMissingExecutions = scopes.Count - scopeExecutions.Count;

		// We need to find out which executions are missing.
		// Consider: elements which did not use to be scopes are now scopes.
		// But, this does not mean that all instances of elements which became scopes
		// are missing their executions. We could have created new instances in the
		// lower part of the tree after legacy behavior was turned off while instances of these elements
		// further up the hierarchy miss scopes. So we need to iterate from the top down and skip all scopes which
		// were not scopes before:
		scopeExecutions.Reverse();
		scopes.Reverse();

		IDictionary<ScopeImpl, PvmExecutionImpl> mapping = new Dictionary<ScopeImpl, PvmExecutionImpl>();
		// process definition / process instance.
		mapping[scopes[0]] = scopeExecutions[0];
		// nested activities
		int executionCounter = 0;
		for (int i = 1; i < scopes.Count; i++)
		{
		  ActivityImpl scope = (ActivityImpl) scopes[i];

		  PvmExecutionImpl scopeExecutionCandidate = null;
		  if (executionCounter + 1 < scopeExecutions.Count)
		  {
			scopeExecutionCandidate = scopeExecutions[executionCounter + 1];
		  }

		  if (numOfMissingExecutions > 0 && wasNoScope(scope, scopeExecutionCandidate))
		  {
			// found a missing scope
			numOfMissingExecutions--;
		  }
		  else
		  {
			executionCounter++;
		  }

		  if (executionCounter >= scopeExecutions.Count)
		  {
			throw new ProcessEngineException("Cannot construct activity-execution mapping: there are " + "more scope executions missing than explained by the flow scope hierarchy.");
		  }

		  PvmExecutionImpl execution = scopeExecutions[executionCounter];
		  mapping[scope] = execution;
		}

		return mapping;
	  }

	  /// <summary>
	  /// Determines whether the given scope was a scope in previous versions
	  /// </summary>
	  protected internal static bool wasNoScope(ActivityImpl activity, PvmExecutionImpl scopeExecutionCandidate)
	  {
		return wasNoScope72(activity) || wasNoScope73(activity, scopeExecutionCandidate);
	  }

	  protected internal static bool wasNoScope72(ActivityImpl activity)
	  {
		ActivityBehavior activityBehavior = activity.ActivityBehavior;
		ActivityBehavior parentActivityBehavior = (ActivityBehavior)(activity.FlowScope != null ? activity.FlowScope.ActivityBehavior : null);
		return (activityBehavior is EventSubProcessActivityBehavior) || (activityBehavior is SubProcessActivityBehavior && parentActivityBehavior is SequentialMultiInstanceActivityBehavior) || (activityBehavior is ReceiveTaskActivityBehavior && parentActivityBehavior is MultiInstanceActivityBehavior);
	  }

	  protected internal static bool wasNoScope73(ActivityImpl activity, PvmExecutionImpl scopeExecutionCandidate)
	  {
		ActivityBehavior activityBehavior = activity.ActivityBehavior;
		return (activityBehavior is CompensationEventActivityBehavior) || (activityBehavior is CancelEndEventActivityBehavior) || isMultiInstanceInCompensation(activity, scopeExecutionCandidate);
	  }

	  protected internal static bool isMultiInstanceInCompensation(ActivityImpl activity, PvmExecutionImpl scopeExecutionCandidate)
	  {
		return activity.ActivityBehavior is MultiInstanceActivityBehavior && ((scopeExecutionCandidate != null && findCompensationThrowingAncestorExecution(scopeExecutionCandidate) != null) || scopeExecutionCandidate == null);
	  }

	  /// <summary>
	  /// This returns true only if the provided execution has reached its wait state in a legacy engine version, because
	  /// only in that case, it can be async and waiting at the inner activity wrapped by the miBody. In versions >= 7.3,
	  /// the execution would reference the multi-instance body instead.
	  /// </summary>
	  protected internal static bool isLegacyAsyncAtMultiInstance(PvmExecutionImpl execution)
	  {
		ActivityImpl activity = execution.getActivity();

		if (activity != null)
		{
		  bool isAsync = string.ReferenceEquals(execution.ActivityInstanceId, null);
		  bool isAtMultiInstance = activity.ParentFlowScopeActivity != null && activity.ParentFlowScopeActivity.ActivityBehavior is MultiInstanceActivityBehavior;


		  return isAsync && isAtMultiInstance;
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Tolerates the broken execution trees fixed with CAM-3727 where there may be more
	  /// ancestor scope executions than ancestor flow scopes;
	  /// 
	  /// In that case, the argument execution is removed, the parent execution of the argument
	  /// is returned such that one level of mismatch is corrected.
	  /// 
	  /// Note that this does not necessarily skip the correct scope execution, since
	  /// the broken parent-child relationships may be anywhere in the tree (e.g. consider a non-interrupting
	  /// boundary event followed by a subprocess (i.e. scope), when the subprocess ends, we would
	  /// skip the subprocess's execution).
	  /// 
	  /// </summary>
	  public static PvmExecutionImpl determinePropagatingExecutionOnEnd(PvmExecutionImpl propagatingExecution, IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
	  {
		if (!propagatingExecution.Scope)
		{
		  // non-scope executions may end in the "wrong" flow scope
		  return propagatingExecution;
		}
		else
		{
		  // superfluous scope executions won't be contained in the activity-execution mapping
		  if (activityExecutionMapping.Values.Contains(propagatingExecution))
		  {
			return propagatingExecution;
		  }
		  else
		  {
			// skip one scope
			propagatingExecution.remove();
			PvmExecutionImpl parent = propagatingExecution.Parent;
			parent.setActivity(propagatingExecution.getActivity());
			return propagatingExecution.Parent;
		  }
		}
	  }

	  /// <summary>
	  /// Concurrent + scope executions are legacy and could occur in processes with non-interrupting
	  /// boundary events or event subprocesses
	  /// </summary>
	  public static bool isConcurrentScope(PvmExecutionImpl propagatingExecution)
	  {
		return propagatingExecution.Concurrent && propagatingExecution.Scope;
	  }

	  /// <summary>
	  /// <para>Required for migrating active sequential MI receive tasks. These activities were formerly not scope,
	  /// but are now. This has the following implications:
	  /// 
	  /// </para>
	  /// <para>Before migration:
	  /// <ul><li> the event subscription is attached to the miBody scope execution</ul>
	  /// 
	  /// </para>
	  /// <para>After migration:
	  /// <ul><li> a new subscription is created for every instance
	  /// <li> the new subscription is attached to a dedicated scope execution as a child of the miBody scope
	  ///   execution</ul>
	  /// 
	  /// </para>
	  /// <para>Thus, this method removes the subscription on the miBody scope
	  /// </para>
	  /// </summary>
	  public static void removeLegacySubscriptionOnParent(ExecutionEntity execution, EventSubscriptionEntity eventSubscription)
	  {
		ActivityImpl activity = execution.getActivity();
		if (activity == null)
		{
		  return;
		}

		ActivityBehavior behavior = activity.ActivityBehavior;
		ActivityBehavior parentBehavior = (ActivityBehavior)(activity.FlowScope != null ? activity.FlowScope.ActivityBehavior : null);

		if (behavior is ReceiveTaskActivityBehavior && parentBehavior is MultiInstanceActivityBehavior)
		{
		  IList<EventSubscriptionEntity> parentSubscriptions = execution.Parent.EventSubscriptions;

		  foreach (EventSubscriptionEntity subscription in parentSubscriptions)
		  {
			// distinguish a boundary event on the mi body with the same message name from the receive task subscription
			if (areEqualEventSubscriptions(subscription, eventSubscription))
			{
			  subscription.delete();
			}
		  }
		}

	  }

	  /// <summary>
	  /// Checks if the parameters are the same apart from the execution id
	  /// </summary>
	  protected internal static bool areEqualEventSubscriptions(EventSubscriptionEntity subscription1, EventSubscriptionEntity subscription2)
	  {
		return valuesEqual(subscription1.EventType, subscription2.EventType) && valuesEqual(subscription1.EventName, subscription2.EventName) && valuesEqual(subscription1.ActivityId, subscription2.ActivityId);

	  }

	  protected internal static bool valuesEqual<T>(T value1, T value2)
	  {
		return (value1 == default(T) && value2 == default(T)) || (value1 != default(T) && value1.Equals(value2));
	  }

	  /// <summary>
	  /// Remove all entries for legacy non-scopes given that the assigned scope execution is also responsible for another scope
	  /// </summary>
	  public static void removeLegacyNonScopesFromMapping(IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
	  {
		IDictionary<PvmExecutionImpl, IList<ScopeImpl>> scopesForExecutions = new Dictionary<PvmExecutionImpl, IList<ScopeImpl>>();

		foreach (KeyValuePair<ScopeImpl, PvmExecutionImpl> mappingEntry in mapping.SetOfKeyValuePairs())
		{
		  IList<ScopeImpl> scopesForExecution = scopesForExecutions[mappingEntry.Value];
		  if (scopesForExecution == null)
		  {
			scopesForExecution = new List<ScopeImpl>();
			scopesForExecutions[mappingEntry.Value] = scopesForExecution;
		  }

		  scopesForExecution.Add(mappingEntry.Key);
		}

		foreach (KeyValuePair<PvmExecutionImpl, IList<ScopeImpl>> scopesForExecution in scopesForExecutions.SetOfKeyValuePairs())
		{
		  IList<ScopeImpl> scopes = scopesForExecution.Value;

		  if (scopes.Count > 1)
		  {
			ScopeImpl topMostScope = getTopMostScope(scopes);

			foreach (ScopeImpl scope in scopes)
			{
			  if (scope != scope.ProcessDefinition && scope != topMostScope)
			  {
				mapping.Remove(scope);
			  }
			}
		  }
		}
	  }

	  protected internal static ScopeImpl getTopMostScope(IList<ScopeImpl> scopes)
	  {
		ScopeImpl topMostScope = null;

		foreach (ScopeImpl candidateScope in scopes)
		{
		  if (topMostScope == null || candidateScope.isAncestorFlowScopeOf(topMostScope))
		  {
			topMostScope = candidateScope;
		  }
		}

		return topMostScope;
	  }

	  /// <summary>
	  /// This is relevant for <seealso cref="GetActivityInstanceCmd"/> where in case of legacy multi-instance execution trees, the default
	  /// algorithm omits multi-instance activity instances.
	  /// </summary>
	  public static void repairParentRelationships(ICollection<ActivityInstanceImpl> values, string processInstanceId)
	  {
		foreach (ActivityInstanceImpl activityInstance in values)
		{
		  // if the determined activity instance id and the parent activity instance are equal,
		  // just put the activity instance under the process instance
		  if (valuesEqual(activityInstance.Id, activityInstance.ParentActivityInstanceId))
		  {
			activityInstance.ParentActivityInstanceId = processInstanceId;
		  }
		}
	  }

	  /// <summary>
	  /// When deploying an async job definition for an activity wrapped in an miBody, set the activity id to the
	  /// miBody except the wrapped activity is marked as async.
	  /// 
	  /// Background: in <= 7.2 async job definitions were created for the inner activity, although the
	  /// semantics are that they are executed before the miBody is entered
	  /// </summary>
	  public static void migrateMultiInstanceJobDefinitions(ProcessDefinitionEntity processDefinition, IList<JobDefinitionEntity> jobDefinitions)
	  {
		foreach (JobDefinitionEntity jobDefinition in jobDefinitions)
		{

		  string activityId = jobDefinition.ActivityId;
		  if (!string.ReferenceEquals(activityId, null))
		  {
			ActivityImpl activity = processDefinition.findActivity(jobDefinition.ActivityId);

			if (!isAsync(activity) && isActivityWrappedInMultiInstanceBody(activity) && isAsyncJobDefinition(jobDefinition))
			{
			  jobDefinition.ActivityId = activity.FlowScope.Id;
			}
		  }
		}
	  }

	  protected internal static bool isAsync(ActivityImpl activity)
	  {
		return activity.AsyncBefore || activity.AsyncAfter;
	  }

	  protected internal static bool isAsyncJobDefinition(JobDefinitionEntity jobDefinition)
	  {
		return AsyncContinuationJobHandler.TYPE.Equals(jobDefinition.JobType);
	  }

	  protected internal static bool isActivityWrappedInMultiInstanceBody(ActivityImpl activity)
	  {
		ScopeImpl flowScope = activity.FlowScope;

		if (flowScope != activity.ProcessDefinition)
		{
		  ActivityImpl flowScopeActivity = (ActivityImpl) flowScope;

		  return flowScopeActivity.ActivityBehavior is MultiInstanceActivityBehavior;
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// When executing an async job for an activity wrapped in an miBody, set the execution to the
	  /// miBody except the wrapped activity is marked as async.
	  /// 
	  /// Background: in <= 7.2 async jobs were created for the inner activity, although the
	  /// semantics are that they are executed before the miBody is entered
	  /// </summary>
	  public static void repairMultiInstanceAsyncJob(ExecutionEntity execution)
	  {
		ActivityImpl activity = execution.getActivity();

		if (!isAsync(activity) && isActivityWrappedInMultiInstanceBody(activity))
		{
		  execution.setActivity((ActivityImpl) activity.FlowScope);
		}
	  }

	  /// <summary>
	  /// With prior versions, the boundary event was already executed when compensation was performed; Thus, after
	  /// compensation completes, the execution is signalled waiting at the boundary event.
	  /// </summary>
	  public static bool signalCancelBoundaryEvent(string signalName)
	  {
		return SIGNAL_COMPENSATION_DONE.Equals(signalName);
	  }

	  /// <seealso cref= #signalCancelBoundaryEvent(String) </seealso>
	  public static void parseCancelBoundaryEvent(ActivityImpl activity)
	  {
		activity.setProperty(BpmnParse.PROPERTYNAME_THROWS_COMPENSATION, true);
	  }

	  /// <summary>
	  /// <para>In general, only leaf executions have activity ids.</para>
	  /// <para>Exception to that rule: compensation throwing executions.</para>
	  /// <para>Legacy exception (<= 7.2) to that rule: miBody executions and parallel gateway executions</para>
	  /// </summary>
	  /// <returns> true, if the argument is not a leaf and has an invalid (i.e. legacy) non-null activity id </returns>
	  public static bool hasInvalidIntermediaryActivityId(PvmExecutionImpl execution)
	  {
		return execution.NonEventScopeExecutions.Count > 0 && !CompensationBehavior.isCompensationThrowing(execution);
	  }

	  /// <summary>
	  /// Returns true if the given execution is in a compensation-throwing activity but there is no dedicated scope execution
	  /// in the given mapping.
	  /// </summary>
	  public static bool isCompensationThrowing(PvmExecutionImpl execution, IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
	  {
		if (CompensationBehavior.isCompensationThrowing(execution))
		{
		  ScopeImpl compensationThrowingActivity = execution.getActivity();

		  if (compensationThrowingActivity.Scope)
		  {
			return activityExecutionMapping[compensationThrowingActivity] == activityExecutionMapping[compensationThrowingActivity.FlowScope];
		  }
		  else
		  {
			// for transaction sub processes with cancel end events, the compensation throwing execution waits in the boundary event, not in the end
			// event; cancel boundary events are currently not scope
			return compensationThrowingActivity.ActivityBehavior is CancelBoundaryEventActivityBehavior;
		  }
		}
		else
		{
		  return false;
		}
	  }

	  public static bool isCompensationThrowing(PvmExecutionImpl execution)
	  {
		return isCompensationThrowing(execution, execution.createActivityExecutionMapping());
	  }

	  protected internal static PvmExecutionImpl findCompensationThrowingAncestorExecution(PvmExecutionImpl execution)
	  {
		ExecutionWalker walker = new ExecutionWalker(execution);
		walker.walkUntil(new WalkConditionAnonymousInnerClass());

		return walker.CurrentElement;
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<PvmExecutionImpl>
	  {
		  public bool isFulfilled(PvmExecutionImpl element)
		  {
			return element == null || CompensationBehavior.isCompensationThrowing(element);
		  }
	  }

	}

}