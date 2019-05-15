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
namespace org.camunda.bpm.engine.impl.json
{

	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using JsonArray = com.google.gson.JsonArray;
	using JsonElement = com.google.gson.JsonElement;
	using JsonObject = com.google.gson.JsonObject;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class JsonTaskQueryConverter : JsonObjectConverter<TaskQuery>
	{

	  public const string ID = "id";
	  public const string TASK_ID = "taskId";
	  public const string NAME = "name";
	  public const string NAME_NOT_EQUAL = "nameNotEqual";
	  public const string NAME_LIKE = "nameLike";
	  public const string NAME_NOT_LIKE = "nameNotLike";
	  public const string DESCRIPTION = "description";
	  public const string DESCRIPTION_LIKE = "descriptionLike";
	  public const string PRIORITY = "priority";
	  public const string MIN_PRIORITY = "minPriority";
	  public const string MAX_PRIORITY = "maxPriority";
	  public const string ASSIGNEE = "assignee";
	  public const string ASSIGNEE_LIKE = "assigneeLike";
	  public const string INVOLVED_USER = "involvedUser";
	  public const string OWNER = "owner";
	  public const string UNASSIGNED = "unassigned";
	  public const string ASSIGNED = "assigned";
	  public const string DELEGATION_STATE = "delegationState";
	  public const string CANDIDATE_USER = "candidateUser";
	  public const string CANDIDATE_GROUP = "candidateGroup";
	  public const string CANDIDATE_GROUPS = "candidateGroups";
	  public const string WITH_CANDIDATE_GROUPS = "withCandidateGroups";
	  public const string WITHOUT_CANDIDATE_GROUPS = "withoutCandidateGroups";
	  public const string WITH_CANDIDATE_USERS = "withCandidateUsers";
	  public const string WITHOUT_CANDIDATE_USERS = "withoutCandidateUsers";
	  public const string INCLUDE_ASSIGNED_TASKS = "includeAssignedTasks";
	  public const string INSTANCE_ID = "instanceId";
	  public const string PROCESS_INSTANCE_ID = "processInstanceId";
	  public const string EXECUTION_ID = "executionId";
	  public const string ACTIVITY_INSTANCE_ID_IN = "activityInstanceIdIn";
	  public const string CREATED = "created";
	  public const string CREATED_BEFORE = "createdBefore";
	  public const string CREATED_AFTER = "createdAfter";
	  public const string KEY = "key";
	  public const string KEYS = "keys";
	  public const string KEY_LIKE = "keyLike";
	  public const string PARENT_TASK_ID = "parentTaskId";
	  public const string PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  public const string PROCESS_DEFINITION_KEYS = "processDefinitionKeys";
	  public const string PROCESS_DEFINITION_ID = "processDefinitionId";
	  public const string PROCESS_DEFINITION_NAME = "processDefinitionName";
	  public const string PROCESS_DEFINITION_NAME_LIKE = "processDefinitionNameLike";
	  public const string PROCESS_INSTANCE_BUSINESS_KEY = "processInstanceBusinessKey";
	  public const string PROCESS_INSTANCE_BUSINESS_KEYS = "processInstanceBusinessKeys";
	  public const string PROCESS_INSTANCE_BUSINESS_KEY_LIKE = "processInstanceBusinessKeyLike";
	  public const string DUE = "due";
	  public const string DUE_DATE = "dueDate";
	  public const string DUE_BEFORE = "dueBefore";
	  public const string DUE_AFTER = "dueAfter";
	  public const string FOLLOW_UP = "followUp";
	  public const string FOLLOW_UP_DATE = "followUpDate";
	  public const string FOLLOW_UP_BEFORE = "followUpBefore";
	  public const string FOLLOW_UP_NULL_ACCEPTED = "followUpNullAccepted";
	  public const string FOLLOW_UP_AFTER = "followUpAfter";
	  public const string EXCLUDE_SUBTASKS = "excludeSubtasks";
	  public const string CASE_DEFINITION_KEY = "caseDefinitionKey";
	  public const string CASE_DEFINITION_ID = "caseDefinitionId";
	  public const string CASE_DEFINITION_NAME = "caseDefinitionName";
	  public const string CASE_DEFINITION_NAME_LIKE = "caseDefinitionNameLike";
	  public const string CASE_INSTANCE_ID = "caseInstanceId";
	  public const string CASE_INSTANCE_BUSINESS_KEY = "caseInstanceBusinessKey";
	  public const string CASE_INSTANCE_BUSINESS_KEY_LIKE = "caseInstanceBusinessKeyLike";
	  public const string CASE_EXECUTION_ID = "caseExecutionId";
	  public const string ACTIVE = "active";
	  public const string SUSPENDED = "suspended";
	  public const string PROCESS_VARIABLES = "processVariables";
	  public const string TASK_VARIABLES = "taskVariables";
	  public const string CASE_INSTANCE_VARIABLES = "caseInstanceVariables";
	  public const string TENANT_IDS = "tenantIds";
	  public const string WITHOUT_TENANT_ID = "withoutTenantId";
	  public const string ORDERING_PROPERTIES = "orderingProperties";
	  public const string OR_QUERIES = "orQueries";

	  /// <summary>
	  /// Exists for backwards compatibility with 7.2; deprecated since 7.3
	  /// </summary>
	  [Obsolete]
	  public const string ORDER_BY = "orderBy";

	  protected internal static JsonTaskQueryVariableValueConverter variableValueConverter = new JsonTaskQueryVariableValueConverter();

	  public override JsonObject toJsonObject(TaskQuery taskQuery)
	  {
		return toJsonObject(taskQuery, false);
	  }

	  public virtual JsonObject toJsonObject(TaskQuery taskQuery, bool isOrQueryActive)
	  {
		JsonObject json = JsonUtil.createObject();
		TaskQueryImpl query = (TaskQueryImpl) taskQuery;

		JsonUtil.addField(json, TASK_ID, query.TaskId);
		JsonUtil.addField(json, NAME, query.Name);
		JsonUtil.addField(json, NAME_NOT_EQUAL, query.NameNotEqual);
		JsonUtil.addField(json, NAME_LIKE, query.NameLike);
		JsonUtil.addField(json, NAME_NOT_LIKE, query.NameNotLike);
		JsonUtil.addField(json, DESCRIPTION, query.Description);
		JsonUtil.addField(json, DESCRIPTION_LIKE, query.DescriptionLike);
		JsonUtil.addField(json, PRIORITY, query.Priority);
		JsonUtil.addField(json, MIN_PRIORITY, query.MinPriority);
		JsonUtil.addField(json, MAX_PRIORITY, query.MaxPriority);
		JsonUtil.addField(json, ASSIGNEE, query.Assignee);
		JsonUtil.addField(json, ASSIGNEE_LIKE, query.AssigneeLike);
		JsonUtil.addField(json, INVOLVED_USER, query.InvolvedUser);
		JsonUtil.addField(json, OWNER, query.Owner);
		JsonUtil.addDefaultField(json, UNASSIGNED, false, query.Unassigned);
		JsonUtil.addDefaultField(json, ASSIGNED, false, query.Assigned);
		JsonUtil.addField(json, DELEGATION_STATE, query.DelegationStateString);
		JsonUtil.addField(json, CANDIDATE_USER, query.CandidateUser);
		JsonUtil.addField(json, CANDIDATE_GROUP, query.CandidateGroup);
		JsonUtil.addListField(json, CANDIDATE_GROUPS, query.CandidateGroupsInternal);
		JsonUtil.addDefaultField(json, WITH_CANDIDATE_GROUPS, false, query.WithCandidateGroups);
		JsonUtil.addDefaultField(json, WITHOUT_CANDIDATE_GROUPS, false, query.WithoutCandidateGroups);
		JsonUtil.addDefaultField(json, WITH_CANDIDATE_USERS, false, query.WithCandidateUsers);
		JsonUtil.addDefaultField(json, WITHOUT_CANDIDATE_USERS, false, query.WithoutCandidateUsers);
		JsonUtil.addField(json, INCLUDE_ASSIGNED_TASKS, query.IncludeAssignedTasksInternal);
		JsonUtil.addField(json, PROCESS_INSTANCE_ID, query.ProcessInstanceId);
		JsonUtil.addField(json, EXECUTION_ID, query.ExecutionId);
		JsonUtil.addArrayField(json, ACTIVITY_INSTANCE_ID_IN, query.ActivityInstanceIdIn);
		JsonUtil.addDateField(json, CREATED, query.CreateTime);
		JsonUtil.addDateField(json, CREATED_BEFORE, query.CreateTimeBefore);
		JsonUtil.addDateField(json, CREATED_AFTER, query.CreateTimeAfter);
		JsonUtil.addField(json, KEY, query.Key);
		JsonUtil.addArrayField(json, KEYS, query.Keys);
		JsonUtil.addField(json, KEY_LIKE, query.KeyLike);
		JsonUtil.addField(json, PARENT_TASK_ID, query.ParentTaskId);
		JsonUtil.addField(json, PROCESS_DEFINITION_KEY, query.ProcessDefinitionKey);
		JsonUtil.addArrayField(json, PROCESS_DEFINITION_KEYS, query.ProcessDefinitionKeys);
		JsonUtil.addField(json, PROCESS_DEFINITION_ID, query.ProcessDefinitionId);
		JsonUtil.addField(json, PROCESS_DEFINITION_NAME, query.ProcessDefinitionName);
		JsonUtil.addField(json, PROCESS_DEFINITION_NAME_LIKE, query.ProcessDefinitionNameLike);
		JsonUtil.addField(json, PROCESS_INSTANCE_BUSINESS_KEY, query.ProcessInstanceBusinessKey);
		JsonUtil.addArrayField(json, PROCESS_INSTANCE_BUSINESS_KEYS, query.ProcessInstanceBusinessKeys);
		JsonUtil.addField(json, PROCESS_INSTANCE_BUSINESS_KEY_LIKE, query.ProcessInstanceBusinessKeyLike);
		addVariablesFields(json, query.Variables);
		JsonUtil.addDateField(json, DUE, query.DueDate);
		JsonUtil.addDateField(json, DUE_BEFORE, query.DueBefore);
		JsonUtil.addDateField(json, DUE_AFTER, query.DueAfter);
		JsonUtil.addDateField(json, FOLLOW_UP, query.FollowUpDate);
		JsonUtil.addDateField(json, FOLLOW_UP_BEFORE, query.FollowUpBefore);
		JsonUtil.addDefaultField(json, FOLLOW_UP_NULL_ACCEPTED, false, query.FollowUpNullAccepted);
		JsonUtil.addDateField(json, FOLLOW_UP_AFTER, query.FollowUpAfter);
		JsonUtil.addDefaultField(json, EXCLUDE_SUBTASKS, false, query.ExcludeSubtasks);
		addSuspensionStateField(json, query.SuspensionState);
		JsonUtil.addField(json, CASE_DEFINITION_KEY, query.CaseDefinitionKey);
		JsonUtil.addField(json, CASE_DEFINITION_ID, query.CaseDefinitionId);
		JsonUtil.addField(json, CASE_DEFINITION_NAME, query.CaseDefinitionName);
		JsonUtil.addField(json, CASE_DEFINITION_NAME_LIKE, query.CaseDefinitionNameLike);
		JsonUtil.addField(json, CASE_INSTANCE_ID, query.CaseInstanceId);
		JsonUtil.addField(json, CASE_INSTANCE_BUSINESS_KEY, query.CaseInstanceBusinessKey);
		JsonUtil.addField(json, CASE_INSTANCE_BUSINESS_KEY_LIKE, query.CaseInstanceBusinessKeyLike);
		JsonUtil.addField(json, CASE_EXECUTION_ID, query.CaseExecutionId);
		addTenantIdFields(json, query);

		if (query.Queries.Count > 1 && !isOrQueryActive)
		{
		  JsonArray orQueries = JsonUtil.createArray();

		  foreach (TaskQueryImpl orQuery in query.Queries)
		  {
			if (orQuery != null && orQuery.OrQueryActive)
			{
			  orQueries.add(toJsonObject(orQuery, true));
			}
		  }

		  JsonUtil.addField(json, OR_QUERIES, orQueries);
		}

		if (query.OrderingProperties != null && !query.OrderingProperties.Empty)
		{
		  JsonUtil.addField(json, ORDERING_PROPERTIES, JsonQueryOrderingPropertyConverter.ARRAY_CONVERTER.toJsonArray(query.OrderingProperties));
		}


		// expressions
		foreach (KeyValuePair<string, string> expressionEntry in query.Expressions.SetOfKeyValuePairs())
		{
		  JsonUtil.addField(json, expressionEntry.Key + "Expression", expressionEntry.Value);
		}

		return json;
	  }

	  protected internal virtual void addSuspensionStateField(JsonObject jsonObject, SuspensionState suspensionState)
	  {
		if (suspensionState != null)
		{
		  if (suspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE))
		  {
			JsonUtil.addField(jsonObject, ACTIVE, true);
		  }
		  else if (suspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED))
		  {
			JsonUtil.addField(jsonObject, SUSPENDED, true);
		  }
		}
	  }

	  protected internal virtual void addTenantIdFields(JsonObject jsonObject, TaskQueryImpl query)
	  {
		if (query.TenantIdSet)
		{
		  if (query.TenantIds != null)
		  {
			JsonUtil.addArrayField(jsonObject, TENANT_IDS, query.TenantIds);
		  }
		  else
		  {
			JsonUtil.addField(jsonObject, WITHOUT_TENANT_ID, true);
		  }
		}
	  }

	  protected internal virtual void addVariablesFields(JsonObject jsonObject, IList<TaskQueryVariableValue> variables)
	  {
		foreach (TaskQueryVariableValue variable in variables)
		{
		  if (variable.ProcessInstanceVariable)
		  {
			addVariable(jsonObject, PROCESS_VARIABLES, variable);
		  }
		  else if (variable.Local)
		  {
			addVariable(jsonObject, TASK_VARIABLES, variable);
		  }
		  else
		  {
			addVariable(jsonObject, CASE_INSTANCE_VARIABLES, variable);
		  }
		}
	  }

	  protected internal virtual void addVariable(JsonObject jsonObject, string variableType, TaskQueryVariableValue variable)
	  {
		JsonArray variables = JsonUtil.getArray(jsonObject, variableType);

		JsonUtil.addElement(variables, variableValueConverter, variable);
		JsonUtil.addField(jsonObject, variableType, variables);
	  }

	  public override TaskQuery toObject(JsonObject json)
	  {
		TaskQueryImpl query = new TaskQueryImpl();

		if (json.has(OR_QUERIES))
		{
		  foreach (JsonElement jsonElement in JsonUtil.getArray(json, OR_QUERIES))
		  {
			query.addOrQuery((TaskQueryImpl) toObject(JsonUtil.getObject(jsonElement)));
		  }
		}
		if (json.has(TASK_ID))
		{
		  query.taskId(JsonUtil.getString(json,TASK_ID));
		}
		if (json.has(NAME))
		{
		  query.taskName(JsonUtil.getString(json, NAME));
		}
		if (json.has(NAME_NOT_EQUAL))
		{
		  query.taskNameNotEqual(JsonUtil.getString(json, NAME_NOT_EQUAL));
		}
		if (json.has(NAME_LIKE))
		{
		  query.taskNameLike(JsonUtil.getString(json, NAME_LIKE));
		}
		if (json.has(NAME_NOT_LIKE))
		{
		  query.taskNameNotLike(JsonUtil.getString(json, NAME_NOT_LIKE));
		}
		if (json.has(DESCRIPTION))
		{
		  query.taskDescription(JsonUtil.getString(json, DESCRIPTION));
		}
		if (json.has(DESCRIPTION_LIKE))
		{
		  query.taskDescriptionLike(JsonUtil.getString(json, DESCRIPTION_LIKE));
		}
		if (json.has(PRIORITY))
		{
		  query.taskPriority(JsonUtil.getInt(json, PRIORITY));
		}
		if (json.has(MIN_PRIORITY))
		{
		  query.taskMinPriority(JsonUtil.getInt(json, MIN_PRIORITY));
		}
		if (json.has(MAX_PRIORITY))
		{
		  query.taskMaxPriority(JsonUtil.getInt(json, MAX_PRIORITY));
		}
		if (json.has(ASSIGNEE))
		{
		  query.taskAssignee(JsonUtil.getString(json, ASSIGNEE));
		}
		if (json.has(ASSIGNEE_LIKE))
		{
		  query.taskAssigneeLike(JsonUtil.getString(json, ASSIGNEE_LIKE));
		}
		if (json.has(INVOLVED_USER))
		{
		  query.taskInvolvedUser(JsonUtil.getString(json, INVOLVED_USER));
		}
		if (json.has(OWNER))
		{
		  query.taskOwner(JsonUtil.getString(json, OWNER));
		}
		if (json.has(ASSIGNED) && JsonUtil.getBoolean(json, ASSIGNED))
		{
		  query.taskAssigned();
		}
		if (json.has(UNASSIGNED) && JsonUtil.getBoolean(json, UNASSIGNED))
		{
		  query.taskUnassigned();
		}
		if (json.has(DELEGATION_STATE))
		{
		  query.taskDelegationState(Enum.Parse(typeof(DelegationState), JsonUtil.getString(json, DELEGATION_STATE)));
		}
		if (json.has(CANDIDATE_USER))
		{
		  query.taskCandidateUser(JsonUtil.getString(json, CANDIDATE_USER));
		}
		if (json.has(CANDIDATE_GROUP))
		{
		  query.taskCandidateGroup(JsonUtil.getString(json, CANDIDATE_GROUP));
		}
		if (json.has(CANDIDATE_GROUPS) && !json.has(CANDIDATE_USER) && !json.has(CANDIDATE_GROUP))
		{
		  query.taskCandidateGroupIn(getList(JsonUtil.getArray(json, CANDIDATE_GROUPS)));
		}
		if (json.has(WITH_CANDIDATE_GROUPS) && JsonUtil.getBoolean(json, WITH_CANDIDATE_GROUPS))
		{
		  query.withCandidateGroups();
		}
		if (json.has(WITHOUT_CANDIDATE_GROUPS) && JsonUtil.getBoolean(json, WITHOUT_CANDIDATE_GROUPS))
		{
		  query.withoutCandidateGroups();
		}
		if (json.has(WITH_CANDIDATE_USERS) && JsonUtil.getBoolean(json, WITH_CANDIDATE_USERS))
		{
		  query.withCandidateUsers();
		}
		if (json.has(WITHOUT_CANDIDATE_USERS) && JsonUtil.getBoolean(json, WITHOUT_CANDIDATE_USERS))
		{
		  query.withoutCandidateUsers();
		}
		if (json.has(INCLUDE_ASSIGNED_TASKS) && JsonUtil.getBoolean(json, INCLUDE_ASSIGNED_TASKS))
		{
		  query.includeAssignedTasksInternal();
		}
		if (json.has(PROCESS_INSTANCE_ID))
		{
		  query.processInstanceId(JsonUtil.getString(json, PROCESS_INSTANCE_ID));
		}
		if (json.has(EXECUTION_ID))
		{
		  query.executionId(JsonUtil.getString(json, EXECUTION_ID));
		}
		if (json.has(ACTIVITY_INSTANCE_ID_IN))
		{
		  query.activityInstanceIdIn(getArray(JsonUtil.getArray(json, ACTIVITY_INSTANCE_ID_IN)));
		}
		if (json.has(CREATED))
		{
		  query.taskCreatedOn(new DateTime(JsonUtil.getLong(json, CREATED)));
		}
		if (json.has(CREATED_BEFORE))
		{
		  query.taskCreatedBefore(new DateTime(JsonUtil.getLong(json, CREATED_BEFORE)));
		}
		if (json.has(CREATED_AFTER))
		{
		  query.taskCreatedAfter(new DateTime(JsonUtil.getLong(json, CREATED_AFTER)));
		}
		if (json.has(KEY))
		{
		  query.taskDefinitionKey(JsonUtil.getString(json, KEY));
		}
		if (json.has(KEYS))
		{
		  query.taskDefinitionKeyIn(getArray(JsonUtil.getArray(json, KEYS)));
		}
		if (json.has(KEY_LIKE))
		{
		  query.taskDefinitionKeyLike(JsonUtil.getString(json, KEY_LIKE));
		}
		if (json.has(PARENT_TASK_ID))
		{
		  query.taskParentTaskId(JsonUtil.getString(json, PARENT_TASK_ID));
		}
		if (json.has(PROCESS_DEFINITION_KEY))
		{
		  query.processDefinitionKey(JsonUtil.getString(json, PROCESS_DEFINITION_KEY));
		}
		if (json.has(PROCESS_DEFINITION_KEYS))
		{
		  query.processDefinitionKeyIn(getArray(JsonUtil.getArray(json, PROCESS_DEFINITION_KEYS)));
		}
		if (json.has(PROCESS_DEFINITION_ID))
		{
		  query.processDefinitionId(JsonUtil.getString(json, PROCESS_DEFINITION_ID));
		}
		if (json.has(PROCESS_DEFINITION_NAME))
		{
		  query.processDefinitionName(JsonUtil.getString(json, PROCESS_DEFINITION_NAME));
		}
		if (json.has(PROCESS_DEFINITION_NAME_LIKE))
		{
		  query.processDefinitionNameLike(JsonUtil.getString(json, PROCESS_DEFINITION_NAME_LIKE));
		}
		if (json.has(PROCESS_INSTANCE_BUSINESS_KEY))
		{
		  query.processInstanceBusinessKey(JsonUtil.getString(json, PROCESS_INSTANCE_BUSINESS_KEY));
		}
		if (json.has(PROCESS_INSTANCE_BUSINESS_KEYS))
		{
		  query.processInstanceBusinessKeyIn(getArray(JsonUtil.getArray(json, PROCESS_INSTANCE_BUSINESS_KEYS)));
		}
		if (json.has(PROCESS_INSTANCE_BUSINESS_KEY_LIKE))
		{
		  query.processInstanceBusinessKeyLike(JsonUtil.getString(json, PROCESS_INSTANCE_BUSINESS_KEY_LIKE));
		}
		if (json.has(TASK_VARIABLES))
		{
		  addVariables(query, JsonUtil.getArray(json, TASK_VARIABLES), true, false);
		}
		if (json.has(PROCESS_VARIABLES))
		{
		  addVariables(query, JsonUtil.getArray(json, PROCESS_VARIABLES), false, true);
		}
		if (json.has(CASE_INSTANCE_VARIABLES))
		{
		  addVariables(query, JsonUtil.getArray(json, CASE_INSTANCE_VARIABLES), false, false);
		}
		if (json.has(DUE))
		{
		  query.dueDate(new DateTime(JsonUtil.getLong(json, DUE)));
		}
		if (json.has(DUE_BEFORE))
		{
		  query.dueBefore(new DateTime(JsonUtil.getLong(json, DUE_BEFORE)));
		}
		if (json.has(DUE_AFTER))
		{
		  query.dueAfter(new DateTime(JsonUtil.getLong(json, DUE_AFTER)));
		}
		if (json.has(FOLLOW_UP))
		{
		  query.followUpDate(new DateTime(JsonUtil.getLong(json, FOLLOW_UP)));
		}
		if (json.has(FOLLOW_UP_BEFORE))
		{
		  query.followUpBefore(new DateTime(JsonUtil.getLong(json, FOLLOW_UP_BEFORE)));
		}
		if (json.has(FOLLOW_UP_AFTER))
		{
		  query.followUpAfter(new DateTime(JsonUtil.getLong(json, FOLLOW_UP_AFTER)));
		}
		if (json.has(FOLLOW_UP_NULL_ACCEPTED))
		{
		  query.FollowUpNullAccepted = JsonUtil.getBoolean(json, FOLLOW_UP_NULL_ACCEPTED);
		}
		if (json.has(EXCLUDE_SUBTASKS) && JsonUtil.getBoolean(json, EXCLUDE_SUBTASKS))
		{
		  query.excludeSubtasks();
		}
		if (json.has(SUSPENDED) && JsonUtil.getBoolean(json, SUSPENDED))
		{
		  query.suspended();
		}
		if (json.has(ACTIVE) && JsonUtil.getBoolean(json, ACTIVE))
		{
		  query.active();
		}
		if (json.has(CASE_DEFINITION_KEY))
		{
		  query.caseDefinitionKey(JsonUtil.getString(json, CASE_DEFINITION_KEY));
		}
		if (json.has(CASE_DEFINITION_ID))
		{
		  query.caseDefinitionId(JsonUtil.getString(json, CASE_DEFINITION_ID));
		}
		if (json.has(CASE_DEFINITION_NAME))
		{
		  query.caseDefinitionName(JsonUtil.getString(json, CASE_DEFINITION_NAME));
		}
		if (json.has(CASE_DEFINITION_NAME_LIKE))
		{
		  query.caseDefinitionNameLike(JsonUtil.getString(json, CASE_DEFINITION_NAME_LIKE));
		}
		if (json.has(CASE_INSTANCE_ID))
		{
		  query.caseInstanceId(JsonUtil.getString(json, CASE_INSTANCE_ID));
		}
		if (json.has(CASE_INSTANCE_BUSINESS_KEY))
		{
		  query.caseInstanceBusinessKey(JsonUtil.getString(json, CASE_INSTANCE_BUSINESS_KEY));
		}
		if (json.has(CASE_INSTANCE_BUSINESS_KEY_LIKE))
		{
		  query.caseInstanceBusinessKeyLike(JsonUtil.getString(json, CASE_INSTANCE_BUSINESS_KEY_LIKE));
		}
		if (json.has(CASE_EXECUTION_ID))
		{
		  query.caseExecutionId(JsonUtil.getString(json, CASE_EXECUTION_ID));
		}
		if (json.has(TENANT_IDS))
		{
		  query.tenantIdIn(getArray(JsonUtil.getArray(json, TENANT_IDS)));
		}
		if (json.has(WITHOUT_TENANT_ID))
		{
		  query.withoutTenantId();
		}
		if (json.has(ORDER_BY))
		{
		  IList<QueryOrderingProperty> orderingProperties = JsonLegacyQueryOrderingPropertyConverter.INSTANCE.fromOrderByString(JsonUtil.getString(json, ORDER_BY));

		  query.OrderingProperties = orderingProperties;
		}
		if (json.has(ORDERING_PROPERTIES))
		{
		  JsonArray jsonArray = JsonUtil.getArray(json, ORDERING_PROPERTIES);
		  query.OrderingProperties = JsonQueryOrderingPropertyConverter.ARRAY_CONVERTER.toObject(jsonArray);
		}

		// expressions
		foreach (KeyValuePair<string, JsonElement> entry in json.entrySet())
		{
		  string key = entry.Key;
		  if (key.EndsWith("Expression", StringComparison.Ordinal))
		  {
			string expression = JsonUtil.getString(json, key);
			query.addExpression(key.Substring(0, key.Length - "Expression".Length), expression);
		  }
		}

		return query;
	  }

	  protected internal virtual string[] getArray(JsonArray array)
	  {
		return getList(array).ToArray();
	  }

	  protected internal virtual IList<string> getList(JsonArray array)
	  {
		IList<string> list = new List<string>();
		foreach (JsonElement entry in array)
		{
		  list.Add(JsonUtil.getString(entry));
		}
		return list;
	  }

	  protected internal virtual void addVariables(TaskQueryImpl query, JsonArray variables, bool isTaskVariable, bool isProcessVariable)
	  {
		foreach (JsonElement variable in variables)
		{
		  JsonObject variableObj = JsonUtil.getObject(variable);
		  string name = JsonUtil.getString(variableObj, NAME);
		  object rawValue = JsonUtil.getRawObject(variableObj, "value");
		  QueryOperator @operator = Enum.Parse(typeof(QueryOperator), JsonUtil.getString(variableObj, "operator"));
		  query.addVariable(name, rawValue, @operator, isTaskVariable, isProcessVariable);
		}
	  }

	}

}