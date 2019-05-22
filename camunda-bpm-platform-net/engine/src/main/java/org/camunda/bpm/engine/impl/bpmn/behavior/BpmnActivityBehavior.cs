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
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using CompensationBehavior = org.camunda.bpm.engine.impl.pvm.runtime.CompensationBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Helper class for implementing BPMN 2.0 activities, offering convenience
	/// methods specific to BPMN 2.0.
	/// 
	/// This class can be used by inheritance or aggregation.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class BpmnActivityBehavior
	{

	  protected internal static BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  /// <summary>
	  /// Performs the default outgoing BPMN 2.0 behavior, which is having parallel
	  /// paths of executions for the outgoing sequence flow.
	  /// 
	  /// More precisely: every sequence flow that has a condition which evaluates to
	  /// true (or which doesn't have a condition), is selected for continuation of
	  /// the process instance. If multiple sequencer flow are selected, multiple,
	  /// parallel paths of executions are created.
	  /// </summary>
	  public virtual void performDefaultOutgoingBehavior(ActivityExecution activityExceution)
	  {
		performOutgoingBehavior(activityExceution, true, null);
	  }

	  /// <summary>
	  /// Performs the default outgoing BPMN 2.0 behavior (@see
	  /// <seealso cref="#performDefaultOutgoingBehavior(ActivityExecution)"/>), but without
	  /// checking the conditions on the outgoing sequence flow.
	  /// 
	  /// This means that every outgoing sequence flow is selected for continuing the
	  /// process instance, regardless of having a condition or not. In case of
	  /// multiple outgoing sequence flow, multiple parallel paths of executions will
	  /// be created.
	  /// </summary>
	  public virtual void performIgnoreConditionsOutgoingBehavior(ActivityExecution activityExecution)
	  {
		performOutgoingBehavior(activityExecution, false, null);
	  }

	  /// <summary>
	  /// Actual implementation of leaving an activity.
	  /// </summary>
	  /// <param name="execution">
	  ///          The current execution context </param>
	  /// <param name="checkConditions">
	  ///          Whether or not to check conditions before determining whether or
	  ///          not to take a transition. </param>
	  protected internal virtual void performOutgoingBehavior(ActivityExecution execution, bool checkConditions, IList<ActivityExecution> reusableExecutions)
	  {

		LOG.leavingActivity(execution.Activity.Id);

		string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		IList<PvmTransition> transitionsToTake = new List<PvmTransition>();

		IList<PvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;
		foreach (PvmTransition outgoingTransition in outgoingTransitions)
		{
		  if (string.ReferenceEquals(defaultSequenceFlow, null) || !outgoingTransition.Id.Equals(defaultSequenceFlow))
		  {
			Condition condition = (Condition) outgoingTransition.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
			if (condition == null || !checkConditions || condition.evaluate(execution))
			{
			  transitionsToTake.Add(outgoingTransition);
			}
		  }
		}

		if (transitionsToTake.Count == 1)
		{

		  execution.leaveActivityViaTransition(transitionsToTake[0]);

		}
		else if (transitionsToTake.Count >= 1)
		{

		  if (reusableExecutions == null || reusableExecutions.Count == 0)
		  {
			execution.leaveActivityViaTransitions(transitionsToTake, Arrays.asList(execution));
		  }
		  else
		  {
			execution.leaveActivityViaTransitions(transitionsToTake, reusableExecutions);
		  }

		}
		else
		{

		  if (!string.ReferenceEquals(defaultSequenceFlow, null))
		  {
			PvmTransition defaultTransition = execution.Activity.findOutgoingTransition(defaultSequenceFlow);
			if (defaultTransition != null)
			{
			  execution.leaveActivityViaTransition(defaultTransition);
			}
			else
			{
			  throw LOG.missingDefaultFlowException(execution.Activity.Id, defaultSequenceFlow);
			}

		  }
		  else if (outgoingTransitions.Count > 0)
		  {
			throw LOG.missingConditionalFlowException(execution.Activity.Id);

		  }
		  else
		  {

			if (((ActivityImpl) execution.Activity).CompensationHandler && isAncestorCompensationThrowing(execution))
			{

			 execution.endCompensation();

			}
			else
			{
			  LOG.missingOutgoingSequenceFlow(execution.Activity.Id);
			  execution.end(true);
			}
		  }
		}
	  }

	  protected internal virtual bool isAncestorCompensationThrowing(ActivityExecution execution)
	  {
		ActivityExecution parent = execution.Parent;
		while (parent != null)
		{
		  if (CompensationBehavior.isCompensationThrowing((PvmExecutionImpl) parent))
		  {
			return true;
		  }
		  parent = parent.Parent;
		}
		return false;
	  }

	}

}