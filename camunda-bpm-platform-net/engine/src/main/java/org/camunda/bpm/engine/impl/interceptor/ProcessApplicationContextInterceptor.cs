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
namespace org.camunda.bpm.engine.impl.interceptor
{

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationContextImpl = org.camunda.bpm.application.impl.ProcessApplicationContextImpl;
	using ProcessApplicationIdentifier = org.camunda.bpm.application.impl.ProcessApplicationIdentifier;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessApplicationContextInterceptor : CommandInterceptor
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  public ProcessApplicationContextInterceptor(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		this.processEngineConfiguration = processEngineConfiguration;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public <T> T execute(final Command<T> command)
	  public override T execute<T>(Command<T> command)
	  {
		ProcessApplicationIdentifier processApplicationIdentifier = ProcessApplicationContextImpl.get();

		if (processApplicationIdentifier != null)
		{
		  // clear the identifier so this interceptor does not apply to nested commands
		  ProcessApplicationContextImpl.clear();

		  try
		  {
			ProcessApplicationReference reference = getPaReference(processApplicationIdentifier);
			return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, command)
		   , reference);

		  }
		  finally
		  {
			// restore the identifier for subsequent commands
			ProcessApplicationContextImpl.set(processApplicationIdentifier);
		  }
		}
		else
		{
		  return next.execute(command);
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<T>
	  {
		  private readonly ProcessApplicationContextInterceptor outerInstance;

		  private org.camunda.bpm.engine.impl.interceptor.Command<T> command;

		  public CallableAnonymousInnerClass(ProcessApplicationContextInterceptor outerInstance, org.camunda.bpm.engine.impl.interceptor.Command<T> command)
		  {
			  this.outerInstance = outerInstance;
			  this.command = command;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T call() throws Exception
		  public override T call()
		  {
			return outerInstance.next.execute(command);
		  }
	  }

	  protected internal virtual ProcessApplicationReference getPaReference(ProcessApplicationIdentifier processApplicationIdentifier)
	  {
		if (processApplicationIdentifier.Reference != null)
		{
		  return processApplicationIdentifier.Reference;
		}
		else if (processApplicationIdentifier.ProcessApplication != null)
		{
		  return processApplicationIdentifier.ProcessApplication.Reference;
		}
		else if (!string.ReferenceEquals(processApplicationIdentifier.Name, null))
		{
		   RuntimeContainerDelegate runtimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		   ProcessApplicationReference reference = runtimeContainerDelegate.getDeployedProcessApplication(processApplicationIdentifier.Name);

		   if (reference == null)
		   {
			 throw LOG.paWithNameNotRegistered(processApplicationIdentifier.Name);
		   }
		   else
		   {
			 return reference;
		   }
		}
		else
		{
		  throw LOG.cannotReolvePa(processApplicationIdentifier);
		}
	  }

	}

}