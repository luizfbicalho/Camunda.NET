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
namespace org.camunda.bpm.engine.test.bpmn.iomapping
{

	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class InputOutputEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();

		VariableLogDelegate.reset();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageThrowEvent()
	  public virtual void testMessageThrowEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// input mapping
		IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
		assertEquals(1, mappedVariables.Count);
		assertEquals("mappedValue", mappedVariables["mappedVariable"]);

		// output mapping
		string variable = (string) runtimeService.getVariableLocal(processInstance.Id, "outVariable");
		assertEquals("mappedValue", variable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageCatchEvent()
	  public virtual void testMessageCatchEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		Execution messageExecution = runtimeService.createExecutionQuery().activityId("messageCatch").singleResult();

		IDictionary<string, object> localVariables = runtimeService.getVariablesLocal(messageExecution.Id);
		assertEquals(1, localVariables.Count);
		assertEquals("mappedValue", localVariables["mappedVariable"]);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["messageVariable"] = "outValue";
		runtimeService.messageEventReceived("IncomingMessage", messageExecution.Id, variables);

		// output mapping
		string variable = (string) runtimeService.getVariableLocal(processInstance.Id, "outVariable");
		assertEquals("outValue", variable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerCatchEvent()
	  public virtual void testTimerCatchEvent()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		DateTime dueDate = DateTimeUtil.now().plusMinutes(5).toDate();
		variables["outerVariable"] = (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dueDate);
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		Job job = managementService.createJobQuery().singleResult();
		TimerEntity timer = (TimerEntity) job;
		assertDateEquals(dueDate, timer.Duedate);
	  }

	  protected internal virtual void assertDateEquals(DateTime expected, DateTime actual)
	  {
		DateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		assertEquals(format.format(expected), format.format(actual));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoneThrowEvent()
	  public virtual void testNoneThrowEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
		assertEquals(1, mappedVariables.Count);
		assertEquals("mappedValue", mappedVariables["mappedVariable"]);

		// output mapping
		string variable = (string) runtimeService.getVariableLocal(processInstance.Id, "outVariable");
		assertEquals("mappedValue", variable);
	  }

	  public virtual void testMessageStartEvent()
	  {

		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputEventTest.testMessageStartEvent.bpmn20.xml").deploy();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("camunda:inputOutput mapping unsupported for element type 'startEvent'", e.Message);
		}
	  }

	  public virtual void testNoneEndEvent()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputEventTest.testNoneEndEvent.bpmn20.xml").deploy();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("camunda:outputParameter not allowed for element type 'endEvent'", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageEndEvent()
	  public virtual void testMessageEndEvent()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// input mapping
		IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
		assertEquals(1, mappedVariables.Count);
		assertEquals("mappedValue", mappedVariables["mappedVariable"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageCatchAfterEventGateway()
	  public virtual void testMessageCatchAfterEventGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		runtimeService.createMessageCorrelation("foo").processInstanceId(processInstance.Id).correlate();

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName("foo").singleResult();

		assertNotNull(variableInstance);
		assertEquals("bar", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerCatchAfterEventGateway()
	  public virtual void testTimerCatchAfterEventGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		// when
		managementService.executeJob(job.Id);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName("foo").singleResult();

		assertNotNull(variableInstance);
		assertEquals("bar", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignalCatchAfterEventGateway()
	  public virtual void testSignalCatchAfterEventGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).signalEventSubscriptionName("foo").singleResult();

		assertNotNull(execution);

		// when
		runtimeService.signalEventReceived("foo", execution.Id);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName("foo").singleResult();

		assertNotNull(variableInstance);
		assertEquals("bar", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConditionalCatchAfterEventGateway()
	  public virtual void testConditionalCatchAfterEventGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		runtimeService.setVariable(processInstance.Id, "var", 1);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).variableName("foo").singleResult();

		assertNotNull(variableInstance);
		assertEquals("bar", variableInstance.Value);
	  }

	  public virtual void testMessageBoundaryEvent()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputEventTest.testMessageBoundaryEvent.bpmn20.xml").deploy();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("camunda:inputOutput mapping unsupported for element type 'boundaryEvent'", e.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		base.tearDown();

		VariableLogDelegate.reset();
	  }

	}

}