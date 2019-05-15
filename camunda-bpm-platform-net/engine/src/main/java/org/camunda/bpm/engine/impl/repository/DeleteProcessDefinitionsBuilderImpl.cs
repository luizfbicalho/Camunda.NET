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
namespace org.camunda.bpm.engine.impl.repository
{
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using DeleteProcessDefinitionsByIdsCmd = org.camunda.bpm.engine.impl.cmd.DeleteProcessDefinitionsByIdsCmd;
	using DeleteProcessDefinitionsByKeyCmd = org.camunda.bpm.engine.impl.cmd.DeleteProcessDefinitionsByKeyCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using DeleteProcessDefinitionsBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsBuilder;
	using DeleteProcessDefinitionsSelectBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsSelectBuilder;
	using DeleteProcessDefinitionsTenantBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsTenantBuilder;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	/// <summary>
	/// Fluent builder implementation to delete process definitions.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public class DeleteProcessDefinitionsBuilderImpl : DeleteProcessDefinitionsBuilder, DeleteProcessDefinitionsSelectBuilder, DeleteProcessDefinitionsTenantBuilder
	{

	  private readonly CommandExecutor commandExecutor;

	  private string processDefinitionKey;
	  private IList<string> processDefinitionIds;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private bool cascade_Renamed;
	  private string tenantId;
	  private bool isTenantIdSet;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private bool skipCustomListeners_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipIoMappings_Renamed;

	  public DeleteProcessDefinitionsBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl byIds(params string[] processDefinitionId)
	  {
		if (processDefinitionId != null)
		{
		  this.processDefinitionIds = new List<string>();
		  ((IList<string>)this.processDefinitionIds).AddRange(Arrays.asList(processDefinitionId));
		}
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl byKey(string processDefinitionKey)
	  {
		this.processDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl withoutTenantId()
	  {
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl withTenantId(string tenantId)
	  {
		ensureNotNull("tenantId", tenantId);
		isTenantIdSet = true;
		this.tenantId = tenantId;
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl cascade()
	  {
		this.cascade_Renamed = true;
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl skipCustomListeners()
	  {
		this.skipCustomListeners_Renamed = true;
		return this;
	  }

	  public virtual DeleteProcessDefinitionsBuilderImpl skipIoMappings()
	  {
		this.skipIoMappings_Renamed = true;
		return this;
	  }

	  public virtual void delete()
	  {
		ensureOnlyOneNotNull(typeof(NullValueException), "'processDefinitionKey' or 'processDefinitionIds' cannot be null", processDefinitionKey, processDefinitionIds);

		Command<Void> command;
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  command = new DeleteProcessDefinitionsByKeyCmd(processDefinitionKey, cascade_Renamed, skipCustomListeners_Renamed, skipIoMappings_Renamed, tenantId, isTenantIdSet);
		}
		else if (processDefinitionIds != null && processDefinitionIds.Count > 0)
		{
		  command = new DeleteProcessDefinitionsByIdsCmd(processDefinitionIds, cascade_Renamed, skipCustomListeners_Renamed, skipIoMappings_Renamed);
		}
		else
		{
		  return;
		}

		commandExecutor.execute(command);
	  }

	}

}