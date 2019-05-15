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

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using JobDefinitionSuspensionStateConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerChangeJobDefinitionSuspensionStateJobHandler.JobDefinitionSuspensionStateConfiguration;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	/// <summary>
	/// @author Daniel Meyer
	/// @author roman.smirnov
	/// </summary>
	public abstract class AbstractSetJobDefinitionStateCmd : AbstractSetStateCmd
	{

	  protected internal string jobDefinitionId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal new DateTime executionDate;

	  protected internal string processDefinitionTenantId;
	  protected internal bool isProcessDefinitionTenantIdSet = false;

	  public AbstractSetJobDefinitionStateCmd(UpdateJobDefinitionSuspensionStateBuilderImpl builder) : base(builder.IncludeJobs, builder.ExecutionDate)
	  {

		this.jobDefinitionId = builder.JobDefinitionId;
		this.processDefinitionId = builder.ProcessDefinitionId;
		this.processDefinitionKey = builder.ProcessDefinitionKey;

		this.isProcessDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
		this.processDefinitionTenantId = builder.ProcessDefinitionTenantId;
	  }

	  protected internal override void checkParameters(CommandContext commandContext)
	  {
		if (string.ReferenceEquals(jobDefinitionId, null) && string.ReferenceEquals(processDefinitionId, null) && string.ReferenceEquals(processDefinitionKey, null))
		{
		  throw new ProcessEngineException("Job definition id, process definition id nor process definition key cannot be null");
		}
	  }

	  protected internal override void checkAuthorization(CommandContext commandContext)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (!string.ReferenceEquals(jobDefinitionId, null))
		  {

			JobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
			JobDefinitionEntity jobDefinition = jobDefinitionManager.findById(jobDefinitionId);

			if (jobDefinition != null && !string.ReferenceEquals(jobDefinition.ProcessDefinitionKey, null))
			{
			  string processDefinitionKey = jobDefinition.ProcessDefinitionKey;
			  checker.checkUpdateProcessDefinitionByKey(processDefinitionKey);

			  if (includeSubResources)
			  {
				checker.checkUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
			  }
			}

		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			checker.checkUpdateProcessDefinitionById(processDefinitionId);

			if (includeSubResources)
			{
			  checker.checkUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
			}

		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionKey, null))
		  {
			checker.checkUpdateProcessDefinitionByKey(processDefinitionKey);

			if (includeSubResources)
			{
			  checker.checkUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
			}
		  }
		  }
		  }
		}
	  }

	  protected internal override void updateSuspensionState(CommandContext commandContext, SuspensionState suspensionState)
	  {
		JobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
		JobManager jobManager = commandContext.JobManager;

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  jobDefinitionManager.updateJobDefinitionSuspensionStateById(jobDefinitionId, suspensionState);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  jobDefinitionManager.updateJobDefinitionSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);
		  jobManager.updateStartTimerJobSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{

		  if (!isProcessDefinitionTenantIdSet)
		  {
			jobDefinitionManager.updateJobDefinitionSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);
			jobManager.updateStartTimerJobSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);

		  }
		  else
		  {
			jobDefinitionManager.updateJobDefinitionSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);
			jobManager.updateStartTimerJobSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);
		  }
		}
	  }

	  protected internal override JobHandlerConfiguration JobHandlerConfiguration
	  {
		  get
		  {
    
			if (!string.ReferenceEquals(jobDefinitionId, null))
			{
			  return JobDefinitionSuspensionStateConfiguration.byJobDefinitionId(jobDefinitionId, IncludeSubResources);
    
			}
			else if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return JobDefinitionSuspensionStateConfiguration.byProcessDefinitionId(processDefinitionId, IncludeSubResources);
    
			}
			else
			{
    
			  if (!isProcessDefinitionTenantIdSet)
			  {
				return JobDefinitionSuspensionStateConfiguration.byProcessDefinitionKey(processDefinitionKey, IncludeSubResources);
    
			  }
			  else
			  {
				return JobDefinitionSuspensionStateConfiguration.ByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, IncludeSubResources);
			  }
			}
    
		  }
	  }

	  protected internal override void logUserOperation(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange(SUSPENSION_STATE_PROPERTY, null, NewSuspensionState.Name);
		commandContext.OperationLogManager.logJobDefinitionOperation(LogEntryOperation, jobDefinitionId, processDefinitionId, processDefinitionKey, propertyChange);
	  }

	  protected internal virtual UpdateJobSuspensionStateBuilderImpl createJobCommandBuilder()
	  {
		UpdateJobSuspensionStateBuilderImpl builder = new UpdateJobSuspensionStateBuilderImpl();

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  builder.byJobDefinitionId(jobDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  builder.byProcessDefinitionId(processDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  builder.byProcessDefinitionKey(processDefinitionKey);

		  if (isProcessDefinitionTenantIdSet && !string.ReferenceEquals(processDefinitionTenantId, null))
		  {
			builder.processDefinitionTenantId(processDefinitionTenantId);

		  }
		  else if (isProcessDefinitionTenantIdSet)
		  {
			builder.processDefinitionWithoutTenantId();
		  }
		}
		return builder;
	  }

	  /// <summary>
	  /// Subclasses should return the type of the <seealso cref="JobHandler"/> here. it will be used when
	  /// the user provides an execution date on which the actual state change will happen.
	  /// </summary>
	  protected internal override abstract string DelayedExecutionJobHandlerType {get;}

	  protected internal override AbstractSetStateCmd NextCommand
	  {
		  get
		  {
			UpdateJobSuspensionStateBuilderImpl jobCommandBuilder = createJobCommandBuilder();
    
			return getNextCommand(jobCommandBuilder);
		  }
	  }

	  protected internal abstract AbstractSetJobStateCmd getNextCommand(UpdateJobSuspensionStateBuilderImpl jobCommandBuilder);
	}

}