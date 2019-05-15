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
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ConditionalEventFactory : BpmnEventFactory
	{

	  protected internal const string VAR_CONDITION = "${any=='any'}";

	  public virtual MigratingBpmnEventTrigger addBoundaryEvent(ProcessEngine engine, BpmnModelInstance modelInstance, string activityId, string boundaryEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).activityBuilder(activityId).boundaryEvent(boundaryEventId).condition(VAR_CONDITION).done();

		ConditionalEventTrigger trigger = new ConditionalEventTrigger();
		trigger.engine = engine;
		trigger.variableName = "any";
		trigger.variableValue = "any";
		trigger.activityId = boundaryEventId;

		return trigger;
	  }

	  public virtual MigratingBpmnEventTrigger addEventSubProcess(ProcessEngine engine, BpmnModelInstance modelInstance, string parentId, string subProcessId, string startEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).addSubProcessTo(parentId).id(subProcessId).triggerByEvent().embeddedSubProcess().startEvent(startEventId).condition(VAR_CONDITION).subProcessDone().done();

		ConditionalEventTrigger trigger = new ConditionalEventTrigger();
		trigger.engine = engine;
		trigger.variableName = "any";
		trigger.variableValue = "any";
		trigger.activityId = startEventId;

		return trigger;
	  }

	  protected internal class ConditionalEventTrigger : MigratingBpmnEventTrigger
	  {

		protected internal ProcessEngine engine;
		protected internal string variableName;
		protected internal object variableValue;
		protected internal string activityId;

		public virtual void trigger(string processInstanceId)
		{
		  engine.RuntimeService.setVariable(processInstanceId, variableName, variableValue);
		}

		public virtual void assertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
		{
		  migrationContext.assertEventSubscriptionMigrated(activityId, targetActivityId, null);
		}

		public virtual MigratingBpmnEventTrigger inContextOf(string newActivityId)
		{
		  ConditionalEventTrigger newTrigger = new ConditionalEventTrigger();
		  newTrigger.activityId = newActivityId;
		  newTrigger.engine = engine;
		  newTrigger.variableName = variableName;
		  newTrigger.variableValue = variableValue;
		  return newTrigger;
		}

	  }

	}

}