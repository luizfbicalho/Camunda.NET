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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using MetricsQueryImpl = org.camunda.bpm.engine.impl.metrics.MetricsQueryImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MeterLogManager : AbstractManager
	{

	  public const string SELECT_METER_INTERVAL = "selectMeterLogAggregatedByTimeInterval";
	  public const string SELECT_METER_SUM = "selectMeterLogSum";
	  public const string DELETE_ALL_METER = "deleteAllMeterLogEntries";
	  public const string DELETE_ALL_METER_BY_TIMESTAMP_AND_REPORTER = "deleteMeterLogEntriesByTimestampAndReporter";

	  public virtual void insert(MeterLogEntity meterLogEntity)
	  {
		DbEntityManager.insert(meterLogEntity);
	  }

	  public virtual long? executeSelectSum(MetricsQueryImpl query)
	  {
		long? result = (long?) DbEntityManager.selectOne(SELECT_METER_SUM, query);
		result = result != null ? result : 0;

		if (shouldAddCurrentUnloggedCount(query))
		{
		  // add current unlogged count
		  Meter meter = Context.ProcessEngineConfiguration.MetricsRegistry.getMeterByName(query.Name);
		  if (meter != null)
		  {
			result += meter.get();
		  }
		}

		return result;
	  }

	  public virtual IList<MetricIntervalValue> executeSelectInterval(MetricsQueryImpl query)
	  {
		IList<MetricIntervalValue> intervalResult = DbEntityManager.selectList(SELECT_METER_INTERVAL, query);
		intervalResult = intervalResult != null ? intervalResult : new List<MetricIntervalValue>();

		string reporterId = Context.ProcessEngineConfiguration.DbMetricsReporter.MetricsCollectionTask.Reporter;
		if (intervalResult.Count > 0 && isEndTimeAfterLastReportInterval(query) && !string.ReferenceEquals(reporterId, null))
		{
		  IDictionary<string, Meter> metrics = Context.ProcessEngineConfiguration.MetricsRegistry.Meters;
		  string queryName = query.Name;
		  //we have to add all unlogged metrics to last interval
		  if (!string.ReferenceEquals(queryName, null))
		  {
			MetricIntervalEntity intervalEntity = (MetricIntervalEntity) intervalResult[0];
			long entityValue = intervalEntity.Value;
			if (metrics[queryName] != null)
			{
			  entityValue += metrics[queryName].get();
			}
			intervalEntity.Value = entityValue;
		  }
		  else
		  {
			ISet<string> metricNames = metrics.Keys;
			DateTime lastIntervalTimestamp = intervalResult[0].Timestamp;
			foreach (string metricName in metricNames)
			{
			  MetricIntervalEntity entity = new MetricIntervalEntity(lastIntervalTimestamp, metricName, reporterId);
			  int idx = intervalResult.IndexOf(entity);
			  if (idx >= 0)
			  {
				MetricIntervalEntity intervalValue = (MetricIntervalEntity) intervalResult[idx];
				intervalValue.Value = intervalValue.Value + metrics[metricName].get();
			  }
			}
		  }
		}
		return intervalResult;
	  }

	  protected internal virtual bool isEndTimeAfterLastReportInterval(MetricsQueryImpl query)
	  {
		long reportingIntervalInSeconds = Context.ProcessEngineConfiguration.DbMetricsReporter.ReportingIntervalInSeconds;

		return (query.EndDate == null || query.EndDateMilliseconds >= ClockUtil.CurrentTime.Ticks - (1000 * reportingIntervalInSeconds));
	  }

	  protected internal virtual bool shouldAddCurrentUnloggedCount(MetricsQueryImpl query)
	  {
		return !string.ReferenceEquals(query.Name, null) && isEndTimeAfterLastReportInterval(query);

	  }

	  public virtual void deleteAll()
	  {
		DbEntityManager.delete(typeof(MeterLogEntity), DELETE_ALL_METER, null);
	  }

	  public virtual void deleteByTimestampAndReporter(DateTime timestamp, string reporter)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		if (timestamp != null)
		{
		  parameters["milliseconds"] = timestamp.Ticks;
		}
		parameters["reporter"] = reporter;
		DbEntityManager.delete(typeof(MeterLogEntity), DELETE_ALL_METER_BY_TIMESTAMP_AND_REPORTER, parameters);
	  }

	}

}