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
namespace org.camunda.bpm.container.impl.jboss.service
{

	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using PreUndeploy = org.camunda.bpm.application.PreUndeploy;
	using ProcessApplicationDeploymentInfo = org.camunda.bpm.application.ProcessApplicationDeploymentInfo;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationDeploymentInfoImpl = org.camunda.bpm.application.impl.ProcessApplicationDeploymentInfoImpl;
	using ProcessApplicationInfoImpl = org.camunda.bpm.application.impl.ProcessApplicationInfoImpl;
	using InjectionUtil = org.camunda.bpm.container.impl.deployment.util.InjectionUtil;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ComponentView = org.jboss.@as.ee.component.ComponentView;
	using ManagedReference = org.jboss.@as.naming.ManagedReference;
	using AnnotationInstance = org.jboss.jandex.AnnotationInstance;
	using MethodInfo = org.jboss.jandex.MethodInfo;
	using Module = org.jboss.modules.Module;
	using Service = org.jboss.msc.service.Service;
	using ServiceContainer = org.jboss.msc.service.ServiceContainer;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;

	/// <summary>
	/// <para>This service is responsible for starting the <seealso cref="MscManagedProcessApplication"/> service.</para>
	/// 
	/// <para>We need this as an extra step since we need a declarative dependency on the
	/// ProcessApplicationComponent in order to call the getName() method on the ProcessApplication.
	/// The name of the process application is subsequently used for composing the name of the
	/// <seealso cref="MscManagedProcessApplication"/> service which means that it must be available when
	/// registering the service.</para>
	/// 
	/// <para>This service depends on all <seealso cref="ProcessApplicationDeploymentService"/> instances started for the
	/// process application. Thus, when this service is started, it is guaranteed that all process application
	/// deployments have completed successfully.</para>
	/// 
	/// <para>This service creates the <seealso cref="ProcessApplicationInfo"/> object and passes it to the
	/// <seealso cref="MscManagedProcessApplication"/> service.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationStartService : Service<ProcessApplicationStartService>
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(ProcessApplicationStartService).FullName);

	  /// <summary>
	  /// the names of the deployment services we depend on; those must be added as
	  /// declarative dependencies when the service is installed. 
	  /// </summary>
	  protected internal readonly ICollection<ServiceName> deploymentServiceNames;

	  // for view-exposing ProcessApplicationComponents
	  protected internal InjectedValue<ComponentView> paComponentViewInjector = new InjectedValue<ComponentView>();
	  protected internal InjectedValue<ProcessApplicationInterface> noViewProcessApplication = new InjectedValue<ProcessApplicationInterface>();

	  /// <summary>
	  /// injector for the default process engine </summary>
	  protected internal InjectedValue<ProcessEngine> defaultProcessEngineInjector = new InjectedValue<ProcessEngine>();

	  protected internal InjectedValue<BpmPlatformPlugins> platformPluginsInjector = new InjectedValue<BpmPlatformPlugins>();

	  protected internal AnnotationInstance preUndeployDescription;
	  protected internal AnnotationInstance postDeployDescription;

	  protected internal ProcessApplicationInfoImpl processApplicationInfo;
	  protected internal HashSet<ProcessEngine> referencedProcessEngines;

	  protected internal Module paModule;

	  public ProcessApplicationStartService(ICollection<ServiceName> deploymentServiceNames, AnnotationInstance postDeployDescription, AnnotationInstance preUndeployDescription, Module paModule)
	  {
		this.deploymentServiceNames = deploymentServiceNames;
		this.postDeployDescription = postDeployDescription;
		this.preUndeployDescription = preUndeployDescription;
		this.paModule = paModule;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public override void start(StartContext context)
	  {

		ManagedReference reference = null;
		try
		{

		  // get the process application component
		  ProcessApplicationInterface processApplication = null;
		  ComponentView componentView = paComponentViewInjector.OptionalValue;
		  if (componentView != null)
		  {
			reference = componentView.createInstance();
			processApplication = (ProcessApplicationInterface) reference.Instance;
		  }
		  else
		  {
			processApplication = noViewProcessApplication.Value;
		  }

		  // create & populate the process application info object
		  processApplicationInfo = new ProcessApplicationInfoImpl();
		  processApplicationInfo.Name = processApplication.Name;
		  processApplicationInfo.Properties = processApplication.Properties;

		  referencedProcessEngines = new HashSet<ProcessEngine>();
		  IList<ProcessApplicationDeploymentInfo> deploymentInfos = new List<ProcessApplicationDeploymentInfo>();

		  foreach (ServiceName deploymentServiceName in deploymentServiceNames)
		  {

			ProcessApplicationDeploymentService value = getDeploymentService(context, deploymentServiceName);
			referencedProcessEngines.Add(value.ProcessEngineInjector.Value);

			ProcessApplicationDeployment deployment = value.Deployment;
			if (deployment != null)
			{
			  foreach (string deploymentId in deployment.ProcessApplicationRegistration.DeploymentIds)
			  {
				ProcessApplicationDeploymentInfoImpl deploymentInfo = new ProcessApplicationDeploymentInfoImpl();
				deploymentInfo.DeploymentId = deploymentId;
				deploymentInfo.ProcessEngineName = value.ProcessEngineName;
				deploymentInfos.Add(deploymentInfo);
			  }
			}

		  }
		  processApplicationInfo.DeploymentInfo = deploymentInfos;

		  notifyBpmPlatformPlugins(platformPluginsInjector.Value, processApplication);

		  if (postDeployDescription != null)
		  {
			invokePostDeploy(processApplication);
		  }

		  // install the ManagedProcessApplication Service as a child to this service
		  // if this service stops (at undeployment) the ManagedProcessApplication service is removed as well.
		  ServiceName serviceName = ServiceNames.forManagedProcessApplication(processApplicationInfo.Name);
		  MscManagedProcessApplication managedProcessApplication = new MscManagedProcessApplication(processApplicationInfo, processApplication.Reference);
		  context.ChildTarget.addService(serviceName, managedProcessApplication).install();

		}
		catch (StartException e)
		{
		  throw e;

		}
		catch (Exception e)
		{
		  throw new StartException(e);

		}
		finally
		{
		  if (reference != null)
		  {
			reference.release();
		  }
		}
	  }

	  protected internal virtual void notifyBpmPlatformPlugins(BpmPlatformPlugins value, ProcessApplicationInterface processApplication)
	  {
		foreach (BpmPlatformPlugin plugin in value.Plugins)
		{
		  plugin.postProcessApplicationDeploy(processApplication);
		}
	  }

	  public override void stop(StopContext context)
	  {

		ManagedReference reference = null;
		try
		{

		  // get the process application component
		  ProcessApplicationInterface processApplication = null;
		  ComponentView componentView = paComponentViewInjector.OptionalValue;
		  if (componentView != null)
		  {
			reference = componentView.createInstance();
			processApplication = (ProcessApplicationInterface) reference.Instance;
		  }
		  else
		  {
			processApplication = noViewProcessApplication.Value;
		  }

		  invokePreUndeploy(processApplication);

		}
		catch (Exception e)
		{
		  LOGGER.log(Level.SEVERE, "Exception while stopping process application", e);

		}
		finally
		{
		  if (reference != null)
		  {
			reference.release();
		  }
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invokePostDeploy(final org.camunda.bpm.application.ProcessApplicationInterface processApplication) throws ClassNotFoundException, org.jboss.msc.service.StartException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void invokePostDeploy(ProcessApplicationInterface processApplication)
	  {
		Type paClass = getPaClass(postDeployDescription);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method postDeployMethod = org.camunda.bpm.container.impl.deployment.util.InjectionUtil.detectAnnotatedMethod(paClass, org.camunda.bpm.application.PostDeploy.class);
		System.Reflection.MethodInfo postDeployMethod = InjectionUtil.detectAnnotatedMethod(paClass, typeof(PostDeploy));

		if (postDeployMethod != null)
		{
		  try
		  {
			processApplication.execute(new CallableAnonymousInnerClass(this, processApplication, postDeployMethod));
		  }
		  catch (Exception e)
		  {
			throw new StartException("Exception while invoking the @PostDeploy method ", e);
		  }
		}

	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ProcessApplicationStartService outerInstance;

		  private ProcessApplicationInterface processApplication;
		  private System.Reflection.MethodInfo postDeployMethod;

		  public CallableAnonymousInnerClass(ProcessApplicationStartService outerInstance, ProcessApplicationInterface processApplication, System.Reflection.MethodInfo postDeployMethod)
		  {
			  this.outerInstance = outerInstance;
			  this.processApplication = processApplication;
			  this.postDeployMethod = postDeployMethod;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			postDeployMethod.invoke(processApplication.RawObject, outerInstance.getInjections(postDeployMethod));
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invokePreUndeploy(final org.camunda.bpm.application.ProcessApplicationInterface processApplication) throws ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void invokePreUndeploy(ProcessApplicationInterface processApplication)
	  {
		if (preUndeployDescription != null)
		{
		  Type paClass = getPaClass(preUndeployDescription);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method preUndeployMethod = org.camunda.bpm.container.impl.deployment.util.InjectionUtil.detectAnnotatedMethod(paClass, org.camunda.bpm.application.PreUndeploy.class);
		  System.Reflection.MethodInfo preUndeployMethod = InjectionUtil.detectAnnotatedMethod(paClass, typeof(PreUndeploy));

		  if (preUndeployMethod != null)
		  {
			try
			{
			  processApplication.execute(new CallableAnonymousInnerClass2(this, processApplication, preUndeployMethod));
			}
			catch (Exception e)
			{
			  throw new Exception("Exception while invoking the @PreUndeploy method ", e);
			}
		  }
		}

	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly ProcessApplicationStartService outerInstance;

		  private ProcessApplicationInterface processApplication;
		  private System.Reflection.MethodInfo preUndeployMethod;

		  public CallableAnonymousInnerClass2(ProcessApplicationStartService outerInstance, ProcessApplicationInterface processApplication, System.Reflection.MethodInfo preUndeployMethod)
		  {
			  this.outerInstance = outerInstance;
			  this.processApplication = processApplication;
			  this.preUndeployMethod = preUndeployMethod;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			preUndeployMethod.invoke(processApplication.RawObject, outerInstance.getInjections(preUndeployMethod));
			return null;
		  }
	  }

	  protected internal virtual object[] getInjections(System.Reflection.MethodInfo lifecycleMethod)
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

			// support injection of the default process engine, if present
			if (parameterClass.IsAssignableFrom(typeof(ProcessEngine)))
			{
			  parameters.Add(defaultProcessEngineInjector.OptionalValue);
			  injectionResolved = true;
			}

			// support injection of the ProcessApplicationInfo
			else if (parameterClass.IsAssignableFrom(typeof(ProcessApplicationInfo)))
			{
			  parameters.Add(processApplicationInfo);
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
			  parameters.Add(new List<ProcessEngine>(referencedProcessEngines));
			  injectionResolved = true;
			}
		  }

		  if (!injectionResolved)
		  {
			throw new ProcessEngineException("Unsupported parametertype " + parameterType);
		  }

		}

		return parameters.ToArray();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class getPaClass(org.jboss.jandex.AnnotationInstance annotation) throws ClassNotFoundException
	  protected internal virtual Type getPaClass(AnnotationInstance annotation)
	  {
		string paClassName = ((MethodInfo)annotation.target()).declaringClass().name().ToString();
		Type paClass = paModule.ClassLoader.loadClass(paClassName);
		return paClass;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private ProcessApplicationDeploymentService getDeploymentService(org.jboss.msc.service.StartContext context, org.jboss.msc.service.ServiceName deploymentServiceName)
	  private ProcessApplicationDeploymentService getDeploymentService(StartContext context, ServiceName deploymentServiceName)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceContainer serviceContainer = context.getController().getServiceContainer();
		ServiceContainer serviceContainer = context.Controller.ServiceContainer;
		ServiceController<ProcessApplicationDeploymentService> deploymentService = (ServiceController<ProcessApplicationDeploymentService>) serviceContainer.getRequiredService(deploymentServiceName);
		return deploymentService.Value;
	  }

	  public virtual InjectedValue<ProcessApplicationInterface> NoViewProcessApplication
	  {
		  get
		  {
			return noViewProcessApplication;
		  }
	  }

	  public virtual InjectedValue<ComponentView> PaComponentViewInjector
	  {
		  get
		  {
			return paComponentViewInjector;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ProcessApplicationStartService getValue() throws IllegalStateException, IllegalArgumentException
	  public override ProcessApplicationStartService Value
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual InjectedValue<ProcessEngine> DefaultProcessEngineInjector
	  {
		  get
		  {
			return defaultProcessEngineInjector;
		  }
	  }

	  public virtual InjectedValue<BpmPlatformPlugins> PlatformPluginsInjector
	  {
		  get
		  {
			return platformPluginsInjector;
		  }
	  }

	}

}