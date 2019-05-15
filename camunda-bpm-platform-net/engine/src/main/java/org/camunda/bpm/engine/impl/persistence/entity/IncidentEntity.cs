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
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using IncidentLogger = org.camunda.bpm.engine.impl.incident.IncidentLogger;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class IncidentEntity : Incident, DbEntity, HasDbRevision, HasDbReferences
	{

	  protected internal static readonly IncidentLogger LOG = ProcessEngineLogger.INCIDENT_LOGGER;

	  protected internal int revision;

	  protected internal string id;
	  protected internal DateTime incidentTimestamp;
	  protected internal string incidentType;
	  protected internal string executionId;
	  protected internal string activityId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string causeIncidentId;
	  protected internal string rootCauseIncidentId;
	  protected internal string configuration;
	  protected internal string incidentMessage;
	  protected internal string tenantId;
	  protected internal string jobDefinitionId;

	  public virtual IList<IncidentEntity> createRecursiveIncidents()
	  {
		IList<IncidentEntity> createdIncidents = new List<IncidentEntity>();
		createRecursiveIncidents(id, createdIncidents);
		return createdIncidents;
	  }

	  /// <summary>
	  /// Instantiate recursive a new incident a super execution
	  /// (i.e. super process instance) which is affected from this
	  /// incident.
	  /// For example: a super process instance called via CallActivity
	  /// a new process instance on which an incident happened, so that
	  /// the super process instance has an incident too. 
	  /// </summary>
	  protected internal virtual void createRecursiveIncidents(string rootCauseIncidentId, IList<IncidentEntity> createdIncidents)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExecutionEntity execution = getExecution();
		ExecutionEntity execution = Execution;

		if (execution != null)
		{

		  ExecutionEntity superExecution = execution.getProcessInstance().getSuperExecution();

		  if (superExecution != null)
		  {

			// create a new incident
			IncidentEntity newIncident = create(incidentType);
			newIncident.Execution = superExecution;
			newIncident.ActivityId = superExecution.CurrentActivityId;
			newIncident.ProcessDefinitionId = superExecution.ProcessDefinitionId;
			newIncident.TenantId = superExecution.TenantId;

			// set cause and root cause
			newIncident.CauseIncidentId = id;
			newIncident.RootCauseIncidentId = rootCauseIncidentId;

			// insert new incident (and create a new historic incident)
			insert(newIncident);

			// add new incident to result set
			createdIncidents.Add(newIncident);

			newIncident.createRecursiveIncidents(rootCauseIncidentId, createdIncidents);
		  }
		}
	  }

	  public static IncidentEntity createAndInsertIncident(string incidentType, IncidentContext context, string message)
	  {
		// create new incident
		IncidentEntity newIncident = create(incidentType);
		newIncident.IncidentMessage = message;

		// set properties from incident context
		newIncident.Configuration = context.Configuration;
		newIncident.ActivityId = context.ActivityId;
		newIncident.ProcessDefinitionId = context.ProcessDefinitionId;
		newIncident.TenantId = context.TenantId;
		newIncident.JobDefinitionId = context.JobDefinitionId;

		if (!string.ReferenceEquals(context.ExecutionId, null))
		{
		  // fetch execution
		  ExecutionEntity execution = Context.CommandContext.ExecutionManager.findExecutionById(context.ExecutionId);

		  if (execution != null)
		  {
			// link incident with execution
			newIncident.Execution = execution;
		  }
		  else
		  {
			LOG.executionNotFound(context.ExecutionId);
		  }
		}

		// insert new incident (and create a new historic incident)
		insert(newIncident);

		return newIncident;
	  }

	  protected internal static IncidentEntity create(string incidentType)
	  {

		string incidentId = Context.ProcessEngineConfiguration.DbSqlSessionFactory.IdGenerator.NextId;

		// decorate new incident
		IncidentEntity newIncident = new IncidentEntity();
		newIncident.Id = incidentId;
		newIncident.IncidentTimestamp = ClockUtil.CurrentTime;
		newIncident.IncidentType = incidentType;
		newIncident.CauseIncidentId = incidentId;
		newIncident.RootCauseIncidentId = incidentId;

		return newIncident;
	  }

	  protected internal static void insert(IncidentEntity incident)
	  {
		// persist new incident
		Context.CommandContext.DbEntityManager.insert(incident);

		incident.fireHistoricIncidentEvent(HistoryEventTypes.INCIDENT_CREATE);
	  }

	  public virtual void delete()
	  {
		remove(false);
	  }

	  public virtual void resolve()
	  {
		remove(true);
	  }

	  protected internal virtual void remove(bool resolved)
	  {

		ExecutionEntity execution = Execution;

		if (execution != null)
		{
		  // Extract possible super execution of the assigned execution
		  ExecutionEntity superExecution = null;
		  if (execution.Id.Equals(execution.ProcessInstanceId))
		  {
			superExecution = execution.getSuperExecution();
		  }
		  else
		  {
			superExecution = execution.getProcessInstance().getSuperExecution();
		  }

		  if (superExecution != null)
		  {
			// get the incident, where this incident is the cause
			IncidentEntity parentIncident = superExecution.getIncidentByCauseIncidentId(Id);

			if (parentIncident != null)
			{
			  // remove the incident
			  parentIncident.remove(resolved);
			}
		  }

		  // remove link to execution
		  execution.removeIncident(this);
		}

		// always delete the incident
		Context.CommandContext.DbEntityManager.delete(this);

		// update historic incident
		HistoryEventType eventType = resolved ? HistoryEventTypes.INCIDENT_RESOLVE : HistoryEventTypes.INCIDENT_DELETE;
		fireHistoricIncidentEvent(eventType);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void fireHistoricIncidentEvent(final org.camunda.bpm.engine.impl.history.event.HistoryEventType eventType)
	  protected internal virtual void fireHistoricIncidentEvent(HistoryEventType eventType)
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
		  private readonly IncidentEntity outerInstance;

		  private HistoryEventType eventType;

		  public HistoryEventCreatorAnonymousInnerClass(IncidentEntity outerInstance, HistoryEventType eventType)
		  {
			  this.outerInstance = outerInstance;
			  this.eventType = eventType;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {

			HistoryEvent @event = null;
			if (HistoryEvent.INCIDENT_CREATE.Equals(eventType.EventName))
			{
			  @event = producer.createHistoricIncidentCreateEvt(outerInstance);

			}
			else if (HistoryEvent.INCIDENT_RESOLVE.Equals(eventType.EventName))
			{
			  @event = producer.createHistoricIncidentResolveEvt(outerInstance);

			}
			else if (HistoryEvent.INCIDENT_DELETE.Equals(eventType.EventName))
			{
			  @event = producer.createHistoricIncidentDeleteEvt(outerInstance);
			}
			return @event;
		  }
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referenceIds = new HashSet<string>();
    
			if (!string.ReferenceEquals(causeIncidentId, null))
			{
			  referenceIds.Add(causeIncidentId);
			}
    
			return referenceIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(causeIncidentId, null))
			{
			  referenceIdAndClass[causeIncidentId] = typeof(IncidentEntity);
			}
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  referenceIdAndClass[processDefinitionId] = typeof(ProcessDefinitionEntity);
			}
			if (!string.ReferenceEquals(processInstanceId, null))
			{
			  referenceIdAndClass[processInstanceId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(jobDefinitionId, null))
			{
			  referenceIdAndClass[jobDefinitionId] = typeof(JobDefinitionEntity);
			}
			if (!string.ReferenceEquals(executionId, null))
			{
			  referenceIdAndClass[executionId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(rootCauseIncidentId, null))
			{
			  referenceIdAndClass[rootCauseIncidentId] = typeof(IncidentEntity);
			}
    
			return referenceIdAndClass;
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


	  public virtual DateTime IncidentTimestamp
	  {
		  get
		  {
			return incidentTimestamp;
		  }
		  set
		  {
			this.incidentTimestamp = value;
		  }
	  }


	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
		  set
		  {
			this.incidentType = value;
		  }
	  }


	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage;
		  }
		  set
		  {
			this.incidentMessage = value;
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


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
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


	  public virtual ProcessDefinitionEntity ProcessDefinition
	  {
		  get
		  {
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
			}
			return null;
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


	  public virtual string CauseIncidentId
	  {
		  get
		  {
			return causeIncidentId;
		  }
		  set
		  {
			this.causeIncidentId = value;
		  }
	  }


	  public virtual string RootCauseIncidentId
	  {
		  get
		  {
			return rootCauseIncidentId;
		  }
		  set
		  {
			this.rootCauseIncidentId = value;
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



	  public virtual string JobDefinitionId
	  {
		  set
		  {
			this.jobDefinitionId = value;
		  }
		  get
		  {
			return jobDefinitionId;
		  }
	  }


	  public virtual ExecutionEntity Execution
	  {
		  set
		  {
			if (value != null)
			{
			  executionId = value.Id;
			  processInstanceId = value.ProcessInstanceId;
			  value.addIncident(this);
			}
			else
			{
			  ExecutionEntity oldExecution = Execution;
			  if (oldExecution != null)
			  {
				oldExecution.removeIncident(this);
			  }
			  executionId = null;
			  processInstanceId = null;
			}
		  }
		  get
		  {
			if (!string.ReferenceEquals(executionId, null))
			{
			  ExecutionEntity execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
    
			  if (execution == null)
			  {
				LOG.executionNotFound(executionId);
			  }
    
			  return execution;
			}
			else
			{
			  return null;
			}
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["executionId"] = executionId;
			persistentState["processDefinitionId"] = processDefinitionId;
			persistentState["activityId"] = activityId;
			persistentState["jobDefinitionId"] = jobDefinitionId;
			return persistentState;
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

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", incidentTimestamp=" + incidentTimestamp + ", incidentType=" + incidentType + ", executionId=" + executionId + ", activityId=" + activityId + ", processInstanceId=" + processInstanceId + ", processDefinitionId=" + processDefinitionId + ", causeIncidentId=" + causeIncidentId + ", rootCauseIncidentId=" + rootCauseIncidentId + ", configuration=" + configuration + ", tenantId=" + tenantId + ", incidentMessage=" + incidentMessage + ", jobDefinitionId=" + jobDefinitionId + "]";
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
		IncidentEntity other = (IncidentEntity) obj;
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

	}

}