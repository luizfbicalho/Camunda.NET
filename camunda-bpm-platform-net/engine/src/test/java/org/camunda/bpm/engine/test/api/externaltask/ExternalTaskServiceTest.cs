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
	using ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;

	using RuntimeSqlException = org.apache.ibatis.jdbc.RuntimeSqlException;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using AssertUtil = org.camunda.bpm.engine.test.util.AssertUtil;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using DateTime = org.joda.time.DateTime;
	using Assert = org.junit.Assert;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.greaterThan;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskServiceTest : PluggableProcessEngineTestCase
	{

	  protected internal const string WORKER_ID = "aWorkerId";
	  protected internal const long LOCK_TIME = 10000L;
	  protected internal const string TOPIC_NAME = "externalTaskTopic";

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

	  public virtual void testFailOnMalformedpriorityInput()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/externaltask/externalTaskInvalidPriority.bpmn20.xml").deploy();
		  fail("deploying a process with malformed priority should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("value 'NOTaNumber' for attribute 'taskPriority' " + "is not a valid number", e.Message);
		}
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetch()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		assertNotNull(task.Id);
		assertEquals(processInstance.Id, task.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
		assertEquals("externalTask", task.ActivityId);
		assertEquals("oneExternalTaskProcess", task.ProcessDefinitionKey);
		assertEquals(TOPIC_NAME, task.TopicName);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("externalTask")[0];

		assertEquals(activityInstance.Id, task.ActivityInstanceId);
		assertEquals(activityInstance.ExecutionIds[0], task.ExecutionId);

		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

		assertEquals(WORKER_ID, task.WorkerId);
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml")]
	  public virtual void testFetchWithPriority()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		assertNotNull(task.Id);
		assertEquals(processInstance.Id, task.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
		assertEquals("externalTaskWithPrio", task.ActivityId);
		assertEquals("twoExternalTaskWithPriorityProcess", task.ProcessDefinitionKey);
		assertEquals(TOPIC_NAME, task.TopicName);
		assertEquals(7, task.Priority);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("externalTaskWithPrio")[0];

		assertEquals(activityInstance.Id, task.ActivityInstanceId);
		assertEquals(activityInstance.ExecutionIds[0], task.ExecutionId);

		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

		assertEquals(WORKER_ID, task.WorkerId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml")]
	  public virtual void testFetchProcessWithPriority()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(2, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(2, externalTasks.Count);

		// then
		//task with no prio gets prio defined by process
		assertEquals(9, externalTasks[0].Priority);
		//task with own prio overrides prio defined by process
		assertEquals(7, externalTasks[1].Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpressionProcess.bpmn20.xml")]
	  public virtual void testFetchProcessWithPriorityExpression()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", 18));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(2, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(2, externalTasks.Count);

		// then
		//task with no prio gets prio defined by process
		assertEquals(18, externalTasks[0].Priority);
		//task with own prio overrides prio defined by process
		assertEquals(7, externalTasks[1].Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testFetchWithPriorityExpression()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", 18));
		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		assertNotNull(task.Id);
		assertEquals(processInstance.Id, task.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, task.ProcessDefinitionId);
		assertEquals("externalTaskWithPrio", task.ActivityId);
		assertEquals("twoExternalTaskWithPriorityProcess", task.ProcessDefinitionKey);
		assertEquals(TOPIC_NAME, task.TopicName);
		assertEquals(18, task.Priority);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id).getActivityInstances("externalTaskWithPrio")[0];

		assertEquals(activityInstance.Id, task.ActivityInstanceId);
		assertEquals(activityInstance.ExecutionIds[0], task.ExecutionId);

		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

		assertEquals(WORKER_ID, task.WorkerId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml")]
	  public virtual void testFetchWithPriorityOrdering()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(2, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(2, externalTasks.Count);
		assertTrue(externalTasks[0].Priority > externalTasks[1].Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml")]
	  public virtual void testFetchNextWithPriority()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then the task is locked
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		long firstPrio = task.Priority;
		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

		// another task with next higher priority can be claimed
		externalTasks = externalTaskService.fetchAndLock(1, "anotherWorkerId", true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);
		assertTrue(firstPrio >= externalTasks[0].Priority);

		// the expiration time expires
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		//first can be claimed
		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);
		assertEquals(firstPrio, externalTasks[0].Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFetchTopicSelection()
	  public virtual void testFetchTopicSelection()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoTopicsProcess");

		// when
		IList<LockedExternalTask> topic1Tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic1", LOCK_TIME).execute();

		IList<LockedExternalTask> topic2Tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic2", LOCK_TIME).execute();

		// then
		assertEquals(1, topic1Tasks.Count);
		assertEquals("topic1", topic1Tasks[0].TopicName);

		assertEquals(1, topic2Tasks.Count);
		assertEquals("topic2", topic2Tasks[0].TopicName);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchWithoutTopicName()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		try
		{
		  externalTaskService.fetchAndLock(1, WORKER_ID).topic(null, LOCK_TIME).execute();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("topicName is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchNullWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		try
		{
		  externalTaskService.fetchAndLock(1, null).topic(TOPIC_NAME, LOCK_TIME).execute();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("workerId is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchNegativeNumberOfTasks()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		try
		{
		  externalTaskService.fetchAndLock(-1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("maxResults is not greater than or equal to 0", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchLessTasksThanExist()
	  {
		// given
		for (int i = 0; i < 10; i++)
		{
		  runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		}

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(5, externalTasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchNegativeLockTime()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		try
		{
		  externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, -1L).execute();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("lockTime is not greater than 0", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchZeroLockTime()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		try
		{
		  externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 0L).execute();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("lockTime is not greater than 0", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchNoTopics()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFetchVariables()
	  public virtual void testFetchVariables()
	  {
		// given
		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", 42).putValue("processVar2", 43));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables("processVar1", "subProcessVar", "taskVar").execute();

		// then
		LockedExternalTask task = externalTasks[0];
		VariableMap variables = task.Variables;
		assertEquals(3, variables.size());

		assertEquals(42, variables.get("processVar1"));
		assertEquals(44L, variables.get("subProcessVar"));
		assertEquals(45L, variables.get("taskVar"));

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testFetchVariables.bpmn20.xml")]
	  public virtual void testShouldNotFetchSerializedVariables()
	  {
		// given
		ExternalTaskCustomValue customValue = new ExternalTaskCustomValue();
		customValue.TestValue = "value1";
		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", customValue));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables("processVar1").execute();

		// then
		LockedExternalTask task = externalTasks[0];
		VariableMap variables = task.Variables;
		assertEquals(1, variables.size());

		try
		{
		  variables.get("processVar1");
		  fail("did not receive an exception although variable was serialized");
		}
		catch (System.InvalidOperationException e)
		{
		  assertEquals("Object is not deserialized.", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testFetchVariables.bpmn20.xml")]
	  public virtual void testFetchSerializedVariables()
	  {
		// given
		ExternalTaskCustomValue customValue = new ExternalTaskCustomValue();
		customValue.TestValue = "value1";
		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", customValue));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables("processVar1").enableCustomObjectDeserialization().execute();

		// then
		LockedExternalTask task = externalTasks[0];
		VariableMap variables = task.Variables;
		assertEquals(1, variables.size());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExternalTaskCustomValue receivedCustomValue = (ExternalTaskCustomValue) variables.get("processVar1");
		ExternalTaskCustomValue receivedCustomValue = (ExternalTaskCustomValue) variables.get("processVar1");
		assertNotNull(receivedCustomValue);
		assertNotNull(receivedCustomValue.TestValue);
		assertEquals("value1", receivedCustomValue.TestValue);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskVariablesTest.testExternalTaskVariablesLocal.bpmn20.xml" })]
	  public virtual void testFetchOnlyLocalVariables()
	  {

		VariableMap globalVars = Variables.putValue("globalVar", "globalVal");

		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess", globalVars);

		const string workerId = "workerId";
		const string topicName = "testTopic";

		IList<LockedExternalTask> lockedExternalTasks = externalTaskService.fetchAndLock(10, workerId).topic(topicName, 60000).execute();

		assertEquals(1, lockedExternalTasks.Count);

		LockedExternalTask lockedExternalTask = lockedExternalTasks[0];
		VariableMap variables = lockedExternalTask.Variables;
		assertEquals(2, variables.size());
		assertEquals("globalVal", variables.getValue("globalVar", typeof(string)));
		assertEquals("localVal", variables.getValue("localVar", typeof(string)));

		externalTaskService.unlock(lockedExternalTask.Id);

		lockedExternalTasks = externalTaskService.fetchAndLock(10, workerId).topic(topicName, 60000).variables("globalVar", "localVar").localVariables().execute();

		assertEquals(1, lockedExternalTasks.Count);

		lockedExternalTask = lockedExternalTasks[0];
		variables = lockedExternalTask.Variables;
		assertEquals(1, variables.size());
		assertEquals("localVal", variables.getValue("localVar", typeof(string)));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskVariablesTest.testExternalTaskVariablesLocal.bpmn20.xml" })]
	  public virtual void testFetchNonExistingLocalVariables()
	  {

		VariableMap globalVars = Variables.putValue("globalVar", "globalVal");

		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess", globalVars);

		const string workerId = "workerId";
		const string topicName = "testTopic";

		IList<LockedExternalTask> lockedExternalTasks = externalTaskService.fetchAndLock(10, workerId).topic(topicName, 60000).variables("globalVar", "nonExistingLocalVar").localVariables().execute();

		assertEquals(1, lockedExternalTasks.Count);

		LockedExternalTask lockedExternalTask = lockedExternalTasks[0];
		VariableMap variables = lockedExternalTask.Variables;
		assertEquals(0, variables.size());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testFetchVariables.bpmn20.xml")]
	  public virtual void testFetchAllVariables()
	  {
		// given
		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", 42).putValue("processVar2", 43));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		LockedExternalTask task = externalTasks[0];
		verifyVariables(task);

		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", 42).putValue("processVar2", 43));

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables((string[]) null).execute();

		task = externalTasks[0];
		verifyVariables(task);

		runtimeService.startProcessInstanceByKey("subProcessExternalTask", Variables.createVariables().putValue("processVar1", 42).putValue("processVar2", 43));

		IList<string> list = null;
		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables(list).execute();

		task = externalTasks[0];
		verifyVariables(task);
	  }

	  private void verifyVariables(LockedExternalTask task)
	  {
		VariableMap variables = task.Variables;
		assertEquals(4, variables.size());

		assertEquals(42, variables.get("processVar1"));
		assertEquals(43, variables.get("processVar2"));
		assertEquals(44L, variables.get("subProcessVar"));
		assertEquals(45L, variables.get("taskVar"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchNonExistingVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).variables("nonExistingVariable").execute();

		LockedExternalTask task = tasks[0];

		// then
		assertTrue(task.Variables.Empty);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFetchMultipleTopics()
	  public virtual void testFetchMultipleTopics()
	  {
		// given a process instance with external tasks for topics "topic1", "topic2", and "topic3"
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess");

		// when fetching tasks for two topics
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic1", LOCK_TIME).topic("topic2", LOCK_TIME * 2).execute();

		// then those two tasks are locked
		assertEquals(2, tasks.Count);
		LockedExternalTask topic1Task = "topic1".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];
		LockedExternalTask topic2Task = "topic2".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];

		assertEquals("topic1", topic1Task.TopicName);
		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), topic1Task.LockExpirationTime);

		assertEquals("topic2", topic2Task.TopicName);
		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME * 2), topic2Task.LockExpirationTime);

		// and the third task can still be fetched
		tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic1", LOCK_TIME).topic("topic2", LOCK_TIME * 2).topic("topic3", LOCK_TIME * 3).execute();

		assertEquals(1, tasks.Count);

		LockedExternalTask topic3Task = tasks[0];
		assertEquals("topic3", topic3Task.TopicName);
		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME * 3), topic3Task.LockExpirationTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFetchMultipleTopicsWithVariables()
	  public virtual void testFetchMultipleTopicsWithVariables()
	  {
		// given a process instance with external tasks for topics "topic1" and "topic2"
		// both have local variables "var1" and "var2"
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", Variables.createVariables().putValue("var1", 0).putValue("var2", 0));

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic1", LOCK_TIME).variables("var1", "var2").topic("topic2", LOCK_TIME).variables("var1").execute();

		LockedExternalTask topic1Task = "topic1".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];
		LockedExternalTask topic2Task = "topic2".Equals(tasks[0].TopicName) ? tasks[0] : tasks[1];

		assertEquals("topic1", topic1Task.TopicName);
		assertEquals("topic2", topic2Task.TopicName);

		// then the correct variables have been fetched
		VariableMap topic1Variables = topic1Task.Variables;
		assertEquals(2, topic1Variables.size());
		assertEquals(1L, topic1Variables.get("var1"));
		assertEquals(1L, topic1Variables.get("var2"));

		VariableMap topic2Variables = topic2Task.Variables;
		assertEquals(1, topic2Variables.size());
		assertEquals(2L, topic2Variables.get("var1"));

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testFetchMultipleTopics.bpmn20.xml")]
	  public virtual void testFetchMultipleTopicsMaxTasks()
	  {
		// given
		for (int i = 0; i < 10; i++)
		{
		  runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess");
		}

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("topic1", LOCK_TIME).topic("topic2", LOCK_TIME).topic("topic3", LOCK_TIME).execute();

		// then 5 tasks were returned in total, not per topic
		assertEquals(5, tasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchSuspendedTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when suspending the process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// then the external task cannot be fetched
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(0, externalTasks.Count);

		// when activating the process instance
		runtimeService.activateProcessInstanceById(processInstance.Id);

		// then the task can be fetched
		externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(1, externalTasks.Count);
	  }

	  /// <summary>
	  /// Note: this does not test a hard API guarantee, i.e. the test is stricter than the API (Javadoc).
	  /// Its purpose is to ensure that the API implementation is less error-prone to use.
	  /// 
	  /// Bottom line: if there is good reason to change behavior such that this test breaks, it may
	  /// be ok to change the test.
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchAndLockWithInitialBuilder()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		ExternalTaskQueryBuilder initialBuilder = externalTaskService.fetchAndLock(1, WORKER_ID);
		initialBuilder.topic(TOPIC_NAME, LOCK_TIME);

		// should execute regardless whether the initial builder is used or the builder returned by the
		// #topic invocation
		IList<LockedExternalTask> tasks = initialBuilder.execute();

		// then
		assertEquals(1, tasks.Count);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
	  public virtual void testFetchByProcessDefinitionId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");
		string processDefinitionId2 = processInstance2.ProcessDefinitionId;

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).processDefinitionId(processDefinitionId2).execute();

		// then
		assertEquals(1, externalTasks.Count);
		assertEquals(processDefinitionId2, externalTasks[0].ProcessDefinitionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchByProcessDefinitionIdCombination()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);
		string processDefinitionId2 = processInstance2.ProcessDefinitionId;


	  //when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).topic(topicName2, lockDuration.Value).processDefinitionId(processDefinitionId2).execute();

		//then
		assertEquals(3, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(pi.ProcessDefinitionId, externalTask.ProcessDefinitionId);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(processDefinitionId2, pi.ProcessDefinitionId);
			assertEquals(processDefinitionId2, externalTask.ProcessDefinitionId);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchByProcessDefinitionIdIn()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";
		string topicName3 = "topic3";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";
		string businessKey3 = "testBusinessKey3";

		long? lockDuration = 60L * 1000L;

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		string processDefinitionId1 = processInstance1.ProcessDefinitionId;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);
		string processDefinitionId2 = processInstance2.ProcessDefinitionId;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey3);
		string processDefinitionId3 = processInstance3.ProcessDefinitionId;

		// when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(2, "externalWorkerId").topic(topicName1, lockDuration.Value).processDefinitionIdIn(processDefinitionId1).processDefinitionKey("parallelExternalTaskProcess").topic(topicName2, lockDuration.Value).processDefinitionId(processDefinitionId2).businessKey(businessKey2).topic(topicName3, lockDuration.Value).processDefinitionId(processDefinitionId3).processDefinitionKeyIn("unexisting").execute();

		// then
		assertEquals(2, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(processDefinitionId1, pi.ProcessDefinitionId);
			assertEquals(processDefinitionId1, externalTask.ProcessDefinitionId);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(processDefinitionId2, pi.ProcessDefinitionId);
			assertEquals(processDefinitionId2, externalTask.ProcessDefinitionId);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
	  public virtual void testFetchByProcessDefinitionIds()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		string processDefinitionId1 = processInstance1.ProcessDefinitionId;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");
		string processDefinitionId2 = processInstance2.ProcessDefinitionId;

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).processDefinitionId(processDefinitionId2).processDefinitionIdIn(processDefinitionId1).execute();

		// then
		assertEquals(0, externalTasks.Count);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
	  public virtual void testFetchByProcessDefinitionKey()
	  {
		// given
		string processDefinitionKey1 = "oneExternalTaskProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey1);
		string processDefinitionKey2 = "twoExternalTaskWithPriorityProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey2);

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).processDefinitionKey(processDefinitionKey2).execute();

		// then
		assertEquals(1, externalTasks.Count);
		assertEquals(processDefinitionKey2, externalTasks[0].ProcessDefinitionKey);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
	  public virtual void testFetchByProcessDefinitionKeyIn()
	  {
		// given
		string processDefinitionKey1 = "oneExternalTaskProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey1);
		string processDefinitionKey2 = "twoExternalTaskWithPriorityProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey2);

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).processDefinitionKeyIn(processDefinitionKey2).execute();

		// then
		assertEquals(1, externalTasks.Count);
		assertEquals(processDefinitionKey2, externalTasks[0].ProcessDefinitionKey);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityProcess.bpmn20.xml" })]
	  public virtual void testFetchByProcessDefinitionKeys()
	  {
		// given
		string processDefinitionKey1 = "oneExternalTaskProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey1);
		string processDefinitionKey2 = "twoExternalTaskWithPriorityProcess";
		runtimeService.startProcessInstanceByKey(processDefinitionKey2);

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).processDefinitionKey(processDefinitionKey1).processDefinitionKeyIn(processDefinitionKey2).execute();

		// then
		assertEquals(0, externalTasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchByProcessDefinitionIdAndKey()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";
		string businessKey3 = "testBusinessKey3";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);
		string processDefinitionId2 = processInstance2.ProcessDefinitionId;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey3);
		string processDefinitionId3 = processInstance3.ProcessDefinitionId;

		//when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).topic(topicName2, lockDuration.Value).processDefinitionIdIn(processDefinitionId2, processDefinitionId3).topic("topic3", lockDuration).processDefinitionIdIn("unexisting").execute();

		//then
		assertEquals(3, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(pi.ProcessDefinitionId, externalTask.ProcessDefinitionId);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(processDefinitionId2, pi.ProcessDefinitionId);
			assertEquals(processDefinitionId2, externalTask.ProcessDefinitionId);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testFetchWithoutTenant()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).withoutTenantId().execute();

		// then
		assertEquals(1, externalTasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testComplete()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		externalTaskService.complete(externalTasks[0].Id, WORKER_ID);

		// then
		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("afterExternalTask").done());
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteWithVariables()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		externalTaskService.complete(externalTasks[0].Id, WORKER_ID, Variables.createVariables().putValue("var", 42));

		// then
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("afterExternalTask").done());

		assertEquals(42, runtimeService.getVariable(processInstance.Id, "var"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteWithWrongWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then it is not possible to complete the task with a different worker id
		try
		{
		  externalTaskService.complete(externalTasks[0].Id, "someCrazyWorkerId");
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent("cannot be completed by worker 'someCrazyWorkerId'. It is locked by worker '" + WORKER_ID + "'.", e.Message);
		}
	  }

	  public virtual void testCompleteNonExistingTask()
	  {
		try
		{
		  externalTaskService.complete("nonExistingTaskId", WORKER_ID);
		  fail("exception expected");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
		}
	  }

	  public virtual void testCompleteNullTaskId()
	  {
		try
		{
		  externalTaskService.complete(null, WORKER_ID);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find external task with id " + null, e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteNullWorkerId()
	  {
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		try
		{
		  externalTaskService.complete(task.Id, null);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("workerId is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteSuspendedTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = externalTasks[0];

		// when suspending the process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// then the external task cannot be completed
		try
		{
		  externalTaskService.complete(task.Id, WORKER_ID);
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ExternalTask with id '" + task.Id + "' is suspended", e.Message);
		}

		assertProcessNotEnded(processInstance.Id);

		// when activating the process instance again
		runtimeService.activateProcessInstanceById(processInstance.Id);

		// then the task can be completed
		externalTaskService.complete(task.Id, WORKER_ID);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testLocking()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then the task is locked
		assertEquals(1, externalTasks.Count);

		LockedExternalTask task = externalTasks[0];
		AssertUtil.assertEqualsSecondPrecision(nowPlus(LOCK_TIME), task.LockExpirationTime);

		// and cannot be retrieved by another query
		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);

		// unless the expiration time expires
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteLockExpiredTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// then the task can successfully be completed
		externalTaskService.complete(externalTasks[0].Id, WORKER_ID);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCompleteReclaimedLockExpiredTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// and it is reclaimed by another worker
		IList<LockedExternalTask> reclaimedTasks = externalTaskService.fetchAndLock(1, "anotherWorkerId").topic(TOPIC_NAME, LOCK_TIME).execute();

		// then the first worker cannot complete the task
		try
		{
		  externalTaskService.complete(externalTasks[0].Id, WORKER_ID);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("cannot be completed by worker '" + WORKER_ID + "'. It is locked by worker 'anotherWorkerId'.", e.Message);
		}

		// and the second worker can
		externalTaskService.complete(reclaimedTasks[0].Id, "anotherWorkerId");

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testDeleteProcessInstance()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		assertEquals(0, externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute().Count);
		assertProcessEnded(processInstance.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExternalTaskExecutionTreeExpansion()
	  public virtual void testExternalTaskExecutionTreeExpansion()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("boundaryExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		LockedExternalTask externalTask = tasks[0];

		// when a non-interrupting boundary event is triggered meanwhile
		// such that the execution tree is expanded
		runtimeService.correlateMessage("Message");

		// then the external task can still be completed
		externalTaskService.complete(externalTask.Id, WORKER_ID);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("afterBoundaryTask").done());

		Task afterBoundaryTask = taskService.createTaskQuery().singleResult();
		taskService.complete(afterBoundaryTask.Id);

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExternalTaskExecutionTreeCompaction()
	  public virtual void testExternalTaskExecutionTreeCompaction()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("concurrentExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		LockedExternalTask externalTask = tasks[0];

		Task userTask = taskService.createTaskQuery().singleResult();

		// when the user task completes meanwhile, thereby trigger execution tree compaction
		taskService.complete(userTask.Id);

		// then the external task can still be completed
		externalTaskService.complete(externalTask.Id, WORKER_ID);

		tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, tasks.Count);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUnlock()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = externalTasks[0];

		// when unlocking the task
		externalTaskService.unlock(task.Id);

		// then it can be acquired again
		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(1, externalTasks.Count);
		LockedExternalTask reAcquiredTask = externalTasks[0];
		assertEquals(task.Id, reAcquiredTask.Id);
	  }

	  public virtual void testUnlockNullTaskId()
	  {
		try
		{
		  externalTaskService.unlock(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, containsString("externalTaskId is null"));
		}
	  }

	  public virtual void testUnlockNonExistingTask()
	  {
		try
		{
		  externalTaskService.unlock("nonExistingId");
		  fail("expected exception");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("Cannot find external task with id nonExistingId", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailure()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		// when submitting a failure (after a simulated processing time of three seconds)
		ClockUtil.CurrentTime = nowPlus(3000L);

		string errorMessage = "errorMessage";
		externalTaskService.handleFailure(task.Id, WORKER_ID, errorMessage, 5, 3000L);

		// then the task cannot be immediately acquired again
		tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, tasks.Count);

		// and no incident exists because there are still retries left
		assertEquals(0, runtimeService.createIncidentQuery().count());

		// but when the retry time expires, the task is available again
		ClockUtil.CurrentTime = nowPlus(4000L);

		tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, tasks.Count);

		// and the retries and error message are accessible
		task = tasks[0];
		assertEquals(errorMessage, task.ErrorMessage);
		assertEquals(5, (int) task.Retries);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureWithErrorDetails()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		// when submitting a failure (after a simulated processing time of three seconds)
		ClockUtil.CurrentTime = nowPlus(3000L);

		string errorMessage;
		string exceptionStackTrace;
		try
		{
		  RuntimeSqlException cause = new RuntimeSqlException("test cause");
		  for (int i = 0; i < 10; i++)
		  {
			cause = new RuntimeSqlException(cause);
		  }
		  throw cause;
		}
		catch (Exception e)
		{
		  exceptionStackTrace = ExceptionUtils.getStackTrace(e);
		  errorMessage = e.Message;
		  while (errorMessage.Length < 1000)
		  {
			errorMessage = errorMessage + ":" + e.Message;
		  }
		}
		Assert.assertThat(exceptionStackTrace,@is(notNullValue()));
	//  make sure that stack trace is longer then errorMessage DB field length
		Assert.assertThat(exceptionStackTrace.Length,@is(greaterThan(4000)));
		externalTaskService.handleFailure(task.Id, WORKER_ID, errorMessage, exceptionStackTrace, 5, 3000L);
		ClockUtil.CurrentTime = nowPlus(4000L);
		tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		Assert.assertThat(tasks.Count, @is(1));

		// verify that exception is accessible properly
		task = tasks[0];
		Assert.assertThat(task.ErrorMessage,@is(errorMessage.Substring(0,666)));
		Assert.assertThat(task.Retries,@is(5));
		Assert.assertThat(externalTaskService.getExternalTaskErrorDetails(task.Id),@is(exceptionStackTrace));
		Assert.assertThat(task.ErrorDetails,@is(exceptionStackTrace));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureZeroRetries()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		// when reporting a failure and setting retries to 0
		ClockUtil.CurrentTime = nowPlus(3000L);

		string errorMessage = "errorMessage";
		externalTaskService.handleFailure(task.Id, WORKER_ID, errorMessage, 0, 3000L);

		// then the task cannot be fetched anymore even when the lock expires
		ClockUtil.CurrentTime = nowPlus(4000L);

		tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, tasks.Count);

		// and an incident has been created
		Incident incident = runtimeService.createIncidentQuery().singleResult();

		assertNotNull(incident);
		assertNotNull(incident.Id);
		assertEquals(errorMessage, incident.IncidentMessage);
		assertEquals(task.ExecutionId, incident.ExecutionId);
		assertEquals("externalTask", incident.ActivityId);
		assertEquals(incident.Id, incident.CauseIncidentId);
		assertEquals("failedExternalTask", incident.IncidentType);
		assertEquals(task.ProcessDefinitionId, incident.ProcessDefinitionId);
		assertEquals(task.ProcessInstanceId, incident.ProcessInstanceId);
		assertEquals(incident.Id, incident.RootCauseIncidentId);
		AssertUtil.assertEqualsSecondPrecision(nowMinus(4000L), incident.IncidentTimestamp);
		assertEquals(task.Id, incident.Configuration);
		assertNull(incident.JobDefinitionId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureAndDeleteProcessInstance()
	  {
		// given a failed external task with incident
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		externalTaskService.handleFailure(task.Id, WORKER_ID, "someError", 0, LOCK_TIME);

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		assertProcessEnded(processInstance.Id);
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureThenComplete()
	  {
		// given a failed external task with incident
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		externalTaskService.handleFailure(task.Id, WORKER_ID, "someError", 0, LOCK_TIME);

		// when
		externalTaskService.complete(task.Id, WORKER_ID);

		// then the task has been completed nonetheless
		Task followingTask = taskService.createTaskQuery().singleResult();
		assertNotNull(followingTask);
		assertEquals("afterExternalTask", followingTask.TaskDefinitionKey);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureWithWrongWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then it is not possible to complete the task with a different worker id
		try
		{
		  externalTaskService.handleFailure(externalTasks[0].Id, "someCrazyWorkerId", "error", 5, LOCK_TIME);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent("Failure of External Task " + externalTasks[0].Id + " cannot be reported by worker 'someCrazyWorkerId'. It is locked by worker '" + WORKER_ID + "'.", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNonExistingTask()
	  {
		try
		{
		  externalTaskService.handleFailure("nonExistingTaskId", WORKER_ID, "error", 5, LOCK_TIME);
		  fail("exception expected");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNullTaskId()
	  {
		try
		{
		  externalTaskService.handleFailure(null, WORKER_ID, "error", 5, LOCK_TIME);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find external task with id " + null, e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNullWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		try
		{
		  externalTaskService.handleFailure(externalTasks[0].Id, null, "error", 5, LOCK_TIME);
		  fail("exception expected");
		}
		catch (NullValueException e)
		{
		  assertTextPresent("workerId is null", e.Message);
		}

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNegativeLockDuration()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		try
		{
		  externalTaskService.handleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, - LOCK_TIME);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("retryDuration is not greater than or equal to 0", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNegativeRetries()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		try
		{
		  externalTaskService.handleFailure(externalTasks[0].Id, WORKER_ID, "error", -5, LOCK_TIME);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("retries is not greater than or equal to 0", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureNullErrorMessage()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// when
		externalTaskService.handleFailure(externalTasks[0].Id, WORKER_ID, null, 5, LOCK_TIME);

		// then the failure was reported successfully and the error message is null
		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();

		assertEquals(5, (int) task.Retries);
		assertNull(task.ErrorMessage);
		assertNull(externalTaskService.getExternalTaskErrorDetails(task.Id));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleFailureSuspendedTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = externalTasks[0];

		// when suspending the process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// then a failure cannot be reported
		try
		{
		  externalTaskService.handleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, LOCK_TIME);
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ExternalTask with id '" + task.Id + "' is suspended", e.Message);
		}

		assertProcessNotEnded(processInstance.Id);

		// when activating the process instance again
		runtimeService.activateProcessInstanceById(processInstance.Id);

		// then the failure can be reported successfully
		externalTaskService.handleFailure(externalTasks[0].Id, WORKER_ID, "error", 5, LOCK_TIME);

		ExternalTask updatedTask = externalTaskService.createExternalTaskQuery().singleResult();
		assertEquals(5, (int) updatedTask.Retries);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetries()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// when
		externalTaskService.setRetries(externalTasks[0].Id, 5);

		// then
		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();

		assertEquals(5, (int) task.Retries);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesResolvesFailureIncident()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask lockedTask = externalTasks[0];
		externalTaskService.handleFailure(lockedTask.Id, WORKER_ID, "error", 0, LOCK_TIME);

		Incident incident = runtimeService.createIncidentQuery().singleResult();

		// when
		externalTaskService.setRetries(lockedTask.Id, 5);

		// then the incident is resolved
		assertEquals(0, runtimeService.createIncidentQuery().count());

		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Id)
		{

		  HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		  assertNotNull(historicIncident);
		  assertEquals(incident.Id, historicIncident.Id);
		  assertTrue(historicIncident.Resolved);
		}

		// and the task can be fetched again
		ClockUtil.CurrentTime = nowPlus(LOCK_TIME + 3000L);

		externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(1, externalTasks.Count);
		assertEquals(lockedTask.Id, externalTasks[0].Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesToZero()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask lockedTask = externalTasks[0];

		// when
		externalTaskService.setRetries(lockedTask.Id, 0);

		// then
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);
		assertEquals(lockedTask.Id, incident.Configuration);

		// and resetting the retries removes the incident again
		externalTaskService.setRetries(lockedTask.Id, 5);

		assertEquals(0, runtimeService.createIncidentQuery().count());

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesNegative()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		try
		{
		  // when
		  externalTaskService.setRetries(externalTasks[0].Id, -5);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("retries is not greater than or equal to 0", e.Message);
		}
	  }

	  public virtual void testSetRetriesNonExistingTask()
	  {
		try
		{
		  externalTaskService.setRetries("someExternalTaskId", 5);
		  fail("expected exception");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("externalTask is null", e.Message);
		}
	  }

	  public virtual void testSetRetriesNullTaskId()
	  {
		try
		{
		  externalTaskService.setRetries((string)null, 5);
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, containsString("externalTaskId is null"));
		}
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetPriority()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// when
		externalTaskService.setPriority(externalTasks[0].Id, 5);

		// then
		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();

		assertEquals(5, (int) task.Priority);
	  }


	  public virtual void testSetPriorityNonExistingTask()
	  {
		try
		{
		  externalTaskService.setPriority("someExternalTaskId", 5);
		  fail("expected exception");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("externalTask is null", e.Message);
		}
	  }

	  public virtual void testSetPriorityNullTaskId()
	  {
		try
		{
		  externalTaskService.setPriority(null, 5);
		  fail("expected exception");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, containsString("externalTaskId is null"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml")]
	  public virtual void testAfterSetPriorityFetchHigherTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(2, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(2, externalTasks.Count);
		LockedExternalTask task = externalTasks[1];
		assertEquals(0, task.Priority);
		externalTaskService.setPriority(task.Id, 9);
		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// then
		externalTasks = externalTaskService.fetchAndLock(1, "anotherWorkerId", true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(externalTasks[0].Priority, 9);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetPriorityLockExpiredTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// then the priority can be set
		externalTaskService.setPriority(externalTasks[0].Id, 9);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID, true).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);
		assertEquals(externalTasks[0].Priority, 9);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelExternalTaskWithBoundaryEvent()
	  public virtual void testCancelExternalTaskWithBoundaryEvent()
	  {
		// given
		runtimeService.startProcessInstanceByKey("boundaryExternalTaskProcess");
		assertEquals(1, externalTaskService.createExternalTaskQuery().count());

		// when the external task is cancelled by a boundary event
		runtimeService.correlateMessage("Message");

		// then the external task instance has been removed
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());

		Task afterBoundaryTask = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBoundaryTask);
		assertEquals("afterBoundaryTask", afterBoundaryTask.TaskDefinitionKey);

	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnError()
	  {
		//given
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");
		// when
		IList<LockedExternalTask> externalTasks = helperHandleBpmnError(1, WORKER_ID, TOPIC_NAME, LOCK_TIME, "ERROR-OCCURED");
		//then
		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBpmnError);
		assertEquals(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorWithoutDefinedBoundary()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		//when
		IList<LockedExternalTask> externalTasks = helperHandleBpmnError(1, WORKER_ID, TOPIC_NAME, LOCK_TIME, "ERROR-OCCURED");

		//then
		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNull(afterBpmnError);
		assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// Helper method to handle a bmpn error on an external task, which is fetched with the given parameters.
	  /// </summary>
	  /// <param name="taskCount"> the count of task to fetch </param>
	  /// <param name="workerID"> the worker id </param>
	  /// <param name="topicName"> the topic name of the external task </param>
	  /// <param name="lockTime"> the lock time for the fetch </param>
	  /// <param name="errorCode"> the error code of the bpmn error </param>
	  /// <returns> returns the locked external tasks after the bpmn error was handled </returns>
	  public virtual IList<LockedExternalTask> helperHandleBpmnError(int taskCount, string workerID, string topicName, long lockTime, string errorCode)
	  {
		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(taskCount, workerID).topic(topicName, lockTime).execute();

		externalTaskService.handleBpmnError(externalTasks[0].Id, workerID, errorCode);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		return externalTasks;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorLockExpiredTask()
	  {
		//given
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");
		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		externalTaskService.handleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-OCCURED");

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBpmnError);
		assertEquals(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorReclaimedLockExpiredTaskWithoutDefinedBoundary()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		handleBpmnErrorReclaimedLockExpiredTask(false);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorReclaimedLockExpiredTaskWithBoundary()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");
		//then
		handleBpmnErrorReclaimedLockExpiredTask(false);
	  }

	  /// <summary>
	  /// Helpher method which reclaims an external task after the lock is expired. </summary>
	  /// <param name="includeVariables"> flag showing if pass or not variables </param>
	  public virtual void handleBpmnErrorReclaimedLockExpiredTask(bool includeVariables)
	  {
		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// and it is reclaimed by another worker
		IList<LockedExternalTask> reclaimedTasks = externalTaskService.fetchAndLock(1, "anotherWorkerId").topic(TOPIC_NAME, LOCK_TIME).execute();

		// then the first worker cannot complete the task
		try
		{
		  externalTaskService.handleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-OCCURED");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Bpmn error of External Task " + externalTasks[0].Id + " cannot be reported by worker '" + WORKER_ID + "'. It is locked by worker 'anotherWorkerId'.", e.Message);
		  if (includeVariables)
		  {
			IList<VariableInstance> list = runtimeService.createVariableInstanceQuery().list();
			assertEquals(0, list.Count);
		  }
		}

		// and the second worker can
		externalTaskService.complete(reclaimedTasks[0].Id, "anotherWorkerId");

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);
	  }

	  public virtual void testHandleBpmnErrorNonExistingTask()
	  {
		try
		{
		  externalTaskService.handleBpmnError("nonExistingTaskId", WORKER_ID, "ERROR-OCCURED");
		  fail("exception expected");
		}
		catch (NotFoundException e)
		{
		  // not found exception lets client distinguish this from other failures
		  assertTextPresent("Cannot find external task with id nonExistingTaskId", e.Message);
		}
	  }

	  public virtual void testHandleBpmnNullTaskId()
	  {
		try
		{
		  externalTaskService.handleBpmnError(null, WORKER_ID, "ERROR-OCCURED");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find external task with id " + null, e.Message);
		}
	  }


	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnNullErrorCode()
	  {
		//given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		//when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		//then
		LockedExternalTask task = tasks[0];
		try
		{
		  externalTaskService.handleBpmnError(task.Id, WORKER_ID, null);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("errorCode is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorNullWorkerId()
	  {
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		try
		{
		  externalTaskService.handleBpmnError(task.Id, null,"ERROR-OCCURED");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("workerId is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorSuspendedTask()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = externalTasks[0];

		// when suspending the process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// then the external task cannot be completed
		try
		{
		  externalTaskService.handleBpmnError(task.Id, WORKER_ID, "ERROR-OCCURED");
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ExternalTask with id '" + task.Id + "' is suspended", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorPassVariablesBoundryEvent()
	  {
		//given
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		variables["transientVar"] = Variables.integerValue(1, true);

		// when
		externalTaskService.handleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-OCCURED", null, variables);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBpmnError);
		assertEquals(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
		IList<VariableInstance> list = runtimeService.createVariableInstanceQuery().processInstanceIdIn(pi.Id).list();
		assertEquals(1, list.Count);
		assertEquals("foo", list[0].Name);
	  }

	  public virtual void testHandleBpmnErrorPassVariablesEventSubProcess()
	  {
		// when
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent("startEvent").serviceTask("externalTask").camundaType("external").camundaTopic(TOPIC_NAME).endEvent("endEvent").done();

		BpmnModelInstance subProcess = modify(process).addSubProcessTo("process").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").error("ERROR-SPEC-10").userTask("afterBpmnError").endEvent().subProcessDone().done();

		BpmnModelInstance targetProcess = modify(subProcess);

		deploymentId = deployment(targetProcess);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		variables["transientVar"] = Variables.integerValue(1, true);

		// when
		externalTaskService.handleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-SPEC-10", null, variables);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBpmnError);
		assertEquals(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
		IList<VariableInstance> list = runtimeService.createVariableInstanceQuery().processInstanceIdIn(pi.Id).list();
		assertEquals(1, list.Count);
		assertEquals("foo", list[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHandleBpmnErrorPassMessageEventSubProcess()
	  public virtual void testHandleBpmnErrorPassMessageEventSubProcess()
	  {
		//given
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// and the lock expires without the task being reclaimed
		ClockUtil.CurrentTime = (new DateTime(ClockUtil.CurrentTime)).plus(LOCK_TIME * 2).toDate();

		// when
		string anErrorMessage = "Some meaningful message";
		externalTaskService.handleBpmnError(externalTasks[0].Id, WORKER_ID, "ERROR-SPEC-10", anErrorMessage);

		externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		// then
		assertEquals(0, externalTasks.Count);
		assertEquals(0, externalTaskService.createExternalTaskQuery().count());
		Task afterBpmnError = taskService.createTaskQuery().singleResult();
		assertNotNull(afterBpmnError);
		assertEquals(afterBpmnError.TaskDefinitionKey, "afterBpmnError");
		IList<VariableInstance> list = runtimeService.createVariableInstanceQuery().processInstanceIdIn(pi.Id).list();
		assertEquals(1, list.Count);
		assertEquals("errorMessageVariable", list[0].Name);
		assertEquals(anErrorMessage, list[0].Value);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml")]
	  public virtual void testHandleBpmnErrorReclaimedLockExpiredTaskWithBoundaryAndPassVariables()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");
		// then
		handleBpmnErrorReclaimedLockExpiredTask(true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByExternalTaskIds()
	  {
		// given
		startProcessInstance("oneExternalTaskProcess", 5);
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		IList<string> externalTaskIds = Arrays.asList(tasks[0].Id, tasks[1].Id, tasks[2].Id, tasks[3].Id, tasks[4].Id);

		// when
		externalTaskService.updateRetries().externalTaskIds(externalTaskIds).set(5);

		// then
		tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByExternalTaskIdArray()
	  {
		// given
		startProcessInstance("oneExternalTaskProcess", 5);
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		IList<string> externalTaskIds = Arrays.asList(tasks[0].Id, tasks[1].Id, tasks[2].Id, tasks[3].Id, tasks[4].Id);

		// when
		externalTaskService.updateRetries().externalTaskIds(externalTaskIds.ToArray()).set(5);

		// then
		tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByProcessInstanceIds()
	  {
		// given
		IList<string> processInstances = startProcessInstance("oneExternalTaskProcess", 5);

		// when
		externalTaskService.updateRetries().processInstanceIds(processInstances).set(5);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByProcessInstanceIdArray()
	  {
		// given
		IList<string> processInstances = startProcessInstance("oneExternalTaskProcess", 5);

		// when
		externalTaskService.updateRetries().processInstanceIds(processInstances.ToArray()).set(5);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByExternalTaskQuery()
	  {
		// given
		startProcessInstance("oneExternalTaskProcess", 5);

		ExternalTaskQuery query = externalTaskService.createExternalTaskQuery();

		// when
		externalTaskService.updateRetries().externalTaskQuery(query).set(5);

		// then
		IList<ExternalTask> tasks = query.list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByProcessInstanceQuery()
	  {
		// given
		startProcessInstance("oneExternalTaskProcess", 5);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneExternalTaskProcess");

		// when
		externalTaskService.updateRetries().processInstanceQuery(processInstanceQuery).set(5);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT), Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByHistoricProcessInstanceQuery()
	  {
		// given
		startProcessInstance("oneExternalTaskProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("oneExternalTaskProcess");

		// when
		externalTaskService.updateRetries().historicProcessInstanceQuery(query).set(5);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT), Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUpdateRetriesByAllParameters()
	  {
		// given
		IList<string> ids = startProcessInstance("oneExternalTaskProcess", 5);

		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().processInstanceId(ids[0]).singleResult();

		ExternalTaskQuery externalTaskQuery = externalTaskService.createExternalTaskQuery().processInstanceId(ids[1]);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(ids[2]);

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery().processInstanceId(ids[3]);

		// when
		externalTaskService.updateRetries().externalTaskIds(externalTask.Id).externalTaskQuery(externalTaskQuery).processInstanceQuery(processInstanceQuery).historicProcessInstanceQuery(historicProcessInstanceQuery).processInstanceIds(ids[4]).set(5);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(5, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(Convert.ToInt32(5), task.Retries);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTime()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date oldCurrentTime = org.camunda.bpm.engine.impl.util.ClockUtil.getCurrentTime();
		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		try
		{
		  // given
		  runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		  ClockUtil.CurrentTime = nowMinus(1000);
		  IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		  // when
		  DateTime extendLockTime = DateTime.Now;
		  ClockUtil.CurrentTime = extendLockTime;

		  externalTaskService.extendLock(lockedTasks[0].Id, WORKER_ID, LOCK_TIME);

		  // then
		  ExternalTask taskWithExtendedLock = externalTaskService.createExternalTaskQuery().locked().singleResult();
		  assertNotNull(taskWithExtendedLock);
		  AssertUtil.assertEqualsSecondPrecision(new DateTime(extendLockTime.Ticks + LOCK_TIME), taskWithExtendedLock.LockExpirationTime);

		}
		finally
		{
		  ClockUtil.CurrentTime = oldCurrentTime;
		}

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeThatExpired()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		ClockUtil.CurrentTime = nowPlus(2);
		// when
		try
		{
		  externalTaskService.extendLock(lockedTasks[0].Id, WORKER_ID, 100);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("Cannot extend a lock that expired"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithoutLock()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();
		// when
		try
		{
		  externalTaskService.extendLock(externalTask.Id, WORKER_ID, 100);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The lock of the External Task " + externalTask.Id + " cannot be extended by worker '" + WORKER_ID));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithNullLockTime()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		// when
		try
		{
		  externalTaskService.extendLock(lockedTasks[0].Id, WORKER_ID, 0);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("lockTime is not greater than 0"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithNegativeLockTime()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		// when
		try
		{
		  externalTaskService.extendLock(lockedTasks[0].Id, WORKER_ID, -1);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("lockTime is not greater than 0"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithNullWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		// when
		try
		{
		  externalTaskService.extendLock(lockedTasks[0].Id, null, 100);
		  fail("Exception expected");
		}
		catch (NullValueException e)
		{
		  // then
		  assertTrue(e.Message.contains("workerId is null"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithDifferentWorkerId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		LockedExternalTask task = lockedTasks[0];
		// when
		try
		{
		  externalTaskService.extendLock(task.Id,"anAnotherWorkerId", 100);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The lock of the External Task " + task.Id + " cannot be extended by worker 'anAnotherWorkerId'"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeWithNullExternalTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, 1L).execute();

		assertNotNull(lockedTasks);
		assertEquals(1, lockedTasks.Count);

		// when
		try
		{
		  externalTaskService.extendLock(null, WORKER_ID, 100);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("Cannot find external task with id null"));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testExtendLockTimeForUnexistingExternalTask()
	  {
		// when
		try
		{
		  externalTaskService.extendLock("unexisting", WORKER_ID, 100);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("Cannot find external task with id unexisting"));
		}
	  }

	  public virtual void testCompleteWithLocalVariables()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask("externalTask").camundaType("external").camundaTopic("foo").camundaTaskPriority("100").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ReadLocalVariableListenerImpl)).userTask("user").endEvent().done();

		deployment(instance);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic("foo", 1L).execute();

		// when
		externalTaskService.complete(lockedTasks[0].Id, WORKER_ID, null, Variables.createVariables().putValue("abc", "bar"));

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).singleResult();
		assertNull(variableInstance);
		if (processEngineConfiguration.HistoryLevel == org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT || processEngineConfiguration.HistoryLevel == org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL)
		{
		  HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().activityInstanceIdIn(lockedTasks[0].ActivityInstanceId).singleResult();
		  assertNotNull(historicVariableInstance);
		  assertEquals("abc", historicVariableInstance.Name);
		  assertEquals("bar", historicVariableInstance.Value);
		}
	  }

	  public virtual void testCompleteWithNonLocalVariables()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask("externalTask").camundaType("external").camundaTopic("foo").camundaTaskPriority("100").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ReadLocalVariableListenerImpl)).userTask("user").endEvent().done();

		deployment(instance);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		IList<LockedExternalTask> lockedTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic("foo", 1L).execute();

		// when
		externalTaskService.complete(lockedTasks[0].Id, WORKER_ID, Variables.createVariables().putValue("abc", "bar"), null);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).singleResult();
		assertNotNull(variableInstance);
		assertEquals("bar", variableInstance.Value);
		assertEquals("abc", variableInstance.Name);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testFetchWithEmptyListOfVariables()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).variables(new string[]{}).execute();

		// then
		assertEquals(1, tasks.Count);

		LockedExternalTask task = tasks[0];
		assertNotNull(task.Id);
		assertEquals(0, task.Variables.size());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByBusinessKey()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";
		string topicName3 = "topic3";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);

		//when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).businessKey(businessKey1).topic(topicName2, lockDuration.Value).businessKey(businessKey2).topic(topicName3, lockDuration.Value).businessKey("fakeBusinessKey").execute();

		//then
		assertEquals(2, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(businessKey1, pi.BusinessKey);
			assertEquals(businessKey1, externalTask.BusinessKey);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(businessKey2, pi.BusinessKey);
			assertEquals(businessKey2, externalTask.BusinessKey);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByBusinessKeyCombination1()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);

		//when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).businessKey(businessKey1).topic(topicName2, lockDuration.Value).execute();

		//then
		assertEquals(3, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(businessKey1, pi.BusinessKey);
			assertEquals(businessKey1, externalTask.BusinessKey);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(pi.BusinessKey, externalTask.BusinessKey);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByBusinessKeyCombination2()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);

		//when
		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).topic(topicName2, lockDuration.Value).businessKey(businessKey2).execute();

		//then
		assertEquals(3, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(pi.BusinessKey, externalTask.BusinessKey);
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(businessKey2, pi.BusinessKey);
			assertEquals(businessKey2, externalTask.BusinessKey);
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/parallelExternalTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByBusinessKeyLocking()
	  {
		// given
		string topicName1 = "topic1";
		string topicName2 = "topic2";
		string topicName3 = "topic3";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		long? lockDuration = 60L * 1000L;

		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcess", businessKey2);

		//when
		IList<LockedExternalTask> lockedTopicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).businessKey(businessKey1).topic(topicName2, lockDuration.Value).businessKey(businessKey2).topic(topicName3, lockDuration.Value).businessKey("fakeBusinessKey").execute();

		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).businessKey(businessKey1).topic(topicName2, 2 * lockDuration).businessKey(businessKey2).topic(topicName3, 2 * lockDuration).businessKey(businessKey1).execute();

		//then
		assertEquals(2, lockedTopicTasks.Count);
		assertEquals(1, topicTasks.Count);

		LockedExternalTask externalTask = topicTasks[0];
		ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();

		assertEquals(businessKey1, pi.BusinessKey);
		assertEquals(businessKey1, externalTask.BusinessKey);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testVariableValueTopicQuery.bpmn20.xml")]
	  public virtual void testTopicQueryByVariableValue()
	  {
		// given
		string topicName1 = "testTopic1";
		string topicName2 = "testTopic2";

		string variableName = "testVariable";
		string variableValue1 = "testValue1";
		string variableValue2 = "testValue2";

		IDictionary<string, object> variables = new Dictionary<string, object>();

		long? lockDuration = 60L * 1000L;

		//when
		variables[variableName] = variableValue1;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		variables[variableName] = variableValue2;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).processInstanceVariableEquals(variableName, variableValue1).topic(topicName2, lockDuration.Value).processInstanceVariableEquals(variableName, variableValue2).execute();

		//then
		assertEquals(2, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(variableValue1, externalTask.Variables.get(variableName));
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(variableValue2, externalTask.Variables.get(variableName));
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testVariableValueTopicQuery.bpmn20.xml")]
	  public virtual void testTopicQueryByVariableValueLocking()
	  {
		// given
		string topicName1 = "testTopic1";
		string topicName2 = "testTopic2";
		string topicName3 = "testTopic3";

		string variableName = "testVariable";
		string variableValue1 = "testValue1";
		string variableValue2 = "testValue2";

		IDictionary<string, object> variables = new Dictionary<string, object>();

		long? lockDuration = 60L * 1000L;

		//when
		variables[variableName] = variableValue1;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		variables[variableName] = variableValue2;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		IList<LockedExternalTask> lockedTopicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).processInstanceVariableEquals(variableName, variableValue1).topic(topicName2, lockDuration.Value).processInstanceVariableEquals(variableName, variableValue2).execute();

		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, 2 * lockDuration).processInstanceVariableEquals(variableName, variableValue1).topic(topicName2, 2 * lockDuration).processInstanceVariableEquals(variableName, variableValue2).topic(topicName3, lockDuration.Value).processInstanceVariableEquals(variableName, variableValue2).execute();

		//then
		assertEquals(2, lockedTopicTasks.Count);
		assertEquals(1, topicTasks.Count);

		LockedExternalTask externalTask = topicTasks[0];
		assertEquals(topicName3, externalTask.TopicName);
		assertEquals(variableValue2, externalTask.Variables.get(variableName));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testVariableValueTopicQuery.bpmn20.xml")]
	  public virtual void testTopicQueryByVariableValues()
	  {
		// given
		string topicName1 = "testTopic1";
		string topicName2 = "testTopic2";
		string topicName3 = "testTopic3";

		string variableName1 = "testVariable1";
		string variableName2 = "testVariable2";
		string variableName3 = "testVariable3";

		string variableValue1 = "testValue1";
		string variableValue2 = "testValue2";
		string variableValue3 = "testValue3";
		string variableValue4 = "testValue4";
		string variableValue5 = "testValue5";
		string variableValue6 = "testValue6";

		IDictionary<string, object> variables = new Dictionary<string, object>();

		long? lockDuration = 60L * 1000L;

		//when
		variables[variableName1] = variableValue1;
		variables[variableName2] = variableValue2;
		variables[variableName3] = variableValue3;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		variables[variableName1] = variableValue4;
		variables[variableName2] = variableValue5;
		variables[variableName3] = variableValue6;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", variables);

		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).processInstanceVariableEquals(variableName1, variableValue1).processInstanceVariableEquals(variableName2, variableValue2).topic(topicName2, lockDuration.Value).processInstanceVariableEquals(variableName2, variableValue5).processInstanceVariableEquals(variableName3, variableValue6).topic(topicName3, lockDuration.Value).processInstanceVariableEquals(variableName1, "fakeVariableValue").execute();

		//then
		assertEquals(2, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  // topic names are not always in the same order
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(variableValue1, externalTask.Variables.get(variableName1));
			assertEquals(variableValue2, externalTask.Variables.get(variableName2));
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(variableValue5, externalTask.Variables.get(variableName2));
			assertEquals(variableValue6, externalTask.Variables.get(variableName3));
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }

		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/ExternalTaskServiceTest.testVariableValueTopicQuery.bpmn20.xml")]
	  public virtual void testTopicQueryByBusinessKeyAndVariableValue()
	  {
		// given
		string topicName1 = "testTopic1";
		string topicName2 = "testTopic2";
		string topicName3 = "testTopic3";

		string businessKey1 = "testBusinessKey1";
		string businessKey2 = "testBusinessKey2";

		string variableName = "testVariable1";
		string variableValue1 = "testValue1";
		string variableValue2 = "testValue2";

		IDictionary<string, object> variables = new Dictionary<string, object>();

		long? lockDuration = 60L * 1000L;

		//when
		variables[variableName] = variableValue1;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", businessKey1, variables);
		variables[variableName] = variableValue2;
		runtimeService.startProcessInstanceByKey("parallelExternalTaskProcessTopicQueryVariableValues", businessKey2, variables);

		IList<LockedExternalTask> topicTasks = externalTaskService.fetchAndLock(3, "externalWorkerId").topic(topicName1, lockDuration.Value).businessKey(businessKey1).processInstanceVariableEquals(variableName, variableValue1).topic(topicName2, lockDuration.Value).businessKey(businessKey2).processInstanceVariableEquals(variableName, variableValue2).topic(topicName3, lockDuration.Value).businessKey("fakeBusinessKey").execute();

		//then
		assertEquals(2, topicTasks.Count);

		foreach (LockedExternalTask externalTask in topicTasks)
		{
		  ProcessInstance pi = runtimeService.createProcessInstanceQuery().processInstanceId(externalTask.ProcessInstanceId).singleResult();
		  // topic names are not always in the same order
		  if (externalTask.TopicName.Equals(topicName1))
		  {
			assertEquals(businessKey1, pi.BusinessKey);
			assertEquals(variableValue1, externalTask.Variables.get(variableName));
		  }
		  else if (externalTask.TopicName.Equals(topicName2))
		  {
			assertEquals(businessKey2, pi.BusinessKey);
			assertEquals(variableValue2, externalTask.Variables.get(variableName));
		  }
		  else
		  {
			fail("No other topic name values should be available!");
		  }
		}
	  }

	  protected internal virtual DateTime nowPlus(long millis)
	  {
		return new DateTime(ClockUtil.CurrentTime.Ticks + millis);
	  }

	  protected internal virtual DateTime nowMinus(long millis)
	  {
		return new DateTime(ClockUtil.CurrentTime.Ticks - millis);
	  }

	  protected internal virtual IList<string> startProcessInstance(string key, int instances)
	  {
		IList<string> ids = new List<string>();
		for (int i = 0; i < instances; i++)
		{
		  ids.Add(runtimeService.startProcessInstanceByKey(key, i.ToString()).Id);
		}
		return ids;
	  }

	  public class ReadLocalVariableListenerImpl : ExecutionListener
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  string value = (string) execution.getVariable("abc");
		  assertEquals("bar", value);
		}
	  }

	}
}