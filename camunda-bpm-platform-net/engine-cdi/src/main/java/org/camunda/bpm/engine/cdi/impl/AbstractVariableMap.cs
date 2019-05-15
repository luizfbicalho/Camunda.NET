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
namespace org.camunda.bpm.engine.cdi.impl
{

	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	internal abstract class AbstractVariableMap : VariableMap
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject protected org.camunda.bpm.engine.cdi.BusinessProcess businessProcess;
	  protected internal BusinessProcess businessProcess;

	  protected internal abstract object getVariable(string variableName);
	  protected internal abstract T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue;

	  protected internal abstract void setVariable(string variableName, object value);

	  public override object get(object key)
	  {
		if (key == null)
		{
		  throw new System.ArgumentException("This map does not support 'null' keys.");
		}
		return getVariable(key.ToString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T> T getValue(String name, Class<T> type)
	  public override T getValue<T>(string name, Type<T> type)
	  {
		object @object = get(name);
		if (@object == null)
		{
		  return null;
		}
		else if (type.IsAssignableFrom(@object.GetType()))
		{
		  return (T) @object;
		}
		else
		{
		  throw new System.InvalidCastException("Cannot cast variable named '" + name + "' with value '" + @object + "' to type '" + type + "'.");
		}
	  }

	  public override T getValueTyped<T>(string name) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		if (string.ReferenceEquals(name, null))
		{
		  throw new System.ArgumentException("This map does not support 'null' keys.");
		}
		return getVariableTyped(name);
	  }

	  public override object put(string key, object value)
	  {
		if (string.ReferenceEquals(key, null))
		{
		  throw new System.ArgumentException("This map does not support 'null' keys.");
		}
		object variableBefore = getVariable(key);
		setVariable(key, value);
		return variableBefore;
	  }

	  public override void putAll<T1>(IDictionary<T1> m) where T1 : string
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (java.util.Map.Entry< ? extends String, ? extends Object> newEntry : m.entrySet())
		foreach (KeyValuePair<string, ? extends object> newEntry in m.SetOfKeyValuePairs())
		{
		  setVariable(newEntry.Key, newEntry.Value);
		}
	  }

	  public override VariableMap putValue(string name, object value)
	  {
		put(name, value);
		return this;
	  }

	  public override VariableMap putValueTyped(string name, TypedValue value)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  throw new System.ArgumentException("This map does not support 'null' names.");
		}
		setVariable(name, value);
		return this;
	  }

	  public override int size()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".size() is not supported.");
	  }

	  public override bool Empty
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.NotSupportedException(this.GetType().FullName + ".isEmpty() is not supported.");
		  }
	  }

	  public override bool containsKey(object key)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".containsKey() is not supported.");
	  }

	  public override bool containsValue(object value)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".containsValue() is not supported.");
	  }

	  public override object remove(object key)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".remove is unsupported. Use " + this.GetType().FullName + ".put(key, null)");
	  }

	  public override void clear()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".clear() is not supported.");
	  }

	  public override ISet<string> keySet()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".keySet() is not supported.");
	  }

	  public override ICollection<object> values()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".values() is not supported.");
	  }

	  public override ISet<KeyValuePair<string, object>> entrySet()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".entrySet() is not supported.");
	  }

	  public virtual VariableContext asVariableContext()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(this.GetType().FullName + ".asVariableContext() is not supported.");
	  }

	}

}