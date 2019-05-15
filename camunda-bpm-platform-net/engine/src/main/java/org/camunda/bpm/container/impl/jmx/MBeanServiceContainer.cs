using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
namespace org.camunda.bpm.container.impl.jmx
{


	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationBuilder = org.camunda.bpm.container.impl.spi.DeploymentOperation.DeploymentOperationBuilder;
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// <para>A simple Service Container that delegates to the JVM's <seealso cref="MBeanServer"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MBeanServiceContainer : PlatformServiceContainer
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal MBeanServer mBeanServer;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<javax.management.ObjectName, org.camunda.bpm.container.impl.spi.PlatformService<?>> servicesByName = new java.util.concurrent.ConcurrentHashMap<javax.management.ObjectName, org.camunda.bpm.container.impl.spi.PlatformService<?>>();
	  protected internal IDictionary<ObjectName, PlatformService<object>> servicesByName = new ConcurrentDictionary<ObjectName, PlatformService<object>>();

	  /// <summary>
	  /// set if the current thread is performing a composite deployment operation </summary>
	  protected internal ThreadLocal<Stack<DeploymentOperation>> activeDeploymentOperations = new ThreadLocal<Stack<DeploymentOperation>>();

	  public const string SERVICE_NAME_EXECUTOR = "executor-service";

	  public virtual void startService<S>(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType serviceType, string localName, PlatformService<S> service)
	  {
		  lock (this)
		  {
        
			string serviceName = composeLocalName(serviceType, localName);
			startService(serviceName, service);
        
		  }
	  }

	  public virtual void startService<S>(string name, PlatformService<S> service)
	  {
		  lock (this)
		  {
        
			ObjectName serviceName = getObjectName(name);
        
			if (getService(serviceName) != default(S))
			{
			  throw new ProcessEngineException("Cannot register service " + serviceName + " with MBeans Container, service with same name already registered.");
			}
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.management.MBeanServer beanServer = getmBeanServer();
			MBeanServer beanServer = getmBeanServer();
			// call the service-provided start behavior
			service.start(this);
        
			try
			{
			  beanServer.registerMBean(service, serviceName);
			  servicesByName[serviceName] = service;
        
			  Stack<DeploymentOperation> currentOperationContext = activeDeploymentOperations.get();
			  if (currentOperationContext != null)
			  {
				currentOperationContext.Peek().serviceAdded(name);
			  }
        
			}
			catch (Exception e)
			{
			  throw LOG.cannotRegisterService(serviceName, e);
			}
		  }
	  }

	  public static ObjectName getObjectName(string serviceName)
	  {
		try
		{
		  return new ObjectName(serviceName);
		}
		catch (Exception e)
		{
		  throw LOG.cannotComposeNameFor(serviceName, e);
		}
	  }

	  public static string composeLocalName(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type, string localName)
	  {
		return type.TypeName + ":type=" + localName;
	  }

	  public virtual void stopService(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType serviceType, string localName)
	  {
		  lock (this)
		  {
			string globalName = composeLocalName(serviceType, localName);
			stopService(globalName);
        
		  }
	  }

	  public virtual void stopService(string name)
	  {
		  lock (this)
		  {
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.management.MBeanServer mBeanServer = getmBeanServer();
			MBeanServer mBeanServer = getmBeanServer();
        
			ObjectName serviceName = getObjectName(name);
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformService<Object> service = getService(serviceName);
			PlatformService<object> service = getService(serviceName);
        
			ensureNotNull("Cannot stop service " + serviceName + ": no such service registered", "service", service);
        
			try
			{
			  // call the service-provided stop behavior
			  service.stop(this);
			}
			finally
			{
			  // always unregister, even if the stop method throws an exception.
			  try
			  {
				mBeanServer.unregisterMBean(serviceName);
				servicesByName.Remove(serviceName);
			  }
			  catch (Exception t)
			  {
				throw LOG.exceptionWhileUnregisteringService(serviceName.CanonicalName, t);
			  }
			}
        
		  }
	  }

	  public virtual DeploymentOperation.DeploymentOperationBuilder createDeploymentOperation(string name)
	  {
		return new DeploymentOperation.DeploymentOperationBuilder(this, name);
	  }

	  public virtual DeploymentOperation.DeploymentOperationBuilder createUndeploymentOperation(string name)
	  {
		DeploymentOperation.DeploymentOperationBuilder builder = new DeploymentOperation.DeploymentOperationBuilder(this, name);
		builder.setUndeploymentOperation();
		return builder;
	  }

	  public virtual void executeDeploymentOperation(DeploymentOperation operation)
	  {

		Stack<DeploymentOperation> currentOperationContext = activeDeploymentOperations.get();
		if (currentOperationContext == null)
		{
		  currentOperationContext = new Stack<DeploymentOperation>();
		  activeDeploymentOperations.set(currentOperationContext);
		}

		try
		{
		  currentOperationContext.Push(operation);
		  // execute the operation
		  operation.execute();
		}
		finally
		{
		  currentOperationContext.Pop();
		  if (currentOperationContext.Count == 0)
		  {
			activeDeploymentOperations.remove();
		  }
		}
	  }

	  /// <summary>
	  /// get a specific service by name or null if no such Service exists.
	  /// 
	  /// </summary>
	  public virtual S getService<S>(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type, string localName)
	  {
		string globalName = composeLocalName(type, localName);
		ObjectName serviceName = getObjectName(globalName);
		return getService(serviceName);
	  }

	  /// <summary>
	  /// get a specific service by name or null if no such Service exists.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <S> S getService(javax.management.ObjectName name)
	  public virtual S getService<S>(ObjectName name)
	  {
		return (S) servicesByName[name];
	  }

	  /// <summary>
	  /// get the service value for a specific service by name or null if no such
	  /// Service exists.
	  /// 
	  /// </summary>
	  public virtual S getServiceValue<S>(ObjectName name)
	  {
		PlatformService<S> service = getService(name);
		if (service != null)
		{
		  return service.Value;
		}
		else
		{
		  return default(S);
		}
	  }

	  /// <summary>
	  /// get the service value for a specific service by name or null if no such
	  /// Service exists.
	  /// 
	  /// </summary>
	  public virtual S getServiceValue<S>(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type, string localName)
	  {
		string globalName = composeLocalName(type, localName);
		ObjectName serviceName = getObjectName(globalName);
		return getServiceValue(serviceName);
	  }

	  /// <returns> all services for a specific <seealso cref="ServiceType"/> </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <S> java.util.List<org.camunda.bpm.container.impl.spi.PlatformService<S>> getServicesByType(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type)
	  public virtual IList<PlatformService<S>> getServicesByType<S>(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type)
	  {

		// query the MBeanServer for all services of the given type
		ISet<string> serviceNames = getServiceNames(type);

		IList<PlatformService<S>> res = new List<PlatformService<S>>();
		foreach (string serviceName in serviceNames)
		{
		  res.Add((PlatformService<S>) servicesByName[getObjectName(serviceName)]);
		}

		return res;
	  }

	  /// <returns> the service names ( <seealso cref="ObjectName"/> ) for all services for a given type </returns>
	  public virtual ISet<string> getServiceNames(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type)
	  {
		string typeName = composeLocalName(type, "*");
		ObjectName typeObjectName = getObjectName(typeName);
		ISet<ObjectName> resultNames = getmBeanServer().queryNames(typeObjectName, null);
		ISet<string> result = new HashSet<string>();
		foreach (ObjectName objectName in resultNames)
		{
		  result.Add(objectName.ToString());
		}
		return result;
	  }

	  /// <returns> the values of all services for a specific <seealso cref="ServiceType"/> </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <S> java.util.List<S> getServiceValuesByType(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type)
	  public virtual IList<S> getServiceValuesByType<S>(org.camunda.bpm.container.impl.spi.PlatformServiceContainer_ServiceType type)
	  {

		// query the MBeanServer for all services of the given type
		ISet<string> serviceNames = getServiceNames(type);

		IList<S> res = new List<S>();
		foreach (string serviceName in serviceNames)
		{
		  PlatformService<S> BpmPlatformService = (PlatformService<S>) servicesByName[getObjectName(serviceName)];
		  if (BpmPlatformService != null)
		  {
			res.Add(BpmPlatformService.Value);
		  }
		}

		return res;
	  }

	  public virtual MBeanServer getmBeanServer()
	  {
		if (mBeanServer == null)
		{
		  lock (this)
		  {
			if (mBeanServer == null)
			{
			  mBeanServer = createOrLookupMbeanServer();
			}
		  }
		}
		return mBeanServer;
	  }

	  public virtual void setmBeanServer(MBeanServer mBeanServer)
	  {
		this.mBeanServer = mBeanServer;
	  }

	  protected internal virtual MBeanServer createOrLookupMbeanServer()
	  {
		return ManagementFactory.PlatformMBeanServer;
	  }

	}

}