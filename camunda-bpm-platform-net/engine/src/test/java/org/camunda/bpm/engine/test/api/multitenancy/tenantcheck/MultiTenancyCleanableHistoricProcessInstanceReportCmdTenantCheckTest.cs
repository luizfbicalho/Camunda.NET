﻿using System;
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
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyCleanableHistoricProcessInstanceReportCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCleanableHistoricProcessInstanceReportCmdTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal static readonly BpmnModelInstance BPMN_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNoAuthenticatedTenants()
	  public virtual void testReportNoAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);

		identityService.setAuthentication("user", null, null);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithAuthenticatedTenants()
	  public virtual void testReportWithAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(TENANT_ONE, reportResults[0].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportDisabledTenantCheck()
	  public virtual void testReportDisabledTenantCheck()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

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
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, null);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_TWO).list();

		// then
		assertEquals(0, reportResultsOne.Count);
		assertEquals(0, reportResultsTwo.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInWithAuthenticatedTenants()
	  public virtual void testReportTenantIdInWithAuthenticatedTenants()
	  {
		// given
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_TWO).list();

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
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_ONE).list();
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsTwo = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_TWO).list();

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
		testRule.deploy(BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, null);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().withoutTenantId().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(null, reportResults[0].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportTenantIdInWithoutTenantId()
	  public virtual void testReportTenantIdInWithoutTenantId()
	  {
		// given
		testRule.deploy(BPMN_PROCESS);
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, TENANT_ONE);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10, null);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().withoutTenantId().list();
		IList<CleanableHistoricProcessInstanceReportResult> reportResultsOne = historyService.createCleanableHistoricProcessInstanceReport().tenantIdIn(TENANT_ONE).list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(null, reportResults[0].TenantId);
		assertEquals(1, reportResultsOne.Count);
		assertEquals(TENANT_ONE, reportResultsOne[0].TenantId);
	  }

	  protected internal virtual void prepareProcessInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount, string tenantId)
	  {
		IList<ProcessDefinition> processDefinitions = null;
		if (string.ReferenceEquals(tenantId, null))
		{
		  processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key).withoutTenantId().list();
		}
		else
		{
		  processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key).tenantIdIn(tenantId).list();
		}
		assertEquals(1, processDefinitions.Count);
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, historyTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		IList<string> processInstanceIds = new List<string>();
		{
		  for (int i = 0; i < instanceCount; i++)
		  {
			string processInstanceId = null;
			if (string.ReferenceEquals(tenantId, null))
			{
			  processInstanceId = runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().execute().Id;
			}
			else
			{
			  processInstanceId = runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(tenantId).execute().Id;
			}
			processInstanceIds.Add(processInstanceId);
		  }
		}
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	}

}