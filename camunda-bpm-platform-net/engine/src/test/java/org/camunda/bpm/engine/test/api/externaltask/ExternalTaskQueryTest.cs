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
namespace org.camunda.bpm.engine.test.api.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskByLockExpirationTime;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskByProcessDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskByProcessDefinitionKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string WORKER_ID = "aWorkerId";
	  protected internal const string TOPIC_NAME = "externalTaskTopic";
	  protected internal const string ERROR_MESSAGE = "error";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		ClockUtil.reset();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSingleResult()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();

		// then
		assertNotNull(externalTask.Id);

		assertEquals(processInstance.Id, externalTask.ProcessInstanceId);
		assertEquals("externalTask", externalTask.ActivityId);
		assertNotNull(externalTask.ActivityInstanceId);
		assertNotNull(externalTask.ExecutionId);
		assertEquals(processInstance.ProcessDefinitionId, externalTask.ProcessDefinitionId);
		assertEquals("oneExternalTaskProcess", externalTask.ProcessDefinitionKey);
		assertEquals(TOPIC_NAME, externalTask.TopicName);
		assertNull(externalTask.WorkerId);
		assertNull(externalTask.LockExpirationTime);
		assertFalse(externalTask.Suspended);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testList()
	  {
		startInstancesByKey("oneExternalTaskProcess", 5);
		assertEquals(5, externalTaskService.createExternalTaskQuery().list().size());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCount()
	  {
		startInstancesByKey("oneExternalTaskProcess", 5);
		assertEquals(5, externalTaskService.createExternalTaskQuery().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByLockState()
	  {
		// given
		startInstancesByKey("oneExternalTaskProcess", 5);
		lockInstances(TOPIC_NAME, 10000L, 3, WORKER_ID);

		// when
		IList<ExternalTask> lockedTasks = externalTaskService.createExternalTaskQuery().locked().list();
		IList<ExternalTask> nonLockedTasks = externalTaskService.createExternalTaskQuery().notLocked().list();

		// then
		assertEquals(3, lockedTasks.Count);
		foreach (ExternalTask task in lockedTasks)
		{
		  assertNotNull(task.LockExpirationTime);
		}

		assertEquals(2, nonLockedTasks.Count);
		foreach (ExternalTask task in nonLockedTasks)
		{
		  assertNull(task.LockExpirationTime);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByProcessDefinitionId()
	  {
		// given
		org.camunda.bpm.engine.repository.Deployment secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml").deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		startInstancesById(processDefinitions[0].Id, 3);
		startInstancesById(processDefinitions[1].Id, 2);

		// when
		IList<ExternalTask> definition1Tasks = externalTaskService.createExternalTaskQuery().processDefinitionId(processDefinitions[0].Id).list();
		IList<ExternalTask> definition2Tasks = externalTaskService.createExternalTaskQuery().processDefinitionId(processDefinitions[1].Id).list();

		// then
		assertEquals(3, definition1Tasks.Count);
		foreach (ExternalTask task in definition1Tasks)
		{
		  assertEquals(processDefinitions[0].Id, task.ProcessDefinitionId);
		}

		assertEquals(2, definition2Tasks.Count);
		foreach (ExternalTask task in definition2Tasks)
		{
		  assertEquals(processDefinitions[1].Id, task.ProcessDefinitionId);
		}

		// cleanup
		repositoryService.deleteDeployment(secondDeployment.Id, true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByActivityId()
	  {
		// given
		startInstancesByKey("parallelExternalTaskProcess", 3);

		// when
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().activityId("externalTask2").list();

		// then
		assertEquals(3, tasks.Count);
		foreach (ExternalTask task in tasks)
		{
		  assertEquals("externalTask2", task.ActivityId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByActivityIdIn()
	  {
		// given
		startInstancesByKey("parallelExternalTaskProcess", 3);

		IList<string> activityIds = new IList<string> {"externalTask1", "externalTask2"};

		// when
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().activityIdIn(activityIds.ToArray()).list();

		// then
		assertEquals(6, tasks.Count);
		foreach (ExternalTask task in tasks)
		{
		  assertTrue(activityIds.Contains(task.ActivityId));
		}
	  }

	  public virtual void testFailQueryByActivityIdInNull()
	  {
		try
		{
		  externalTaskService.createExternalTaskQuery().activityIdIn((string) null);
		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByTopicName()
	  {
		// given
		startInstancesByKey("parallelExternalTaskProcess", 3);

		// when
		IList<ExternalTask> topic1Tasks = externalTaskService.createExternalTaskQuery().topicName("topic1").list();

		// then
		assertEquals(3, topic1Tasks.Count);
		foreach (ExternalTask task in topic1Tasks)
		{
		  assertEquals("topic1", task.TopicName);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByProcessInstanceId()
	  {
		// given
		IList<ProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 3);

		// when
		ExternalTask task = externalTaskService.createExternalTaskQuery().processInstanceId(processInstances[0].Id).singleResult();

		// then
		assertNotNull(task);
		assertEquals(processInstances[0].Id, task.ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByLargeListOfProcessInstanceIdIn()
	  {
		// given
		IList<string> processInstances = new List<string>();
		for (int i = 0; i < 1001; i++)
		{
		  processInstances.Add(runtimeService.startProcessInstanceByKey("oneExternalTaskProcess").ProcessInstanceId);
		}

		// when
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().processInstanceIdIn(processInstances.ToArray()).list();

		// then
		assertNotNull(tasks);
		assertEquals(1001, tasks.Count);
		foreach (ExternalTask task in tasks)
		{
		  assertTrue(processInstances.Contains(task.ProcessInstanceId));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByProcessInstanceIdIn()
	  {
		// given
		IList<ProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 3);

		IList<string> processInstanceIds = new IList<string> {processInstances[0].Id, processInstances[1].Id};

		// when
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().processInstanceIdIn(processInstances[0].Id, processInstances[1].Id).list();

		// then
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);
		foreach (ExternalTask task in tasks)
		{
		  assertTrue(processInstanceIds.Contains(task.ProcessInstanceId));
		}
	  }

	  public virtual void testQueryByNonExistingProcessInstanceId()
	  {
		ExternalTaskQuery query = externalTaskService.createExternalTaskQuery().processInstanceIdIn("nonExisting");

		assertEquals(0, query.count());
	  }

	  public virtual void testQueryByProcessInstanceIdNull()
	  {
		try
		{
		  externalTaskService.createExternalTaskQuery().processInstanceIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByExecutionId()
	  {
		// given
		IList<ProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 3);

		ProcessInstance firstInstance = processInstances[0];

		ActivityInstance externalTaskActivityInstance = runtimeService.getActivityInstance(firstInstance.Id).getActivityInstances("externalTask")[0];
		string executionId = externalTaskActivityInstance.ExecutionIds[0];

		// when
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().executionId(executionId).singleResult();

		// then
		assertNotNull(externalTask);
		assertEquals(executionId, externalTask.ExecutionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByWorkerId()
	  {
		// given
		startInstancesByKey("oneExternalTaskProcess", 10);
		lockInstances(TOPIC_NAME, 10000L, 3, "worker1");
		lockInstances(TOPIC_NAME, 10000L, 4, "worker2");

		// when
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().workerId("worker1").list();

		// then
		assertEquals(3, tasks.Count);
		foreach (ExternalTask task in tasks)
		{
		  assertEquals("worker1", task.WorkerId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByLockExpirationTime()
	  {
		// given
		startInstancesByKey("oneExternalTaskProcess", 10);
		lockInstances(TOPIC_NAME, 5000L, 3, WORKER_ID);
		lockInstances(TOPIC_NAME, 10000L, 4, WORKER_ID);

		// when
		DateTime lockDate = new DateTime(ClockUtil.CurrentTime.Ticks + 7000L);
		IList<ExternalTask> lockedExpirationBeforeTasks = externalTaskService.createExternalTaskQuery().lockExpirationBefore(lockDate).list();

		IList<ExternalTask> lockedExpirationAfterTasks = externalTaskService.createExternalTaskQuery().lockExpirationAfter(lockDate).list();

		// then
		assertEquals(3, lockedExpirationBeforeTasks.Count);
		foreach (ExternalTask task in lockedExpirationBeforeTasks)
		{
		  assertNotNull(task.LockExpirationTime);
		  assertTrue(task.LockExpirationTime.Ticks < lockDate.Ticks);
		}

		assertEquals(4, lockedExpirationAfterTasks.Count);
		foreach (ExternalTask task in lockedExpirationAfterTasks)
		{
		  assertNotNull(task.LockExpirationTime);
		  assertTrue(task.LockExpirationTime.Ticks > lockDate.Ticks);
		}
	  }

	  public virtual void testQueryWithNullValues()
	  {
		try
		{
		  externalTaskService.createExternalTaskQuery().externalTaskId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("externalTaskId is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().activityId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("activityId is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().executionId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("executionId is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().lockExpirationAfter(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("lockExpirationAfter is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().lockExpirationBefore(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("lockExpirationBefore is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().processDefinitionId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("processDefinitionId is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().processInstanceId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("processInstanceId is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().topicName(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("topicName is null", e.Message);
		}

		try
		{
		  externalTaskService.createExternalTaskQuery().workerId(null).list();
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("workerId is null", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testQuerySorting()
	  {

		startInstancesByKey("oneExternalTaskProcess", 5);
		startInstancesByKey("twoExternalTaskProcess", 5);

		lockInstances(TOPIC_NAME, 5000L, 5, WORKER_ID);
		lockInstances(TOPIC_NAME, 10000L, 5, WORKER_ID);

		// asc
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().orderById().asc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, externalTaskById());

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessInstanceId().asc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, externalTaskByProcessInstanceId());

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessDefinitionId().asc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, externalTaskByProcessDefinitionId());

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessDefinitionKey().asc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, externalTaskByProcessDefinitionKey());

		tasks = externalTaskService.createExternalTaskQuery().orderByLockExpirationTime().asc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, externalTaskByLockExpirationTime());

		// desc
		tasks = externalTaskService.createExternalTaskQuery().orderById().desc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, inverted(externalTaskById()));

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessInstanceId().desc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, inverted(externalTaskByProcessInstanceId()));

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessDefinitionId().desc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, inverted(externalTaskByProcessDefinitionId()));

		tasks = externalTaskService.createExternalTaskQuery().orderByProcessDefinitionKey().desc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, inverted(externalTaskByProcessDefinitionKey()));

		tasks = externalTaskService.createExternalTaskQuery().orderByLockExpirationTime().desc().list();
		assertEquals(10, tasks.Count);
		verifySorting(tasks, inverted(externalTaskByLockExpirationTime()));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryBySuspensionState()
	  {
		// given
		startInstancesByKey("oneExternalTaskProcess", 5);
		suspendInstances(3);

		// when
		IList<ExternalTask> suspendedTasks = externalTaskService.createExternalTaskQuery().suspended().list();
		IList<ExternalTask> activeTasks = externalTaskService.createExternalTaskQuery().active().list();

		// then
		assertEquals(3, suspendedTasks.Count);
		foreach (ExternalTask task in suspendedTasks)
		{
		  assertTrue(task.Suspended);
		}

		assertEquals(2, activeTasks.Count);
		foreach (ExternalTask task in activeTasks)
		{
		  assertFalse(task.Suspended);
		  assertFalse(suspendedTasks.Contains(task));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByRetries()
	  {
		// given
		startInstancesByKey("oneExternalTaskProcess", 5);

		IList<LockedExternalTask> tasks = lockInstances(TOPIC_NAME, 10000L, 3, WORKER_ID);
		failInstances(tasks.subList(0, 2), ERROR_MESSAGE, 0, 5000L); // two tasks have no retries left
		failInstances(tasks.subList(2, 3), ERROR_MESSAGE, 4, 5000L); // one task has retries left

		// when
		IList<ExternalTask> tasksWithRetries = externalTaskService.createExternalTaskQuery().withRetriesLeft().list();
		IList<ExternalTask> tasksWithoutRetries = externalTaskService.createExternalTaskQuery().noRetriesLeft().list();

		// then
		assertEquals(3, tasksWithRetries.Count);
		foreach (ExternalTask task in tasksWithRetries)
		{
		  assertTrue(task.Retries == null || task.Retries > 0);
		}

		assertEquals(2, tasksWithoutRetries.Count);
		foreach (ExternalTask task in tasksWithoutRetries)
		{
		  assertTrue(task.Retries == 0);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryById()
	  {
		// given
		IList<ProcessInstance> processInstances = startInstancesByKey("oneExternalTaskProcess", 2);
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();

		ProcessInstance firstInstance = processInstances[0];
		ExternalTask firstTask = tasks[0];
		if (!firstTask.ProcessInstanceId.Equals(firstInstance.Id))
		{
		  firstTask = tasks[1];
		}

		// when
		ExternalTask resultTask = externalTaskService.createExternalTaskQuery().externalTaskId(firstTask.Id).singleResult();

		// then
		assertEquals(firstTask.Id, resultTask.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByBusinessKey()
	  {
		// given
		string businessKey = "theUltimateKey";
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess", businessKey);

		// when
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();

		// then
		assertNotNull(externalTask);
		assertEquals(businessKey, externalTask.BusinessKey);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryListByBusinessKey()
	  {
		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey("oneExternalTaskProcess", "businessKey" + i);
		}

		assertEquals(5, externalTaskService.createExternalTaskQuery().count());
		IList<ExternalTask> list = externalTaskService.createExternalTaskQuery().list();
		foreach (ExternalTask externalTask in list)
		{
		  assertNotNull(externalTask.BusinessKey);
		}
	  }


	  protected internal virtual IList<ProcessInstance> startInstancesByKey(string processDefinitionKey, int number)
	  {
		IList<ProcessInstance> processInstances = new List<ProcessInstance>();
		for (int i = 0; i < number; i++)
		{
		  processInstances.Add(runtimeService.startProcessInstanceByKey(processDefinitionKey));
		}

		return processInstances;
	  }

	  protected internal virtual IList<ProcessInstance> startInstancesById(string processDefinitionId, int number)
	  {
		IList<ProcessInstance> processInstances = new List<ProcessInstance>();
		for (int i = 0; i < number; i++)
		{
		  processInstances.Add(runtimeService.startProcessInstanceById(processDefinitionId));
		}

		return processInstances;
	  }

	  protected internal virtual void suspendInstances(int number)
	  {
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().listPage(0, number);

		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.suspendProcessInstanceById(processInstance.Id);
		}
	  }

	  protected internal virtual IList<LockedExternalTask> lockInstances(string topic, long duration, int number, string workerId)
	  {
		return externalTaskService.fetchAndLock(number, workerId).topic(topic, duration).execute();
	  }

	  protected internal virtual void failInstances(IList<LockedExternalTask> tasks, string errorMessage, int retries, long retryTimeout)
	  {
		this.failInstances(tasks,errorMessage,null,retries,retryTimeout);
	  }

	  protected internal virtual void failInstances(IList<LockedExternalTask> tasks, string errorMessage, string errorDetails, int retries, long retryTimeout)
	  {
		foreach (LockedExternalTask task in tasks)
		{
		  externalTaskService.handleFailure(task.Id, task.WorkerId, errorMessage, errorDetails, retries, retryTimeout);
		}
	  }

	}

}