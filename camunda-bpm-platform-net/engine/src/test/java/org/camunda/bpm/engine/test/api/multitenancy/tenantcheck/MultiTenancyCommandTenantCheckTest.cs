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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	public class MultiTenancyCommandTenantCheckTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService = engineRule.IdentityService;

		identityService.setAuthentication("user", null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableTenantCheckForProcessEngine()
	  public virtual void disableTenantCheckForProcessEngine()
	  {
		// disable tenant check for process engine
		processEngineConfiguration.TenantCheckEnabled = false;

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			// cannot enable tenant check for command when it is disabled for process engine
			commandContext.enableTenantCheck();
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(false));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableTenantCheckForCommand()
	  public virtual void disableTenantCheckForCommand()
	  {

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass2(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			// disable tenant check for the current command
			commandContext.disableTenantCheck();
			assertThat(commandContext.TenantCheckEnabled, @is(false));
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(false));

			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass3(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			// assert that it is enabled again for further commands
			assertThat(commandContext.TenantCheckEnabled, @is(true));
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(true));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableAndEnableTenantCheckForCommand()
	  public virtual void disableAndEnableTenantCheckForCommand()
	  {

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass4(this));
	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass4(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {

			commandContext.disableTenantCheck();
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(false));

			commandContext.enableTenantCheck();
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(true));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableTenantCheckForCamundaAdmin()
	  public virtual void disableTenantCheckForCamundaAdmin()
	  {
		identityService.setAuthentication("user", Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), null);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass5(this));
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass5(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			// camunda-admin should access data from all tenants
			assertThat(commandContext.TenantManager.TenantCheckEnabled, @is(false));

			return null;
		  }
	  }

	}

}