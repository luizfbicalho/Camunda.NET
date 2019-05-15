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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricDataCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricDataCmdsTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "failingProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal CaseService caseService;
	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal static readonly BpmnModelInstance BPMN_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().endEvent().done();

	  protected internal static readonly BpmnModelInstance BPMN_ONETASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask("task1").moveToActivity("task1").endEvent().done();

	  protected internal static readonly BpmnModelInstance FAILING_BPMN_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaExpression("${failing}").camundaAsyncBefore().endEvent().done();

	  protected internal const string CMMN_PROCESS_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn";

	  protected internal const string DMN = "org/camunda/bpm/engine/test/api/multitenancy/simpleDecisionTable.dmn";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		caseService = engineRule.CaseService;
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.clearAuthentication();
		foreach (HistoricTaskInstance instance in historyService.createHistoricTaskInstanceQuery().list())
		{
		  historyService.deleteHistoricTaskInstance(instance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricProcessInstanceNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricProcessInstanceNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		string processInstanceId = startProcessInstance(null);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No historic process instance found");

		historyService.deleteHistoricProcessInstance(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricProcessInstanceWithAuthenticatedTenant()
	  public virtual void deleteHistoricProcessInstanceWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		string processInstanceId = startProcessInstance(null);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricProcessInstance(processInstanceId);

		identityService.clearAuthentication();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricProcessInstanceWithDisabledTenantCheck()
	  public virtual void deleteHistoricProcessInstanceWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_PROCESS);

		string processInstanceIdOne = startProcessInstance(TENANT_ONE);
		string processInstanceIdTwo = startProcessInstance(TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricProcessInstance(processInstanceIdOne);
		historyService.deleteHistoricProcessInstance(processInstanceIdTwo);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricTaskInstanceNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricTaskInstanceNoAuthenticatedTenants()
	  {
		string taskId = createTaskForTenant(TENANT_ONE);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the historic task instance");

		historyService.deleteHistoricTaskInstance(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricTaskInstanceWithAuthenticatedTenant()
	  public virtual void deleteHistoricTaskInstanceWithAuthenticatedTenant()
	  {
		string taskId = createTaskForTenant(TENANT_ONE);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricTaskInstance(taskId);

		identityService.clearAuthentication();

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricTaskInstanceWithDisabledTenantCheck()
	  public virtual void deleteHistoricTaskInstanceWithDisabledTenantCheck()
	  {
		string taskIdOne = createTaskForTenant(TENANT_ONE);
		string taskIdTwo = createTaskForTenant(TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricTaskInstance(taskIdOne);
		historyService.deleteHistoricTaskInstance(taskIdTwo);

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricCaseInstanceNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricCaseInstanceNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		string caseInstanceId = createAndCloseCaseInstance(null);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the historic case instance");

		historyService.deleteHistoricCaseInstance(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricCaseInstanceWithAuthenticatedTenant()
	  public virtual void deleteHistoricCaseInstanceWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		string caseInstanceId = createAndCloseCaseInstance(null);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricCaseInstance(caseInstanceId);

		identityService.clearAuthentication();

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricCaseInstanceWithDisabledTenantCheck()
	  public virtual void deleteHistoricCaseInstanceWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		testRule.deployForTenant(TENANT_TWO, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);

		string caseInstanceIdOne = createAndCloseCaseInstance(TENANT_ONE);
		string caseInstanceIdTwo = createAndCloseCaseInstance(TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricCaseInstance(caseInstanceIdOne);
		historyService.deleteHistoricCaseInstance(caseInstanceIdTwo);

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricDecisionInstanceNoAuthenticatedTenants()
	  public virtual void deleteHistoricDecisionInstanceNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, DMN);

		string decisionDefinitionId = evaluateDecisionTable(null);

		identityService.setAuthentication("user", null, null);

		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		identityService.clearAuthentication();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricDecisionInstanceWithAuthenticatedTenant()
	  public virtual void deleteHistoricDecisionInstanceWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, DMN);
		string decisionDefinitionId = evaluateDecisionTable(null);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		identityService.clearAuthentication();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricDecisionInstanceWithDisabledTenantCheck()
	  public virtual void deleteHistoricDecisionInstanceWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_TWO, DMN);

		string decisionDefinitionIdOne = evaluateDecisionTable(TENANT_ONE);
		string decisionDefinitionIdTwo = evaluateDecisionTable(TENANT_TWO);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionIdOne);
		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionIdTwo);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricDecisionInstanceByInstanceIdNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricDecisionInstanceByInstanceIdNoAuthenticatedTenants()
	  {

		// given
		testRule.deployForTenant(TENANT_ONE, DMN);
		evaluateDecisionTable(null);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		HistoricDecisionInstance historicDecisionInstance = query.includeInputs().includeOutputs().singleResult();

		// when
		identityService.setAuthentication("user", null, null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the historic decision instance");

		historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricDecisionInstanceByInstanceIdWithAuthenticatedTenant()
	  public virtual void deleteHistoricDecisionInstanceByInstanceIdWithAuthenticatedTenant()
	  {

		// given
		testRule.deployForTenant(TENANT_ONE, DMN);
		evaluateDecisionTable(null);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		HistoricDecisionInstance historicDecisionInstance = query.includeInputs().includeOutputs().singleResult();

		// when
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));
		historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		// then
		identityService.clearAuthentication();
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricDecisionInstanceByInstanceIdWithDisabledTenantCheck()
	  public virtual void deleteHistoricDecisionInstanceByInstanceIdWithDisabledTenantCheck()
	  {

		// given
		testRule.deployForTenant(TENANT_ONE, DMN);
		testRule.deployForTenant(TENANT_TWO, DMN);

		evaluateDecisionTable(TENANT_ONE);
		evaluateDecisionTable(TENANT_TWO);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		IList<HistoricDecisionInstance> historicDecisionInstances = query.includeInputs().includeOutputs().list();
		assertThat(historicDecisionInstances.Count, @is(2));

		// when user has no authorization
		identityService.setAuthentication("user", null, null);
		// and when tenant check is disabled
		processEngineConfiguration.TenantCheckEnabled = false;
		// and when all decision instances are deleted
		foreach (HistoricDecisionInstance @in in historicDecisionInstances)
		{
		  historyService.deleteHistoricDecisionInstanceByInstanceId(@in.Id);
		}

		// then
		identityService.clearAuthentication();
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToGetHistoricJobLogExceptionStacktraceNoAuthenticatedTenants()
	  public virtual void failToGetHistoricJobLogExceptionStacktraceNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);
		string processInstanceId = startProcessInstance(null);

		string historicJobLogId = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId).singleResult().Id;

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the historic job log");

		historyService.getHistoricJobLogExceptionStacktrace(historicJobLogId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getHistoricJobLogExceptionStacktraceWithAuthenticatedTenant()
	  public virtual void getHistoricJobLogExceptionStacktraceWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);
		string processInstanceId = startProcessInstance(null);

		testRule.executeAvailableJobs();

		HistoricJobLog log = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId).failureLog().listPage(0, 1).get(0);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		string historicJobLogExceptionStacktrace = historyService.getHistoricJobLogExceptionStacktrace(log.Id);

		assertThat(historicJobLogExceptionStacktrace, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getHistoricJobLogExceptionStacktraceWithDisabledTenantCheck()
	  public virtual void getHistoricJobLogExceptionStacktraceWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);

		string processInstanceId = startProcessInstance(TENANT_ONE);

		testRule.executeAvailableJobs();

		HistoricJobLog log = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId).failureLog().listPage(0, 1).get(0);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		string historicJobLogExceptionStacktrace = historyService.getHistoricJobLogExceptionStacktrace(log.Id);

		assertThat(historicJobLogExceptionStacktrace, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricVariableInstanceNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricVariableInstanceNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		string processInstanceId = startProcessInstance(null);
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceId).singleResult().Id;

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the historic variable instance '" + variableInstanceId + "' because it belongs to no authenticated tenant.");

		try
		{
		  historyService.deleteHistoricVariableInstance(variableInstanceId);
		}
		finally
		{
		  cleanUpAfterVariableInstanceTest(processInstanceId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricVariableInstanceWithAuthenticatedTenant()
	  public virtual void deleteHistoricVariableInstanceWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		string processInstanceId = startProcessInstance(null);
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue");
		HistoricVariableInstanceQuery variableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceId);

		assertThat(variableQuery.count(), @is(1L));
		string variableInstanceId = variableQuery.singleResult().Id;

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricVariableInstance(variableInstanceId);
		assertThat(variableQuery.count(), @is(0L));
		cleanUpAfterVariableInstanceTest(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricVariableInstanceWithDisabledTenantCheck()
	  public virtual void deleteHistoricVariableInstanceWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_ONETASK_PROCESS);

		string processInstanceIdOne = startProcessInstance(TENANT_ONE);
		string processInstanceIdTwo = startProcessInstance(TENANT_TWO);

		runtimeService.setVariable(processInstanceIdOne, "myVariable", "testValue");
		runtimeService.setVariable(processInstanceIdTwo, "myVariable", "testValue");
		HistoricVariableInstanceQuery variableQueryOne = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceIdOne);
		HistoricVariableInstanceQuery variableQueryTwo = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceIdTwo);

		assertThat(variableQueryOne.count(), @is(1L));
		assertThat(variableQueryTwo.count(), @is(1L));
		string variableInstanceIdOne = variableQueryOne.singleResult().Id;
		string variableInstanceIdTwo = variableQueryTwo.singleResult().Id;

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricVariableInstance(variableInstanceIdOne);
		historyService.deleteHistoricVariableInstance(variableInstanceIdTwo);
		assertThat(variableQueryOne.count(), @is(0L));
		assertThat(variableQueryTwo.count(), @is(0L));

		cleanUpAfterVariableInstanceTest(processInstanceIdOne, processInstanceIdTwo);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteHistoricVariableInstancesNoAuthenticatedTenants()
	  public virtual void failToDeleteHistoricVariableInstancesNoAuthenticatedTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		string processInstanceId = startProcessInstance(null);
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue");
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue2");

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot delete the historic variable instances of process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");

		try
		{
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId(processInstanceId);
		}
		finally
		{
		  cleanUpAfterVariableInstanceTest(processInstanceId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricVariableInstancesWithAuthenticatedTenant()
	  public virtual void deleteHistoricVariableInstancesWithAuthenticatedTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		string processInstanceId = startProcessInstance(null);
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue");
		runtimeService.setVariable(processInstanceId, "myVariable", "testValue2");

		HistoricVariableInstanceQuery variableQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceId);
		assertThat(variableQuery.count(), @is(1L));

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		historyService.deleteHistoricVariableInstancesByProcessInstanceId(processInstanceId);
		assertThat(variableQuery.count(), @is(0L));
		cleanUpAfterVariableInstanceTest(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricVariableInstancesWithDisabledTenantCheck()
	  public virtual void deleteHistoricVariableInstancesWithDisabledTenantCheck()
	  {
		testRule.deployForTenant(TENANT_ONE, BPMN_ONETASK_PROCESS);
		testRule.deployForTenant(TENANT_TWO, BPMN_ONETASK_PROCESS);

		string processInstanceIdOne = startProcessInstance(TENANT_ONE);
		string processInstanceIdTwo = startProcessInstance(TENANT_TWO);

		runtimeService.setVariable(processInstanceIdOne, "myVariable", "testValue");
		runtimeService.setVariable(processInstanceIdOne, "mySecondVariable", "testValue2");
		runtimeService.setVariable(processInstanceIdTwo, "myVariable", "testValue");
		runtimeService.setVariable(processInstanceIdTwo, "mySecondVariable", "testValue2");
		HistoricVariableInstanceQuery variableQueryOne = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceIdOne);
		HistoricVariableInstanceQuery variableQueryTwo = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstanceIdTwo);

		assertThat(variableQueryOne.count(), @is(2L));
		assertThat(variableQueryTwo.count(), @is(2L));

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		historyService.deleteHistoricVariableInstancesByProcessInstanceId(processInstanceIdOne);
		assertThat(variableQueryOne.count(), @is(0L));
		assertThat(variableQueryTwo.count(), @is(2L));

		historyService.deleteHistoricVariableInstancesByProcessInstanceId(processInstanceIdTwo);
		assertThat(variableQueryTwo.count(), @is(0L));

		cleanUpAfterVariableInstanceTest(processInstanceIdOne, processInstanceIdTwo);
	  }

	  // helper //////////////////////////////////////////////////////////

	  protected internal virtual string startProcessInstance(string tenantId)
	  {
		if (string.ReferenceEquals(tenantId, null))
		{
		  return runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		}
		else
		{
		  return runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(tenantId).execute().Id;
		}
	  }

	  protected internal virtual string createAndCloseCaseInstance(string tenantId)
	  {
		string caseInstanceId;

		CaseInstanceBuilder builder = caseService.withCaseDefinitionByKey("oneTaskCase");
		if (string.ReferenceEquals(tenantId, null))
		{
		  caseInstanceId = builder.create().Id;
		}
		else
		{
		  caseInstanceId = builder.caseDefinitionTenantId(tenantId).create().Id;
		}

		caseService.completeCaseExecution(caseInstanceId);
		caseService.closeCaseInstance(caseInstanceId);

		return caseInstanceId;
	  }

	  protected internal virtual string evaluateDecisionTable(string tenantId)
	  {
		string decisionDefinitionId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().singleResult().Id;
		}
		else
		{
		  decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().tenantIdIn(tenantId).singleResult().Id;
		}

		VariableMap variables = Variables.createVariables().putValue("status", "bronze");
		decisionService.evaluateDecisionTableById(decisionDefinitionId, variables);

		return decisionDefinitionId;
	  }

	  protected internal virtual string createTaskForTenant(string tenantId)
	  {
		Task task = taskService.newTask();
		task.TenantId = TENANT_ONE;

		taskService.saveTask(task);
		taskService.complete(task.Id);

		return task.Id;
	  }

	  protected internal virtual void cleanUpAfterVariableInstanceTest(params string[] processInstanceIds)
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		foreach (string processInstanceId in processInstanceIds)
		{
		  Task task = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult();
		  if (task != null)
		  {
			taskService.complete(task.Id);
		  }
		  historyService.deleteHistoricProcessInstance(processInstanceId);
		}

		identityService.clearAuthentication();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();
		assertThat(query.count(), @is(0L));
		processEngineConfiguration.TenantCheckEnabled = true;
	  }

	}

}