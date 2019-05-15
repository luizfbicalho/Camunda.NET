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


	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CompositePermissionCheck = org.camunda.bpm.engine.impl.db.CompositePermissionCheck;
	using PermissionCheck = org.camunda.bpm.engine.impl.db.PermissionCheck;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Documentation = org.camunda.bpm.model.bpmn.instance.Documentation;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensurePositive;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Saeid Mirzaei
	/// </summary>
	[Serializable]
	public class ProcessDefinitionQueryImpl : AbstractQuery<ProcessDefinitionQuery, ProcessDefinition>, ProcessDefinitionQuery
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
	  protected internal string[] keys;
	  protected internal string keyLike;
	  protected internal string resourceName;
	  protected internal string resourceNameLike;
	  protected internal int? version;
	  protected internal bool latest = false;
	  protected internal SuspensionState suspensionState;
	  protected internal string authorizationUserId;
	  protected internal string procDefId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessageLike_Renamed;

	  protected internal string eventSubscriptionName;
	  protected internal string eventSubscriptionType;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal bool includeDefinitionsWithoutTenantId = false;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string versionTag_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string versionTagLike_Renamed;

	  protected internal bool isStartableInTasklist = false;
	  protected internal bool isNotStartableInTasklist = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool startablePermissionCheck_Renamed = false;
	  // for internal use
	  protected internal IList<PermissionCheck> processDefinitionCreatePermissionChecks = new List<PermissionCheck>();

	  public ProcessDefinitionQueryImpl()
	  {
	  }

	  public ProcessDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.id = processDefinitionId;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionIdIn(params string[] ids)
	  {
		this.ids = ids;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionCategory(string category)
	  {
		ensureNotNull("category", category);
		this.category = category;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionCategoryLike(string categoryLike)
	  {
		ensureNotNull("categoryLike", categoryLike);
		this.categoryLike = categoryLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionName(string name)
	  {
		ensureNotNull("name", name);
		this.name = name;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionNameLike(string nameLike)
	  {
		ensureNotNull("nameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl deploymentId(string deploymentId)
	  {
		ensureNotNull("deploymentId", deploymentId);
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionKey(string key)
	  {
		ensureNotNull("key", key);
		this.key = key;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionKeysIn(params string[] keys)
	  {
		ensureNotNull("keys", (object[]) keys);
		this.keys = keys;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionKeyLike(string keyLike)
	  {
		ensureNotNull("keyLike", keyLike);
		this.keyLike = keyLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionResourceName(string resourceName)
	  {
		ensureNotNull("resourceName", resourceName);
		this.resourceName = resourceName;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionResourceNameLike(string resourceNameLike)
	  {
		ensureNotNull("resourceNameLike", resourceNameLike);
		this.resourceNameLike = resourceNameLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionVersion(int? version)
	  {
		ensureNotNull("version", version);
		ensurePositive("version", version.Value);
		this.version = version;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual ProcessDefinitionQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual ProcessDefinitionQuery messageEventSubscription(string messageName)
	  {
		return eventSubscription(EventType.MESSAGE, messageName);
	  }

	  public virtual ProcessDefinitionQuery messageEventSubscriptionName(string messageName)
	  {
		return eventSubscription(EventType.MESSAGE, messageName);
	  }

	  public virtual ProcessDefinitionQuery processDefinitionStarter(string procDefId)
	  {
		this.procDefId = procDefId;
		return this;
	  }

	  public virtual ProcessDefinitionQuery eventSubscription(EventType eventType, string eventName)
	  {
		ensureNotNull("event type", eventType);
		ensureNotNull("event name", eventName);
		this.eventSubscriptionType = eventType.name();
		this.eventSubscriptionName = eventName;
		return this;
	  }

	  public virtual ProcessDefinitionQuery incidentType(string incidentType)
	  {
		ensureNotNull("incident type", incidentType);
		this.incidentType_Renamed = incidentType;
		return this;
	  }

	  public virtual ProcessDefinitionQuery incidentId(string incidentId)
	  {
		ensureNotNull("incident id", incidentId);
		this.incidentId_Renamed = incidentId;
		return this;
	  }

	  public virtual ProcessDefinitionQuery incidentMessage(string incidentMessage)
	  {
		ensureNotNull("incident message", incidentMessage);
		this.incidentMessage_Renamed = incidentMessage;
		return this;
	  }

	  public virtual ProcessDefinitionQuery incidentMessageLike(string incidentMessageLike)
	  {
		ensureNotNull("incident messageLike", incidentMessageLike);
		this.incidentMessageLike_Renamed = incidentMessageLike;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.elementIsNotContainedInArray(id, ids);
	  }

	  public virtual ProcessDefinitionQueryImpl tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual ProcessDefinitionQuery includeProcessDefinitionsWithoutTenantId()
	  {
		this.includeDefinitionsWithoutTenantId = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery versionTag(string versionTag)
	  {
		ensureNotNull("versionTag", versionTag);
		this.versionTag_Renamed = versionTag;

		return this;
	  }

	  public virtual ProcessDefinitionQuery versionTagLike(string versionTagLike)
	  {
		ensureNotNull("versionTagLike", versionTagLike);
		this.versionTagLike_Renamed = versionTagLike;

		return this;
	  }

	  public virtual ProcessDefinitionQuery startableInTasklist()
	  {
		this.isStartableInTasklist = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery notStartableInTasklist()
	  {
		this.isNotStartableInTasklist = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery startablePermissionCheck()
	  {
		this.startablePermissionCheck_Renamed = true;
		return this;
	  }

	  //sorting ////////////////////////////////////////////

	  public virtual ProcessDefinitionQuery orderByDeploymentId()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.DEPLOYMENT_ID);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionKey()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.PROCESS_DEFINITION_KEY);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionCategory()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.PROCESS_DEFINITION_CATEGORY);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionId()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionVersion()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.PROCESS_DEFINITION_VERSION);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionName()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.PROCESS_DEFINITION_NAME);
	  }

	  public virtual ProcessDefinitionQuery orderByTenantId()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual ProcessDefinitionQuery orderByVersionTag()
	  {
		return orderBy(ProcessDefinitionQueryProperty_Fields.VERSION_TAG);
	  }

	  //results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.ProcessDefinitionManager.findProcessDefinitionCountByQueryCriteria(this);
	  }

	  public override IList<ProcessDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		IList<ProcessDefinition> list = commandContext.ProcessDefinitionManager.findProcessDefinitionsByQueryCriteria(this, page);

		bool shouldQueryAddBpmnModelInstancesToCache = commandContext.ProcessEngineConfiguration.EnableFetchProcessDefinitionDescription;
		if (shouldQueryAddBpmnModelInstancesToCache)
		{
		  addProcessDefinitionToCacheAndRetrieveDocumentation(list);
		}

		return list;
	  }

	  protected internal virtual void addProcessDefinitionToCacheAndRetrieveDocumentation(IList<ProcessDefinition> list)
	  {
		foreach (ProcessDefinition processDefinition in list)
		{

		  BpmnModelInstance bpmnModelInstance = Context.ProcessEngineConfiguration.DeploymentCache.findBpmnModelInstanceForProcessDefinition((ProcessDefinitionEntity) processDefinition);

		  ModelElementInstance processElement = bpmnModelInstance.getModelElementById(processDefinition.Key);
		  if (processElement != null)
		  {
			ICollection<Documentation> documentations = processElement.getChildElementsByType(typeof(Documentation));
			IList<string> docStrings = new List<string>();
			foreach (Documentation documentation in documentations)
			{
			  docStrings.Add(documentation.TextContent);
			}

			ProcessDefinitionEntity processDefinitionEntity = (ProcessDefinitionEntity) processDefinition;
			processDefinitionEntity.setProperty(BpmnParse.PROPERTYNAME_DOCUMENTATION, BpmnParse.parseDocumentation(docStrings));
		  }

		}
	  }

	  public override void checkQueryOk()
	  {
		base.checkQueryOk();

		if (latest && ((!string.ReferenceEquals(id, null)) || (version != null) || (!string.ReferenceEquals(deploymentId_Renamed, null))))
		{
		  throw new ProcessEngineException("Calling latest() can only be used in combination with key(String) and keyLike(String) or name(String) and nameLike(String)");
		}
	  }

	  //getters ////////////////////////////////////////////

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
		  }
	  }
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
	  public virtual SuspensionState SuspensionState
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

	  public virtual string IncidentId
	  {
		  get
		  {
			return incidentId_Renamed;
		  }
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType_Renamed;
		  }
	  }

	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage_Renamed;
		  }
	  }

	  public virtual string IncidentMessageLike
	  {
		  get
		  {
			return incidentMessageLike_Renamed;
		  }
	  }

	  public virtual string VersionTag
	  {
		  get
		  {
			return versionTag_Renamed;
		  }
	  }

	  public virtual bool StartableInTasklist
	  {
		  get
		  {
			return isStartableInTasklist;
		  }
	  }

	  public virtual bool NotStartableInTasklist
	  {
		  get
		  {
			return isNotStartableInTasklist;
		  }
	  }

	  public virtual bool StartablePermissionCheck
	  {
		  get
		  {
			return startablePermissionCheck_Renamed;
		  }
	  }

	  public virtual IList<PermissionCheck> ProcessDefinitionCreatePermissionChecks
	  {
		  set
		  {
			this.processDefinitionCreatePermissionChecks = value;
		  }
		  get
		  {
			return processDefinitionCreatePermissionChecks;
		  }
	  }


	  public virtual void addProcessDefinitionCreatePermissionCheck(CompositePermissionCheck processDefinitionCreatePermissionCheck)
	  {
		((IList<PermissionCheck>)processDefinitionCreatePermissionChecks).AddRange(processDefinitionCreatePermissionCheck.AllPermissionChecks);
	  }

	  public virtual ProcessDefinitionQueryImpl startableByUser(string userId)
	  {
		ensureNotNull("userId", userId);
		this.authorizationUserId = userId;
		return this;
	  }

	}

}