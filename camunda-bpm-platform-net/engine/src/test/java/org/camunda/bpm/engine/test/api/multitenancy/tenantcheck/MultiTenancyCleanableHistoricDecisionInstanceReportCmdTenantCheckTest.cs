using System;
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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyCleanableHistoricDecisionInstanceReportCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCleanableHistoricDecisionInstanceReportCmdTenantCheckTest()
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
	  protected internal const string TENANT_TWO = "tenant2";

	  private const string DECISION_DEFINITION_KEY = "decision";

	  protected internal const string DMN_MODEL = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNoAuthenticatedTenants()
	  public virtual void testReportNoAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		identityService.setAuthentication("user", null, null);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithAuthenticatedTenants()
	  public virtual void testReportWithAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(TENANT_ONE, reportResults[0].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportDisabledTenantCheck()
	  public virtual void testReportDisabledTenantCheck()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10,TENANT_ONE);
		testRule.deployForTenant(TENANT_TWO, DMN_MODEL);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(2, reportResults.Count);
		assertEquals(TENANT_ONE, reportResults[0].TenantId);
		assertEquals(TENANT_TWO, reportResults[1].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInNoAuthenticatedTenants()
	  public virtual void testReportTenantIdInNoAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		testRule.deployForTenant(TENANT_TWO, DMN_MODEL);

		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, null);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_TWO).list();

		// then
		assertEquals(0, reportResultsOne.Count);
		assertEquals(0, reportResultsTwo.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInWithAuthenticatedTenants()
	  public virtual void testReportTenantIdInWithAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		testRule.deployForTenant(TENANT_TWO, DMN_MODEL);

		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_TWO).list();

		// then
		assertEquals(1, reportResultsOne.Count);
		assertEquals(TENANT_ONE, reportResultsOne[0].TenantId);
		assertEquals(0, reportResultsTwo.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInDisabledTenantCheck()
	  public virtual void testReportTenantIdInDisabledTenantCheck()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);
		testRule.deployForTenant(TENANT_TWO, DMN_MODEL);

		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_TWO).list();

		// then
		assertEquals(1, reportResultsOne.Count);
		assertEquals(TENANT_ONE, reportResultsOne[0].TenantId);
		assertEquals(1, reportResultsTwo.Count);
		assertEquals(TENANT_TWO, reportResultsTwo[0].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithoutTenantId()
	  public virtual void testReportWithoutTenantId()
	  {
		// given
		testRule.deploy(DMN_MODEL);

		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, null);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().withoutTenantId().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(null, reportResults[0].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInWithoutTenantId()
	  public virtual void testReportTenantIdInWithoutTenantId()
	  {
		// given
		testRule.deploy(DMN_MODEL);
		testRule.deployForTenant(TENANT_ONE, DMN_MODEL);

		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, null);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().withoutTenantId().list();
		IList<CleanableHistoricDecisionInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricDecisionInstanceReport().tenantIdIn(TENANT_ONE).list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(null, reportResults[0].TenantId);
		assertEquals(1, reportResultsOne.Count);
		assertEquals(TENANT_ONE, reportResultsOne[0].TenantId);
	  }

	  protected internal virtual void prepareDecisionInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount, string tenantId)
	  {
		IList<DecisionDefinition> decisionDefinitions = null;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(key).tenantIdIn(tenantId).list();
		}
		else
		{
		  decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(key).withoutTenantId().list();
		}
		assertEquals(1, decisionDefinitions.Count);
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, historyTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		IDictionary<string, object> variables = Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
		for (int i = 0; i < instanceCount; i++)
		{
		  if (!string.ReferenceEquals(tenantId, null))
		  {
			engineRule.DecisionService.evaluateDecisionByKey(key).decisionDefinitionTenantId(tenantId).variables(variables).evaluate();
		  }
		  else
		  {
			engineRule.DecisionService.evaluateDecisionByKey(key).decisionDefinitionWithoutTenantId().variables(variables).evaluate();
		  }
		}

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	}

}