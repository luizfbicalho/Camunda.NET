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
namespace org.camunda.bpm.engine.impl.cmd
{


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public class ModifyProcessInstanceAsyncCmd : Command<Batch>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal ProcessInstanceModificationBuilderImpl builder;

	  public ModifyProcessInstanceAsyncCmd(ProcessInstanceModificationBuilderImpl processInstanceModificationBuilder)
	  {
		this.builder = processInstanceModificationBuilder;
	  }

	  public virtual Batch execute(CommandContext commandContext)
	  {
		string processInstanceId = builder.ProcessInstanceId;

		ExecutionManager executionManager = commandContext.ExecutionManager;
		ExecutionEntity processInstance = executionManager.findExecutionById(processInstanceId);
		ensureProcessInstanceExists(processInstanceId, processInstance);

		checkPermissions(commandContext);

		commandContext.OperationLogManager.logProcessInstanceOperation(LogEntryOperation, processInstanceId, null, null, Collections.singletonList(PropertyChange.EMPTY_CHANGE));

		IList<AbstractProcessInstanceModificationCommand> instructions = builder.ModificationOperations;
		BatchEntity batch = createBatch(commandContext, instructions, processInstance);
		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();
		return batch;
	  }

	  protected internal virtual void checkPermissions(CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateBatch(BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES);
		}
	  }

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, IList<AbstractProcessInstanceModificationCommand> instructions, ExecutionEntity processInstance)
	  {

		string processInstanceId = processInstance.ProcessInstanceId;
		string processDefinitionId = processInstance.ProcessDefinitionId;
		string tenantId = processInstance.TenantId;

		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler<ModificationBatchConfiguration> batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		ModificationBatchConfiguration configuration = new ModificationBatchConfiguration(Arrays.asList(processInstanceId), processDefinitionId, instructions, builder.SkipCustomListeners, builder.SkipIoMappings);

		BatchEntity batch = new BatchEntity();

		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = 1;
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		batch.TenantId = tenantId;
		commandContext.BatchManager.insertBatch(batch);

		return batch;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.impl.batch.BatchJobHandler<org.camunda.bpm.engine.impl.ModificationBatchConfiguration> getBatchJobHandler(org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration)
	  protected internal virtual BatchJobHandler<ModificationBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> batchHandlers = processEngineConfiguration.getBatchHandlers();
		IDictionary<string, BatchJobHandler<object>> batchHandlers = processEngineConfiguration.BatchHandlers;
		return (BatchJobHandler<ModificationBatchConfiguration>) batchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION];
	  }

	  protected internal virtual void ensureProcessInstanceExists(string processInstanceId, ExecutionEntity processInstance)
	  {
		if (processInstance == null)
		{
		  throw LOG.processInstanceDoesNotExist(processInstanceId);
		}
	  }

	  protected internal virtual string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE;
		  }
	  }

	}

}