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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using SetRemovalTimeBatchConfiguration = org.camunda.bpm.engine.impl.batch.removaltime.SetRemovalTimeBatchConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using SetRemovalTimeToHistoricProcessInstancesBuilderImpl = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricProcessInstancesBuilderImpl;
	using Mode = org.camunda.bpm.engine.impl.history.SetRemovalTimeToHistoricProcessInstancesBuilderImpl.Mode;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeToHistoricProcessInstancesCmd : AbstractIDBasedBatchCmd<Batch>
	{

	  protected internal SetRemovalTimeToHistoricProcessInstancesBuilderImpl builder;

	  public SetRemovalTimeToHistoricProcessInstancesCmd(SetRemovalTimeToHistoricProcessInstancesBuilderImpl builder)
	  {
		this.builder = builder;
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		ISet<string> historicProcessInstanceIds = new HashSet<string>();

		IList<string> instanceIds = builder.Ids;
		HistoricProcessInstanceQuery instanceQuery = builder.Query;
		if (instanceQuery == null && instanceIds == null)
		{
		  throw new BadUserRequestException("Either query nor ids provided.");

		}

		if (instanceQuery != null)
		{
		  foreach (HistoricProcessInstance historicDecisionInstance in instanceQuery.list())
		  {
			historicProcessInstanceIds.Add(historicDecisionInstance.Id);

		  }
		}

		if (instanceIds != null)
		{
		  historicProcessInstanceIds.addAll(findHistoricInstanceIds(instanceIds, commandContext));

		}

		ensureNotNull(typeof(BadUserRequestException), "removalTime", builder.getMode());
		ensureNotEmpty(typeof(BadUserRequestException), "historicProcessInstances", historicProcessInstanceIds);

		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_SET_REMOVAL_TIME);

		writeUserOperationLog(commandContext, historicProcessInstanceIds.Count, builder.getMode(), builder.RemovalTime, builder.Hierarchical, true);

		BatchEntity batch = createBatch(commandContext, new List<>(historicProcessInstanceIds));

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal virtual IList<string> findHistoricInstanceIds(IList<string> instanceIds, CommandContext commandContext)
	  {
		IList<HistoricProcessInstance> historicProcessInstances = createHistoricDecisionInstanceQuery(commandContext).processInstanceIds(new HashSet<HistoricProcessInstance>(instanceIds)).list();

		IList<string> ids = new List<string>();
		foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		{
		  ids.Add(historicProcessInstance.Id);
		}

		return ids;
	  }

	  protected internal virtual HistoricProcessInstanceQuery createHistoricDecisionInstanceQuery(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.HistoryService.createHistoricProcessInstanceQuery();
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int numInstances, SetRemovalTimeToHistoricProcessInstancesBuilderImpl.Mode mode, DateTime removalTime, bool hierarchical, bool async)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("mode", null, mode));
		propertyChanges.Add(new PropertyChange("removalTime", null, removalTime));
		propertyChanges.Add(new PropertyChange("hierarchical", null, hierarchical));
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_REMOVAL_TIME, propertyChanges);
	  }

	  protected internal override BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> ids)
	  {
		return (new SetRemovalTimeBatchConfiguration(ids)).setHierarchical(builder.Hierarchical).setHasRemovalTime(hasRemovalTime(builder.getMode())).setRemovalTime(builder.RemovalTime);
	  }

	  protected internal virtual bool hasRemovalTime(SetRemovalTimeToHistoricProcessInstancesBuilderImpl.Mode mode)
	  {
		return builder.getMode() == SetRemovalTimeToHistoricProcessInstancesBuilderImpl.Mode.ABSOLUTE_REMOVAL_TIME || builder.getMode() == SetRemovalTimeToHistoricProcessInstancesBuilderImpl.Mode.CLEARED_REMOVAL_TIME;
	  }

	  protected internal override BatchJobHandler getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_SET_REMOVAL_TIME];
	  }

	}

}