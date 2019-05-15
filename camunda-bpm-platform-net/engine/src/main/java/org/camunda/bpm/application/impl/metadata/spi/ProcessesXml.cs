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
namespace org.camunda.bpm.application.impl.metadata.spi
{

	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;

	/// <summary>
	/// <para>Java API representation of the <seealso cref="ProcessesXml"/> Metadata.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessesXml
	{

	  /// <returns> A <seealso cref="List"/> of <seealso cref="ProcessEngineXml"/> Metadata Items representing process engine configurations. </returns>
	  IList<ProcessEngineXml> ProcessEngines {get;}

	  /// <returns> A <seealso cref="List"/> of <seealso cref="ProcessArchiveXml"/> Metadata Items representing process archive deployments. </returns>
	  IList<ProcessArchiveXml> ProcessArchives {get;}

	  /// <summary>
	  /// <para>Constant representing the empty processes.xml</para>
	  /// </summary>
	//JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
	//  public final static ProcessesXml EMPTY_PROCESSES_XML = new ProcessesXml()
	//  {
	//
	//	public List<ProcessEngineXml> getProcessEngines()
	//	{
	//	  return Collections.emptyList();
	//	}
	//
	//	public List<ProcessArchiveXml> getProcessArchives()
	//	{
	//	  List<ProcessArchiveXml> processArchives = new ArrayList<ProcessArchiveXml>();
	//
	//	  // add single PA
	//	  ProcessArchiveXmlImpl pa = new ProcessArchiveXmlImpl();
	//	  processArchives.add(pa);
	//
	//	  pa.setProcessResourceNames(Collections.emptyList<String>());
	//
	//	  // with default properties
	//	  HashMap<String, String> properties = new HashMap<String, String>();
	//	  pa.setProperties(properties);
	//	  properties.put(ProcessArchiveXml.PROP_IS_DELETE_UPON_UNDEPLOY, false.toString());
	//	  properties.put(ProcessArchiveXml.PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS, true.toString());
	//	  properties.put(ProcessArchiveXml.PROP_IS_DEPLOY_CHANGED_ONLY, false.toString());
	//	  properties.put(ProcessArchiveXml.PROP_RESUME_PREVIOUS_BY, ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY);
	//
	//	  return processArchives;
	//	}
	//
	//  };

	}

}