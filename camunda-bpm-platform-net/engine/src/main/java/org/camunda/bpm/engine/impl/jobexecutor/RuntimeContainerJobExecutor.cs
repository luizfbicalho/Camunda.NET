using System.Collections.Generic;
using System.Threading;

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

	using ExecutorService = org.camunda.bpm.container.ExecutorService;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;

	/// <summary>
	/// <para>JobExecutor implementation that delegates the execution of jobs
	/// to the <seealso cref="RuntimeContainerDelegate RuntimeContainer"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class RuntimeContainerJobExecutor : JobExecutor
	{

	  protected internal override void startExecutingJobs()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
		RuntimeContainerDelegate runtimeContainerDelegate = RuntimeContainerDelegate;

		// schedule job acquisition
		if (!runtimeContainerDelegate.ExecutorService.schedule(acquireJobsRunnable, true))
		{
		  throw new ProcessEngineException("Could not schedule AcquireJobsRunnable for execution.");
		}

	  }

	  protected internal override void stopExecutingJobs()
	  {
		// nothing to do
	  }

	  public override void executeJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
		RuntimeContainerDelegate runtimeContainerDelegate = RuntimeContainerDelegate;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.ExecutorService executorService = runtimeContainerDelegate.getExecutorService();
		ExecutorService executorService = runtimeContainerDelegate.ExecutorService;

		ThreadStart executeJobsRunnable = getExecuteJobsRunnable(jobIds, processEngine);

		// delegate job execution to runtime container
		if (!executorService.schedule(executeJobsRunnable, false))
		{

		  logRejectedExecution(processEngine, jobIds.Count);
		  rejectedJobsHandler.jobsRejected(jobIds, processEngine, this);
		}
	  }

	  protected internal virtual RuntimeContainerDelegate RuntimeContainerDelegate
	  {
		  get
		  {
			return org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		  }
	  }

	  public override ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
		RuntimeContainerDelegate runtimeContainerDelegate = RuntimeContainerDelegate;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.ExecutorService executorService = runtimeContainerDelegate.getExecutorService();
		ExecutorService executorService = runtimeContainerDelegate.ExecutorService;

		return executorService.getExecuteJobsRunnable(jobIds, processEngine);
	  }

	}

}