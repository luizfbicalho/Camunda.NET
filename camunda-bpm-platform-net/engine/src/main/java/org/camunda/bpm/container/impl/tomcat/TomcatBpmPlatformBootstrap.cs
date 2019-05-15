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
namespace org.camunda.bpm.container.impl.tomcat
{
	using Lifecycle = org.apache.catalina.Lifecycle;
	using LifecycleEvent = org.apache.catalina.LifecycleEvent;
	using LifecycleListener = org.apache.catalina.LifecycleListener;
	using StandardServer = org.apache.catalina.core.StandardServer;
	using DiscoverBpmPlatformPluginsStep = org.camunda.bpm.container.impl.deployment.DiscoverBpmPlatformPluginsStep;
	using PlatformXmlStartProcessEnginesStep = org.camunda.bpm.container.impl.deployment.PlatformXmlStartProcessEnginesStep;
	using StopProcessApplicationsStep = org.camunda.bpm.container.impl.deployment.StopProcessApplicationsStep;
	using StopProcessEnginesStep = org.camunda.bpm.container.impl.deployment.StopProcessEnginesStep;
	using UnregisterBpmPlatformPluginsStep = org.camunda.bpm.container.impl.deployment.UnregisterBpmPlatformPluginsStep;
	using StartJobExecutorStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StartJobExecutorStep;
	using StartManagedThreadPoolStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StartManagedThreadPoolStep;
	using StopJobExecutorStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StopJobExecutorStep;
	using StopManagedThreadPoolStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StopManagedThreadPoolStep;
	using TomcatAttachments = org.camunda.bpm.container.impl.tomcat.deployment.TomcatAttachments;
	using TomcatParseBpmPlatformXmlStep = org.camunda.bpm.container.impl.tomcat.deployment.TomcatParseBpmPlatformXmlStep;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>Apache Tomcat server listener responsible for deploying the bpm platform.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TomcatBpmPlatformBootstrap : LifecycleListener
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal ProcessEngine processEngine;

	  protected internal RuntimeContainerDelegateImpl containerDelegate;

	  public virtual void lifecycleEvent(LifecycleEvent @event)
	  {

		if (Lifecycle.START_EVENT.Equals(@event.Type))
		{

		  // the Apache Tomcat integration uses the Jmx Container for managing process engines and applications.
		  containerDelegate = (RuntimeContainerDelegateImpl) org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();

		  deployBpmPlatform(@event);

		}
		else if (Lifecycle.STOP_EVENT.Equals(@event.Type))
		{

		  undeployBpmPlatform(@event);

		}

	  }

	  protected internal virtual void deployBpmPlatform(LifecycleEvent @event)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.catalina.core.StandardServer server = (org.apache.catalina.core.StandardServer) event.getSource();
		StandardServer server = (StandardServer) @event.Source;

		containerDelegate.ServiceContainer.createDeploymentOperation("deploy BPM platform").addAttachment(TomcatAttachments.SERVER, server).addStep(new TomcatParseBpmPlatformXmlStep()).addStep(new DiscoverBpmPlatformPluginsStep()).addStep(new StartManagedThreadPoolStep()).addStep(new StartJobExecutorStep()).addStep(new PlatformXmlStartProcessEnginesStep()).execute();

		LOG.camundaBpmPlatformSuccessfullyStarted(server.ServerInfo);

	  }


	  protected internal virtual void undeployBpmPlatform(LifecycleEvent @event)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.catalina.core.StandardServer server = (org.apache.catalina.core.StandardServer) event.getSource();
		StandardServer server = (StandardServer) @event.Source;

		containerDelegate.ServiceContainer.createUndeploymentOperation("undeploy BPM platform").addAttachment(TomcatAttachments.SERVER, server).addStep(new StopJobExecutorStep()).addStep(new StopManagedThreadPoolStep()).addStep(new StopProcessApplicationsStep()).addStep(new StopProcessEnginesStep()).addStep(new UnregisterBpmPlatformPluginsStep()).execute();

		LOG.camundaBpmPlatformStopped(server.ServerInfo);
	  }

	}

}