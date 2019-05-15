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

	using AssociationDirection = org.camunda.bpm.model.bpmn.AssociationDirection;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Association = org.camunda.bpm.model.bpmn.instance.Association;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BoundaryEvent = org.camunda.bpm.model.bpmn.instance.BoundaryEvent;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompensationModels
	{

	  public static readonly BpmnModelInstance ONE_COMPENSATION_TASK_MODEL = ProcessModels.newModel().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").userTask("userTask2").intermediateThrowEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().done();
	  static CompensationModels()
	  {
		addUserTaskCompensationHandler(ONE_COMPENSATION_TASK_MODEL, "compensationBoundary", "compensationHandler");
		addUserTaskCompensationHandler(COMPENSATION_ONE_TASK_SUBPROCESS_MODEL, "compensationBoundary", "compensationHandler");
		addUserTaskCompensationHandler(COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL, "compensationBoundary", "compensationHandler");
		CompensationModels.addUserTaskCompensationHandler(DOUBLE_SUBPROCESS_MODEL, "compensationBoundary", "compensationHandler");
		addUserTaskCompensationHandler(COMPENSATION_END_EVENT_MODEL, "compensationBoundary", "compensationHandler");
		addUserTaskCompensationHandler(TRANSACTION_COMPENSATION_MODEL, "compensationBoundary", "compensationHandler");
	  }

	public static readonly BpmnModelInstance COMPENSATION_ONE_TASK_SUBPROCESS_MODEL = ProcessModels.newModel().startEvent().subProcess("subProcess").embeddedSubProcess().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").endEvent().subProcessDone().userTask("userTask2").intermediateThrowEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().done();

	  public static readonly BpmnModelInstance COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL = ProcessModels.newModel().startEvent().subProcess("subProcess").embeddedSubProcess().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").userTask("userTask2").endEvent("subProcessEnd").subProcessDone().intermediateThrowEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().done();


	  public static readonly BpmnModelInstance DOUBLE_SUBPROCESS_MODEL = ProcessModels.newModel().startEvent().subProcess("outerSubProcess").embeddedSubProcess().startEvent().subProcess("innerSubProcess").embeddedSubProcess().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").endEvent().subProcessDone().endEvent().subProcessDone().userTask("userTask2").intermediateThrowEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().done();

	  public static readonly BpmnModelInstance COMPENSATION_END_EVENT_MODEL = ProcessModels.newModel().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").userTask("userTask2").endEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).done();


	  public static readonly BpmnModelInstance TRANSACTION_COMPENSATION_MODEL = modify(TransactionModels.CANCEL_BOUNDARY_EVENT).activityBuilder("userTask").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().done();

	  public static readonly BpmnModelInstance COMPENSATION_EVENT_SUBPROCESS_MODEL = modify(COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).addSubProcessTo("subProcess").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").compensateEventDefinition().compensateEventDefinitionDone().userTask("eventSubProcessTask").intermediateThrowEvent("eventSubProcessCompensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().endEvent().done();

	  public static void addUserTaskCompensationHandler(BpmnModelInstance modelInstance, string boundaryEventId, string compensationHandlerId)
	  {

		BoundaryEvent boundaryEvent = modelInstance.getModelElementById(boundaryEventId);
		BaseElement scope = (BaseElement) boundaryEvent.ParentElement;

		UserTask compensationHandler = modelInstance.newInstance(typeof(UserTask));
		compensationHandler.Id = compensationHandlerId;
		compensationHandler.ForCompensation = true;
		scope.addChildElement(compensationHandler);

		Association association = modelInstance.newInstance(typeof(Association));
		association.AssociationDirection = AssociationDirection.One;
		association.Source = boundaryEvent;
		association.Target = compensationHandler;
		scope.addChildElement(association);

	  }


	}

}