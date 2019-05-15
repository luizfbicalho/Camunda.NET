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
namespace org.camunda.bpm.engine.impl.el
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// Variable-scope only used to resolve variables when NO execution is active but
	/// expression-resolving is needed. This occurs eg. when start-form properties have default's
	/// defined. Even though variables are not available yet, expressions should be resolved
	/// anyway.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class StartProcessVariableScope : VariableScope
	{

	  private static readonly StartProcessVariableScope INSTANCE = new StartProcessVariableScope();

	  private static VariableMap EMPTY_VARIABLE_MAP = Variables.fromMap(System.Linq.Enumerable.Empty<string, object>());

	  /// <summary>
	  /// Since a <seealso cref="StartProcessVariableScope"/> has no state, it's safe to use the same
	  /// instance to prevent too many useless instances created.
	  /// </summary>
	  public static StartProcessVariableScope SharedInstance
	  {
		  get
		  {
			return INSTANCE;
		  }
	  }

	  public virtual string VariableScopeKey
	  {
		  get
		  {
			return "scope";
		  }
	  }

	  public virtual VariableMap getVariables()
	  {
		return EMPTY_VARIABLE_MAP;
	  }

	  public virtual VariableMap getVariablesLocal()
	  {
		return EMPTY_VARIABLE_MAP;
	  }

	  public virtual object getVariable(string variableName)
	  {
		return null;
	  }

	  public virtual object getVariableLocal(string variableName)
	  {
		return null;
	  }

	  public virtual VariableMap getVariablesTyped(bool deserializeObjectValues)
	  {
		return getVariables();
	  }

	  public virtual VariableMap VariablesLocalTyped
	  {
		  get
		  {
			return getVariablesLocalTyped(true);
		  }
	  }

	  public virtual VariableMap VariablesTyped
	  {
		  get
		  {
			return getVariablesTyped(true);
		  }
	  }

	  public virtual VariableMap getVariablesLocalTyped(bool deserializeObjectValues)
	  {
		return getVariablesLocal();
	  }

	  public virtual object getVariable(string variableName, bool deserializeObjectValue)
	  {
		return null;
	  }

	  public virtual object getVariableLocal(string variableName, bool deserializeObjectValue)
	  {
		return null;
	  }

	  public virtual T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return default(T);
	  }

	  public virtual T getVariableTyped<T>(string variableName, bool deserializeObjectValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return default(T);
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return default(T);
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName, bool deserializeObjectValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return default(T);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Set<String> getVariableNames()
	  public virtual ISet<string> VariableNames
	  {
		  get
		  {
			return Collections.EMPTY_SET;
		  }
	  }

	  public virtual ISet<string> VariableNamesLocal
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public virtual void setVariableLocal(string variableName, object value)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public virtual void setVariables<T1>(IDictionary<T1> variables) where T1 : object
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public virtual void setVariablesLocal<T1>(IDictionary<T1> variables) where T1 : object
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public virtual bool hasVariables()
	  {
		return false;
	  }

	  public virtual bool hasVariablesLocal()
	  {
		return false;
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		return false;
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		return false;
	  }

	  public virtual void removeVariable(string variableName)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariables()
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariablesLocal()
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual IDictionary<string, CoreVariableInstance> VariableInstances
	  {
		  get
		  {
			return Collections.emptyMap();
		  }
	  }

	  public virtual CoreVariableInstance getVariableInstance(string name)
	  {
		return null;
	  }

	  public virtual IDictionary<string, CoreVariableInstance> VariableInstancesLocal
	  {
		  get
		  {
			return Collections.emptyMap();
		  }
	  }

	  public virtual CoreVariableInstance getVariableInstanceLocal(string name)
	  {
		return null;
	  }

	}

}