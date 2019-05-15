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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using BatchUtil = org.camunda.bpm.engine.impl.util.BatchUtil;

	public class ProcessInstanceModificationBatchCmd : AbstractModificationCmd<Batch>
	{

	  protected internal static readonly CommandLogger LOGGER = ProcessEngineLogger.CMD_LOGGER;

	  public ProcessInstanceModificationBatchCmd(ModificationBuilderImpl modificationBuilderImpl) : base(modificationBuilderImpl)
	  {
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
		ICollection<string> processInstanceIds = collectProcessInstanceIds(commandContext);

		ensureNotEmpty(typeof(BadUserRequestException), "Modification instructions cannot be empty", instructions);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "Process instance ids", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "Process instance ids", processInstanceIds);

		checkPermissions(commandContext);

		ProcessDefinitionEntity processDefinition = getProcessDefinition(commandContext, builder.ProcessDefinitionId);
		ensureNotNull(typeof(BadUserRequestException), "Process definition id cannot be null", processDefinition);

		writeUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, true);

		BatchEntity batch = createBatch(commandContext, instructions, processInstanceIds, processDefinition);
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

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, IList<AbstractProcessInstanceModificationCommand> instructions, ICollection<string> processInstanceIds, ProcessDefinitionEntity processDefinition)
	  {

		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler<ModificationBatchConfiguration> batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		ModificationBatchConfiguration configuration = new ModificationBatchConfiguration(new List<string>(processInstanceIds), builder.ProcessDefinitionId, instructions, builder.SkipCustomListeners, builder.SkipIoMappings);

		BatchEntity batch = new BatchEntity();

		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = BatchUtil.calculateBatchSize(processEngineConfiguration, configuration);
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		batch.TenantId = processDefinition.TenantId;
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

	}

}