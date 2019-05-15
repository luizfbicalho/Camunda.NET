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


	public abstract class HalIdResourceCacheLinkResolver : HalCachingLinkResolver
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public final static java.util.Comparator<org.camunda.bpm.engine.rest.hal.HalResource<?>> ID_COMPARATOR = new HalIdResourceComparator();
	  public static readonly IComparer<HalResource<object>> ID_COMPARATOR = new HalIdResourceComparator();

	  protected internal override string getResourceId<T1>(HalResource<T1> resource)
	  {
		return ((HalIdResource) resource).Id;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected java.util.Comparator<org.camunda.bpm.engine.rest.hal.HalResource<?>> getResourceComparator()
	  protected internal override IComparer<HalResource<object>> ResourceComparator
	  {
		  get
		  {
			return ID_COMPARATOR;
		  }
	  }

	  public class HalIdResourceComparator : IComparer<HalResource<JavaToDotNetGenericWildcard>>
	  {

		public virtual int compare<T1, T2>(HalResource<T1> resource1, HalResource<T2> resource2)
		{
		  string id1 = ((HalIdResource) resource1).Id;
		  string id2 = ((HalIdResource) resource2).Id;
		  return id1.CompareTo(id2);
		}

	  }

	}

}