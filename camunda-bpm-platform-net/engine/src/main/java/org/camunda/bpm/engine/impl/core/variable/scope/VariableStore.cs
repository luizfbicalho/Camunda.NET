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


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableStore<T> where T : org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance
	{

	  protected internal VariablesProvider<T> variablesProvider;
	  protected internal IDictionary<string, T> variables;

	  protected internal IDictionary<string, T> removedVariables = new Dictionary<string, T>();

	  protected internal IList<VariableStoreObserver<T>> observers;

	  public VariableStore() : this(VariableCollectionProvider.emptyVariables<T>())
	  {
	  }

	  public VariableStore(VariablesProvider<T> provider, params VariableStoreObserver<T>[] observers)
	  {
		this.variablesProvider = provider;
		this.observers = new List<VariableStoreObserver<T>>();
		((IList<VariableStoreObserver<T>>)this.observers).AddRange(Arrays.asList(observers));
	  }

	  /// <summary>
	  /// The variables provider can be exchanged as long as the variables are not yet initialized
	  /// </summary>
	  public virtual void setVariablesProvider(VariablesProvider<T> variablesProvider)
	  {
		if (variables != null)
		{
		  // already initialized
		  return;
		}
		else
		{
		  this.variablesProvider = variablesProvider;
		}

	  }

	  protected internal virtual IDictionary<string, T> VariablesMap
	  {
		  get
		  {
			forceInitialization();
    
			return variables;
		  }
	  }

	  protected internal virtual IDictionary<string, T> getVariablesMap(ICollection<string> variableNames)
	  {
		if (variableNames == null)
		{
		  return VariablesMap;
		}

		IDictionary<string, T> result = new Dictionary<string, T>();

		if (Initialized)
		{
		  foreach (string variableName in variableNames)
		  {
			if (variables.ContainsKey(variableName))
			{
			  result[variableName] = variables[variableName];
			}
		  }
		}
		else
		{
		  // in this case we don't initialize the variables map,
		  // otherwise it would most likely contain only a subset
		  // of existing variables
		  foreach (T variable in variablesProvider.provideVariables(variableNames))
		  {
			result[variable.Name] = variable;
		  }
		}

		return result;
	  }

	  public virtual T getRemovedVariable(string name)
	  {
		return removedVariables[name];
	  }

	  public virtual T getVariable(string name)
	  {

		return VariablesMap[name];
	  }

	  public virtual IList<T> Variables
	  {
		  get
		  {
			return new List<T>(VariablesMap.Values);
		  }
	  }

	  public virtual IList<T> getVariables(ICollection<string> variableNames)
	  {
		return new List<T>(getVariablesMap(variableNames).Values);
	  }

	  public virtual void addVariable(T value)
	  {

		if (containsKey(value.Name))
		{
		  throw ProcessEngineLogger.CORE_LOGGER.duplicateVariableInstanceException(value);
		}

		VariablesMap[value.Name] = value;

		foreach (VariableStoreObserver<T> listener in observers)
		{
		  listener.onAdd(value);
		}

		if (removedVariables.ContainsKey(value.Name))
		{
		  removedVariables.Remove(value.Name);
		}
	  }

	  public virtual void updateVariable(T value)
	  {
		if (!containsKey(value.Name))
		{
		  throw ProcessEngineLogger.CORE_LOGGER.duplicateVariableInstanceException(value);
		}
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return VariablesMap.Count == 0;
		  }
	  }

	  public virtual bool containsValue(T value)
	  {
		return VariablesMap.ContainsValue(value);
	  }

	  public virtual bool containsKey(string key)
	  {
		return VariablesMap.ContainsKey(key);
	  }

	  public virtual ISet<string> Keys
	  {
		  get
		  {
			return new HashSet<string>(VariablesMap.Keys);
		  }
	  }

	  public virtual bool Initialized
	  {
		  get
		  {
			return variables != null;
		  }
	  }

	  public virtual void forceInitialization()
	  {
		if (!Initialized)
		{
		  variables = new Dictionary<string, T>();

		  foreach (T variable in variablesProvider.provideVariables())
		  {
			variables[variable.Name] = variable;
		  }
		}
	  }

	  public virtual T removeVariable(string variableName)
	  {

		if (!VariablesMap.ContainsKey(variableName))
		{
		  return default(T);
		}

		T value = VariablesMap.Remove(variableName);

		foreach (VariableStoreObserver<T> observer in observers)
		{
		  observer.onRemove(value);
		}

		removedVariables[variableName] = value;

		return value;
	  }

	  public virtual void removeVariables()
	  {
		IEnumerator<T> valuesIt = VariablesMap.Values.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		removedVariables.putAll(variables);
		while (valuesIt.MoveNext())
		{
		  T nextVariable = valuesIt.Current;

//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  valuesIt.remove();

		  foreach (VariableStoreObserver<T> observer in observers)
		  {
			observer.onRemove(nextVariable);
		  }
		}
	  }

	  public virtual void addObserver(VariableStoreObserver<T> observer)
	  {
		observers.Add(observer);
	  }

	  public virtual void removeObserver(VariableStoreObserver<T> observer)
	  {
		observers.Remove(observer);
	  }

	  public interface VariableStoreObserver<T> where T : org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance
	  {

		void onAdd(T variable);

		void onRemove(T variable);
	  }

	  public interface VariablesProvider<T> where T : org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance
	  {

		ICollection<T> provideVariables();

		ICollection<T> provideVariables(ICollection<string> variableNames);

	  }

	  public virtual bool isRemoved(string variableName)
	  {
		return removedVariables.ContainsKey(variableName);
	  }

	}

}