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
//	import static org.camunda.bpm.engine.query.PeriodUnit.MONTH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricProcessInstanceReportCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricProcessInstanceReportCmdTenantCheckTest()
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
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal static readonly BpmnModelInstance BPMN_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDurationReportByMonthNoAuthenticatedTenants()
	  public virtual void getDurationReportByMonthNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.setAuthentication("user", null, null);

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		assertThat(result.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDurationReportByMonthWithAuthenticatedTenant()
	  public virtual void getDurationReportByMonthWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		assertThat(result.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getDurationReportByMonthDisabledTenantCheck()
	  public virtual void getDurationReportByMonthDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		assertThat(result.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByMultipleProcessDefinitionIdByMonthNoAuthenticatedTenants()
	  public virtual void getReportByMultipleProcessDefinitionIdByMonthNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult().Id;
		string processDefinitionIdTwo = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO).singleResult().Id;

		identityService.setAuthentication("user", null, null);

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).duration(MONTH);

		assertThat(result.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByMultipleProcessDefinitionIdByMonthWithAuthenticatedTenant()
	  public virtual void getReportByMultipleProcessDefinitionIdByMonthWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult().Id;
		string processDefinitionIdTwo = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO).singleResult().Id;

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).duration(MONTH);

		assertThat(result.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByMultipleProcessDefinitionIdByMonthDisabledTenantCheck()
	  public virtual void getReportByMultipleProcessDefinitionIdByMonthDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_ONE).singleResult().Id;
		string processDefinitionIdTwo = repositoryService.createProcessDefinitionQuery().tenantIdIn(TENANT_TWO).singleResult().Id;

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).duration(MONTH);

		assertThat(result.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByProcessDefinitionKeyByMonthNoAuthenticatedTenants()
	  public virtual void getReportByProcessDefinitionKeyByMonthNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.setAuthentication("user", null, null);

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_DEFINITION_KEY).duration(MONTH);

		assertThat(result.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByProcessDefinitionKeyByMonthWithAuthenticatedTenant()
	  public virtual void getReportByProcessDefinitionKeyByMonthWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_DEFINITION_KEY).duration(MONTH);

		assertThat(result.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getReportByProcessDefinitionKeyByMonthDisabledTenantCheck()
	  public virtual void getReportByProcessDefinitionKeyByMonthDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_DEFINITION_KEY).duration(MONTH);

		assertThat(result.Count, @is(2));
	  }

	  // helper //////////////////////////////////////////////////////////

	  protected internal virtual string startAndCompleteProcessInstance(string tenantId)
	  {
		string processInstanceId = null;
		if (string.ReferenceEquals(tenantId, null))
		{
		  processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		}
		else
		{
		  processInstanceId = runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(tenantId).execute().Id;
		}

		addToCalendar(DateTime.MONTH, 5);

		Task task = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult();

		taskService.complete(task.Id);

		return processInstanceId;
	  }

	  protected internal virtual void addToCalendar(int field, int month)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(ClockUtil.CurrentTime);
		calendar.add(field, month);
		ClockUtil.CurrentTime = calendar;
	  }

	}

}