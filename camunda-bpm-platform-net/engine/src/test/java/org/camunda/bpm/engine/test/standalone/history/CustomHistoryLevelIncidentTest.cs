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
namespace org.camunda.bpm.engine.test.standalone.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class CustomHistoryLevelIncidentTest
	public class CustomHistoryLevelIncidentTest
	{
		private bool InstanceFieldsInitialized = false;

		public CustomHistoryLevelIncidentTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			migrationRule = new MigrationTestRule(engineRule);
			migrationHelper = new BatchMigrationHelper(engineRule, migrationRule);
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule).around(migrationRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
		public static ICollection<object[]> data()
		{
		return Arrays.asList(new object[][]
		{
			new object[]{Arrays.asList(HistoryEventTypes.INCIDENT_CREATE)},
			new object[]{Arrays.asList(HistoryEventTypes.INCIDENT_CREATE, HistoryEventTypes.INCIDENT_RESOLVE)},
			new object[]{Arrays.asList(HistoryEventTypes.INCIDENT_DELETE, HistoryEventTypes.INCIDENT_CREATE, HistoryEventTypes.INCIDENT_MIGRATE, HistoryEventTypes.INCIDENT_RESOLVE)}
		});
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public static java.util.List<org.camunda.bpm.engine.impl.history.event.HistoryEventTypes> eventTypes;
	  public static IList<HistoryEventTypes> eventTypes;

	  internal CustomHistoryLevelIncident customHistoryLevelIncident = new CustomHistoryLevelIncident(eventTypes);

	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl processEngineConfiguration)
		  {
			processEngineConfiguration.JdbcUrl = "jdbc:h2:mem:" + typeof(CustomHistoryLevelIncident).Name;
			IList<HistoryLevel> levels = new List<HistoryLevel>();
			levels.Add(outerInstance.customHistoryLevelIncident);
			processEngineConfiguration.CustomHistoryLevels = levels;
			processEngineConfiguration.History = "aCustomHistoryLevelIncident";
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_CREATE_DROP;
			return processEngineConfiguration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper migrationHelper;
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule).around(migrationRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal TaskService taskService;
	  protected internal ProcessEngineConfigurationImpl configuration;

	  internal DeploymentWithDefinitions deployment;

	  public static string PROCESS_DEFINITION_KEY = "oneFailingServiceTaskProcess";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static BpmnModelInstance FAILING_SERVICE_TASK_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").serviceTask("task").camundaAsyncBefore().camundaClass(typeof(FailingDelegate).FullName).endEvent("end").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		repositoryService = engineRule.RepositoryService;
		taskService = engineRule.TaskService;
		configuration = engineRule.ProcessEngineConfiguration;

		customHistoryLevelIncident.EventTypes = eventTypes;
		configuration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		customHistoryLevelIncident.EventTypes = null;
		if (deployment != null)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
		migrationHelper.removeAllRunningAndHistoricBatches();

		configuration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CustomHistoryLevelIncidentTest outerInstance;

		  public CommandAnonymousInnerClass(CustomHistoryLevelIncidentTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.managementService.createJobQuery().list();
			foreach (Job job in jobs)
			{
			  commandContext.JobManager.deleteJob((JobEntity) job);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			}

			IList<HistoricIncident> historicIncidents = outerInstance.historyService.createHistoricIncidentQuery().list();
			foreach (HistoricIncident historicIncident in historicIncidents)
			{
			  commandContext.DbEntityManager.delete((HistoricIncidentEntity) historicIncident);
			}

			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricIncidentByProcDefId()
	  public virtual void testDeleteHistoricIncidentByProcDefId()
	  {
		// given
		deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", FAILING_SERVICE_TASK_MODEL).deployWithResult();
		string processDefinitionId = deployment.DeployedProcessDefinitions[0].Id;

		runtimeService.startProcessInstanceById(processDefinitionId);
		executeAvailableJobs();


		if (eventTypes != null)
		{
		  HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		  assertNotNull(historicIncident);
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey(PROCESS_DEFINITION_KEY).cascade().delete();

		// then
		IList<HistoricIncident> incidents = historyService.createHistoricIncidentQuery().list();
		assertEquals(0, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricIncidentByBatchId()
	  public virtual void testDeleteHistoricIncidentByBatchId()
	  {
		// given
		initBatchOperationHistoryTimeToLive();
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, -11);

		BatchEntity batch = (BatchEntity) createFailingMigrationBatch();

		migrationHelper.executeSeedJob(batch);

		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (((JobEntity) job).JobHandlerType.Equals("instance-migration"))
		  {
			managementService.setJobRetries(job.Id, 1);
		  }
		}
		migrationHelper.executeJobs(batch);

		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, -10);
		managementService.deleteBatch(batch.Id, false);
		ClockUtil.CurrentTime = DateTime.Now;

		// assume
		if (eventTypes != null)
		{
		  HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		  assertNotNull(historicIncident);
		}

		// when
		historyService.cleanUpHistoryAsync(true);
		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  managementService.executeJob(job.Id);
		}

		// then
		IList<HistoricIncident> incidents = historyService.createHistoricIncidentQuery().list();
		assertEquals(0, incidents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricIncidentByJobDefinitionId()
	  public virtual void testDeleteHistoricIncidentByJobDefinitionId()
	  {
		// given
		BatchEntity batch = (BatchEntity) createFailingMigrationBatch();

		migrationHelper.executeSeedJob(batch);

		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (((JobEntity) job).JobHandlerType.Equals("instance-migration"))
		  {
			managementService.setJobRetries(job.Id, 1);
		  }
		}
		migrationHelper.executeJobs(batch);

		// assume
		if (eventTypes != null)
		{
		  HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		  assertNotNull(historicIncident);
		}

		// when
		managementService.deleteBatch(batch.Id, true);

		// then
		IList<HistoricIncident> incidents = historyService.createHistoricIncidentQuery().list();
		assertEquals(0, incidents.Count);
	  }

	  protected internal virtual void executeAvailableJobs()
	  {
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();

		if (jobs.Count == 0)
		{
		  return;
		}

		foreach (Job job in jobs)
		{
		  try
		  {
			managementService.executeJob(job.Id);
		  }
		  catch (Exception)
		  {
		  }
		}

		executeAvailableJobs();
	  }

	  protected internal virtual void initBatchOperationHistoryTimeToLive()
	  {
		configuration.BatchOperationHistoryTimeToLive = "P0D";
		configuration.initHistoryCleanup();
	  }

	  protected internal virtual Batch createFailingMigrationBatch()
	  {
		BpmnModelInstance instance = createModelInstance();

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(instance);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(instance);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id, "unknownId")).executeAsync();
		return batch;
	  }

	  protected internal virtual BpmnModelInstance createModelInstance()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent("start").userTask("userTask1").endEvent("end").done();
		return instance;
	  }
	}

}