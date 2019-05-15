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
namespace org.camunda.bpm.engine.test.api.task
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TaskVariablesTest : PluggableProcessEngineTestCase
	{

	  public virtual void testStandaloneTaskVariables()
	  {
		Task task = taskService.newTask();
		task.Name = "gonzoTask";
		taskService.saveTask(task);

		string taskId = task.Id;
		taskService.setVariable(taskId, "instrument", "trumpet");
		assertEquals("trumpet", taskService.getVariable(taskId, "instrument"));

		taskService.deleteTask(taskId, true);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/task/TaskVariablesTest.testTaskExecutionVariables.bpmn20.xml"})]
	  public virtual void testTaskExecutionVariableLongValue()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		StringBuilder longString = new StringBuilder();
		for (int i = 0; i < 500; i++)
		{
		  longString.Append("tensymbols");
		}
		try
		{
		  runtimeService.setVariable(processInstanceId, "var", longString.ToString());
		}
		catch (Exception ex)
		{
		  if (!(ex is BadUserRequestException))
		  {
			fail("BadUserRequestException is expected, but another exception was received:  " + ex);
		  }
		  assertEquals("Variable value is too long", ex.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskExecutionVariables()
	  public virtual void testTaskExecutionVariables()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.createTaskQuery().singleResult().Id;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		assertEquals(expectedVariables, runtimeService.getVariables(processInstanceId));
		assertEquals(expectedVariables, taskService.getVariables(taskId));
		assertEquals(expectedVariables, runtimeService.getVariablesLocal(processInstanceId));
		assertEquals(expectedVariables, taskService.getVariablesLocal(taskId));

		runtimeService.setVariable(processInstanceId, "instrument", "trumpet");

		expectedVariables = new Dictionary<string, object>();
		assertEquals(expectedVariables, taskService.getVariablesLocal(taskId));
		expectedVariables["instrument"] = "trumpet";
		assertEquals(expectedVariables, runtimeService.getVariables(processInstanceId));
		assertEquals(expectedVariables, taskService.getVariables(taskId));
		assertEquals(expectedVariables, runtimeService.getVariablesLocal(processInstanceId));

		taskService.setVariable(taskId, "player", "gonzo");

		expectedVariables = new Dictionary<string, object>();
		assertEquals(expectedVariables, taskService.getVariablesLocal(taskId));
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		assertEquals(expectedVariables, runtimeService.getVariables(processInstanceId));
		assertEquals(expectedVariables, taskService.getVariables(taskId));
		assertEquals(expectedVariables, runtimeService.getVariablesLocal(processInstanceId));
		assertEquals(expectedVariables, runtimeService.getVariablesLocal(processInstanceId, null));
		assertEquals(expectedVariables, runtimeService.getVariablesLocalTyped(processInstanceId, null, true));

		taskService.setVariableLocal(taskId, "budget", "unlimited");

		expectedVariables = new Dictionary<string, object>();
		expectedVariables["budget"] = "unlimited";
		assertEquals(expectedVariables, taskService.getVariablesLocal(taskId));
		assertEquals(expectedVariables, taskService.getVariablesLocalTyped(taskId, true));
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		assertEquals(expectedVariables, taskService.getVariables(taskId));
		assertEquals(expectedVariables, taskService.getVariablesTyped(taskId, true));

		assertEquals(expectedVariables, taskService.getVariables(taskId, null));
		assertEquals(expectedVariables, taskService.getVariablesTyped(taskId, null, true));

		expectedVariables = new Dictionary<string, object>();
		expectedVariables["player"] = "gonzo";
		expectedVariables["instrument"] = "trumpet";
		assertEquals(expectedVariables, runtimeService.getVariables(processInstanceId));
		assertEquals(expectedVariables, runtimeService.getVariablesLocal(processInstanceId));


		// typed variable API

		List<string> serializableValue = new List<string>();
		serializableValue.Add("1");
		serializableValue.Add("2");
		taskService.setVariable(taskId, "objectVariable", objectValue(serializableValue).create());

		List<string> serializableValueLocal = new List<string>();
		serializableValueLocal.Add("3");
		serializableValueLocal.Add("4");
		taskService.setVariableLocal(taskId, "objectVariableLocal", objectValue(serializableValueLocal).create());

		object value = taskService.getVariable(taskId, "objectVariable");
		assertEquals(serializableValue, value);

		object valueLocal = taskService.getVariableLocal(taskId, "objectVariableLocal");
		assertEquals(serializableValueLocal, valueLocal);

		ObjectValue typedValue = taskService.getVariableTyped(taskId, "objectVariable");
		assertEquals(serializableValue, typedValue.Value);

		ObjectValue serializedValue = taskService.getVariableTyped(taskId, "objectVariable", false);
		assertFalse(serializedValue.Deserialized);

		ObjectValue typedValueLocal = taskService.getVariableLocalTyped(taskId, "objectVariableLocal");
		assertEquals(serializableValueLocal, typedValueLocal.Value);

		ObjectValue serializedValueLocal = taskService.getVariableLocalTyped(taskId, "objectVariableLocal", false);
		assertFalse(serializedValueLocal.Deserialized);

		try
		{
		  StringValue val = taskService.getVariableTyped(taskId, "objectVariable");
		  fail("expected exception");
		}
		catch (System.InvalidCastException)
		{
		  //happy path
		}

	  }
	}

}