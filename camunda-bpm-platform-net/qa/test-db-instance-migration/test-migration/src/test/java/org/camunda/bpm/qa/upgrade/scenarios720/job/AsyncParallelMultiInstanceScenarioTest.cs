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
namespace org.camunda.bpm.qa.upgrade.scenarios720.job
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	[ScenarioUnderTest("AsyncParallelMultiInstanceScenario"), Origin("7.2.0")]
	public class AsyncParallelMultiInstanceScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeSubprocess.1") public void testInitAsyncBeforeSubprocessCompletion()
	  [ScenarioUnderTest("initAsyncBeforeSubprocess.1")]
	  public virtual void testInitAsyncBeforeSubprocessCompletion()
	  {
		// given
		Job asyncJob = rule.jobQuery().singleResult();

		// when
		rule.ManagementService.executeJob(asyncJob.Id);

		// then the process can be completed successfully
		IList<Task> subProcessTasks = rule.taskQuery().list();
		Assert.assertEquals(3, subProcessTasks.Count);

		foreach (Task subProcessTask in subProcessTasks)
		{
		  rule.TaskService.complete(subProcessTask.Id);
		}

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeSubprocess.2") public void testInitAsyncBeforeSubprocessActivityInstanceTree()
	  [ScenarioUnderTest("initAsyncBeforeSubprocess.2")]
	  public virtual void testInitAsyncBeforeSubprocessActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).transition("miSubProcess").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeSubprocess.3") public void testInitAsyncBeforeSubprocessDeletion()
	  [ScenarioUnderTest("initAsyncBeforeSubprocess.3")]
	  public virtual void testInitAsyncBeforeSubprocessDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

	  /// <summary>
	  /// Note: this test is not really isolated since the job
	  /// definition is migrated when the process definition is accessed the first time.
	  /// This might happen already before this test case is executed.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeSubprocess.4") public void testInitAsyncBeforeSubprocessJobDefinition()
	  [ScenarioUnderTest("initAsyncBeforeSubprocess.4")]
	  public virtual void testInitAsyncBeforeSubprocessJobDefinition()
	  {
		// when the process is redeployed into the cache (instantiation should trigger that)
		rule.RuntimeService.startProcessInstanceByKey("AsyncBeforeParallelMultiInstanceSubprocess");

		// then the old job definition referencing "miSubProcess" has been migrated
		JobDefinition asyncJobDefinition = rule.jobDefinitionQuery().singleResult();
		Assert.assertEquals("miSubProcess#multiInstanceBody", asyncJobDefinition.ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeTask.1") public void testInitAsyncBeforeTaskCompletion()
	  [ScenarioUnderTest("initAsyncBeforeTask.1")]
	  public virtual void testInitAsyncBeforeTaskCompletion()
	  {
		// given
		Job asyncJob = rule.jobQuery().singleResult();

		// when
		rule.ManagementService.executeJob(asyncJob.Id);

		// then the process can be completed successfully
		IList<Task> subProcessTasks = rule.taskQuery().list();
		Assert.assertEquals(3, subProcessTasks.Count);

		foreach (Task subProcessTask in subProcessTasks)
		{
		  rule.TaskService.complete(subProcessTask.Id);
		}

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeTask.2") public void testInitAsyncBeforeTaskActivityInstanceTree()
	  [ScenarioUnderTest("initAsyncBeforeTask.2")]
	  public virtual void testInitAsyncBeforeTaskActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).transition("miTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeTask.3") public void testInitAsyncBeforeTaskDeletion()
	  [ScenarioUnderTest("initAsyncBeforeTask.3")]
	  public virtual void testInitAsyncBeforeTaskDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

	  /// <summary>
	  /// Note: this test is not really isolated since the job
	  /// definition is migrated when the process definition is accessed the first time.
	  /// This might happen already before this test case is executed.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initAsyncBeforeTask.4") public void testInitAsyncBeforeTaskJobDefinition()
	  [ScenarioUnderTest("initAsyncBeforeTask.4")]
	  public virtual void testInitAsyncBeforeTaskJobDefinition()
	  {
		// when the process is redeployed into the cache (instantiation should trigger that)
		rule.RuntimeService.startProcessInstanceByKey("AsyncBeforeParallelMultiInstanceTask");

		// then the old job definition referencing "miSubProcess" has been migrated
		JobDefinition asyncJobDefinition = rule.jobDefinitionQuery().singleResult();
		Assert.assertEquals("miTask#multiInstanceBody", asyncJobDefinition.ActivityId);
	  }

	}

}