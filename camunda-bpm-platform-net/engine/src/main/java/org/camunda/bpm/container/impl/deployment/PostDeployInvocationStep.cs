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
namespace org.camunda.bpm.container.impl.deployment
{


	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using InjectionUtil = org.camunda.bpm.container.impl.deployment.util.InjectionUtil;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>Operation step responsible for invoking the {@literal @}<seealso cref="PostDeploy"/> method of a
	/// ProcessApplication class.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PostDeployInvocationStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  private const string CALLBACK_NAME = "@PostDeploy";

	  public override string Name
	  {
		  get
		  {
			return "Invoking @PostDeploy";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String paName = processApplication.getName();
		string paName = processApplication.Name;

		Type paClass = processApplication.GetType();
		System.Reflection.MethodInfo postDeployMethod = InjectionUtil.detectAnnotatedMethod(paClass, typeof(PostDeploy));

		if (postDeployMethod == null)
		{
		  LOG.debugPaLifecycleMethodNotFound(CALLBACK_NAME, paName);
		  return;
		}

		LOG.debugFoundPaLifecycleCallbackMethod(CALLBACK_NAME, paName);

		// resolve injections
		object[] injections = InjectionUtil.resolveInjections(operationContext, postDeployMethod);

		try
		{
		  // perform the actual invocation
		  postDeployMethod.invoke(processApplication, injections);
		}
		catch (System.ArgumentException e)
		{
		  throw LOG.exceptionWhileInvokingPaLifecycleCallback(CALLBACK_NAME, paName, e);

		}
		catch (IllegalAccessException e)
		{
		  throw LOG.exceptionWhileInvokingPaLifecycleCallback(CALLBACK_NAME, paName, e);
		}
		catch (InvocationTargetException e)
		{
		  Exception cause = e.InnerException;
		  if (cause is Exception)
		  {
			throw (Exception) cause;
		  }
		  else
		  {
			throw LOG.exceptionWhileInvokingPaLifecycleCallback(CALLBACK_NAME, paName, e);
		  }
		}

	  }

	}

}