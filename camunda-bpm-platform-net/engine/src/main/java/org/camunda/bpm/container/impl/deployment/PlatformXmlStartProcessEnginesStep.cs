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
//	import static org.camunda.bpm.container.impl.deployment.Attachments.BPM_PLATFORM_XML;

	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;

	/// <summary>
	/// <para> Retrieves the List of ProcessEngines from an attached <seealso cref="ProcessesXml"/>.</para>
	/// </summary>
	/// <seealso cref= AbstractParseBpmPlatformXmlStep 
	///  </seealso>
	public class PlatformXmlStartProcessEnginesStep : AbstractStartProcessEnginesStep
	{

	  protected internal override IList<ProcessEngineXml> getProcessEnginesXmls(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml bpmPlatformXml = operationContext.getAttachment(BPM_PLATFORM_XML);
		BpmPlatformXml bpmPlatformXml = operationContext.getAttachment(BPM_PLATFORM_XML);

		return bpmPlatformXml.ProcessEngines;
	  }

	}

}