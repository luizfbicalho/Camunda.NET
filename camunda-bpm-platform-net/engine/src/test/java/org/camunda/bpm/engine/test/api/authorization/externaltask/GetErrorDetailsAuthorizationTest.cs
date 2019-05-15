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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using org.junit;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// Please note that if you want to reuse Rule and other fields you should create abstract class
	/// and pack it there.
	/// </summary>
	/// <seealso cref= HandleExternalTaskAuthorizationTest
	/// 
	/// @author Askar Akhmerov </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class GetErrorDetailsAuthorizationTest
	public class GetErrorDetailsAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetErrorDetailsAuthorizationTest()
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

	  private const string ERROR_DETAILS = "theDetails";
	  protected internal string deploymentId;
	  private string currentDetails;

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstanceId", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "oneExternalTaskProcess", "userId", Permissions.READ_INSTANCE)), scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "processInstanceId", "userId", Permissions.READ)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.READ)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinitionKey", "userId", Permissions.READ_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.READ_INSTANCE)).succeeds());
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
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testCompleteExternalTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteExternalTask()
	  {

		// given
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> tasks = engineRule.ExternalTaskService.fetchAndLock(5, "workerId").topic("externalTaskTopic", 5000L).execute();

		LockedExternalTask task = tasks[0];

		//preconditions method
		engineRule.ExternalTaskService.handleFailure(task.Id,task.WorkerId,"anError",ERROR_DETAILS,1,1000L);

		// when
		authRule.init(scenario).withUser("userId").bindResource("processInstanceId", processInstance.Id).bindResource("processDefinitionKey", "oneExternalTaskProcess").start();

		//execution method
		currentDetails = engineRule.ExternalTaskService.getExternalTaskErrorDetails(task.Id);

		// then
		if (authRule.assertScenario(scenario))
		{
		  //assertion method
		  assertThat(currentDetails,@is(ERROR_DETAILS));
		}
	  }

	}

}