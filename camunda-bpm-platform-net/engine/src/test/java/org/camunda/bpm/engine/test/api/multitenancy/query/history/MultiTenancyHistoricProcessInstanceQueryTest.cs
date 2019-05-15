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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class MultiTenancyHistoricProcessInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().endEvent().done();

		deployment(oneTaskProcess);
		deploymentForTenant(TENANT_ONE, oneTaskProcess);
		deploymentForTenant(TENANT_TWO, oneTaskProcess);

		startProcessInstance();
		startProcessInstanceForTenant(TENANT_ONE);
		startProcessInstanceForTenant(TENANT_TWO);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = historyService.createHistoricProcessInstanceQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testQueryByWithoutTenantId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(historicProcessInstances.Count, @is(2));
		assertThat(historicProcessInstances[0].TenantId, @is(TENANT_ONE));
		assertThat(historicProcessInstances[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(historicProcessInstances.Count, @is(2));
		assertThat(historicProcessInstances[0].TenantId, @is(TENANT_TWO));
		assertThat(historicProcessInstances[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Collections.singletonList(TENANT_ONE));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(tenant).execute();
	  }

	  protected internal virtual ProcessInstance startProcessInstance()
	  {
		return runtimeService.createProcessInstanceByKey("testProcess").processDefinitionWithoutTenantId().execute();
	  }

	}

}