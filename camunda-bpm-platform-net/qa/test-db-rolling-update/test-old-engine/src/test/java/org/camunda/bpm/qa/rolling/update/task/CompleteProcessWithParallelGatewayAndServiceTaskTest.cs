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
namespace org.camunda.bpm.qa.rolling.update.task
{
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// This test ensures that the old engine can complete an
	/// existing process with parallel gateway and service task on the new schema.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithParallelGatewayAndServiceTaskScenario")]
	public class CompleteProcessWithParallelGatewayAndServiceTaskTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.none.1") public void testCompleteProcessWithParallelGateway()
		[ScenarioUnderTest("init.none.1")]
		public virtual void testCompleteProcessWithParallelGateway()
		{
		//given an already started process instance with one user task
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);
		Task task = rule.taskQuery().singleResult();
		Assert.assertNotNull(task);
		//and completed service task
		HistoricActivityInstanceQuery historicActQuery = rule.HistoryService.createHistoricActivityInstanceQuery().activityType("serviceTask").processInstanceId(oldInstance.Id).finished();
		Assert.assertEquals(1, historicActQuery.count());

		//when completing the user task
		rule.TaskService.complete(task.Id);

		//then there exists no more tasks
		//and the process instance is also completed
		Assert.assertEquals(0, rule.taskQuery().count());
		rule.assertScenarioEnded();
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.async.1") public void testCompleteProcessWithParallelGatewayAndSingleUserTask()
	  [ScenarioUnderTest("init.async.1")]
	  public virtual void testCompleteProcessWithParallelGatewayAndSingleUserTask()
	  {
		//given an already started process instance
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);
		//with one user task
		IList<Task> tasks = rule.taskQuery().list();
		Assert.assertEquals(1, tasks.Count);
		//and async service task
		Job job = rule.jobQuery().singleResult();
		Assert.assertNotNull(job);

		//when job is executed
		rule.ManagementService.executeJob(job.Id);
		//and user task completed
		rule.TaskService.complete(rule.taskQuery().singleResult().Id);

		//then there exists no more tasks
		//and the process instance is also completed
		Assert.assertEquals(0, rule.taskQuery().count());
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.async.complete.1") public void testQueryHistoricProcessWithParallelGateway()
	  [ScenarioUnderTest("init.async.complete.1")]
	  public virtual void testQueryHistoricProcessWithParallelGateway()
	  {
		//given an already started process instance
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);
		//with one completed user task
		HistoricTaskInstanceQuery historicTaskQuery = rule.HistoryService.createHistoricTaskInstanceQuery().processInstanceId(oldInstance.Id).finished();
		Assert.assertEquals(1, historicTaskQuery.count());
		//and one async service task
		Job job = rule.jobQuery().singleResult();
		Assert.assertNotNull(job);

		//when job is executed
		rule.ManagementService.executeJob(job.Id);

		//then there exists no more tasks
		//and the process instance is also completed
		Assert.assertEquals(0, rule.taskQuery().count());
		rule.assertScenarioEnded();
	  }

	}

}