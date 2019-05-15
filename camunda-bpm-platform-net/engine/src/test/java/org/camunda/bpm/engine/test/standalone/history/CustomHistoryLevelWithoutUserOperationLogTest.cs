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
namespace org.camunda.bpm.engine.test.standalone.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationTestBaseRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestBaseRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class CustomHistoryLevelWithoutUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public CustomHistoryLevelWithoutUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestBaseRule(engineRule);
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(authRule).around(testRule);
		}


	  public const string USER_ID = "demo";
	  private const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string ONE_TASK_CASE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  internal HistoryLevel customHistoryLevelFullWUOL = new CustomHistoryLevelFullWithoutUserOperationLog();
	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JdbcUrl = "jdbc:h2:mem:CustomHistoryLevelWithoutUserOperationLogTest";
			configuration.CustomHistoryLevels = Arrays.asList(outerInstance.customHistoryLevelFullWUOL);
			configuration.History = "aCustomHistoryLevelWUOL";
			configuration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_CREATE_DROP;
			return configuration;
		  }
	  }

	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public AuthorizationTestBaseRule authRule;
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(authRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementServiceImpl managementService;
	  protected internal IdentityService identityService;
	  protected internal RepositoryService repositoryService;
	  protected internal TaskService taskService;
	  protected internal CaseService caseService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal ProcessInstance process;
	  protected internal Task userTask;
	  protected internal string processTaskId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		managementService = (ManagementServiceImpl) engineRule.ManagementService;
		identityService = engineRule.IdentityService;
		repositoryService = engineRule.RepositoryService;
		taskService = engineRule.TaskService;
		caseService = engineRule.CaseService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService.AuthenticatedUserId = USER_ID;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryProcessInstanceOperationsByProcessDefinitionKey()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessInstanceOperationsByProcessDefinitionKey()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey("oneTaskProcess");
		runtimeService.activateProcessInstanceByProcessDefinitionKey("oneTaskProcess");

		// then
		assertEquals(0, query().entityType(PROCESS_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryProcessDefinitionOperationsById()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessDefinitionOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		repositoryService.suspendProcessDefinitionById(process.ProcessDefinitionId, true, null);
		repositoryService.activateProcessDefinitionById(process.ProcessDefinitionId, true, null);

		// then
		assertEquals(0, query().entityType(PROCESS_DEFINITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"}) public void testQueryJobOperations()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryJobOperations()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("process");

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
		managementService.activateJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
		managementService.suspendJobByProcessInstanceId(process.Id);
		managementService.activateJobByProcessInstanceId(process.Id);

		// then
		assertEquals(0, query().entityType(JOB_DEFINITION).count());
		assertEquals(0, query().entityType(JOB).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" }) public void testQueryJobRetryOperationsById()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" })]
	  public virtual void testQueryJobRetryOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("failedServiceTask");
		Job job = managementService.createJobQuery().processInstanceId(process.ProcessInstanceId).singleResult();

		managementService.setJobRetries(job.Id, 10);

		// then
		assertEquals(0, query().entityType(JOB).operationType(OPERATION_TYPE_SET_JOB_RETRIES).count());
	  }

	  // ----- PROCESS INSTANCE MODIFICATION -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testQueryProcessInstanceModificationOperation()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testQueryProcessInstanceModificationOperation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processInstance.Id;

		repositoryService.createProcessDefinitionQuery().singleResult();

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();

		UserOperationLogQuery logQuery = query().entityType(EntityTypes.PROCESS_INSTANCE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE);

		assertEquals(0, logQuery.count());
	  }

	  // ----- ADD VARIABLES -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryAddExecutionVariablesMapOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddExecutionVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.setVariables(process.Id, createMapForVariableAddition());

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryAddTaskVariableOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddTaskVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariable(processTaskId, "testVariable1", "THIS IS TESTVARIABLE!!!");

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE);
	  }

	  // ----- PATCH VARIABLES -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryPatchExecutionVariablesOperation()
		[Deployment(resources : {ONE_TASK_PROCESS})]
		public virtual void testQueryPatchExecutionVariablesOperation()
		{
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		((RuntimeServiceImpl) runtimeService).updateVariables(process.Id, createMapForVariableAddition(), createCollectionForVariableDeletion());

		// then
	   verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryPatchTaskVariablesOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryPatchTaskVariablesOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		((TaskServiceImpl) taskService).updateVariablesLocal(processTaskId, createMapForVariableAddition(), createCollectionForVariableDeletion());

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE);
	  }

	  // ----- REMOVE VARIABLES -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryRemoveExecutionVariableOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveExecutionVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.removeVariable(process.Id, "testVariable1");

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryByEntityTypes()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryByEntityTypes()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setAssignee(processTaskId, "foo");
		taskService.setVariable(processTaskId, "foo", "bar");

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().entityTypeIn(EntityTypes.TASK, EntityTypes.VARIABLE);

		assertEquals(0, query.count());
	  }

	  // ----- DELETE VARIABLE HISTORY -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariableHistoryOperationOnRunningInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariableHistoryOperationOnHistoryInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnHistoryInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryDeleteVariableHistoryOperationOnCase()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnCase()
	  {
		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");
		caseService.setVariable(caseInstance.Id, "myVariable", 1);
		caseService.setVariable(caseInstance.Id, "myVariable", 2);
		caseService.setVariable(caseInstance.Id, "myVariable", 3);
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryDeleteVariableHistoryOperationOnStandaloneTask()
	  public virtual void testQueryDeleteVariableHistoryOperationOnStandaloneTask()
	  {
		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariable(task.Id, "testVariable", "testValue");
		taskService.setVariable(task.Id, "testVariable", "testValue2");
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);

		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariablesHistoryOperationOnRunningInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariablesHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test2");
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariablesHistoryOperationOnHistoryInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariablesHistoryOperationOnHistoryInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariableAndVariablesHistoryOperationOnRunningInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableAndVariablesHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test2");
		runtimeService.setVariable(process.Id, "testVariable3", "test");
		runtimeService.setVariable(process.Id, "testVariable3", "test2");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().variableName("testVariable").singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryDeleteVariableAndVariablesHistoryOperationOnHistoryInstance()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableAndVariablesHistoryOperationOnHistoryInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable3", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().variableName("testVariable").singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyVariableOperationAsserts(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  // --------------- CMMN --------------------

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={ONE_TASK_CASE}) public void testQueryByCaseDefinitionId()
	  [Deployment(resources:{ONE_TASK_CASE})]
	  public virtual void testQueryByCaseDefinitionId()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		taskService.setAssignee(task.Id, "demo");

		// then

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseDefinitionId(caseDefinitionId);

		assertEquals(0, query.count());

		taskService.setAssignee(task.Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDeploymentId()
	  public virtual void testQueryByDeploymentId()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(ONE_TASK_PROCESS).deploy().Id;

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().deploymentId(deploymentId);

		// then
		assertEquals(0, query.count());

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  protected internal virtual IDictionary<string, object> createMapForVariableAddition()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["testVariable1"] = "THIS IS TESTVARIABLE!!!";
		variables["testVariable2"] = "OVER 9000!";

		return variables;
	  }

	  protected internal virtual ICollection<string> createCollectionForVariableDeletion()
	  {
		ICollection<string> variables = new List<string>();
		variables.Add("testVariable3");
		variables.Add("testVariable4");

		return variables;
	  }

	  protected internal virtual void verifyVariableOperationAsserts(string operationType)
	  {
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(0, logQuery.count());
	  }

	  protected internal virtual UserOperationLogQuery query()
	  {
		return historyService.createUserOperationLogQuery();
	  }

	}

}