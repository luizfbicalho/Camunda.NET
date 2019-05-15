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


	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	public class MultiTenancyCaseDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCaseDefinitionCmdsTenantCheckTest()
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

	  protected internal const string CMMN_MODEL = "org/camunda/bpm/engine/test/api/cmmn/emptyStageCase.cmmn";
	  protected internal const string CMMN_DIAGRAM = "org/camunda/bpm/engine/test/api/cmmn/emptyStageCase.png";

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

	  protected internal string caseDefinitionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, CMMN_MODEL, CMMN_DIAGRAM);

		caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetCaseModelNoAuthenticatedTenants()
	  public virtual void failToGetCaseModelNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case definition");

		repositoryService.getCaseModel(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseModelWithAuthenticatedTenant()
	  public virtual void getCaseModelWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getCaseModel(caseDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseModelDisabledTenantCheck()
	  public virtual void getCaseModelDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getCaseModel(caseDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetCaseDiagramNoAuthenticatedTenants()
	  public virtual void failToGetCaseDiagramNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case definition");

		repositoryService.getCaseDiagram(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseDiagramWithAuthenticatedTenant()
	  public virtual void getCaseDiagramWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		Stream inputStream = repositoryService.getCaseDiagram(caseDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseDiagramDisabledTenantCheck()
	  public virtual void getCaseDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		Stream inputStream = repositoryService.getCaseDiagram(caseDefinitionId);

		assertThat(inputStream, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetCaseDefinitionNoAuthenticatedTenants()
	  public virtual void failToGetCaseDefinitionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case definition");

		repositoryService.getCaseDefinition(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseDefinitionWithAuthenticatedTenant()
	  public virtual void getCaseDefinitionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCaseDefinitionDisabledTenantCheck()
	  public virtual void getCaseDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetCmmnModelInstanceNoAuthenticatedTenants()
	  public virtual void failToGetCmmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case definition");

		repositoryService.getCmmnModelInstance(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCmmnModelInstanceWithAuthenticatedTenant()
	  public virtual void getCmmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		CmmnModelInstance modelInstance = repositoryService.getCmmnModelInstance(caseDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCmmnModelInstanceDisabledTenantCheck()
	  public virtual void getCmmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		CmmnModelInstance modelInstance = repositoryService.getCmmnModelInstance(caseDefinitionId);

		assertThat(modelInstance, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinitionId);

		assertThat(definition.TenantId, @is(TENANT_ONE));
		assertThat(definition.HistoryTimeToLive, @is(6));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateHistoryTimeToLiveDisabledTenantCheck()
	  public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinitionId);

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
		thrown.expectMessage("Cannot update the case definition");

		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitionId, 6);
	  }

	}

}