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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SaveAuthorizationCmd : Command<Authorization>
	{

	  protected internal AuthorizationEntity authorization;

	  public SaveAuthorizationCmd(Authorization authorization)
	  {
		this.authorization = (AuthorizationEntity) authorization;
		validate();
	  }

	  protected internal virtual void validate()
	  {
		ensureOnlyOneNotNull("Authorization must either have a 'userId' or a 'groupId'.", authorization.UserId, authorization.GroupId);
		ensureNotNull("Authorization 'resourceType' cannot be null.", "authorization.getResource()", authorization.getResource());
	  }

	  public virtual Authorization execute(CommandContext commandContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager authorizationManager = commandContext.getAuthorizationManager();
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;

		authorizationManager.validateResourceCompatibility(authorization);

		string operationType = null;
		AuthorizationEntity previousValues = null;
		if (string.ReferenceEquals(authorization.Id, null))
		{
		  authorizationManager.insert(authorization);
		  operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;
		}
		else
		{
		  previousValues = commandContext.DbEntityManager.selectById(typeof(AuthorizationEntity), authorization.Id);
		  authorizationManager.update(authorization);
		  operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;
		}
		commandContext.OperationLogManager.logAuthorizationOperation(operationType, authorization, previousValues);

		return authorization;
	  }

	}

}