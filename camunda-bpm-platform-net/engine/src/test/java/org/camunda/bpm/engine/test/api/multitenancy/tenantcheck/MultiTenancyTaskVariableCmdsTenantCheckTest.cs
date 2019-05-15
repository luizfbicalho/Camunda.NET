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

	public class MultiTenancyTaskVariableCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskVariableCmdsTenantCheckTest()
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

	  protected internal static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask("task").endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal string taskId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(VARIABLE_1, VARIABLE_VALUE_1).putValue(VARIABLE_2, VARIABLE_VALUE_2)).Id;

		taskId = engineRule.TaskService.createTaskQuery().singleResult().Id;

	  }

	  // get task variable
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableWithAuthenticatedTenant()
	  public virtual void getTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		assertEquals(VARIABLE_VALUE_1, engineRule.TaskService.getVariable(taskId, VARIABLE_1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableWithNoAuthenticatedTenant()
	  public virtual void getTaskVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.getVariable(taskId, VARIABLE_1);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableWithDisabledTenantCheck()
	  public virtual void getTaskVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.TaskService.getVariable(taskId, VARIABLE_1));
	  }

	  // get task variable typed
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableTypedWithAuthenticatedTenant()
	  public virtual void getTaskVariableTypedWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.TaskService.getVariableTyped(taskId, VARIABLE_1).Value);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableTypedWithNoAuthenticatedTenant()
	  public virtual void getTaskVariableTypedWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.getVariableTyped(taskId, VARIABLE_1).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariableTypedWithDisableTenantCheck()
	  public virtual void getTaskVariableTypedWithDisableTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(VARIABLE_VALUE_1, engineRule.TaskService.getVariableTyped(taskId, VARIABLE_1).Value);
	  }

	  // get task variables
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariablesWithAuthenticatedTenant()
	  public virtual void getTaskVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(2, engineRule.TaskService.getVariables(taskId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariablesWithNoAuthenticatedTenant()
	  public virtual void getTaskVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.getVariables(taskId).Count;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTaskVariablesWithDisabledTenantCheck()
	  public virtual void getTaskVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		assertEquals(2, engineRule.TaskService.getVariables(taskId).Count);
	  }


	  // set variable test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setTaskVariableWithAuthenticatedTenant()
	  public virtual void setTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		engineRule.TaskService.setVariable(taskId, "newVariable", "newValue");

		assertEquals(3, engineRule.TaskService.getVariables(taskId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setTaskVariableWithNoAuthenticatedTenant()
	  public virtual void setTaskVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.setVariable(taskId, "newVariable", "newValue");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setTaskVariableWithDisabledTenantCheck()
	  public virtual void setTaskVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		engineRule.TaskService.setVariable(taskId, "newVariable", "newValue");
		assertEquals(3, engineRule.TaskService.getVariables(taskId).Count);

	  }

	  // remove variable test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeTaskVariableWithAuthenticatedTenant()
	  public virtual void removeTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		engineRule.TaskService.removeVariable(taskId, VARIABLE_1);
		// then
		assertEquals(1, engineRule.TaskService.getVariables(taskId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeTaskVariablesWithNoAuthenticatedTenant()
	  public virtual void removeTaskVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the task '" + taskId + "' because it belongs to no authenticated tenant.");

		engineRule.TaskService.removeVariable(taskId, VARIABLE_1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeTaskVariablesWithDisabledTenantCheck()
	  public virtual void removeTaskVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		engineRule.TaskService.removeVariable(taskId, VARIABLE_1);
		assertEquals(1, engineRule.TaskService.getVariables(taskId).Count);
	  }

	}

}