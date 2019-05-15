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
namespace org.camunda.bpm.engine.impl.core.model
{

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	[Serializable]
	public abstract class CoreModelElement
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal Properties properties = new Properties();

	  /// <summary>
	  /// contains built-in listeners </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>> builtInListeners = new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>>();
	  protected internal IDictionary<string, IList<DelegateListener<BaseDelegateExecution>>> builtInListeners = new Dictionary<string, IList<DelegateListener<BaseDelegateExecution>>>();

	  /// <summary>
	  /// contains all listeners (built-in + user-provided) </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>> listeners = new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>>();
	  protected internal IDictionary<string, IList<DelegateListener<BaseDelegateExecution>>> listeners = new Dictionary<string, IList<DelegateListener<BaseDelegateExecution>>>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> builtInVariableListeners = new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>();
	  protected internal IDictionary<string, IList<VariableListener<object>>> builtInVariableListeners = new Dictionary<string, IList<VariableListener<object>>>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> variableListeners = new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>>();
	  protected internal IDictionary<string, IList<VariableListener<object>>> variableListeners = new Dictionary<string, IList<VariableListener<object>>>();

	  public CoreModelElement(string id)
	  {
		this.id = id;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }

	  /// <seealso cref= Properties#set(PropertyKey, Object) </seealso>
	  public virtual void setProperty(string name, object value)
	  {
		properties.set(new PropertyKey<object>(name), value);
	  }

	  /// <seealso cref= Properties#get(PropertyKey) </seealso>
	  public virtual object getProperty(string name)
	  {
		return properties.get(new PropertyKey<object>(name));
	  }

	  /// <summary>
	  /// Returns the properties of the element.
	  /// </summary>
	  /// <returns> the properties </returns>
	  public virtual Properties Properties
	  {
		  get
		  {
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }




	  //event listeners //////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> getListeners(String eventName)
	  public virtual IList<DelegateListener<BaseDelegateExecution>> getListeners(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> listenerList = getListeners().get(eventName);
		IList<DelegateListener<BaseDelegateExecution>> listenerList = Listeners[eventName];
		if (listenerList != null)
		{
		  return listenerList;
		}
		return Collections.emptyList();
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> getBuiltInListeners(String eventName)
	  public virtual IList<DelegateListener<BaseDelegateExecution>> getBuiltInListeners(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> listenerList = getBuiltInListeners().get(eventName);
		IList<DelegateListener<BaseDelegateExecution>> listenerList = BuiltInListeners[eventName];
		if (listenerList != null)
		{
		  return listenerList;
		}
		return Collections.emptyList();
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> getVariableListenersLocal(String eventName)
	  public virtual IList<VariableListener<object>> getVariableListenersLocal(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listenerList = getVariableListeners().get(eventName);
		IList<VariableListener<object>> listenerList = VariableListeners[eventName];
		if (listenerList != null)
		{
		  return listenerList;
		}
		return Collections.emptyList();
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> getBuiltInVariableListenersLocal(String eventName)
	  public virtual IList<VariableListener<object>> getBuiltInVariableListenersLocal(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listenerList = getBuiltInVariableListeners().get(eventName);
		IList<VariableListener<object>> listenerList = BuiltInVariableListeners[eventName];
		if (listenerList != null)
		{
		  return listenerList;
		}
		return Collections.emptyList();
	  }

	  public virtual void addListener<T1>(string eventName, DelegateListener<T1> listener) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		addListener(eventName, listener, -1);
	  }

	  public virtual void addBuiltInListener<T1>(string eventName, DelegateListener<T1> listener) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		addBuiltInListener(eventName, listener, -1);
	  }

	  public virtual void addBuiltInListener<T1>(string eventName, DelegateListener<T1> listener, int index) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		addListenerToMap(listeners, eventName, listener, index);
		addListenerToMap(builtInListeners, eventName, listener, index);
	  }

	  public virtual void addListener<T1>(string eventName, DelegateListener<T1> listener, int index) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		addListenerToMap(listeners, eventName, listener, index);
	  }

	  protected internal virtual void addListenerToMap<T>(IDictionary<string, IList<T>> listenerMap, string eventName, T listener, int index)
	  {
		IList<T> listeners = listenerMap[eventName];
		if (listeners == null)
		{
		  listeners = new List<T>();
		  listenerMap[eventName] = listeners;
		}
		if (index < 0)
		{
		  listeners.Add(listener);
		}
		else
		{
		  listeners.Insert(index, listener);
		}
	  }

	  public virtual void addVariableListener<T1>(string eventName, VariableListener<T1> listener)
	  {
		addVariableListener(eventName, listener, -1);
	  }

	  public virtual void addVariableListener<T1>(string eventName, VariableListener<T1> listener, int index)
	  {
		addListenerToMap(variableListeners, eventName, listener, index);
	  }

	  public virtual void addBuiltInVariableListener<T1>(string eventName, VariableListener<T1> listener)
	  {
		addBuiltInVariableListener(eventName, listener, -1);
	  }

	  public virtual void addBuiltInVariableListener<T1>(string eventName, VariableListener<T1> listener, int index)
	  {
		addListenerToMap(variableListeners, eventName, listener, index);
		addListenerToMap(builtInVariableListeners, eventName, listener, index);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>> getListeners()
	  public virtual IDictionary<string, IList<DelegateListener<BaseDelegateExecution>>> Listeners
	  {
		  get
		  {
			return listeners;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>>> getBuiltInListeners()
	  public virtual IDictionary<string, IList<DelegateListener<BaseDelegateExecution>>> BuiltInListeners
	  {
		  get
		  {
			return builtInListeners;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> getBuiltInVariableListeners()
	  public virtual IDictionary<string, IList<VariableListener<object>>> BuiltInVariableListeners
	  {
		  get
		  {
			return builtInVariableListeners;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> getVariableListeners()
	  public virtual IDictionary<string, IList<VariableListener<object>>> VariableListeners
	  {
		  get
		  {
			return variableListeners;
		  }
	  }

	}

}