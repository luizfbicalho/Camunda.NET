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

	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;

	/// <summary>
	/// <para>Implementation of the <seealso cref="JobExecutorXml"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobExecutorXmlImpl : JobExecutorXml
	{

	  protected internal IList<JobAcquisitionXml> jobAcquisitions;
	  protected internal string jobExecutorClass;
	  protected internal IDictionary<string, string> properties;

	  public virtual IList<JobAcquisitionXml> JobAcquisitions
	  {
		  get
		  {
			return jobAcquisitions;
		  }
		  set
		  {
			this.jobAcquisitions = value;
		  }
	  }


	  public virtual string JobExecutorClass
	  {
		  get
		  {
			return jobExecutorClass;
		  }
		  set
		  {
			this.jobExecutorClass = value;
		  }
	  }


	  public virtual IDictionary<string, string> Properties
	  {
		  set
		  {
			this.properties = value;
		  }
		  get
		  {
			return properties;
		  }
	  }


	}

}