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
namespace org.camunda.bpm.engine.test.api.authorization
{
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using User = org.camunda.bpm.engine.identity.User;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class TaskCountByCandidateGroupAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskCountByCandidateGroupAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}


	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  public RuleChain ruleChain;


	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;
	  protected internal AuthorizationService authorizationService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string userId = "user";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		taskService = processEngineRule.TaskService;
		identityService = processEngineRule.IdentityService;
		authorizationService = processEngineRule.AuthorizationService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchTaskCountWithAuthorization()
	  public virtual void shouldFetchTaskCountWithAuthorization()
	  {
		// given
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.addPermission(READ);
		authorization.Resource = TASK;
		authorization.ResourceId = ANY;
		authorization.UserId = userId;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;
		authenticate();

		// when
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();
		processEngineConfiguration.AuthorizationEnabled = false;
		authorizationService.deleteAuthorization(authorization.Id);
		identityService.deleteUser(userId);

		assertEquals(0, results.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailToFetchTaskCountWithMissingAuthorization()
	  public virtual void shouldFailToFetchTaskCountWithMissingAuthorization()
	  {
		// given
		bool testFailed = false;
		processEngineConfiguration.AuthorizationEnabled = true;
		authenticate();

		// when
		try
		{
		  taskService.createTaskReport().taskCountByCandidateGroup();
		  testFailed = true;

		}
		catch (AuthorizationException aex)
		{
		  if (!aex.Message.contains(userId + "' does not have 'READ' permission on resource '*' of type 'Task'"))
		  {
			testFailed = true;
		  }
		}

		// then
		processEngineConfiguration.AuthorizationEnabled = false;

		if (testFailed)
		{
		  fail("There should be an authorization exception for '" + userId + "' because of a missing 'READ' permission on 'Task'.");
		}
	  }

	  protected internal virtual void authenticate()
	  {
		identityService.setAuthentication(userId, null, null);
	  }
	}

}