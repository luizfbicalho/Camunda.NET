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
namespace org.camunda.bpm.engine.test.api.mgmt
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.jobByPriority;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySortingAndCount;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobQueryByPriorityTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testOrderByPriority()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("jobPrioExpressionProcess", Variables.createVariables().putValue("priority", i)));
		}

		// then querying and ordering by priority works
		verifySortingAndCount(managementService.createJobQuery().orderByJobPriority().asc(), 5, jobByPriority());
		verifySortingAndCount(managementService.createJobQuery().orderByJobPriority().desc(), 5, inverted(jobByPriority()));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testFilterByJobPriorityLowerThanOrEquals()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("jobPrioExpressionProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a job query and filtering by job priority
		// then the correct jobs are returned
		IList<Job> jobs = managementService.createJobQuery().priorityLowerThanOrEquals(2).list();
		assertEquals(3, jobs.Count);

		ISet<string> processInstanceIds = new HashSet<string>();
		processInstanceIds.Add(instances[0].Id);
		processInstanceIds.Add(instances[1].Id);
		processInstanceIds.Add(instances[2].Id);

		foreach (Job job in jobs)
		{
		  assertTrue(job.Priority <= 2);
		  assertTrue(processInstanceIds.Contains(job.ProcessInstanceId));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testFilterByJobPriorityLowerThanOrEqualsAndHigherThanOrEqual()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("jobPrioExpressionProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a job query and filtering by disjunctive job priority
		// then the no jobs are returned
		assertEquals(0, managementService.createJobQuery().priorityLowerThanOrEquals(2).priorityHigherThanOrEquals(3).count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testFilterByJobPriorityHigherThanOrEquals()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("jobPrioExpressionProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a job query and filtering by job priority
		// then the correct jobs are returned
		IList<Job> jobs = managementService.createJobQuery().priorityHigherThanOrEquals(2L).list();
		assertEquals(3, jobs.Count);

		ISet<string> processInstanceIds = new HashSet<string>();
		processInstanceIds.Add(instances[2].Id);
		processInstanceIds.Add(instances[3].Id);
		processInstanceIds.Add(instances[4].Id);

		foreach (Job job in jobs)
		{
		  assertTrue(job.Priority >= 2);
		  assertTrue(processInstanceIds.Contains(job.ProcessInstanceId));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testFilterByJobPriorityLowerAndHigher()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("jobPrioExpressionProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a job query and filtering by job priority
		// then the correct job is returned
		Job job = managementService.createJobQuery().priorityHigherThanOrEquals(2L).priorityLowerThanOrEquals(2L).singleResult();
		assertNotNull(job);
		assertEquals(2, job.Priority);
		assertEquals(instances[2].Id, job.ProcessInstanceId);
	  }
	}

}