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

	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	public class JobExecutorAcquireJobsByDueDateNotPriorityTest : AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void prepareProcessEngineConfiguration()
	  public virtual void prepareProcessEngineConfiguration()
	  {
		configuration.JobExecutorAcquireByDueDate = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml") public void testJobPriorityIsNotConsidered()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/jobPrioProcess.bpmn20.xml")]
	  public virtual void testJobPriorityIsNotConsidered()
	  {
		// prio 5
		string instance1 = startProcess("jobPrioProcess", "task2");

		// prio 10
		incrementClock(1);
		string instance2 = startProcess("jobPrioProcess", "task1");

		// prio 5
		incrementClock(1);
		string instance3 = startProcess("jobPrioProcess", "task2");

		// prio 10
		incrementClock(1);
		string instance4 = startProcess("jobPrioProcess", "task1");

		IList<JobEntity> acquirableJobs = findAcquirableJobs();
		assertEquals(4, acquirableJobs.Count);

		assertEquals(5, (int) acquirableJobs[0].Priority);
		assertEquals(instance1, acquirableJobs[0].ProcessInstanceId);
		assertEquals(10, (int) acquirableJobs[1].Priority);
		assertEquals(instance2, acquirableJobs[1].ProcessInstanceId);
		assertEquals(5, (int) acquirableJobs[2].Priority);
		assertEquals(instance3, acquirableJobs[2].ProcessInstanceId);
		assertEquals(10, (int) acquirableJobs[3].Priority);
		assertEquals(instance4, acquirableJobs[3].ProcessInstanceId);
	  }

	}

}