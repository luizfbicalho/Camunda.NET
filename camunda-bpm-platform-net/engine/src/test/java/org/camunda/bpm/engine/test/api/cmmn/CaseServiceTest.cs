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
namespace org.camunda.bpm.engine.test.api.cmmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.*;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionCommandBuilder = org.camunda.bpm.engine.runtime.CaseExecutionCommandBuilder;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceTest : PluggableProcessEngineTestCase
	{

	  public virtual void testCreateCaseInstanceQuery()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		assertNotNull(query);
	  }

	  public virtual void testCreateCaseExecutionQuery()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		assertNotNull(query);
	  }

	  public virtual void testWithCaseExecution()
	  {
		CaseExecutionCommandBuilder builder = caseService.withCaseExecution("aCaseExecutionId");

		assertNotNull(builder);
	  }

	  public virtual void testManualStartInvalidCaseExecution()
	  {
		try
		{
		  caseService.withCaseExecution("invalid").manualStart();
		  fail();
		}
		catch (NotFoundException)
		{
		}

		try
		{
		  caseService.withCaseExecution(null).manualStart();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testCompleteInvalidCaseExeuction()
	  {
		try
		{
		  caseService.withCaseExecution("invalid").complete();
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.withCaseExecution(null).complete();
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  public virtual void testCloseInvalidCaseExeuction()
	  {
		try
		{
		  caseService.withCaseExecution("invalid").close();
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.withCaseExecution(null).close();
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  public virtual void testTerminateInvalidCaseExeuction()
	  {
		try
		{
		  caseService.withCaseExecution("invalid").terminate();
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.withCaseExecution(null).terminate();
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", stringValue("abc")).setVariable("anotherVariableName", integerValue(null)).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals(stringValue("abc"), variable.TypedValue);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(integerValue(null), variable.TypedValue);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariablesTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableMap variables = createVariables().putValueTyped("aVariableName", stringValue("abc")).putValueTyped("anotherVariableName", integerValue(null));

		// when
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals(stringValue("abc"), variable.TypedValue);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(integerValue(null), variable.TypedValue);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableAndVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).setVariable("aThirdVariable", 123).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else if (variable.Name.Equals("aThirdVariable"))
		  {
			assertEquals("aThirdVariable", variable.Name);
			assertEquals(123, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableAndVariablesTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableMap variables = createVariables().putValueTyped("aVariableName", stringValue("abc")).putValueTyped("anotherVariableName", integerValue(null));

		// when
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).setVariable("aThirdVariable", booleanValue(null)).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals(stringValue("abc"), variable.TypedValue);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(integerValue(null), variable.TypedValue);
		  }
		  else if (variable.Name.Equals("aThirdVariable"))
		  {
			assertEquals("aThirdVariable", variable.Name);
			assertEquals(booleanValue(null), variable.TypedValue);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariablesLocal(variables).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariablesLocalTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableMap variables = createVariables().putValueTyped("aVariableName", stringValue("abc")).putValueTyped("anotherVariableName", integerValue(null));

		// when
		caseService.withCaseExecution(caseExecutionId).setVariablesLocal(variables).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals(stringValue("abc"), variable.TypedValue);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(integerValue(null), variable.TypedValue);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableLocalAndVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariablesLocal(variables).setVariableLocal("aThirdVariable", 123).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else if (variable.Name.Equals("aThirdVariable"))
		  {
			assertEquals("aThirdVariable", variable.Name);
			assertEquals(123, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else if (variable.Name.Equals("aThirdVariable"))
		  {
			assertEquals("aThirdVariable", variable.Name);
			assertEquals(123, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteSetVariableAndVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).setVariableLocal("aThirdVariable", 123).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		VariableInstance aThirdVariable = result[0];

		assertNotNull(aThirdVariable);
		assertEquals("aThirdVariable", aThirdVariable.Name);
		assertEquals(123, aThirdVariable.Value);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(3, result.Count);

		foreach (VariableInstance variable in result)
		{


		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals(caseInstanceId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);

		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals(caseInstanceId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);

		  }
		  else if (variable.Name.Equals("aThirdVariable"))
		  {
			assertEquals(caseExecutionId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("aThirdVariable", variable.Name);
			assertEquals(123, variable.Value);

		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariable("aVariableName").removeVariable("anotherVariableName").execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IList<string> variableNames = new List<string>();
		variableNames.Add("aVariableName");
		variableNames.Add("anotherVariableName");

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariables(variableNames).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariableAndVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariable("aThirdVariable", 123).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		IList<string> variableNames = new List<string>();
		variableNames.Add("aVariableName");
		variableNames.Add("anotherVariableName");

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariables(variableNames).removeVariable("aThirdVariable").execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariableLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariableLocal("aVariableName").removeVariableLocal("anotherVariableName").execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		IList<string> variableNames = new List<string>();
		variableNames.Add("aVariableName");
		variableNames.Add("anotherVariableName");

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariablesLocal(variableNames).execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariableLocalAndVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).setVariableLocal("aThirdVariable", 123).execute();

		IList<string> variableNames = new List<string>();
		variableNames.Add("aVariableName");
		variableNames.Add("anotherVariableName");

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariablesLocal(variableNames).removeVariableLocal("aThirdVariable").execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveVariableAndVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariableLocal("aThirdVariable", 123).execute();

		IList<string> variableNames = new List<string>();
		variableNames.Add("aVariableName");
		variableNames.Add("anotherVariableName");

		// when
		caseService.withCaseExecution(caseExecutionId).removeVariables(variableNames).removeVariableLocal("aThirdVariable").execute();

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by caseInstanceId
		result = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveAndSetSameVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).removeVariable("aVariableName").setVariable("aVariableName", "xyz").execute();
		}
		catch (NotValidException e)
		{
		  // then
		  assertTextPresent("Cannot set and remove a variable with the same variable name: 'aVariableName' within a command.", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExecuteRemoveAndSetSameLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "xyz").removeVariableLocal("aVariableName").execute();
		}
		catch (NotValidException e)
		{
		  // then
		  assertTextPresent("Cannot set and remove a variable with the same variable name: 'aVariableName' within a command.", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 // when
		 IDictionary<string, object> variables = caseService.getVariables(caseExecutionId);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Count == 0);
		 assertEquals(2, variables.Count);

		 assertEquals("abc", variables["aVariableName"]);
		 assertEquals(999, variables["anotherVariableName"]);

		 assertEquals(variables, caseService.getVariablesTyped(caseExecutionId, true));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 // when
		 VariableMap variables = caseService.getVariablesTyped(caseExecutionId);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Empty);
		 assertEquals(2, variables.size());

		 assertEquals("abc", variables.get("aVariableName"));
		 assertEquals(999, variables.get("anotherVariableName"));

		 assertEquals(variables, caseService.getVariablesTyped(caseExecutionId, true));
	  }

	  public virtual void testGetVariablesInvalidCaseExecutionId()
	  {

		try
		{
		  caseService.getVariables("invalid");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariables(null);
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesWithVariableNames()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariable("thirVariable", "xyz").create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 IList<string> names = new List<string>();
		 names.Add("aVariableName");
		 names.Add("anotherVariableName");

		 // when
		 IDictionary<string, object> variables = caseService.getVariables(caseExecutionId, names);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Count == 0);
		 assertEquals(2, variables.Count);

		 assertEquals("abc", variables["aVariableName"]);
		 assertEquals(999, variables["anotherVariableName"]);

		 assertEquals(variables, caseService.getVariables(caseExecutionId, names));
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesWithVariableNamesTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariable("thirVariable", "xyz").create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 IList<string> names = new List<string>();
		 names.Add("aVariableName");
		 names.Add("anotherVariableName");

		 // when
		 VariableMap variables = caseService.getVariablesTyped(caseExecutionId, names, true);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Empty);
		 assertEquals(2, variables.size());

		 assertEquals("abc", variables.get("aVariableName"));
		 assertEquals(999, variables.get("anotherVariableName"));

		 assertEquals(variables, caseService.getVariables(caseExecutionId, names));
	  }

	  public virtual void testGetVariablesWithVariablesNamesInvalidCaseExecutionId()
	  {

		try
		{
		  caseService.getVariables("invalid", null);
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariables(null, null);
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		 // when
		 IDictionary<string, object> variables = caseService.getVariablesLocal(caseExecutionId);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Count == 0);
		 assertEquals(2, variables.Count);

		 assertEquals("abc", variables["aVariableName"]);
		 assertEquals(999, variables["anotherVariableName"]);

		 assertEquals(variables, caseService.getVariablesLocal(caseExecutionId));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocalTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		 // when
		 VariableMap variables = caseService.getVariablesLocalTyped(caseExecutionId);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Empty);
		 assertEquals(2, variables.size());

		 assertEquals("abc", variables.get("aVariableName"));
		 assertEquals(999, variables.get("anotherVariableName"));

		 assertEquals(variables, caseService.getVariablesLocalTyped(caseExecutionId, true));
	  }

	  public virtual void testGetVariablesLocalInvalidCaseExecutionId()
	  {

		try
		{
		  caseService.getVariablesLocal("invalid");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariablesLocal(null);
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocalWithVariableNames()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		 IList<string> names = new List<string>();
		 names.Add("aVariableName");
		 names.Add("anotherVariableName");

		 // when
		 IDictionary<string, object> variables = caseService.getVariablesLocal(caseExecutionId, names);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Count == 0);
		 assertEquals(2, variables.Count);

		 assertEquals("abc", variables["aVariableName"]);
		 assertEquals(999, variables["anotherVariableName"]);

		 assertEquals(variables, caseService.getVariablesLocal(caseExecutionId, names));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocalWithVariableNamesTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		 IList<string> names = new List<string>();
		 names.Add("aVariableName");
		 names.Add("anotherVariableName");

		 // when
		 VariableMap variables = caseService.getVariablesLocalTyped(caseExecutionId, names, true);

		 // then
		 assertNotNull(variables);
		 assertFalse(variables.Empty);
		 assertEquals(2, variables.size());

		 assertEquals("abc", variables.get("aVariableName"));
		 assertEquals(999, variables.get("anotherVariableName"));

		 assertEquals(variables, caseService.getVariablesLocal(caseExecutionId, names));
	  }
	  public virtual void testGetVariablesLocalWithVariablesNamesInvalidCaseExecutionId()
	  {

		try
		{
		  caseService.getVariablesLocal("invalid", null);
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariablesLocal(null, null);
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariable("thirVariable", "xyz").create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 // when
		 object value = caseService.getVariable(caseExecutionId, "aVariableName");

		 // then
		 assertNotNull(value);
		 assertEquals("abc", value);
	  }

	  public virtual void testGetVariableInvalidCaseExecutionId()
	  {
		try
		{
		  caseService.getVariable("invalid", "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariable(null, "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariableLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).execute();

		 // when
		 object value = caseService.getVariableLocal(caseExecutionId, "aVariableName");

		 // then
		 assertNotNull(value);
		 assertEquals("abc", value);
	  }

	  public virtual void testGetVariableLocalInvalidCaseExecutionId()
	  {
		try
		{
		  caseService.getVariableLocal("invalid", "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariableLocal(null, "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariableTyped()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).setVariable("aSerializedObject", Variables.objectValue(Arrays.asList("1", "2")).create()).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 // when
		 StringValue stringValue = caseService.getVariableTyped(caseExecutionId, "aVariableName");
		 ObjectValue objectValue = caseService.getVariableTyped(caseExecutionId, "aSerializedObject");
		 ObjectValue serializedObjectValue = caseService.getVariableTyped(caseExecutionId, "aSerializedObject", false);

		 // then
		 assertNotNull(stringValue.Value);
		 assertNotNull(objectValue.Value);
		 assertTrue(objectValue.Deserialized);
		 assertEquals(Arrays.asList("1", "2"), objectValue.Value);
		 assertFalse(serializedObjectValue.Deserialized);
		 assertNotNull(serializedObjectValue.ValueSerialized);
	  }

	  public virtual void testGetVariableTypedInvalidCaseExecutionId()
	  {
		try
		{
		  caseService.getVariableTyped("invalid", "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariableTyped(null, "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testSetVariable()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.setVariable(caseExecutionId, "aVariableName", "abc");

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		VariableInstance variable = result[0];
		assertEquals("aVariableName", variable.Name);
		assertEquals("abc", variable.Value);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testSetVariables()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 123;
		caseService.setVariables(caseExecutionId, variables);

		// then

		// query by caseExecutionId
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertTrue(result.Count == 0);

		// query by case instance id
		result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseInstanceId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{
		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals(caseInstanceId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);

		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals(caseInstanceId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("anotherVariableName", variable.Name);
			assertEquals(123, variable.Value);

		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testSetVariableLocal()
	  {
		// given:
		// a deployed case definition
		// an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.setVariableLocal(caseExecutionId, "aVariableName", "abc");

		// then

		// query by case instance id
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);

		// query by caseExecutionId
		result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(1, result.Count);

		VariableInstance variable = result[0];
		assertEquals("aVariableName", variable.Name);
		assertEquals("abc", variable.Value);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testSetVariablesLocal()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 123;
		caseService.setVariablesLocal(caseExecutionId, variables);

		// then

		// query by case instance id
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseInstanceId).list();

		assertTrue(result.Count == 0);

		// query by caseExecutionId
		result = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(caseExecutionId).list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{
		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals(caseExecutionId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);

		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals(caseExecutionId, variable.CaseExecutionId);
			assertEquals(caseInstanceId, variable.CaseInstanceId);

			assertEquals("anotherVariableName", variable.Name);
			assertEquals(123, variable.Value);

		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariableTypedLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		 caseService.withCaseDefinition(caseDefinitionId).create().Id;

		 string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		 caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).setVariableLocal("aSerializedObject", Variables.objectValue(Arrays.asList("1", "2")).create()).execute();

		 // when
		 StringValue stringValue = caseService.getVariableLocalTyped(caseExecutionId, "aVariableName");
		 ObjectValue objectValue = caseService.getVariableLocalTyped(caseExecutionId, "aSerializedObject");
		 ObjectValue serializedObjectValue = caseService.getVariableLocalTyped(caseExecutionId, "aSerializedObject", false);

		 // then
		 assertNotNull(stringValue.Value);
		 assertNotNull(objectValue.Value);
		 assertTrue(objectValue.Deserialized);
		 assertEquals(Arrays.asList("1", "2"), objectValue.Value);
		 assertFalse(serializedObjectValue.Deserialized);
		 assertNotNull(serializedObjectValue.ValueSerialized);
	  }

	  public virtual void testGetVariableLocalTypedInvalidCaseExecutionId()
	  {
		try
		{
		  caseService.getVariableLocalTyped("invalid", "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotFoundException)
		{

		}

		try
		{
		  caseService.getVariableLocalTyped(null, "aVariableName");
		  fail("The case execution should not be found.");
		}
		catch (NotValidException)
		{

		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testRemoveVariable()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "abc").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.removeVariable(caseExecutionId, "aVariableName");

		// then the variable should be gone
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testRemoveVariables()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "abc";
		variables["anotherVariable"] = 123;

		caseService.withCaseDefinitionByKey("oneTaskCase").setVariables(variables).setVariable("aThirdVariable", "def").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.removeVariables(caseExecutionId, variables.Keys);

		// then there should be only one variable left
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("aThirdVariable", variable.Name);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testRemoveVariableLocal()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.setVariableLocal(caseExecutionId, "aVariableName", "abc");

		// when
		caseService.removeVariableLocal(caseInstanceId, "aVariableName");

		// then the variable should still be there
		assertEquals(1, runtimeService.createVariableInstanceQuery().count());

		// when
		caseService.removeVariableLocal(caseExecutionId, "aVariableName");

		// then the variable should be gone
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testRemoveVariablesLocal()
	  {
		// given:
		// a deployed case definition
		// and an active case instance
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;


		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "abc";
		variables["anotherVariable"] = 123;

		caseService.setVariablesLocal(caseExecutionId, variables);
		caseService.setVariableLocal(caseExecutionId, "aThirdVariable", "def");

		// when
		caseService.removeVariablesLocal(caseInstanceId, variables.Keys);

		// then no variables should have been removed
		assertEquals(3, runtimeService.createVariableInstanceQuery().count());

		// when
		caseService.removeVariablesLocal(caseExecutionId, variables.Keys);

		// then there should be only one variable left
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();
		assertNotNull(variable);
		assertEquals("aThirdVariable", variable.Name);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/loan-application.cmmn")]
	  public virtual void testCreateCaseInstanceById()
	  {
		// given
		// there exists a deployment containing a case definition with key "loanApplication"

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("loanApplication").singleResult();

		assertNotNull(caseDefinition);

		// when
		// create a new case instance by id

		CaseInstance caseInstance = caseService.withCaseDefinition(caseDefinition.Id).create();

		// then
		// the returned caseInstance is not null

		assertNotNull(caseInstance);

		// verify that the case instance is persisted using the API

		CaseInstance instance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstance.Id).singleResult();

		assertNotNull(instance);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/loan-application.cmmn")]
	  public virtual void testCreateCaseInstanceByKey()
	  {
		// given
		// there exists a deployment containing a case definition with key "loanApplication"

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("loanApplication").singleResult();

		assertNotNull(caseDefinition);

		// when
		// create a new case instance by key

		CaseInstance caseInstance = caseService.withCaseDefinitionByKey(caseDefinition.Key).create();

		// then
		// the returned caseInstance is not null

		assertNotNull(caseInstance);

		// verify that the case instance is persisted using the API

		CaseInstance instance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstance.Id).singleResult();

		assertNotNull(instance);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/loan-application.cmmn")]
	  public virtual void testCaseExecutionQuery()
	  {
		// given
		// there exists a deployment containing a case definition with key "loanApplication"

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("loanApplication").singleResult();

		assertNotNull(caseDefinition);

		// when
		// create a new case instance by key

		CaseInstance caseInstance = caseService.withCaseDefinitionByKey(caseDefinition.Key).create();

		// then
		// the returned caseInstance is not null

		assertNotNull(caseInstance);

		// verify that there are three case execution:
		// - the case instance itself (ie. for the casePlanModel)
		// - a case execution for the stage
		// - a case execution for the humanTask

		IList<CaseExecution> caseExecutions = caseService.createCaseExecutionQuery().caseInstanceId(caseInstance.Id).list();

		assertEquals(3, caseExecutions.Count);

		CaseExecution casePlanModelExecution = caseService.createCaseExecutionQuery().activityId("CasePlanModel_1").singleResult();

		assertNotNull(casePlanModelExecution);

		CaseExecution stageExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNotNull(stageExecution);

		CaseExecution humanTaskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_6").singleResult();

		assertNotNull(humanTaskExecution);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/loan-application.cmmn")]
	  public virtual void testCaseInstanceQuery()
	  {
		// given
		// there exists a deployment containing a case definition with key "loanApplication"

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("loanApplication").singleResult();

		assertNotNull(caseDefinition);

		// when
		// create a new case instance by key

		CaseInstance caseInstance = caseService.withCaseDefinitionByKey(caseDefinition.Key).create();

		// then
		// the returned caseInstance is not null

		assertNotNull(caseInstance);

		// verify that there is one caseInstance

		// only select ACTIVE case instances
		IList<CaseInstance> caseInstances = caseService.createCaseInstanceQuery().active().list();

		assertEquals(1, caseInstances.Count);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesByEmptyList()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		// when
		IDictionary<string, object> variables = caseService.getVariables(caseInstanceId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesTypedByEmptyList()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		// when
		IDictionary<string, object> variables = caseService.getVariablesTyped(caseInstanceId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocalByEmptyList()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		// when
		IDictionary<string, object> variables = caseService.getVariablesLocal(caseInstanceId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testGetVariablesLocalTypedByEmptyList()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		// when
		IDictionary<string, object> variables = caseService.getVariablesLocalTyped(caseInstanceId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	}

}