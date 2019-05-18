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
namespace org.camunda.bpm.engine.variable.impl.value
{

	using PrimitiveValueType = org.camunda.bpm.engine.variable.type.PrimitiveValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using DateValue = org.camunda.bpm.engine.variable.value.DateValue;
	using DoubleValue = org.camunda.bpm.engine.variable.value.DoubleValue;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;
	using NumberValue = org.camunda.bpm.engine.variable.value.NumberValue;
	using PrimitiveValue = org.camunda.bpm.engine.variable.value.PrimitiveValue;
	using ShortValue = org.camunda.bpm.engine.variable.value.ShortValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class PrimitiveTypeValueImpl<T> : AbstractTypedValue<T>, PrimitiveValue<T>
	{

	  private const long serialVersionUID = 1L;

	  public PrimitiveTypeValueImpl(T value, PrimitiveValueType type) : base(value, type)
	  {
	  }

	  public override PrimitiveValueType Type
	  {
		  get
		  {
			return (PrimitiveValueType) base.Type;
		  }
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((type == null) ? 0 : type.GetHashCode());
		result = prime * result + ((value == default(T)) ? 0 : value.GetHashCode());
		result = prime * result + (isTransient ? 1 : 0);
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: PrimitiveTypeValueImpl<?> other = (PrimitiveTypeValueImpl<?>) obj;
		PrimitiveTypeValueImpl<object> other = (PrimitiveTypeValueImpl<object>) obj;
		if (type == null)
		{
		  if (other.type != null)
		  {
			return false;
		  }
		}
		else if (!type.Equals(other.type))
		{
		  return false;
		}
		if (value == default(T))
		{
		  if (other.value != default(T))
		  {
			return false;
		  }
		}
		else if (!value.Equals(other.value))
		{
		  return false;
		}
		if (isTransient != other.Transient)
		{
		  return false;
		}
		return true;
	  }


	  // value type implementations ////////////////////////////////////

	  [Serializable]
	  public class BooleanValueImpl : PrimitiveTypeValueImpl<bool>, BooleanValue
	  {

		internal const long serialVersionUID = 1L;

		public BooleanValueImpl(bool? value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.BOOLEAN)
		{
		}

		public BooleanValueImpl(bool? value, bool isTransient) : this(value)
		{
			this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class BytesValueImpl : PrimitiveTypeValueImpl<sbyte[]>, BytesValue
	  {

		internal const long serialVersionUID = 1L;

		public BytesValueImpl(sbyte[] value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.BYTES)
		{
		}

		public BytesValueImpl(sbyte[] value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class DateValueImpl : PrimitiveTypeValueImpl<DateTime>, DateValue
	  {

		internal const long serialVersionUID = 1L;

		public DateValueImpl(DateTime value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.DATE)
		{
		}

		public DateValueImpl(DateTime value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class DoubleValueImpl : PrimitiveTypeValueImpl<double>, DoubleValue
	  {

		internal const long serialVersionUID = 1L;

		public DoubleValueImpl(double? value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.DOUBLE)
		{
		}

		public DoubleValueImpl(double? value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class IntegerValueImpl : PrimitiveTypeValueImpl<int>, IntegerValue
	  {

		internal const long serialVersionUID = 1L;

		public IntegerValueImpl(int? value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.INTEGER)
		{
		}

		public IntegerValueImpl(int? value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class LongValueImpl : PrimitiveTypeValueImpl<long>, LongValue
	  {

		internal const long serialVersionUID = 1L;

		public LongValueImpl(long? value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.LONG)
		{
		}

		public LongValueImpl(long? value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class ShortValueImpl : PrimitiveTypeValueImpl<short>, ShortValue
	  {

		internal const long serialVersionUID = 1L;

		public ShortValueImpl(short? value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.SHORT)
		{
		}

		public ShortValueImpl(short? value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class StringValueImpl : PrimitiveTypeValueImpl<string>, StringValue
	  {

		internal const long serialVersionUID = 1L;

		public StringValueImpl(string value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.STRING)
		{
		}

		public StringValueImpl(string value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	  [Serializable]
	  public class NumberValueImpl : PrimitiveTypeValueImpl<Number>, NumberValue
	  {

		internal const long serialVersionUID = 1L;

		public NumberValueImpl(Number value) : base(value, org.camunda.bpm.engine.variable.type.ValueType_Fields.NUMBER)
		{
		}

		public NumberValueImpl(Number value, bool isTransient) : this(value)
		{
		  this.isTransient = isTransient;
		}
	  }

	}

}