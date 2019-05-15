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
namespace org.camunda.bpm.container.impl.deployment.jobexecutor
{

	using BpmPlatformXmlImpl = org.camunda.bpm.container.impl.metadata.BpmPlatformXmlImpl;
	using JobAcquisitionXmlImpl = org.camunda.bpm.container.impl.metadata.JobAcquisitionXmlImpl;
	using JobExecutorXmlImpl = org.camunda.bpm.container.impl.metadata.JobExecutorXmlImpl;
	using ProcessEngineXmlImpl = org.camunda.bpm.container.impl.metadata.ProcessEngineXmlImpl;
	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PlatformJobExecutorActivateTest
	{

	  private const string ENGINE_NAME = "PlatformJobExecutorActivateTest-engine";
	  private const string ACQUISITION_NAME = "PlatformJobExecutorActivateTest-acquisition";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAutoActivateIfNoPropertySet()
	  public virtual void shouldAutoActivateIfNoPropertySet()
	  {

		// given
		JobExecutorXmlImpl jobExecutorXml = defineJobExecutor();
		ProcessEngineXmlImpl processEngineXml = defineProcessEngine();
		BpmPlatformXmlImpl bpmPlatformXml = new BpmPlatformXmlImpl(jobExecutorXml, Collections.singletonList<ProcessEngineXml>(processEngineXml));

		// when
		deployPlatform(bpmPlatformXml);

		try
		{
		  ProcessEngine processEngine = getProcessEngine(ENGINE_NAME);
		  ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		  // then
		  assertEquals(true, processEngineConfiguration.JobExecutor.Active);
		}
		finally
		{
		  undeployPlatform();
		}


	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotAutoActivateIfConfigured()
	  public virtual void shouldNotAutoActivateIfConfigured()
	  {

		// given
		JobExecutorXmlImpl jobExecutorXml = defineJobExecutor();
		ProcessEngineXmlImpl processEngineXml = defineProcessEngine();
		// activate set to false
		processEngineXml.Properties["jobExecutorActivate"] = "false";
		BpmPlatformXmlImpl bpmPlatformXml = new BpmPlatformXmlImpl(jobExecutorXml, Collections.singletonList<ProcessEngineXml>(processEngineXml));

		// when
		deployPlatform(bpmPlatformXml);

		try
		{
		  ProcessEngine processEngine = getProcessEngine(ENGINE_NAME);
		  ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		  // then
		  assertEquals(false, processEngineConfiguration.JobExecutor.Active);
		}
		finally
		{
		  undeployPlatform();
		}
	  }


	  protected internal virtual ProcessEngine getProcessEngine(string engineName)
	  {
		RuntimeContainerDelegateImpl containerDelegate = (RuntimeContainerDelegateImpl) org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		return containerDelegate.getProcessEngine(engineName);
	  }

	  private ProcessEngineXmlImpl defineProcessEngine()
	  {
		ProcessEngineXmlImpl processEngineXml = new ProcessEngineXmlImpl();
		Dictionary<string, string> properties = new Dictionary<string, string>();
		properties["jdbcUrl"] = "jdbc:h2:mem:PlatformJobExecutorActivateTest-db";
		processEngineXml.Properties = properties;
		processEngineXml.Plugins = new List<ProcessEnginePluginXml>();
		processEngineXml.Name = ENGINE_NAME;
		processEngineXml.JobAcquisitionName = ACQUISITION_NAME;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		processEngineXml.ConfigurationClass = typeof(StandaloneInMemProcessEngineConfiguration).FullName;
		processEngineXml.Default = true;
		return processEngineXml;
	  }


	  private JobExecutorXmlImpl defineJobExecutor()
	  {
		JobAcquisitionXmlImpl jobAcquisition = new JobAcquisitionXmlImpl();
		jobAcquisition.Properties = new Dictionary<string, string>();
		jobAcquisition.Name = ACQUISITION_NAME;
		JobExecutorXmlImpl jobExecutorXml = new JobExecutorXmlImpl();
		jobExecutorXml.Properties = new Dictionary<string, string>();
		jobExecutorXml.JobAcquisitions = Collections.singletonList<JobAcquisitionXml>(jobAcquisition);
		return jobExecutorXml;
	  }

	  private void undeployPlatform()
	  {
		RuntimeContainerDelegateImpl containerDelegate = (RuntimeContainerDelegateImpl) org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		containerDelegate.ServiceContainer.createUndeploymentOperation("deploy BPM platform").addStep(new StopJobExecutorStep()).addStep(new StopProcessEnginesStep()).addStep(new StopManagedThreadPoolStep()).execute();
	  }

	  private void deployPlatform(BpmPlatformXmlImpl bpmPlatformXml)
	  {
		RuntimeContainerDelegateImpl containerDelegate = (RuntimeContainerDelegateImpl) org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		containerDelegate.ServiceContainer.createDeploymentOperation("deploy BPM platform").addAttachment(Attachments.BPM_PLATFORM_XML, bpmPlatformXml).addStep(new StartManagedThreadPoolStep()).addStep(new StartJobExecutorStep()).addStep(new PlatformXmlStartProcessEnginesStep()).execute();
	  }
	}

}