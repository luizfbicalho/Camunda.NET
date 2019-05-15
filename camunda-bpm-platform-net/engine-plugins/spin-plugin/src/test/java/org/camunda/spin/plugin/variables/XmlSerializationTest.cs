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
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variables.TypedValueAssert.assertObjectValueDeserializedNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variables.TypedValueAssert.assertObjectValueSerializedNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variables.TypedValueAssert.assertUntypedNullValue;

	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using SerializedObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.SerializedObjectValueBuilder;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	public class XmlSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";

	  protected internal static readonly string XML_FORMAT_NAME = DataFormats.XML_DATAFORMAT_NAME;

	  protected internal string originalSerializationFormat;

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializationAsXml()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		XmlSerializable bean = new XmlSerializable("a String", 42, true);
		// request object to be serialized as XML
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(XML_FORMAT_NAME).create());

		// validate untyped value
		object value = runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(bean, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertEquals(ValueType.OBJECT, typedValue.Type);

		assertTrue(typedValue.Deserialized);

		assertEquals(bean, typedValue.Value);
		assertEquals(bean, typedValue.getValue(typeof(XmlSerializable)));
		assertEquals(typeof(XmlSerializable), typedValue.ObjectType);

		assertEquals(XML_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(XmlSerializable).FullName, typedValue.ObjectTypeName);
		SpinXmlElement serializedValue = Spin.XML(typedValue.ValueSerialized);
		assertEquals(bean.StringProperty, serializedValue.childElement("stringProperty").textContent());
		assertEquals(bean.BooleanProperty, bool.Parse(serializedValue.childElement("booleanProperty").textContent()));
		assertEquals(bean.IntProperty, int.Parse(serializedValue.childElement("intProperty").textContent()));
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailingSerialization()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		FailingXmlSerializable failingBean = new FailingXmlSerializable("a String", 42, true);

		try
		{
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(failingBean).serializationDataFormat(XML_FORMAT_NAME));
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("I am failing", e.Message);
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailingDeserialization()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		FailingXmlDeserializationBean failingBean = new FailingXmlDeserializationBean("a String", 42, true);

		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(failingBean).serializationDataFormat(XML_FORMAT_NAME));

		try
		{
		  runtimeService.getVariable(instance.Id, "simpleBean");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot deserialize object in variable 'simpleBean'", e.Message);
		}

		try
		{
		  runtimeService.getVariableTyped(instance.Id, "simpleBean");
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		// However, I can access the serialized value
		ObjectValue objectValue = runtimeService.getVariableTyped(instance.Id, "simpleBean", false);
		assertFalse(objectValue.Deserialized);
		assertNotNull(objectValue.ObjectTypeName);
		assertNotNull(objectValue.ValueSerialized);
		// but not the deserialized properties
		try
		{
		  objectValue.Value;
		  fail("exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("Object is not deserialized", e.Message);
		}

		try
		{
		  objectValue.getValue(typeof(XmlSerializable));
		  fail("exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("Object is not deserialized", e.Message);
		}

		try
		{
		  objectValue.ObjectType;
		  fail("exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("Object is not deserialized", e.Message);
		}

	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailForNonExistingSerializationFormat()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		XmlSerializable XmlSerializable = new XmlSerializable();

		try
		{
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(XmlSerializable).serializationDataFormat("non existing data format"));
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find serializer for value", e.Message);
		  // happy path
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testVariableValueCaching()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, instance));

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();

		object returnedBean = variableInstance.Value;
		object theSameReturnedBean = variableInstance.Value;
		assertSame(returnedBean, theSameReturnedBean);
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly XmlSerializationTest outerInstance;

		  private ProcessInstance instance;

		  public CommandAnonymousInnerClass(XmlSerializationTest outerInstance, ProcessInstance instance)
		  {
			  this.outerInstance = outerInstance;
			  this.instance = instance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			XmlSerializable bean = new XmlSerializable("a String", 42, true);
			outerInstance.runtimeService.setVariable(instance.Id, "simpleBean", bean);

			object returnedBean = outerInstance.runtimeService.getVariable(instance.Id, "simpleBean");
			assertSame(bean, returnedBean);

			return null;
		  }
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetSerializedVariableValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		XmlSerializable bean = new XmlSerializable("a String", 42, true);
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(XML_FORMAT_NAME).create());

		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean", false);

		SpinXmlElement serializedValue = Spin.XML(typedValue.ValueSerialized);
		assertEquals(bean.StringProperty, serializedValue.childElement("stringProperty").textContent());
		assertEquals(bean.BooleanProperty, bool.Parse(serializedValue.childElement("booleanProperty").textContent()));
		assertEquals(bean.IntProperty, int.Parse(serializedValue.childElement("intProperty").textContent()));
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		XmlSerializable bean = new XmlSerializable("a String", 42, true);
		string beanAsXml = bean.toExpectedXmlString();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsXml).serializationDataFormat(XML_FORMAT_NAME).objectTypeName(bean.GetType().FullName);

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// java object can be retrieved
		XmlSerializable returnedBean = (XmlSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(bean, returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertEquals(bean, typedValue.Value);
		assertEquals(XML_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		assertEquals(bean.GetType().FullName, typedValue.ObjectTypeName);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNoTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		XmlSerializable bean = new XmlSerializable("a String", 42, true);
		string beanAsXml = bean.toExpectedXmlString();

		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsXml).serializationDataFormat(XML_FORMAT_NAME);
		  // no type name

		try
		{
		  runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);
		  fail("Exception expected.");
		}
		catch (Exception e)
		{
		  assertTextPresent("no 'objectTypeName' provided for non-null value", e.Message);
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueMismatchingTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		XmlSerializable bean = new XmlSerializable("a String", 42, true);
		string beanAsXml = bean.toExpectedXmlString();

		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsXml).serializationDataFormat(XML_FORMAT_NAME).objectTypeName("Insensible type name."); // < not a valid type name

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		try
		{
		  runtimeService.getVariable(instance.Id, "simpleBean");
		  fail("Exception expected.");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }


	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNull()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		SerializedObjectValueBuilder serializedValue = serializedObjectValue().serializationDataFormat(XML_FORMAT_NAME).objectTypeName(typeof(XmlSerializable).FullName);

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// null can be retrieved
		XmlSerializable returnedBean = (XmlSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertNull(returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertNull(typedValue.Value);
		assertNull(typedValue.ValueSerialized);
		assertEquals(XML_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		assertEquals(typeof(XmlSerializable).FullName, typedValue.ObjectTypeName);

	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNullNoTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		SerializedObjectValueBuilder serializedValue = serializedObjectValue().serializationDataFormat(XML_FORMAT_NAME);
		// no objectTypeName specified

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// null can be retrieved
		XmlSerializable returnedBean = (XmlSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertNull(returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertNull(typedValue.Value);
		assertNull(typedValue.ValueSerialized);
		assertEquals(XML_FORMAT_NAME, typedValue.SerializationDataFormat);
		assertNull(typedValue.ObjectTypeName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaOjectNullDeserialized() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaOjectNullDeserialized()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// set null value as "deserialized" object
		runtimeService.setVariable(instance.Id, "nullObject", objectValue(null).serializationDataFormat(XML_FORMAT_NAME).create());

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertObjectValueDeserializedNull(typedValue);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaOjectNullSerialized() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaOjectNullSerialized()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// set null value as "serialized" object
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(XML_FORMAT_NAME).create()); // Note: no object type name provided

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue deserializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertObjectValueDeserializedNull(deserializedTypedValue);

		ObjectValue serializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject", false);
		assertObjectValueSerializedNull(serializedTypedValue);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaOjectNullSerializedObjectTypeName() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaOjectNullSerializedObjectTypeName()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string typeName = "some.type.Name";

		// set null value as "serialized" object
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(XML_FORMAT_NAME).objectTypeName(typeName).create());

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue deserializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertNotNull(deserializedTypedValue);
		assertTrue(deserializedTypedValue.Deserialized);
		assertEquals(XML_FORMAT_NAME, deserializedTypedValue.SerializationDataFormat);
		assertNull(deserializedTypedValue.Value);
		assertNull(deserializedTypedValue.ValueSerialized);
		assertNull(deserializedTypedValue.ObjectType);
		assertEquals(typeName, deserializedTypedValue.ObjectTypeName);

		ObjectValue serializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject", false);
		assertNotNull(serializedTypedValue);
		assertFalse(serializedTypedValue.Deserialized);
		assertEquals(XML_FORMAT_NAME, serializedTypedValue.SerializationDataFormat);
		assertNull(serializedTypedValue.ValueSerialized);
		assertEquals(typeName, serializedTypedValue.ObjectTypeName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetUntypedNullForExistingVariable() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetUntypedNullForExistingVariable()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// initially the variable has a value
		XmlSerializable @object = new XmlSerializable();

		runtimeService.setVariable(instance.Id, "varName", objectValue(@object).serializationDataFormat(XML_FORMAT_NAME).create());

		// get value via untyped api
		assertEquals(@object, runtimeService.getVariable(instance.Id, "varName"));

		// set the variable to null via untyped Api
		runtimeService.setVariable(instance.Id, "varName", null);

		// variable is now untyped null
		TypedValue nullValue = runtimeService.getVariableTyped(instance.Id, "varName");
		assertUntypedNullValue(nullValue);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetTypedNullForExistingVariable() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetTypedNullForExistingVariable()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// initially the variable has a value
		XmlSerializable javaSerializable = new XmlSerializable();

		runtimeService.setVariable(instance.Id, "varName", objectValue(javaSerializable).serializationDataFormat(XML_FORMAT_NAME).create());

		// get value via untyped api
		assertEquals(javaSerializable, runtimeService.getVariable(instance.Id, "varName"));

		// set the variable to null via typed Api
		runtimeService.setVariable(instance.Id, "varName", objectValue(null));

		// variable is still of type object
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "varName");
		assertObjectValueDeserializedNull(typedValue);
	  }

	  public virtual void testTransientXmlValue()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().exclusiveGateway("gtw").sequenceFlowId("flow1").condition("cond", "${x.stringProperty == \"bar\"}").userTask("userTask1").endEvent().moveToLastGateway().sequenceFlowId("flow2").userTask("userTask2").endEvent().done();

		deployment(modelInstance);

		XmlSerializable bean = new XmlSerializable("bar", 42, true);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue xmlValue = serializedObjectValue(bean.toExpectedXmlString(), true).serializationDataFormat(XML_FORMAT_NAME).objectTypeName(typeof(XmlSerializable).FullName).create();
		VariableMap variables = Variables.createVariables().putValueTyped("x", xmlValue);

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