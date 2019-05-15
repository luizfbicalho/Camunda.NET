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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;


	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Stefan Hentschel
	/// </summary>
	public class AsyncTaskTest : PluggableProcessEngineTestCase
	{

	  public static bool INVOCATION;
	  public static int NUM_INVOCATIONS = 0;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceNoListeners()
	  public virtual void testAsyncServiceNoListeners()
	  {
		INVOCATION = false;
		// start process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncService");

		// now we have one transition instance below the process instance:
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(1, activityInstance.ChildTransitionInstances.Length);
		assertEquals(0, activityInstance.ChildActivityInstances.Length);

		assertNotNull(activityInstance.ChildTransitionInstances[0]);

		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the service was not invoked:
		assertFalse(INVOCATION);

		executeAvailableJobs();

		// the service was invoked
		assertTrue(INVOCATION);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceListeners()
	  public virtual void testAsyncServiceListeners()
	  {
		string pid = runtimeService.startProcessInstanceByKey("asyncService").ProcessInstanceId;
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not yet invoked:
		assertNull(runtimeService.getVariable(pid, "listener"));

		executeAvailableJobs();

		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceConcurrent()
	  public virtual void testAsyncServiceConcurrent()
	  {
		INVOCATION = false;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");
		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the service was not invoked:
		assertFalse(INVOCATION);

		executeAvailableJobs();

		// the service was invoked
		assertTrue(INVOCATION);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncSequentialMultiInstanceWithServiceTask()
	  public virtual void testAsyncSequentialMultiInstanceWithServiceTask()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for the multi-instance body to execute:
		executeAvailableJobs(1);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncParallelMultiInstanceWithServiceTask()
	  public virtual void testAsyncParallelMultiInstanceWithServiceTask()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for the multi-instance body to execute:
		executeAvailableJobs(1);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceWrappedInSequentialMultiInstance()
	  public virtual void testAsyncServiceWrappedInSequentialMultiInstance()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for the first service task wrapped in the multi-instance body:
		assertEquals(1, managementService.createJobQuery().count());
		// execute all jobs - one for each service task:
		executeAvailableJobs(5);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceWrappedInParallelMultiInstance()
	  public virtual void testAsyncServiceWrappedInParallelMultiInstance()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for each service task wrapped in the multi-instance body:
		assertEquals(5, managementService.createJobQuery().count());
		// execute all jobs:
		executeAvailableJobs(5);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncBeforeAndAfterOfServiceWrappedInParallelMultiInstance()
	  public virtual void testAsyncBeforeAndAfterOfServiceWrappedInParallelMultiInstance()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for each service task wrapped in the multi-instance body:
		assertEquals(5, managementService.createJobQuery().count());
		// execute all jobs - one for asyncBefore and another for asyncAfter:
		executeAvailableJobs(5 + 5);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncBeforeSequentialMultiInstanceWithAsyncAfterServiceWrappedInMultiInstance()
	  public virtual void testAsyncBeforeSequentialMultiInstanceWithAsyncAfterServiceWrappedInMultiInstance()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for the multi-instance body:
		assertEquals(1, managementService.createJobQuery().count());
		// execute all jobs - one for multi-instance body and one for each service task wrapped in the multi-instance body:
		executeAvailableJobs(1 + 5);

		// the service was invoked
		assertEquals(5, NUM_INVOCATIONS);
		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

	  protected internal virtual void assertTransitionInstances(string processInstanceId, string activityId, int numInstances)
	  {
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		assertEquals(numInstances, tree.getTransitionInstances(activityId).Length);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncBeforeAndAfterParallelMultiInstanceWithAsyncBeforeAndAfterServiceWrappedInMultiInstance()
	  public virtual void testAsyncBeforeAndAfterParallelMultiInstanceWithAsyncBeforeAndAfterServiceWrappedInMultiInstance()
	  {
		NUM_INVOCATIONS = 0;
		// start process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncService");

		// the service was not invoked:
		assertEquals(0, NUM_INVOCATIONS);

		// now there should be one job for the multi-instance body:
		assertEquals(1, managementService.createJobQuery().count());
		assertTransitionInstances(processInstance.Id, "service" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, 1);

		// when the mi body before job is executed
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// then there are five inner async before jobs
		IList<Job> innerBeforeJobs = managementService.createJobQuery().list();
		assertEquals(5, innerBeforeJobs.Count);
		assertTransitionInstances(processInstance.Id, "service", 5);
		assertEquals(0, NUM_INVOCATIONS);

		// when executing all inner jobs
		foreach (Job innerBeforeJob in innerBeforeJobs)
		{
		  managementService.executeJob(innerBeforeJob.Id);
		}
		assertEquals(5, NUM_INVOCATIONS);

		// then there are five async after jobs
		IList<Job> innerAfterJobs = managementService.createJobQuery().list();
		assertEquals(5, innerAfterJobs.Count);
		assertTransitionInstances(processInstance.Id, "service", 5);

		// when executing all inner jobs
		foreach (Job innerAfterJob in innerAfterJobs)
		{
		  managementService.executeJob(innerAfterJob.Id);
		}

		// then there is one mi body after job
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertTransitionInstances(processInstance.Id, "service" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, 1);

		// when executing this job, the process ends
		managementService.executeJob(job.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/async/AsyncTaskTest.testAsyncServiceWrappedInParallelMultiInstance.bpmn20.xml")]
	  public virtual void testAsyncServiceWrappedInParallelMultiInstanceActivityInstance()
	  {
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncService");

		// when there are five jobs for the inner activity
		assertEquals(5, managementService.createJobQuery().count());

		// then they are represented in the activity instance tree by transition instances
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("service#multiInstanceBody").transition("service").transition("service").transition("service").transition("service").transition("service").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingAsyncServiceTimer()
	  public virtual void testFailingAsyncServiceTimer()
	  {
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");
		// now there should be one job in the database, and it is a message
		assertEquals(1, managementService.createJobQuery().count());
		Job job = managementService.createJobQuery().singleResult();
		if (!(job is MessageEntity))
		{
		  fail("the job must be a message");
		}

		executeAvailableJobs();

		// the service failed: the execution is still sitting in the service task:
		Execution execution = runtimeService.createExecutionQuery().singleResult();
		assertNotNull(execution);
		assertEquals("service", runtimeService.getActiveActivityIds(execution.Id)[0]);

		// there is still a single job because the timer was created in the same transaction as the
		// service was executed (which rolled back)
		assertEquals(1, managementService.createJobQuery().count());

		runtimeService.deleteProcessInstance(execution.Id, "dead");
	  }

	  // TODO: Think about this:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testFailingAsyncServiceTimer()
	  public virtual void FAILING_testFailingAsyncServiceTimer()
	  {
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");
		// now there are two jobs the message and a timer:
		assertEquals(2, managementService.createJobQuery().count());

		// let 'max-retires' on the message be reached
		executeAvailableJobs();

		// the service failed: the execution is still sitting in the service task:
		Execution execution = runtimeService.createExecutionQuery().singleResult();
		assertNotNull(execution);
		assertEquals("service", runtimeService.getActiveActivityIds(execution.Id)[0]);

		// there are two jobs, the message and the timer (the message will not be retried anymore, max retires is reached.)
		assertEquals(2, managementService.createJobQuery().count());

		// now the timer triggers:
		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 10000);
		executeAvailableJobs();

		// and we are done:
		assertNull(runtimeService.createExecutionQuery().singleResult());
		// and there are no more jobs left:
		assertEquals(0, managementService.createJobQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceSubProcessTimer()
	  public virtual void testAsyncServiceSubProcessTimer()
	  {
		INVOCATION = false;
		// start process
		runtimeService.startProcessInstanceByKey("asyncService");
		// now there should be two jobs in the database:
		assertEquals(2, managementService.createJobQuery().count());
		// the service was not invoked:
		assertFalse(INVOCATION);

		Job job = managementService.createJobQuery().messages().singleResult();
		managementService.executeJob(job.Id);

		// the service was invoked
		assertTrue(INVOCATION);
		// both the timer and the message are cancelled
		assertEquals(0, managementService.createJobQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncServiceSubProcess()
	  public virtual void testAsyncServiceSubProcess()
	  {
		// start process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncService");

		assertEquals(1, managementService.createJobQuery().count());

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("subProcess").done());

		executeAvailableJobs();

		// both the timer and the message are cancelled
		assertEquals(0, managementService.createJobQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncTask()
	  public virtual void testAsyncTask()
	  {
		// start process
		runtimeService.startProcessInstanceByKey("asyncTask");
		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());

		executeAvailableJobs();

		// the job is done
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncScript()
	  public virtual void testAsyncScript()
	  {
		// start process
		runtimeService.startProcessInstanceByKey("asyncScript").ProcessInstanceId;
		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the script was not invoked:
		string eid = runtimeService.createExecutionQuery().singleResult().Id;
		assertNull(runtimeService.getVariable(eid, "invoked"));

		executeAvailableJobs();

		// and the job is done
		assertEquals(0, managementService.createJobQuery().count());

		// the script was invoked
		assertEquals("true", runtimeService.getVariable(eid, "invoked"));

		runtimeService.signal(eid);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/async/AsyncTaskTest.testAsyncCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/async/AsyncTaskTest.testAsyncServiceNoListeners.bpmn20.xml"})]
	  public virtual void testAsyncCallActivity()
	  {
		// start process
		runtimeService.startProcessInstanceByKey("asyncCallactivity");
		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());

		executeAvailableJobs();

		assertEquals(0, managementService.createJobQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncUserTask()
	  public virtual void testAsyncUserTask()
	  {
		// start process
		string pid = runtimeService.startProcessInstanceByKey("asyncUserTask").ProcessInstanceId;
		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not yet invoked:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no usertask
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();
		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));

		// there is a usertask
		assertNotNull(taskService.createTaskQuery().singleResult());
		// and no more job
		assertEquals(0, managementService.createJobQuery().count());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncManualTask()
	  public virtual void testAsyncManualTask()
	  {
		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncManualTask").ProcessInstanceId;

		// now there should be one job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not yet invoked:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no manual Task
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();

		// the listener was invoked now:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore:
		assertEquals(0, managementService.createJobQuery().count());
		// now there is a userTask
		assertNotNull(taskService.createTaskQuery().singleResult());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncIntermediateCatchEvent()
	  public virtual void testAsyncIntermediateCatchEvent()
	  {
		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncIntermediateCatchEvent").ProcessInstanceId;

		// now there is 1 job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not invoked now:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no intermediate catch event:
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();
		runtimeService.correlateMessage("testMessage1");

		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore
		assertEquals(0, managementService.createJobQuery().count());
		// now there is a userTask
		assertNotNull(taskService.createTaskQuery().singleResult());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncIntermediateThrowEvent()
	  public virtual void testAsyncIntermediateThrowEvent()
	  {
		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncIntermediateThrowEvent").ProcessInstanceId;

		// now there is 1 job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not invoked now:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no intermediate throw event:
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();

		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore
		assertEquals(0, managementService.createJobQuery().count());
		// now there is a userTask
		assertNotNull(taskService.createTaskQuery().singleResult());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncExclusiveGateway()
	  public virtual void testAsyncExclusiveGateway()
	  {
		// The test needs variables to work properly
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["flow"] = false;

		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncExclusiveGateway", variables).ProcessInstanceId;

		// now there is 1 job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not invoked now:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no gateway:
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();

		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore
		assertEquals(0, managementService.createJobQuery().count());
		// now there is a userTask
		assertNotNull(taskService.createTaskQuery().singleResult());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncInclusiveGateway()
	  public virtual void testAsyncInclusiveGateway()
	  {
		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncInclusiveGateway").ProcessInstanceId;

		// now there is 1 job in the database:
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not invoked now:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no gateway:
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();

		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore
		assertEquals(0, managementService.createJobQuery().count());
		// now there are 2 user tasks
		IList<Task> list = taskService.createTaskQuery().list();
		assertEquals(2, list.Count);

		// complete these tasks and finish the process instance
		foreach (Task task in list)
		{
		  taskService.complete(task.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncEventGateway()
	  public virtual void testAsyncEventGateway()
	  {
		// start PI
		string pid = runtimeService.startProcessInstanceByKey("asyncEventGateway").ProcessInstanceId;

		// now there is a job in the database
		assertEquals(1, managementService.createJobQuery().count());
		// the listener was not invoked now:
		assertNull(runtimeService.getVariable(pid, "listener"));
		// there is no task:
		assertNull(taskService.createTaskQuery().singleResult());

		executeAvailableJobs();

		// the listener was now invoked:
		assertNotNull(runtimeService.getVariable(pid, "listener"));
		// there isn't a job anymore
		assertEquals(0, managementService.createJobQuery().count());

		// correlate Message
		runtimeService.correlateMessage("testMessageDef1");

		// now there is a userTask
		assertNotNull(taskService.createTaskQuery().singleResult());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);
	  }

	  /// <summary>
	  /// CAM-3707
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeleteShouldNotInvokeListeners()
	  public virtual void testDeleteShouldNotInvokeListeners()
	  {
		RecorderExecutionListener.clear();

		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("asyncListener", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));
		assertEquals(1, managementService.createJobQuery().count());

		// when deleting the process instance
		runtimeService.deleteProcessInstance(instance.Id, "");

		// then no listeners for the async activity should have been invoked because
		// it was not active yet
		assertEquals(0, RecorderExecutionListener.RecordedEvents.Count);

		RecorderExecutionListener.clear();
	  }

	  /// <summary>
	  /// CAM-3707
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeleteInScopeShouldNotInvokeListeners()
	  public virtual void testDeleteInScopeShouldNotInvokeListeners()
	  {
		RecorderExecutionListener.clear();

		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("asyncListenerSubProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));
		assertEquals(1, managementService.createJobQuery().count());

		// when deleting the process instance
		runtimeService.deleteProcessInstance(instance.Id, "");

		// then the async task end listener has not been executed but the listeners of the sub
		// process and the process

		IList<RecorderExecutionListener.RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);
		assertEquals("subProcess", recordedEvents[0].ActivityId);
		assertNull(recordedEvents[1].ActivityId); // process instance end event has no activity id

		RecorderExecutionListener.clear();
	  }

	  /// <summary>
	  /// CAM-3708
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeleteShouldNotInvokeOutputMapping()
	  public virtual void testDeleteShouldNotInvokeOutputMapping()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("asyncOutputMapping");
		assertEquals(1, managementService.createJobQuery().count());

		// when
		runtimeService.deleteProcessInstance(instance.Id, "");

		// then the output mapping has not been executed because the
		// activity was not active yet
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{
		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		}

	  }

	  /// <summary>
	  /// CAM-3708
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeleteInScopeShouldNotInvokeOutputMapping()
	  public virtual void testDeleteInScopeShouldNotInvokeOutputMapping()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("asyncOutputMappingSubProcess");
		assertEquals(1, managementService.createJobQuery().count());

		// when
		runtimeService.deleteProcessInstance(instance.Id, "");

		// then
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{
		  // the output mapping of the task has not been executed because the
		  // activity was not active yet
		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().variableName("taskOutputMappingExecuted").count());

		  // but the containing sub process output mapping was executed
		  assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("subProcessOutputMappingExecuted").count());
		}
	  }

	  public virtual void testDeployAndRemoveAsyncActivity()
	  {
		ISet<string> deployments = new HashSet<string>();

		try
		{
		  // given a deployment that contains a process called "process" with an async task "task"
		  org.camunda.bpm.engine.repository.Deployment deployment1 = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/async/AsyncTaskTest.testDeployAndRemoveAsyncActivity.v1.bpmn20.xml").deploy();
		  deployments.Add(deployment1.Id);

		  // when redeploying the process where that task is not contained anymore
		  org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/async/AsyncTaskTest.testDeployAndRemoveAsyncActivity.v2.bpmn20.xml").deploy();
		  deployments.Add(deployment2.Id);

		  // and clearing the deployment cache (note that the equivalent of this in a real-world
		  // scenario would be making the deployment with a different engine
		  processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		  // then it should be possible to load the latest process definition
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		  assertNotNull(processInstance);

		}
		finally
		{
		  foreach (string deploymentId in deployments)
		  {
			repositoryService.deleteDeployment(deploymentId, true);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/async/processWithGatewayAndTwoEndEvents.bpmn20.xml"})]
	  public virtual void testGatewayWithTwoEndEventsLastJobReAssignedToParentExe()
	  {
		string processKey = repositoryService.createProcessDefinitionQuery().singleResult().Key;
		string processInstanceId = runtimeService.startProcessInstanceByKey(processKey).Id;

		IList<Job> jobList = managementService.createJobQuery().processInstanceId(processInstanceId).list();

		// There should be two jobs
		assertNotNull(jobList);
		assertEquals(2, jobList.Count);

		managementService.executeJob(jobList[0].Id);

		// There should be only one job left
		jobList = managementService.createJobQuery().list();
		assertEquals(1, jobList.Count);

		// There should only be 1 execution left - the root execution
		assertEquals(1, runtimeService.createExecutionQuery().list().size());

		// root execution should be attached to the last job
		assertEquals(processInstanceId, jobList[0].ExecutionId);

		managementService.executeJob(jobList[0].Id);

		// There should be no more jobs
		jobList = managementService.createJobQuery().list();
		assertEquals(0, jobList.Count);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/async/processGatewayAndTwoEndEventsPlusTimer.bpmn20.xml"})]
	  public virtual void testGatewayWithTwoEndEventsLastTimerReAssignedToParentExe()
	  {
		string processKey = repositoryService.createProcessDefinitionQuery().singleResult().Key;
		string processInstanceId = runtimeService.startProcessInstanceByKey(processKey).Id;

		IList<Job> jobList = managementService.createJobQuery().processInstanceId(processInstanceId).list();

		// There should be two jobs
		assertNotNull(jobList);
		assertEquals(2, jobList.Count);

		// execute timer first
		string timerId = managementService.createJobQuery().timers().singleResult().Id;
		managementService.executeJob(timerId);

		// There should be only one job left
		jobList = managementService.createJobQuery().list();
		assertEquals(1, jobList.Count);

		// There should only be 1 execution left - the root execution
		assertEquals(1, runtimeService.createExecutionQuery().list().size());

		// root execution should be attached to the last job
		assertEquals(processInstanceId, jobList[0].ExecutionId);

		// execute service task
		managementService.executeJob(jobList[0].Id);

		// There should be no more jobs
		jobList = managementService.createJobQuery().list();
		assertEquals(0, jobList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testLongProcessDefinitionKey()
	  public virtual void FAILING_testLongProcessDefinitionKey()
	  {
		string key = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";
		string processInstanceId = runtimeService.startProcessInstanceByKey(key).Id;

		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();

		assertEquals(key, job.ProcessDefinitionKey);
	  }
	}

}