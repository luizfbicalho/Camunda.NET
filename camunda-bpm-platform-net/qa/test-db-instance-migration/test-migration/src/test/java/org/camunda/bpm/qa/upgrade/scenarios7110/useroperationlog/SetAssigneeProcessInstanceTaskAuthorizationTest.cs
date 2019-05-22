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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.useroperationlog
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Yana.Vasileva
	/// 
	/// </summary>
	public class SetAssigneeProcessInstanceTaskAuthorizationTest
	{

	  private const string USER_ID = "jane" + "SetAssigneeProcessInstanceTask";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
	  public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

	  protected internal HistoryService historyService;
	  protected internal AuthorizationService authorizationService;
	  protected internal ProcessEngineConfigurationImpl engineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void assignServices()
	  public virtual void assignServices()
	  {
		historyService = engineRule.HistoryService;
		authorizationService = engineRule.AuthorizationService;
		engineConfiguration = engineRule.ProcessEngineConfiguration;
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = false;
		engineRule.IdentityService.clearAuthentication();
		IList<Authorization> auths = authorizationService.createAuthorizationQuery().userIdIn(USER_ID).list();
		foreach (Authorization authorization in auths)
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutAuthorization()
	  public virtual void testWithoutAuthorization()
	  {
		// given
		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processDefinitionKey("oneTaskProcess_userOpLog");

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithReadHistoryPermissionOnAnyProcessDefinition()
	  public virtual void testWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		Authorization auth = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		auth.UserId = USER_ID;
		auth.Permissions = new Permissions[] {Permissions.READ_HISTORY};
		auth.Resource = Resources.PROCESS_DEFINITION;
		auth.ResourceId = "*";

		authorizationService.saveAuthorization(auth);
		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;
		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processDefinitionKey("oneTaskProcess_userOpLog");

		// then
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithReadHistoryPermissionOnProcessDefinition()
	  public virtual void testWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		Authorization auth = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		auth.UserId = USER_ID;
		auth.Permissions = new Permissions[] {Permissions.READ_HISTORY};
		auth.Resource = Resources.PROCESS_DEFINITION;
		auth.ResourceId = "oneTaskProcess_userOpLog";

		authorizationService.saveAuthorization(auth);
		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;
		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processDefinitionKey("oneTaskProcess_userOpLog");

		// then
		assertEquals(1, query.count());
	  }
	}

}