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
namespace org.camunda.bpm.engine.impl.cmd
{
	using static org.camunda.bpm.engine.impl.util.EnsureUtil;

	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;

	/// <summary>
	/// <para>Command allowing to perform an authorization check</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationCheckCmd : Command<bool>
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string userId;
	  protected internal IList<string> groupIds;
	  protected internal Permission permission;
	  protected internal Resource resource;
	  protected internal string resourceId;

	  public AuthorizationCheckCmd(string userId, IList<string> groupIds, Permission permission, Resource resource, string resourceId)
	  {
		this.userId = userId;
		this.groupIds = groupIds;
		this.permission = permission;
		this.resource = resource;
		this.resourceId = resourceId;
		validate(userId, groupIds, permission, resource);
	  }

	  public virtual bool? execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager authorizationManager = commandContext.getAuthorizationManager();
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;
		if (authorizationManager.isPermissionDisabled(permission))
		{
		  throw LOG.disabledPermissionException(permission.Name);
		}
		return authorizationManager.isAuthorized(userId, groupIds, permission, resource, resourceId);
	  }

	  protected internal virtual void validate(string userId, IList<string> groupIds, Permission permission, Resource resource)
	  {
		ensureAtLeastOneNotNull("Authorization must have a 'userId' or/and a 'groupId'.", userId, groupIds);
		ensureNotNull("Invalid permission for an authorization", "authorization.getResource()", permission);
		ensureNotNull("Invalid resource for an authorization", "authorization.getResource()", resource);
	  }

	}

}