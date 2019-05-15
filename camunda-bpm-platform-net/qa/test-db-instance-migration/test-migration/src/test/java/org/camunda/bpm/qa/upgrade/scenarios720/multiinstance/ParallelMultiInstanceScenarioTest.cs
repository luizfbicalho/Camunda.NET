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
namespace org.camunda.bpm.qa.upgrade.scenarios720.multiinstance
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

	[ScenarioUnderTest("ParallelMultiInstanceSubprocessScenario"), Origin("7.2.0")]
	public class ParallelMultiInstanceScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.1") public void testInitNonInterruptingBoundaryEventCompletionCase1()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.1")]
	  public virtual void testInitNonInterruptingBoundaryEventCompletionCase1()
	  {
		// given
		IList<Task> subProcessTasks = rule.taskQuery().taskDefinitionKey("subProcessTask").list();
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when all instances are completed
		foreach (Task subProcessTask in subProcessTasks)
		{
		  rule.TaskService.complete(subProcessTask.Id);
		}

		// and
		rule.TaskService.complete(afterBoundaryTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.2") public void testInitNonInterruptingBoundaryEventCompletionCase2()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.2")]
	  public virtual void testInitNonInterruptingBoundaryEventCompletionCase2()
	  {
		// given
		IList<Task> subProcessTasks = rule.taskQuery().taskDefinitionKey("subProcessTask").list();
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.TaskService.complete(afterBoundaryTask.Id);

		foreach (Task subProcessTask in subProcessTasks)
		{
		  rule.TaskService.complete(subProcessTask.Id);
		}

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.3") public void testInitNonInterruptingBoundaryEventCompletionCase3()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.3")]
	  public virtual void testInitNonInterruptingBoundaryEventCompletionCase3()
	  {
		// given
		IList<Task> subProcessTasks = rule.taskQuery().taskDefinitionKey("subProcessTask").list();
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when the first instance and the other two instances are completed
		rule.TaskService.complete(subProcessTasks[0].Id);

		rule.TaskService.complete(afterBoundaryTask.Id);
		rule.TaskService.complete(subProcessTasks[1].Id);
		rule.TaskService.complete(subProcessTasks[2].Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.4") public void testInitNonInterruptingBoundaryEventActivityInstanceTree()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.4")]
	  public virtual void testInitNonInterruptingBoundaryEventActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("afterBoundaryTask").beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().beginScope("miSubProcess").activity("subProcessTask").endScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.5") public void testInitNonInterruptingBoundaryEventDeletion()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.5")]
	  public virtual void testInitNonInterruptingBoundaryEventDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.6") public void testInitNonInterruptingBoundaryEventThrowError()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.6")]
	  public virtual void testInitNonInterruptingBoundaryEventThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task miSubprocessTask = rule.taskQuery().taskDefinitionKey("subProcessTask").list().get(0);
		Task afterBoundaryTask = rule.taskQuery().taskDefinitionKey("afterBoundaryTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(miSubprocessTask.Id);

		// then
		Assert.assertEquals(2, rule.taskQuery().count());

		Task escalatedTask = rule.taskQuery().taskDefinitionKey("escalatedTask").singleResult();
		Assert.assertNotNull(escalatedTask);

		// and
		rule.TaskService.complete(escalatedTask.Id);
		rule.TaskService.complete(afterBoundaryTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initNonInterruptingBoundaryEvent.7") public void testInitNonInterruptingBoundaryEventThrowUnhandledException()
	  [ScenarioUnderTest("initNonInterruptingBoundaryEvent.7")]
	  public virtual void testInitNonInterruptingBoundaryEventThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task miSubprocessTask = rule.taskQuery().taskDefinitionKey("subProcessTask").list().get(0);

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(miSubprocessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

	}

}