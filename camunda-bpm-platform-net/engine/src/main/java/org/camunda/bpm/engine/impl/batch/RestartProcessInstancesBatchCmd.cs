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
namespace org.camunda.bpm.engine.impl.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using AbstractRestartProcessInstanceCmd = org.camunda.bpm.engine.impl.cmd.AbstractRestartProcessInstanceCmd;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using BatchUtil = org.camunda.bpm.engine.impl.util.BatchUtil;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class RestartProcessInstancesBatchCmd : AbstractRestartProcessInstanceCmd<Batch>
	{

	  private readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public RestartProcessInstancesBatchCmd(CommandExecutor commandExecutor, RestartProcessInstanceBuilderImpl builder) : base(commandExecutor, builder)
	  {
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
		ICollection<string> processInstanceIds = collectProcessInstanceIds();

		ensureNotEmpty(typeof(BadUserRequestException), "Restart instructions cannot be empty", "instructions", instructions);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "processInstanceIds", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "processInstanceIds", processInstanceIds);

		checkPermissions(commandContext);
		ProcessDefinitionEntity processDefinition = getProcessDefinition(commandContext, builder.ProcessDefinitionId);

		ensureNotNull(typeof(BadUserRequestException), "Process definition cannot be null", processDefinition);
		ensureTenantAuthorized(commandContext, processDefinition);

		writeUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, true);

		List<string> ids = new List<string>();
		ids.AddRange(processInstanceIds);
		BatchEntity batch = createBatch(commandContext, instructions, ids, processDefinition);
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
		  checker.checkCreateBatch(BatchPermissions.CREATE_BATCH_RESTART_PROCESS_INSTANCES);
		}
	  }

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, IList<AbstractProcessInstanceModificationCommand> instructions, IList<string> processInstanceIds, ProcessDefinitionEntity processDefinition)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler<RestartProcessInstancesBatchConfiguration> batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		RestartProcessInstancesBatchConfiguration configuration = new RestartProcessInstancesBatchConfiguration(processInstanceIds, instructions, builder.ProcessDefinitionId, builder.InitialVariables, builder.SkipCustomListeners, builder.SkipIoMappings, builder.WithoutBusinessKey);

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
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected BatchJobHandler<org.camunda.bpm.engine.impl.RestartProcessInstancesBatchConfiguration> getBatchJobHandler(org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration)
	  protected internal virtual BatchJobHandler<RestartProcessInstancesBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, BatchJobHandler<?>> batchHandlers = processEngineConfiguration.getBatchHandlers();
		IDictionary<string, BatchJobHandler<object>> batchHandlers = processEngineConfiguration.BatchHandlers;
		return (BatchJobHandler<RestartProcessInstancesBatchConfiguration>) batchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_RESTART];
	  }

	  protected internal virtual void ensureTenantAuthorized(CommandContext commandContext, ProcessDefinitionEntity processDefinition)
	  {
		if (!commandContext.TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("restart process instances of process definition '" + processDefinition.Id + "'");
		}
	  }
	}

}