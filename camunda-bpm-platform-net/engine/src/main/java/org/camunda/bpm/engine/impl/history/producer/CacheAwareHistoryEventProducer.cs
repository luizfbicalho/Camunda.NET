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
namespace org.camunda.bpm.engine.impl.history.producer
{
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using org.camunda.bpm.engine.impl.history.@event;
	using DbHistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.DbHistoryEventHandler;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// <summary>
	/// <para>This HistoryEventProducer is aware of the <seealso cref="DbEntityManager"/> cache
	/// and works in combination with the <seealso cref="DbHistoryEventHandler"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CacheAwareHistoryEventProducer : DefaultHistoryEventProducer
	{

	   protected internal override HistoricActivityInstanceEventEntity loadActivityInstanceEventEntity(ExecutionEntity execution)
	   {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String activityInstanceId = execution.getActivityInstanceId();
		string activityInstanceId = execution.ActivityInstanceId;

		HistoricActivityInstanceEventEntity cachedEntity = findInCache(typeof(HistoricActivityInstanceEventEntity), activityInstanceId);

		if (cachedEntity != null)
		{
		  return cachedEntity;

		}
		else
		{
		  return newActivityInstanceEventEntity(execution);

		}

	   }

	  protected internal override HistoricProcessInstanceEventEntity loadProcessInstanceEventEntity(ExecutionEntity execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = execution.getProcessInstanceId();
		string processInstanceId = execution.ProcessInstanceId;

		HistoricProcessInstanceEventEntity cachedEntity = findInCache(typeof(HistoricProcessInstanceEventEntity), processInstanceId);

		if (cachedEntity != null)
		{
		  return cachedEntity;

		}
		else
		{
		  return newProcessInstanceEventEntity(execution);

		}

	  }

	  protected internal override HistoricTaskInstanceEventEntity loadTaskInstanceEvent(DelegateTask task)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String taskId = task.getId();
		string taskId = task.Id;

		HistoricTaskInstanceEventEntity cachedEntity = findInCache(typeof(HistoricTaskInstanceEventEntity), taskId);

		if (cachedEntity != null)
		{
		  return cachedEntity;

		}
		else
		{
		  return newTaskInstanceEventEntity(task);

		}
	  }

	  protected internal override HistoricIncidentEventEntity loadIncidentEvent(Incident incident)
	  {
		string incidentId = incident.Id;

		HistoricIncidentEventEntity cachedEntity = findInCache(typeof(HistoricIncidentEventEntity), incidentId);

		if (cachedEntity != null)
		{
		  return cachedEntity;

		}
		else
		{
		  return newIncidentEventEntity(incident);

		}
	  }

	  protected internal override HistoricBatchEntity loadBatchEntity(BatchEntity batch)
	  {
		string batchId = batch.Id;

		HistoricBatchEntity cachedEntity = findInCache(typeof(HistoricBatchEntity), batchId);

		if (cachedEntity != null)
		{
		  return cachedEntity;

		}
		else
		{
		  return newBatchEventEntity(batch);

		}
	  }

	  /// <summary>
	  /// find a cached entity by primary key </summary>
	  protected internal virtual T findInCache<T>(Type type, string id) where T : HistoryEvent
	  {
			  type = typeof(T);
		return Context.CommandContext.DbEntityManager.getCachedEntity(type, id);
	  }

	}

}