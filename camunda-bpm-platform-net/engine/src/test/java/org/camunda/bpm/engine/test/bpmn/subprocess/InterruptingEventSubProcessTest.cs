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
namespace org.camunda.bpm.engine.test.bpmn.subprocess
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// </summary>
	public class InterruptingEventSubProcessTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:"org/camunda/bpm/engine/test/bpmn/subprocess/InterruptingEventSubProcessTest.testCancelEventSubscriptions.bpmn")]
	  public virtual void testCancelEventSubscriptionsWhenReceivingAMessage()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		TaskQuery taskQuery = taskService.createTaskQuery();
		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery();

		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<EventSubscription> eventSubscriptions = eventSubscriptionQuery.list();
		assertEquals(2, eventSubscriptions.Count);

		runtimeService.messageEventReceived("newMessage", pi.Id);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		assertEquals(0, eventSubscriptionQuery.count());

		try
		{
		  runtimeService.signalEventReceived("newSignal", pi.Id);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // expected exception;
		}

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/bpmn/subprocess/InterruptingEventSubProcessTest.testCancelEventSubscriptions.bpmn")]
	  public virtual void testCancelEventSubscriptionsWhenReceivingASignal()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		TaskQuery taskQuery = taskService.createTaskQuery();
		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery();

		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<EventSubscription> eventSubscriptions = eventSubscriptionQuery.list();
		assertEquals(2, eventSubscriptions.Count);

		runtimeService.signalEventReceived("newSignal", pi.Id);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("tastAfterSignalStartEvent", task.TaskDefinitionKey);

		assertEquals(0, eventSubscriptionQuery.count());

		try
		{
		  runtimeService.messageEventReceived("newMessage", pi.Id);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // expected exception;
		}

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelTimer()
	  public virtual void testCancelTimer()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		TaskQuery taskQuery = taskService.createTaskQuery();
		JobQuery jobQuery = managementService.createJobQuery().timers();

		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		Job timer = jobQuery.singleResult();
		assertNotNull(timer);

		runtimeService.messageEventReceived("newMessage", pi.Id);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		assertEquals(0, jobQuery.count());

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testKeepCompensation()
	  public virtual void testKeepCompensation()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		TaskQuery taskQuery = taskService.createTaskQuery();
		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery();

		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<EventSubscription> eventSubscriptions = eventSubscriptionQuery.list();
		assertEquals(2, eventSubscriptions.Count);

		runtimeService.messageEventReceived("newMessage", pi.Id);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		assertEquals(1, eventSubscriptionQuery.count());

		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimeCycle()
	  public virtual void testTimeCycle()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery();
		assertEquals(0, eventSubscriptionQuery.count());

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());
		Task task = taskQuery.singleResult();
		assertEquals("task", task.TaskDefinitionKey);

		JobQuery jobQuery = managementService.createJobQuery().timers();
		assertEquals(1, jobQuery.count());

		string jobId = jobQuery.singleResult().Id;
		managementService.executeJob(jobId);

		assertEquals(0, jobQuery.count());

		assertEquals(1, taskQuery.count());
		task = taskQuery.singleResult();
		assertEquals("eventSubProcessTask", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		assertProcessEnded(processInstanceId);
	  }

	}

}