﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.persistence.deploy
{

	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using UnregisterDeploymentCmd = org.camunda.bpm.engine.impl.cmd.UnregisterDeploymentCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	public class DeploymentFailListener : TransactionListener
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal ISet<string> deploymentIds;

	  public DeploymentFailListener(string deploymentId, CommandExecutor commandExecutor)
	  {
		this.deploymentIds = Collections.singleton(deploymentId);
		this.commandExecutor = commandExecutor;
	  }

	  public DeploymentFailListener(ISet<string> deploymentIds, CommandExecutor commandExecutor)
	  {
		this.deploymentIds = deploymentIds;
		this.commandExecutor = commandExecutor;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		//unregister deployment without authorization
		commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly DeploymentFailListener outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass(DeploymentFailListener outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
		  public Void execute(CommandContext commandContext)
		  {
			commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
			return null;
		  }

		  private class CallableAnonymousInnerClass : Callable<Void>
		  {
			  private readonly CommandAnonymousInnerClass outerInstance;

			  private CommandContext commandContext;

			  public CallableAnonymousInnerClass(CommandAnonymousInnerClass outerInstance, CommandContext commandContext)
			  {
				  this.outerInstance = outerInstance;
				  this.commandContext = commandContext;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
			  public Void call()
			  {
				(new UnregisterDeploymentCmd(outerInstance.outerInstance.deploymentIds)).execute(commandContext);
				return null;
			  }
		  }
	  }

	}

}