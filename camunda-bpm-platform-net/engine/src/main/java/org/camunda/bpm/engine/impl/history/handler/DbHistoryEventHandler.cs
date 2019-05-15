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
namespace org.camunda.bpm.engine.impl.history.handler
{

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoricDecisionEvaluationEvent = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionEvaluationEvent;
	using HistoricScopeInstanceEvent = org.camunda.bpm.engine.impl.history.@event.HistoricScopeInstanceEvent;
	using HistoricVariableUpdateEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricVariableUpdateEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;

	/// <summary>
	/// <para>History event handler that writes history events to the process engine
	/// database using the DbEntityManager.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbHistoryEventHandler : HistoryEventHandler
	{

	  public virtual void handleEvent(HistoryEvent historyEvent)
	  {

		if (historyEvent is HistoricVariableUpdateEventEntity)
		{
		  insertHistoricVariableUpdateEntity((HistoricVariableUpdateEventEntity) historyEvent);
		}
		else if (historyEvent is HistoricDecisionEvaluationEvent)
		{
		  insertHistoricDecisionEvaluationEvent((HistoricDecisionEvaluationEvent) historyEvent);
		}
		else
		{
		  insertOrUpdate(historyEvent);
		}

	  }

	  public virtual void handleEvents(IList<HistoryEvent> historyEvents)
	  {
		foreach (HistoryEvent historyEvent in historyEvents)
		{
		  handleEvent(historyEvent);
		}
	  }

	  /// <summary>
	  /// general history event insert behavior </summary>
	  protected internal virtual void insertOrUpdate(HistoryEvent historyEvent)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager dbEntityManager = getDbEntityManager();
		DbEntityManager dbEntityManager = DbEntityManager;

		if (isInitialEvent(historyEvent))
		{
		  dbEntityManager.insert(historyEvent);
		}
		else
		{
		  if (dbEntityManager.getCachedEntity(historyEvent.GetType(), historyEvent.Id) == null)
		  {
			if (historyEvent is HistoricScopeInstanceEvent)
			{
			  // if this is a scope, get start time from existing event in DB
			  HistoricScopeInstanceEvent existingEvent = (HistoricScopeInstanceEvent) dbEntityManager.selectById(historyEvent.GetType(), historyEvent.Id);
			  if (existingEvent != null)
			  {
				HistoricScopeInstanceEvent historicScopeInstanceEvent = (HistoricScopeInstanceEvent) historyEvent;
				historicScopeInstanceEvent.StartTime = existingEvent.StartTime;
			  }
			}
			if (string.ReferenceEquals(historyEvent.Id, null))
			{
	//          dbSqlSession.insert(historyEvent);
			}
			else
			{
			  dbEntityManager.merge(historyEvent);
			}
		  }
		}
	  }


	  /// <summary>
	  /// customized insert behavior for HistoricVariableUpdateEventEntity </summary>
	  protected internal virtual void insertHistoricVariableUpdateEntity(HistoricVariableUpdateEventEntity historyEvent)
	  {
		DbEntityManager dbEntityManager = DbEntityManager;

		// insert update only if history level = FULL
		if (shouldWriteHistoricDetail(historyEvent))
		{

		  // insert byte array entity (if applicable)
		  sbyte[] byteValue = historyEvent.ByteValue;
		  if (byteValue != null)
		  {
			ByteArrayEntity byteArrayEntity = new ByteArrayEntity(historyEvent.VariableName, byteValue, ResourceTypes.HISTORY);
			byteArrayEntity.RootProcessInstanceId = historyEvent.RootProcessInstanceId;
			byteArrayEntity.RemovalTime = historyEvent.RemovalTime;

			Context.CommandContext.ByteArrayManager.insertByteArray(byteArrayEntity);
			historyEvent.ByteArrayId = byteArrayEntity.Id;

		  }
		  dbEntityManager.insert(historyEvent);
		}

		// always insert/update HistoricProcessVariableInstance
		if (historyEvent.isEventOfType(HistoryEventTypes.VARIABLE_INSTANCE_CREATE))
		{
		  HistoricVariableInstanceEntity persistentObject = new HistoricVariableInstanceEntity(historyEvent);
		  dbEntityManager.insert(persistentObject);

		}
		else if (historyEvent.isEventOfType(HistoryEventTypes.VARIABLE_INSTANCE_UPDATE) || historyEvent.isEventOfType(HistoryEventTypes.VARIABLE_INSTANCE_MIGRATE))
		{
		  HistoricVariableInstanceEntity historicVariableInstanceEntity = dbEntityManager.selectById(typeof(HistoricVariableInstanceEntity), historyEvent.VariableInstanceId);
		  if (historicVariableInstanceEntity != null)
		  {
			historicVariableInstanceEntity.updateFromEvent(historyEvent);
			historicVariableInstanceEntity.State = org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_CREATED;

		  }
		  else
		  {
			// #CAM-1344 / #SUPPORT-688
			// this is a FIX for process instances which were started in camunda fox 6.1 and migrated to camunda BPM 7.0.
			// in fox 6.1 the HistoricVariable instances were flushed to the DB when the process instance completed.
			// Since fox 6.2 we populate the HistoricVariable table as we go.
			HistoricVariableInstanceEntity persistentObject = new HistoricVariableInstanceEntity(historyEvent);
			dbEntityManager.insert(persistentObject);
		  }

		}
		else if (historyEvent.isEventOfType(HistoryEventTypes.VARIABLE_INSTANCE_DELETE))
		{
		  HistoricVariableInstanceEntity historicVariableInstanceEntity = dbEntityManager.selectById(typeof(HistoricVariableInstanceEntity), historyEvent.VariableInstanceId);
		  if (historicVariableInstanceEntity != null)
		  {
			historicVariableInstanceEntity.State = org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_DELETED;
		  }
		}
	  }

	  protected internal virtual bool shouldWriteHistoricDetail(HistoricVariableUpdateEventEntity historyEvent)
	  {

		return Context.ProcessEngineConfiguration.HistoryLevel.isHistoryEventProduced(HistoryEventTypes.VARIABLE_INSTANCE_UPDATE_DETAIL, historyEvent) && !historyEvent.isEventOfType(HistoryEventTypes.VARIABLE_INSTANCE_MIGRATE);
	  }


	  protected internal virtual void insertHistoricDecisionEvaluationEvent(HistoricDecisionEvaluationEvent @event)
	  {

		Context.CommandContext.HistoricDecisionInstanceManager.insertHistoricDecisionInstances(@event);
	  }


	  protected internal virtual bool isInitialEvent(HistoryEvent historyEvent)
	  {
		return string.ReferenceEquals(historyEvent.EventType, null) || historyEvent.isEventOfType(HistoryEventTypes.ACTIVITY_INSTANCE_START) || historyEvent.isEventOfType(HistoryEventTypes.PROCESS_INSTANCE_START) || historyEvent.isEventOfType(HistoryEventTypes.TASK_INSTANCE_CREATE) || historyEvent.isEventOfType(HistoryEventTypes.FORM_PROPERTY_UPDATE) || historyEvent.isEventOfType(HistoryEventTypes.INCIDENT_CREATE) || historyEvent.isEventOfType(HistoryEventTypes.CASE_INSTANCE_CREATE) || historyEvent.isEventOfType(HistoryEventTypes.DMN_DECISION_EVALUATE) || historyEvent.isEventOfType(HistoryEventTypes.BATCH_START) || historyEvent.isEventOfType(HistoryEventTypes.IDENTITY_LINK_ADD) || historyEvent.isEventOfType(HistoryEventTypes.IDENTITY_LINK_DELETE);
	  }

	  protected internal virtual DbEntityManager DbEntityManager
	  {
		  get
		  {
			return Context.CommandContext.DbEntityManager;
		  }
	  }

	}

}