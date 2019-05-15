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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DeleteAuthorizationCmd : Command<Void>
	{

	  protected internal string authorizationId;

	  public DeleteAuthorizationCmd(string authorizationId)
	  {
		this.authorizationId = authorizationId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager authorizationManager = commandContext.getAuthorizationManager();
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;

		AuthorizationEntity authorization = (AuthorizationEntity) (new AuthorizationQueryImpl()).authorizationId(authorizationId).singleResult();

		ensureNotNull("Authorization for Id '" + authorizationId + "' does not exist", "authorization", authorization);

		authorizationManager.delete(authorization);
		commandContext.OperationLogManager.logAuthorizationOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, authorization, null);

		return null;
	  }

	}

}