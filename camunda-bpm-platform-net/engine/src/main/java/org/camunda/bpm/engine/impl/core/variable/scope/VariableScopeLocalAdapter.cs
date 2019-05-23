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
namespace org.camunda.bpm.engine.impl.core.variable.scope
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Wraps a variable scope as if it has no parent such that it is reduced to its local
	/// variables. For example <seealso cref="getVariable(string)"/> simply delegates to
	/// <seealso cref="getVariableLocal(string)"/>.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class VariableScopeLocalAdapter : VariableScope
	{

	  protected internal VariableScope wrappedScope;

	  public VariableScopeLocalAdapter(VariableScope wrappedScope)
	  {
		this.wrappedScope = wrappedScope;
	  }

	  public virtual string VariableScopeKey
	  {
		  get
		  {
			return wrappedScope.VariableScopeKey;
		  }
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return VariablesLocal;
		  }
		  set
		  {
			VariablesLocal = value;
    
		  }
	  }

	  public virtual VariableMap VariablesTyped
	  {
		  get
		  {
			return VariablesLocalTyped;
		  }
	  }

	  public virtual VariableMap getVariablesTyped(bool deserializeValues)
	  {
		return getVariablesLocalTyped(deserializeValues);
	  }

	  public virtual IDictionary<string, object> VariablesLocal
	  {
		  get
		  {
			return wrappedScope.VariablesLocal;
		  }
		  set
		  {
			wrappedScope.VariablesLocal = value;
    
		  }
	  }

	  public virtual VariableMap VariablesLocalTyped
	  {
		  get
		  {
			return wrappedScope.VariablesLocalTyped;
		  }
	  }

	  public virtual VariableMap getVariablesLocalTyped(bool deserializeValues)
	  {
		return wrappedScope.getVariablesLocalTyped(deserializeValues);
	  }

	  public virtual object getVariable(string variableName)
	  {
		return getVariableLocal(variableName);
	  }

	  public virtual object getVariableLocal(string variableName)
	  {
		return wrappedScope.getVariableLocal(variableName);
	  }

	  public virtual T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableLocalTyped(variableName);
	  }

	  public virtual T getVariableTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableLocalTyped(variableName, deserializeValue);
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return wrappedScope.getVariableLocalTyped(variableName);
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return wrappedScope.getVariableLocalTyped(variableName, deserializeValue);
	  }

	  public virtual ISet<string> VariableNames
	  {
		  get
		  {
			return VariableNamesLocal;
		  }
	  }

	  public virtual ISet<string> VariableNamesLocal
	  {
		  get
		  {
			return wrappedScope.VariableNamesLocal;
		  }
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		setVariableLocal(variableName, value);
	  }

	  public virtual void setVariableLocal(string variableName, object value)
	  {
		wrappedScope.setVariableLocal(variableName, value);

	  }



	  public virtual bool hasVariables()
	  {
		return hasVariablesLocal();
	  }

	  public virtual bool hasVariablesLocal()
	  {
		return wrappedScope.hasVariablesLocal();
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		return hasVariableLocal(variableName);
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		return wrappedScope.hasVariableLocal(variableName);
	  }

	  public virtual void removeVariable(string variableName)
	  {
		removeVariableLocal(variableName);
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		wrappedScope.removeVariableLocal(variableName);
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		removeVariablesLocal(variableNames);
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		wrappedScope.removeVariablesLocal(variableNames);
	  }

	  public virtual void removeVariables()
	  {
		removeVariablesLocal();
	  }

	  public virtual void removeVariablesLocal()
	  {
		wrappedScope.removeVariablesLocal();
	  }

	}

}