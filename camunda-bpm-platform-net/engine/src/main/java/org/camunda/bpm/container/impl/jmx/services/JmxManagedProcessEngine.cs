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
namespace org.camunda.bpm.container.impl.jmx.services
{

	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// <para>Represents a process engine managed by the <seealso cref="MBeanServiceContainer"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JmxManagedProcessEngine : PlatformService<ProcessEngine>, JmxManagedProcessEngineMBean
	{

	  protected internal ProcessEngine processEngine;

	  // for subclasses
	  protected internal JmxManagedProcessEngine()
	  {
	  }

	  public JmxManagedProcessEngine(ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
	  }

	  public virtual void start(PlatformServiceContainer contanier)
	  {
		// this one has no lifecycle support
	  }

	  public virtual void stop(PlatformServiceContainer container)
	  {
		// this one has no lifecycle support
	  }

	  public virtual string Name
	  {
		  get
		  {
			return processEngine.Name;
		  }
	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return processEngine;
		  }
	  }

	  public virtual ProcessEngine Value
	  {
		  get
		  {
			return processEngine;
		  }
	  }

	  public virtual ISet<string> RegisteredDeployments
	  {
		  get
		  {
			ManagementService managementService = processEngine.ManagementService;
			return managementService.RegisteredDeployments;
		  }
	  }

	  public virtual void registerDeployment(string deploymentId)
	  {
		ManagementService managementService = processEngine.ManagementService;
		managementService.registerDeploymentForJobExecutor(deploymentId);
	  }

	  public virtual void unregisterDeployment(string deploymentId)
	  {
		ManagementService managementService = processEngine.ManagementService;
		managementService.unregisterDeploymentForJobExecutor(deploymentId);
	  }

	  public virtual void reportDbMetrics()
	  {
		ManagementService managementService = processEngine.ManagementService;
		managementService.reportDbMetricsNow();
	  }

	}

}