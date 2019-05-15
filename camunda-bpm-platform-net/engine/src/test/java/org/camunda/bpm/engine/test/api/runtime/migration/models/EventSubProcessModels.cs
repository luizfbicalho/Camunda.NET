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
namespace org.camunda.bpm.engine.test.api.runtime.migration.models
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EventSubProcessModels
	{

	  public const string MESSAGE_NAME = "Message";
	  public const string SIGNAL_NAME = "Signal";
	  public const string VAR_CONDITION = "${any=='any'}";
	  public const string FALSE_CONDITION = "${false}";

	  public const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
	  public const string EVENT_SUB_PROCESS_ID = "eventSubProcess";
	  public const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
	  public const string SUB_PROCESS_ID = "subProcess";
	  public const string USER_TASK_ID = "userTask";

	  public static readonly BpmnModelInstance CONDITIONAL_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(VAR_CONDITION).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();


	  public static readonly BpmnModelInstance FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).condition(FALSE_CONDITION).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();


	  public static readonly BpmnModelInstance MESSAGE_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).message(MESSAGE_NAME).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS = ProcessModels.newModel().startEvent().subProcess(EVENT_SUB_PROCESS_ID).embeddedSubProcess().startEvent().intermediateCatchEvent("catchMessage").message(MESSAGE_NAME).userTask("userTask").endEvent().subProcessDone().endEvent().done();

	  public static readonly BpmnModelInstance TIMER_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).timerWithDuration("PT10M").userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance SIGNAL_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).signal(SIGNAL_NAME).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance ESCALATION_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).escalation().userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance ERROR_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.ONE_TASK_PROCESS).addSubProcessTo(ProcessModels.PROCESS_KEY).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).error().userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance COMPENSATE_EVENT_SUBPROCESS_PROCESS = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo(SUB_PROCESS_ID).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).compensation().userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();

	  public static readonly BpmnModelInstance NESTED_EVENT_SUB_PROCESS_PROCESS = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo(SUB_PROCESS_ID).id(EVENT_SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent(EVENT_SUB_PROCESS_START_ID).message(MESSAGE_NAME).userTask(EVENT_SUB_PROCESS_TASK_ID).endEvent().subProcessDone().done();
	}

}