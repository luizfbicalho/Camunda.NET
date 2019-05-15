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
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using SignalTrigger = org.camunda.bpm.engine.test.api.runtime.migration.util.SignalEventFactory.SignalTrigger;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TimerEventFactory : BpmnEventFactory
	{

	  public const string TIMER_DATE = "2016-02-11T12:13:14Z";

	  public virtual MigratingBpmnEventTrigger addBoundaryEvent(ProcessEngine engine, BpmnModelInstance modelInstance, string activityId, string boundaryEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).activityBuilder(activityId).boundaryEvent(boundaryEventId).timerWithDate(TIMER_DATE).done();

		TimerEventTrigger trigger = new TimerEventTrigger();
		trigger.engine = engine;
		trigger.activityId = boundaryEventId;
		trigger.handlerType = TimerExecuteNestedActivityJobHandler.TYPE;

		return trigger;
	  }

	  public virtual MigratingBpmnEventTrigger addEventSubProcess(ProcessEngine engine, BpmnModelInstance modelInstance, string parentId, string subProcessId, string startEventId)
	  {
		ModifiableBpmnModelInstance.wrap(modelInstance).addSubProcessTo(parentId).id(subProcessId).triggerByEvent().embeddedSubProcess().startEvent(startEventId).timerWithDuration("PT10M").subProcessDone().done();

		TimerEventTrigger trigger = new TimerEventTrigger();
		trigger.engine = engine;
		trigger.activityId = startEventId;
		trigger.handlerType = TimerStartEventSubprocessJobHandler.TYPE;

		return trigger;
	  }


	  protected internal class TimerEventTrigger : MigratingBpmnEventTrigger
	  {

		protected internal ProcessEngine engine;
		protected internal string activityId;
		protected internal string handlerType;

		public virtual void trigger(string processInstanceId)
		{
		  ManagementService managementService = engine.ManagementService;
		  Job timerJob = managementService.createJobQuery().processInstanceId(processInstanceId).activityId(activityId).singleResult();

		  if (timerJob == null)
		  {
			throw new ProcessEngineException("No job for this event found in context of process instance " + processInstanceId);
		  }

		  managementService.executeJob(timerJob.Id);
		}

		public virtual void assertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
		{
		  migrationContext.assertJobMigrated(activityId, targetActivityId, handlerType);
		}

		public virtual MigratingBpmnEventTrigger inContextOf(string newActivityId)
		{
		  TimerEventTrigger newTrigger = new TimerEventTrigger();
		  newTrigger.activityId = newActivityId;
		  newTrigger.engine = engine;
		  newTrigger.handlerType = handlerType;
		  return newTrigger;
		}

	  }

	}

}