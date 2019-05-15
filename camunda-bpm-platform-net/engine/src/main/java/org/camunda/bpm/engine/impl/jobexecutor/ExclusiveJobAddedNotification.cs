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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ExclusiveJobAddedNotification : TransactionListener
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal readonly string jobId;
	  protected internal readonly JobExecutorContext jobExecutorContext;

	  public ExclusiveJobAddedNotification(string jobId, JobExecutorContext jobExecutorContext)
	  {
		this.jobId = jobId;
		this.jobExecutorContext = jobExecutorContext;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		LOG.debugAddingNewExclusiveJobToJobExecutorCOntext(jobId);
		jobExecutorContext.CurrentProcessorJobQueue.Add(jobId);
		logExclusiveJobAdded(commandContext);
	  }

	  protected internal virtual void logExclusiveJobAdded(CommandContext commandContext)
	  {
		if (commandContext.ProcessEngineConfiguration.MetricsEnabled)
		{
		  commandContext.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_LOCKED_EXCLUSIVE);
		}
	  }

	}

}