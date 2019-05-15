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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_INSTANCE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_HISTORY_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.READ_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.BATCH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_REQUIREMENTS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DEPLOYMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;

	using BatchQueryImpl = org.camunda.bpm.engine.impl.batch.BatchQueryImpl;
	using BatchStatisticsQueryImpl = org.camunda.bpm.engine.impl.batch.BatchStatisticsQueryImpl;
	using HistoricBatchQueryImpl = org.camunda.bpm.engine.impl.batch.history.HistoricBatchQueryImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using AuthorizationCheck = org.camunda.bpm.engine.impl.db.AuthorizationCheck;
	using CompositePermissionCheck = org.camunda.bpm.engine.impl.db.CompositePermissionCheck;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using PermissionCheck = org.camunda.bpm.engine.impl.db.PermissionCheck;
	using PermissionCheckBuilder = org.camunda.bpm.engine.impl.db.PermissionCheckBuilder;
	using DecisionDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionQueryImpl;
	using DecisionRequirementsDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionQueryImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public class AuthorizationManager extends org.camunda.bpm.engine.impl.persistence.AbstractManager
	public class AuthorizationManager : AbstractManager
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  // Used instead of Collections.emptyList() as mybatis uses reflection to call methods
	  // like size() which can lead to problems as Collections.EmptyList is a private implementation
	  protected internal static readonly IList<string> EMPTY_LIST = new List<string>();

	  /// <summary>
	  /// Group ids for which authorizations exist in the database.
	  /// This is initialized once per command by the <seealso cref="#filterAuthenticatedGroupIds(List)"/> method. (Manager
	  /// instances are command scoped).
	  /// It is used to only check authorizations for groups for which authorizations exist. In other words,
	  /// if for a given group no authorization exists in the DB, then auth checks are not performed for this group.
	  /// </summary>
	  protected internal ISet<string> availableAuthorizedGroupIds = null;

	  protected internal bool? isRevokeAuthCheckUsed = null;

	  public virtual PermissionCheckBuilder newPermissionCheckBuilder()
	  {
		return new PermissionCheckBuilder();
	  }

	  public virtual Authorization createNewAuthorization(int type)
	  {
		checkAuthorization(CREATE, AUTHORIZATION, null);
		return new AuthorizationEntity(type);
	  }

	  public override void insert(DbEntity authorization)
	  {
		checkAuthorization(CREATE, AUTHORIZATION, null);
		DbEntityManager.insert(authorization);
	  }

	  public virtual IList<Authorization> selectAuthorizationByQueryCriteria(AuthorizationQueryImpl authorizationQuery)
	  {
		configureQuery(authorizationQuery, AUTHORIZATION);
		return DbEntityManager.selectList("selectAuthorizationByQueryCriteria", authorizationQuery);
	  }

	  public virtual long? selectAuthorizationCountByQueryCriteria(AuthorizationQueryImpl authorizationQuery)
	  {
		configureQuery(authorizationQuery, AUTHORIZATION);
		return (long?) DbEntityManager.selectOne("selectAuthorizationCountByQueryCriteria", authorizationQuery);
	  }

	  public virtual AuthorizationEntity findAuthorizationByUserIdAndResourceId(int type, string userId, Resource resource, string resourceId)
	  {
		return findAuthorization(type, userId, null, resource, resourceId);
	  }

	  public virtual AuthorizationEntity findAuthorizationByGroupIdAndResourceId(int type, string groupId, Resource resource, string resourceId)
	  {
		return findAuthorization(type, null, groupId, resource, resourceId);
	  }

	  public virtual AuthorizationEntity findAuthorization(int type, string userId, string groupId, Resource resource, string resourceId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();

		@params["type"] = type;
		@params["userId"] = userId;
		@params["groupId"] = groupId;
		@params["resourceId"] = resourceId;

		if (resource != null)
		{
		  @params["resourceType"] = resource.resourceType();
		}

		return (AuthorizationEntity) DbEntityManager.selectOne("selectAuthorizationByParameters", @params);
	  }

	  public virtual void update(AuthorizationEntity authorization)
	  {
		checkAuthorization(UPDATE, AUTHORIZATION, authorization.Id);
		DbEntityManager.merge(authorization);
	  }

	  public override void delete(DbEntity authorization)
	  {
		checkAuthorization(DELETE, AUTHORIZATION, authorization.Id);
		deleteAuthorizationsByResourceId(AUTHORIZATION, authorization.Id);
		base.delete(authorization);
	  }

	  // authorization checks ///////////////////////////////////////////

	  public virtual void checkAuthorization(CompositePermissionCheck compositePermissionCheck)
	  {
		if (AuthCheckExecuted)
		{

		  Authentication currentAuthentication = CurrentAuthentication;
		  string userId = currentAuthentication.UserId;

		  bool isAuthorized = isAuthorized(compositePermissionCheck);
		  if (!isAuthorized)
		  {

			IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();

			foreach (PermissionCheck check in compositePermissionCheck.AllPermissionChecks)
			{
			  missingAuthorizations.Add(new MissingAuthorization(check.Permission.Name, check.Resource.resourceName(), check.ResourceId));
			}

			throw new AuthorizationException(userId, missingAuthorizations);
		  }
		}
	  }

	  public virtual void checkAuthorization(Permission permission, Resource resource)
	  {
		checkAuthorization(permission, resource, null);
	  }

	  public override void checkAuthorization(Permission permission, Resource resource, string resourceId)
	  {
		if (AuthCheckExecuted)
		{
		  Authentication currentAuthentication = CurrentAuthentication;
		  bool isAuthorized = isAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, permission, resource, resourceId);
		  if (!isAuthorized)
		  {
			throw new AuthorizationException(currentAuthentication.UserId, permission.Name, resource.resourceName(), resourceId);
		  }
		}

	  }

	  public virtual bool isAuthorized(Permission permission, Resource resource, string resourceId)
	  {
		// this will be called by LdapIdentityProviderSession#isAuthorized() for executing LdapQueries.
		// to be backward compatible a check whether authorization has been enabled inside the given
		// command context will not be done.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.Authentication currentAuthentication = getCurrentAuthentication();
		Authentication currentAuthentication = CurrentAuthentication;

		if (AuthorizationEnabled && currentAuthentication != null && !string.ReferenceEquals(currentAuthentication.UserId, null))
		{
		  return isAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, permission, resource, resourceId);

		}
		else
		{
		  return true;

		}
	  }

	  public virtual bool isAuthorized(string userId, IList<string> groupIds, Permission permission, Resource resource, string resourceId)
	  {
		if (!isPermissionDisabled(permission))
		{
		  PermissionCheck permCheck = new PermissionCheck();
		  permCheck.Permission = permission;
		  permCheck.Resource = resource;
		  permCheck.ResourceId = resourceId;

		  return isAuthorized(userId, groupIds, permCheck);
		}
		else
		{
		  return true;
		}
	  }

	  public virtual bool isAuthorized(string userId, IList<string> groupIds, PermissionCheck permissionCheck)
	  {
		if (!AuthorizationEnabled)
		{
		  return true;
		}

		if (!isResourceValidForPermission(permissionCheck))
		{
		  throw LOG.invalidResourceForPermission(permissionCheck.Resource.resourceName(), permissionCheck.Permission.Name);
		}

		IList<string> filteredGroupIds = filterAuthenticatedGroupIds(groupIds);

		bool isRevokeAuthorizationCheckEnabled = isRevokeAuthCheckEnabled(userId, groupIds);
		CompositePermissionCheck compositePermissionCheck = createCompositePermissionCheck(permissionCheck);
		AuthorizationCheck authCheck = new AuthorizationCheck(userId, filteredGroupIds, compositePermissionCheck, isRevokeAuthorizationCheckEnabled);
		return DbEntityManager.selectBoolean("isUserAuthorizedForResource", authCheck);
	  }

	  protected internal virtual bool isRevokeAuthCheckEnabled(string userId, IList<string> groupIds)
	  {
		bool? isRevokeAuthCheckEnabled = this.isRevokeAuthCheckUsed;

		if (isRevokeAuthCheckEnabled == null)
		{
		  string configuredMode = Context.ProcessEngineConfiguration.AuthorizationCheckRevokes;
		  if (!string.ReferenceEquals(configuredMode, null))
		  {
			configuredMode = configuredMode.ToLower();
		  }
		  if (ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_ALWAYS.Equals(configuredMode))
		  {
			isRevokeAuthCheckEnabled = true;
		  }
		  else if (ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_NEVER.Equals(configuredMode))
		  {
			isRevokeAuthCheckEnabled = false;
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, Object> params = new java.util.HashMap<String, Object>();
			IDictionary<string, object> @params = new Dictionary<string, object>();
			@params["userId"] = userId;
			@params["authGroupIds"] = filterAuthenticatedGroupIds(groupIds);
			isRevokeAuthCheckEnabled = DbEntityManager.selectBoolean("selectRevokeAuthorization", @params);
		  }
		  this.isRevokeAuthCheckUsed = isRevokeAuthCheckEnabled;
		}

		return isRevokeAuthCheckEnabled.Value;
	  }

	  protected internal virtual CompositePermissionCheck createCompositePermissionCheck(PermissionCheck permissionCheck)
	  {
		CompositePermissionCheck compositePermissionCheck = new CompositePermissionCheck();
		compositePermissionCheck.AtomicChecks = Arrays.asList(permissionCheck);
		return compositePermissionCheck;
	  }

	  public virtual bool isAuthorized(string userId, IList<string> groupIds, CompositePermissionCheck compositePermissionCheck)
	  {
		foreach (PermissionCheck permissionCheck in compositePermissionCheck.AllPermissionChecks)
		{
		  if (!isResourceValidForPermission(permissionCheck))
		  {
			throw LOG.invalidResourceForPermission(permissionCheck.Resource.resourceName(), permissionCheck.Permission.Name);
		  }
		}
		IList<string> filteredGroupIds = filterAuthenticatedGroupIds(groupIds);

		bool isRevokeAuthorizationCheckEnabled = isRevokeAuthCheckEnabled(userId, groupIds);
		AuthorizationCheck authCheck = new AuthorizationCheck(userId, filteredGroupIds, compositePermissionCheck, isRevokeAuthorizationCheckEnabled);
		return DbEntityManager.selectBoolean("isUserAuthorizedForResource", authCheck);
	  }

	  public virtual bool isAuthorized(CompositePermissionCheck compositePermissionCheck)
	  {
		Authentication currentAuthentication = CurrentAuthentication;

		if (currentAuthentication != null)
		{
		  return isAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, compositePermissionCheck);
		}
		else
		{
		  return true;
		}
	  }

	  protected internal virtual bool isResourceValidForPermission(PermissionCheck permissionCheck)
	  {
		Resource[] permissionResources = permissionCheck.Permission.Types;
		Resource givenResource = permissionCheck.Resource;
		return ResourceTypeUtil.resourceIsContainedInArray(givenResource.resourceType(), permissionResources);
	  }

	  public virtual void validateResourceCompatibility(AuthorizationEntity authorization)
	  {
		int resourceType = authorization.ResourceType;
		ISet<Permission> permissionSet = authorization.CachedPermissions;

		foreach (Permission permission in permissionSet)
		{
		  if (!ResourceTypeUtil.resourceIsContainedInArray(resourceType, permission.Types))
		  {
			throw LOG.invalidResourceForAuthorization(resourceType, permission.Name);
		  }
		}
	  }


	  // authorization checks on queries ////////////////////////////////

	  public virtual void configureQuery(ListQueryParameterObject query)
	  {

		AuthorizationCheck authCheck = query.AuthCheck;
		authCheck.PermissionChecks.clear();

		if (AuthCheckExecuted)
		{
		  Authentication currentAuthentication = CurrentAuthentication;
		  authCheck.AuthUserId = currentAuthentication.UserId;
		  authCheck.AuthGroupIds = currentAuthentication.GroupIds;
		  enableQueryAuthCheck(authCheck);
		}
		else
		{
		  authCheck.AuthorizationCheckEnabled = false;
		  authCheck.AuthUserId = null;
		  authCheck.AuthGroupIds = null;
		}
	  }

	  public virtual void configureQueryHistoricFinishedInstanceReport(ListQueryParameterObject query, Resource resource)
	  {
		configureQuery(query);

		CompositePermissionCheck compositePermissionCheck = (new PermissionCheckBuilder()).conjunctive().atomicCheck(resource, "RES.KEY_", READ).atomicCheck(resource, "RES.KEY_", READ_HISTORY).build();

		query.AuthCheck.PermissionChecks = compositePermissionCheck;
	  }

	  public virtual void enableQueryAuthCheck(AuthorizationCheck authCheck)
	  {
		IList<string> authGroupIds = authCheck.AuthGroupIds;
		string authUserId = authCheck.AuthUserId;

		authCheck.AuthorizationCheckEnabled = true;
		authCheck.AuthGroupIds = filterAuthenticatedGroupIds(authGroupIds);
		authCheck.RevokeAuthorizationCheckEnabled = isRevokeAuthCheckEnabled(authUserId, authGroupIds);
	  }

	  public override void configureQuery(AbstractQuery query, Resource resource)
	  {
		configureQuery(query, resource, "RES.ID_");
	  }

	  public virtual void configureQuery(AbstractQuery query, Resource resource, string queryParam)
	  {
		configureQuery(query, resource, queryParam, Permissions.READ);
	  }

	  public virtual void configureQuery(AbstractQuery query, Resource resource, string queryParam, Permission permission)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).atomicCheck(resource, queryParam, permission).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  public virtual bool isPermissionDisabled(Permission permission)
	  {
		IList<string> disabledPermissions = CommandContext.ProcessEngineConfiguration.DisabledPermissions;
		if (disabledPermissions != null)
		{
		  foreach (string disabledPermission in disabledPermissions)
		  {
			if (permission.Name.Equals(disabledPermission))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	  protected internal virtual void addPermissionCheck(AuthorizationCheck authCheck, CompositePermissionCheck compositeCheck)
	  {
		CommandContext commandContext = CommandContext;
		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
		{
		  authCheck.PermissionChecks = compositeCheck;
		}
	  }

	  // delete authorizations //////////////////////////////////////////////////

	  public virtual void deleteAuthorizationsByResourceId(Resource resource, string resourceId)
	  {

		if (string.ReferenceEquals(resourceId, null))
		{
		  throw new System.ArgumentException("Resource id cannot be null");
		}

		if (AuthorizationEnabled)
		{
		  IDictionary<string, object> deleteParams = new Dictionary<string, object>();
		  deleteParams["resourceType"] = resource.resourceType();
		  deleteParams["resourceId"] = resourceId;
		  DbEntityManager.delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
		}

	  }

	  public virtual void deleteAuthorizationsByResourceIdAndUserId(Resource resource, string resourceId, string userId)
	  {

		if (string.ReferenceEquals(resourceId, null))
		{
		  throw new System.ArgumentException("Resource id cannot be null");
		}

		if (AuthorizationEnabled)
		{
		  IDictionary<string, object> deleteParams = new Dictionary<string, object>();
		  deleteParams["resourceType"] = resource.resourceType();
		  deleteParams["resourceId"] = resourceId;
		  deleteParams["userId"] = userId;
		  DbEntityManager.delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
		}

	  }

	  public virtual void deleteAuthorizationsByResourceIdAndGroupId(Resource resource, string resourceId, string groupId)
	  {

		if (string.ReferenceEquals(resourceId, null))
		{
		  throw new System.ArgumentException("Resource id cannot be null");
		}

		if (AuthorizationEnabled)
		{
		  IDictionary<string, object> deleteParams = new Dictionary<string, object>();
		  deleteParams["resourceType"] = resource.resourceType();
		  deleteParams["resourceId"] = resourceId;
		  deleteParams["groupId"] = groupId;
		  DbEntityManager.delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
		}

	  }

	  // predefined authorization checks

	  /* MEMBER OF CAMUNDA_ADMIN */

	  /// <summary>
	  /// Checks if the current authentication contains the group
	  /// <seealso cref="Groups#CAMUNDA_ADMIN"/>. The check is ignored if the authorization is
	  /// disabled or no authentication exists.
	  /// </summary>
	  /// <exception cref="AuthorizationException"> </exception>
	  public virtual void checkCamundaAdmin()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.Authentication currentAuthentication = getCurrentAuthentication();
		Authentication currentAuthentication = CurrentAuthentication;
		CommandContext commandContext = Context.CommandContext;

		if (AuthorizationEnabled && commandContext.AuthorizationCheckEnabled && currentAuthentication != null && !isCamundaAdmin(currentAuthentication))
		{

		  throw LOG.requiredCamundaAdminException();
		}
	  }

	  /// <param name="authentication">
	  ///          authentication to check, cannot be <code>null</code> </param>
	  /// <returns> <code>true</code> if the given authentication contains the group
	  ///         <seealso cref="Groups#CAMUNDA_ADMIN"/> or the user </returns>
	  public virtual bool isCamundaAdmin(Authentication authentication)
	  {
		IList<string> groupIds = authentication.GroupIds;
		if (groupIds != null)
		{
		  CommandContext commandContext = Context.CommandContext;
		  IList<string> adminGroups = commandContext.ProcessEngineConfiguration.AdminGroups;
		  foreach (string adminGroup in adminGroups)
		  {
			if (groupIds.Contains(adminGroup))
			{
			  return true;
			}
		  }
		}

		string userId = authentication.UserId;
		if (!string.ReferenceEquals(userId, null))
		{
		  CommandContext commandContext = Context.CommandContext;
		  IList<string> adminUsers = commandContext.ProcessEngineConfiguration.AdminUsers;
		  return adminUsers != null && adminUsers.Contains(userId);
		}

		return false;
	  }

	  /* QUERIES */

	  // deployment query ////////////////////////////////////////

	  public virtual void configureDeploymentQuery(DeploymentQueryImpl query)
	  {
		configureQuery(query, DEPLOYMENT);
	  }

	  // process definition query ////////////////////////////////

	  public virtual void configureProcessDefinitionQuery(ProcessDefinitionQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.KEY_");

		if (query.StartablePermissionCheck)
		{
		  AuthorizationCheck authorizationCheck = query.AuthCheck;

		  if (!authorizationCheck.RevokeAuthorizationCheckEnabled)
		  {
			CompositePermissionCheck permCheck = (new PermissionCheckBuilder()).atomicCheck(PROCESS_DEFINITION, "RES.KEY_", Permissions.CREATE_INSTANCE).build();

			query.addProcessDefinitionCreatePermissionCheck(permCheck);

		  }
		  else
		  {
			CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).conjunctive().atomicCheck(PROCESS_DEFINITION, "RES.KEY_", READ).atomicCheck(PROCESS_DEFINITION, "RES.KEY_", Permissions.CREATE_INSTANCE).build();
			addPermissionCheck(authorizationCheck, permissionCheck);
		  }

		}

	  }

	  // execution/process instance query ////////////////////////

	  public virtual void configureExecutionQuery(AbstractQuery query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "P.KEY_", READ_INSTANCE).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  // task query //////////////////////////////////////////////

	  public virtual void configureTaskQuery(TaskQueryImpl query)
	  {
		configureQuery(query);

		if (query.AuthCheck.AuthorizationCheckEnabled)
		{

		  // necessary authorization check when the task is part of
		  // a running process instance

		  CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(TASK, "RES.ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_TASK).build();
			addPermissionCheck(query.AuthCheck, permissionCheck);
		}
	  }

	  // event subscription query //////////////////////////////

	  public virtual void configureEventSubscriptionQuery(EventSubscriptionQueryImpl query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  public virtual void configureConditionalEventSubscriptionQuery(ListQueryParameterObject query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).atomicCheck(PROCESS_DEFINITION, "P.KEY_", READ).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  // incident query ///////////////////////////////////////

	  public virtual void configureIncidentQuery(IncidentQueryImpl query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  // variable instance query /////////////////////////////

	  protected internal virtual void configureVariableInstanceQuery(VariableInstanceQueryImpl query)
	  {
		configureQuery(query);

		if (query.AuthCheck.AuthorizationCheckEnabled)
		{

		  CompositePermissionCheck permissionCheck;
		  if (EnsureSpecificVariablePermission)
		  {
			permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE_VARIABLE).atomicCheck(TASK, "RES.TASK_ID_", READ_VARIABLE).build();
		  }
		  else
		  {
			permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).atomicCheck(TASK, "RES.TASK_ID_", READ).build();
		  }
			addPermissionCheck(query.AuthCheck, permissionCheck);
		}
	  }

	  // job definition query ////////////////////////////////////////////////

	  public virtual void configureJobDefinitionQuery(JobDefinitionQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_");
	  }

	  // job query //////////////////////////////////////////////////////////

	  public virtual void configureJobQuery(JobQueryImpl query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROCESS_INSTANCE_ID_", READ).atomicCheck(PROCESS_DEFINITION, "RES.PROCESS_DEF_KEY_", READ_INSTANCE).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  /* HISTORY */

	  // historic process instance query ///////////////////////////////////

	  public virtual void configureHistoricProcessInstanceQuery(HistoricProcessInstanceQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "SELF.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // historic activity instance query /////////////////////////////////

	  public virtual void configureHistoricActivityInstanceQuery(HistoricActivityInstanceQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // historic task instance query ////////////////////////////////////

	  public virtual void configureHistoricTaskInstanceQuery(HistoricTaskInstanceQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // historic variable instance query ////////////////////////////////

	  public virtual void configureHistoricVariableInstanceQuery(HistoricVariableInstanceQueryImpl query)
	  {
		Permission readPermission = READ_HISTORY;
		if (EnsureSpecificVariablePermission)
		{
		  readPermission = READ_HISTORY_VARIABLE;
		}
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", readPermission);
	  }

	  // historic detail query ////////////////////////////////

	  public virtual void configureHistoricDetailQuery(HistoricDetailQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // historic job log query ////////////////////////////////

	  public virtual void configureHistoricJobLogQuery(HistoricJobLogQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROCESS_DEF_KEY_", READ_HISTORY);
	  }

	  // historic incident query ////////////////////////////////

	  public virtual void configureHistoricIncidentQuery(HistoricIncidentQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  //historic identity link query ////////////////////////////////

	  public virtual void configureHistoricIdentityLinkQuery(HistoricIdentityLinkLogQueryImpl query)
	  {
	   configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  public virtual void configureHistoricDecisionInstanceQuery(HistoricDecisionInstanceQueryImpl query)
	  {
		configureQuery(query, DECISION_DEFINITION, "RES.DEC_DEF_KEY_", READ_HISTORY);
	  }

	  // historic external task log query /////////////////////////////////

	  public virtual void configureHistoricExternalTaskLogQuery(HistoricExternalTaskLogQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // user operation log query ///////////////////////////////

	  public virtual void configureUserOperationLogQuery(UserOperationLogQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_HISTORY);
	  }

	  // batch

	  public virtual void configureHistoricBatchQuery(HistoricBatchQueryImpl query)
	  {
		configureQuery(query, BATCH, "RES.ID_", READ_HISTORY);
	  }

	  /* STATISTICS QUERY */

	  public virtual void configureDeploymentStatisticsQuery(DeploymentStatisticsQueryImpl query)
	  {
		configureQuery(query, DEPLOYMENT, "RES.ID_");

		query.ProcessInstancePermissionChecks.Clear();
		query.JobPermissionChecks.Clear();
		query.IncidentPermissionChecks.Clear();

		if (query.AuthCheck.AuthorizationCheckEnabled)
		{

		  CompositePermissionCheck processInstancePermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "EXECUTION.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).build();

		  query.addProcessInstancePermissionCheck(processInstancePermissionCheck.AllPermissionChecks);

		  if (query.FailedJobsToInclude)
		  {
			CompositePermissionCheck jobPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "JOB.PROCESS_INSTANCE_ID_", READ).atomicCheck(PROCESS_DEFINITION, "JOB.PROCESS_DEF_KEY_", READ_INSTANCE).build();

			query.addJobPermissionCheck(jobPermissionCheck.AllPermissionChecks);
		  }

		  if (query.IncidentsToInclude)
		  {
			CompositePermissionCheck incidentPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "INC.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).build();

			query.addIncidentPermissionCheck(incidentPermissionCheck.AllPermissionChecks);

		  }
		}
	  }

	  public virtual void configureProcessDefinitionStatisticsQuery(ProcessDefinitionStatisticsQueryImpl query)
	  {
		configureQuery(query, PROCESS_DEFINITION, "RES.KEY_");
	  }

	  public virtual void configureActivityStatisticsQuery(ActivityStatisticsQueryImpl query)
	  {
		configureQuery(query);

		query.ProcessInstancePermissionChecks.Clear();
		query.JobPermissionChecks.Clear();
		query.IncidentPermissionChecks.Clear();

		if (query.AuthCheck.AuthorizationCheckEnabled)
		{

		  CompositePermissionCheck processInstancePermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "E.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "P.KEY_", READ_INSTANCE).build();

		  // the following is need in order to evaluate whether to perform authCheck or not
		  query.AuthCheck.PermissionChecks = processInstancePermissionCheck;
		  // the actual check
		  query.addProcessInstancePermissionCheck(processInstancePermissionCheck.AllPermissionChecks);

		  if (query.FailedJobsToInclude)
		  {
			CompositePermissionCheck jobPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "JOB.PROCESS_INSTANCE_ID_", READ).atomicCheck(PROCESS_DEFINITION, "JOB.PROCESS_DEF_KEY_", READ_INSTANCE).build();

			// the following is need in order to evaluate whether to perform authCheck or not
			query.AuthCheck.PermissionChecks = jobPermissionCheck;
			// the actual check
			query.addJobPermissionCheck(jobPermissionCheck.AllPermissionChecks);
		  }

		  if (query.IncidentsToInclude)
		  {
			CompositePermissionCheck incidentPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "I.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "PROCDEF.KEY_", READ_INSTANCE).build();

			// the following is need in order to evaluate whether to perform authCheck or not
			query.AuthCheck.PermissionChecks = incidentPermissionCheck;
			// the actual check
			query.addIncidentPermissionCheck(incidentPermissionCheck.AllPermissionChecks);

		  }
		}
	  }

	  public virtual void configureExternalTaskQuery(ExternalTaskQueryImpl query)
	  {
		configureQuery(query);
		CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_INSTANCE).build();
		addPermissionCheck(query.AuthCheck, permissionCheck);
	  }

	  public virtual void configureExternalTaskFetch(ListQueryParameterObject parameter)
	  {
		configureQuery(parameter);

		CompositePermissionCheck permissionCheck = newPermissionCheckBuilder().conjunctive().composite().disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", READ).atomicCheck(PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", READ_INSTANCE).done().composite().disjunctive().atomicCheck(PROCESS_INSTANCE, "RES.PROC_INST_ID_", UPDATE).atomicCheck(PROCESS_DEFINITION, "RES.PROC_DEF_KEY_", UPDATE_INSTANCE).done().build();

		addPermissionCheck(parameter.AuthCheck, permissionCheck);
	  }

	  public virtual void configureDecisionDefinitionQuery(DecisionDefinitionQueryImpl query)
	  {
		configureQuery(query, DECISION_DEFINITION, "RES.KEY_");
	  }

	  public virtual void configureDecisionRequirementsDefinitionQuery(DecisionRequirementsDefinitionQueryImpl query)
	  {
		configureQuery(query, DECISION_REQUIREMENTS_DEFINITION, "RES.KEY_");
	  }

	  public virtual void configureBatchQuery(BatchQueryImpl query)
	  {
		configureQuery(query, BATCH, "RES.ID_", READ);
	  }

	  public virtual void configureBatchStatisticsQuery(BatchStatisticsQueryImpl query)
	  {
		configureQuery(query, BATCH, "RES.ID_", READ);
	  }

	  public virtual IList<string> filterAuthenticatedGroupIds(IList<string> authenticatedGroupIds)
	  {
		if (authenticatedGroupIds == null || authenticatedGroupIds.Count == 0)
		{
		  return EMPTY_LIST;
		}
		else
		{
		  if (availableAuthorizedGroupIds == null)
		  {
			availableAuthorizedGroupIds = new HashSet<string>(DbEntityManager.selectList("selectAuthorizedGroupIds"));
		  }
		  ISet<string> copy = new HashSet<string>(availableAuthorizedGroupIds);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		  copy.retainAll(authenticatedGroupIds);
		  return new List<string>(copy);
		}
	  }

	  protected internal virtual bool AuthCheckExecuted
	  {
		  get
		  {
    
			Authentication currentAuthentication = CurrentAuthentication;
			CommandContext commandContext = Context.CommandContext;
    
			return AuthorizationEnabled && commandContext.AuthorizationCheckEnabled && currentAuthentication != null && !string.ReferenceEquals(currentAuthentication.UserId, null);
    
		  }
	  }

	  public virtual bool EnsureSpecificVariablePermission
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.EnforceSpecificVariablePermission;
		  }
	  }

	}

}