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
namespace org.camunda.bpm.application
{
	using DefaultElResolverLookup = org.camunda.bpm.application.impl.DefaultElResolverLookup;
	using ProcessApplicationLogger = org.camunda.bpm.application.impl.ProcessApplicationLogger;
	using ProcessApplicationScriptEnvironment = org.camunda.bpm.application.impl.ProcessApplicationScriptEnvironment;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProcessEngines = org.camunda.bpm.engine.ProcessEngines;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using BeanELResolver = org.camunda.bpm.engine.impl.javax.el.BeanELResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;



	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public abstract class AbstractProcessApplication : ProcessApplicationInterface
	{
		public abstract ProcessApplicationReference Reference {get;}

	  private static ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  protected internal ELResolver processApplicationElResolver;
	  protected internal BeanELResolver processApplicationBeanElResolver;
	  protected internal ProcessApplicationScriptEnvironment processApplicationScriptEnvironment;

	  protected internal VariableSerializers variableSerializers;

	  protected internal bool isDeployed = false;

	  protected internal string defaultDeployToEngineName = ProcessEngines.NAME_DEFAULT;

	  // deployment /////////////////////////////////////////////////////

	  public virtual void deploy()
	  {
		if (isDeployed)
		{
		  LOG.alreadyDeployed();
		}
		else
		{
		  // deploy the application
		  org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().deployProcessApplication(this);
		  isDeployed = true;
		}
	  }

	  public virtual void undeploy()
	  {
		if (!isDeployed)
		{
		  LOG.notDeployed();
		}
		else
		{
		  // delegate stopping of the process application to the runtime container.
		  org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().undeployProcessApplication(this);
		  isDeployed = false;
		}
	  }

	  public virtual void createDeployment(string processArchiveName, DeploymentBuilder deploymentBuilder)
	  {
		// default implementation does nothing
	  }

	  // Runtime ////////////////////////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			Type processApplicationClass = this.GetType();
			string name = null;
    
			ProcessApplication annotation = processApplicationClass.getAnnotation(typeof(ProcessApplication));
			if (annotation != null)
			{
			  name = annotation.value();
    
			  if (string.ReferenceEquals(name, null) || name.Length == 0)
			  {
				name = annotation.name();
			  }
			}
    
    
			if (string.ReferenceEquals(name, null) || name.Length == 0)
			{
			  name = autodetectProcessApplicationName();
			}
    
			return name;
		  }
	  }

	  /// <summary>
	  /// Override this method to autodetect an application name in case the
	  /// <seealso cref="ProcessApplication"/> annotation was used but without parameter.
	  /// </summary>
	  protected internal abstract string autodetectProcessApplicationName();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T execute(java.util.concurrent.Callable<T> callable) throws ProcessApplicationExecutionException
	  public virtual T execute<T>(Callable<T> callable)
	  {
		ClassLoader originalClassloader = ClassLoaderUtil.ContextClassloader;
		ClassLoader processApplicationClassloader = ProcessApplicationClassloader;

		try
		{
		  ClassLoaderUtil.ContextClassloader = processApplicationClassloader;

		  return callable.call();

		}
		catch (Exception e)
		{
		  throw LOG.processApplicationExecutionException(e);
		}
		finally
		{
		  ClassLoaderUtil.ContextClassloader = originalClassloader;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T execute(java.util.concurrent.Callable<T> callable, InvocationContext invocationContext) throws ProcessApplicationExecutionException
	  public virtual T execute<T>(Callable<T> callable, InvocationContext invocationContext)
	  {
		// allows to hook into the invocation
		return execute(callable);
	  }

	  public virtual ClassLoader ProcessApplicationClassloader
	  {
		  get
		  {
			// the default implementation uses the classloader that loaded
			// the application-provided subclass of this class.
			return ClassLoaderUtil.getClassloader(this.GetType());
		  }
	  }

	  public virtual ProcessApplicationInterface RawObject
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return System.Linq.Enumerable.Empty<string, string>();
		  }
	  }

	  public virtual ELResolver ElResolver
	  {
		  get
		  {
			if (processApplicationElResolver == null)
			{
			  lock (this)
			  {
				if (processApplicationElResolver == null)
				{
				  processApplicationElResolver = initProcessApplicationElResolver();
				}
			  }
			}
			return processApplicationElResolver;
    
		  }
	  }

	  public virtual BeanELResolver BeanElResolver
	  {
		  get
		  {
			if (processApplicationBeanElResolver == null)
			{
			  lock (this)
			  {
				if (processApplicationBeanElResolver == null)
				{
				  processApplicationBeanElResolver = new BeanELResolver();
				}
			  }
			}
			return processApplicationBeanElResolver;
		  }
	  }

	  /// <summary>
	  /// <para>Initializes the process application provided ElResolver. This implementation uses the
	  /// Java SE <seealso cref="ServiceLoader"/> facilities for resolving implementations of <seealso cref="ProcessApplicationElResolver"/>.</para>
	  /// <para>
	  /// </para>
	  /// <para>If you want to provide a custom implementation in your application, place a file named
	  /// <code>META-INF/org.camunda.bpm.application.ProcessApplicationElResolver</code> inside your application
	  /// which contains the fully qualified classname of your implementation. Or simply override this method.</para>
	  /// </summary>
	  /// <returns> the process application ElResolver. </returns>
	  protected internal virtual ELResolver initProcessApplicationElResolver()
	  {

		return DefaultElResolverLookup.lookupResolver(this);

	  }

	  public virtual ExecutionListener ExecutionListener
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual TaskListener TaskListener
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// see <seealso cref="ProcessApplicationScriptEnvironment.getScriptEngineForName(string, bool)"/>
	  /// </summary>
	  public virtual ScriptEngine getScriptEngineForName(string name, bool cache)
	  {
		return ProcessApplicationScriptEnvironment.getScriptEngineForName(name, cache);
	  }

	  /// <summary>
	  /// see <seealso cref="ProcessApplicationScriptEnvironment.getEnvironmentScripts()"/>
	  /// </summary>
	  public virtual IDictionary<string, IList<ExecutableScript>> EnvironmentScripts
	  {
		  get
		  {
			return ProcessApplicationScriptEnvironment.EnvironmentScripts;
		  }
	  }

	  protected internal virtual ProcessApplicationScriptEnvironment ProcessApplicationScriptEnvironment
	  {
		  get
		  {
			if (processApplicationScriptEnvironment == null)
			{
			  lock (this)
			  {
				if (processApplicationScriptEnvironment == null)
				{
				  processApplicationScriptEnvironment = new ProcessApplicationScriptEnvironment(this);
				}
			  }
			}
			return processApplicationScriptEnvironment;
		  }
	  }

	  public virtual VariableSerializers VariableSerializers
	  {
		  get
		  {
			return variableSerializers;
		  }
		  set
		  {
			this.variableSerializers = value;
		  }
	  }


	  /// <summary>
	  /// <para>Provides the default Process Engine name to deploy to, if no Process Engine
	  /// was defined in <code>processes.xml</code>.</para>
	  /// </summary>
	  /// <returns> the default deploy-to Process Engine name.
	  ///         The default value is "default". </returns>
	  public virtual string DefaultDeployToEngineName
	  {
		  get
		  {
			return defaultDeployToEngineName;
		  }
		  set
		  {
			this.defaultDeployToEngineName = value;
		  }
	  }

	}

}