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
namespace org.camunda.bpm.engine.test.history
{

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricVariableInstanceScopeTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
		public virtual void testSetVariableOnProcessInstanceStart()
		{
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["testVar"] = "testValue";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		assertNotNull(variable);

		// the variable is in the process instance scope
		assertEquals(pi.Id, variable.ActivityInstanceId);

		taskService.complete(taskService.createTaskQuery().singleResult().Id);
		assertProcessEnded(pi.Id);
		}

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetVariableLocalOnUserTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.setVariableLocal(task.Id, "testVar", "testValue");
		ExecutionEntity taskExecution = (ExecutionEntity) runtimeService.createExecutionQuery().executionId(task.ExecutionId).singleResult();
		assertNotNull(taskExecution);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		assertNotNull(variable);

		// the variable is in the task scope
		assertEquals(taskExecution.ActivityInstanceId, variable.ActivityInstanceId);

		taskService.complete(task.Id);
		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetVariableOnProcessIntanceStartAndSetVariableLocalOnUserTask()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["testVar"] = "testValue";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.setVariableLocal(task.Id, "testVar", "anotherTestValue");
		ExecutionEntity taskExecution = (ExecutionEntity) runtimeService.createExecutionQuery().singleResult();
		assertNotNull(taskExecution);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(2, query.count());

		IList<HistoricVariableInstance> result = query.list();

		HistoricVariableInstance firstVar = result[0];
		assertEquals("testVar", firstVar.VariableName);
		assertEquals("testValue", firstVar.Value);
		// the variable is in the process instance scope
		assertEquals(pi.Id, firstVar.ActivityInstanceId);

		HistoricVariableInstance secondVar = result[1];
		assertEquals("testVar", secondVar.VariableName);
		assertEquals("anotherTestValue", secondVar.Value);
		// the variable is in the task scope
		assertEquals(taskExecution.ActivityInstanceId, secondVar.ActivityInstanceId);

		taskService.complete(task.Id);
		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testSetVariableOnUserTaskInsideSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("startSimpleSubProcess");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.setVariable(task.Id, "testVar", "testValue");

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the process instance scope
		assertEquals(pi.Id, variable.ActivityInstanceId);

		taskService.complete(task.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetVariableOnServiceTaskInsideSubProcess()
	  public virtual void testSetVariableOnServiceTaskInsideSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the process instance scope
		assertEquals(pi.Id, variable.ActivityInstanceId);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetVariableLocalOnServiceTaskInsideSubProcess()
	  public virtual void testSetVariableLocalOnServiceTaskInsideSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		string activityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("SubProcess_1").singleResult().Id;

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the sub process scope
		assertEquals(activityInstanceId, variable.ActivityInstanceId);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetVariableLocalOnTaskInsideParallelBranch()
	  public virtual void testSetVariableLocalOnTaskInsideParallelBranch()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.setVariableLocal(task.Id, "testVar", "testValue");
		ExecutionEntity taskExecution = (ExecutionEntity) runtimeService.createExecutionQuery().executionId(task.ExecutionId).singleResult();
		assertNotNull(taskExecution);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the user task scope
		assertEquals(taskExecution.ActivityInstanceId, variable.ActivityInstanceId);

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricVariableInstanceScopeTest.testSetVariableLocalOnTaskInsideParallelBranch.bpmn"})]
	  public virtual void testSetVariableOnTaskInsideParallelBranch()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.setVariable(task.Id, "testVar", "testValue");

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the process instance scope
		assertEquals(pi.Id, variable.ActivityInstanceId);

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetVariableOnServiceTaskInsideParallelBranch()
	  public virtual void testSetVariableOnServiceTaskInsideParallelBranch()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the process instance scope
		assertEquals(pi.Id, variable.ActivityInstanceId);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetVariableLocalOnServiceTaskInsideParallelBranch()
	  public virtual void testSetVariableLocalOnServiceTaskInsideParallelBranch()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstance serviceTask = historyService.createHistoricActivityInstanceQuery().activityId("serviceTask1").singleResult();
		assertNotNull(serviceTask);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertEquals(1, query.count());

		HistoricVariableInstance variable = query.singleResult();
		// the variable is in the service task scope
		assertEquals(serviceTask.Id, variable.ActivityInstanceId);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testHistoricCaseVariableInstanceQuery()
	  {
		// start case instance with variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase", variables).Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("CasePlanModel_1").singleResult().Id;
		string taskExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// set variable on both executions
		caseService.setVariableLocal(caseExecutionId, "case", "execution");
		caseService.setVariableLocal(taskExecutionId, "task", "execution");

		// update variable on both executions
		caseService.setVariableLocal(caseExecutionId, "case", "update");
		caseService.setVariableLocal(taskExecutionId, "task", "update");

		assertEquals(3, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(3, historyService.createHistoricVariableInstanceQuery().caseInstanceId(caseInstanceId).count());
		assertEquals(3, historyService.createHistoricVariableInstanceQuery().caseExecutionIdIn(caseExecutionId, taskExecutionId).count());
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().caseExecutionIdIn(taskExecutionId).count());

		HistoryLevel historyLevel = processEngineConfiguration.HistoryLevel;
		if (historyLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  assertEquals(5, historyService.createHistoricDetailQuery().count());
		  assertEquals(5, historyService.createHistoricDetailQuery().caseInstanceId(caseInstanceId).count());
		  assertEquals(3, historyService.createHistoricDetailQuery().caseExecutionId(caseExecutionId).count());
		  assertEquals(2, historyService.createHistoricDetailQuery().caseExecutionId(taskExecutionId).count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputMappings()
	  public virtual void testInputMappings()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		HistoricActivityInstanceQuery activityInstanceQuery = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstanceId);

		string theService1Id = activityInstanceQuery.activityId("theService1").singleResult().Id;
		string theService2Id = activityInstanceQuery.activityId("theService2").singleResult().Id;
		string theTaskId = activityInstanceQuery.activityId("theTask").singleResult().Id;

		// when (1)
		HistoricVariableInstance firstVariable = historyService.createHistoricVariableInstanceQuery().variableName("firstInputVariable").singleResult();

		// then (1)
		assertEquals(theService1Id, firstVariable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail firstVariableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(firstVariable.Id).singleResult();
		  assertEquals(theService1Id, firstVariableDetail.ActivityInstanceId);
		}

		// when (2)
		HistoricVariableInstance secondVariable = historyService.createHistoricVariableInstanceQuery().variableName("secondInputVariable").singleResult();

		// then (2)
		assertEquals(theService2Id, secondVariable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail secondVariableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(secondVariable.Id).singleResult();
		  assertEquals(theService2Id, secondVariableDetail.ActivityInstanceId);
		}

		// when (3)
		HistoricVariableInstance thirdVariable = historyService.createHistoricVariableInstanceQuery().variableName("thirdInputVariable").singleResult();

		// then (3)
		assertEquals(theTaskId, thirdVariable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail thirdVariableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(thirdVariable.Id).singleResult();
		  assertEquals(theTaskId, thirdVariableDetail.ActivityInstanceId);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCmmnActivityInstanceIdOnCaseInstance()
	  {

		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");

		string taskExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(taskExecutionId).setVariable("foo", "bar").execute();

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("foo").singleResult();

		assertNotNull(variable);
		assertEquals(caseInstance.Id, variable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail variableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(variable.Id).singleResult();
		  assertEquals(taskExecutionId, variableDetail.ActivityInstanceId);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCmmnActivityInstanceIdOnCaseExecution()
	  {

		// given
		caseService.createCaseInstanceByKey("oneTaskCase");

		string taskExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(taskExecutionId).setVariableLocal("foo", "bar").execute();

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("foo").singleResult();

		assertNotNull(variable);
		assertEquals(taskExecutionId, variable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail variableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(variable.Id).singleResult();
		  assertEquals(taskExecutionId, variableDetail.ActivityInstanceId);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCmmnActivityInstanceIdOnTask()
	  {

		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");

		string taskExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.setVariable(task.Id, "foo", "bar");

		// then
		HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("foo").singleResult();

		assertNotNull(variable);
		assertEquals(caseInstance.Id, variable.ActivityInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricDetail variableDetail = historyService.createHistoricDetailQuery().variableUpdates().variableInstanceId(variable.Id).singleResult();
		  assertEquals(taskExecutionId, variableDetail.ActivityInstanceId);
		}

	  }

	}

}