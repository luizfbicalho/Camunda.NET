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
namespace org.camunda.bpm.engine.impl.batch.update
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;

	public class UpdateProcessInstancesSuspendStateJobHandler : AbstractBatchJobHandler<UpdateProcessInstancesSuspendStateBatchConfiguration>
	{

	  public static readonly BatchJobDeclaration JOB_DECLARATION = new BatchJobDeclaration(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_UPDATE_SUSPENSION_STATE);

	  public override string Type
	  {
		  get
		  {
			return org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_UPDATE_SUSPENSION_STATE;
		  }
	  }

	  protected internal override UpdateProcessInstancesSuspendStateBatchConfigurationJsonConverter JsonConverterInstance
	  {
		  get
		  {
			return UpdateProcessInstancesSuspendStateBatchConfigurationJsonConverter.INSTANCE;
		  }
	  }

	  public override JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration
	  {
		  get
		  {
			return JOB_DECLARATION;
		  }
	  }

	  protected internal override UpdateProcessInstancesSuspendStateBatchConfiguration createJobConfiguration(UpdateProcessInstancesSuspendStateBatchConfiguration configuration, IList<string> processIdsForJob)
	  {
		return new UpdateProcessInstancesSuspendStateBatchConfiguration(processIdsForJob, configuration.Suspended);
	  }

	  public override void execute(BatchJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		ByteArrayEntity configurationEntity = commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), configuration.ConfigurationByteArrayId);

		UpdateProcessInstancesSuspendStateBatchConfiguration batchConfiguration = readConfiguration(configurationEntity.Bytes);

		bool initialLegacyRestrictions = commandContext.RestrictUserOperationLogToAuthenticatedUsers;
		commandContext.disableUserOperationLog();
		commandContext.RestrictUserOperationLogToAuthenticatedUsers = true;
		try
		{
		  if (batchConfiguration.Suspended)
		  {
			commandContext.ProcessEngineConfiguration.RuntimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(batchConfiguration.Ids).suspend();
		  }
		  else
		  {
			 commandContext.ProcessEngineConfiguration.RuntimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(batchConfiguration.Ids).activate();
		  }
		}
		finally
		{
		  commandContext.enableUserOperationLog();
		  commandContext.RestrictUserOperationLogToAuthenticatedUsers = initialLegacyRestrictions;
		}
	  }

	}

}