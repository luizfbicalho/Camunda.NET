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
namespace org.camunda.bpm.engine.test.api.multitenancy.cmmn.query.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class MultiTenancyHistoricCaseInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deployment(CMMN_FILE);
		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		caseService.withCaseDefinitionByKey("oneTaskCase").caseDefinitionWithoutTenantId().create();
		createCaseInstance(TENANT_ONE);
		createCaseInstance(TENANT_TWO);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = historyService.createHistoricCaseInstanceQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByInstancesWithoutTenantId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricCaseInstanceQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude historic case instances without tenant id because of database-specific ordering
		IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(historicCaseInstances.Count, @is(2));
		assertThat(historicCaseInstances[0].TenantId, @is(TENANT_ONE));
		assertThat(historicCaseInstances[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude historic case instances without tenant id because of database-specific ordering
		IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(historicCaseInstances.Count, @is(2));
		assertThat(historicCaseInstances[0].TenantId, @is(TENANT_TWO));
		assertThat(historicCaseInstances[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual void createCaseInstance(string tenantId)
	  {
		CaseInstanceBuilder builder = caseService.withCaseDefinitionByKey("oneTaskCase");
		builder.caseDefinitionTenantId(tenantId).create();
	  }

	}

}