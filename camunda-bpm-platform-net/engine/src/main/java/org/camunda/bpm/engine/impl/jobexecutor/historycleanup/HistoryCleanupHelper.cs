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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public abstract class HistoryCleanupHelper
	{

	  private static readonly SimpleDateFormat TIME_FORMAT_WITHOUT_SECONDS = new SimpleDateFormat("yyyy-MM-ddHH:mm");

	  private static readonly SimpleDateFormat TIME_FORMAT_WITHOUT_SECONDS_WITH_TIMEZONE = new SimpleDateFormat("yyyy-MM-ddHH:mmZ");

	  private static readonly SimpleDateFormat DATE_FORMAT_WITHOUT_TIME = new SimpleDateFormat("yyyy-MM-dd");

	  /// <summary>
	  /// Checks if given date is within a batch window. Batch window start time is checked inclusively. </summary>
	  /// <param name="date">
	  /// @return </param>
	  public static bool isWithinBatchWindow(DateTime date, ProcessEngineConfigurationImpl configuration)
	  {
		if (configuration.BatchWindowManager.isBatchWindowConfigured(configuration))
		{
		  BatchWindow batchWindow = configuration.BatchWindowManager.getCurrentOrNextBatchWindow(date, configuration);
		  if (batchWindow == null)
		  {
			return false;
		  }
		  return batchWindow.isWithin(date);
		}
		else
		{
		  return false;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized java.util.Date parseTimeConfiguration(String time) throws java.text.ParseException
	  public static DateTime parseTimeConfiguration(string time)
	  {
		  lock (typeof(HistoryCleanupHelper))
		  {
			string today = DATE_FORMAT_WITHOUT_TIME.format(ClockUtil.CurrentTime);
			try
			{
			  return TIME_FORMAT_WITHOUT_SECONDS_WITH_TIMEZONE.parse(today + time);
			}
			catch (ParseException)
			{
			  return TIME_FORMAT_WITHOUT_SECONDS.parse(today + time);
			}
		  }
	  }

	  private static int? getHistoryCleanupBatchSize(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.HistoryCleanupBatchSize;
	  }

	  /// <summary>
	  /// Creates next batch object for history cleanup. First searches for historic process instances ready for cleanup. If there is still some place left in batch (configured batch
	  /// size was not reached), searches for historic decision instances and also adds them to the batch. Then if there is still some place left in batch, searches for historic case
	  /// instances and historic batches - and adds them to the batch.
	  /// </summary>
	  /// <param name="commandContext">
	  /// @return </param>
	  public static void prepareNextBatch(HistoryCleanupBatch historyCleanupBatch, CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HistoryCleanupJobHandlerConfiguration configuration = historyCleanupBatch.getConfiguration();
		HistoryCleanupJobHandlerConfiguration configuration = historyCleanupBatch.Configuration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> batchSize = getHistoryCleanupBatchSize(commandContext);
		int? batchSize = getHistoryCleanupBatchSize(commandContext);
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;

		//add process instance ids
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> historicProcessInstanceIds = commandContext.getHistoricProcessInstanceManager().findHistoricProcessInstanceIdsForCleanup(batchSize, configuration.getMinuteFrom(), configuration.getMinuteTo());
		IList<string> historicProcessInstanceIds = commandContext.HistoricProcessInstanceManager.findHistoricProcessInstanceIdsForCleanup(batchSize, configuration.MinuteFrom, configuration.MinuteTo);
		if (historicProcessInstanceIds.Count > 0)
		{
		  historyCleanupBatch.HistoricProcessInstanceIds = historicProcessInstanceIds;
		}

		//if batch is not full, add decision instance ids
		if (historyCleanupBatch.size() < batchSize.Value && processEngineConfiguration.DmnEnabled)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> historicDecisionInstanceIds = commandContext.getHistoricDecisionInstanceManager().findHistoricDecisionInstanceIdsForCleanup(batchSize - historyCleanupBatch.size(), configuration.getMinuteFrom(), configuration.getMinuteTo());
		  IList<string> historicDecisionInstanceIds = commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstanceIdsForCleanup(batchSize - historyCleanupBatch.size(), configuration.MinuteFrom, configuration.MinuteTo);
		  if (historicDecisionInstanceIds.Count > 0)
		  {
			historyCleanupBatch.HistoricDecisionInstanceIds = historicDecisionInstanceIds;
		  }
		}

		//if batch is not full, add case instance ids
		if (historyCleanupBatch.size() < batchSize.Value && processEngineConfiguration.CmmnEnabled)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> historicCaseInstanceIds = commandContext.getHistoricCaseInstanceManager().findHistoricCaseInstanceIdsForCleanup(batchSize - historyCleanupBatch.size(), configuration.getMinuteFrom(), configuration.getMinuteTo());
		  IList<string> historicCaseInstanceIds = commandContext.HistoricCaseInstanceManager.findHistoricCaseInstanceIdsForCleanup(batchSize - historyCleanupBatch.size(), configuration.MinuteFrom, configuration.MinuteTo);
		  if (historicCaseInstanceIds.Count > 0)
		  {
			historyCleanupBatch.HistoricCaseInstanceIds = historicCaseInstanceIds;
		  }
		}

		//if batch is not full, add batch ids
		IDictionary<string, int> batchOperationsForHistoryCleanup = processEngineConfiguration.ParsedBatchOperationsForHistoryCleanup;
		if (historyCleanupBatch.size() < batchSize.Value && batchOperationsForHistoryCleanup != null && batchOperationsForHistoryCleanup.Count > 0)
		{
		  IList<string> historicBatchIds = commandContext.HistoricBatchManager.findHistoricBatchIdsForCleanup(batchSize - historyCleanupBatch.size(), batchOperationsForHistoryCleanup, configuration.MinuteFrom, configuration.MinuteTo);
		  if (historicBatchIds.Count > 0)
		  {
			historyCleanupBatch.HistoricBatchIds = historicBatchIds;
		  }
		}
	  }

	  public static int[][] listMinuteChunks(int numberOfChunks)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] minuteChunks = new int[numberOfChunks][2];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] minuteChunks = new int[numberOfChunks][2];
		int[][] minuteChunks = RectangularArrays.RectangularIntArray(numberOfChunks, 2);
		int chunkLength = 60 / numberOfChunks;
		for (int i = 0; i < numberOfChunks; i++)
		{
		  minuteChunks[i][0] = chunkLength * i;
		  minuteChunks[i][1] = chunkLength * (i + 1) - 1;
		}
		minuteChunks[numberOfChunks - 1][1] = 59;
		return minuteChunks;
	  }

	  public static bool isBatchWindowConfigured(CommandContext commandContext)
	  {
		return commandContext.ProcessEngineConfiguration.BatchWindowManager.isBatchWindowConfigured(commandContext.ProcessEngineConfiguration);
	  }
	}

}