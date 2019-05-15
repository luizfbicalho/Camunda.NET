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
namespace org.camunda.bpm.qa.upgrade.scenarios730.compensation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("ParallelMultiInstanceCompensationScenario"), Origin("7.3.0")]
	public class ParallelMultiInstanceCompensationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.multiInstancePartial.1") public void testSingleActivityHandlerMultiInstancePartialCompletion()
	  [ScenarioUnderTest("singleActivityHandler.multiInstancePartial.1")]
	  public virtual void testSingleActivityHandlerMultiInstancePartialCompletion()
	  {
		// given the last multi instance task
		Task lastMiTask = rule.taskQuery().singleResult();

		// when completing it
		rule.TaskService.complete(lastMiTask.Id);

		// then it is possible to throw compensation, compensate the three instances,
		// and finish the process successfully
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(beforeCompensateTask);
		rule.TaskService.complete(beforeCompensateTask.Id);

		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.multiInstancePartial.2") public void testSingleActivityHandlerMultiInstancePartialDeletion()
	  [ScenarioUnderTest("singleActivityHandler.multiInstancePartial.2")]
	  public virtual void testSingleActivityHandlerMultiInstancePartialDeletion()
	  {
		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.multiInstancePartial.3") public void testSingleActivityHandlerMultiInstancePartialActivityInstanceTree()
	  [ScenarioUnderTest("singleActivityHandler.multiInstancePartial.3")]
	  public virtual void testSingleActivityHandlerMultiInstancePartialActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("throwCompensate").activity("undoTask").activity("undoTask").endScope().beginMiBody("userTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.1") public void testSingleActivityHandlerBeforeCompensateCompletion()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.1")]
	  public virtual void testSingleActivityHandlerBeforeCompensateCompletion()
	  {
		// given
		Task beforeCompensateTask = rule.taskQuery().singleResult();

		// when throwing compensation
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to compensate the three instances,
		// and finish the process successfully
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.2") public void testSingleActivityHandlerBeforeCompensateDeletion()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.2")]
	  public virtual void testSingleActivityHandlerBeforeCompensateDeletion()
	  {
		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.3") public void testSingleActivityHandlerBeforeCompensateActivityInstanceTree()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.3")]
	  public virtual void testSingleActivityHandlerBeforeCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("throwCompensate").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.1") public void testSingleActivityHandlerThrowCompensateCompletion()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.1")]
	  public virtual void testSingleActivityHandlerThrowCompensateCompletion()
	  {
		// given active compensation
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		// when completing the compensation handlers
		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		// then it is possible to complete the process successfully
		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);

		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.2") public void testSingleActivityHandlerThrowCompensateDeletion()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.2")]
	  public virtual void testSingleActivityHandlerThrowCompensateDeletion()
	  {
		// it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.3") public void testSingleActivityHandlerThrowCompensateActivityInstanceTree()
	  [ScenarioUnderTest("singleActivityHandler.beforeCompensate.throwCompensate.3")]
	  public virtual void testSingleActivityHandlerThrowCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("throwCompensate").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.multiInstancePartial.1") public void testDefaultHandlerMultiInstancePartialCompletion()
	  [ScenarioUnderTest("defaultHandler.multiInstancePartial.1")]
	  public virtual void testDefaultHandlerMultiInstancePartialCompletion()
	  {
		// given the last multi instance task
		Task lastMiTask = rule.taskQuery().singleResult();

		// when completing it
		rule.TaskService.complete(lastMiTask.Id);

		// then it is possible to throw compensation, compensate the three instances,
		// and finish the process successfully
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(beforeCompensateTask);
		rule.TaskService.complete(beforeCompensateTask.Id);

		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.multiInstancePartial.2") public void testDefaultHandlerMultiInstancePartialDeletion()
	  [ScenarioUnderTest("defaultHandler.multiInstancePartial.2")]
	  public virtual void testDefaultHandlerMultiInstancePartialDeletion()
	  {
		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.multiInstancePartial.3") public void testDefaultHandlerMultiInstancePartialActivityInstanceTree()
	  [ScenarioUnderTest("defaultHandler.multiInstancePartial.3")]
	  public virtual void testDefaultHandlerMultiInstancePartialActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("throwCompensate").beginScope("subProcess", "userTask#multiInstanceBody", "subProcess#multiInstanceBody").beginScope("subProcess").activity("undoTask").endScope().activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.1") public void testDefaultHandlerBeforeCompensateCompletion()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.1")]
	  public virtual void testDefaultHandlerBeforeCompensateCompletion()
	  {
		// given
		Task beforeCompensateTask = rule.taskQuery().singleResult();

		// when throwing compensation
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to compensate the three instances,
		// and finish the process successfully
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.2") public void testDefaultHandlerBeforeCompensateDeletion()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.2")]
	  public virtual void testDefaultHandlerBeforeCompensateDeletion()
	  {
		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.3") public void testDefaultHandlerBeforeCompensateActivityInstanceTree()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.3")]
	  public virtual void testDefaultHandlerBeforeCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("throwCompensate").beginScope("subProcess").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.1") public void testDefaultHandlerThrowCompensateCompletion()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.1")]
	  public virtual void testDefaultHandlerThrowCompensateCompletion()
	  {
		// given active compensation
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		// when completing the compensation handlers
		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		// then it is possible to complete the process successfully
		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);

		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.2") public void testDefaultHandlerThrowCompensateDeletion()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.2")]
	  public virtual void testDefaultHandlerThrowCompensateDeletion()
	  {
		// it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.3") public void testDefaultHandlerThrowCompensateActivityInstanceTree()
	  [ScenarioUnderTest("defaultHandler.beforeCompensate.throwCompensate.3")]
	  public virtual void testDefaultHandlerThrowCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("throwCompensate", "subProcess").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.multiInstancePartial.1") public void testSubProcessHandlerMultiInstancePartialCompletion()
	  [ScenarioUnderTest("subProcessHandler.multiInstancePartial.1")]
	  public virtual void testSubProcessHandlerMultiInstancePartialCompletion()
	  {
		// given the last multi instance task
		Task lastMiTask = rule.taskQuery().singleResult();

		// when completing it
		rule.TaskService.complete(lastMiTask.Id);

		// then it is possible to throw compensation, compensate the three instances,
		// and finish the process successfully
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(beforeCompensateTask);
		rule.TaskService.complete(beforeCompensateTask.Id);

		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.multiInstancePartial.2") public void testSubProcessHandlerMultiInstancePartialDeletion()
	  [ScenarioUnderTest("subProcessHandler.multiInstancePartial.2")]
	  public virtual void testSubProcessHandlerMultiInstancePartialDeletion()
	  {
		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.multiInstancePartial.3") public void testSubProcessHandlerMultiInstancePartialActivityInstanceTree()
	  [ScenarioUnderTest("subProcessHandler.multiInstancePartial.3")]
	  public virtual void testSubProcessHandlerMultiInstancePartialActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task lastMiTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(lastMiTask.Id);

		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("throwCompensate").beginScope("undoSubProcess", "userTask#multiInstanceBody").beginScope("undoSubProcess").activity("undoTask").endScope().activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.1") public void testSubProcessHandlerBeforeCompensateCompletion()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.1")]
	  public virtual void testSubProcessHandlerBeforeCompensateCompletion()
	  {
		// given
		Task beforeCompensateTask = rule.taskQuery().singleResult();

		// when throwing compensation
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to compensate the three instances,
		// and finish the process successfully
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.2") public void testSubProcessHandlerBeforeCompensateDeletion()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.2")]
	  public virtual void testSubProcessHandlerBeforeCompensateDeletion()
	  {
		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.3") public void testSubProcessHandlerBeforeCompensateActivityInstanceTree()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.3")]
	  public virtual void testSubProcessHandlerBeforeCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when throwing compensation
		Task beforeCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensateTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("throwCompensate").beginScope("undoSubProcess").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.1") public void testSubProcessHandlerThrowCompensateCompletion()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.1")]
	  public virtual void testSubProcessHandlerThrowCompensateCompletion()
	  {
		// given active compensation
		IList<Task> miCompensationTasks = rule.taskQuery().list();
		Assert.assertEquals(3, miCompensationTasks.Count);

		// when completing the compensation handlers
		for (int i = 0; i < miCompensationTasks.Count; i++)
		{
		  Assert.assertEquals(3 - i, rule.taskQuery().count());

		  Task compensationTask = miCompensationTasks[i];
		  Assert.assertEquals("undoTask", compensationTask.TaskDefinitionKey);

		  rule.TaskService.complete(compensationTask.Id);
		}

		// then it is possible to complete the process successfully
		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);

		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.2") public void testSubProcessHandlerThrowCompensateDeletion()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.2")]
	  public virtual void testSubProcessHandlerThrowCompensateDeletion()
	  {
		// it is possible to delete the process instance
		rule.RuntimeService.deleteProcessInstance(rule.processInstance().Id, null);

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.3") public void testSubProcessHandlerThrowCompensateActivityInstanceTree()
	  [ScenarioUnderTest("subProcessHandler.beforeCompensate.throwCompensate.3")]
	  public virtual void testSubProcessHandlerThrowCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("throwCompensate", "undoSubProcess").activity("undoTask").activity("undoTask").activity("undoTask").done());
	  }

	}

}