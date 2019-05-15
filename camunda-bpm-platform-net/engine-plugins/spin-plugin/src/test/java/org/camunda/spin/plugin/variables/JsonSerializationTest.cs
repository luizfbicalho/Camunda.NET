using System;
using System.Collections.Generic;
using System.Text;

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
	using JSONException = org.json.JSONException;
	using JSONAssert = org.skyscreamer.jsonassert.JSONAssert;

	public class JsonSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";
	  protected internal const string SERVICE_TASK_PROCESS = "org/camunda/spin/plugin/serviceTaskProcess.bpmn20.xml";

	  protected internal static readonly string JSON_FORMAT_NAME = DataFormats.JSON_DATAFORMAT_NAME;

	  protected internal string originalSerializationFormat;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSerializationAsJson() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializationAsJson()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		// request object to be serialized as JSON
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(JSON_FORMAT_NAME).create());

		// validate untyped value
		object value = runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(bean, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertEquals(ValueType.OBJECT, typedValue.Type);

		assertTrue(typedValue.Deserialized);

		assertEquals(bean, typedValue.Value);
		assertEquals(bean, typedValue.getValue(typeof(JsonSerializable)));
		assertEquals(typeof(JsonSerializable), typedValue.ObjectType);

		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(JsonSerializable).FullName, typedValue.ObjectTypeName);
		JSONAssert.assertEquals(bean.toExpectedJsonString(), new string(typedValue.ValueSerialized), true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testListSerializationAsJson() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testListSerializationAsJson()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<JsonSerializable> beans = new List<JsonSerializable>();
		for (int i = 0; i < 20; i++)
		{
		  beans.Add(new JsonSerializable("a String" + i, 42 + i, true));
		}

		runtimeService.setVariable(instance.Id, "simpleBeans", objectValue(beans).serializationDataFormat(JSON_FORMAT_NAME).create());

		// validate untyped value
		object value = runtimeService.getVariable(instance.Id, "simpleBeans");
		assertEquals(beans, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBeans");
		assertEquals(ValueType.OBJECT, typedValue.Type);
		assertEquals(beans, typedValue.Value);
		assertTrue(typedValue.Deserialized);
		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
		assertNotNull(typedValue.ObjectTypeName);
		JSONAssert.assertEquals(toExpectedJsonArray(beans), new string(typedValue.ValueSerialized), true);

	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailingSerialization()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		FailingSerializationBean failingBean = new FailingSerializationBean("a String", 42, true);

		try
		{
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(failingBean).serializationDataFormat(JSON_FORMAT_NAME));
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailingDeserialization()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		FailingDeserializationBean failingBean = new FailingDeserializationBean("a String", 42, true);

		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(failingBean).serializationDataFormat(JSON_FORMAT_NAME));

		try
		{
		  runtimeService.getVariable(instance.Id, "simpleBean");
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
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
		  objectValue.getValue(typeof(JsonSerializable));
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

		JsonSerializable jsonSerializable = new JsonSerializable();

		try
		{
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(jsonSerializable).serializationDataFormat("non existing data format"));
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
		  private readonly JsonSerializationTest outerInstance;

		  private ProcessInstance instance;

		  public CommandAnonymousInnerClass(JsonSerializationTest outerInstance, ProcessInstance instance)
		  {
			  this.outerInstance = outerInstance;
			  this.instance = instance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			JsonSerializable bean = new JsonSerializable("a String", 42, true);
			outerInstance.runtimeService.setVariable(instance.Id, "simpleBean", bean);

			object returnedBean = outerInstance.runtimeService.getVariable(instance.Id, "simpleBean");
			assertSame(bean, returnedBean);

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testGetSerializedVariableValue() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testGetSerializedVariableValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(JSON_FORMAT_NAME).create());

		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean", false);

		string serializedValue = typedValue.ValueSerialized;
		JSONAssert.assertEquals(bean.toExpectedJsonString(), serializedValue, true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetSerializedVariableValue() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValue()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		string beanAsJson = bean.toExpectedJsonString();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsJson).serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(bean.GetType().FullName);

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// java object can be retrieved
		JsonSerializable returnedBean = (JsonSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(bean, returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertEquals(bean, typedValue.Value);
		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		assertEquals(bean.GetType().FullName, typedValue.ObjectTypeName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetSerializedVariableValueNoTypeName() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNoTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		string beanAsJson = bean.toExpectedJsonString();

		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsJson).serializationDataFormat(JSON_FORMAT_NAME);
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetSerializedVariableValueMismatchingTypeName() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueMismatchingTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		string beanAsJson = bean.toExpectedJsonString();

		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsJson).serializationDataFormat(JSON_FORMAT_NAME).objectTypeName("Insensible type name."); // < not a valid type name

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		try
		{
		  runtimeService.getVariable(instance.Id, "simpleBean");
		  fail("Exception expected.");
		}
		catch (Exception)
		{
		  // happy path
		}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		serializedValue = serializedObjectValue(beanAsJson).serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(typeof(JsonSerializationTest).FullName); // < not the right type name

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		try
		{
		  runtimeService.getVariable(instance.Id, "simpleBean");
		  fail("Exception expected.");
		}
		catch (Exception)
		{
		  // happy path
		}
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetSerializedVariableValueNull() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNull()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		SerializedObjectValueBuilder serializedValue = serializedObjectValue().serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(typeof(JsonSerializable).FullName);

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// null can be retrieved
		JsonSerializable returnedBean = (JsonSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertNull(returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertNull(typedValue.Value);
		assertNull(typedValue.ValueSerialized);
		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		assertEquals(typeof(JsonSerializable).FullName, typedValue.ObjectTypeName);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetSerializedVariableValueNullNoTypeName() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetSerializedVariableValueNullNoTypeName()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		SerializedObjectValueBuilder serializedValue = serializedObjectValue().serializationDataFormat(JSON_FORMAT_NAME);
		// no objectTypeName specified

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// null can be retrieved
		JsonSerializable returnedBean = (JsonSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertNull(returnedBean);

		// validate typed value metadata
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertNull(typedValue.Value);
		assertNull(typedValue.ValueSerialized);
		assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
		assertNull(typedValue.ObjectTypeName);
	  }

	  protected internal virtual string toExpectedJsonArray(IList<JsonSerializable> beans)
	  {
		StringBuilder jsonBuilder = new StringBuilder();

		jsonBuilder.Append("[");
		for (int i = 0; i < beans.Count; i++)
		{
		  jsonBuilder.Append(beans[i].toExpectedJsonString());

		  if (i != beans.Count - 1)
		  {
			jsonBuilder.Append(", ");
		  }
		}
		jsonBuilder.Append("]");

		return jsonBuilder.ToString();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaOjectNullDeserialized() throws Exception
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaOjectNullDeserialized()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// set null value as "deserialized" object
		runtimeService.setVariable(instance.Id, "nullObject", objectValue(null).serializationDataFormat(JSON_FORMAT_NAME).create());

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
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(JSON_FORMAT_NAME).create()); // Note: no object type name provided

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
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(typeName).create());

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue deserializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertNotNull(deserializedTypedValue);
		assertTrue(deserializedTypedValue.Deserialized);
		assertEquals(JSON_FORMAT_NAME, deserializedTypedValue.SerializationDataFormat);
		assertNull(deserializedTypedValue.Value);
		assertNull(deserializedTypedValue.ValueSerialized);
		assertNull(deserializedTypedValue.ObjectType);
		assertEquals(typeName, deserializedTypedValue.ObjectTypeName);

		ObjectValue serializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject", false);
		assertNotNull(serializedTypedValue);
		assertFalse(serializedTypedValue.Deserialized);
		assertEquals(JSON_FORMAT_NAME, serializedTypedValue.SerializationDataFormat);
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
		JsonSerializable @object = new JsonSerializable();

		runtimeService.setVariable(instance.Id, "varName", objectValue(@object).serializationDataFormat(JSON_FORMAT_NAME).create());

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
		JsonSerializable javaSerializable = new JsonSerializable();

		runtimeService.setVariable(instance.Id, "varName", objectValue(javaSerializable).serializationDataFormat(JSON_FORMAT_NAME).create());

		// get value via untyped api
		assertEquals(javaSerializable, runtimeService.getVariable(instance.Id, "varName"));

		// set the variable to null via typed Api
		runtimeService.setVariable(instance.Id, "varName", objectValue(null));

		// variable is still of type object
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "varName");
		assertObjectValueDeserializedNull(typedValue);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testRemoveVariable() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testRemoveVariable()
	  {
		// given a serialized json variable
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		JsonSerializable bean = new JsonSerializable("a String", 42, true);
		string beanAsJson = bean.toExpectedJsonString();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		SerializedObjectValueBuilder serializedValue = serializedObjectValue(beanAsJson).serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(bean.GetType().FullName);

		runtimeService.setVariable(instance.Id, "simpleBean", serializedValue);

		// when
		runtimeService.removeVariable(instance.Id, "simpleBean");

		// then
		assertNull(runtimeService.getVariable(instance.Id, "simpleBean"));
		assertNull(runtimeService.getVariableTyped(instance.Id, "simpleBean"));
		assertNull(runtimeService.getVariableTyped(instance.Id, "simpleBean", false));
	  }


	  /// <summary>
	  /// CAM-3222
	  /// </summary>
	  [Deployment(resources : SERVICE_TASK_PROCESS)]
	  public virtual void testImplicitlyUpdateEmptyList()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("serviceTaskProcess", Variables.createVariables().putValueTyped("listVar", Variables.objectValue(new List<JsonSerializable>()).serializationDataFormat("application/json").create()).putValue("delegate", new UpdateValueDelegate()));

		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "listVar");
		// this should match Jackson's format
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string expectedTypeName = typeof(List<object>).FullName + "<" + typeof(JsonSerializable).FullName + ">";
		assertEquals(expectedTypeName, typedValue.ObjectTypeName);

		IList<JsonSerializable> list = (IList<JsonSerializable>) typedValue.Value;
		assertEquals(1, list.Count);
		assertTrue(list[0] is JsonSerializable);
		assertEquals(UpdateValueDelegate.STRING_PROPERTY, list[0].StringProperty);
	  }

	  public virtual void testTransientJsonValue()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().exclusiveGateway("gtw").sequenceFlowId("flow1").condition("cond", "${x.stringProperty == \"bar\"}").userTask("userTask1").endEvent().moveToLastGateway().sequenceFlowId("flow2").userTask("userTask2").endEvent().done();

		deployment(modelInstance);

		JsonSerializable bean = new JsonSerializable("bar", 42, true);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue jsonValue = serializedObjectValue(bean.toExpectedJsonString(), true).serializationDataFormat(JSON_FORMAT_NAME).objectTypeName(typeof(JsonSerializable).FullName).create();
		VariableMap variables = Variables.createVariables().putValueTyped("x", jsonValue);

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