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
namespace org.camunda.bpm.engine.test.bpmn.async
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;

	public class AsyncStartEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncStartEvent()
	  public virtual void testAsyncStartEvent()
	  {
		runtimeService.startProcessInstanceByKey("asyncStartEvent");

		Task task = taskService.createTaskQuery().singleResult();
		Assert.assertNull("The user task should not have been reached yet", task);

		Assert.assertEquals(1, runtimeService.createExecutionQuery().activityId("startEvent").count());

		executeAvailableJobs();
		task = taskService.createTaskQuery().singleResult();

		Assert.assertEquals(0, runtimeService.createExecutionQuery().activityId("startEvent").count());

		Assert.assertNotNull("The user task should have been reached", task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncStartEventListeners()
	  public virtual void testAsyncStartEventListeners()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("asyncStartEvent");

		Assert.assertNull(runtimeService.getVariable(instance.Id, "listener"));

		executeAvailableJobs();

		Assert.assertNotNull(runtimeService.getVariable(instance.Id, "listener"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncStartEvent.bpmn20.xml")]
	  public virtual void testAsyncStartEventActivityInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncStartEvent");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("startEvent").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleAsyncStartEvents()
	  public virtual void testMultipleAsyncStartEvents()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		runtimeService.correlateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		executeAvailableJobs();

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		// assert process instance is ended
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testCallActivity-super.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testCallActivity-sub.bpmn20.xml" })]
	  public virtual void testCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("super");

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().processDefinitionKey("sub").singleResult();

		assertTrue(pi is ExecutionEntity);

		assertEquals("theSubStart", ((ExecutionEntity)pi).ActivityId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncSubProcessStartEvent()
	  public virtual void testAsyncSubProcessStartEvent()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		assertNull("The subprocess user task should not have been reached yet", task);

		assertEquals(1, runtimeService.createExecutionQuery().activityId("StartEvent_2").count());

		executeAvailableJobs();
		task = taskService.createTaskQuery().singleResult();

		assertEquals(0, runtimeService.createExecutionQuery().activityId("StartEvent_2").count());
		assertNotNull("The subprocess user task should have been reached", task);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncStartEventTest.testAsyncSubProcessStartEvent.bpmn")]
	  public virtual void testAsyncSubProcessStartEventActivityInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("SubProcess_1").transition("StartEvent_2").done());
	  }
	}

}