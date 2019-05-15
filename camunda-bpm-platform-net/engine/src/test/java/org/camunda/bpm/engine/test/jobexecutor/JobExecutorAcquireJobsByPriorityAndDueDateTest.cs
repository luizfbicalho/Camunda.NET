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

	public class JobExecutorAcquireJobsByPriorityAndDueDateTest : AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void prepareProcessEngineConfiguration()
	  public virtual void prepareProcessEngineConfiguration()
	  {
		configuration.JobExecutorAcquireByPriority = true;
		configuration.JobExecutorAcquireByDueDate = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineConfiguration()
	  public virtual void testProcessEngineConfiguration()
	  {
		assertFalse(configuration.JobExecutorPreferTimerJobs);
		assertTrue(configuration.JobExecutorAcquireByDueDate);
		assertTrue(configuration.JobExecutorAcquireByPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/timerJobPrioProcess.bpmn20.xml" }) public void testAcquisitionByPriorityAndDueDate()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/timerJobPrioProcess.bpmn20.xml" })]
	  public virtual void testAcquisitionByPriorityAndDueDate()
	  {
		// job with priority 10
		string instance1 = startProcess("jobPrioProcess", "task1");

		// job with priority 5
		incrementClock(1);
		string instance2 = startProcess("jobPrioProcess", "task2");

		// job with priority 10
		incrementClock(1);
		string instance3 = startProcess("jobPrioProcess", "task1");

		// job with priority 5
		incrementClock(1);
		string instance4 = startProcess("jobPrioProcess", "task2");

		IList<JobEntity> acquirableJobs = findAcquirableJobs();
		assertEquals(4, acquirableJobs.Count);
		assertEquals(instance1, acquirableJobs[0].ProcessInstanceId);
		assertEquals(instance3, acquirableJobs[1].ProcessInstanceId);
		assertEquals(instance2, acquirableJobs[2].ProcessInstanceId);
		assertEquals(instance4, acquirableJobs[3].ProcessInstanceId);
	  }

	}

}