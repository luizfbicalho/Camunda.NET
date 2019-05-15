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
namespace org.camunda.bpm.container.impl.ejb
{


	using JcaExecutorServiceConnection = org.camunda.bpm.container.impl.threading.ra.outbound.JcaExecutorServiceConnection;
	using JcaExecutorServiceConnectionFactory = org.camunda.bpm.container.impl.threading.ra.outbound.JcaExecutorServiceConnectionFactory;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;

	/// <summary>
	/// Bean exposing the JCA implementation of the <seealso cref="ExecutorService"/> as Stateless Bean.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stateless @Local(ExecutorService.class) @TransactionAttribute(TransactionAttributeType.SUPPORTS) public class ExecutorServiceBean implements org.camunda.bpm.container.ExecutorService
	public class ExecutorServiceBean : ExecutorService
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Resource(mappedName="eis/JcaExecutorServiceConnectionFactory") protected org.camunda.bpm.container.impl.threading.ra.outbound.JcaExecutorServiceConnectionFactory executorConnectionFactory;
		protected internal JcaExecutorServiceConnectionFactory executorConnectionFactory;

	  protected internal JcaExecutorServiceConnection executorConnection;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostConstruct protected void openConnection()
	  protected internal virtual void openConnection()
	  {
		try
		{
		  executorConnection = executorConnectionFactory.Connection;
		}
		catch (ResourceException e)
		{
		  throw new ProcessEngineException("Could not open connection to executor service connection factory ", e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PreDestroy protected void closeConnection()
	  protected internal virtual void closeConnection()
	  {
		if (executorConnection != null)
		{
		  executorConnection.closeConnection();
		}
	  }

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {
		return executorConnection.schedule(runnable, isLongRunning);
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return executorConnection.getExecuteJobsRunnable(jobIds, processEngine);
	  }

	}

}