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
namespace org.camunda.bpm.engine.test.api.authorization.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;


	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class SetExternalTasksRetriesAuthorizationTest
	public class SetExternalTasksRetriesAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetExternalTasksRetriesAuthorizationTest()
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "processInstanceId2", "userId", Permissions.UPDATE)).withoutAuthorizations().failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstanceId1", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "oneExternalTaskProcess", "userId", Permissions.UPDATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "processInstanceId1", "userId", Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstanceId2", "userId", Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.UPDATE_INSTANCE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testSetRetrieSync()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetrieSync()
	  {

		// given
		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = engineRule.RuntimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<ExternalTask> tasks = engineRule.ExternalTaskService.createExternalTaskQuery().list();

		// when
		authRule.init(scenario).withUser("userId").bindResource("processInstanceId1", processInstance1.Id).bindResource("processInstanceId2", processInstance2.Id).bindResource("processDefinitionKey", "oneExternalTaskProcess").start();

		List<string> externalTaskIds = new List<string>();
		externalTaskIds.Add(tasks[0].Id);
		externalTaskIds.Add(tasks[1].Id);

		engineRule.ExternalTaskService.setRetries(externalTaskIds, 5);

		// then
		if (authRule.assertScenario(scenario))
		{
		  tasks = engineRule.ExternalTaskService.createExternalTaskQuery().list();
		  Assert.assertEquals(5, (int) tasks[0].Retries);
		  Assert.assertEquals(5, (int) tasks[1].Retries);
		}

	  }
	}
}