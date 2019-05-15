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


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using EventHandler = org.camunda.bpm.engine.impl.@event.EventHandler;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.EventSubscriptionJobDeclaration;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class EventSubscriptionEntity : EventSubscription, DbEntity, HasDbRevision, HasDbReferences
	{

	  private const long serialVersionUID = 1L;

	  // persistent state ///////////////////////////
	  protected internal string id;
	  protected internal int revision = 1;
	  protected internal string eventType;
	  protected internal string eventName;

	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal string activityId;
	  protected internal string configuration;
	  protected internal DateTime created;
	  protected internal string tenantId;

	  // runtime state /////////////////////////////
	  protected internal ExecutionEntity execution;
	  protected internal ActivityImpl activity;
	  protected internal EventSubscriptionJobDeclaration jobDeclaration;

	  /////////////////////////////////////////////

	  //only for mybatis
	  public EventSubscriptionEntity()
	  {
	  }

	  public EventSubscriptionEntity(EventType eventType)
	  {
		this.created = ClockUtil.CurrentTime;
		this.eventType = eventType.name();
	  }

	  public EventSubscriptionEntity(ExecutionEntity executionEntity, EventType eventType) : this(eventType)
	  {
		Execution = executionEntity;
		Activity = execution.getActivity();
		this.processInstanceId = executionEntity.ProcessInstanceId;
		this.tenantId = executionEntity.TenantId;
	  }

	  // processing /////////////////////////////
	  public virtual void eventReceived(object payload, bool processASync)
	  {
		eventReceived(payload, null, null, processASync);
	  }

	  public virtual void eventReceived(object payload, object payloadLocal, string businessKey, bool processASync)
	  {
		if (processASync)
		{
		  scheduleEventAsync(payload, payloadLocal, businessKey);
		}
		else
		{
		  processEventSync(payload, payloadLocal, businessKey);
		}
	  }

	  protected internal virtual void processEventSync(object payload)
	  {
		this.processEventSync(payload, null, null);
	  }

	  protected internal virtual void processEventSync(object payload, object payloadLocal, string businessKey)
	  {
		EventHandler eventHandler = Context.ProcessEngineConfiguration.getEventHandler(eventType);
		ensureNotNull("Could not find eventhandler for event of type '" + eventType + "'", "eventHandler", eventHandler);
		eventHandler.handleEvent(this, payload, payloadLocal, businessKey, Context.CommandContext);
	  }

	  protected internal virtual void scheduleEventAsync(object payload, object payloadLocal, string businessKey)
	  {

		EventSubscriptionJobDeclaration asyncDeclaration = JobDeclaration;

		if (asyncDeclaration == null)
		{
		  // fallback to sync if we couldn't find a job declaration
		  processEventSync(payload, payloadLocal, businessKey);
		}
		else
		{
		  MessageEntity message = asyncDeclaration.createJobInstance(this);
		  CommandContext commandContext = Context.CommandContext;
		  commandContext.JobManager.send(message);
		}
	  }

	  // persistence behavior /////////////////////

	  public virtual void delete()
	  {
		Context.CommandContext.EventSubscriptionManager.deleteEventSubscription(this);
		removeFromExecution();
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.EventSubscriptionManager.insert(this);
		addToExecution();
	  }


	  public static EventSubscriptionEntity createAndInsert(ExecutionEntity executionEntity, EventType eventType, ActivityImpl activity)
	  {
		return createAndInsert(executionEntity, eventType, activity, null);
	  }

	  public static EventSubscriptionEntity createAndInsert(ExecutionEntity executionEntity, EventType eventType, ActivityImpl activity, string configuration)
	  {
		EventSubscriptionEntity eventSubscription = new EventSubscriptionEntity(executionEntity, eventType);
		eventSubscription.Activity = activity;
		eventSubscription.TenantId = executionEntity.TenantId;
		eventSubscription.Configuration = configuration;
		eventSubscription.insert();
		return eventSubscription;
	  }

	 // referential integrity -> ExecutionEntity ////////////////////////////////////

	  protected internal virtual void addToExecution()
	  {
		// add reference in execution
		ExecutionEntity execution = Execution;
		if (execution != null)
		{
		  execution.addEventSubscription(this);
		}
	  }

	  protected internal virtual void removeFromExecution()
	  {
		// remove reference in execution
		ExecutionEntity execution = Execution;
		if (execution != null)
		{
		  execution.removeEventSubscription(this);
		}
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			Dictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["executionId"] = executionId;
			persistentState["configuration"] = configuration;
			persistentState["activityId"] = activityId;
			persistentState["eventName"] = eventName;
			return persistentState;
		  }
	  }

	  // getters & setters ////////////////////////////

	  public virtual ExecutionEntity Execution
	  {
		  get
		  {
			if (execution == null && !string.ReferenceEquals(executionId, null))
			{
			  execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
			}
			return execution;
		  }
		  set
		  {
			if (value != null)
			{
			  this.execution = value;
			  this.executionId = value.Id;
			  addToExecution();
			}
			else
			{
			  removeFromExecution();
			  this.executionId = null;
			  this.execution = null;
			}
		  }
	  }


	  public virtual ActivityImpl Activity
	  {
		  get
		  {
			if (activity == null && !string.ReferenceEquals(activityId, null))
			{
			  ProcessDefinitionImpl processDefinition = ProcessDefinition;
			  activity = processDefinition.findActivity(activityId);
			}
			return activity;
		  }
		  set
		  {
			this.activity = value;
			if (value != null)
			{
			  this.activityId = value.Id;
			}
		  }
	  }

	  public virtual ProcessDefinitionEntity ProcessDefinition
	  {
		  get
		  {
			if (!string.ReferenceEquals(executionId, null))
			{
			  ExecutionEntity execution = Execution;
			  return (ProcessDefinitionEntity) execution.getProcessDefinition();
			}
			else
			{
			  // this assumes that start event subscriptions have the process definition id
			  // as their configuration (which holds for message and signal start events)
			  string processDefinitionId = Configuration;
			  return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
			}
		  }
	  }


	  public virtual EventSubscriptionJobDeclaration JobDeclaration
	  {
		  get
		  {
			if (jobDeclaration == null)
			{
			  jobDeclaration = EventSubscriptionJobDeclaration.findDeclarationForSubscription(this);
			}
    
			return jobDeclaration;
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

	  public virtual bool isSubscriptionForEventType(EventType eventType)
	  {
		return this.eventType.Equals(eventType.name());
	  }

	  public virtual string EventType
	  {
		  get
		  {
			return eventType;
		  }
		  set
		  {
			this.eventType = value;
		  }
	  }


	  public virtual string EventName
	  {
		  get
		  {
			return this.eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
			this.activity = null;
		  }
	  }


	  public virtual DateTime Created
	  {
		  get
		  {
			return created;
		  }
		  set
		  {
			this.created = value;
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


	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(id, null)) ? 0 : id.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		EventSubscriptionEntity other = (EventSubscriptionEntity) obj;
		if (string.ReferenceEquals(id, null))
		{
		  if (!string.ReferenceEquals(other.id, null))
		  {
			return false;
		  }
		}
		else if (!id.Equals(other.id))
		{
		  return false;
		}
		return true;
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
    
			if (!string.ReferenceEquals(executionId, null))
			{
			  referenceIdAndClass[executionId] = typeof(ExecutionEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", eventType=" + eventType + ", eventName=" + eventName + ", executionId=" + executionId + ", processInstanceId=" + processInstanceId + ", activityId=" + activityId + ", tenantId=" + tenantId + ", configuration=" + configuration + ", revision=" + revision + ", created=" + created + "]";
	  }
	}

}