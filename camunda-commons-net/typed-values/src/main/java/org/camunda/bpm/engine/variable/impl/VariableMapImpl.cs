using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.variable.impl
{

	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class VariableMapImpl : VariableMap, VariableContext
	{

	  private const long serialVersionUID = 1L;

	  protected internal IDictionary<string, TypedValue> variables = new Dictionary<string, TypedValue>();

	  public VariableMapImpl(VariableMapImpl map)
	  {
		variables = new Dictionary<string, TypedValue>(map.variables);
	  }

	  public VariableMapImpl(IDictionary<string, object> map)
	  {
		if (map != null)
		{
		  putAll(map);
		}
	  }

	  public VariableMapImpl()
	  {
	  }

	  // VariableMap implementation //////////////////////////////

	  public virtual VariableMap putValue(string name, object value)
	  {
		put(name, value);
		return this;
	  }

	  public virtual VariableMap putValueTyped(string name, TypedValue value)
	  {
		variables[name] = value;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getValue(String name, Class<T> type)
	  public virtual T getValue<T>(string name, Type type)
	  {
			  type = typeof(T);
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getValueTyped(String name)
	  public virtual T getValueTyped<T>(string name) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return (T) variables[name];
	  }

	  // java.uitil Map<String, Object> implementation ////////////////////////////////////////

	  public virtual int size()
	  {
		return variables.Count;
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return variables.Count == 0;
		  }
	  }

	  public virtual bool containsKey(object key)
	  {
		return variables.ContainsKey(key);
	  }

	  public virtual bool containsValue(object value)
	  {
		foreach (TypedValue varValue in variables.Values)
		{
		  if (value == varValue.Value)
		  {
			return true;
		  }
		  else if (value != null && value.Equals(varValue.Value))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual object get(object key)
	  {
		TypedValue typedValue = variables[key];

		if (typedValue != null)
		{
		  return typedValue.Value;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual object put(string key, object value)
	  {

		TypedValue typedValue = Variables.untypedValue(value);

		TypedValue prevValue = variables[key] = typedValue;

		if (prevValue != null)
		{
		  return prevValue.Value;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual object remove(object key)
	  {
		TypedValue prevValue = variables.Remove(key);

		if (prevValue != null)
		{
		  return prevValue.Value;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual void putAll<T1>(IDictionary<T1> m) where T1 : string
	  {
		if (m != null)
		{
		  if (m is VariableMapImpl)
		  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			variables.putAll(((VariableMapImpl)m).variables);
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (java.util.Map.Entry<? extends String, ? extends Object> entry : m.entrySet())
			foreach (KeyValuePair<string, ? extends object> entry in m.SetOfKeyValuePairs())
			{
			  put(entry.Key, entry.Value);
			}
		  }
		}
	  }

	  public virtual void clear()
	  {
		variables.Clear();
	  }

	  public virtual ISet<string> keySet()
	  {
		return variables.Keys;
	  }

	  public virtual ICollection<object> values()
	  {

		// NOTE: cannot naively return List of values here. A proper implementation must return a
		// Collection which is backed by the actual variable map

		return new AbstractCollectionAnonymousInnerClass(this);
	  }

	  private class AbstractCollectionAnonymousInnerClass : AbstractCollection<object>
	  {
		  private readonly VariableMapImpl outerInstance;

		  public AbstractCollectionAnonymousInnerClass(VariableMapImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IEnumerator<object> iterator()
		  {

			// wrapped iterator. Must be local to the iterator() method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<org.camunda.bpm.engine.variable.value.TypedValue> iterator = variables.values().iterator();
			IEnumerator<TypedValue> iterator = outerInstance.variables.Values.GetEnumerator();

			return new IteratorAnonymousInnerClass(this, iterator);
		  }

		  private class IteratorAnonymousInnerClass : IEnumerator<object>
		  {
			  private readonly AbstractCollectionAnonymousInnerClass outerInstance;

			  private IEnumerator<TypedValue> iterator;

			  public IteratorAnonymousInnerClass(AbstractCollectionAnonymousInnerClass outerInstance, IEnumerator<TypedValue> iterator)
			  {
				  this.outerInstance = outerInstance;
				  this.iterator = iterator;
			  }

			  public bool hasNext()
			  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return iterator.hasNext();
			  }
			  public object next()
			  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return iterator.next().Value;
			  }
			  public void remove()
			  {
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
				iterator.remove();
			  }
		  }

		  public int size()
		  {
			return outerInstance.variables.Count;
		  }

	  }

	  public virtual ISet<KeyValuePair<string, object>> entrySet()
	  {

		// NOTE: cannot naively return Set of entries here. A proper implementation must
		// return a Set which is backed by the actual map

		return new AbstractSetAnonymousInnerClass(this);
	  }

	  private class AbstractSetAnonymousInnerClass : AbstractSet<KeyValuePair<string, object>>
	  {
		  private readonly VariableMapImpl outerInstance;

		  public AbstractSetAnonymousInnerClass(VariableMapImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IEnumerator<KeyValuePair<string, object>> iterator()
		  {

			return new IteratorAnonymousInnerClass2(this);
		  }

		  private class IteratorAnonymousInnerClass2 : IEnumerator<KeyValuePair<string, object>>
		  {
			  private readonly AbstractSetAnonymousInnerClass outerInstance;

			  public IteratorAnonymousInnerClass2(AbstractSetAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
				  iterator = outerInstance.outerInstance.variables.SetOfKeyValuePairs().GetEnumerator();
			  }


					// wrapped iterator. Must be local to the iterator() method
			  internal readonly IEnumerator<KeyValuePair<string, TypedValue>> iterator;

			  public bool hasNext()
			  {
				return iterator.hasNext();
			  }

			  public KeyValuePair<string, object> next()
			  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map.Entry<String, org.camunda.bpm.engine.variable.value.TypedValue> underlyingEntry = iterator.next();
				KeyValuePair<string, TypedValue> underlyingEntry = iterator.next();

				// return wrapper backed by the underlying entry
				return new EntryAnonymousInnerClass(this, underlyingEntry);
			  }

			  private class EntryAnonymousInnerClass : Entry<string, object>
			  {
				  private readonly IteratorAnonymousInnerClass2 outerInstance;

				  private KeyValuePair<string, TypedValue> underlyingEntry;

				  public EntryAnonymousInnerClass(IteratorAnonymousInnerClass2 outerInstance, KeyValuePair<string, TypedValue> underlyingEntry)
				  {
					  this.outerInstance = outerInstance;
					  this.underlyingEntry = underlyingEntry;
				  }

				  public string Key
				  {
					  get
					  {
						return underlyingEntry.Key;
					  }
				  }
				  public object Value
				  {
					  get
					  {
						return underlyingEntry.Value.Value;
					  }
				  }
				  public object setValue(object value)
				  {
					TypedValue typedValue = Variables.untypedValue(value);
					return underlyingEntry.setValue(typedValue);
				  }
				  public sealed override bool Equals(object o)
				  {
					if (!(o is DictionaryEntry))
					{
					  return false;
					}
					Entry e = (Entry) o;
					object k1 = Key;
					object k2 = e.Key;
					if (k1 == k2 || (k1 != null && k1.Equals(k2)))
					{
					  object v1 = Value;
					  object v2 = e.Value;
					  if (v1 == v2 || (v1 != null && v1.Equals(v2)))
					  {
						return true;
					  }
					}
					return false;
				  }
				  public sealed override int GetHashCode()
				  {
					string key = Key;
					object value = Value;
					return (string.ReferenceEquals(key, null) ? 0 : key.GetHashCode()) ^ (value == null ? 0 : value.GetHashCode());
				  }
			  }

			  public void remove()
			  {
				iterator.remove();
			  }
		  }

		  public int size()
		  {
			return outerInstance.variables.Count;
		  }

	  }

	  public override string ToString()
	  {
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{\n");
		foreach (Entry<string, TypedValue> variable in variables.SetOfKeyValuePairs())
		{
		  stringBuilder.Append("  ");
		  stringBuilder.Append(variable.Key);
		  stringBuilder.Append(" => ");
		  stringBuilder.Append(variable.Value);
		  stringBuilder.Append("\n");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	  }

	  public override bool Equals(object obj)
	  {
		return asValueMap().Equals(obj);
	  }

	  public override int GetHashCode()
	  {
		return asValueMap().GetHashCode();
	  }

	  public virtual IDictionary<string, object> asValueMap()
	  {
		return new Dictionary<string, object>(this);
	  }

	  public virtual TypedValue resolve(string variableName)
	  {
		return getValueTyped(variableName);
	  }

	  public virtual bool containsVariable(string variableName)
	  {
		return containsKey(variableName);
	  }

	  public virtual VariableContext asVariableContext()
	  {
		return this;
	  }

	}

}