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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using SetRetriesBatchConfiguration = org.camunda.bpm.engine.impl.batch.SetRetriesBatchConfiguration;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using BatchUtil = org.camunda.bpm.engine.impl.util.BatchUtil;

	public class SetExternalTasksRetriesBatchCmd : AbstractSetExternalTaskRetriesCmd<Batch>
	{

	  public SetExternalTasksRetriesBatchCmd(UpdateExternalTaskRetriesBuilderImpl builder) : base(builder)
	  {
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<string> externalTaskIds = collectExternalTaskIds();

		ensureNotEmpty(typeof(BadUserRequestException), "externalTaskIds", externalTaskIds);

		checkPermissions(commandContext);

		writeUserOperationLog(commandContext, builder.Retries, externalTaskIds.Count, true);

		BatchEntity batch = createBatch(commandContext, externalTaskIds);

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
		  checker.checkCreateBatch(BatchPermissions.CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES);
		}
	  }

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, ICollection<string> processInstanceIds)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler<SetRetriesBatchConfiguration> batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		SetRetriesBatchConfiguration configuration = new SetRetriesBatchConfiguration(new List<string>(processInstanceIds), builder.Retries);

		BatchEntity batch = new BatchEntity();
		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = BatchUtil.calculateBatchSize(processEngineConfiguration, configuration);
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		commandContext.BatchManager.insertBatch(batch);

		return batch;
	  }


	  protected internal virtual BatchJobHandler<SetRetriesBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<SetRetriesBatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_SET_EXTERNAL_TASK_RETRIES];
	  }
	}

}