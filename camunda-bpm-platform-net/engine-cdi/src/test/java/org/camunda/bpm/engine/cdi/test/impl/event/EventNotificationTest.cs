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
namespace org.camunda.bpm.engine.cdi.test.impl.@event
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class EventNotificationTest extends org.camunda.bpm.engine.cdi.test.CdiProcessEngineTestCase
	public class EventNotificationTest : CdiProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"}) public void testReceiveAll()
		[Deployment(resources : {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"})]
		public virtual void testReceiveAll()
		{
		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		// assert that the bean has received 0 events
		assertEquals(0, listenerBean.EventsReceived.Count);
		runtimeService.startProcessInstanceByKey("process1");

		// complete user task
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals(16, listenerBean.EventsReceived.Count);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml", "org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process2.bpmn20.xml" }) public void testSelectEventsPerProcessDefinition()
	  [Deployment(resources : { "org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml", "org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process2.bpmn20.xml" })]
	  public virtual void testSelectEventsPerProcessDefinition()
	  {
		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		assertEquals(0, listenerBean.EventsReceivedByKey.Count);
		//start the 2 processes
		runtimeService.startProcessInstanceByKey("process1");
		runtimeService.startProcessInstanceByKey("process2");

		// assert that now the bean has received 11 events
		assertEquals(11, listenerBean.EventsReceivedByKey.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"}) public void testSelectEventsPerActivity()
	  [Deployment(resources : {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"})]
	  public virtual void testSelectEventsPerActivity()
	  {
		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		assertEquals(0, listenerBean.EndActivityService1);
		assertEquals(0, listenerBean.StartActivityService1);
		assertEquals(0, listenerBean.TakeTransition1);

		// start the process
		runtimeService.startProcessInstanceByKey("process1");

		// assert
		assertEquals(1, listenerBean.EndActivityService1);
		assertEquals(1, listenerBean.StartActivityService1);
		assertEquals(1, listenerBean.TakeTransition1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"}) public void testSelectEventsPerTask()
	  [Deployment(resources : {"org/camunda/bpm/engine/cdi/test/impl/event/EventNotificationTest.process1.bpmn20.xml"})]
	  public virtual void testSelectEventsPerTask()
	  {
		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		assertEquals(0, listenerBean.CreateTaskUser1);
		assertEquals(0, listenerBean.AssignTaskUser1);
		assertEquals(0, listenerBean.CompleteTaskUser1);
		assertEquals(0, listenerBean.DeleteTaskUser1);

		// assert that the bean has received 0 events
		assertEquals(0, listenerBean.EventsReceived.Count);
		runtimeService.startProcessInstanceByKey("process1");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.setAssignee(task.Id, "demo");

		taskService.complete(task.Id);

		assertEquals(1, listenerBean.CreateTaskUser1);
		assertEquals(1, listenerBean.AssignTaskUser1);
		assertEquals(1, listenerBean.CompleteTaskUser1);
		assertEquals(0, listenerBean.DeleteTaskUser1);

		listenerBean.reset();
		assertEquals(0, listenerBean.DeleteTaskUser1);

		// assert that the bean has received 0 events
		assertEquals(0, listenerBean.EventsReceived.Count);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process1");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		assertEquals(1, listenerBean.DeleteTaskUser1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testMultiInstanceEvents()
	  public virtual void testMultiInstanceEvents()
	  {
		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		assertThat(listenerBean.EventsReceived.Count, @is(0));
		runtimeService.startProcessInstanceByKey("process1");
		waitForJobExecutorToProcessAllJobs(TimeUnit.SECONDS.toMillis(5L), 500L);

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));

		// 2: start event (start + end)
		// 1: transition to first mi activity
		// 2: first mi body (start + end)
		// 4: two instances of the inner activity (start + end)
		// 1: transition to second mi activity
		// 2: second mi body (start + end)
		// 4: two instances of the inner activity (start + end)
		// 1: transition to the user task
		// 2: user task (start + task create event)
		// = 19
		assertThat(listenerBean.EventsReceived.Count, @is(19));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testMultiInstanceEventsAfterExternalTrigger()
	  public virtual void testMultiInstanceEventsAfterExternalTrigger()
	  {

		runtimeService.startProcessInstanceByKey("process");

		TestEventListener listenerBean = getBeanInstance(typeof(TestEventListener));
		listenerBean.reset();

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		// 6: three user task instances (complete + end)
		// 1: one mi body instance (end)
		// 1: one sequence flow instance (take)
		// 2: one end event instance (start + end)
		// = 5
		ISet<BusinessProcessEvent> eventsReceived = listenerBean.EventsReceived;
		assertThat(eventsReceived.Count, @is(10));
	  }

	}

}