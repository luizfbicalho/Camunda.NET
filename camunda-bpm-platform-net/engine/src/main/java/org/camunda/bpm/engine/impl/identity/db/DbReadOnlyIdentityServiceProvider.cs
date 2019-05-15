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
namespace org.camunda.bpm.engine.impl.identity.db
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EncryptionUtil.saltPassword;


	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using TenantEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantEntity;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;

	/// <summary>
	/// <para>Read only implementation of DB-backed identity service</para>
	/// 
	/// @author Daniel Meyer
	/// @author nico.rehwaldt
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public class DbReadOnlyIdentityServiceProvider extends org.camunda.bpm.engine.impl.persistence.AbstractManager implements org.camunda.bpm.engine.impl.identity.ReadOnlyIdentityProvider
	public class DbReadOnlyIdentityServiceProvider : AbstractManager, ReadOnlyIdentityProvider
	{

	  // users /////////////////////////////////////////

	  public virtual UserEntity findUserById(string userId)
	  {
		checkAuthorization(Permissions.READ, Resources.USER, userId);
		return DbEntityManager.selectById(typeof(UserEntity), userId);
	  }

	  public virtual UserQuery createUserQuery()
	  {
		return new DbUserQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual UserQueryImpl createUserQuery(CommandContext commandContext)
	  {
		return new DbUserQueryImpl();
	  }

	  public virtual NativeUserQuery createNativeUserQuery()
	  {
		return new NativeUserQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual long findUserCountByQueryCriteria(DbUserQueryImpl query)
	  {
		configureQuery(query, Resources.USER);
		return (long?) DbEntityManager.selectOne("selectUserCountByQueryCriteria", query).Value;
	  }

	  public virtual IList<User> findUserByQueryCriteria(DbUserQueryImpl query)
	  {
		configureQuery(query, Resources.USER);
		return DbEntityManager.selectList("selectUserByQueryCriteria", query);
	  }

	  public virtual IList<User> findUserByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectUserByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findUserCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectUserCountByNativeQuery", parameterMap).Value;
	  }

	  public virtual bool checkPassword(string userId, string password)
	  {
		UserEntity user = findUserById(userId);
		if ((user != null) && (!string.ReferenceEquals(password, null)) && matchPassword(password, user))
		{
		  return true;
		}
		else
		{
		  return false;
		}
	  }

	  protected internal virtual bool matchPassword(string password, UserEntity user)
	  {
		string saltedPassword = saltPassword(password, user.Salt);
		return Context.ProcessEngineConfiguration.PasswordManager.check(saltedPassword, user.Password);
	  }

	  // groups //////////////////////////////////////////

	  public virtual GroupEntity findGroupById(string groupId)
	  {
		checkAuthorization(Permissions.READ, Resources.GROUP, groupId);
		return DbEntityManager.selectById(typeof(GroupEntity), groupId);
	  }

	  public virtual GroupQuery createGroupQuery()
	  {
		return new DbGroupQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual GroupQuery createGroupQuery(CommandContext commandContext)
	  {
		return new DbGroupQueryImpl();
	  }

	  public virtual long findGroupCountByQueryCriteria(DbGroupQueryImpl query)
	  {
		configureQuery(query, Resources.GROUP);
		return (long?) DbEntityManager.selectOne("selectGroupCountByQueryCriteria", query).Value;
	  }

	  public virtual IList<Group> findGroupByQueryCriteria(DbGroupQueryImpl query)
	  {
		configureQuery(query, Resources.GROUP);
		return DbEntityManager.selectList("selectGroupByQueryCriteria", query);
	  }

	  //tenants //////////////////////////////////////////

	  public virtual TenantEntity findTenantById(string tenantId)
	  {
		checkAuthorization(Permissions.READ, Resources.TENANT, tenantId);
		return DbEntityManager.selectById(typeof(TenantEntity), tenantId);
	  }

	  public virtual TenantQuery createTenantQuery()
	  {
		return new DbTenantQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
	  }

	  public virtual TenantQuery createTenantQuery(CommandContext commandContext)
	  {
		return new DbTenantQueryImpl();
	  }

	  public virtual long findTenantCountByQueryCriteria(DbTenantQueryImpl query)
	  {
		configureQuery(query, Resources.TENANT);
		return (long?) DbEntityManager.selectOne("selectTenantCountByQueryCriteria", query).Value;
	  }

	  public virtual IList<Tenant> findTenantByQueryCriteria(DbTenantQueryImpl query)
	  {
		configureQuery(query, Resources.TENANT);
		return DbEntityManager.selectList("selectTenantByQueryCriteria", query);
	  }

	  //memberships //////////////////////////////////////////
	  protected internal virtual bool existsMembership(string userId, string groupId)
	  {
		IDictionary<string, string> key = new Dictionary<string, string>();
		key["userId"] = userId;
		key["groupId"] = groupId;
		return ((long?) DbEntityManager.selectOne("selectMembershipCount", key)) > 0;
	  }

	  protected internal virtual bool existsTenantMembership(string tenantId, string userId, string groupId)
	  {
		IDictionary<string, string> key = new Dictionary<string, string>();
		key["tenantId"] = tenantId;
		if (!string.ReferenceEquals(userId, null))
		{
		  key["userId"] = userId;
		}
		if (!string.ReferenceEquals(groupId, null))
		{
		  key["groupId"] = groupId;
		}
		return ((long?) DbEntityManager.selectOne("selectTenantMembershipCount", key)) > 0;
	  }

	  //authorizations ////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override protected void configureQuery(@SuppressWarnings("rawtypes") org.camunda.bpm.engine.impl.AbstractQuery query, org.camunda.bpm.engine.authorization.Resource resource)
	  protected internal override void configureQuery(AbstractQuery query, Resource resource)
	  {
		Context.CommandContext.AuthorizationManager.configureQuery(query, resource);
	  }

	  protected internal override void checkAuthorization(Permission permission, Resource resource, string resourceId)
	  {
		Context.CommandContext.AuthorizationManager.checkAuthorization(permission, resource, resourceId);
	  }

	}

}