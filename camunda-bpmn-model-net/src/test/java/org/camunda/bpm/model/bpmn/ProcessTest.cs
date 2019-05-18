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
namespace org.camunda.bpm.model.bpmn
{
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using BpmnModelResource = org.camunda.bpm.model.bpmn.util.BpmnModelResource;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessTest : BpmnModelTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @BpmnModelResource public void shouldImportProcess()
	  public virtual void shouldImportProcess()
	  {

		ModelElementInstance modelElementById = bpmnModelInstance.getModelElementById("exampleProcessId");
		assertThat(modelElementById).NotNull;

		ICollection<RootElement> rootElements = bpmnModelInstance.Definitions.RootElements;
		assertThat(rootElements).hasSize(1);
		Process process = (Process) rootElements.GetEnumerator().next();

		assertThat(process.Id).isEqualTo("exampleProcessId");
		assertThat(process.Name).Null;
		assertThat(process.ProcessType).isEqualTo(ProcessType.None);
		assertThat(process.Executable).False;
		assertThat(process.Closed).False;



	  }


	}

}