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
namespace org.camunda.bpm.container.impl.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESSES_XML_RESOURCES;


	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;

	/// <summary>
	/// <para>
	/// Deployment step responsible for creating individual
	/// <seealso cref="DeployProcessArchiveStep"/> instances for each process archive
	/// configured in the META-INF/processes.xml file.
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DeployProcessArchivesStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Deploy process archvies";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

		IDictionary<URL, ProcessesXml> processesXmls = operationContext.getAttachment(PROCESSES_XML_RESOURCES);

		foreach (KeyValuePair<URL, ProcessesXml> processesXml in processesXmls.SetOfKeyValuePairs())
		{
		  foreach (ProcessArchiveXml processArchive in processesXml.Value.ProcessArchives)
		  {
			// for each process archive add an individual operation step
			operationContext.addStep(createDeployProcessArchiveStep(processArchive, processesXml.Key));
		  }
		}
	  }

	  protected internal virtual DeployProcessArchiveStep createDeployProcessArchiveStep(ProcessArchiveXml parsedProcessArchive, URL url)
	  {
		return new DeployProcessArchiveStep(parsedProcessArchive, url);
	  }
	}

}