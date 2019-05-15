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
namespace org.camunda.bpm.engine.rest.hal.tenant
{

	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using HalIdResourceCacheLinkResolver = org.camunda.bpm.engine.rest.hal.cache.HalIdResourceCacheLinkResolver;

	public class HalTenantResolver : HalIdResourceCacheLinkResolver
	{

	  protected internal override Type HalResourceClass
	  {
		  get
		  {
			return typeof(HalTenant);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveNotCachedLinks(String[] linkedIds, org.camunda.bpm.engine.ProcessEngine processEngine)
	  protected internal override IList<HalResource<object>> resolveNotCachedLinks(string[] linkedIds, ProcessEngine processEngine)
	  {
		IdentityService identityService = processEngine.IdentityService;

		IList<Tenant> tenants = identityService.createTenantQuery().tenantIdIn(linkedIds).list();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolvedTenants = new java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>>();
		IList<HalResource<object>> resolvedTenants = new List<HalResource<object>>();
		foreach (Tenant tenant in tenants)
		{
		  resolvedTenants.Add(HalTenant.fromTenant(tenant));
		}

		return resolvedTenants;
	  }

	}

}