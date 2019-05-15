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
namespace org.camunda.bpm.engine.impl.cmd
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using SetRetriesBatchConfiguration = org.camunda.bpm.engine.impl.batch.SetRetriesBatchConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AbstractIDBasedBatchCmd = org.camunda.bpm.engine.impl.cmd.batch.AbstractIDBasedBatchCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public abstract class AbstractSetJobsRetriesBatchCmd : AbstractIDBasedBatchCmd<Batch>
	{
	  protected internal int retries;

	  public override Batch execute(CommandContext commandContext)
	  {
		IList<string> jobIds = collectJobIds(commandContext);

		ensureNotEmpty(typeof(BadUserRequestException), "jobIds", jobIds);
		EnsureUtil.ensureGreaterThanOrEqual("Retries count", retries, 0);
		checkAuthorizations(commandContext, BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES);
		writeUserOperationLog(commandContext, retries, jobIds.Count, true);

		BatchEntity batch = createBatch(commandContext, jobIds);

		batch.createSeedJobDefinition();
		batch.createMonitorJobDefinition();
		batch.createBatchJobDefinition();

		batch.fireHistoricStartEvent();

		batch.createSeedJob();

		return batch;
	  }


	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int retries, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));
		propertyChanges.Add(new PropertyChange("retries", null, retries));

		commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, null, null, null, null, null, propertyChanges);
	  }

	  protected internal abstract IList<string> collectJobIds(CommandContext commandContext);

	  protected internal override SetRetriesBatchConfiguration getAbstractIdsBatchConfiguration(IList<string> ids)
	  {
		return new SetRetriesBatchConfiguration(ids, retries);
	  }

	  protected internal override BatchJobHandler<SetRetriesBatchConfiguration> getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return (BatchJobHandler<SetRetriesBatchConfiguration>) processEngineConfiguration.BatchHandlers[org.camunda.bpm.engine.batch.Batch_Fields.TYPE_SET_JOB_RETRIES];
	  }
	}

}