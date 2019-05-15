﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.qa.upgrade.scenarios730.boundary
{

	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("NonInterruptingBoundaryEventScenario"), Origin("7.3.0")]
	public class NonInterruptingBoundaryEventScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testInitActivityInstanceStatistics()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testInitActivityInstanceStatistics()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		IList<ActivityStatistics> activityStatistics = rule.ManagementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).list();

		// then
		Assert.assertEquals(2, activityStatistics.Count);

		ActivityStatistics outerTaskStatistics = getStatistics(activityStatistics, "outerTask");
		Assert.assertNotNull(outerTaskStatistics);
		Assert.assertEquals("outerTask", outerTaskStatistics.Id);
		Assert.assertEquals(1, outerTaskStatistics.Instances);

		ActivityStatistics afterBoundaryStatistics = getStatistics(activityStatistics, "afterBoundaryTask");
		Assert.assertNotNull(afterBoundaryStatistics);
		Assert.assertEquals("afterBoundaryTask", afterBoundaryStatistics.Id);
		Assert.assertEquals(1, afterBoundaryStatistics.Instances);
	  }

	  protected internal virtual ActivityStatistics getStatistics(IList<ActivityStatistics> activityStatistics, string activityId)
	  {
		foreach (ActivityStatistics statistics in activityStatistics)
		{
		  if (activityId.Equals(statistics.Id))
		  {
			return statistics;
		  }
		}

		return null;
	  }
	}

}