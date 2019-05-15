using System;

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
	using BatchSeedJobConfiguration = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler.BatchSeedJobConfiguration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;

	/// <summary>
	/// Job declaration for batch seed jobs. The batch seed job
	/// creates all batch jobs required to complete the batch.
	/// </summary>
	[Serializable]
	public class BatchSeedJobDeclaration : JobDeclaration<BatchEntity, MessageEntity>
	{

	  private const long serialVersionUID = 1L;

	  public BatchSeedJobDeclaration() : base(BatchSeedJobHandler.TYPE)
	  {
	  }

	  protected internal virtual ExecutionEntity resolveExecution(BatchEntity batch)
	  {
		return null;
	  }

	  protected internal virtual MessageEntity newJobInstance(BatchEntity batch)
	  {
		return new MessageEntity();
	  }

	  protected internal override JobHandlerConfiguration resolveJobHandlerConfiguration(BatchEntity batch)
	  {
		return new BatchSeedJobConfiguration(batch.Id);
	  }

	  protected internal override string resolveJobDefinitionId(BatchEntity batch)
	  {
		return batch.SeedJobDefinitionId;
	  }

	  public override ParameterValueProvider JobPriorityProvider
	  {
		  get
		  {
			long batchJobPriority = Context.ProcessEngineConfiguration.BatchJobPriority;
			return new ConstantValueProvider(batchJobPriority);
		  }
	  }

	}

}