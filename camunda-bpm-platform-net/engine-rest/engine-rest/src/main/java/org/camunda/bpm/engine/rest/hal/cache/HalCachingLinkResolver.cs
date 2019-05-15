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
namespace org.camunda.bpm.engine.rest.hal.cache
{

	using Cache = org.camunda.bpm.engine.rest.cache.Cache;

	public abstract class HalCachingLinkResolver : HalLinkResolver
	{

	  /// <summary>
	  /// Resolve resources for linked ids, if configured uses a cache.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveLinks(String[] linkedIds, org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual IList<HalResource<object>> resolveLinks(string[] linkedIds, ProcessEngine processEngine)
	  {
		Cache cache = Cache;

		if (cache == null)
		{
		  return resolveNotCachedLinks(linkedIds, processEngine);
		}
		else
		{
		  List<string> notCachedLinkedIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolvedResources = resolveCachedLinks(linkedIds, cache, notCachedLinkedIds);
		  IList<HalResource<object>> resolvedResources = resolveCachedLinks(linkedIds, cache, notCachedLinkedIds);

		  if (notCachedLinkedIds.Count > 0)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> notCachedResources = resolveNotCachedLinks(notCachedLinkedIds.toArray(new String[notCachedLinkedIds.size()]), processEngine);
			IList<HalResource<object>> notCachedResources = resolveNotCachedLinks(notCachedLinkedIds.ToArray(), processEngine);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: resolvedResources.addAll(notCachedResources);
			((IList<HalResource<object>>)resolvedResources).AddRange(notCachedResources);
			putIntoCache(notCachedResources);
		  }

		  sortResolvedResources(resolvedResources);

		  return resolvedResources;
		}
	  }

	  /// <summary>
	  /// Sort the resolved resources to ensure consistent order of resolved resources.
	  /// </summary>
	  protected internal virtual void sortResolvedResources<T1>(IList<T1> resolvedResources)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Comparator<org.camunda.bpm.engine.rest.hal.HalResource<?>> comparator = getResourceComparator();
		IComparer<HalResource<object>> comparator = ResourceComparator;
		if (comparator != null)
		{
		  resolvedResources.Sort(comparator);
		}
	  }

	  /// <returns> the cache for this resolver </returns>
	  protected internal virtual Cache Cache
	  {
		  get
		  {
			return Hal.Instance.getHalRelationCache(HalResourceClass);
		  }
	  }

	  /// <summary>
	  /// Returns a list with all resources which are cached.
	  /// </summary>
	  /// <param name="linkedIds"> the ids to resolve </param>
	  /// <param name="cache"> the cache to use </param>
	  /// <param name="notCachedLinkedIds"> a list with ids which are not found in the cache </param>
	  /// <returns> the cached resources </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveCachedLinks(String[] linkedIds, org.camunda.bpm.engine.rest.cache.Cache cache, java.util.List<String> notCachedLinkedIds)
	  protected internal virtual IList<HalResource<object>> resolveCachedLinks(string[] linkedIds, Cache cache, IList<string> notCachedLinkedIds)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolvedResources = new java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>>();
		List<HalResource<object>> resolvedResources = new List<HalResource<object>>();

		foreach (string linkedId in linkedIds)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.rest.hal.HalResource<?> resource = (org.camunda.bpm.engine.rest.hal.HalResource<?>) cache.get(linkedId);
		  HalResource<object> resource = (HalResource<object>) cache.get(linkedId);
		  if (resource != null)
		  {
			resolvedResources.Add(resource);
		  }
		  else
		  {
			notCachedLinkedIds.Add(linkedId);
		  }
		}

		return resolvedResources;
	  }

	  /// <summary>
	  /// Put a resource into the cache.
	  /// </summary>
	  protected internal virtual void putIntoCache<T1>(IList<T1> notCachedResources)
	  {
		Cache cache = Cache;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.rest.hal.HalResource<?> notCachedResource : notCachedResources)
		foreach (HalResource<object> notCachedResource in notCachedResources)
		{
		  cache.put(getResourceId(notCachedResource), notCachedResource);
		}
	  }

	  /// <returns> the class of the entity which is resolved </returns>
	  protected internal abstract Type HalResourceClass {get;}

	  /// <returns> a comparator for this HAL resource if not overridden sorting is skipped </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Comparator<org.camunda.bpm.engine.rest.hal.HalResource<?>> getResourceComparator()
	  protected internal virtual IComparer<HalResource<object>> ResourceComparator
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <returns> the resolved resources which are currently not cached </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveNotCachedLinks(String[] linkedIds, org.camunda.bpm.engine.ProcessEngine processEngine);
	  protected internal abstract IList<HalResource<object>> resolveNotCachedLinks(string[] linkedIds, ProcessEngine processEngine);

	  /// <returns> the id which identifies a resource in the cache </returns>
	  protected internal abstract string getResourceId<T1>(HalResource<T1> resource);

	}

}