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

	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using ComponentView = org.jboss.@as.ee.component.ComponentView;
	using ManagedReference = org.jboss.@as.naming.ManagedReference;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;

	/// <summary>
	/// Responsible for invoking <seealso cref="BpmPlatformPlugin.postProcessApplicationUndeploy(ProcessApplicationInterface)"/>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationStopService : Service<ProcessApplicationStopService>
	{


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(ProcessApplicationStopService).FullName);

	  // for view-exposing ProcessApplicationComponents
	  protected internal InjectedValue<ComponentView> paComponentViewInjector = new InjectedValue<ComponentView>();
	  protected internal InjectedValue<ProcessApplicationInterface> noViewProcessApplication = new InjectedValue<ProcessApplicationInterface>();

	  protected internal InjectedValue<BpmPlatformPlugins> platformPluginsInjector = new InjectedValue<BpmPlatformPlugins>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ProcessApplicationStopService getValue() throws IllegalStateException, IllegalArgumentException
	  public override ProcessApplicationStopService Value
	  {
		  get
		  {
			return this;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void start(org.jboss.msc.service.StartContext arg0) throws org.jboss.msc.service.StartException
	  public override void start(StartContext arg0)
	  {
		// nothing to do
	  }

	  public override void stop(StopContext arg0)
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

		  BpmPlatformPlugins bpmPlatformPlugins = platformPluginsInjector.Value;
		  IList<BpmPlatformPlugin> plugins = bpmPlatformPlugins.Plugins;

		  foreach (BpmPlatformPlugin bpmPlatformPlugin in plugins)
		  {
			bpmPlatformPlugin.postProcessApplicationUndeploy(processApplication);
		  }

		}
		catch (Exception e)
		{
		  LOGGER.log(Level.WARNING, "Exception while invoking BpmPlatformPlugin.postProcessApplicationUndeploy", e);

		}
		finally
		{
		  if (reference != null)
		  {
			reference.release();
		  }
		}
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

	  public virtual InjectedValue<BpmPlatformPlugins> PlatformPluginsInjector
	  {
		  get
		  {
			return platformPluginsInjector;
		  }
	  }

	}

}