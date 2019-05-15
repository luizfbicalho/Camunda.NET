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
namespace org.camunda.spin.plugin.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.DataFormats.json;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.SpinValues.jsonValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.type.SpinValueType_Fields.JSON;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using JsonValue = org.camunda.spin.plugin.variable.value.JsonValue;
	using JsonValueBuilder = org.camunda.spin.plugin.variable.value.builder.JsonValueBuilder;
	using JSONException = org.json.JSONException;
	using JSONAssert = org.skyscreamer.jsonassert.JSONAssert;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class JsonValueTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";
	  protected internal static readonly string JSON_FORMAT_NAME = DataFormats.JSON_DATAFORMAT_NAME;

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";

	  protected internal string jsonString = "{\"foo\": \"bar\"}";
	  protected internal string brokenJsonString = "{\"foo: \"bar\"}";

	  protected internal string variableName = "x";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testGetUntypedJsonValue() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetUntypedJsonValue()
	  {
		// given
		JsonValue jsonValue = jsonValue(jsonString).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, jsonValue);

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// when
		SpinJsonNode value = (SpinJsonNode) runtimeService.getVariable(processInstanceId, variableName);

		// then
		JSONAssert.assertEquals(jsonString, value.ToString(), true);
		assertEquals(json().Name, value.DataFormatName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testGetTypedJsonValue() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetTypedJsonValue()
	  {
		// given
		JsonValue jsonValue = jsonValue(jsonString).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, jsonValue);

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// when
		JsonValue typedValue = runtimeService.getVariableTyped(processInstanceId, variableName);

		// then
		SpinJsonNode value = typedValue.Value;
		JSONAssert.assertEquals(jsonString, value.ToString(), true);

		assertTrue(typedValue.Deserialized);
		assertEquals(JSON, typedValue.Type);
		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
		JSONAssert.assertEquals(jsonString, typedValue.ValueSerialized, true);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testBrokenJsonSerialization()
	  {
		// given
		JsonValue value = jsonValue(brokenJsonString).create();

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when
		  runtimeService.setVariable(processInstanceId, variableName, value);
		}
		catch (Exception)
		{
		  fail("no exception expected");
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailingDeserialization()
	  {
		// given
		JsonValue value = jsonValue(brokenJsonString).create();

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		runtimeService.setVariable(processInstanceId, variableName, value);

		try
		{
		  // when
		  runtimeService.getVariable(processInstanceId, variableName);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		try
		{
		  runtimeService.getVariableTyped(processInstanceId, variableName);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		// However, I can access the serialized value
		JsonValue jsonValue = runtimeService.getVariableTyped(processInstanceId, variableName, false);
		assertFalse(jsonValue.Deserialized);
		assertEquals(brokenJsonString, jsonValue.ValueSerialized);

		// but not the deserialized properties
		try
		{
		  jsonValue.Value;
		  fail("exception expected");
		}
		catch (SpinRuntimeException)
		{
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailForNonExistingSerializationFormat()
	  {
		// given
		JsonValueBuilder builder = jsonValue(jsonString).serializationDataFormat("non existing data format");
		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;

		try
		{
		  // when (1)
		  runtimeService.setVariable(processInstanceId, variableName, builder);
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then (1)
		  assertTextPresent("Cannot find serializer for value", e.Message);
		  // happy path
		}

		try
		{
		  // when (2)
		  runtimeService.setVariable(processInstanceId, variableName, builder.create());
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then (2)
		  assertTextPresent("Cannot find serializer for value", e.Message);
		  // happy path
		}
	  }

	  [Deployment(resources : "org/camunda/spin/plugin/jsonConditionProcess.bpmn20.xml")]
	  public virtual void testJsonValueInCondition()
	  {
		// given
		string jsonString = "{\"age\": 22 }";
		JsonValue value = jsonValue(jsonString).create();
		VariableMap variables = Variables.createVariables().putValueTyped("customer", value);

		// when
		runtimeService.startProcessInstanceByKey("process", variables);

		// then
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task1", task.TaskDefinitionKey);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testTransientJsonValueFluent()
	  {
		// given
		JsonValue jsonValue = jsonValue(jsonString).setTransient(true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, jsonValue);

		// when
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testTransientJsonValue()
	  {
		// given
		JsonValue jsonValue = jsonValue(jsonString, true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, jsonValue);

		// when
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
	  }

	  public virtual void testApplyValueInfoFromSerializedValue()
	  {
		// given
		IDictionary<string, object> valueInfo = new Dictionary<string, object>();
		valueInfo[ValueType.VALUE_INFO_TRANSIENT] = true;

		// when
		JsonValue jsonValue = (JsonValue) org.camunda.spin.plugin.variable.type.SpinValueType_Fields.JSON.createValueFromSerialized(jsonString, valueInfo);

		// then
		assertEquals(true, jsonValue.Transient);
		IDictionary<string, object> returnedValueInfo = org.camunda.spin.plugin.variable.type.SpinValueType_Fields.JSON.getValueInfo(jsonValue);
		assertEquals(true, returnedValueInfo[ValueType.VALUE_INFO_TRANSIENT]);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9932
	  /// </summary>
	  public virtual void FAILING_testTransientJsonSpinVariables()
	  {
		repositoryService.createDeployment().addModelInstance("model.bpmn", Bpmn.createExecutableProcess("aProcess").startEvent().serviceTask().camundaClass(typeof(JsonDelegate)).endEvent().done()).deploy();

		runtimeService.startProcessInstanceByKey("aProcess").Id;
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9932
	  /// </summary>
	  public virtual void FAILING_testTransientXmlSpinVariables()
	  {
		repositoryService.createDeployment().addModelInstance("model.bpmn", Bpmn.createExecutableProcess("aProcess").startEvent().serviceTask().camundaClass(typeof(XmlDelegate)).endEvent().done()).deploy();

		runtimeService.startProcessInstanceByKey("aProcess").Id;
	  }

	  public virtual void testDeserializeTransientJsonValue()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().exclusiveGateway("gtw").sequenceFlowId("flow1").condition("cond", "${S(" + variableName + ").prop(\"foo\").stringValue() == \"bar\"}").userTask("userTask1").endEvent().moveToLastGateway().sequenceFlowId("flow2").userTask("userTask2").endEvent().done();

		deployment(modelInstance);

		JsonValue jsonValue = jsonValue(jsonString, true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, jsonValue);

		// when
		runtimeService.startProcessInstanceByKey("foo", variables);

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("userTask1", task.TaskDefinitionKey);
	  }

	}

}