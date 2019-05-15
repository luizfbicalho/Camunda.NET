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
namespace org.camunda.bpm.engine.impl
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class AuthorizationQueryImpl : AbstractQuery<AuthorizationQuery, Authorization>, AuthorizationQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string[] userIds;
	  protected internal string[] groupIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int resourceType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string resourceId_Renamed;
	  protected internal int permission = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? authorizationType_Renamed;
	  protected internal bool queryByPermission = false;
	  protected internal bool queryByResourceType = false;

	  private ISet<Resource> resourcesIntersection = new HashSet<Resource>();

	  public AuthorizationQueryImpl()
	  {
	  }

	  public AuthorizationQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual AuthorizationQuery authorizationId(string id)
	  {
		this.id = id;
		return this;
	  }

	  public virtual AuthorizationQuery userIdIn(params string[] userIdIn)
	  {
		if (groupIds != null)
		{
		  throw new ProcessEngineException("Cannot query for user and group authorizations at the same time.");
		}
		this.userIds = userIdIn;
		return this;
	  }

	  public virtual AuthorizationQuery groupIdIn(params string[] groupIdIn)
	  {
		if (userIds != null)
		{
		  throw new ProcessEngineException("Cannot query for user and group authorizations at the same time.");
		}
		this.groupIds = groupIdIn;
		return this;
	  }

	  public virtual AuthorizationQuery resourceType(Resource resource)
	  {
		return resourceType(resource.resourceType());
	  }

	  public virtual AuthorizationQuery resourceType(int resourceType)
	  {
		this.resourceType_Renamed = resourceType;
		queryByResourceType = true;
		return this;
	  }

	  public virtual AuthorizationQuery resourceId(string resourceId)
	  {
		this.resourceId_Renamed = resourceId;
		return this;
	  }

	  public virtual AuthorizationQuery hasPermission(Permission p)
	  {
		queryByPermission = true;

		if (resourcesIntersection.Count == 0)
		{
		  resourcesIntersection.addAll(Arrays.asList(p.Types));
		}
		else
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		  resourcesIntersection.retainAll(new HashSet<Resource>(Arrays.asList(p.Types)));
		}

		this.permission |= p.Value;
		return this;
	  }

	  public virtual AuthorizationQuery authorizationType(int? type)
	  {
		this.authorizationType_Renamed = type;
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.AuthorizationManager.selectAuthorizationCountByQueryCriteria(this).Value;
	  }

	  public override IList<Authorization> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.AuthorizationManager.selectAuthorizationByQueryCriteria(this);
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || containsIncompatiblePermissions() || containsIncompatibleResourceType();
	  }

	  /// <summary>
	  /// check whether there are any compatible resources
	  /// for all of the filtered permission parameters
	  /// </summary>
	  private bool containsIncompatiblePermissions()
	  {
		return queryByPermission && resourcesIntersection.Count == 0;
	  }

	  /// <summary>
	  /// check whether the permissions' resources
	  /// are compatible to the filtered resource parameter
	  /// </summary>
	  private bool containsIncompatibleResourceType()
	  {
		if (queryByResourceType && queryByPermission)
		{
		  Resource[] resources = resourcesIntersection.toArray(new Resource[resourcesIntersection.Count]);
		  return !ResourceTypeUtil.resourceIsContainedInArray(resourceType_Renamed, resources);
		}
		return false;
	  }

	  // getters ////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual bool QueryByPermission
	  {
		  get
		  {
			return queryByPermission;
		  }
	  }

	  public virtual string[] UserIds
	  {
		  get
		  {
			return userIds;
		  }
	  }

	  public virtual string[] GroupIds
	  {
		  get
		  {
			return groupIds;
		  }
	  }

	  public virtual int ResourceType
	  {
		  get
		  {
			return resourceType_Renamed;
		  }
	  }

	  public virtual string ResourceId
	  {
		  get
		  {
			return resourceId_Renamed;
		  }
	  }

	  public virtual int Permission
	  {
		  get
		  {
			return permission;
		  }
	  }

	  public virtual bool QueryByResourceType
	  {
		  get
		  {
			return queryByResourceType;
		  }
	  }

	  public virtual ISet<Resource> ResourcesIntersection
	  {
		  get
		  {
			return resourcesIntersection;
		  }
	  }

	  public virtual AuthorizationQuery orderByResourceType()
	  {
		orderBy(AuthorizationQueryProperty_Fields.RESOURCE_TYPE);
		return this;
	  }

	  public virtual AuthorizationQuery orderByResourceId()
	  {
		orderBy(AuthorizationQueryProperty_Fields.RESOURCE_ID);
		return this;
	  }

	}

}