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
namespace org.camunda.bpm.engine.test.api.variables
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class PrimitiveTypeValueSerializationTest
	public class PrimitiveTypeValueSerializationTest
	{

	  protected internal const string BPMN_FILE = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";
	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal const string VARIABLE_NAME = "variable";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: variable = {0}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {Variables.stringValue("a"), Variables.stringValue(null)},
			new object[] {Variables.booleanValue(true), Variables.booleanValue(null)},
			new object[] {Variables.integerValue(4), Variables.integerValue(null)},
			new object[] {Variables.shortValue((short) 2), Variables.shortValue(null)},
			new object[] {Variables.longValue(6L), Variables.longValue(null)},
			new object[] {Variables.doubleValue(4.2), Variables.doubleValue(null)},
			new object[] {Variables.dateValue(DateTime.Now), Variables.dateValue(null)}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public org.camunda.bpm.engine.variable.value.TypedValue typedValue;
	  public TypedValue typedValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public org.camunda.bpm.engine.variable.value.TypedValue nullValue;
	  public TypedValue nullValue;

	  private RuntimeService runtimeService;
	  private RepositoryService repositoryService;
	  private string deploymentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		runtimeService = rule.RuntimeService;
		repositoryService = rule.RepositoryService;

		deploymentId = repositoryService.createDeployment().addClasspathResource(BPMN_FILE).deploy().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void teardown()
	  public virtual void teardown()
	  {
		repositoryService.deleteDeployment(deploymentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGetUntypedVariable()
	  public virtual void shouldGetUntypedVariable()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.setVariable(instance.Id, VARIABLE_NAME, typedValue);

		object variableValue = runtimeService.getVariable(instance.Id, VARIABLE_NAME);
		assertEquals(typedValue.Value, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGetTypedVariable()
	  public virtual void shouldGetTypedVariable()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.setVariable(instance.Id, VARIABLE_NAME, typedValue);

		TypedValue typedVariableValue = runtimeService.getVariableTyped(instance.Id, VARIABLE_NAME);
		assertEquals(typedValue.Type, typedVariableValue.Type);
		assertEquals(typedValue.Value, typedVariableValue.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGetTypedNullVariable()
	  public virtual void shouldGetTypedNullVariable()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.setVariable(instance.Id, VARIABLE_NAME, nullValue);

		assertEquals(null, runtimeService.getVariable(instance.Id, VARIABLE_NAME));

		TypedValue typedVariableValue = runtimeService.getVariableTyped(instance.Id, VARIABLE_NAME);
		assertEquals(nullValue.Type, typedVariableValue.Type);
		assertEquals(null, typedVariableValue.Value);
	  }

	}

}