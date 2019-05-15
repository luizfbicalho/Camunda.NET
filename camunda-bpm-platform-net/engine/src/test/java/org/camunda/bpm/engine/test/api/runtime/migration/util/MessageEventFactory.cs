﻿/*
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
	using ConditionalEventTrigger = org.camunda.bpm.engine.test.api.runtime.migration.util.ConditionalEventFactory.ConditionalEventTrigger;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MessageEventFactory : BpmnEventFactory
	{

	  public const string MESSAGE_NAME = "message";

	  public virtual MigratingBpmnEventTrigger addBoundaryEvent(ProcessEngine engine, BpmnModelInstance modelInstance, string activityId, string boundaryEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).activityBuilder(activityId).boundaryEvent(boundaryEventId).message(MESSAGE_NAME).done();

		MessageTrigger trigger = new MessageTrigger();
		trigger.engine = engine;
		trigger.messageName = MESSAGE_NAME;
		trigger.activityId = boundaryEventId;

		return trigger;
	  }

	  public virtual MigratingBpmnEventTrigger addEventSubProcess(ProcessEngine engine, BpmnModelInstance modelInstance, string parentId, string subProcessId, string startEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).addSubProcessTo(parentId).id(subProcessId).triggerByEvent().embeddedSubProcess().startEvent(startEventId).message(MESSAGE_NAME).subProcessDone().done();

		MessageTrigger trigger = new MessageTrigger();
		trigger.engine = engine;
		trigger.messageName = MESSAGE_NAME;
		trigger.activityId = startEventId;

		return trigger;
	  }


	  protected internal class MessageTrigger : MigratingBpmnEventTrigger
	  {

		protected internal ProcessEngine engine;
		protected internal string activityId;
		protected internal string messageName;

		public virtual void trigger(string processInstanceId)
		{
		  engine.RuntimeService.createMessageCorrelation(messageName).processInstanceId(processInstanceId).correlateWithResult();
		}

		public virtual void assertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
		{
		  migrationContext.assertEventSubscriptionMigrated(activityId, targetActivityId, messageName);
		}

		public virtual MigratingBpmnEventTrigger inContextOf(string newActivityId)
		{
		  MessageTrigger newTrigger = new MessageTrigger();
		  newTrigger.activityId = newActivityId;
		  newTrigger.engine = engine;
		  newTrigger.messageName = messageName;
		  return newTrigger;
		}

	  }

	}

}