using System;

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
namespace org.camunda.bpm.engine.test.api.runtime.migration.util
{
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using MessageTrigger = org.camunda.bpm.engine.test.api.runtime.migration.util.MessageEventFactory.MessageTrigger;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SignalEventFactory : BpmnEventFactory
	{

	  public const string SIGNAL_NAME = "signal";

	  public virtual MigratingBpmnEventTrigger addBoundaryEvent(ProcessEngine engine, BpmnModelInstance modelInstance, string activityId, string boundaryEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).activityBuilder(activityId).boundaryEvent(boundaryEventId).signal(SIGNAL_NAME).done();

		SignalTrigger trigger = new SignalTrigger();
		trigger.engine = engine;
		trigger.signalName = SIGNAL_NAME;
		trigger.activityId = boundaryEventId;

		return trigger;
	  }

	  public virtual MigratingBpmnEventTrigger addEventSubProcess(ProcessEngine engine, BpmnModelInstance modelInstance, string parentId, string subProcessId, string startEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).addSubProcessTo(parentId).id(subProcessId).triggerByEvent().embeddedSubProcess().startEvent(startEventId).signal(SIGNAL_NAME).subProcessDone().done();

		SignalTrigger trigger = new SignalTrigger();
		trigger.engine = engine;
		trigger.signalName = SIGNAL_NAME;
		trigger.activityId = startEventId;

		return trigger;
	  }

	  protected internal class SignalTrigger : MigratingBpmnEventTrigger
	  {

		protected internal ProcessEngine engine;
		protected internal string signalName;
		protected internal string activityId;

		public virtual void trigger(string processInstanceId)
		{
		  EventSubscription eventSubscription = engine.RuntimeService.createEventSubscriptionQuery().activityId(activityId).eventName(signalName).processInstanceId(processInstanceId).singleResult();

		  if (eventSubscription == null)
		  {
			throw new Exception("Event subscription not found");
		  }

		  engine.RuntimeService.signalEventReceived(eventSubscription.EventName, eventSubscription.ExecutionId);
		}

		public virtual void assertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
		{
		  migrationContext.assertEventSubscriptionMigrated(activityId, targetActivityId, SIGNAL_NAME);
		}

		public virtual MigratingBpmnEventTrigger inContextOf(string newActivityId)
		{
		  SignalTrigger newTrigger = new SignalTrigger();
		  newTrigger.activityId = newActivityId;
		  newTrigger.engine = engine;
		  newTrigger.signalName = signalName;
		  return newTrigger;
		}

	  }

	}

}