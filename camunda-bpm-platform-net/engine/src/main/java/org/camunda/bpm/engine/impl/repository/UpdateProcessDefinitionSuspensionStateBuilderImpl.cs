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
namespace org.camunda.bpm.engine.impl.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using ActivateProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.ActivateProcessDefinitionCmd;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using SuspendProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.SuspendProcessDefinitionCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using UpdateProcessDefinitionSuspensionStateBuilder = org.camunda.bpm.engine.repository.UpdateProcessDefinitionSuspensionStateBuilder;
	using UpdateProcessDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.repository.UpdateProcessDefinitionSuspensionStateSelectBuilder;
	using UpdateProcessDefinitionSuspensionStateTenantBuilder = org.camunda.bpm.engine.repository.UpdateProcessDefinitionSuspensionStateTenantBuilder;

	public class UpdateProcessDefinitionSuspensionStateBuilderImpl : UpdateProcessDefinitionSuspensionStateBuilder, UpdateProcessDefinitionSuspensionStateSelectBuilder, UpdateProcessDefinitionSuspensionStateTenantBuilder
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal readonly CommandExecutor commandExecutor;

	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeProcessInstances_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executionDate_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionTenantId_Conflict;
	  protected internal bool isTenantIdSet = false;

	  public UpdateProcessDefinitionSuspensionStateBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  /// <summary>
	  /// Creates a builder without CommandExecutor which can not be used to update
	  /// the suspension state via <seealso cref="activate()"/> or <seealso cref="suspend()"/>. Can be
	  /// used in combination with your own command.
	  /// </summary>
	  public UpdateProcessDefinitionSuspensionStateBuilderImpl() : this(null)
	  {
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl byProcessDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId = processDefinitionId;
		return this;
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl byProcessDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl includeProcessInstances(bool includeProcessInstance)
	  {
		this.includeProcessInstances_Conflict = includeProcessInstance;
		return this;
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl executionDate(DateTime date)
	  {
		this.executionDate_Conflict = date;
		return this;
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl processDefinitionWithoutTenantId()
	  {
		this.processDefinitionTenantId_Conflict = null;
		this.isTenantIdSet = true;
		return this;
	  }

	  public virtual UpdateProcessDefinitionSuspensionStateBuilderImpl processDefinitionTenantId(string tenantId)
	  {
		ensureNotNull("tenantId", tenantId);

		this.processDefinitionTenantId_Conflict = tenantId;
		this.isTenantIdSet = true;
		return this;
	  }

	  public virtual void activate()
	  {
		validateParameters();

		ActivateProcessDefinitionCmd command = new ActivateProcessDefinitionCmd(this);
		commandExecutor.execute(command);
	  }

	  public virtual void suspend()
	  {
		validateParameters();

		SuspendProcessDefinitionCmd command = new SuspendProcessDefinitionCmd(this);
		commandExecutor.execute(command);
	  }

	  protected internal virtual void validateParameters()
	  {
		ensureOnlyOneNotNull("Need to specify either a process instance id or a process definition key.", processDefinitionId, processDefinitionKey);

		if (!string.ReferenceEquals(processDefinitionId, null) && isTenantIdSet)
		{
		  throw LOG.exceptionUpdateSuspensionStateForTenantOnlyByProcessDefinitionKey();
		}

		ensureNotNull("commandExecutor", commandExecutor);
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual bool IncludeProcessInstances
	  {
		  get
		  {
			return includeProcessInstances_Conflict;
		  }
	  }

	  public virtual DateTime ExecutionDate
	  {
		  get
		  {
			return executionDate_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionTenantId
	  {
		  get
		  {
			return processDefinitionTenantId_Conflict;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	}

}