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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using CustomSerializable = org.camunda.bpm.engine.test.api.runtime.util.CustomSerializable;
	using FailingSerializable = org.camunda.bpm.engine.test.api.runtime.util.FailingSerializable;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class VariableInstanceQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQuery()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQuery()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intVar"] = 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		assertNotNull(query);

		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertNotNull(var.Id);
		  if (var.Name.Equals("intVar"))
		  {
			assertEquals("intVar", var.Name);
			assertEquals(123, var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByVariableId()
	  public virtual void testQueryByVariableId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var1"] = "test";
		variables["var2"] = "test";
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariablesLocal(task.Id, variables);
		VariableInstance result = runtimeService.createVariableInstanceQuery().variableName("var1").singleResult();
		assertNotNull(result);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableId(result.Id);

		// then
		assertNotNull(query);
		VariableInstance resultById = query.singleResult();
		assertEquals(result.Id, resultById.Id);

		// delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableName()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableName()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("stringVar");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("stringVar", var.Name);
		assertEquals("test", var.Value);
		assertEquals("string", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableNames()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableNames()
	  {
		// given
		string variableValue = "a";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["process"] = variableValue;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, "task", variableValue);
		runtimeService.setVariableLocal(task.ExecutionId, "execution", variableValue);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableNameIn("task", "process", "execution");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance variableInstance in result)
		{
		  assertEquals(variableValue, variableInstance.Value);
		  assertEquals("string", variableInstance.TypeName);
		}

		assertEquals(1, runtimeService.createVariableInstanceQuery().variableName("task").variableNameIn("task", "execution").count());
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("task").variableNameIn("process", "execution").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableNameLike()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableNameLike()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["string%Var"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableNameLike("%ing\\%V%");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("string%Var", var.Name);
		assertEquals("test", var.Value);
		assertEquals("string", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableNameLikeWithoutAnyResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableNameLikeWithoutAnyResult()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableNameLike("%ingV_");

		// then
		IList<VariableInstance> result = query.list();
		assertTrue(result.Count == 0);

		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_String()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("stringVar", "test");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("stringVar", var.Name);
		assertEquals("test", var.Value);
		assertEquals("string", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueNotEquals_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueNotEquals_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("stringVar", "test123");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("stringVar", var.Name);
		assertEquals("test", var.Value);
		assertEquals("string", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueGreaterThan_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueGreaterThan_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "a";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "b";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["stringVar"] = "c";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThan("stringVar", "a");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);
		  if (var.Value.Equals("b"))
		  {
			assertEquals("b", var.Value);
		  }
		  else if (var.Value.Equals("c"))
		  {
			assertEquals("c", var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueGreaterThanOrEqual_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueGreaterThanOrEqual_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "a";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "b";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["stringVar"] = "c";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThanOrEqual("stringVar", "a");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);
		  if (var.Value.Equals("a"))
		  {
			assertEquals("a", var.Value);
		  }
		  else if (var.Value.Equals("b"))
		  {
			assertEquals("b", var.Value);
		  }
		  else if (var.Value.Equals("c"))
		  {
			assertEquals("c", var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueLessThan_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueLessThan_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "a";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "b";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["stringVar"] = "c";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThan("stringVar", "c");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);
		  if (var.Value.Equals("a"))
		  {
			assertEquals("a", var.Value);
		  }
		  else if (var.Value.Equals("b"))
		  {
			assertEquals("b", var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueLessThanOrEqual_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueLessThanOrEqual_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "a";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "b";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["stringVar"] = "c";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThanOrEqual("stringVar", "c");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);
		  if (var.Value.Equals("a"))
		  {
			assertEquals("a", var.Value);
		  }
		  else if (var.Value.Equals("b"))
		  {
			assertEquals("b", var.Value);
		  }
		  else if (var.Value.Equals("c"))
		  {
			assertEquals("c", var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueLike_String()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueLike_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test456";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["stringVar"] = "test789";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLike("stringVar", "test%");

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);
		  if (var.Value.Equals("test123"))
		  {
			assertEquals("test123", var.Value);
		  }
		  else if (var.Value.Equals("test456"))
		  {
			assertEquals("test456", var.Value);
		  }
		  else if (var.Value.Equals("test789"))
		  {
			assertEquals("test789", var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" }) public void testQueryByNameAndVariableValueLikeWithEscape_String()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByNameAndVariableValueLikeWithEscape_String()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test_123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test%456";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLike("stringVar", "test\\_%");
		verifyQueryResult(query, "test_123");

		query = runtimeService.createVariableInstanceQuery().variableValueLike("stringVar", "test\\%%");
		verifyQueryResult(query, "test%456");

	  }

	  private void verifyQueryResult(VariableInstanceQuery query, string varValue)
	  {
		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("stringVar", var.Name);
		  assertEquals("string", var.TypeName);

		  assertEquals(varValue, var.Value);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Integer()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("intValue", 1234);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("intValue", var.Name);
		assertEquals(1234, var.Value);
		assertEquals("integer", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueNotEquals_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueNotEquals_Integer()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["intValue"] = 5555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("intValue", 5555);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("intValue", var.Name);
		assertEquals(1234, var.Value);
		assertEquals("integer", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThan_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThan_Integer()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["intValue"] = 5555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["intValue"] = 9876;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThan("intValue", 1234);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("intValue", var.Name);
		  assertEquals("integer", var.TypeName);
		  if (var.Value.Equals(5555))
		  {
			assertEquals(5555, var.Value);
		  }
		  else if (var.Value.Equals(9876))
		  {
			assertEquals(9876, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThanAndEqual_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Integer()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["intValue"] = 5555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["intValue"] = 9876;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThanOrEqual("intValue", 1234);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("intValue", var.Name);
		  assertEquals("integer", var.TypeName);
		  if (var.Value.Equals(1234))
		  {
			assertEquals(1234, var.Value);
		  }
		  else if (var.Value.Equals(5555))
		  {
			assertEquals(5555, var.Value);
		  }
		  else if (var.Value.Equals(9876))
		  {
			assertEquals(9876, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThan_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThan_Integer()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["intValue"] = 5555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["intValue"] = 9876;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThan("intValue", 9876);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("intValue", var.Name);
		  assertEquals("integer", var.TypeName);
		  if (var.Value.Equals(5555))
		  {
			assertEquals(5555, var.Value);
		  }
		  else if (var.Value.Equals(1234))
		  {
			assertEquals(1234, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThanAndEqual_Integer()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThanAndEqual_Integer()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intValue"] = 1234;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["intValue"] = 5555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["intValue"] = 9876;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThanOrEqual("intValue", 9876);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("intValue", var.Name);
		  assertEquals("integer", var.TypeName);
		  if (var.Value.Equals(1234))
		  {
			assertEquals(1234, var.Value);
		  }
		  else if (var.Value.Equals(5555))
		  {
			assertEquals(5555, var.Value);
		  }
		  else if (var.Value.Equals(9876))
		  {
			assertEquals(9876, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Long()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("longValue", 123456L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("longValue", var.Name);
		assertEquals(123456L, var.Value);
		assertEquals("long", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueNotEquals_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueNotEquals_Long()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["longValue"] = 987654L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("longValue", 987654L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("longValue", var.Name);
		assertEquals(123456L, var.Value);
		assertEquals("long", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThan_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThan_Long()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["longValue"] = 987654L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["longValue"] = 555555L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThan("longValue", 123456L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("longValue", var.Name);
		  assertEquals("long", var.TypeName);
		  if (var.Value.Equals(555555L))
		  {
			assertEquals(555555L, var.Value);
		  }
		  else if (var.Value.Equals(987654L))
		  {
			assertEquals(987654L, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThanAndEqual_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Long()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["longValue"] = 987654L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["longValue"] = 555555L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThanOrEqual("longValue", 123456L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("longValue", var.Name);
		  assertEquals("long", var.TypeName);
		  if (var.Value.Equals(123456L))
		  {
			assertEquals(123456L, var.Value);
		  }
		  else if (var.Value.Equals(555555L))
		  {
			assertEquals(555555L, var.Value);
		  }
		  else if (var.Value.Equals(987654L))
		  {
			assertEquals(987654L, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThan_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThan_Long()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["longValue"] = 987654L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["longValue"] = 555555L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThan("longValue", 987654L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("longValue", var.Name);
		  assertEquals("long", var.TypeName);
		  if (var.Value.Equals(123456L))
		  {
			assertEquals(123456L, var.Value);
		  }
		  else if (var.Value.Equals(555555L))
		  {
			assertEquals(555555L, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThanAndEqual_Long()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThanAndEqual_Long()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["longValue"] = 123456L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["longValue"] = 987654L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["longValue"] = 555555L;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThanOrEqual("longValue", 987654L);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("longValue", var.Name);
		  assertEquals("long", var.TypeName);
		  if (var.Value.Equals(123456L))
		  {
			assertEquals(123456L, var.Value);
		  }
		  else if (var.Value.Equals(555555L))
		  {
			assertEquals(555555L, var.Value);
		  }
		  else if (var.Value.Equals(987654L))
		  {
			assertEquals(987654L, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Double()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("doubleValue", 123.456);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("doubleValue", var.Name);
		assertEquals(123.456, var.Value);
		assertEquals("double", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueNotEquals_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueNotEquals_Double()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["doubleValue"] = 654.321;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("doubleValue", 654.321);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("doubleValue", var.Name);
		assertEquals(123.456, var.Value);
		assertEquals("double", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThan_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThan_Double()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["doubleValue"] = 654.321;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["doubleValue"] = 999.999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThan("doubleValue", 123.456);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("doubleValue", var.Name);
		  assertEquals("double", var.TypeName);
		  if (var.Value.Equals(654.321))
		  {
			assertEquals(654.321, var.Value);
		  }
		  else if (var.Value.Equals(999.999))
		  {
			assertEquals(999.999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThanAndEqual_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Double()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["doubleValue"] = 654.321;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["doubleValue"] = 999.999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThanOrEqual("doubleValue", 123.456);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("doubleValue", var.Name);
		  assertEquals("double", var.TypeName);
		  if (var.Value.Equals(123.456))
		  {
			assertEquals(123.456, var.Value);
		  }
		  else if (var.Value.Equals(654.321))
		  {
			assertEquals(654.321, var.Value);
		  }
		  else if (var.Value.Equals(999.999))
		  {
			assertEquals(999.999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThan_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThan_Double()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["doubleValue"] = 654.321;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["doubleValue"] = 999.999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThan("doubleValue", 999.999);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("doubleValue", var.Name);
		  assertEquals("double", var.TypeName);
		  if (var.Value.Equals(123.456))
		  {
			assertEquals(123.456, var.Value);
		  }
		  else if (var.Value.Equals(654.321))
		  {
			assertEquals(654.321, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThanAndEqual_Double()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThanAndEqual_Double()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["doubleValue"] = 123.456;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["doubleValue"] = 654.321;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["doubleValue"] = 999.999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThanOrEqual("doubleValue", 999.999);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("doubleValue", var.Name);
		  assertEquals("double", var.TypeName);
		  if (var.Value.Equals(123.456))
		  {
			assertEquals(123.456, var.Value);
		  }
		  else if (var.Value.Equals(654.321))
		  {
			assertEquals(654.321, var.Value);
		  }
		  else if (var.Value.Equals(999.999))
		  {
			assertEquals(999.999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Short()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("shortValue", (short) 123);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("shortValue", var.Name);
		assertEquals((short) 123, var.Value);
		assertEquals("short", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableValueNotEquals_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableValueNotEquals_Short()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["shortValue"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("shortValue", (short) 999);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("shortValue", var.Name);
		assertEquals((short) 123, var.Value);
		assertEquals("short", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThan_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThan_Short()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["shortValue"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["shortValue"] = (short) 555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThan("shortValue", (short) 123);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("shortValue", var.Name);
		  assertEquals("short", var.TypeName);
		  if (var.Value.Equals((short) 555))
		  {
			assertEquals((short) 555, var.Value);
		  }
		  else if (var.Value.Equals((short) 999))
		  {
			assertEquals((short) 999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableGreaterThanAndEqual_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Short()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["shortValue"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["shortValue"] = (short) 555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueGreaterThanOrEqual("shortValue", (short) 123);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("shortValue", var.Name);
		  assertEquals("short", var.TypeName);
		  if (var.Value.Equals((short) 123))
		  {
			assertEquals((short) 123, var.Value);
		  }
		  else if (var.Value.Equals((short) 555))
		  {
			assertEquals((short) 555, var.Value);
		  }
		  else if (var.Value.Equals((short) 999))
		  {
			assertEquals((short) 999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThan_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThan_Short()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["shortValue"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["shortValue"] = (short) 555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThan("shortValue", (short) 999);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("shortValue", var.Name);
		  assertEquals("short", var.TypeName);
		  if (var.Value.Equals((short) 123))
		  {
			assertEquals((short) 123, var.Value);
		  }
		  else if (var.Value.Equals((short) 555))
		  {
			assertEquals((short) 555, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableLessThanAndEqual_Short()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableLessThanAndEqual_Short()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["shortValue"] = (short) 123;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["shortValue"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["shortValue"] = (short) 555;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueLessThanOrEqual("shortValue", (short) 999);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("shortValue", var.Name);
		  assertEquals("short", var.TypeName);
		  if (var.Value.Equals((short) 123))
		  {
			assertEquals((short) 123, var.Value);
		  }
		  else if (var.Value.Equals((short) 555))
		  {
			assertEquals((short) 555, var.Value);
		  }
		  else if (var.Value.Equals((short) 999))
		  {
			assertEquals((short) 999, var.Value);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Bytes()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Bytes()
	  {
		// given
		sbyte[] bytes = "somebytes".GetBytes();
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["bytesVar"] = bytes;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("bytesVar", bytes);

		// then
		try
		{
		  query.list();
		  fail("A ProcessEngineException was expected: Variables of type ByteArray cannot be used to query.");
		}
		catch (ProcessEngineException)
		{
		  // expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_Date()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_Date()
	  {
		// given
		 DateTime now = DateTime.Now;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["date"] = now;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("date", now);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("date", var.Name);
		assertEquals(now, var.Value);
		assertEquals("date", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEqualsWihtoutAnyResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEqualsWihtoutAnyResult()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("stringVar", "notFoundValue");

		// then
		IList<VariableInstance> result = query.list();
		assertTrue(result.Count == 0);

		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByNameAndVariableValueEquals_NullValue()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByNameAndVariableValueEquals_NullValue()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["nullValue"] = null;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueEquals("nullValue", null);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("nullValue", var.Name);
		assertEquals(null, var.Value);
		assertEquals("null", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByVariableValueNotEquals_NullValue()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableValueNotEquals_NullValue()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["value"] = null;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["value"] = (short) 999;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["value"] = "abc";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableValueNotEquals("value", null);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("value", var.Name);
		  if (var.Value.Equals((short) 999))
		  {
			assertEquals((short) 999, var.Value);
			assertEquals("short", var.TypeName);
		  }
		  else if (var.Value.Equals("abc"))
		  {
			assertEquals("abc", var.Value);
			assertEquals("string", var.TypeName);
		  }
		  else
		  {
			fail("A non expected value occured: " + var.Value);
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByProcessInstanceId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		variables["myVar"] = "test123";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("myVar"))
		  {
			assertEquals("myVar", var.Name);
			assertEquals("test123", var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByProcessInstanceIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceIds()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		variables["myVar"] = "test123";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance1.Id, processInstance2.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("myVar"))
		  {
			assertEquals("myVar", var.Name);
			assertEquals("test123", var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByProcessInstanceIdWithoutAnyResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByProcessInstanceIdWithoutAnyResult()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn("aProcessInstanceId");

		// then
		IList<VariableInstance> result = query.list();
		assertTrue(result.Count == 0);

		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByExecutionId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByExecutionId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		variables["myVar"] = "test123";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().executionIdIn(processInstance.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("myVar"))
		  {
			assertEquals("myVar", var.Name);
			assertEquals("test123", var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByExecutionIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByExecutionIds()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["myVar"] = "test123";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["myVar"] = "test123";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().executionIdIn(processInstance1.Id, processInstance2.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		assertEquals(3, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("myVar"))
		  {
			assertEquals("myVar", var.Name);
			assertEquals("test123", var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByExecutionIdWithoutAnyResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByExecutionIdWithoutAnyResult()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().executionIdIn("anExecutionId");

		// then
		IList<VariableInstance> result = query.list();
		assertTrue(result.Count == 0);

		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByTaskId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByTaskId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		taskService.setVariableLocal(task.Id, "taskVariable", "aCustomValue");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().taskIdIn(task.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		assertEquals(1, query.count());

		VariableInstance var = result[0];
		assertEquals("taskVariable", var.Name);
		assertEquals("aCustomValue", var.Value);
		assertEquals("string", var.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByTaskIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByTaskIds()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task1 = taskService.createTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		Task task2 = taskService.createTaskQuery().processInstanceId(processInstance2.Id).singleResult();
		Task task3 = taskService.createTaskQuery().processInstanceId(processInstance3.Id).singleResult();

		taskService.setVariableLocal(task1.Id, "taskVariable", "aCustomValue");
		taskService.setVariableLocal(task2.Id, "anotherTaskVariable", "aCustomValue");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().taskIdIn(task1.Id, task2.Id, task3.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, query.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("taskVariable"))
		  {
			assertEquals("taskVariable", var.Name);
			assertEquals("aCustomValue", var.Value);
		  }
		  else if (var.Name.Equals("anotherTaskVariable"))
		  {
			assertEquals("anotherTaskVariable", var.Name);
			assertEquals("aCustomValue", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByTaskIdWithoutAnyResult()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByTaskIdWithoutAnyResult()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		taskService.setVariableLocal(task.Id, "taskVariable", "aCustomValue");

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().taskIdIn("aTaskId");

		// then
		IList<VariableInstance> result = query.list();
		assertTrue(result.Count == 0);

		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/VariableInstanceQueryTest.taskInEmbeddedSubProcess.bpmn20.xml"}) public void testQueryByVariableScopeId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/VariableInstanceQueryTest.taskInEmbeddedSubProcess.bpmn20.xml"})]
	  public virtual void testQueryByVariableScopeId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		// get variable scope ids
		string taskId = task.Id;
		string executionId = task.ExecutionId;
		string processInstanceId = task.ProcessInstanceId;

		// set variables
		string variableName = "foo";
		IDictionary<string, string> variables = new Dictionary<string, string>();
		variables[taskId] = "task";
		variables[executionId] = "execution";
		variables[processInstanceId] = "processInstance";

		taskService.setVariableLocal(taskId, variableName, variables[taskId]);
		runtimeService.setVariableLocal(executionId, variableName, variables[executionId]);
		runtimeService.setVariableLocal(processInstanceId, variableName, variables[processInstanceId]);

		IList<VariableInstance> variableInstances;

		// query by variable scope id
		foreach (string variableScopeId in variables.Keys)
		{
		  variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(variableScopeId).list();
		  assertEquals(1, variableInstances.Count);
		  assertEquals(variableName, variableInstances[0].Name);
		  assertEquals(variables[variableScopeId], variableInstances[0].Value);
		}

		// query by multiple variable scope ids
		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(taskId, executionId, processInstanceId).list();
		assertEquals(3, variableInstances.Count);

		// remove task variable
		taskService.removeVariableLocal(taskId, variableName);

		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(taskId).list();
		assertEquals(0, variableInstances.Count);

		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(taskId, executionId, processInstanceId).list();
		assertEquals(2, variableInstances.Count);

		// remove process instance variable variable
		runtimeService.removeVariable(processInstanceId, variableName);

		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(processInstanceId, taskId).list();
		assertEquals(0, variableInstances.Count);

		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(taskId, executionId, processInstanceId).list();
		assertEquals(1, variableInstances.Count);

		// remove execution variable
		runtimeService.removeVariable(executionId, variableName);

		variableInstances = runtimeService.createVariableInstanceQuery().variableScopeIdIn(taskId, executionId, processInstanceId).list();
		assertEquals(0, variableInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByActivityInstanceId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByActivityInstanceId()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		string activityId = runtimeService.getActivityInstance(processInstance.Id).ChildActivityInstances[0].Id;

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, "taskVariable", "aCustomValue");

		// when
		VariableInstanceQuery taskVariablesQuery = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityId);
		VariableInstanceQuery processVariablesQuery = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(processInstance.Id);

		// then
		VariableInstance taskVar = taskVariablesQuery.singleResult();
		assertNotNull(taskVar);

		assertEquals(1, taskVariablesQuery.count());
		assertEquals("string", taskVar.TypeName);
		assertEquals("taskVariable", taskVar.Name);
		assertEquals("aCustomValue", taskVar.Value);

		VariableInstance processVar = processVariablesQuery.singleResult();

		assertEquals(1, processVariablesQuery.count());
		assertEquals("string", processVar.TypeName);
		assertEquals("stringVar", processVar.Name);
		assertEquals("test", processVar.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryByActivityInstanceIds()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryByActivityInstanceIds()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["stringVar"] = "test";
		variables1["myVar"] = "test123";
		ProcessInstance procInst1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["myVar"] = "test123";
		ProcessInstance procInst2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		IDictionary<string, object> variables3 = new Dictionary<string, object>();
		variables3["myVar"] = "test123";
		ProcessInstance procInst3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables3);

		Task task1 = taskService.createTaskQuery().processInstanceId(procInst1.Id).singleResult();
		Task task2 = taskService.createTaskQuery().processInstanceId(procInst2.Id).singleResult();

		taskService.setVariableLocal(task1.Id, "taskVariable", "aCustomValue");
		taskService.setVariableLocal(task2.Id, "anotherTaskVariable", "aCustomValue");

		// when
		VariableInstanceQuery processVariablesQuery = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(procInst1.Id, procInst2.Id, procInst3.Id);

		VariableInstanceQuery taskVariablesQuery = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(runtimeService.getActivityInstance(procInst1.Id).ChildActivityInstances[0].Id, runtimeService.getActivityInstance(procInst2.Id).ChildActivityInstances[0].Id);

		// then (process variables)
		IList<VariableInstance> result = processVariablesQuery.list();
		assertFalse(result.Count == 0);
		assertEquals(4, result.Count);

		assertEquals(4, processVariablesQuery.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("myVar"))
		  {
			assertEquals("myVar", var.Name);
			assertEquals("test123", var.Value);
		  }
		  else if (var.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", var.Name);
			assertEquals("test", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}

		// then (task variables)
		result = taskVariablesQuery.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		assertEquals(2, taskVariablesQuery.count());

		foreach (VariableInstance var in result)
		{
		  assertEquals("string", var.TypeName);
		  if (var.Name.Equals("taskVariable"))
		  {
			assertEquals("taskVariable", var.Name);
			assertEquals("aCustomValue", var.Value);
		  }
		  else if (var.Name.Equals("anotherTaskVariable"))
		  {
			assertEquals("anotherTaskVariable", var.Name);
			assertEquals("aCustomValue", var.Value);
		  }
		  else
		  {
			fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByName_Asc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByName_Asc()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		variables["myVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByVariableName().asc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		assertEquals("myVar", first.Name);
		assertEquals("stringVar", second.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByName_Desc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByName_Desc()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "test";
		variables["myVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByVariableName().desc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		assertEquals("stringVar", first.Name);
		assertEquals("string", first.TypeName);
		assertEquals("myVar", second.Name);
		assertEquals("string", second.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByType_Asc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByType_Asc()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["intVar"] = 123;
		variables["myVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByVariableType().asc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		assertEquals("intVar", first.Name); // integer
		assertEquals("integer", first.TypeName);
		assertEquals("myVar", second.Name); // string
		assertEquals("string", second.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByType_Desc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByType_Desc()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["intVar"] = 123;
		variables["myVar"] = "test123";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByVariableType().desc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		assertEquals("myVar", first.Name); // string
		assertEquals("string", first.TypeName);
		assertEquals("intVar", second.Name); // integer
		assertEquals("integer", second.TypeName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByActivityInstanceId_Asc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByActivityInstanceId_Asc()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intVar"] = 123;
		ProcessInstance procInst1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);
		string activityId1 = runtimeService.getActivityInstance(procInst1.Id).ChildActivityInstances[0].Id;

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test";
		ProcessInstance procInst2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);
		string activityId2 = runtimeService.getActivityInstance(procInst2.Id).ChildActivityInstances[0].Id;

		int comparisonResult = activityId1.CompareTo(activityId2);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByActivityInstanceId().asc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		if (comparisonResult < 0)
		{
		  assertEquals("intVar", first.Name);
		  assertEquals("integer", first.TypeName);
		  assertEquals("stringVar", second.Name);
		  assertEquals("string", second.TypeName);
		}
		else if (comparisonResult > 0)
		{
		  assertEquals("stringVar", first.Name);
		  assertEquals("string", first.TypeName);
		  assertEquals("intVar", second.Name);
		  assertEquals("integer", second.TypeName);
		}
		else
		{
		  fail("Something went wrong: both activity instances have the same id " + activityId1 + " and " + activityId2);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testQueryOrderByActivityInstanceId_Desc()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryOrderByActivityInstanceId_Desc()
	  {
		// given
		IDictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["intVar"] = 123;
		ProcessInstance procInst1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables1);

		IDictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["stringVar"] = "test";
		ProcessInstance procInst2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables2);

		string activityId1 = runtimeService.getActivityInstance(procInst1.Id).ChildActivityInstances[0].Id;
		string activityId2 = runtimeService.getActivityInstance(procInst2.Id).ChildActivityInstances[0].Id;

		int comparisonResult = activityId1.CompareTo(activityId2);
		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().orderByActivityInstanceId().desc();

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		VariableInstance first = result[0];
		VariableInstance second = result[1];

		if (comparisonResult < 0)
		{
		  assertEquals("stringVar", first.Name);
		  assertEquals("string", first.TypeName);
		  assertEquals("intVar", second.Name);
		  assertEquals("integer", second.TypeName);
		}
		else if (comparisonResult > 0)
		{
		  assertEquals("intVar", first.Name);
		  assertEquals("integer", first.TypeName);
		  assertEquals("stringVar", second.Name);
		  assertEquals("string", second.TypeName);
		}
		else
		{
		  fail("Something went wrong: both activity instances have the same id " + activityId1 + " and " + activityId2);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"}) public void testGetValueOfSerializableVar()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetValueOfSerializableVar()
	  {
		// given
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["serializableVar"] = serializable;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id);

		// then
		IList<VariableInstance> result = query.list();
		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		VariableInstance instance = result[0];

		assertEquals("serializableVar", instance.Name);
		assertNotNull(instance.Value);
		assertEquals(serializable, instance.Value);
		assertEquals(ValueType.OBJECT.Name, instance.TypeName);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubProcessVariablesWithParallelGateway()
	  public virtual void testSubProcessVariablesWithParallelGateway()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("processWithSubProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(tree);
		ActivityInstance[] subprocessInstances = tree.getActivityInstances("SubProcess_1");
		assertEquals(5, subprocessInstances.Length);

		//when
		string activityInstanceId1 = subprocessInstances[0].Id;
		VariableInstanceQuery query1 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId1);

		string activityInstanceId2 = subprocessInstances[1].Id;
		VariableInstanceQuery query2 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId2);

		string activityInstanceId3 = subprocessInstances[2].Id;
		VariableInstanceQuery query3 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId3);

		string activityInstanceId4 = subprocessInstances[3].Id;
		VariableInstanceQuery query4 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId4);

		string activityInstanceId5 = subprocessInstances[4].Id;
		VariableInstanceQuery query5 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(activityInstanceId5);

		// then
		checkVariables(query1.list());
		checkVariables(query2.list());
		checkVariables(query3.list());
		checkVariables(query4.list());
		checkVariables(query5.list());
	  }

	  private void checkVariables(IList<VariableInstance> variableInstances)
	  {
		assertFalse(variableInstances.Count == 0);
		foreach (VariableInstance instance in variableInstances)
		{
		  if (instance.Name.Equals("nrOfInstances"))
		  {
			assertEquals("nrOfInstances", instance.Name);
			assertEquals("integer", instance.TypeName);
		  }
		  else if (instance.Name.Equals("nrOfCompletedInstances"))
		  {
			assertEquals("nrOfCompletedInstances", instance.Name);
			assertEquals("integer", instance.TypeName);
		  }
		  else if (instance.Name.Equals("nrOfActiveInstances"))
		  {
			assertEquals("nrOfActiveInstances", instance.Name);
			assertEquals("integer", instance.TypeName);
		  }
		  else if (instance.Name.Equals("loopCounter"))
		  {
			assertEquals("loopCounter", instance.Name);
			assertEquals("integer", instance.TypeName);
		  }
		  else if (instance.Name.Equals("nullVar"))
		  {
			assertEquals("nullVar", instance.Name);
			assertEquals("null", instance.TypeName);
		  }
		  else if (instance.Name.Equals("integerVar"))
		  {
			assertEquals("integerVar", instance.Name);
			assertEquals("integer", instance.TypeName);
		  }
		  else if (instance.Name.Equals("dateVar"))
		  {
			assertEquals("dateVar", instance.Name);
			assertEquals("date", instance.TypeName);
		  }
		  else if (instance.Name.Equals("stringVar"))
		  {
			assertEquals("stringVar", instance.Name);
			assertEquals("string", instance.TypeName);
		  }
		  else if (instance.Name.Equals("shortVar"))
		  {
			assertEquals("shortVar", instance.Name);
			assertEquals("short", instance.TypeName);
		  }
		  else if (instance.Name.Equals("longVar"))
		  {
			assertEquals("longVar", instance.Name);
			assertEquals("long", instance.TypeName);
		  }
		  else if (instance.Name.Equals("byteVar"))
		  {
			assertEquals("bytes", instance.TypeName);
		  }
		  else if (instance.Name.Equals("serializableVar"))
		  {
			assertEquals("serializableVar", instance.Name);
			try
			{
			  instance.Value;
			}
			catch (System.NullReferenceException)
			{
			  // the serialized value has not been initially loaded
			}
		  }
		  else
		  {
			fail("An unexpected variable '" + instance.Name + "' was found with value " + instance.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubProcessVariables()
	  public virtual void testSubProcessVariables()
	  {
		// given
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["processVariable"] = "aProcessVariable";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("processWithSubProcess", processVariables);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(tree);
		assertEquals(1, tree.ChildActivityInstances.Length);

		// when
		VariableInstanceQuery query1 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(tree.Id);

		// then
		VariableInstance processVariable = query1.singleResult();
		assertNotNull(processVariable);
		assertEquals("processVariable", processVariable.Name);
		assertEquals("aProcessVariable", processVariable.Value);

		// when
		ActivityInstance subProcessActivityInstance = tree.getActivityInstances("SubProcess_1")[0];
		VariableInstanceQuery query2 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(subProcessActivityInstance.Id);

		// then
		checkVariables(query2.list());

		// when setting a task local variable
		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariableLocal(task.Id, "taskVariable", "taskVariableValue");

		// skip mi body instance
		ActivityInstance taskActivityInstance = subProcessActivityInstance.ChildActivityInstances[0];
		VariableInstanceQuery query3 = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(taskActivityInstance.Id);

		// then
		IList<VariableInstance> variables = query3.list();
		VariableInstance taskVariable = query3.singleResult();
		assertNotNull(taskVariable);
		assertEquals("taskVariable", taskVariable.Name);
		assertEquals("taskVariableValue", taskVariable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParallelGatewayVariables()
	  public virtual void testParallelGatewayVariables()
	  {
		// given
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["processVariable"] = "aProcessVariable";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGatewayProcess", processVariables);

		Execution execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		runtimeService.setVariableLocal(execution.Id, "aLocalVariable", "aLocalValue");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(2, tree.ChildActivityInstances.Length);
		ActivityInstance task1Instance = tree.getActivityInstances("task1")[0];

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("aLocalVariable").activityInstanceIdIn(task1Instance.Id);
		VariableInstance localVariable = query.singleResult();
		assertNotNull(localVariable);
		assertEquals("aLocalVariable", localVariable.Name);
		assertEquals("aLocalValue", localVariable.Value);

		Task task = taskService.createTaskQuery().executionId(execution.Id).singleResult();
		taskService.complete(task.Id);

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(2, tree.ChildActivityInstances.Length);
		ActivityInstance task3Instance = tree.getActivityInstances("task3")[0];

		query = runtimeService.createVariableInstanceQuery().variableName("aLocalVariable").activityInstanceIdIn(task3Instance.Id);
		localVariable = query.singleResult();
		assertNotNull(localVariable);
		assertEquals("aLocalVariable", localVariable.Name);
		assertEquals("aLocalValue", localVariable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleSubProcessVariables()
	  public virtual void testSimpleSubProcessVariables()
	  {
		// given
		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		processVariables["processVariable"] = "aProcessVariable";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("processWithSubProcess", processVariables);

		Task task = taskService.createTaskQuery().taskDefinitionKey("UserTask_1").singleResult();
		runtimeService.setVariableLocal(task.ExecutionId, "aLocalVariable", "aLocalValue");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertEquals(1, tree.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance = tree.getActivityInstances("SubProcess_1")[0];

		// then the local variable has activity instance Id of the subprocess
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(subProcessInstance.Id);
		VariableInstance localVariable = query.singleResult();
		assertNotNull(localVariable);
		assertEquals("aLocalVariable", localVariable.Name);
		assertEquals("aLocalValue", localVariable.Value);

		// and the global variable has the activity instance Id of the process instance:
		query = runtimeService.createVariableInstanceQuery().activityInstanceIdIn(processInstance.Id);
		VariableInstance globalVariable = query.singleResult();
		assertNotNull(localVariable);
		assertEquals("processVariable", globalVariable.Name);
		assertEquals("aProcessVariable", globalVariable.Value);

		taskService.complete(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableBinaryFetching()
	  public virtual void testDisableBinaryFetching()
	  {
		sbyte[] binaryContent = "some binary content".GetBytes();

		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["binaryVariable"] = binaryContent;
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariablesLocal(task.Id, variables);

		// when binary fetching is enabled (default)
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then value is fetched
		VariableInstance result = query.singleResult();
		assertNotNull(result.Value);

		// when binary fetching is disabled
		query = runtimeService.createVariableInstanceQuery().disableBinaryFetching();

		// then value is not fetched
		result = query.singleResult();
		assertNull(result.Value);

		// delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources= "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml") public void testDisableBinaryFetchingForFileValues()
	  [Deployment(resources: "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDisableBinaryFetchingForFileValues()
	  {
		// given
		string fileName = "text.txt";
		string encoding = "crazy-encoding";
		string mimeType = "martini/dry";

		FileValue fileValue = Variables.fileValue(fileName).file("ABC".GetBytes()).encoding(encoding).mimeType(mimeType).create();

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("fileVar", fileValue));

		// when enabling binary fetching
		VariableInstance fileVariableInstance = runtimeService.createVariableInstanceQuery().singleResult();

		// then the binary value is accessible
		assertNotNull(fileVariableInstance.Value);

		// when disabling binary fetching
		fileVariableInstance = runtimeService.createVariableInstanceQuery().disableBinaryFetching().singleResult();

		// then the byte value is not fetched
		assertNotNull(fileVariableInstance);
		assertEquals("fileVar", fileVariableInstance.Name);

		assertNull(fileVariableInstance.Value);

		FileValue typedValue = (FileValue) fileVariableInstance.TypedValue;
		assertNull(typedValue.Value);

		// but typed value metadata is accessible
		assertEquals(ValueType.FILE, typedValue.Type);
		assertEquals(fileName, typedValue.Filename);
		assertEquals(encoding, typedValue.Encoding);
		assertEquals(mimeType, typedValue.MimeType);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableCustomObjectDeserialization()
	  public virtual void testDisableCustomObjectDeserialization()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariablesLocal(task.Id, variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().disableCustomObjectDeserialization();

		// then
		IList<VariableInstance> results = query.list();

		// both variables are not deserialized, but their serialized values are available
		assertEquals(2, results.Count);

		foreach (VariableInstance variableInstance in results)
		{
		  assertNull(variableInstance.ErrorMessage);

		  ObjectValue typedValue = (ObjectValue) variableInstance.TypedValue;
		  assertNotNull(typedValue);
		  assertFalse(typedValue.Deserialized);
		  // cannot access the deserialized value
		  try
		  {
			typedValue.Value;
		  }
		  catch (System.InvalidOperationException e)
		  {
			assertTextPresent("Object is not deserialized", e.Message);
		  }
		  assertNotNull(typedValue.ValueSerialized);
		}

		// delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSerializableErrorMessage()
	  public virtual void testSerializableErrorMessage()
	  {

		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["customSerializable"] = new CustomSerializable();
		variables["failingSerializable"] = new FailingSerializable();
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariablesLocal(task.Id, variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		IList<VariableInstance> results = query.list();

		// both variables are fetched
		assertEquals(2, results.Count);

		foreach (VariableInstance variableInstance in results)
		{
		  if (variableInstance.Name.Equals("customSerializable"))
		  {
			assertNotNull(variableInstance.Value);
			assertTrue(variableInstance.Value is CustomSerializable);
		  }
		  if (variableInstance.Name.Equals("failingSerializable"))
		  {
			// no value was fetched
			assertNull(variableInstance.Value);
			// error message is present
			assertNotNull(variableInstance.ErrorMessage);
		  }
		}

		// delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseExecutionId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseExecutionId()
	  {
		CaseInstance instance = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.caseExecutionIdIn(instance.Id);

		VariableInstance result = query.singleResult();

		assertNotNull(result);

		assertEquals("aVariableName", result.Name);
		assertEquals("abc", result.Value);
		assertEquals(instance.Id, result.CaseExecutionId);
		assertEquals(instance.Id, result.CaseInstanceId);

		assertNull(result.ExecutionId);
		assertNull(result.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseExecutionIds()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseExecutionIds()
	  {
		CaseInstance instance1 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		CaseInstance instance2 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("anotherVariableName", "xyz").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.caseExecutionIdIn(instance1.Id, instance2.Id).orderByVariableName().asc();

		IList<VariableInstance> result = query.list();

		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("abc", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals("xyz", variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseInstanceId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceId()
	  {
		CaseInstance instance = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.caseInstanceIdIn(instance.Id);

		VariableInstance result = query.singleResult();

		assertNotNull(result);

		assertEquals("aVariableName", result.Name);
		assertEquals("abc", result.Value);
		assertEquals(instance.Id, result.CaseExecutionId);
		assertEquals(instance.Id, result.CaseInstanceId);

		assertNull(result.ExecutionId);
		assertNull(result.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseInstanceIds()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceIds()
	  {
		CaseInstance instance1 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		CaseInstance instance2 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("anotherVariableName", "xyz").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.caseInstanceIdIn(instance1.Id, instance2.Id).orderByVariableName().asc();

		IList<VariableInstance> result = query.list();

		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("abc", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals("xyz", variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseActivityInstanceId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseActivityInstanceId()
	  {
		CaseInstance instance = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.activityInstanceIdIn(instance.Id);

		VariableInstance result = query.singleResult();

		assertNotNull(result);

		assertEquals("aVariableName", result.Name);
		assertEquals("abc", result.Value);
		assertEquals(instance.Id, result.CaseExecutionId);
		assertEquals(instance.Id, result.CaseInstanceId);

		assertNull(result.ExecutionId);
		assertNull(result.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void testQueryByCaseActivityInstanceIds()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseActivityInstanceIds()
	  {
		CaseInstance instance1 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create();

		CaseInstance instance2 = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("anotherVariableName", "xyz").create();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		query.activityInstanceIdIn(instance1.Id, instance2.Id).orderByVariableName().asc();

		IList<VariableInstance> result = query.list();

		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("abc", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals("xyz", variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMultiInstanceSubProcess()
	  public virtual void testSequentialMultiInstanceSubProcess()
	  {
		// given a process instance in sequential MI
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");

		// when
		VariableInstance nrOfInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfInstances").singleResult();
		VariableInstance nrOfActiveInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfActiveInstances").singleResult();
		VariableInstance nrOfCompletedInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfCompletedInstances").singleResult();
		VariableInstance loopCounter = runtimeService.createVariableInstanceQuery().variableName("loopCounter").singleResult();

		// then the activity instance ids of the variable instances should be correct
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfInstances.ActivityInstanceId);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfActiveInstances.ActivityInstanceId);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfCompletedInstances.ActivityInstanceId);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, loopCounter.ActivityInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMultiInstanceSubProcess()
	  public virtual void testParallelMultiInstanceSubProcess()
	  {
		// given a process instance in sequential MI
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("miSequentialSubprocess");

		// when
		VariableInstance nrOfInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfInstances").singleResult();
		VariableInstance nrOfActiveInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfActiveInstances").singleResult();
		VariableInstance nrOfCompletedInstances = runtimeService.createVariableInstanceQuery().variableName("nrOfCompletedInstances").singleResult();
		IList<VariableInstance> loopCounters = runtimeService.createVariableInstanceQuery().variableName("loopCounter").list();

		// then the activity instance ids of the variable instances should be correct
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfInstances.ActivityInstanceId);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfActiveInstances.ActivityInstanceId);
		assertEquals(tree.getActivityInstances("miSubProcess#multiInstanceBody")[0].Id, nrOfCompletedInstances.ActivityInstanceId);

		ISet<string> loopCounterActivityInstanceIds = new HashSet<string>();
		foreach (VariableInstance loopCounter in loopCounters)
		{
		  loopCounterActivityInstanceIds.Add(loopCounter.ActivityInstanceId);
		}

		assertEquals(4, loopCounterActivityInstanceIds.Count);

		foreach (ActivityInstance subProcessInstance in tree.getActivityInstances("miSubProcess"))
		{
		  assertTrue(loopCounterActivityInstanceIds.Contains(subProcessInstance.Id));
		}
	  }


	}

}