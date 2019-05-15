﻿/*
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
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricDecisionInstanceStatisticsQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricDecisionInstanceStatisticsQueryTest()
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
	  protected internal const string DISH_DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal const string DISH_DECISION = "dish-decision";
	  protected internal const string TEMPERATURE = "temperature";
	  protected internal const string DAY_TYPE = "dayType";
	  protected internal const string WEEKEND = "Weekend";
	  protected internal const string USER_ID = "user";

	  protected internal DecisionService decisionService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		decisionService = engineRule.DecisionService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, DISH_DRG_DMN);

		decisionService.evaluateDecisionByKey(DISH_DECISION).decisionDefinitionTenantId(TENANT_ONE).variables(Variables.createVariables().putValue(TEMPERATURE, 21).putValue(DAY_TYPE, WEEKEND)).evaluate();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoAuthenticatedTenants()
	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult();

		identityService.setAuthentication(USER_ID, null, null);

		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAuthenticatedTenant()
	  public virtual void testQueryAuthenticatedTenant()
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult();

		identityService.setAuthentication(USER_ID, null, Arrays.asList(TENANT_ONE));

		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		assertThat(query.count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryDisabledTenantCheck()
	  public virtual void testQueryDisabledTenantCheck()
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult();

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication(USER_ID, null, null);

		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		assertThat(query.count(), @is(3L));
	  }

	}

}