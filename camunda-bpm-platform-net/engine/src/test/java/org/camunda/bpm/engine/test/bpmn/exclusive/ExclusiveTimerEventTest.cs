using System;

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
namespace org.camunda.bpm.engine.test.bpmn.exclusive
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	public class ExclusiveTimerEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchingTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCatchingTimerEvent()
	  {

		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		// After process start, there should be 3 timers created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("exclusiveTimers");
		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		assertEquals(3, jobQuery.count());

		// After setting the clock to time '50minutes and 5 seconds', the timers should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((50 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);

		assertEquals(0, jobQuery.count());
		assertProcessEnded(pi.ProcessInstanceId);


	  }


	}
}