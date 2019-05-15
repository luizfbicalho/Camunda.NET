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
namespace org.camunda.bpm.engine.test.api.multitenancy.query
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;

	public class MultiTenancyDecisionDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DECISION_DEFINITION_KEY = "decision";
	  protected internal const string DMN = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal override void setUp()
	  {
		deployment(DMN);
		deploymentForTenant(TENANT_ONE, DMN);
		deploymentForTenant(TENANT_TWO, DMN);
	  }

	  public virtual void testQueryNoTenantIdSet()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByDefinitionsWithoutTenantId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE).includeDecisionDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_TWO).includeDecisionDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

	  public virtual void testQueryByKey()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY);
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).withoutTenantId();
		// one definition without tenant id
		assertThat(query.count(), @is(1L));

		query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).tenantIdIn(TENANT_ONE);
		// one definition for tenant one
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, DMN);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion();
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		IDictionary<string, DecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.list());
		assertThat(decisionDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(decisionDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(decisionDefinitionsForTenant[null].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, DMN);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		DecisionDefinition decisionDefinition = query.singleResult();
		assertThat(decisionDefinition.TenantId, @is(TENANT_ONE));
		assertThat(decisionDefinition.Version, @is(2));

		query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));

		decisionDefinition = query.singleResult();
		assertThat(decisionDefinition.TenantId, @is(TENANT_TWO));
		assertThat(decisionDefinition.Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		deploymentForTenant(TENANT_ONE, DMN);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc();
		// one definition for each tenant
		assertThat(query.count(), @is(2L));

		IDictionary<string, DecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.list());
		assertThat(decisionDefinitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(decisionDefinitionsForTenant[TENANT_TWO].Version, @is(1));
	  }

	  public virtual void testQueryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(DMN);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion().withoutTenantId();

		assertThat(query.count(), @is(1L));

		DecisionDefinition decisionDefinition = query.singleResult();
		assertThat(decisionDefinition.TenantId, @is(nullValue()));
		assertThat(decisionDefinition.Version, @is(2));
	  }

	  public virtual void testQueryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		deployment(DMN);
		// deploy a third version for tenant one
		deploymentForTenant(TENANT_ONE, DMN);
		deploymentForTenant(TENANT_ONE, DMN);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));

		IDictionary<string, DecisionDefinition> decisionDefinitionsForTenant = getDecisionDefinitionsForTenant(query.list());
		assertThat(decisionDefinitionsForTenant[TENANT_ONE].Version, @is(3));
		assertThat(decisionDefinitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(decisionDefinitionsForTenant[null].Version, @is(2));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  repositoryService.createDecisionDefinitionQuery().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(decisionDefinitions.Count, @is(2));
		assertThat(decisionDefinitions[0].TenantId, @is(TENANT_ONE));
		assertThat(decisionDefinitions[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(decisionDefinitions.Count, @is(2));
		assertThat(decisionDefinitions[0].TenantId, @is(TENANT_TWO));
		assertThat(decisionDefinitions[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionDefinitionsWithoutTenantId().count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual IDictionary<string, DecisionDefinition> getDecisionDefinitionsForTenant(IList<DecisionDefinition> decisionDefinitions)
	  {
		IDictionary<string, DecisionDefinition> definitionsForTenant = new Dictionary<string, DecisionDefinition>();

		foreach (DecisionDefinition definition in decisionDefinitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	}

}