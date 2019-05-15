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
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class DecisionSetRemovalTimeJobHandler : AbstractBatchJobHandler<SetRemovalTimeBatchConfiguration>
	{

	  public static readonly BatchJobDeclaration JOB_DECLARATION = new BatchJobDeclaration(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_DECISION_SET_REMOVAL_TIME);

	  public virtual void execute(BatchJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		if (isDmnEnabled(commandContext))
		{

		  string byteArrayId = configuration.ConfigurationByteArrayId;
		  sbyte[] configurationByteArray = findByteArrayById(byteArrayId, commandContext).Bytes;

		  SetRemovalTimeBatchConfiguration batchConfiguration = readConfiguration(configurationByteArray);

		  foreach (string instanceId in batchConfiguration.Ids)
		  {

			HistoricDecisionInstanceEntity instance = findDecisionInstanceById(instanceId, commandContext);

			if (instance != null)
			{
			  if (batchConfiguration.Hierarchical)
			  {

				string rootDecisionInstanceId = getRootDecisionInstance(instance);

				HistoricDecisionInstanceEntity rootInstance = findDecisionInstanceById(rootDecisionInstanceId, commandContext);

				DateTime removalTime = getOrCalculateRemovalTime(batchConfiguration, rootInstance, commandContext);

				addRemovalTimeToHierarchy(rootDecisionInstanceId, removalTime, commandContext);

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
	  }

	  protected internal virtual string getRootDecisionInstance(HistoricDecisionInstanceEntity instance)
	  {
		return string.ReferenceEquals(instance.RootDecisionInstanceId, null) ? instance.Id : instance.RootDecisionInstanceId;
	  }

	  protected internal virtual DateTime getOrCalculateRemovalTime(SetRemovalTimeBatchConfiguration batchConfiguration, HistoricDecisionInstanceEntity instance, CommandContext commandContext)
	  {
		if (batchConfiguration.hasRemovalTime())
		{
		  return batchConfiguration.RemovalTime;

		}
		else if (hasBaseTime(commandContext))
		{
		  return calculateRemovalTime(instance, commandContext);

		}
		else
		{
		  return null;

		}
	  }

	  protected internal virtual void addRemovalTimeToHierarchy(string instanceId, DateTime removalTime, CommandContext commandContext)
	  {
		commandContext.HistoricDecisionInstanceManager.addRemovalTimeToDecisionsByRootDecisionInstanceId(instanceId, removalTime);
	  }

	  protected internal virtual void addRemovalTime(string instanceId, DateTime removalTime, CommandContext commandContext)
	  {
		commandContext.HistoricDecisionInstanceManager.addRemovalTimeToDecisionsByDecisionInstanceId(instanceId, removalTime);
	  }

	  protected internal virtual bool hasBaseTime(CommandContext commandContext)
	  {
		return isStrategyStart(commandContext) || isStrategyEnd(commandContext);
	  }

	  protected internal virtual bool isStrategyStart(CommandContext commandContext)
	  {
		return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(getHistoryRemovalTimeStrategy(commandContext));
	  }

	  protected internal virtual bool isStrategyEnd(CommandContext commandContext)
	  {
		return HISTORY_REMOVAL_TIME_STRATEGY_END.Equals(getHistoryRemovalTimeStrategy(commandContext));
	  }

	  protected internal virtual string getHistoryRemovalTimeStrategy(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
	  }

	  protected internal virtual DecisionDefinition findDecisionDefinitionById(string decisionDefinitionId, CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);
	  }

	  protected internal virtual bool isDmnEnabled(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.DmnEnabled;
	  }

	  protected internal virtual DateTime calculateRemovalTime(HistoricDecisionInstanceEntity decisionInstance, CommandContext commandContext)
	  {
		DecisionDefinition decisionDefinition = findDecisionDefinitionById(decisionInstance.DecisionDefinitionId, commandContext);

		return commandContext.ProcessEngineConfiguration.HistoryRemovalTimeProvider.calculateRemovalTime(decisionInstance, decisionDefinition);
	  }

	  protected internal virtual ByteArrayEntity findByteArrayById(string byteArrayId, CommandContext commandContext)
	  {
		return commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), byteArrayId);
	  }

	  protected internal virtual HistoricDecisionInstanceEntity findDecisionInstanceById(string instanceId, CommandContext commandContext)
	  {
		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstance(instanceId);
	  }

	  public override JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration
	  {
		  get
		  {
			return JOB_DECLARATION;
		  }
	  }

	  protected internal virtual SetRemovalTimeBatchConfiguration createJobConfiguration(SetRemovalTimeBatchConfiguration configuration, IList<string> decisionInstanceIds)
	  {
		return (new SetRemovalTimeBatchConfiguration(decisionInstanceIds)).setRemovalTime(configuration.RemovalTime).setHasRemovalTime(configuration.hasRemovalTime()).setHierarchical(configuration.Hierarchical);
	  }

	  protected internal override JsonObjectConverter JsonConverterInstance
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
			return org.camunda.bpm.engine.batch.Batch_Fields.TYPE_DECISION_SET_REMOVAL_TIME;
		  }
	  }

	}

}