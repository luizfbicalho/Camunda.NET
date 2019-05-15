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
namespace org.camunda.bpm.container.impl.jboss.service
{

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using BindingUtil = org.camunda.bpm.container.impl.jboss.util.BindingUtil;
	using PlatformServiceReferenceFactory = org.camunda.bpm.container.impl.jboss.util.PlatformServiceReferenceFactory;
	using ServiceTracker = org.camunda.bpm.container.impl.jboss.util.ServiceTracker;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;
	using ContextNames = org.jboss.@as.naming.deployment.ContextNames;
	using ModuleClassLoader = org.jboss.modules.ModuleClassLoader;
	using Service = org.jboss.msc.service.Service;
	using ServiceContainer = org.jboss.msc.service.ServiceContainer;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using ServiceNotFoundException = org.jboss.msc.service.ServiceNotFoundException;
	using ServiceTarget = org.jboss.msc.service.ServiceTarget;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;


	/// <summary>
	/// <para>A <seealso cref="RuntimeContainerDelegate"/> implementation for JBoss AS 7</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class MscRuntimeContainerDelegate : Service<MscRuntimeContainerDelegate>, RuntimeContainerDelegate, ProcessEngineService, ProcessApplicationService
	{

	  // used for installing services
	  protected internal ServiceTarget childTarget;
	  // used for looking up services
	  protected internal ServiceContainer serviceContainer;

	  protected internal ServiceTracker<ProcessEngine> processEngineServiceTracker;
	  protected internal ISet<ProcessEngine> processEngines = new CopyOnWriteArraySet<ProcessEngine>();

	  protected internal ServiceTracker<MscManagedProcessApplication> processApplicationServiceTracker;
	  protected internal ISet<MscManagedProcessApplication> processApplications = new CopyOnWriteArraySet<MscManagedProcessApplication>();

	  // Lifecycle /////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {
		serviceContainer = context.Controller.ServiceContainer;
		childTarget = context.ChildTarget;

		startTrackingServices();
		createJndiBindings();

		// set this implementation as Runtime Container
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.set(this);
	  }

	  public virtual void stop(StopContext context)
	  {
		stopTrackingServices();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MscRuntimeContainerDelegate getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual MscRuntimeContainerDelegate Value
	  {
		  get
		  {
			return this;
		  }
	  }

	  // RuntimeContainerDelegate implementation /////////////////////////////

	  public virtual void registerProcessEngine(ProcessEngine processEngine)
	  {

		if (processEngine == null)
		{
		  throw new ProcessEngineException("Cannot register process engine with Msc Runtime Container: process engine is 'null'");
		}

		ServiceName serviceName = ServiceNames.forManagedProcessEngine(processEngine.Name);

		if (serviceContainer.getService(serviceName) == null)
		{
		  MscManagedProcessEngine processEngineRegistration = new MscManagedProcessEngine(processEngine);

		  // install the service asynchronously.
		  childTarget.addService(serviceName, processEngineRegistration).setInitialMode(ServiceController.Mode.ACTIVE).addDependency(ServiceNames.forMscRuntimeContainerDelegate(), typeof(MscRuntimeContainerDelegate), processEngineRegistration.RuntimeContainerDelegateInjector).install();
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void unregisterProcessEngine(org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual void unregisterProcessEngine(ProcessEngine processEngine)
	  {

		if (processEngine == null)
		{
		  throw new ProcessEngineException("Cannot unregister process engine with Msc Runtime Container: process engine is 'null'");
		}

		ServiceName serviceName = ServiceNames.forManagedProcessEngine(processEngine.Name);

		// remove the service asynchronously
		ServiceController<ProcessEngine> service = (ServiceController<ProcessEngine>) serviceContainer.getService(serviceName);
		if (service != null && service.Service is MscManagedProcessEngine)
		{
		  service.Mode = ServiceController.Mode.REMOVE;
		}

	  }

	  public virtual void deployProcessApplication(AbstractProcessApplication processApplication)
	  {
		if (processApplication is ServletProcessApplication)
		{
		  deployServletProcessApplication((ServletProcessApplication)processApplication);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void deployServletProcessApplication(org.camunda.bpm.application.impl.ServletProcessApplication processApplication)
	  protected internal virtual void deployServletProcessApplication(ServletProcessApplication processApplication)
	  {

		ClassLoader contextClassloader = ClassLoaderUtil.ContextClassloader;
		string moduleName = ((ModuleClassLoader)contextClassloader).Module.Identifier.ToString();

		ServiceName serviceName = ServiceNames.forNoViewProcessApplicationStartService(moduleName);
		ServiceName paModuleService = ServiceNames.forProcessApplicationModuleService(moduleName);

		if (serviceContainer.getService(serviceName) == null)
		{

		  ServiceController<ServiceTarget> requiredService = (ServiceController<ServiceTarget>) serviceContainer.getRequiredService(paModuleService);

		  NoViewProcessApplicationStartService service = new NoViewProcessApplicationStartService(processApplication.Reference);
		  requiredService.Value.addService(serviceName, service).setInitialMode(ServiceController.Mode.ACTIVE).install();

		}
	  }

	  public virtual void undeployProcessApplication(AbstractProcessApplication processApplication)
	  {
		// nothing to do
	  }

	  public virtual ProcessEngineService ProcessEngineService
	  {
		  get
		  {
			// TODO: return proxy?
			return this;
		  }
	  }

	  public virtual ProcessApplicationService ProcessApplicationService
	  {
		  get
		  {
			// TODO: return proxy?
			return this;
		  }
	  }

	  public virtual ExecutorService ExecutorService
	  {
		  get
		  {
			return (ExecutorService) serviceContainer.getRequiredService(ServiceNames.forMscExecutorService()).Value;
		  }
	  }

	  // ProcessEngineService implementation /////////////////////////////////

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return getProcessEngineService(ServiceNames.forDefaultProcessEngine());
		  }
	  }

	  public virtual IList<ProcessEngine> ProcessEngines
	  {
		  get
		  {
			return new List<ProcessEngine>(processEngines);
		  }
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			HashSet<string> result = new HashSet<string>();
			foreach (ProcessEngine engine in processEngines)
			{
			  result.Add(engine.Name);
			}
			return result;
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		return getProcessEngineService(ServiceNames.forManagedProcessEngine(name));
	  }

	  // ProcessApplicationService implementation //////////////////////////////

	  public virtual ProcessApplicationInfo getProcessApplicationInfo(string processApplicationName)
	  {
		MscManagedProcessApplication managedPa = getManagedProcessApplication(processApplicationName);
		if (managedPa == null)
		{
		  return null;
		}
		else
		{
		  return managedPa.ProcessApplicationInfo;
		}
	  }

	  public virtual ISet<string> ProcessApplicationNames
	  {
		  get
		  {
			HashSet<string> result = new HashSet<string>();
			foreach (MscManagedProcessApplication application in processApplications)
			{
			  result.Add(application.ProcessApplicationInfo.Name);
			}
			return result;
		  }
	  }

	  public virtual ProcessApplicationReference getDeployedProcessApplication(string name)
	  {
		MscManagedProcessApplication managedPa = getManagedProcessApplication(name);
		if (managedPa == null)
		{
		  return null;
		}
		else
		{
		  return managedPa.ProcessApplicationReference;
		}
	  }

	  // internal implementation ///////////////////////////////

	  protected internal virtual void createProcessEngineServiceJndiBindings()
	  {

	  }

	  protected internal virtual void createJndiBindings()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.jboss.util.PlatformServiceReferenceFactory managedReferenceFactory = new org.camunda.bpm.container.impl.jboss.util.PlatformServiceReferenceFactory(this);
		PlatformServiceReferenceFactory managedReferenceFactory = new PlatformServiceReferenceFactory(this);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName processEngineServiceBindingServiceName = org.jboss.as.naming.deployment.ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(org.camunda.bpm.BpmPlatform.APP_JNDI_NAME).append(org.camunda.bpm.BpmPlatform.MODULE_JNDI_NAME).append(org.camunda.bpm.BpmPlatform.PROCESS_ENGINE_SERVICE_NAME);
		ServiceName processEngineServiceBindingServiceName = ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(BpmPlatform.APP_JNDI_NAME).append(BpmPlatform.MODULE_JNDI_NAME).append(BpmPlatform.PROCESS_ENGINE_SERVICE_NAME);

		// bind process engine service
		BindingUtil.createJndiBindings(childTarget, processEngineServiceBindingServiceName, BpmPlatform.PROCESS_ENGINE_SERVICE_JNDI_NAME, managedReferenceFactory);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName processApplicationServiceBindingServiceName = org.jboss.as.naming.deployment.ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(org.camunda.bpm.BpmPlatform.APP_JNDI_NAME).append(org.camunda.bpm.BpmPlatform.MODULE_JNDI_NAME).append(org.camunda.bpm.BpmPlatform.PROCESS_APPLICATION_SERVICE_NAME);
		ServiceName processApplicationServiceBindingServiceName = ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(BpmPlatform.APP_JNDI_NAME).append(BpmPlatform.MODULE_JNDI_NAME).append(BpmPlatform.PROCESS_APPLICATION_SERVICE_NAME);

		// bind process application service
		BindingUtil.createJndiBindings(childTarget, processApplicationServiceBindingServiceName, BpmPlatform.PROCESS_APPLICATION_SERVICE_JNDI_NAME, managedReferenceFactory);

	  }

	  protected internal virtual ProcessEngine getProcessEngineService(ServiceName processEngineServiceName)
	  {
		try
		{
		  ServiceController<ProcessEngine> serviceController = getProcessEngineServiceController(processEngineServiceName);
		  return serviceController.Value;
		}
		catch (ServiceNotFoundException)
		{
		  return null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.jboss.msc.service.ServiceController<org.camunda.bpm.engine.ProcessEngine> getProcessEngineServiceController(org.jboss.msc.service.ServiceName processEngineServiceName)
	  protected internal virtual ServiceController<ProcessEngine> getProcessEngineServiceController(ServiceName processEngineServiceName)
	  {
		ServiceController<ProcessEngine> serviceController = (ServiceController<ProcessEngine>) serviceContainer.getRequiredService(processEngineServiceName);
		return serviceController;
	  }

	  protected internal virtual void startTrackingServices()
	  {
		processEngineServiceTracker = new ServiceTracker<ProcessEngine>(ServiceNames.forManagedProcessEngines(), processEngines);
		serviceContainer.addListener(processEngineServiceTracker);

		processApplicationServiceTracker = new ServiceTracker<MscManagedProcessApplication>(ServiceNames.forManagedProcessApplications(), processApplications);
		serviceContainer.addListener(processApplicationServiceTracker);
	  }

	  protected internal virtual void stopTrackingServices()
	  {
		serviceContainer.removeListener(processEngineServiceTracker);
	  }

	  /// <summary>
	  /// <para>invoked by the <seealso cref="MscManagedProcessEngine"/> and <seealso cref="MscManagedProcessEngineController"/>
	  /// when a process engine is started</para>
	  /// </summary>
	  public virtual void processEngineStarted(ProcessEngine processEngine)
	  {
		processEngines.Add(processEngine);
	  }

	  /// <summary>
	  /// <para>invoked by the <seealso cref="MscManagedProcessEngine"/> and <seealso cref="MscManagedProcessEngineController"/>
	  /// when a process engine is stopped</para>
	  /// </summary>
	  public virtual void processEngineStopped(ProcessEngine processEngine)
	  {
		processEngines.remove(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected MscManagedProcessApplication getManagedProcessApplication(String name)
	  protected internal virtual MscManagedProcessApplication getManagedProcessApplication(string name)
	  {
		ServiceController<MscManagedProcessApplication> serviceController = (ServiceController<MscManagedProcessApplication>) serviceContainer.getService(ServiceNames.forManagedProcessApplication(name));

		if (serviceController != null)
		{
		  return serviceController.Value;
		}
		else
		{
		  return null;
		}
	  }

	}

}