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

	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;
	using CompensateEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.CompensateEventDefinition;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Behavior for a compensation end event.
	/// </summary>
	/// <seealso cref= IntermediateThrowCompensationEventActivityBehavior
	/// 
	/// @author Philipp Ossler
	///  </seealso>
	public class CompensationEventActivityBehavior : FlowNodeActivityBehavior
	{

	  protected internal readonly CompensateEventDefinition compensateEventDefinition;

	  public CompensationEventActivityBehavior(CompensateEventDefinition compensateEventDefinition)
	  {
		this.compensateEventDefinition = compensateEventDefinition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity> eventSubscriptions = collectEventSubscriptions(execution);
		IList<EventSubscriptionEntity> eventSubscriptions = collectEventSubscriptions(execution);
		if (eventSubscriptions.Count == 0)
		{
		  leave(execution);
		}
		else
		{
		  // async (waitForCompletion=false in bpmn) is not supported
		  CompensationUtil.throwCompensationEvent(eventSubscriptions, execution, false);
		}
	  }

	  protected internal virtual IList<EventSubscriptionEntity> collectEventSubscriptions(ActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String activityRef = compensateEventDefinition.getActivityRef();
		string activityRef = compensateEventDefinition.ActivityRef;
		if (!string.ReferenceEquals(activityRef, null))
		{
		  return CompensationUtil.collectCompensateEventSubscriptionsForActivity(execution, activityRef);
		}
		else
		{
		  return CompensationUtil.collectCompensateEventSubscriptionsForScope(execution);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		// join compensating executions -
		// only wait for non-event-scope executions cause a compensation event subprocess consume the compensation event and
		// do not have to compensate embedded subprocesses (which are still non-event-scope executions)

		if (((PvmExecutionImpl) execution).NonEventScopeExecutions.Count == 0)
		{
		  leave(execution);
		}
		else
		{
		  ((ExecutionEntity) execution).forceUpdate();
		}
	  }

	}
}