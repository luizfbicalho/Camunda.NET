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
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class CompensateEventHistoryTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventHistoryTest.testBoundaryCompensationHandlerHistory.bpmn20.xml")]
		public virtual void testBoundaryCompensationHandlerHistoryActivityInstance()
		{
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("boundaryHandlerProcess");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		string compensationHandlerActivityInstanceId = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("compensationHandler")[0].Id;

		// .. and completing compensation
		Task compensationHandler = taskService.createTaskQuery().singleResult();
		taskService.complete(compensationHandler.Id);

		// then there is a historic activity instance for the compensation handler
		HistoricActivityInstance historicCompensationHandlerInstance = historyService.createHistoricActivityInstanceQuery().activityId("compensationHandler").singleResult();

		assertNotNull(historicCompensationHandlerInstance);
		assertEquals(compensationHandlerActivityInstanceId, historicCompensationHandlerInstance.Id);
		assertEquals(processInstance.Id, historicCompensationHandlerInstance.ParentActivityInstanceId);
		}

	  /// <summary>
	  /// Fix CAM-4351
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventHistoryTest.testBoundaryCompensationHandlerHistory.bpmn20.xml")]
	  public virtual void FAILING_testBoundaryCompensationHandlerHistoryVariableInstance()
	  {
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("boundaryHandlerProcess");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		string compensationHandlerActivityInstanceId = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("compensationHandler")[0].Id;

		// .. setting a variable via task service API
		Task compensationHandler = taskService.createTaskQuery().singleResult();
		runtimeService.setVariableLocal(compensationHandler.ExecutionId, "apiVariable", "someValue");

		// .. and completing compensation
		taskService.complete(compensationHandler.Id);

		// then there is a historic variable instance for the variable set by API
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		assertNotNull(historicVariableInstance);
		assertEquals(compensationHandlerActivityInstanceId, historicVariableInstance.ActivityInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventHistoryTest.testDefaultCompensationHandlerHistory.bpmn20.xml")]
	  public virtual void testDefaultCompensationHandlerHistoryActivityInstance()
	  {
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("defaultHandlerProcess");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		string compensationHandlerActivityInstanceId = tree.getActivityInstances("compensationHandler")[0].Id;

		string subProcessActivityInstanceId = tree.getActivityInstances("subProcess")[0].Id;

		// .. and completing compensation
		Task compensationHandler = taskService.createTaskQuery().singleResult();
		taskService.complete(compensationHandler.Id);

		// then there is a historic activity instance for the compensation handler
		HistoricActivityInstance historicCompensationHandlerInstance = historyService.createHistoricActivityInstanceQuery().activityId("compensationHandler").singleResult();

		assertNotNull(historicCompensationHandlerInstance);
		assertEquals(compensationHandlerActivityInstanceId, historicCompensationHandlerInstance.Id);
		assertEquals(subProcessActivityInstanceId, historicCompensationHandlerInstance.ParentActivityInstanceId);
	  }

	  /// <summary>
	  /// Fix CAM-4351
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventHistoryTest.testDefaultCompensationHandlerHistory.bpmn20.xml")]
	  public virtual void FAILING_testDefaultCompensationHandlerHistoryVariableInstance()
	  {
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("defaultHandlerProcess");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		string compensationHandlerActivityInstanceId = tree.getActivityInstances("compensationHandler")[0].Id;

		// .. setting a variable via task service API
		Task compensationHandler = taskService.createTaskQuery().singleResult();
		runtimeService.setVariableLocal(compensationHandler.ExecutionId, "apiVariable", "someValue");

		// .. and completing compensation
		taskService.complete(compensationHandler.Id);

		// then there is a historic variable instance for the variable set by API
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		assertNotNull(historicVariableInstance);
		assertEquals(compensationHandlerActivityInstanceId, historicVariableInstance.ActivityInstanceId);
	  }


	}

}