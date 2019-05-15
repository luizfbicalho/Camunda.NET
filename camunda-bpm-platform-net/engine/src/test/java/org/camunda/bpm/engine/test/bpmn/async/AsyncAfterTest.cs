using System;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsCollectionWithSize.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ThrowBpmnErrorDelegate = org.camunda.bpm.engine.test.bpmn.@event.error.ThrowBpmnErrorDelegate;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Stefan Hentschel
	/// 
	/// </summary>
	public class AsyncAfterTest : PluggableProcessEngineTestCase
	{

	  public virtual void testTransitionIdRequired()
	  {

		// if an outgoing sequence flow has no id, we cannot use it in asyncAfter
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/async/AsyncAfterTest.testTransitionIdRequired.bpmn20.xml").deploy();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Sequence flow with sourceRef='service' must have an id, activity with id 'service' uses 'asyncAfter'.", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterServiceTask()
	  public virtual void testAsyncAfterServiceTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// listeners should be fired by now
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the catch event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the waiting job is executed, the process instance should end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterMultiInstanceUserTask()
	  public virtual void testAsyncAfterMultiInstanceUserTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<Task> list = taskService.createTaskQuery().list();
		// multiinstance says three in the bpmn
		assertThat(list, hasSize(3));

		foreach (Task task in list)
		{
		  taskService.complete(task.Id);
		}

		waitForJobExecutorToProcessAllJobs(TimeUnit.MILLISECONDS.convert(5L, TimeUnit.SECONDS));

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeServiceTask()
	  public virtual void testAsyncAfterAndBeforeServiceTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// the service task is not yet invoked
		assertNotListenerStartInvoked(pi);
		assertNotBehaviorInvoked(pi);
		assertNotListenerEndInvoked(pi);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the job is executed
		managementService.executeJob(job.Id);

		// the manual task is invoked
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and now the process is waiting *after* the manual task
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the waiting job, the process instance will end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterServiceTaskMultipleTransitions()
	  public virtual void testAsyncAfterServiceTaskMultipleTransitions()
	  {

		// start process instance
		IDictionary<string, object> varMap = new Dictionary<string, object>();
		varMap["flowToTake"] = "flow2";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", varMap);

		// the service task is completely invoked
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi);
		assertListenerEndInvoked(pi);

		// and the execution is waiting *after* the service task
		Job continuationJob = managementService.createJobQuery().singleResult();
		assertNotNull(continuationJob);

		// if we execute the job, the process instance continues along the selected path
		managementService.executeJob(continuationJob.Id);

		assertNotNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow2").singleResult());
		assertNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow3").singleResult());

		// end the process
		runtimeService.signal(pi.Id);

		//////////////////////////////////////////////////////////////

		// start process instance
		varMap = new Dictionary<string, object>();
		varMap["flowToTake"] = "flow3";
		pi = runtimeService.startProcessInstanceByKey("testProcess", varMap);

		// the service task is completely invoked
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi);
		assertListenerEndInvoked(pi);

		// and the execution is waiting *after* the service task
		continuationJob = managementService.createJobQuery().singleResult();
		assertNotNull(continuationJob);

		// if we execute the job, the process instance continues along the selected path
		managementService.executeJob(continuationJob.Id);

		assertNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow2").singleResult());
		assertNotNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow3").singleResult());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterServiceTaskMultipleTransitionsConcurrent()
	  public virtual void testAsyncAfterServiceTaskMultipleTransitionsConcurrent()
	  {

		// start process instance
		IDictionary<string, object> varMap = new Dictionary<string, object>();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", varMap);

		// the service task is completely invoked
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi);
		assertListenerEndInvoked(pi);

		// there are two async jobs
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(2, jobs.Count);
		managementService.executeJob(jobs[0].Id);
		managementService.executeJob(jobs[1].Id);

		// both subsequent tasks are activated
		assertNotNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow2").singleResult());
		assertNotNull(runtimeService.createExecutionQuery().activityId("taskAfterFlow3").singleResult());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterWithoutTransition()
	  public virtual void testAsyncAfterWithoutTransition()
	  {

		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// the service task is completely invoked
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi);
		assertListenerEndInvoked(pi);

		// and the execution is waiting *after* the service task
		Job continuationJob = managementService.createJobQuery().singleResult();
		assertNotNull(continuationJob);

		// but the process end listeners have not been invoked yet
		assertNull(runtimeService.getVariable(pi.Id, "process-listenerEndInvoked"));

		// if we execute the job, the process instance ends.
		managementService.executeJob(continuationJob.Id);
		assertProcessEnded(pi.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterInNestedWithoutTransition()
	  public virtual void testAsyncAfterInNestedWithoutTransition()
	  {

		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// the service task is completely invoked
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi);
		assertListenerEndInvoked(pi);

		// and the execution is waiting *after* the service task
		Job continuationJob = managementService.createJobQuery().singleResult();
		assertNotNull(continuationJob);

		// but the subprocess end listeners have not been invoked yet
		assertNull(runtimeService.getVariable(pi.Id, "subprocess-listenerEndInvoked"));

		// if we execute the job, the listeners are invoked;
		managementService.executeJob(continuationJob.Id);
		assertTrue((bool?)runtimeService.getVariable(pi.Id, "subprocess-listenerEndInvoked"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterManualTask()
	  public virtual void testAsyncAfterManualTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testManualTask");

		// listeners should be fired by now
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the catch event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the waiting job is executed, the process instance should end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeManualTask()
	  public virtual void testAsyncAfterAndBeforeManualTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testManualTask");

		// the service task is not yet invoked
		assertNotListenerStartInvoked(pi);
		assertNotListenerEndInvoked(pi);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the job is executed
		managementService.executeJob(job.Id);

		// the manual task is invoked
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and now the process is waiting *after* the manual task
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the waiting job, the process instance will end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterIntermediateCatchEvent()
	  public virtual void testAsyncAfterIntermediateCatchEvent()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testIntermediateCatchEvent");

		// the intermediate catch event is waiting for its message
		runtimeService.correlateMessage("testMessage1");

		// listeners should be fired by now
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the catch event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the waiting job is executed, the process instance should end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeIntermediateCatchEvent()
	  public virtual void testAsyncAfterAndBeforeIntermediateCatchEvent()
	  {

		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testIntermediateCatchEvent");

		// check that no listener is invoked by now
		assertNotListenerStartInvoked(pi);
		assertNotListenerEndInvoked(pi);

		// the process is waiting before the message event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// execute job to get to the message event
		executeAvailableJobs();

		// now we need to trigger the message to proceed
		runtimeService.correlateMessage("testMessage1");

		// now the listener should be invoked
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and now the process is waiting *after* the intermediate catch event
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the waiting job, the process instance will end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterIntermediateThrowEvent()
	  public virtual void testAsyncAfterIntermediateThrowEvent()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testIntermediateThrowEvent");

		// listeners should be fired by now
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the throw event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the waiting job is executed, the process instance should end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeIntermediateThrowEvent()
	  public virtual void testAsyncAfterAndBeforeIntermediateThrowEvent()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testIntermediateThrowEvent");

		// the throw event is not yet invoked
		assertNotListenerStartInvoked(pi);
		assertNotListenerEndInvoked(pi);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// if the job is executed
		managementService.executeJob(job.Id);

		// the listeners are invoked
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and now the process is waiting *after* the throw event
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the waiting job, the process instance will end
		managementService.executeJob(job.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterInclusiveGateway()
	  public virtual void testAsyncAfterInclusiveGateway()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testInclusiveGateway");

		// listeners should be fired
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the gateway
		assertEquals(2, managementService.createJobQuery().active().count());

		executeAvailableJobs();

		// if the waiting job is executed there should be 2 user tasks
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.active().count());

		// finish tasks
		IList<Task> tasks = taskQuery.active().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeInclusiveGateway()
	  public virtual void testAsyncAfterAndBeforeInclusiveGateway()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testInclusiveGateway");

		// no listeners are fired:
		assertNotListenerStartInvoked(pi);
		assertNotListenerEndInvoked(pi);

		// we should wait *before* the gateway:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the gateway:
		managementService.executeJob(job.Id);

		// the listeners are fired:
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and we will wait *after* the gateway:
		IList<Job> jobs = managementService.createJobQuery().active().list();
		assertEquals(2, jobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterExclusiveGateway()
	  public virtual void testAsyncAfterExclusiveGateway()
	  {
		// start process instance with variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["flow"] = false;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExclusiveGateway", variables);

		// listeners should be fired
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// the process should wait *after* the gateway
		assertEquals(1, managementService.createJobQuery().active().count());

		executeAvailableJobs();

		// if the waiting job is executed there should be 2 user tasks
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.active().count());

		// finish tasks
		IList<Task> tasks = taskQuery.active().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterAndBeforeExclusiveGateway()
	  public virtual void testAsyncAfterAndBeforeExclusiveGateway()
	  {
		// start process instance with variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["flow"] = false;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExclusiveGateway", variables);

		// no listeners are fired:
		assertNotListenerStartInvoked(pi);
		assertNotListenerEndInvoked(pi);

		// we should wait *before* the gateway:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// after executing the gateway:
		managementService.executeJob(job.Id);

		// the listeners are fired:
		assertListenerStartInvoked(pi);
		assertListenerEndInvoked(pi);

		// and we will wait *after* the gateway:
		assertEquals(1, managementService.createJobQuery().active().count());
	  }
	  /// <summary>
	  /// Test for CAM-2518: Fixes an issue that creates an infinite loop when using
	  /// asyncAfter together with an execution listener on sequence flow event "take".
	  /// So the only required assertion here is that the process executes successfully.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterWithExecutionListener()
	  public virtual void testAsyncAfterWithExecutionListener()
	  {
		// given an async after job and an execution listener on that task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		assertNotListenerTakeInvoked(processInstance);

		// when the job is executed
		managementService.executeJob(job.Id);

		// then the process should advance and not recreate the job
		job = managementService.createJobQuery().singleResult();
		assertNull(job);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		assertListenerTakeInvoked(processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterOnParallelGatewayFork()
	  public virtual void testAsyncAfterOnParallelGatewayFork()
	  {
		string configuration = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE.CanonicalName;
		string config1 = configuration + "$afterForkFlow1";
		string config2 = configuration + "$afterForkFlow2";

		runtimeService.startProcessInstanceByKey("process");

		// there are two jobs
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(2, jobs.Count);
		Job jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, config1);
		assertNotNull(jobToExecute);
		managementService.executeJob(jobToExecute.Id);

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("theTask1").singleResult();
		assertNotNull(task1);

		// there is one left
		jobs = managementService.createJobQuery().list();
		assertEquals(1, jobs.Count);
		jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, config2);
		managementService.executeJob(jobToExecute.Id);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("theTask2").singleResult();
		assertNotNull(task2);

		assertEquals(2, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterParallelMultiInstanceWithServiceTask()
	  public virtual void testAsyncAfterParallelMultiInstanceWithServiceTask()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// listeners and behavior should be invoked by now
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi, 5);
		assertListenerEndInvoked(pi);

		// the process should wait *after* execute all service tasks
		executeAvailableJobs(1);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterServiceWrappedInParallelMultiInstance()
	  public virtual void testAsyncAfterServiceWrappedInParallelMultiInstance()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// listeners and behavior should be invoked by now
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi, 5);
		assertListenerEndInvoked(pi);

		// the process should wait *after* execute each service task wrapped in the multi-instance body
		assertEquals(5L, managementService.createJobQuery().count());
		// execute all jobs - one for each service task
		executeAvailableJobs(5);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterServiceWrappedInSequentialMultiInstance()
	  public virtual void testAsyncAfterServiceWrappedInSequentialMultiInstance()
	  {
		// start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// listeners and behavior should be invoked by now
		assertListenerStartInvoked(pi);
		assertBehaviorInvoked(pi, 1);
		assertListenerEndInvoked(pi);

		// the process should wait *after* execute each service task step-by-step
		assertEquals(1L, managementService.createJobQuery().count());
		// execute all jobs - one for each service task wrapped in the multi-instance body
		executeAvailableJobs(5);

		// behavior should be invoked for each service task
		assertBehaviorInvoked(pi, 5);

		// the process should wait on user task after execute all service tasks
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testAsyncAfterOnParallelGatewayJoin()
	  public virtual void FAILING_testAsyncAfterOnParallelGatewayJoin()
	  {
		string configuration = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_END.CanonicalName;

		runtimeService.startProcessInstanceByKey("process");

		// there are three jobs
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(3, jobs.Count);
		Job jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, configuration);
		assertNotNull(jobToExecute);
		managementService.executeJob(jobToExecute.Id);

		// there are two jobs left
		jobs = managementService.createJobQuery().list();
		assertEquals(2, jobs.Count);
		jobToExecute = fetchFirstJobByHandlerConfiguration(jobs, configuration);
		managementService.executeJob(jobToExecute.Id);

		// there is one job left
		jobToExecute = managementService.createJobQuery().singleResult();
		assertNotNull(jobToExecute);
		managementService.executeJob(jobToExecute.Id);

		// the process should stay in the user task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncAfterBoundaryEvent()
	  public virtual void testAsyncAfterBoundaryEvent()
	  {
		// given process instance
		runtimeService.startProcessInstanceByKey("Process");

		// assume
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// when we trigger the event
		runtimeService.correlateMessage("foo");

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		task = taskService.createTaskQuery().singleResult();
		assertNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncBeforeBoundaryEvent()
	  public virtual void testAsyncBeforeBoundaryEvent()
	  {
		// given process instance
		runtimeService.startProcessInstanceByKey("Process");

		// assume
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// when we trigger the event
		runtimeService.correlateMessage("foo");

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		task = taskService.createTaskQuery().singleResult();
		assertNull(task);
	  }

	  public virtual void testAsyncAfterErrorEvent()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent().serviceTask("servTask").camundaClass(typeof(ThrowBpmnErrorDelegate)).boundaryEvent().camundaAsyncAfter(true).camundaFailedJobRetryTimeCycle("R10/PT10S").errorEventDefinition().errorEventDefinitionDone().serviceTask().camundaClass("foo").endEvent().moveToActivity("servTask").endEvent().done();
		deployment(instance);

		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

	   // when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(9, job.Retries);
	  }

	  protected internal virtual Job fetchFirstJobByHandlerConfiguration(IList<Job> jobs, string configuration)
	  {
		foreach (Job job in jobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  string jobConfig = jobEntity.JobHandlerConfigurationRaw;
		  if (configuration.Equals(jobConfig))
		  {
			return job;
		  }
		}

		return null;
	  }

	  protected internal virtual void assertListenerStartInvoked(Execution e)
	  {
		assertTrue((bool?) runtimeService.getVariable(e.Id, "listenerStartInvoked"));
	  }

	  protected internal virtual void assertListenerTakeInvoked(Execution e)
	  {
		assertTrue((bool?) runtimeService.getVariable(e.Id, "listenerTakeInvoked"));
	  }

	  protected internal virtual void assertListenerEndInvoked(Execution e)
	  {
		assertTrue((bool?) runtimeService.getVariable(e.Id, "listenerEndInvoked"));
	  }

	  protected internal virtual void assertBehaviorInvoked(Execution e)
	  {
		assertTrue((bool?) runtimeService.getVariable(e.Id, "behaviorInvoked"));
	  }

	  private void assertBehaviorInvoked(ProcessInstance pi, int times)
	  {
		long? behaviorInvoked = (long?) runtimeService.getVariable(pi.Id, "behaviorInvoked");
		assertNotNull("behavior was not invoked", behaviorInvoked);
		assertEquals(times, behaviorInvoked.Value);

	  }

	  protected internal virtual void assertNotListenerStartInvoked(Execution e)
	  {
		assertNull(runtimeService.getVariable(e.Id, "listenerStartInvoked"));
	  }

	  protected internal virtual void assertNotListenerTakeInvoked(Execution e)
	  {
		assertNull(runtimeService.getVariable(e.Id, "listenerTakeInvoked"));
	  }

	  protected internal virtual void assertNotListenerEndInvoked(Execution e)
	  {
		assertNull(runtimeService.getVariable(e.Id, "listenerEndInvoked"));
	  }

	  protected internal virtual void assertNotBehaviorInvoked(Execution e)
	  {
		assertNull(runtimeService.getVariable(e.Id, "behaviorInvoked"));
	  }

	}

}