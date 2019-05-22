using System;
using System.Collections.Generic;
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

	public class BooleanOperations
	{
		private static readonly ISet<Type> SIMPLE_INTEGER_TYPES = new HashSet<Type>();
		private static readonly ISet<Type> SIMPLE_FLOAT_TYPES = new HashSet<Type>();

		static BooleanOperations()
		{
			SIMPLE_INTEGER_TYPES.Add(typeof(Byte));
			SIMPLE_INTEGER_TYPES.Add(typeof(Short));
			SIMPLE_INTEGER_TYPES.Add(typeof(Integer));
			SIMPLE_INTEGER_TYPES.Add(typeof(Long));
			SIMPLE_FLOAT_TYPES.Add(typeof(Float));
			SIMPLE_FLOAT_TYPES.Add(typeof(Double));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static final boolean lt0(TypeConverter converter, Object o1, Object o2)
		private static bool lt0(TypeConverter converter, object o1, object o2)
		{
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsAssignableFrom(typeof(decimal)) || t2.IsAssignableFrom(typeof(decimal)))
			{
				return converter.convert(o1, typeof(decimal)).compareTo(converter.convert(o2, typeof(decimal))) < 0;
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Double)) < converter.convert(o2, typeof(Double));
			}
			if (t1.IsAssignableFrom(typeof(BigInteger)) || t2.IsAssignableFrom(typeof(BigInteger)))
			{
				return converter.convert(o1, typeof(BigInteger)).compareTo(converter.convert(o2, typeof(BigInteger))) < 0;
			}
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Long)) < converter.convert(o2, typeof(Long));
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.convert(o1, typeof(string)).compareTo(converter.convert(o2, typeof(string))) < 0;
			}
			if (o1 is IComparable)
			{
				return ((IComparable)o1).CompareTo(o2) < 0;
			}
			if (o2 is IComparable)
			{
				return ((IComparable)o2).CompareTo(o1) > 0;
			}
			throw new ELException(LocalMessages.get("error.compare.types", o1.GetType(), o2.GetType()));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static final boolean gt0(TypeConverter converter, Object o1, Object o2)
		private static bool gt0(TypeConverter converter, object o1, object o2)
		{
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsAssignableFrom(typeof(decimal)) || t2.IsAssignableFrom(typeof(decimal)))
			{
				return converter.convert(o1, typeof(decimal)).compareTo(converter.convert(o2, typeof(decimal))) > 0;
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Double)) > converter.convert(o2, typeof(Double));
			}
			if (t1.IsAssignableFrom(typeof(BigInteger)) || t2.IsAssignableFrom(typeof(BigInteger)))
			{
				return converter.convert(o1, typeof(BigInteger)).compareTo(converter.convert(o2, typeof(BigInteger))) > 0;
			}
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Long)) > converter.convert(o2, typeof(Long));
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.convert(o1, typeof(string)).compareTo(converter.convert(o2, typeof(string))) > 0;
			}
			if (o1 is IComparable)
			{
				return ((IComparable)o1).CompareTo(o2) > 0;
			}
			if (o2 is IComparable)
			{
				return ((IComparable)o2).CompareTo(o1) < 0;
			}
			throw new ELException(LocalMessages.get("error.compare.types", o1.GetType(), o2.GetType()));
		}

		public static bool lt(TypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return false;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return lt0(converter, o1, o2);
		}

		public static bool gt(TypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return false;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return gt0(converter, o1, o2);
		}

		public static bool ge(TypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return !lt0(converter, o1, o2);
		}

		public static bool le(TypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return !gt0(converter, o1, o2);
		}

		public static bool eq(TypeConverter converter, object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			Type t1 = o1.GetType();
			Type t2 = o2.GetType();
			if (t1.IsAssignableFrom(typeof(decimal)) || t2.IsAssignableFrom(typeof(decimal)))
			{
				return converter.convert(o1, typeof(decimal)).Equals(converter.convert(o2, typeof(decimal)));
			}
			if (SIMPLE_FLOAT_TYPES.Contains(t1) || SIMPLE_FLOAT_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Double)).Equals(converter.convert(o2, typeof(Double)));
			}
			if (t1.IsAssignableFrom(typeof(BigInteger)) || t2.IsAssignableFrom(typeof(BigInteger)))
			{
				return converter.convert(o1, typeof(BigInteger)).Equals(converter.convert(o2, typeof(BigInteger)));
			}
			if (SIMPLE_INTEGER_TYPES.Contains(t1) || SIMPLE_INTEGER_TYPES.Contains(t2))
			{
				return converter.convert(o1, typeof(Long)).Equals(converter.convert(o2, typeof(Long)));
			}
			if (t1 == typeof(Boolean) || t2 == typeof(Boolean))
			{
				return converter.convert(o1, typeof(Boolean)).Equals(converter.convert(o2, typeof(Boolean)));
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (o1 instanceof Enum<?>)
			if (o1 is Enum<object>)
			{
				return o1 == converter.convert(o2, o1.GetType());
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (o2 instanceof Enum<?>)
			if (o2 is Enum<object>)
			{
				return converter.convert(o1, o2.GetType()) == o2;
			}
			if (t1 == typeof(string) || t2 == typeof(string))
			{
				return converter.convert(o1, typeof(string)).Equals(converter.convert(o2, typeof(string)));
			}
			return o1.Equals(o2);
		}

		public static bool ne(TypeConverter converter, object o1, object o2)
		{
			return !eq(converter, o1, o2);
		}

		public static bool empty(TypeConverter converter, object o)
		{
			if (o == null || "".Equals(o))
			{
				return true;
			}
			if (o is object[])
			{
				return ((object[])o).Length == 0;
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (o instanceof java.util.Map<?,?>)
			if (o is IDictionary<object, ?>)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: return ((java.util.Map<?,?>)o).isEmpty();
				return ((IDictionary<object, ?>)o).Count == 0;
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (o instanceof java.util.Collection<?>)
			if (o is ICollection<object>)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: return ((java.util.Collection<?>)o).isEmpty();
				return ((ICollection<object>)o).Count == 0;
			}
			return false;
		}
	}

}