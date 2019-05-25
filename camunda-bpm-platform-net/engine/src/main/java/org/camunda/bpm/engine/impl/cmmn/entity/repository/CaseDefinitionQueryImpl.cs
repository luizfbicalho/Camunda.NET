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
namespace org.camunda.bpm.engine.impl.cmmn.entity.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensurePositive;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseDefinitionQueryImpl : AbstractQuery<CaseDefinitionQuery, CaseDefinition>, CaseDefinitionQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string[] ids;
	  protected internal string category;
	  protected internal string categoryLike;
	  protected internal string name;
	  protected internal string nameLike;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Conflict;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string resourceName;
	  protected internal string resourceNameLike;
	  protected internal int? version;
	  protected internal bool latest = false;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal bool includeDefinitionsWithoutTenantId = false;

	  public CaseDefinitionQueryImpl()
	  {
	  }

	  public CaseDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  // Query parameter //////////////////////////////////////////////////////////////

	  public virtual CaseDefinitionQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionId", caseDefinitionId);
		this.id = caseDefinitionId;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionIdIn(params string[] ids)
	  {
		this.ids = ids;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionCategory(string caseDefinitionCategory)
	  {
		ensureNotNull(typeof(NotValidException), "category", caseDefinitionCategory);
		this.category = caseDefinitionCategory;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionCategoryLike(string caseDefinitionCategoryLike)
	  {
		ensureNotNull(typeof(NotValidException), "categoryLike", caseDefinitionCategoryLike);
		this.categoryLike = caseDefinitionCategoryLike;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionName(string caseDefinitionName)
	  {
		ensureNotNull(typeof(NotValidException), "name", caseDefinitionName);
		this.name = caseDefinitionName;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionNameLike(string caseDefinitionNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "nameLike", caseDefinitionNameLike);
		this.nameLike = caseDefinitionNameLike;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "key", caseDefinitionKey);
		this.key = caseDefinitionKey;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionKeyLike(string caseDefinitionKeyLike)
	  {
		ensureNotNull(typeof(NotValidException), "keyLike", caseDefinitionKeyLike);
		this.keyLike = caseDefinitionKeyLike;
		return this;
	  }

	  public virtual CaseDefinitionQuery deploymentId(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionVersion(int? caseDefinitionVersion)
	  {
		ensureNotNull(typeof(NotValidException), "version", caseDefinitionVersion);
		ensurePositive(typeof(NotValidException), "version", caseDefinitionVersion.Value);
		this.version = caseDefinitionVersion;
		return this;
	  }

	  public virtual CaseDefinitionQuery latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionResourceName(string resourceName)
	  {
		ensureNotNull(typeof(NotValidException), "resourceName", resourceName);
		this.resourceName = resourceName;
		return this;
	  }

	  public virtual CaseDefinitionQuery caseDefinitionResourceNameLike(string resourceNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "resourceNameLike", resourceNameLike);
		this.resourceNameLike = resourceNameLike;
		return this;
	  }

	  public virtual CaseDefinitionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseDefinitionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual CaseDefinitionQuery includeCaseDefinitionsWithoutTenantId()
	  {
		this.includeDefinitionsWithoutTenantId = true;
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByCaseDefinitionCategory()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.CASE_DEFINITION_CATEGORY);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByCaseDefinitionKey()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.CASE_DEFINITION_KEY);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByCaseDefinitionId()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.CASE_DEFINITION_ID);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByCaseDefinitionVersion()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.CASE_DEFINITION_VERSION);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByCaseDefinitionName()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.CASE_DEFINITION_NAME);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByDeploymentId()
	  {
		orderBy(CaseDefinitionQueryProperty_Fields.DEPLOYMENT_ID);
		return this;
	  }

	  public virtual CaseDefinitionQuery orderByTenantId()
	  {
		return orderBy(CaseDefinitionQueryProperty_Fields.TENANT_ID);
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.elementIsNotContainedInArray(id, ids);
	  }

	  //results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.CaseDefinitionManager.findCaseDefinitionCountByQueryCriteria(this);
	  }

	  public override IList<CaseDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.CaseDefinitionManager.findCaseDefinitionsByQueryCriteria(this, page);
	  }

	  public override void checkQueryOk()
	  {
		base.checkQueryOk();

		// latest() makes only sense when used with key() or keyLike()
		if (latest && ((!string.ReferenceEquals(id, null)) || (!string.ReferenceEquals(name, null)) || (!string.ReferenceEquals(nameLike, null)) || (version != null) || (!string.ReferenceEquals(deploymentId_Conflict, null))))
		{
		  throw new NotValidException("Calling latest() can only be used in combination with key(String) and keyLike(String)");
		}
	  }

	  // getters ////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string[] Ids
	  {
		  get
		  {
			return ids;
		  }
	  }

	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
	  }

	  public virtual string CategoryLike
	  {
		  get
		  {
			return categoryLike;
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

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Conflict;
		  }
	  }

	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
	  }

	  public virtual string KeyLike
	  {
		  get
		  {
			return keyLike;
		  }
	  }

	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
	  }

	  public virtual string ResourceNameLike
	  {
		  get
		  {
			return resourceNameLike;
		  }
	  }

	  public virtual int? Version
	  {
		  get
		  {
			return version;
		  }
	  }

	  public virtual bool Latest
	  {
		  get
		  {
			return latest;
		  }
	  }

	}

}