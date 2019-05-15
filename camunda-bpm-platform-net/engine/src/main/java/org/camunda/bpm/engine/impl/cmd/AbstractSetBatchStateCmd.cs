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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using BatchManager = org.camunda.bpm.engine.impl.persistence.entity.BatchManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	public abstract class AbstractSetBatchStateCmd : Command<Void>
	{

	  public const string SUSPENSION_STATE_PROPERTY = "suspensionState";

	  protected internal string batchId;

	  public AbstractSetBatchStateCmd(string batchId)
	  {
		this.batchId = batchId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull(typeof(BadUserRequestException), "Batch id must not be null", "batch id", batchId);

		BatchManager batchManager = commandContext.BatchManager;

		BatchEntity batch = batchManager.findBatchById(batchId);
		ensureNotNull(typeof(BadUserRequestException), "Batch for id '" + batchId + "' cannot be found", "batch", batch);

		checkAccess(commandContext, batch);

		setJobDefinitionState(commandContext, batch.SeedJobDefinitionId);
		setJobDefinitionState(commandContext, batch.MonitorJobDefinitionId);
		setJobDefinitionState(commandContext, batch.BatchJobDefinitionId);

		batchManager.updateBatchSuspensionStateById(batchId, NewSuspensionState);

		logUserOperation(commandContext);

		return null;
	  }

	  protected internal abstract SuspensionState NewSuspensionState {get;}

	  protected internal virtual void checkAccess(CommandContext commandContext, BatchEntity batch)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checkAccess(checker, batch);
		}
	  }

	  protected internal abstract void checkAccess(CommandChecker checker, BatchEntity batch);

	  protected internal virtual void setJobDefinitionState(CommandContext commandContext, string jobDefinitionId)
	  {
		createSetJobDefinitionStateCommand(jobDefinitionId).execute(commandContext);
	  }

	  protected internal virtual AbstractSetJobDefinitionStateCmd createSetJobDefinitionStateCommand(string jobDefinitionId)
	  {
		AbstractSetJobDefinitionStateCmd suspendJobDefinitionCmd = createSetJobDefinitionStateCommand(new UpdateJobDefinitionSuspensionStateBuilderImpl()
		  .byJobDefinitionId(jobDefinitionId).includeJobs(true));
		suspendJobDefinitionCmd.disableLogUserOperation();
		return suspendJobDefinitionCmd;
	  }

	  protected internal abstract AbstractSetJobDefinitionStateCmd createSetJobDefinitionStateCommand(UpdateJobDefinitionSuspensionStateBuilderImpl builder);

	  protected internal virtual void logUserOperation(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange(SUSPENSION_STATE_PROPERTY, null, NewSuspensionState.Name);
		commandContext.OperationLogManager.logBatchOperation(UserOperationType, batchId, propertyChange);
	  }

	  protected internal abstract string UserOperationType {get;}
	}

}