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
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;

	[Serializable]
	public class DecisionRequirementsDefinitionQueryImpl : AbstractQuery<DecisionRequirementsDefinitionQuery, DecisionRequirementsDefinition>, DecisionRequirementsDefinitionQuery
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

	  public DecisionRequirementsDefinitionQueryImpl()
	  {
	  }

	  public DecisionRequirementsDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  // Query parameter //////////////////////////////////////////////////////////////

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionId(string id)
	  {
		ensureNotNull(typeof(NotValidException), "id", id);
		this.id = id;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionIdIn(params string[] ids)
	  {
		this.ids = ids;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionCategory(string category)
	  {
		ensureNotNull(typeof(NotValidException), "category", category);
		this.category = category;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionCategoryLike(string categoryLike)
	  {
		ensureNotNull(typeof(NotValidException), "categoryLike", categoryLike);
		this.categoryLike = categoryLike;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionName(string name)
	  {
		ensureNotNull(typeof(NotValidException), "name", name);
		this.name = name;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionNameLike(string nameLike)
	  {
		ensureNotNull(typeof(NotValidException), "nameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionKey(string key)
	  {
		ensureNotNull(typeof(NotValidException), "key", key);
		this.key = key;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionKeyLike(string keyLike)
	  {
		ensureNotNull(typeof(NotValidException), "keyLike", keyLike);
		this.keyLike = keyLike;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery deploymentId(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionVersion(int? version)
	  {
		ensureNotNull(typeof(NotValidException), "version", version);
		ensurePositive(typeof(NotValidException), "version", version.Value);
		this.version = version;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionResourceName(string resourceName)
	  {
		ensureNotNull(typeof(NotValidException), "resourceName", resourceName);
		this.resourceName = resourceName;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionResourceNameLike(string resourceNameLike)
	  {
		ensureNotNull(typeof(NotValidException), "resourceNameLike", resourceNameLike);
		this.resourceNameLike = resourceNameLike;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery includeDecisionRequirementsDefinitionsWithoutTenantId()
	  {
		this.includeDefinitionsWithoutTenantId = true;
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionCategory()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DECISION_REQUIREMENTS_DEFINITION_CATEGORY);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionKey()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DECISION_REQUIREMENTS_DEFINITION_KEY);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionId()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DECISION_REQUIREMENTS_DEFINITION_ID);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionVersion()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DECISION_REQUIREMENTS_DEFINITION_VERSION);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionName()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DECISION_REQUIREMENTS_DEFINITION_NAME);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByDeploymentId()
	  {
		orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.DEPLOYMENT_ID);
		return this;
	  }

	  public virtual DecisionRequirementsDefinitionQuery orderByTenantId()
	  {
		return orderBy(DecisionRequirementsDefinitionQueryProperty_Fields.TENANT_ID);
	  }

	  //results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.DecisionRequirementsDefinitionManager.findDecisionRequirementsDefinitionCountByQueryCriteria(this);
	  }

	  public override IList<DecisionRequirementsDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.DecisionRequirementsDefinitionManager.findDecisionRequirementsDefinitionsByQueryCriteria(this, page);
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