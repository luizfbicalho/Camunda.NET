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
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using DelegateEvent = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateEvent;
	using DelegateExecutionListener = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateExecutionListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.startsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ModificationExecutionAsyncTest
	public class ModificationExecutionAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public ModificationExecutionAsyncTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(rule);
			helper = new BatchModificationHelper(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchModificationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;

	  protected internal BpmnModelInstance instance;

	  private int defaultBatchJobsPerSeed;
	  private int defaultInvocationsPerBatchJob;
	  private bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public boolean ensureJobDueDateSet;
	  public bool ensureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public java.util.Date currentTime;
	  public DateTime currentTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Job DueDate is set: {0}") public static java.util.Collection<Object[]> scenarios() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {false, null},
			new object[] {true, START_DATE}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockUtil.CurrentTime = START_DATE;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		configuration = rule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
		defaultEnsureJobDueDateSet = configuration.EnsureJobDueDateNotNull;
		configuration.EnsureJobDueDateNotNull = ensureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createBpmnModelInstance()
	  public virtual void createBpmnModelInstance()
	  {
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").userTask("user1").sequenceFlowId("seq").userTask("user2").endEvent("end").done();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreEngineSettings()
	  public virtual void restoreEngineSettings()
	  {
		configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
		configuration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeInstanceIds()
	  public virtual void removeInstanceIds()
	  {
		helper.currentProcessInstances = new List<>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchModification()
	  public virtual void createBatchModification()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		Batch batch = runtimeService.createModification(processDefinition.Id).startAfterActivity("user2").processInstanceIds(processInstanceIds).executeAsync();

		assertBatchCreated(batch, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessInstanceIdsListAsync()
	  public virtual void createModificationWithNullProcessInstanceIdsListAsync()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds((IList<string>) null).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessDefinitionId()
	  public virtual void createModificationWithNullProcessDefinitionId()
	  {
		try
		{
		  runtimeService.createModification(null).cancelAllForActivity("activityId").processInstanceIds(Arrays.asList("20", "1--0")).executeAsync();
		  fail("Should not succed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("processDefinitionId is null"));
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationUsingProcessInstanceIdsListWithNullValueAsync()
	  public virtual void createModificationUsingProcessInstanceIdsListWithNullValueAsync()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds(Arrays.asList("foo", null, "bar")).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithEmptyProcessInstanceIdsListAsync()
	  public virtual void createModificationWithEmptyProcessInstanceIdsListAsync()
	  {
		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds(System.Linq.Enumerable.Empty<string> ()).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessInstanceIdsArrayAsync()
	  public virtual void createModificationWithNullProcessInstanceIdsArrayAsync()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds((string[]) null).executeAsync();
		  fail("Should not be able to modify");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationUsingProcessInstanceIdsArrayWithNullValueAsync()
	  public virtual void createModificationUsingProcessInstanceIdsArrayWithNullValueAsync()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").cancelAllForActivity("user1").processInstanceIds("foo", null, "bar").executeAsync();
		  fail("Should not be able to modify");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceQueryAsync()
	  public virtual void testNullProcessInstanceQueryAsync()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceQuery(null).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNonExistingProcessDefinitionId()
	  public virtual void createModificationWithNonExistingProcessDefinitionId()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);
		try
		{
		  runtimeService.createModification("foo").cancelAllForActivity("activityId").processInstanceIds(processInstanceIds).executeAsync();
		  fail("Should not succed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("processDefinition is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createSeedJob()
	  public virtual void createSeedJob()
	  {
		// when
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startAfterAsync("process1", 3, "user1", processDefinition.Id);

		// then there exists a seed job definition with the batch id as
		// configuration
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);
		assertEquals(batch.Id, seedJobDefinition.JobConfiguration);
		assertEquals(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

		// and there exists a modification job definition
		JobDefinition modificationJobDefinition = helper.getExecutionJobDefinition(batch);
		assertNotNull(modificationJobDefinition);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION, modificationJobDefinition.JobType);

		// and a seed job with no relation to a process or execution etc.
		Job seedJob = helper.getSeedJob(batch);
		assertNotNull(seedJob);
		assertEquals(seedJobDefinition.Id, seedJob.JobDefinitionId);
		assertEquals(currentTime, seedJob.Duedate);
		assertNull(seedJob.DeploymentId);
		assertNull(seedJob.ProcessDefinitionId);
		assertNull(seedJob.ProcessDefinitionKey);
		assertNull(seedJob.ProcessInstanceId);
		assertNull(seedJob.ExecutionId);

		// but no modification jobs where created
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);
		assertEquals(0, modificationJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationJobs()
	  public virtual void createModificationJobs()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		rule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;
		Batch batch = helper.startAfterAsync("process1", 20, "user1", processDefinition.Id);
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		JobDefinition modificationJobDefinition = helper.getExecutionJobDefinition(batch);

		helper.executeSeedJob(batch);

		IList<Job> modificationJobs = helper.getJobsForDefinition(modificationJobDefinition);
		assertEquals(10, modificationJobs.Count);

		foreach (Job modificationJob in modificationJobs)
		{
		  assertEquals(modificationJobDefinition.Id, modificationJob.JobDefinitionId);
		  assertEquals(currentTime, modificationJob.Duedate);
		  assertNull(modificationJob.ProcessDefinitionId);
		  assertNull(modificationJob.ProcessDefinitionKey);
		  assertNull(modificationJob.ProcessInstanceId);
		  assertNull(modificationJob.ExecutionId);
		}

		// and the seed job still exists
		Job seedJob = helper.getJobForDefinition(seedJobDefinition);
		assertNotNull(seedJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createMonitorJob()
	  public virtual void createMonitorJob()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startAfterAsync("process1", 10, "user1", processDefinition.Id);

		// when
		helper.executeSeedJob(batch);

		// then the seed job definition still exists but the seed job is removed
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);

		Job seedJob = helper.getSeedJob(batch);
		assertNull(seedJob);

		// and a monitor job definition and job exists
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertNotNull(monitorJobDefinition);

		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartAfter()
	  public virtual void executeModificationJobsForStartAfter()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		Batch batch = helper.startAfterAsync("process1", 10, "user1", processDefinition.Id);
		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartBefore()
	  public virtual void executeModificationJobsForStartBefore()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		Batch batch = helper.startBeforeAsync("process1", 10, "user2", processDefinition.Id);
		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartTransition()
	  public virtual void executeModificationJobsForStartTransition()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		Batch batch = helper.startTransitionAsync("process1", 10, "seq", processDefinition.Id);
		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForCancelAll()
	  public virtual void executeModificationJobsForCancelAll()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.cancelAllAsync("process1", 10, "user1", processDefinition.Id);
		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNull(updatedTree);
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartAfterAndCancelAll()
	  public virtual void executeModificationJobsForStartAfterAndCancelAll()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];
		IList<string> instances = helper.startInstances("process1", 10);

		Batch batch = runtimeService.createModification(processDefinition.Id).startAfterActivity("user1").cancelAllForActivity("user1").processInstanceIds(instances).executeAsync();

		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user2").done());
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartBeforeAndCancelAll()
	  public virtual void executeModificationJobsForStartBeforeAndCancelAll()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		IList<string> instances = helper.startInstances("process1", 10);

		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("user1").cancelAllForActivity("user1").processInstanceIds(instances).executeAsync();

		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNull(updatedTree);
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForStartTransitionAndCancelAll()
	  public virtual void executeModificationJobsForStartTransitionAndCancelAll()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		IList<string> instances = helper.startInstances("process1", 10);

		Batch batch = runtimeService.createModification(processDefinition.Id).startTransition("seq").cancelAllForActivity("user1").processInstanceIds(instances).executeAsync();

		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user2").done());
		}

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationJobsForProcessInstancesWithDifferentStates()
	  public virtual void executeModificationJobsForProcessInstancesWithDifferentStates()
	  {

		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 1);
		Task task = rule.TaskService.createTaskQuery().singleResult();
		rule.TaskService.complete(task.Id);

		IList<string> anotherProcessInstanceIds = helper.startInstances("process1", 1);
		((IList<string>)processInstanceIds).AddRange(anotherProcessInstanceIds);

		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("user2").processInstanceIds(processInstanceIds).executeAsync();

		helper.executeSeedJob(batch);
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job modificationJob in modificationJobs)
		{
		  helper.executeJob(modificationJob);
		}

		// then all process instances where modified
		ActivityInstance updatedTree = null;
		string processInstanceId = processInstanceIds[0];
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user2").activity("user2").done());

		processInstanceId = processInstanceIds[1];
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());

		// and the no modification jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobPollingForCompletion()
	  public virtual void testMonitorJobPollingForCompletion()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startAfterAsync("process1", 3, "user1", processDefinition.Id);

		// when the seed job creates the monitor job
		DateTime createDate = START_DATE;
		helper.executeSeedJob(batch);

		// then the monitor job has a no due date set
		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
		assertEquals(currentTime, monitorJob.Duedate);

		// when the monitor job is executed
		helper.executeMonitorJob(batch);

		// then the monitor job has a due date of the default batch poll time
		monitorJob = helper.getMonitorJob(batch);
		DateTime dueDate = helper.addSeconds(createDate, 30);
		assertEquals(dueDate, monitorJob.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobRemovesBatchAfterCompletion()
	  public virtual void testMonitorJobRemovesBatchAfterCompletion()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startBeforeAsync("process1", 10, "user2", processDefinition.Id);
		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// when
		helper.executeMonitorJob(batch);

		// then the batch was completed and removed
		assertEquals(0, rule.ManagementService.createBatchQuery().count());

		// and the seed jobs was removed
		assertEquals(0, rule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithCascade()
	  public virtual void testBatchDeletionWithCascade()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startTransitionAsync("process1", 10, "seq", processDefinition.Id);
		helper.executeSeedJob(batch);

		// when
		rule.ManagementService.deleteBatch(batch.Id, true);

		// then the batch was deleted
		assertEquals(0, rule.ManagementService.createBatchQuery().count());

		// and the seed and modification job definition were deleted
		assertEquals(0, rule.ManagementService.createJobDefinitionQuery().count());

		// and the seed job and modification jobs were deleted
		assertEquals(0, rule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithoutCascade()
	  public virtual void testBatchDeletionWithoutCascade()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startBeforeAsync("process1", 10, "user2", processDefinition.Id);
		helper.executeSeedJob(batch);

		// when
		rule.ManagementService.deleteBatch(batch.Id, false);

		// then the batch was deleted
		assertEquals(0, rule.ManagementService.createBatchQuery().count());

		// and the seed and modification job definition were deleted
		assertEquals(0, rule.ManagementService.createJobDefinitionQuery().count());

		// and the seed job and modification jobs were deleted
		assertEquals(0, rule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedSeedJobDeletionWithCascade()
	  public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.cancelAllAsync("process1", 2, "user1", processDefinition.Id);

		// create incident
		Job seedJob = helper.getSeedJob(batch);
		rule.ManagementService.setJobRetries(seedJob.Id, 0);

		// when
		rule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = rule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedModificationJobDeletionWithCascade()
	  public virtual void testBatchWithFailedModificationJobDeletionWithCascade()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startAfterAsync("process1", 2, "user1", processDefinition.Id);
		helper.executeSeedJob(batch);

		// create incidents
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);
		foreach (Job modificationJob in modificationJobs)
		{
		  rule.ManagementService.setJobRetries(modificationJob.Id, 0);
		}

		// when
		rule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = rule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedMonitorJobDeletionWithCascade()
	  public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch batch = helper.startBeforeAsync("process1", 2, "user2", processDefinition.Id);
		helper.executeSeedJob(batch);

		// create incident
		Job monitorJob = helper.getMonitorJob(batch);
		rule.ManagementService.setJobRetries(monitorJob.Id, 0);

		// when
		rule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = rule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testModificationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  public virtual void testModificationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = rule.ProcessEngineConfiguration;

		processEngineConfiguration.AuthorizationEnabled = true;
		ProcessDefinition processDefinition = testRule.deployForTenantAndGetDefinition("tenantId", instance);

		try
		{
		  Batch batch = helper.startAfterAsync("process1", 10, "user1", processDefinition.Id);
		  helper.executeSeedJob(batch);

		  testRule.executeAvailableJobs();

		  // then all process instances where modified
		  foreach (string processInstanceId in helper.currentProcessInstances)
		  {
			ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
			assertNotNull(updatedTree);
			assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

			assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());
		  }

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchExecutionFailureWithMissingProcessInstance()
	  public virtual void testBatchExecutionFailureWithMissingProcessInstance()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];
		Batch batch = helper.startAfterAsync("process1", 2, "user1", processDefinition.Id);
		helper.executeSeedJob(batch);

		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		string deletedProcessInstanceId = processInstances[0].Id;

		// when
		runtimeService.deleteProcessInstance(deletedProcessInstanceId, "test");
		helper.executeJobs(batch);

		// then the remaining process instance was modified
		foreach (string processInstanceId in helper.currentProcessInstances)
		{
		  if (processInstanceId.Equals(helper.currentProcessInstances[0]))
		  {
			ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
			assertNull(updatedTree);
			continue;
		  }

		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("user1").activity("user2").done());
		}

		// and one batch job failed and has 2 retries left
		IList<Job> modificationJobs = helper.getExecutionJobs(batch);
		assertEquals(1, modificationJobs.Count);

		Job failedJob = modificationJobs[0];
		assertEquals(2, failedJob.Retries);
		assertThat(failedJob.ExceptionMessage, startsWith("ENGINE-13036"));
		assertThat(failedJob.ExceptionMessage, containsString("Process instance '" + deletedProcessInstanceId + "' cannot be modified"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreationWithProcessInstanceQuery()
	  public virtual void testBatchCreationWithProcessInstanceQuery()
	  {
		int processInstanceCount = 15;
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];
		helper.startInstances("process1", 15);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id);
		assertEquals(processInstanceCount, processInstanceQuery.count());

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startAfterActivity("user1").processInstanceQuery(processInstanceQuery).executeAsync();

		// then a batch is created
		assertBatchCreated(batch, processInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  {
		int processInstanceCount = 15;
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition processDefinition = deployment.DeployedProcessDefinitions[0];
		IList<string> processInstanceIds = helper.startInstances("process1", 15);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id);
		assertEquals(processInstanceCount, processInstanceQuery.count());

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startTransition("seq").processInstanceIds(processInstanceIds).processInstanceQuery(processInstanceQuery).executeAsync();

		// then a batch is created
		assertBatchCreated(batch, processInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListenerInvocation()
	  public virtual void testListenerInvocation()
	  {
		// given
		DelegateEvent.clearEvents();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(instance).activityBuilder("user2").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("user2").processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(1, recordedEvents.Count);

		DelegateEvent @event = recordedEvents[0];
		assertEquals(processDefinition.Id, @event.ProcessDefinitionId);
		assertEquals("user2", @event.CurrentActivityId);

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipListenerInvocationF()
	  public virtual void testSkipListenerInvocationF()
	  {
		// given
		DelegateEvent.clearEvents();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(instance).activityBuilder("user2").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Batch batch = runtimeService.createModification(processDefinition.Id).cancelAllForActivity("user2").processInstanceIds(Arrays.asList(processInstance.Id)).skipCustomListeners().executeAsync();

		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		assertEquals(0, DelegateEvent.Events.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIoMappingInvocation()
	  public virtual void testIoMappingInvocation()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(instance).activityBuilder("user1").camundaInputParameter("foo", "bar").done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Batch batch = runtimeService.createModification(processDefinition.Id).startAfterActivity("user2").processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		VariableInstance inputVariable = runtimeService.createVariableInstanceQuery().singleResult();
		Assert.assertNotNull(inputVariable);
		assertEquals("foo", inputVariable.Name);
		assertEquals("bar", inputVariable.Value);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(activityInstance.getActivityInstances("user1")[0].Id, inputVariable.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipIoMappingInvocation()
	  public virtual void testSkipIoMappingInvocation()
	  {
		// given

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(instance).activityBuilder("user2").camundaInputParameter("foo", "bar").done());


		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("user2").processInstanceIds(Arrays.asList(processInstance.Id)).skipIoMappings().executeAsync();

		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithoutFlag()
	  public virtual void testCancelWithoutFlag()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user").processInstanceIds(processInstanceIds).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// then
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithoutFlag2()
	  public virtual void testCancelWithoutFlag2()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", false).processInstanceIds(processInstanceIds).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// then
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithFlag()
	  public virtual void testCancelWithFlag()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", true).processInstanceIds(processInstanceIds).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// then
		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().singleResult();
		assertNotNull(execution);
		assertEquals("user", execution.ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithFlagForManyInstances()
	  public virtual void testCancelWithFlagForManyInstances()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 10);

		// when
		Batch batch = runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", true).processInstanceIds(processInstanceIds).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// then
		foreach (string processInstanceId in processInstanceIds)
		{
		  Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstanceId).singleResult();
		  assertNotNull(execution);
		  assertEquals("user", ((ExecutionEntity) execution).ActivityId);
		}
	  }

	  protected internal virtual void assertBatchCreated(Batch batch, int processInstanceCount)
	  {
		assertNotNull(batch);
		assertNotNull(batch.Id);
		assertEquals("instance-modification", batch.Type);
		assertEquals(processInstanceCount, batch.TotalJobs);
		assertEquals(defaultBatchJobsPerSeed, batch.BatchJobsPerSeed);
		assertEquals(defaultInvocationsPerBatchJob, batch.InvocationsPerBatchJob);
	  }

	}

}