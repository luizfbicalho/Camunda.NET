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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using TaskPermissions = org.camunda.bpm.engine.authorization.TaskPermissions;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationTestBaseRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestBaseRule;
	using TestPermissions = org.camunda.bpm.engine.test.api.identity.TestPermissions;
	using TestResource = org.camunda.bpm.engine.test.api.identity.TestResource;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class DisabledPermissionsAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DisabledPermissionsAuthorizationTest()
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
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string USER_ID = "user";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestBaseRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException exceptionRule = org.junit.rules.ExpectedException.none();
	  public ExpectedException exceptionRule = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

	  internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  internal RepositoryService repositoryService;
	  internal AuthorizationService authorizationService;
	  internal RuntimeService runtimeService;
	  internal ManagementService managementService;
	  internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup(USER_ID, "group");
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		authorizationService = engineRule.AuthorizationService;
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.disableAuthorization();
		authRule.deleteUsersAndGroups();
		processEngineConfiguration.DisabledPermissions = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedForIgnoredPermission()
	  public virtual void testIsUserAuthorizedForIgnoredPermission()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(READ.name());

		authRule.createGrantAuthorization(PROCESS_INSTANCE, ANY, USER_ID, ProcessInstancePermissions.RETRY_JOB);

		authRule.enableAuthorization(USER_ID);

		// expected exception
		exceptionRule.expect(typeof(BadUserRequestException));
		exceptionRule.expectMessage("The 'READ' permission is disabled, please check your process engine configuration.");

		// when
		authorizationService.isUserAuthorized(USER_ID, null, READ, PROCESS_DEFINITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomPermissionDuplicateValue()
	  public virtual void testCustomPermissionDuplicateValue()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(ProcessInstancePermissions.SUSPEND.name());
		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;

		// assume
		assertEquals(ProcessInstancePermissions.SUSPEND.Value, TestPermissions.RANDOM.Value);

		// when
		authRule.createGrantAuthorization(resource1, ANY, USER_ID, TestPermissions.RANDOM);
		authRule.createGrantAuthorization(resource2, "resource2-1", USER_ID, TestPermissions.RANDOM);
		authRule.enableAuthorization(USER_ID);

		// then
		// verify that the custom permission with the same value is not affected by disabling the build-in permission
		assertEquals(true, authorizationService.isUserAuthorized(USER_ID, null, TestPermissions.RANDOM, resource1));
		assertEquals(true, authorizationService.isUserAuthorized(USER_ID, null, TestPermissions.RANDOM, resource2, "resource2-1"));
	  }

	  // specific scenarios //////////////////////////////////////
	  // the next tests cover different combination in the authorization checks
	  // i.e. the query doesn't fail if all permissions are disabled in the specific authorization check

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableIgnoreTaskRead()
	  public virtual void testGetVariableIgnoreTaskRead()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(TaskPermissions.READ.name());
		string taskId = "taskId";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		taskService.setVariables(taskId, Variables.createVariables().putValue("foo", "bar"));
		authRule.enableAuthorization(USER_ID);

		// when
		object variable = taskService.getVariable(taskId, "foo");

		// then
		assertEquals("bar", variable);
		authRule.disableAuthorization();
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryTaskIgnoreTaskRead()
	  public virtual void testQueryTaskIgnoreTaskRead()
	  {
		// given
		IList<string> permissions = new List<string>();
		permissions.Add(TaskPermissions.READ.name());
		permissions.Add(ProcessDefinitionPermissions.READ_TASK.name());
		processEngineConfiguration.DisabledPermissions = permissions;
		string taskId = "taskId";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		authRule.enableAuthorization(USER_ID);

		// when
		Task returnedTask = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(returnedTask);
		authRule.disableAuthorization();
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testDeleteHistoricProcessInstanceIgnoreDeleteHistory()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT), Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDeleteHistoricProcessInstanceIgnoreDeleteHistory()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(Permissions.DELETE_HISTORY.name());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.deleteProcessInstance(processInstance.Id, "any");
		authRule.enableAuthorization(USER_ID);

		engineRule.HistoryService.deleteHistoricProcessInstance(processInstance.Id);
		authRule.disableAuthorization();
		assertNull(engineRule.HistoryService.createHistoricProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testQueryDeploymentIgnoreRead()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testQueryDeploymentIgnoreRead()
	  {
		// given
		engineRule.ProcessEngineConfiguration.DisabledPermissions = Arrays.asList(READ.name());

		// when
		authRule.enableAuthorization(USER_ID);
		IList<org.camunda.bpm.engine.repository.Deployment> deployments = engineRule.RepositoryService.createDeploymentQuery().list();

		// then
		assertEquals(1, deployments.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testStartableInTasklistIgnoreRead()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testStartableInTasklistIgnoreRead()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(READ.name());
		authRule.createGrantAuthorization(PROCESS_DEFINITION, "oneTaskProcess", USER_ID, CREATE_INSTANCE);
		authRule.createGrantAuthorization(PROCESS_INSTANCE, "*", USER_ID, CREATE);

		authRule.disableAuthorization();
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();
		authRule.enableAuthorization(USER_ID);

		// when
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().list();
		// then
		assertNotNull(processDefinitions);
		assertEquals(1, repositoryService.createProcessDefinitionQuery().startablePermissionCheck().startableInTasklist().count());
		assertEquals(definition.Id, processDefinitions[0].Id);
		assertTrue(processDefinitions[0].StartableInTasklist);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml") public void testDeploymentStatisticsIgnoreReadInstance()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml")]
	  public virtual void testDeploymentStatisticsIgnoreReadInstance()
	  {
		// given
		processEngineConfiguration.DisabledPermissions = Arrays.asList(READ_INSTANCE.name());

		runtimeService.startProcessInstanceByKey("timerBoundaryProcess");

		authRule.enableAuthorization(USER_ID);

		// when
		DeploymentStatisticsQuery query = engineRule.ManagementService.createDeploymentStatisticsQuery();

		// then
		IList<DeploymentStatistics> statistics = query.list();

		foreach (DeploymentStatistics deploymentStatistics in statistics)
		{
		  assertEquals("Instances", 1, deploymentStatistics.Instances);
		  assertEquals("Failed Jobs", 0, deploymentStatistics.FailedJobs);

		  IList<IncidentStatistics> incidentStatistics = deploymentStatistics.IncidentStatistics;
		  assertTrue("Incidents supposed to be empty", incidentStatistics.Count == 0);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml") public void testActivityStatisticsIgnoreRead()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml")]
	  public virtual void testActivityStatisticsIgnoreRead()
	  {
		// given
		IList<string> permissions = new List<string>();
		permissions.Add(READ.name());
		permissions.Add(READ_INSTANCE.name());
		processEngineConfiguration.DisabledPermissions = permissions;
		string processDefinitionId = runtimeService.startProcessInstanceByKey("timerBoundaryProcess").ProcessDefinitionId;

		authRule.enableAuthorization(USER_ID);

		// when
		ActivityStatistics statistics = managementService.createActivityStatisticsQuery(processDefinitionId).singleResult();

		// then
		assertNotNull(statistics);
		assertEquals("task", statistics.Id);
		assertEquals(1, statistics.Instances);
		assertEquals(0, statistics.FailedJobs);
		assertTrue(statistics.IncidentStatistics.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-9888") @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testFetchAndLockIgnoreRead()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchAndLockIgnoreRead()
	  {
		// given
		IList<string> permissions = new List<string>();
		permissions.Add(READ.name());
		permissions.Add(READ_INSTANCE.name());
		processEngineConfiguration.DisabledPermissions = permissions;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		authRule.createGrantAuthorization(PROCESS_INSTANCE, "*", USER_ID, UPDATE);

		authRule.enableAuthorization(USER_ID);

		// when
		IList<LockedExternalTask> externalTasks = engineRule.ExternalTaskService.fetchAndLock(1, "aWorkerId").topic("externalTaskTopic", 10000L).execute();

		// then
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		assertNotNull(task.Id);
		assertEquals(processInstance.Id, task.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
		assertEquals("externalTask", task.ActivityId);
		assertEquals("oneExternalTaskProcess", task.ProcessDefinitionKey);
	  }

	  protected internal virtual void startProcessAndExecuteJob(string processDefinitionKey)
	  {
		runtimeService.startProcessInstanceByKey(processDefinitionKey);
		executeAvailableJobs(processDefinitionKey);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void executeAvailableJobs(final String key)
	  protected internal virtual void executeAvailableJobs(string key)
	  {
		IList<Job> jobs = managementService.createJobQuery().processDefinitionKey(key).withRetriesLeft().list();

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

		executeAvailableJobs(key);
		return;
	  }

	}

}