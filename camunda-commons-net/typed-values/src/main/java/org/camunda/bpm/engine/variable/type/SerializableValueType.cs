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
namespace org.camunda.bpm.engine.variable.type
{

	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @since 7.2
	/// </summary>
	public interface SerializableValueType : ValueType
	{

	  /// <summary>
	  /// Identifies the object's java type name.
	  /// </summary>

	  /// <summary>
	  /// Identifies the format in which the object is serialized.
	  /// </summary>


	  /// <summary>
	  /// Creates a new TypedValue using this type. </summary>
	  /// <param name="serializedValue"> the value in serialized form </param>
	  /// <returns> the typed value for the value </returns>
	  SerializableValue createValueFromSerialized(string serializedValue, IDictionary<string, object> valueInfo);

	}

	public static class SerializableValueType_Fields
	{
	  public const string VALUE_INFO_OBJECT_TYPE_NAME = "objectTypeName";
	  public const string VALUE_INFO_SERIALIZATION_DATA_FORMAT = "serializationDataFormat";
	}

}