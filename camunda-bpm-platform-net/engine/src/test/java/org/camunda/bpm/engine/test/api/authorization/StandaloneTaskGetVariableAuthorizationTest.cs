﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.READ_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Yana.Vasileva
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class StandaloneTaskGetVariableAuthorizationTest
	public class StandaloneTaskGetVariableAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public StandaloneTaskGetVariableAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(authRule);
		}



	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal TaskService taskService;
	  protected internal RuntimeService runtimeService;

	  protected internal const string userId = "userId";
	  protected internal string taskId = "myTask";
	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal bool ensureSpecificVariablePermission;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(TASK, "taskId", userId, READ_VARIABLE)), scenario().withAuthorizations(grant(TASK, "taskId", userId, READ_VARIABLE)), scenario().withAuthorizations(grant(TASK, "*", userId, READ_VARIABLE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;

		authRule.createUserAndGroup("userId", "groupId");
		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;
		// prerequisite of the whole test suite
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		taskService.deleteTask(taskId, true);
		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariable()
	  public virtual void testGetVariable()
	  {
		// given
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		object variable = taskService.getVariable(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(VARIABLE_VALUE, variable);
		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableLocal()
	  public virtual void testGetVariableLocal()
	  {
		// given
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		object variable = taskService.getVariableLocal(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(VARIABLE_VALUE, variable);
		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableTyped()
	  public virtual void testGetVariableTyped()
	  {
		// given
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		TypedValue typedValue = taskService.getVariableTyped(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertNotNull(typedValue);
		  assertEquals(VARIABLE_VALUE, typedValue.Value);
		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableLocalTyped()
	  public virtual void testGetVariableLocalTyped()
	  {
		// given
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		TypedValue typedValue = taskService.getVariableLocalTyped(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertNotNull(typedValue);
		  assertEquals(VARIABLE_VALUE, typedValue.Value);
		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariables()
	  public virtual void testGetVariables()
	  {
		// given
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		IDictionary<string, object> variables = taskService.getVariables(taskId);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocal()
	  public virtual void testGetVariablesLocal()
	  {
		// given
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		IDictionary<string, object> variables = taskService.getVariablesLocal(taskId);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesTyped()
	  public virtual void testGetVariablesTyped()
	  {
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		VariableMap variables = taskService.getVariablesTyped(taskId);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocalTyped()
	  public virtual void testGetVariablesLocalTyped()
	  {
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		VariableMap variables = taskService.getVariablesLocalTyped(taskId);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesByName()
	  public virtual void testGetVariablesByName()
	  {
		// given
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		IDictionary<string, object> variables = taskService.getVariables(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocalByName()
	  public virtual void testGetVariablesLocalByName()
	  {
		// given
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		IDictionary<string, object> variables = taskService.getVariablesLocal(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesTypedByName()
	  public virtual void testGetVariablesTypedByName()
	  {
		createTask(taskId);

		taskService.setVariables(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		VariableMap variables = taskService.getVariablesTyped(taskId, Arrays.asList(VARIABLE_NAME), false);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocalTypedByName()
	  public virtual void testGetVariablesLocalTypedByName()
	  {
		createTask(taskId);

		taskService.setVariablesLocal(taskId, Variables);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		VariableMap variables = taskService.getVariablesLocalTyped(taskId, Arrays.asList(VARIABLE_NAME), false);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyGetVariables(variables);

		  deleteAuthorizations();
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void createTask(final String taskId)
	  protected internal virtual void createTask(string taskId)
	  {
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);
	  }

	  protected internal virtual VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue(VARIABLE_NAME, VARIABLE_VALUE);
		  }
	  }

	  protected internal virtual void deleteAuthorizations()
	  {
		AuthorizationService authorizationService = engineRule.AuthorizationService;
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  protected internal virtual void verifyGetVariables(IDictionary<string, object> variables)
	  {
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);
		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	}

}