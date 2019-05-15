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

	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricProcessInstanceManager : AbstractHistoricManager
	{

	  public virtual HistoricProcessInstanceEntity findHistoricProcessInstance(string processInstanceId)
	  {
		if (HistoryEnabled)
		{
		  return DbEntityManager.selectById(typeof(HistoricProcessInstanceEntity), processInstanceId);
		}
		return null;
	  }

	  public virtual HistoricProcessInstanceEventEntity findHistoricProcessInstanceEvent(string eventId)
	  {
		if (HistoryEnabled)
		{
		  return DbEntityManager.selectById(typeof(HistoricProcessInstanceEventEntity), eventId);
		}
		return null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricProcessInstanceByProcessDefinitionId(String processDefinitionId)
	  public virtual void deleteHistoricProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		if (HistoryEnabled)
		{
		  IList<string> historicProcessInstanceIds = DbEntityManager.selectList("selectHistoricProcessInstanceIdsByProcessDefinitionId", processDefinitionId);

		  if (historicProcessInstanceIds.Count > 0)
		  {
			deleteHistoricProcessInstanceByIds(historicProcessInstanceIds);
		  }
		}
	  }

	  public virtual void deleteHistoricProcessInstanceByIds(IList<string> processInstanceIds)
	  {
		CommandContext commandContext = Context.CommandContext;

		commandContext.HistoricDetailManager.deleteHistoricDetailsByProcessInstanceIds(processInstanceIds);
		commandContext.HistoricVariableInstanceManager.deleteHistoricVariableInstanceByProcessInstanceIds(processInstanceIds);
		commandContext.CommentManager.deleteCommentsByProcessInstanceIds(processInstanceIds);
		commandContext.AttachmentManager.deleteAttachmentsByProcessInstanceIds(processInstanceIds);
		commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstancesByProcessInstanceIds(processInstanceIds, false);
		commandContext.HistoricActivityInstanceManager.deleteHistoricActivityInstancesByProcessInstanceIds(processInstanceIds);
		commandContext.HistoricIncidentManager.deleteHistoricIncidentsByProcessInstanceIds(processInstanceIds);
		commandContext.HistoricJobLogManager.deleteHistoricJobLogsByProcessInstanceIds(processInstanceIds);
		commandContext.HistoricExternalTaskLogManager.deleteHistoricExternalTaskLogsByProcessInstanceIds(processInstanceIds);

		commandContext.DbEntityManager.deletePreserveOrder(typeof(HistoricProcessInstanceEntity), "deleteHistoricProcessInstances", processInstanceIds);
	  }

	  public virtual long findHistoricProcessInstanceCountByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  {
		if (HistoryEnabled)
		{
		  configureQuery(historicProcessInstanceQuery);
		  return (long?) DbEntityManager.selectOne("selectHistoricProcessInstanceCountByQueryCriteria", historicProcessInstanceQuery).Value;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl historicProcessInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricProcessInstance> findHistoricProcessInstancesByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery, Page page)
	  {
		if (HistoryEnabled)
		{
		  configureQuery(historicProcessInstanceQuery);
		  return DbEntityManager.selectList("selectHistoricProcessInstancesByQueryCriteria", historicProcessInstanceQuery, page);
		}
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricProcessInstance> findHistoricProcessInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricProcessInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricProcessInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricProcessInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureQuery(HistoricProcessInstanceQueryImpl query)
	  {
		AuthorizationManager.configureHistoricProcessInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findHistoricProcessInstanceIdsForCleanup(System.Nullable<int> batchSize, int minuteFrom, int minuteTo)
	  public virtual IList<string> findHistoricProcessInstanceIdsForCleanup(int? batchSize, int minuteFrom, int minuteTo)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["currentTimestamp"] = ClockUtil.CurrentTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		ListQueryParameterObject parameterObject = new ListQueryParameterObject(parameters, 0, batchSize.Value);
		return (IList<string>) DbEntityManager.selectList("selectHistoricProcessInstanceIdsForCleanup", parameterObject);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findHistoricProcessInstanceIds(org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  public virtual IList<string> findHistoricProcessInstanceIds(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  {
		configureQuery(historicProcessInstanceQuery);
		return (IList<string>) DbEntityManager.selectList("selectHistoricProcessInstanceIdsByQueryCriteria", historicProcessInstanceQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult> findCleanableHistoricProcessInstancesReportByCriteria(org.camunda.bpm.engine.impl.CleanableHistoricProcessInstanceReportImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CleanableHistoricProcessInstanceReportResult> findCleanableHistoricProcessInstancesReportByCriteria(CleanableHistoricProcessInstanceReportImpl query, Page page)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;

		AuthorizationManager.configureQueryHistoricFinishedInstanceReport(query, Resources.PROCESS_DEFINITION);
		TenantManager.configureQuery(query);
		return DbEntityManager.selectList("selectFinishedProcessInstancesReportEntities", query, page);
	  }

	  public virtual long findCleanableHistoricProcessInstancesReportCountByCriteria(CleanableHistoricProcessInstanceReportImpl query)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;

		AuthorizationManager.configureQueryHistoricFinishedInstanceReport(query, Resources.PROCESS_DEFINITION);
		TenantManager.configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectFinishedProcessInstancesReportEntitiesCount", query).Value;
	  }

	  public virtual void addRemovalTimeToProcessInstancesByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		CommandContext commandContext = Context.CommandContext;

		commandContext.HistoricActivityInstanceManager.addRemovalTimeToActivityInstancesByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricTaskInstanceManager.addRemovalTimeToTaskInstancesByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricVariableInstanceManager.addRemovalTimeToVariableInstancesByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricDetailManager.addRemovalTimeToDetailsByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricIncidentManager.addRemovalTimeToIncidentsByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricExternalTaskLogManager.addRemovalTimeToExternalTaskLogByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricJobLogManager.addRemovalTimeToJobLogByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.OperationLogManager.addRemovalTimeToUserOperationLogByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.HistoricIdentityLinkManager.addRemovalTimeToIdentityLinkLogByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.CommentManager.addRemovalTimeToCommentsByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.AttachmentManager.addRemovalTimeToAttachmentsByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		commandContext.ByteArrayManager.addRemovalTimeToByteArraysByRootProcessInstanceId(rootProcessInstanceId, removalTime);

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricProcessInstanceEventEntity), "updateHistoricProcessInstanceEventsByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeById(string processInstanceId, DateTime removalTime)
	  {
		CommandContext commandContext = Context.CommandContext;

		commandContext.HistoricActivityInstanceManager.addRemovalTimeToActivityInstancesByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricTaskInstanceManager.addRemovalTimeToTaskInstancesByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricVariableInstanceManager.addRemovalTimeToVariableInstancesByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricDetailManager.addRemovalTimeToDetailsByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricIncidentManager.addRemovalTimeToIncidentsByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricExternalTaskLogManager.addRemovalTimeToExternalTaskLogByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricJobLogManager.addRemovalTimeToJobLogByProcessInstanceId(processInstanceId, removalTime);

		commandContext.OperationLogManager.addRemovalTimeToUserOperationLogByProcessInstanceId(processInstanceId, removalTime);

		commandContext.HistoricIdentityLinkManager.addRemovalTimeToIdentityLinkLogByProcessInstanceId(processInstanceId, removalTime);

		commandContext.CommentManager.addRemovalTimeToCommentsByProcessInstanceId(processInstanceId, removalTime);

		commandContext.AttachmentManager.addRemovalTimeToAttachmentsByProcessInstanceId(processInstanceId, removalTime);

		commandContext.ByteArrayManager.addRemovalTimeToByteArraysByProcessInstanceId(processInstanceId, removalTime);

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricProcessInstanceEventEntity), "updateHistoricProcessInstanceByProcessInstanceId", parameters);
	  }

	  public virtual IDictionary<Type, DbOperation> deleteHistoricProcessInstancesByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		CommandContext commandContext = Context.CommandContext;

		IDictionary<Type, DbOperation> deleteOperations = new Dictionary<Type, DbOperation>();

		DbOperation deleteActivityInstances = commandContext.HistoricActivityInstanceManager.deleteHistoricActivityInstancesByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteActivityInstances.EntityType] = deleteActivityInstances;

		DbOperation deleteTaskInstances = commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstancesByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteTaskInstances.EntityType] = deleteTaskInstances;

		DbOperation deleteVariableInstances = commandContext.HistoricVariableInstanceManager.deleteHistoricVariableInstancesByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteVariableInstances.EntityType] = deleteVariableInstances;

		DbOperation deleteDetails = commandContext.HistoricDetailManager.deleteHistoricDetailsByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteDetails.EntityType] = deleteDetails;

		DbOperation deleteIncidents = commandContext.HistoricIncidentManager.deleteHistoricIncidentsByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteIncidents.EntityType] = deleteIncidents;

		DbOperation deleteTaskLog = commandContext.HistoricExternalTaskLogManager.deleteExternalTaskLogByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteTaskLog.EntityType] = deleteTaskLog;

		DbOperation deleteJobLog = commandContext.HistoricJobLogManager.deleteJobLogByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteJobLog.EntityType] = deleteJobLog;

		DbOperation deleteOperationLog = commandContext.OperationLogManager.deleteOperationLogByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteOperationLog.EntityType] = deleteOperationLog;

		DbOperation deleteIdentityLinkLog = commandContext.HistoricIdentityLinkManager.deleteHistoricIdentityLinkLogByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteIdentityLinkLog.EntityType] = deleteIdentityLinkLog;

		DbOperation deleteComments = commandContext.CommentManager.deleteCommentsByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteComments.EntityType] = deleteComments;

		DbOperation deleteAttachments = commandContext.AttachmentManager.deleteAttachmentsByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteAttachments.EntityType] = deleteAttachments;

		DbOperation deleteByteArrays = commandContext.ByteArrayManager.deleteByteArraysByRemovalTime(removalTime, minuteFrom, minuteTo, batchSize);

		deleteOperations[deleteByteArrays.EntityType] = deleteByteArrays;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		DbOperation deleteProcessInstances = DbEntityManager.deletePreserveOrder(typeof(HistoricProcessInstanceEntity), "deleteHistoricProcessInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));

		deleteOperations[deleteProcessInstances.EntityType] = deleteProcessInstances;

		return deleteOperations;
	  }

	}

}