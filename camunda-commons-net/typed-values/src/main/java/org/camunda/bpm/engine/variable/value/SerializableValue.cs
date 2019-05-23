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
namespace org.camunda.bpm.engine.variable.value
{
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;

	/// <summary>
	/// A <seealso cref="TypedValue"/> for which a serialized value can be obtained and specified
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface SerializableValue : TypedValue
	{

	  /// <summary>
	  /// Returns true in case the value is deserialized. If this method returns true,
	  /// it is safe to call the <seealso cref="getValue()"/> method
	  /// </summary>
	  /// <returns> true if the object is deserialized. </returns>
	  bool Deserialized {get;}

	  /// <summary>
	  /// Returns the value or null in case the value is null.
	  /// </summary>
	  /// <returns> the value represented by this TypedValue. </returns>
	  /// <exception cref="IllegalStateException"> in case the value is not deserialized. See <seealso cref="isDeserialized()"/>. </exception>
	  object Value {get;}

	  /// <summary>
	  /// Returns the serialized value. In case the serializaton data format
	  /// (as returned by <seealso cref="getSerializationDataFormat()"/>) is not text based,
	  /// a base 64 encoded representation of the value is returned
	  /// 
	  /// The serialized value is a snapshot of the state of the value as it is
	  /// serialized to the process engine database.
	  /// </summary>
	  string ValueSerialized {get;}

	  /// <summary>
	  /// The serialization format used to serialize this value.
	  /// </summary>
	  /// <returns> the serialization format used to serialize this variable. </returns>
	  string SerializationDataFormat {get;}

	  SerializableValueType Type {get;}

	}

}