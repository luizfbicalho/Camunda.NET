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
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyDecisionRequirementsDefinitionQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionRequirementsDefinitionQueryTest()
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


	  protected internal const string DECISION_REQUIREMENTS_DEFINITION_KEY = "score";
	  protected internal const string DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.deploy(DMN);
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_TWO, DMN);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryNoTenantIdSet()
	  public virtual void queryNoTenantIdSet()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByTenantId()
	  public virtual void queryByTenantId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByTenantIds()
	  public virtual void queryByTenantIds()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByDefinitionsWithoutTenantId()
	  public virtual void queryByDefinitionsWithoutTenantId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  public virtual void queryByTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE).includeDecisionRequirementsDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_TWO).includeDecisionRequirementsDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(2L));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionRequirementsDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByKey()
	  public virtual void queryByKey()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY);
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).withoutTenantId();
		// one definition without tenant id
		assertThat(query.count(), @is(1L));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).tenantIdIn(TENANT_ONE);
		// one definition for tenant one
		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatestNoTenantIdSet()
	  public virtual void queryByLatestNoTenantIdSet()
	  {
		// deploy a second version for tenant one
		testRule.deployForTenant(TENANT_ONE, DMN);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion();
		// one definition for each tenant
		assertThat(query.count(), @is(3L));

		IDictionary<string, DecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.list());
		assertThat(definitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(definitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(definitionsForTenant[null].Version, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatestWithTenantId()
	  public virtual void queryByLatestWithTenantId()
	  {
		// deploy a second version for tenant one
		testRule.deployForTenant(TENANT_ONE, DMN);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		DecisionRequirementsDefinition DecisionRequirementsDefinition = query.singleResult();
		assertThat(DecisionRequirementsDefinition.TenantId, @is(TENANT_ONE));
		assertThat(DecisionRequirementsDefinition.Version, @is(2));

		query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));

		DecisionRequirementsDefinition = query.singleResult();
		assertThat(DecisionRequirementsDefinition.TenantId, @is(TENANT_TWO));
		assertThat(DecisionRequirementsDefinition.Version, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatestWithTenantIds()
	  public virtual void queryByLatestWithTenantIds()
	  {
		// deploy a second version for tenant one
		testRule.deployForTenant(TENANT_ONE, DMN);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc();
		// one definition for each tenant
		assertThat(query.count(), @is(2L));

		IDictionary<string, DecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.list());
		assertThat(definitionsForTenant[TENANT_ONE].Version, @is(2));
		assertThat(definitionsForTenant[TENANT_TWO].Version, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatestWithoutTenantId()
	  public virtual void queryByLatestWithoutTenantId()
	  {
		// deploy a second version without tenant id
		testRule.deploy(DMN);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion().withoutTenantId();

		assertThat(query.count(), @is(1L));

		DecisionRequirementsDefinition DecisionRequirementsDefinition = query.singleResult();
		assertThat(DecisionRequirementsDefinition.TenantId, @is(nullValue()));
		assertThat(DecisionRequirementsDefinition.Version, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  public virtual void queryByLatestWithTenantIdsIncludeDefinitionsWithoutTenantId()
	  {
		// deploy a second version without tenant id
		testRule.deploy(DMN);
		// deploy a third version for tenant one
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_ONE, DMN);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(DECISION_REQUIREMENTS_DEFINITION_KEY).latestVersion().tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionRequirementsDefinitionsWithoutTenantId();

		assertThat(query.count(), @is(3L));

		IDictionary<string, DecisionRequirementsDefinition> definitionsForTenant = getDecisionRequirementsDefinitionsForTenant(query.list());
		assertThat(definitionsForTenant[TENANT_ONE].Version, @is(3));
		assertThat(definitionsForTenant[TENANT_TWO].Version, @is(1));
		assertThat(definitionsForTenant[null].Version, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByNonExistingTenantId()
	  public virtual void queryByNonExistingTenantId()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failQueryByTenantIdNull()
	  public virtual void failQueryByTenantIdNull()
	  {

		thrown.expect(typeof(NullValueException));

		repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn((string) null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void querySortingAsc()
	  public virtual void querySortingAsc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<DecisionRequirementsDefinition> DecisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(DecisionRequirementsDefinitions.Count, @is(2));
		assertThat(DecisionRequirementsDefinitions[0].TenantId, @is(TENANT_ONE));
		assertThat(DecisionRequirementsDefinitions[1].TenantId, @is(TENANT_TWO));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void querySortingDesc()
	  public virtual void querySortingDesc()
	  {
		// exclude definitions without tenant id because of database-specific ordering
		IList<DecisionRequirementsDefinition> DecisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(DecisionRequirementsDefinitions.Count, @is(2));
		assertThat(DecisionRequirementsDefinitions[0].TenantId, @is(TENANT_TWO));
		assertThat(DecisionRequirementsDefinitions[1].TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryNoAuthenticatedTenants()
	  public virtual void queryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();
		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryAuthenticatedTenant()
	  public virtual void queryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).includeDecisionRequirementsDefinitionsWithoutTenantId().count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryAuthenticatedTenants()
	  public virtual void queryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryDisabledTenantCheck()
	  public virtual void queryDisabledTenantCheck()
	  {
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual IDictionary<string, DecisionRequirementsDefinition> getDecisionRequirementsDefinitionsForTenant(IList<DecisionRequirementsDefinition> definitions)
	  {
		IDictionary<string, DecisionRequirementsDefinition> definitionsForTenant = new Dictionary<string, DecisionRequirementsDefinition>();

		foreach (DecisionRequirementsDefinition definition in definitions)
		{
		  definitionsForTenant[definition.TenantId] = definition;
		}
		return definitionsForTenant;
	  }

	}

}