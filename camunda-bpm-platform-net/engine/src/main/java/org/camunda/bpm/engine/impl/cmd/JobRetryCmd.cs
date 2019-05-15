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
	using TransactionContext = org.camunda.bpm.engine.impl.cfg.TransactionContext;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using MessageAddedNotification = org.camunda.bpm.engine.impl.jobexecutor.MessageAddedNotification;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;

	/// <summary>
	/// @author Roman Smirnov
	/// </summary>
	public abstract class JobRetryCmd : Command<object>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal const long serialVersionUID = 1L;
	  protected internal string jobId;
	  protected internal Exception exception;

	  public JobRetryCmd(string jobId, Exception exception)
	  {
		this.jobId = jobId;
		this.exception = exception;
	  }

	  protected internal virtual JobEntity Job
	  {
		  get
		  {
			return Context.CommandContext.JobManager.findJobById(jobId);
		  }
	  }

	  protected internal virtual void logException(JobEntity job)
	  {
		if (exception != null)
		{
		  job.ExceptionMessage = exception.Message;
		  job.ExceptionStacktrace = ExceptionStacktrace;
		}
	  }

	  protected internal virtual void decrementRetries(JobEntity job)
	  {
		if (exception == null || shouldDecrementRetriesFor(exception))
		{
		  job.Retries = job.Retries - 1;
		}
	  }

	  protected internal virtual string ExceptionStacktrace
	  {
		  get
		  {
			return ExceptionUtil.getExceptionStacktrace(exception);
		  }
	  }

	  protected internal virtual bool shouldDecrementRetriesFor(Exception t)
	  {
		return !(t is OptimisticLockingException);
	  }

	  protected internal virtual void notifyAcquisition(CommandContext commandContext)
	  {
		JobExecutor jobExecutor = Context.ProcessEngineConfiguration.JobExecutor;
		MessageAddedNotification messageAddedNotification = new MessageAddedNotification(jobExecutor);
		TransactionContext transactionContext = commandContext.TransactionContext;
		transactionContext.addTransactionListener(TransactionState.COMMITTED, messageAddedNotification);
	  }
	}

}