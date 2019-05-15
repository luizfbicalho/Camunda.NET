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
namespace org.camunda.bpm.engine.test.jobexecutor
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// Throws an exception from a nested command; Unlike <seealso cref="TweetExceptionHandler"/>, this handler always throws exceptions.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class TweetNestedCommandExceptionHandler : JobHandler<JobHandlerConfiguration>
	{

	  public const string TYPE = "tweet-exception-nested";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(JobHandlerConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		Context.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, commandContext));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly TweetNestedCommandExceptionHandler outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass(TweetNestedCommandExceptionHandler outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			throw new Exception("nested command exception");
		  }

	  }

	  public virtual JobHandlerConfiguration newConfiguration(string canonicalString)
	  {
		return new JobHandlerConfigurationAnonymousInnerClass(this);
	  }

	  private class JobHandlerConfigurationAnonymousInnerClass : JobHandlerConfiguration
	  {
		  private readonly TweetNestedCommandExceptionHandler outerInstance;

		  public JobHandlerConfigurationAnonymousInnerClass(TweetNestedCommandExceptionHandler outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string toCanonicalString()
		  {
			return null;
		  }
	  }

	  public virtual void onDelete(JobHandlerConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}