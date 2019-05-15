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
namespace org.camunda.bpm.engine.impl.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DeleteBatchCmd : Command<Void>
	{

	  protected internal bool cascadeToHistory;
	  protected internal string batchId;

	  public DeleteBatchCmd(string batchId, bool cascadeToHistory)
	  {
		this.batchId = batchId;
		this.cascadeToHistory = cascadeToHistory;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull(typeof(BadUserRequestException), "Batch id must not be null", "batch id", batchId);

		BatchEntity batchEntity = commandContext.BatchManager.findBatchById(batchId);
		ensureNotNull(typeof(BadUserRequestException), "Batch for id '" + batchId + "' cannot be found", "batch", batchEntity);

		checkAccess(commandContext, batchEntity);
		writeUserOperationLog(commandContext);
		batchEntity.delete(cascadeToHistory);

		return null;
	  }

	  protected internal virtual void checkAccess(CommandContext commandContext, BatchEntity batch)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteBatch(batch);
		}
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext)
	  {
		commandContext.OperationLogManager.logBatchOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, batchId, new PropertyChange("cascadeToHistory", null, cascadeToHistory));
	  }
	}

}