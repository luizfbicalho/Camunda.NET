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
namespace org.camunda.bpm.engine.impl.jobexecutor
{

	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.TimerJobConfiguration;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class RepeatingFailedJobListener : TransactionListener
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal string jobId;

	  public RepeatingFailedJobListener(CommandExecutor commandExecutor, string jobId)
	  {
		this.commandExecutor = commandExecutor;
		this.jobId = jobId;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		CreateNewTimerJobCommand cmd = new CreateNewTimerJobCommand(this, jobId);
		commandExecutor.execute(cmd);
	  }

	  protected internal class CreateNewTimerJobCommand : Command<Void>
	  {
		  private readonly RepeatingFailedJobListener outerInstance;


		protected internal string jobId;

		public CreateNewTimerJobCommand(RepeatingFailedJobListener outerInstance, string jobId)
		{
			this.outerInstance = outerInstance;
		  this.jobId = jobId;
		}

		public virtual Void execute(CommandContext commandContext)
		{

		  TimerEntity failedJob = (TimerEntity) commandContext.JobManager.findJobById(jobId);

		  DateTime newDueDate = failedJob.calculateRepeat();

		  if (newDueDate != null)
		  {
			failedJob.createNewTimerJob(newDueDate);

			// update configuration of failed job
			TimerJobConfiguration config = (TimerJobConfiguration) failedJob.JobHandlerConfiguration;
			config.FollowUpJobCreated = true;
			failedJob.JobHandlerConfiguration = config;
		  }

		  return null;
		}

	  }

	}

}