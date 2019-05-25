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
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Ingo Richtsmeier
	/// </summary>
	[Serializable]
	public class DeploymentQueryImpl : AbstractQuery<DeploymentQuery, Deployment>, DeploymentQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Conflict;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal bool sourceQueryParamEnabled;
	  protected internal string source;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime deploymentBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime deploymentAfter_Conflict;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeDeploymentsWithoutTenantId_Conflict = false;

	  public DeploymentQueryImpl()
	  {
	  }

	  public DeploymentQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual DeploymentQueryImpl deploymentId(string deploymentId)
	  {
		ensureNotNull("Deployment id", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentName(string deploymentName)
	  {
		ensureNotNull("deploymentName", deploymentName);
		this.name = deploymentName;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentNameLike(string nameLike)
	  {
		ensureNotNull("deploymentNameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual DeploymentQuery deploymentSource(string source)
	  {
		sourceQueryParamEnabled = true;
		this.source = source;
		return this;
	  }

	  public virtual DeploymentQuery deploymentBefore(DateTime before)
	  {
		ensureNotNull("deploymentBefore", before);
		this.deploymentBefore_Conflict = before;
		return this;
	  }

	  public virtual DeploymentQuery deploymentAfter(DateTime after)
	  {
		ensureNotNull("deploymentAfter", after);
		this.deploymentAfter_Conflict = after;
		return this;
	  }

	  public virtual DeploymentQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DeploymentQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual DeploymentQuery includeDeploymentsWithoutTenantId()
	  {
		this.includeDeploymentsWithoutTenantId_Conflict = true;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(deploymentAfter_Conflict, deploymentBefore_Conflict);
	  }

	  //sorting ////////////////////////////////////////////////////////

	  public virtual DeploymentQuery orderByDeploymentId()
	  {
		return orderBy(DeploymentQueryProperty_Fields.DEPLOYMENT_ID);
	  }

	  public virtual DeploymentQuery orderByDeploymenTime()
	  {
		return orderBy(DeploymentQueryProperty_Fields.DEPLOY_TIME);
	  }

	  public virtual DeploymentQuery orderByDeploymentTime()
	  {
		return orderBy(DeploymentQueryProperty_Fields.DEPLOY_TIME);
	  }

	  public virtual DeploymentQuery orderByDeploymentName()
	  {
		return orderBy(DeploymentQueryProperty_Fields.DEPLOYMENT_NAME);
	  }

	  public virtual DeploymentQuery orderByTenantId()
	  {
		return orderBy(DeploymentQueryProperty_Fields.TENANT_ID);
	  }

	  //results ////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.DeploymentManager.findDeploymentCountByQueryCriteria(this);
	  }

	  public override IList<Deployment> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.DeploymentManager.findDeploymentsByQueryCriteria(this, page);
	  }

	  //getters ////////////////////////////////////////////////////////

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Conflict;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }

	  public virtual bool SourceQueryParamEnabled
	  {
		  get
		  {
			return sourceQueryParamEnabled;
		  }
	  }

	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
	  }

	  public virtual DateTime DeploymentBefore
	  {
		  get
		  {
			return deploymentBefore_Conflict;
		  }
	  }

	  public virtual DateTime DeploymentAfter
	  {
		  get
		  {
			return deploymentAfter_Conflict;
		  }
	  }
	}

}