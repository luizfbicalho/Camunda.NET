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
namespace org.camunda.bpm.engine.spring.components.jobexecutor
{

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ExecuteJobsRunnable = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobsRunnable;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using TaskExecutor = org.springframework.core.task.TaskExecutor;

	/// 
	/// <summary>
	/// <para>
	/// This is a spring based implementation of the <seealso cref="JobExecutor"/> using spring abstraction <seealso cref="TaskExecutor"/>
	/// for performing background task execution.
	/// </para>
	/// <para>
	/// The idea behind this implementation is to externalize the configuration of the task executor, so it can leverage to
	/// Application servers controller thread pools, for example using the commonj API. The use of unmanaged thread in application servers
	/// is discouraged by the Java EE spec.
	/// </para>
	/// 
	/// @author Pablo Ganga
	/// </summary>
	public class SpringJobExecutor : JobExecutor
	{

		private TaskExecutor taskExecutor;

		public virtual TaskExecutor TaskExecutor
		{
			get
			{
				return taskExecutor;
			}
			set
			{
				this.taskExecutor = value;
			}
		}


		public override void executeJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
		{
		  try
		  {
		  taskExecutor.execute(getExecuteJobsRunnable(jobIds, processEngine));
		  }
		catch (RejectedExecutionException)
		{

		  logRejectedExecution(processEngine, jobIds.Count);
		  rejectedJobsHandler.jobsRejected(jobIds, processEngine, this);
		}
		}

		protected internal override void startExecutingJobs()
		{
			startJobAcquisitionThread();
		}

		protected internal override void stopExecutingJobs()
		{
			stopJobAcquisitionThread();
		}
	}

}