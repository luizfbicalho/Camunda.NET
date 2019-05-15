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
	/// Util class for comparisons.
	/// 
	/// @author Filip Hrisafov
	/// </summary>
	public class CompareUtil
	{

	  /// <summary>
	  /// Checks if any of the values are not in an ascending order. The check is done based on the <seealso cref="Comparable#compareTo(Object)"/> method.
	  /// 
	  /// E.g. if we have {@code minPriority = 10}, {@code priority = 13} and {@code maxPriority = 5} and
	  /// {@code Integer[] values = {minPriority, priority, maxPriority}}. Then a call to <seealso cref="CompareUtil#areNotInAscendingOrder(Comparable[] values)"/>
	  /// will return {@code true}
	  /// </summary>
	  /// <param name="values"> to validate </param>
	  /// @param <T> the type of the comparable </param>
	  /// <returns> {@code false} if the not null values are in an ascending order or all the values are null, {@code true} otherwise </returns>
	  public static bool areNotInAscendingOrder<T>(params T[] values) where T : IComparable<T>
	  {
		bool excluding = false;
		if (values != null)
		{
		  excluding = areNotInAscendingOrder(Arrays.asList(values));
		}
		return excluding;
	  }

	  /// <summary>
	  /// Checks if any of the values are not in an ascending order. The check is done based on the <seealso cref="Comparable#compareTo(Object)"/> method.
	  /// 
	  /// E.g. if we have {@code minPriority = 10}, {@code priority = 13} and {@code maxPriority = 5} and
	  /// {@code List<Integer> values = {minPriority, priority, maxPriority}}. Then a call to <seealso cref="CompareUtil#areNotInAscendingOrder(List values)"/>
	  /// will return {@code true}
	  /// </summary>
	  /// <param name="values"> to validate </param>
	  /// @param <T> the type of the comparable </param>
	  /// <returns> {@code false} if the not null values are in an ascending order or all the values are null, {@code true} otherwise </returns>
	  public static bool areNotInAscendingOrder<T>(IList<T> values) where T : IComparable<T>
	  {

		int lastNotNull = -1;
		for (int i = 0; i < values.Count; i++)
		{
		  T value = values[i];

		  if (value != default(T))
		  {
			if (lastNotNull != -1 && values[lastNotNull].compareTo(value) > 0)
			{
			  return true;
			}

			lastNotNull = i;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Checks if the element is not contained within the list of values. If the element, or the list are null then true is returned.
	  /// </summary>
	  /// <param name="element"> to check </param>
	  /// <param name="values"> to check in </param>
	  /// @param <T> the type of the element </param>
	  /// <returns> {@code true} if the element and values are not {@code null} and the values does not contain the element, {@code false} otherwise </returns>
	  public static bool elementIsNotContainedInList<T>(T element, ICollection<T> values)
	  {
		if (element != default(T) && values != null)
		{
		  return !values.Contains(element);
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Checks if the element is contained within the list of values. If the element, or the list are null then true is returned.
	  /// </summary>
	  /// <param name="element"> to check </param>
	  /// <param name="values"> to check in </param>
	  /// @param <T> the type of the element </param>
	  /// <returns> {@code true} if the element and values are not {@code null} and the values does not contain the element, {@code false} otherwise </returns>
	  public static bool elementIsNotContainedInArray<T>(T element, params T[] values)
	  {
		if (element != default(T) && values != null)
		{
		  return elementIsNotContainedInList(element, Arrays.asList(values));
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Checks if the element is contained within the list of values.
	  /// </summary>
	  /// <param name="element"> to check </param>
	  /// <param name="values"> to check in </param>
	  /// @param <T> the type of the element </param>
	  /// <returns> {@code true} if the element and values are not {@code null} and the values contain the element,
	  ///   {@code false} otherwise </returns>
	  public static bool elementIsContainedInList<T>(T element, ICollection<T> values)
	  {
		if (element != default(T) && values != null)
		{
		  return values.Contains(element);
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Checks if the element is contained within the list of values.
	  /// </summary>
	  /// <param name="element"> to check </param>
	  /// <param name="values"> to check in </param>
	  /// @param <T> the type of the element </param>
	  /// <returns> {@code true} if the element and values are not {@code null} and the values contain the element,
	  ///   {@code false} otherwise </returns>
	  public static bool elementIsContainedInArray<T>(T element, params T[] values)
	  {
		if (element != default(T) && values != null)
		{
		  return elementIsContainedInList(element, Arrays.asList(values));
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Returns any element if obj1.compareTo(obj2) == 0
	  /// </summary>
	  public static T min<T>(T obj1, T obj2) where T : IComparable<T>
	  {
		return obj1.compareTo(obj2) <= 0 ? obj1 : obj2;
	  }

	  /// <summary>
	  /// Returns any element if obj1.compareTo(obj2) == 0
	  /// </summary>
	  public static T max<T>(T obj1, T obj2) where T : IComparable<T>
	  {
		return obj1.compareTo(obj2) >= 0 ? obj1 : obj2;
	  }
	}

}