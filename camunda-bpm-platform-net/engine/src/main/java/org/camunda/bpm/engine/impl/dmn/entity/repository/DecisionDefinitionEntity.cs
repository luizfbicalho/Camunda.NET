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

	using DmnDecisionImpl = org.camunda.bpm.dmn.engine.impl.DmnDecisionImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	[Serializable]
	public class DecisionDefinitionEntity : DmnDecisionImpl, DecisionDefinition, ResourceDefinitionEntity<DecisionDefinitionEntity>, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

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
	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string decisionRequirementsDefinitionKey;

	  // firstVersion is true, when version == 1 or when
	  // this definition does not have any previous definitions
	  protected internal bool firstVersion = false;
	  protected internal string previousDecisionDefinitionId;

	  protected internal int? historyTimeToLive;
	  protected internal string versionTag;

	  public DecisionDefinitionEntity()
	  {

	  }

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


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
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
			this.firstVersion = (this.version == 1);
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


	  public virtual string DecisionRequirementsDefinitionId
	  {
		  get
		  {
			return decisionRequirementsDefinitionId;
		  }
		  set
		  {
			this.decisionRequirementsDefinitionId = value;
		  }
	  }


	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  get
		  {
			return decisionRequirementsDefinitionKey;
		  }
		  set
		  {
			this.decisionRequirementsDefinitionKey = value;
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["historyTimeToLive"] = this.historyTimeToLive;
			return persistentState;
		  }
	  }

	  /// <summary>
	  /// Updates all modifiable fields from another decision definition entity.
	  /// </summary>
	  /// <param name="updatingDecisionDefinition"> </param>
	  public virtual void updateModifiableFieldsFromEntity(DecisionDefinitionEntity updatingDecisionDefinition)
	  {
		if (this.key.Equals(updatingDecisionDefinition.key) && this.deploymentId.Equals(updatingDecisionDefinition.deploymentId))
		{
		  this.revision = updatingDecisionDefinition.revision;
		  this.historyTimeToLive = updatingDecisionDefinition.historyTimeToLive;
		}
		else
		{
		  LOG.logUpdateUnrelatedDecisionDefinitionEntity(this.key, updatingDecisionDefinition.key, this.deploymentId, updatingDecisionDefinition.deploymentId);
		}
	  }

	  // previous decision definition //////////////////////////////////////////////

	  public virtual DecisionDefinitionEntity PreviousDefinition
	  {
		  get
		  {
			DecisionDefinitionEntity previousDecisionDefinition = null;
    
			string previousDecisionDefinitionId = PreviousDecisionDefinitionId;
			if (!string.ReferenceEquals(previousDecisionDefinitionId, null))
			{
    
			  previousDecisionDefinition = loadDecisionDefinition(previousDecisionDefinitionId);
    
			  if (previousDecisionDefinition == null)
			  {
				resetPreviousDecisionDefinitionId();
				previousDecisionDefinitionId = PreviousDecisionDefinitionId;
    
				if (!string.ReferenceEquals(previousDecisionDefinitionId, null))
				{
				  previousDecisionDefinition = loadDecisionDefinition(previousDecisionDefinitionId);
				}
			  }
			}
    
			return previousDecisionDefinition;
		  }
	  }

	  /// <summary>
	  /// Returns the cached version if exists; does not update the entity from the database in that case
	  /// </summary>
	  protected internal virtual DecisionDefinitionEntity loadDecisionDefinition(string decisionDefinitionId)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		DecisionDefinitionEntity decisionDefinition = deploymentCache.findDecisionDefinitionFromCache(decisionDefinitionId);

		if (decisionDefinition == null)
		{
		  CommandContext commandContext = Context.CommandContext;
		  DecisionDefinitionManager decisionDefinitionManager = commandContext.DecisionDefinitionManager;
		  decisionDefinition = decisionDefinitionManager.findDecisionDefinitionById(decisionDefinitionId);

		  if (decisionDefinition != null)
		  {
			decisionDefinition = deploymentCache.resolveDecisionDefinition(decisionDefinition);
		  }
		}

		return decisionDefinition;

	  }

	  public virtual string PreviousDecisionDefinitionId
	  {
		  get
		  {
			ensurePreviousDecisionDefinitionIdInitialized();
			return previousDecisionDefinitionId;
		  }
		  set
		  {
			this.previousDecisionDefinitionId = value;
		  }
	  }


	  protected internal virtual void resetPreviousDecisionDefinitionId()
	  {
		previousDecisionDefinitionId = null;
		ensurePreviousDecisionDefinitionIdInitialized();
	  }

	  protected internal virtual void ensurePreviousDecisionDefinitionIdInitialized()
	  {
		if (string.ReferenceEquals(previousDecisionDefinitionId, null) && !firstVersion)
		{
		  previousDecisionDefinitionId = Context.CommandContext.DecisionDefinitionManager.findPreviousDecisionDefinitionId(key, version, tenantId);

		  if (string.ReferenceEquals(previousDecisionDefinitionId, null))
		  {
			firstVersion = true;
		  }
		}
	  }

	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return historyTimeToLive;
		  }
		  set
		  {
			this.historyTimeToLive = value;
		  }
	  }


	  public virtual string VersionTag
	  {
		  get
		  {
			return versionTag;
		  }
		  set
		  {
			this.versionTag = value;
		  }
	  }


	  public override string ToString()
	  {
		return "DecisionDefinitionEntity{" +
		  "id='" + id + '\'' +
		  ", name='" + name + '\'' +
		  ", category='" + category + '\'' +
		  ", key='" + key + '\'' +
		  ", version=" + version +
		  ", versionTag=" + versionTag +
		  ", decisionRequirementsDefinitionId='" + decisionRequirementsDefinitionId + '\'' +
		  ", decisionRequirementsDefinitionKey='" + decisionRequirementsDefinitionKey + '\'' +
		  ", deploymentId='" + deploymentId + '\'' +
		  ", tenantId='" + tenantId + '\'' +
		  ", historyTimeToLive=" + historyTimeToLive +
		  '}';
	  }

	}

}