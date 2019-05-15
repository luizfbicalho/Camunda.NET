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
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
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

	public class CustomHistoryLevelUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public CustomHistoryLevelUserOperationLogTest()
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
	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string ONE_TASK_CASE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  internal HistoryLevel customHistoryLevelUOL = new CustomHistoryLevelUserOperationLog();
	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JdbcUrl = "jdbc:h2:mem:CustomHistoryLevelUserOperationLogTest";
			configuration.CustomHistoryLevels = Arrays.asList(outerInstance.customHistoryLevelUOL);
			configuration.History = "aCustomHistoryLevelUOL";
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
		IList<UserOperationLogEntry> logs = query().list();
		foreach (UserOperationLogEntry log in logs)
		{
		  historyService.deleteUserOperationLogEntry(log.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testQueryProcessInstanceOperationsById()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testQueryProcessInstanceOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.suspendProcessInstanceById(process.Id);
		runtimeService.activateProcessInstanceById(process.Id);

		runtimeService.deleteProcessInstance(process.Id, "a delete reason");

		// then
		assertEquals(4, query().entityType(PROCESS_INSTANCE).count());

		UserOperationLogEntry deleteEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_DELETE).singleResult();

		assertNotNull(deleteEntry);
		assertEquals(process.Id, deleteEntry.ProcessInstanceId);
		assertNotNull(deleteEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", deleteEntry.ProcessDefinitionKey);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, deleteEntry.Category);

		UserOperationLogEntry suspendEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_SUSPEND).singleResult();

		assertNotNull(suspendEntry);
		assertEquals(process.Id, suspendEntry.ProcessInstanceId);
		assertNotNull(suspendEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendEntry.ProcessDefinitionKey);

		assertEquals("suspensionState", suspendEntry.Property);
		assertEquals("suspended", suspendEntry.NewValue);
		assertNull(suspendEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendEntry.Category);

		UserOperationLogEntry activateEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_ACTIVATE).singleResult();

		assertNotNull(activateEntry);
		assertEquals(process.Id, activateEntry.ProcessInstanceId);
		assertNotNull(activateEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateEntry.ProcessDefinitionKey);

		assertEquals("suspensionState", activateEntry.Property);
		assertEquals("active", activateEntry.NewValue);
		assertNull(activateEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateEntry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryProcessDefinitionOperationsByKey()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessDefinitionOperationsByKey()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		repositoryService.suspendProcessDefinitionByKey("oneTaskProcess", true, null);
		repositoryService.activateProcessDefinitionByKey("oneTaskProcess", true, null);
		repositoryService.deleteProcessDefinitions().byKey("oneTaskProcess").cascade().delete();

		// then
		assertEquals(3, query().entityType(PROCESS_DEFINITION).count());

		UserOperationLogEntry suspendDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION).singleResult();

		assertNotNull(suspendDefinitionEntry);
		assertNull(suspendDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendDefinitionEntry.ProcessDefinitionKey);
		assertNull(suspendDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", suspendDefinitionEntry.Property);
		assertEquals("suspended", suspendDefinitionEntry.NewValue);
		assertNull(suspendDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendDefinitionEntry.Category);

		UserOperationLogEntry activateDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION).singleResult();

		assertNotNull(activateDefinitionEntry);
		assertNull(activateDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateDefinitionEntry.ProcessDefinitionKey);
		assertNull(activateDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", activateDefinitionEntry.Property);
		assertEquals("active", activateDefinitionEntry.NewValue);
		assertNull(activateDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateDefinitionEntry.Category);

		UserOperationLogEntry deleteDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_DELETE).singleResult();

		assertNotNull(deleteDefinitionEntry);
		assertNotNull(deleteDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", deleteDefinitionEntry.ProcessDefinitionKey);
		assertNotNull(deleteDefinitionEntry.DeploymentId);

		assertEquals("cascade", deleteDefinitionEntry.Property);
		assertEquals("true", deleteDefinitionEntry.NewValue);
		assertNotNull(deleteDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, deleteDefinitionEntry.Category);
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
		assertEquals(2, query().entityType(JOB_DEFINITION).count());
		assertEquals(2, query().entityType(JOB).count());

		// active job definition
		UserOperationLogEntry activeJobDefinitionEntry = query().entityType(JOB_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_ACTIVATE_JOB_DEFINITION).singleResult();

		assertNotNull(activeJobDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, activeJobDefinitionEntry.ProcessDefinitionId);

		assertEquals("suspensionState", activeJobDefinitionEntry.Property);
		assertEquals("active", activeJobDefinitionEntry.NewValue);
		assertNull(activeJobDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activeJobDefinitionEntry.Category);

		// active job
		UserOperationLogEntry activateJobIdEntry = query().entityType(JOB).processInstanceId(process.ProcessInstanceId).operationType(OPERATION_TYPE_ACTIVATE_JOB).singleResult();

		assertNotNull(activateJobIdEntry);
		assertEquals(process.ProcessInstanceId, activateJobIdEntry.ProcessInstanceId);

		assertEquals("suspensionState", activateJobIdEntry.Property);
		assertEquals("active", activateJobIdEntry.NewValue);
		assertNull(activateJobIdEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateJobIdEntry.Category);

		// suspended job definition
		UserOperationLogEntry suspendJobDefinitionEntry = query().entityType(JOB_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_SUSPEND_JOB_DEFINITION).singleResult();

		assertNotNull(suspendJobDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, suspendJobDefinitionEntry.ProcessDefinitionId);

		assertEquals("suspensionState", suspendJobDefinitionEntry.Property);
		assertEquals("suspended", suspendJobDefinitionEntry.NewValue);
		assertNull(suspendJobDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendJobDefinitionEntry.Category);

		// suspended job
		UserOperationLogEntry suspendedJobEntry = query().entityType(JOB).processInstanceId(process.ProcessInstanceId).operationType(OPERATION_TYPE_SUSPEND_JOB).singleResult();

		assertNotNull(suspendedJobEntry);
		assertEquals(process.ProcessInstanceId, suspendedJobEntry.ProcessInstanceId);

		assertEquals("suspensionState", suspendedJobEntry.Property);
		assertEquals("suspended", suspendedJobEntry.NewValue);
		assertNull(suspendedJobEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendedJobEntry.Category);
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
		assertEquals(1, query().entityType(JOB).operationType(OPERATION_TYPE_SET_JOB_RETRIES).count());

		UserOperationLogEntry jobRetryEntry = query().entityType(JOB).jobId(job.Id).operationType(OPERATION_TYPE_SET_JOB_RETRIES).singleResult();

		assertNotNull(jobRetryEntry);
		assertEquals(job.Id, jobRetryEntry.JobId);

		assertEquals("3", jobRetryEntry.OrgValue);
		assertEquals("10", jobRetryEntry.NewValue);
		assertEquals("retries", jobRetryEntry.Property);
		assertEquals(job.JobDefinitionId, jobRetryEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, jobRetryEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionKey, jobRetryEntry.ProcessDefinitionKey);
		assertEquals(job.ProcessDefinitionId, jobRetryEntry.ProcessDefinitionId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, jobRetryEntry.Category);
	  }

	  // ----- PROCESS INSTANCE MODIFICATION -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testQueryProcessInstanceModificationOperation()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testQueryProcessInstanceModificationOperation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().singleResult();

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();

		UserOperationLogQuery logQuery = query().entityType(EntityTypes.PROCESS_INSTANCE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE);

		assertEquals(1, logQuery.count());
		UserOperationLogEntry logEntry = logQuery.singleResult();

		assertEquals(processInstanceId, logEntry.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, logEntry.ProcessDefinitionId);
		assertEquals(definition.Key, logEntry.ProcessDefinitionKey);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE, logEntry.OperationType);
		assertEquals(EntityTypes.PROCESS_INSTANCE, logEntry.EntityType);
		assertNull(logEntry.Property);
		assertNull(logEntry.OrgValue);
		assertNull(logEntry.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
	  }

	  // ----- ADD VARIABLES -----

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testQueryAddExecutionVariableOperation()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testQueryAddExecutionVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.setVariable(process.Id, "testVariable1", "THIS IS TESTVARIABLE!!!");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testQueryAddTaskVariablesSingleAndMapOperation()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testQueryAddTaskVariablesSingleAndMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariable(processTaskId, "testVariable3", "foo");
		taskService.setVariables(processTaskId, createMapForVariableAddition());
		taskService.setVariable(processTaskId, "testVariable4", "bar");

		// then
		verifyVariableOperationAsserts(3, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
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
	   verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
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
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  // ----- REMOVE VARIABLES -----


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryRemoveExecutionVariablesMapOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveExecutionVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.removeVariables(process.Id, createCollectionForVariableDeletion());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryRemoveTaskVariableOperation()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveTaskVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.removeVariable(processTaskId, "testVariable1");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
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

		verifyQueryResults(query, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {ONE_TASK_PROCESS}) public void testQueryByCategories()
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryByCategories()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setAssignee(processTaskId, "foo");
		taskService.setVariable(processTaskId, "foo", "bar");

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);

		verifyQueryResults(query, 2);
	  }
	  // --------------- CMMN --------------------

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={ONE_TASK_CASE}) public void testQueryByCaseExecutionId()
	  [Deployment(resources:{ONE_TASK_CASE})]
	  public virtual void testQueryByCaseExecutionId()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		taskService.setAssignee(task.Id, "demo");

		// then

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseExecutionId(caseExecutionId);

		verifyQueryResults(query, 1);
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
		verifyQueryResults(query, 1);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { ONE_TASK_PROCESS }) public void testUserOperationLogDeletion()
	  [Deployment(resources : { ONE_TASK_PROCESS })]
	  public virtual void testUserOperationLogDeletion()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable1", "THIS IS TESTVARIABLE!!!");

		// assume
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
		UserOperationLogQuery query = query().entityType(EntityTypes.VARIABLE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE);
		assertEquals(1, query.count());

		// when
		historyService.deleteUserOperationLogEntry(query.singleResult().Id);

		// then
		assertEquals(0, query.count());
	  }

	  protected internal virtual void verifyQueryResults(UserOperationLogQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  protected internal virtual void verifySingleResultFails(UserOperationLogQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
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

	  protected internal virtual void verifyVariableOperationAsserts(int countAssertValue, string operationType, string category)
	  {
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(countAssertValue, logQuery.count());

		if (countAssertValue > 1)
		{
		  IList<UserOperationLogEntry> logEntryList = logQuery.list();

		  foreach (UserOperationLogEntry logEntry in logEntryList)
		  {
			assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
			assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
			assertEquals(category, logEntry.Category);
		  }
		}
		else
		{
		  UserOperationLogEntry logEntry = logQuery.singleResult();
		  assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
		  assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
		  assertEquals(category, logEntry.Category);
		}
	  }

	  protected internal virtual UserOperationLogQuery query()
	  {
		return historyService.createUserOperationLogQuery();
	  }

	}

}