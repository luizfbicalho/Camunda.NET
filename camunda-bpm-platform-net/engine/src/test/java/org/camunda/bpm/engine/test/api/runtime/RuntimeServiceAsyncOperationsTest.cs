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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	using IncrementCounterListener = org.camunda.bpm.engine.test.api.runtime.util.IncrementCounterListener;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class RuntimeServiceAsyncOperationsTest : AbstractAsyncOperationsTest
	{
		private bool InstanceFieldsInitialized = false;

		public RuntimeServiceAsyncOperationsTest()
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
			migrationRule = new MigrationTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
			migrationChain = RuleChain.outerRule(testRule).around(migrationRule);
		}


	  public new ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public new ProcessEngineTestRule testRule;

	  protected internal MigrationTestRule migrationRule;

	  private int defaultBatchJobsPerSeed;
	  private int defaultInvocationsPerBatchJob;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain migrationChain = org.junit.rules.RuleChain.outerRule(testRule).around(migrationRule);
	  public RuleChain migrationChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public override void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		IList<Batch> batches = managementService.createBatchQuery().list();
		if (batches.Count > 0)
		{
		  foreach (Batch batch in batches)
		  {
			managementService.deleteBatch(batch.Id, true);
		  }
		}

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreEngineSettings()
	  public virtual void restoreEngineSettings()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithList()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE);

		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithLargeList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithLargeList()
	  {
		// given
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 1010;
		IList<string> processIds = startTestProcesses(1100);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE);

		createAndExecuteSeedJobs(batch.SeedJobDefinitionId, 2);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();

		// cleanup
		if (!testRule.HistoryLevelNone)
		{
		  batch = historyService.deleteHistoricProcessInstancesAsync(processIds, null);
		  createAndExecuteSeedJobs(batch.SeedJobDefinitionId, 2);
		  executeBatchJobs(batch);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithListOnly() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithListOnly()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, TESTING_INSTANCE_DELETE);

		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithFake() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithFake()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);
		processIds.Add("aFake");

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE);

		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertEquals(0, exceptions.Count);

		assertThat(managementService.createJobQuery().withException().list().size(), @is(0));

		processIds.Remove("aFake");
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithNullList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithNullList()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("processInstanceIds is empty");

		runtimeService.deleteProcessInstancesAsync(null, null, TESTING_INSTANCE_DELETE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithEmptyList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithEmptyList()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("processInstanceIds is empty");

		runtimeService.deleteProcessInstancesAsync(new List<string>(), null, TESTING_INSTANCE_DELETE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithQuery()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceIds(new HashSet<string>(processIds));

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(null, processInstanceQuery, TESTING_INSTANCE_DELETE);

		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithQueryOnly() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithQueryOnly()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceIds(new HashSet<string>(processIds));

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processInstanceQuery, TESTING_INSTANCE_DELETE);

		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, TESTING_INSTANCE_DELETE, testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithQueryWithoutDeleteReason() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithQueryWithoutDeleteReason()
	  {
		// given
		IList<string> processIds = startTestProcesses(2);
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceIds(new HashSet<string>(processIds));

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(null, processInstanceQuery, null);

		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertHistoricTaskDeletionPresent(processIds, "deleted", testRule);
		assertHistoricBatchExists(testRule);
		assertProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithNullQueryParameter() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithNullQueryParameter()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("processInstanceIds is empty");

		runtimeService.deleteProcessInstancesAsync(null, null, TESTING_INSTANCE_DELETE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testDeleteProcessInstancesAsyncWithInvalidQueryParameter() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstancesAsyncWithInvalidQueryParameter()
	  {
		// given
		startTestProcesses(2);
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("invalid");

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("processInstanceIds is empty");

		// when
		runtimeService.deleteProcessInstancesAsync(null, query, TESTING_INSTANCE_DELETE);
	  }

	  protected internal virtual void assertProcessInstancesAreDeleted()
	  {
		assertThat(runtimeService.createProcessInstanceQuery().list().size(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesAsyncWithSkipCustomListeners()
	  public virtual void testDeleteProcessInstancesAsyncWithSkipCustomListeners()
	  {

		// given
		IncrementCounterListener.counter = 0;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = ProcessModels.newModel(ONE_TASK_PROCESS).startEvent().userTask().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(IncrementCounterListener).FullName).endEvent().done();

		testRule.deploy(instance);
		IList<string> processIds = startTestProcesses(1);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE, true);
		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertThat(IncrementCounterListener.counter, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesAsyncWithSkipSubprocesses()
	  public virtual void testDeleteProcessInstancesAsyncWithSkipSubprocesses()
	  {

		// given
		BpmnModelInstance callingInstance = ProcessModels.newModel(ONE_TASK_PROCESS).startEvent().callActivity().calledElement("called").endEvent().done();

		BpmnModelInstance calledInstance = ProcessModels.newModel("called").startEvent().userTask().endEvent().done();

		testRule.deploy(callingInstance, calledInstance);
		IList<string> processIds = startTestProcesses(1);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE, false, true);
		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		ProcessInstance superInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processIds[0]).singleResult();
		assertNull(superInstance);

		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("called").singleResult();
		assertNotNull(subInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesAsyncWithoutSkipSubprocesses()
	  public virtual void testDeleteProcessInstancesAsyncWithoutSkipSubprocesses()
	  {

		// given
		BpmnModelInstance callingInstance = ProcessModels.newModel(ONE_TASK_PROCESS).startEvent().callActivity().calledElement("called").endEvent().done();

		BpmnModelInstance calledInstance = ProcessModels.newModel("called").startEvent().userTask().endEvent().done();

		testRule.deploy(callingInstance, calledInstance);
		IList<string> processIds = startTestProcesses(1);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, null, TESTING_INSTANCE_DELETE, false, false);
		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		ProcessInstance superInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processIds[0]).singleResult();
		assertNull(superInstance);

		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("called").singleResult();
		assertNull(subInstance);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvokeListenersWhenDeletingProcessInstancesAsync()
	  public virtual void testInvokeListenersWhenDeletingProcessInstancesAsync()
	  {

		// given
		IncrementCounterListener.counter = 0;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = ProcessModels.newModel(ONE_TASK_PROCESS).startEvent().userTask().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(IncrementCounterListener).FullName).endEvent().done();

		migrationRule.deploy(instance);
		IList<string> processIds = startTestProcesses(1);

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processIds, TESTING_INSTANCE_DELETE);
		executeSeedJob(batch);
		executeBatchJobs(batch);

		// then
		assertThat(IncrementCounterListener.counter, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstancesAsyncWithListInDifferentDeployments()
	  public virtual void testDeleteProcessInstancesAsyncWithListInDifferentDeployments()
	  {
		// given
		ProcessDefinition sourceDefinition1 = testRule.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "ONE_TASK_PROCESS"));
		ProcessDefinition sourceDefinition2 = testRule.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "TWO_TASKS_PROCESS"));
		IList<string> processInstanceIds = createProcessInstances(sourceDefinition1, sourceDefinition2, 15, 10);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String firstDeploymentId = sourceDefinition1.getDeploymentId();
		string firstDeploymentId = sourceDefinition1.DeploymentId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String secondDeploymentId = sourceDefinition2.getDeploymentId();
		string secondDeploymentId = sourceDefinition2.DeploymentId;

		IList<string> processInstanceIdsFromFirstDeployment = getProcessInstanceIdsByDeploymentId(firstDeploymentId);
		IList<string> processInstanceIdsFromSecondDeployment = getProcessInstanceIdsByDeploymentId(secondDeploymentId);

		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 3;

		// when
		Batch batch = runtimeService.deleteProcessInstancesAsync(processInstanceIds, null, "test_reason");

		string seedJobDefinitionId = batch.SeedJobDefinitionId;
		// seed jobs
		int expectedSeedJobsCount = 5;
		createAndExecuteSeedJobs(seedJobDefinitionId, expectedSeedJobsCount);

		// then
		IList<Job> jobs = managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list();

		// execute jobs related to the first deployment
		IList<string> jobIdsForFirstDeployment = getJobIdsByDeployment(jobs, firstDeploymentId);
		assertNotNull(jobIdsForFirstDeployment);
		foreach (string jobId in jobIdsForFirstDeployment)
		{
		  managementService.executeJob(jobId);
		}

		// the process instances related to the first deployment should be deleted
		assertEquals(0, runtimeService.createProcessInstanceQuery().deploymentId(firstDeploymentId).count());
		assertHistoricTaskDeletionPresent(processInstanceIdsFromFirstDeployment, "test_reason", testRule);
		// and process instances related to the second deployment should not be deleted
		assertEquals(processInstanceIdsFromSecondDeployment.Count, runtimeService.createProcessInstanceQuery().deploymentId(secondDeploymentId).count());
		assertHistoricTaskDeletionPresent(processInstanceIdsFromSecondDeployment, null, testRule);

		// execute jobs related to the second deployment
		IList<string> jobIdsForSecondDeployment = getJobIdsByDeployment(jobs, secondDeploymentId);
		assertNotNull(jobIdsForSecondDeployment);
		foreach (string jobId in jobIdsForSecondDeployment)
		{
		  managementService.executeJob(jobId);
		}

		// all of the process instances should be deleted
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  private IList<string> createProcessInstances(ProcessDefinition sourceDefinition1, ProcessDefinition sourceDefinition2, int instanceCountDef1, int instanceCountDef2)
	  {
		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < instanceCountDef1; i++)
		{
		  ProcessInstance processInstance1 = runtimeService.startProcessInstanceById(sourceDefinition1.Id);
		  processInstanceIds.Add(processInstance1.Id);
		  if (i < instanceCountDef2)
		  {
			ProcessInstance processInstance2 = runtimeService.startProcessInstanceById(sourceDefinition2.Id);
			processInstanceIds.Add(processInstance2.Id);
		  }
		}
		return processInstanceIds;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private List<String> getProcessInstanceIdsByDeploymentId(final String deploymentId)
	  private IList<string> getProcessInstanceIdsByDeploymentId(string deploymentId)
	  {
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().deploymentId(deploymentId).list();
		IList<string> processInstanceIds = new List<string>();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  processInstanceIds.Add(processInstance.Id);
		}
		return processInstanceIds;
	  }

	  private IList<string> getJobIdsByDeployment(IList<Job> jobs, string deploymentId)
	  {
		IList<string> jobIdsForDeployment = new LinkedList<string>();
		for (int i = 0; i < jobs.Count; i++)
		{
		  if (jobs[i].DeploymentId.Equals(deploymentId))
		  {
			jobIdsForDeployment.Add(jobs[i].Id);
		  }
		}
		return jobIdsForDeployment;
	  }

	  private void createAndExecuteSeedJobs(string seedJobDefinitionId, int expectedSeedJobsCount)
	  {
		for (int i = 0; i <= expectedSeedJobsCount; i++)
		{
		  Job seedJob = managementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult();
		  if (i != expectedSeedJobsCount)
		  {
			assertNotNull(seedJob);
			managementService.executeJob(seedJob.Id);
		  }
		  else
		  {
			//the last seed job should not trigger another seed job
			assertNull(seedJob);
		  }
		}
	  }
	}

}