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
namespace org.camunda.bpm.engine.test.api.runtime.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class AssertVariableInstancesDelegate : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {

		// validate integer variable
		int? expectedIntValue = 1234;
		assertEquals(expectedIntValue, execution.getVariable("anIntegerVariable"));
		assertEquals(expectedIntValue, execution.getVariableTyped("anIntegerVariable").Value);
		assertEquals(ValueType.INTEGER, execution.getVariableTyped("anIntegerVariable").Type);
		assertNull(execution.getVariableLocal("anIntegerVariable"));
		assertNull(execution.getVariableLocalTyped("anIntegerVariable"));

		// set an additional local variable
		execution.setVariableLocal("aStringVariable", "aStringValue");

		string expectedStringValue = "aStringValue";
		assertEquals(expectedStringValue, execution.getVariable("aStringVariable"));
		assertEquals(expectedStringValue, execution.getVariableTyped("aStringVariable").Value);
		assertEquals(ValueType.STRING, execution.getVariableTyped("aStringVariable").Type);
		assertEquals(expectedStringValue, execution.getVariableLocal("aStringVariable"));
		assertEquals(expectedStringValue, execution.getVariableLocalTyped("aStringVariable").Value);
		assertEquals(ValueType.STRING, execution.getVariableLocalTyped("aStringVariable").Type);

		SimpleSerializableBean objectValue = (SimpleSerializableBean) execution.getVariable("anObjectValue");
		assertNotNull(objectValue);
		assertEquals(10, objectValue.IntProperty);
		ObjectValue variableTyped = execution.getVariableTyped("anObjectValue");
		assertEquals(10, variableTyped.getValue(typeof(SimpleSerializableBean)).IntProperty);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);

		objectValue = (SimpleSerializableBean) execution.getVariable("anUntypedObjectValue");
		assertNotNull(objectValue);
		assertEquals(30, objectValue.IntProperty);
		variableTyped = execution.getVariableTyped("anUntypedObjectValue");
		assertEquals(30, variableTyped.getValue(typeof(SimpleSerializableBean)).IntProperty);
		assertEquals(Context.ProcessEngineConfiguration.DefaultSerializationFormat, variableTyped.SerializationDataFormat);

	  }


	}

}