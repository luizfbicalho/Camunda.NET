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

	[ScenarioUnderTest("NestedNonInterruptingEventSubprocessNestedSubprocessScenario"), Origin("7.2.0")]
	public class NestedNonInterruptingEventSubprocessNestedSubprocessTest
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
		Task outerSubProcessTask = rule.taskQuery().taskDefinitionKey("outerSubProcessTask").singleResult();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(outerSubProcessTask.Id);
		rule.TaskService.complete(eventSubprocessTask.Id);

		// then
		Task innerSubprocessTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(innerSubprocessTask);
		rule.TaskService.complete(innerSubprocessTask.Id);

		// and
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.2") public void testInitCompletionCase2()
	  [ScenarioUnderTest("init.2")]
	  public virtual void testInitCompletionCase2()
	  {
		// given
		Task outerSubProcessTask = rule.taskQuery().taskDefinitionKey("outerSubProcessTask").singleResult();
		Task eventSubprocessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(eventSubprocessTask.Id);
		rule.TaskService.complete(outerSubProcessTask.Id);

		// then
		Task innerSubprocessTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(innerSubprocessTask);
		rule.TaskService.complete(innerSubprocessTask.Id);

		// and
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
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("outerSubProcess").activity("outerSubProcessTask").activity("eventSubProcessTask").done());
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
		Task eventSubProcessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(eventSubProcessTask.Id);

		// and
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();
		Assert.assertNotNull(innerSubProcessTask);
		rule.TaskService.complete(innerSubProcessTask.Id);

		// then
		Task afterErrorTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterErrorTask);
		Assert.assertEquals("escalatedTask", afterErrorTask.TaskDefinitionKey);

		// and
		rule.TaskService.complete(afterErrorTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.6") public void testInitThrowUnhandledException()
	  [ScenarioUnderTest("init.6")]
	  public virtual void testInitThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task eventSubProcessTask = rule.taskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(eventSubProcessTask.Id);

		// and
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();
		Assert.assertNotNull(innerSubProcessTask);

		// then
		try
		{
		  rule.TaskService.complete(innerSubProcessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.1") public void testInitInnerSubProcessCompletionCase1()
	  [ScenarioUnderTest("init.innerSubProcess.1")]
	  public virtual void testInitInnerSubProcessCompletionCase1()
	  {
		// given
		Task outerSubProcessTask = rule.taskQuery().taskDefinitionKey("outerSubProcessTask").singleResult();
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(outerSubProcessTask.Id);
		rule.TaskService.complete(innerSubProcessTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.2") public void testInitInnerSubProcessCompletionCase2()
	  [ScenarioUnderTest("init.innerSubProcess.2")]
	  public virtual void testInitInnerSubProcessCompletionCase2()
	  {
		// given
		Task outerSubProcessTask = rule.taskQuery().taskDefinitionKey("outerSubProcessTask").singleResult();
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(innerSubProcessTask.Id);
		rule.TaskService.complete(outerSubProcessTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.3") public void testInitInnerSubProcessActivityInstanceTree()
	  [ScenarioUnderTest("init.innerSubProcess.3")]
	  public virtual void testInitInnerSubProcessActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("outerSubProcess").activity("outerSubProcessTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.4") public void testInitInnerSubProcessDeletion()
	  [ScenarioUnderTest("init.innerSubProcess.4")]
	  public virtual void testInitInnerSubProcessDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.5") public void testInitInnerSubProcessThrowError()
	  [ScenarioUnderTest("init.innerSubProcess.5")]
	  public virtual void testInitInnerSubProcessThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(innerSubProcessTask.Id);

		// then
		Task afterErrorTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterErrorTask);
		Assert.assertEquals("escalatedTask", afterErrorTask.TaskDefinitionKey);

		// and
		rule.TaskService.complete(afterErrorTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.innerSubProcess.6") public void testInitInnerSubProcessThrowUnhandledException()
	  [ScenarioUnderTest("init.innerSubProcess.6")]
	  public virtual void testInitInnerSubProcessThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task innerSubProcessTask = rule.taskQuery().taskDefinitionKey("innerSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(innerSubProcessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

	}

}