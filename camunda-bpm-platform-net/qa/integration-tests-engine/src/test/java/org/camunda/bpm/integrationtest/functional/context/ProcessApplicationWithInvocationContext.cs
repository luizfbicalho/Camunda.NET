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
namespace org.camunda.bpm.integrationtest.functional.context
{

	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessApplicationExecutionException = org.camunda.bpm.application.ProcessApplicationExecutionException;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;

	[ProcessApplication("app")]
	public class ProcessApplicationWithInvocationContext : ServletProcessApplication
	{

	  private static InvocationContext invocationContext = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public <T> T execute(java.util.concurrent.Callable<T> callable, org.camunda.bpm.application.InvocationContext invocationContext) throws org.camunda.bpm.application.ProcessApplicationExecutionException
	  public override T execute<T>(Callable<T> callable, InvocationContext invocationContext)
	  {
		lock (typeof(ProcessApplicationWithInvocationContext))
		{
		  ProcessApplicationWithInvocationContext.invocationContext = invocationContext;
		}

		return execute(callable);
	  }

	  public static InvocationContext InvocationContext
	  {
		  get
		  {
			  lock (typeof(ProcessApplicationWithInvocationContext))
			  {
				return ProcessApplicationWithInvocationContext.invocationContext;
			  }
		  }
	  }

	  public static void clearInvocationContext()
	  {
		  lock (typeof(ProcessApplicationWithInvocationContext))
		  {
			ProcessApplicationWithInvocationContext.invocationContext = null;
		  }
	  }

	  public override void undeploy()
	  {
		clearInvocationContext();
		base.undeploy();
	  }

	}

}