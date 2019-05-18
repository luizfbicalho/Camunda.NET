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
namespace org.camunda.commons.utils.cache
{

	/// <summary>
	/// A Map-like data structure that stores key-value pairs and provides temporary
	/// access to it.
	/// </summary>
	/// @param <K> the type of keys </param>
	/// @param <V> the type of mapped values </param>
	public interface Cache<K, V>
	{

	  /// <summary>
	  /// Gets an entry from the cache.
	  /// </summary>
	  /// <param name="key"> the key whose associated value is to be returned </param>
	  /// <returns> the element, or <code>null</code>, if it does not exist. </returns>
	  V get(K key);

	  /// <summary>
	  /// Associates the specified value with the specified key in the cache.
	  /// </summary>
	  /// <param name="key">   key with which the specified value is to be associated </param>
	  /// <param name="value"> value to be associated with the specified key </param>
	  /// <exception cref="NullPointerException"> if key is <code>null</code> or if value is <code>null</code> </exception>
	  void put(K key, V value);

	  /// <summary>
	  /// Clears the contents of the cache.
	  /// </summary>
	  void clear();

	  /// <summary>
	  /// Removes an entry from the cache.
	  /// </summary>
	  /// <param name="key"> key with which the specified value is to be associated. </param>
	  void remove(K key);

	  /// <summary>
	  /// Returns a Set view of the keys contained in this cache.
	  /// </summary>
	  ISet<K> keySet();

	  /// <returns> the current size of the cache </returns>
	  int size();

	  /// <summary>
	  /// Returns <code>true</code> if this cache contains no key-value mappings.
	  /// </summary>
	  bool Empty {get;}

	}
}