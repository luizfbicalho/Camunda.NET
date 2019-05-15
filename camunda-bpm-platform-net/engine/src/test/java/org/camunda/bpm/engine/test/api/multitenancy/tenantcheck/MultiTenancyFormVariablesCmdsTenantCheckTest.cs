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

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
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

	public class MultiTenancyFormVariablesCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyFormVariablesCmdsTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal const string VARIABLE_1 = "testVariable1";
	  protected internal const string VARIABLE_2 = "testVariable2";

	  protected internal const string VARIABLE_VALUE_1 = "test1";
	  protected internal const string VARIABLE_VALUE_2 = "test2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessInstance instance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal const string START_FORM_RESOURCE = "org/camunda/bpm/engine/test/api/form/FormServiceTest.startFormFields.bpmn20.xml";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		// deploy tenants
		testRule.deployForTenant(TENANT_ONE, START_FORM_RESOURCE);
		instance = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(VARIABLE_1, VARIABLE_VALUE_1).putValue(VARIABLE_2, VARIABLE_VALUE_2));
	  }

	  // start form variables
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesWithAuthenticatedTenant()
	  public virtual void testGetStartFormVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		assertEquals(4, engineRule.FormService.getStartFormVariables(instance.ProcessDefinitionId).size());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesWithNoAuthenticatedTenant()
	  public virtual void testGetStartFormVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot get the process definition '" + instance.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		engineRule.FormService.getStartFormVariables(instance.ProcessDefinitionId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesWithDisabledTenantCheck()
	  public virtual void testGetStartFormVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		assertEquals(4, engineRule.FormService.getStartFormVariables(instance.ProcessDefinitionId).size());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithAuthenticatedTenant()
	  public virtual void testGetTaskFormVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		assertEquals(2, engineRule.FormService.getTaskFormVariables(task.Id).size());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithNoAuthenticatedTenant()
	  public virtual void testGetTaskFormVariablesWithNoAuthenticatedTenant()
	  {

		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		engineRule.FormService.getTaskFormVariables(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithDisabledTenantCheck()
	  public virtual void testGetTaskFormVariablesWithDisabledTenantCheck()
	  {

		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		assertEquals(2, engineRule.FormService.getTaskFormVariables(task.Id).size());

	  }
	}

}