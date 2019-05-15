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

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using TestVariableScope = org.camunda.bpm.engine.test.api.runtime.util.TestVariableScope;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class VariableScopeTest
	{

	  private const string VAR_NAME = "foo";

	  private const string VAR_VALUE_STRING = "bar";

	  private VariableScope variableScope;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		this.variableScope = new TestVariableScope();
		variableScope.setVariable(VAR_NAME, VAR_VALUE_STRING);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariables()
	  public virtual void testGetVariables()
	  {
		IDictionary<string, object> variables = variableScope.Variables;
		assertNotNull(variables);
		assertEquals(VAR_VALUE_STRING, variables[VAR_NAME]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesTyped()
	  public virtual void testGetVariablesTyped()
	  {
		VariableMap variables = variableScope.VariablesTyped;
		assertNotNull(variables);
		assertEquals(VAR_VALUE_STRING, variables.get(VAR_NAME));
		assertEquals(variables, variableScope.getVariablesTyped(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocal()
	  public virtual void testGetVariablesLocal()
	  {
		IDictionary<string, object> variables = variableScope.VariablesLocal;
		assertNotNull(variables);
		assertEquals(VAR_VALUE_STRING, variables[VAR_NAME]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesLocalTyped()
	  public virtual void testGetVariablesLocalTyped()
	  {
		IDictionary<string, object> variables = variableScope.VariablesLocalTyped;
		assertNotNull(variables);
		assertEquals(VAR_VALUE_STRING, variables[VAR_NAME]);
		assertEquals(variables, variableScope.getVariablesLocalTyped(true));
	  }

	}

}