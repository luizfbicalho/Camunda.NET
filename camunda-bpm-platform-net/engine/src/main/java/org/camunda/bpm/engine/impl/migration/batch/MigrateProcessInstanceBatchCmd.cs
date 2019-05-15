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
namespace org.camunda.bpm.engine.impl.migration.batch
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
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	public class MigrateProcessInstanceBatchCmd : AbstractMigrationCmd<Batch>
	{

	  protected internal static readonly MigrationLogger LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  public MigrateProcessInstanceBatchCmd(MigrationPlanExecutionBuilderImpl migrationPlanExecutionBuilder) : base(migrationPlanExecutionBuilder)
	  {
	  }

	  public override Batch execute(CommandContext commandContext)
	  {

		MigrationPlan migrationPlan = executionBuilder.MigrationPlan;
		ICollection<string> processInstanceIds = collectProcessInstanceIds(commandContext);

		ensureNotNull(typeof(BadUserRequestException), "Migration plan cannot be null", "migration plan", migrationPlan);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot empty", "process instance ids", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "process instance ids", processInstanceIds);

		ProcessDefinitionEntity sourceProcessDefinition = resolveSourceProcessDefinition(commandContext);
		ProcessDefinitionEntity targetProcessDefinition = resolveTargetProcessDefinition(commandContext);

		checkAuthorizations(commandContext, sourceProcessDefinition, targetProcessDefinition, processInstanceIds);
		writeUserOperationLog(commandContext, sourceProcessDefinition, targetProcessDefinition, processInstanceIds.Count, true);

		BatchEntity batch = createBatch(commandContext, migrationPlan, processInstanceIds, sourceProcessDefinition);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal override void checkAuthorizations(CommandContext commandContext, ProcessDefinitionEntity sourceDefinition, ProcessDefinitionEntity targetDefinition, ICollection<string> processInstanceIds)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateBatch(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		}

		base.checkAuthorizations(commandContext, sourceDefinition, targetDefinition, processInstanceIds);
	  }

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, MigrationPlan migrationPlan, ICollection<string> processInstanceIds, ProcessDefinitionEntity sourceProcessDefinition)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler<MigrationBatchConfiguration> batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		MigrationBatchConfiguration configuration = new MigrationBatchConfiguration(new List<string>(processInstanceIds), migrationPlan, executionBuilder.SkipCustomListeners, executionBuilder.SkipIoMappings);

		BatchEntity batch = new BatchEntity();
		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = BatchUtil.calculateBatchSize(processEngineConfiguration, configuration);
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		batch.TenantId = sourceProcessDefinition.TenantId;
		commandContext.BatchManager.insertBatch(batch);

		return batch;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.impl.batch.BatchJobHandler<MigrationBatchConfiguration> getBatchJobHandler(org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration)
	  protected internal virtual BatchJobHandler<MigrationBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<MigrationBatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION];
	  }

	}

}