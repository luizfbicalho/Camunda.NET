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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;

	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// Tests the operationId field in historic tables, which helps to correlate records from different tables.
	/// 
	/// @author Svetlana Dorokhova
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class UserOperationIdTest
	{
		private bool InstanceFieldsInitialized = false;

		public UserOperationIdTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
		public ProcessEngineRule engineRule = new ProcessEngineRule(true);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testRule = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testRule;

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal string deploymentId;

	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		formService = engineRule.FormService;
		identityService = engineRule.IdentityService;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testResolveTaskOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testResolveTaskOperationId()
	  {
		// given
		identityService.AuthenticatedUserId = "demo";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE).taskId(taskId).list();
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();
		verifySameOperationId(userOperationLogEntries, historicDetails);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testSubmitTaskFormOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskFormOperationId()
	  {
		// given
		identityService.AuthenticatedUserId = "demo";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		formService.submitTaskForm(taskId, Variables);

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE).taskId(taskId).list();
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();
		verifySameOperationId(userOperationLogEntries, historicDetails);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testSetTaskVariablesOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetTaskVariablesOperationId()
	  {
		// given
		identityService.AuthenticatedUserId = "demo";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariables(taskId, Variables);

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE).taskId(taskId).list();
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();
		verifySameOperationId(userOperationLogEntries, historicDetails);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testWithoutAuthentication()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testWithoutAuthentication()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId, Variables);

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().taskId(taskId).list();
		assertEquals(0, userOperationLogEntries.Count);
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();
		assertTrue(historicDetails.Count > 0);
		//history detail records must have null userOperationId as user operation log was not created
		foreach (HistoricDetail historicDetail in historicDetails)
		{
		  assertNull(historicDetail.UserOperationId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetTaskVariablesInServiceTask()
	  public virtual void testSetTaskVariablesInServiceTask()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask().serviceTask().camundaExpression("${execution.setVariable('foo', 'bar')}").endEvent().done();
		testRule.deploy(bpmnModelInstance);

		identityService.AuthenticatedUserId = "demo";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.complete(task.Id);

		//then
		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();
		// no user operation log id is set for this update, as it is not written as part of the user operation
		assertNull(historicDetail.UserOperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testStartProcessOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartProcessOperationId()
	  {
		// given
		identityService.AuthenticatedUserId = "demo";

		// when
		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables);

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).processInstanceId(pi.Id).list();
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();

		assertFalse(userOperationLogEntries.Count == 0);
		assertFalse(historicDetails.Count == 0);
		verifySameOperationId(userOperationLogEntries, historicDetails);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testStartProcessAtActivityOperationId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartProcessAtActivityOperationId()
	  {
		// given
		identityService.AuthenticatedUserId = "demo";

		// when
		ProcessInstance pi = runtimeService.createProcessInstanceByKey(PROCESS_KEY).startBeforeActivity("theTask").setVariables(Variables).execute();

		//then
		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).processInstanceId(pi.Id).list();
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();

		assertFalse(userOperationLogEntries.Count == 0);
		assertFalse(historicDetails.Count == 0);
		verifySameOperationId(userOperationLogEntries, historicDetails);
	  }

	  private void verifySameOperationId(IList<UserOperationLogEntry> userOperationLogEntries, IList<HistoricDetail> historicDetails)
	  {
		assertTrue("Operation log entry must exist", userOperationLogEntries.Count > 0);
		string operationId = userOperationLogEntries[0].OperationId;
		assertNotNull(operationId);
		assertTrue("Some historic details are expected to be present", historicDetails.Count > 0);
		foreach (UserOperationLogEntry userOperationLogEntry in userOperationLogEntries)
		{
		  assertEquals("OperationIds must be the same", operationId, userOperationLogEntry.OperationId);
		}
		foreach (HistoricDetail historicDetail in historicDetails)
		{
		  assertEquals("OperationIds must be the same", operationId, historicDetail.UserOperationId);
		}
	  }

	  protected internal virtual VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue("aVariableName", "aVariableValue").putValue("anotherVariableName", "anotherVariableValue");
		  }
	  }

	}

}