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
namespace org.camunda.bpm.engine.rest.hal.cache
{

	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Cache = org.camunda.bpm.engine.rest.cache.Cache;

	public class DefaultHalResourceCache : Cache
	{

	  public static readonly IComparer<HalResourceCacheEntry> COMPARATOR = HalResourceCacheEntryComparator.Instance;

	  protected internal int capacity;
	  protected internal long secondsToLive;
	  protected internal IDictionary<string, HalResourceCacheEntry> cache;

	  public DefaultHalResourceCache() : this(100, 100)
	  {
	  }

	  public DefaultHalResourceCache(int capacity, long secondsToLive)
	  {
		this.capacity = capacity;
		this.secondsToLive = secondsToLive;
		cache = new Dictionary<string, HalResourceCacheEntry>();
	  }

	  public virtual int Capacity
	  {
		  get
		  {
			return capacity;
		  }
		  set
		  {
			this.capacity = value;
		  }
	  }


	  public virtual long SecondsToLive
	  {
		  get
		  {
			return secondsToLive;
		  }
		  set
		  {
			this.secondsToLive = value;
		  }
	  }


	  public virtual int size()
	  {
		return cache.Count;
	  }

	  public virtual void put(string id, object resource)
	  {
		cache[id] = new HalResourceCacheEntry(id, resource);
		ensureCapacityLimit();
	  }

	  public virtual void remove(string id)
	  {
		cache.Remove(id);
	  }

	  public virtual object get(string id)
	  {
		HalResourceCacheEntry cacheEntry = cache[id];
		if (cacheEntry != null)
		{
		  if (expired(cacheEntry))
		  {
			remove(cacheEntry.Id);
			return null;
		  }
		  else
		  {
			return cacheEntry.Resource;
		  }
		}
		else
		{
		  return null;
		}
	  }

	  public virtual void destroy()
	  {
		cache.Clear();
	  }

	  protected internal virtual void ensureCapacityLimit()
	  {
		if (size() > Capacity)
		{
		  IList<HalResourceCacheEntry> resources = new List<HalResourceCacheEntry>(cache.Values);
		  NavigableSet<HalResourceCacheEntry> remainingResources = new SortedSet<HalResourceCacheEntry>(COMPARATOR);

		  // remove expired resources
		  foreach (HalResourceCacheEntry resource in resources)
		  {
			if (expired(resource))
			{
			  remove(resource.Id);
			}
			else
			{
			  remainingResources.add(resource);
			}

			if (size() <= Capacity)
			{
			  // abort if capacity is reached
			  return;
			}
		  }

		  // if still exceed capacity remove oldest
		  while (remainingResources.size() > capacity)
		  {
			HalResourceCacheEntry resourceToRemove = remainingResources.pollFirst();
			if (resourceToRemove != null)
			{
			  remove(resourceToRemove.Id);
			}
			else
			{
			  break;
			}
		  }
		}
	  }

	  protected internal virtual bool expired(HalResourceCacheEntry entry)
	  {
		return entry.CreateTime + secondsToLive * 1000 < ClockUtil.CurrentTime.Ticks;
	  }

	}

}