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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class SignalEventReceivedBuilderTest : PluggableProcessEngineTestCase
	{

	  protected internal virtual BpmnModelInstance signalStartProcess(string processId)
	  {
		return Bpmn.createExecutableProcess(processId).startEvent().signal("signal").userTask().endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance signalCatchProcess(string processId)
	  {
		return Bpmn.createExecutableProcess(processId).startEvent().intermediateCatchEvent().signal("signal").userTask().endEvent().done();
	  }

	  public virtual void testSendSignalToStartEvent()
	  {
		deployment(signalStartProcess("signalStart"));

		runtimeService.createSignalEvent("signal").send();

		assertThat(taskService.createTaskQuery().count(), @is(1L));
	  }

	  public virtual void testSendSignalToIntermediateCatchEvent()
	  {
		deployment(signalCatchProcess("signalCatch"));

		runtimeService.startProcessInstanceByKey("signalCatch");

		runtimeService.createSignalEvent("signal").send();

		assertThat(taskService.createTaskQuery().count(), @is(1L));
	  }

	  public virtual void testSendSignalToStartAndIntermediateCatchEvent()
	  {
		deployment(signalStartProcess("signalStart"), signalCatchProcess("signalCatch"));

		runtimeService.startProcessInstanceByKey("signalCatch");

		runtimeService.createSignalEvent("signal").send();

		assertThat(taskService.createTaskQuery().count(), @is(2L));
	  }

	  public virtual void testSendSignalToMultipleStartEvents()
	  {
		deployment(signalStartProcess("signalStart"), signalStartProcess("signalStart2"));

		runtimeService.createSignalEvent("signal").send();

		assertThat(taskService.createTaskQuery().count(), @is(2L));
	  }

	  public virtual void testSendSignalToMultipleIntermediateCatchEvents()
	  {
		deployment(signalCatchProcess("signalCatch"), signalCatchProcess("signalCatch2"));

		runtimeService.startProcessInstanceByKey("signalCatch");
		runtimeService.startProcessInstanceByKey("signalCatch2");

		runtimeService.createSignalEvent("signal").send();

		assertThat(taskService.createTaskQuery().count(), @is(2L));
	  }

	  public virtual void testSendSignalWithExecutionId()
	  {
		deployment(signalCatchProcess("signalCatch"), signalCatchProcess("signalCatch2"));

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalCatch");
		runtimeService.startProcessInstanceByKey("signalCatch2");

		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().processInstanceId(processInstance.Id).singleResult();
		string executionId = eventSubscription.ExecutionId;

		runtimeService.createSignalEvent("signal").executionId(executionId).send();

		assertThat(taskService.createTaskQuery().count(), @is(1L));
	  }

	  public virtual void testSendSignalToStartEventWithVariables()
	  {
		deployment(signalStartProcess("signalStart"));

		IDictionary<string, object> variables = Variables.createVariables().putValue("var1", "a").putValue("var2", "b");

		runtimeService.createSignalEvent("signal").setVariables(variables).send();

		Execution execution = runtimeService.createExecutionQuery().singleResult();
		assertThat(runtimeService.getVariables(execution.Id), @is(variables));
	  }

	  public virtual void testSendSignalToIntermediateCatchEventWithVariables()
	  {
		deployment(signalCatchProcess("signalCatch"));

		runtimeService.startProcessInstanceByKey("signalCatch");

		IDictionary<string, object> variables = Variables.createVariables().putValue("var1", "a").putValue("var2", "b");

		runtimeService.createSignalEvent("signal").setVariables(variables).send();

		Execution execution = runtimeService.createExecutionQuery().singleResult();
		assertThat(runtimeService.getVariables(execution.Id), @is(variables));
	  }

	  public virtual void testNoSignalEventSubscription()
	  {
		// assert that no exception is thrown
		runtimeService.createSignalEvent("signal").send();
	  }

	  public virtual void testNonExistingExecutionId()
	  {

		try
		{
		  runtimeService.createSignalEvent("signal").executionId("nonExisting").send();

		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot find execution with id 'nonExisting'"));
		}
	  }

	  public virtual void testNoSignalEventSubscriptionWithExecutionId()
	  {
		deployment(Bpmn.createExecutableProcess("noSignal").startEvent().userTask().endEvent().done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("noSignal");
		string executionId = processInstance.Id;

		try
		{
		  runtimeService.createSignalEvent("signal").executionId(executionId).send();

		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Execution '" + executionId + "' has not subscribed to a signal event with name 'signal'"));
		}
	  }

	}

}