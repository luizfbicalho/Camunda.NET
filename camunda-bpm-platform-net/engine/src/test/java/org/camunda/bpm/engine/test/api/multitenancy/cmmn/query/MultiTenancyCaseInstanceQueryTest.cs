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
namespace org.camunda.bpm.engine.test.api.multitenancy.cmmn.query
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

	public class MultiTenancyCaseInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deployment(CMMN_FILE);
		deploymentForTenant(TENANT_ONE, CMMN_FILE);
		deploymentForTenant(TENANT_TWO, CMMN_FILE);

		createCaseInstance(null);
		createCaseInstance(TENANT_ONE);
		createCaseInstance(TENANT_TWO);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = caseService.createCaseInstanceQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByInstancesWithoutTenantId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  caseService.createCaseInstanceQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude case instances without tenant id because of database-specific ordering
		IList<CaseInstance> caseInstances = caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(caseInstances.Count, @is(2));
		assertThat(caseInstances[0].TenantId, @is(TENANT_ONE));
		assertThat(caseInstances[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude case instances without tenant id because of database-specific ordering
		IList<CaseInstance> caseInstances = caseService.createCaseInstanceQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(caseInstances.Count, @is(2));
		assertThat(caseInstances[0].TenantId, @is(TENANT_TWO));
		assertThat(caseInstances[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual void createCaseInstance(string tenantId)
	  {
		string caseDefinitionId = null;

		CaseDefinitionQuery caseDefinitionQuery = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase");
		if (string.ReferenceEquals(tenantId, null))
		{
		  caseDefinitionId = caseDefinitionQuery.withoutTenantId().singleResult().Id;
		}
		else
		{
		  caseDefinitionId = caseDefinitionQuery.tenantIdIn(tenantId).singleResult().Id;
		}

		caseService.withCaseDefinition(caseDefinitionId).create();
	  }

	}

}