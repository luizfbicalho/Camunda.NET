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
namespace org.camunda.bpm.qa.rolling.update.externalTask
{
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithExternalTaskScenario")]
	public class CompleteProcessWithExternalTaskTest : AbstractRollingUpdateTestCase
	{

	  public const long LOCK_TIME = 5 * 60 * 1000;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteProcessWithExternalTask()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testCompleteProcessWithExternalTask()
	  {
		//given process with external task
		string buisnessKey = rule.BuisnessKey;
		IList<LockedExternalTask> externalTasks = rule.ExternalTaskService.fetchAndLock(1, buisnessKey).topic(buisnessKey, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);

		//when external task is completed
		rule.ExternalTaskService.complete(externalTasks[0].Id, buisnessKey);

		//then process instance is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.fetch.1") public void testCompleteProcessWithFetchedExternalTask() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [ScenarioUnderTest("init.fetch.1")]
	  public virtual void testCompleteProcessWithFetchedExternalTask()
	  {
		//given process with locked external task
		string buisnessKey = rule.BuisnessKey;
		ExternalTask task = rule.ExternalTaskService.createExternalTaskQuery().locked().topicName(buisnessKey).workerId(buisnessKey).singleResult();
		Assert.assertNotNull(task);

		//when external task is completed
		rule.ExternalTaskService.complete(task.Id, buisnessKey);

		//then no locked external task with worker id exists
		task = rule.ExternalTaskService.createExternalTaskQuery().locked().topicName(buisnessKey).workerId(buisnessKey).singleResult();
		assertNull(task);
		rule.assertScenarioEnded();
	  }
	}

}