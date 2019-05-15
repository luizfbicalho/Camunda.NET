using System;

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
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HalRelation
	{

	  /// <summary>
	  /// the name of the relation </summary>
	  protected internal string relName;

	  /// <summary>
	  /// the url template used by the relation to construct links </summary>
	  protected internal UriBuilder uriTemplate;

	  /// <summary>
	  /// the type of the resource we build a relation to. </summary>
	  protected internal Type resourceType;

	  /// <summary>
	  /// Build a relation to a resource.
	  /// </summary>
	  /// <param name="relName"> the name of the relation. </param>
	  /// <param name="resourceType"> the type of the resource </param>
	  /// <returns> the relation </returns>
	  public static HalRelation build(string relName, Type resourceType, UriBuilder urlTemplate)
	  {
		HalRelation relation = new HalRelation();
		relation.relName = relName;
		relation.uriTemplate = urlTemplate;
		relation.resourceType = resourceType;
		return relation;
	  }

	  public virtual string RelName
	  {
		  get
		  {
			return relName;
		  }
	  }

	  public virtual UriBuilder UriTemplate
	  {
		  get
		  {
			return uriTemplate;
		  }
	  }

	  public virtual Type ResourceType
	  {
		  get
		  {
			return resourceType;
		  }
	  }

	}

}