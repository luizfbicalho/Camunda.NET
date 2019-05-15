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
namespace org.camunda.bpm.engine.test.bpmn.servicetask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using ServiceTask = org.camunda.bpm.model.bpmn.instance.ServiceTask;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ServiceTaskBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
	{

	  private const string PROCESS_ID = "process";
	  private new string deploymentId;

	  public virtual void testJavaDelegateModelExecutionContext()
	  {
		deploy();

		runtimeService.startProcessInstanceByKey(PROCESS_ID);

		BpmnModelInstance modelInstance = ModelExecutionContextServiceTask.modelInstance;
		assertNotNull(modelInstance);

		Model model = modelInstance.Model;
		ICollection<ModelElementInstance> events = modelInstance.getModelElementsByType(model.getType(typeof(Event)));
		assertEquals(2, events.Count);
		ICollection<ModelElementInstance> tasks = modelInstance.getModelElementsByType(model.getType(typeof(Task)));
		assertEquals(1, tasks.Count);

		Process process = (Process) modelInstance.Definitions.RootElements.GetEnumerator().next();
		assertEquals(PROCESS_ID, process.Id);
		assertTrue(process.Executable);

		ServiceTask serviceTask = ModelExecutionContextServiceTask.serviceTask;
		assertNotNull(serviceTask);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(ModelExecutionContextServiceTask).FullName, serviceTask.CamundaClass);
	  }

	  private void deploy()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(typeof(ModelExecutionContextServiceTask).FullName).endEvent().done();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;
	  }

	  public virtual void tearDown()
	  {
		ModelExecutionContextServiceTask.clear();
		repositoryService.deleteDeployment(deploymentId, true);
	  }

	}

}