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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using org.junit;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class LegacyUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public LegacyUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(processEngineRule);
			chain = RuleChain.outerRule(processEngineRule).around(testHelper);
		}


	  public const string USER_ID = "demo";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule("org/camunda/bpm/engine/test/history/useroperationlog/enable.legacy.user.operation.log.camunda.cfg.xml");
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule("org/camunda/bpm/engine/test/history/useroperationlog/enable.legacy.user.operation.log.camunda.cfg.xml");
	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(processEngineRule).around(testHelper);
	  public RuleChain chain;

	  protected internal IdentityService identityService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

	  protected internal Batch batch;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		identityService = processEngineRule.IdentityService;
		runtimeService = processEngineRule.RuntimeService;
		taskService = processEngineRule.TaskService;
		historyService = processEngineRule.HistoryService;
		managementService = processEngineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatch()
	  public virtual void removeBatch()
	  {
		Batch batch = managementService.createBatchQuery().singleResult();
		if (batch != null)
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml") public void testLogAllOperationWithAuthentication()
	  [Deployment(resources : "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml")]
	  public virtual void testLogAllOperationWithAuthentication()
	  {
		try
		{
		  // given
		  identityService.AuthenticatedUserId = USER_ID;
		  string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		  string taskId = taskService.createTaskQuery().singleResult().Id;

		  // when
		  taskService.complete(taskId);

		  // then
		  assertTrue((bool?) runtimeService.getVariable(processInstanceId, "taskListenerCalled"));
		  assertTrue((bool?) runtimeService.getVariable(processInstanceId, "serviceTaskCalled"));

		  UserOperationLogQuery query = userOperationLogQuery().userId(USER_ID);
		  assertEquals(4, query.count());
		  assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
		  assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE).count());
		  assertEquals(2, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE).count());

		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml") public void testLogOperationWithoutAuthentication()
	  [Deployment(resources : "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml")]
	  public virtual void testLogOperationWithoutAuthentication()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		// then
		assertTrue((bool?) runtimeService.getVariable(processInstanceId, "taskListenerCalled"));
		assertTrue((bool?) runtimeService.getVariable(processInstanceId, "serviceTaskCalled"));

		assertEquals(5, userOperationLogQuery().count());
		assertEquals(1, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE).count());
		assertEquals(2, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE).count());
		assertEquals(1, userOperationLogQuery().entityType(EntityTypes.DEPLOYMENT).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
		assertEquals(1, userOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml") public void testLogSetVariableWithoutAuthentication()
	  [Deployment(resources : "org/camunda/bpm/engine/test/history/useroperationlog/UserOperationLogTaskTest.testOnlyTaskCompletionIsLogged.bpmn20.xml")]
	  public virtual void testLogSetVariableWithoutAuthentication()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		runtimeService.setVariable(processInstanceId, "aVariable", "aValue");

		// then
		assertEquals(3, userOperationLogQuery().count());
		assertEquals(1, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE).count());
		assertEquals(1, userOperationLogQuery().entityType(EntityTypes.DEPLOYMENT).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
		assertEquals(1, userOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDontWriteDuplicateLogOnBatchDeletionJobExecution()
	  public virtual void testDontWriteDuplicateLogOnBatchDeletionJobExecution()
	  {
		ProcessDefinition definition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(definition.Id);
		batch = runtimeService.deleteProcessInstancesAsync(Arrays.asList(processInstance.Id), null, "test reason");

		Job seedJob = managementService.createJobQuery().singleResult();
		managementService.executeJob(seedJob.Id);

		foreach (Job pending in managementService.createJobQuery().list())
		{
		  managementService.executeJob(pending.Id);
		}

		assertEquals(5, userOperationLogQuery().entityTypeIn(EntityTypes.PROCESS_INSTANCE, EntityTypes.DEPLOYMENT).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDontWriteDuplicateLogOnBatchMigrationJobExecution()
	  public virtual void testDontWriteDuplicateLogOnBatchMigrationJobExecution()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		batch = runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();
		Job seedJob = managementService.createJobQuery().singleResult();
		managementService.executeJob(seedJob.Id);

		Job migrationJob = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).singleResult();

		// when
		managementService.executeJob(migrationJob.Id);

		// then
		assertEquals(9, userOperationLogQuery().count());
		assertEquals(2, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).entityType(EntityTypes.DEPLOYMENT).count());
		assertEquals(1, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).entityType(EntityTypes.PROCESS_INSTANCE).count());
		assertEquals(3, userOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MIGRATE).entityType(EntityTypes.PROCESS_INSTANCE).count());
	  }

	  protected internal virtual UserOperationLogQuery userOperationLogQuery()
	  {
		return historyService.createUserOperationLogQuery();
	  }

	}

}