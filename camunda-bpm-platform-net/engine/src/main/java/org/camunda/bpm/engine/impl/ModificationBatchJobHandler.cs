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
namespace org.camunda.bpm.engine.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using AbstractBatchJobHandler = org.camunda.bpm.engine.impl.batch.AbstractBatchJobHandler;
	using BatchJobConfiguration = org.camunda.bpm.engine.impl.batch.BatchJobConfiguration;
	using BatchJobContext = org.camunda.bpm.engine.impl.batch.BatchJobContext;
	using BatchJobDeclaration = org.camunda.bpm.engine.impl.batch.BatchJobDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using ModificationBatchConfigurationJsonConverter = org.camunda.bpm.engine.impl.json.ModificationBatchConfigurationJsonConverter;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

	public class ModificationBatchJobHandler : AbstractBatchJobHandler<ModificationBatchConfiguration>
	{

	  public static readonly BatchJobDeclaration JOB_DECLARATION = new BatchJobDeclaration(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION);

	  public override string Type
	  {
		  get
		  {
			return org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION;
		  }
	  }

	  protected internal override void postProcessJob(ModificationBatchConfiguration configuration, JobEntity job)
	  {
		CommandContext commandContext = Context.CommandContext;
		ProcessDefinitionEntity processDefinitionEntity = commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(configuration.ProcessDefinitionId);
		job.DeploymentId = processDefinitionEntity.DeploymentId;
	  }

	  public override void execute(BatchJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		ByteArrayEntity configurationEntity = commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), configuration.ConfigurationByteArrayId);

		ModificationBatchConfiguration batchConfiguration = readConfiguration(configurationEntity.Bytes);

		ModificationBuilderImpl executionBuilder = (ModificationBuilderImpl) commandContext.ProcessEngineConfiguration.RuntimeService.createModification(batchConfiguration.ProcessDefinitionId).processInstanceIds(batchConfiguration.Ids);

		executionBuilder.Instructions = batchConfiguration.Instructions;

		if (batchConfiguration.SkipCustomListeners)
		{
		  executionBuilder.skipCustomListeners();
		}
		if (batchConfiguration.SkipIoMappings)
		{
		  executionBuilder.skipIoMappings();
		}

		executionBuilder.execute(false);

		commandContext.ByteArrayManager.delete(configurationEntity);

	  }

	  public override JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration
	  {
		  get
		  {
			return JOB_DECLARATION;
		  }
	  }

	  protected internal override ModificationBatchConfiguration createJobConfiguration(ModificationBatchConfiguration configuration, IList<string> processIdsForJob)
	  {
		return new ModificationBatchConfiguration(processIdsForJob, configuration.ProcessDefinitionId, configuration.Instructions, configuration.SkipCustomListeners, configuration.SkipIoMappings);
	  }


	  protected internal override ModificationBatchConfigurationJsonConverter JsonConverterInstance
	  {
		  get
		  {
			return ModificationBatchConfigurationJsonConverter.INSTANCE;
		  }
	  }

	  protected internal virtual ProcessDefinitionEntity getProcessDefinition(CommandContext commandContext, string processDefinitionId)
	  {
		return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }

	}

}