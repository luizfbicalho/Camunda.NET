using System;

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
namespace org.camunda.bpm.engine.impl.interceptor
{

	using ProcessEngineServicesAware = org.camunda.bpm.engine.@delegate.ProcessEngineServicesAware;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// <para>Interceptor used for opening the <seealso cref="CommandContext"/> and <seealso cref="CommandInvocationContext"/>.</para>
	/// 
	/// <para>Since 7.1, this interceptor will not always open a new command context but instead reuse an existing
	/// command context if possible. This is required for supporting process engine public API access from
	/// delegation code (see <seealso cref="ProcessEngineServicesAware"/>.). However, for every command, a new
	/// command invocation context is created. While a command context holds resources that are
	/// shared between multiple commands, such as database sessions, a command invocation context holds
	/// resources specific for a single command.</para>
	/// 
	/// <para>The interceptor will check whether an open command context exists. If true, it will reuse the
	/// command context. If false, it will open a new one. We will always push the context to the
	/// <seealso cref="Context"/> stack. So ins some situations, you will see the same context being pushed to the sack
	/// multiple times. The rationale is that the size of  the stack should allow you to determine whether
	/// you are currently running an 'inner' command or an 'outer' command as well as your current stack size.
	/// Existing code may rely on this behavior.</para>
	/// 
	/// <para>The interceptor can be configured using the property <seealso cref="alwaysOpenNew"/>.
	/// If this property is set to true, we will always open a new context regardless whether there already
	/// exists an active context or not. This is required for properly supporting REQUIRES_NEW semantics for
	/// commands run through the <seealso cref="ProcessEngineConfigurationImpl.getCommandInterceptorsTxRequiresNew()"/>
	/// chain. In that context the 'inner' command must be able to succeed / fail independently from the
	/// 'outer' command.</para>
	/// 
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class CommandContextInterceptor : CommandInterceptor
	{

	  private static readonly CommandLogger LOG = CommandLogger.CMD_LOGGER;

	  protected internal CommandContextFactory commandContextFactory;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  /// <summary>
	  /// if true, we will always open a new command context </summary>
	  protected internal bool alwaysOpenNew;

	  public CommandContextInterceptor()
	  {
	  }

	  public CommandContextInterceptor(CommandContextFactory commandContextFactory, ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		this.commandContextFactory = commandContextFactory;
		this.processEngineConfiguration = processEngineConfiguration;
	  }

	  public CommandContextInterceptor(CommandContextFactory commandContextFactory, ProcessEngineConfigurationImpl processEngineConfiguration, bool alwaysOpenNew) : this(commandContextFactory, processEngineConfiguration)
	  {
		this.alwaysOpenNew = alwaysOpenNew;
	  }

	  public override T execute<T>(Command<T> command)
	  {
		CommandContext context = null;

		if (!alwaysOpenNew)
		{
		  // check whether we can reuse the command context
		  CommandContext existingCommandContext = Context.CommandContext;
		  if (existingCommandContext != null && isFromSameEngine(existingCommandContext))
		  {
			context = existingCommandContext;
		  }
		}

		bool openNew = (context == null);

		CommandInvocationContext commandInvocationContext = new CommandInvocationContext(command);
		Context.CommandInvocationContext = commandInvocationContext;

		try
		{
		  if (openNew)
		  {
			LOG.debugOpeningNewCommandContext();
			context = commandContextFactory.createCommandContext();

		  }
		  else
		  {
			LOG.debugReusingExistingCommandContext();

		  }

		  Context.CommandContext = context;
		  Context.ProcessEngineConfiguration = processEngineConfiguration;

		  // delegate to next interceptor in chain
		  return next.execute(command);

		}
		catch (Exception t)
		{
		  commandInvocationContext.trySetThrowable(t);

		}
		finally
		{
		  try
		  {
			if (openNew)
			{
			  LOG.closingCommandContext();
			  context.close(commandInvocationContext);
			}
			else
			{
			  commandInvocationContext.rethrow();
			}
		  }
		  finally
		  {
			Context.removeCommandInvocationContext();
			Context.removeCommandContext();
			Context.removeProcessEngineConfiguration();
		  }
		}

		return default(T);
	  }

	  protected internal virtual bool isFromSameEngine(CommandContext existingCommandContext)
	  {
		return processEngineConfiguration == existingCommandContext.ProcessEngineConfiguration;
	  }

	  public virtual CommandContextFactory CommandContextFactory
	  {
		  get
		  {
			return commandContextFactory;
		  }
		  set
		  {
			this.commandContextFactory = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl ProcessEngineContext
	  {
		  set
		  {
			this.processEngineConfiguration = value;
		  }
	  }
	}

}