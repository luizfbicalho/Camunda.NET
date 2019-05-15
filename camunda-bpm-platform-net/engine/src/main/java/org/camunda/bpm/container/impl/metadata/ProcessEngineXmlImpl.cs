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
namespace org.camunda.bpm.container.impl.metadata
{

	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;

	/// <summary>
	/// <para>Implementation of the <seealso cref="ProcessEngineXml"/> descriptor.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessEngineXmlImpl : ProcessEngineXml
	{

	  protected internal string name;
	  protected internal bool isDefault;
	  protected internal string configurationClass;
	  protected internal string jobAcquisitionName;
	  protected internal string datasource;
	  protected internal IDictionary<string, string> properties;
	  protected internal IList<ProcessEnginePluginXml> plugins;

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual bool Default
	  {
		  get
		  {
			return isDefault;
		  }
		  set
		  {
			this.isDefault = value;
		  }
	  }


	  public virtual string ConfigurationClass
	  {
		  get
		  {
			return configurationClass;
		  }
		  set
		  {
			this.configurationClass = value;
		  }
	  }


	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }


	  public virtual string Datasource
	  {
		  get
		  {
			return datasource;
		  }
		  set
		  {
			this.datasource = value;
		  }
	  }


	  public virtual string JobAcquisitionName
	  {
		  get
		  {
			return jobAcquisitionName;
		  }
		  set
		  {
			this.jobAcquisitionName = value;
		  }
	  }


	  public virtual IList<ProcessEnginePluginXml> Plugins
	  {
		  get
		  {
			return plugins;
		  }
		  set
		  {
			this.plugins = value;
		  }
	  }


	}

}