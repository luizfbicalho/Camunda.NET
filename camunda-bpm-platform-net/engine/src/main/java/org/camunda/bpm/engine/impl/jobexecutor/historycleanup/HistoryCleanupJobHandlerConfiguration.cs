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
	using JsonObject = com.google.gson.JsonObject;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class HistoryCleanupJobHandlerConfiguration : JobHandlerConfiguration
	{

	  public const int START_DELAY = 10; //10 seconds
	  public const int MAX_DELAY = 60 * 60; //hour

	  public const string JOB_CONFIG_COUNT_EMPTY_RUNS = "countEmptyRuns";
	  public const string JOB_CONFIG_EXECUTE_AT_ONCE = "immediatelyDue";
	  public const string JOB_CONFIG_MINUTE_FROM = "minuteFrom";
	  public const string JOB_CONFIG_MINUTE_TO = "minuteTo";

	  /// <summary>
	  /// Counts runs without data. Is used within batch window to calculate the delay between two job runs in case no data for cleanup was found.
	  /// </summary>
	  private int countEmptyRuns = 0;

	  /// <summary>
	  /// Indicated that the job was triggered manually and must be executed at once without waiting for batch window start time.
	  /// </summary>
	  private bool immediatelyDue;

	  /// <summary>
	  /// Process definition id.
	  /// </summary>
	  private int minuteFrom = 0;

	  private int minuteTo = 59;

	  public HistoryCleanupJobHandlerConfiguration()
	  {
	  }

	  public virtual string toCanonicalString()
	  {
		JsonObject json = JsonUtil.createObject();
		JsonUtil.addField(json, JOB_CONFIG_COUNT_EMPTY_RUNS, countEmptyRuns);
		JsonUtil.addField(json, JOB_CONFIG_EXECUTE_AT_ONCE, immediatelyDue);
		JsonUtil.addField(json, JOB_CONFIG_MINUTE_FROM, minuteFrom);
		JsonUtil.addField(json, JOB_CONFIG_MINUTE_TO, minuteTo);
		return json.ToString();
	  }

	  public static HistoryCleanupJobHandlerConfiguration fromJson(JsonObject jsonObject)
	  {
		HistoryCleanupJobHandlerConfiguration config = new HistoryCleanupJobHandlerConfiguration();
		if (jsonObject.has(JOB_CONFIG_COUNT_EMPTY_RUNS))
		{
		  config.CountEmptyRuns = JsonUtil.getInt(jsonObject, JOB_CONFIG_COUNT_EMPTY_RUNS);
		}
		if (jsonObject.has(JOB_CONFIG_EXECUTE_AT_ONCE))
		{
		  config.ImmediatelyDue = JsonUtil.getBoolean(jsonObject, JOB_CONFIG_EXECUTE_AT_ONCE);
		}
		config.MinuteFrom = JsonUtil.getInt(jsonObject, JOB_CONFIG_MINUTE_FROM);
		config.MinuteTo = JsonUtil.getInt(jsonObject, JOB_CONFIG_MINUTE_TO);
		return config;
	  }

	  /// <summary>
	  /// The delay between two "empty" runs increases twice each time until it reaches <seealso cref="HistoryCleanupJobHandlerConfiguration#MAX_DELAY"/> value. </summary>
	  /// <param name="date"> date to count delay from </param>
	  /// <returns> date with delay </returns>
	  public virtual DateTime getNextRunWithDelay(DateTime date)
	  {
		DateTime result = addSeconds(date, Math.Min((int)(Math.Pow(2.0, (double)countEmptyRuns) * START_DELAY), MAX_DELAY));
		return result;
	  }

	  private DateTime addSeconds(DateTime date, int amount)
	  {
		DateTime c = new DateTime();
		c = new DateTime(date);
		c.AddSeconds(amount);
		return c;
	  }

	  public virtual int CountEmptyRuns
	  {
		  get
		  {
			return countEmptyRuns;
		  }
		  set
		  {
			this.countEmptyRuns = value;
		  }
	  }


	  public virtual bool ImmediatelyDue
	  {
		  get
		  {
			return immediatelyDue;
		  }
		  set
		  {
			this.immediatelyDue = value;
		  }
	  }


	  public virtual int MinuteFrom
	  {
		  get
		  {
			return minuteFrom;
		  }
		  set
		  {
			this.minuteFrom = value;
		  }
	  }


	  public virtual int MinuteTo
	  {
		  get
		  {
			return minuteTo;
		  }
		  set
		  {
			this.minuteTo = value;
		  }
	  }

	}


}