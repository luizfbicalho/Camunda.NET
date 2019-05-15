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
namespace org.camunda.bpm.engine.test.api.multitenancy.query.history
{
	using org.camunda.bpm.engine;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.DEFAULT_PROCESS_KEY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.DEFAULT_TOPIC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricExternalTaskLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricExternalTaskLogTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal readonly string TENANT_ONE = "tenant1";
	  protected internal readonly string TENANT_TWO = "tenant2";

	  protected internal readonly string WORKER_ID = "aWorkerId";
	  protected internal readonly string ERROR_DETAILS = "These are the error details!";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		runtimeService = engineRule.RuntimeService;
		identityService = engineRule.IdentityService;
		externalTaskService = engineRule.ExternalTaskService;

		testRule.deployForTenant(TENANT_ONE, ONE_EXTERNAL_TASK_PROCESS);
		testRule.deployForTenant(TENANT_TWO, ONE_EXTERNAL_TASK_PROCESS);

		startProcessInstanceAndFailExternalTask(TENANT_ONE);
		startProcessInstanceFailAndCompleteExternalTask(TENANT_TWO);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutTenantId()
	  public virtual void testQueryWithoutTenantId()
	  {

		//given two process with different tenants

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		assertThat(query.count(), @is(5L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {

		// given two process with different tenants

		// when
		HistoricExternalTaskLogQuery queryTenant1 = historyService.createHistoricExternalTaskLogQuery().tenantIdIn(TENANT_ONE);
		HistoricExternalTaskLogQuery queryTenant2 = historyService.createHistoricExternalTaskLogQuery().tenantIdIn(TENANT_TWO);

		// then
		assertThat(queryTenant1.count(), @is(2L));
		assertThat(queryTenant2.count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIds()
	  public virtual void testQueryByTenantIds()
	  {

		//given two process with different tenants

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		// then
		assertThat(query.count(), @is(5L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingTenantId()
	  public virtual void testQueryByNonExistingTenantId()
	  {

		//given two process with different tenants

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery().tenantIdIn("nonExisting");

		// then
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailQueryByTenantIdNull()
	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricExternalTaskLogQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		  // test passed
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingAsc()
	  public virtual void testQuerySortingAsc()
	  {

		//given two process with different tenants

		// when
		IList<HistoricExternalTaskLog> HistoricExternalTaskLogs = historyService.createHistoricExternalTaskLogQuery().orderByTenantId().asc().list();

		// then
		assertThat(HistoricExternalTaskLogs.Count, @is(5));
		assertThat(HistoricExternalTaskLogs[0].TenantId, @is(TENANT_ONE));
		assertThat(HistoricExternalTaskLogs[1].TenantId, @is(TENANT_ONE));
		assertThat(HistoricExternalTaskLogs[2].TenantId, @is(TENANT_TWO));
		assertThat(HistoricExternalTaskLogs[3].TenantId, @is(TENANT_TWO));
		assertThat(HistoricExternalTaskLogs[4].TenantId, @is(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingDesc()
	  public virtual void testQuerySortingDesc()
	  {

		//given two process with different tenants

		// when
		IList<HistoricExternalTaskLog> HistoricExternalTaskLogs = historyService.createHistoricExternalTaskLogQuery().orderByTenantId().desc().list();

		// then
		assertThat(HistoricExternalTaskLogs.Count, @is(5));
		assertThat(HistoricExternalTaskLogs[0].TenantId, @is(TENANT_TWO));
		assertThat(HistoricExternalTaskLogs[1].TenantId, @is(TENANT_TWO));
		assertThat(HistoricExternalTaskLogs[2].TenantId, @is(TENANT_TWO));
		assertThat(HistoricExternalTaskLogs[3].TenantId, @is(TENANT_ONE));
		assertThat(HistoricExternalTaskLogs[4].TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoAuthenticatedTenants()
	  public virtual void testQueryNoAuthenticatedTenants()
	  {

		// given
		identityService.setAuthentication("user", null, null);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAuthenticatedTenant()
	  public virtual void testQueryAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAuthenticatedTenants()
	  public virtual void testQueryAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		assertThat(query.count(), @is(5L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryDisabledTenantCheck()
	  public virtual void testQueryDisabledTenantCheck()
	  {
		// given
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		assertThat(query.count(), @is(5L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsNoAuthenticatedTenants()
	  public virtual void testGetErrorDetailsNoAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().tenantIdIn(TENANT_ONE).singleResult().Id;
		identityService.clearAuthentication();
		identityService.setAuthentication("user", null, null);


		try
		{
		  // when
		  historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
		  fail("Exception expected: It should not be possible to retrieve the error details");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  string errorMessage = e.Message;
		  assertThat(errorMessage.Contains("Cannot get the historic external task log "), @is(true));
		  assertThat(errorMessage.Contains(failedHistoricExternalTaskLogId), @is(true));
		  assertThat(errorMessage.Contains("because it belongs to no authenticated tenant."), @is(true));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsAuthenticatedTenant()
	  public virtual void testGetErrorDetailsAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().tenantIdIn(TENANT_ONE).singleResult().Id;

		// when
		string stacktrace = historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

		// then
		assertThat(stacktrace, @is(notNullValue()));
		assertThat(stacktrace, @is(ERROR_DETAILS));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsAuthenticatedTenants()
	  public virtual void testGetErrorDetailsAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		string logIdTenant1 = historyService.createHistoricExternalTaskLogQuery().failureLog().tenantIdIn(TENANT_ONE).singleResult().Id;

		string logIdTenant2 = historyService.createHistoricExternalTaskLogQuery().failureLog().tenantIdIn(TENANT_ONE).singleResult().Id;

		// when
		string stacktrace1 = historyService.getHistoricExternalTaskLogErrorDetails(logIdTenant1);
		string stacktrace2 = historyService.getHistoricExternalTaskLogErrorDetails(logIdTenant2);

		// then
		assertThat(stacktrace1, @is(notNullValue()));
		assertThat(stacktrace1, @is(ERROR_DETAILS));
		assertThat(stacktrace2, @is(notNullValue()));
		assertThat(stacktrace2, @is(ERROR_DETAILS));
	  }

	  // helper methods

	  protected internal virtual void completeExternalTask(string externalTaskId)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, WORKER_ID, true).topic(DEFAULT_TOPIC, LOCK_DURATION).execute();
		externalTaskService.complete(externalTaskId, WORKER_ID);
		// unlock the remaining tasks
		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  if (!lockedExternalTask.Id.Equals(externalTaskId))
		  {
			externalTaskService.unlock(lockedExternalTask.Id);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") protected org.camunda.bpm.engine.externaltask.ExternalTask startProcessInstanceAndFailExternalTask(String tenant)
	  protected internal virtual ExternalTask startProcessInstanceAndFailExternalTask(string tenant)
	  {
		ProcessInstance pi = runtimeService.createProcessInstanceByKey(DEFAULT_PROCESS_KEY).processDefinitionTenantId(tenant).execute();
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
		reportExternalTaskFailure(externalTask.Id);
		return externalTask;
	  }

	  protected internal virtual void startProcessInstanceFailAndCompleteExternalTask(string tenant)
	  {
		ExternalTask task = startProcessInstanceAndFailExternalTask(tenant);
		completeExternalTask(task.Id);
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId)
	  {
		reportExternalTaskFailure(externalTaskId, DEFAULT_TOPIC, WORKER_ID, 1, false, "This is an error!");
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string topic, string workerId, int? retries, bool usePriority, string errorMessage)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, workerId, usePriority).topic(topic, LOCK_DURATION).execute();
		externalTaskService.handleFailure(externalTaskId, workerId, errorMessage, ERROR_DETAILS, retries.Value, 0L);

		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  externalTaskService.unlock(lockedExternalTask.Id);
		}
	  }
	}

}