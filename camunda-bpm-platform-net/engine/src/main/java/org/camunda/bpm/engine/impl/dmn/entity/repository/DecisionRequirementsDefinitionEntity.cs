using System;

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

	using DmnDecisionRequirementsGraphImpl = org.camunda.bpm.dmn.engine.impl.DmnDecisionRequirementsGraphImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;

	[Serializable]
	public class DecisionRequirementsDefinitionEntity : DmnDecisionRequirementsGraphImpl, DecisionRequirementsDefinition, ResourceDefinitionEntity<DecisionRequirementsDefinitionEntity>, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision = 1;
	  protected internal string name;
	  protected internal string category;
	  protected internal string key;
	  protected internal int version;
	  protected internal string deploymentId;
	  protected internal string resourceName;
	  protected internal string diagramResourceName;
	  protected internal string tenantId;

	  // firstVersion is true, when version == 1 or when
	  // this definition does not have any previous definitions
	  protected internal bool firstVersion = false;
	  protected internal string previousDecisionRequirementsDefinitionId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }

	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }

	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
		  }
	  }

	  public virtual int Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
		  }
	  }

	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
		  set
		  {
			this.resourceName = value;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }

	  public virtual string DiagramResourceName
	  {
		  get
		  {
			return diagramResourceName;
		  }
		  set
		  {
			this.diagramResourceName = value;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }

	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return null;
		  }
		  set
		  {
			throw new System.NotSupportedException();
		  }
	  }

	  public virtual int Revision
	  {
		  set
		  {
			this.revision = value;
		  }
		  get
		  {
			return revision;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			return typeof(DecisionRequirementsDefinitionEntity);
		  }
	  }










	  public virtual ResourceDefinitionEntity PreviousDefinition
	  {
		  get
		  {
			DecisionRequirementsDefinitionEntity previousDecisionDefinition = null;
    
			string previousDecisionDefinitionId = PreviousDecisionRequirementsDefinitionId;
			if (!string.ReferenceEquals(previousDecisionDefinitionId, null))
			{
    
			  previousDecisionDefinition = loadDecisionRequirementsDefinition(previousDecisionDefinitionId);
    
			  if (previousDecisionDefinition == null)
			  {
				resetPreviousDecisionRequirementsDefinitionId();
				previousDecisionDefinitionId = PreviousDecisionRequirementsDefinitionId;
    
				if (!string.ReferenceEquals(previousDecisionDefinitionId, null))
				{
				  previousDecisionDefinition = loadDecisionRequirementsDefinition(previousDecisionDefinitionId);
				}
			  }
			}
    
			return previousDecisionDefinition;
		  }
	  }

	  public virtual void updateModifiableFieldsFromEntity(DecisionRequirementsDefinitionEntity updatingDefinition)
	  {
	  }

	  /// <summary>
	  /// Returns the cached version if exists; does not update the entity from the database in that case
	  /// </summary>
	  protected internal virtual DecisionRequirementsDefinitionEntity loadDecisionRequirementsDefinition(string decisionRequirementsDefinitionId)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		DecisionRequirementsDefinitionEntity decisionRequirementsDefinition = deploymentCache.findDecisionRequirementsDefinitionFromCache(decisionRequirementsDefinitionId);

		if (decisionRequirementsDefinition == null)
		{
		  CommandContext commandContext = Context.CommandContext;
		  DecisionRequirementsDefinitionManager manager = commandContext.DecisionRequirementsDefinitionManager;
		  decisionRequirementsDefinition = manager.findDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);

		  if (decisionRequirementsDefinition != null)
		  {
			decisionRequirementsDefinition = deploymentCache.resolveDecisionRequirementsDefinition(decisionRequirementsDefinition);
		  }
		}

		return decisionRequirementsDefinition;
	  }

	  public virtual string PreviousDecisionRequirementsDefinitionId
	  {
		  get
		  {
			ensurePreviousDecisionRequirementsDefinitionIdInitialized();
			return previousDecisionRequirementsDefinitionId;
		  }
	  }

	  public virtual string PreviousDecisionDefinitionId
	  {
		  set
		  {
			this.previousDecisionRequirementsDefinitionId = value;
		  }
	  }

	  protected internal virtual void resetPreviousDecisionRequirementsDefinitionId()
	  {
		previousDecisionRequirementsDefinitionId = null;
		ensurePreviousDecisionRequirementsDefinitionIdInitialized();
	  }

	  protected internal virtual void ensurePreviousDecisionRequirementsDefinitionIdInitialized()
	  {
		if (string.ReferenceEquals(previousDecisionRequirementsDefinitionId, null) && !firstVersion)
		{
		  previousDecisionRequirementsDefinitionId = Context.CommandContext.DecisionRequirementsDefinitionManager.findPreviousDecisionRequirementsDefinitionId(key, version, tenantId);

		  if (string.ReferenceEquals(previousDecisionRequirementsDefinitionId, null))
		  {
			firstVersion = true;
		  }
		}
	  }


	  public override string ToString()
	  {
		return "DecisionRequirementsDefinitionEntity [id=" + id + ", revision=" + revision + ", name=" + name + ", category=" + category + ", key=" + key + ", version=" + version + ", deploymentId=" + deploymentId + ", tenantId=" + tenantId + "]";
	  }

	}

}