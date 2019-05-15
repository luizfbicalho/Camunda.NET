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
namespace org.camunda.bpm.engine.test.bpmn.tasklistener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderTaskListener = org.camunda.bpm.engine.test.bpmn.tasklistener.util.RecorderTaskListener;
	using RecordedTaskEvent = org.camunda.bpm.engine.test.bpmn.tasklistener.util.RecorderTaskListener.RecordedTaskEvent;
	using TaskDeleteListener = org.camunda.bpm.engine.test.bpmn.tasklistener.util.TaskDeleteListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TaskListenerTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskListenerTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"}) public void testTaskCreateListener()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"})]
	  public virtual void testTaskCreateListener()
	  {
		runtimeService.startProcessInstanceByKey("taskListenerProcess");
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Schedule meeting", task.Name);
		assertEquals("TaskCreateListener is listening!", task.Description);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"}) public void testTaskCompleteListener()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"})]
	  public virtual void testTaskCompleteListener()
	  {
		TaskDeleteListener.clear();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerProcess");
		assertEquals(null, runtimeService.getVariable(processInstance.Id, "greeting"));
		assertEquals(null, runtimeService.getVariable(processInstance.Id, "expressionValue"));

		// Completing first task will change the description
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// Check that the completion did not execute the delete listener
		assertEquals(0, TaskDeleteListener.eventCounter);
		assertNull(TaskDeleteListener.lastTaskDefinitionKey);
		assertNull(TaskDeleteListener.lastDeleteReason);

		assertEquals("Hello from The Process", runtimeService.getVariable(processInstance.Id, "greeting"));
		assertEquals("Act", runtimeService.getVariable(processInstance.Id, "shortName"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"}) public void testTaskDeleteListenerByProcessDeletion()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"})]
	  public virtual void testTaskDeleteListenerByProcessDeletion()
	  {
		TaskDeleteListener.clear();
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerProcess");

		assertEquals(0, TaskDeleteListener.eventCounter);
		assertNull(TaskDeleteListener.lastTaskDefinitionKey);
		assertNull(TaskDeleteListener.lastDeleteReason);

		// delete process instance to delete task
		Task task = taskService.createTaskQuery().singleResult();
		runtimeService.deleteProcessInstance(processInstance.ProcessInstanceId, "test delete task listener");

		assertEquals(1, TaskDeleteListener.eventCounter);
		assertEquals(task.TaskDefinitionKey, TaskDeleteListener.lastTaskDefinitionKey);
		assertEquals("test delete task listener", TaskDeleteListener.lastDeleteReason);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"}) public void testTaskDeleteListenerByBoundaryEvent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"})]
	  public virtual void testTaskDeleteListenerByBoundaryEvent()
	  {
		TaskDeleteListener.clear();
		runtimeService.startProcessInstanceByKey("taskListenerProcess");

		assertEquals(0, TaskDeleteListener.eventCounter);
		assertNull(TaskDeleteListener.lastTaskDefinitionKey);
		assertNull(TaskDeleteListener.lastDeleteReason);

		// correlate message to delete task
		Task task = taskService.createTaskQuery().singleResult();
		runtimeService.correlateMessage("message");

		assertEquals(1, TaskDeleteListener.eventCounter);
		assertEquals(task.TaskDefinitionKey, TaskDeleteListener.lastTaskDefinitionKey);
		assertEquals("deleted", TaskDeleteListener.lastDeleteReason);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"}) public void testTaskListenerWithExpression()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.bpmn20.xml"})]
	  public virtual void testTaskListenerWithExpression()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerProcess");
		assertEquals(null, runtimeService.getVariable(processInstance.Id, "greeting2"));

		// Completing first task will change the description
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertEquals("Write meeting notes", runtimeService.getVariable(processInstance.Id, "greeting2"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testScriptListener()
	  public virtual void testScriptListener()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "create"));

		taskService.setAssignee(task.Id, "test");
		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "assignment"));

		taskService.complete(task.Id);
		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "complete"));

		task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		if (processEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("delete").singleResult();
		  assertNotNull(variable);
		  assertTrue((bool?) variable.Value);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.testScriptResourceListener.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/tasklistener/taskListener.groovy" }) public void testScriptResourceListener()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.testScriptResourceListener.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/tasklistener/taskListener.groovy" })]
	  public virtual void testScriptResourceListener()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "create"));

		taskService.setAssignee(task.Id, "test");
		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "assignment"));

		taskService.complete(task.Id);
		assertTrue((bool?) runtimeService.getVariable(processInstance.Id, "complete"));

		task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		if (processEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstance variable = historyService.createHistoricVariableInstanceQuery().variableName("delete").singleResult();
		  assertNotNull(variable);
		  assertTrue((bool?) variable.Value);
		}
	  }


	  public class TaskCreateListener : TaskListener
	  {
		public virtual void notify(DelegateTask delegateTask)
		{
		  delegateTask.complete();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskInCreateTaskListener()
	  public virtual void testCompleteTaskInCreateTaskListener()
	  {
		// given process with user task and task create listener
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("startToEnd").startEvent().userTask().camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, typeof(TaskCreateListener).FullName).name("userTask").endEvent().done();

		testRule.deploy(modelInstance);

		// when process is started and user task completed in task create listener
		runtimeService.startProcessInstanceByKey("startToEnd");

		// then task is successfully completed without an exception
		assertNull(taskService.createTaskQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskInCreateTaskListenerWithIdentityLinks()
	  public virtual void testCompleteTaskInCreateTaskListenerWithIdentityLinks()
	  {
		// given process with user task, identity links and task create listener
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("startToEnd").startEvent().userTask().camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, typeof(TaskCreateListener).FullName).name("userTask").camundaCandidateUsers(Arrays.asList(new string[]{"users1", "user2"})).camundaCandidateGroups(Arrays.asList(new string[]{"group1", "group2"})).endEvent().done();

		testRule.deploy(modelInstance);

		// when process is started and user task completed in task create listener
		runtimeService.startProcessInstanceByKey("startToEnd");

		// then task is successfully completed without an exception
		assertNull(taskService.createTaskQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityInstanceIdOnDeleteInCalledProcess()
	  public virtual void testActivityInstanceIdOnDeleteInCalledProcess()
	  {
		// given
		RecorderTaskListener.clear();

		BpmnModelInstance callActivityProcess = Bpmn.createExecutableProcess("calling").startEvent().callActivity().calledElement("called").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance calledProcess = Bpmn.createExecutableProcess("called").startEvent().userTask().camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, typeof(RecorderTaskListener).FullName).camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, typeof(RecorderTaskListener).FullName).endEvent().done();

		testRule.deploy(callActivityProcess, calledProcess);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("calling");

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		IList<RecorderTaskListener.RecordedTaskEvent> recordedEvents = RecorderTaskListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);
		string createActivityInstanceId = recordedEvents[0].ActivityInstanceId;
		string deleteActivityInstanceId = recordedEvents[1].ActivityInstanceId;

		assertEquals(createActivityInstanceId, deleteActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAccessOnDeleteInCalledProcess()
	  public virtual void testVariableAccessOnDeleteInCalledProcess()
	  {
		// given
		VariablesCollectingListener.reset();

		BpmnModelInstance callActivityProcess = Bpmn.createExecutableProcess("calling").startEvent().callActivity().camundaIn("foo", "foo").calledElement("called").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance calledProcess = Bpmn.createExecutableProcess("called").startEvent().userTask().camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, typeof(VariablesCollectingListener).FullName).endEvent().done();

		testRule.deploy(callActivityProcess, calledProcess);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("calling", Variables.createVariables().putValue("foo", "bar"));

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		VariableMap collectedVariables = VariablesCollectingListener.CollectedVariables;
		assertNotNull(collectedVariables);
		assertEquals(1, collectedVariables.size());
		assertEquals("bar", collectedVariables.get("foo"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskOnCreateListenerWithFollowingCallActivity()
	  public virtual void testCompleteTaskOnCreateListenerWithFollowingCallActivity()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance subProcess = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("subProc").startEvent().userTask("calledTask").endEvent().done();
		BpmnModelInstance subProcess = Bpmn.createExecutableProcess("subProc").startEvent().userTask("calledTask").endEvent().done();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance instance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("mainProc").startEvent().userTask("mainTask").camundaTaskListenerClass(org.camunda.bpm.engine.delegate.TaskListener_Fields.EVENTNAME_CREATE, CreateTaskListener.class.getName()).callActivity().calledElement("subProc").endEvent().done();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("mainProc").startEvent().userTask("mainTask").camundaTaskListenerClass(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, typeof(CreateTaskListener).FullName).callActivity().calledElement("subProc").endEvent().done();

		testRule.deploy(subProcess);
		testRule.deploy(instance);

		engineRule.RuntimeService.startProcessInstanceByKey("mainProc");
		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		Assert.assertEquals(task.TaskDefinitionKey, "calledTask");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssignmentTaskListenerWhenSavingTask()
	  public virtual void testAssignmentTaskListenerWhenSavingTask()
	  {
		AssignmentTaskListener.reset();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance process = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("process").startEvent().userTask("task").camundaTaskListenerClass("assignment", AssignmentTaskListener.class).endEvent().done();
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().userTask("task").camundaTaskListenerClass("assignment", typeof(AssignmentTaskListener)).endEvent().done();

		testRule.deploy(process);
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// given
		Task task = engineRule.TaskService.createTaskQuery().singleResult();

		// when
		task.Assignee = "gonzo";
		engineRule.TaskService.saveTask(task);

		// then
		assertEquals(1, AssignmentTaskListener.eventCounter);
	  }

	  public class VariablesCollectingListener : TaskListener
	  {

		protected internal static VariableMap collectedVariables;

		public static VariableMap CollectedVariables
		{
			get
			{
			  return collectedVariables;
			}
		}

		public static void reset()
		{
		  collectedVariables = null;
		}

		public virtual void notify(DelegateTask delegateTask)
		{
		  collectedVariables = delegateTask.VariablesTyped;
		}

	  }

	  public class CreateTaskListener : TaskListener
	  {

		  public virtual void notify(DelegateTask delegateTask)
		  {
			  delegateTask.ProcessEngineServices.TaskService.complete(delegateTask.Id);
		  }
	  }

	  public class AssignmentTaskListener : TaskListener
	  {

		public static int eventCounter = 0;

		public virtual void notify(DelegateTask delegateTask)
		{
		  eventCounter++;
		}

		public static void reset()
		{
		  eventCounter = 0;
		}

	  }
	}

}