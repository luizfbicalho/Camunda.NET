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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using ResourceTypeUtil = org.camunda.bpm.engine.impl.util.ResourceTypeUtil;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class AuthorizationEntity : Authorization, DbEntity, HasDbRevision, HasDbReferences
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal int authorizationType;
	  protected internal int permissions;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal int? resourceType;
	  protected internal string resourceId;

	  private ISet<Permission> cachedPermissions = new HashSet<Permission>();

	  public AuthorizationEntity()
	  {
	  }

	  public AuthorizationEntity(int type)
	  {
		this.authorizationType = type;

		if (authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL)
		{
		  this.userId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		}

		resetPermissions();
	  }

	  protected internal virtual void resetPermissions()
	  {
		cachedPermissions = new HashSet<>();

		if (authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL)
		{
		  this.permissions = Permissions.NONE.Value;

		}
		else if (authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)
		{
		  this.permissions = Permissions.NONE.Value;

		}
		else if (authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE)
		{
		  this.permissions = Permissions.ALL.Value;

		}
		else
		{
		  throw LOG.engineAuthorizationTypeException(authorizationType, org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL, org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT, org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE);
		}
	  }

	  // grant / revoke methods ////////////////////////////

	  public virtual void addPermission(Permission p)
	  {
		cachedPermissions.Add(p);
		permissions |= p.Value;
	  }

	  public virtual void removePermission(Permission p)
	  {
		cachedPermissions.Add(p);
		permissions &= ~p.Value;
	  }

	  public virtual bool isPermissionGranted(Permission p)
	  {
		if (org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE == authorizationType)
		{
		  throw LOG.permissionStateException("isPermissionGranted", "REVOKE");
		}

		ensureNotNull("Authorization 'resourceType' cannot be null", "authorization.getResource()", resourceType);

		if (!ResourceTypeUtil.resourceIsContainedInArray(resourceType, p.Types))
		{
		  return false;
		}
		return (permissions & p.Value) == p.Value;
	  }

	  public virtual bool isPermissionRevoked(Permission p)
	  {
		if (org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT == authorizationType)
		{
		  throw LOG.permissionStateException("isPermissionRevoked", "GRANT");
		}

		ensureNotNull("Authorization 'resourceType' cannot be null", "authorization.getResource()", resourceType);

		if (!ResourceTypeUtil.resourceIsContainedInArray(resourceType, p.Types))
		{
		  return false;
		}
		return (permissions & p.Value) != p.Value;
	  }

	  public virtual bool EveryPermissionGranted
	  {
		  get
		  {
			if (org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE == authorizationType)
			{
			  throw LOG.permissionStateException("isEveryPermissionGranted", "REVOKE");
			}
			return permissions == Permissions.ALL.Value;
		  }
	  }

	  public virtual bool EveryPermissionRevoked
	  {
		  get
		  {
			if (authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)
			{
			  throw LOG.permissionStateException("isEveryPermissionRevoked", "GRANT");
			}
			return permissions == 0;
		  }
	  }

	  public virtual Permission[] getPermissions(Permission[] permissions)
	  {

		IList<Permission> result = new List<Permission>();

		foreach (Permission permission in permissions)
		{
		  if ((org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL == authorizationType || org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT == authorizationType) && isPermissionGranted(permission))
		  {

			result.Add(permission);

		  }
		  else if (org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE == authorizationType && isPermissionRevoked(permission))
		  {

			result.Add(permission);

		  }
		}
		return result.ToArray();
	  }

	  public virtual void setPermissions(Permission[] permissions)
	  {
		resetPermissions();
		foreach (Permission permission in permissions)
		{
		  if (org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE == authorizationType)
		  {
			removePermission(permission);

		  }
		  else
		  {
			addPermission(permission);

		  }
		}
	  }

	  // getters setters ///////////////////////////////

	  public virtual int AuthorizationType
	  {
		  get
		  {
			return authorizationType;
		  }
		  set
		  {
			this.authorizationType = value;
		  }
	  }


	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
		  set
		  {
			if (!string.ReferenceEquals(value, null) && authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL)
			{
			  throw LOG.notUsableGroupIdForGlobalAuthorizationException();
			}
			this.groupId = value;
		  }
	  }


	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			if (!string.ReferenceEquals(value, null) && authorizationType == org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL && !org.camunda.bpm.engine.authorization.Authorization_Fields.ANY.Equals(value))
			{
			  throw LOG.illegalValueForUserIdException(value, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY);
			}
			this.userId = value;
		  }
	  }


	  public virtual int ResourceType
	  {
		  get
		  {
			return resourceType.Value;
		  }
		  set
		  {
			this.resourceType = value;
		  }
	  }


	  public virtual int? getResource()
	  {
		return resourceType;
	  }

	  public virtual void setResource(Resource resource)
	  {
		this.resourceType = resource.resourceType();
	  }

	  public virtual string ResourceId
	  {
		  get
		  {
			return resourceId;
		  }
		  set
		  {
			this.resourceId = value;
		  }
	  }


	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }

	  public virtual void setPermissions(int permissions)
	  {
		this.permissions = permissions;
	  }

	  public virtual int getPermissions()
	  {
		return permissions;
	  }

	  public virtual ISet<Permission> CachedPermissions
	  {
		  get
		  {
			return cachedPermissions;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
    
			Dictionary<string, object> state = new Dictionary<string, object>();
			state["userId"] = userId;
			state["groupId"] = groupId;
			state["resourceType"] = resourceType;
			state["resourceId"] = resourceId;
			state["permissions"] = permissions;
    
			return state;
		  }
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
			return referenceIdAndClass;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", authorizationType=" + authorizationType + ", permissions=" + permissions + ", userId=" + userId + ", groupId=" + groupId + ", resourceType=" + resourceType + ", resourceId=" + resourceId + "]";
	  }
	}

}