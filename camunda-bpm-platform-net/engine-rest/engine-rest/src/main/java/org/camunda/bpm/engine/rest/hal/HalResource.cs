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
	/// Base class for implementing a HAL resource as defined in
	/// <a href="http://tools.ietf.org/html/draft-kelly-json-hal-06#section-4">json-hal-06#section-4</a>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class HalResource<T>
	{

	  /// <summary>
	  /// This resource links </summary>
	  protected internal IDictionary<string, HalLink> _links;

	  /// <summary>
	  /// Embedded resources </summary>
	  protected internal IDictionary<string, object> _embedded;

	  // the linker used by this resource
	  [NonSerialized]
	  protected internal HalLinker linker;

	  public HalResource()
	  {
		this.linker = Hal.Instance.createLinker(this);
	  }

	  public virtual IDictionary<string, HalLink> get_links()
	  {
		return _links;
	  }

	  public virtual IDictionary<string, object> get_embedded()
	  {
		return _embedded;
	  }

	  public virtual void addLink(string rel, string href)
	  {
		if (_links == null)
		{
		  _links = new SortedDictionary<string, HalLink>();
		}
		_links[rel] = new HalLink(href);
	  }

	  public virtual void addLink(string rel, URI hrefUri)
	  {
		addLink(rel, hrefUri.ToString());
	  }

	  public virtual void addEmbedded<T1>(string name, HalResource<T1> embedded)
	  {
		linker.mergeLinks(embedded);
		addEmbeddedObject(name, embedded);
	  }

	  private void addEmbeddedObject(string name, object embedded)
	  {
		if (_embedded == null)
		{
		  _embedded = new SortedDictionary<string, object>();
		}
		_embedded[name] = embedded;
	  }

	  public virtual void addEmbedded<T1>(string name, IList<T1> embeddedCollection)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (HalResource<?> resource : embeddedCollection)
		foreach (HalResource<object> resource in embeddedCollection)
		{
		  linker.mergeLinks(resource);
		}
		addEmbeddedObject(name, embeddedCollection);
	  }

	  public virtual object getEmbedded(string name)
	  {
		return _embedded[name];
	  }

	  /// <summary>
	  /// Can be used to embed a relation. Embedded all linked resources in the given relation.
	  /// </summary>
	  /// <param name="relation"> the relation to embedded </param>
	  /// <param name="processEngine"> used to resolve the resources </param>
	  /// <returns> the resource itself. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T embed(HalRelation relation, org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual T embed(HalRelation relation, ProcessEngine processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<HalResource<?>> resolvedLinks = linker.resolve(relation, processEngine);
		IList<HalResource<object>> resolvedLinks = linker.resolve(relation, processEngine);
		if (resolvedLinks != null && resolvedLinks.Count > 0)
		{
		  addEmbedded(relation.relName, resolvedLinks);
		}
		return (T) this;
	  }

	}

}