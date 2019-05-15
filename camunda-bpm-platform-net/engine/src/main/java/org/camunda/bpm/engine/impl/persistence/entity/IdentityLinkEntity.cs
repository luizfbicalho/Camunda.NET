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

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;


	/// <summary>
	/// @author Joram Barrez
	/// @author Deivarayan Azhagappan
	/// </summary>
	[Serializable]
	public class IdentityLinkEntity : IdentityLink, DbEntity, HasDbReferences
	{

	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string id;

	  protected internal string type;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string taskId;

	  protected internal string processDefId;

	  protected internal string tenantId;

	  protected internal TaskEntity task;

	  protected internal ProcessDefinitionEntity processDef;

	  public virtual object PersistentState
	  {
		  get
		  {
			return this.type;
		  }
	  }

	  public static IdentityLinkEntity createAndInsert()
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		identityLinkEntity.insert();
		return identityLinkEntity;
	  }

	  public static IdentityLinkEntity newIdentityLink()
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		 return identityLinkEntity;
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.DbEntityManager.insert(this);
		fireHistoricIdentityLinkEvent(HistoryEventTypes.IDENTITY_LINK_ADD);
	  }

	  public virtual void delete()
	  {
		delete(true);
	  }

	  public virtual void delete(bool withHistory)
	  {
		Context.CommandContext.DbEntityManager.delete(this);
		if (withHistory)
		{
		  fireHistoricIdentityLinkEvent(HistoryEventTypes.IDENTITY_LINK_DELETE);
		}
	  }

	  public virtual bool User
	  {
		  get
		  {
			return !string.ReferenceEquals(userId, null);
		  }
	  }

	  public virtual bool Group
	  {
		  get
		  {
			return !string.ReferenceEquals(groupId, null);
		  }
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


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			if (!string.ReferenceEquals(this.groupId, null) && !string.ReferenceEquals(value, null))
			{
			  throw LOG.taskIsAlreadyAssignedException("userId", "groupId");
			}
			this.userId = value;
		  }
	  }


	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
		  set
		  {
			if (!string.ReferenceEquals(this.userId, null) && !string.ReferenceEquals(value, null))
			{
			  throw LOG.taskIsAlreadyAssignedException("groupId", "userId");
			}
			this.groupId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string ProcessDefId
	  {
		  get
		  {
			return processDefId;
		  }
		  set
		  {
			this.processDefId = value;
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


	  public virtual TaskEntity Task
	  {
		  get
		  {
			if ((task == null) && (!string.ReferenceEquals(taskId, null)))
			{
			  this.task = Context.CommandContext.TaskManager.findTaskById(taskId);
			}
			return task;
		  }
		  set
		  {
			this.task = value;
			this.taskId = value.Id;
		  }
	  }


	  public virtual ProcessDefinitionEntity ProcessDef
	  {
		  get
		  {
			if ((processDef == null) && (!string.ReferenceEquals(processDefId, null)))
			{
			  this.processDef = Context.CommandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefId);
			}
			return processDef;
		  }
		  set
		  {
			this.processDef = value;
			this.processDefId = value.Id;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireHistoricIdentityLinkEvent(final org.camunda.bpm.engine.impl.history.event.HistoryEventType eventType)
	  public virtual void fireHistoricIdentityLinkEvent(HistoryEventType eventType)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		HistoryLevel historyLevel = processEngineConfiguration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(eventType, this))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, eventType));

		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly IdentityLinkEntity outerInstance;

		  private HistoryEventType eventType;

		  public HistoryEventCreatorAnonymousInnerClass(IdentityLinkEntity outerInstance, HistoryEventType eventType)
		  {
			  this.outerInstance = outerInstance;
			  this.eventType = eventType;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			HistoryEvent @event = null;
			if (HistoryEvent.IDENTITY_LINK_ADD.Equals(eventType.EventName))
			{
			  @event = producer.createHistoricIdentityLinkAddEvent(outerInstance);
			}
			else if (HistoryEvent.IDENTITY_LINK_DELETE.Equals(eventType.EventName))
			{
			  @event = producer.createHistoricIdentityLinkDeleteEvent(outerInstance);
			}
			return @event;
		  }
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(processDefId, null))
			{
			  referenceIdAndClass[processDefId] = typeof(ProcessDefinitionEntity);
			}
			if (!string.ReferenceEquals(taskId, null))
			{
			  referenceIdAndClass[taskId] = typeof(TaskEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", type=" + type + ", userId=" + userId + ", groupId=" + groupId + ", taskId=" + taskId + ", processDefId=" + processDefId + ", task=" + task + ", processDef=" + processDef + ", tenantId=" + tenantId + "]";
	  }
	}

}