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
namespace org.camunda.bpm.engine.test.bpmn.gateway
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;


	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tom Van Buskirk
	/// @author Tijs Rademakers
	/// </summary>
	public class InclusiveGatewayTest : PluggableProcessEngineTestCase
	{

	  private const string TASK1_NAME = "Task 1";
	  private const string TASK2_NAME = "Task 2";
	  private const string TASK3_NAME = "Task 3";

	  private const string BEAN_TASK1_NAME = "Basic service";
	  private const string BEAN_TASK2_NAME = "Standard service";
	  private const string BEAN_TASK3_NAME = "Gold Member service";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDivergingInclusiveGateway()
	  public virtual void testDivergingInclusiveGateway()
	  {
		for (int i = 1; i <= 3; i++)
		{
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveGwDiverging", CollectionUtil.singletonMap("input", i));
		  IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		  IList<string> expectedNames = new List<string>();
		  if (i == 1)
		  {
			expectedNames.Add(TASK1_NAME);
		  }
		  if (i <= 2)
		  {
			expectedNames.Add(TASK2_NAME);
		  }
		  expectedNames.Add(TASK3_NAME);
		  foreach (Task task in tasks)
		  {
			Console.WriteLine("task " + task.Name);
		  }
		  assertEquals(4 - i, tasks.Count);
		  foreach (Task task in tasks)
		  {
			expectedNames.Remove(task.Name);
		  }
		  assertEquals(0, expectedNames.Count);
		  runtimeService.deleteProcessInstance(pi.Id, "testing deletion");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMergingInclusiveGateway()
	  public virtual void testMergingInclusiveGateway()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveGwMerging", CollectionUtil.singletonMap("input", 2));
		assertEquals(1, taskService.createTaskQuery().count());

		runtimeService.deleteProcessInstance(pi.Id, "testing deletion");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMergingInclusiveGatewayAsync()
	  public virtual void testMergingInclusiveGatewayAsync()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveGwMerging", CollectionUtil.singletonMap("input", 2));
		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  managementService.executeJob(job.Id);
		}
		assertEquals(1, taskService.createTaskQuery().count());

		runtimeService.deleteProcessInstance(pi.Id, "testing deletion");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPartialMergingInclusiveGateway()
	  public virtual void testPartialMergingInclusiveGateway()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("partialInclusiveGwMerging", CollectionUtil.singletonMap("input", 2));
		Task partialTask = taskService.createTaskQuery().singleResult();
		assertEquals("partialTask", partialTask.TaskDefinitionKey);

		taskService.complete(partialTask.Id);

		Task fullTask = taskService.createTaskQuery().singleResult();
		assertEquals("theTask", fullTask.TaskDefinitionKey);

		runtimeService.deleteProcessInstance(pi.Id, "testing deletion");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoSequenceFlowSelected()
	  public virtual void testNoSequenceFlowSelected()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("inclusiveGwNoSeqFlowSelected", CollectionUtil.singletonMap("input", 4));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-02004 No outgoing sequence flow for the element with id 'inclusiveGw' could be selected for continuing the process.", e.Message);
		}
	  }

	  /// <summary>
	  /// Test for ACT-1216: When merging a concurrent execution the parent is not activated correctly
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParentActivationOnNonJoiningEnd() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParentActivationOnNonJoiningEnd()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parentActivationOnNonJoiningEnd");

		IList<Execution> executionsBefore = runtimeService.createExecutionQuery().list();
		assertEquals(3, executionsBefore.Count);

		// start first round of tasks
		IList<Task> firstTasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();

		assertEquals(2, firstTasks.Count);

		foreach (Task t in firstTasks)
		{
		  taskService.complete(t.Id);
		}

		// start first round of tasks
		IList<Task> secondTasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();

		assertEquals(2, secondTasks.Count);

		// complete one task
		Task task = secondTasks[0];
		taskService.complete(task.Id);

		// should have merged last child execution into parent
		IList<Execution> executionsAfter = runtimeService.createExecutionQuery().list();
		assertEquals(1, executionsAfter.Count);

		Execution execution = executionsAfter[0];

		// and should have one active activity
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(execution.Id);
		assertEquals(1, activeActivityIds.Count);

		// Completing last task should finish the process instance

		Task lastTask = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.complete(lastTask.Id);

		assertEquals(0l, runtimeService.createProcessInstanceQuery().active().count());
	  }

	  /// <summary>
	  /// Test for bug ACT-10: whitespaces/newlines in expressions lead to exceptions
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testWhitespaceInExpression()
	  public virtual void testWhitespaceInExpression()
	  {
		// Starting a process instance will lead to an exception if whitespace are
		// incorrectly handled
		runtimeService.startProcessInstanceByKey("inclusiveWhiteSpaceInExpression", CollectionUtil.singletonMap("input", 1));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/gateway/InclusiveGatewayTest.testDivergingInclusiveGateway.bpmn20.xml" })]
	  public virtual void testUnknownVariableInExpression()
	  {
		// Instead of 'input' we're starting a process instance with the name
		// 'iinput' (ie. a typo)
		try
		{
		  runtimeService.startProcessInstanceByKey("inclusiveGwDiverging", CollectionUtil.singletonMap("iinput", 1));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown property used in expression", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnBeanProperty()
	  public virtual void testDecideBasedOnBeanProperty()
	  {
		runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnBeanProperty", CollectionUtil.singletonMap("order", new InclusiveGatewayTestOrder(150)));
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);
		IDictionary<string, string> expectedNames = new Dictionary<string, string>();
		expectedNames[BEAN_TASK2_NAME] = BEAN_TASK2_NAME;
		expectedNames[BEAN_TASK3_NAME] = BEAN_TASK3_NAME;
		foreach (Task task in tasks)
		{
		  expectedNames.Remove(task.Name);
		}
		assertEquals(0, expectedNames.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnListOrArrayOfBeans()
	  public virtual void testDecideBasedOnListOrArrayOfBeans()
	  {
		IList<InclusiveGatewayTestOrder> orders = new List<InclusiveGatewayTestOrder>();
		orders.Add(new InclusiveGatewayTestOrder(50));
		orders.Add(new InclusiveGatewayTestOrder(300));
		orders.Add(new InclusiveGatewayTestOrder(175));

		ProcessInstance pi = null;
		try
		{
		  pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orders));
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expect an exception to be thrown here
		}

		orders[1] = new InclusiveGatewayTestOrder(175);
		pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orders));
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(task);
		assertEquals(BEAN_TASK3_NAME, task.Name);

		orders[1] = new InclusiveGatewayTestOrder(125);
		pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orders));
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);
		IList<string> expectedNames = new List<string>();
		expectedNames.Add(BEAN_TASK2_NAME);
		expectedNames.Add(BEAN_TASK3_NAME);
		foreach (Task t in tasks)
		{
		  expectedNames.Remove(t.Name);
		}
		assertEquals(0, expectedNames.Count);

		// Arrays are usable in exactly the same way
		InclusiveGatewayTestOrder[] orderArray = orders.ToArray();
		orderArray[1].Price = 10;
		pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orderArray));
		tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertNotNull(tasks);
		assertEquals(3, tasks.Count);
		expectedNames.Clear();
		expectedNames.Add(BEAN_TASK1_NAME);
		expectedNames.Add(BEAN_TASK2_NAME);
		expectedNames.Add(BEAN_TASK3_NAME);
		foreach (Task t in tasks)
		{
		  expectedNames.Remove(t.Name);
		}
		assertEquals(0, expectedNames.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnBeanMethod()
	  public virtual void testDecideBasedOnBeanMethod()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.singletonMap("order", new InclusiveGatewayTestOrder(200)));
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(task);
		assertEquals(BEAN_TASK3_NAME, task.Name);

		pi = runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.singletonMap("order", new InclusiveGatewayTestOrder(125)));
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(2, tasks.Count);
		IList<string> expectedNames = new List<string>();
		expectedNames.Add(BEAN_TASK2_NAME);
		expectedNames.Add(BEAN_TASK3_NAME);
		foreach (Task t in tasks)
		{
		  expectedNames.Remove(t.Name);
		}
		assertEquals(0, expectedNames.Count);

		try
		{
		  runtimeService.startProcessInstanceByKey("inclusiveDecisionBasedOnBeanMethod", CollectionUtil.singletonMap("order", new InclusiveGatewayTestOrder(300)));
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Should get an exception indicating that no path could be taken
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInvalidMethodExpression()
	  public virtual void testInvalidMethodExpression()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("inclusiveInvalidMethodExpression", CollectionUtil.singletonMap("order", new InclusiveGatewayTestOrder(50)));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown method used in expression", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultSequenceFlow()
	  public virtual void testDefaultSequenceFlow()
	  {
		// Input == 1 -> default is not selected, other 2 tasks are selected
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 1));
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(2, tasks.Count);
		IDictionary<string, string> expectedNames = new Dictionary<string, string>();
		expectedNames["Input is one"] = "Input is one";
		expectedNames["Input is three or one"] = "Input is three or one";
		foreach (Task t in tasks)
		{
		  expectedNames.Remove(t.Name);
		}
		assertEquals(0, expectedNames.Count);
		runtimeService.deleteProcessInstance(pi.Id, null);

		// Input == 3 -> default is not selected, "one or three" is selected
		pi = runtimeService.startProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 3));
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Input is three or one", task.Name);

		// Default input
		pi = runtimeService.startProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 5));
		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Default input", task.Name);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/gateway/InclusiveGatewayTest.testDefaultSequenceFlow.bpmn20.xml")]
	  public virtual void testDefaultSequenceFlowExecutionIsActive()
	  {
		// given a triggered inclusive gateway default flow
		runtimeService.startProcessInstanceByKey("inclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 5));

		// then the process instance execution is not deactivated
		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().singleResult();
		assertEquals("theTask2", execution.ActivityId);
		assertTrue(execution.Active);
	  }

	  /// <summary>
	  /// 1. or split
	  /// 2. or join
	  /// 3. that same or join splits again (in this case has a single default sequence flow)
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSplitMergeSplit()
	  public virtual void testSplitMergeSplit()
	  {
		// given a process instance with two concurrent tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("inclusiveGwSplitAndMerge", CollectionUtil.singletonMap("input", 1));

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		// when the executions are joined at an inclusive gateway and the gateway itself has an outgoing default flow
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// then the task after the inclusive gateway is reached by the process instance execution (i.e. concurrent root)
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		assertEquals(processInstance.Id, task.ExecutionId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoIdOnSequenceFlow()
	  public virtual void testNoIdOnSequenceFlow()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveNoIdOnSequenceFlow", CollectionUtil.singletonMap("input", 3));
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Input is more than one", task.Name);

		// Both should be enabled on 1
		pi = runtimeService.startProcessInstanceByKey("inclusiveNoIdOnSequenceFlow", CollectionUtil.singletonMap("input", 1));
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(2, tasks.Count);
		IDictionary<string, string> expectedNames = new Dictionary<string, string>();
		expectedNames["Input is one"] = "Input is one";
		expectedNames["Input is more than one"] = "Input is more than one";
		foreach (Task t in tasks)
		{
		  expectedNames.Remove(t.Name);
		}
		assertEquals(0, expectedNames.Count);
	  }

	  /// <summary>
	  /// This test the isReachable() check thaty is done to check if
	  /// upstream tokens can reach the inclusive gateway.
	  /// 
	  /// In case of loops, special care needs to be taken in the algorithm,
	  /// or else stackoverflows will happen very quickly.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLoop()
	  public virtual void testLoop()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveTestLoop", CollectionUtil.singletonMap("counter", 1));

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task C", task.Name);

		taskService.complete(task.Id);
		assertEquals(0, taskService.createTaskQuery().count());


		foreach (Execution execution in runtimeService.createExecutionQuery().list())
		{
		  Console.WriteLine(((ExecutionEntity) execution).ActivityId);
		}

		assertEquals("Found executions: " + runtimeService.createExecutionQuery().list(), 0, runtimeService.createExecutionQuery().count());
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterSubprocesses()
	  public virtual void testJoinAfterSubprocesses()
	  {
		// Test case to test act-1204
		IDictionary<string, object> variableMap = new Dictionary<string, object>();
		variableMap["a"] = 1;
		variableMap["b"] = 1;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("InclusiveGateway", variableMap);
		assertNotNull(processInstance.Id);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(2, taskService.createTaskQuery().count());

		taskService.complete(tasks[0].Id);
		assertEquals(1, taskService.createTaskQuery().count());

		taskService.complete(tasks[1].Id);

		Task task = taskService.createTaskQuery().taskAssignee("c").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertNull(processInstance);

		variableMap = new Dictionary<string, object>();
		variableMap["a"] = 1;
		variableMap["b"] = 2;
		processInstance = runtimeService.startProcessInstanceByKey("InclusiveGateway", variableMap);
		assertNotNull(processInstance.Id);

		tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(1, taskService.createTaskQuery().count());

		task = tasks[0];
		assertEquals("a", task.Assignee);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskAssignee("c").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertNull(processInstance);

		variableMap = new Dictionary<string, object>();
		variableMap["a"] = 2;
		variableMap["b"] = 2;
		try
		{
		  processInstance = runtimeService.startProcessInstanceByKey("InclusiveGateway", variableMap);
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("No outgoing sequence flow"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/gateway/InclusiveGatewayTest.testJoinAfterCall.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/gateway/InclusiveGatewayTest.testJoinAfterCallSubProcess.bpmn20.xml"})]
	  public virtual void testJoinAfterCall()
	  {
		// Test case to test act-1026
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("InclusiveGatewayAfterCall");
		assertNotNull(processInstance.Id);
		assertEquals(3, taskService.createTaskQuery().count());

		// now complete task A and check number of remaining tasks.
		// inclusive gateway should wait for the "Task B" and "Task C"
		Task taskA = taskService.createTaskQuery().taskName("Task A").singleResult();
		assertNotNull(taskA);
		taskService.complete(taskA.Id);
		assertEquals(2, taskService.createTaskQuery().count());

		// now complete task B and check number of remaining tasks
		// inclusive gateway should wait for "Task C"
		Task taskB = taskService.createTaskQuery().taskName("Task B").singleResult();
		assertNotNull(taskB);
		taskService.complete(taskB.Id);
		assertEquals(1, taskService.createTaskQuery().count());

		// now complete task C. Gateway activates and "Task C" remains
		Task taskC = taskService.createTaskQuery().taskName("Task C").singleResult();
		assertNotNull(taskC);
		taskService.complete(taskC.Id);
		assertEquals(1, taskService.createTaskQuery().count());

		// check that remaining task is in fact task D
		Task taskD = taskService.createTaskQuery().taskName("Task D").singleResult();
		assertNotNull(taskD);
		assertEquals("Task D", taskD.Name);
		taskService.complete(taskD.Id);

		processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertNull(processInstance);
	  }

	  /// <summary>
	  /// The test process has an OR gateway where, the 'input' variable is used to
	  /// select the expected outgoing sequence flow.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecisionFunctionality()
	  public virtual void testDecisionFunctionality()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();

		// Test with input == 1
		variables["input"] = 1;
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("inclusiveGateway", variables);
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(3, tasks.Count);
		IDictionary<string, string> expectedMessages = new Dictionary<string, string>();
		expectedMessages[TASK1_NAME] = TASK1_NAME;
		expectedMessages[TASK2_NAME] = TASK2_NAME;
		expectedMessages[TASK3_NAME] = TASK3_NAME;
		foreach (Task task in tasks)
		{
		  expectedMessages.Remove(task.Name);
		}
		assertEquals(0, expectedMessages.Count);

		// Test with input == 2
		variables["input"] = 2;
		pi = runtimeService.startProcessInstanceByKey("inclusiveGateway", variables);
		tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(2, tasks.Count);
		expectedMessages = new Dictionary<string, string>();
		expectedMessages[TASK2_NAME] = TASK2_NAME;
		expectedMessages[TASK3_NAME] = TASK3_NAME;
		foreach (Task task in tasks)
		{
		  expectedMessages.Remove(task.Name);
		}
		assertEquals(0, expectedMessages.Count);

		// Test with input == 3
		variables["input"] = 3;
		pi = runtimeService.startProcessInstanceByKey("inclusiveGateway", variables);
		tasks = taskService.createTaskQuery().processInstanceId(pi.Id).list();
		assertEquals(1, tasks.Count);
		expectedMessages = new Dictionary<string, string>();
		expectedMessages[TASK3_NAME] = TASK3_NAME;
		foreach (Task task in tasks)
		{
		  expectedMessages.Remove(task.Name);
		}
		assertEquals(0, expectedMessages.Count);

		// Test with input == 4
		variables["input"] = 4;
		try
		{
		  runtimeService.startProcessInstanceByKey("inclusiveGateway", variables);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected since no outgoing sequence flow matches
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterSequentialMultiInstanceSubProcess()
	  public virtual void testJoinAfterSequentialMultiInstanceSubProcess()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		TaskQuery query = taskService.createTaskQuery();

		// when
		Task task = query.taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);

		// then
		assertNull(query.taskDefinitionKey("taskAfterJoin").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterParallelMultiInstanceSubProcess()
	  public virtual void testJoinAfterParallelMultiInstanceSubProcess()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		TaskQuery query = taskService.createTaskQuery();

		// when
		Task task = query.taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);

		// then
		assertNull(query.taskDefinitionKey("taskAfterJoin").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterNestedScopes()
	  public virtual void testJoinAfterNestedScopes()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		TaskQuery query = taskService.createTaskQuery();

		// when
		Task task = query.taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);

		// then
		assertNull(query.taskDefinitionKey("taskAfterJoin").singleResult());
	  }

	  public virtual void testTriggerGatewayWithEnoughArrivedTokens()
	  {
		deployment(Bpmn.createExecutableProcess("process").startEvent().userTask("beforeTask").inclusiveGateway("gw").userTask("afterTask").endEvent().done());

		// given
		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey("process").startBeforeActivity("beforeTask").startBeforeActivity("beforeTask").execute();

		Task task = taskService.createTaskQuery().list().get(0);

		// when
		taskService.complete(task.Id);

		// then
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("beforeTask").activity("afterTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLoopingInclusiveGateways()
	  public virtual void testLoopingInclusiveGateways()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);

		// then
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").activity("inclusiveGw3").done());
	  }

	  public virtual void testRemoveConcurrentExecutionLocalVariablesOnJoin()
	  {
		deployment(Bpmn.createExecutableProcess("process").startEvent().inclusiveGateway("fork").userTask("task1").inclusiveGateway("join").userTask("afterTask").endEvent().moveToNode("fork").userTask("task2").connectTo("join").done());

		// given
		runtimeService.startProcessInstanceByKey("process");

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  runtimeService.setVariableLocal(task.ExecutionId, "var", "value");
		}

		// when
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterEventBasedGateway()
	  public virtual void testJoinAfterEventBasedGateway()
	  {
		// given
		TaskQuery taskQuery = taskService.createTaskQuery();

		runtimeService.startProcessInstanceByKey("process");
		Task task = taskQuery.singleResult();
		taskService.complete(task.Id);

		// assume
		assertNull(taskQuery.singleResult());

		// when
		runtimeService.correlateMessage("foo");

		// then
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterJoin", task.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterEventBasedGatewayInSubProcess()
	  public virtual void testJoinAfterEventBasedGatewayInSubProcess()
	  {
		// given
		TaskQuery taskQuery = taskService.createTaskQuery();

		runtimeService.startProcessInstanceByKey("process");
		Task task = taskQuery.singleResult();
		taskService.complete(task.Id);

		// assume
		assertNull(taskQuery.singleResult());

		// when
		runtimeService.correlateMessage("foo");

		// then
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterJoin", task.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoinAfterEventBasedGatewayContainedInSubProcess()
	  public virtual void testJoinAfterEventBasedGatewayContainedInSubProcess()
	  {
		// given
		TaskQuery taskQuery = taskService.createTaskQuery();

		runtimeService.startProcessInstanceByKey("process");
		Task task = taskQuery.singleResult();
		taskService.complete(task.Id);

		// assume
		assertNull(taskQuery.singleResult());

		// when
		runtimeService.correlateMessage("foo");

		// then
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfterJoin", task.TaskDefinitionKey);
	  }

	}

}