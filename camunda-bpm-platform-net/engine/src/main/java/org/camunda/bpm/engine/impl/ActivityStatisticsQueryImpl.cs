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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using PermissionCheck = org.camunda.bpm.engine.impl.db.PermissionCheck;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;

	[Serializable]
	public class ActivityStatisticsQueryImpl : AbstractQuery<ActivityStatisticsQuery, ActivityStatistics>, ActivityStatisticsQuery
	{

	  protected internal const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeFailedJobs_Conflict = false;
	  protected internal string processDefinitionId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeIncidents_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string includeIncidentsForType_Conflict;

	  // for internal use
	  protected internal IList<PermissionCheck> processInstancePermissionChecks = new List<PermissionCheck>();
	  protected internal IList<PermissionCheck> jobPermissionChecks = new List<PermissionCheck>();
	  protected internal IList<PermissionCheck> incidentPermissionChecks = new List<PermissionCheck>();

	  public ActivityStatisticsQueryImpl(string processDefinitionId, CommandExecutor executor) : base(executor)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsCountGroupedByActivity(this);
	  }

	  public override IList<ActivityStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsGroupedByActivity(this, page);
	  }

	  public virtual ActivityStatisticsQuery includeFailedJobs()
	  {
		includeFailedJobs_Conflict = true;
		return this;
	  }

	  public virtual ActivityStatisticsQuery includeIncidents()
	  {
		includeIncidents_Conflict = true;
		return this;
	  }

	  public virtual ActivityStatisticsQuery includeIncidentsForType(string incidentType)
	  {
		this.includeIncidentsForType_Conflict = incidentType;
		return this;
	  }

	  public virtual bool FailedJobsToInclude
	  {
		  get
		  {
			return includeFailedJobs_Conflict;
		  }
	  }

	  public virtual bool IncidentsToInclude
	  {
		  get
		  {
			return includeIncidents_Conflict || !string.ReferenceEquals(includeIncidentsForType_Conflict, null);
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		ensureNotNull("No valid process definition id supplied", "processDefinitionId", processDefinitionId);
		if (includeIncidents_Conflict && !string.ReferenceEquals(includeIncidentsForType_Conflict, null))
		{
		  throw new ProcessEngineException("Invalid query: It is not possible to use includeIncident() and includeIncidentForType() to execute one query.");
		}
	  }

	  // getter/setter for authorization check

	  public virtual IList<PermissionCheck> ProcessInstancePermissionChecks
	  {
		  get
		  {
			return processInstancePermissionChecks;
		  }
		  set
		  {
			this.processInstancePermissionChecks = value;
		  }
	  }


	  public virtual void addProcessInstancePermissionCheck(IList<PermissionCheck> permissionChecks)
	  {
		((IList<PermissionCheck>)processInstancePermissionChecks).AddRange(permissionChecks);
	  }

	  public virtual IList<PermissionCheck> JobPermissionChecks
	  {
		  get
		  {
			return jobPermissionChecks;
		  }
		  set
		  {
			this.jobPermissionChecks = value;
		  }
	  }


	  public virtual void addJobPermissionCheck(IList<PermissionCheck> permissionChecks)
	  {
		((IList<PermissionCheck>)jobPermissionChecks).AddRange(permissionChecks);
	  }

	  public virtual IList<PermissionCheck> IncidentPermissionChecks
	  {
		  get
		  {
			return incidentPermissionChecks;
		  }
		  set
		  {
			this.incidentPermissionChecks = value;
		  }
	  }


	  public virtual void addIncidentPermissionCheck(IList<PermissionCheck> permissionChecks)
	  {
		((IList<PermissionCheck>)incidentPermissionChecks).AddRange(permissionChecks);
	  }
	}

}