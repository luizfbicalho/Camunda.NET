using System.Collections.Generic;
using System.Collections.Concurrent;

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
	/// A thread-safe LRU <seealso cref="Cache"/> with a fixed capacity. If the cache reaches
	/// the capacity, it discards the least recently used entry first.
	/// <para>
	/// *Note*: The consistency of the keys queue with the keys in the cache is not ensured! This means, the keys queue
	/// can contain duplicates of the same key and not all the keys of the queue are necessarily in the cache.
	/// However, all the keys of the cache are at least once contained in the keys queue.
	/// 
	/// </para>
	/// </summary>
	/// @param <K> the type of keys </param>
	/// @param <V> the type of mapped values </param>
	public class ConcurrentLruCache<K, V> : Cache<K, V>
	{

	  private readonly int capacity;

	  private readonly ConcurrentMap<K, V> cache = new ConcurrentDictionary<K, V>();
	  private readonly ConcurrentLinkedQueue<K> keys = new ConcurrentLinkedQueue<K>();

	  /// <summary>
	  /// Creates the cache with a fixed capacity.
	  /// </summary>
	  /// <param name="capacity"> max number of cache entries </param>
	  /// <exception cref="IllegalArgumentException"> if capacity is negative </exception>
	  public ConcurrentLruCache(int capacity)
	  {
		if (capacity < 0)
		{
		  throw new System.ArgumentException();
		}
		this.capacity = capacity;
	  }

	  public virtual V get(K key)
	  {
		V value = cache.get(key);
		if (value != default(V))
		{
		  keys.remove(key);
		  keys.add(key);
		}
		return value;
	  }

	  public virtual void put(K key, V value)
	  {
		if (key == default(K) || value == default(V))
		{
		  throw new System.NullReferenceException();
		}

		V previousValue = cache.put(key, value);
		if (previousValue != default(V))
		{
		  keys.remove(key);
		}
		keys.add(key);

		if (cache.size() > capacity)
		{
		  K lruKey = keys.poll();
		  if (lruKey != default(K))
		  {
			cache.remove(lruKey);

			// remove duplicated keys
			this.removeAll(lruKey);

			// queue may not contain any key of a possibly concurrently added entry of the same key in the cache
			if (cache.containsKey(lruKey))
			{
			  keys.add(lruKey);
			}
		  }
		}
	  }

	  public virtual void remove(K key)
	  {
		this.cache.remove(key);
		keys.remove(key);
	  }

	  public virtual void clear()
	  {
		cache.clear();
		keys.clear();
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return cache.Empty;
		  }
	  }

	  public virtual ISet<K> keySet()
	  {
		return cache.Keys;
	  }

	  public virtual int size()
	  {
		return cache.size();
	  }

	  /// <summary>
	  /// Removes all instances of the given key within the keys queue.
	  /// </summary>
	  protected internal virtual void removeAll(K key)
	  {
		while (keys.remove(key))
		{
		}
	  }

	}

}