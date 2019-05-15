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
namespace org.camunda.bpm.engine.impl.util
{

	/// <summary>
	/// helper/convience methods for working with collections.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class CollectionUtil
	{

	  // No need to instantiate
	  private CollectionUtil()
	  {
	  }

	  /// <summary>
	  /// Helper method that creates a singleton map.
	  /// 
	  /// Alternative for Collections.singletonMap(), since that method returns a
	  /// generic typed map <K,T> depending on the input type, but we often need a
	  /// <String, Object> map.
	  /// </summary>
	  public static IDictionary<string, object> singletonMap(string key, object value)
	  {
		IDictionary<string, object> map = new Dictionary<string, object>();
		map[key] = value;
		return map;
	  }

	  /// <summary>
	  /// Arrays.asList cannot be reliably used for SQL parameters on MyBatis < 3.3.0
	  /// </summary>
	  public static IList<T> asArrayList<T>(T[] values)
	  {
		List<T> result = new List<T>();
		Collections.addAll(result, values);

		return result;
	  }

	  public static ISet<T> asHashSet<T>(params T[] elements)
	  {
		ISet<T> set = new HashSet<T>();
		Collections.addAll(set, elements);

		return set;
	  }

	  public static void addToMapOfLists<S, T>(IDictionary<S, IList<T>> map, S key, T value)
	  {
		IList<T> list = map[key];
		if (list == null)
		{
		  list = new List<T>();
		  map[key] = list;
		}
		list.Add(value);
	  }

	  public static void addToMapOfSets<S, T>(IDictionary<S, ISet<T>> map, S key, T value)
	  {
		ISet<T> set = map[key];
		if (set == null)
		{
		  set = new HashSet<T>();
		  map[key] = set;
		}
		set.Add(value);
	  }

	  public static void addCollectionToMapOfSets<S, T>(IDictionary<S, ISet<T>> map, S key, ICollection<T> values)
	  {
		ISet<T> set = map[key];
		if (set == null)
		{
		  set = new HashSet<T>();
		  map[key] = set;
		}
		set.addAll(values);
	  }

	  /// <summary>
	  /// Chops a list into non-view sublists of length partitionSize.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T> java.util.List<java.util.List<T>> partition(java.util.List<T> list, final int partitionSize)
	  public static IList<IList<T>> partition<T>(IList<T> list, int partitionSize)
	  {
		IList<IList<T>> parts = new List<IList<T>>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int listSize = list.size();
		int listSize = list.Count;
		for (int i = 0; i < listSize; i += partitionSize)
		{
		  parts.Add(list.GetRange(i, Math.Min(listSize, i + partitionSize)));
		}
		return parts;
	  }
	}

}