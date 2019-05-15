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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HistoricEntity = org.camunda.bpm.engine.impl.db.HistoricEntity;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;

	/// <summary>
	/// <para>The base class for all history events.</para>
	/// 
	/// <para>A history event contains data about an event that has happened
	/// in a process instance. Such an event may be the start of an activity,
	/// the end of an activity, a task instance that is created or other similar
	/// events...</para>
	/// 
	/// <para>History events contain data in a serializable form. Some
	/// implementations may persist events directly or may serialize
	/// them as an intermediate representation for later processing
	/// (ie. in an asynchronous implementation).</para>
	/// 
	/// <para>This class implements <seealso cref="DbEntity"/>. This was chosen so
	/// that <seealso cref="HistoryEvent"/>s can be easily persisted using the
	/// <seealso cref="DbEntityManager"/>. This may not be used by all <seealso cref="HistoryEventHandler"/>
	/// implementations but it does also not cause harm.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class HistoryEvent : DbEntity, HistoricEntity
	{

	  private const long serialVersionUID = 1L;

	  // constants deprecated since 7.2

	  [Obsolete]
	  public static readonly string ACTIVITY_EVENT_TYPE_START = HistoryEventTypes.ACTIVITY_INSTANCE_START.EventName;
	  [Obsolete]
	  public static readonly string ACTIVITY_EVENT_TYPE_UPDATE = HistoryEventTypes.ACTIVITY_INSTANCE_END.EventName;
	  [Obsolete]
	  public static readonly string ACTIVITY_EVENT_TYPE_END = HistoryEventTypes.ACTIVITY_INSTANCE_END.EventName;

	  [Obsolete]
	  public static readonly string TASK_EVENT_TYPE_CREATE = HistoryEventTypes.TASK_INSTANCE_CREATE.EventName;
	  [Obsolete]
	  public static readonly string TASK_EVENT_TYPE_UPDATE = HistoryEventTypes.TASK_INSTANCE_UPDATE.EventName;
	  [Obsolete]
	  public static readonly string TASK_EVENT_TYPE_COMPLETE = HistoryEventTypes.TASK_INSTANCE_COMPLETE.EventName;
	  [Obsolete]
	  public static readonly string TASK_EVENT_TYPE_DELETE = HistoryEventTypes.TASK_INSTANCE_DELETE.EventName;

	  [Obsolete]
	  public static readonly string VARIABLE_EVENT_TYPE_CREATE = HistoryEventTypes.VARIABLE_INSTANCE_CREATE.EventName;
	  [Obsolete]
	  public static readonly string VARIABLE_EVENT_TYPE_UPDATE = HistoryEventTypes.VARIABLE_INSTANCE_UPDATE.EventName;
	  [Obsolete]
	  public static readonly string VARIABLE_EVENT_TYPE_DELETE = HistoryEventTypes.VARIABLE_INSTANCE_DELETE.EventName;

	  [Obsolete]
	  public static readonly string FORM_PROPERTY_UPDATE = HistoryEventTypes.FORM_PROPERTY_UPDATE.EventName;

	  [Obsolete]
	  public static readonly string INCIDENT_CREATE = HistoryEventTypes.INCIDENT_CREATE.EventName;
	  [Obsolete]
	  public static readonly string INCIDENT_DELETE = HistoryEventTypes.INCIDENT_DELETE.EventName;
	  [Obsolete]
	  public static readonly string INCIDENT_RESOLVE = HistoryEventTypes.INCIDENT_RESOLVE.EventName;

	  public static readonly string IDENTITY_LINK_ADD = HistoryEventTypes.IDENTITY_LINK_ADD.EventName;

	  public static readonly string IDENTITY_LINK_DELETE = HistoryEventTypes.IDENTITY_LINK_DELETE.EventName;

	  /// <summary>
	  /// each <seealso cref="HistoryEvent"/> has a unique id </summary>
	  protected internal string id;

	  /// <summary>
	  /// the root process instance in which the event has happened </summary>
	  protected internal string rootProcessInstanceId;

	  /// <summary>
	  /// the process instance in which the event has happened </summary>
	  protected internal string processInstanceId;

	  /// <summary>
	  /// the id of the execution in which the event has happened </summary>
	  protected internal string executionId;

	  /// <summary>
	  /// the id of the process definition </summary>
	  protected internal string processDefinitionId;

	  /// <summary>
	  /// the key of the process definition </summary>
	  protected internal string processDefinitionKey;

	  /// <summary>
	  /// the name of the process definition </summary>
	  protected internal string processDefinitionName;

	  /// <summary>
	  /// the version of the process definition </summary>
	  protected internal int? processDefinitionVersion;

	  /// <summary>
	  /// the case instance in which the event has happened </summary>
	  protected internal string caseInstanceId;

	  /// <summary>
	  /// the id of the case execution in which the event has happened </summary>
	  protected internal string caseExecutionId;

	  /// <summary>
	  /// the id of the case definition </summary>
	  protected internal string caseDefinitionId;

	  /// <summary>
	  /// the key of the case definition </summary>
	  protected internal string caseDefinitionKey;

	  /// <summary>
	  /// the name of the case definition </summary>
	  protected internal string caseDefinitionName;

	  /// <summary>
	  /// The type of the activity audit event. </summary>
	  /// <seealso cref= HistoryEventType#getEventName()
	  ///  </seealso>
	  protected internal string eventType;

	  protected internal long sequenceCounter;

	  /* the time when the history event will be deleted */
	  protected internal DateTime removalTime;

	  // getters / setters ///////////////////////////////////

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


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
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


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName;
		  }
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }


	  public virtual int? ProcessDefinitionVersion
	  {
		  get
		  {
			return processDefinitionVersion;
		  }
		  set
		  {
			this.processDefinitionVersion = value;
		  }
	  }


	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName;
		  }
		  set
		  {
			this.caseDefinitionName = value;
		  }
	  }


	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }


	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }


	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
		  get
		  {
			return id;
		  }
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


	  public virtual long SequenceCounter
	  {
		  get
		  {
			return sequenceCounter;
		  }
		  set
		  {
			this.sequenceCounter = value;
		  }
	  }


	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
		  set
		  {
			this.removalTime = value;
		  }
	  }


	  // persistent object implementation ///////////////

	  public virtual object PersistentState
	  {
		  get
		  {
			// events are immutable
			return typeof(HistoryEvent);
		  }
	  }

	  // state inspection

	  public virtual bool isEventOfType(HistoryEventType type)
	  {
		return type.EventName.Equals(eventType);
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", processInstanceId=" + processInstanceId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + "]";
	  }

	}

}