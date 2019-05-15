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
	using ExecuteJobsCmd = org.camunda.bpm.engine.impl.cmd.ExecuteJobsCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	public class ExecuteJobHelper
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  public static ExceptionLoggingHandler LOGGING_HANDLER = new ExceptionLoggingHandlerAnonymousInnerClass();

	  private class ExceptionLoggingHandlerAnonymousInnerClass : ExceptionLoggingHandler
	  {

		  public override void exceptionWhileExecutingJob(string jobId, Exception exception)
		  {

			// Default behavior, just log exception
			LOG.exceptionWhileExecutingJob(jobId, exception);
		  }

	  }

	  public static void executeJob(string jobId, CommandExecutor commandExecutor)
	  {

		JobFailureCollector jobFailureCollector = new JobFailureCollector(jobId);

		executeJob(jobId, commandExecutor, jobFailureCollector, new ExecuteJobsCmd(jobId, jobFailureCollector));

	  }

	  public static void executeJob(string nextJobId, CommandExecutor commandExecutor, JobFailureCollector jobFailureCollector, Command<Void> cmd)
	  {
		try
		{

		  commandExecutor.execute(cmd);

		}
		catch (Exception exception)
		{
		  handleJobFailure(nextJobId, jobFailureCollector, exception);
		  // throw the original exception to indicate the ExecuteJobCmd failed
		  throw exception;

		}
		catch (Exception exception)
		{
		  handleJobFailure(nextJobId, jobFailureCollector, exception);
		  // wrap the exception and throw it to indicate the ExecuteJobCmd failed
		  throw LOG.wrapJobExecutionFailure(jobFailureCollector, exception);

		}
		finally
		{
		  invokeJobListener(commandExecutor, jobFailureCollector);
		}

	  }

	  protected internal static void invokeJobListener(CommandExecutor commandExecutor, JobFailureCollector jobFailureCollector)
	  {
		if (!string.ReferenceEquals(jobFailureCollector.JobId, null))
		{
		  if (jobFailureCollector.Failure != null)
		  {
			// the failed job listener is responsible for decrementing the retries and logging the exception to the DB.

			FailedJobListener failedJobListener = createFailedJobListener(commandExecutor, jobFailureCollector.Failure, jobFailureCollector.JobId);

			OptimisticLockingException exception = callFailedJobListenerWithRetries(commandExecutor, failedJobListener);
			if (exception != null)
			{
			  throw exception;
			}

		  }
		  else
		  {
			SuccessfulJobListener successListener = createSuccessfulJobListener(commandExecutor);
			commandExecutor.execute(successListener);
		  }
		}
	  }

	  /// <summary>
	  /// Calls FailedJobListener, in case of OptimisticLockException retries configured amount of times.
	  /// </summary>
	  /// <returns> exception or null if succeeded </returns>
	  private static OptimisticLockingException callFailedJobListenerWithRetries(CommandExecutor commandExecutor, FailedJobListener failedJobListener)
	  {
		try
		{
		  commandExecutor.execute(failedJobListener);
		  return null;
		}
		catch (OptimisticLockingException ex)
		{
		  failedJobListener.incrementCountRetries();
		  if (failedJobListener.RetriesLeft > 0)
		  {
			return callFailedJobListenerWithRetries(commandExecutor, failedJobListener);
		  }
		  return ex;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected static void handleJobFailure(final String nextJobId, final JobFailureCollector jobFailureCollector, Throwable exception)
	  protected internal static void handleJobFailure(string nextJobId, JobFailureCollector jobFailureCollector, Exception exception)
	  {
		LOGGING_HANDLER.exceptionWhileExecutingJob(nextJobId, exception);
		jobFailureCollector.Failure = exception;
	  }


	  protected internal static FailedJobListener createFailedJobListener(CommandExecutor commandExecutor, Exception exception, string jobId)
	  {
		return new FailedJobListener(commandExecutor, jobId, exception);
	  }

	  protected internal static SuccessfulJobListener createSuccessfulJobListener(CommandExecutor commandExecutor)
	  {
		return new SuccessfulJobListener();
	  }

	  public interface ExceptionLoggingHandler
	  {
		void exceptionWhileExecutingJob(string jobId, Exception exception);
	  }

	}

}