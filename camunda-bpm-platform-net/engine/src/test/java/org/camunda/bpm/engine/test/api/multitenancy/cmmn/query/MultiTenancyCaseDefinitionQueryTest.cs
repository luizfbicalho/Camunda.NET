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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;

	public class MultiTenancyCaseDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CASE_DEFINITION_KEY = "Case_1";
	  protected internal const string CMMN = "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testSimpleDeployment.cmmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deployment(CMMN);
		deploymentForTenant(TENANT_ONE, CMMN);
		deploymentForTenant(TENANT_TWO, CMMN);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByDefinitionsWithoutTenantId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().withoutTenantId();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE).includeCaseDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_TWO).includeCaseDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeCaseDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByKey()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY);
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).withoutTenantId();
		// one definition without tenant id
		assertThat(query.count(), @is(1L));

		query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).tenantIdIn(TENANT_ONE);
		// one definition for tenant one
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, CMMN);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion();
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		IDictionary<string, CaseDefinition> caseDefinitionsForTenant = getCaseDefinitionsForTenant(query.list());
		assertThat(caseDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(caseDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(caseDefinitionsForTenant[null].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, CMMN);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		CaseDefinition caseDefinition = query.singleResult();
		assertThat(caseDefinition.TenantId, @is(TENANT_ONE));
		assertThat(caseDefinition.Version, @is(2));

		query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));

		caseDefinition = query.singleResult();
		assertThat(caseDefinition.TenantId, @is(TENANT_TWO));
		assertThat(caseDefinition.Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, CMMN);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO);
		// one definition for each tenant
		assertThat(query.count(), @is(2L));

		IDictionary<string, CaseDefinition> caseDefinitionsForTenant = getCaseDefinitionsForTenant(query.list());
		assertThat(caseDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(caseDefinitionsForTenant[TENANT_TWO].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(CMMN);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion().withoutTenantId();

		assertThat(query.count(), @is(1L));

		CaseDefinition cDefinition = query.singleResult();
		assertThat(cDefinition.TenantId, @is(nullValue()));
		assertThat(cDefinition.Version, @is(2));
	  }

	  public virtual void testQueryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(CMMN);
		// deploy a third version for tenant one
		deploymentForTenant(TENANT_ONE, CMMN);
		deploymentForTenant(TENANT_ONE, CMMN);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).includeCaseDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));

		IDictionary<string, CaseDefinition> caseDefinitionsForTenant = getCaseDefinitionsForTenant(query.list());
		assertThat(caseDefinitionsForTenant[TENANT_ONE].Version, @is(3));
		assertThat(caseDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(caseDefinitionsForTenant[null].Version, @is(2));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.createCaseDefinitionQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(caseDefinitions.Count, @is(2));
		assertThat(caseDefinitions[0].TenantId, @is(TENANT_ONE));
		assertThat(caseDefinitions[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(caseDefinitions.Count, @is(2));
		assertThat(caseDefinitions[0].TenantId, @is(TENANT_TWO));
		assertThat(caseDefinitions[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeCaseDefinitionsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual IDictionary<string, CaseDefinition> getCaseDefinitionsForTenant(IList<CaseDefinition> definitions)
	  {
		IDictionary<string, CaseDefinition> definitionsForTenant = new Dictionary<string, CaseDefinition>();

		foreach (CaseDefinition definition in definitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	}

}