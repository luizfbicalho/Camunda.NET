using System;

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

	using IdentityOperationResult = org.camunda.bpm.engine.impl.identity.IdentityOperationResult;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	[Serializable]
	public class DeleteTenantGroupMembershipCmd : AbstractWritableIdentityServiceCmd<Void>, Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string tenantId;
	  protected internal readonly string groupId;

	  public DeleteTenantGroupMembershipCmd(string tenantId, string groupId)
	  {
		this.tenantId = tenantId;
		this.groupId = groupId;
	  }

	  protected internal override Void executeCmd(CommandContext commandContext)
	  {
		ensureNotNull("tenantId", tenantId);
		ensureNotNull("groupId", groupId);

		IdentityOperationResult operationResult = commandContext.WritableIdentityProvider.deleteTenantGroupMembership(tenantId, groupId);

		commandContext.OperationLogManager.logMembershipOperation(operationResult, null, groupId, tenantId);

		return null;
	  }

	}

}