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

	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
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
	public class MultiTenancyExecutionVariableCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyExecutionVariableCmdsTenantCheckTest()
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

	  protected internal const string VARIABLE_1 = "testVariable1";
	  protected internal const string VARIABLE_2 = "testVariable2";

	  protected internal const string VARIABLE_VALUE_1 = "test1";
	  protected internal const string VARIABLE_VALUE_2 = "test2";

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string processInstanceId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(VARIABLE_1, VARIABLE_VALUE_1).putValue(VARIABLE_2, VARIABLE_VALUE_2)).Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableWithAuthenticatedTenant()
	  public virtual void getExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.RuntimeService.getVariable(processInstanceId, VARIABLE_1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableWithNoAuthenticatedTenant()
	  public virtual void getExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.getVariable(processInstanceId, VARIABLE_1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableWithDisabledTenantCheck()
	  public virtual void getExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.RuntimeService.getVariable(processInstanceId, VARIABLE_1));

	  }

	  // get typed execution variable
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableTypedWithAuthenticatedTenant()
	  public virtual void getExecutionVariableTypedWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.RuntimeService.getVariableTyped(processInstanceId, VARIABLE_1).Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableTypedWithNoAuthenticatedTenant()
	  public virtual void getExecutionVariableTypedWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// then
		engineRule.RuntimeService.getVariableTyped(processInstanceId, VARIABLE_1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariableTypedWithDisabledTenantCheck()
	  public virtual void getExecutionVariableTypedWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// if
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.RuntimeService.getVariableTyped(processInstanceId, VARIABLE_1).Value);

	  }

	  // get execution variables
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariablesWithAuthenticatedTenant()
	  public virtual void getExecutionVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(2, engineRule.RuntimeService.getVariables(processInstanceId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariablesWithNoAuthenticatedTenant()
	  public virtual void getExecutionVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.getVariables(processInstanceId).Count;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getExecutionVariablesWithDisabledTenantCheck()
	  public virtual void getExecutionVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(2, engineRule.RuntimeService.getVariables(processInstanceId).Count);

	  }

	  // set execution variable
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setExecutionVariableWithAuthenticatedTenant()
	  public virtual void setExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		engineRule.RuntimeService.setVariable(processInstanceId, "newVariable", "newValue");
		assertEquals(3, engineRule.RuntimeService.getVariables(processInstanceId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setExecutionVariableWithNoAuthenticatedTenant()
	  public virtual void setExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.setVariable(processInstanceId, "newVariable", "newValue");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setExecutionVariableWithDisabledTenantCheck()
	  public virtual void setExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.RuntimeService.setVariable(processInstanceId, "newVariable", "newValue");
		assertEquals(3, engineRule.RuntimeService.getVariables(processInstanceId).Count);
	  }

	  // remove execution variable
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeExecutionVariableWithAuthenticatedTenant()
	  public virtual void removeExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		engineRule.RuntimeService.removeVariable(processInstanceId, VARIABLE_1);

		// then
		assertEquals(1, engineRule.RuntimeService.getVariables(processInstanceId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeExecutionVariableWithNoAuthenticatedTenant()
	  public virtual void removeExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.removeVariable(processInstanceId, VARIABLE_1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeExecutionVariableWithDisabledTenantCheck()
	  public virtual void removeExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		engineRule.RuntimeService.removeVariable(processInstanceId, VARIABLE_1);

		// then
		assertEquals(1, engineRule.RuntimeService.getVariables(processInstanceId).Count);
	  }
	}

}