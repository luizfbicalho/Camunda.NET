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
namespace org.camunda.bpm.engine.cdi.test.bpmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Test = org.junit.Test;

	public class SignalEventTest : CdiProcessEngineTestCase
	{


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public static class SignalReceivedDelegate implements org.camunda.bpm.engine.delegate.JavaDelegate
	  public class SignalReceivedDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.BusinessProcess businessProcess;
		  internal BusinessProcess businessProcess;

		public virtual void execute(DelegateExecution execution)
		{
		  businessProcess.setVariable("processName", "catchSignal-visited (was " + businessProcess.getVariable("processName") + ")");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public static class SendSignalDelegate implements org.camunda.bpm.engine.delegate.JavaDelegate
	  public class SendSignalDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
		  internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.BusinessProcess businessProcess;
		internal BusinessProcess businessProcess;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  businessProcess.setVariable("processName", "throwSignal-visited (was " + businessProcess.getVariable("processName") + ")");

		  string signalProcessInstanceId = (string) execution.getVariable("signalProcessInstanceId");
		  string executionId = runtimeService.createExecutionQuery().processInstanceId(signalProcessInstanceId).signalEventSubscriptionName("alert").singleResult().Id;

		  runtimeService.signalEventReceived("alert", executionId);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/cdi/test/bpmn/SignalEventTests.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml", "org/camunda/bpm/engine/cdi/test/bpmn/SignalEventTests.throwAlertSignalWithDelegate.bpmn20.xml"}) public void testSignalCatchBoundaryWithVariables()
	  [Deployment(resources : {"org/camunda/bpm/engine/cdi/test/bpmn/SignalEventTests.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml", "org/camunda/bpm/engine/cdi/test/bpmn/SignalEventTests.throwAlertSignalWithDelegate.bpmn20.xml"})]
	  public virtual void testSignalCatchBoundaryWithVariables()
	  {
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["processName"] = "catchSignal";
		ProcessInstance piCatchSignal = runtimeService.startProcessInstanceByKey("catchSignal", variables1);

		Dictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["processName"] = "throwSignal";
		variables2["signalProcessInstanceId"] = piCatchSignal.ProcessInstanceId;
		ProcessInstance piThrowSignal = runtimeService.startProcessInstanceByKey("throwSignal", variables2);

		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(piCatchSignal.ProcessInstanceId).activityId("receiveTask").count());
		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(piThrowSignal.ProcessInstanceId).activityId("receiveTask").count());

		assertEquals("catchSignal-visited (was catchSignal)", runtimeService.getVariable(piCatchSignal.Id, "processName"));
		assertEquals("throwSignal-visited (was throwSignal)", runtimeService.getVariable(piThrowSignal.Id, "processName"));

		// clean up
		runtimeService.signal(piCatchSignal.Id);
		runtimeService.signal(piThrowSignal.Id);
	  }

	}

}