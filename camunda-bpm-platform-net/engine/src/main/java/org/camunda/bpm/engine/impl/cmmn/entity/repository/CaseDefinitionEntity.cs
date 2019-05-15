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

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseDefinitionEntity : CmmnCaseDefinition, CaseDefinition, ResourceDefinitionEntity<CaseDefinitionEntity>, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal int revision = 1;
	  protected internal string category;
	  protected internal string key;
	  protected internal int version;
	  protected internal string deploymentId;
	  protected internal string resourceName;
	  protected internal string diagramResourceName;
	  protected internal string tenantId;
	  protected internal int? historyTimeToLive;

	  protected internal IDictionary<string, TaskDefinition> taskDefinitions;

	  // firstVersion is true, when version == 1 or when
	  // this definition does not have any previous definitions
	  protected internal bool firstVersion = false;
	  protected internal string previousCaseDefinitionId;

	  public CaseDefinitionEntity() : base(null)
	  {
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


	  public virtual IDictionary<string, TaskDefinition> TaskDefinitions
	  {
		  get
		  {
			return taskDefinitions;
		  }
		  set
		  {
			this.taskDefinitions = value;
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
			return historyTimeToLive;
		  }
		  set
		  {
			this.historyTimeToLive = value;
		  }
	  }


	  // previous case definition //////////////////////////////////////////////

	  public virtual CaseDefinitionEntity PreviousDefinition
	  {
		  get
		  {
			CaseDefinitionEntity previousCaseDefinition = null;
    
			string previousCaseDefinitionId = PreviousCaseDefinitionId;
			if (!string.ReferenceEquals(previousCaseDefinitionId, null))
			{
    
			  previousCaseDefinition = loadCaseDefinition(previousCaseDefinitionId);
    
			  if (previousCaseDefinition == null)
			  {
				resetPreviousCaseDefinitionId();
				previousCaseDefinitionId = PreviousCaseDefinitionId;
    
				if (!string.ReferenceEquals(previousCaseDefinitionId, null))
				{
				  previousCaseDefinition = loadCaseDefinition(previousCaseDefinitionId);
				}
			  }
			}
    
			return previousCaseDefinition;
		  }
	  }

	  /// <summary>
	  /// Returns the cached version if exists; does not update the entity from the database in that case
	  /// </summary>
	  protected internal virtual CaseDefinitionEntity loadCaseDefinition(string caseDefinitionId)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		CaseDefinitionEntity caseDefinition = deploymentCache.findCaseDefinitionFromCache(caseDefinitionId);

		if (caseDefinition == null)
		{
		  CommandContext commandContext = Context.CommandContext;
		  CaseDefinitionManager caseDefinitionManager = commandContext.CaseDefinitionManager;
		  caseDefinition = caseDefinitionManager.findCaseDefinitionById(caseDefinitionId);

		  if (caseDefinition != null)
		  {
			caseDefinition = deploymentCache.resolveCaseDefinition(caseDefinition);
		  }
		}

		return caseDefinition;

	  }

	  protected internal virtual string PreviousCaseDefinitionId
	  {
		  get
		  {
			ensurePreviousCaseDefinitionIdInitialized();
			return previousCaseDefinitionId;
		  }
		  set
		  {
			this.previousCaseDefinitionId = value;
		  }
	  }


	  protected internal virtual void resetPreviousCaseDefinitionId()
	  {
		previousCaseDefinitionId = null;
		ensurePreviousCaseDefinitionIdInitialized();
	  }

	  protected internal virtual void ensurePreviousCaseDefinitionIdInitialized()
	  {
		if (string.ReferenceEquals(previousCaseDefinitionId, null) && !firstVersion)
		{
		  previousCaseDefinitionId = Context.CommandContext.CaseDefinitionManager.findPreviousCaseDefinitionId(key, version, tenantId);

		  if (string.ReferenceEquals(previousCaseDefinitionId, null))
		  {
			firstVersion = true;
		  }
		}
	  }

	  protected internal override CmmnExecution newCaseInstance()
	  {
		CaseExecutionEntity caseInstance = new CaseExecutionEntity();

		if (!string.ReferenceEquals(tenantId, null))
		{
		  caseInstance.TenantId = tenantId;
		}

		Context.CommandContext.CaseExecutionManager.insertCaseExecution(caseInstance);
		return caseInstance;
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

	  public override string ToString()
	  {
		return "CaseDefinitionEntity[" + id + "]";
	  }

	  /// <summary>
	  /// Updates all modifiable fields from another case definition entity. </summary>
	  /// <param name="updatingCaseDefinition"> </param>
	  public virtual void updateModifiableFieldsFromEntity(CaseDefinitionEntity updatingCaseDefinition)
	  {
		if (this.key.Equals(updatingCaseDefinition.key) && this.deploymentId.Equals(updatingCaseDefinition.deploymentId))
		{
		  this.revision = updatingCaseDefinition.revision;
		  this.historyTimeToLive = updatingCaseDefinition.historyTimeToLive;
		}
		else
		{
		  LOG.logUpdateUnrelatedCaseDefinitionEntity(this.key, updatingCaseDefinition.key, this.deploymentId, updatingCaseDefinition.deploymentId);
		}
	  }
	}

}