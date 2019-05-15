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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

	/// <summary>
	/// Job handler for history cleanup job.
	/// @author Svetlana Dorokhova
	/// </summary>
	public class HistoryCleanupJobHandler : JobHandler<HistoryCleanupJobHandlerConfiguration>
	{

	  public const string TYPE = "history-cleanup";

	  public virtual void execute(HistoryCleanupJobHandlerConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {

		HistoryCleanupHandler cleanupHandler = initCleanupHandler(configuration, commandContext);

		if (configuration.ImmediatelyDue || isWithinBatchWindow(commandContext))
		{
		  cleanupHandler.performCleanup();
		}

		commandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, cleanupHandler);

	  }

	  protected internal virtual HistoryCleanupHandler initCleanupHandler(HistoryCleanupJobHandlerConfiguration configuration, CommandContext commandContext)
	  {
		HistoryCleanupHandler cleanupHandler = null;

		if (isHistoryCleanupStrategyRemovalTimeBased(commandContext))
		{
		  cleanupHandler = new HistoryCleanupRemovalTime();
		}
		else
		{
		  cleanupHandler = new HistoryCleanupBatch();
		}

		CommandExecutor commandExecutor = commandContext.ProcessEngineConfiguration.CommandExecutorTxRequiresNew;

		string jobId = commandContext.CurrentJob.Id;

		return cleanupHandler.setConfiguration(configuration).setCommandExecutor(commandExecutor).setJobId(jobId);
	  }

	  protected internal virtual bool isHistoryCleanupStrategyRemovalTimeBased(CommandContext commandContext)
	  {
		string historyRemovalTimeStrategy = commandContext.ProcessEngineConfiguration.HistoryCleanupStrategy;

		return HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED.Equals(historyRemovalTimeStrategy);
	  }

	  protected internal virtual bool isWithinBatchWindow(CommandContext commandContext)
	  {
		return HistoryCleanupHelper.isWithinBatchWindow(ClockUtil.CurrentTime, commandContext.ProcessEngineConfiguration);
	  }

	  public virtual HistoryCleanupJobHandlerConfiguration newConfiguration(string canonicalString)
	  {
		JsonObject jsonObject = JsonUtil.asObject(canonicalString);
		return HistoryCleanupJobHandlerConfiguration.fromJson(jsonObject);
	  }

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void onDelete(HistoryCleanupJobHandlerConfiguration configuration, JobEntity jobEntity)
	  {
	  }

	}

}