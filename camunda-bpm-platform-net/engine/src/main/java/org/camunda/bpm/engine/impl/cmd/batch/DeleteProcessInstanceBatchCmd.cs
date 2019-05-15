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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using DeleteProcessInstanceBatchConfiguration = org.camunda.bpm.engine.impl.batch.deletion.DeleteProcessInstanceBatchConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteProcessInstanceBatchCmd : AbstractIDBasedBatchCmd<Batch>
	{
	  protected internal readonly string deleteReason;
	  protected internal IList<string> processInstanceIds;
	  protected internal ProcessInstanceQuery processInstanceQuery;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipSubprocesses;

	  public DeleteProcessInstanceBatchCmd(IList<string> processInstances, ProcessInstanceQuery processInstanceQuery, string deleteReason, bool skipCustomListeners, bool skipSubprocesses) : base()
	  {
		this.processInstanceIds = processInstances;
		this.processInstanceQuery = processInstanceQuery;
		this.deleteReason = deleteReason;
		this.skipCustomListeners = skipCustomListeners;
		this.skipSubprocesses = skipSubprocesses;
	  }

	  protected internal virtual IList<string> collectProcessInstanceIds()
	  {

		ISet<string> collectedProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = this.ProcessInstanceIds;
		if (processInstanceIds != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery = (org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl) this.processInstanceQuery;
		ProcessInstanceQueryImpl processInstanceQuery = (ProcessInstanceQueryImpl) this.processInstanceQuery;
		if (processInstanceQuery != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
		}

		return new List<string>(collectedProcessInstanceIds);
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<string> processInstanceIds = collectProcessInstanceIds();

		ensureNotEmpty(typeof(BadUserRequestException), "processInstanceIds", processInstanceIds);
		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES);
		writeUserOperationLog(commandContext, deleteReason, processInstanceIds.Count, true);

		BatchEntity batch = createBatch(commandContext, processInstanceIds);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }

	  protected internal override BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
	  {
		return new DeleteProcessInstanceBatchConfiguration(processInstanceIds, deleteReason, skipCustomListeners, skipSubprocesses, false);
	  }

	  protected internal override BatchJobHandler<DeleteProcessInstanceBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<DeleteProcessInstanceBatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_DELETION];
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, string deleteReason, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));
		propertyChanges.Add(new PropertyChange("deleteReason", null, deleteReason));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, null, null, null, propertyChanges);
	  }

	}

}