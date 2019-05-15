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
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public abstract class AbstractSetJobStateCmd : AbstractSetStateCmd
	{

	  protected internal string jobId;
	  protected internal string jobDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

	  protected internal string processDefinitionTenantId;
	  protected internal bool processDefinitionTenantIdSet = false;

	  public AbstractSetJobStateCmd(UpdateJobSuspensionStateBuilderImpl builder) : base(false, null)
	  {

		this.jobId = builder.JobId;
		this.jobDefinitionId = builder.JobDefinitionId;
		this.processInstanceId = builder.ProcessInstanceId;
		this.processDefinitionId = builder.ProcessDefinitionId;
		this.processDefinitionKey = builder.ProcessDefinitionKey;

		this.processDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
		this.processDefinitionTenantId = builder.ProcessDefinitionTenantId;
	  }

	  protected internal override void checkParameters(CommandContext commandContext)
	  {
		if (string.ReferenceEquals(jobId, null) && string.ReferenceEquals(jobDefinitionId, null) && string.ReferenceEquals(processInstanceId, null) && string.ReferenceEquals(processDefinitionId, null) && string.ReferenceEquals(processDefinitionKey, null))
		{
		  throw new ProcessEngineException("Job id, job definition id, process instance id, process definition id nor process definition key cannot be null");
		}
	  }

	  protected internal override void checkAuthorization(CommandContext commandContext)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (!string.ReferenceEquals(jobId, null))
		  {

			JobManager jobManager = commandContext.JobManager;
			JobEntity job = jobManager.findJobById(jobId);

			if (job != null)
			{

			  string processInstanceId = job.ProcessInstanceId;
			  if (!string.ReferenceEquals(processInstanceId, null))
			  {
				checker.checkUpdateProcessInstanceById(processInstanceId);
			  }
			  else
			  {
				// start timer job is not assigned to a specific process
				// instance, that's why we have to check whether there
				// exists a UPDATE_INSTANCES permission on process definition or
				// a UPDATE permission on any process instance
				string processDefinitionKey = job.ProcessDefinitionKey;
				if (!string.ReferenceEquals(processDefinitionKey, null))
				{
				  checker.checkUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
				}
			  }
			  // if (processInstanceId == null && processDefinitionKey == null):
			  // job is not assigned to any process instance nor process definition
			  // then it is always possible to activate/suspend the corresponding job
			  // -> no authorization check necessary
			}
		  }
		  else
		  {

		  if (!string.ReferenceEquals(jobDefinitionId, null))
		  {

			JobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
			JobDefinitionEntity jobDefinition = jobDefinitionManager.findById(jobDefinitionId);

			if (jobDefinition != null)
			{
			  string processDefinitionKey = jobDefinition.ProcessDefinitionKey;
			  checker.checkUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
			}

		  }
		  else
		  {

		  if (!string.ReferenceEquals(processInstanceId, null))
		  {
			checker.checkUpdateProcessInstanceById(processInstanceId);
		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			checker.checkUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionKey, null))
		  {
			checker.checkUpdateProcessInstanceByProcessDefinitionKey(processDefinitionKey);
		  }
		  }
		  }
		  }
		  }
		}
	  }

	  protected internal override void updateSuspensionState(CommandContext commandContext, SuspensionState suspensionState)
	  {
		JobManager jobManager = commandContext.JobManager;

		if (!string.ReferenceEquals(jobId, null))
		{
		  jobManager.updateJobSuspensionStateById(jobId, suspensionState);

		}
		else if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  jobManager.updateJobSuspensionStateByJobDefinitionId(jobDefinitionId, suspensionState);

		}
		else if (!string.ReferenceEquals(processInstanceId, null))
		{
		  jobManager.updateJobSuspensionStateByProcessInstanceId(processInstanceId, suspensionState);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  jobManager.updateJobSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{

		  if (!processDefinitionTenantIdSet)
		  {
			jobManager.updateJobSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);

		  }
		  else
		  {
			jobManager.updateJobSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);
		  }
		}
	  }

	  protected internal override void logUserOperation(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange(SUSPENSION_STATE_PROPERTY, null, NewSuspensionState.Name);
		commandContext.OperationLogManager.logJobOperation(LogEntryOperation, jobId, jobDefinitionId, processInstanceId, processDefinitionId, processDefinitionKey, propertyChange);
	  }
	}

}