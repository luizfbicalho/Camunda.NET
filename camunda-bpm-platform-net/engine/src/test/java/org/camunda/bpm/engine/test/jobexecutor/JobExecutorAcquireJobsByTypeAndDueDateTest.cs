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
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	public class JobExecutorAcquireJobsByTypeAndDueDateTest : AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void prepareProcessEngineConfiguration()
	  public virtual void prepareProcessEngineConfiguration()
	  {
		configuration.JobExecutorPreferTimerJobs = true;
		configuration.JobExecutorAcquireByDueDate = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineConfiguration()
	  public virtual void testProcessEngineConfiguration()
	  {
		assertTrue(configuration.JobExecutorPreferTimerJobs);
		assertTrue(configuration.JobExecutorAcquireByDueDate);
		assertFalse(configuration.JobExecutorAcquireByPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testMessageJobHasDueDateSet()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testMessageJobHasDueDateSet()
	  {
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job.Duedate);
		assertEquals(ClockUtil.CurrentTime, job.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/processWithTimerCatch.bpmn20.xml" }) public void testTimerAndOldJobsArePreferred()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/processWithTimerCatch.bpmn20.xml" })]
	  public virtual void testTimerAndOldJobsArePreferred()
	  {
		// first start process with timer job
		ProcessInstance timerProcess1 = runtimeService.startProcessInstanceByKey("testProcess");
		// then start process with async task
		incrementClock(1);
		ProcessInstance asyncProcess1 = runtimeService.startProcessInstanceByKey("simpleAsyncProcess");
		// then start process with timer job
		incrementClock(1);
		ProcessInstance timerProcess2 = runtimeService.startProcessInstanceByKey("testProcess");
		// and another process with async task after the timers are acquirable
		incrementClock(61);
		ProcessInstance asyncProcess2 = runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		Job timerJob1 = managementService.createJobQuery().processInstanceId(timerProcess1.Id).singleResult();
		Job timerJob2 = managementService.createJobQuery().processInstanceId(timerProcess2.Id).singleResult();
		Job messageJob1 = managementService.createJobQuery().processInstanceId(asyncProcess1.Id).singleResult();
		Job messageJob2 = managementService.createJobQuery().processInstanceId(asyncProcess2.Id).singleResult();

		assertNotNull(timerJob1.Duedate);
		assertNotNull(timerJob2.Duedate);
		assertNotNull(messageJob1.Duedate);
		assertNotNull(messageJob2.Duedate);

		assertTrue(messageJob1.Duedate < timerJob1.Duedate);
		assertTrue(timerJob1.Duedate < timerJob2.Duedate);
		assertTrue(timerJob2.Duedate < messageJob2.Duedate);

		IList<JobEntity> acquirableJobs = findAcquirableJobs();
		assertEquals(4, acquirableJobs.Count);
		assertEquals(timerJob1.Id, acquirableJobs[0].Id);
		assertEquals(timerJob2.Id, acquirableJobs[1].Id);
		assertEquals(messageJob1.Id, acquirableJobs[2].Id);
		assertEquals(messageJob2.Id, acquirableJobs[3].Id);
	  }
	}

}