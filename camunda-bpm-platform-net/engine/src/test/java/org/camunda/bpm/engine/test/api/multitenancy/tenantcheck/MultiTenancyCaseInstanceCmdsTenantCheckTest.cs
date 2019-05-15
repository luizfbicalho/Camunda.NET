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
//	import static org.hamcrest.CoreMatchers.hasItem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author kristin.polenz
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyCaseInstanceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCaseInstanceCmdsTenantCheckTest()
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


	  protected internal const string VARIABLE_NAME = "myVar";
	  protected internal const string VARIABLE_VALUE = "myValue";

	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string CMMN_MODEL = "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn";

	  protected internal const string ACTIVITY_ID = "PI_HumanTask_1";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal IdentityService identityService;
	  protected internal CaseService caseService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService = engineRule.IdentityService;
		caseService = engineRule.CaseService;
		historyService = engineRule.HistoryService;

		testRule.deployForTenant(TENANT_ONE, CMMN_MODEL);

		caseInstanceId = createCaseInstance(null);

		caseExecutionId = CaseExecution.Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionNoAuthenticatedTenants()
	  public virtual void manuallyStartCaseExecutionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.manuallyStartCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionWithAuthenticatedTenant()
	  public virtual void manuallyStartCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.manuallyStartCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		CaseExecution caseExecution = CaseExecution;

		assertThat(caseExecution.Active, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionDisabledTenantCheck()
	  public virtual void manuallyStartCaseExecutionDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.manuallyStartCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		CaseExecution caseExecution = CaseExecution;

		assertThat(caseExecution.Active, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionNoAuthenticatedTenants()
	  public virtual void disableCaseExecutionNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.disableCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionWithAuthenticatedTenant()
	  public virtual void disableCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.disableCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		HistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		assertThat(historicCaseActivityInstance, notNullValue());
		assertThat(historicCaseActivityInstance.Disabled, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionDisabledTenantCheck()
	  public virtual void disableCaseExecutionDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.disableCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		HistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		assertThat(historicCaseActivityInstance, notNullValue());
		assertThat(historicCaseActivityInstance.Disabled, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionNoAuthenticatedTenants()
	  public virtual void reenableCaseExecutionNoAuthenticatedTenants()
	  {
		caseService.disableCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.reenableCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionWithAuthenticatedTenant()
	  public virtual void reenableCaseExecutionWithAuthenticatedTenant()
	  {
		caseService.disableCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.reenableCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		CaseExecution caseExecution = CaseExecution;

		assertThat(caseExecution.Enabled, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionDisabledTenantCheck()
	  public virtual void reenableCaseExecutionDisabledTenantCheck()
	  {
		caseService.disableCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.reenableCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		CaseExecution caseExecution = CaseExecution;

		assertThat(caseExecution.Enabled, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionNoAuthenticatedTenants()
	  public virtual void completeCaseExecutionNoAuthenticatedTenants()
	  {
		caseService.manuallyStartCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.completeCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionWithAuthenticatedTenant()
	  public virtual void completeCaseExecutionWithAuthenticatedTenant()
	  {
		caseService.manuallyStartCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.completeCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		HistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		assertThat(historicCaseActivityInstance, notNullValue());
		assertThat(historicCaseActivityInstance.Completed, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionDisabledTenantCheck()
	  public virtual void completeCaseExecutionDisabledTenantCheck()
	  {
		caseService.manuallyStartCaseExecution(caseExecutionId);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.completeCaseExecution(caseExecutionId);

		identityService.clearAuthentication();

		HistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		assertThat(historicCaseActivityInstance, notNullValue());
		assertThat(historicCaseActivityInstance.Completed, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceNoAuthenticatedTenants()
	  public virtual void closeCaseInstanceNoAuthenticatedTenants()
	  {
		caseService.completeCaseExecution(caseInstanceId);

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.closeCaseInstance(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceWithAuthenticatedTenant()
	  public virtual void closeCaseInstanceWithAuthenticatedTenant()
	  {
		caseService.completeCaseExecution(caseInstanceId);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.closeCaseInstance(caseInstanceId);

		identityService.clearAuthentication();

		HistoricCaseInstance historicCaseInstance = HistoricCaseInstance;

		assertThat(historicCaseInstance, notNullValue());
		assertThat(historicCaseInstance.Closed, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceDisabledTenantCheck()
	  public virtual void closeCaseInstanceDisabledTenantCheck()
	  {
		caseService.completeCaseExecution(caseInstanceId);

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.closeCaseInstance(caseInstanceId);

		identityService.clearAuthentication();

		HistoricCaseInstance historicCaseInstance = HistoricCaseInstance;

		assertThat(historicCaseInstance, notNullValue());
		assertThat(historicCaseInstance.Closed, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseInstanceNoAuthenticatedTenants()
	  public virtual void terminateCaseInstanceNoAuthenticatedTenants()
	  {

		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.terminateCaseExecution(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseExecutionWithAuthenticatedTenant()
	  public virtual void terminateCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.terminateCaseExecution(caseInstanceId);

		HistoricCaseInstance historicCaseInstance = HistoricCaseInstance;

		assertThat(historicCaseInstance, notNullValue());
		assertThat(historicCaseInstance.Terminated, @is(true));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseExecutionDisabledTenantCheck()
	  public virtual void terminateCaseExecutionDisabledTenantCheck()
	  {

		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.terminateCaseExecution(caseInstanceId);

		HistoricCaseInstance historicCaseInstance = HistoricCaseInstance;

		assertThat(historicCaseInstance, notNullValue());
		assertThat(historicCaseInstance.Terminated, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesNoAuthenticatedTenants()
	  public virtual void getVariablesNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case execution");

		caseService.getVariables(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesWithAuthenticatedTenant()
	  public virtual void getVariablesWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);

		assertThat(variables, notNullValue());
		assertThat(variables.Keys, hasItem(VARIABLE_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesDisabledTenantCheck()
	  public virtual void getVariablesDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);

		assertThat(variables, notNullValue());
		assertThat(variables.Keys, hasItem(VARIABLE_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableNoAuthenticatedTenants()
	  public virtual void getVariableNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case execution");

		caseService.getVariable(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableWithAuthenticatedTenant()
	  public virtual void getVariableWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		string variableValue = (string) caseService.getVariable(caseExecutionId, VARIABLE_NAME);

		assertThat(variableValue, @is(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableDisabledTenantCheck()
	  public virtual void getVariableDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		string variableValue = (string) caseService.getVariable(caseExecutionId, VARIABLE_NAME);

		assertThat(variableValue, @is(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedNoAuthenticatedTenants()
	  public virtual void getVariableTypedNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the case execution");

		caseService.getVariableTyped(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedWithAuthenticatedTenant()
	  public virtual void getVariableTypedWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		StringValue variable = caseService.getVariableTyped(caseExecutionId, VARIABLE_NAME);

		assertThat(variable.Value, @is(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedDisabledTenantCheck()
	  public virtual void getVariableTypedDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		StringValue variable = caseService.getVariableTyped(caseExecutionId, VARIABLE_NAME);

		assertThat(variable.Value, @is(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesNoAuthenticatedTenants()
	  public virtual void removeVariablesNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.removeVariable(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesWithAuthenticatedTenant()
	  public virtual void removeVariablesWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.removeVariable(caseExecutionId, VARIABLE_NAME);

		identityService.clearAuthentication();

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);
		assertThat(variables.Count == 0, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesDisabledTenantCheck()
	  public virtual void removeVariablesDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.removeVariable(caseExecutionId, VARIABLE_NAME);

		identityService.clearAuthentication();

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);
		assertThat(variables.Count == 0, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableNoAuthenticatedTenants()
	  public virtual void setVariableNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the case execution");

		caseService.setVariable(caseExecutionId, "newVar", "newValue");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableWithAuthenticatedTenant()
	  public virtual void setVariableWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		caseService.setVariable(caseExecutionId, "newVar", "newValue");

		identityService.clearAuthentication();

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);
		assertThat(variables, notNullValue());
		assertThat(variables.Keys, hasItems(VARIABLE_NAME, "newVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableDisabledTenantCheck()
	  public virtual void setVariableDisabledTenantCheck()
	  {
		identityService.setAuthentication("user", null, null);
		processEngineConfiguration.TenantCheckEnabled = false;

		caseService.setVariable(caseExecutionId, "newVar", "newValue");

		identityService.clearAuthentication();

		IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);
		assertThat(variables, notNullValue());
		assertThat(variables.Keys, hasItems(VARIABLE_NAME, "newVar"));
	  }

	  protected internal virtual string createCaseInstance(string tenantId)
	  {
		VariableMap variables = Variables.putValue(VARIABLE_NAME, VARIABLE_VALUE);
		CaseInstanceBuilder builder = caseService.withCaseDefinitionByKey("twoTaskCase").setVariables(variables);
		if (string.ReferenceEquals(tenantId, null))
		{
		  return builder.create().Id;
		}
		else
		{
		  return builder.caseDefinitionTenantId(tenantId).create().Id;
		}
	  }

	  protected internal virtual CaseExecution CaseExecution
	  {
		  get
		  {
			return caseService.createCaseExecutionQuery().activityId(ACTIVITY_ID).singleResult();
		  }
	  }

	  protected internal virtual HistoricCaseActivityInstance HistoricCaseActivityInstance
	  {
		  get
		  {
			return historyService.createHistoricCaseActivityInstanceQuery().caseActivityId(ACTIVITY_ID).singleResult();
		  }
	  }

	  protected internal virtual HistoricCaseInstance HistoricCaseInstance
	  {
		  get
		  {
			return historyService.createHistoricCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		  }
	  }

	}

}