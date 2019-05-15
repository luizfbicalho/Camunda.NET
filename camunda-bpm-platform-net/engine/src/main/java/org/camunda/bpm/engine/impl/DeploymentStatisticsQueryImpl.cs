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

	using PermissionCheck = org.camunda.bpm.engine.impl.db.PermissionCheck;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;

	[Serializable]
	public class DeploymentStatisticsQueryImpl : AbstractQuery<DeploymentStatisticsQuery, DeploymentStatistics>, DeploymentStatisticsQuery
	{

	  protected internal const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeFailedJobs_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeIncidents_Renamed = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string includeIncidentsForType_Renamed;

	  // for internal use
	  protected internal IList<PermissionCheck> processInstancePermissionChecks = new List<PermissionCheck>();
	  protected internal IList<PermissionCheck> jobPermissionChecks = new List<PermissionCheck>();
	  protected internal IList<PermissionCheck> incidentPermissionChecks = new List<PermissionCheck>();

	  public DeploymentStatisticsQueryImpl(CommandExecutor executor) : base(executor)
	  {
	  }

	  public virtual DeploymentStatisticsQuery includeFailedJobs()
	  {
		includeFailedJobs_Renamed = true;
		return this;
	  }

	  public virtual DeploymentStatisticsQuery includeIncidents()
	  {
		includeIncidents_Renamed = true;
		return this;
	  }

	  public virtual DeploymentStatisticsQuery includeIncidentsForType(string incidentType)
	  {
		this.includeIncidentsForType_Renamed = incidentType;
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsCountGroupedByDeployment(this);
	  }

	  public override IList<DeploymentStatistics> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.StatisticsManager.getStatisticsGroupedByDeployment(this, page);
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
			return includeIncidents_Renamed || !string.ReferenceEquals(includeIncidentsForType_Renamed, null);
		  }
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		if (includeIncidents_Renamed && !string.ReferenceEquals(includeIncidentsForType_Renamed, null))
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