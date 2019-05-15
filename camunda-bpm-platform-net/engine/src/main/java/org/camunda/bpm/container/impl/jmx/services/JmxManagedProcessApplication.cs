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

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationInfoImpl = org.camunda.bpm.application.impl.ProcessApplicationInfoImpl;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using DeployedProcessArchive = org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive;
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JmxManagedProcessApplication : PlatformService<JmxManagedProcessApplication>, JmxManagedProcessApplicationMBean
	{

	  protected internal readonly ProcessApplicationInfoImpl processApplicationInfo;
	  protected internal readonly ProcessApplicationReference processApplicationReference;

	  protected internal IList<ProcessesXml> processesXmls;
	  protected internal IDictionary<string, DeployedProcessArchive> deploymentMap;

		public JmxManagedProcessApplication(ProcessApplicationInfoImpl processApplicationInfo, ProcessApplicationReference processApplicationReference)
		{
		this.processApplicationInfo = processApplicationInfo;
		this.processApplicationReference = processApplicationReference;
		}

		public virtual string ProcessApplicationName
		{
			get
			{
				return processApplicationInfo.Name;
			}
		}

		public virtual void start(PlatformServiceContainer mBeanServiceContainer)
		{
		}

		public virtual void stop(PlatformServiceContainer mBeanServiceContainer)
		{
		}

		public virtual JmxManagedProcessApplication Value
		{
			get
			{
				return this;
			}
		}

	  public virtual IList<ProcessesXml> ProcessesXmls
	  {
		  set
		  {
			this.processesXmls = value;
		  }
		  get
		  {
			return processesXmls;
		  }
	  }


	  public virtual IDictionary<string, DeployedProcessArchive> DeploymentMap
	  {
		  set
		  {
			this.deploymentMap = value;
		  }
	  }

	  public virtual IDictionary<string, DeployedProcessArchive> ProcessArchiveDeploymentMap
	  {
		  get
		  {
			return deploymentMap;
		  }
	  }

	  public virtual IList<string> DeploymentIds
	  {
		  get
		  {
			IList<string> deploymentIds = new List<string>();
			foreach (DeployedProcessArchive registration in deploymentMap.Values)
			{
			  ((IList<string>)deploymentIds).AddRange(registration.AllDeploymentIds);
			}
			return deploymentIds;
		  }
	  }

	  public virtual IList<string> DeploymentNames
	  {
		  get
		  {
			return new List<string>(deploymentMap.Keys);
		  }
	  }

	  public virtual ProcessApplicationInfoImpl ProcessApplicationInfo
	  {
		  get
		  {
			return processApplicationInfo;
		  }
	  }

	  public virtual ProcessApplicationReference ProcessApplicationReference
	  {
		  get
		  {
			return processApplicationReference;
		  }
	  }

	}

}