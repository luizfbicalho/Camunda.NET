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
//	import static org.junit.Assert.assertThat;


	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>

	public class MultiTenancyDecisionRequirementsDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDecisionRequirementsDefinitionCmdsTenantCheckTest()
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

	  protected internal const string DRG_DMN = "org/camunda/bpm/engine/test/api/multitenancy/DecisionRequirementsGraph.dmn";

	  protected internal const string DRD_DMN = "org/camunda/bpm/engine/test/api/multitenancy/DecisionRequirementsGraph.png";

	  protected internal ProcessEngineRule engineRule = new ProcessEngineRule(true);

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string decisionRequirementsDefinitionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, DRG_DMN, DRD_DMN);
		decisionRequirementsDefinitionId = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionRequirementsDefinitionNoAuthenticatedTenants()
	  public virtual void failToGetDecisionRequirementsDefinitionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision requirements definition");

		repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionRequirementsDefinitionWithAuthenticatedTenant()
	  public virtual void getDecisionRequirementsDefinitionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		DecisionRequirementsDefinition definition = repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionRequirementsDefinitionDisabledTenantCheck()
	  public virtual void getDecisionRequirementsDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		DecisionRequirementsDefinition definition = repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionRequirementsModelNoAuthenticatedTenants()
	  public virtual void failToGetDecisionRequirementsModelNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision requirements definition");

		repositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionRequirementsModelWithAuthenticatedTenant()
	  public virtual void getDecisionRequirementsModelWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionRequirementsModelDisabledTenantCheck()
	  public virtual void getDecisionRequirementsModelDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetDecisionRequirementsDiagramNoAuthenticatedTenants()
	  public virtual void failToGetDecisionRequirementsDiagramNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the decision requirements definition");

		repositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDiagramWithAuthenticatedTenant()
	  public virtual void getDecisionDiagramWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDecisionDiagramDisabledTenantCheck()
	  public virtual void getDecisionDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

	}

}