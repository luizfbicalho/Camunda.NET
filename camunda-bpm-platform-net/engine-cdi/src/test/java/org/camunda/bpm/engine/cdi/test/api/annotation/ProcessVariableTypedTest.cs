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
namespace org.camunda.bpm.engine.cdi.test.api.annotation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using DeclarativeProcessController = org.camunda.bpm.engine.cdi.test.impl.beans.DeclarativeProcessController;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessVariableTypedTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/annotation/CompleteTaskTest.bpmn20.xml") public void testProcessVariableTypeAnnotation()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/annotation/CompleteTaskTest.bpmn20.xml")]
	  public virtual void testProcessVariableTypeAnnotation()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		VariableMap variables = Variables.createVariables().putValue("injectedValue", "camunda");
		businessProcess.startProcessByKey("keyOfTheProcess", variables);

		TypedValue value = getBeanInstance(typeof(DeclarativeProcessController)).InjectedValue;
		assertNotNull(value);
		assertTrue(value is StringValue);
		assertEquals(ValueType.STRING, value.Type);
		assertEquals("camunda", value.Value);
	  }

	}

}