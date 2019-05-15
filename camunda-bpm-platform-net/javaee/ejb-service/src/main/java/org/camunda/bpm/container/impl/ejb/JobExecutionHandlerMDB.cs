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

	using JobExecutionHandler = org.camunda.bpm.container.impl.threading.ra.inflow.JobExecutionHandler;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecuteJobHelper = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobHelper;


	/// <summary>
	/// <para>MessageDrivenBean implementation of the <seealso cref="JobExecutionHandler"/> interface</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @MessageDriven(name="JobExecutionHandlerMDB", messageListenerInterface=JobExecutionHandler.class) @TransactionAttribute(TransactionAttributeType.NOT_SUPPORTED) public class JobExecutionHandlerMDB implements org.camunda.bpm.container.impl.threading.ra.inflow.JobExecutionHandler
	public class JobExecutionHandlerMDB : JobExecutionHandler
	{

	  public virtual void executeJob(string job, CommandExecutor commandExecutor)
	  {
		ExecuteJobHelper.executeJob(job, commandExecutor);
	  }

	}

}