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

	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using JSONException = org.json.JSONException;
	using JSONAssert = org.skyscreamer.jsonassert.JSONAssert;

	public class HistoricVariableJsonSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";

	  protected internal static readonly string JSON_FORMAT_NAME = DataFormats.json().Name;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSelectHistoricVariableInstances() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSelectHistoricVariableInstances()
	  {
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{
		  ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		  JsonSerializable bean = new JsonSerializable("a String", 42, false);
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(JSON_FORMAT_NAME).create());

		  HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		  assertNotNull(historicVariable.Value);
		  assertNull(historicVariable.ErrorMessage);

		  assertEquals(ValueType.OBJECT.Name, historicVariable.TypeName);
		  assertEquals(ValueType.OBJECT.Name, historicVariable.VariableTypeName);

		  JsonSerializable historyValue = (JsonSerializable) historicVariable.Value;
		  assertEquals(bean.StringProperty, historyValue.StringProperty);
		  assertEquals(bean.IntProperty, historyValue.IntProperty);
		  assertEquals(bean.BooleanProperty, historyValue.BooleanProperty);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSelectHistoricSerializedValues() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSelectHistoricSerializedValues()
	  {
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{


		  ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		  JsonSerializable bean = new JsonSerializable("a String", 42, false);
		  runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(JSON_FORMAT_NAME));

		  HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().singleResult();
		  assertNotNull(historicVariable.Value);
		  assertNull(historicVariable.ErrorMessage);

		  ObjectValue typedValue = (ObjectValue) historicVariable.TypedValue;
		  assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
		  JSONAssert.assertEquals(bean.toExpectedJsonString(),new string(typedValue.ValueSerialized), true);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertEquals(typeof(JsonSerializable).FullName, typedValue.ObjectTypeName);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testSelectHistoricSerializedValuesUpdate() throws org.json.JSONException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSelectHistoricSerializedValuesUpdate()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		JsonSerializable bean = new JsonSerializable("a String", 42, false);
		runtimeService.setVariable(instance.Id, "simpleBean", objectValue(bean).serializationDataFormat(JSON_FORMAT_NAME));

		if (ProcessEngineConfiguration.HISTORY_FULL.Equals(processEngineConfiguration.History))
		{

		  HistoricVariableUpdate historicUpdate = (HistoricVariableUpdate) historyService.createHistoricDetailQuery().variableUpdates().singleResult();

		  assertNotNull(historicUpdate.Value);
		  assertNull(historicUpdate.ErrorMessage);

		  assertEquals(ValueType.OBJECT.Name, historicUpdate.TypeName);
		  assertEquals(ValueType.OBJECT.Name, historicUpdate.VariableTypeName);

		  JsonSerializable historyValue = (JsonSerializable) historicUpdate.Value;
		  assertEquals(bean.StringProperty, historyValue.StringProperty);
		  assertEquals(bean.IntProperty, historyValue.IntProperty);
		  assertEquals(bean.BooleanProperty, historyValue.BooleanProperty);

		  ObjectValue typedValue = (ObjectValue) historicUpdate.TypedValue;
		  assertEquals(JSON_FORMAT_NAME, typedValue.SerializationDataFormat);
		  JSONAssert.assertEquals(bean.toExpectedJsonString(),new string(typedValue.ValueSerialized), true);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertEquals(typeof(JsonSerializable).FullName, typedValue.ObjectTypeName);

		}
	  }

	}

}