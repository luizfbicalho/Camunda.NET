using System.IO;

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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	public class MultiTenancyDecisionDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionDefinitionCmdsTenantCheckTest()
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


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string DMN_MODEL = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string decisionDefinitionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);

		decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().singleResult().Id;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionModelNoAuthenticatedTenants()
	  public virtual void failToGetDecisionModelNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision definition");

		repositoryService.getDecisionModel(decisionDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionModelWithAuthenticatedTenant()
	  public virtual void getDecisionModelWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getDecisionModel(decisionDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionModelDisabledTenantCheck()
	  public virtual void getDecisionModelDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getDecisionModel(decisionDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionDiagramNoAuthenticatedTenants()
	  public virtual void failToGetDecisionDiagramNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision definition");

		repositoryService.getDecisionDiagram(decisionDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDiagramWithAuthenticatedTenant()
	  public virtual void getDecisionDiagramWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getDecisionDiagram(decisionDefinitionId);

		// inputStream is always null because there is no decision diagram at the moment
		// what should be deployed as a diagram resource for DMN? 
		assertThat(inputStream, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDiagramDisabledTenantCheck()
	  public virtual void getDecisionDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getDecisionDiagram(decisionDefinitionId);

		// inputStream is always null because there is no decision diagram at the moment
		// what should be deployed as a diagram resource for DMN? 
		assertThat(inputStream, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionDefinitionNoAuthenticatedTenants()
	  public virtual void failToGetDecisionDefinitionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision definition");

		repositoryService.getDecisionDefinition(decisionDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDefinitionWithAuthenticatedTenant()
	  public virtual void getDecisionDefinitionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DecisionDefinition definition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDefinitionDisabledTenantCheck()
	  public virtual void getDecisionDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DecisionDefinition definition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDmnModelInstanceNoAuthenticatedTenants()
	  public virtual void failToGetDmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision definition");

		repositoryService.getDmnModelInstance(decisionDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

		DecisionDefinition definition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
		assertThat(definition.HistoryTimeToLive, @is(6));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveDisabledTenantCheck()
	  public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);

		DecisionDefinition definition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
		assertThat(definition.HistoryTimeToLive, @is(6));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  public virtual void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the decision definition");

		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, 6);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDmnModelInstanceWithAuthenticatedTenant()
	  public virtual void getDmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DmnModelInstance modelInstance = repositoryService.getDmnModelInstance(decisionDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDmnModelInstanceDisabledTenantCheck()
	  public virtual void getDmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DmnModelInstance modelInstance = repositoryService.getDmnModelInstance(decisionDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

	}

}