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
//	import static org.camunda.spin.DataFormats.xml;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.SpinValues.xmlValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML;


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
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using XmlValue = org.camunda.spin.plugin.variable.value.XmlValue;
	using XmlValueBuilder = org.camunda.spin.plugin.variable.value.builder.XmlValueBuilder;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class XmlValueTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";
	  protected internal static readonly string XML_FORMAT_NAME = DataFormats.XML_DATAFORMAT_NAME;

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";

	  protected internal string xmlString = "<elementName attrName=\"attrValue\" />";
	  protected internal string brokenXmlString = "<elementName attrName=attrValue\" />";

	  protected internal string variableName = "x";

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetUntypedXmlValue()
	  {
		// given
		XmlValue xmlValue = xmlValue(xmlString).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, xmlValue);

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// when
		SpinXmlElement value = (SpinXmlElement) runtimeService.getVariable(processInstanceId, variableName);

		// then
		assertTrue(value.hasAttr("attrName"));
		assertEquals("attrValue", value.attr("attrName").value());
		assertTrue(value.childElements().Empty);
		assertEquals(xml().Name, value.DataFormatName);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetTypedXmlValue()
	  {
		// given
		XmlValue xmlValue = xmlValue(xmlString).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, xmlValue);

		string processInstanceId = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// when
		XmlValue typedValue = runtimeService.getVariableTyped(processInstanceId, variableName);

		// then
		SpinXmlElement value = typedValue.Value;
		assertTrue(value.hasAttr("attrName"));
		assertEquals("attrValue", value.attr("attrName").value());
		assertTrue(value.childElements().Empty);

		assertTrue(typedValue.Deserialized);
		assertEquals(XML, typedValue.Type);
		assertEquals(XML_FORMAT_NAME, typedValue.SerializationDataFormat);
		assertEquals(xmlString, typedValue.ValueSerialized);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testBrokenXmlSerialization()
	  {
		// given
		XmlValue value = xmlValue(brokenXmlString).create();

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
		XmlValue value = xmlValue(brokenXmlString).create();

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
		XmlValue xmlValue = runtimeService.getVariableTyped(processInstanceId, variableName, false);
		assertFalse(xmlValue.Deserialized);
		assertEquals(brokenXmlString, xmlValue.ValueSerialized);

		// but not the deserialized properties
		try
		{
		  xmlValue.Value;
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
		XmlValueBuilder builder = xmlValue(xmlString).serializationDataFormat("non existing data format");
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

	  [Deployment(resources : "org/camunda/spin/plugin/xmlConditionProcess.bpmn20.xml")]
	  public virtual void testXmlValueInCondition()
	  {
		// given
		string xmlString = "<customer age=\"22\" />";
		XmlValue value = xmlValue(xmlString).create();
		VariableMap variables = Variables.createVariables().putValueTyped("customer", value);

		// when
		runtimeService.startProcessInstanceByKey("process", variables);

		// then
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task1", task.TaskDefinitionKey);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testTransientXmlValueFluent()
	  {
		// given
		XmlValue xmlValue = xmlValue(xmlString).setTransient(true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, xmlValue);

		// when
		runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY, variables).Id;

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testTransientXmlValue()
	  {
		// given
		XmlValue xmlValue = xmlValue(xmlString, true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, xmlValue);

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
		XmlValue xmlValue = (XmlValue) org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML.createValueFromSerialized(xmlString, valueInfo);

		// then
		assertEquals(true, xmlValue.Transient);
		IDictionary<string, object> returnedValueInfo = org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML.getValueInfo(xmlValue);
		assertEquals(true, returnedValueInfo[ValueType.VALUE_INFO_TRANSIENT]);
	  }

	  public virtual void testDeserializeTransientXmlValue()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().exclusiveGateway("gtw").sequenceFlowId("flow1").condition("cond", "${XML(" + variableName + ").attr('attrName').value() == 'attrValue'}").userTask("userTask1").endEvent().moveToLastGateway().sequenceFlowId("flow2").userTask("userTask2").endEvent().done();

		deployment(modelInstance);

		XmlValue xmlValue = xmlValue(xmlString, true).create();
		VariableMap variables = Variables.createVariables().putValueTyped(variableName, xmlValue);

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