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
namespace org.camunda.bpm.engine.test.standalone.db.entitymanager
{
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using DatabasePurgeReport = org.camunda.bpm.engine.impl.management.DatabasePurgeReport;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;
	using CachePurgeReport = org.camunda.bpm.engine.impl.persistence.deploy.cache.CachePurgeReport;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Task = org.camunda.bpm.engine.task.Task;
	using TestPermissions = org.camunda.bpm.engine.test.api.identity.TestPermissions;
	using TestResource = org.camunda.bpm.engine.test.api.identity.TestResource;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	using static org.camunda.bpm.engine.authorization.Authorization;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.test.TestHelper.assertAndEnsureCleanDbAndCache;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class PurgeDatabaseTest
	{

	  protected internal const string PROCESS_DEF_KEY = "test";
	  protected internal const string PROCESS_MODEL_NAME = "test.bpmn20.xml";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  private ProcessEngineConfigurationImpl processEngineConfiguration;
	  private string databaseTablePrefix;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		processEngineConfiguration.DbMetricsReporterActivate = true;
		databaseTablePrefix = processEngineConfiguration.DatabaseTablePrefix;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		processEngineConfiguration.DbMetricsReporterActivate = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurge()
	  public virtual void testPurge()
	  {
		// given data
		BpmnModelInstance test = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().endEvent().done();
		engineRule.RepositoryService.createDeployment().addModelInstance(PROCESS_MODEL_NAME, test).deploy();
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY);

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		managementService.purge();

		// then no more data exist
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeWithExistingProcessInstance()
	  public virtual void testPurgeWithExistingProcessInstance()
	  {
		//given process with variable and staying process instance in second user task
		BpmnModelInstance test = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().userTask().userTask().endEvent().done();
		engineRule.RepositoryService.createDeployment().addModelInstance(PROCESS_MODEL_NAME, test).deploy();

		VariableMap variables = Variables.createVariables();
		variables.put("key", "value");
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, variables);
		Task task = engineRule.TaskService.createTaskQuery().singleResult();
		engineRule.TaskService.complete(task.Id);

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		managementService.purge();

		// then no more data exist
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeWithAsyncProcessInstance()
	  public virtual void testPurgeWithAsyncProcessInstance()
	  {
		// given process with variable and async process instance
		BpmnModelInstance test = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().camundaAsyncBefore().userTask().userTask().endEvent().done();
		engineRule.RepositoryService.createDeployment().addModelInstance(PROCESS_MODEL_NAME, test).deploy();

		VariableMap variables = Variables.createVariables();
		variables.put("key", "value");
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, variables);
		Job job = engineRule.ManagementService.createJobQuery().singleResult();
		engineRule.ManagementService.executeJob(job.Id);
		Task task = engineRule.TaskService.createTaskQuery().singleResult();
		engineRule.TaskService.complete(task.Id);

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		managementService.purge();

		// then no more data exist
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeComplexProcess()
	  public virtual void testPurgeComplexProcess()
	  {
		// given complex process with authentication
		// process is executed two times
		// metrics are reported

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().camundaAsyncBefore().parallelGateway("parallel").serviceTask("external").camundaType("external").camundaTopic("external").boundaryEvent().message("message").moveToNode("parallel").serviceTask().camundaAsyncBefore().camundaExpression("${1/0}").moveToLastGateway().userTask().done();

		createAuthenticationData();
		engineRule.RepositoryService.createDeployment().addModelInstance(PROCESS_MODEL_NAME, modelInstance).deploy();

		executeComplexBpmnProcess(true);
		executeComplexBpmnProcess(false);

		processEngineConfiguration.DbMetricsReporter.reportNow();

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		PurgeReport purge = managementService.purge();

		// then database and cache are empty
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);

		// and report contains deleted data
		assertFalse(purge.Empty);
		CachePurgeReport cachePurgeReport = purge.CachePurgeReport;
		assertEquals(1, cachePurgeReport.getReportValue(CachePurgeReport.PROCESS_DEF_CACHE).Count);

		DatabasePurgeReport databasePurgeReport = purge.DatabasePurgeReport;
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_ID_TENANT_MEMBER"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_EVENT_SUBSCR"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_EXT_TASK"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_ID_MEMBERSHIP"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_TASK"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_JOB"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_GE_BYTEARRAY"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_JOBDEF"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_ID_USER"));
		assertEquals(5, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_EXECUTION"));
		assertEquals(10, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_METER_LOG"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_VARIABLE"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_PROCDEF"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_ID_TENANT"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_ID_GROUP"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_AUTHORIZATION"));

		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_INCIDENT"));
		  assertEquals(9, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_ACTINST"));
		  assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_PROCINST"));
		  assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_DETAIL"));
		  assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_TASKINST"));
		  assertEquals(7, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_JOB_LOG"));
		  assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_VARINST"));
		  assertEquals(8, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_OP_LOG"));
		}

	  }

	  private void createAuthenticationData()
	  {
		IdentityService identityService = engineRule.IdentityService;
		Group group = identityService.newGroup("group");
		identityService.saveGroup(group);
		User user = identityService.newUser("user");
		User user2 = identityService.newUser("user2");
		identityService.saveUser(user);
		identityService.saveUser(user2);
		Tenant tenant = identityService.newTenant("tenant");
		identityService.saveTenant(tenant);
		Tenant tenant2 = identityService.newTenant("tenant2");
		identityService.saveTenant(tenant2);
		identityService.createMembership("user", "group");
		identityService.createTenantUserMembership("tenant", "user");
		identityService.createTenantUserMembership("tenant2", "user2");


		Resource resource1 = TestResource.RESOURCE1;
		// create global authorization which grants all permissions to all users (on resource1):
		AuthorizationService authorizationService = engineRule.AuthorizationService;
		Authorization globalAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalAuth.Resource = resource1;
		globalAuth.ResourceId = ANY;
		globalAuth.addPermission(TestPermissions.ALL);
		authorizationService.saveAuthorization(globalAuth);

		//grant user read auth on resource2
		Resource resource2 = TestResource.RESOURCE2;
		Authorization userGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		userGrant.UserId = "user";
		userGrant.Resource = resource2;
		userGrant.ResourceId = ANY;
		userGrant.addPermission(TestPermissions.READ);
		authorizationService.saveAuthorization(userGrant);

		identityService.AuthenticatedUserId = "user";
	  }

	  private void executeComplexBpmnProcess(bool complete)
	  {
		VariableMap variables = Variables.createVariables();
		variables.put("key", "value");
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, variables);
		//execute start event
		Job job = engineRule.ManagementService.createJobQuery().singleResult();
		engineRule.ManagementService.executeJob(job.Id);

		//fetch tasks and jobs
		IList<LockedExternalTask> externalTasks = engineRule.ExternalTaskService.fetchAndLock(1, "worker").topic("external", 1500).execute();
		job = engineRule.ManagementService.createJobQuery().singleResult();
		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		//complete
		if (complete)
		{
		  engineRule.ManagementService.setJobRetries(job.Id, 0);
		  engineRule.ManagementService.executeJob(job.Id);
		  engineRule.ExternalTaskService.complete(externalTasks[0].Id, "worker");
		  engineRule.TaskService.complete(task.Id);
		}
	  }

	  // CMMN //////////////////////////////////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeCmmnProcess()
	  public virtual void testPurgeCmmnProcess()
	  {
		// given cmmn process which is not managed by process engine rule

		engineRule.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/standalone/db/entitymanager/PurgeDatabaseTest.testPurgeCmmnProcess.cmmn").deploy();
		VariableMap variables = Variables.createVariables();
		variables.put("key", "value");
		engineRule.CaseService.createCaseInstanceByKey(PROCESS_DEF_KEY, variables);

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		PurgeReport purge = managementService.purge();

		// then database and cache is cleaned
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);

		// and report contains deleted entities
		assertFalse(purge.Empty);
		CachePurgeReport cachePurgeReport = purge.CachePurgeReport;
		assertEquals(1, cachePurgeReport.getReportValue(CachePurgeReport.CASE_DEF_CACHE).Count);

		DatabasePurgeReport databasePurgeReport = purge.DatabasePurgeReport;
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_TASK"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_GE_BYTEARRAY"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_CASE_DEF"));
		assertEquals(3, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_CASE_EXECUTION"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_VARIABLE"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RU_CASE_SENTRY_PART"));

		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_DETAIL"));
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_TASKINST"));
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_VARINST"));
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_CASEINST"));
		  assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_CASEACTINST"));
		}
	  }

	  // DMN ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPurgeDmnProcess()
	  public virtual void testPurgeDmnProcess()
	  {
		// given dmn process which is not managed by process engine rule
		engineRule.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/standalone/db/entitymanager/PurgeDatabaseTest.testPurgeDmnProcess.dmn").deploy();
		VariableMap variables = Variables.createVariables().putValue("key", "value").putValue("season", "Test");
		engineRule.DecisionService.evaluateDecisionByKey("decisionId").variables(variables).evaluate();

		// when purge is executed
		ManagementServiceImpl managementService = (ManagementServiceImpl) engineRule.ManagementService;
		PurgeReport purge = managementService.purge();

		// then database and cache is cleaned
		assertAndEnsureCleanDbAndCache(engineRule.ProcessEngine, true);

		// and report contains deleted entities
		assertFalse(purge.Empty);
		CachePurgeReport cachePurgeReport = purge.CachePurgeReport;
		assertEquals(2, cachePurgeReport.getReportValue(CachePurgeReport.DMN_DEF_CACHE).Count);
		assertEquals(1, cachePurgeReport.getReportValue(CachePurgeReport.DMN_REQ_DEF_CACHE).Count);

		DatabasePurgeReport databasePurgeReport = purge.DatabasePurgeReport;
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_GE_BYTEARRAY"));
		assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_DECISION_REQ_DEF"));
		assertEquals(2, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_RE_DECISION_DEF"));

		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_DECINST"));
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_DEC_IN"));
		  assertEquals(1, (long) databasePurgeReport.getReportValue(databaseTablePrefix + "ACT_HI_DEC_OUT"));
		}
	  }
	}

}