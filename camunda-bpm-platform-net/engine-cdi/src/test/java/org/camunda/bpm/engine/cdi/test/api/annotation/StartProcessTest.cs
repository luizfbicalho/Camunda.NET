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
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using StartProcessInterceptor = org.camunda.bpm.engine.cdi.impl.annotation.StartProcessInterceptor;
	using DeclarativeProcessController = org.camunda.bpm.engine.cdi.test.impl.beans.DeclarativeProcessController;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// Testcase for assuring that the <seealso cref="StartProcessInterceptor"/> behaves as
	/// expected.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class StartProcessTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/annotation/StartProcessTest.bpmn20.xml") public void testStartProcessByKey()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/annotation/StartProcessTest.bpmn20.xml")]
	  public virtual void testStartProcessByKey()
	  {

		assertNull(runtimeService.createProcessInstanceQuery().singleResult());

		getBeanInstance(typeof(DeclarativeProcessController)).startProcessByKey();
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		assertNotNull(runtimeService.createProcessInstanceQuery().singleResult());

		assertEquals("camunda", businessProcess.getVariable("name"));

		TypedValue nameTypedValue = businessProcess.getVariableTyped("name");
		assertNotNull(nameTypedValue);
		assertTrue(nameTypedValue is StringValue);
		assertEquals(ValueType.STRING, nameTypedValue.Type);
		assertEquals("camunda", nameTypedValue.Value);

		assertEquals("untypedName", businessProcess.getVariable("untypedName"));

		TypedValue untypedNameTypedValue = businessProcess.getVariableTyped("untypedName");
		assertNotNull(untypedNameTypedValue);
		assertTrue(untypedNameTypedValue is StringValue);
		assertEquals(ValueType.STRING, untypedNameTypedValue.Type);
		assertEquals("untypedName", untypedNameTypedValue.Value);


		assertEquals("typedName", businessProcess.getVariable("typedName"));

		TypedValue typedNameTypedValue = businessProcess.getVariableTyped("typedName");
		assertNotNull(typedNameTypedValue);
		assertTrue(typedNameTypedValue is StringValue);
		assertEquals(ValueType.STRING, typedNameTypedValue.Type);
		assertEquals("typedName", typedNameTypedValue.Value);

		businessProcess.startTask(taskService.createTaskQuery().singleResult().Id);
		businessProcess.completeTask();
	  }


	}

}