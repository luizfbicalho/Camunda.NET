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
namespace org.camunda.bpm.qa.upgrade.scenarios.authorization
{
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class AuthorizationScenario
	{

	  protected internal string user = "test";
	  protected internal string group = "accounting";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployOneTaskProcess()
	  public static string deployOneTaskProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/authorization/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("startProcessInstance")]
	  public static ScenarioSetup startProcessInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			IdentityService identityService = engine.IdentityService;

			// create an user
			string userId = "test";
			User user = identityService.newUser(userId);
			identityService.saveUser(user);

			// create group
			string groupId = "accounting";
			Group group = identityService.newGroup(groupId);
			identityService.saveGroup(group);

			// create membership
			identityService.createMembership("test", "accounting");

			// start a process instance
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", scenarioName);
		  }
	  }

	  // user ////////////////////////////////////////////////////////////////

	  protected internal virtual User createUser(IdentityService identityService, string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);
		return user;
	  }

	  // group //////////////////////////////////////////////////////////////

	  protected internal virtual Group createGroup(IdentityService identityService, string groupId)
	  {
		Group group = identityService.newGroup(groupId);
		identityService.saveGroup(group);
		return group;
	  }

	}

}