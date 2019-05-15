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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{

	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public abstract class HistoryCleanupHandler : TransactionListener
	{

	  /// <summary>
	  /// Maximum allowed batch size.
	  /// </summary>
	  public const int MAX_BATCH_SIZE = 500;

	  protected internal HistoryCleanupJobHandlerConfiguration configuration;
	  protected internal string jobId;
	  protected internal CommandExecutor commandExecutor;

	  public virtual void execute(CommandContext commandContext)
	  {
		// passed commandContext may be in an inconsistent state
		commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupHandler outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass(HistoryCleanupHandler outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IDictionary<string, long> report = outerInstance.reportMetrics();
			bool isRescheduleNow = outerInstance.shouldRescheduleNow();

			(new HistoryCleanupSchedulerCmd(isRescheduleNow, report, outerInstance.configuration, outerInstance.jobId)).execute(commandContext);

			return null;
		  }
	  }

	  internal abstract void performCleanup();

	  internal abstract IDictionary<string, long> reportMetrics();

	  internal abstract bool shouldRescheduleNow();

	  public virtual HistoryCleanupJobHandlerConfiguration Configuration
	  {
		  get
		  {
			return configuration;
		  }
	  }

	  public virtual HistoryCleanupHandler setConfiguration(HistoryCleanupJobHandlerConfiguration configuration)
	  {
		this.configuration = configuration;
		return this;
	  }

	  public virtual HistoryCleanupHandler setJobId(string jobId)
	  {
		this.jobId = jobId;
		return this;
	  }

	  public virtual HistoryCleanupHandler setCommandExecutor(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		return this;
	  }

	}

}