using System;
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
namespace org.camunda.bpm.integrationtest.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;


	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using TimerExpressionBean = org.camunda.bpm.integrationtest.jobexecutor.beans.TimerExpressionBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author Tobias Metzke
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TimerRecalculationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TimerRecalculationTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		WebArchive archive = initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/jobexecutor/TimerRecalculation.bpmn20.xml").addClass(typeof(TimerExpressionBean));

		return archive;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimerRecalculationBasedOnProcessVariable()
	  public virtual void testTimerRecalculationBasedOnProcessVariable()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["timerExpression"] = "PT10S";
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("TimerRecalculationProcess", variables);

		ProcessInstanceQuery instancesQuery = runtimeService.createProcessInstanceQuery().processInstanceId(instance.Id);
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, instancesQuery.count());
		assertEquals(1, jobQuery.count());

		Job job = jobQuery.singleResult();
		DateTime oldDueDate = job.Duedate;

		// when
		runtimeService.setVariable(instance.Id, "timerExpression", "PT1S");
		managementService.recalculateJobDuedate(job.Id, true);

		// then
		assertEquals(1, jobQuery.count());
		Job jobRecalculated = jobQuery.singleResult();
		assertNotEquals(oldDueDate, jobRecalculated.Duedate);

		DateTime calendar = new DateTime();
		calendar = new DateTime(jobRecalculated.CreateTime);
		calendar.AddSeconds(1);
		DateTime expectedDate = calendar;
		assertEquals(expectedDate, jobRecalculated.Duedate);

		waitForJobExecutorToProcessAllJobs();

		assertEquals(0, instancesQuery.count());
	  }
	}

}