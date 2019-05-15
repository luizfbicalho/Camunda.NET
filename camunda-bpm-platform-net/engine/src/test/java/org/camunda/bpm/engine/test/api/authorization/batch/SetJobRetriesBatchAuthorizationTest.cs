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
namespace org.camunda.bpm.engine.test.api.authorization.batch
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationScenarioWithCount = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenarioWithCount;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class SetJobRetriesBatchAuthorizationTest extends AbstractBatchAuthorizationTest
	public class SetJobRetriesBatchAuthorizationTest : AbstractBatchAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetJobRetriesBatchAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string DEFINITION_XML = "org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml";
	  protected internal const long BATCH_OPERATIONS = 3;
	  protected internal const int RETRIES = 5;

	  protected internal virtual void assertRetries(IList<string> allJobIds, int i)
	  {
		foreach (string id in allJobIds)
		{
		  Assert.assertThat(managementService.createJobQuery().jobId(id).singleResult().Retries, @is(i));
		}
	  }

	  protected internal virtual IList<string> AllJobIds
	  {
		  get
		  {
			List<string> result = new List<string>();
			foreach (Job job in managementService.createJobQuery().processDefinitionId(sourceDefinition.Id).list())
			{
			  if (!string.ReferenceEquals(job.ProcessInstanceId, null))
			  {
				result.Add(job.Id);
			  }
			}
			return result;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenarioWithCount scenario;
	  public AuthorizationScenarioWithCount scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @Before public void deployProcesses()
	  public override void deployProcesses()
	  {
		Deployment deploy = testHelper.deploy(DEFINITION_XML);
		sourceDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().deploymentId(deploy.Id).singleResult();
		processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		processInstance2 = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(AuthorizationScenarioWithCount.scenario().withCount(3).withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "exceptionInJobExecution", "userId", Permissions.UPDATE_INSTANCE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", ProcessInstancePermissions.RETRY_JOB), grant(Resources.PROCESS_DEFINITION, "exceptionInJobExecution", "userId", ProcessDefinitionPermissions.RETRY_JOB)), AuthorizationScenarioWithCount.scenario().withCount(5).withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.ALL), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.ALL)).succeeds(), AuthorizationScenarioWithCount.scenario().withCount(5).withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_DEFINITION, "Process", "userId", Permissions.READ_INSTANCE, Permissions.UPDATE_INSTANCE)).succeeds(), AuthorizationScenarioWithCount.scenario().withCount(5).withAuthorizations(grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES), grant(Resources.PROCESS_DEFINITION, "Process", "userId", Permissions.READ_INSTANCE, Permissions.UPDATE_INSTANCE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithTwoInvocationsJobsListBased()
	  public virtual void testWithTwoInvocationsJobsListBased()
	  {
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		setupAndExecuteJobsListBasedTest();

		// then
		assertScenario();

		assertRetries(AllJobIds, Convert.ToInt64(Scenario.Count).intValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithTwoInvocationsJobsQueryBased()
	  public virtual void testWithTwoInvocationsJobsQueryBased()
	  {
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		setupAndExecuteJobsQueryBasedTest();

		// then
		assertScenario();

		assertRetries(AllJobIds, Convert.ToInt64(Scenario.Count).intValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobsListBased()
	  public virtual void testJobsListBased()
	  {
		setupAndExecuteJobsListBasedTest();
		// then
		assertScenario();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobsListQueryBased()
	  public virtual void testJobsListQueryBased()
	  {
		setupAndExecuteJobsQueryBasedTest();
		// then
		assertScenario();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithTwoInvocationsProcessListBased()
	  public virtual void testWithTwoInvocationsProcessListBased()
	  {
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		setupAndExecuteProcessListBasedTest();

		// then
		assertScenario();

		assertRetries(AllJobIds, Convert.ToInt64(Scenario.Count).intValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithTwoInvocationsProcessQueryBased()
	  public virtual void testWithTwoInvocationsProcessQueryBased()
	  {
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		setupAndExecuteJobsQueryBasedTest();

		// then
		assertScenario();

		assertRetries(AllJobIds, Convert.ToInt64(Scenario.Count).intValue());
	  }

	  private void setupAndExecuteProcessListBasedTest()
	  {
		//given
		IList<string> processInstances = Arrays.asList(new string[]{processInstance.Id, processInstance2.Id});
		authRule.init(scenario).withUser("userId").bindResource("Process", sourceDefinition.Key).bindResource("processInstance1", processInstance.Id).bindResource("processInstance2", processInstance2.Id).start();

		// when
		batch = managementService.setJobRetriesAsync(processInstances, (ProcessInstanceQuery) null, RETRIES);

		executeSeedAndBatchJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessList()
	  public virtual void testProcessList()
	  {
		setupAndExecuteProcessListBasedTest();
		// then
		assertScenario();
	  }

	  protected internal virtual void setupAndExecuteJobsListBasedTest()
	  {
		//given
		IList<string> allJobIds = AllJobIds;
		authRule.init(scenario).withUser("userId").bindResource("Process", sourceDefinition.Key).bindResource("processInstance1", processInstance.Id).bindResource("processInstance2", processInstance2.Id).start();

		// when
		batch = managementService.setJobRetriesAsync(allJobIds, RETRIES);

		executeSeedAndBatchJobs();
	  }

	  protected internal virtual void setupAndExecuteJobsQueryBasedTest()
	  {
		//given
		JobQuery jobQuery = managementService.createJobQuery();
		authRule.init(scenario).withUser("userId").bindResource("Process", sourceDefinition.Key).bindResource("processInstance1", processInstance.Id).bindResource("processInstance2", processInstance2.Id).start();

		// when

		batch = managementService.setJobRetriesAsync(jobQuery, RETRIES);

		executeSeedAndBatchJobs();
	  }

	  public override AuthorizationScenarioWithCount Scenario
	  {
		  get
		  {
			return scenario;
		  }
	  }

	  protected internal virtual void assertScenario()
	  {
		if (authRule.assertScenario(Scenario))
		{
		  Batch batch = engineRule.ManagementService.createBatchQuery().singleResult();
		  assertEquals("userId", batch.CreateUserId);

		  if (testHelper.HistoryLevelFull)
		  {
			assertThat(engineRule.HistoryService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES).count(), @is(BATCH_OPERATIONS));
			HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().list().get(0);
			assertEquals("userId", historicBatch.CreateUserId);
		  }
		  assertRetries(AllJobIds, 5);
		}
	  }
	}

}