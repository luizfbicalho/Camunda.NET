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
namespace org.camunda.bpm.engine.impl.cmd.batch.removaltime
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using SetRemovalTimeBatchConfiguration = org.camunda.bpm.engine.impl.batch.removaltime.SetRemovalTimeBatchConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using SetRemovalTimeToHistoricDecisionInstancesBuilderImpl = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricDecisionInstancesBuilderImpl;
	using Mode = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricDecisionInstancesBuilderImpl.Mode;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeToHistoricDecisionInstancesCmd : AbstractIDBasedBatchCmd<Batch>
	{

	  protected internal SetRemovalTimeToHistoricDecisionInstancesBuilderImpl builder;

	  public SetRemovalTimeToHistoricDecisionInstancesCmd(SetRemovalTimeToHistoricDecisionInstancesBuilderImpl builder)
	  {
		this.builder = builder;
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		ISet<string> historicDecisionInstanceIds = new HashSet<string>();

		IList<string> instanceIds = builder.Ids;
		HistoricDecisionInstanceQuery instanceQuery = builder.Query;
		if (instanceQuery == null && instanceIds == null)
		{
		  throw new BadUserRequestException("Either query nor ids provided.");

		}

		if (instanceQuery != null)
		{
		  foreach (HistoricDecisionInstance historicDecisionInstance in instanceQuery.list())
		  {
			historicDecisionInstanceIds.Add(historicDecisionInstance.Id);

		  }
		}

		if (instanceIds != null)
		{
		  historicDecisionInstanceIds.addAll(findHistoricInstanceIds(instanceIds, commandContext));

		}

		ensureNotNull(typeof(BadUserRequestException), "removalTime", builder.getMode());
		ensureNotEmpty(typeof(BadUserRequestException), "historicDecisionInstances", historicDecisionInstanceIds);

		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_SET_REMOVAL_TIME);

		writeUserOperationLog(commandContext, historicDecisionInstanceIds.Count, builder.getMode(), builder.RemovalTime, builder.Hierarchical, true);

		BatchEntity batch = createBatch(commandContext, new List<>(historicDecisionInstanceIds));

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal virtual IList<string> findHistoricInstanceIds(IList<string> instanceIds, CommandContext commandContext)
	  {
		IList<HistoricDecisionInstance> historicDecisionInstances = createHistoricDecisionInstanceQuery(commandContext).decisionInstanceIdIn(instanceIds.ToArray()).list();

		IList<string> ids = new List<string>();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  ids.Add(historicDecisionInstance.Id);
		}

		return ids;
	  }

	  protected internal virtual HistoricDecisionInstanceQuery createHistoricDecisionInstanceQuery(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.HistoryService.createHistoricDecisionInstanceQuery();
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int numInstances, SetRemovalTimeToHistoricDecisionInstancesBuilderImpl.Mode mode, DateTime removalTime, bool hierarchical, bool async)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("mode", null, mode));
		propertyChanges.Add(new PropertyChange("removalTime", null, removalTime));
		propertyChanges.Add(new PropertyChange("hierarchical", null, hierarchical));
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));

		commandContext.OperationLogManager.logDecisionInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_REMOVAL_TIME, propertyChanges);
	  }

	  protected internal override BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> ids)
	  {
		return (new SetRemovalTimeBatchConfiguration(ids)).setHierarchical(builder.Hierarchical).setHasRemovalTime(hasRemovalTime(builder.getMode())).setRemovalTime(builder.RemovalTime);
	  }

	  protected internal virtual bool hasRemovalTime(SetRemovalTimeToHistoricDecisionInstancesBuilderImpl.Mode mode)
	  {
		return builder.getMode() == SetRemovalTimeToHistoricDecisionInstancesBuilderImpl.Mode.ABSOLUTE_REMOVAL_TIME || builder.getMode() == SetRemovalTimeToHistoricDecisionInstancesBuilderImpl.Mode.CLEARED_REMOVAL_TIME;
	  }

	  protected internal override BatchJobHandler getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_DECISION_SET_REMOVAL_TIME];
	  }

	}

}