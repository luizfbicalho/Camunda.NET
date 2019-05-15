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
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class CancelEndEventActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal PvmActivity cancelBoundaryEvent;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		EnsureUtil.ensureNotNull("Could not find cancel boundary event for cancel end event " + execution.Activity, "cancelBoundaryEvent", cancelBoundaryEvent);

		IList<EventSubscriptionEntity> compensateEventSubscriptions = CompensationUtil.collectCompensateEventSubscriptionsForScope(execution);

		if (compensateEventSubscriptions.Count == 0)
		{
		  leave(execution);
		}
		else
		{
		  CompensationUtil.throwCompensationEvent(compensateEventSubscriptions, execution, false);
		}

	  }

	  public override void doLeave(ActivityExecution execution)
	  {
		// continue via the appropriate cancel boundary event
		ScopeImpl eventScope = (ScopeImpl) cancelBoundaryEvent.EventScope;

		ActivityExecution boundaryEventScopeExecution = execution.findExecutionForFlowScope(eventScope);
		boundaryEventScopeExecution.executeActivity(cancelBoundaryEvent);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {

		// join compensating executions
		if (!execution.hasChildren())
		{
		  leave(execution);
		}
		else
		{
		  ((ExecutionEntity)execution).forceUpdate();
		}
	  }

	  public virtual PvmActivity CancelBoundaryEvent
	  {
		  set
		  {
			this.cancelBoundaryEvent = value;
		  }
		  get
		  {
			return cancelBoundaryEvent;
		  }
	  }


	}

}