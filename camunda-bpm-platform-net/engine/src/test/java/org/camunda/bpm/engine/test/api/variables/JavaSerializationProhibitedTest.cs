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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using JavaObjectSerializer = org.camunda.bpm.engine.impl.variable.serializer.JavaObjectSerializer;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueDeserialized;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueSerializedJava;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class JavaSerializationProhibitedTest
	{
		private bool InstanceFieldsInitialized = false;

		public JavaSerializationProhibitedTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			engineRule = new ProvidedProcessEngineRule(bootstrapRule);
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";

	  protected internal static readonly string JAVA_DATA_FORMAT = Variables.SerializationDataFormats.JAVA.Name;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule();

	  protected internal ProvidedProcessEngineRule engineRule;
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
		((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration).VariableSerializers.addSerializer(new JavaCustomSerializer(this));
	  }

	  //still works for normal objects (not serialized)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObject() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObject()
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

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot set variable with name simpleBean. Java serialization format is prohibited");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.setVariable(instance.Id, "simpleBean", serializedObjectValue(serializedObject).serializationDataFormat(JAVA_DATA_FORMAT).objectTypeName(typeof(JavaSerializable).FullName).create());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSetJavaObjectSerializedEmptySerializationDataFormat() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSetJavaObjectSerializedEmptySerializationDataFormat()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JavaSerializable javaSerializable = new JavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot set variable with name simpleBean. Java serialization format is prohibited");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.setVariable(instance.Id, "simpleBean", serializedObjectValue(serializedObject).objectTypeName(typeof(JavaSerializable).FullName).create());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStandaloneTaskTransientVariableSerializedObject()
	  public virtual void testStandaloneTaskTransientVariableSerializedObject()
	  {
		Task task = taskService.newTask();
		task.Name = "gonzoTask";
		taskService.saveTask(task);
		string taskId = task.Id;

		try
		{
		  thrown.expect(typeof(ProcessEngineException));
		  thrown.expectMessage("Cannot set variable with name instrument. Java serialization format is prohibited");

		  taskService.setVariable(taskId, "instrument", Variables.serializedObjectValue("any value").serializationDataFormat(Variables.SerializationDataFormats.JAVA).setTransient(true).create());
		}
		finally
		{
		  taskService.deleteTask(taskId, true);
		}

	  }

	  private class JavaCustomSerializer : JavaObjectSerializer
	  {
		  private readonly JavaSerializationProhibitedTest outerInstance;

		  public JavaCustomSerializer(JavaSerializationProhibitedTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		protected internal override bool canWriteValue(TypedValue typedValue)
		{
		  //do NOT check serializationDataFormat
		  return true;
		}
	  }
	}

}