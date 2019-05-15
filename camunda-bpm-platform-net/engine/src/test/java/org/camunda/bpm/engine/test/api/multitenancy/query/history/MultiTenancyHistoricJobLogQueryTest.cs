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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricJobLogQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal static readonly BpmnModelInstance BPMN = Bpmn.createExecutableProcess("failingProcess").startEvent().serviceTask().camundaExpression("${failing}").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R1/PT1M").endEvent().done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deploymentForTenant(TENANT_ONE, BPMN);
		deploymentForTenant(TENANT_TWO, BPMN);

		startProcessInstanceAndExecuteFailingJobForTenant(TENANT_ONE);
		startProcessInstanceAndExecuteFailingJobForTenant(TENANT_TWO);
	  }

	  public virtual void testQueryWithoutTenantId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		assertThat(query.count(), @is(4L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(2L));

		query = historyService.createHistoricJobLogQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(4L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricJobLogQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		IList<HistoricJobLog> historicJobLogs = historyService.createHistoricJobLogQuery().orderByTenantId().asc().list();

		assertThat(historicJobLogs.Count, @is(4));
		assertThat(historicJobLogs[0].TenantId, @is(TENANT_ONE));
		assertThat(historicJobLogs[1].TenantId, @is(TENANT_ONE));
		assertThat(historicJobLogs[2].TenantId, @is(TENANT_TWO));
		assertThat(historicJobLogs[3].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		IList<HistoricJobLog> historicJobLogs = historyService.createHistoricJobLogQuery().orderByTenantId().desc().list();

		assertThat(historicJobLogs.Count, @is(4));
		assertThat(historicJobLogs[0].TenantId, @is(TENANT_TWO));
		assertThat(historicJobLogs[1].TenantId, @is(TENANT_TWO));
		assertThat(historicJobLogs[2].TenantId, @is(TENANT_ONE));
		assertThat(historicJobLogs[3].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();
		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		assertThat(query.count(), @is(4L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(2L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();
		assertThat(query.count(), @is(4L));
	  }

	  protected internal virtual void startProcessInstanceAndExecuteFailingJobForTenant(string tenant)
	  {
		runtimeService.createProcessInstanceByKey("failingProcess").processDefinitionTenantId(tenant).execute();

		executeAvailableJobs();
	  }

	}

}