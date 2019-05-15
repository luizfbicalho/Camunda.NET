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


	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using DemoDelegate = org.camunda.bpm.integrationtest.jobexecutor.beans.DemoDelegate;
	using DemoVariableClass = org.camunda.bpm.integrationtest.jobexecutor.beans.DemoVariableClass;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// Test camunda BPM platform container job exectuor.
	/// FAILING ATM!
	/// Expected a job with an exception but it isn't left in db with 0 retries, instead it is completely removed from the job table!
	/// 
	/// @author christian.lipphardt@camunda.com
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class FailingJobBoundaryTimerWithDelegateVariablesTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class FailingJobBoundaryTimerWithDelegateVariablesTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(DemoDelegate)).addClass(typeof(DemoVariableClass)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/ImmediatelyFailing.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
	  private new RuntimeService runtimeService;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.ManagementService managementService;
	  private new ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingJobBoundaryTimerWithDelegateVariables() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testFailingJobBoundaryTimerWithDelegateVariables()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("ImmediatelyFailing");

		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(1, jobs.Count);
		assertEquals(3, jobs[0].Retries);

		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(pi.ProcessInstanceId).activityId("usertask1").count());
		assertEquals(2, runtimeService.createExecutionQuery().processInstanceId(pi.ProcessInstanceId).count());

		assertEquals(1, managementService.createJobQuery().executable().count());

		waitForJobExecutorToProcessAllJobs();

		assertEquals(0, managementService.createJobQuery().executable().count()); // should be 0, because it has failed 3 times
		assertEquals(1, managementService.createJobQuery().withException().count()); // should be 1, because job failed!

		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(pi.ProcessInstanceId).activityId("usertask1").count());
		assertEquals(2, runtimeService.createExecutionQuery().processInstanceId(pi.ProcessInstanceId).count());

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id); // complete task with failed job => complete process

		assertEquals(0, runtimeService.createExecutionQuery().processInstanceId(pi.ProcessInstanceId).count());
		assertEquals(0, managementService.createJobQuery().count()); // should be 0, because process is finished.
	  }

	}

}