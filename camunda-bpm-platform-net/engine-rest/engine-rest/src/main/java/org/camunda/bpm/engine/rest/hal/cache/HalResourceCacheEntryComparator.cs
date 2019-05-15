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

	public class HalResourceCacheEntryComparator : IComparer<HalResourceCacheEntry>
	{

	  public static readonly IComparer<HalResourceCacheEntry> INSTANCE = new HalResourceCacheEntryComparator();
	  public static readonly IComparer<HalResourceCacheEntry> REVERSE = Collections.reverseOrder(INSTANCE);

	  /// <summary>
	  /// Sort cache entries by ascending create time (oldest first) </summary>
	  public static IComparer<HalResourceCacheEntry> Instance
	  {
		  get
		  {
			return INSTANCE;
		  }
	  }

	  /// <summary>
	  /// Sort cache entries by descending create time (newest first) </summary>
	  public static IComparer<HalResourceCacheEntry> Reverse
	  {
		  get
		  {
			return REVERSE;
		  }
	  }

	  public virtual int Compare(HalResourceCacheEntry entry1, HalResourceCacheEntry entry2)
	  {
		int compareTime = ((long?) entry1.CreateTime).compareTo(entry2.CreateTime);
		if (compareTime != 0)
		{
		  return compareTime;
		}
		else
		{
		  return entry1.Id.CompareTo(entry2.Id);
		}
	  }

	}

}