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
namespace org.camunda.bpm.engine.impl.core.model
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using VariableScopeLocalAdapter = org.camunda.bpm.engine.impl.core.variable.scope.VariableScopeLocalAdapter;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CallableElementParameter
	{

	  protected internal ParameterValueProvider sourceValueProvider;
	  protected internal string target;
	  protected internal bool allVariables;
	  protected internal bool readLocal = false;

	  // source ////////////////////////////////////////////////////////

	  public virtual object getSource(VariableScope variableScope)
	  {

		if (sourceValueProvider is ConstantValueProvider)
		{
		  string variableName = (string) sourceValueProvider.getValue(variableScope);

		  return variableScope.getVariableTyped(variableName);
		}
		else
		{
		  return sourceValueProvider.getValue(variableScope);
		}

	  }

	  public virtual void applyTo(VariableScope variableScope, VariableMap variables)
	  {
		if (readLocal)
		{
		  variableScope = new VariableScopeLocalAdapter(variableScope);
		}

		if (allVariables)
		{
		  IDictionary<string, object> allVariables = variableScope.Variables;
		  variables.putAll(allVariables);

		}
		else
		{
		  object value = getSource(variableScope);
		  variables.put(target, value);
		}
	  }

	  public virtual ParameterValueProvider SourceValueProvider
	  {
		  get
		  {
			return sourceValueProvider;
		  }
		  set
		  {
			this.sourceValueProvider = value;
		  }
	  }


	  // target //////////////////////////////////////////////////////////

	  public virtual string Target
	  {
		  get
		  {
			return target;
		  }
		  set
		  {
			this.target = value;
		  }
	  }


	  // all variables //////////////////////////////////////////////////

	  public virtual bool AllVariables
	  {
		  get
		  {
			return allVariables;
		  }
		  set
		  {
			this.allVariables = value;
		  }
	  }


	  // local

	  public virtual bool ReadLocal
	  {
		  set
		  {
			this.readLocal = value;
		  }
		  get
		  {
			return readLocal;
		  }
	  }


	}

}