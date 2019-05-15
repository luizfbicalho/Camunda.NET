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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using SetProcessDefinitionVersionCmd = org.camunda.bpm.engine.impl.cmd.SetProcessDefinitionVersionCmd;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Falko Menge
	/// </summary>
	public class SetProcessDefinitionVersionCmdTest : PluggableProcessEngineTestCase
	{

	  private const string TEST_PROCESS_WITH_PARALLEL_GATEWAY = "org/camunda/bpm/engine/test/bpmn/gateway/ParallelGatewayTest.testForkJoin.bpmn20.xml";
	  private const string TEST_PROCESS = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersion.bpmn20.xml";
	  private const string TEST_PROCESS_ACTIVITY_MISSING = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionActivityMissing.bpmn20.xml";

	  private const string TEST_PROCESS_CALL_ACTIVITY = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.withCallActivity.bpmn20.xml";
	  private const string TEST_PROCESS_USER_TASK_V1 = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionWithTask.bpmn20.xml";
	  private const string TEST_PROCESS_USER_TASK_V2 = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionWithTaskV2.bpmn20.xml";

	  private const string TEST_PROCESS_SERVICE_TASK_V1 = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionWithServiceTask.bpmn20.xml";
	  private const string TEST_PROCESS_SERVICE_TASK_V2 = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionWithServiceTaskV2.bpmn20.xml";

	  private const string TEST_PROCESS_WITH_MULTIPLE_PARENTS = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testSetProcessDefinitionVersionWithMultipleParents.bpmn";

	  private const string TEST_PROCESS_ONE_JOB = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.oneJobProcess.bpmn20.xml";
	  private const string TEST_PROCESS_TWO_JOBS = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.twoJobsProcess.bpmn20.xml";

	  private const string TEST_PROCESS_ATTACHED_TIMER = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.testAttachedTimer.bpmn20.xml";

	  public virtual void testSetProcessDefinitionVersionEmptyArguments()
	  {
		try
		{
		  new SetProcessDefinitionVersionCmd(null, 23);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("The process instance id is mandatory: processInstanceId is null", ae.Message);
		}

		try
		{
		  new SetProcessDefinitionVersionCmd("", 23);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("The process instance id is mandatory: processInstanceId is empty", ae.Message);
		}

		try
		{
		  new SetProcessDefinitionVersionCmd("42", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("The process definition version is mandatory: processDefinitionVersion is null", ae.Message);
		}

		try
		{
		  new SetProcessDefinitionVersionCmd("42", -1);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("The process definition version must be positive: processDefinitionVersion is not greater than 0", ae.Message);
		}
	  }

	  public virtual void testSetProcessDefinitionVersionNonExistingPI()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		try
		{
		  commandExecutor.execute(new SetProcessDefinitionVersionCmd("42", 23));
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("No process instance found for id = '42'.", ae.Message);
		}
	  }

	  [Deployment(resources : {TEST_PROCESS_WITH_PARALLEL_GATEWAY})]
	  public virtual void testSetProcessDefinitionVersionPIIsSubExecution()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("forkJoin");

		Execution execution = runtimeService.createExecutionQuery().activityId("receivePayment").singleResult();
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		SetProcessDefinitionVersionCmd command = new SetProcessDefinitionVersionCmd(execution.Id, 1);
		try
		{
		  commandExecutor.execute(command);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("A process instance id is required, but the provided id '" + execution.Id + "' points to a child execution of process instance '" + pi.Id + "'. Please invoke the " + command.GetType().Name + " with a root execution id.", ae.Message);
		}
	  }

	  [Deployment(resources : {TEST_PROCESS})]
	  public virtual void testSetProcessDefinitionVersionNonExistingPD()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("receiveTask");

		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		try
		{
		  commandExecutor.execute(new SetProcessDefinitionVersionCmd(pi.Id, 23));
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("no processes deployed with key = 'receiveTask', version = '23'", ae.Message);
		}
	  }

	  [Deployment(resources : {TEST_PROCESS})]
	  public virtual void testSetProcessDefinitionVersionActivityMissing()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("receiveTask");

		// check that receive task has been reached
		Execution execution = runtimeService.createExecutionQuery().activityId("waitState1").singleResult();
		assertNotNull(execution);

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_ACTIVITY_MISSING).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		// migrate process instance to new process definition version
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		SetProcessDefinitionVersionCmd setProcessDefinitionVersionCmd = new SetProcessDefinitionVersionCmd(pi.Id, 2);
		try
		{
		  commandExecutor.execute(setProcessDefinitionVersionCmd);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("The new process definition (key = 'receiveTask') does not contain the current activity (id = 'waitState1') of the process instance (id = '", ae.Message);
		}

		// undeploy "manually" deployed process definition
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetProcessDefinitionVersion()
	  public virtual void testSetProcessDefinitionVersion()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("receiveTask");

		// check that receive task has been reached
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(pi.Id).activityId("waitState1").singleResult();
		assertNotNull(execution);

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		// migrate process instance to new process definition version
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

		// signal process instance
		runtimeService.signal(execution.Id);

		// check that the instance now uses the new process definition version
		ProcessDefinition newProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();
		pi = runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).singleResult();
		assertEquals(newProcessDefinition.Id, pi.ProcessDefinitionId);

		// check history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  HistoricProcessInstance historicPI = historyService.createHistoricProcessInstanceQuery().processInstanceId(pi.Id).singleResult();

	//      assertEquals(newProcessDefinition.getId(), historicPI.getProcessDefinitionId());
		}

		// undeploy "manually" deployed process definition
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : {TEST_PROCESS_WITH_PARALLEL_GATEWAY})]
	  public virtual void testSetProcessDefinitionVersionSubExecutions()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("forkJoin");

		// check that the user tasks have been reached
		assertEquals(2, taskService.createTaskQuery().count());

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_WITH_PARALLEL_GATEWAY).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		// migrate process instance to new process definition version
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

		// check that all executions of the instance now use the new process definition version
		ProcessDefinition newProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(pi.Id).list();
		foreach (Execution execution in executions)
		{
		  assertEquals(newProcessDefinition.Id, ((ExecutionEntity) execution).ProcessDefinitionId);
		}

		// undeploy "manually" deployed process definition
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : {TEST_PROCESS_CALL_ACTIVITY})]
	  public virtual void testSetProcessDefinitionVersionWithCallActivity()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("parentProcess");

		// check that receive task has been reached
		Execution execution = runtimeService.createExecutionQuery().activityId("waitState1").processDefinitionKey("childProcess").singleResult();
		assertNotNull(execution);

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_CALL_ACTIVITY).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().processDefinitionKey("parentProcess").count());

		// migrate process instance to new process definition version
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

		// signal process instance
		runtimeService.signal(execution.Id);

		// should be finished now
		assertEquals(0, runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).count());

		// undeploy "manually" deployed process definition
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : {TEST_PROCESS_USER_TASK_V1})]
	  public virtual void testSetProcessDefinitionVersionWithWithTask()
	  {
		try
		{
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("userTask");

		// check that user task has been reached
		assertEquals(1, taskService.createTaskQuery().processInstanceId(pi.Id).count());

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_USER_TASK_V2).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().processDefinitionKey("userTask").count());

		ProcessDefinition newProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("userTask").processDefinitionVersion(2).singleResult();

		// migrate process instance to new process definition version
		processEngineConfiguration.CommandExecutorTxRequired.execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

		// check UserTask
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals(newProcessDefinition.Id, task.ProcessDefinitionId);
		assertEquals("testFormKey", formService.getTaskFormData(task.Id).FormKey);

		// continue
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);

		// undeploy "manually" deployed process definition
		repositoryService.deleteDeployment(deployment.Id, true);
		}
		catch (Exception ex)
		{
		 Console.WriteLine(ex.ToString());
		 Console.Write(ex.StackTrace);
		}
	  }

	  [Deployment(resources : TEST_PROCESS_SERVICE_TASK_V1)]
	  public virtual void testSetProcessDefinitionVersionWithFollowUpTask()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_SERVICE_TASK_V2).deploy().Id;

		runtimeService.startProcessInstanceById(processDefinitionId);

		// execute job that triggers the migrating service task
		Job migrationJob = managementService.createJobQuery().singleResult();
		assertNotNull(migrationJob);

		managementService.executeJob(migrationJob.Id);

		Task followUpTask = taskService.createTaskQuery().singleResult();

		assertNotNull("Should have migrated to the new version and immediately executed the correct follow-up activity", followUpTask);

		repositoryService.deleteDeployment(secondDeploymentId, true);
	  }

	  [Deployment(resources : {TEST_PROCESS_WITH_MULTIPLE_PARENTS})]
	  public virtual void testSetProcessDefinitionVersionWithMultipleParents()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("multipleJoins");

		// check that the user tasks have been reached
		assertEquals(2, taskService.createTaskQuery().count());

		//finish task1
		Task task = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		taskService.complete(task.Id);

		//we have reached task4
		task = taskService.createTaskQuery().taskDefinitionKey("task4").singleResult();
		assertNotNull(task);

		//The timer job has been created
		Job job = managementService.createJobQuery().executionId(task.ExecutionId).singleResult();
		assertNotNull(job);

		// check there are 2 user tasks task4 and task2
		assertEquals(2, taskService.createTaskQuery().count());

		// deploy new version of the process definition
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_WITH_MULTIPLE_PARENTS).deploy();
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());

		// migrate process instance to new process definition version
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(pi.Id, 2));

		// check that all executions of the instance now use the new process definition version
		ProcessDefinition newProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2).singleResult();
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(pi.Id).list();
		foreach (Execution execution in executions)
		{
			assertEquals(newProcessDefinition.Id, ((ExecutionEntity) execution).ProcessDefinitionId);
		}

		// undeploy "manually" deployed process definition
		  repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : TEST_PROCESS_ONE_JOB)]
	  public virtual void testSetProcessDefinitionVersionMigrateJob()
	  {
		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneJobProcess");

		// with a job
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// and a second deployment of the process
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_ONE_JOB).deploy();

		ProcessDefinition newDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		assertNotNull(newDefinition);

		// when the process instance is migrated
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

		// then the the job should also be migrated
		Job migratedJob = managementService.createJobQuery().singleResult();
		assertNotNull(migratedJob);
		assertEquals(job.Id, migratedJob.Id);
		assertEquals(newDefinition.Id, migratedJob.ProcessDefinitionId);
		assertEquals(deployment.Id, migratedJob.DeploymentId);

		JobDefinition newJobDefinition = managementService.createJobDefinitionQuery().processDefinitionId(newDefinition.Id).singleResult();
		assertNotNull(newJobDefinition);
		assertEquals(newJobDefinition.Id, migratedJob.JobDefinitionId);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : TEST_PROCESS_TWO_JOBS)]
	  public virtual void testMigrateJobWithMultipleDefinitionsOnActivity()
	  {
		// given a process instance
		ProcessInstance asyncAfterInstance = runtimeService.startProcessInstanceByKey("twoJobsProcess");

		// with an async after job
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);
		Job asyncAfterJob = managementService.createJobQuery().singleResult();

		// and a process instance with an before after job
		ProcessInstance asyncBeforeInstance = runtimeService.startProcessInstanceByKey("twoJobsProcess");
		Job asyncBeforeJob = managementService.createJobQuery().processInstanceId(asyncBeforeInstance.Id).singleResult();

		// and a second deployment of the process
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_TWO_JOBS).deploy();

		ProcessDefinition newDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		assertNotNull(newDefinition);

		JobDefinition asnycBeforeJobDefinition = managementService.createJobDefinitionQuery().jobConfiguration(MessageJobDeclaration.ASYNC_BEFORE).processDefinitionId(newDefinition.Id).singleResult();
		JobDefinition asnycAfterJobDefinition = managementService.createJobDefinitionQuery().jobConfiguration(MessageJobDeclaration.ASYNC_AFTER).processDefinitionId(newDefinition.Id).singleResult();

		assertNotNull(asnycBeforeJobDefinition);
		assertNotNull(asnycAfterJobDefinition);

		// when the process instances are migrated
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(asyncBeforeInstance.Id, 2));
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(asyncAfterInstance.Id, 2));

		// then the the job's definition reference should also be migrated
		Job migratedAsyncBeforeJob = managementService.createJobQuery().processInstanceId(asyncBeforeInstance.Id).singleResult();
		assertEquals(asyncBeforeJob.Id, migratedAsyncBeforeJob.Id);
		assertNotNull(migratedAsyncBeforeJob);
		assertEquals(asnycBeforeJobDefinition.Id, migratedAsyncBeforeJob.JobDefinitionId);

		Job migratedAsyncAfterJob = managementService.createJobQuery().processInstanceId(asyncAfterInstance.Id).singleResult();
		assertEquals(asyncAfterJob.Id, migratedAsyncAfterJob.Id);
		assertNotNull(migratedAsyncAfterJob);
		assertEquals(asnycAfterJobDefinition.Id, migratedAsyncAfterJob.JobDefinitionId);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : TEST_PROCESS_ONE_JOB)]
	  public virtual void testSetProcessDefinitionVersionMigrateIncident()
	  {
		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneJobProcess", Variables.createVariables().putValue("shouldFail", true));

		// with a failed job
		executeAvailableJobs();

		// and an incident
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);

		// and a second deployment of the process
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_ONE_JOB).deploy();

		ProcessDefinition newDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		assertNotNull(newDefinition);

		// when the process instance is migrated
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

		// then the the incident should also be migrated
		Incident migratedIncident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(migratedIncident);
		assertEquals(newDefinition.Id, migratedIncident.ProcessDefinitionId);
		assertEquals(instance.Id, migratedIncident.ProcessInstanceId);
		assertEquals(instance.Id, migratedIncident.ExecutionId);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
	  [Deployment(resources : TEST_PROCESS_ONE_JOB)]
	  public virtual void testPreserveTimestampOnUpdatedIncident()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneJobProcess", Variables.createVariables().putValue("shouldFail", true));

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);

		DateTime timestamp = incident.IncidentTimestamp;

		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_ONE_JOB).deploy();

		ProcessDefinition newDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		assertNotNull(newDefinition);

		// when
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

		Incident migratedIncident = runtimeService.createIncidentQuery().singleResult();

		// then
		assertEquals(timestamp, migratedIncident.IncidentTimestamp);

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : TEST_PROCESS_ATTACHED_TIMER)]
	  public virtual void testSetProcessDefinitionVersionAttachedTimer()
	  {
		// given a process instance
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("attachedTimer");

		// and a second deployment of the process
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(TEST_PROCESS_ATTACHED_TIMER).deploy();

		ProcessDefinition newDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		assertNotNull(newDefinition);

		// when the process instance is migrated
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(instance.Id, 2));

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(newDefinition.Id, job.ProcessDefinitionId);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testHistoryOfSetProcessDefinitionVersionCmd()
	  {
		// given
		string resource = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.bpmn";

		// Deployments
		org.camunda.bpm.engine.repository.Deployment firstDeployment = repositoryService.createDeployment().addClasspathResource(resource).deploy();

		org.camunda.bpm.engine.repository.Deployment secondDeployment = repositoryService.createDeployment().addClasspathResource(resource).deploy();

		// Process definitions
		ProcessDefinition processDefinitionV1 = repositoryService.createProcessDefinitionQuery().deploymentId(firstDeployment.Id).singleResult();

		ProcessDefinition processDefinitionV2 = repositoryService.createProcessDefinitionQuery().deploymentId(secondDeployment.Id).singleResult();

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinitionV1.Id);

		// when
		setProcessDefinitionVersion(processInstance.Id, 2);

		// then
		ProcessInstance processInstanceAfterMigration = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals(processDefinitionV2.Id, processInstanceAfterMigration.ProcessDefinitionId);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		  assertEquals(processDefinitionV2.Id, historicProcessInstance.ProcessDefinitionId);
		}

		// Clean up the test
		repositoryService.deleteDeployment(firstDeployment.Id, true);
		repositoryService.deleteDeployment(secondDeployment.Id, true);
	  }

	  public virtual void testOpLogSetProcessDefinitionVersionCmd()
	  {
		// given
		try
		{
		  identityService.AuthenticatedUserId = "demo";
		  string resource = "org/camunda/bpm/engine/test/api/runtime/migration/SetProcessDefinitionVersionCmdTest.bpmn";

		  // Deployments
		  org.camunda.bpm.engine.repository.Deployment firstDeployment = repositoryService.createDeployment().addClasspathResource(resource).deploy();

		  org.camunda.bpm.engine.repository.Deployment secondDeployment = repositoryService.createDeployment().addClasspathResource(resource).deploy();

		  // Process definitions
		  ProcessDefinition processDefinitionV1 = repositoryService.createProcessDefinitionQuery().deploymentId(firstDeployment.Id).singleResult();

		  ProcessDefinition processDefinitionV2 = repositoryService.createProcessDefinitionQuery().deploymentId(secondDeployment.Id).singleResult();

		  // start process instance
		  ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinitionV1.Id);

		  // when
		  setProcessDefinitionVersion(processInstance.Id, 2);

		  // then
		  ProcessInstance processInstanceAfterMigration = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		  assertEquals(processDefinitionV2.Id, processInstanceAfterMigration.ProcessDefinitionId);

		  if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		  {
			IList<UserOperationLogEntry> userOperations = historyService.createUserOperationLogQuery().processInstanceId(processInstance.Id).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE).list();

			assertEquals(1, userOperations.Count);

			UserOperationLogEntry userOperationLogEntry = userOperations[0];
			assertEquals("processDefinitionVersion", userOperationLogEntry.Property);
			assertEquals("1", userOperationLogEntry.OrgValue);
			assertEquals("2", userOperationLogEntry.NewValue);
		  }

		  // Clean up the test
		  repositoryService.deleteDeployment(firstDeployment.Id, true);
		  repositoryService.deleteDeployment(secondDeployment.Id, true);
		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

	  protected internal virtual void setProcessDefinitionVersion(string processInstanceId, int newProcessDefinitionVersion)
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		commandExecutor.execute(new SetProcessDefinitionVersionCmd(processInstanceId, newProcessDefinitionVersion));
	  }
	}

}