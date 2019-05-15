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
namespace org.camunda.bpm.application.impl.metadata
{

	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;

	public class ProcessArchiveXmlImpl : ProcessArchiveXml
	{

	  private string name;
	  private string tenantId;
	  private string processEngineName;
	  private IList<string> processResourceNames;
	  private IDictionary<string, string> properties;

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


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
		  set
		  {
			this.processEngineName = value;
		  }
	  }


	  public virtual IList<string> ProcessResourceNames
	  {
		  get
		  {
			return processResourceNames;
		  }
		  set
		  {
			this.processResourceNames = value;
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


	}

}