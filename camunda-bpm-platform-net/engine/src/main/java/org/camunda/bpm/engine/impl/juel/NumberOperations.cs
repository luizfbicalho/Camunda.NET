using System;
using System.Numerics;

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
	/// Arithmetic Operations as specified in chapter 1.7.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class NumberOperations
	{
		private static readonly long? LONG_ZERO = Convert.ToInt64(0L);

		private static bool isDotEe(string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				switch (value[i])
				{
					case '.':
					case 'E':
					case 'e':
						return true;
				}
			}
			return false;
		}

		private static bool isDotEe(object value)
		{
			return value is string && isDotEe((string)value);
		}

		private static bool isFloatOrDouble(object value)
		{
			return value is float? || value is double?;
		}

		private static bool isFloatOrDoubleOrDotEe(object value)
		{
			return isFloatOrDouble(value) || isDotEe(value);
		}

		private static bool isBigDecimalOrBigInteger(object value)
		{
			return value is decimal || value is BigInteger;
		}

		private static bool isBigDecimalOrFloatOrDoubleOrDotEe(object value)
		{
			return value is decimal || isFloatOrDoubleOrDotEe(value);
		}

		public static Number add(TypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return converter.convert(o1, typeof(decimal)).add(converter.convert(o2, typeof(decimal)));
			}
			if (isFloatOrDoubleOrDotEe(o1) || isFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is BigInteger || o2 is BigInteger)
				{
					return converter.convert(o1, typeof(decimal)).add(converter.convert(o2, typeof(decimal)));
				}
				return converter.convert(o1, typeof(Double)) + converter.convert(o2, typeof(Double));
			}
			if (o1 is BigInteger || o2 is BigInteger)
			{
				return converter.convert(o1, typeof(BigInteger)).add(converter.convert(o2, typeof(BigInteger)));
			}
			return converter.convert(o1, typeof(Long)) + converter.convert(o2, typeof(Long));
		}

		public static Number sub(TypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return converter.convert(o1, typeof(decimal)).subtract(converter.convert(o2, typeof(decimal)));
			}
			if (isFloatOrDoubleOrDotEe(o1) || isFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is BigInteger || o2 is BigInteger)
				{
					return converter.convert(o1, typeof(decimal)).subtract(converter.convert(o2, typeof(decimal)));
				}
				return converter.convert(o1, typeof(Double)) - converter.convert(o2, typeof(Double));
			}
			if (o1 is BigInteger || o2 is BigInteger)
			{
				return converter.convert(o1, typeof(BigInteger)).subtract(converter.convert(o2, typeof(BigInteger)));
			}
			return converter.convert(o1, typeof(Long)) - converter.convert(o2, typeof(Long));
		}

		public static Number mul(TypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (o1 is decimal || o2 is decimal)
			{
				return converter.convert(o1, typeof(decimal)).multiply(converter.convert(o2, typeof(decimal)));
			}
			if (isFloatOrDoubleOrDotEe(o1) || isFloatOrDoubleOrDotEe(o2))
			{
				if (o1 is BigInteger || o2 is BigInteger)
				{
					return converter.convert(o1, typeof(decimal)).multiply(converter.convert(o2, typeof(decimal)));
				}
				return converter.convert(o1, typeof(Double)) * converter.convert(o2, typeof(Double));
			}
			if (o1 is BigInteger || o2 is BigInteger)
			{
				return converter.convert(o1, typeof(BigInteger)).multiply(converter.convert(o2, typeof(BigInteger)));
			}
			return converter.convert(o1, typeof(Long)) * converter.convert(o2, typeof(Long));
		}

		public static Number div(TypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (isBigDecimalOrBigInteger(o1) || isBigDecimalOrBigInteger(o2))
			{
				return converter.convert(o1, typeof(decimal)).divide(converter.convert(o2, typeof(decimal)), decimal.ROUND_HALF_UP);
			}
			return converter.convert(o1, typeof(Double)) / converter.convert(o2, typeof(Double));
		}

		public static Number mod(TypeConverter converter, object o1, object o2)
		{
			if (o1 == null && o2 == null)
			{
				return LONG_ZERO;
			}
			if (isBigDecimalOrFloatOrDoubleOrDotEe(o1) || isBigDecimalOrFloatOrDoubleOrDotEe(o2))
			{
				return converter.convert(o1, typeof(Double)) % converter.convert(o2, typeof(Double));
			}
			if (o1 is BigInteger || o2 is BigInteger)
			{
				return converter.convert(o1, typeof(BigInteger)).remainder(converter.convert(o2, typeof(BigInteger)));
			}
			return converter.convert(o1, typeof(Long)) % converter.convert(o2, typeof(Long));
		}

		public static Number neg(TypeConverter converter, object value)
		{
			if (value == null)
			{
				return LONG_ZERO;
			}
			if (value is decimal)
			{
				return -((decimal)value);
			}
			if (value is BigInteger)
			{
				return -((BigInteger)value);
			}
			if (value is double?)
			{
				return Convert.ToDouble(-((double?)value).Value);
			}
			if (value is float?)
			{
				return Convert.ToSingle(-((float?)value).Value);
			}
			if (value is string)
			{
				if (isDotEe((string)value))
				{
					return Convert.ToDouble(-converter.convert(value, typeof(Double)).doubleValue());
				}
				return Convert.ToInt64(-converter.convert(value, typeof(Long)).longValue());
			}
			if (value is long?)
			{
				return Convert.ToInt64(-((long?)value).Value);
			}
			if (value is int?)
			{
				return Convert.ToInt32(-((int?)value).Value);
			}
			if (value is short?)
			{
				return Convert.ToInt16((short)-((short?)value).Value);
			}
			if (value is sbyte?)
			{
				return Convert.ToSByte((sbyte)-((sbyte?)value).Value);
			}
			throw new ELException(LocalMessages.get("error.negate", value.GetType()));
		}
	}

}