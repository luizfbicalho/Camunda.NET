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
namespace org.camunda.bpm.engine.test.standalone.interceptor
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	public class CommandInvocationContextTest : PluggableProcessEngineTestCase
	{

	  /// <summary>
	  /// Test that the command invocation context always holds the correct command;
	  /// in outer commands as well as nested commands.
	  /// </summary>
	  public virtual void testGetCurrentCommand()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.interceptor.Command<?> outerCommand = new SelfAssertingCommand(new SelfAssertingCommand(null));
		Command<object> outerCommand = new SelfAssertingCommand(this, new SelfAssertingCommand(this, null));

		processEngineConfiguration.CommandExecutorTxRequired.execute(outerCommand);
	  }

	  protected internal class SelfAssertingCommand : Command<Void>
	  {
		  private readonly CommandInvocationContextTest outerInstance;


		protected internal Command<Void> innerCommand;

		public SelfAssertingCommand(CommandInvocationContextTest outerInstance, Command<Void> innerCommand)
		{
			this.outerInstance = outerInstance;
		  this.innerCommand = innerCommand;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  assertEquals(this, Context.CommandInvocationContext.Command);

		  if (innerCommand != null)
		  {
			CommandExecutor commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;
			commandExecutor.execute(innerCommand);

			// should still be correct after command invocation
			assertEquals(this, Context.CommandInvocationContext.Command);
		  }

		  return null;
		}

	  }

	}

}