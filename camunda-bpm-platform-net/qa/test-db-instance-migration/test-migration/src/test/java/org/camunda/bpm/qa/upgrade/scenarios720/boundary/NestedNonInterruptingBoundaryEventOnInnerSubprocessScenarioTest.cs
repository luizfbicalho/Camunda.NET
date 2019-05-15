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
namespace org.camunda.bpm.qa.upgrade.scenarios720.boundary
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ThrowBpmnErrorDelegate = org.camunda.bpm.qa.upgrade.util.ThrowBpmnErrorDelegate;
	using ThrowBpmnErrorDelegateException = org.camunda.bpm.qa.upgrade.util.ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	[ScenarioUnderTest("NestedNonInterruptingBoundaryEventOnInnerSubprocessScenario"), Origin("7.2.0")]
	public class NestedNonInterruptingBoundaryEventOnInnerSubprocessScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.1") public void testInitTimerCompletionCase1()
	  [ScenarioUnderTest("initTimer.1")]
	  public virtual void testInitTimerCompletionCase1()
	  {
		// given
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.messageCorrelation("ReceiveTaskMessage").correlate();
		rule.TaskService.complete(afterBoundaryTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.2") public void testInitTimerCompletionCase2()
	  [ScenarioUnderTest("initTimer.2")]
	  public virtual void testInitTimerCompletionCase2()
	  {
		// given
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.TaskService.complete(afterBoundaryTask.Id);
		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.3") public void testInitTimerActivityInstanceTree()
	  [ScenarioUnderTest("initTimer.3")]
	  public virtual void testInitTimerActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("outerSubProcess").activity("afterBoundaryTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.4") public void testInitTimerDeletion()
	  [ScenarioUnderTest("initTimer.4")]
	  public virtual void testInitTimerDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.5") public void testInitTimerTriggerBoundary()
	  [ScenarioUnderTest("initTimer.5")]
	  public virtual void testInitTimerTriggerBoundary()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when the boundary event is triggered another 2 times
		for (int i = 0; i < 2; i++)
		{
		  Job job = rule.ManagementService.createJobQuery().processInstanceId(instance.Id).singleResult();
		  rule.ManagementService.executeJob(job.Id);
		}

		// and the tasks are completed
		IList<Task> afterBoundaryTasks = rule.taskQuery().list();
		Assert.assertEquals(3, afterBoundaryTasks.Count);

		foreach (Task afterBoundaryTask in afterBoundaryTasks)
		{
		  rule.TaskService.complete(afterBoundaryTask.Id);
		}

		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.6") public void testInitTimerThrowError()
	  [ScenarioUnderTest("initTimer.6")]
	  public virtual void testInitTimerThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		Task afterErrorTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterErrorTask);
		Assert.assertEquals("escalatedTask", afterErrorTask.TaskDefinitionKey);

		// and
		rule.TaskService.complete(afterErrorTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initTimer.7") public void testInitTimerThrowUnhandledException()
	  [ScenarioUnderTest("initTimer.7")]
	  public virtual void testInitTimerThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.messageCorrelation("ReceiveTaskMessage").correlate();
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.1") public void testInitMessageCompletionCase1()
	  [ScenarioUnderTest("initMessage.1")]
	  public virtual void testInitMessageCompletionCase1()
	  {
		// given
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.messageCorrelation("ReceiveTaskMessage").correlate();
		rule.TaskService.complete(afterBoundaryTask.Id);

		// TODO: assert all historic activity instances have end times?

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.2") public void testInitMessageCompletionCase2()
	  [ScenarioUnderTest("initMessage.2")]
	  public virtual void testInitMessageCompletionCase2()
	  {
		// given
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.TaskService.complete(afterBoundaryTask.Id);
		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.3") public void testInitMessageActivityInstanceTree()
	  [ScenarioUnderTest("initMessage.3")]
	  public virtual void testInitMessageActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("outerSubProcess").activity("afterBoundaryTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.4") public void testInitMessageDeletion()
	  [ScenarioUnderTest("initMessage.4")]
	  public virtual void testInitMessageDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.5") public void testInitMessageTriggerBoundary()
	  [ScenarioUnderTest("initMessage.5")]
	  public virtual void testInitMessageTriggerBoundary()
	  {
		// when the boundary event is triggered another 2 times
		for (int i = 0; i < 2; i++)
		{
		  rule.messageCorrelation("BoundaryEventMessage").correlate();
		}

		// and the tasks are completed
		IList<Task> afterBoundaryTasks = rule.taskQuery().list();
		Assert.assertEquals(3, afterBoundaryTasks.Count);

		foreach (Task afterBoundaryTask in afterBoundaryTasks)
		{
		  rule.TaskService.complete(afterBoundaryTask.Id);
		}

		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.6") public void testInitMessageThrowError()
	  [ScenarioUnderTest("initMessage.6")]
	  public virtual void testInitMessageThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.messageCorrelation("ReceiveTaskMessage").correlate();

		// then
		Task afterErrorTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterErrorTask);
		Assert.assertEquals("escalatedTask", afterErrorTask.TaskDefinitionKey);

		// and
		rule.TaskService.complete(afterErrorTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initMessage.7") public void testInitMessageThrowUnhandledException()
	  [ScenarioUnderTest("initMessage.7")]
	  public virtual void testInitMessageThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.messageCorrelation("ReceiveTaskMessage").correlate();
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

	}

}