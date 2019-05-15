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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// Testcase for camunda input / output in BPMN
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class InputOutputTest : PluggableProcessEngineTestCase
	{

	  // Input parameters /////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputNullValue()
	  public virtual void testInputNullValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals("null", variable.TypeName);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputStringConstantValue()
	  public virtual void testInputStringConstantValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals("stringValue", variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputElValue()
	  public virtual void testInputElValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2l, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputScriptValue()
	  public virtual void testInputScriptValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputScriptValueAsVariable()
	  public virtual void testInputScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "return 1 + 1";
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputScriptValueAsBean()
	  public virtual void testInputScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalScriptValue()
	  public virtual void testInputExternalScriptValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalScriptValueAsVariable()
	  public virtual void testInputExternalScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalScriptValueAsBean()
	  public virtual void testInputExternalScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalClasspathScriptValue()
	  public virtual void testInputExternalClasspathScriptValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalClasspathScriptValueAsVariable()
	  public virtual void testInputExternalClasspathScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "classpath://org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputExternalClasspathScriptValueAsBean()
	  public virtual void testInputExternalClasspathScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testInputExternalDeploymentScriptValue.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testInputExternalDeploymentScriptValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testInputExternalDeploymentScriptValueAsVariable.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testInputExternalDeploymentScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "deployment://org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testInputExternalDeploymentScriptValueAsBean.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testInputExternalDeploymentScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(execution.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testInputListElValues()
	  public virtual void testInputListElValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals(2l, value[0]);
		assertEquals(3l, value[1]);
		assertEquals(4l, value[2]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testInputListMixedValues()
	  public virtual void testInputListMixedValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals("constantStringValue", value[0]);
		assertEquals("elValue", value[1]);
		assertEquals("scriptValue", value[2]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testInputMapElValues()
	  public virtual void testInputMapElValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals(2l, value["a"]);
		assertEquals(3l, value["b"]);
		assertEquals(4l, value["c"]);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputMultipleElValue()
	  public virtual void testInputMultipleElValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(var1);
		assertEquals(2l, var1.Value);
		assertEquals(execution.Id, var1.ExecutionId);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals(3l, var2.Value);
		assertEquals(execution.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInputMultipleMixedValue()
	  public virtual void testInputMultipleMixedValue()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(var1);
		assertEquals(2l, var1.Value);
		assertEquals(execution.Id, var1.ExecutionId);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals("stringConstantValue", var2.Value);
		assertEquals(execution.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testInputNested()
	  public virtual void testInputNested()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exprKey"] = "b";
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		Execution execution = runtimeService.createExecutionQuery().activityId("wait").singleResult();

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		SortedDictionary<string, object> value = (SortedDictionary) var1.Value;
		IList<object> nestedList = (IList<object>) value["a"];
		assertEquals("stringInListNestedInMap", nestedList[0]);
		assertEquals("b", nestedList[1]);
		assertEquals("stringValueWithExprKey", value["b"]);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals("stringConstantValue", var2.Value);
		assertEquals(execution.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testInputNestedListValues()
	  public virtual void testInputNestedListValues()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exprKey"] = "vegie";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals("constantStringValue", value[0]);
		assertEquals("elValue", value[1]);
		assertEquals("scriptValue", value[2]);

		IList<object> nestedList = (IList<object>) value[3];
		IList<object> nestedNestedList = (IList<object>) nestedList[0];
		assertEquals("a", nestedNestedList[0]);
		assertEquals("b", nestedNestedList[1]);
		assertEquals("c", nestedNestedList[2]);
		assertEquals("d", nestedList[1]);

		SortedDictionary<string, object> nestedMap = (SortedDictionary<string, object>) value[4];
		assertEquals("bar", nestedMap["foo"]);
		assertEquals("world", nestedMap["hello"]);
		assertEquals("potato", nestedMap["vegie"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testInputMapElKey()
	  public virtual void testInputMapElKey()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["varExpr1"] = "a";
		variables["varExpr2"] = "b";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals("potato", value["a"]);
		assertEquals("tomato", value["b"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testInputMapElMixedKey()
	  public virtual void testInputMapElMixedKey()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["varExpr1"] = "a";
		variables["varExpr2"] = "b";
		variables["varExprMapValue"] = "avocado";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals("potato", value["a"]);
		assertEquals("tomato", value["b"]);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testInputMapElKey.bpmn")]
	  public virtual void testInputMapElUndefinedKey()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown property used in expression: ${varExpr1}", e.Message);
		}
	  }

	  // output parameter ///////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputNullValue()
	  public virtual void testOutputNullValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals("null", variable.TypeName);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputStringConstantValue()
	  public virtual void testOutputStringConstantValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals("stringValue", variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputElValue()
	  public virtual void testOutputElValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2l, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputScriptValue()
	  public virtual void testOutputScriptValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputScriptValueAsVariable()
	  public virtual void testOutputScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "return 1 + 1";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

	  // related to CAM-8072
	  public virtual void testOutputParameterAvailableAfterParallelGateway()
	  {
		// given
		BpmnModelInstance processDefinition = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaOutputParameter("variable", "A").camundaExpression("${'this value does not matter'}").parallelGateway("fork").endEvent().moveToNode("fork").serviceTask().camundaExpression("${variable}").receiveTask().endEvent().done();

		// when
		deployment(processDefinition);
		runtimeService.startProcessInstanceByKey("process");

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().variableName("variable").singleResult();
		assertNotNull(variableInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputScriptValueAsBean()
	  public virtual void testOutputScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalScriptValue()
	  public virtual void testOutputExternalScriptValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalScriptValueAsVariable()
	  public virtual void testOutputExternalScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalScriptValueAsBean()
	  public virtual void testOutputExternalScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalClasspathScriptValue()
	  public virtual void testOutputExternalClasspathScriptValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalClasspathScriptValueAsVariable()
	  public virtual void testOutputExternalClasspathScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "classpath://org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputExternalClasspathScriptValueAsBean()
	  public virtual void testOutputExternalClasspathScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testOutputExternalDeploymentScriptValue.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testOutputExternalDeploymentScriptValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testOutputExternalDeploymentScriptValueAsVariable.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testOutputExternalDeploymentScriptValueAsVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptPath"] = "deployment://org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testOutputExternalDeploymentScriptValueAsBean.bpmn", "org/camunda/bpm/engine/test/bpmn/iomapping/oneplusone.groovy" })]
	  public virtual void testOutputExternalDeploymentScriptValueAsBean()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["onePlusOneBean"] = new OnePlusOneBean();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		assertEquals(2, variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testOutputListElValues()
	  public virtual void testOutputListElValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals(2l, value[0]);
		assertEquals(3l, value[1]);
		assertEquals(4l, value[2]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testOutputListMixedValues()
	  public virtual void testOutputListMixedValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals("constantStringValue", value[0]);
		assertEquals("elValue", value[1]);
		assertEquals("scriptValue", value[2]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testOutputMapElValues()
	  public virtual void testOutputMapElValues()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals(2l, value["a"]);
		assertEquals(3l, value["b"]);
		assertEquals(4l, value["c"]);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputMultipleElValue()
	  public virtual void testOutputMultipleElValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(var1);
		assertEquals(2l, var1.Value);
		assertEquals(pi.Id, var1.ExecutionId);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals(3l, var2.Value);
		assertEquals(pi.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputMultipleMixedValue()
	  public virtual void testOutputMultipleMixedValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(var1);
		assertEquals(2l, var1.Value);
		assertEquals(pi.Id, var1.ExecutionId);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals("stringConstantValue", var2.Value);
		assertEquals(pi.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testOutputNested()
	  public virtual void testOutputNested()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exprKey"] = "b";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance var1 = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		SortedDictionary<string, object> value = (SortedDictionary) var1.Value;
		IList<object> nestedList = (IList<object>) value["a"];
		assertEquals("stringInListNestedInMap", nestedList[0]);
		assertEquals("b", nestedList[1]);
		assertEquals(pi.Id, var1.ExecutionId);
		assertEquals("stringValueWithExprKey", value["b"]);

		VariableInstance var2 = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();
		assertNotNull(var2);
		assertEquals("stringConstantValue", var2.Value);
		assertEquals(pi.Id, var2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings("unchecked") public void testOutputListNestedValues()
	  public virtual void testOutputListNestedValues()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exprKey"] = "vegie";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		IList<object> value = (IList<object>) variable.Value;
		assertEquals("constantStringValue", value[0]);
		assertEquals("elValue", value[1]);
		assertEquals("scriptValue", value[2]);

		IList<object> nestedList = (IList<object>) value[3];
		IList<object> nestedNestedList = (IList<object>) nestedList[0];
		assertEquals("a", nestedNestedList[0]);
		assertEquals("b", nestedNestedList[1]);
		assertEquals("c", nestedNestedList[2]);
		assertEquals("d", nestedList[1]);

		SortedDictionary<string, object> nestedMap = (SortedDictionary<string, object>) value[4];
		assertEquals("bar", nestedMap["foo"]);
		assertEquals("world", nestedMap["hello"]);
		assertEquals("potato", nestedMap["vegie"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testOutputMapElKey()
	  public virtual void testOutputMapElKey()
	  {


		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["varExpr1"] = "a";
		variables["varExpr2"] = "b";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals("potato", value["a"]);
		assertEquals("tomato", value["b"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @SuppressWarnings({ "unchecked", "rawtypes" }) public void testOutputMapElMixedKey()
	  public virtual void testOutputMapElMixedKey()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["varExpr1"] = "a";
		variables["varExpr2"] = "b";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(variable);
		SortedDictionary<string, object> value = (SortedDictionary) variable.Value;
		assertEquals("potato", value["a"]);
		assertEquals("tomato", value["b"]);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testOutputMapElKey.bpmn")]
	  public virtual void testOutputMapElUndefinedKey()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Unknown property used in expression: ${varExpr1}", e.Message);
		}
	  }

	  // ensure Io supported on event subprocess /////////////////////////////////

	  public virtual void testInterruptingEventSubprocessIoSupport()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testInterruptingEventSubprocessIoSupport.bpmn").deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("camunda:inputOutput mapping unsupported for element type 'subProcess' with attribute 'triggeredByEvent = true'", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSubprocessIoSupport()
	  public virtual void testSubprocessIoSupport()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["processVar"] = "value";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		Execution subprocessExecution = runtimeService.createExecutionQuery().activityId("subprocessTask").singleResult();
		IDictionary<string, object> variablesLocal = runtimeService.getVariablesLocal(subprocessExecution.Id);
		assertEquals(1, variablesLocal.Count);
		assertEquals("value", variablesLocal["innerVar"]);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		string outerVariable = (string) runtimeService.getVariableLocal(processInstance.Id, "outerVar");
		assertNotNull(outerVariable);
		assertEquals("value", outerVariable);


	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMIActivityIoSupport()
	  public virtual void testSequentialMIActivityIoSupport()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["counter"] = new AtomicInteger();
		variables["nrOfLoops"] = 2;
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSequentialActivity", variables);

		// first sequential mi execution
		Execution miExecution = runtimeService.createExecutionQuery().activityId("miTask").singleResult();
		assertNotNull(miExecution);
		assertFalse(instance.Id.Equals(miExecution.Id));
		assertEquals(0, runtimeService.getVariable(miExecution.Id, "loopCounter"));

		// input mapping
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
		assertEquals(1, runtimeService.getVariableLocal(miExecution.Id, "miCounterValue"));

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// second sequential mi execution
		miExecution = runtimeService.createExecutionQuery().activityId("miTask").singleResult();
		assertNotNull(miExecution);
		assertFalse(instance.Id.Equals(miExecution.Id));
		assertEquals(1, runtimeService.getVariable(miExecution.Id, "loopCounter"));

		// input mapping
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
		assertEquals(2, runtimeService.getVariableLocal(miExecution.Id, "miCounterValue"));

		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// variable does not exist outside of scope
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMISubprocessIoSupport()
	  public virtual void testSequentialMISubprocessIoSupport()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["counter"] = new AtomicInteger();
		variables["nrOfLoops"] = 2;
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess", variables);

		// first sequential mi execution
		Execution miScopeExecution = runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertNotNull(miScopeExecution);
		assertEquals(0, runtimeService.getVariable(miScopeExecution.Id, "loopCounter"));

		// input mapping
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
		assertEquals(1, runtimeService.getVariableLocal(miScopeExecution.Id, "miCounterValue"));

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// second sequential mi execution
		miScopeExecution = runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertNotNull(miScopeExecution);
		assertFalse(instance.Id.Equals(miScopeExecution.Id));
		assertEquals(1, runtimeService.getVariable(miScopeExecution.Id, "loopCounter"));

		// input mapping
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
		assertEquals(2, runtimeService.getVariableLocal(miScopeExecution.Id, "miCounterValue"));

		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// variable does not exist outside of scope
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMIActivityIoSupport()
	  public virtual void testParallelMIActivityIoSupport()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["counter"] = new AtomicInteger();
		variables["nrOfLoops"] = 2;
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miParallelActivity", variables);

		ISet<int> counters = new HashSet<int>();

		// first mi execution
		Execution miExecution1 = runtimeService.createExecutionQuery().activityId("miTask").variableValueEquals("loopCounter", 0).singleResult();
		assertNotNull(miExecution1);
		assertFalse(instance.Id.Equals(miExecution1.Id));
		counters.Add((int?) runtimeService.getVariableLocal(miExecution1.Id, "miCounterValue"));

		// second mi execution
		Execution miExecution2 = runtimeService.createExecutionQuery().activityId("miTask").variableValueEquals("loopCounter", 1).singleResult();
		assertNotNull(miExecution2);
		assertFalse(instance.Id.Equals(miExecution2.Id));
		counters.Add((int?) runtimeService.getVariableLocal(miExecution2.Id, "miCounterValue"));

		assertTrue(counters.Contains(1));
		assertTrue(counters.Contains(2));

		assertEquals(2, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());

		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}

		// variable does not exist outside of scope
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMISubprocessIoSupport()
	  public virtual void testParallelMISubprocessIoSupport()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["counter"] = new AtomicInteger();
		variables["nrOfLoops"] = 2;
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miParallelSubprocess", variables);

		ISet<int> counters = new HashSet<int>();

		// first parallel mi execution
		Execution miScopeExecution1 = runtimeService.createExecutionQuery().activityId("task").variableValueEquals("loopCounter", 0).singleResult();
		assertNotNull(miScopeExecution1);
		counters.Add((int?) runtimeService.getVariableLocal(miScopeExecution1.Id, "miCounterValue"));

		// second parallel mi execution
		Execution miScopeExecution2 = runtimeService.createExecutionQuery().activityId("task").variableValueEquals("loopCounter", 1).singleResult();
		assertNotNull(miScopeExecution2);
		assertFalse(instance.Id.Equals(miScopeExecution2.Id));
		counters.Add((int?) runtimeService.getVariableLocal(miScopeExecution2.Id, "miCounterValue"));

		assertTrue(counters.Contains(1));
		assertTrue(counters.Contains(2));

		assertEquals(2, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());

		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}

		// variable does not exist outside of scope
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("miCounterValue").count());
	  }

	  public virtual void testMIOutputMappingDisallowed()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testMIOutputMappingDisallowed.bpmn20.xml").deploy();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("camunda:outputParameter not allowed for multi-instance constructs", e.Message);
		}

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testThrowErrorInScriptInputOutputMapping.bpmn")]
	  public virtual void FAILING_testBpmnErrorInScriptInputMapping()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testThrowErrorInScriptInputOutputMapping.bpmn")]
	  public virtual void testExceptionInScriptInputMapping()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "in";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testThrowErrorInScriptInputOutputMapping.bpmn")]
	  public virtual void FAILING_testBpmnErrorInScriptOutputMapping()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new BpmnError("error");
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		//we will only reach the user task if the BPMNError from the script was handled by the boundary event
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("User Task"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/iomapping/InputOutputTest.testThrowErrorInScriptInputOutputMapping.bpmn")]
	  public virtual void testExceptionInScriptOutputMapping()
	  {
		string exceptionMessage = "myException";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["throwInMapping"] = "out";
		variables["exception"] = new Exception(exceptionMessage);
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess", variables);
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(exceptionMessage));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testOutputMappingOnErrorBoundaryEvent()
	  public virtual void FAILING_testOutputMappingOnErrorBoundaryEvent()
	  {

		// case 1: no error occurs
		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskOk", task.TaskDefinitionKey);

		// then: variable mapped exists
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("localNotMapped").count());
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("localMapped").count());
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// case 2: error occurs
		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("throwError", true));

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskError", task.TaskDefinitionKey);

		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("localNotMapped").count());
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("localMapped").count());
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testOutputMappingOnMessageBoundaryEvent()
	  public virtual void FAILING_testOutputMappingOnMessageBoundaryEvent()
	  {

		// case 1: no error occurs
		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("wait", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskOk", task.TaskDefinitionKey);

		// then: variable mapped exists
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// case 2: error occurs
		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("throwError", true));

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("wait", task.TaskDefinitionKey);

		runtimeService.correlateMessage("message");

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskError", task.TaskDefinitionKey);

		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testOutputMappingOnTimerBoundaryEvent()
	  public virtual void FAILING_testOutputMappingOnTimerBoundaryEvent()
	  {

		// case 1: no error occurs
		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("wait", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskOk", task.TaskDefinitionKey);

		// then: variable mapped exists
		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// case 2: error occurs
		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("throwError", true));

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("wait", task.TaskDefinitionKey);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals("taskError", task.TaskDefinitionKey);

		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("mapped").count());

		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScopeActivityInstanceId()
	  public virtual void testScopeActivityInstanceId()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		ActivityInstance theTaskInstance = tree.getActivityInstances("theTask")[0];

		// when
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();

		// then
		assertEquals(theTaskInstance.Id, variableInstance.ActivityInstanceId);
	  }

	  public virtual void testCompositeExpressionForInputValue()
	  {

		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().receiveTask().camundaInputParameter("var", "Hello World${'!'}").endEvent("end").done();

		deployment(instance);
		runtimeService.startProcessInstanceByKey("Process");

		// when
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().variableName("var").singleResult();

		// then
		assertEquals("Hello World!", variableInstance.Value);
	  }

	  public virtual void testCompositeExpressionForOutputValue()
	  {

		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaExpression("${true}").camundaInputParameter("var1", "World!").camundaOutputParameter("var2", "Hello ${var1}").userTask().endEvent("end").done();

		deployment(instance);
		runtimeService.startProcessInstanceByKey("Process");

		// when
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().variableName("var2").singleResult();

		// then
		assertEquals("Hello World!", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOutputPlainTask()
	  public virtual void testOutputPlainTask()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process", variables);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("var").singleResult();
		assertNotNull(variable);
		assertEquals("baroque", variable.Value);
		assertEquals(pi.Id, variable.ExecutionId);
	  }
	}

}