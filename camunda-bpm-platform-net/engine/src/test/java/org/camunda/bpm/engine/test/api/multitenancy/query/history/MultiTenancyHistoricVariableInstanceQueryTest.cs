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
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class MultiTenancyHistoricVariableInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string TENANT_ONE_VAR = "tenant1Var";
	  protected internal const string TENANT_TWO_VAR = "tenant2Var";

	  protected internal override void setUp()
	  {
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().endEvent().done();

		deploymentForTenant(TENANT_ONE, oneTaskProcess);
		deploymentForTenant(TENANT_TWO, oneTaskProcess);

		startProcessInstanceForTenant(TENANT_ONE, TENANT_ONE_VAR);
		startProcessInstanceForTenant(TENANT_TWO, TENANT_TWO_VAR);
	  }

	  public virtual void testQueryWithoutTenantId()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));
		assertEquals(query.list().get(0).Value, TENANT_ONE_VAR);

		query = historyService.createHistoricVariableInstanceQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
		assertEquals(query.list().get(0).Value, TENANT_TWO_VAR);
	  }

	  public virtual void testQueryByTenantIds()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricVariableInstanceQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().orderByTenantId().asc().list();

		assertThat(historicVariableInstances.Count, @is(2));
		assertThat(historicVariableInstances[0].TenantId, @is(TENANT_ONE));
		assertEquals(historicVariableInstances[0].Value, TENANT_ONE_VAR);
		assertThat(historicVariableInstances[1].TenantId, @is(TENANT_TWO));
		assertEquals(historicVariableInstances[1].Value, TENANT_TWO_VAR);
	  }

	  public virtual void testQuerySortingDesc()
	  {
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().orderByTenantId().desc().list();

		assertThat(historicVariableInstances.Count, @is(2));
		assertThat(historicVariableInstances[0].TenantId, @is(TENANT_TWO));
		assertEquals(historicVariableInstances[0].Value, TENANT_TWO_VAR);
		assertThat(historicVariableInstances[1].TenantId, @is(TENANT_ONE));
		assertEquals(historicVariableInstances[1].Value, TENANT_ONE_VAR);
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		assertThat(query.count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		assertThat(query.count(), @is(2L));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceForTenant(string tenant, string var)
	  {
		return runtimeService.createProcessInstanceByKey("testProcess").setVariable("myVar", var).processDefinitionTenantId(tenant).execute();
	  }

	}

}