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
namespace org.camunda.bpm.qa.upgrade.scenarios720.eventsubprocess
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ThrowBpmnErrorDelegate = org.camunda.bpm.qa.upgrade.util.ThrowBpmnErrorDelegate;
	using ThrowBpmnErrorDelegateException = org.camunda.bpm.qa.upgrade.util.ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	[ScenarioUnderTest("NestedParallelNonInterruptingEventSubprocessScenario"), Origin("7.2.0")]
	public class NestedParallelNonInterruptingEventSubprocessScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testInitCompletionCase1()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testInitCompletionCase1()
	  {
		// given
		Task innerTask = rule.taskQuery().taskDefinitionKey("innerTask").singleResult();
		Task eventSubprocessTask1 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();
		Task eventSubprocessTask2 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask2").singleResult();

		// when
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(eventSubprocessTask1.Id);
		rule.TaskService.complete(eventSubprocessTask2.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.2") public void testInitCompletionCase2()
	  [ScenarioUnderTest("init.2")]
	  public virtual void testInitCompletionCase2()
	  {
		// given
		Task innerTask = rule.taskQuery().taskDefinitionKey("innerTask").singleResult();
		Task eventSubprocessTask1 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();
		Task eventSubprocessTask2 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask2").singleResult();

		// when
		rule.TaskService.complete(eventSubprocessTask1.Id);
		rule.TaskService.complete(eventSubprocessTask2.Id);
		rule.TaskService.complete(innerTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.3") public void testInitActivityInstanceTree()
	  [ScenarioUnderTest("init.3")]
	  public virtual void testInitActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("innerTask").activity("eventSubProcessTask1").activity("eventSubProcessTask2").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.4") public void testInitDeletion()
	  [ScenarioUnderTest("init.4")]
	  public virtual void testInitDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.5") public void testInitThrowError()
	  [ScenarioUnderTest("init.5")]
	  public virtual void testInitThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(eventSubprocessTask.Id);

		// then
		Task escalatedTask = rule.taskQuery().singleResult();
		Assert.assertEquals("escalatedTask", escalatedTask.TaskDefinitionKey);
		Assert.assertNotNull(escalatedTask);

		rule.TaskService.complete(escalatedTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.6") public void testInitThrowUnhandledException()
	  [ScenarioUnderTest("init.6")]
	  public virtual void testInitThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(eventSubprocessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerTask.1") public void testInitInnerTaskCompletion()
	  [ScenarioUnderTest("init.innerTask.1")]
	  public virtual void testInitInnerTaskCompletion()
	  {
		// given
		Task eventSubprocessTask1 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();
		Task eventSubprocessTask2 = rule.taskQuery().taskDefinitionKey("eventSubProcessTask2").singleResult();

		// when
		rule.TaskService.complete(eventSubprocessTask1.Id);
		rule.TaskService.complete(eventSubprocessTask2.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerTask.2") public void testInitInnerTaskActivityInstanceTree()
	  [ScenarioUnderTest("init.innerTask.2")]
	  public virtual void testInitInnerTaskActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("eventSubProcessTask1").activity("eventSubProcessTask2").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerTask.3") public void testInitInnerTaskDeletion()
	  [ScenarioUnderTest("init.innerTask.3")]
	  public virtual void testInitInnerTaskDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerTask.4") public void testInitInnerTaskThrowError()
	  [ScenarioUnderTest("init.innerTask.4")]
	  public virtual void testInitInnerTaskThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(eventSubprocessTask.Id);

		// then
		Task escalatedTask = rule.taskQuery().singleResult();
		Assert.assertEquals("escalatedTask", escalatedTask.TaskDefinitionKey);
		Assert.assertNotNull(escalatedTask);

		rule.TaskService.complete(escalatedTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerTask.5") public void testInitInnerTaskThrowUnhandledException()
	  [ScenarioUnderTest("init.innerTask.5")]
	  public virtual void testInitInnerTaskThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask1").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(eventSubprocessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }



	}

}