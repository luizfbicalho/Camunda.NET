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
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessesXmlImpl : ProcessesXml
	{

	  private IList<ProcessEngineXml> processEngineXmls;
	  private IList<ProcessArchiveXml> processArchiveXmls;

	  public ProcessesXmlImpl(IList<ProcessEngineXml> processEngineXmls, IList<ProcessArchiveXml> processArchiveXmls)
	  {
		this.processEngineXmls = processEngineXmls;
		this.processArchiveXmls = processArchiveXmls;
	  }

	  public virtual IList<ProcessEngineXml> ProcessEngines
	  {
		  get
		  {
			return processEngineXmls;
		  }
	  }

	  public virtual IList<ProcessArchiveXml> ProcessArchives
	  {
		  get
		  {
			return processArchiveXmls;
		  }
	  }

	}

}