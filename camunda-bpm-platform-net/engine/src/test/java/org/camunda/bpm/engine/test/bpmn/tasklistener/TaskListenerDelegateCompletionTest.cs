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
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class TaskListenerDelegateCompletionTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskListenerDelegateCompletionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string COMPLETE_LISTENER = "org.camunda.bpm.engine.test.bpmn.tasklistener.util.CompletingTaskListener";
	  protected internal const string TASK_LISTENER_PROCESS = "taskListenerProcess";
	  protected internal const string ACTIVITY_ID = "UT";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		if (runtimeService.createProcessInstanceQuery().count() > 0)
		{
		  runtimeService.deleteProcessInstance(runtimeService.createProcessInstanceQuery().singleResult().Id,null,true);
		}
	  }


	  protected internal static BpmnModelInstance setupProcess(string eventName)
	  {
		return Bpmn.createExecutableProcess(TASK_LISTENER_PROCESS).startEvent().userTask(ACTIVITY_ID).camundaTaskListenerClass(eventName,COMPLETE_LISTENER).endEvent().done();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletionIsPossibleOnCreation()
	  public virtual void testCompletionIsPossibleOnCreation()
	  {
		//given
		createProcessWithListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE);

		//when
		runtimeService.startProcessInstanceByKey(TASK_LISTENER_PROCESS);

		//then
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletionIsPossibleOnAssignment()
	  public virtual void testCompletionIsPossibleOnAssignment()
	  {
		//given
		createProcessWithListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT);

		//when
		runtimeService.startProcessInstanceByKey(TASK_LISTENER_PROCESS);
		Task task = taskService.createTaskQuery().singleResult();
		taskService.setAssignee(task.Id,"test assignee");

		//then
		task = taskService.createTaskQuery().singleResult();
		assertThat(task, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletionIsNotPossibleOnComplete()
	  public virtual void testCompletionIsNotPossibleOnComplete()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("invalid task state"));
		//given
		createProcessWithListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE);

		//when
		runtimeService.startProcessInstanceByKey(TASK_LISTENER_PROCESS);
		Task task = taskService.createTaskQuery().singleResult();

		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletionIsNotPossibleOnDelete()
	  public virtual void testCompletionIsNotPossibleOnDelete()
	  {
		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage(containsString("invalid task state"));

		//given
		createProcessWithListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE);

		//when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(TASK_LISTENER_PROCESS);
		runtimeService.deleteProcessInstance(processInstance.Id,"test reason");
	  }

	  protected internal virtual void createProcessWithListener(string eventName)
	  {
		BpmnModelInstance bpmnModelInstance = setupProcess(eventName);
		testHelper.deploy(bpmnModelInstance);
	  }

	}

}