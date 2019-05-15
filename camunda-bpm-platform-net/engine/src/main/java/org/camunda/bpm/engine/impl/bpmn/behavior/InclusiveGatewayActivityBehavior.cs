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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// Implementation of the Inclusive Gateway/OR gateway/inclusive data-based
	/// gateway as defined in the BPMN specification.
	/// 
	/// @author Tijs Rademakers
	/// @author Tom Van Buskirk
	/// @author Joram Barrez
	/// </summary>
	public class InclusiveGatewayActivityBehavior : GatewayActivityBehavior
	{

	  protected internal static new BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		execution.inactivate();
		lockConcurrentRoot(execution);

		PvmActivity activity = execution.Activity;
		if (activatesGateway(execution, activity))
		{

		  LOG.activityActivation(activity.Id);

		  IList<ActivityExecution> joinedExecutions = execution.findInactiveConcurrentExecutions(activity);
		  string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		  IList<PvmTransition> transitionsToTake = new List<PvmTransition>();

		  // find matching non-default sequence flows
		  foreach (PvmTransition outgoingTransition in execution.Activity.OutgoingTransitions)
		  {
			if (string.ReferenceEquals(defaultSequenceFlow, null) || !outgoingTransition.Id.Equals(defaultSequenceFlow))
			{
			  Condition condition = (Condition) outgoingTransition.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
			  if (condition == null || condition.evaluate(execution))
			  {
				transitionsToTake.Add(outgoingTransition);
			  }
			}
		  }

		  // if none found, add default flow
		  if (transitionsToTake.Count == 0)
		  {
			if (!string.ReferenceEquals(defaultSequenceFlow, null))
			{
			  PvmTransition defaultTransition = execution.Activity.findOutgoingTransition(defaultSequenceFlow);
			  if (defaultTransition == null)
			  {
				throw LOG.missingDefaultFlowException(execution.Activity.Id, defaultSequenceFlow);
			  }

			  transitionsToTake.Add(defaultTransition);

			}
			else
			{
			  // No sequence flow could be found, not even a default one
			  throw LOG.stuckExecutionException(execution.Activity.Id);
			}
		  }

		  // take the flows found
		  execution.leaveActivityViaTransitions(transitionsToTake, joinedExecutions);
		}
		else
		{
		  LOG.noActivityActivation(activity.Id);
		}
	  }

	  protected internal virtual ICollection<ActivityExecution> getLeafExecutions(ActivityExecution parent)
	  {
		IList<ActivityExecution> executionlist = new List<ActivityExecution>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution> subExecutions = parent.getNonEventScopeExecutions();
		IList<ActivityExecution> subExecutions = parent.NonEventScopeExecutions;
		if (subExecutions.Count == 0)
		{
		  executionlist.Add(parent);
		}
		else
		{
		  foreach (ActivityExecution concurrentExecution in subExecutions)
		  {
			((IList<ActivityExecution>)executionlist).AddRange(getLeafExecutions(concurrentExecution));
		  }
		}

		return executionlist;
	  }

	  protected internal virtual bool activatesGateway(ActivityExecution execution, PvmActivity gatewayActivity)
	  {
		int numExecutionsGuaranteedToActivate = gatewayActivity.IncomingTransitions.Count;
		ActivityExecution scopeExecution = execution.Scope ? execution : execution.Parent;

		IList<ActivityExecution> executionsAtGateway = execution.findInactiveConcurrentExecutions(gatewayActivity);

		if (executionsAtGateway.Count >= numExecutionsGuaranteedToActivate)
		{
		  return true;
		}
		else
		{
		  ICollection<ActivityExecution> executionsNotAtGateway = getLeafExecutions(scopeExecution);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		  executionsNotAtGateway.removeAll(executionsAtGateway);

		  foreach (ActivityExecution executionNotAtGateway in executionsNotAtGateway)
		  {
			if (canReachActivity(executionNotAtGateway, gatewayActivity))
			{
			  return false;
			}
		  }

		  // if no more token may arrive, then activate
		  return true;
		}

	  }

	  protected internal virtual bool canReachActivity(ActivityExecution execution, PvmActivity activity)
	  {
		PvmTransition pvmTransition = execution.Transition;
		if (pvmTransition != null)
		{
		  return isReachable(pvmTransition.Destination, activity, new HashSet<PvmActivity>());
		}
		else
		{
		  return isReachable(execution.Activity, activity, new HashSet<PvmActivity>());
		}
	  }

	  protected internal virtual bool isReachable(PvmActivity srcActivity, PvmActivity targetActivity, ISet<PvmActivity> visitedActivities)
	  {
		if (srcActivity.Equals(targetActivity))
		{
		  return true;
		}

		if (visitedActivities.Contains(srcActivity))
		{
		  return false;
		}

		// To avoid infinite looping, we must capture every node we visit and
		// check before going further in the graph if we have already visited the node.
		visitedActivities.Add(srcActivity);

		IList<PvmTransition> outgoingTransitions = srcActivity.OutgoingTransitions;

		if (outgoingTransitions.Count == 0)
		{

		  if (srcActivity.ActivityBehavior is EventBasedGatewayActivityBehavior)
		  {

			ActivityImpl eventBasedGateway = (ActivityImpl) srcActivity;
			ISet<ActivityImpl> eventActivities = eventBasedGateway.EventActivities;

			foreach (ActivityImpl eventActivity in eventActivities)
			{
			  bool isReachable = isReachable(eventActivity, targetActivity, visitedActivities);

			  if (isReachable)
			  {
				return true;
			  }
			}

		  }
		  else
		  {

			ScopeImpl flowScope = srcActivity.FlowScope;
			if (flowScope != null && flowScope is PvmActivity)
			{
			  return isReachable((PvmActivity) flowScope, targetActivity, visitedActivities);
			}

		  }

		  return false;
		}
		else
		{
		  foreach (PvmTransition pvmTransition in outgoingTransitions)
		  {
			PvmActivity destinationActivity = pvmTransition.Destination;
			if (destinationActivity != null && !visitedActivities.Contains(destinationActivity))
			{

			  bool reachable = isReachable(destinationActivity, targetActivity, visitedActivities);

			  // If false, we should investigate other paths, and not yet return the result
			  if (reachable)
			  {
				return true;
			  }

			}
		  }
		}

		return false;
	  }

	}

}