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
namespace org.camunda.bpm.container.impl.ejb
{
	using DiscoverBpmPlatformPluginsStep = org.camunda.bpm.container.impl.deployment.DiscoverBpmPlatformPluginsStep;
	using PlatformXmlStartProcessEnginesStep = org.camunda.bpm.container.impl.deployment.PlatformXmlStartProcessEnginesStep;
	using StopProcessApplicationsStep = org.camunda.bpm.container.impl.deployment.StopProcessApplicationsStep;
	using StopProcessEnginesStep = org.camunda.bpm.container.impl.deployment.StopProcessEnginesStep;
	using UnregisterBpmPlatformPluginsStep = org.camunda.bpm.container.impl.deployment.UnregisterBpmPlatformPluginsStep;
	using StartJobExecutorStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StartJobExecutorStep;
	using StopJobExecutorStep = org.camunda.bpm.container.impl.deployment.jobexecutor.StopJobExecutorStep;
	using EjbJarParsePlatformXmlStep = org.camunda.bpm.container.impl.ejb.deployment.EjbJarParsePlatformXmlStep;
	using StartJcaExecutorServiceStep = org.camunda.bpm.container.impl.ejb.deployment.StartJcaExecutorServiceStep;
	using StopJcaExecutorServiceStep = org.camunda.bpm.container.impl.ejb.deployment.StopJcaExecutorServiceStep;




	/// <summary>
	/// <para>Bootstrap for the camunda BPM platform using a singleton EJB</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Startup @Singleton(name="BpmPlatformBootstrap") @TransactionAttribute(TransactionAttributeType.NOT_SUPPORTED) public class EjbBpmPlatformBootstrap
	public class EjbBpmPlatformBootstrap
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(EjbBpmPlatformBootstrap).FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @EJB protected org.camunda.bpm.container.ExecutorService executorServiceBean;
	  protected internal ExecutorService executorServiceBean;

	  protected internal ProcessEngineService processEngineService;
	  protected internal ProcessApplicationService processApplicationService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostConstruct protected void start()
	  protected internal virtual void start()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.RuntimeContainerDelegateImpl containerDelegate = getContainerDelegate();
		RuntimeContainerDelegateImpl containerDelegate = ContainerDelegate;

		containerDelegate.ServiceContainer.createDeploymentOperation("deploying camunda BPM platform").addStep(new EjbJarParsePlatformXmlStep()).addStep(new DiscoverBpmPlatformPluginsStep()).addStep(new StartJcaExecutorServiceStep(executorServiceBean)).addStep(new StartJobExecutorStep()).addStep(new PlatformXmlStartProcessEnginesStep()).execute();

		processEngineService = containerDelegate.ProcessEngineService;
		processApplicationService = containerDelegate.ProcessApplicationService;

		LOGGER.log(Level.INFO, "camunda BPM platform started successfully.");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PreDestroy protected void stop()
	  protected internal virtual void stop()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.RuntimeContainerDelegateImpl containerDelegate = getContainerDelegate();
		RuntimeContainerDelegateImpl containerDelegate = ContainerDelegate;

		containerDelegate.ServiceContainer.createUndeploymentOperation("undeploying camunda BPM platform").addStep(new StopProcessApplicationsStep()).addStep(new StopProcessEnginesStep()).addStep(new StopJobExecutorStep()).addStep(new StopJcaExecutorServiceStep()).addStep(new UnregisterBpmPlatformPluginsStep()).execute();

		LOGGER.log(Level.INFO, "camunda BPM platform stopped.");

	  }

	  protected internal virtual RuntimeContainerDelegateImpl ContainerDelegate
	  {
		  get
		  {
			return (RuntimeContainerDelegateImpl) org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		  }
	  }

	  // getters //////////////////////////////////////////////

	  public virtual ProcessEngineService ProcessEngineService
	  {
		  get
		  {
			return processEngineService;
		  }
	  }

	  public virtual ProcessApplicationService ProcessApplicationService
	  {
		  get
		  {
			return processApplicationService;
		  }
	  }

	}

}