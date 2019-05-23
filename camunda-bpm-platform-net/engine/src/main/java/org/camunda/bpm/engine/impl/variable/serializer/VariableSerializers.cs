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

	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Interface describing a container for all available <seealso cref="TypedValueSerializer"/>s of variables.
	/// 
	/// @author dsyer
	/// @author Frederik Heremans
	/// @author Daniel Meyer
	/// </summary>
	public interface VariableSerializers
	{

	  /// <summary>
	  /// Selects the <seealso cref="TypedValueSerializer"/> which should be used for persisting a VariableValue.
	  /// </summary>
	  /// <param name="value"> the value to persist </param>
	  /// <param name="fallBackSerializerFactory"> a factory to build a fallback serializer in case no suiting serializer
	  ///   can be determined. If this factory is not able to build serializer either, an exception is thrown. May be null </param>
	  /// <returns> the VariableValueserializer selected for persisting the value or 'null' in case no serializer can be found </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public TypedValueSerializer findSerializerForValue(org.camunda.bpm.engine.variable.value.TypedValue value, VariableSerializerFactory fallBackSerializerFactory);
	  TypedValueSerializer findSerializerForValue(TypedValue value, VariableSerializerFactory fallBackSerializerFactory);

	  /// <summary>
	  /// Same as calling <seealso cref="VariableSerializers.findSerializerForValue(TypedValue, VariableSerializerFactory)"/>
	  /// with no fallback serializer factory.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public TypedValueSerializer findSerializerForValue(org.camunda.bpm.engine.variable.value.TypedValue value);
	  TypedValueSerializer findSerializerForValue(TypedValue value);

	  /// 
	  /// <returns> the serializer for the given serializerName name.
	  /// Returns null if no type was found with the name. </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public TypedValueSerializer<?> getSerializerByName(String serializerName);
	  TypedValueSerializer<object> getSerializerByName(string serializerName);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public VariableSerializers addSerializer(TypedValueSerializer<?> serializer);
	  VariableSerializers addSerializer<T1>(TypedValueSerializer<T1> serializer);

	  /// <summary>
	  /// Add type at the given index. The index is used when finding a serializer for a VariableValue. When
	  /// different serializers can store a specific variable value, the one with the smallest
	  /// index will be used.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public VariableSerializers addSerializer(TypedValueSerializer<?> serializer, int index);
	  VariableSerializers addSerializer<T1>(TypedValueSerializer<T1> serializer, int index);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public VariableSerializers removeSerializer(TypedValueSerializer<?> serializer);
	  VariableSerializers removeSerializer<T1>(TypedValueSerializer<T1> serializer);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public int getSerializerIndex(TypedValueSerializer<?> serializer);
	  int getSerializerIndex<T1>(TypedValueSerializer<T1> serializer);

	  int getSerializerIndexByName(string serializerName);

	  /// <summary>
	  /// Merges two <seealso cref="VariableSerializers"/> instances into one. Implementations may apply
	  /// different merging strategies.
	  /// </summary>
	  VariableSerializers join(VariableSerializers other);

	  /// <summary>
	  /// Returns the serializers as a list in the order of their indices.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<TypedValueSerializer<?>> getSerializers();
	  IList<TypedValueSerializer<object>> Serializers {get;}

	}
}