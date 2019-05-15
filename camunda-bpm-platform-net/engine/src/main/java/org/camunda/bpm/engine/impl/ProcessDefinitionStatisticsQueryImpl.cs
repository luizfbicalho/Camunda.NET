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

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;


	[Serializable]
	public class ProcessDefinitionStatisticsQueryImpl : AbstractQuery<ProcessDefinitionStatisticsQuery, ProcessDefinitionStatistics>, ProcessDefinitionStatisticsQuery
	{

	  protected internal const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeFailedJobs_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeIncidents_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeRootIncidents_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string includeIncidentsForType_Renamed;

	  public ProcessDefinitionStatisticsQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsCountGroupedByProcessDefinitionVersion(this);
	  }

	  public override IList<ProcessDefinitionStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsGroupedByProcessDefinitionVersion(this, page);
	  }

	  public virtual ProcessDefinitionStatisticsQuery includeFailedJobs()
	  {
		includeFailedJobs_Renamed = true;
		return this;
	  }

	  public virtual ProcessDefinitionStatisticsQuery includeIncidents()
	  {
		includeIncidents_Renamed = true;
		return this;
	  }

	  public virtual ProcessDefinitionStatisticsQuery includeIncidentsForType(string incidentType)
	  {
		this.includeIncidentsForType_Renamed = incidentType;
		return this;
	  }

	  public virtual bool FailedJobsToInclude
	  {
		  get
		  {
			return includeFailedJobs_Renamed;
		  }
	  }

	  public virtual bool IncidentsToInclude
	  {
		  get
		  {
			return includeIncidents_Renamed || includeRootIncidents_Renamed || !string.ReferenceEquals(includeIncidentsForType_Renamed, null);
		  }
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		if (includeIncidents_Renamed && !string.ReferenceEquals(includeIncidentsForType_Renamed, null))
		{
		  throw new ProcessEngineException("Invalid query: It is not possible to use includeIncident() and includeIncidentForType() to execute one query.");
		}
		if (includeRootIncidents_Renamed && !string.ReferenceEquals(includeIncidentsForType_Renamed, null))
		{
		  throw new ProcessEngineException("Invalid query: It is not possible to use includeRootIncident() and includeIncidentForType() to execute one query.");
		}
		if (includeIncidents_Renamed && includeRootIncidents_Renamed)
		{
		  throw new ProcessEngineException("Invalid query: It is not possible to use includeIncident() and includeRootIncidents() to execute one query.");
		}
	  }

	  public virtual ProcessDefinitionStatisticsQuery includeRootIncidents()
	  {
		this.includeRootIncidents_Renamed = true;
		return this;
	  }

	}

}