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
namespace org.camunda.bpm.engine.variable
{

	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using EmptyVariableContext = org.camunda.bpm.engine.variable.impl.context.EmptyVariableContext;
	using AbstractTypedValue = org.camunda.bpm.engine.variable.impl.value.AbstractTypedValue;
	using FileValueImpl = org.camunda.bpm.engine.variable.impl.value.FileValueImpl;
	using NullValueImpl = org.camunda.bpm.engine.variable.impl.value.NullValueImpl;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using BooleanValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.BooleanValueImpl;
	using BytesValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.BytesValueImpl;
	using DateValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.DateValueImpl;
	using DoubleValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.DoubleValueImpl;
	using IntegerValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.IntegerValueImpl;
	using LongValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.LongValueImpl;
	using NumberValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.NumberValueImpl;
	using ShortValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.ShortValueImpl;
	using StringValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.StringValueImpl;
	using FileValueBuilderImpl = org.camunda.bpm.engine.variable.impl.value.builder.FileValueBuilderImpl;
	using ObjectVariableBuilderImpl = org.camunda.bpm.engine.variable.impl.value.builder.ObjectVariableBuilderImpl;
	using SerializedObjectValueBuilderImpl = org.camunda.bpm.engine.variable.impl.value.builder.SerializedObjectValueBuilderImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using DateValue = org.camunda.bpm.engine.variable.value.DateValue;
	using DoubleValue = org.camunda.bpm.engine.variable.value.DoubleValue;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;
	using NumberValue = org.camunda.bpm.engine.variable.value.NumberValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using SerializationDataFormat = org.camunda.bpm.engine.variable.value.SerializationDataFormat;
	using ShortValue = org.camunda.bpm.engine.variable.value.ShortValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using FileValueBuilder = org.camunda.bpm.engine.variable.value.builder.FileValueBuilder;
	using ObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.ObjectValueBuilder;
	using SerializedObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.SerializedObjectValueBuilder;
	using TypedValueBuilder = org.camunda.bpm.engine.variable.value.builder.TypedValueBuilder;

	/// <summary>
	/// <para>This class is the entry point to the process engine's typed variables API.
	/// Users can import the methods provided by this class using a static import:</para>
	/// 
	/// <code>
	/// import static org.camunda.bpm.engine.variable.Variables.*;
	/// </code>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Variables
	{

	  /// <summary>
	  /// <para>A set of builtin serialization dataformat constants. These constants can be used to specify
	  /// how java object variables should be serialized by the process engine:</para>
	  /// 
	  /// <pre>
	  /// CustomerData customerData = new CustomerData();
	  /// // ...
	  /// ObjectValue customerDataValue = Variables.objectValue(customerData)
	  ///   .serializationDataFormat(Variables.SerializationDataFormats.JSON)
	  ///   .create();
	  /// 
	  /// execution.setVariable("someVariable", customerDataValue);
	  /// </pre>
	  /// 
	  /// <para>Note that not all of the formats provided here are supported out of the box.</para>
	  /// 
	  /// @author Daniel Meyer
	  /// </summary>
	  public sealed class SerializationDataFormats : SerializationDataFormat
	  {

		/// <summary>
		/// <para>The Java Serialization Data format. If this data format is used for serializing an object,
		/// the object is serialized using default Java <seealso cref="Serializable"/>.</para>
		/// 
		/// <para>The process engine provides a serializer for this dataformat out of the box.</para>
		/// </summary>
		public static readonly SerializationDataFormats JAVA = new SerializationDataFormats("JAVA", InnerEnum.JAVA, "application/x-java-serialized-object");

		/// <summary>
		/// <para>The Json Serialization Data format. If this data format is used for serializing an object,
		/// the object is serialized as Json text.</para>
		/// 
		/// <para><strong>NOTE:</strong> the process does NOT provide a serializer for this dataformat out of the box.
		/// If you want to serialize objects using the Json dataformat, you need to provide a serializer. The optinal
		/// camunda Spin process engine plugin provides such a serializer.</para>
		/// </summary>
		public static readonly SerializationDataFormats JSON = new SerializationDataFormats("JSON", InnerEnum.JSON, "application/json");

		/// <summary>
		/// <para>The Xml Serialization Data format. If this data format is used for serializing an object,
		/// the object is serialized as Xml text.</para>
		/// 
		/// <para><strong>NOTE:</strong> the process does NOT provide a serializer for this dataformat out of the box.
		/// If you want to serialize objects using the Xml dataformat, you need to provide a serializer. The optinal
		/// camunda Spin process engine plugin provides such a serializer.</para>
		/// </summary>
		public static readonly SerializationDataFormats XML = new SerializationDataFormats("XML", InnerEnum.XML, "application/xml");

		private static readonly IList<SerializationDataFormats> valueList = new List<SerializationDataFormats>();

		static SerializationDataFormats()
		{
			valueList.Add(JAVA);
			valueList.Add(JSON);
			valueList.Add(XML);
		}

		public enum InnerEnum
		{
			JAVA,
			JSON,
			XML
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		internal readonly string name;

		internal SerializationDataFormats(string name, InnerEnum innerEnum, string name)
		{
		  this.name = name;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public string Name
		{
			get
			{
			  return name;
			}
		}

		  public static IList<SerializationDataFormats> values()
		  {
			  return valueList;
		  }

		  public int ordinal()
		  {
			  return ordinalValue;
		  }

		  public override string ToString()
		  {
			  return nameValue;
		  }

		  public static SerializationDataFormats valueOf(string name)
		  {
			  foreach (SerializationDataFormats enumInstance in SerializationDataFormats.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new System.ArgumentException(name);
		  }
	  }

	  /// <summary>
	  /// Returns a new <seealso cref="VariableMap"/> instance.
	  /// </summary>
	  public static VariableMap createVariables()
	  {
		return new VariableMapImpl();
	  }

	  /// <summary>
	  /// If the given map is not a variable map, adds all its entries as untyped
	  /// values to a new <seealso cref="VariableMap"/>. If the given map is a <seealso cref="VariableMap"/>,
	  /// it is returned as is.
	  /// </summary>
	  public static VariableMap fromMap(IDictionary<string, object> map)
	  {
		if (map is VariableMap)
		{
		  return (VariableMap) map;
		}
		else
		{
		  return new VariableMapImpl(map);
		}
	  }

	  /// <summary>
	  /// Shortcut for {@code Variables.createVariables().putValue(name, value)}
	  /// </summary>
	  public static VariableMap putValue(string name, object value)
	  {
		return createVariables().putValue(name, value);
	  }

	  /// <summary>
	  /// Shortcut for {@code Variables.createVariables().putValueTyped(name, value)}
	  /// </summary>
	  public static VariableMap putValueTyped(string name, TypedValue value)
	  {
		return createVariables().putValueTyped(name, value);
	  }

	  /// <summary>
	  /// Returns a builder to create a new <seealso cref="ObjectValue"/> that encapsulates
	  /// the given {@code value}.
	  /// </summary>
	  public static ObjectValueBuilder objectValue(object value)
	  {
		return new ObjectVariableBuilderImpl(value);
	  }

	  /// <summary>
	  /// Returns a builder to create a new <seealso cref="ObjectValue"/> that encapsulates
	  /// the given {@code value}.
	  /// </summary>
	  public static ObjectValueBuilder objectValue(object value, bool isTransient)
	  {
		return (ObjectValueBuilder) objectValue(value).setTransient(isTransient);
	  }

	  /// <summary>
	  /// Returns a builder to create a new <seealso cref="ObjectValue"/> from a serialized
	  /// object representation.
	  /// </summary>
	  public static SerializedObjectValueBuilder serializedObjectValue()
	  {
		return new SerializedObjectValueBuilderImpl();
	  }

	  /// <summary>
	  /// Shortcut for {@code Variables.serializedObjectValue().serializedValue(value)}
	  /// </summary>
	  public static SerializedObjectValueBuilder serializedObjectValue(string value)
	  {
		return serializedObjectValue().serializedValue(value);
	  }

	  /// <summary>
	  /// Shortcut for {@code Variables.serializedObjectValue().serializedValue(value).setTransient(isTransient)}
	  /// </summary>
	  public static SerializedObjectValueBuilder serializedObjectValue(string value, bool isTransient)
	  {
		return (SerializedObjectValueBuilder) serializedObjectValue().serializedValue(value).setTransient(isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="IntegerValue"/> that encapsulates the given <code>integer</code>
	  /// </summary>
	  public static IntegerValue integerValue(int? integer)
	  {
		return integerValue(integer, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="IntegerValue"/> that encapsulates the given <code>integer</code>
	  /// </summary>
	  public static IntegerValue integerValue(int? integer, bool isTransient)
	  {
		return new IntegerValueImpl(integer, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="StringValue"/> that encapsulates the given <code>stringValue</code>
	  /// </summary>
	  public static StringValue stringValue(string stringValue)
	  {
		return stringValue(stringValue, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="StringValue"/> that encapsulates the given <code>stringValue</code>
	  /// </summary>
	  public static StringValue stringValue(string stringValue, bool isTransient)
	  {
		return new StringValueImpl(stringValue, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="BooleanValue"/> that encapsulates the given <code>booleanValue</code>
	  /// </summary>
	  public static BooleanValue booleanValue(bool? booleanValue)
	  {
		return booleanValue(booleanValue, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="BooleanValue"/> that encapsulates the given <code>booleanValue</code>
	  /// </summary>
	  public static BooleanValue booleanValue(bool? booleanValue, bool isTransient)
	  {
		return new BooleanValueImpl(booleanValue, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="BytesValue"/> that encapsulates the given <code>bytes</code>
	  /// </summary>
	  public static BytesValue byteArrayValue(sbyte[] bytes)
	  {
		return byteArrayValue(bytes, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="BytesValue"/> that encapsulates the given <code>bytes</code>
	  /// </summary>
	  public static BytesValue byteArrayValue(sbyte[] bytes, bool isTransient)
	  {
		return new BytesValueImpl(bytes, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="DateValue"/> that encapsulates the given <code>date</code>
	  /// </summary>
	  public static DateValue dateValue(DateTime date)
	  {
		return dateValue(date, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="DateValue"/> that encapsulates the given <code>date</code>
	  /// </summary>
	  public static DateValue dateValue(DateTime date, bool isTransient)
	  {
		return new DateValueImpl(date, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="LongValue"/> that encapsulates the given <code>longValue</code>
	  /// </summary>
	  public static LongValue longValue(long? longValue)
	  {
		return longValue(longValue, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="LongValue"/> that encapsulates the given <code>longValue</code>
	  /// </summary>
	  public static LongValue longValue(long? longValue, bool isTransient)
	  {
		return new LongValueImpl(longValue, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ShortValue"/> that encapsulates the given <code>shortValue</code>
	  /// </summary>
	  public static ShortValue shortValue(short? shortValue)
	  {
		return shortValue(shortValue, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ShortValue"/> that encapsulates the given <code>shortValue</code>
	  /// </summary>
	  public static ShortValue shortValue(short? shortValue, bool isTransient)
	  {
		return new ShortValueImpl(shortValue, isTransient);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="DoubleValue"/> that encapsulates the given <code>doubleValue</code>
	  /// </summary>
	  public static DoubleValue doubleValue(double? doubleValue)
	  {
		return doubleValue(doubleValue, false);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="DoubleValue"/> that encapsulates the given <code>doubleValue</code>
	  /// </summary>
	  public static DoubleValue doubleValue(double? doubleValue, bool isTransient)
	  {
		return new DoubleValueImpl(doubleValue, isTransient);
	  }

	  /// <summary>
	  /// Creates an abstract Number value. Note that this value cannot be used to set variables.
	  /// Use the specific methods <seealso cref="Variables.integerValue(Integer)"/>, <seealso cref="shortValue(Short)"/>,
	  /// <seealso cref="longValue(Long)"/> and <seealso cref="doubleValue(Double)"/> instead.
	  /// </summary>
	  public static NumberValue numberValue(Number numberValue)
	  {
		return numberValue(numberValue, false);
	  }

	  /// <summary>
	  /// Creates an abstract Number value. Note that this value cannot be used to set variables.
	  /// Use the specific methods <seealso cref="Variables.integerValue(Integer)"/>, <seealso cref="shortValue(Short)"/>,
	  /// <seealso cref="longValue(Long)"/> and <seealso cref="doubleValue(Double)"/> instead.
	  /// </summary>
	  public static NumberValue numberValue(Number numberValue, bool isTransient)
	  {
		return new NumberValueImpl(numberValue, isTransient);
	  }

	  /// <summary>
	  /// Creates a <seealso cref="TypedValue"/> with value {@code null} and type <seealso cref="ValueType.NULL"/>
	  /// </summary>
	  public static TypedValue untypedNullValue()
	  {
		return untypedNullValue(false);
	  }

	  /// <summary>
	  /// Creates a <seealso cref="TypedValue"/> with value {@code null} and type <seealso cref="ValueType.NULL"/>
	  /// </summary>
	  public static TypedValue untypedNullValue(bool isTransient)
	  {
		if (isTransient)
		{
		  return NullValueImpl.INSTANCE_TRANSIENT;
		}
		else
		{
		  return NullValueImpl.INSTANCE;
		}
	  }

	  /// <summary>
	  /// Creates an untyped value, i.e. <seealso cref="TypedValue.getType()"/> returns <code>null</code>
	  /// for the returned instance.
	  /// </summary>
	  public static TypedValue untypedValue(object value)
	  {
		if (value is TypedValue)
		{
		  return untypedValue(value, ((TypedValue) value).Transient);
		}
		else
		{
			return untypedValue(value, false);
		}
	  }

	  /// <summary>
	  /// Creates an untyped value, i.e. <seealso cref="TypedValue.getType()"/> returns <code>null</code>
	  /// for the returned instance.
	  /// </summary>
	  public static TypedValue untypedValue(object value, bool isTransient)
	  {
		if (value == null)
		{
		  return untypedNullValue(isTransient);
		}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: else if (value instanceof org.camunda.bpm.engine.variable.value.builder.TypedValueBuilder<?>)
		else if (value is TypedValueBuilder<object>)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: return ((org.camunda.bpm.engine.variable.value.builder.TypedValueBuilder<?>) value).setTransient(isTransient).create();
		  return ((TypedValueBuilder<object>) value).setTransient(isTransient).create();
		}
		else if (value is TypedValue)
		{
		  TypedValue transientValue = (TypedValue) value;
		  if (value is NullValueImpl)
		  {
			transientValue = untypedNullValue(isTransient);
		  }
		  else if (value is FileValue)
		  {
			((FileValueImpl) transientValue).Transient = isTransient;
		  }
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: else if (value instanceof org.camunda.bpm.engine.variable.impl.value.AbstractTypedValue<?>)
		  else if (value is AbstractTypedValue<object>)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.engine.variable.impl.value.AbstractTypedValue<?>) transientValue).setTransient(isTransient);
			((AbstractTypedValue<object>) transientValue).Transient = isTransient;
		  }
		  return transientValue;
		}
		else
		{
		  // unknown value
		  return new UntypedValueImpl(value, isTransient);
		}
	  }

	  /// <summary>
	  /// Returns a builder to create a new <seealso cref="FileValue"/> with the given
	  /// {@code filename}.
	  /// </summary>
	  public static FileValueBuilder fileValue(string filename)
	  {
		return fileValue(filename, false);
	  }

	  /// <summary>
	  /// Returns a builder to create a new <seealso cref="FileValue"/> with the given
	  /// {@code filename}.
	  /// </summary>
	  public static FileValueBuilder fileValue(string filename, bool isTransient)
	  {
		return (new FileValueBuilderImpl(filename)).setTransient(isTransient);
	  }

	  /// <summary>
	  /// Shortcut for calling {@code Variables.fileValue(name).file(file).mimeType(type).create()}.
	  /// The name is set to the file name and the mime type is detected via <seealso cref="MimetypesFileTypeMap"/>.
	  /// </summary>
	  public static FileValue fileValue(File file)
	  {
		string contentType = MimetypesFileTypeMap.DefaultFileTypeMap.getContentType(file);
		return (new FileValueBuilderImpl(file.Name)).file(file).mimeType(contentType).create();
	  }

	  /// <summary>
	  /// Shortcut for calling {@code Variables.fileValue(name).file(file).mimeType(type).setTransient(isTransient).create()}.
	  /// The name is set to the file name and the mime type is detected via <seealso cref="MimetypesFileTypeMap"/>.
	  /// </summary>
	  public static FileValue fileValue(File file, bool isTransient)
	  {
		string contentType = MimetypesFileTypeMap.DefaultFileTypeMap.getContentType(file);
		return (new FileValueBuilderImpl(file.Name)).file(file).mimeType(contentType).setTransient(isTransient).create();
	  }

	  /// <returns> an empty <seealso cref="VariableContext"/> (from which no variables can be resolved). </returns>
	  public static VariableContext emptyVariableContext()
	  {
		return EmptyVariableContext.INSTANCE;
	  }

	}

}