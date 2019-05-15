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
namespace org.camunda.bpm.container.impl.deployment.util
{

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationDeploymentInfo = org.camunda.bpm.application.ProcessApplicationDeploymentInfo;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class InjectionUtil
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public static System.Reflection.MethodInfo detectAnnotatedMethod(Type clazz, Type annotationType)
	  {

		System.Reflection.MethodInfo[] methods = clazz.GetMethods();
		foreach (System.Reflection.MethodInfo method in methods)
		{
		  foreach (Annotation annotaiton in method.GetCustomAttributes(true))
		  {
			if (annotationType.Equals(annotaiton.annotationType()))
			{
			  return method;
			}
		  }
		}

		return null;

	  }

	  public static object[] resolveInjections(DeploymentOperation operationContext, System.Reflection.MethodInfo lifecycleMethod)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Type[] parameterTypes = lifecycleMethod.getGenericParameterTypes();
		Type[] parameterTypes = lifecycleMethod.GenericParameterTypes;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Object> parameters = new java.util.ArrayList<Object>();
		IList<object> parameters = new List<object>();

		foreach (Type parameterType in parameterTypes)
		{

		  bool injectionResolved = false;

		  if (parameterType is Type)
		  {

			Type parameterClass = (Type)parameterType;

			// support injection of the default process engine
			if (parameterClass.IsAssignableFrom(typeof(ProcessEngine)))
			{
			  parameters.Add(getDefaultProcessEngine(operationContext));
			  injectionResolved = true;
			}

			// support injection of the ProcessApplicationInfo
			else if (parameterClass.IsAssignableFrom(typeof(ProcessApplicationInfo)))
			{
			  parameters.Add(getProcessApplicationInfo(operationContext));
			  injectionResolved = true;
			}

		  }
		  else if (parameterType is ParameterizedType)
		  {

			ParameterizedType parameterizedType = (ParameterizedType) parameterType;
			Type[] actualTypeArguments = parameterizedType.ActualTypeArguments;

			// support injection of List<ProcessEngine>
			if (actualTypeArguments.Length == 1 && (Type) actualTypeArguments[0].IsAssignableFrom(typeof(ProcessEngine)))
			{
			  parameters.Add(getProcessEngines(operationContext));
			  injectionResolved = true;
			}
		  }

		  if (!injectionResolved)
		  {
			throw LOG.unsuppoertedParameterType(parameterType);
		  }

		}

		return parameters.ToArray();
	  }

	  public static ProcessApplicationInfo getProcessApplicationInfo(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);

		JmxManagedProcessApplication managedPa = serviceContainer.getServiceValue(ServiceTypes.PROCESS_APPLICATION, processApplication.Name);
		return managedPa.ProcessApplicationInfo;
	  }

	  public static IList<ProcessEngine> getProcessEngines(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationInfo processApplicationInfo = getProcessApplicationInfo(operationContext);
		ProcessApplicationInfo processApplicationInfo = getProcessApplicationInfo(operationContext);

		IList<ProcessEngine> processEngines = new List<ProcessEngine>();
		foreach (ProcessApplicationDeploymentInfo deploymentInfo in processApplicationInfo.DeploymentInfo)
		{
		  string processEngineName = deploymentInfo.ProcessEngineName;
		  processEngines.Add((ProcessEngine) serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, processEngineName));
		}

		return processEngines;
	  }

	  public static ProcessEngine getDefaultProcessEngine(DeploymentOperation operationContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
		return serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, "default");
	  }

	}

}