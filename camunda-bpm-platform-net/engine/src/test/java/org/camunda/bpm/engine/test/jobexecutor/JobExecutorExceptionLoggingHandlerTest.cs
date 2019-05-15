using System;
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
namespace org.camunda.bpm.engine.test.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecuteJobHelper = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobHelper;
	using Test = org.junit.Test;

	public class JobExecutorExceptionLoggingHandlerTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void shouldBeAbleToReplaceLoggingHandler()
	  public virtual void shouldBeAbleToReplaceLoggingHandler()
	  {
		ExecuteJobHelper.ExceptionLoggingHandler originalHandler = ExecuteJobHelper.LOGGING_HANDLER;
		CollectingHandler collectingHandler = new CollectingHandler();
		Exception exception = new Exception();

		try
		{
		  ExecuteJobHelper.LOGGING_HANDLER = collectingHandler;
		  CommandExecutor failingCommandExecutor = mock(typeof(CommandExecutor));
		  when(failingCommandExecutor.execute(any(typeof(Command)))).thenThrow(exception);

		  // when
		  ExecuteJobHelper.executeJob("10", failingCommandExecutor);

		  fail("exception expected");
		}
		catch (Exception e)
		{
		  // then
		  Exception collectedException = collectingHandler.collectedExceptions["10"];
		  assertEquals(collectedException, e);
		  assertEquals(collectedException, exception);
		}
		finally
		{
		  ExecuteJobHelper.LOGGING_HANDLER = originalHandler;
		}
	  }

	  internal class CollectingHandler : ExecuteJobHelper.ExceptionLoggingHandler
	  {

		internal IDictionary<string, Exception> collectedExceptions = new Dictionary<string, Exception>();

		public virtual void exceptionWhileExecutingJob(string jobId, Exception exception)
		{
		  collectedExceptions[jobId] = exception;
		}

	  }

	}

}