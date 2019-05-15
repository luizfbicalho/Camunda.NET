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
namespace org.camunda.bpm.engine.impl.cfg
{
	using HistoryCleanupHelper = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHelper;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class BatchWindowConfiguration
	{

	  protected internal static readonly ConfigurationLogger LOG = ConfigurationLogger.CONFIG_LOGGER;

	  private string startTime;

	  private DateTime startTimeAsDate;

	  private string endTime = "00:00";

	  private DateTime endTimeAsDate;

	  public BatchWindowConfiguration()
	  {
	  }

	  public BatchWindowConfiguration(string startTime, string endTime)
	  {
		this.startTime = startTime;
		initStartTimeAsDate();
		if (!string.ReferenceEquals(endTime, null))
		{
		  this.endTime = endTime;
		}
		initEndTimeAsDate();
	  }

	  private void initStartTimeAsDate()
	  {
		try
		{
		  startTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(startTime);
		}
		catch (ParseException)
		{
		  throw LOG.invalidPropertyValue("startTime", startTime);
		}
	  }

	  private void initEndTimeAsDate()
	  {
		try
		{
		  endTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(endTime);
		}
		catch (ParseException)
		{
		  throw LOG.invalidPropertyValue("endTime", endTime);
		}
	  }

	  public virtual string StartTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			this.startTime = value;
			initStartTimeAsDate();
		  }
	  }


	  public virtual string EndTime
	  {
		  get
		  {
			return endTime;
		  }
		  set
		  {
			this.endTime = value;
			initEndTimeAsDate();
		  }
	  }


	  public virtual DateTime StartTimeAsDate
	  {
		  get
		  {
			return startTimeAsDate;
		  }
	  }

	  public virtual DateTime EndTimeAsDate
	  {
		  get
		  {
			return endTimeAsDate;
		  }
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		BatchWindowConfiguration that = (BatchWindowConfiguration) o;

		if (!string.ReferenceEquals(startTime, null) ?!startTime.Equals(that.startTime) :!string.ReferenceEquals(that.startTime, null))
		{
		  return false;
		}
		return !string.ReferenceEquals(endTime, null) ? endTime.Equals(that.endTime) : string.ReferenceEquals(that.endTime, null);
	  }

	  public override int GetHashCode()
	  {
		int result = !string.ReferenceEquals(startTime, null) ? startTime.GetHashCode() : 0;
		result = 31 * result + (!string.ReferenceEquals(endTime, null) ? endTime.GetHashCode() : 0);
		return result;
	  }
	}

}