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


	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class SignalEventCatchBoundaryWithVariablesTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class SignalEventCatchBoundaryWithVariablesTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(SendSignalDelegate)).addClass(typeof(SignalReceivedDelegate)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/SignalEventCatchBoundaryWithVariablesTest.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/SignalEventCatchBoundaryWithVariablesTest.throwAlertSignalWithDelegate.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
	  private new RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalCatchBoundaryWithVariables() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSignalCatchBoundaryWithVariables()
	  {
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["processName"] = "catchSignal";
		ProcessInstance piCatchSignal = runtimeService.startProcessInstanceByKey("catchSignal", variables1);

		Dictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["processName"] = "throwSignal";
		variables2["signalProcessInstanceId"] = piCatchSignal.ProcessInstanceId;
		ProcessInstance piThrowSignal = runtimeService.startProcessInstanceByKey("throwSignal", variables2);

		waitForJobExecutorToProcessAllJobs();

		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(piCatchSignal.ProcessInstanceId).activityId("receiveTask").count());
		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(piThrowSignal.ProcessInstanceId).activityId("receiveTask").count());

		// TODO: THis fails because of http://jira.codehaus.org/browse/ACT-1257,
		// should be fixed and re-enabled :-)
		assertEquals("catchSignal-visited (was catchSignal)", runtimeService.getVariable(piCatchSignal.Id, "processName"));
		assertEquals("throwSignal-visited (was throwSignal)", runtimeService.getVariable(piThrowSignal.Id, "processName"));

		// clean up
		runtimeService.signal(piCatchSignal.Id);
		runtimeService.signal(piThrowSignal.Id);

		assertEquals(0, runtimeService.createExecutionQuery().processInstanceId(piCatchSignal.ProcessInstanceId).count());
		assertEquals(0, runtimeService.createExecutionQuery().processInstanceId(piThrowSignal.ProcessInstanceId).count());
	  }

	}

}