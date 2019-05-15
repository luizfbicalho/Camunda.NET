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

	using BindingUtil = org.camunda.bpm.container.impl.jboss.util.BindingUtil;
	using ProcessEngineManagedReferenceFactory = org.camunda.bpm.container.impl.jboss.util.ProcessEngineManagedReferenceFactory;
	using JmxManagedProcessEngine = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessEngine;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ManagedReferenceFactory = org.jboss.@as.naming.ManagedReferenceFactory;
	using ContextNames = org.jboss.@as.naming.deployment.ContextNames;
	using Injector = org.jboss.msc.inject.Injector;
	using Service = org.jboss.msc.service.Service;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;

	/// <summary>
	/// <para>Service representing a managed process engine instance registered with the Msc.</para>
	/// 
	/// <para>Instances of this service are created and registered by the <seealso cref="MscRuntimeContainerDelegate"/> 
	/// when <seealso cref="MscRuntimeContainerDelegate#registerProcessEngine(ProcessEngine)"/> is called.</para>
	/// 
	/// <para>This is the JBoass Msc counterpart of the <seealso cref="JmxManagedProcessEngine"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MscManagedProcessEngine : Service<ProcessEngine>
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGG = Logger.getLogger(typeof(MscManagedProcessEngine).FullName);

	  protected internal InjectedValue<MscRuntimeContainerDelegate> runtimeContainerDelegateInjector = new InjectedValue<MscRuntimeContainerDelegate>();

	  /// <summary>
	  /// the process engine managed by this service </summary>
	  protected internal ProcessEngine processEngine;

	  private ServiceController<ManagedReferenceFactory> bindingService;

	  // for subclasses only
	  protected internal MscManagedProcessEngine()
	  {
	  }

	  public MscManagedProcessEngine(ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.ProcessEngine getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual ProcessEngine Value
	  {
		  get
		  {
			return processEngine;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {
		MscRuntimeContainerDelegate runtimeContainerDelegate = runtimeContainerDelegateInjector.Value;
		runtimeContainerDelegate.processEngineStarted(processEngine);

		createProcessEngineJndiBinding(context);
	  }

	  protected internal virtual void createProcessEngineJndiBinding(StartContext context)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.jboss.util.ProcessEngineManagedReferenceFactory managedReferenceFactory = new org.camunda.bpm.container.impl.jboss.util.ProcessEngineManagedReferenceFactory(processEngine);
		ProcessEngineManagedReferenceFactory managedReferenceFactory = new ProcessEngineManagedReferenceFactory(processEngine);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName processEngineServiceBindingServiceName = org.jboss.as.naming.deployment.ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(org.camunda.bpm.BpmPlatform.APP_JNDI_NAME).append(org.camunda.bpm.BpmPlatform.MODULE_JNDI_NAME).append(processEngine.getName());
		ServiceName processEngineServiceBindingServiceName = ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append(BpmPlatform.APP_JNDI_NAME).append(BpmPlatform.MODULE_JNDI_NAME).append(processEngine.Name);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jndiName = org.camunda.bpm.BpmPlatform.JNDI_NAME_PREFIX + "/" + org.camunda.bpm.BpmPlatform.APP_JNDI_NAME + "/" + org.camunda.bpm.BpmPlatform.MODULE_JNDI_NAME + "/" +processEngine.getName();
		string jndiName = BpmPlatform.JNDI_NAME_PREFIX + "/" + BpmPlatform.APP_JNDI_NAME + "/" + BpmPlatform.MODULE_JNDI_NAME + "/" + processEngine.Name;

		// bind process engine service
		bindingService = BindingUtil.createJndiBindings(context.ChildTarget, processEngineServiceBindingServiceName, jndiName, managedReferenceFactory);

		// log info message
		LOGG.info("jndi binding for process engine " + processEngine.Name + " is " + jndiName);
	  }

	  protected internal virtual void removeProcessEngineJndiBinding()
	  {
		bindingService.Mode = ServiceController.Mode.REMOVE;
	  }

	  public virtual void stop(StopContext context)
	  {
		MscRuntimeContainerDelegate runtimeContainerDelegate = runtimeContainerDelegateInjector.Value;
		runtimeContainerDelegate.processEngineStopped(processEngine);
	  }

	  public virtual Injector<MscRuntimeContainerDelegate> RuntimeContainerDelegateInjector
	  {
		  get
		  {
			return runtimeContainerDelegateInjector;
		  }
	  }

	}

}