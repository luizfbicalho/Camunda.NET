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
namespace org.camunda.bpm.engine.impl.management
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using ActivateJobDefinitionCmd = org.camunda.bpm.engine.impl.cmd.ActivateJobDefinitionCmd;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using SuspendJobDefinitionCmd = org.camunda.bpm.engine.impl.cmd.SuspendJobDefinitionCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using UpdateJobDefinitionSuspensionStateBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateBuilder;
	using UpdateJobDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateSelectBuilder;
	using UpdateJobDefinitionSuspensionStateTenantBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateTenantBuilder;

	public class UpdateJobDefinitionSuspensionStateBuilderImpl : UpdateJobDefinitionSuspensionStateBuilder, UpdateJobDefinitionSuspensionStateSelectBuilder, UpdateJobDefinitionSuspensionStateTenantBuilder
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal readonly CommandExecutor commandExecutor;

	  protected internal string jobDefinitionId;

	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionTenantId_Renamed;
	  protected internal bool isProcessDefinitionTenantIdSet = false;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeJobs_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executionDate_Renamed;

	  public UpdateJobDefinitionSuspensionStateBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  /// <summary>
	  /// Creates a builder without CommandExecutor which can not be used to update
	  /// the suspension state via <seealso cref="#activate()"/> or <seealso cref="#suspend()"/>. Can only be
	  /// used in combination with your own command.
	  /// </summary>
	  public UpdateJobDefinitionSuspensionStateBuilderImpl() : this(null)
	  {
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl byJobDefinitionId(string jobDefinitionId)
	  {
		ensureNotNull("jobDefinitionId", jobDefinitionId);
		this.jobDefinitionId = jobDefinitionId;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl byProcessDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId = processDefinitionId;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl byProcessDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl processDefinitionWithoutTenantId()
	  {
		this.processDefinitionTenantId_Renamed = null;
		this.isProcessDefinitionTenantIdSet = true;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl processDefinitionTenantId(string tenantId)
	  {
		ensureNotNull("tenantId", tenantId);

		this.processDefinitionTenantId_Renamed = tenantId;
		this.isProcessDefinitionTenantIdSet = true;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl includeJobs(bool includeJobs)
	  {
		this.includeJobs_Renamed = includeJobs;
		return this;
	  }

	  public virtual UpdateJobDefinitionSuspensionStateBuilderImpl executionDate(DateTime executionDate)
	  {
		this.executionDate_Renamed = executionDate;
		return this;
	  }


	  public virtual void activate()
	  {
		validateParameters();

		ActivateJobDefinitionCmd command = new ActivateJobDefinitionCmd(this);
		commandExecutor.execute(command);
	  }

	  public virtual void suspend()
	  {
		validateParameters();

		SuspendJobDefinitionCmd command = new SuspendJobDefinitionCmd(this);
		commandExecutor.execute(command);
	  }

	  protected internal virtual void validateParameters()
	  {
		ensureOnlyOneNotNull("Need to specify either a job definition id, a process definition id or a process definition key.", jobDefinitionId, processDefinitionId, processDefinitionKey);

		if (isProcessDefinitionTenantIdSet && (!string.ReferenceEquals(jobDefinitionId, null) || !string.ReferenceEquals(processDefinitionId, null)))
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

	  public virtual string ProcessDefinitionTenantId
	  {
		  get
		  {
			return processDefinitionTenantId_Renamed;
		  }
	  }

	  public virtual bool ProcessDefinitionTenantIdSet
	  {
		  get
		  {
			return isProcessDefinitionTenantIdSet;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
	  }

	  public virtual bool IncludeJobs
	  {
		  get
		  {
			return includeJobs_Renamed;
		  }
	  }

	  public virtual DateTime ExecutionDate
	  {
		  get
		  {
			return executionDate_Renamed;
		  }
	  }

	}

}