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
namespace org.camunda.bpm.engine.impl.dmn.entity.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensurePositive;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;

	[Serializable]
	public class DecisionDefinitionQueryImpl : AbstractQuery<DecisionDefinitionQuery, DecisionDefinition>, DecisionDefinitionQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string[] ids;
	  protected internal string category;
	  protected internal string categoryLike;
	  protected internal string name;
	  protected internal string nameLike;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Renamed;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string resourceName;
	  protected internal string resourceNameLike;
	  protected internal int? version;
	  protected internal bool latest = false;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionRequirementsDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionRequirementsDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withoutDecisionRequirementsDefinition_Renamed = false;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal bool includeDefinitionsWithoutTenantId = false;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string versionTag_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string versionTagLike_Renamed;

	  public DecisionDefinitionQueryImpl()
	  {
	  }

	  public DecisionDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  // Query parameter //////////////////////////////////////////////////////////////

	  public virtual DecisionDefinitionQuery decisionDefinitionId(string decisionDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "decisionDefinitionId", decisionDefinitionId);
		this.id = decisionDefinitionId;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionIdIn(params string[] ids)
	  {
		this.ids = ids;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionCategory(string decisionDefinitionCategory)
	  {
		ensureNotNull(typeof(NotValidException), "category", decisionDefinitionCategory);
		this.category = decisionDefinitionCategory;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionCategoryLike(string decisionDefinitionCategoryLike)
	  {
		ensureNotNull(typeof(NotValidException), "categoryLike", decisionDefinitionCategoryLike);
		this.categoryLike = decisionDefinitionCategoryLike;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionName(string decisionDefinitionName)
	  {
		ensureNotNull(typeof(NotValidException), "name", decisionDefinitionName);
		this.name = decisionDefinitionName;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionNameLike(string decisionDefinitionNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "nameLike", decisionDefinitionNameLike);
		this.nameLike = decisionDefinitionNameLike;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionKey(string decisionDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "key", decisionDefinitionKey);
		this.key = decisionDefinitionKey;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionKeyLike(string decisionDefinitionKeyLike)
	  {
		ensureNotNull(typeof(NotValidException), "keyLike", decisionDefinitionKeyLike);
		this.keyLike = decisionDefinitionKeyLike;
		return this;
	  }

	  public virtual DecisionDefinitionQuery deploymentId(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionVersion(int? decisionDefinitionVersion)
	  {
		ensureNotNull(typeof(NotValidException), "version", decisionDefinitionVersion);
		ensurePositive(typeof(NotValidException), "version", decisionDefinitionVersion.Value);
		this.version = decisionDefinitionVersion;
		return this;
	  }

	  public virtual DecisionDefinitionQuery latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionResourceName(string resourceName)
	  {
		ensureNotNull(typeof(NotValidException), "resourceName", resourceName);
		this.resourceName = resourceName;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionDefinitionResourceNameLike(string resourceNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "resourceNameLike", resourceNameLike);
		this.resourceNameLike = resourceNameLike;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionRequirementsDefinitionId(string decisionRequirementsDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "decisionRequirementsDefinitionId", decisionRequirementsDefinitionId);
		this.decisionRequirementsDefinitionId_Renamed = decisionRequirementsDefinitionId;
		return this;
	  }

	  public virtual DecisionDefinitionQuery decisionRequirementsDefinitionKey(string decisionRequirementsDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "decisionRequirementsDefinitionKey", decisionRequirementsDefinitionKey);
		this.decisionRequirementsDefinitionKey_Renamed = decisionRequirementsDefinitionKey;
		return this;
	  }

	  public virtual DecisionDefinitionQuery versionTag(string versionTag)
	  {
		ensureNotNull(typeof(NotValidException), "versionTag", versionTag);
		this.versionTag_Renamed = versionTag;
		return this;
	  }

	  public virtual DecisionDefinitionQuery versionTagLike(string versionTagLike)
	  {
		ensureNotNull(typeof(NotValidException), "versionTagLike", versionTagLike);
		this.versionTagLike_Renamed = versionTagLike;
		return this;
	  }

	  public virtual DecisionDefinitionQuery withoutDecisionRequirementsDefinition()
	  {
		withoutDecisionRequirementsDefinition_Renamed = true;
		return this;
	  }

	  public virtual DecisionDefinitionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DecisionDefinitionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual DecisionDefinitionQuery includeDecisionDefinitionsWithoutTenantId()
	  {
		this.includeDefinitionsWithoutTenantId = true;
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDecisionDefinitionCategory()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DECISION_DEFINITION_CATEGORY);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDecisionDefinitionKey()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DECISION_DEFINITION_KEY);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDecisionDefinitionId()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DECISION_DEFINITION_ID);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDecisionDefinitionVersion()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DECISION_DEFINITION_VERSION);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDecisionDefinitionName()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DECISION_DEFINITION_NAME);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByDeploymentId()
	  {
		orderBy(DecisionDefinitionQueryProperty_Fields.DEPLOYMENT_ID);
		return this;
	  }

	  public virtual DecisionDefinitionQuery orderByTenantId()
	  {
		return orderBy(DecisionDefinitionQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual DecisionDefinitionQuery orderByVersionTag()
	  {
		return orderBy(DecisionDefinitionQueryProperty_Fields.VERSION_TAG);
	  }

	  //results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.DecisionDefinitionManager.findDecisionDefinitionCountByQueryCriteria(this);
	  }

	  public override IList<DecisionDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.DecisionDefinitionManager.findDecisionDefinitionsByQueryCriteria(this, page);
	  }

	  public override void checkQueryOk()
	  {
		base.checkQueryOk();

		// latest() makes only sense when used with key() or keyLike()
		if (latest && ((!string.ReferenceEquals(id, null)) || (!string.ReferenceEquals(name, null)) || (!string.ReferenceEquals(nameLike, null)) || (version != null) || (!string.ReferenceEquals(deploymentId_Renamed, null))))
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
			return deploymentId_Renamed;
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

	  public virtual string VersionTag
	  {
		  get
		  {
			return versionTag_Renamed;
		  }
	  }

	  public virtual string VersionTagLike
	  {
		  get
		  {
			return versionTagLike_Renamed;
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