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
namespace org.camunda.bpm.engine.variable.impl.type
{

	using DoubleValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.DoubleValueImpl;
	using IntegerValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.IntegerValueImpl;
	using LongValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.LongValueImpl;
	using ShortValueImpl = org.camunda.bpm.engine.variable.impl.value.PrimitiveTypeValueImpl.ShortValueImpl;
	using PrimitiveValueType = org.camunda.bpm.engine.variable.type.PrimitiveValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using DateValue = org.camunda.bpm.engine.variable.value.DateValue;
	using DoubleValue = org.camunda.bpm.engine.variable.value.DoubleValue;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;
	using NumberValue = org.camunda.bpm.engine.variable.value.NumberValue;
	using ShortValue = org.camunda.bpm.engine.variable.value.ShortValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Implementation of the primitive variable value types
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public abstract class PrimitiveValueTypeImpl : AbstractValueTypeImpl, PrimitiveValueType
	{
		public override abstract TypedValue createValue(object value, IDictionary<string, object> valueInfo);

	  private const long serialVersionUID = 1L;

	  protected internal Type javaType;

	  public PrimitiveValueTypeImpl(Type javaType) : this(javaType.Name.ToLower(), javaType)
	  {
	  }

	  public PrimitiveValueTypeImpl(string name, Type javaType) : base(name)
	  {
		this.javaType = javaType;
	  }

	  public virtual Type JavaType
	  {
		  get
		  {
			return javaType;
		  }
	  }

	  public override bool PrimitiveValueType
	  {
		  get
		  {
			return true;
		  }
	  }

	  public override string ToString()
	  {
		return "PrimitiveValueType[" + Name + "]";
	  }

	  public override IDictionary<string, object> getValueInfo(TypedValue typedValue)
	  {
		IDictionary<string, object> result = new Dictionary<string, object>();
		if (typedValue.Transient)
		{
		  result[org.camunda.bpm.engine.variable.type.ValueType_Fields.VALUE_INFO_TRANSIENT] = typedValue.Transient;
		}
		return result;
	  }

	  // concrete types ///////////////////////////////////////////////////

	  [Serializable]
	  public class BooleanTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public BooleanTypeImpl() : base(typeof(Boolean))
		{
		}

		public override BooleanValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.booleanValue((bool?) value, isTransient(valueInfo).Value);
		}

	  }

	  [Serializable]
	  public class BytesTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public BytesTypeImpl() : base("bytes", typeof(sbyte[]))
		{
		}

		public override BytesValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.byteArrayValue((sbyte[]) value, isTransient(valueInfo).Value);
		}

	  }

	  [Serializable]
	  public class DateTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public DateTypeImpl() : base(typeof(DateTime))
		{
		}

		public override DateValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.dateValue((DateTime) value, isTransient(valueInfo).Value);
		}

	  }

	  [Serializable]
	  public class DoubleTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public DoubleTypeImpl() : base(typeof(Double))
		{
		}

		public override DoubleValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.doubleValue((double?) value, isTransient(valueInfo).Value);
		}

		public override ValueType Parent
		{
			get
			{
			  return org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER;
			}
		}

		public override bool canConvertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			return false;
		  }

		  return true;
		}

		public override DoubleValue convertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			throw unsupportedConversion(typedValue.Type);
		  }
		  DoubleValueImpl doubleValue = null;
		  NumberValue numberValue = (NumberValue) typedValue;
		  if (numberValue.Value != null)
		  {
			doubleValue = (DoubleValueImpl) Variables.doubleValue(numberValue.Value.doubleValue());
		  }
		  else
		  {
			doubleValue = (DoubleValueImpl) Variables.doubleValue(null);
		  }
		  doubleValue.Transient = numberValue.Transient;
		  return doubleValue;
		}
	  }

	  [Serializable]
	  public class IntegerTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public IntegerTypeImpl() : base(typeof(Integer))
		{
		}

		public override IntegerValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.integerValue((int?) value, isTransient(valueInfo).Value);
		}

		public override ValueType Parent
		{
			get
			{
			  return org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER;
			}
		}

		public override bool canConvertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			return false;
		  }

		  if (typedValue.Value != null)
		  {
			NumberValue numberValue = (NumberValue) typedValue;
			double doubleValue = numberValue.Value.doubleValue();

			// returns false if the value changes due to conversion (e.g. by overflows
			// or by loss in precision)
			if (numberValue.Value.intValue() != doubleValue)
			{
			  return false;
			}
		  }

		  return true;
		}

		public override IntegerValue convertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			throw unsupportedConversion(typedValue.Type);
		  }

		  IntegerValueImpl integerValue = null;
		  NumberValue numberValue = (NumberValue) typedValue;
		  if (numberValue.Value != null)
		  {
			integerValue = (IntegerValueImpl) Variables.integerValue(numberValue.Value.intValue());
		  }
		  else
		  {
			integerValue = (IntegerValueImpl) Variables.integerValue(null);
		  }
		  integerValue.Transient = numberValue.Transient;
		  return integerValue;
		}
	  }

	  [Serializable]
	  public class LongTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public LongTypeImpl() : base(typeof(Long))
		{
		}

		public override LongValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.longValue((long?) value, isTransient(valueInfo).Value);
		}

		public override ValueType Parent
		{
			get
			{
			  return org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER;
			}
		}

		public override bool canConvertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			return false;
		  }

		  if (typedValue.Value != null)
		  {
			NumberValue numberValue = (NumberValue) typedValue;
			double doubleValue = numberValue.Value.doubleValue();

			// returns false if the value changes due to conversion (e.g. by overflows
			// or by loss in precision)
			if (numberValue.Value.longValue() != doubleValue)
			{
			  return false;
			}
		  }

		  return true;
		}

		public override LongValue convertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			throw unsupportedConversion(typedValue.Type);
		  }

		  LongValueImpl longvalue = null;
		  NumberValue numberValue = (NumberValue) typedValue;

		  if (numberValue.Value != null)
		  {
			longvalue = (LongValueImpl) Variables.longValue(numberValue.Value.longValue());
		  }
		  else
		  {
			longvalue = (LongValueImpl) Variables.longValue(null);
		  }
		  longvalue.Transient = numberValue.Transient;
		  return longvalue;
		}
	  }

	  [Serializable]
	  public class NullTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public NullTypeImpl() : base("null", typeof(NullType))
		{
		}

		public override TypedValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.untypedNullValue(isTransient(valueInfo).Value);
		}

	  }

	  [Serializable]
	  public class ShortTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public ShortTypeImpl() : base(typeof(Short))
		{
		}

		public override ShortValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.shortValue((short?) value, isTransient(valueInfo).Value);
		}

		public override ValueType Parent
		{
			get
			{
			  return org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER;
			}
		}

		public override ShortValue convertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			throw unsupportedConversion(typedValue.Type);
		  }

		  ShortValueImpl shortValue = null;
		  NumberValue numberValue = (NumberValue) typedValue;
		  if (numberValue.Value != null)
		  {
			shortValue = (ShortValueImpl) Variables.shortValue(numberValue.Value.shortValue());
		  }
		  else
		  {
			shortValue = (ShortValueImpl) Variables.shortValue(null);
		  }
		  shortValue.Transient = numberValue.Transient;
		  return shortValue;
		}

		public override bool canConvertFromTypedValue(TypedValue typedValue)
		{
		  if (typedValue.Type != org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		  {
			return false;
		  }

		  if (typedValue.Value != null)
		  {
			NumberValue numberValue = (NumberValue) typedValue;
			double doubleValue = numberValue.Value.doubleValue();

			// returns false if the value changes due to conversion (e.g. by overflows
			// or by loss in precision)
			if (numberValue.Value.shortValue() != doubleValue)
			{
			  return false;
			}
		  }

		  return true;
		}
	  }

	  [Serializable]
	  public class StringTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public StringTypeImpl() : base(typeof(string))
		{
		}

		public override StringValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.stringValue((string) value, isTransient(valueInfo).Value);
		}
	  }

	  [Serializable]
	  public class NumberTypeImpl : PrimitiveValueTypeImpl
	  {

		internal const long serialVersionUID = 1L;

		public NumberTypeImpl() : base(typeof(Number))
		{
		}

		public override NumberValue createValue(object value, IDictionary<string, object> valueInfo)
		{
		  return Variables.numberValue((Number) value, isTransient(valueInfo).Value);
		}

		public override bool Abstract
		{
			get
			{
			  return true;
			}
		}
	  }


	}

}