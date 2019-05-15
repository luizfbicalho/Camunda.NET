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
namespace org.camunda.bpm.engine.test.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ClockTestUtil.incrementClock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	public class JobExecutorAcquireJobsByPriorityTest : AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void prepareProcessEngineConfiguration()
	  public virtual void prepareProcessEngineConfiguration()
	  {
		configuration.JobExecutorAcquireByPriority = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineConfiguration()
	  public virtual void testProcessEngineConfiguration()
	  {
		assertFalse(configuration.JobExecutorPreferTimerJobs);
		assertFalse(configuration.JobExecutorAcquireByDueDate);
		assertTrue(configuration.JobExecutorAcquireByPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/timerJobPrioProcess.bpmn20.xml" }) public void testAcquisitionByPriority()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/timerJobPrioProcess.bpmn20.xml" })]
	  public virtual void testAcquisitionByPriority()
	  {
		// jobs with priority 10
		startProcess("jobPrioProcess", "task1", 5);

		// jobs with priority 5
		startProcess("jobPrioProcess", "task2", 5);

		// jobs with priority 8
		startProcess("timerJobPrioProcess", "timer1", 5);

		// jobs with priority 4
		startProcess("timerJobPrioProcess", "timer2", 5);

		// make timers due
		incrementClock(61);

		IList<JobEntity> acquirableJobs = findAcquirableJobs();
		assertEquals(20, acquirableJobs.Count);
		for (int i = 0; i < 5; i++)
		{
		  assertEquals(10, acquirableJobs[i].Priority);
		}

		for (int i = 5; i < 10; i++)
		{
		  assertEquals(8, acquirableJobs[i].Priority);
		}

		for (int i = 10; i < 15; i++)
		{
		  assertEquals(5, acquirableJobs[i].Priority);
		}

		for (int i = 15; i < 20; i++)
		{
		  assertEquals(4, acquirableJobs[i].Priority);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml") public void testMixedPriorityAcquisition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml")]
	  public virtual void testMixedPriorityAcquisition()
	  {
		// jobs with priority 10
		startProcess("jobPrioProcess", "task1", 5);

		// jobs with priority 5
		startProcess("jobPrioProcess", "task2", 5);

		// set some job priorities to NULL indicating that they were produced without priorities
	  }

	}

}