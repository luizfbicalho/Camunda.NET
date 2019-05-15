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
namespace org.camunda.bpm.engine.impl.cmd.batch
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteHistoricProcessInstancesBatchCmd : AbstractIDBasedBatchCmd<Batch>
	{
	  protected internal readonly string deleteReason;
	  protected internal IList<string> historicProcessInstanceIds;
	  protected internal HistoricProcessInstanceQuery historicProcessInstanceQuery;

	  public DeleteHistoricProcessInstancesBatchCmd(IList<string> historicProcessInstanceIds, HistoricProcessInstanceQuery historicProcessInstanceQuery, string deleteReason) : base()
	  {
		this.historicProcessInstanceIds = historicProcessInstanceIds;
		this.historicProcessInstanceQuery = historicProcessInstanceQuery;
		this.deleteReason = deleteReason;
	  }

	  protected internal virtual IList<string> collectHistoricProcessInstanceIds()
	  {

		ISet<string> collectedProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = this.HistoricProcessInstanceIds;
		if (processInstanceIds != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl processInstanceQuery = (org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl) this.historicProcessInstanceQuery;
		HistoricProcessInstanceQueryImpl processInstanceQuery = (HistoricProcessInstanceQueryImpl) this.historicProcessInstanceQuery;
		if (processInstanceQuery != null)
		{
		  foreach (HistoricProcessInstance hpi in processInstanceQuery.list())
		  {
			collectedProcessInstanceIds.Add(hpi.Id);
		  }
		}

		return new List<string>(collectedProcessInstanceIds);
	  }

	  public virtual IList<string> HistoricProcessInstanceIds
	  {
		  get
		  {
			return historicProcessInstanceIds;
		  }
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<string> processInstanceIds = collectHistoricProcessInstanceIds();

		ensureNotEmpty(typeof(BadUserRequestException), "historicProcessInstanceIds", processInstanceIds);
		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES);
		writeUserOperationLog(commandContext, deleteReason, processInstanceIds.Count, true);

		BatchEntity batch = createBatch(commandContext, processInstanceIds);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, string deleteReason, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));
		propertyChanges.Add(new PropertyChange("deleteReason", null, deleteReason));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, null, null, null, propertyChanges);
	  }

	  protected internal override BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
	  {
		return new BatchConfiguration(processInstanceIds, false);
	  }

	  protected internal override BatchJobHandler<BatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<BatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION];
	  }
	}

}