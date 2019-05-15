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
namespace org.camunda.bpm.engine.test.api.authorization.task.updatevariable
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.UPDATE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
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
//ORIGINAL LINE: @RunWith(Parameterized.class) public class StandaloneTaskAuthorizationTest
	public class StandaloneTaskAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public StandaloneTaskAuthorizationTest()
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
	  protected internal HistoryService historyService;

	  protected internal const string userId = "userId";
	  protected internal string taskId = "myTask";
	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(TASK, "taskId", userId, UPDATE), grant(TASK, "taskId", userId, UPDATE_VARIABLE)), scenario().withAuthorizations(grant(TASK, "taskId", userId, UPDATE)), scenario().withAuthorizations(grant(TASK, "*", userId, UPDATE)), scenario().withAuthorizations(grant(TASK, "taskId", userId, UPDATE_VARIABLE)), scenario().withAuthorizations(grant(TASK, "*", userId, UPDATE_VARIABLE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;

		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		taskService.deleteTask(taskId, true);
		foreach (HistoricVariableInstance var in historyService.createHistoricVariableInstanceQuery().includeDeleted().list())
		{
		  historyService.deleteHistoricVariableInstance(var.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariable()
	  public virtual void testSetVariable()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableLocal()
	  public virtual void testSetVariableLocal()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariables()
	  public virtual void testSetVariables()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.setVariables(taskId, Variables);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariablesLocal()
	  public virtual void testSetVariablesLocal()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.setVariablesLocal(taskId, Variables);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariable()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariable()
	  {
		// given
		createTask(taskId);

		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariable(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariableLocal()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariableLocal()
	  {
		// given
		createTask(taskId);

		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariableLocal(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariables()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariables()
	  {
		// given
		createTask(taskId);

		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariables(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariablesLocal()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariablesLocal()
	  {
		// given
		createTask(taskId);

		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariablesLocal(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesAdd()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesAdd()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariables(taskId, Variables, null);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesRemove()
	  {
		// given
		createTask(taskId);
		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariables(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesAddRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesAddRemove()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariables(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesLocalAdd()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesLocalAdd()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, Variables, null);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifySetVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesLocalRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesLocalRemove()
	  {
		// given
		createTask(taskId);
		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesLocalAddRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesLocalAddRemove()
	  {
		// given
		createTask(taskId);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariable();
		}
	  }

	  protected internal virtual void verifySetVariables()
	  {
		verifyVariableInstanceCount(1);
		assertNotNull(runtimeService.createVariableInstanceQuery().singleResult());
	  }

	  protected internal virtual void verifyRemoveVariable()
	  {
		verifyVariableInstanceCount(0);
		assertNull(runtimeService.createVariableInstanceQuery().singleResult());
		HistoricVariableInstance deletedVariable = historyService.createHistoricVariableInstanceQuery().includeDeleted().singleResult();
		Assert.assertEquals("DELETED", deletedVariable.State);
	  }

	  protected internal virtual void verifyVariableInstanceCount(int count)
	  {
		assertEquals(count, runtimeService.createVariableInstanceQuery().list().size());
		assertEquals(count, runtimeService.createVariableInstanceQuery().count());
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

	}

}