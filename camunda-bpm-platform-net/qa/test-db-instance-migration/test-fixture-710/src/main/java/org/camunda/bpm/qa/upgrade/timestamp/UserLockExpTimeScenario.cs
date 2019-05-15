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
namespace org.camunda.bpm.qa.upgrade.timestamp
{
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using IdentityInfoManager = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoManager;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class UserLockExpTimeScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string USER_ID = "lockExpTimeTestUser";
	  protected internal const string PASSWORD = "testPassword";

	  [DescribesScenario("initUserLockExpirationTime"), Times(1)]
	  public static ScenarioSetup initUserLockExpirationTime()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string s)
		  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = processEngine.getIdentityService();
			IdentityService identityService = processEngine.IdentityService;

			User user = identityService.newUser(USER_ID);
			user.Password = PASSWORD;
			identityService.saveUser(user);

			((ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, identityService));
		  }

		  private class CommandAnonymousInnerClass : Command<Void>
		  {
			  private readonly ScenarioSetupAnonymousInnerClass outerInstance;

			  private IdentityService identityService;

			  public CommandAnonymousInnerClass(ScenarioSetupAnonymousInnerClass outerInstance, IdentityService identityService)
			  {
				  this.outerInstance = outerInstance;
				  this.identityService = identityService;
			  }

			  public Void execute(CommandContext context)
			  {
				IdentityInfoManager identityInfoManager = Context.CommandContext.getSession(typeof(IdentityInfoManager));

				UserEntity userEntity = (UserEntity) identityService.createUserQuery().userId(USER_ID).singleResult();

				identityInfoManager.updateUserLock(userEntity, 10, TIMESTAMP);
				return null;
			  }
		  }
	  }
	}
}