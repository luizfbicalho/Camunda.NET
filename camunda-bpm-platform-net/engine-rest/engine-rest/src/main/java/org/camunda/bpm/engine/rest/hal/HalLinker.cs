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
namespace org.camunda.bpm.engine.rest.hal
{


	/// <summary>
	/// A stateful linker which collects information about the links it creates.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalLinker
	{

	  /// <summary>
	  /// linked resource ids by <seealso cref="HalRelation"/>
	  /// </summary>
	  internal IDictionary<HalRelation, ISet<string>> linkedResources = new Dictionary<HalRelation, ISet<string>>();

	  protected internal readonly Hal hal;

	  /// <summary>
	  /// The HalResource on which the links are constructed
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected final HalResource<?> resource;
	  protected internal readonly HalResource<object> resource;

	  public HalLinker<T1>(Hal hal, HalResource<T1> resource)
	  {
		this.hal = hal;
		this.resource = resource;
	  }

	  /// <summary>
	  /// Creates a link in a given relation.
	  /// </summary>
	  /// <param name="rel"> the <seealso cref="HalRelation"/> for which a link should be constructed </param>
	  /// <param name="pathParams"> the path params to populate the url template with.
	  ///  </param>
	  public virtual void createLink(HalRelation rel, params string[] pathParams)
	  {
		if (pathParams != null && pathParams.Length > 0 && !string.ReferenceEquals(pathParams[0], null))
		{
		  ISet<string> linkedResourceIds = linkedResources[rel];
		  if (linkedResourceIds == null)
		  {
			linkedResourceIds = new HashSet<string>();
			linkedResources[rel] = linkedResourceIds;
		  }

		  // Hmm... use the last id in the pathParams as linked resource id
		  linkedResourceIds.Add(pathParams[pathParams.Length - 1]);

		  resource.addLink(rel.relName, rel.uriTemplate.build((object[])pathParams));
		}
	  }

	  public virtual ISet<HalRelation> LinkedRelations
	  {
		  get
		  {
			return linkedResources.Keys;
		  }
	  }

	  public virtual ISet<string> getLinkedResourceIdsByRelation(HalRelation relation)
	  {
		ISet<string> result = linkedResources[relation];
		if (result != null)
		{
		  return result;
		}
		else
		{
		  return java.util.Collections.emptySet();
		}
	  }

	  /// <summary>
	  /// Resolves a relation. Locates a HalLinkResolver for resolving the set of all linked resources in the relation.
	  /// </summary>
	  /// <param name="relation"> the relation to resolve </param>
	  /// <param name="processEngine"> the process engine to use </param>
	  /// <returns> the list of resolved resources </returns>
	  /// <exception cref="RuntimeException"> if no HalLinkResolver can be found for the linked resource type. </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public List<HalResource<?>> resolve(HalRelation relation, org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual IList<HalResource<object>> resolve(HalRelation relation, ProcessEngine processEngine)
	  {
		HalLinkResolver linkResolver = hal.getLinkResolver(relation.resourceType);
		if (linkResolver != null)
		{
		  ISet<string> linkedIds = getLinkedResourceIdsByRelation(relation);
		  if (linkedIds.Count > 0)
		  {
			return linkResolver.resolveLinks(linkedIds.toArray(new string[linkedIds.Count]), processEngine);
		  }
		  else
		  {
			return java.util.Collections.emptyList();
		  }
		}
		else
		{
		  throw new Exception("Cannot find HAL link resolver for resource type '" + relation.resourceType + "'.");
		}
	  }

	  /// <summary>
	  /// merge the links of an embedded resource into this linker.
	  /// This is useful when building resources which are actually resource collections.
	  /// You can then merge the relations of all resources in the collection and the unique the set of linked resources to embed.
	  /// </summary>
	  /// <param name="embedded"> the embedded resource for which the links should be merged into this linker. </param>
	  public virtual void mergeLinks<T1>(HalResource<T1> embedded)
	  {
		foreach (KeyValuePair<HalRelation, ISet<string>> linkentry in embedded.linker.linkedResources.SetOfKeyValuePairs())
		{
		  ISet<string> linkedIdSet = linkedResources[linkentry.Key];
		  if (linkedIdSet != null)
		  {
			linkedIdSet.addAll(linkentry.Value);
		  }
		  else
		  {
			linkedResources[linkentry.Key] = linkentry.Value;
		  }
		}
	  }
	}

}