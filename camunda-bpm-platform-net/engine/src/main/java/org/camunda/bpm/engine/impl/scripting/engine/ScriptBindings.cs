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
namespace org.camunda.bpm.engine.impl.scripting.engine
{


	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// <para>A <seealso cref="Bindings"/> implementation which wraps an existing binding and enhances the key / value map with
	/// <strong>read-only</strong> access to:
	/// <ul>
	/// <li>variables provided in a <seealso cref="VariableScope"/>,</li>
	/// <li>additional bindings provided through a set of <seealso cref="Resolver Resolvers"/>.</li>
	/// </ul>
	/// 
	/// </para>
	/// <para><strong>Note on backwards compatibility:</strong> before 7.2 the Script
	/// bindings behaved in a way that all script variables were automatically exposed
	/// as process variables. You can enable this behavior by setting <seealso cref="autoStoreScriptVariables"/>.
	/// </para>
	/// 
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ScriptBindings : Bindings
	{

	  /// <summary>
	  /// The script engine implementations put some key/value pairs into the binding.
	  /// This list contains those keys, such that they wouldn't be stored as process variable.
	  /// 
	  /// This list contains the keywords for JUEL, Javascript and Groovy.
	  /// </summary>
	  protected internal static readonly ISet<string> UNSTORED_KEYS = new HashSet<string>(Arrays.asList("out", "out:print", "lang:import", "context", "elcontext", "print", "println", "S", "XML", "JSON", ScriptEngine.ARGV, "execution", "__doc__"));

	  protected internal IList<Resolver> scriptResolvers;
	  protected internal VariableScope variableScope;

	  protected internal Bindings wrappedBindings;

	  /// <summary>
	  /// if true, all script variables will be set in the variable scope. </summary>
	  protected internal bool autoStoreScriptVariables;

	  public ScriptBindings(IList<Resolver> scriptResolvers, VariableScope variableScope, Bindings wrappedBindings)
	  {
		this.scriptResolvers = scriptResolvers;
		this.variableScope = variableScope;
		this.wrappedBindings = wrappedBindings;
		autoStoreScriptVariables = AutoStoreScriptVariablesEnabled;
	  }

	  protected internal virtual bool AutoStoreScriptVariablesEnabled
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			if (processEngineConfiguration != null)
			{
			  return processEngineConfiguration.AutoStoreScriptVariables;
			}
			return false;
		  }
	  }

	  public virtual bool containsKey(object key)
	  {
		foreach (Resolver scriptResolver in scriptResolvers)
		{
		  if (scriptResolver.containsKey(key))
		  {
			return true;
		  }
		}
		return wrappedBindings.containsKey(key);
	  }

	  public virtual object get(object key)
	  {
		object result = null;

		if (wrappedBindings.containsKey(key))
		{
		  result = wrappedBindings.get(key);

		}
		else
		{
		  foreach (Resolver scriptResolver in scriptResolvers)
		  {
			if (scriptResolver.containsKey(key))
			{
			  result = scriptResolver.get(key);
			}
		  }
		}

		return result;
	  }

	  public virtual object put(string name, object value)
	  {

		if (autoStoreScriptVariables)
		{
		  if (!UNSTORED_KEYS.Contains(name))
		  {
			object oldValue = variableScope.getVariable(name);
			variableScope.setVariable(name, value);
			return oldValue;
		  }
		}

		return wrappedBindings.put(name, value);
	  }

	  public virtual ISet<KeyValuePair<string, object>> entrySet()
	  {
		return calculateBindingMap().SetOfKeyValuePairs();
	  }

	  public virtual ISet<string> keySet()
	  {
		return calculateBindingMap().Keys;
	  }

	  public virtual int size()
	  {
		return calculateBindingMap().Count;
	  }

	  public virtual ICollection<object> values()
	  {
		return calculateBindingMap().Values;
	  }

	  public virtual void putAll<T1>(IDictionary<T1> toMerge) where T1 : string
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (java.util.Map.Entry<? extends String, ? extends Object> entry : toMerge.entrySet())
		foreach (KeyValuePair<string, ? extends object> entry in toMerge.SetOfKeyValuePairs())
		{
		  put(entry.Key, entry.Value);
		}
	  }

	  public virtual object remove(object key)
	  {
		if (UNSTORED_KEYS.Contains(key))
		{
		  return null;
		}
		return wrappedBindings.remove(key);
	  }

	  public virtual void clear()
	  {
		wrappedBindings.clear();
	  }

	  public virtual bool containsValue(object value)
	  {
		return calculateBindingMap().ContainsValue(value);
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return calculateBindingMap().Count == 0;
		  }
	  }

	  protected internal virtual IDictionary<string, object> calculateBindingMap()
	  {

		IDictionary<string, object> bindingMap = new Dictionary<string, object>();

		foreach (Resolver resolver in scriptResolvers)
		{
		  foreach (string key in resolver.Keys)
		  {
			bindingMap[key] = resolver.get(key);
		  }
		}

		ISet<KeyValuePair<string, object>> wrappedBindingsEntries = wrappedBindings.entrySet();
		foreach (Entry<string, object> entry in wrappedBindingsEntries)
		{
		  bindingMap[entry.Key] = entry.Value;
		}

		return bindingMap;
	  }

	}

}