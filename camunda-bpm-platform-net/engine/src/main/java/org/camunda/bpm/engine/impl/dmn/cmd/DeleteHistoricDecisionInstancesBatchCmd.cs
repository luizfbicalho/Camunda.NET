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
namespace org.camunda.bpm.engine.impl.dmn.cmd
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AbstractIDBasedBatchCmd = org.camunda.bpm.engine.impl.cmd.batch.AbstractIDBasedBatchCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;

	public class DeleteHistoricDecisionInstancesBatchCmd : AbstractIDBasedBatchCmd<Batch>
	{

	  protected internal IList<string> historicDecisionInstanceIds;
	  protected internal HistoricDecisionInstanceQuery historicDecisionInstanceQuery;
	  protected internal string deleteReason;

	  public DeleteHistoricDecisionInstancesBatchCmd(IList<string> historicDecisionInstanceIds, HistoricDecisionInstanceQuery historicDecisionInstanceQuery, string deleteReason)
	  {
		this.historicDecisionInstanceIds = historicDecisionInstanceIds;
		this.historicDecisionInstanceQuery = historicDecisionInstanceQuery;
		this.deleteReason = deleteReason;
	  }

	  protected internal virtual IList<string> collectHistoricDecisionInstanceIds()
	  {

		ISet<string> collectedDecisionInstanceIds = new HashSet<string>();

		IList<string> decisionInstanceIds = HistoricDecisionInstanceIds;
		if (decisionInstanceIds != null)
		{
		  collectedDecisionInstanceIds.addAll(decisionInstanceIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.HistoricDecisionInstanceQueryImpl decisionInstanceQuery = (org.camunda.bpm.engine.impl.HistoricDecisionInstanceQueryImpl) historicDecisionInstanceQuery;
		HistoricDecisionInstanceQueryImpl decisionInstanceQuery = (HistoricDecisionInstanceQueryImpl) historicDecisionInstanceQuery;
		if (decisionInstanceQuery != null)
		{
		  foreach (HistoricDecisionInstance hdi in decisionInstanceQuery.list())
		  {
			collectedDecisionInstanceIds.Add(hdi.Id);
		  }
		}

		return new List<string>(collectedDecisionInstanceIds);
	  }

	  public virtual IList<string> HistoricDecisionInstanceIds
	  {
		  get
		  {
			return historicDecisionInstanceIds;
		  }
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<string> decisionInstanceIds = collectHistoricDecisionInstanceIds();
		ensureNotEmpty(typeof(BadUserRequestException), "historicDecisionInstanceIds", decisionInstanceIds);

		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES);
		writeUserOperationLog(commandContext, decisionInstanceIds.Count);

		BatchEntity batch = createBatch(commandContext, decisionInstanceIds);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int numInstances)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, true));
		propertyChanges.Add(new PropertyChange("deleteReason", null, deleteReason));

		commandContext.OperationLogManager.logDecisionInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, propertyChanges);
	  }

	  protected internal override BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
	  {
		return new BatchConfiguration(processInstanceIds);
	  }

	  protected internal override BatchJobHandler<BatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<BatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION];
	  }

	}

}