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
namespace org.camunda.bpm.engine.rest.hal.identitylink
{

	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using HalCachingLinkResolver = org.camunda.bpm.engine.rest.hal.cache.HalCachingLinkResolver;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

	public class HalIdentityLinkResolver : HalCachingLinkResolver
	{

	  protected internal override Type HalResourceClass
	  {
		  get
		  {
			return typeof(HalIdentityLink);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override public java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveLinks(String[] linkedIds, org.camunda.bpm.engine.ProcessEngine processEngine)
	  public override IList<HalResource<object>> resolveLinks(string[] linkedIds, ProcessEngine processEngine)
	  {
		if (linkedIds.Length > 1)
		{
		  throw new InvalidRequestException(Response.Status.INTERNAL_SERVER_ERROR, "The identity link resolver can only handle one task id");
		}

		return base.resolveLinks(linkedIds, processEngine);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolveNotCachedLinks(String[] linkedIds, org.camunda.bpm.engine.ProcessEngine processEngine)
	  protected internal override IList<HalResource<object>> resolveNotCachedLinks(string[] linkedIds, ProcessEngine processEngine)
	  {
		TaskService taskService = processEngine.TaskService;

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(linkedIds[0]);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> resolvedIdentityLinks = new java.util.ArrayList<org.camunda.bpm.engine.rest.hal.HalResource<?>>();
		IList<HalResource<object>> resolvedIdentityLinks = new List<HalResource<object>>();
		foreach (IdentityLink identityLink in identityLinks)
		{
		  resolvedIdentityLinks.Add(HalIdentityLink.fromIdentityLink(identityLink));
		}

		return resolvedIdentityLinks;
	  }

	  protected internal override void putIntoCache<T1>(IList<T1> notCachedResources)
	  {
		// this resolver only can handle a single task and resolves a list of hal resources for this task
		if (notCachedResources != null && notCachedResources.Count > 0)
		{
		  string taskId = getResourceId(notCachedResources[0]);
		  Cache.put(taskId, notCachedResources);
		}
	  }

	  protected internal override string getResourceId<T1>(HalResource<T1> resource)
	  {
		return ((HalIdentityLink) resource).TaskId;
	  }

	}

}