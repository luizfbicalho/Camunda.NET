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
namespace org.camunda.bpm.engine.impl.variable.serializer
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class DefaultVariableSerializers : VariableSerializers
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<TypedValueSerializer<?>> serializerList = new java.util.ArrayList<TypedValueSerializer<?>>();
	  protected internal IList<TypedValueSerializer<object>> serializerList = new List<TypedValueSerializer<object>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, TypedValueSerializer<?>> serializerMap = new java.util.HashMap<String, TypedValueSerializer<?>>();
	  protected internal IDictionary<string, TypedValueSerializer<object>> serializerMap = new Dictionary<string, TypedValueSerializer<object>>();

	  public DefaultVariableSerializers()
	  {
	  }

	  public DefaultVariableSerializers(DefaultVariableSerializers serializers)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: this.serializerList.addAll(serializers.serializerList);
		((IList<TypedValueSerializer<object>>)this.serializerList).AddRange(serializers.serializerList);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		this.serializerMap.putAll(serializers.serializerMap);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public TypedValueSerializer<?> getSerializerByName(String serializerName)
	  public virtual TypedValueSerializer<object> getSerializerByName(string serializerName)
	  {
		 return serializerMap[serializerName];
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public TypedValueSerializer<?> findSerializerForValue(org.camunda.bpm.engine.variable.value.TypedValue value, VariableSerializerFactory fallBackSerializerFactory)
	  public virtual TypedValueSerializer<object> findSerializerForValue(TypedValue value, VariableSerializerFactory fallBackSerializerFactory)
	  {

		string defaultSerializationFormat = Context.ProcessEngineConfiguration.DefaultSerializationFormat;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<TypedValueSerializer<?>> matchedSerializers = new java.util.ArrayList<TypedValueSerializer<?>>();
		IList<TypedValueSerializer<object>> matchedSerializers = new List<TypedValueSerializer<object>>();

		ValueType type = value.Type;
		if (type != null && type.Abstract)
		{
		  throw new ProcessEngineException("Cannot serialize value of abstract type " + type.Name);
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (TypedValueSerializer<?> serializer : serializerList)
		foreach (TypedValueSerializer<object> serializer in serializerList)
		{
		  if (type == null || serializer.Type.Equals(type))
		  {

			// if type is null => ask handler whether it can handle the value
			// OR if types match, this handler can handle values of this type
			//    => BUT we still need to ask as the handler may not be able to handle ALL values of this type.

			if (serializer.canHandle(value))
			{
			  matchedSerializers.Add(serializer);
			  if (serializer.Type.PrimitiveValueType)
			  {
				break;
			  }
			}
		  }
		}

		if (matchedSerializers.Count == 0)
		{
		  if (fallBackSerializerFactory != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: TypedValueSerializer<?> serializer = fallBackSerializerFactory.getSerializer(value);
			TypedValueSerializer<object> serializer = fallBackSerializerFactory.getSerializer(value);
			if (serializer != null)
			{
			  return serializer;
			}
		  }

		  throw new ProcessEngineException("Cannot find serializer for value '" + value + "'.");
		}
		else if (matchedSerializers.Count == 1)
		{
		  return matchedSerializers[0];
		}
		else
		{
		  // ambiguous match, use default serializer
		  if (!string.ReferenceEquals(defaultSerializationFormat, null))
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (TypedValueSerializer<?> typedValueSerializer : matchedSerializers)
			foreach (TypedValueSerializer<object> typedValueSerializer in matchedSerializers)
			{
			  if (defaultSerializationFormat.Equals(typedValueSerializer.SerializationDataformat))
			  {
				return typedValueSerializer;
			  }
			}
		  }
		  // no default serialization dataformat defined or default dataformat cannot serialize this value => use first serializer
		  return matchedSerializers[0];
		}

	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public TypedValueSerializer<?> findSerializerForValue(org.camunda.bpm.engine.variable.value.TypedValue value)
	  public virtual TypedValueSerializer<object> findSerializerForValue(TypedValue value)
	  {
		return findSerializerForValue(value, null);
	  }

	  public virtual DefaultVariableSerializers addSerializer<T1>(TypedValueSerializer<T1> serializer)
	  {
		return addSerializer(serializer, serializerList.Count);
	  }

	  public virtual DefaultVariableSerializers addSerializer<T1>(TypedValueSerializer<T1> serializer, int index)
	  {
		serializerList.Insert(index, serializer);
		serializerMap[serializer.Name] = serializer;
		return this;
	  }

	  public virtual IList<T1> SerializerList<T1>
	  {
		  set
		  {
			this.serializerList.Clear();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: this.serializerList.addAll(value);
			((IList<TypedValueSerializer<object>>)this.serializerList).AddRange(value);
			this.serializerMap.Clear();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: for (TypedValueSerializer<?> serializer : value)
			foreach (TypedValueSerializer<object> serializer in value)
			{
			  serializerMap[serializer.Name] = serializer;
			}
		  }
	  }

	  public virtual int getSerializerIndex<T1>(TypedValueSerializer<T1> serializer)
	  {
		return serializerList.IndexOf(serializer);
	  }

	  public virtual int getSerializerIndexByName(string serializerName)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: TypedValueSerializer<?> serializer = serializerMap.get(serializerName);
		TypedValueSerializer<object> serializer = serializerMap[serializerName];
		if (serializer != null)
		{
		  return getSerializerIndex(serializer);
		}
		else
		{
		  return -1;
		}
	  }

	  public virtual VariableSerializers removeSerializer<T1>(TypedValueSerializer<T1> serializer)
	  {
		serializerList.Remove(serializer);
		serializerMap.Remove(serializer.Name);
		return this;
	  }

	  public virtual VariableSerializers join(VariableSerializers other)
	  {
		DefaultVariableSerializers copy = new DefaultVariableSerializers();

		// "other" serializers override existing ones if their names match
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (TypedValueSerializer<?> thisSerializer : serializerList)
		foreach (TypedValueSerializer<object> thisSerializer in serializerList)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: TypedValueSerializer<?> serializer = other.getSerializerByName(thisSerializer.getName());
		  TypedValueSerializer<object> serializer = other.getSerializerByName(thisSerializer.Name);

		  if (serializer == null)
		  {
			serializer = thisSerializer;
		  }

		  copy.addSerializer(serializer);
		}

		// add all "other" serializers that did not exist before to the end of the list
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (TypedValueSerializer<?> otherSerializer : other.getSerializers())
		foreach (TypedValueSerializer<object> otherSerializer in other.Serializers)
		{
		  if (!copy.serializerMap.ContainsKey(otherSerializer.Name))
		  {
			copy.addSerializer(otherSerializer);
		  }
		}


		return copy;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<TypedValueSerializer<?>> getSerializers()
	  public virtual IList<TypedValueSerializer<object>> Serializers
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: return new java.util.ArrayList<TypedValueSerializer<?>>(serializerList);
			return new List<TypedValueSerializer<object>>(serializerList);
		  }
	  }

	}

}