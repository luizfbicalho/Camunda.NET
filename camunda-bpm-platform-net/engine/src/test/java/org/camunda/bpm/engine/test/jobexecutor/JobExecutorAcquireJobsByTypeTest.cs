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
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ClockTestUtil.incrementClock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	public class JobExecutorAcquireJobsByTypeTest : AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void prepareProcessEngineConfiguration()
	  public virtual void prepareProcessEngineConfiguration()
	  {
		configuration.JobExecutorPreferTimerJobs = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineConfiguration()
	  public virtual void testProcessEngineConfiguration()
	  {
		assertTrue(configuration.JobExecutorPreferTimerJobs);
		assertFalse(configuration.JobExecutorAcquireByDueDate);
		assertFalse(configuration.JobExecutorAcquireByPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testMessageJobHasNoDueDateSet()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testMessageJobHasNoDueDateSet()
	  {
		configuration.EnsureJobDueDateNotNull = false;

		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		Job job = managementService.createJobQuery().singleResult();
		assertNull(job.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testMessageJobHasDueDateSet()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testMessageJobHasDueDateSet()
	  {
		configuration.EnsureJobDueDateNotNull = true;

		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		Job job = managementService.createJobQuery().singleResult();

		// time is fixed for the purposes of the test
		assertEquals(ClockUtil.CurrentTime, job.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/processWithTimerCatch.bpmn20.xml" }) public void testTimerJobsArePreferred()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/processWithTimerCatch.bpmn20.xml" })]
	  public virtual void testTimerJobsArePreferred()
	  {
		// first start process with timer job
		runtimeService.startProcessInstanceByKey("testProcess");
		// then start process with async task
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");
		// then start process with timer job
		runtimeService.startProcessInstanceByKey("testProcess");
		// and another process with async task
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// increment clock so that timer events are acquirable
		incrementClock(70);

		IList<JobEntity> acquirableJobs = findAcquirableJobs();
		assertEquals(4, acquirableJobs.Count);
		assertTrue(acquirableJobs[0] is TimerEntity);
		assertTrue(acquirableJobs[1] is TimerEntity);
		assertTrue(acquirableJobs[2] is MessageEntity);
		assertTrue(acquirableJobs[3] is MessageEntity);
	  }

	}

}