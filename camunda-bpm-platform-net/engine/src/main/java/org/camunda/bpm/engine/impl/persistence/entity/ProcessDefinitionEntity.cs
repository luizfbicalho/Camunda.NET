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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using StartFormHandler = org.camunda.bpm.engine.impl.form.handler.StartFormHandler;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ProcessDefinitionEntity : ProcessDefinitionImpl, ProcessDefinition, ResourceDefinitionEntity<ProcessDefinitionEntity>, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string key;
	  protected internal int revision = 1;
	  protected internal int version;
	  protected internal string category;
	  protected internal string deploymentId;
	  protected internal string resourceName;
	  protected internal int? historyLevel;
	  protected internal StartFormHandler startFormHandler;
	  protected internal string diagramResourceName;
	  protected internal bool isGraphicalNotationDefined;
	  protected internal IDictionary<string, TaskDefinition> taskDefinitions;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool hasStartFormKey_Renamed;
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;
	  protected internal string tenantId;
	  protected internal string versionTag;
	  protected internal int? historyTimeToLive;
	  protected internal bool isIdentityLinksInitialized = false;
	  protected internal IList<IdentityLinkEntity> definitionIdentityLinkEntities = new List<IdentityLinkEntity>();
	  protected internal ISet<Expression> candidateStarterUserIdExpressions = new HashSet<Expression>();
	  protected internal ISet<Expression> candidateStarterGroupIdExpressions = new HashSet<Expression>();
	  protected internal bool isStartableInTasklist;

	  // firstVersion is true, when version == 1 or when
	  // this definition does not have any previous definitions
	  protected internal bool firstVersion = false;
	  protected internal string previousProcessDefinitionId;

	  public ProcessDefinitionEntity() : base(null)
	  {
	  }

	  protected internal virtual void ensureNotSuspended()
	  {
		if (Suspended)
		{
		  throw LOG.suspendedEntityException("Process Definition", id);
		}
	  }

	  public override ExecutionEntity createProcessInstance()
	  {
		return (ExecutionEntity) base.createProcessInstance();
	  }

	  public override ExecutionEntity createProcessInstance(string businessKey)
	  {
		return (ExecutionEntity) base.createProcessInstance(businessKey);
	  }

	  public override ExecutionEntity createProcessInstance(string businessKey, string caseInstanceId)
	  {
		return (ExecutionEntity) base.createProcessInstance(businessKey, caseInstanceId);
	  }

	  public override ExecutionEntity createProcessInstance(string businessKey, ActivityImpl initial)
	  {
		return (ExecutionEntity) base.createProcessInstance(businessKey, initial);
	  }

	  protected internal override PvmExecutionImpl newProcessInstance()
	  {
		ExecutionEntity newExecution = ExecutionEntity.createNewExecution();

		if (!string.ReferenceEquals(tenantId, null))
		{
		  newExecution.TenantId = tenantId;
		}

		return newExecution;
	  }

	  public override ExecutionEntity createProcessInstance(string businessKey, string caseInstanceId, ActivityImpl initial)
	  {
		ensureNotSuspended();

		ExecutionEntity processInstance = (ExecutionEntity) createProcessInstanceForInitial(initial);

		// do not reset executions (CAM-2557)!
		// processInstance.setExecutions(new ArrayList<ExecutionEntity>());

		processInstance.setProcessDefinition(processDefinition);

		// Do not initialize variable map (let it happen lazily)

		// reset the process instance in order to have the db-generated process instance id available
		processInstance.setProcessInstance(processInstance);

		// initialize business key
		if (!string.ReferenceEquals(businessKey, null))
		{
		  processInstance.BusinessKey = businessKey;
		}

		// initialize case instance id
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  processInstance.CaseInstanceId = caseInstanceId;
		}

		if (!string.ReferenceEquals(tenantId, null))
		{
		  processInstance.TenantId = tenantId;
		}

		return processInstance;
	  }

	  public virtual IdentityLinkEntity addIdentityLink(string userId, string groupId)
	  {
		IdentityLinkEntity identityLinkEntity = IdentityLinkEntity.newIdentityLink();
		IdentityLinks.Add(identityLinkEntity);
		identityLinkEntity.ProcessDef = this;
		identityLinkEntity.UserId = userId;
		identityLinkEntity.GroupId = groupId;
		identityLinkEntity.Type = IdentityLinkType.CANDIDATE;
		identityLinkEntity.TenantId = TenantId;
		identityLinkEntity.insert();
		return identityLinkEntity;
	  }

	  public virtual void deleteIdentityLink(string userId, string groupId)
	  {
		IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkManager.findIdentityLinkByProcessDefinitionUserAndGroup(id, userId, groupId);

		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  identityLink.delete();
		}
	  }

	  public virtual IList<IdentityLinkEntity> IdentityLinks
	  {
		  get
		  {
			if (!isIdentityLinksInitialized)
			{
			  definitionIdentityLinkEntities = Context.CommandContext.IdentityLinkManager.findIdentityLinksByProcessDefinitionId(id);
			  isIdentityLinksInitialized = true;
			}
    
			return definitionIdentityLinkEntities;
		  }
	  }

	  public override string ToString()
	  {
		return "ProcessDefinitionEntity[" + id + "]";
	  }

	  /// <summary>
	  /// Updates all modifiable fields from another process definition entity. </summary>
	  /// <param name="updatingProcessDefinition"> </param>
	  public virtual void updateModifiableFieldsFromEntity(ProcessDefinitionEntity updatingProcessDefinition)
	  {
		if (this.key.Equals(updatingProcessDefinition.key) && this.deploymentId.Equals(updatingProcessDefinition.deploymentId))
		{
		  // TODO: add a guard once the mismatch between revisions in deployment cache and database has been resolved
		  this.revision = updatingProcessDefinition.revision;
		  this.suspensionState = updatingProcessDefinition.suspensionState;
		  this.historyTimeToLive = updatingProcessDefinition.historyTimeToLive;
		}
		else
		{
		  LOG.logUpdateUnrelatedProcessDefinitionEntity(this.key, updatingProcessDefinition.key, this.deploymentId, updatingProcessDefinition.deploymentId);
		}
	  }

	  // previous process definition //////////////////////////////////////////////

	  public virtual ProcessDefinitionEntity PreviousDefinition
	  {
		  get
		  {
			ProcessDefinitionEntity previousProcessDefinition = null;
    
			string previousProcessDefinitionId = PreviousProcessDefinitionId;
			if (!string.ReferenceEquals(previousProcessDefinitionId, null))
			{
    
			  previousProcessDefinition = loadProcessDefinition(previousProcessDefinitionId);
    
			  if (previousProcessDefinition == null)
			  {
				resetPreviousProcessDefinitionId();
				previousProcessDefinitionId = PreviousProcessDefinitionId;
    
				if (!string.ReferenceEquals(previousProcessDefinitionId, null))
				{
				  previousProcessDefinition = loadProcessDefinition(previousProcessDefinitionId);
				}
			  }
			}
    
			return previousProcessDefinition;
		  }
	  }

	  /// <summary>
	  /// Returns the cached version if exists; does not update the entity from the database in that case
	  /// </summary>
	  protected internal virtual ProcessDefinitionEntity loadProcessDefinition(string processDefinitionId)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		ProcessDefinitionEntity processDefinition = deploymentCache.findProcessDefinitionFromCache(processDefinitionId);

		if (processDefinition == null)
		{
		  CommandContext commandContext = Context.CommandContext;
		  ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;
		  processDefinition = processDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);

		  if (processDefinition != null)
		  {
			processDefinition = deploymentCache.resolveProcessDefinition(processDefinition);
		  }
		}

		return processDefinition;

	  }

	  public virtual string PreviousProcessDefinitionId
	  {
		  get
		  {
			ensurePreviousProcessDefinitionIdInitialized();
			return previousProcessDefinitionId;
		  }
		  set
		  {
			this.previousProcessDefinitionId = value;
		  }
	  }

	  protected internal virtual void resetPreviousProcessDefinitionId()
	  {
		previousProcessDefinitionId = null;
		ensurePreviousProcessDefinitionIdInitialized();
	  }


	  protected internal virtual void ensurePreviousProcessDefinitionIdInitialized()
	  {
		if (string.ReferenceEquals(previousProcessDefinitionId, null) && !firstVersion)
		{
		  previousProcessDefinitionId = Context.CommandContext.ProcessDefinitionManager.findPreviousProcessDefinitionId(key, version, tenantId);

		  if (string.ReferenceEquals(previousProcessDefinitionId, null))
		  {
			firstVersion = true;
		  }
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["suspensionState"] = this.suspensionState;
			persistentState["historyTimeToLive"] = this.historyTimeToLive;
			return persistentState;
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


	  public override string Description
	  {
		  get
		  {
			return (string) getProperty(BpmnParse.PROPERTYNAME_DOCUMENTATION);
		  }
	  }

	  public override string DeploymentId
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


	  public virtual int Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
			firstVersion = (this.version == 1);
		  }
	  }


	  public override string Id
	  {
		  set
		  {
			this.id = value;
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


	  public virtual int? HistoryLevel
	  {
		  get
		  {
			return historyLevel;
		  }
		  set
		  {
			this.historyLevel = value;
		  }
	  }


	  public virtual StartFormHandler StartFormHandler
	  {
		  get
		  {
			return startFormHandler;
		  }
		  set
		  {
			this.startFormHandler = value;
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


	  public override string DiagramResourceName
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


	  public virtual bool hasStartFormKey()
	  {
		return hasStartFormKey_Renamed;
	  }

	  public virtual bool HasStartFormKey
	  {
		  get
		  {
			return hasStartFormKey_Renamed;
		  }
		  set
		  {
			this.hasStartFormKey_Renamed = value;
		  }
	  }

	  public virtual bool StartFormKey
	  {
		  set
		  {
			this.hasStartFormKey_Renamed = value;
		  }
	  }


	  public virtual bool GraphicalNotationDefined
	  {
		  get
		  {
			return isGraphicalNotationDefined;
		  }
		  set
		  {
			this.isGraphicalNotationDefined = value;
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

	  public virtual int SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }

	  public virtual ISet<Expression> CandidateStarterUserIdExpressions
	  {
		  get
		  {
			return candidateStarterUserIdExpressions;
		  }
	  }

	  public virtual void addCandidateStarterUserIdExpression(Expression userId)
	  {
		candidateStarterUserIdExpressions.Add(userId);
	  }

	  public virtual ISet<Expression> CandidateStarterGroupIdExpressions
	  {
		  get
		  {
			return candidateStarterGroupIdExpressions;
		  }
	  }

	  public virtual void addCandidateStarterGroupIdExpression(Expression groupId)
	  {
		candidateStarterGroupIdExpressions.Add(groupId);
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


	  public virtual bool StartableInTasklist
	  {
		  get
		  {
			return isStartableInTasklist;
		  }
		  set
		  {
			this.isStartableInTasklist = value;
		  }
	  }

	}

}