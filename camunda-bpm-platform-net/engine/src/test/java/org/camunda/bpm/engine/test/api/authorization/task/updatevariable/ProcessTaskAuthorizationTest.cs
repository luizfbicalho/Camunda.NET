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
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
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
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ProcessTaskAuthorizationTest
	public class ProcessTaskAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessTaskAuthorizationTest()
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


	  private const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string userId = "userId";
	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";

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

	  protected internal bool ensureSpecificVariablePermission;
	  protected internal string deploumentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(TASK, "taskId", userId, UPDATE), grant(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK), grant(TASK, "taskId", userId, UPDATE_VARIABLE), grant(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK_VARIABLE)), scenario().withAuthorizations(grant(TASK, "taskId", userId, UPDATE)), scenario().withAuthorizations(grant(TASK, "*", userId, UPDATE)), scenario().withAuthorizations(grant(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK)), scenario().withAuthorizations(grant(PROCESS_DEFINITION, "*", userId, UPDATE_TASK)), scenario().withAuthorizations(grant(TASK, "taskId", userId, UPDATE_VARIABLE)), scenario().withAuthorizations(grant(TASK, "*", userId, UPDATE_VARIABLE)), scenario().withAuthorizations(grant(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK_VARIABLE)), scenario().withAuthorizations(grant(PROCESS_DEFINITION, "*", userId, UPDATE_TASK_VARIABLE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;

		authRule.createUserAndGroup("userId", "groupId");
		deploumentId = engineRule.RepositoryService.createDeployment().addClasspathResource(ONE_TASK_PROCESS).deployWithResult().Id;
		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;

		// prerequisite of the whole test suite
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
		engineRule.RepositoryService.deleteDeployment(deploumentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariable()
	  public virtual void testSetVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariable(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariableLocal()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariableLocal()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariableLocal(taskId, VARIABLE_NAME);

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariables()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariables()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariables(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testRemoveVariablesLocal()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testRemoveVariablesLocal()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		taskService.removeVariablesLocal(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesAdd()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesAdd()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariables(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesAddRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesAddRemove()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariables(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesLocalAdd()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesLocalAdd()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

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
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUpdateVariablesLocalAddRemove()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUpdateVariablesLocalAddRemove()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		authRule.init(scenario).withUser("userId").bindResource("taskId", taskId).start();

		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then
		if (authRule.assertScenario(scenario))
		{
		  verifyRemoveVariables();
		}
	  }

	  protected internal virtual void verifySetVariables()
	  {
		verifyVariableInstanceCount(1);
		assertNotNull(runtimeService.createVariableInstanceQuery().singleResult());
	  }

	  protected internal virtual void verifyRemoveVariables()
	  {
		verifyVariableInstanceCount(0);
		assertNull(runtimeService.createVariableInstanceQuery().singleResult());
		HistoricVariableInstance deletedVariable = engineRule.HistoryService.createHistoricVariableInstanceQuery().includeDeleted().singleResult();
		Assert.assertEquals("DELETED", deletedVariable.State);
	  }

	  protected internal virtual void verifyVariableInstanceCount(int count)
	  {
		assertEquals(count, runtimeService.createVariableInstanceQuery().list().size());
		assertEquals(count, runtimeService.createVariableInstanceQuery().count());
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