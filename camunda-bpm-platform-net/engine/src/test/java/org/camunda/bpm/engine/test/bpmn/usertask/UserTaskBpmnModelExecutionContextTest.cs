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
namespace org.camunda.bpm.engine.test.bpmn.usertask
{
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using org.camunda.bpm.model.bpmn.instance;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UserTaskBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
	{

	  private const string PROCESS_ID = "process";
	  private const string USER_TASK_ID = "userTask";
	  private new string deploymentId;

	  public virtual void testGetBpmnModelElementInstanceOnCreate()
	  {
		string eventName = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		deployProcess(eventName);

		runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertModelInstance();
		assertUserTask(eventName);

		string taskId = taskService.createTaskQuery().active().singleResult().Id;
		taskService.complete(taskId);
	  }

	  public virtual void testGetBpmnModelElementInstanceOnAssignment()
	  {
		string eventName = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT;
		deployProcess(eventName);

		runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertNull(ModelExecutionContextTaskListener.modelInstance);
		assertNull(ModelExecutionContextTaskListener.userTask);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setAssignee(taskId, "demo");

		assertModelInstance();
		assertUserTask(eventName);

		taskService.complete(taskId);
	  }

	  public virtual void testGetBpmnModelElementInstanceOnComplete()
	  {
		string eventName = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		deployProcess(eventName);

		runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertNull(ModelExecutionContextTaskListener.modelInstance);
		assertNull(ModelExecutionContextTaskListener.userTask);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setAssignee(taskId, "demo");

		assertNull(ModelExecutionContextTaskListener.modelInstance);
		assertNull(ModelExecutionContextTaskListener.userTask);

		taskService.complete(taskId);

		assertModelInstance();
		assertUserTask(eventName);
	  }

	  private void assertModelInstance()
	  {
		BpmnModelInstance modelInstance = ModelExecutionContextTaskListener.modelInstance;
		assertNotNull(modelInstance);

		ICollection<ModelElementInstance> events = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(Event)));
		assertEquals(2, events.Count);

		ICollection<ModelElementInstance> tasks = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(Task)));
		assertEquals(1, tasks.Count);

		Process process = (Process) modelInstance.Definitions.RootElements.GetEnumerator().next();
		assertEquals(PROCESS_ID, process.Id);
		assertTrue(process.Executable);
	  }

	  private void assertUserTask(string eventName)
	  {
		UserTask userTask = ModelExecutionContextTaskListener.userTask;
		assertNotNull(userTask);

		ModelElementInstance taskListener = userTask.ExtensionElements.getUniqueChildElementByNameNs(CAMUNDA_NS, "taskListener");
		assertEquals(eventName, taskListener.getAttributeValueNs(CAMUNDA_NS, "event"));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(ModelExecutionContextTaskListener).FullName, taskListener.getAttributeValueNs(CAMUNDA_NS, "class"));

		BpmnModelInstance modelInstance = ModelExecutionContextTaskListener.modelInstance;
		ICollection<ModelElementInstance> tasks = modelInstance.getModelElementsByType(modelInstance.Model.getType(typeof(Task)));
		assertTrue(tasks.Contains(userTask));
	  }

	  private void deployProcess(string eventName)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().userTask(USER_TASK_ID).endEvent().done();

		ExtensionElements extensionElements = modelInstance.newInstance(typeof(ExtensionElements));
		ModelElementInstance taskListener = extensionElements.addExtensionElement(CAMUNDA_NS, "taskListener");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		taskListener.setAttributeValueNs(CAMUNDA_NS, "class", typeof(ModelExecutionContextTaskListener).FullName);
		taskListener.setAttributeValueNs(CAMUNDA_NS, "event", eventName);

		UserTask userTask = modelInstance.getModelElementById(USER_TASK_ID);
		userTask.ExtensionElements = extensionElements;

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;
	  }

	  public virtual void tearDown()
	  {
		ModelExecutionContextTaskListener.clear();
		repositoryService.deleteDeployment(deploymentId, true);
	  }
	}

}