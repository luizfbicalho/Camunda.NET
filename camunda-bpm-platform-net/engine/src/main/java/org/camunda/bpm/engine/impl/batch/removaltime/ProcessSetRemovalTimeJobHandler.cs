using System;
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
namespace org.camunda.bpm.engine.impl.batch.removaltime
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricProcessInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class ProcessSetRemovalTimeJobHandler : AbstractBatchJobHandler<SetRemovalTimeBatchConfiguration>
	{

	  public static readonly BatchJobDeclaration JOB_DECLARATION = new BatchJobDeclaration(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_SET_REMOVAL_TIME);

	  public virtual void execute(BatchJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {

		string byteArrayId = configuration.ConfigurationByteArrayId;
		sbyte[] configurationByteArray = findByteArrayById(byteArrayId, commandContext).Bytes;

		SetRemovalTimeBatchConfiguration batchConfiguration = readConfiguration(configurationByteArray);

		foreach (string instanceId in batchConfiguration.Ids)
		{

		  HistoricProcessInstanceEntity instance = findProcessInstanceById(instanceId, commandContext);

		  if (instance != null)
		  {
			if (batchConfiguration.Hierarchical && hasHierarchy(instance))
			{
			  string rootProcessInstanceId = instance.RootProcessInstanceId;

			  HistoricProcessInstanceEntity rootInstance = findProcessInstanceById(rootProcessInstanceId, commandContext);
			  DateTime removalTime = getOrCalculateRemovalTime(batchConfiguration, rootInstance, commandContext);

			  addRemovalTimeToHierarchy(rootProcessInstanceId, removalTime, commandContext);

			}
			else
			{
			  DateTime removalTime = getOrCalculateRemovalTime(batchConfiguration, instance, commandContext);

			  if (removalTime != instance.RemovalTime)
			  {
				addRemovalTime(instanceId, removalTime, commandContext);

			  }
			}
		  }
		}
	  }

	  protected internal virtual DateTime getOrCalculateRemovalTime(SetRemovalTimeBatchConfiguration batchConfiguration, HistoricProcessInstanceEntity instance, CommandContext commandContext)
	  {
		if (batchConfiguration.hasRemovalTime())
		{
		  return batchConfiguration.RemovalTime;

		}
		else if (hasBaseTime(instance, commandContext))
		{
		  return calculateRemovalTime(instance, commandContext);

		}
		else
		{
		  return null;

		}
	  }

	  protected internal virtual void addRemovalTimeToHierarchy(string rootProcessInstanceId, DateTime removalTime, CommandContext commandContext)
	  {
		commandContext.HistoricProcessInstanceManager.addRemovalTimeToProcessInstancesByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		if (isDmnEnabled(commandContext))
		{
		  commandContext.HistoricDecisionInstanceManager.addRemovalTimeToDecisionsByRootProcessInstanceId(rootProcessInstanceId, removalTime);
		}
	  }

	  protected internal virtual void addRemovalTime(string instanceId, DateTime removalTime, CommandContext commandContext)
	  {
		commandContext.HistoricProcessInstanceManager.addRemovalTimeById(instanceId, removalTime);

		if (isDmnEnabled(commandContext))
		{
		  commandContext.HistoricDecisionInstanceManager.addRemovalTimeToDecisionsByProcessInstanceId(instanceId, removalTime);
		}
	  }

	  protected internal virtual bool hasBaseTime(HistoricProcessInstanceEntity instance, CommandContext commandContext)
	  {
		return isStrategyStart(commandContext) || (isStrategyEnd(commandContext) && isEnded(instance));
	  }

	  protected internal virtual bool isEnded(HistoricProcessInstanceEntity instance)
	  {
		return instance.EndTime != null;
	  }

	  protected internal virtual bool isStrategyStart(CommandContext commandContext)
	  {
		return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(getHistoryRemovalTimeStrategy(commandContext));
	  }

	  protected internal virtual bool isStrategyEnd(CommandContext commandContext)
	  {
		return HISTORY_REMOVAL_TIME_STRATEGY_END.Equals(getHistoryRemovalTimeStrategy(commandContext));
	  }

	  protected internal virtual bool hasHierarchy(HistoricProcessInstanceEntity instance)
	  {
		return !string.ReferenceEquals(instance.RootProcessInstanceId, null);
	  }

	  protected internal virtual string getHistoryRemovalTimeStrategy(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
	  }

	  protected internal virtual ProcessDefinition findProcessDefinitionById(string processDefinitionId, CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }

	  protected internal virtual bool isDmnEnabled(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.DmnEnabled;
	  }

	  protected internal virtual DateTime calculateRemovalTime(HistoricProcessInstanceEntity processInstance, CommandContext commandContext)
	  {
		ProcessDefinition processDefinition = findProcessDefinitionById(processInstance.ProcessDefinitionId, commandContext);

		return commandContext.ProcessEngineConfiguration.HistoryRemovalTimeProvider.calculateRemovalTime(processInstance, processDefinition);
	  }

	  protected internal virtual ByteArrayEntity findByteArrayById(string byteArrayId, CommandContext commandContext)
	  {
		return commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), byteArrayId);
	  }

	  protected internal virtual HistoricProcessInstanceEntity findProcessInstanceById(string instanceId, CommandContext commandContext)
	  {
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstance(instanceId);
	  }

	  public override JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration
	  {
		  get
		  {
			return JOB_DECLARATION;
		  }
	  }

	  protected internal virtual SetRemovalTimeBatchConfiguration createJobConfiguration(SetRemovalTimeBatchConfiguration configuration, IList<string> processInstanceIds)
	  {
		return (new SetRemovalTimeBatchConfiguration(processInstanceIds)).setRemovalTime(configuration.RemovalTime).setHasRemovalTime(configuration.hasRemovalTime()).setHierarchical(configuration.Hierarchical);
	  }

	  protected internal override JsonObjectConverter<SetRemovalTimeBatchConfiguration> JsonConverterInstance
	  {
		  get
		  {
			return SetRemovalTimeJsonConverter.INSTANCE;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_SET_REMOVAL_TIME;
		  }
	  }

	}

}