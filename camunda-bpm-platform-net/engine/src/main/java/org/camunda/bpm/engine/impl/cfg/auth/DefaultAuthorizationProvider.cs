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
namespace org.camunda.bpm.engine.impl.cfg.auth
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DEPLOYMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureValidIndividualResourceId;


	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using TaskPermissions = org.camunda.bpm.engine.authorization.TaskPermissions;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// <para>Provides the default authorizations for camunda BPM.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultAuthorizationProvider : ResourceAuthorizationProvider
	{

	  public virtual AuthorizationEntity[] newUser(User user)
	  {
		// create an authorization which gives the user all permissions on himself:
		string userId = user.Id;

		ensureValidIndividualResourceId("Cannot create default authorization for user " + userId, userId);
		AuthorizationEntity resourceOwnerAuthorization = createGrantAuthorization(userId, null, USER, userId, ALL);

		return new AuthorizationEntity[]{resourceOwnerAuthorization};
	  }

	  public virtual AuthorizationEntity[] newGroup(Group group)
	  {
		IList<AuthorizationEntity> authorizations = new List<AuthorizationEntity>();

		// whenever a new group is created, all users part of the
		// group are granted READ permissions on the group
		string groupId = group.Id;

		ensureValidIndividualResourceId("Cannot create default authorization for group " + groupId, groupId);

		AuthorizationEntity groupMemberAuthorization = createGrantAuthorization(null, groupId, GROUP, groupId, READ);
		authorizations.Add(groupMemberAuthorization);

		return authorizations.ToArray();
	  }

	  public virtual AuthorizationEntity[] newTenant(Tenant tenant)
	  {
		// no default authorizations on tenants.
		return null;
	  }

	  public virtual AuthorizationEntity[] groupMembershipCreated(string groupId, string userId)
	  {

		// no default authorizations on memberships.

		return null;
	  }

	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, User user)
	  {

		AuthorizationEntity userAuthorization = createGrantAuthorization(user.Id, null, TENANT, tenant.Id, READ);

		return new AuthorizationEntity[]{userAuthorization};
	  }

	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, Group group)
	  {
		AuthorizationEntity userAuthorization = createGrantAuthorization(null, group.Id, TENANT, tenant.Id, READ);

		return new AuthorizationEntity[]{userAuthorization};
	  }

	  public virtual AuthorizationEntity[] newFilter(Filter filter)
	  {

		string owner = filter.Owner;
		if (!string.ReferenceEquals(owner, null))
		{
		  // create an authorization which gives the owner of the filter all permissions on the filter
		  string filterId = filter.Id;

		  ensureValidIndividualResourceId("Cannot create default authorization for filter owner " + owner, owner);

		  AuthorizationEntity filterOwnerAuthorization = createGrantAuthorization(owner, null, FILTER, filterId, ALL);

		  return new AuthorizationEntity[]{filterOwnerAuthorization};

		}
		else
		{
		  return null;

		}
	  }

	  // Deployment ///////////////////////////////////////////////

	  public virtual AuthorizationEntity[] newDeployment(Deployment deployment)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		IdentityService identityService = processEngineConfiguration.IdentityService;
		Authentication currentAuthentication = identityService.CurrentAuthentication;

		if (currentAuthentication != null && !string.ReferenceEquals(currentAuthentication.UserId, null))
		{
		  string userId = currentAuthentication.UserId;
		  string deploymentId = deployment.Id;
		  AuthorizationEntity authorization = createGrantAuthorization(userId, null, DEPLOYMENT, deploymentId, READ, DELETE);
		  return new AuthorizationEntity[]{authorization};
		}

		return null;
	  }

	  // Process Definition //////////////////////////////////////

	  public virtual AuthorizationEntity[] newProcessDefinition(ProcessDefinition processDefinition)
	  {
		// no default authorizations on process definitions.
		return null;
	  }

	  // Process Instance ///////////////////////////////////////

	  public virtual AuthorizationEntity[] newProcessInstance(ProcessInstance processInstance)
	  {
		// no default authorizations on process instances.
		return null;
	  }

	  // Task /////////////////////////////////////////////////

	  public virtual AuthorizationEntity[] newTask(Task task)
	  {
		// no default authorizations on tasks.
		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskAssignee(Task task, string oldAssignee, string newAssignee)
	  {
		if (!string.ReferenceEquals(newAssignee, null))
		{

		  ensureValidIndividualResourceId("Cannot create default authorization for assignee " + newAssignee, newAssignee);

		  // create (or update) an authorization for the new assignee.

		  string taskId = task.Id;

		  // fetch existing authorization
		  AuthorizationEntity authorization = getGrantAuthorizationByUserId(newAssignee, TASK, taskId);

		  // update authorization:
		  // (1) fetched authorization == null -> create a new authorization (with READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled)
		  // (2) fetched authorization != null -> add READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled
		  // Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
		  authorization = updateAuthorization(authorization, newAssignee, null, TASK, taskId, READ, DefaultUserPermissionForTask, SpecificReadVariablePermission);

		  // return always created or updated authorization
		  return new AuthorizationEntity[]{authorization};
		}

		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskOwner(Task task, string oldOwner, string newOwner)
	  {
		if (!string.ReferenceEquals(newOwner, null))
		{

		  ensureValidIndividualResourceId("Cannot create default authorization for owner " + newOwner, newOwner);

		  // create (or update) an authorization for the new owner.
		  string taskId = task.Id;

		  // fetch existing authorization
		  AuthorizationEntity authorization = getGrantAuthorizationByUserId(newOwner, TASK, taskId);

		  // update authorization:
		  // (1) fetched authorization == null -> create a new authorization (with READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled)
		  // (2) fetched authorization != null -> add READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled
		  // Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
		  authorization = updateAuthorization(authorization, newOwner, null, TASK, taskId, READ, DefaultUserPermissionForTask, SpecificReadVariablePermission);

		  // return always created or updated authorization
		  return new AuthorizationEntity[]{authorization};
		}

		return null;
	  }

	  public virtual AuthorizationEntity[] newTaskUserIdentityLink(Task task, string userId, string type)
	  {
		// create (or update) an authorization for the given user
		// whenever a new user identity link will be added

		ensureValidIndividualResourceId("Cannot grant default authorization for identity link to user " + userId, userId);

		string taskId = task.Id;

		// fetch existing authorization
		AuthorizationEntity authorization = getGrantAuthorizationByUserId(userId, TASK, taskId);

		// update authorization:
		// (1) fetched authorization == null -> create a new authorization (with READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled)
		// (2) fetched authorization != null -> add READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled
		// Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
		authorization = updateAuthorization(authorization, userId, null, TASK, taskId, READ, DefaultUserPermissionForTask, SpecificReadVariablePermission);

		// return always created or updated authorization
		return new AuthorizationEntity[]{authorization};
	  }

	  public virtual AuthorizationEntity[] newTaskGroupIdentityLink(Task task, string groupId, string type)
	  {

		ensureValidIndividualResourceId("Cannot grant default authorization for identity link to group " + groupId, groupId);

		// create (or update) an authorization for the given group
		// whenever a new user identity link will be added
		string taskId = task.Id;

		// fetch existing authorization
		AuthorizationEntity authorization = getGrantAuthorizationByGroupId(groupId, TASK, taskId);

		// update authorization:
		// (1) fetched authorization == null -> create a new authorization (with READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled)
		// (2) fetched authorization != null -> add READ, (UPDATE/TASK_WORK) permission, and READ_VARIABLE if enabled
		// Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
		authorization = updateAuthorization(authorization, null, groupId, TASK, taskId, READ, DefaultUserPermissionForTask, SpecificReadVariablePermission);

		// return always created or updated authorization
		return new AuthorizationEntity[]{authorization};
	  }

	  public virtual AuthorizationEntity[] deleteTaskUserIdentityLink(Task task, string userId, string type)
	  {
		// an existing authorization will not be deleted in such a case
		return null;
	  }

	  public virtual AuthorizationEntity[] deleteTaskGroupIdentityLink(Task task, string groupId, string type)
	  {
		// an existing authorization will not be deleted in such a case
		return null;
	  }

	  public virtual AuthorizationEntity[] newDecisionDefinition(DecisionDefinition decisionDefinition)
	  {
		// no default authorizations on decision definitions.
		return null;
	  }

	  public virtual AuthorizationEntity[] newDecisionRequirementsDefinition(DecisionRequirementsDefinition decisionRequirementsDefinition)
	  {
		// no default authorizations on decision requirements definitions.
		return null;
	  }

	  // helper //////////////////////////////////////////////////////////////

	  protected internal virtual AuthorizationManager AuthorizationManager
	  {
		  get
		  {
			CommandContext commandContext = Context.CommandContext;
			return commandContext.AuthorizationManager;
		  }
	  }

	  protected internal virtual AuthorizationEntity getGrantAuthorizationByUserId(string userId, Resource resource, string resourceId)
	  {
		AuthorizationManager authorizationManager = AuthorizationManager;
		return authorizationManager.findAuthorizationByUserIdAndResourceId(AUTH_TYPE_GRANT, userId, resource, resourceId);
	  }

	  protected internal virtual AuthorizationEntity getGrantAuthorizationByGroupId(string groupId, Resource resource, string resourceId)
	  {
		AuthorizationManager authorizationManager = AuthorizationManager;
		return authorizationManager.findAuthorizationByGroupIdAndResourceId(AUTH_TYPE_GRANT, groupId, resource, resourceId);
	  }

	  protected internal virtual AuthorizationEntity updateAuthorization(AuthorizationEntity authorization, string userId, string groupId, Resource resource, string resourceId, params Permission[] permissions)
	  {
		if (authorization == null)
		{
		  authorization = createGrantAuthorization(userId, groupId, resource, resourceId);
		  updateAuthorizationBasedOnCacheEntries(authorization, userId, groupId, resource, resourceId);
		}

		if (permissions != null)
		{
		  foreach (Permission permission in permissions)
		  {
			if (permission != null)
			{
			  authorization.addPermission(permission);
			}
		  }
		}

		return authorization;
	  }

	  protected internal virtual AuthorizationEntity createGrantAuthorization(string userId, string groupId, Resource resource, string resourceId, params Permission[] permissions)
	  {
		// assuming that there are no default authorizations for *
		if (!string.ReferenceEquals(userId, null))
		{
		  ensureValidIndividualResourceId("Cannot create authorization for user " + userId, userId);
		}
		if (!string.ReferenceEquals(groupId, null))
		{
		  ensureValidIndividualResourceId("Cannot create authorization for group " + groupId, groupId);
		}

		AuthorizationEntity authorization = new AuthorizationEntity(AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.GroupId = groupId;
		authorization.setResource(resource);
		authorization.ResourceId = resourceId;

		if (permissions != null)
		{
		  foreach (Permission permission in permissions)
		  {
			authorization.addPermission(permission);
		  }
		}

		return authorization;
	  }

	  protected internal virtual Permission DefaultUserPermissionForTask
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.DefaultUserPermissionForTask;
		  }
	  }

	  protected internal virtual Permission SpecificReadVariablePermission
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.EnforceSpecificVariablePermission ? TaskPermissions.READ_VARIABLE : null;
		  }
	  }

	  /// <summary>
	  /// Searches through the cache, if there is already an authorization with same rights. If that's the case
	  /// update the given authorization with the permissions and remove the old one from the cache.
	  /// </summary>
	  protected internal virtual void updateAuthorizationBasedOnCacheEntries(AuthorizationEntity authorization, string userId, string groupId, Resource resource, string resourceId)
	  {
		DbEntityManager dbManager = Context.CommandContext.DbEntityManager;
		IList<AuthorizationEntity> list = dbManager.getCachedEntitiesByType(typeof(AuthorizationEntity));
		foreach (AuthorizationEntity authEntity in list)
		{
		  bool hasSameAuthRights = hasEntitySameAuthorizationRights(authEntity, userId, groupId, resource, resourceId);
		  if (hasSameAuthRights)
		  {
			int previousPermissions = authEntity.getPermissions();
			authorization.setPermissions(previousPermissions);
			dbManager.DbEntityCache.remove(authEntity);
			return;
		  }
		}
	  }

	  protected internal virtual bool hasEntitySameAuthorizationRights(AuthorizationEntity authEntity, string userId, string groupId, Resource resource, string resourceId)
	  {
		bool sameUserId = areIdsEqual(authEntity.UserId, userId);
		bool sameGroupId = areIdsEqual(authEntity.GroupId, groupId);
		bool sameResourceId = areIdsEqual(authEntity.ResourceId, (resourceId));
		bool sameResourceType = authEntity.ResourceType == resource.resourceType();
		bool sameAuthorizationType = authEntity.AuthorizationType == AUTH_TYPE_GRANT;
		return sameUserId && sameGroupId && sameResourceType && sameResourceId && sameAuthorizationType;
	  }

	  protected internal virtual bool areIdsEqual(string firstId, string secondId)
	  {
		if (string.ReferenceEquals(firstId, null) || string.ReferenceEquals(secondId, null))
		{
		  return string.ReferenceEquals(firstId, secondId);
		}
		else
		{
		  return firstId.Equals(secondId);
		}
	  }
	}

}