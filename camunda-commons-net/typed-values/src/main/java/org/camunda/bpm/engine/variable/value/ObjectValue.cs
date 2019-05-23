using System;

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
	/// <summary>
	/// <para>A typed value representing a Java Object.</para>
	/// 
	/// @author Daniel Meyer
	/// @since 7.2
	/// </summary>
	public interface ObjectValue : SerializableValue
	{

	  /// <summary>
	  /// Returns true in case the object is deserialized. If this method returns true,
	  /// it is safe to call the methods
	  /// <ul>
	  ///   <li><seealso cref="getValue()"/> and <seealso cref="getValue(System.Type)"/></li>
	  ///   <li><seealso cref="getObjectType()"/></li>
	  /// </ul>
	  /// </summary>
	  /// <returns> true if the object is deserialized. </returns>
	  bool Deserialized {get;}

	  /// <summary>
	  /// Returns the Object or null in case the value is null.
	  /// </summary>
	  /// <returns> the object represented by this TypedValue. </returns>
	  /// <exception cref="IllegalStateException"> in case the object is not deserialized. See <seealso cref="isDeserialized()"/>. </exception>
	  object Value {get;}

	  /// <summary>
	  /// Returns the object provided by this VariableValue. Allows type-safe access to objects
	  /// by passing in the class.
	  /// </summary>
	  /// <param name="type"> the java class the value should be cast to </param>
	  /// <returns> the object represented by this TypedValue. </returns>
	  /// <exception cref="IllegalStateException"> in case the object is not deserialized. See <seealso cref="isDeserialized()"/>. </exception>
	  T getValue<T>(Type type);

	  /// <summary>
	  /// Returns the Class this object is an instance of.
	  /// </summary>
	  /// <returns> the Class this object is an instance of </returns>
	  /// <exception cref="IllegalStateException"> in case the object is not deserialized. See <seealso cref="isDeserialized()"/>. </exception>
	  Type ObjectType {get;}

	  /// <summary>
	  /// A String representation of the Object's type name.
	  /// Usually the canonical class name of the Java Class this object
	  /// is an instance of.
	  /// </summary>
	  /// <returns> the Object's type name. </returns>
	  string ObjectTypeName {get;}

	}

}