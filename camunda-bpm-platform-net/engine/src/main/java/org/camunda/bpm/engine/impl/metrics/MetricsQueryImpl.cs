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
namespace org.camunda.bpm.engine.impl.metrics
{
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class MetricsQueryImpl : ListQueryParameterObject, Command<object>, MetricsQuery
	{

	  public const int DEFAULT_LIMIT_SELECT_INTERVAL = 200;
	  public const long DEFAULT_SELECT_INTERVAL = 15 * 60;

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string reporter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startDate_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime endDate_Renamed;
	  protected internal long? startDateMilliseconds;
	  protected internal long? endDateMilliseconds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? interval_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? aggregateByReporter_Renamed;

	  [NonSerialized]
	  protected internal CommandExecutor commandExecutor;

	  public MetricsQueryImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		this.maxResults = DEFAULT_LIMIT_SELECT_INTERVAL;
		this.interval_Renamed = DEFAULT_SELECT_INTERVAL;
	  }

	  public virtual MetricsQueryImpl name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MetricsQuery reporter(string reporter)
	  {
		this.reporter_Renamed = reporter;
		return this;
	  }

	  public virtual MetricsQueryImpl startDate(DateTime startDate)
	  {
		this.startDate_Renamed = startDate;
		this.startDateMilliseconds = startDate.Ticks;
		return this;
	  }

	  public virtual MetricsQueryImpl endDate(DateTime endDate)
	  {
		this.endDate_Renamed = endDate;
		this.endDateMilliseconds = endDate.Ticks;
		return this;
	  }

	  /// <summary>
	  /// Contains the command implementation which should be executed either
	  /// metric sum or select metric grouped by time interval.
	  /// 
	  /// Note: this enables to quit with the enum distinction
	  /// </summary>
	  protected internal Command<object> callback;

	  public virtual IList<MetricIntervalValue> interval()
	  {
		callback = new CommandAnonymousInnerClass(this);

		return (IList<MetricIntervalValue>) commandExecutor.execute(this);
	  }

	  private class CommandAnonymousInnerClass : Command
	  {
		  private readonly MetricsQueryImpl outerInstance;

		  public CommandAnonymousInnerClass(MetricsQueryImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			return commandContext.MeterLogManager.executeSelectInterval(outerInstance);
		  }
	  }

	  public virtual IList<MetricIntervalValue> interval(long interval)
	  {
		this.interval_Renamed = interval;
		return interval();
	  }

	  public virtual long sum()
	  {
		callback = new CommandAnonymousInnerClass2(this);

		return (long?) commandExecutor.execute(this).Value;
	  }

	  private class CommandAnonymousInnerClass2 : Command
	  {
		  private readonly MetricsQueryImpl outerInstance;

		  public CommandAnonymousInnerClass2(MetricsQueryImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			return commandContext.MeterLogManager.executeSelectSum(outerInstance);
		  }
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (callback != null)
		{
		  return callback.execute(commandContext);
		}
		throw new ProcessEngineException("Query can't be executed. Use either sum or interval to query the metrics.");
	  }

	  public virtual MetricsQuery offset(int offset)
	  {
		FirstResult = offset;
		return this;
	  }

	  public virtual MetricsQuery limit(int maxResults)
	  {
		MaxResults = maxResults;
		return this;
	  }

	  public virtual MetricsQuery aggregateByReporter()
	  {
		aggregateByReporter_Renamed = true;
		return this;
	  }

	  public override int MaxResults
	  {
		  set
		  {
			if (value > DEFAULT_LIMIT_SELECT_INTERVAL)
			{
			  throw new ProcessEngineException("Metrics interval query row limit can't be set larger than " + DEFAULT_LIMIT_SELECT_INTERVAL + '.');
			}
			this.maxResults = value;
		  }
		  get
		  {
			if (maxResults > DEFAULT_LIMIT_SELECT_INTERVAL)
			{
			  return DEFAULT_LIMIT_SELECT_INTERVAL;
			}
			return base.MaxResults;
		  }
	  }

	  public virtual DateTime StartDate
	  {
		  get
		  {
			return startDate_Renamed;
		  }
	  }

	  public virtual DateTime EndDate
	  {
		  get
		  {
			return endDate_Renamed;
		  }
	  }

	  public virtual long? StartDateMilliseconds
	  {
		  get
		  {
			return startDateMilliseconds;
		  }
	  }

	  public virtual long? EndDateMilliseconds
	  {
		  get
		  {
			return endDateMilliseconds;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_Renamed;
		  }
	  }

	  public virtual string Reporter
	  {
		  get
		  {
			return reporter_Renamed;
		  }
	  }

	  public virtual long? Interval
	  {
		  get
		  {
			if (interval_Renamed == null)
			{
			  return DEFAULT_SELECT_INTERVAL;
			}
			return interval_Renamed;
		  }
	  }


	}

}