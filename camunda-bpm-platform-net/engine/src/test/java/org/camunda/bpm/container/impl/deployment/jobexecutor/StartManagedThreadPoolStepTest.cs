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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using MBeanServiceContainer = org.camunda.bpm.container.impl.jmx.MBeanServiceContainer;
	using JmxManagedThreadPool = org.camunda.bpm.container.impl.jmx.services.JmxManagedThreadPool;
	using BpmPlatformXmlImpl = org.camunda.bpm.container.impl.metadata.BpmPlatformXmlImpl;
	using JobExecutorXmlImpl = org.camunda.bpm.container.impl.metadata.JobExecutorXmlImpl;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class StartManagedThreadPoolStepTest
	{

	  private MBeanServiceContainer container = new MBeanServiceContainer();

	  private DeploymentOperation deploymentOperation;

	  private JobExecutorXmlImpl jobExecutorXml;

	  private BpmPlatformXml bpmPlatformXml;

	  private StartManagedThreadPoolStep step;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		step = new StartManagedThreadPoolStep();
		deploymentOperation = new DeploymentOperation("name", container, System.Linq.Enumerable.Empty<DeploymentOperationStep> ());
		jobExecutorXml = new JobExecutorXmlImpl();
		bpmPlatformXml = new BpmPlatformXmlImpl(jobExecutorXml, System.Linq.Enumerable.Empty<ProcessEngineXml>());
		deploymentOperation.addAttachment(Attachments.BPM_PLATFORM_XML, bpmPlatformXml);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		container.stopService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_EXECUTOR);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void performOperationStepWithDefaultProperties()
	  public virtual void performOperationStepWithDefaultProperties()
	  {
		IDictionary<string, string> properties = new Dictionary<string, string>();
		jobExecutorXml.Properties = properties;
		step.performOperationStep(deploymentOperation);

		PlatformService<JmxManagedThreadPool> service = container.getService(ObjectNameForExecutor);
		ThreadPoolExecutor executor = service.Value.ThreadPoolExecutor;

		//since no jobs will start, remaining capacity is sufficent to check the size
		assertThat(executor.Queue.remainingCapacity(), @is(3));
		assertThat(executor.CorePoolSize, @is(3));
		assertThat(executor.MaximumPoolSize, @is(10));
		assertThat(executor.getKeepAliveTime(TimeUnit.MILLISECONDS), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void performOperationStepWithPropertiesInXml()
	  public virtual void performOperationStepWithPropertiesInXml()
	  {
		IDictionary<string, string> properties = new Dictionary<string, string>();
		string queueSize = "5";
		string corePoolSize = "12";
		string maxPoolSize = "20";
		string keepAliveTime = "100";
		properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.CORE_POOL_SIZE] = corePoolSize;
		properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.KEEP_ALIVE_TIME] = keepAliveTime;
		properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.MAX_POOL_SIZE] = maxPoolSize;
		properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.QUEUE_SIZE] = queueSize;
		jobExecutorXml.Properties = properties;
		step.performOperationStep(deploymentOperation);

		PlatformService<JmxManagedThreadPool> service = container.getService(ObjectNameForExecutor);
		ThreadPoolExecutor executor = service.Value.ThreadPoolExecutor;

		//since no jobs will start, remaining capacity is sufficent to check the size
		assertThat(executor.Queue.remainingCapacity(), @is(int.Parse(queueSize)));
		assertThat(executor.CorePoolSize, @is(int.Parse(corePoolSize)));
		assertThat(executor.MaximumPoolSize, @is(int.Parse(maxPoolSize)));
		assertThat(executor.getKeepAliveTime(TimeUnit.MILLISECONDS), @is(long.Parse(keepAliveTime)));
	  }

	  private ObjectName ObjectNameForExecutor
	  {
		  get
		  {
			string localName = MBeanServiceContainer.composeLocalName(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_EXECUTOR);
			return MBeanServiceContainer.getObjectName(localName);
		  }
	  }
	}

}