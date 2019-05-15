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
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using UpdateProcessInstancesSuspendStateBatchConfiguration = org.camunda.bpm.engine.impl.batch.update.UpdateProcessInstancesSuspendStateBatchConfiguration;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using BatchUtil = org.camunda.bpm.engine.impl.util.BatchUtil;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	public class UpdateProcessInstancesSuspendStateBatchCmd : AbstractUpdateProcessInstancesSuspendStateCmd<Batch>
	{

	  public UpdateProcessInstancesSuspendStateBatchCmd(CommandExecutor commandExecutor, UpdateProcessInstancesSuspensionStateBuilderImpl builder, bool suspending) : base(commandExecutor, builder, suspending)
	  {
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		ICollection<string> processInstanceIds = collectProcessInstanceIds();

		EnsureUtil.ensureNotEmpty(typeof(BadUserRequestException), "No process instance ids given", "process Instance Ids", processInstanceIds);
		EnsureUtil.ensureNotContainsNull(typeof(BadUserRequestException), "Cannot be null.", "Process Instance ids", processInstanceIds);
		checkAuthorizations(commandContext);
		writeUserOperationLog(commandContext, processInstanceIds.Count, true);
		BatchEntity batch = createBatch(commandContext, processInstanceIds);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();
		return batch;
	  }

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, ICollection<string> processInstanceIds)
	  {

		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		BatchConfiguration configuration = getAbstractIdsBatchConfiguration(new List<string>(processInstanceIds));

		BatchEntity batch = new BatchEntity();

		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = BatchUtil.calculateBatchSize(processEngineConfiguration, (UpdateProcessInstancesSuspendStateBatchConfiguration) configuration);
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		commandContext.BatchManager.insertBatch(batch);

		return batch;
	  }

	  protected internal virtual BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
	  {
		return new UpdateProcessInstancesSuspendStateBatchConfiguration(processInstanceIds, suspending);
	  }

	  protected internal virtual BatchJobHandler<UpdateProcessInstancesSuspendStateBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> batchHandlers = processEngineConfiguration.getBatchHandlers();
		IDictionary<string, BatchJobHandler<object>> batchHandlers = processEngineConfiguration.BatchHandlers;
		return (BatchJobHandler<UpdateProcessInstancesSuspendStateBatchConfiguration>) batchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_UPDATE_SUSPENSION_STATE];
	  }

	  protected internal virtual void checkAuthorizations(CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateBatch(BatchPermissions.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND);
		}
	  }

	}

}