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
namespace org.camunda.bpm.application.impl.@event
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationEventListenerTest : ResourceProcessEngineTestCase
	{

	  public ProcessApplicationEventListenerTest() : base("org/camunda/bpm/application/impl/event/pa.event.listener.camunda.cfg.xml")
	  {
		// ProcessApplicationEventListenerPlugin is activated in configuration
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();
	  }

	  protected internal override void closeDownProcessEngine()
	  {
		managementService.unregisterProcessApplication(deploymentId, false);
		base.closeDownProcessEngine();
	  }

	  [Deployment(resources : { "org/camunda/bpm/application/impl/event/ProcessApplicationEventListenerTest.testExecutionListener.bpmn20.xml" })]
	  public virtual void testExecutionListenerNull()
	  {

		// this test verifies that the process application can return a 'null'
		// execution listener
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		// register app so that it receives events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);
		// I can start a process event though the process app does not provide an
		// event listener.
		runtimeService.startProcessInstanceByKey("startToEnd");

	  }

	  [Deployment(resources : { "org/camunda/bpm/application/impl/event/ProcessApplicationEventListenerTest.testExecutionListener.bpmn20.xml" })]
	  public virtual void testShouldInvokeExecutionListenerOnStartAndEndOfProcessInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger processDefinitionEventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger processDefinitionEventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass(this, processDefinitionEventCount);

		// register app so that it receives events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// Start process instance.
		runtimeService.startProcessInstanceByKey("startToEnd");

		// Start and end of the process 
		assertEquals(2, processDefinitionEventCount.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger processDefinitionEventCount;

		  public EmbeddedProcessApplicationAnonymousInnerClass(ProcessApplicationEventListenerTest outerInstance, AtomicInteger processDefinitionEventCount)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionEventCount = processDefinitionEventCount;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				// this process application returns an execution listener
				return new ExecutionListenerAnonymousInnerClass(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass outerInstance;

			  public ExecutionListenerAnonymousInnerClass(EmbeddedProcessApplicationAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				if (((CoreExecution) execution).EventSource is ProcessDefinitionEntity)
				{
				  outerInstance.processDefinitionEventCount.incrementAndGet();
				}
			  }
		  }
	  }

	  [Deployment(resources : { "org/camunda/bpm/application/impl/event/ProcessApplicationEventListenerTest.testExecutionListener.bpmn20.xml" })]
	  public virtual void testShouldNotIncrementExecutionListenerCountOnStartAndEndOfProcessInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass2(this, eventCount);

		// register app so that it receives events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// Start process instance.
		runtimeService.startProcessInstanceByKey("startToEnd");

		assertEquals(5, eventCount.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass2 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger eventCount;

		  public EmbeddedProcessApplicationAnonymousInnerClass2(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCount)
		  {
			  this.outerInstance = outerInstance;
			  this.eventCount = eventCount;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				// this process application returns an execution listener
				return new ExecutionListenerAnonymousInnerClass2(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass2 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass2 outerInstance;

			  public ExecutionListenerAnonymousInnerClass2(EmbeddedProcessApplicationAnonymousInnerClass2 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				if (!(((CoreExecution) execution).EventSource is ProcessDefinitionEntity))
				{
				  outerInstance.eventCount.incrementAndGet();
				}
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListener()
	  public virtual void testExecutionListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass3(this, eventCount);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// start process instance
		runtimeService.startProcessInstanceByKey("startToEnd");

		// 7 events received
		assertEquals(7, eventCount.get());
	  }

	   private class EmbeddedProcessApplicationAnonymousInnerClass3 : EmbeddedProcessApplication
	   {
		   private readonly ProcessApplicationEventListenerTest outerInstance;

		   private AtomicInteger eventCount;

		   public EmbeddedProcessApplicationAnonymousInnerClass3(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCount)
		   {
			   this.outerInstance = outerInstance;
			   this.eventCount = eventCount;
		   }

		   public override ExecutionListener ExecutionListener
		   {
			   get
			   {
				 // this process application returns an execution listener
				 return new ExecutionListenerAnonymousInnerClass3(this);
			   }
		   }

		   private class ExecutionListenerAnonymousInnerClass3 : ExecutionListener
		   {
			   private readonly EmbeddedProcessApplicationAnonymousInnerClass3 outerInstance;

			   public ExecutionListenerAnonymousInnerClass3(EmbeddedProcessApplicationAnonymousInnerClass3 outerInstance)
			   {
				   this.outerInstance = outerInstance;
			   }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			   public void notify(DelegateExecution execution)
			   {
				   outerInstance.eventCount.incrementAndGet();
			   }
		   }
	   }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerWithErrorBoundaryEvent()
	  public virtual void testExecutionListenerWithErrorBoundaryEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass4(this, eventCount);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// 1. (start)startEvent(end) -(take)-> (start)serviceTask(end) -(take)-> (start)endEvent(end) (8 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		assertEquals(10, eventCount.get());

		// reset counter
		eventCount.set(0);

		// 2. (start)startEvent(end) -(take)-> (start)serviceTask(end)/(start)errorBoundaryEvent(end) -(take)-> (start)endEvent(end) (10 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener", Collections.singletonMap<string, object>("shouldThrowError", true));

		assertEquals(12, eventCount.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass4 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger eventCount;

		  public EmbeddedProcessApplicationAnonymousInnerClass4(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCount)
		  {
			  this.outerInstance = outerInstance;
			  this.eventCount = eventCount;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass4(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass4 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass4 outerInstance;

			  public ExecutionListenerAnonymousInnerClass4(EmbeddedProcessApplicationAnonymousInnerClass4 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				  outerInstance.eventCount.incrementAndGet();
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerWithTimerBoundaryEvent()
	  public virtual void testExecutionListenerWithTimerBoundaryEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass5(this, eventCount);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// 1. (start)startEvent(end) -(take)-> (start)userTask(end) -(take)-> (start)endEvent(end) (8 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		// complete task
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals(10, eventCount.get());

		// reset counter
		eventCount.set(0);

		// 2. (start)startEvent(end) -(take)-> (start)userTask(end)/(start)timerBoundaryEvent(end) -(take)-> (start)endEvent(end) (10 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		// fire timer event
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		assertEquals(12, eventCount.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass5 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger eventCount;

		  public EmbeddedProcessApplicationAnonymousInnerClass5(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCount)
		  {
			  this.outerInstance = outerInstance;
			  this.eventCount = eventCount;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass5(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass5 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass5 outerInstance;

			  public ExecutionListenerAnonymousInnerClass5(EmbeddedProcessApplicationAnonymousInnerClass5 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				  outerInstance.eventCount.incrementAndGet();
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerWithSignalBoundaryEvent()
	  public virtual void testExecutionListenerWithSignalBoundaryEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCount = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCount = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass6(this, eventCount);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// 1. (start)startEvent(end) -(take)-> (start)userTask(end) -(take)-> (start)endEvent(end) (8 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		// complete task
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals(10, eventCount.get());

		// reset counter
		eventCount.set(0);

		// 2. (start)startEvent(end) -(take)-> (start)userTask(end)/(start)signalBoundaryEvent(end) -(take)-> (start)endEvent(end) (10 Events)

		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		// signal event
		runtimeService.signalEventReceived("signal");

		assertEquals(12, eventCount.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass6 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger eventCount;

		  public EmbeddedProcessApplicationAnonymousInnerClass6(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCount)
		  {
			  this.outerInstance = outerInstance;
			  this.eventCount = eventCount;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass6(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass6 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass6 outerInstance;

			  public ExecutionListenerAnonymousInnerClass6(EmbeddedProcessApplicationAnonymousInnerClass6 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				  outerInstance.eventCount.incrementAndGet();
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerWithMultiInstanceBody()
	  public virtual void testExecutionListenerWithMultiInstanceBody()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger eventCountForMultiInstanceBody = new java.util.concurrent.atomic.AtomicInteger();
		AtomicInteger eventCountForMultiInstanceBody = new AtomicInteger();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass7(this, eventCountForMultiInstanceBody);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);


		// start process instance
		runtimeService.startProcessInstanceByKey("executionListener");

		// complete task
		IList<Task> miTasks = taskService.createTaskQuery().list();
		foreach (Task task in miTasks)
		{
		  taskService.complete(task.Id);
		}

		// 2 events are expected: one for mi body start; one for mi body end
		assertEquals(2, eventCountForMultiInstanceBody.get());
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass7 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private AtomicInteger eventCountForMultiInstanceBody;

		  public EmbeddedProcessApplicationAnonymousInnerClass7(ProcessApplicationEventListenerTest outerInstance, AtomicInteger eventCountForMultiInstanceBody)
		  {
			  this.outerInstance = outerInstance;
			  this.eventCountForMultiInstanceBody = eventCountForMultiInstanceBody;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass7(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass7 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass7 outerInstance;

			  public ExecutionListenerAnonymousInnerClass7(EmbeddedProcessApplicationAnonymousInnerClass7 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
			  public void notify(DelegateExecution execution)
			  {
				if ("miTasks#multiInstanceBody".Equals(execution.CurrentActivityId) && (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START.Equals(execution.EventName) || org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END.Equals(execution.EventName)))
				{
				  outerInstance.eventCountForMultiInstanceBody.incrementAndGet();
				}
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListener()
	  public virtual void testTaskListener()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> events = new java.util.ArrayList<String>();
		IList<string> events = new List<string>();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass8(this, events);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// start process instance
		ProcessInstance taskListenerProcess = runtimeService.startProcessInstanceByKey("taskListenerProcess");

		// create event received
		assertEquals(1, events.Count);
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, events[0]);

		Task task = taskService.createTaskQuery().singleResult();
		//assign task:
		taskService.setAssignee(task.Id, "jonny");
		assertEquals(2, events.Count);
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, events[1]);

		// complete task
		taskService.complete(task.Id);
		assertEquals(4, events.Count);
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE, events[2]);
		// next task was created
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, events[3]);

		// delete process instance so last task will be deleted
		runtimeService.deleteProcessInstance(taskListenerProcess.ProcessInstanceId, "test delete event");
		assertEquals(5, events.Count);
		assertEquals(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, events[4]);

	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass8 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private IList<string> events;

		  public EmbeddedProcessApplicationAnonymousInnerClass8(ProcessApplicationEventListenerTest outerInstance, IList<string> events)
		  {
			  this.outerInstance = outerInstance;
			  this.events = events;
		  }

		  public override TaskListener TaskListener
		  {
			  get
			  {
				return new TaskListenerAnonymousInnerClass(this);
			  }
		  }

		  private class TaskListenerAnonymousInnerClass : TaskListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass8 outerInstance;

			  public TaskListenerAnonymousInnerClass(EmbeddedProcessApplicationAnonymousInnerClass8 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

			  public void notify(DelegateTask delegateTask)
			  {
				outerInstance.events.Add(delegateTask.EventName);
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIntermediateTimerEvent()
	  public virtual void testIntermediateTimerEvent()
	  {

		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> timerEvents = new java.util.ArrayList<String>();
		IList<string> timerEvents = new List<string>();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass9(this, timerEvents);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// when
		runtimeService.startProcessInstanceByKey("process");
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.executeJob(jobId);

		// then
		assertEquals(2, timerEvents.Count);

		// "start" event listener
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, timerEvents[0]);

		// "end" event listener
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, timerEvents[1]);
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass9 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private IList<string> timerEvents;

		  public EmbeddedProcessApplicationAnonymousInnerClass9(ProcessApplicationEventListenerTest outerInstance, IList<string> timerEvents)
		  {
			  this.outerInstance = outerInstance;
			  this.timerEvents = timerEvents;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass8(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass8 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass9 outerInstance;

			  public ExecutionListenerAnonymousInnerClass8(EmbeddedProcessApplicationAnonymousInnerClass9 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

			  public void notify(DelegateExecution delegateExecution)
			  {
				string currentActivityId = delegateExecution.CurrentActivityId;
				string eventName = delegateExecution.EventName;
				if ("timer".Equals(currentActivityId) && (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START.Equals(eventName) || org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END.Equals(eventName)))
				{
				  outerInstance.timerEvents.Add(delegateExecution.EventName);
				}
			  }
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIntermediateSignalEvent()
	  public virtual void testIntermediateSignalEvent()
	  {

		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> timerEvents = new java.util.ArrayList<String>();
		IList<string> timerEvents = new List<string>();

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplicationAnonymousInnerClass10(this, timerEvents);

		// register app so that it is notified about events
		managementService.registerProcessApplication(deploymentId, processApplication.Reference);

		// when
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.signalEventReceived("abort");

		// then
		assertEquals(2, timerEvents.Count);

		// "start" event listener
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, timerEvents[0]);

		// "end" event listener
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, timerEvents[1]);
	  }

	  private class EmbeddedProcessApplicationAnonymousInnerClass10 : EmbeddedProcessApplication
	  {
		  private readonly ProcessApplicationEventListenerTest outerInstance;

		  private IList<string> timerEvents;

		  public EmbeddedProcessApplicationAnonymousInnerClass10(ProcessApplicationEventListenerTest outerInstance, IList<string> timerEvents)
		  {
			  this.outerInstance = outerInstance;
			  this.timerEvents = timerEvents;
		  }

		  public override ExecutionListener ExecutionListener
		  {
			  get
			  {
				return new ExecutionListenerAnonymousInnerClass9(this);
			  }
		  }

		  private class ExecutionListenerAnonymousInnerClass9 : ExecutionListener
		  {
			  private readonly EmbeddedProcessApplicationAnonymousInnerClass10 outerInstance;

			  public ExecutionListenerAnonymousInnerClass9(EmbeddedProcessApplicationAnonymousInnerClass10 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

			  public void notify(DelegateExecution delegateExecution)
			  {
				string currentActivityId = delegateExecution.CurrentActivityId;
				string eventName = delegateExecution.EventName;
				if ("signal".Equals(currentActivityId) && (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START.Equals(eventName) || org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END.Equals(eventName)))
				{
				  outerInstance.timerEvents.Add(delegateExecution.EventName);
				}
			  }
		  }
	  }

	}

}