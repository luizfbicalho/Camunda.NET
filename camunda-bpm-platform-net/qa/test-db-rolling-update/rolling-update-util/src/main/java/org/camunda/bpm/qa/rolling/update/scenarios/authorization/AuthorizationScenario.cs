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
namespace org.camunda.bpm.qa.rolling.update.scenarios.authorization
{
	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using DescribesScenario = org.camunda.bpm.qa.upgrade.DescribesScenario;
	using ScenarioSetup = org.camunda.bpm.qa.upgrade.ScenarioSetup;
	using Times = org.camunda.bpm.qa.upgrade.Times;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class AuthorizationScenario
	{

	  protected internal const string USER_ID = "user";
	  protected internal const string GROUP_ID = "group";
	  public const string PROCESS_DEF_KEY = "oneTaskProcess";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployOneTaskProcess()
	  public static string deployOneTaskProcess()
	  {
		return "org/camunda/bpm/qa/rolling/update/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("startProcessInstance"), Times(1)]
	  public static ScenarioSetup startProcessInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			IdentityService identityService = engine.IdentityService;

			string userId = USER_ID + scenarioName;
			string groupid = GROUP_ID + scenarioName;
			// create an user
			User user = identityService.newUser(userId);
			identityService.saveUser(user);

			// create group
			Group group = identityService.newGroup(groupid);
			identityService.saveGroup(group);

			// create membership
			identityService.createMembership(userId, groupid);

			//create full authorization
			AuthorizationService authorizationService = engine.AuthorizationService;

			//authorization for process definition
			Authorization authProcDef = createAuthorization(authorizationService, Permissions.ALL, Resources.PROCESS_DEFINITION, userId);
			engine.AuthorizationService.saveAuthorization(authProcDef);

			//authorization for deployment
			Authorization authDeployment = createAuthorization(authorizationService, Permissions.ALL, Resources.DEPLOYMENT, userId);
			engine.AuthorizationService.saveAuthorization(authDeployment);

			//authorization for process instance create
			Authorization authProcessInstance = createAuthorization(authorizationService, Permissions.CREATE, Resources.PROCESS_INSTANCE, userId);
			engine.AuthorizationService.saveAuthorization(authProcessInstance);

			// start a process instance
			engine.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, scenarioName);
		  }
	  }

	  protected internal static Authorization createAuthorization(AuthorizationService authorizationService, Permission permission, Resources resource, string userId)
	  {
		Authorization auth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		auth.addPermission(permission);
		auth.Resource = resource;
		auth.ResourceId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		auth.UserId = userId;
		return auth;
	  }
	}

}