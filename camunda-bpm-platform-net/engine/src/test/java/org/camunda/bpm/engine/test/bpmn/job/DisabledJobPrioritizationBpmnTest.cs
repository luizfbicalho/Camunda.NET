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
namespace org.camunda.bpm.engine.test.bpmn.job
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DisabledJobPrioritizationBpmnTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		processEngineConfiguration.ProducePrioritizedJobs = false;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		processEngineConfiguration.ProducePrioritizedJobs = true;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioProcess.bpmn20.xml")]
	  public virtual void testJobPriority()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task1").startBeforeActivity("task2").execute();

		// then
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(2, jobs.Count);

		foreach (Job job in jobs)
		{
		  assertNotNull(job);
		  assertEquals(0, job.Priority);
		}
	  }
	}

}