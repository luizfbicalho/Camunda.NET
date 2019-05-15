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
namespace org.camunda.bpm.engine.test.bpmn.multiinstance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate.leaveExecution;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate.throwError;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate.throwException;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;


	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ThrowErrorDelegate = org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate;
	using ActivityInstanceAssert = org.camunda.bpm.engine.test.util.ActivityInstanceAssert;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Joram Barrez
	/// @author Bernd Ruecker
	/// </summary>
	public class MultiInstanceTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml"})]
	  public virtual void testSequentialUserTasks()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miSequentialUserTasks", CollectionUtil.singletonMap("nrOfLoops", 3));
		string procId = processInstance.Id;

		// now there is now 1 activity instance below the pi:
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		ActivityInstance expectedTree = describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("miTasks").activity("miTasks").done();
		assertThat(tree).hasStructure(expectedTree);

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);
		assertEquals("kermit_0", task.Assignee);
		taskService.complete(task.Id);

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(expectedTree);

		task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);
		assertEquals("kermit_1", task.Assignee);
		taskService.complete(task.Id);

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(expectedTree);

		task = taskService.createTaskQuery().singleResult();
		assertEquals("My Task", task.Name);
		assertEquals("kermit_2", task.Assignee);
		taskService.complete(task.Id);

		assertNull(taskService.createTaskQuery().singleResult());
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml"})]
	  public virtual void testSequentialUserTasksHistory()
	  {
		runtimeService.startProcessInstanceByKey("miSequentialUserTasks", CollectionUtil.singletonMap("nrOfLoops", 4)).Id;
		for (int i = 0; i < 4; i++)
		{
		  taskService.complete(taskService.createTaskQuery().singleResult().Id);
		}

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityType("userTask").list();
		  assertEquals(4, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.ActivityId);
			assertNotNull(hai.ActivityName);
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
			assertNotNull(hai.Assignee);
		  }

		}

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  IList<HistoricTaskInstance> historicTaskInstances = historyService.createHistoricTaskInstanceQuery().list();
		  assertEquals(4, historicTaskInstances.Count);
		  foreach (HistoricTaskInstance ht in historicTaskInstances)
		  {
			assertNotNull(ht.Assignee);
			assertNotNull(ht.StartTime);
			assertNotNull(ht.EndTime);
		  }

		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml"})]
	  public virtual void testSequentialUserTasksWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialUserTasks", CollectionUtil.singletonMap("nrOfLoops", 3)).Id;

		// Complete 1 tasks
		taskService.complete(taskService.createTaskQuery().singleResult().Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml"})]
	  public virtual void testSequentialUserTasksCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialUserTasks", CollectionUtil.singletonMap("nrOfLoops", 10)).Id;

		// 10 tasks are to be created, but completionCondition stops them at 5
		for (int i = 0; i < 5; i++)
		{
		  Task task = taskService.createTaskQuery().singleResult();
		  taskService.complete(task.Id);
		}
		assertNull(taskService.createTaskQuery().singleResult());
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMITasksExecutionListener()
	  public virtual void testSequentialMITasksExecutionListener()
	  {
		RecordInvocationListener.reset();

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["nrOfLoops"] = 2;
		runtimeService.startProcessInstanceByKey("miSequentialListener", vars);

		assertEquals(1, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertNull(RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals(2, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertEquals(1, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);

		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals(2, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertEquals(2, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMITasksExecutionListener()
	  public virtual void testParallelMITasksExecutionListener()
	  {
		RecordInvocationListener.reset();

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["nrOfLoops"] = 5;
		runtimeService.startProcessInstanceByKey("miSequentialListener", vars);

		assertEquals(5, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertNull(RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);

		IList<Task> tasks = taskService.createTaskQuery().list();
		taskService.complete(tasks[0].Id);

		assertEquals(5, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertEquals(1, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);

		taskService.complete(tasks[1].Id);
		taskService.complete(tasks[2].Id);
		taskService.complete(tasks[3].Id);
		taskService.complete(tasks[4].Id);

		assertEquals(5, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START]);
		assertEquals(5, (int) RecordInvocationListener.INVOCATIONS[org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSequentialUserTasks()
	  public virtual void testNestedSequentialUserTasks()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedSequentialUserTasks").Id;

		for (int i = 0; i < 3; i++)
		{
		  Task task = taskService.createTaskQuery().taskAssignee("kermit").singleResult();
		  assertEquals("My Task", task.Name);
		  ActivityInstance processInstance = runtimeService.getActivityInstance(procId);
		  IList<ActivityInstance> instancesForActivitiyId = getInstancesForActivityId(processInstance, "miTasks");
		  assertEquals(1, instancesForActivitiyId.Count);
		  taskService.complete(task.Id);
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasks()
	  public virtual void testParallelUserTasks()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		string procId = procInst.Id;

		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(3, tasks.Count);
		assertEquals("My Task 0", tasks[0].Name);
		assertEquals("My Task 1", tasks[1].Name);
		assertEquals("My Task 2", tasks[2].Name);

		ActivityInstance processInstance = runtimeService.getActivityInstance(procId);
		assertEquals(3, processInstance.getActivityInstances("miTasks").Length);

		taskService.complete(tasks[0].Id);

		processInstance = runtimeService.getActivityInstance(procId);

		assertEquals(2, processInstance.getActivityInstances("miTasks").Length);

		taskService.complete(tasks[1].Id);

		processInstance = runtimeService.getActivityInstance(procId);
		assertEquals(1, processInstance.getActivityInstances("miTasks").Length);

		taskService.complete(tasks[2].Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelReceiveTasks()
	  public virtual void testParallelReceiveTasks()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("miParallelReceiveTasks");
		string procId = procInst.Id;

		assertEquals(3, runtimeService.createEventSubscriptionQuery().count());

		IList<Execution> receiveTaskExecutions = runtimeService.createExecutionQuery().activityId("miTasks").list();

		foreach (Execution execution in receiveTaskExecutions)
		{
		  runtimeService.messageEventReceived("message", execution.Id);
		}
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelReceiveTasks.bpmn20.xml")]
	  public virtual void testParallelReceiveTasksAssertEventSubscriptionRemoval()
	  {
		runtimeService.startProcessInstanceByKey("miParallelReceiveTasks");

		assertEquals(3, runtimeService.createEventSubscriptionQuery().count());

		IList<Execution> receiveTaskExecutions = runtimeService.createExecutionQuery().activityId("miTasks").list();

		// signal one of the instances
		runtimeService.messageEventReceived("message", receiveTaskExecutions[0].Id);

		// now there should be two subscriptions left
		assertEquals(2, runtimeService.createEventSubscriptionQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelUserTasks.bpmn20.xml"})]
	  public virtual void testParallelUserTasksHistory()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}

		// Validate history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricTaskInstance> historicTaskInstances = historyService.createHistoricTaskInstanceQuery().orderByTaskAssignee().asc().list();
		  for (int i = 0; i < historicTaskInstances.Count; i++)
		  {
			HistoricTaskInstance hi = historicTaskInstances[i];
			assertNotNull(hi.StartTime);
			assertNotNull(hi.EndTime);
			assertEquals("kermit_" + i, hi.Assignee);
		  }

		  HistoricActivityInstance multiInstanceBodyInstance = historyService.createHistoricActivityInstanceQuery().activityId("miTasks#multiInstanceBody").singleResult();
		  assertNotNull(multiInstanceBodyInstance);
		  assertEquals(pi.Id, multiInstanceBodyInstance.ParentActivityInstanceId);

		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityType("userTask").list();
		  assertEquals(3, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
			assertNotNull(hai.Assignee);
			assertEquals("userTask", hai.ActivityType);
			assertEquals(multiInstanceBodyInstance.Id, hai.ParentActivityInstanceId);
			assertNotNull(hai.TaskId);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasksWithTimer()
	  public virtual void testParallelUserTasksWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelUserTasksWithTimer").Id;

		IList<Task> tasks = taskService.createTaskQuery().list();
		taskService.complete(tasks[0].Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasksCompletionCondition()
	  public virtual void testParallelUserTasksCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelUserTasksCompletionCondition").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(5, tasks.Count);

		// Completing 3 tasks gives 50% of tasks completed, which triggers completionCondition
		for (int i = 0; i < 3; i++)
		{
		  assertEquals(5 - i, taskService.createTaskQuery().count());
		  taskService.complete(tasks[i].Id);
		}
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasksBasedOnCollection()
	  public virtual void testParallelUserTasksBasedOnCollection()
	  {
		IList<string> assigneeList = Arrays.asList("kermit", "gonzo", "mispiggy", "fozzie", "bubba");
		string procId = runtimeService.startProcessInstanceByKey("miParallelUserTasksBasedOnCollection", CollectionUtil.singletonMap("assigneeList", assigneeList)).Id;

		IList<Task> tasks = taskService.createTaskQuery().orderByTaskAssignee().asc().list();
		assertEquals(5, tasks.Count);
		assertEquals("bubba", tasks[0].Assignee);
		assertEquals("fozzie", tasks[1].Assignee);
		assertEquals("gonzo", tasks[2].Assignee);
		assertEquals("kermit", tasks[3].Assignee);
		assertEquals("mispiggy", tasks[4].Assignee);

		// Completing 3 tasks will trigger completioncondition
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		taskService.complete(tasks[2].Id);
		assertEquals(0, taskService.createTaskQuery().count());
		assertProcessEnded(procId);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelUserTasksBasedOnCollection.bpmn20.xml")]
	  public virtual void testEmptyCollectionInMI()
	  {
		IList<string> assigneeList = new List<string>();
		string procId = runtimeService.startProcessInstanceByKey("miParallelUserTasksBasedOnCollection", CollectionUtil.singletonMap("assigneeList", assigneeList)).Id;

		assertEquals(0, taskService.createTaskQuery().count());
		assertProcessEnded(procId);
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> activities = historyService.createHistoricActivityInstanceQuery().processInstanceId(procId).orderByActivityId().asc().list();
		  assertEquals(3, activities.Count);
		  assertEquals("miTasks#multiInstanceBody", activities[0].ActivityId);
		  assertEquals("theEnd", activities[1].ActivityId);
		  assertEquals("theStart", activities[2].ActivityId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testParallelUserTasksBasedOnCollectionExpression()
	  public virtual void FAILING_testParallelUserTasksBasedOnCollectionExpression()
	  {
		DelegateEvent.clearEvents();

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("myBean", new DelegateBean()));

		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(2, recordedEvents.Count);

		assertEquals("miTasks#multiInstanceBody", recordedEvents[0].CurrentActivityId);
		assertEquals("miTasks#multiInstanceBody", recordedEvents[1].CurrentActivityId); // or miTasks

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasksCustomExtensions()
	  public virtual void testParallelUserTasksCustomExtensions()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		IList<string> assigneeList = Arrays.asList("kermit", "gonzo", "fozzie");
		vars["assigneeList"] = assigneeList;
		runtimeService.startProcessInstanceByKey("miSequentialUserTasks", vars);

		foreach (string assignee in assigneeList)
		{
		  Task task = taskService.createTaskQuery().singleResult();
		  assertEquals(assignee, task.Assignee);
		  taskService.complete(task.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelUserTasksExecutionAndTaskListeners()
	  public virtual void testParallelUserTasksExecutionAndTaskListeners()
	  {
		runtimeService.startProcessInstanceByKey("miParallelUserTasks");
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		Execution waitState = runtimeService.createExecutionQuery().singleResult();
		assertEquals(3, runtimeService.getVariable(waitState.Id, "taskListenerCounter"));
		assertEquals(3, runtimeService.getVariable(waitState.Id, "executionListenerCounter"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedParallelUserTasks()
	  public virtual void testNestedParallelUserTasks()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelUserTasks").Id;

		IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		foreach (Task task in tasks)
		{
		  assertEquals("My Task", task.Name);
		  taskService.complete(task.Id);
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialScriptTasks()
	  public virtual void testSequentialScriptTasks()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sum"] = 0;
		vars["nrOfLoops"] = 5;
		runtimeService.startProcessInstanceByKey("miSequentialScriptTask", vars);
		Execution waitStateExecution = runtimeService.createExecutionQuery().singleResult();
		int sum = (int?) runtimeService.getVariable(waitStateExecution.Id, "sum").Value;
		assertEquals(10, sum);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialScriptTasks.bpmn20.xml")]
	  public virtual void testSequentialScriptTasksNoStackOverflow()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sum"] = 0;
		vars["nrOfLoops"] = 200;
		runtimeService.startProcessInstanceByKey("miSequentialScriptTask", vars);
		Execution waitStateExecution = runtimeService.createExecutionQuery().singleResult();
		int sum = (int?) runtimeService.getVariable(waitStateExecution.Id, "sum").Value;
		assertEquals(19900, sum);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialScriptTasks.bpmn20.xml"})]
	  public virtual void testSequentialScriptTasksHistory()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sum"] = 0;
		vars["nrOfLoops"] = 7;
		runtimeService.startProcessInstanceByKey("miSequentialScriptTask", vars);

		// Validate history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().activityType("scriptTask").orderByActivityId().asc().list();
		  assertEquals(7, historicInstances.Count);
		  for (int i = 0; i < 7; i++)
		  {
			HistoricActivityInstance hai = historicInstances[i];
			assertEquals("scriptTask", hai.ActivityType);
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialScriptTasksCompletionCondition()
	  public virtual void testSequentialScriptTasksCompletionCondition()
	  {
		runtimeService.startProcessInstanceByKey("miSequentialScriptTaskCompletionCondition").Id;
		Execution waitStateExecution = runtimeService.createExecutionQuery().singleResult();
		int sum = (int?) runtimeService.getVariable(waitStateExecution.Id, "sum").Value;
		assertEquals(5, sum);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelScriptTasks()
	  public virtual void testParallelScriptTasks()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sum"] = 0;
		vars["nrOfLoops"] = 10;
		runtimeService.startProcessInstanceByKey("miParallelScriptTask", vars);
		Execution waitStateExecution = runtimeService.createExecutionQuery().singleResult();
		int sum = (int?) runtimeService.getVariable(waitStateExecution.Id, "sum").Value;
		assertEquals(45, sum);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelScriptTasks.bpmn20.xml"})]
	  public virtual void testParallelScriptTasksHistory()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sum"] = 0;
		vars["nrOfLoops"] = 4;
		runtimeService.startProcessInstanceByKey("miParallelScriptTask", vars);
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityType("scriptTask").list();
		  assertEquals(4, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.StartTime);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelScriptTasksCompletionCondition()
	  public virtual void testParallelScriptTasksCompletionCondition()
	  {
		runtimeService.startProcessInstanceByKey("miParallelScriptTaskCompletionCondition");
		Execution waitStateExecution = runtimeService.createExecutionQuery().singleResult();
		int sum = (int?) runtimeService.getVariable(waitStateExecution.Id, "sum").Value;
		assertEquals(2, sum);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelScriptTasksCompletionCondition.bpmn20.xml"})]
	  public virtual void testParallelScriptTasksCompletionConditionHistory()
	  {
		runtimeService.startProcessInstanceByKey("miParallelScriptTaskCompletionCondition");
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityType("scriptTask").list();
		  assertEquals(2, historicActivityInstances.Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialSubProcess()
	  public virtual void testSequentialSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialSubprocess").Id;

		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		for (int i = 0; i < 4; i++)
		{
		  IList<Task> tasks = query.list();
		  assertEquals(2, tasks.Count);

		  assertEquals("task one", tasks[0].Name);
		  assertEquals("task two", tasks[1].Name);

		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);

		  if (i != 3)
		  {
			IList<string> activities = runtimeService.getActiveActivityIds(procId);
			assertNotNull(activities);
			assertEquals(2, activities.Count);
		  }
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialSubProcessEndEvent()
	  public virtual void testSequentialSubProcessEndEvent()
	  {
		// ACT-1185: end-event in subprocess causes inactivated execution
		string procId = runtimeService.startProcessInstanceByKey("miSequentialSubprocess").Id;

		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		for (int i = 0; i < 4; i++)
		{
		  IList<Task> tasks = query.list();
		  assertEquals(1, tasks.Count);

		  assertEquals("task one", tasks[0].Name);

		  taskService.complete(tasks[0].Id);

		  // Last run, the execution no longer exists
		  if (i != 3)
		  {
			IList<string> activities = runtimeService.getActiveActivityIds(procId);
			assertNotNull(activities);
			assertEquals(1, activities.Count);
		  }
		}

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialSubProcess.bpmn20.xml"})]
	  public virtual void testSequentialSubProcessHistory()
	  {
		runtimeService.startProcessInstanceByKey("miSequentialSubprocess");
		for (int i = 0; i < 4; i++)
		{
		  IList<Task> tasks = taskService.createTaskQuery().list();
		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		// Validate history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> onlySubProcessInstances = historyService.createHistoricActivityInstanceQuery().activityType("subProcess").list();
		  assertEquals(4, onlySubProcessInstances.Count);

		  IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().activityType("subProcess").list();
		  assertEquals(4, historicInstances.Count);
		  foreach (HistoricActivityInstance hai in historicInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
		  }

		  historicInstances = historyService.createHistoricActivityInstanceQuery().activityType("userTask").list();
		  assertEquals(8, historicInstances.Count);
		  foreach (HistoricActivityInstance hai in historicInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialSubProcessWithTimer()
	  public virtual void testSequentialSubProcessWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialSubprocessWithTimer").Id;

		// Complete one subprocess
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialSubProcessCompletionCondition()
	  public virtual void testSequentialSubProcessCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialSubprocessCompletionCondition").Id;

		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		for (int i = 0; i < 3; i++)
		{
		  IList<Task> tasks = query.list();
		  assertEquals(2, tasks.Count);

		  assertEquals("task one", tasks[0].Name);
		  assertEquals("task two", tasks[1].Name);

		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSequentialSubProcess()
	  public virtual void testNestedSequentialSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedSequentialSubProcess").Id;

		for (int i = 0; i < 3; i++)
		{
		  IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSequentialSubProcessWithTimer()
	  public virtual void testNestedSequentialSubProcessWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedSequentialSubProcessWithTimer").Id;

		for (int i = 0; i < 2; i++)
		{
		  IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		// Complete one task, to make it a bit more trickier
		IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		taskService.complete(tasks[0].Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelSubProcess()
	  public virtual void testParallelSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelSubprocess").Id;
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(4, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelSubProcess.bpmn20.xml"})]
	  public virtual void testParallelSubProcessHistory()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		// Validate history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityId("miSubProcess").list();
		  assertEquals(2, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.StartTime);
			// now end time is null
			assertNull(hai.EndTime);
			assertNotNull(pi.Id, hai.ParentActivityInstanceId);
		  }
		}

		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}

		// Validate history
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityId("miSubProcess").list();
		  assertEquals(2, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
			assertNotNull(pi.Id, hai.ParentActivityInstanceId);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelSubProcessWithTimer()
	  public virtual void testParallelSubProcessWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelSubprocessWithTimer").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(6, tasks.Count);

		// Complete two tasks
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelSubProcessCompletionCondition()
	  public virtual void testParallelSubProcessCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelSubprocessCompletionCondition").Id;

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(4, tasks.Count);

		// get activities of a single subprocess
		ActivityInstance[] taskActivities = runtimeService.getActivityInstance(procId).getActivityInstances("miSubProcess")[0].ChildActivityInstances;

		foreach (ActivityInstance taskActivity in taskActivities)
		{
		  Task task = taskService.createTaskQuery().activityInstanceIdIn(taskActivity.Id).singleResult();
		  taskService.complete(task.Id);
		}

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelSubProcessAllAutomatic()
	  public virtual void testParallelSubProcessAllAutomatic()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelSubprocessAllAutomatics", CollectionUtil.singletonMap("nrOfLoops", 5)).Id;
		Execution waitState = runtimeService.createExecutionQuery().singleResult();
		assertEquals(10, runtimeService.getVariable(waitState.Id, "sum"));

		runtimeService.signal(waitState.Id);
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelSubProcessAllAutomatic.bpmn20.xml"})]
	  public virtual void testParallelSubProcessAllAutomaticCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelSubprocessAllAutomatics", CollectionUtil.singletonMap("nrOfLoops", 10)).Id;
		Execution waitState = runtimeService.createExecutionQuery().singleResult();
		assertEquals(12, runtimeService.getVariable(waitState.Id, "sum"));

		runtimeService.signal(waitState.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedParallelSubProcess()
	  public virtual void testNestedParallelSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelSubProcess").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(8, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedParallelSubProcessWithTimer()
	  public virtual void testNestedParallelSubProcessWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelSubProcess").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(12, tasks.Count);

		for (int i = 0; i < 3; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml"})]
	  public virtual void testSequentialCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialCallActivity").Id;

		for (int i = 0; i < 3; i++)
		{
		  IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		  assertEquals(2, tasks.Count);
		  assertEquals("task one", tasks[0].Name);
		  assertEquals("task two", tasks[1].Name);
		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialCallActivityWithList.bpmn20.xml")]
	  public virtual void testSequentialCallActivityWithList()
	  {
		List<string> list = new List<string>();
		list.Add("one");
		list.Add("two");

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["list"] = list;

		string procId = runtimeService.startProcessInstanceByKey("parentProcess", variables).Id;

		Task task1 = taskService.createTaskQuery().processVariableValueEquals("element", "one").singleResult();
		Task task2 = taskService.createTaskQuery().processVariableValueEquals("element", "two").singleResult();

		assertNotNull(task1);
		assertNotNull(task2);

		Dictionary<string, object> subVariables = new Dictionary<string, object>();
		subVariables["x"] = "y";

		taskService.complete(task1.Id, subVariables);
		taskService.complete(task2.Id, subVariables);

		Task task3 = taskService.createTaskQuery().processDefinitionKey("midProcess").singleResult();
		assertNotNull(task3);
		taskService.complete(task3.Id);

		Task task4 = taskService.createTaskQuery().processDefinitionKey("parentProcess").singleResult();
		assertNotNull(task4);
		taskService.complete(task4.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testSequentialCallActivityWithTimer.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testSequentialCallActivityWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miSequentialCallActivityWithTimer").Id;

		// Complete first subprocess
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("task one", tasks[0].Name);
		assertEquals("task two", tasks[1].Name);
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testParallelCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelCallActivity").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(12, tasks.Count);
		for (int i = 0; i < tasks.Count; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testParallelCallActivityHistory()
	  {
		runtimeService.startProcessInstanceByKey("miParallelCallActivity");
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(12, tasks.Count);
		for (int i = 0; i < tasks.Count; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // Validate historic processes
		  IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		  assertEquals(7, historicProcessInstances.Count); // 6 subprocesses + main process
		  foreach (HistoricProcessInstance hpi in historicProcessInstances)
		  {
			assertNotNull(hpi.StartTime);
			assertNotNull(hpi.EndTime);
		  }

		  // Validate historic activities
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().activityType("callActivity").list();
		  assertEquals(6, historicActivityInstances.Count);
		  foreach (HistoricActivityInstance hai in historicActivityInstances)
		  {
			assertNotNull(hai.StartTime);
			assertNotNull(hai.EndTime);
		  }
		}

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  // Validate historic tasks
		  IList<HistoricTaskInstance> historicTaskInstances = historyService.createHistoricTaskInstanceQuery().list();
		  assertEquals(12, historicTaskInstances.Count);
		  foreach (HistoricTaskInstance hti in historicTaskInstances)
		  {
			assertNotNull(hti.StartTime);
			assertNotNull(hti.EndTime);
			assertNotNull(hti.Assignee);
			assertEquals("completed", hti.DeleteReason);
		  }
		}
	  }


	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelCallActivityWithTimer.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testParallelCallActivityWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miParallelCallActivity").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(6, tasks.Count);
		for (int i = 0; i < 2; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedSequentialCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testNestedSequentialCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedSequentialCallActivity").Id;

		for (int i = 0; i < 4; i++)
		{
		  IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		  assertEquals(2, tasks.Count);
		  assertEquals("task one", tasks[0].Name);
		  assertEquals("task two", tasks[1].Name);
		  taskService.complete(tasks[0].Id);
		  taskService.complete(tasks[1].Id);
		}

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedSequentialCallActivityWithTimer.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testNestedSequentialCallActivityWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedSequentialCallActivityWithTimer").Id;

		// first instance
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("task one", tasks[0].Name);
		assertEquals("task two", tasks[1].Name);
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// one task of second instance
		tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);
		taskService.complete(tasks[0].Id);

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedParallelCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testNestedParallelCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelCallActivity").Id;

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(14, tasks.Count);
		for (int i = 0; i < 14; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedParallelCallActivityWithTimer.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testNestedParallelCallActivityWithTimer()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelCallActivityWithTimer").Id;

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(4, tasks.Count);
		for (int i = 0; i < 3; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		// Fire timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		Task taskAfterTimer = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
		taskService.complete(taskAfterTimer.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedParallelCallActivityCompletionCondition.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.externalSubProcess.bpmn20.xml" })]
	  public virtual void testNestedParallelCallActivityCompletionCondition()
	  {
		string procId = runtimeService.startProcessInstanceByKey("miNestedParallelCallActivityCompletionCondition").Id;

		assertEquals(8, taskService.createTaskQuery().count());

		for (int i = 0; i < 2; i++)
		{
		  ProcessInstance nextSubProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("externalSubProcess").listPage(0, 1).get(0);
		  IList<Task> tasks = taskService.createTaskQuery().processInstanceId(nextSubProcessInstance.Id).list();
		  assertEquals(2, tasks.Count);
		  foreach (Task task in tasks)
		  {
			taskService.complete(task.Id);
		  }
		}

		assertProcessEnded(procId);
	  }

	  // ACT-764
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialServiceTaskWithClass()
	  public virtual void testSequentialServiceTaskWithClass()
	  {
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("multiInstanceServiceTask", CollectionUtil.singletonMap("result", 5));
		int? result = (int?) runtimeService.getVariable(procInst.Id, "result");
		assertEquals(160, result.Value);

		runtimeService.signal(procInst.Id);
		assertProcessEnded(procInst.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialServiceTaskWithClassAndCollection()
	  public virtual void testSequentialServiceTaskWithClassAndCollection()
	  {
		ICollection<int> items = Arrays.asList(1,2,3,4,5,6);
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["result"] = 1;
		vars["items"] = items;

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("multiInstanceServiceTask", vars);
		int? result = (int?) runtimeService.getVariable(procInst.Id, "result");
		assertEquals(720, result.Value);

		runtimeService.signal(procInst.Id);
		assertProcessEnded(procInst.Id);
	  }

	  // ACT-901
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAct901()
	  public virtual void testAct901()
	  {

		DateTime startTime = ClockUtil.CurrentTime;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("multiInstanceSubProcess");
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc().list();

		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + 61000L); // timer is set to one minute
		IList<Job> timers = managementService.createJobQuery().list();
		assertEquals(5, timers.Count);

		// Execute all timers one by one (single thread vs thread pool of job executor, which leads to optimisticlockingexceptions!)
		foreach (Job timer in timers)
		{
		  managementService.executeJob(timer.Id);
		}

		// All tasks should be canceled
		tasks = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc().list();
		assertEquals(0, tasks.Count);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.callActivityWithBoundaryErrorEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.throwingErrorEventSubProcess.bpmn20.xml" })]
	  public virtual void testMultiInstanceCallActivityWithErrorBoundaryEvent()
	  {
		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["assignees"] = Arrays.asList("kermit", "gonzo");

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variableMap);

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		// finish first call activity with error
		variableMap = new Dictionary<string, object>();
		variableMap["done"] = false;
		taskService.complete(tasks[0].Id, variableMap);

		tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);

		taskService.complete(tasks[0].Id);

		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey("process").list();
		assertEquals(0, processInstances.Count);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.callActivityWithBoundaryErrorEventSequential.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.throwingErrorEventSubProcess.bpmn20.xml" })]
	  public virtual void testSequentialMultiInstanceCallActivityWithErrorBoundaryEvent()
	  {
		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["assignees"] = Arrays.asList("kermit", "gonzo");

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variableMap);

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);

		// finish first call activity with error
		variableMap = new Dictionary<string, object>();
		variableMap["done"] = false;
		taskService.complete(tasks[0].Id, variableMap);

		tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);

		taskService.complete(tasks[0].Id);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedMultiInstanceTasks.bpmn20.xml"})]
	  public virtual void testNestedMultiInstanceTasks()
	  {
		IList<string> processes = Arrays.asList("process A", "process B");
		IList<string> assignees = Arrays.asList("kermit", "gonzo");
		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["subProcesses"] = processes;
		variableMap["assignees"] = assignees;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miNestedMultiInstanceTasks", variableMap);

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(processes.Count * assignees.Count, tasks.Count);

		foreach (Task t in tasks)
		{
		  taskService.complete(t.Id);
		}

		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey("miNestedMultiInstanceTasks").list();
		assertEquals(0, processInstances.Count);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testNestedMultiInstanceTasks.bpmn20.xml"})]
	  public virtual void testNestedMultiInstanceTasksActivityInstance()
	  {
		IList<string> processes = Arrays.asList("process A", "process B");
		IList<string> assignees = Arrays.asList("kermit", "gonzo");
		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["subProcesses"] = processes;
		variableMap["assignees"] = assignees;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("miNestedMultiInstanceTasks", variableMap);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		ActivityInstanceAssert.assertThat(activityInstance).hasStructure(ActivityInstanceAssert.describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("subprocess1").beginScope("subprocess1").beginMiBody("miTasks").activity("miTasks").activity("miTasks").endScope().endScope().beginScope("subprocess1").beginMiBody("miTasks").activity("miTasks").activity("miTasks").endScope().endScope().done());

	  }


	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testParallelUserTasks.bpmn20.xml"})]
	  public virtual void testActiveExecutionsInParallelTasks()
	  {
		runtimeService.startProcessInstanceByKey("miParallelUserTasks").Id;

		ProcessInstance instance = runtimeService.createProcessInstanceQuery().singleResult();

		IList<Execution> executions = runtimeService.createExecutionQuery().list();
		assertEquals(5, executions.Count);

		foreach (Execution execution in executions)
		{
		  ExecutionEntity entity = (ExecutionEntity) execution;

		  if (!entity.Id.Equals(instance.Id) && !entity.ParentId.Equals(instance.Id))
		  {
			// child executions
			assertTrue(entity.Active);
		  }
		  else
		  {
			// process instance and scope execution
			assertFalse(entity.Active);
		  }
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfSequentialAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfSequentialAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfSequentialAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfSequentialAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfParallelAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfParallelAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfParallelAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfParallelAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfSequentialDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwException());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfSequentialDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwError());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfSequentialDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfSequentialDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		// signal 2 times to execute first sequential behaviors
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);
		runtimeService.setVariables(pi, leaveExecution());
		runtimeService.signal(runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult().Id);

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfParallelDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwException());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfParallelDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwError());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfParallelDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/multiinstance/MultiInstanceTest.testCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfParallelDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").list().get(3);
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }
	}

}