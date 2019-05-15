using System;

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
	using BatchWindowConfiguration = org.camunda.bpm.engine.impl.cfg.BatchWindowConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class DefaultBatchWindowManager : BatchWindowManager
	{

	  public virtual BatchWindow getPreviousDayBatchWindow(DateTime date, ProcessEngineConfigurationImpl configuration)
	  {
		DateTime previousDay = addDays(date, -1);
		return getBatchWindowForDate(previousDay, configuration);
	  }

	  private BatchWindow getBatchWindowForDate(DateTime date, ProcessEngineConfigurationImpl configuration)
	  {

		//get configuration for given day of week
		BatchWindowConfiguration batchWindowConfiguration = configuration.HistoryCleanupBatchWindows[dayOfWeek(date)];
		if (batchWindowConfiguration == null && !string.ReferenceEquals(configuration.HistoryCleanupBatchWindowStartTime, null))
		{
		  batchWindowConfiguration = new BatchWindowConfiguration(configuration.HistoryCleanupBatchWindowStartTime, configuration.HistoryCleanupBatchWindowEndTime);
		}

		if (batchWindowConfiguration == null)
		{
		  return null;
		}

		DateTime startTime = updateTime(date, batchWindowConfiguration.StartTimeAsDate);
		DateTime endTime = updateTime(date, batchWindowConfiguration.EndTimeAsDate);
		if (!endTime > startTime)
		{
		  endTime = addDays(endTime, 1);
		}

		return new BatchWindow(startTime, endTime);
	  }

	  private int? dayOfWeek(DateTime date)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(date);
		return calendar.DayOfWeek;
	  }

	  public virtual BatchWindow getCurrentOrNextBatchWindow(DateTime date, ProcessEngineConfigurationImpl configuration)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow previousDayBatchWindow = getPreviousDayBatchWindow(date, configuration);
		BatchWindow previousDayBatchWindow = getPreviousDayBatchWindow(date, configuration);
		if (previousDayBatchWindow != null && previousDayBatchWindow.isWithin(date))
		{
		  return previousDayBatchWindow;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow currentDayBatchWindow = getBatchWindowForDate(date, configuration);
		BatchWindow currentDayBatchWindow = getBatchWindowForDate(date, configuration);
		if (currentDayBatchWindow != null && (currentDayBatchWindow.isWithin(date) || date < currentDayBatchWindow.Start))
		{
		  return currentDayBatchWindow;
		}

		//check next week
		for (int i = 1; i <= 7; i++)
		{
		  DateTime dateToCheck = addDays(date, i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow batchWindowForDate = getBatchWindowForDate(dateToCheck, configuration);
		  BatchWindow batchWindowForDate = getBatchWindowForDate(dateToCheck, configuration);
		  if (batchWindowForDate != null)
		  {
			return batchWindowForDate;
		  }
		}

		return null;
	  }

	  public virtual BatchWindow getNextBatchWindow(DateTime date, ProcessEngineConfigurationImpl configuration)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow currentDayBatchWindow = getBatchWindowForDate(date, configuration);
		BatchWindow currentDayBatchWindow = getBatchWindowForDate(date, configuration);
		if (currentDayBatchWindow != null && date < currentDayBatchWindow.Start)
		{
		  return currentDayBatchWindow;
		}
		else
		{
		  //check next week
		  for (int i = 1; i <= 7; i++)
		  {
			DateTime dateToCheck = addDays(date, i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow batchWindowForDate = getBatchWindowForDate(dateToCheck, configuration);
			BatchWindow batchWindowForDate = getBatchWindowForDate(dateToCheck, configuration);
			if (batchWindowForDate != null)
			{
			  return batchWindowForDate;
			}
		  }
		}
		return null;
	  }

	  public virtual bool isBatchWindowConfigured(ProcessEngineConfigurationImpl configuration)
	  {
		return configuration.HistoryCleanupBatchWindowStartTimeAsDate != null || configuration.HistoryCleanupBatchWindows.Count > 0;
	  }

	  private static DateTime updateTime(DateTime now, DateTime newTime)
	  {
		DateTime c = new DateTime();
		c = new DateTime(now);
		DateTime newTimeCalendar = new DateTime();
		newTimeCalendar = new DateTime(newTime);
		c.set(DateTime.ZONE_OFFSET, newTimeCalendar.get(DateTime.ZONE_OFFSET));
		c.set(DateTime.DST_OFFSET, newTimeCalendar.get(DateTime.DST_OFFSET));
		c.set(DateTime.HOUR_OF_DAY, newTimeCalendar.Hour);
		c.set(DateTime.MINUTE, newTimeCalendar.Minute);
		c.set(DateTime.SECOND, newTimeCalendar.Second);
		c.set(DateTime.MILLISECOND, newTimeCalendar.Millisecond);
		return c;
	  }

	  private static DateTime addDays(DateTime date, int amount)
	  {
		DateTime c = new DateTime();
		c = new DateTime(date);
		c.AddDays(amount);
		return c;
	  }

	}

}