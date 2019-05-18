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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	using Definitions = org.camunda.bpm.model.bpmn.instance.Definitions;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using Test = org.junit.Test;

	public class GenerateIdTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotGenerateIdsOnRead()
	  public virtual void shouldNotGenerateIdsOnRead()
	  {
		BpmnModelInstance modelInstance = Bpmn.readModelFromStream(typeof(GenerateIdTest).getResourceAsStream("GenerateIdTest.bpmn"));
		Definitions definitions = modelInstance.Definitions;
		assertThat(definitions.Id).Null;

		Process process = modelInstance.getModelElementsByType(typeof(Process)).GetEnumerator().next();
		assertThat(process.Id).Null;

		StartEvent startEvent = modelInstance.getModelElementsByType(typeof(StartEvent)).GetEnumerator().next();
		assertThat(startEvent.Id).Null;

		UserTask userTask = modelInstance.getModelElementsByType(typeof(UserTask)).GetEnumerator().next();
		assertThat(userTask.Id).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateIdsOnCreate()
	  public virtual void shouldGenerateIdsOnCreate()
	  {
		BpmnModelInstance modelInstance = Bpmn.createEmptyModel();
		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		assertThat(definitions.Id).NotNull;

		Process process = modelInstance.newInstance(typeof(Process));
		assertThat(process.Id).NotNull;

		StartEvent startEvent = modelInstance.newInstance(typeof(StartEvent));
		assertThat(startEvent.Id).NotNull;

		UserTask userTask = modelInstance.newInstance(typeof(UserTask));
		assertThat(userTask.Id).NotNull;
	  }

	}

}