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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Group = org.camunda.bpm.engine.identity.Group;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class TaskQueryImpl : AbstractQuery<TaskQuery, Task>, TaskQuery
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			queries = new List<TaskQueryImpl>(Arrays.asList(this));
		}


	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
	  protected internal string name;
	  protected internal string nameNotEqual;
	  protected internal string nameLike;
	  protected internal string nameNotLike;
	  protected internal string description;
	  protected internal string descriptionLike;
	  protected internal int? priority;
	  protected internal int? minPriority;
	  protected internal int? maxPriority;
	  protected internal string assignee;
	  protected internal string assigneeLike;
	  protected internal string involvedUser;
	  protected internal string owner;
	  protected internal bool? unassigned;
	  protected internal bool? assigned;
	  protected internal bool noDelegationState = false;
	  protected internal DelegationState delegationState;
	  protected internal string candidateUser;
	  protected internal string candidateGroup;
	  protected internal IList<string> candidateGroups;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withCandidateGroups_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withoutCandidateGroups_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withCandidateUsers_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withoutCandidateUsers_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? includeAssignedTasks_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] activityInstanceIdIn_Conflict;
	  protected internal DateTime createTime;
	  protected internal DateTime createTimeBefore;
	  protected internal DateTime createTimeAfter;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string[] taskDefinitionKeys;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
	  protected internal string[] processDefinitionKeys;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionNameLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceBusinessKey_Conflict;
	  protected internal string[] processInstanceBusinessKeys;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceBusinessKeyLike_Conflict;
	  protected internal IList<TaskQueryVariableValue> variables = new List<TaskQueryVariableValue>();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime dueDate_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime dueBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime dueAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime followUpDate_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime followUpBefore_Conflict;
	  protected internal bool followUpNullAccepted = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime followUpAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool excludeSubtasks_Conflict = false;
	  protected internal SuspensionState suspensionState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool initializeFormKeys_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool taskNameCaseInsensitive_Conflict = false;

	  protected internal bool? variableNamesIgnoreCase;
	  protected internal bool? variableValuesIgnoreCase;

	  protected internal string parentTaskId;
	  protected internal bool isTenantIdSet = false;

	  protected internal string[] tenantIds;
	  // case management /////////////////////////////
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionNameLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceBusinessKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceBusinessKeyLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;

	  protected internal IList<string> cachedCandidateGroups;

	  // or query /////////////////////////////
	  protected internal IList<TaskQueryImpl> queries;
	  protected internal bool isOrQueryActive = false;

	  public TaskQueryImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public TaskQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public virtual TaskQueryImpl taskId(string taskId)
	  {
		ensureNotNull("Task id", taskId);
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual TaskQueryImpl taskName(string name)
	  {
		this.name = name;
		return this;
	  }

	  public virtual TaskQueryImpl taskNameLike(string nameLike)
	  {
		ensureNotNull("Task nameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual TaskQueryImpl taskDescription(string description)
	  {
		ensureNotNull("Description", description);
		this.description = description;
		return this;
	  }

	  public virtual TaskQuery taskDescriptionLike(string descriptionLike)
	  {
		ensureNotNull("Task descriptionLike", descriptionLike);
		this.descriptionLike = descriptionLike;
		return this;
	  }

	  public virtual TaskQuery taskPriority(int? priority)
	  {
		ensureNotNull("Priority", priority);
		this.priority = priority;
		return this;
	  }

	  public virtual TaskQuery taskMinPriority(int? minPriority)
	  {
		ensureNotNull("Min Priority", minPriority);
		this.minPriority = minPriority;
		return this;
	  }

	  public virtual TaskQuery taskMaxPriority(int? maxPriority)
	  {
		ensureNotNull("Max Priority", maxPriority);
		this.maxPriority = maxPriority;
		return this;
	  }

	  public virtual TaskQueryImpl taskAssignee(string assignee)
	  {
		ensureNotNull("Assignee", assignee);
		this.assignee = assignee;
		expressions.Remove("taskAssignee");
		return this;
	  }

	  public virtual TaskQuery taskAssigneeExpression(string assigneeExpression)
	  {
		ensureNotNull("Assignee expression", assigneeExpression);
		expressions["taskAssignee"] = assigneeExpression;
		return this;
	  }

	  public virtual TaskQuery taskAssigneeLike(string assignee)
	  {
		ensureNotNull("Assignee", assignee);
		this.assigneeLike = assignee;
		expressions.Remove("taskAssigneeLike");
		return this;
	  }

	  public virtual TaskQuery taskAssigneeLikeExpression(string assigneeLikeExpression)
	  {
		ensureNotNull("Assignee like expression", assigneeLikeExpression);
		expressions["taskAssigneeLike"] = assigneeLikeExpression;
		return this;
	  }

	  public virtual TaskQueryImpl taskOwner(string owner)
	  {
		ensureNotNull("Owner", owner);
		this.owner = owner;
		expressions.Remove("taskOwner");
		return this;
	  }

	  public virtual TaskQuery taskOwnerExpression(string ownerExpression)
	  {
		ensureNotNull("Owner expression", ownerExpression);
		expressions["taskOwner"] = ownerExpression;
		return this;
	  }

	  /// <seealso cref= <seealso cref="taskUnassigned"/> </seealso>
	  [Obsolete]
	  public virtual TaskQuery taskUnnassigned()
	  {
		return taskUnassigned();
	  }

	  public virtual TaskQuery taskUnassigned()
	  {
		this.unassigned = true;
		return this;
	  }

	  public virtual TaskQuery taskAssigned()
	  {
		this.assigned = true;
		return this;
	  }

	  public virtual TaskQuery taskDelegationState(DelegationState delegationState)
	  {
		if (delegationState == null)
		{
		  this.noDelegationState = true;
		}
		else
		{
		  this.delegationState = delegationState;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskCandidateUser(string candidateUser)
	  {
		ensureNotNull("Candidate user", candidateUser);
		if (!isOrQueryActive)
		{
		  if (!string.ReferenceEquals(candidateGroup, null) || expressions.ContainsKey("taskCandidateGroup"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateUser and candidateGroup");
		  }
		  if (candidateGroups != null || expressions.ContainsKey("taskCandidateGroupIn"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateUser and candidateGroupIn");
		  }
		}

		this.candidateUser = candidateUser;
		expressions.Remove("taskCandidateUser");
		return this;
	  }

	  public virtual TaskQuery taskCandidateUserExpression(string candidateUserExpression)
	  {
		ensureNotNull("Candidate user expression", candidateUserExpression);

		if (!string.ReferenceEquals(candidateGroup, null) || expressions.ContainsKey("taskCandidateGroup"))
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set both candidateUser and candidateGroup");
		}
		if (candidateGroups != null || expressions.ContainsKey("taskCandidateGroupIn"))
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set both candidateUser and candidateGroupIn");
		}

		expressions["taskCandidateUser"] = candidateUserExpression;
		return this;
	  }

	  public virtual TaskQueryImpl taskInvolvedUser(string involvedUser)
	  {
		ensureNotNull("Involved user", involvedUser);
		this.involvedUser = involvedUser;
		expressions.Remove("taskInvolvedUser");
		return this;
	  }

	  public virtual TaskQuery taskInvolvedUserExpression(string involvedUserExpression)
	  {
		ensureNotNull("Involved user expression", involvedUserExpression);
		expressions["taskInvolvedUser"] = involvedUserExpression;
		return this;
	  }

	  public virtual TaskQuery withCandidateGroups()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set withCandidateGroups() within 'or' query");
		}

		this.withCandidateGroups_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery withoutCandidateGroups()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set withoutCandidateGroups() within 'or' query");
		}

		this.withoutCandidateGroups_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery withCandidateUsers()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set withCandidateUsers() within 'or' query");
		}

		this.withCandidateUsers_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery withoutCandidateUsers()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set withoutCandidateUsers() within 'or' query");
		}

		this.withoutCandidateUsers_Conflict = true;
		return this;
	  }

	  public virtual TaskQueryImpl taskCandidateGroup(string candidateGroup)
	  {
		ensureNotNull("Candidate group", candidateGroup);

		if (!isOrQueryActive)
		{
		  if (!string.ReferenceEquals(candidateUser, null) || expressions.ContainsKey("taskCandidateUser"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateGroup and candidateUser");
		  }
		}

		this.candidateGroup = candidateGroup;
		expressions.Remove("taskCandidateGroup");
		return this;
	  }

	  public virtual TaskQuery taskCandidateGroupExpression(string candidateGroupExpression)
	  {
		ensureNotNull("Candidate group expression", candidateGroupExpression);

		if (!isOrQueryActive)
		{
		  if (!string.ReferenceEquals(candidateUser, null) || expressions.ContainsKey("taskCandidateUser"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateGroup and candidateUser");
		  }
		}

		expressions["taskCandidateGroup"] = candidateGroupExpression;
		return this;
	  }

	  public virtual TaskQuery taskCandidateGroupIn(IList<string> candidateGroups)
	  {
		ensureNotEmpty("Candidate group list", candidateGroups);

		if (!isOrQueryActive)
		{
		  if (!string.ReferenceEquals(candidateUser, null) || expressions.ContainsKey("taskCandidateUser"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateGroupIn and candidateUser");
		  }
		}

		this.candidateGroups = candidateGroups;
		expressions.Remove("taskCandidateGroupIn");
		return this;
	  }

	  public virtual TaskQuery taskCandidateGroupInExpression(string candidateGroupsExpression)
	  {
		ensureNotEmpty("Candidate group list expression", candidateGroupsExpression);

		if (!isOrQueryActive)
		{
		  if (!string.ReferenceEquals(candidateUser, null) || expressions.ContainsKey("taskCandidateUser"))
		  {
			throw new ProcessEngineException("Invalid query usage: cannot set both candidateGroupIn and candidateUser");
		  }
		}

		expressions["taskCandidateGroupIn"] = candidateGroupsExpression;
		return this;
	  }

	  public virtual TaskQuery includeAssignedTasks()
	  {
		if (string.ReferenceEquals(candidateUser, null) && string.ReferenceEquals(candidateGroup, null) && candidateGroups == null && !WithCandidateGroups && !WithoutCandidateGroups && !WithCandidateUsers && !WithoutCandidateUsers && !expressions.ContainsKey("taskCandidateUser") && !expressions.ContainsKey("taskCandidateGroup") && !expressions.ContainsKey("taskCandidateGroupIn"))
		{
		  throw new ProcessEngineException("Invalid query usage: candidateUser, candidateGroup, candidateGroupIn, withCandidateGroups, withoutCandidateGroups, withCandidateUsers, withoutCandidateUsers has to be called before 'includeAssignedTasks'.");
		}

		includeAssignedTasks_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery includeAssignedTasksInternal()
	  {
		includeAssignedTasks_Conflict = true;
		return this;
	  }

	  public virtual TaskQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual TaskQueryImpl processInstanceBusinessKey(string processInstanceBusinessKey)
	  {
		this.processInstanceBusinessKey_Conflict = processInstanceBusinessKey;
		expressions.Remove("processInstanceBusinessKey");
		return this;
	  }

	  public virtual TaskQuery processInstanceBusinessKeyExpression(string processInstanceBusinessKeyExpression)
	  {
		ensureNotNull("processInstanceBusinessKey expression", processInstanceBusinessKeyExpression);
		expressions["processInstanceBusinessKey"] = processInstanceBusinessKeyExpression;
		return this;
	  }

	  public virtual TaskQuery processInstanceBusinessKeyIn(params string[] processInstanceBusinessKeys)
	  {
		this.processInstanceBusinessKeys = processInstanceBusinessKeys;
		return this;
	  }

	  public virtual TaskQuery processInstanceBusinessKeyLike(string processInstanceBusinessKey)
	  {
		this.processInstanceBusinessKeyLike_Conflict = processInstanceBusinessKey;
		expressions.Remove("processInstanceBusinessKeyLike");
		  return this;
	  }

	  public virtual TaskQuery processInstanceBusinessKeyLikeExpression(string processInstanceBusinessKeyLikeExpression)
	  {
		ensureNotNull("processInstanceBusinessKeyLike expression", processInstanceBusinessKeyLikeExpression);
		expressions["processInstanceBusinessKeyLike"] = processInstanceBusinessKeyLikeExpression;
		return this;
	  }

	  public virtual TaskQueryImpl executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual TaskQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		this.activityInstanceIdIn_Conflict = activityInstanceIds;
		return this;
	  }

	  public virtual TaskQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		this.isTenantIdSet = true;
		return this;
	  }

	  public virtual TaskQuery withoutTenantId()
	  {
		this.tenantIds = null;
		this.isTenantIdSet = true;
		return this;
	  }

	  public virtual TaskQueryImpl taskCreatedOn(DateTime createTime)
	  {
		this.createTime = createTime;
		expressions.Remove("taskCreatedOn");
		return this;
	  }

	  public virtual TaskQuery taskCreatedOnExpression(string createTimeExpression)
	  {
		expressions["taskCreatedOn"] = createTimeExpression;
		return this;
	  }

	  public virtual TaskQuery taskCreatedBefore(DateTime before)
	  {
		this.createTimeBefore = before;
		expressions.Remove("taskCreatedBefore");
		return this;
	  }

	  public virtual TaskQuery taskCreatedBeforeExpression(string beforeExpression)
	  {
		expressions["taskCreatedBefore"] = beforeExpression;
		return this;
	  }

	  public virtual TaskQuery taskCreatedAfter(DateTime after)
	  {
		this.createTimeAfter = after;
		expressions.Remove("taskCreatedAfter");
		return this;
	  }

	  public virtual TaskQuery taskCreatedAfterExpression(string afterExpression)
	  {
		expressions["taskCreatedAfter"] = afterExpression;
		return this;
	  }

	  public virtual TaskQuery taskDefinitionKey(string key)
	  {
		this.key = key;
		return this;
	  }

	  public virtual TaskQuery taskDefinitionKeyLike(string keyLike)
	  {
		this.keyLike = keyLike;
		return this;
	  }

	  public virtual TaskQuery taskDefinitionKeyIn(params string[] taskDefinitionKeys)
	  {
		this.taskDefinitionKeys = taskDefinitionKeys;
		  return this;
	  }

	  public virtual TaskQuery taskParentTaskId(string taskParentTaskId)
	  {
		this.parentTaskId = taskParentTaskId;
		return this;
	  }

	  public virtual TaskQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull("caseInstanceId", caseInstanceId);
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual TaskQuery caseInstanceBusinessKey(string caseInstanceBusinessKey)
	  {
		ensureNotNull("caseInstanceBusinessKey", caseInstanceBusinessKey);
		this.caseInstanceBusinessKey_Conflict = caseInstanceBusinessKey;
		return this;
	  }

	  public virtual TaskQuery caseInstanceBusinessKeyLike(string caseInstanceBusinessKeyLike)
	  {
		ensureNotNull("caseInstanceBusinessKeyLike", caseInstanceBusinessKeyLike);
		this.caseInstanceBusinessKeyLike_Conflict = caseInstanceBusinessKeyLike;
		return this;
	  }

	  public virtual TaskQuery caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull("caseExecutionId", caseExecutionId);
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual TaskQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull("caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual TaskQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		ensureNotNull("caseDefinitionKey", caseDefinitionKey);
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual TaskQuery caseDefinitionName(string caseDefinitionName)
	  {
		ensureNotNull("caseDefinitionName", caseDefinitionName);
		this.caseDefinitionName_Conflict = caseDefinitionName;
		return this;
	  }

	  public virtual TaskQuery caseDefinitionNameLike(string caseDefinitionNameLike)
	  {
		ensureNotNull("caseDefinitionNameLike", caseDefinitionNameLike);
		this.caseDefinitionNameLike_Conflict = caseDefinitionNameLike;
		return this;
	  }

	  public virtual TaskQuery taskVariableValueEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.EQUALS, true, false);
		return this;
	  }

	  public virtual TaskQuery taskVariableValueNotEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.NOT_EQUALS, true, false);
		return this;
	  }

	  public virtual TaskQuery taskVariableValueLike(string variableName, string variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LIKE, true, false);
		  return this;
	  }

	  public virtual TaskQuery taskVariableValueGreaterThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN, true, false);
		  return this;
	  }

	  public virtual TaskQuery taskVariableValueGreaterThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN_OR_EQUAL, true, false);
		  return this;
	  }

	  public virtual TaskQuery taskVariableValueLessThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN, true, false);
		  return this;
	  }

	  public virtual TaskQuery taskVariableValueLessThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN_OR_EQUAL, true, false);
		  return this;
	  }

	  public virtual TaskQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.EQUALS, false, true);
		return this;
	  }

	  public virtual TaskQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.NOT_EQUALS, false, true);
		return this;
	  }

	  public virtual TaskQuery processVariableValueLike(string variableName, string variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LIKE, false, true);
		  return this;
	  }

	  public virtual TaskQuery processVariableValueGreaterThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN, false, true);
		  return this;
	  }

	  public virtual TaskQuery processVariableValueGreaterThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN_OR_EQUAL, false, true);
		  return this;
	  }

	  public virtual TaskQuery processVariableValueLessThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN, false, true);
		  return this;
	  }

	  public virtual TaskQuery processVariableValueLessThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN_OR_EQUAL, false, true);
		  return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.EQUALS, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueNotEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.NOT_EQUALS, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueLike(string variableName, string variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LIKE, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueGreaterThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueGreaterThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN_OR_EQUAL, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueLessThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN, false, false);
		return this;
	  }

	  public virtual TaskQuery caseInstanceVariableValueLessThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN_OR_EQUAL, false, false);
		return this;
	  }

	  public virtual TaskQuery processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual TaskQuery processDefinitionKeyIn(params string[] processDefinitionKeys)
	  {
		this.processDefinitionKeys = processDefinitionKeys;
		return this;
	  }

	  public virtual TaskQuery processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual TaskQuery processDefinitionName(string processDefinitionName)
	  {
		this.processDefinitionName_Conflict = processDefinitionName;
		return this;
	  }

	  public virtual TaskQuery processDefinitionNameLike(string processDefinitionName)
	  {
		this.processDefinitionNameLike_Conflict = processDefinitionName;
		  return this;
	  }

	  public virtual TaskQuery dueDate(DateTime dueDate)
	  {
		this.dueDate_Conflict = dueDate;
		expressions.Remove("dueDate");
		return this;
	  }

	  public virtual TaskQuery dueDateExpression(string dueDateExpression)
	  {
		expressions["dueDate"] = dueDateExpression;
		return this;
	  }

	  public virtual TaskQuery dueBefore(DateTime dueBefore)
	  {
		this.dueBefore_Conflict = dueBefore;
		expressions.Remove("dueBefore");
		return this;
	  }

	  public virtual TaskQuery dueBeforeExpression(string dueDate)
	  {
		expressions["dueBefore"] = dueDate;
		return this;
	  }

	  public virtual TaskQuery dueAfter(DateTime dueAfter)
	  {
		this.dueAfter_Conflict = dueAfter;
		expressions.Remove("dueAfter");
		return this;
	  }

	  public virtual TaskQuery dueAfterExpression(string dueDateExpression)
	  {
		expressions["dueAfter"] = dueDateExpression;
		return this;
	  }

	  public virtual TaskQuery followUpDate(DateTime followUpDate)
	  {
		this.followUpDate_Conflict = followUpDate;
		expressions.Remove("followUpDate");
		return this;
	  }

	  public virtual TaskQuery followUpDateExpression(string followUpDateExpression)
	  {
		expressions["followUpDate"] = followUpDateExpression;
		return this;
	  }

	  public virtual TaskQuery followUpBefore(DateTime followUpBefore)
	  {
		this.followUpBefore_Conflict = followUpBefore;
		this.followUpNullAccepted = false;
		expressions.Remove("followUpBefore");
		return this;
	  }

	  public virtual TaskQuery followUpBeforeExpression(string followUpBeforeExpression)
	  {
		this.followUpNullAccepted = false;
		expressions["followUpBefore"] = followUpBeforeExpression;
		return this;
	  }

	  public virtual TaskQuery followUpBeforeOrNotExistent(DateTime followUpDate)
	  {
		this.followUpBefore_Conflict = followUpDate;
		this.followUpNullAccepted = true;
		expressions.Remove("followUpBeforeOrNotExistent");
		return this;
	  }

	  public virtual TaskQuery followUpBeforeOrNotExistentExpression(string followUpDateExpression)
	  {
		expressions["followUpBeforeOrNotExistent"] = followUpDateExpression;
		this.followUpNullAccepted = true;
		return this;
	  }

	  public virtual bool FollowUpNullAccepted
	  {
		  set
		  {
			this.followUpNullAccepted = value;
		  }
		  get
		  {
			return followUpNullAccepted;
		  }
	  }

	  public virtual TaskQuery followUpAfter(DateTime followUpAfter)
	  {
		this.followUpAfter_Conflict = followUpAfter;
		expressions.Remove("followUpAfter");
		return this;
	  }

	  public virtual TaskQuery followUpAfterExpression(string followUpAfterExpression)
	  {
		expressions["followUpAfter"] = followUpAfterExpression;
		return this;
	  }

	  public virtual TaskQuery excludeSubtasks()
	  {
		this.excludeSubtasks_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual TaskQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual TaskQuery initializeFormKeys()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set initializeFormKeys() within 'or' query");
		}

		this.initializeFormKeys_Conflict = true;
		return this;
	  }

	  public virtual TaskQuery taskNameCaseInsensitive()
	  {
		this.taskNameCaseInsensitive_Conflict = true;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(minPriority, priority, maxPriority) || CompareUtil.areNotInAscendingOrder(dueAfter_Conflict, dueDate_Conflict, dueBefore_Conflict) || CompareUtil.areNotInAscendingOrder(followUpAfter_Conflict, followUpDate_Conflict, followUpBefore_Conflict) || CompareUtil.areNotInAscendingOrder(createTimeAfter, createTime, createTimeBefore) || CompareUtil.elementIsNotContainedInArray(key, taskDefinitionKeys) || CompareUtil.elementIsNotContainedInArray(processDefinitionKey_Conflict, processDefinitionKeys) || CompareUtil.elementIsNotContainedInArray(processInstanceBusinessKey_Conflict, processInstanceBusinessKeys);
	  }

	  public virtual IList<string> CandidateGroups
	  {
		  get
		  {
    
			if (cachedCandidateGroups != null)
			{
			  return cachedCandidateGroups;
			}
    
			if (isOrQueryActive)
			{
    
			  if (!string.ReferenceEquals(candidateGroup, null))
			  {
				cachedCandidateGroups = new List<string>();
				cachedCandidateGroups.Add(candidateGroup);
    
				if (candidateGroups != null)
				{
				  ((IList<string>)cachedCandidateGroups).AddRange(candidateGroups);
				}
    
			  }
			  else if (candidateGroups != null)
			  {
				cachedCandidateGroups = candidateGroups;
			  }
    
			  return cachedCandidateGroups;
			}
    
			if (!string.ReferenceEquals(candidateGroup, null) && candidateGroups != null)
			{
			  //get intersection of candidateGroups and candidateGroup
			  cachedCandidateGroups = new List<string>(candidateGroups);
	//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
			  cachedCandidateGroups.retainAll(Arrays.asList(candidateGroup));
			}
			else if (!string.ReferenceEquals(candidateGroup, null))
			{
			  cachedCandidateGroups = Arrays.asList(candidateGroup);
			}
			else if (!string.ReferenceEquals(candidateUser, null))
			{
			  cachedCandidateGroups = getGroupsForCandidateUser(candidateUser);
			}
			else if (candidateGroups != null)
			{
			  cachedCandidateGroups = candidateGroups;
			}
    
			return cachedCandidateGroups;
		  }
	  }

	  public virtual bool? WithCandidateGroups
	  {
		  get
		  {
			if (withCandidateGroups_Conflict == null)
			{
			  return false;
			}
			else
			{
			  return withCandidateGroups_Conflict;
			}
		  }
	  }

	  public virtual bool? WithCandidateUsers
	  {
		  get
		  {
			if (withCandidateUsers_Conflict == null)
			{
			  return false;
			}
			else
			{
			  return withCandidateUsers_Conflict;
			}
		  }
	  }

	  public virtual bool? WithCandidateGroupsInternal
	  {
		  get
		  {
			return withCandidateGroups_Conflict;
		  }
	  }

	  public virtual bool? WithoutCandidateGroups
	  {
		  get
		  {
			if (withoutCandidateGroups_Conflict == null)
			{
			  return false;
			}
			else
			{
			  return withoutCandidateGroups_Conflict;
			}
		  }
	  }

	  public virtual bool? WithoutCandidateUsers
	  {
		  get
		  {
			if (withoutCandidateUsers_Conflict == null)
			{
			  return false;
			}
			else
			{
			  return withoutCandidateUsers_Conflict;
			}
		  }
	  }

	  public virtual bool? WithoutCandidateGroupsInternal
	  {
		  get
		  {
			return withoutCandidateGroups_Conflict;
		  }
	  }

	  public virtual IList<string> CandidateGroupsInternal
	  {
		  get
		  {
			return candidateGroups;
		  }
	  }

	  protected internal virtual IList<string> getGroupsForCandidateUser(string candidateUser)
	  {
		IList<Group> groups = Context.CommandContext.ReadOnlyIdentityProvider.createGroupQuery().groupMember(candidateUser).list();
		IList<string> groupIds = new List<string>();
		foreach (Group group in groups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	  protected internal virtual void ensureOrExpressionsEvaluated()
	  {
		// skips first query as it has already been evaluated
		for (int i = 1; i < queries.Count; i++)
		{
		  queries[i].validate();
		  queries[i].evaluateExpressions();
		}
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		VariableSerializers types = Context.ProcessEngineConfiguration.VariableSerializers;
		foreach (QueryVariableValue var in variables)
		{
		  var.initialize(types);
		}

		if (queries.Count > 0)
		{
		  foreach (TaskQueryImpl orQuery in queries)
		  {
			foreach (QueryVariableValue var in orQuery.variables)
			{
			  var.initialize(types);
			}
		  }
		}
	  }

	  public virtual void addVariable(string name, object value, QueryOperator @operator, bool isTaskVariable, bool isProcessInstanceVariable)
	  {
		ensureNotNull("name", name);

		if (value == null || isBoolean(value))
		{
		  // Null-values and booleans can only be used in EQUALS and NOT_EQUALS
		  switch (@operator)
		  {
		  case org.camunda.bpm.engine.impl.QueryOperator.GREATER_THAN:
			throw new ProcessEngineException("Booleans and null cannot be used in 'greater than' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LESS_THAN:
			throw new ProcessEngineException("Booleans and null cannot be used in 'less than' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.GREATER_THAN_OR_EQUAL:
			throw new ProcessEngineException("Booleans and null cannot be used in 'greater than or equal' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LESS_THAN_OR_EQUAL:
			throw new ProcessEngineException("Booleans and null cannot be used in 'less than or equal' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LIKE:
			throw new ProcessEngineException("Booleans and null cannot be used in 'like' condition");
		  default:
			break;
		  }
		}

		bool shouldMatchVariableValuesIgnoreCase = true.Equals(variableValuesIgnoreCase) && value != null && value.GetType().IsAssignableFrom(typeof(string));
		addVariable(new TaskQueryVariableValue(name, value, @operator, isTaskVariable, isProcessInstanceVariable, true.Equals(variableNamesIgnoreCase), shouldMatchVariableValuesIgnoreCase));
	  }

	  protected internal virtual void addVariable(TaskQueryVariableValue taskQueryVariableValue)
	  {
		variables.Add(taskQueryVariableValue);
	  }

	  private bool isBoolean(object value)
	  {
		  if (value == null)
		  {
			return false;
		  }
		  return value.GetType().IsAssignableFrom(typeof(Boolean)) || value.GetType().IsAssignableFrom(typeof(bool));
	  }

	  //ordering ////////////////////////////////////////////////////////////////

	  public virtual TaskQuery orderByTaskId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.TASK_ID);
	  }

	  public virtual TaskQuery orderByTaskName()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskName() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.NAME);
	  }

	  public virtual TaskQuery orderByTaskNameCaseInsensitive()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskNameCaseInsensitive() within 'or' query");
		}

		taskNameCaseInsensitive();
		return orderBy(TaskQueryProperty_Fields.NAME_CASE_INSENSITIVE);
	  }

	  public virtual TaskQuery orderByTaskDescription()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskDescription() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.DESCRIPTION);
	  }

	  public virtual TaskQuery orderByTaskPriority()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskPriority() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.PRIORITY);
	  }

	  public virtual TaskQuery orderByProcessInstanceId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByProcessInstanceId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.PROCESS_INSTANCE_ID);
	  }

	  public virtual TaskQuery orderByCaseInstanceId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByCaseInstanceId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.CASE_INSTANCE_ID);
	  }

	  public virtual TaskQuery orderByExecutionId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByExecutionId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.EXECUTION_ID);
	  }

	  public virtual TaskQuery orderByTenantId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTenantId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual TaskQuery orderByCaseExecutionId()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByCaseExecutionId() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.CASE_EXECUTION_ID);
	  }

	  public virtual TaskQuery orderByTaskAssignee()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskAssignee() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.ASSIGNEE);
	  }

	  public virtual TaskQuery orderByTaskCreateTime()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskCreateTime() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.CREATE_TIME);
	  }

	  public virtual TaskQuery orderByDueDate()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByDueDate() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.DUE_DATE);
	  }

	  public virtual TaskQuery orderByFollowUpDate()
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByFollowUpDate() within 'or' query");
		}

		return orderBy(TaskQueryProperty_Fields.FOLLOW_UP_DATE);
	  }

	  public virtual TaskQuery orderByProcessVariable(string variableName, ValueType valueType)
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByProcessVariable() within 'or' query");
		}

		ensureNotNull("variableName", variableName);
		ensureNotNull("valueType", valueType);

		orderBy(VariableOrderProperty.forProcessInstanceVariable(variableName, valueType));
		return this;
	  }

	  public virtual TaskQuery orderByExecutionVariable(string variableName, ValueType valueType)
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByExecutionVariable() within 'or' query");
		}

		ensureNotNull("variableName", variableName);
		ensureNotNull("valueType", valueType);

		orderBy(VariableOrderProperty.forExecutionVariable(variableName, valueType));
		return this;
	  }

	  public virtual TaskQuery orderByTaskVariable(string variableName, ValueType valueType)
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByTaskVariable() within 'or' query");
		}

		ensureNotNull("variableName", variableName);
		ensureNotNull("valueType", valueType);

		orderBy(VariableOrderProperty.forTaskVariable(variableName, valueType));
		return this;
	  }

	  public virtual TaskQuery orderByCaseExecutionVariable(string variableName, ValueType valueType)
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByCaseExecutionVariable() within 'or' query");
		}

		ensureNotNull("variableName", variableName);
		ensureNotNull("valueType", valueType);

		orderBy(VariableOrderProperty.forCaseExecutionVariable(variableName, valueType));
		return this;
	  }

	  public virtual TaskQuery orderByCaseInstanceVariable(string variableName, ValueType valueType)
	  {
		if (isOrQueryActive)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set orderByCaseInstanceVariable() within 'or' query");
		}

		ensureNotNull("variableName", variableName);
		ensureNotNull("valueType", valueType);

		orderBy(VariableOrderProperty.forCaseInstanceVariable(variableName, valueType));
		return this;
	  }

	  //results ////////////////////////////////////////////////////////////////

	  public override IList<Task> executeList(CommandContext commandContext, Page page)
	  {
		ensureOrExpressionsEvaluated();
		ensureVariablesInitialized();
		checkQueryOk();

		resetCachedCandidateGroups();

		//check if candidateGroup and candidateGroups intersect
		if (!string.ReferenceEquals(CandidateGroup, null) && CandidateGroupsInternal != null && CandidateGroups.Count == 0)
		{
		  return Collections.emptyList();
		}

		IList<Task> taskList = commandContext.TaskManager.findTasksByQueryCriteria(this);

		if (initializeFormKeys_Conflict)
		{
		  foreach (Task task in taskList)
		  {
			// initialize the form keys of the tasks
			((TaskEntity) task).initializeFormKey();
		  }
		}

		return taskList;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		ensureOrExpressionsEvaluated();
		ensureVariablesInitialized();
		checkQueryOk();

		resetCachedCandidateGroups();

		//check if candidateGroup and candidateGroups intersect
		if (!string.ReferenceEquals(CandidateGroup, null) && CandidateGroupsInternal != null && CandidateGroups.Count == 0)
		{
		  return 0;
		}
		return commandContext.TaskManager.findTaskCountByQueryCriteria(this);
	  }

	  protected internal virtual void resetCachedCandidateGroups()
	  {
		cachedCandidateGroups = null;
		for (int i = 1; i < queries.Count; i++)
		{
		  queries[i].cachedCandidateGroups = null;
		}
	  }

	  //getters ////////////////////////////////////////////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string NameNotEqual
	  {
		  get
		  {
			return nameNotEqual;
		  }
	  }

	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }

	  public virtual string NameNotLike
	  {
		  get
		  {
			return nameNotLike;
		  }
	  }

	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }

	  public virtual string AssigneeLike
	  {
		  get
		  {
			return assigneeLike;
		  }
	  }

	  public virtual string InvolvedUser
	  {
		  get
		  {
			return involvedUser;
		  }
	  }

	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
	  }

	  public virtual bool? Assigned
	  {
		  get
		  {
			if (assigned == null)
			{
			  return false;
			}
			else
			{
			  return assigned;
			}
		  }
	  }

	  public virtual bool? AssignedInternal
	  {
		  get
		  {
			return assigned;
		  }
	  }

	  public virtual bool Unassigned
	  {
		  get
		  {
			if (unassigned == null)
			{
			  return false;
			}
			else
			{
			  return unassigned.Value;
			}
		  }
	  }

	  public virtual bool? UnassignedInternal
	  {
		  get
		  {
			return unassigned;
		  }
	  }

	  public virtual DelegationState DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
	  }

	  public virtual bool NoDelegationState
	  {
		  get
		  {
			return noDelegationState;
		  }
	  }

	  public virtual string DelegationStateString
	  {
		  get
		  {
			return (delegationState != null ? delegationState.ToString() : null);
		  }
	  }

	  public virtual string CandidateUser
	  {
		  get
		  {
			return candidateUser;
		  }
	  }

	  public virtual string CandidateGroup
	  {
		  get
		  {
			return candidateGroup;
		  }
	  }

	  public virtual bool IncludeAssignedTasks
	  {
		  get
		  {
			return includeAssignedTasks_Conflict != null ? includeAssignedTasks_Conflict.Value : false;
		  }
	  }

	  public virtual bool? IncludeAssignedTasksInternal
	  {
		  get
		  {
			return includeAssignedTasks_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string[] ActivityInstanceIdIn
	  {
		  get
		  {
			return activityInstanceIdIn_Conflict;
		  }
	  }

	  public virtual string[] TenantIds
	  {
		  get
		  {
			return tenantIds;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual string DescriptionLike
	  {
		  get
		  {
			return descriptionLike;
		  }
	  }

	  public virtual int? Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual int? MinPriority
	  {
		  get
		  {
			return minPriority;
		  }
	  }

	  public virtual int? MaxPriority
	  {
		  get
		  {
			return maxPriority;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	  public virtual DateTime CreateTimeBefore
	  {
		  get
		  {
			return createTimeBefore;
		  }
	  }

	  public virtual DateTime CreateTimeAfter
	  {
		  get
		  {
			return createTimeAfter;
		  }
	  }

	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
	  }

	  public virtual string[] Keys
	  {
		  get
		  {
			return taskDefinitionKeys;
		  }
	  }

	  public virtual string KeyLike
	  {
		  get
		  {
			return keyLike;
		  }
	  }

	  public virtual string ParentTaskId
	  {
		  get
		  {
			return parentTaskId;
		  }
	  }

	  public virtual IList<TaskQueryVariableValue> Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string[] ProcessDefinitionKeys
	  {
		  get
		  {
			return processDefinitionKeys;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionNameLike
	  {
		  get
		  {
			return processDefinitionNameLike_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceBusinessKey
	  {
		  get
		  {
			return processInstanceBusinessKey_Conflict;
		  }
	  }

	  public virtual string[] ProcessInstanceBusinessKeys
	  {
		  get
		  {
			return processInstanceBusinessKeys;
		  }
	  }

	  public virtual string ProcessInstanceBusinessKeyLike
	  {
		  get
		  {
			return processInstanceBusinessKeyLike_Conflict;
		  }
	  }

	  public virtual DateTime DueDate
	  {
		  get
		  {
			return dueDate_Conflict;
		  }
	  }

	  public virtual DateTime DueBefore
	  {
		  get
		  {
			return dueBefore_Conflict;
		  }
	  }

	  public virtual DateTime DueAfter
	  {
		  get
		  {
			return dueAfter_Conflict;
		  }
	  }

	  public virtual DateTime FollowUpDate
	  {
		  get
		  {
			return followUpDate_Conflict;
		  }
	  }

	  public virtual DateTime FollowUpBefore
	  {
		  get
		  {
			return followUpBefore_Conflict;
		  }
	  }

	  public virtual DateTime FollowUpAfter
	  {
		  get
		  {
			return followUpAfter_Conflict;
		  }
	  }

	  public virtual bool ExcludeSubtasks
	  {
		  get
		  {
			return excludeSubtasks_Conflict;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseInstanceBusinessKey
	  {
		  get
		  {
			return caseInstanceBusinessKey_Conflict;
		  }
	  }

	  public virtual string CaseInstanceBusinessKeyLike
	  {
		  get
		  {
			return caseInstanceBusinessKeyLike_Conflict;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionNameLike
	  {
		  get
		  {
			return caseDefinitionNameLike_Conflict;
		  }
	  }

	  public virtual bool InitializeFormKeys
	  {
		  get
		  {
			return initializeFormKeys_Conflict;
		  }
	  }

	  public virtual bool TaskNameCaseInsensitive
	  {
		  get
		  {
			return taskNameCaseInsensitive_Conflict;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public virtual string[] TaskDefinitionKeys
	  {
		  get
		  {
			return taskDefinitionKeys;
		  }
	  }

	  public virtual bool IsTenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public virtual bool? VariableNamesIgnoreCase
	  {
		  get
		  {
			return variableNamesIgnoreCase;
		  }
	  }

	  public virtual bool? VariableValuesIgnoreCase
	  {
		  get
		  {
			return variableValuesIgnoreCase;
		  }
	  }

	  public virtual IList<TaskQueryImpl> Queries
	  {
		  get
		  {
			return queries;
		  }
	  }

	  public virtual bool OrQueryActive
	  {
		  get
		  {
			return isOrQueryActive;
		  }
	  }

	  public virtual void addOrQuery(TaskQueryImpl orQuery)
	  {
		orQuery.isOrQueryActive = true;
		this.queries.Add(orQuery);
	  }

	  public virtual void setOrQueryActive()
	  {
		isOrQueryActive = true;
	  }

	  public override TaskQuery extend(TaskQuery extending)
	  {
		TaskQueryImpl extendingQuery = (TaskQueryImpl) extending;
		TaskQueryImpl extendedQuery = new TaskQueryImpl();

		// only add the base query's validators to the new query;
		// this is because the extending query's validators may not be applicable to the base
		// query and should therefore be executed before extending the query
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: extendedQuery.validators = new java.util.HashSet<Validator<AbstractQuery<?, ?>>>(validators);
		extendedQuery.validators = new HashSet<Validator<AbstractQuery<object, ?>>>(validators);

		if (!string.ReferenceEquals(extendingQuery.Name, null))
		{
		  extendedQuery.taskName(extendingQuery.Name);
		}
		else if (!string.ReferenceEquals(this.Name, null))
		{
		  extendedQuery.taskName(this.Name);
		}

		if (!string.ReferenceEquals(extendingQuery.NameLike, null))
		{
		  extendedQuery.taskNameLike(extendingQuery.NameLike);
		}
		else if (!string.ReferenceEquals(this.NameLike, null))
		{
		  extendedQuery.taskNameLike(this.NameLike);
		}

		if (!string.ReferenceEquals(extendingQuery.NameNotEqual, null))
		{
		  extendedQuery.taskNameNotEqual(extendingQuery.NameNotEqual);
		}
		else if (!string.ReferenceEquals(this.NameNotEqual, null))
		{
		  extendedQuery.taskNameNotEqual(this.NameNotEqual);
		}

		if (!string.ReferenceEquals(extendingQuery.NameNotLike, null))
		{
		  extendedQuery.taskNameNotLike(extendingQuery.NameNotLike);
		}
		else if (!string.ReferenceEquals(this.NameNotLike, null))
		{
		  extendedQuery.taskNameNotLike(this.NameNotLike);
		}

		if (!string.ReferenceEquals(extendingQuery.Assignee, null))
		{
		  extendedQuery.taskAssignee(extendingQuery.Assignee);
		}
		else if (!string.ReferenceEquals(this.Assignee, null))
		{
		  extendedQuery.taskAssignee(this.Assignee);
		}

		if (!string.ReferenceEquals(extendingQuery.AssigneeLike, null))
		{
		  extendedQuery.taskAssigneeLike(extendingQuery.AssigneeLike);
		}
		else if (!string.ReferenceEquals(this.AssigneeLike, null))
		{
		  extendedQuery.taskAssigneeLike(this.AssigneeLike);
		}

		if (!string.ReferenceEquals(extendingQuery.InvolvedUser, null))
		{
		  extendedQuery.taskInvolvedUser(extendingQuery.InvolvedUser);
		}
		else if (!string.ReferenceEquals(this.InvolvedUser, null))
		{
		  extendedQuery.taskInvolvedUser(this.InvolvedUser);
		}

		if (!string.ReferenceEquals(extendingQuery.Owner, null))
		{
		  extendedQuery.taskOwner(extendingQuery.Owner);
		}
		else if (!string.ReferenceEquals(this.Owner, null))
		{
		  extendedQuery.taskOwner(this.Owner);
		}

		if (extendingQuery.Assigned || this.Assigned)
		{
		  extendedQuery.taskAssigned();
		}

		if (extendingQuery.Unassigned || this.Unassigned)
		{
		  extendedQuery.taskUnassigned();
		}

		if (extendingQuery.DelegationState != null)
		{
		  extendedQuery.taskDelegationState(extendingQuery.DelegationState);
		}
		else if (this.DelegationState != null)
		{
		  extendedQuery.taskDelegationState(this.DelegationState);
		}

		if (!string.ReferenceEquals(extendingQuery.CandidateUser, null))
		{
		  extendedQuery.taskCandidateUser(extendingQuery.CandidateUser);
		}
		else if (!string.ReferenceEquals(this.CandidateUser, null))
		{
		  extendedQuery.taskCandidateUser(this.CandidateUser);
		}

		if (!string.ReferenceEquals(extendingQuery.CandidateGroup, null))
		{
		  extendedQuery.taskCandidateGroup(extendingQuery.CandidateGroup);
		}
		else if (!string.ReferenceEquals(this.CandidateGroup, null))
		{
		  extendedQuery.taskCandidateGroup(this.CandidateGroup);
		}

		if (extendingQuery.WithCandidateGroups || this.WithCandidateGroups)
		{
		  extendedQuery.withCandidateGroups();
		}

		if (extendingQuery.WithCandidateUsers || this.WithCandidateUsers)
		{
		  extendedQuery.withCandidateUsers();
		}

		if (extendingQuery.WithoutCandidateGroups || this.WithoutCandidateGroups)
		{
		  extendedQuery.withoutCandidateGroups();
		}

		if (extendingQuery.WithoutCandidateUsers || this.WithoutCandidateUsers)
		{
		  extendedQuery.withoutCandidateUsers();
		}

		if (extendingQuery.CandidateGroupsInternal != null)
		{
		  extendedQuery.taskCandidateGroupIn(extendingQuery.CandidateGroupsInternal);
		}
		else if (this.CandidateGroupsInternal != null)
		{
		  extendedQuery.taskCandidateGroupIn(this.CandidateGroupsInternal);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessInstanceId, null))
		{
		  extendedQuery.processInstanceId(extendingQuery.ProcessInstanceId);
		}
		else if (!string.ReferenceEquals(this.ProcessInstanceId, null))
		{
		  extendedQuery.processInstanceId(this.ProcessInstanceId);
		}

		if (!string.ReferenceEquals(extendingQuery.ExecutionId, null))
		{
		  extendedQuery.executionId(extendingQuery.ExecutionId);
		}
		else if (!string.ReferenceEquals(this.ExecutionId, null))
		{
		  extendedQuery.executionId(this.ExecutionId);
		}

		if (extendingQuery.ActivityInstanceIdIn != null)
		{
		  extendedQuery.activityInstanceIdIn(extendingQuery.ActivityInstanceIdIn);
		}
		else if (this.ActivityInstanceIdIn != null)
		{
		  extendedQuery.activityInstanceIdIn(this.ActivityInstanceIdIn);
		}

		if (!string.ReferenceEquals(extendingQuery.TaskId, null))
		{
		  extendedQuery.taskId(extendingQuery.TaskId);
		}
		else if (!string.ReferenceEquals(this.TaskId, null))
		{
		  extendedQuery.taskId(this.TaskId);
		}

		if (!string.ReferenceEquals(extendingQuery.Description, null))
		{
		  extendedQuery.taskDescription(extendingQuery.Description);
		}
		else if (!string.ReferenceEquals(this.Description, null))
		{
		  extendedQuery.taskDescription(this.Description);
		}

		if (!string.ReferenceEquals(extendingQuery.DescriptionLike, null))
		{
		  extendedQuery.taskDescriptionLike(extendingQuery.DescriptionLike);
		}
		else if (!string.ReferenceEquals(this.DescriptionLike, null))
		{
		  extendedQuery.taskDescriptionLike(this.DescriptionLike);
		}

		if (extendingQuery.Priority != null)
		{
		  extendedQuery.taskPriority(extendingQuery.Priority);
		}
		else if (this.Priority != null)
		{
		  extendedQuery.taskPriority(this.Priority);
		}

		if (extendingQuery.MinPriority != null)
		{
		  extendedQuery.taskMinPriority(extendingQuery.MinPriority);
		}
		else if (this.MinPriority != null)
		{
		  extendedQuery.taskMinPriority(this.MinPriority);
		}

		if (extendingQuery.MaxPriority != null)
		{
		  extendedQuery.taskMaxPriority(extendingQuery.MaxPriority);
		}
		else if (this.MaxPriority != null)
		{
		  extendedQuery.taskMaxPriority(this.MaxPriority);
		}

		if (extendingQuery.CreateTime != null)
		{
		  extendedQuery.taskCreatedOn(extendingQuery.CreateTime);
		}
		else if (this.CreateTime != null)
		{
		  extendedQuery.taskCreatedOn(this.CreateTime);
		}

		if (extendingQuery.CreateTimeBefore != null)
		{
		  extendedQuery.taskCreatedBefore(extendingQuery.CreateTimeBefore);
		}
		else if (this.CreateTimeBefore != null)
		{
		  extendedQuery.taskCreatedBefore(this.CreateTimeBefore);
		}

		if (extendingQuery.CreateTimeAfter != null)
		{
		  extendedQuery.taskCreatedAfter(extendingQuery.CreateTimeAfter);
		}
		else if (this.CreateTimeAfter != null)
		{
		  extendedQuery.taskCreatedAfter(this.CreateTimeAfter);
		}

		if (!string.ReferenceEquals(extendingQuery.Key, null))
		{
		  extendedQuery.taskDefinitionKey(extendingQuery.Key);
		}
		else if (!string.ReferenceEquals(this.Key, null))
		{
		  extendedQuery.taskDefinitionKey(this.Key);
		}

		if (!string.ReferenceEquals(extendingQuery.KeyLike, null))
		{
		  extendedQuery.taskDefinitionKeyLike(extendingQuery.KeyLike);
		}
		else if (!string.ReferenceEquals(this.KeyLike, null))
		{
		  extendedQuery.taskDefinitionKeyLike(this.KeyLike);
		}

		if (extendingQuery.Keys != null)
		{
		  extendedQuery.taskDefinitionKeyIn(extendingQuery.Keys);
		}
		else if (this.Keys != null)
		{
		  extendedQuery.taskDefinitionKeyIn(this.Keys);
		}

		if (!string.ReferenceEquals(extendingQuery.ParentTaskId, null))
		{
		  extendedQuery.taskParentTaskId(extendingQuery.ParentTaskId);
		}
		else if (!string.ReferenceEquals(this.ParentTaskId, null))
		{
		  extendedQuery.taskParentTaskId(this.ParentTaskId);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessDefinitionKey, null))
		{
		  extendedQuery.processDefinitionKey(extendingQuery.ProcessDefinitionKey);
		}
		else if (!string.ReferenceEquals(this.ProcessDefinitionKey, null))
		{
		  extendedQuery.processDefinitionKey(this.ProcessDefinitionKey);
		}

		if (extendingQuery.ProcessDefinitionKeys != null)
		{
		  extendedQuery.processDefinitionKeyIn(extendingQuery.ProcessDefinitionKeys);
		}
		else if (this.ProcessDefinitionKeys != null)
		{
		  extendedQuery.processDefinitionKeyIn(this.ProcessDefinitionKeys);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessDefinitionId, null))
		{
		  extendedQuery.processDefinitionId(extendingQuery.ProcessDefinitionId);
		}
		else if (!string.ReferenceEquals(this.ProcessDefinitionId, null))
		{
		  extendedQuery.processDefinitionId(this.ProcessDefinitionId);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessDefinitionName, null))
		{
		  extendedQuery.processDefinitionName(extendingQuery.ProcessDefinitionName);
		}
		else if (!string.ReferenceEquals(this.ProcessDefinitionName, null))
		{
		  extendedQuery.processDefinitionName(this.ProcessDefinitionName);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessDefinitionNameLike, null))
		{
		  extendedQuery.processDefinitionNameLike(extendingQuery.ProcessDefinitionNameLike);
		}
		else if (!string.ReferenceEquals(this.ProcessDefinitionNameLike, null))
		{
		  extendedQuery.processDefinitionNameLike(this.ProcessDefinitionNameLike);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessInstanceBusinessKey, null))
		{
		  extendedQuery.processInstanceBusinessKey(extendingQuery.ProcessInstanceBusinessKey);
		}
		else if (!string.ReferenceEquals(this.ProcessInstanceBusinessKey, null))
		{
		  extendedQuery.processInstanceBusinessKey(this.ProcessInstanceBusinessKey);
		}

		if (!string.ReferenceEquals(extendingQuery.ProcessInstanceBusinessKeyLike, null))
		{
		  extendedQuery.processInstanceBusinessKeyLike(extendingQuery.ProcessInstanceBusinessKeyLike);
		}
		else if (!string.ReferenceEquals(this.ProcessInstanceBusinessKeyLike, null))
		{
		  extendedQuery.processInstanceBusinessKeyLike(this.ProcessInstanceBusinessKeyLike);
		}

		if (extendingQuery.DueDate != null)
		{
		  extendedQuery.dueDate(extendingQuery.DueDate);
		}
		else if (this.DueDate != null)
		{
		  extendedQuery.dueDate(this.DueDate);
		}

		if (extendingQuery.DueBefore != null)
		{
		  extendedQuery.dueBefore(extendingQuery.DueBefore);
		}
		else if (this.DueBefore != null)
		{
		  extendedQuery.dueBefore(this.DueBefore);
		}

		if (extendingQuery.DueAfter != null)
		{
		  extendedQuery.dueAfter(extendingQuery.DueAfter);
		}
		else if (this.DueAfter != null)
		{
		  extendedQuery.dueAfter(this.DueAfter);
		}

		if (extendingQuery.FollowUpDate != null)
		{
		  extendedQuery.followUpDate(extendingQuery.FollowUpDate);
		}
		else if (this.FollowUpDate != null)
		{
		  extendedQuery.followUpDate(this.FollowUpDate);
		}

		if (extendingQuery.FollowUpBefore != null)
		{
		  extendedQuery.followUpBefore(extendingQuery.FollowUpBefore);
		}
		else if (this.FollowUpBefore != null)
		{
		  extendedQuery.followUpBefore(this.FollowUpBefore);
		}

		if (extendingQuery.FollowUpAfter != null)
		{
		  extendedQuery.followUpAfter(extendingQuery.FollowUpAfter);
		}
		else if (this.FollowUpAfter != null)
		{
		  extendedQuery.followUpAfter(this.FollowUpAfter);
		}

		if (extendingQuery.FollowUpNullAccepted || this.FollowUpNullAccepted)
		{
		  extendedQuery.FollowUpNullAccepted = true;
		}

		if (extendingQuery.ExcludeSubtasks || this.ExcludeSubtasks)
		{
		  extendedQuery.excludeSubtasks();
		}

		if (extendingQuery.SuspensionState != null)
		{
		  if (extendingQuery.SuspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE))
		  {
			extendedQuery.active();
		  }
		  else if (extendingQuery.SuspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED))
		  {
			extendedQuery.suspended();
		  }
		}
		else if (this.SuspensionState != null)
		{
		  if (this.SuspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE))
		  {
			extendedQuery.active();
		  }
		  else if (this.SuspensionState.Equals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED))
		  {
			extendedQuery.suspended();
		  }
		}

		if (!string.ReferenceEquals(extendingQuery.CaseInstanceId, null))
		{
		  extendedQuery.caseInstanceId(extendingQuery.CaseInstanceId);
		}
		else if (!string.ReferenceEquals(this.CaseInstanceId, null))
		{
		  extendedQuery.caseInstanceId(this.CaseInstanceId);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseInstanceBusinessKey, null))
		{
		  extendedQuery.caseInstanceBusinessKey(extendingQuery.CaseInstanceBusinessKey);
		}
		else if (!string.ReferenceEquals(this.CaseInstanceBusinessKey, null))
		{
		  extendedQuery.caseInstanceBusinessKey(this.CaseInstanceBusinessKey);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseInstanceBusinessKeyLike, null))
		{
		  extendedQuery.caseInstanceBusinessKeyLike(extendingQuery.CaseInstanceBusinessKeyLike);
		}
		else if (!string.ReferenceEquals(this.CaseInstanceBusinessKeyLike, null))
		{
		  extendedQuery.caseInstanceBusinessKeyLike(this.CaseInstanceBusinessKeyLike);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseExecutionId, null))
		{
		  extendedQuery.caseExecutionId(extendingQuery.CaseExecutionId);
		}
		else if (!string.ReferenceEquals(this.CaseExecutionId, null))
		{
		  extendedQuery.caseExecutionId(this.CaseExecutionId);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseDefinitionId, null))
		{
		  extendedQuery.caseDefinitionId(extendingQuery.CaseDefinitionId);
		}
		else if (!string.ReferenceEquals(this.CaseDefinitionId, null))
		{
		  extendedQuery.caseDefinitionId(this.CaseDefinitionId);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseDefinitionKey, null))
		{
		  extendedQuery.caseDefinitionKey(extendingQuery.CaseDefinitionKey);
		}
		else if (!string.ReferenceEquals(this.CaseDefinitionKey, null))
		{
		  extendedQuery.caseDefinitionKey(this.CaseDefinitionKey);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseDefinitionName, null))
		{
		  extendedQuery.caseDefinitionName(extendingQuery.CaseDefinitionName);
		}
		else if (!string.ReferenceEquals(this.CaseDefinitionName, null))
		{
		  extendedQuery.caseDefinitionName(this.CaseDefinitionName);
		}

		if (!string.ReferenceEquals(extendingQuery.CaseDefinitionNameLike, null))
		{
		  extendedQuery.caseDefinitionNameLike(extendingQuery.CaseDefinitionNameLike);
		}
		else if (!string.ReferenceEquals(this.CaseDefinitionNameLike, null))
		{
		  extendedQuery.caseDefinitionNameLike(this.CaseDefinitionNameLike);
		}

		if (extendingQuery.InitializeFormKeys || this.InitializeFormKeys)
		{
		  extendedQuery.initializeFormKeys();
		}

		if (extendingQuery.TaskNameCaseInsensitive || this.TaskNameCaseInsensitive)
		{
		  extendedQuery.taskNameCaseInsensitive();
		}

		if (extendingQuery.TenantIdSet)
		{
		  if (extendingQuery.TenantIds != null)
		  {
			extendedQuery.tenantIdIn(extendingQuery.TenantIds);
		  }
		  else
		  {
			extendedQuery.withoutTenantId();
		  }
		}
		else if (this.TenantIdSet)
		{
		  if (this.TenantIds != null)
		  {
			extendedQuery.tenantIdIn(this.TenantIds);
		  }
		  else
		  {
			extendedQuery.withoutTenantId();
		  }
		}

		// merge variables
		mergeVariables(extendedQuery, extendingQuery);

		// merge expressions
		mergeExpressions(extendedQuery, extendingQuery);

		// include taskAssigned tasks has to be set after expression as it asserts on already set
		// candidate properties which could be expressions
		if (extendingQuery.IncludeAssignedTasks || this.IncludeAssignedTasks)
		{
		  extendedQuery.includeAssignedTasks();
		}

		mergeOrdering(extendedQuery, extendingQuery);

		extendedQuery.queries = new List<TaskQueryImpl>(Arrays.asList(extendedQuery));

		if (queries.Count > 1)
		{
		  queries.RemoveAt(0);
		  ((IList<TaskQueryImpl>)extendedQuery.queries).AddRange(queries);
		}

		if (extendingQuery.queries.Count > 1)
		{
		  extendingQuery.queries.RemoveAt(0);
		  ((IList<TaskQueryImpl>)extendedQuery.queries).AddRange(extendingQuery.queries);
		}

		return extendedQuery;
	  }

	  /// <summary>
	  /// Simple implementation of variable merging. Variables are only overridden if they have the same name and are
	  /// in the same scope (ie are process instance, task or case execution variables).
	  /// </summary>
	  protected internal virtual void mergeVariables(TaskQueryImpl extendedQuery, TaskQueryImpl extendingQuery)
	  {
		IList<TaskQueryVariableValue> extendingVariables = extendingQuery.Variables;

		ISet<TaskQueryVariableValueComparable> extendingVariablesComparable = new HashSet<TaskQueryVariableValueComparable>();

		// set extending variables and save names for comparison of original variables
		foreach (TaskQueryVariableValue extendingVariable in extendingVariables)
		{
		  extendedQuery.addVariable(extendingVariable);
		  extendingVariablesComparable.Add(new TaskQueryVariableValueComparable(this, extendingVariable));
		}

		foreach (TaskQueryVariableValue originalVariable in this.Variables)
		{
		  if (!extendingVariablesComparable.Contains(new TaskQueryVariableValueComparable(this, originalVariable)))
		  {
			extendedQuery.addVariable(originalVariable);
		  }
		}

	  }

	  protected internal class TaskQueryVariableValueComparable
	  {
		  private readonly TaskQueryImpl outerInstance;


		protected internal TaskQueryVariableValue variableValue;

		public TaskQueryVariableValueComparable(TaskQueryImpl outerInstance, TaskQueryVariableValue variableValue)
		{
			this.outerInstance = outerInstance;
		  this.variableValue = variableValue;
		}

		public virtual TaskQueryVariableValue VariableValue
		{
			get
			{
			  return variableValue;
			}
		}

		public override bool Equals(object o)
		{
		  if (this == o)
		  {
			  return true;
		  }
		  if (o == null || this.GetType() != o.GetType())
		  {
			  return false;
		  }

		  TaskQueryVariableValue other = ((TaskQueryVariableValueComparable) o).VariableValue;

		  return variableValue.Name.Equals(other.Name) && variableValue.ProcessInstanceVariable == other.ProcessInstanceVariable && variableValue.Local == other.Local;
		}

		public override int GetHashCode()
		{
		  int result = !string.ReferenceEquals(variableValue.Name, null) ? variableValue.Name.GetHashCode() : 0;
		  result = 31 * result + (variableValue.ProcessInstanceVariable ? 1 : 0);
		  result = 31 * result + (variableValue.Local ? 1 : 0);
		  return result;
		}

	  }


	  public virtual TaskQuery taskNameNotEqual(string name)
	  {
		this.nameNotEqual = name;
		return this;
	  }

	  public virtual TaskQuery taskNameNotLike(string nameNotLike)
	  {
		ensureNotNull("Task nameNotLike", nameNotLike);
		this.nameNotLike = nameNotLike;
		return this;
	  }

	  public virtual TaskQuery or()
	  {
		if (this != queries[0])
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set or() within 'or' query");
		}

		TaskQueryImpl orQuery = new TaskQueryImpl();
		orQuery.isOrQueryActive = true;
		orQuery.queries = queries;
		queries.Add(orQuery);
		return orQuery;
	  }

	  public virtual TaskQuery endOr()
	  {
		if (queries.Count > 0 && this != queries[queries.Count - 1])
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set endOr() before or()");
		}

		return queries[0];
	  }

	  public virtual TaskQuery matchVariableNamesIgnoreCase()
	  {
		this.variableNamesIgnoreCase = true;
		foreach (TaskQueryVariableValue variable in this.variables)
		{
		  variable.VariableNameIgnoreCase = true;
		}
		return this;
	  }

	  public virtual TaskQuery matchVariableValuesIgnoreCase()
	  {
		this.variableValuesIgnoreCase = true;
		foreach (TaskQueryVariableValue variable in this.variables)
		{
		  variable.VariableValueIgnoreCase = true;
		}
		return this;
	  }
	}
}