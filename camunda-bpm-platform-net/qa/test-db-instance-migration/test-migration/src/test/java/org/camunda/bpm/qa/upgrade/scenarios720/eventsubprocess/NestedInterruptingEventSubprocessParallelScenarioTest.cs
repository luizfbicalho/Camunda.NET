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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using CompleteTaskThread = org.camunda.bpm.qa.upgrade.util.CompleteTaskThread;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("NestedInterruptingEventSubprocessParallelScenario"), Origin("7.2.0")]
	public class NestedInterruptingEventSubprocessParallelScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testInitSynchronization()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testInitSynchronization()
	  {
		// given
		Task eventSubProcessTask1 = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask1").singleResult();
		Task eventSubProcessTask2 = rule.taskQuery().taskDefinitionKey("innerEventSubProcessTask2").singleResult();

		// when
		CompleteTaskThread completeTaskThread1 = new CompleteTaskThread(eventSubProcessTask1.Id, (ProcessEngineConfigurationImpl) rule.ProcessEngine.ProcessEngineConfiguration);

		CompleteTaskThread completeTaskThread2 = new CompleteTaskThread(eventSubProcessTask2.Id, (ProcessEngineConfigurationImpl) rule.ProcessEngine.ProcessEngineConfiguration);

		completeTaskThread1.startAndWaitUntilControlIsReturned();
		completeTaskThread2.startAndWaitUntilControlIsReturned();

		completeTaskThread1.proceedAndWaitTillDone();
		completeTaskThread2.proceedAndWaitTillDone();

		// then
		Assert.assertNull(completeTaskThread1.Exception);
		Assert.assertNotNull(completeTaskThread2.Exception);
	  }
	}

}