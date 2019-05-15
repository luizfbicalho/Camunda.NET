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
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// A <seealso cref="TypedValueSerializer"/> persists <seealso cref="TypedValue TypedValues"/> of a given
	/// <seealso cref="ValueType"/> to provided <seealso cref="ValueFields"/>.
	/// <para>
	/// Replaces the "VariableType" interface in previous versions.
	/// 
	/// @author Daniel Meyer
	/// 
	/// @since 7.2
	/// </para>
	/// </summary>
	public interface TypedValueSerializer<T> where T : org.camunda.bpm.engine.variable.value.TypedValue
	{

	  /// <summary>
	  /// The name of this serializer. The name is used when persisting the ValueFields populated by this serializer.
	  /// </summary>
	  /// <returns> the name of this serializer. </returns>
	  string Name {get;}

	  /// <summary>
	  /// The <seealso cref="ValueType VariableType"/> supported </summary>
	  /// <returns> the VariableType supported </returns>
	  ValueType Type {get;}

	  /// <summary>
	  /// Serialize a <seealso cref="TypedValue"/> to the <seealso cref="ValueFields"/>.
	  /// </summary>
	  /// <param name="value"> the <seealso cref="TypedValue"/> to persist </param>
	  /// <param name="valueFields"> the <seealso cref="ValueFields"/> to which the value should be persisted </param>
	  void writeValue(T value, ValueFields valueFields);

	  /// <summary>
	  /// Retrieve a <seealso cref="TypedValue"/> from the provided <seealso cref="ValueFields"/>.
	  /// </summary>
	  /// <param name="valueFields"> the <seealso cref="ValueFields"/> to retrieve the value from </param>
	  /// <param name="deserializeValue"> indicates whether a <seealso cref="SerializableValue"/> should be deserialized.
	  /// </param>
	  /// <returns> the <seealso cref="TypedValue"/> </returns>
	  T readValue(ValueFields valueFields, bool deserializeValue);

	  /// <summary>
	  /// Used for auto-detecting the value type of a variable.
	  /// An implementation must return true if it is able to write values of the provided type.
	  /// </summary>
	  /// <param name="value"> the value </param>
	  /// <returns> true if this <seealso cref="TypedValueSerializer"/> is able to handle the provided value </returns>
	  bool canHandle(TypedValue value);

	  /// <summary>
	  /// Returns a typed value for the provided untyped value. This is used on cases where the user sets an untyped
	  /// value which is then detected to be handled by this <seealso cref="TypedValueSerializer"/> (by invocation of <seealso cref="#canHandle(TypedValue)"/>).
	  /// </summary>
	  /// <param name="untypedValue"> the untyped value </param>
	  /// <returns> the corresponding typed value </returns>
	  T convertToTypedValue(UntypedValueImpl untypedValue);

	  /// 
	  /// <returns> the dataformat used by the serializer or null if this is not an object serializer </returns>
	  string SerializationDataformat {get;}

	  /// <returns> whether values serialized by this serializer can be mutable and
	  /// should be re-serialized if changed </returns>
	  bool isMutableValue(T typedValue);

	}

}