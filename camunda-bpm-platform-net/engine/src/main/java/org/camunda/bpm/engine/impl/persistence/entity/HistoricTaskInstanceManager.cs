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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricTaskInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricTaskInstanceEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author  Tom Baeyens
	/// </summary>
	public class HistoricTaskInstanceManager : AbstractHistoricManager
	{

	  /// <summary>
	  /// Deletes all data related with tasks, which belongs to specified process instance ids. </summary>
	  /// <param name="processInstanceIds"> </param>
	  /// <param name="deleteVariableInstances"> when true, will also delete variable instances. Can be false when variable instances were deleted separately. </param>
	  public virtual void deleteHistoricTaskInstancesByProcessInstanceIds(IList<string> processInstanceIds, bool deleteVariableInstances)
	  {

		CommandContext commandContext = Context.CommandContext;

		if (deleteVariableInstances)
		{
		  HistoricVariableInstanceManager.deleteHistoricVariableInstancesByTaskProcessInstanceIds(processInstanceIds);
		}

		HistoricDetailManager.deleteHistoricDetailsByTaskProcessInstanceIds(processInstanceIds);

		commandContext.CommentManager.deleteCommentsByTaskProcessInstanceIds(processInstanceIds);

		AttachmentManager.deleteAttachmentsByTaskProcessInstanceIds(processInstanceIds);

		HistoricIdentityLinkManager.deleteHistoricIdentityLinksLogByTaskProcessInstanceIds(processInstanceIds);

		DbEntityManager.deletePreserveOrder(typeof(HistoricTaskInstanceEntity), "deleteHistoricTaskInstanceByProcessInstanceIds", processInstanceIds);
	  }

	  public virtual void deleteHistoricTaskInstancesByCaseInstanceIds(IList<string> caseInstanceIds)
	  {

		CommandContext commandContext = Context.CommandContext;

		HistoricDetailManager.deleteHistoricDetailsByTaskCaseInstanceIds(caseInstanceIds);

		commandContext.CommentManager.deleteCommentsByTaskCaseInstanceIds(caseInstanceIds);

		AttachmentManager.deleteAttachmentsByTaskCaseInstanceIds(caseInstanceIds);

		HistoricIdentityLinkManager.deleteHistoricIdentityLinksLogByTaskCaseInstanceIds(caseInstanceIds);

		DbEntityManager.deletePreserveOrder(typeof(HistoricTaskInstanceEntity), "deleteHistoricTaskInstanceByCaseInstanceIds", caseInstanceIds);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public long findHistoricTaskInstanceCountByQueryCriteria(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  public virtual long findHistoricTaskInstanceCountByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  {
		if (HistoryEnabled)
		{
		  configureQuery(historicTaskInstanceQuery);
		  return (long?) DbEntityManager.selectOne("selectHistoricTaskInstanceCountByQueryCriteria",historicTaskInstanceQuery).Value;
		}

		return 0;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByQueryCriteria(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery, final org.camunda.bpm.engine.impl.Page page)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual IList<HistoricTaskInstance> findHistoricTaskInstancesByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery, Page page)
	  {
		if (HistoryEnabled)
		{
		  configureQuery(historicTaskInstanceQuery);
		  return DbEntityManager.selectList("selectHistoricTaskInstancesByQueryCriteria", historicTaskInstanceQuery, page);
		}

		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public HistoricTaskInstanceEntity findHistoricTaskInstanceById(final String taskId)
	  public virtual HistoricTaskInstanceEntity findHistoricTaskInstanceById(string taskId)
	  {
		ensureNotNull("Invalid historic task id", "taskId", taskId);

		if (HistoryEnabled)
		{
		  return (HistoricTaskInstanceEntity) DbEntityManager.selectOne("selectHistoricTaskInstance", taskId);
		}

		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deleteHistoricTaskInstanceById(final String taskId)
	  public virtual void deleteHistoricTaskInstanceById(string taskId)
	  {
		if (HistoryEnabled)
		{
		  HistoricTaskInstanceEntity historicTaskInstance = findHistoricTaskInstanceById(taskId);
		  if (historicTaskInstance != null)
		  {
			CommandContext commandContext = Context.CommandContext;

			commandContext.HistoricDetailManager.deleteHistoricDetailsByTaskId(taskId);

			commandContext.HistoricVariableInstanceManager.deleteHistoricVariableInstancesByTaskId(taskId);

			commandContext.CommentManager.deleteCommentsByTaskId(taskId);

			commandContext.AttachmentManager.deleteAttachmentsByTaskId(taskId);

			commandContext.HistoricIdentityLinkManager.deleteHistoricIdentityLinksLogByTaskId(taskId);

			DbEntityManager.delete(historicTaskInstance);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByNativeQuery(final java.util.Map<String, Object> parameterMap, final int firstResult, final int maxResults)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual IList<HistoricTaskInstance> findHistoricTaskInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricTaskInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public long findHistoricTaskInstanceCountByNativeQuery(final java.util.Map<String, Object> parameterMap)
	  public virtual long findHistoricTaskInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricTaskInstanceCountByNativeQuery", parameterMap).Value;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void updateHistoricTaskInstance(final TaskEntity taskEntity)
	  public virtual void updateHistoricTaskInstance(TaskEntity taskEntity)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.TASK_INSTANCE_UPDATE, taskEntity))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, taskEntity));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricTaskInstanceManager outerInstance;

		  private org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity;

		  public HistoryEventCreatorAnonymousInnerClass(HistoricTaskInstanceManager outerInstance, org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.taskEntity = taskEntity;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createTaskInstanceUpdateEvt(taskEntity);
		  }
	  }

	  public virtual void addRemovalTimeToTaskInstancesByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricTaskInstanceEventEntity), "updateHistoricTaskInstancesByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToTaskInstancesByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricTaskInstanceEventEntity), "updateHistoricTaskInstancesByProcessInstanceId", parameters);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void markTaskInstanceEnded(String taskId, final String deleteReason)
	  public virtual void markTaskInstanceEnded(string taskId, string deleteReason)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TaskEntity taskEntity = org.camunda.bpm.engine.impl.context.Context.getCommandContext().getDbEntityManager().selectById(TaskEntity.class, taskId);
		TaskEntity taskEntity = Context.CommandContext.DbEntityManager.selectById(typeof(TaskEntity), taskId);

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.TASK_INSTANCE_COMPLETE, taskEntity))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this, deleteReason, taskEntity));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricTaskInstanceManager outerInstance;

		  private string deleteReason;
		  private org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity;

		  public HistoryEventCreatorAnonymousInnerClass2(HistoricTaskInstanceManager outerInstance, string deleteReason, org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.deleteReason = deleteReason;
			  this.taskEntity = taskEntity;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createTaskInstanceCompleteEvt(taskEntity, deleteReason);
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void createHistoricTask(final TaskEntity task)
	  public virtual void createHistoricTask(TaskEntity task)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.TASK_INSTANCE_CREATE, task))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass3(this, task));

		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass3 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricTaskInstanceManager outerInstance;

		  private org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task;

		  public HistoryEventCreatorAnonymousInnerClass3(HistoricTaskInstanceManager outerInstance, org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task)
		  {
			  this.outerInstance = outerInstance;
			  this.task = task;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createTaskInstanceCreateEvt(task);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void configureQuery(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl query)
	  protected internal virtual void configureQuery(HistoricTaskInstanceQueryImpl query)
	  {
		AuthorizationManager.configureHistoricTaskInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

	  public virtual DbOperation deleteHistoricTaskInstancesByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricTaskInstanceEntity), "deleteHistoricTaskInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}

}