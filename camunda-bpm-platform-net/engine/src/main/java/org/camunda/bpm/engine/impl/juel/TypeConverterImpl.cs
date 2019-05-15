using System;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.engine.impl.juel
{

	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;

	/// <summary>
	/// Type Conversions as described in EL 2.1 specification (section 1.17).
	/// </summary>
	[Serializable]
	public class TypeConverterImpl : TypeConverter
	{
		private const long serialVersionUID = 1L;

		protected internal virtual bool? coerceToBoolean(object value)
		{
			if (value == null || "".Equals(value))
			{
				return false;
			}
			if (value is bool?)
			{
				return (bool?)value;
			}
			if (value is string)
			{
				return Convert.ToBoolean((string)value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Boolean)));
		}

		protected internal virtual char? coerceToCharacter(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToChar((char)0);
			}
			if (value is char?)
			{
				return (char?)value;
			}
			if (value is Number)
			{
				return Convert.ToChar((char)((Number)value).shortValue());
			}
			if (value is string)
			{
				return Convert.ToChar(((string)value)[0]);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Character)));
		}

		protected internal virtual decimal coerceToBigDecimal(object value)
		{
			if (value == null || "".Equals(value))
			{
				return decimal.valueOf(0l);
			}
			if (value is decimal)
			{
				return (decimal)value;
			}
			if (value is System.Numerics.BigInteger)
			{
				return new decimal((System.Numerics.BigInteger)value);
			}
			if (value is Number)
			{
				return new decimal(((Number)value).doubleValue());
			}
			if (value is string)
			{
				try
				{
					return new decimal((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(decimal)));
				}
			}
			if (value is char?)
			{
				return new decimal((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(decimal)));
		}

		protected internal virtual System.Numerics.BigInteger coerceToBigInteger(object value)
		{
			if (value == null || "".Equals(value))
			{
				return System.Numerics.BigInteger.valueOf(0l);
			}
			if (value is System.Numerics.BigInteger)
			{
				return (System.Numerics.BigInteger)value;
			}
			if (value is decimal)
			{
				return ((decimal)value).toBigInteger();
			}
			if (value is Number)
			{
				return System.Numerics.BigInteger.valueOf(((Number)value).longValue());
			}
			if (value is string)
			{
				try
				{
					return new System.Numerics.BigInteger((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(System.Numerics.BigInteger)));
				}
			}
			if (value is char?)
			{
				return System.Numerics.BigInteger.valueOf((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(System.Numerics.BigInteger)));
		}

		protected internal virtual double? coerceToDouble(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToDouble(0);
			}
			if (value is double?)
			{
				return (double?)value;
			}
			if (value is Number)
			{
				return Convert.ToDouble(((Number)value).doubleValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToDouble((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Double)));
				}
			}
			if (value is char?)
			{
				return Convert.ToDouble((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Double)));
		}

		protected internal virtual float? coerceToFloat(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToSingle(0);
			}
			if (value is float?)
			{
				return (float?)value;
			}
			if (value is Number)
			{
				return Convert.ToSingle(((Number)value).floatValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToSingle((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Float)));
				}
			}
			if (value is char?)
			{
				return Convert.ToSingle((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Float)));
		}

		protected internal virtual long? coerceToLong(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToInt64(0l);
			}
			if (value is long?)
			{
				return (long?)value;
			}
			if (value is Number)
			{
				return Convert.ToInt64(((Number)value).longValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToInt64((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Long)));
				}
			}
			if (value is char?)
			{
				return Convert.ToInt64((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Long)));
		}

		protected internal virtual int? coerceToInteger(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToInt32(0);
			}
			if (value is int?)
			{
				return (int?)value;
			}
			if (value is Number)
			{
				return Convert.ToInt32(((Number)value).intValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToInt32((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Integer)));
				}
			}
			if (value is char?)
			{
				return Convert.ToInt32((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Integer)));
		}

		protected internal virtual short? coerceToShort(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToInt16((short)0);
			}
			if (value is short?)
			{
				return (short?)value;
			}
			if (value is Number)
			{
				return Convert.ToInt16(((Number)value).shortValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToInt16((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Short)));
				}
			}
			if (value is char?)
			{
				return Convert.ToInt16((short)((char?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Short)));
		}

		protected internal virtual sbyte? coerceToByte(object value)
		{
			if (value == null || "".Equals(value))
			{
				return Convert.ToSByte((sbyte)0);
			}
			if (value is sbyte?)
			{
				return (sbyte?)value;
			}
			if (value is Number)
			{
				return Convert.ToSByte(((Number)value).byteValue());
			}
			if (value is string)
			{
				try
				{
					return Convert.ToSByte((string)value);
				}
				catch (System.FormatException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, typeof(Byte)));
				}
			}
			if (value is char?)
			{
				return Convert.ToSByte(Convert.ToInt16((short)((char?)value).Value).byteValue());
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), typeof(Byte)));
		}

		protected internal virtual string coerceToString(object value)
		{
			if (value == null)
			{
				return "";
			}
			if (value is string)
			{
				return (string)value;
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (value instanceof Enum<?>)
			if (value is Enum<object>)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: return ((Enum<?>)value).name();
				return ((Enum<object>)value).name();
			}
			return value.ToString();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends Enum<T>> T coerceToEnum(Object value, Class<T> type)
		protected internal virtual T coerceToEnum<T>(object value, Type<T> type) where T : Enum<T>
		{
			if (value == null || "".Equals(value))
			{
				return null;
			}
			if (type.IsInstanceOfType(value))
			{
				return (T)value;
			}
			if (value is string)
			{
				try
				{
					return Enum.valueOf(type, (string)value);
				}
				catch (System.ArgumentException)
				{
					throw new ELException(LocalMessages.get("error.coerce.value", value, type));
				}
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), type));
		}

		protected internal virtual object coerceStringToType(string value, Type type)
		{
			PropertyEditor editor = PropertyEditorManager.findEditor(type);
			if (editor == null)
			{
				if ("".Equals(value))
				{
					return null;
				}
				throw new ELException(LocalMessages.get("error.coerce.type", typeof(string), type));
			}
			else
			{
				if ("".Equals(value))
				{
					try
					{
						editor.AsText = value;
					}
					catch (System.ArgumentException)
					{
						return null;
					}
				}
				else
				{
					try
					{
						editor.AsText = value;
					}
					catch (System.ArgumentException)
					{
						throw new ELException(LocalMessages.get("error.coerce.value", value, type));
					}
				}
				return editor.Value;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected Object coerceToType(Object value, Class type)
		protected internal virtual object coerceToType(object value, Type type)
		{
			if (type == typeof(string))
			{
				return coerceToString(value);
			}
			if (type == typeof(Long) || type == typeof(long))
			{
				return coerceToLong(value);
			}
			if (type == typeof(Double) || type == typeof(double))
			{
				return coerceToDouble(value);
			}
			if (type == typeof(Boolean) || type == typeof(bool))
			{
				return coerceToBoolean(value);
			}
			if (type == typeof(Integer) || type == typeof(int))
			{
				return coerceToInteger(value);
			}
			if (type == typeof(Float) || type == typeof(float))
			{
				return coerceToFloat(value);
			}
			if (type == typeof(Short) || type == typeof(short))
			{
				return coerceToShort(value);
			}
			if (type == typeof(Byte) || type == typeof(sbyte))
			{
				return coerceToByte(value);
			}
			if (type == typeof(Character) || type == typeof(char))
			{
				return coerceToCharacter(value);
			}
			if (type == typeof(decimal))
			{
				return coerceToBigDecimal(value);
			}
			if (type == typeof(System.Numerics.BigInteger))
			{
				return coerceToBigInteger(value);
			}
			if (type.BaseType == typeof(Enum))
			{
				return coerceToEnum(value, (Type)type);
			}
			if (value == null || value.GetType() == type || type.IsInstanceOfType(value))
			{
				return value;
			}
			if (value is string)
			{
				return coerceStringToType((string)value, type);
			}
			throw new ELException(LocalMessages.get("error.coerce.type", value.GetType(), type));
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType().Equals(this.GetType());
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T convert(Object value, Class<T> type) throws org.camunda.bpm.engine.impl.javax.el.ELException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual T convert<T>(object value, Type<T> type)
		{
			return (T)coerceToType(value, type);
		}
	}

}