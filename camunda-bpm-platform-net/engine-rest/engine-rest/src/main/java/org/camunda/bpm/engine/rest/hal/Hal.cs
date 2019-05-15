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

	using Cache = org.camunda.bpm.engine.rest.cache.Cache;
	using HalCaseDefinitionResolver = org.camunda.bpm.engine.rest.hal.caseDefinition.HalCaseDefinitionResolver;
	using HalGroupResolver = org.camunda.bpm.engine.rest.hal.group.HalGroupResolver;
	using HalIdentityLinkResolver = org.camunda.bpm.engine.rest.hal.identitylink.HalIdentityLinkResolver;
	using HalProcessDefinitionResolver = org.camunda.bpm.engine.rest.hal.processDefinition.HalProcessDefinitionResolver;
	using HalUserResolver = org.camunda.bpm.engine.rest.hal.user.HalUserResolver;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Hal
	{

	  public const string APPLICATION_HAL_JSON = "application/hal+json";
	  public static readonly MediaType APPLICATION_HAL_JSON_TYPE = new MediaType("application", "hal+json");

	  public static Hal instance = new Hal();

	  protected internal IDictionary<Type, HalLinkResolver> halLinkResolvers = new Dictionary<Type, HalLinkResolver>();
	  protected internal IDictionary<Type, Cache> halRelationCaches = new Dictionary<Type, Cache>();

	  public Hal()
	  {
		// register the built-in resolvers
		halLinkResolvers[typeof(UserRestService)] = new HalUserResolver();
		halLinkResolvers[typeof(GroupRestService)] = new HalGroupResolver();
		halLinkResolvers[typeof(ProcessDefinitionRestService)] = new HalProcessDefinitionResolver();
		halLinkResolvers[typeof(CaseDefinitionRestService)] = new HalCaseDefinitionResolver();
		halLinkResolvers[typeof(IdentityRestService)] = new HalIdentityLinkResolver();
	  }

	  public static Hal Instance
	  {
		  get
		  {
			return instance;
		  }
	  }

	  public virtual HalLinker createLinker<T1>(HalResource<T1> resource)
	  {
		return new HalLinker(this, resource);
	  }

	  public virtual HalLinkResolver getLinkResolver(Type resourceClass)
	  {
		return halLinkResolvers[resourceClass];
	  }

	  public virtual void registerHalRelationCache(Type entityClass, Cache cache)
	  {
		halRelationCaches[entityClass] = cache;
	  }

	  public virtual Cache getHalRelationCache(Type resourceClass)
	  {
		return halRelationCaches[resourceClass];
	  }

	  public virtual void destroyHalRelationCaches()
	  {
		foreach (Cache cache in halRelationCaches.Values)
		{
		  cache.destroy();
		}
		halRelationCaches.Clear();
	  }

	}

}