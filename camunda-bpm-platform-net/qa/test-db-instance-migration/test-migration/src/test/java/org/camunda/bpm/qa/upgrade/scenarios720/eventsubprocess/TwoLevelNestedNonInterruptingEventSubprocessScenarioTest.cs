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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("TwoLevelNestedNonInterruptingEventSubprocessScenario"), Origin("7.2.0")]
	public class TwoLevelNestedNonInterruptingEventSubprocessScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.1") public void testInitLevel1CompletionCase1()
	  [ScenarioUnderTest("initLevel1.1")]
	  public virtual void testInitLevel1CompletionCase1()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();

		// when
		rule.TaskService.complete(outerTask.Id);
		rule.TaskService.complete(innerTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.2") public void testInitLevel1CompletionCase2()
	  [ScenarioUnderTest("initLevel1.2")]
	  public virtual void testInitLevel1CompletionCase2()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();

		// when
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(outerTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.3") public void testInitLevel1CompletionCase3()
	  [ScenarioUnderTest("initLevel1.3")]
	  public virtual void testInitLevel1CompletionCase3()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();

		// when
		rule.messageCorrelation("InnerEventSubProcessMessage").correlate();

		// then
		Assert.assertEquals(3, rule.taskQuery().count());

		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();
		Assert.assertNotNull(innerEventSubprocessTask);

		// and
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(innerEventSubprocessTask.Id);
		rule.TaskService.complete(outerTask.Id);

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.4") public void testInitLevel1ActivityInstanceTree()
	  [ScenarioUnderTest("initLevel1.4")]
	  public virtual void testInitLevel1ActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("subProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.5") public void testInitLevel1Deletion()
	  [ScenarioUnderTest("initLevel1.5")]
	  public virtual void testInitLevel1Deletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

	  [ScenarioUnderTest("initLevel1.6")]
	  public virtual void testInitLevel1ThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		rule.messageCorrelation("InnerEventSubProcessMessage").correlate();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(innerEventSubprocessTask.Id);

		// then
		Task escalatedTask = rule.taskQuery().singleResult();
		Assert.assertEquals("escalatedTask", escalatedTask.TaskDefinitionKey);
		Assert.assertNotNull(escalatedTask);

		rule.TaskService.complete(escalatedTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.7") public void testInitLevel1ThrowUnhandledException()
	  [ScenarioUnderTest("initLevel1.7")]
	  public virtual void testInitLevel1ThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		rule.messageCorrelation("InnerEventSubProcessMessage").correlate();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(innerEventSubprocessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.1") public void testInitLevel1InitLevel2CompletionCase1()
	  [ScenarioUnderTest("initLevel1.initLevel2.1")]
	  public virtual void testInitLevel1InitLevel2CompletionCase1()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(outerTask.Id);
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(innerEventSubprocessTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.2") public void testInitLevel1InitLevel2CompletionCase2()
	  [ScenarioUnderTest("initLevel1.initLevel2.2")]
	  public virtual void testInitLevel1InitLevel2CompletionCase2()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.TaskService.complete(innerEventSubprocessTask.Id);
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(outerTask.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.3") public void testInitLevel1InitLevel2CompletionCase3()
	  [ScenarioUnderTest("initLevel1.initLevel2.3")]
	  public virtual void testInitLevel1InitLevel2CompletionCase3()
	  {
		// given
		Task outerTask = rule.taskQuery().taskDefinitionKey("outerTask").singleResult();
		Task innerTask = rule.taskQuery().taskDefinitionKey("subProcessTask").singleResult();

		// when (the inner subprocess is triggered another time)
		rule.messageCorrelation("InnerEventSubProcessMessage").correlate();

		// then
		Assert.assertEquals(4, rule.taskQuery().count());

		IList<Task> innerEventSubprocessTasks = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").list();
		Assert.assertEquals(2, innerEventSubprocessTasks.Count);

		// and
		rule.TaskService.complete(innerTask.Id);
		rule.TaskService.complete(innerEventSubprocessTasks[0].Id);
		rule.TaskService.complete(outerTask.Id);
		rule.TaskService.complete(innerEventSubprocessTasks[1].Id);

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.4") public void testInitLevel1InitLevel2ActivityInstanceTree()
	  [ScenarioUnderTest("initLevel1.initLevel2.4")]
	  public virtual void testInitLevel1InitLevel2ActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("subProcessTask").activity("innerEventSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.5") public void testInitLevel1InitLevel2Deletion()
	  [ScenarioUnderTest("initLevel1.initLevel2.5")]
	  public virtual void testInitLevel1InitLevel2Deletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

	  [ScenarioUnderTest("initLevel1.initLevel2.6")]
	  public virtual void testInitLevel1InitLevel2ThrowError()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.ERROR_INDICATOR_VARIABLE, true);
		rule.TaskService.complete(innerEventSubprocessTask.Id);

		// then
		Task escalatedTask = rule.taskQuery().singleResult();
		Assert.assertEquals("escalatedTask", escalatedTask.TaskDefinitionKey);
		Assert.assertNotNull(escalatedTask);

		rule.TaskService.complete(escalatedTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initLevel1.initLevel2.7") public void testInitLevel1InitLevel2ThrowUnhandledException()
	  [ScenarioUnderTest("initLevel1.initLevel2.7")]
	  public virtual void testInitLevel1InitLevel2ThrowUnhandledException()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		Task innerEventSubprocessTask = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask").singleResult();

		// when
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_INDICATOR_VARIABLE, true);
		rule.RuntimeService.setVariable(instance.Id, ThrowBpmnErrorDelegate.EXCEPTION_MESSAGE_VARIABLE, "unhandledException");

		// then
		try
		{
		  rule.TaskService.complete(innerEventSubprocessTask.Id);
		  Assert.fail("should throw a ThrowBpmnErrorDelegateException");

		}
		catch (ThrowBpmnErrorDelegate.ThrowBpmnErrorDelegateException e)
		{
		  Assert.assertEquals("unhandledException", e.Message);
		}
	  }
	}

}