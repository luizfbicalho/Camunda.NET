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

	using FileValueTypeImpl = org.camunda.bpm.engine.variable.impl.type.FileValueTypeImpl;
	using ObjectTypeImpl = org.camunda.bpm.engine.variable.impl.type.ObjectTypeImpl;
	using BooleanTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.BooleanTypeImpl;
	using BytesTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.BytesTypeImpl;
	using DateTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.DateTypeImpl;
	using DoubleTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.DoubleTypeImpl;
	using IntegerTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.IntegerTypeImpl;
	using LongTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.LongTypeImpl;
	using NullTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.NullTypeImpl;
	using NumberTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.NumberTypeImpl;
	using ShortTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.ShortTypeImpl;
	using StringTypeImpl = org.camunda.bpm.engine.variable.impl.type.PrimitiveValueTypeImpl.StringTypeImpl;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// 
	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// 
	/// @since 7.2
	/// </summary>
	public interface ValueType
	{
	  /// <summary>
	  /// Returns the name of the variable type
	  /// </summary>
	  string Name {get;}

	  /// <summary>
	  /// Indicates whether this type is primitive valued. Primitive valued types can be handled
	  /// natively by the process engine.
	  /// </summary>
	  /// <returns> true if this is a primitive valued type. False otherwise </returns>
	  bool PrimitiveValueType {get;}

	  /// <summary>
	  /// Get the value info (meta data) for a <seealso cref="TypedValue"/>.
	  /// The keys of the returned map for a <seealso cref="TypedValue"/> are available
	  /// as constants in the value's <seealso cref="ValueType"/> interface.
	  /// </summary>
	  /// <param name="typedValue">
	  /// @return </param>
	  IDictionary<string, object> getValueInfo(TypedValue typedValue);

	  /// <summary>
	  /// Creates a new TypedValue using this type. </summary>
	  /// <param name="value"> the value </param>
	  /// <returns> the typed value for the value </returns>
	  TypedValue createValue(object value, IDictionary<string, object> valueInfo);

	  /// <summary>
	  /// <para>Gets the parent value type.</para>
	  /// 
	  /// <para>Value type hierarchy is only relevant for queries and has the
	  /// following meaning: When a value query is made
	  /// (e.g. all tasks with a certain variable value), a "child" type's value
	  /// also matches a parameter value of the parent type. This is only
	  /// supported when the parent value type's implementation of <seealso cref="#isAbstract()"/>
	  /// returns <code>true</code>.</para>
	  /// </summary>
	  ValueType Parent {get;}

	  /// <summary>
	  /// Determines whether the argument typed value can be converted to a
	  /// typed value of this value type.
	  /// </summary>
	  bool canConvertFromTypedValue(TypedValue typedValue);

	  /// <summary>
	  /// Converts a typed value to a typed value of this type.
	  /// This does not suceed if <seealso cref="#canConvertFromTypedValue(TypedValue)"/>
	  /// returns <code>false</code>.
	  /// </summary>
	  TypedValue convertFromTypedValue(TypedValue typedValue);

	  /// <summary>
	  /// <para>Returns whether the value type is abstract. This is <b>not related
	  /// to the term <i>abstract</i> in the Java language.</b></para>
	  /// 
	  /// Abstract value types cannot be used as types for variables but only used for querying.
	  /// </summary>
	  bool Abstract {get;}


	}

	public static class ValueType_Fields
	{
	  public static readonly PrimitiveValueType NULL = new NullTypeImpl();
	  public static readonly PrimitiveValueType BOOLEAN = new BooleanTypeImpl();
	  public static readonly PrimitiveValueType SHORT = new ShortTypeImpl();
	  public static readonly PrimitiveValueType LONG = new LongTypeImpl();
	  public static readonly PrimitiveValueType DOUBLE = new DoubleTypeImpl();
	  public static readonly PrimitiveValueType STRING = new StringTypeImpl();
	  public static readonly PrimitiveValueType INTEGER = new IntegerTypeImpl();
	  public static readonly PrimitiveValueType DATE = new DateTypeImpl();
	  public static readonly PrimitiveValueType BYTES = new BytesTypeImpl();
	  public static readonly PrimitiveValueType NUMBER = new NumberTypeImpl();
	  public static readonly SerializableValueType OBJECT = new ObjectTypeImpl();
	  public static readonly FileValueType FILE = new FileValueTypeImpl();
	  public const string VALUE_INFO_TRANSIENT = "transient";
	}

}