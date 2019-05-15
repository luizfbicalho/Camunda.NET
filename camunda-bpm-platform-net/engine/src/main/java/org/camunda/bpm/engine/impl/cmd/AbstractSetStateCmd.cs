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
namespace org.camunda.bpm.engine.impl.cmd
{

	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractSetStateCmd : Command<Void>
	{

	  protected internal const string SUSPENSION_STATE_PROPERTY = "suspensionState";

	  protected internal bool includeSubResources;
	  protected internal bool isLogUserOperationDisabled;
	  protected internal DateTime executionDate;

	  public AbstractSetStateCmd(bool includeSubResources, DateTime executionDate)
	  {
		this.includeSubResources = includeSubResources;
		this.executionDate = executionDate;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Void execute(CommandContext commandContext)
	  {
		checkParameters(commandContext);
		checkAuthorization(commandContext);

		if (executionDate == null)
		{

		  updateSuspensionState(commandContext, NewSuspensionState);

		  if (IncludeSubResources)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AbstractSetStateCmd cmd = getNextCommand();
			AbstractSetStateCmd cmd = NextCommand;
			if (cmd != null)
			{
			  cmd.disableLogUserOperation();
			  // avoids unnecessary authorization checks
			  // pre-requirement: the necessary authorization check
			  // for included resources should be done before this
			  // call.
			  commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, cmd));
			}
		  }

		  triggerHistoryEvent(commandContext);
		}
		else
		{
		  scheduleSuspensionStateUpdate(commandContext);
		}

		if (!LogUserOperationDisabled)
		{
		  logUserOperation(commandContext);
		}

		return null;
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly AbstractSetStateCmd outerInstance;

		  private CommandContext commandContext;
		  private org.camunda.bpm.engine.impl.cmd.AbstractSetStateCmd cmd;

		  public CallableAnonymousInnerClass(AbstractSetStateCmd outerInstance, CommandContext commandContext, org.camunda.bpm.engine.impl.cmd.AbstractSetStateCmd cmd)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.cmd = cmd;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			cmd.execute(commandContext);
			return null;
		  }
	  }

	  protected internal virtual void triggerHistoryEvent(CommandContext commandContext)
	  {

	  }

	  public virtual void disableLogUserOperation()
	  {
		this.isLogUserOperationDisabled = true;
	  }

	  protected internal virtual bool LogUserOperationDisabled
	  {
		  get
		  {
			return isLogUserOperationDisabled;
		  }
	  }

	  protected internal virtual bool IncludeSubResources
	  {
		  get
		  {
			return includeSubResources;
		  }
	  }

	  protected internal virtual void scheduleSuspensionStateUpdate(CommandContext commandContext)
	  {
		TimerEntity timer = new TimerEntity();

		JobHandlerConfiguration jobHandlerConfiguration = JobHandlerConfiguration;

		timer.Duedate = executionDate;
		timer.JobHandlerType = DelayedExecutionJobHandlerType;
		timer.JobHandlerConfigurationRaw = jobHandlerConfiguration.toCanonicalString();

		commandContext.JobManager.schedule(timer);
	  }

	  protected internal virtual string DelayedExecutionJobHandlerType
	  {
		  get
		  {
			return null;
		  }
	  }

	  protected internal virtual JobHandlerConfiguration JobHandlerConfiguration
	  {
		  get
		  {
			return null;
		  }
	  }

	  protected internal virtual AbstractSetStateCmd NextCommand
	  {
		  get
		  {
			return null;
		  }
	  }

	  protected internal abstract void checkAuthorization(CommandContext commandContext);

	  protected internal abstract void checkParameters(CommandContext commandContext);

	  protected internal abstract void updateSuspensionState(CommandContext commandContext, SuspensionState suspensionState);

	  protected internal abstract void logUserOperation(CommandContext commandContext);

	  protected internal abstract string LogEntryOperation {get;}

	  protected internal abstract SuspensionState NewSuspensionState {get;}

	}

}