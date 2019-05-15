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


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	[Serializable]
	public class JobDefinitionQueryImpl : AbstractQuery<JobDefinitionQuery, JobDefinition>, JobDefinitionQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string[] activityIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobConfiguration_Renamed;
	  protected internal SuspensionState suspensionState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withOverridingJobPriority_Renamed;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeJobDefinitionsWithoutTenantId_Renamed = false;

	  public JobDefinitionQueryImpl()
	  {
	  }

	  public JobDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual JobDefinitionQuery jobDefinitionId(string jobDefinitionId)
	  {
		ensureNotNull("Job definition id", jobDefinitionId);
		this.id = jobDefinitionId;
		return this;
	  }

	  public virtual JobDefinitionQuery activityIdIn(params string[] activityIds)
	  {
		ensureNotNull("Activity ids", (object[]) activityIds);
		this.activityIds = activityIds;
		return this;
	  }

	  public virtual JobDefinitionQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("Process definition id", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual JobDefinitionQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("Process definition key", processDefinitionKey);
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual JobDefinitionQuery jobType(string jobType)
	  {
		ensureNotNull("Job type", jobType);
		this.jobType_Renamed = jobType;
		return this;
	  }

	  public virtual JobDefinitionQuery jobConfiguration(string jobConfiguration)
	  {
		ensureNotNull("Job configuration", jobConfiguration);
		this.jobConfiguration_Renamed = jobConfiguration;
		return this;
	  }

	  public virtual JobDefinitionQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual JobDefinitionQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual JobDefinitionQuery withOverridingJobPriority()
	  {
		this.withOverridingJobPriority_Renamed = true;
		return this;
	  }

	  public virtual JobDefinitionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual JobDefinitionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual JobDefinitionQuery includeJobDefinitionsWithoutTenantId()
	  {
		this.includeJobDefinitionsWithoutTenantId_Renamed = true;
		return this;
	  }

	  // order by ///////////////////////////////////////////

	  public virtual JobDefinitionQuery orderByJobDefinitionId()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.JOB_DEFINITION_ID);
	  }

	  public virtual JobDefinitionQuery orderByActivityId()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.ACTIVITY_ID);
	  }

	  public virtual JobDefinitionQuery orderByProcessDefinitionId()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual JobDefinitionQuery orderByProcessDefinitionKey()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.PROCESS_DEFINITION_KEY);
	  }

	  public virtual JobDefinitionQuery orderByJobType()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.JOB_TYPE);
	  }

	  public virtual JobDefinitionQuery orderByJobConfiguration()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.JOB_CONFIGURATION);
	  }

	  public virtual JobDefinitionQuery orderByTenantId()
	  {
		return orderBy(JobDefinitionQueryProperty_Fields.TENANT_ID);
	  }

	  // results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.JobDefinitionManager.findJobDefinitionCountByQueryCriteria(this);
	  }

	  public override IList<JobDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.JobDefinitionManager.findJobDefnitionByQueryCriteria(this, page);
	  }

	  // getters /////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string[] ActivityIds
	  {
		  get
		  {
			return activityIds;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string JobType
	  {
		  get
		  {
			return jobType_Renamed;
		  }
	  }

	  public virtual string JobConfiguration
	  {
		  get
		  {
			return jobConfiguration_Renamed;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
	  }

	  public virtual bool? WithOverridingJobPriority
	  {
		  get
		  {
			return withOverridingJobPriority_Renamed;
		  }
	  }

	}

}