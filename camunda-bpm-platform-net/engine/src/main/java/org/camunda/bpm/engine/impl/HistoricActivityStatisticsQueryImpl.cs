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
namespace org.camunda.bpm.engine.impl
{
	using DateUtils = org.apache.tools.ant.util.DateUtils;
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricActivityStatisticsQueryImpl : AbstractQuery<HistoricActivityStatisticsQuery, HistoricActivityStatistics>, HistoricActivityStatisticsQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processDefinitionId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeFinished_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeCanceled_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeCompleteScope_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedAfter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedAfter_Renamed;

	  public HistoricActivityStatisticsQueryImpl(string processDefinitionId, CommandExecutor commandExecutor) : base(commandExecutor)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual HistoricActivityStatisticsQuery includeFinished()
	  {
		includeFinished_Renamed = true;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery includeCanceled()
	  {
		includeCanceled_Renamed = true;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery includeCompleteScope()
	  {
		includeCompleteScope_Renamed = true;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery startedAfter(DateTime date)
	  {
		startedAfter_Renamed = date;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery startedBefore(DateTime date)
	  {
		startedBefore_Renamed = date;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery finishedAfter(DateTime date)
	  {
		finishedAfter_Renamed = date;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery finishedBefore(DateTime date)
	  {
		finishedBefore_Renamed = date;
		return this;
	  }

	  public virtual HistoricActivityStatisticsQuery orderByActivityId()
	  {
		return orderBy(HistoricActivityStatisticsQueryProperty_Fields.ACTIVITY_ID_);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricStatisticsManager.getHistoricStatisticsCountGroupedByActivity(this);
	  }

	  public override IList<HistoricActivityStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricStatisticsManager.getHistoricStatisticsGroupedByActivity(this, page);
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		ensureNotNull("No valid process definition id supplied", "processDefinitionId", processDefinitionId);
	  }

	  // getters /////////////////////////////////////////////////

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual bool IncludeFinished
	  {
		  get
		  {
			return includeFinished_Renamed;
		  }
	  }

	  public virtual bool IncludeCanceled
	  {
		  get
		  {
			return includeCanceled_Renamed;
		  }
	  }

	  public virtual bool IncludeCompleteScope
	  {
		  get
		  {
			return includeCompleteScope_Renamed;
		  }
	  }

	}

}