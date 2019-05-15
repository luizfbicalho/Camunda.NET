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


	/// <summary>
	/// implementation of the Exclusive Gateway/XOR gateway/exclusive data-based gateway
	/// as defined in the BPMN specification.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class ExclusiveGatewayActivityBehavior : GatewayActivityBehavior
	{

	  protected internal static new BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  /// <summary>
	  /// The default behaviour of BPMN, taking every outgoing sequence flow
	  /// (where the condition evaluates to true), is not valid for an exclusive
	  /// gateway.
	  /// 
	  /// Hence, this behaviour is overriden and replaced by the correct behavior:
	  /// selecting the first sequence flow which condition evaluates to true
	  /// (or which hasn't got a condition) and leaving the activity through that
	  /// sequence flow.
	  /// 
	  /// If no sequence flow is selected (ie all conditions evaluate to false),
	  /// then the default sequence flow is taken (if defined).
	  /// </summary>
	  public override void doLeave(ActivityExecution execution)
	  {

		LOG.leavingActivity(execution.Activity.Id);

		PvmTransition outgoingSeqFlow = null;
		string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		IEnumerator<PvmTransition> transitionIterator = execution.Activity.OutgoingTransitions.GetEnumerator();
		while (outgoingSeqFlow == null && transitionIterator.MoveNext())
		{
		  PvmTransition seqFlow = transitionIterator.Current;

		  Condition condition = (Condition) seqFlow.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
		  if ((condition == null && (string.ReferenceEquals(defaultSequenceFlow, null) || !defaultSequenceFlow.Equals(seqFlow.Id))) || (condition != null && condition.evaluate(execution)))
		  {

			LOG.outgoingSequenceFlowSelected(seqFlow.Id);
			outgoingSeqFlow = seqFlow;
		  }
		}

		if (outgoingSeqFlow != null)
		{
		  execution.leaveActivityViaTransition(outgoingSeqFlow);
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
		  else
		  {
			//No sequence flow could be found, not even a default one
			throw LOG.stuckExecutionException(execution.Activity.Id);
		  }
		}
	  }

	}

}