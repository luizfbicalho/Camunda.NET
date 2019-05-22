using System;

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
	using org.camunda.bpm.model.bpmn.instance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CreateModelTest
	{

	  public BpmnModelInstance modelInstance;
	  public Definitions definitions;
	  public Process process;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createEmptyModel()
	  public virtual void createEmptyModel()
	  {
		modelInstance = Bpmn.createEmptyModel();
		definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "http://camunda.org/examples";
		modelInstance.Definitions = definitions;
	  }

	  protected internal virtual T createElement<T>(BpmnModelElementInstance parentElement, string id, Type elementClass) where T : BpmnModelElementInstance
	  {
			  elementClass = typeof(T);
		T element = modelInstance.newInstance(elementClass);
		element.setAttributeValue("id", id, true);
		parentElement.addChildElement(element);
		return element;
	  }

	  public virtual SequenceFlow createSequenceFlow(Process process, FlowNode from, FlowNode to)
	  {
		SequenceFlow sequenceFlow = createElement(process, from.Id + "-" + to.Id, typeof(SequenceFlow));
		process.addChildElement(sequenceFlow);
		sequenceFlow.Source = from;
		from.Outgoing.Add(sequenceFlow);
		sequenceFlow.Target = to;
		to.Incoming.Add(sequenceFlow);
		return sequenceFlow;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createProcessWithOneTask()
	  public virtual void createProcessWithOneTask()
	  {
		// create process
		Process process = createElement(definitions, "process-with-one-task", typeof(Process));

		// create elements
		StartEvent startEvent = createElement(process, "start", typeof(StartEvent));
		UserTask task1 = createElement(process, "task1", typeof(UserTask));
		EndEvent endEvent = createElement(process, "end", typeof(EndEvent));

		// create flows
		createSequenceFlow(process, startEvent, task1);
		createSequenceFlow(process, task1, endEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createProcessWithParallelGateway()
	  public virtual void createProcessWithParallelGateway()
	  {
		// create process
		Process process = createElement(definitions, "process-with-parallel-gateway", typeof(Process));

		// create elements
		StartEvent startEvent = createElement(process, "start", typeof(StartEvent));
		ParallelGateway fork = createElement(process, "fork", typeof(ParallelGateway));
		UserTask task1 = createElement(process, "task1", typeof(UserTask));
		ServiceTask task2 = createElement(process, "task2", typeof(ServiceTask));
		ParallelGateway join = createElement(process, "join", typeof(ParallelGateway));
		EndEvent endEvent = createElement(process, "end", typeof(EndEvent));

		// create flows
		createSequenceFlow(process, startEvent, fork);
		createSequenceFlow(process, fork, task1);
		createSequenceFlow(process, fork, task2);
		createSequenceFlow(process, task1, join);
		createSequenceFlow(process, task2, join);
		createSequenceFlow(process, join, endEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel()
	  public virtual void validateModel()
	  {
		Bpmn.validateModel(modelInstance);
	  }

	}

}