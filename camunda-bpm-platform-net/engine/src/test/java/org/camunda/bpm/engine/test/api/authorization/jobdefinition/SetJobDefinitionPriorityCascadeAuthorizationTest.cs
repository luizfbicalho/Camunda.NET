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
namespace org.camunda.bpm.engine.test.api.authorization.jobdefinition
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class SetJobDefinitionPriorityCascadeAuthorizationTest
	public class SetJobDefinitionPriorityCascadeAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetJobDefinitionPriorityCascadeAuthorizationTest()
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
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstanceId", "userId", Permissions.UPDATE)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.UPDATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.UPDATE, Permissions.UPDATE_INSTANCE)).succeeds());
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
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml") public void testSetJobDefinitionPriority()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriority()
	  {

		// given
		JobDefinition jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().singleResult();
		ProcessInstance instance = engineRule.RuntimeService.startProcessInstanceByKey("process");
		Job job = engineRule.ManagementService.createJobQuery().singleResult();

		// when
		authRule.init(scenario).withUser("userId").bindResource("processDefinitionKey", "process").bindResource("processInstanceId", instance.Id).start();

		engineRule.ManagementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42, true);

		// then
		if (authRule.assertScenario(scenario))
		{
		  jobDefinition = engineRule.ManagementService.createJobDefinitionQuery().singleResult();
		  Assert.assertEquals(42L, (long) jobDefinition.OverridingJobPriority);

		  job = engineRule.ManagementService.createJobQuery().singleResult();
		  Assert.assertEquals(42L, job.Priority);
		}

	  }

	}

}