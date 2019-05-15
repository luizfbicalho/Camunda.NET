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
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using JSONException = org.json.JSONException;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;

	/// <summary>
	/// Here we test how the engine behaves, when more than one object serializers are available.
	/// 
	/// @author Svetlana Dorokhova
	/// </summary>
	public class JavaSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSerializationAsJava() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializationAsJava()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JavaSerializable bean = new JavaSerializable("a String", 42, true);
		// request object to be serialized as Java
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create());

		// validate untyped value
		object value = runtimeService.getVariable(instance.Id, "simpleBean");
		assertEquals(bean, value);

		// validate typed value
		ObjectValue typedValue = runtimeService.getVariableTyped(instance.Id, "simpleBean");
		assertEquals(ValueType.OBJECT, typedValue.Type);

		assertTrue(typedValue.Deserialized);

		assertEquals(bean, typedValue.Value);
		assertEquals(bean, typedValue.getValue(typeof(JavaSerializable)));
		assertEquals(typeof(JavaSerializable), typedValue.ObjectType);

		assertEquals(Variables.SerializationDataFormats.JAVA.Name, typedValue.SerializationDataFormat);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(JavaSerializable).FullName, typedValue.ObjectTypeName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testJavaSerializedValuesAreProhibited() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testJavaSerializedValuesAreProhibited()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		try
		{
		  // request object to be serialized as Java
		  runtimeService.setVariable(instance.Id, "simpleBean", serializedObjectValue("").serializationDataFormat(Variables.SerializationDataFormats.JAVA).create());
		  fail("Exception is expected.");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals("ENGINE-17007 Cannot set variable with name simpleBean. Java serialization format is prohibited", ex.Message);
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testJavaSerializedValuesAreProhibitedForTransient() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testJavaSerializedValuesAreProhibitedForTransient()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		try
		{
		  // request object to be serialized as Java
		  runtimeService.setVariable(instance.Id, "simpleBean", serializedObjectValue("").serializationDataFormat(Variables.SerializationDataFormats.JAVA).create());
		  fail("Exception is expected.");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals("ENGINE-17007 Cannot set variable with name simpleBean. Java serialization format is prohibited", ex.Message);
		}

	  }

	  public virtual void testStandaloneTaskVariable()
	  {
		Task task = taskService.newTask();
		task.Name = "gonzoTask";
		taskService.saveTask(task);

		string taskId = task.Id;

		try
		{
		  taskService.setVariable(taskId, "instrument", Variables.serializedObjectValue("trumpet").serializationDataFormat(Variables.SerializationDataFormats.JAVA).create());
		  fail("Exception is expected.");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals("ENGINE-17007 Cannot set variable with name instrument. Java serialization format is prohibited", ex.Message);
		}
		finally
		{
		  taskService.deleteTask(taskId, true);
		}

	  }

	  public virtual void testStandaloneTaskTransientVariable()
	  {
		Task task = taskService.newTask();
		task.Name = "gonzoTask";
		taskService.saveTask(task);

		string taskId = task.Id;

		try
		{
		  taskService.setVariable(taskId, "instrument", Variables.serializedObjectValue("trumpet").serializationDataFormat(Variables.SerializationDataFormats.JAVA).setTransient(true).create());
		  fail("Exception is expected.");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals("ENGINE-17007 Cannot set variable with name instrument. Java serialization format is prohibited", ex.Message);
		}
		finally
		{
		  taskService.deleteTask(taskId, true);
		}

	  }

	}

}