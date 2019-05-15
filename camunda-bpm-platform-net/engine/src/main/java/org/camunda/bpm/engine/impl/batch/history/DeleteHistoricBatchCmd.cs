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
namespace org.camunda.bpm.engine.impl.batch.history
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	public class DeleteHistoricBatchCmd : Command<object>
	{

	  protected internal string batchId;

	  public DeleteHistoricBatchCmd(string batchId)
	  {
		this.batchId = batchId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Historic batch id must not be null", "historic batch id", batchId);

		HistoricBatchEntity historicBatch = commandContext.HistoricBatchManager.findHistoricBatchById(batchId);
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Historic batch for id '" + batchId + "' cannot be found", "historic batch", historicBatch);

		checkAccess(commandContext, historicBatch);

		writeUserOperationLog(commandContext);

		historicBatch.delete();

		return null;
	  }

	  protected internal virtual void checkAccess(CommandContext commandContext, HistoricBatchEntity batch)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricBatch(batch);
		}
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext)
	  {
		commandContext.OperationLogManager.logBatchOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, batchId, PropertyChange.EMPTY_CHANGE);
	  }

	}

}