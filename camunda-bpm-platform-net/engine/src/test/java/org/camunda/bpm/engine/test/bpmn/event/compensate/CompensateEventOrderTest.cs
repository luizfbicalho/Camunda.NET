using System;
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
namespace org.camunda.bpm.engine.test.bpmn.@event.compensate
{
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using IncreaseCurrentTimeServiceTask = org.camunda.bpm.engine.test.bpmn.@event.compensate.helper.IncreaseCurrentTimeServiceTask;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using AssociationDirection = org.camunda.bpm.model.bpmn.AssociationDirection;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Association = org.camunda.bpm.model.bpmn.instance.Association;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BoundaryEvent = org.camunda.bpm.model.bpmn.instance.BoundaryEvent;
	using ServiceTask = org.camunda.bpm.model.bpmn.instance.ServiceTask;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class CompensateEventOrderTest
	{
		private bool InstanceFieldsInitialized = false;

		public CompensateEventOrderTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
		public ProcessEngineRule engineRule = new ProcessEngineRule(true);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testHelper = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTwoCompensateEventsInReverseOrder()
	  public virtual void testTwoCompensateEventsInReverseOrder()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().serviceTask("serviceTask1").camundaClass(typeof(IncreaseCurrentTimeServiceTask).FullName).boundaryEvent("compensationBoundary1").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("serviceTask1").serviceTask("serviceTask2").camundaClass(typeof(IncreaseCurrentTimeServiceTask).FullName).boundaryEvent("compensationBoundary2").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("serviceTask2").intermediateThrowEvent("compensationEvent").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().endEvent().done();

		addServiceTaskCompensationHandler(model, "compensationBoundary1", "A");
		addServiceTaskCompensationHandler(model, "compensationBoundary2", "B");

		testHelper.deploy(model);

		//when
		engineRule.RuntimeService.startProcessInstanceByKey("Process_1", Variables.createVariables().putValue("currentTime", DateTime.Now));

		//then compensation activities are executed in the reverse order
		IList<HistoricActivityInstance> list = engineRule.HistoryService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().asc().list();

		long indexA = searchForActivityIndex(list, "A");
		long indexB = searchForActivityIndex(list, "B");

		assertNotEquals(-1, indexA);
		assertNotEquals(-1, indexB);

		assertTrue("Compensation activities were executed in wrong order.", indexA > indexB);

	  }

	  private long searchForActivityIndex(IList<HistoricActivityInstance> historicActivityInstances, string activityId)
	  {
		for (int i = 0; i < historicActivityInstances.Count; i++)
		{
		  HistoricActivityInstance historicActivityInstance = historicActivityInstances[i];
		  if (historicActivityInstance.ActivityId.Equals(activityId))
		  {
			return i;
		  }
		}
		return -1;
	  }

	  private void addServiceTaskCompensationHandler(BpmnModelInstance modelInstance, string boundaryEventId, string compensationHandlerId)
	  {

		BoundaryEvent boundaryEvent = modelInstance.getModelElementById(boundaryEventId);
		BaseElement scope = (BaseElement) boundaryEvent.ParentElement;

		ServiceTask compensationHandler = modelInstance.newInstance(typeof(ServiceTask));
		compensationHandler.Id = compensationHandlerId;
		compensationHandler.ForCompensation = true;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		compensationHandler.CamundaClass = typeof(IncreaseCurrentTimeServiceTask).FullName;
		scope.addChildElement(compensationHandler);

		Association association = modelInstance.newInstance(typeof(Association));
		association.AssociationDirection = AssociationDirection.One;
		association.Source = boundaryEvent;
		association.Target = compensationHandler;
		scope.addChildElement(association);

	  }


	}

}