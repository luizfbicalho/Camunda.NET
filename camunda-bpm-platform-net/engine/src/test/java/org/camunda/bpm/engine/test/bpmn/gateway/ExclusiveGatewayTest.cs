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
namespace org.camunda.bpm.engine.test.bpmn.gateway
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ExclusiveGatewayTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDivergingExclusiveGateway()
	  public virtual void testDivergingExclusiveGateway()
	  {
		for (int i = 1; i <= 3; i++)
		{
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("exclusiveGwDiverging", CollectionUtil.singletonMap("input", i));
		  assertEquals("Task " + i, taskService.createTaskQuery().singleResult().Name);
		  runtimeService.deleteProcessInstance(pi.Id, "testing deletion");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMergingExclusiveGateway()
	  public virtual void testMergingExclusiveGateway()
	  {
		runtimeService.startProcessInstanceByKey("exclusiveGwMerging");
		assertEquals(3, taskService.createTaskQuery().count());
	  }

	  // If there are multiple outgoing seqFlow with valid conditions, the first
	  // defined one should be chosen.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleValidConditions()
	  public virtual void testMultipleValidConditions()
	  {
		runtimeService.startProcessInstanceByKey("exclusiveGwMultipleValidConditions", CollectionUtil.singletonMap("input", 5));
		assertEquals("Task 2", taskService.createTaskQuery().singleResult().Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoSequenceFlowSelected()
	  public virtual void testNoSequenceFlowSelected()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("exclusiveGwNoSeqFlowSelected", CollectionUtil.singletonMap("input", 4));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-02004 No outgoing sequence flow for the element with id 'exclusiveGw' could be selected for continuing the process.", e.Message);
		}
	  }

	  /// <summary>
	  /// Test for bug ACT-10: whitespaces/newlines in expressions lead to exceptions
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testWhitespaceInExpression()
	  public virtual void testWhitespaceInExpression()
	  {
		// Starting a process instance will lead to an exception if whitespace are incorrectly handled
		runtimeService.startProcessInstanceByKey("whiteSpaceInExpression", CollectionUtil.singletonMap("input", 1));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/gateway/ExclusiveGatewayTest.testDivergingExclusiveGateway.bpmn20.xml"})]
	  public virtual void testUnknownVariableInExpression()
	  {
		// Instead of 'input' we're starting a process instance with the name 'iinput' (ie. a typo)
		try
		{
		  runtimeService.startProcessInstanceByKey("exclusiveGwDiverging", CollectionUtil.singletonMap("iinput", 1));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown property used in expression", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnBeanProperty()
	  public virtual void testDecideBasedOnBeanProperty()
	  {
		runtimeService.startProcessInstanceByKey("decisionBasedOnBeanProperty", CollectionUtil.singletonMap("order", new ExclusiveGatewayTestOrder(150)));

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Standard service", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnListOrArrayOfBeans()
	  public virtual void testDecideBasedOnListOrArrayOfBeans()
	  {
		IList<ExclusiveGatewayTestOrder> orders = new List<ExclusiveGatewayTestOrder>();
		orders.Add(new ExclusiveGatewayTestOrder(50));
		orders.Add(new ExclusiveGatewayTestOrder(300));
		orders.Add(new ExclusiveGatewayTestOrder(175));

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("decisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orders));

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(task);
		assertEquals("Gold Member service", task.Name);


		// Arrays are usable in exactly the same way
		ExclusiveGatewayTestOrder[] orderArray = orders.ToArray();
		orderArray[1].Price = 10;
		pi = runtimeService.startProcessInstanceByKey("decisionBasedOnListOrArrayOfBeans", CollectionUtil.singletonMap("orders", orderArray));

		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(task);
		assertEquals("Basic service", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecideBasedOnBeanMethod()
	  public virtual void testDecideBasedOnBeanMethod()
	  {
		runtimeService.startProcessInstanceByKey("decisionBasedOnBeanMethod", CollectionUtil.singletonMap("order", new ExclusiveGatewayTestOrder(300)));

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("Gold Member service", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInvalidMethodExpression()
	  public virtual void testInvalidMethodExpression()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("invalidMethodExpression", CollectionUtil.singletonMap("order", new ExclusiveGatewayTestOrder(50)));
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown method used in expression", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultSequenceFlow()
	  public virtual void testDefaultSequenceFlow()
	  {

		// Input == 1 -> default is not selected
		string procId = runtimeService.startProcessInstanceByKey("exclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 1)).Id;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Input is one", task.Name);
		runtimeService.deleteProcessInstance(procId, null);

		procId = runtimeService.startProcessInstanceByKey("exclusiveGwDefaultSequenceFlow", CollectionUtil.singletonMap("input", 5)).Id;
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Default input", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoIdOnSequenceFlow()
	  public virtual void testNoIdOnSequenceFlow()
	  {
		runtimeService.startProcessInstanceByKey("noIdOnSequenceFlow", CollectionUtil.singletonMap("input", 3));
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Input is more than one", task.Name);
	  }

	  public virtual void testInvalidProcessDefinition()
	  {
		string flowWithoutConditionNoDefaultFlow = "<?xml version='1.0' encoding='UTF-8'?>" +
				"<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" +
				"  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " +
				"    <startEvent id='theStart' /> " +
				"    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " +

				"    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' /> " + // no default = "flow3" !!
				"    <sequenceFlow id='flow2' sourceRef='exclusiveGw' targetRef='theTask1'> " +
				"      <conditionExpression xsi:type='tFormalExpression'>${input == 1}</conditionExpression> " +
				"    </sequenceFlow> " +
				"    <sequenceFlow id='flow3' sourceRef='exclusiveGw' targetRef='theTask2'/> " + // one would be OK
				"    <sequenceFlow id='flow4' sourceRef='exclusiveGw' targetRef='theTask2'/> " + // but two unconditional not!

				"    <userTask id='theTask1' name='Input is one' /> " +
				"    <userTask id='theTask2' name='Default input' /> " +
				"  </process>" +
				"</definitions>";
		try
		{
		  repositoryService.createDeployment().addString("myprocess.bpmn20.xml", flowWithoutConditionNoDefaultFlow).deploy();
		  fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
		}
		catch (ProcessEngineException ex)
		{
		  assertTrue(ex.Message.contains("Exclusive Gateway 'exclusiveGw' has outgoing sequence flow 'flow3' without condition which is not the default flow."));
		}

		string defaultFlowWithCondition = "<?xml version='1.0' encoding='UTF-8'?>" +
				"<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" +
				"  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " +
				"    <startEvent id='theStart' /> " +
				"    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " +

				"    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' default='flow3' /> " +
				"    <sequenceFlow id='flow2' sourceRef='exclusiveGw' targetRef='theTask1'> " +
				"      <conditionExpression xsi:type='tFormalExpression'>${input == 1}</conditionExpression> " +
				"    </sequenceFlow> " +
				"    <sequenceFlow id='flow3' sourceRef='exclusiveGw' targetRef='theTask2'> " +
				"      <conditionExpression xsi:type='tFormalExpression'>${input == 3}</conditionExpression> " +
				"    </sequenceFlow> " +

				"    <userTask id='theTask1' name='Input is one' /> " +
				"    <userTask id='theTask2' name='Default input' /> " +
				"  </process>" +
				"</definitions>";
		try
		{
		  repositoryService.createDeployment().addString("myprocess.bpmn20.xml", defaultFlowWithCondition).deploy();
		  fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
		}
		catch (ProcessEngineException ex)
		{
		  assertTrue(ex.Message.contains("Exclusive Gateway 'exclusiveGw' has outgoing sequence flow 'flow3' which is the default flow but has a condition too."));
		}

		string noOutgoingFlow = "<?xml version='1.0' encoding='UTF-8'?>" +
				"<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" +
				"  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " +
				"    <startEvent id='theStart' /> " +
				"    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " +
				"    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' /> " +
				"  </process>" +
				"</definitions>";
		try
		{
		  repositoryService.createDeployment().addString("myprocess.bpmn20.xml", noOutgoingFlow).deploy();
		  fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
		}
		catch (ProcessEngineException ex)
		{
		  Console.WriteLine(ex.ToString());
		  Console.Write(ex.StackTrace);
		  assertTrue(ex.Message.contains("Exclusive Gateway 'exclusiveGw' has no outgoing sequence flows."));
		}

	  }

	  // see CAM-4172
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLoopWithManyIterations()
	  public virtual void testLoopWithManyIterations()
	  {
		int numOfIterations = 1000;

		// this should not fail
		runtimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("numOfIterations", numOfIterations));
	  }

	  /// <summary>
	  /// The test process has an XOR gateway where, the 'input' variable is used to
	  /// select one of the outgoing sequence flow. Every one of those sequence flow
	  /// goes to another task, allowing us to test the decision very easily.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecisionFunctionality()
	  public virtual void testDecisionFunctionality()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();

		// Test with input == 1
		variables["input"] = 1;
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("exclusiveGateway", variables);
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Send e-mail for more information", task.Name);

		// Test with input == 2
		variables["input"] = 2;
		pi = runtimeService.startProcessInstanceByKey("exclusiveGateway", variables);
		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Check account balance", task.Name);

		// Test with input == 3
		variables["input"] = 3;
		pi = runtimeService.startProcessInstanceByKey("exclusiveGateway", variables);
		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Call customer", task.Name);

		// Test with input == 4
		variables["input"] = 4;
		try
		{
		  runtimeService.startProcessInstanceByKey("exclusiveGateway", variables);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected since no outgoing sequence flow matches
		}

	  }
	}

}