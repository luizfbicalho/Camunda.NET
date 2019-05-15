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
	using AuthorizationCheckCmd = org.camunda.bpm.engine.impl.cmd.AuthorizationCheckCmd;
	using CreateAuthorizationCommand = org.camunda.bpm.engine.impl.cmd.CreateAuthorizationCommand;
	using DeleteAuthorizationCmd = org.camunda.bpm.engine.impl.cmd.DeleteAuthorizationCmd;
	using SaveAuthorizationCmd = org.camunda.bpm.engine.impl.cmd.SaveAuthorizationCmd;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationServiceImpl : ServiceImpl, AuthorizationService
	{

	  public virtual AuthorizationQuery createAuthorizationQuery()
	  {
		return new AuthorizationQueryImpl(commandExecutor);
	  }

	  public virtual Authorization createNewAuthorization(int type)
	  {
		return commandExecutor.execute(new CreateAuthorizationCommand(type));
	  }

	  public virtual Authorization saveAuthorization(Authorization authorization)
	  {
		return commandExecutor.execute(new SaveAuthorizationCmd(authorization));
	  }

	  public virtual void deleteAuthorization(string authorizationId)
	  {
		commandExecutor.execute(new DeleteAuthorizationCmd(authorizationId));
	  }

	  public virtual bool isUserAuthorized(string userId, IList<string> groupIds, Permission permission, Resource resource)
	  {
		return commandExecutor.execute(new AuthorizationCheckCmd(userId, groupIds, permission, resource, null));
	  }

	  public virtual bool isUserAuthorized(string userId, IList<string> groupIds, Permission permission, Resource resource, string resourceId)
	  {
		return commandExecutor.execute(new AuthorizationCheckCmd(userId, groupIds, permission, resource, resourceId));
	  }

	}

}