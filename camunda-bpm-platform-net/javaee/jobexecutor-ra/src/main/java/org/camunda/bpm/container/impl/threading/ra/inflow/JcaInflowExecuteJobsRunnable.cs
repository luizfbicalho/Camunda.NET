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
namespace org.camunda.bpm.container.impl.threading.ra.inflow
{


	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecuteJobsRunnable = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobsRunnable;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JcaInflowExecuteJobsRunnable : ExecuteJobsRunnable
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private Logger log = Logger.getLogger(typeof(JcaInflowExecuteJobsRunnable).FullName);

	  protected internal readonly JcaExecutorServiceConnector ra;

	  protected internal static System.Reflection.MethodInfo method;

	  public JcaInflowExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine, JcaExecutorServiceConnector connector) : base(jobIds, processEngine)
	  {
		this.ra = connector;
		if (method == null)
		{
		  loadMethod();
		}
	  }

	  protected internal override void executeJob(string nextJobId, CommandExecutor commandExecutor)
	  {
		JobExecutionHandlerActivation jobHandlerActivation = ra.JobHandlerActivation;
		if (jobHandlerActivation == null)
		{
		  // TODO: stop acquisition / only activate acquisition if MDB active?
		  log.warning("Cannot execute acquired job, no JobExecutionHandler MDB deployed.");
		  return;
		}
		MessageEndpoint endpoint = null;
		try
		{
		  endpoint = jobHandlerActivation.MessageEndpointFactory.createEndpoint(null);

		  try
		  {
			endpoint.beforeDelivery(method);
		  }
		  catch (NoSuchMethodException e)
		  {
			log.log(Level.WARNING, "NoSuchMethodException while invoking beforeDelivery() on MessageEndpoint '" + endpoint + "'", e);
		  }
		  catch (ResourceException e)
		  {
			log.log(Level.WARNING, "ResourceException while invoking beforeDelivery() on MessageEndpoint '" + endpoint + "'", e);
		  }

		  try
		  {
			((JobExecutionHandler)endpoint).executeJob(nextJobId, commandExecutor);
		  }
		  catch (Exception e)
		  {
			log.log(Level.WARNING, "Exception while executing job with id '" + nextJobId + "'.", e);
		  }

		  try
		  {
			endpoint.afterDelivery();
		  }
		  catch (ResourceException e)
		  {
			log.log(Level.WARNING, "ResourceException while invoking afterDelivery() on MessageEndpoint '" + endpoint + "'", e);
		  }

		}
		catch (UnavailableException e)
		{
		  log.log(Level.SEVERE, "UnavailableException while attempting to create messaging endpoint for executing job", e);
		}
		finally
		{
		  if (endpoint != null)
		  {
			endpoint.release();
		  }
		}
	  }

	  protected internal virtual void loadMethod()
	  {
		try
		{
		  method = typeof(JobExecutionHandler).GetMethod("executeJob", new Type[] {typeof(string), typeof(CommandExecutor)});
		}
		catch (SecurityException e)
		{
		  throw new Exception("SecurityException while invoking getMethod() on class " + typeof(JobExecutionHandler), e);
		}
		catch (NoSuchMethodException e)
		{
		  throw new Exception("NoSuchMethodException while invoking getMethod() on class " + typeof(JobExecutionHandler), e);
		}
	  }
	}

}