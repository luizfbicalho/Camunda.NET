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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// This test ensures that the old engine can complete an
	/// existing process with parallel gateway and user task on the new schema.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithParallelGatewayScenario")]
	public class CompleteProcessWithParallelGatewayTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.none.1") public void testCompleteProcessWithParallelGateway()
		[ScenarioUnderTest("init.none.1")]
		public virtual void testCompleteProcessWithParallelGateway()
		{
		//given an already started process instance with two user tasks
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);

		IList<Task> tasks = rule.taskQuery().list();
		Assert.assertEquals(2, tasks.Count);

		//when completing the user tasks
		foreach (Task task in tasks)
		{
		  rule.TaskService.complete(task.Id);
		}

		//then there exists no more tasks
		//and the process instance is also completed
		Assert.assertEquals(0, rule.taskQuery().count());
		rule.assertScenarioEnded();
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.complete.one.1") public void testCompleteProcessWithParallelGatewayAndSingleUserTask()
	  [ScenarioUnderTest("init.complete.one.1")]
	  public virtual void testCompleteProcessWithParallelGatewayAndSingleUserTask()
	  {
		//given an already started process instance
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);

		//with one completed user task
		HistoricTaskInstanceQuery historicTaskQuery = rule.HistoryService.createHistoricTaskInstanceQuery().processInstanceId(oldInstance.Id).finished();
		Assert.assertEquals(1, historicTaskQuery.count());

		//and one waiting
		Task task = rule.taskQuery().singleResult();
		Assert.assertNotNull(task);

		//when completing the user task
		rule.TaskService.complete(task.Id);

		//then there exists no more tasks
		Assert.assertEquals(0, rule.taskQuery().count());
		//and two historic tasks
		Assert.assertEquals(2, historicTaskQuery.count());
		//and the process instance is also completed
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.complete.two.1") public void testQueryHistoricProcessWithParallelGateway()
	  [ScenarioUnderTest("init.complete.two.1")]
	  public virtual void testQueryHistoricProcessWithParallelGateway()
	  {
		//given an already finished process instance with parallel gateway and two user tasks
		HistoricProcessInstance historicProcessInstance = rule.historicProcessInstance();

		//when query history
		HistoricTaskInstanceQuery historicTaskQuery = rule.HistoryService.createHistoricTaskInstanceQuery().processInstanceId(historicProcessInstance.Id);

		//then two historic user tasks are returned
		Assert.assertEquals(2, historicTaskQuery.count());
	  }

	}

}