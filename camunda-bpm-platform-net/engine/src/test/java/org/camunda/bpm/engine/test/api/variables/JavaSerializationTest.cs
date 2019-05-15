using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueDeserialized;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueDeserializedNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueSerializedJava;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueSerializedNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertUntypedNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class JavaSerializationTest
	{
		private bool InstanceFieldsInitialized = false;

		public JavaSerializationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";

	  protected internal static readonly string JAVA_DATA_FORMAT = Variables.SerializationDataFormats.JAVA.Name;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializationAsJava() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializationAsJava()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JavaSerializable javaSerializable = new JavaSerializable("foo");
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(javaSerializable).serializationDataFormat(JAVA_DATA_FORMAT).create());

		// validate untyped value
		JavaSerializable value = (JavaSerializable) runtimeService.getVariable(instance.Id, "simpleBean");

		assertEquals(javaSerializable, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertObjectValueDeserialized(typedValue, javaSerializable);
		assertObjectValueSerializedJava(typedValue, javaSerializable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObjectSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObjectSerialized()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JavaSerializable javaSerializable = new JavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.setVariable(instance.Id, "simpleBean", serializedObjectValue(serializedObject).serializationDataFormat(JAVA_DATA_FORMAT).objectTypeName(typeof(JavaSerializable).FullName).create());

		// validate untyped value
		JavaSerializable value = (JavaSerializable) runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(javaSerializable, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertObjectValueDeserialized(typedValue, javaSerializable);
		assertObjectValueSerializedJava(typedValue, javaSerializable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testJavaObjectDeserializedInFirstCommand() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testJavaObjectDeserializedInFirstCommand()
	  {

		// this test makes sure that if a serialized value is set, it can be deserialized in the same command in which it is set.

		// given
		// a serialized Java Object
		JavaSerializable javaSerializable = new JavaSerializable("foo");
		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// if
		// I start a process instance in which a Java Delegate reads the value in its deserialized form
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("varName", serializedObjectValue(serializedObject).serializationDataFormat(JAVA_DATA_FORMAT).objectTypeName(typeof(JavaSerializable).FullName).create()));

		// then
		// it does not fail
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testJavaObjectNotDeserializedIfNotRequested() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testJavaObjectNotDeserializedIfNotRequested()
	  {

		// this test makes sure that if a serialized value is set, it is not automatically deserialized if deserialization is not requested

		// given
		// a serialized Java Object
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");
		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		sbyte[] serializedObjectBytes = baos.toByteArray();
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(serializedObjectBytes), engineRule.ProcessEngine);

		thrown.expect(typeof(Exception));
		thrown.expectMessage("Exception while deserializing object");

		// which cannot be deserialized
		ObjectInputStream objectInputStream = new ObjectInputStream(new MemoryStream(serializedObjectBytes));
		objectInputStream.readObject();

		// if
		// I start a process instance in which a Java Delegate reads the value in its serialized form
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("varName", serializedObjectValue(serializedObject).serializationDataFormat(JAVA_DATA_FORMAT).objectTypeName(typeof(JavaSerializable).FullName).create()));

		// then
		// it does not fail
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObjectNullDeserialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObjectNullDeserialized()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// set null value as "deserialized" object
		runtimeService.setVariable(instance.Id, "nullObject", objectValue(null).serializationDataFormat(JAVA_DATA_FORMAT).create());

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertObjectValueDeserializedNull(typedValue);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObjectNullSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObjectNullSerialized()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// set null value as "serialized" object
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(JAVA_DATA_FORMAT).create()); // Note: no object type name provided

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue deserializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertObjectValueDeserializedNull(deserializedTypedValue);

		ObjectValue serializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject", false);
		assertObjectValueSerializedNull(serializedTypedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObjectNullSerializedObjectTypeName() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObjectNullSerializedObjectTypeName()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string typeName = "some.type.Name";

		// set null value as "serialized" object
		runtimeService.setVariable(instance.Id, "nullObject", serializedObjectValue().serializationDataFormat(JAVA_DATA_FORMAT).objectTypeName(typeName).create());

		// get null value via untyped api
		assertNull(runtimeService.getVariable(instance.Id, "nullObject"));

		// get null via typed api
		ObjectValue deserializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject");
		assertNotNull(deserializedTypedValue);
		assertTrue(deserializedTypedValue.Deserialized);
		assertEquals(JAVA_DATA_FORMAT, deserializedTypedValue.SerializationDataFormat);
		assertNull(deserializedTypedValue.Value);
		assertNull(deserializedTypedValue.ValueSerialized);
		assertNull(deserializedTypedValue.ObjectType);
		assertEquals(typeName, deserializedTypedValue.ObjectTypeName);

		ObjectValue serializedTypedValue = runtimeService.getVariableTyped(instance.Id, "nullObject", false);
		assertNotNull(serializedTypedValue);
		assertFalse(serializedTypedValue.Deserialized);
		assertEquals(JAVA_DATA_FORMAT, serializedTypedValue.SerializationDataFormat);
		assertNull(serializedTypedValue.ValueSerialized);
		assertEquals(typeName, serializedTypedValue.ObjectTypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetUntypedNullForExistingVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetUntypedNullForExistingVariable()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// initially the variable has a value
		JavaSerializable javaSerializable = new JavaSerializable("foo");

		runtimeService.setVariable(instance.Id, "varName", objectValue(javaSerializable).serializationDataFormat(JAVA_DATA_FORMAT).create());

		// get value via untyped api
		assertEquals(javaSerializable, runtimeService.getVariable(instance.Id, "varName"));

		// set the variable to null via untyped Api
		runtimeService.setVariable(instance.Id, "varName", null);

		// variable is now untyped null
		TypedValue nullValue = runtimeService.getVariableTyped(instance.Id, "varName");
		assertUntypedNullValue(nullValue);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetTypedNullForExistingVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetTypedNullForExistingVariable()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// initially the variable has a value
		JavaSerializable javaSerializable = new JavaSerializable("foo");

		runtimeService.setVariable(instance.Id, "varName", objectValue(javaSerializable).serializationDataFormat(JAVA_DATA_FORMAT).create());

		// get value via untyped api
		assertEquals(javaSerializable, runtimeService.getVariable(instance.Id, "varName"));

		// set the variable to null via typed Api
		runtimeService.setVariable(instance.Id, "varName", objectValue(null));

		// variable is still of type object
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "varName");
		assertObjectValueDeserializedNull(typedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStandaloneTaskTransientVariable() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStandaloneTaskTransientVariable()
	  {
		Task task = taskService.newTask();
		task.Name = "gonzoTask";
		taskService.saveTask(task);
		string taskId = task.Id;
		try
		{

		  MemoryStream baos = new MemoryStream();
		  (new ObjectOutputStream(baos)).writeObject("trumpet");
		  string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  taskService.setVariable(taskId, "instrument", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(string).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).setTransient(true));
		  assertEquals("trumpet", taskService.getVariable(taskId, "instrument"));
		}
		finally
		{
		  taskService.deleteTask(taskId, true);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransientObjectValue() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransientObjectValue()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().exclusiveGateway("gtw").sequenceFlowId("flow1").condition("cond", "${x.property == \"bar\"}").userTask("userTask1").endEvent().moveToLastGateway().sequenceFlowId("flow2").userTask("userTask2").endEvent().done();

		testRule.deploy(modelInstance);

		JavaSerializable bean = new JavaSerializable("bar");
		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(bean);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue javaValue = serializedObjectValue(serializedObject, true).serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName(typeof(JavaSerializable).FullName).create();
		VariableMap variables = Variables.createVariables().putValueTyped("x", javaValue);

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