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
	using ProcessDefinitionSuspensionStateConfiguration = org.camunda.bpm.engine.impl.jobexecutor.TimerChangeProcessDefinitionSuspensionStateJobHandler.ProcessDefinitionSuspensionStateConfiguration;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using UpdateProcessDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.repository.UpdateProcessDefinitionSuspensionStateBuilderImpl;
	using UpdateProcessInstanceSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.runtime.UpdateProcessInstanceSuspensionStateBuilderImpl;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// @author roman.smirnov
	/// </summary>
	public abstract class AbstractSetProcessDefinitionStateCmd : AbstractSetStateCmd
	{

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

	  protected internal string tenantId;
	  protected internal bool isTenantIdSet = false;

	  public AbstractSetProcessDefinitionStateCmd(UpdateProcessDefinitionSuspensionStateBuilderImpl builder) : base(builder.IncludeProcessInstances, builder.ExecutionDate)
	  {

		this.processDefinitionId = builder.ProcessDefinitionId;
		this.processDefinitionKey = builder.ProcessDefinitionKey;

		this.isTenantIdSet = builder.TenantIdSet;
		this.tenantId = builder.ProcessDefinitionTenantId;
	  }

	  protected internal override void checkParameters(CommandContext commandContext)
	  {
		// Validation of input parameters
		if (string.ReferenceEquals(processDefinitionId, null) && string.ReferenceEquals(processDefinitionKey, null))
		{
		  throw new ProcessEngineException("Process definition id / key cannot be null");
		}
	  }

	  protected internal override void checkAuthorization(CommandContext commandContext)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			checker.checkUpdateProcessDefinitionSuspensionStateById(processDefinitionId);

			if (includeSubResources)
			{
			  checker.checkUpdateProcessInstanceSuspensionStateByProcessDefinitionId(processDefinitionId);
			}
		  }
		  else
		  {

			if (!string.ReferenceEquals(processDefinitionKey, null))
			{
			  checker.checkUpdateProcessDefinitionSuspensionStateByKey(processDefinitionKey);

			  if (includeSubResources)
			  {
				checker.checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(processDefinitionKey);
			  }
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected void updateSuspensionState(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, org.camunda.bpm.engine.impl.persistence.entity.SuspensionState suspensionState)
	  protected internal override void updateSuspensionState(CommandContext commandContext, SuspensionState suspensionState)
	  {
		ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  processDefinitionManager.updateProcessDefinitionSuspensionStateById(processDefinitionId, suspensionState);

		}
		else if (isTenantIdSet)
		{
		  processDefinitionManager.updateProcessDefinitionSuspensionStateByKeyAndTenantId(processDefinitionKey, tenantId, suspensionState);

		}
		else
		{
		  processDefinitionManager.updateProcessDefinitionSuspensionStateByKey(processDefinitionKey, suspensionState);
		}

		commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly AbstractSetProcessDefinitionStateCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(AbstractSetProcessDefinitionStateCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionSuspensionStateBuilder = outerInstance.createJobDefinitionCommandBuilder();
			AbstractSetJobDefinitionStateCmd jobDefinitionCmd = outerInstance.getSetJobDefinitionStateCmd(jobDefinitionSuspensionStateBuilder);
			jobDefinitionCmd.disableLogUserOperation();
			jobDefinitionCmd.execute(commandContext);
			return null;
		  }
	  }

	  protected internal virtual UpdateJobDefinitionSuspensionStateBuilderImpl createJobDefinitionCommandBuilder()
	  {
		UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionBuilder = new UpdateJobDefinitionSuspensionStateBuilderImpl();

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  jobDefinitionBuilder.byProcessDefinitionId(processDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  jobDefinitionBuilder.byProcessDefinitionKey(processDefinitionKey);

		  if (isTenantIdSet && !string.ReferenceEquals(tenantId, null))
		  {
			jobDefinitionBuilder.processDefinitionTenantId(tenantId);

		  }
		  else if (isTenantIdSet)
		  {
			jobDefinitionBuilder.processDefinitionWithoutTenantId();
		  }
		}
		return jobDefinitionBuilder;
	  }

	  protected internal virtual UpdateProcessInstanceSuspensionStateBuilderImpl createProcessInstanceCommandBuilder()
	  {
		UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceBuilder = new UpdateProcessInstanceSuspensionStateBuilderImpl();

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  processInstanceBuilder.byProcessDefinitionId(processDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  processInstanceBuilder.byProcessDefinitionKey(processDefinitionKey);

		  if (isTenantIdSet && !string.ReferenceEquals(tenantId, null))
		  {
			processInstanceBuilder.processDefinitionTenantId(tenantId);

		  }
		  else if (isTenantIdSet)
		  {
			processInstanceBuilder.processDefinitionWithoutTenantId();
		  }
		}
		return processInstanceBuilder;
	  }

	  protected internal override JobHandlerConfiguration JobHandlerConfiguration
	  {
		  get
		  {
    
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return ProcessDefinitionSuspensionStateConfiguration.byProcessDefinitionId(processDefinitionId, IncludeSubResources);
    
			}
			else if (isTenantIdSet)
			{
			  return ProcessDefinitionSuspensionStateConfiguration.byProcessDefinitionKeyAndTenantId(processDefinitionKey, tenantId, IncludeSubResources);
    
			}
			else
			{
			  return ProcessDefinitionSuspensionStateConfiguration.byProcessDefinitionKey(processDefinitionKey, IncludeSubResources);
			}
		  }
	  }

	  protected internal override void logUserOperation(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange(SUSPENSION_STATE_PROPERTY, null, NewSuspensionState.Name);
		commandContext.OperationLogManager.logProcessDefinitionOperation(LogEntryOperation, processDefinitionId, processDefinitionKey, propertyChange);
	  }

	  // ABSTRACT METHODS ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Subclasses should return the type of the <seealso cref="JobHandler"/> here. it will be used when
	  /// the user provides an execution date on which the actual state change will happen.
	  /// </summary>
	  protected internal override abstract string DelayedExecutionJobHandlerType {get;}

	  /// <summary>
	  /// Subclasses should return the type of the <seealso cref="AbstractSetJobDefinitionStateCmd"/> here.
	  /// It will be used to suspend or activate the <seealso cref="JobDefinition"/>s. </summary>
	  /// <param name="jobDefinitionSuspensionStateBuilder"> </param>
	  protected internal abstract AbstractSetJobDefinitionStateCmd getSetJobDefinitionStateCmd(UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionSuspensionStateBuilder);

	  protected internal override AbstractSetProcessInstanceStateCmd NextCommand
	  {
		  get
		  {
			UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceCommandBuilder = createProcessInstanceCommandBuilder();
    
			return getNextCommand(processInstanceCommandBuilder);
		  }
	  }

	  protected internal abstract AbstractSetProcessInstanceStateCmd getNextCommand(UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceCommandBuilder);

	}

}