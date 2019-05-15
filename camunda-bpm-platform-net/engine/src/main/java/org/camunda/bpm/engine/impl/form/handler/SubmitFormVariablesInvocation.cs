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
namespace org.camunda.bpm.engine.impl.form.handler
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using DelegateInvocation = org.camunda.bpm.engine.impl.@delegate.DelegateInvocation;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SubmitFormVariablesInvocation : DelegateInvocation
	{

	  protected internal FormHandler formHandler;
	  protected internal VariableMap properties;
	  protected internal VariableScope variableScope;


	  public SubmitFormVariablesInvocation(FormHandler formHandler, VariableMap properties, VariableScope variableScope) : base(null, null)
	  {
		this.formHandler = formHandler;
		this.properties = properties;
		this.variableScope = variableScope;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		formHandler.submitFormVariables(properties, variableScope);
	  }

	}

}