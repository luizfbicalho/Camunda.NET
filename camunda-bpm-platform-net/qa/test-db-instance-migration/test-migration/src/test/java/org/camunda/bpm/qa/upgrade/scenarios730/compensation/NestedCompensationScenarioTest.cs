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
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("NestedCompensationScenario"), Origin("7.3.0")]
	public class NestedCompensationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwCompensate.1") public void testHistory()
	  [ScenarioUnderTest("init.throwCompensate.1")]
	  public virtual void testHistory()
	  {
		// given
		Task compensationHandler = rule.taskQuery().singleResult();

		// when
		rule.TaskService.complete(compensationHandler.Id);

		// then history is written for the remaining activity instances
		HistoricProcessInstance historicProcessInstance = rule.historicProcessInstance();
		Assert.assertNotNull(historicProcessInstance);
		Assert.assertNotNull(historicProcessInstance.EndTime);

		HistoricActivityInstance subProcessInstance = rule.HistoryService.createHistoricActivityInstanceQuery().processInstanceId(historicProcessInstance.Id).activityId("subProcess").singleResult();

		Assert.assertNotNull(subProcessInstance);
		Assert.assertNotNull(subProcessInstance.EndTime);
		Assert.assertEquals(historicProcessInstance.Id, subProcessInstance.ParentActivityInstanceId);

		HistoricActivityInstance compensationThrowInstance = rule.HistoryService.createHistoricActivityInstanceQuery().processInstanceId(historicProcessInstance.Id).activityId("throwCompensate").singleResult();

		Assert.assertNotNull(compensationThrowInstance);
		Assert.assertNotNull(compensationThrowInstance.EndTime);
		Assert.assertEquals(subProcessInstance.Id, compensationThrowInstance.ParentActivityInstanceId);

		HistoricActivityInstance compensationHandlerInstance = rule.HistoryService.createHistoricActivityInstanceQuery().processInstanceId(historicProcessInstance.Id).activityId("undoTask").singleResult();

		Assert.assertNotNull(compensationHandlerInstance);
		Assert.assertNotNull(compensationHandlerInstance.EndTime);
		Assert.assertEquals(subProcessInstance.Id, compensationHandlerInstance.ParentActivityInstanceId);
	  }
	}

}