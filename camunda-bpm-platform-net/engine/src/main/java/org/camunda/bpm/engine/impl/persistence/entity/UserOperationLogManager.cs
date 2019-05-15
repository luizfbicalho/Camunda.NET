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

	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using UserOperationLogEntryEventEntity = org.camunda.bpm.engine.impl.history.@event.UserOperationLogEntryEventEntity;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using IdentityOperationResult = org.camunda.bpm.engine.impl.identity.IdentityOperationResult;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using UserOperationLogContext = org.camunda.bpm.engine.impl.oplog.UserOperationLogContext;
	using UserOperationLogContextEntryBuilder = org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntryBuilder;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using PermissionConverter = org.camunda.bpm.engine.impl.util.PermissionConverter;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;

	/// <summary>
	/// Manager for <seealso cref="UserOperationLogEntryEventEntity"/> that also provides a generic and some specific log methods.
	/// 
	/// @author Danny Gräf
	/// @author Tobias Metzke
	/// </summary>
	public class UserOperationLogManager : AbstractHistoricManager
	{

	  public virtual UserOperationLogEntry findOperationLogById(string entryId)
	  {
		return DbEntityManager.selectById(typeof(UserOperationLogEntryEventEntity), entryId);
	  }

	  public virtual long findOperationLogEntryCountByQueryCriteria(UserOperationLogQueryImpl query)
	  {
		AuthorizationManager.configureUserOperationLogQuery(query);
		return (long?) DbEntityManager.selectOne("selectUserOperationLogEntryCountByQueryCriteria", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.UserOperationLogEntry> findOperationLogEntriesByQueryCriteria(org.camunda.bpm.engine.impl.UserOperationLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<UserOperationLogEntry> findOperationLogEntriesByQueryCriteria(UserOperationLogQueryImpl query, Page page)
	  {
		AuthorizationManager.configureUserOperationLogQuery(query);
		return DbEntityManager.selectList("selectUserOperationLogEntriesByQueryCriteria", query, page);
	  }

	  public virtual void addRemovalTimeToUserOperationLogByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(UserOperationLogEntryEventEntity), "updateUserOperationLogByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToUserOperationLogByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(UserOperationLogEntryEventEntity), "updateUserOperationLogByProcessInstanceId", parameters);
	  }

	  public virtual void deleteOperationLogEntryById(string entryId)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(UserOperationLogEntryEventEntity), "deleteUserOperationLogEntryById", entryId);
		}
	  }

	  public virtual DbOperation deleteOperationLogByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(UserOperationLogEntryEventEntity), "deleteUserOperationLogByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  public virtual void logUserOperations(UserOperationLogContext context)
	  {
		if (UserOperationLogEnabled)
		{
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logUserOperation(IdentityOperationResult operationResult, string userId)
	  {
		logUserOperation(getOperationType(operationResult), userId);
	  }

	  public virtual void logUserOperation(string operation, string userId)
	  {
		if (!string.ReferenceEquals(operation, null) && UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.USER).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN).propertyChanges(new PropertyChange("userId", null, userId));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logGroupOperation(IdentityOperationResult operationResult, string groupId)
	  {
		logGroupOperation(getOperationType(operationResult), groupId);
	  }

	  public virtual void logGroupOperation(string operation, string groupId)
	  {
		if (!string.ReferenceEquals(operation, null) && UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.GROUP).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN).propertyChanges(new PropertyChange("groupId", null, groupId));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logTenantOperation(IdentityOperationResult operationResult, string tenantId)
	  {
		logTenantOperation(getOperationType(operationResult), tenantId);
	  }

	  public virtual void logTenantOperation(string operation, string tenantId)
	  {
		if (!string.ReferenceEquals(operation, null) && UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.TENANT).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN).propertyChanges(new PropertyChange("tenantId", null, tenantId));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logMembershipOperation(IdentityOperationResult operationResult, string userId, string groupId, string tenantId)
	  {
		logMembershipOperation(getOperationType(operationResult), userId, groupId, tenantId);
	  }

	  public virtual void logMembershipOperation(string operation, string userId, string groupId, string tenantId)
	  {
		if (!string.ReferenceEquals(operation, null) && UserOperationLogEnabled)
		{
		  string entityType = string.ReferenceEquals(tenantId, null) ? EntityTypes.GROUP_MEMBERSHIP : EntityTypes.TENANT_MEMBERSHIP;

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, entityType).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN);
		  IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		  if (!string.ReferenceEquals(userId, null))
		  {
			propertyChanges.Add(new PropertyChange("userId", null, userId));
		  }
		  if (!string.ReferenceEquals(groupId, null))
		  {
			propertyChanges.Add(new PropertyChange("groupId", null, groupId));
		  }
		  if (!string.ReferenceEquals(tenantId, null))
		  {
			propertyChanges.Add(new PropertyChange("tenantId", null, tenantId));
		  }
		  entryBuilder.propertyChanges(propertyChanges);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logTaskOperations(string operation, TaskEntity task, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.TASK).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).inContextOf(task, propertyChanges);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logTaskOperations(string operation, HistoricTaskInstance historicTask, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.TASK).inContextOf(historicTask, propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logLinkOperation(string operation, TaskEntity task, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.IDENTITY_LINK).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).inContextOf(task, Arrays.asList(propertyChange));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logProcessInstanceOperation(string operation, IList<PropertyChange> propertyChanges)
	  {
		logProcessInstanceOperation(operation, null, null, null, propertyChanges);
	  }

	  public virtual void logProcessInstanceOperation(string operation, string processInstanceId, string processDefinitionId, string processDefinitionKey, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.PROCESS_INSTANCE).propertyChanges(propertyChanges).processInstanceId(processInstanceId).processDefinitionId(processDefinitionId).processDefinitionKey(processDefinitionKey).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  if (!string.ReferenceEquals(processInstanceId, null))
		  {
			ExecutionEntity instance = ProcessInstanceManager.findExecutionById(processInstanceId);

			if (instance != null)
			{
			  entryBuilder.inContextOf(instance);
			}
		  }
		  else if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
			if (definition != null)
			{
			  entryBuilder.inContextOf(definition);
			}
		  }

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logProcessDefinitionOperation(string operation, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.PROCESS_DEFINITION).propertyChanges(propertyChange).processDefinitionId(processDefinitionId).processDefinitionKey(processDefinitionKey).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
			entryBuilder.inContextOf(definition);
		  }

		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logCaseInstanceOperation(string operation, string caseInstanceId, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.CASE_INSTANCE).caseInstanceId(caseInstanceId).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logCaseDefinitionOperation(string operation, string caseDefinitionId, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.CASE_DEFINITION).propertyChanges(propertyChanges).caseDefinitionId(caseDefinitionId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logDecisionDefinitionOperation(string operation, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.DECISION_DEFINITION).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logJobOperation(string operation, string jobId, string jobDefinitionId, string processInstanceId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
	  {
		logJobOperation(operation, jobId, jobDefinitionId, processInstanceId, processDefinitionId, processDefinitionKey, Collections.singletonList(propertyChange));
	  }

	  public virtual void logJobOperation(string operation, string jobId, string jobDefinitionId, string processInstanceId, string processDefinitionId, string processDefinitionKey, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.JOB).jobId(jobId).jobDefinitionId(jobDefinitionId).processDefinitionId(processDefinitionId).processDefinitionKey(processDefinitionKey).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  if (!string.ReferenceEquals(jobId, null))
		  {
			JobEntity job = JobManager.findJobById(jobId);
			// Backward compatibility
			if (job != null)
			{
			  entryBuilder.inContextOf(job);
			}
		  }
		  else
		  {

		  if (!string.ReferenceEquals(jobDefinitionId, null))
		  {
			JobDefinitionEntity jobDefinition = JobDefinitionManager.findById(jobDefinitionId);
			// Backward compatibility
			if (jobDefinition != null)
			{
			  entryBuilder.inContextOf(jobDefinition);
			}
		  }
		  else if (!string.ReferenceEquals(processInstanceId, null))
		  {
			ExecutionEntity processInstance = ProcessInstanceManager.findExecutionById(processInstanceId);
			// Backward compatibility
			if (processInstance != null)
			{
			  entryBuilder.inContextOf(processInstance);
			}
		  }
		  else if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
			// Backward compatibility
			if (definition != null)
			{
			  entryBuilder.inContextOf(definition);
			}
		  }
		  }

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logJobDefinitionOperation(string operation, string jobDefinitionId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.JOB_DEFINITION).jobDefinitionId(jobDefinitionId).processDefinitionId(processDefinitionId).processDefinitionKey(processDefinitionKey).propertyChanges(propertyChange).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  if (!string.ReferenceEquals(jobDefinitionId, null))
		  {
			JobDefinitionEntity jobDefinition = JobDefinitionManager.findById(jobDefinitionId);
			// Backward compatibility
			if (jobDefinition != null)
			{
			  entryBuilder.inContextOf(jobDefinition);
			}
		  }
		  else if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
			// Backward compatibility
			if (definition != null)
			{
			  entryBuilder.inContextOf(definition);
			}
		  }

		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logAttachmentOperation(string operation, TaskEntity task, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.ATTACHMENT).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).inContextOf(task, Arrays.asList(propertyChange));
		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logAttachmentOperation(string operation, ExecutionEntity processInstance, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.ATTACHMENT).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).inContextOf(processInstance, Arrays.asList(propertyChange));
		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logVariableOperation(string operation, string executionId, string taskId, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.VARIABLE).propertyChanges(propertyChange);

		  if (!string.ReferenceEquals(executionId, null))
		  {
			ExecutionEntity execution = ProcessInstanceManager.findExecutionById(executionId);
			entryBuilder.inContextOf(execution).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
		  }
		  else if (!string.ReferenceEquals(taskId, null))
		  {
			TaskEntity task = TaskManager.findTaskById(taskId);
			entryBuilder.inContextOf(task, Arrays.asList(propertyChange)).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
		  }

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logHistoricVariableOperation<T1>(string operation, HistoricProcessInstanceEntity historicProcessInstance, ResourceDefinitionEntity<T1> definition, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.VARIABLE).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).propertyChanges(propertyChange).inContextOf(historicProcessInstance, definition, Arrays.asList(propertyChange));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logHistoricVariableOperation<T1>(string operation, HistoricVariableInstanceEntity historicVariableInstance, ResourceDefinitionEntity<T1> definition, PropertyChange propertyChange)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.VARIABLE).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).propertyChanges(propertyChange).inContextOf(historicVariableInstance, definition, Arrays.asList(propertyChange));

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logDeploymentOperation(string operation, string deploymentId, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();

		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.DEPLOYMENT).deploymentId(deploymentId).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}

	  }

	  public virtual void logBatchOperation(string operation, IList<PropertyChange> propertyChange)
	  {
		logBatchOperation(operation, null, propertyChange);
	  }

	  public virtual void logBatchOperation(string operation, string batchId, PropertyChange propertyChange)
	  {
		logBatchOperation(operation, batchId, Collections.singletonList(propertyChange));
	  }

	  public virtual void logBatchOperation(string operation, string batchId, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.BATCH).batchId(batchId).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logDecisionInstanceOperation(string operation, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.DECISION_INSTANCE).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  context.addEntry(entryBuilder.create());

		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logExternalTaskOperation(string operation, ExternalTaskEntity externalTask, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.EXTERNAL_TASK).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		  if (externalTask != null)
		  {
			ExecutionEntity instance = null;
			ProcessDefinitionEntity definition = null;
			if (!string.ReferenceEquals(externalTask.ProcessInstanceId, null))
			{
			  instance = ProcessInstanceManager.findExecutionById(externalTask.ProcessInstanceId);
			}
			else if (!string.ReferenceEquals(externalTask.ProcessDefinitionId, null))
			{
			  definition = ProcessDefinitionManager.findLatestProcessDefinitionById(externalTask.ProcessDefinitionId);
			}
			entryBuilder.processInstanceId(externalTask.ProcessInstanceId).processDefinitionId(externalTask.ProcessDefinitionId).processDefinitionKey(externalTask.ProcessDefinitionKey).inContextOf(externalTask, instance, definition);
		  }

		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logMetricsOperation(string operation, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.METRICS).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logFilterOperation(string operation, string filterId)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.FILTER).propertyChanges(new PropertyChange("filterId", null, filterId)).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logPropertyOperation(string operation, IList<PropertyChange> propertyChanges)
	  {
		if (UserOperationLogEnabled)
		{
		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.PROPERTY).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN);
		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  public virtual void logAuthorizationOperation(string operation, AuthorizationEntity authorization, AuthorizationEntity previousValues)
	  {
		if (UserOperationLogEnabled)
		{
		  IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		  propertyChanges.Add(new PropertyChange("permissionBits", previousValues == null ? null : previousValues.getPermissions(), authorization.getPermissions()));
		  propertyChanges.Add(new PropertyChange("permissions", previousValues == null ? null : getPermissionStringList(previousValues), getPermissionStringList(authorization)));
		  propertyChanges.Add(new PropertyChange("type", previousValues == null ? null : previousValues.AuthorizationType, authorization.AuthorizationType));
		  propertyChanges.Add(new PropertyChange("resource", previousValues == null ? null : getResourceName(previousValues.ResourceType), getResourceName(authorization.ResourceType)));
		  propertyChanges.Add(new PropertyChange("resourceId", previousValues == null ? null : previousValues.ResourceId, authorization.ResourceId));
		  if (!string.ReferenceEquals(authorization.UserId, null) || (previousValues != null && !string.ReferenceEquals(previousValues.UserId, null)))
		  {
			propertyChanges.Add(new PropertyChange("userId", previousValues == null ? null : previousValues.UserId, authorization.UserId));
		  }
		  if (!string.ReferenceEquals(authorization.GroupId, null) || (previousValues != null && !string.ReferenceEquals(previousValues.GroupId, null)))
		  {
			propertyChanges.Add(new PropertyChange("groupId", previousValues == null ? null : previousValues.GroupId, authorization.GroupId));
		  }

		  UserOperationLogContext context = new UserOperationLogContext();
		  UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.entry(operation, EntityTypes.AUTHORIZATION).propertyChanges(propertyChanges).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN);
		  context.addEntry(entryBuilder.create());
		  fireUserOperationLog(context);
		}
	  }

	  protected internal virtual string getPermissionStringList(AuthorizationEntity authorization)
	  {
		Permission[] permissionsForResource = Context.ProcessEngineConfiguration.PermissionProvider.getPermissionsForResource(authorization.ResourceType);
		Permission[] permissions = authorization.getPermissions(permissionsForResource);
		string[] namesForPermissions = PermissionConverter.getNamesForPermissions(authorization, permissions);
		if (namesForPermissions.Length == 0)
		{
		  return Permissions.NONE.Name;
		}
		return StringUtil.trimToMaximumLengthAllowed(StringUtil.join(Arrays.asList(namesForPermissions).GetEnumerator()));
	  }

	  protected internal virtual string getResourceName(int resourceType)
	  {
		return Context.ProcessEngineConfiguration.PermissionProvider.getNameForResource(resourceType);
	  }

	  public virtual bool UserOperationLogEnabled
	  {
		  get
		  {
			return HistoryEventProduced && ((UserOperationLogEnabledOnCommandContext && UserAuthenticated) || !writeUserOperationLogOnlyWithLoggedInUser());
		  }
	  }

	  protected internal virtual bool HistoryEventProduced
	  {
		  get
		  {
			HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
			return historyLevel.isHistoryEventProduced(HistoryEventTypes.USER_OPERATION_LOG, null);
		  }
	  }

	  protected internal virtual bool UserAuthenticated
	  {
		  get
		  {
			string userId = AuthenticatedUserId;
			return !string.ReferenceEquals(userId, null) && userId.Length > 0;
		  }
	  }

	  protected internal virtual string AuthenticatedUserId
	  {
		  get
		  {
			CommandContext commandContext = Context.CommandContext;
			return commandContext.AuthenticatedUserId;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void fireUserOperationLog(final org.camunda.bpm.engine.impl.oplog.UserOperationLogContext context)
	  protected internal virtual void fireUserOperationLog(UserOperationLogContext context)
	  {
		if (string.ReferenceEquals(context.UserId, null))
		{
		  context.UserId = AuthenticatedUserId;
		}

		HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, context));
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly UserOperationLogManager outerInstance;

		  private UserOperationLogContext context;

		  public HistoryEventCreatorAnonymousInnerClass(UserOperationLogManager outerInstance, UserOperationLogContext context)
		  {
			  this.outerInstance = outerInstance;
			  this.context = context;
		  }

		  public override IList<HistoryEvent> createHistoryEvents(HistoryEventProducer producer)
		  {
			return producer.createUserOperationLogEvents(context);
		  }
	  }

	  protected internal virtual bool writeUserOperationLogOnlyWithLoggedInUser()
	  {
		return Context.CommandContext.RestrictUserOperationLogToAuthenticatedUsers;
	  }

	  protected internal virtual bool UserOperationLogEnabledOnCommandContext
	  {
		  get
		  {
			return Context.CommandContext.UserOperationLogEnabled;
		  }
	  }

	  protected internal virtual string getOperationType(IdentityOperationResult operationResult)
	  {
		switch (operationResult.Operation)
		{
		case IdentityOperationResult.OPERATION_CREATE:
		  return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;
		case IdentityOperationResult.OPERATION_UPDATE:
		  return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;
		case IdentityOperationResult.OPERATION_DELETE:
		  return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE;
		case IdentityOperationResult.OPERATION_UNLOCK:
		  return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UNLOCK;
		default:
		  return null;
		}
	  }

	}

}