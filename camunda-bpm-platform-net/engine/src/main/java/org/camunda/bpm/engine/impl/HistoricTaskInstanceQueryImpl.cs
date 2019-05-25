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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class HistoricTaskInstanceQueryImpl : AbstractQuery<HistoricTaskInstanceQuery, HistoricTaskInstance>, HistoricTaskInstanceQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceBusinessKey_Conflict;
	  protected internal string[] processInstanceBusinessKeys;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceBusinessKeyLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
	  protected internal string[] activityInstanceIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskNameLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskParentTaskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskDescription_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskDescriptionLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskDeleteReason_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskDeleteReasonLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskOwner_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskOwnerLike_Conflict;
	  protected internal bool? assigned;
	  protected internal bool? unassigned;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskAssignee_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskAssigneeLike_Conflict;
	  protected internal string[] taskDefinitionKeys;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskInvolvedUser_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskInvolvedGroup_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskHadCandidateUser_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskHadCandidateGroup_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withCandidateGroups_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? withoutCandidateGroups_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? taskPriority_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool finished_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool unfinished_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool processFinished_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool processUnfinished_Conflict;
	  protected internal IList<TaskQueryVariableValue> variables = new List<TaskQueryVariableValue>();
	  protected internal DateTime dueDate;
	  protected internal DateTime dueAfter;
	  protected internal DateTime dueBefore;
	  protected internal DateTime followUpDate;
	  protected internal DateTime followUpBefore;

	  protected internal DateTime followUpAfter;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedBefore_Conflict;

	  public HistoricTaskInstanceQueryImpl()
	  {
	  }

	  public HistoricTaskInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		return commandContext.HistoricTaskInstanceManager.findHistoricTaskInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricTaskInstance> executeList(CommandContext commandContext, Page page)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		return commandContext.HistoricTaskInstanceManager.findHistoricTaskInstancesByQueryCriteria(this, page);
	  }


	  public virtual HistoricTaskInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processInstanceBusinessKey(string processInstanceBusinessKey)
	  {
		this.processInstanceBusinessKey_Conflict = processInstanceBusinessKey;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processInstanceBusinessKeyIn(params string[] processInstanceBusinessKeys)
	  {
		ensureNotNull("processInstanceBusinessKeys", (object[]) processInstanceBusinessKeys);
		this.processInstanceBusinessKeys = processInstanceBusinessKeys;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processInstanceBusinessKeyLike(string processInstanceBusinessKey)
	  {
		this.processInstanceBusinessKeyLike_Conflict = processInstanceBusinessKey;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		ensureNotNull("activityInstanceIds", (object[]) activityInstanceIds);
		this.activityInstanceIds = activityInstanceIds;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionName(string processDefinitionName)
	  {
		this.processDefinitionName_Conflict = processDefinitionName;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskId(string taskId)
	  {
		this.taskId_Conflict = taskId;
		return this;
	  }
	  public virtual HistoricTaskInstanceQueryImpl taskName(string taskName)
	  {
		this.taskName_Conflict = taskName;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskNameLike(string taskNameLike)
	  {
		this.taskNameLike_Conflict = taskNameLike;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskParentTaskId(string parentTaskId)
	  {
		this.taskParentTaskId_Conflict = parentTaskId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskDescription(string taskDescription)
	  {
		this.taskDescription_Conflict = taskDescription;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskDescriptionLike(string taskDescriptionLike)
	  {
		this.taskDescriptionLike_Conflict = taskDescriptionLike;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskDeleteReason(string taskDeleteReason)
	  {
		this.taskDeleteReason_Conflict = taskDeleteReason;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskDeleteReasonLike(string taskDeleteReasonLike)
	  {
		this.taskDeleteReasonLike_Conflict = taskDeleteReasonLike;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskAssigned()
	  {
		this.assigned = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskUnassigned()
	  {
		this.unassigned = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskAssignee(string taskAssignee)
	  {
		this.taskAssignee_Conflict = taskAssignee;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskAssigneeLike(string taskAssigneeLike)
	  {
		this.taskAssigneeLike_Conflict = taskAssigneeLike;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskOwner(string taskOwner)
	  {
		this.taskOwner_Conflict = taskOwner;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskOwnerLike(string taskOwnerLike)
	  {
		this.taskOwnerLike_Conflict = taskOwnerLike;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery caseDefinitionName(string caseDefinitionName)
	  {
		this.caseDefinitionName_Conflict = caseDefinitionName;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl finished()
	  {
		this.finished_Conflict = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl unfinished()
	  {
		this.unfinished_Conflict = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl taskVariableValueEquals(string variableName, object variableValue)
	  {
		variables.Add(new TaskQueryVariableValue(variableName, variableValue, QueryOperator.EQUALS, true, false));
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		variables.Add(new TaskQueryVariableValue(variableName, variableValue, QueryOperator.EQUALS, false, true));
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.NOT_EQUALS, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLike(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LIKE, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueGreaterThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueGreaterThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.GREATER_THAN_OR_EQUAL, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLessThan(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLessThanOrEquals(string variableName, object variableValue)
	  {
		addVariable(variableName, variableValue, QueryOperator.LESS_THAN_OR_EQUAL, false, true);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDefinitionKey(string taskDefinitionKey)
	  {
		return taskDefinitionKeyIn(taskDefinitionKey);
	  }

	  public virtual HistoricTaskInstanceQuery taskDefinitionKeyIn(params string[] taskDefinitionKeys)
	  {
		ensureNotNull(typeof(NotValidException), "taskDefinitionKeys", (object[]) taskDefinitionKeys);
		this.taskDefinitionKeys = taskDefinitionKeys;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskPriority(int? taskPriority)
	  {
		this.taskPriority_Conflict = taskPriority;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processFinished()
	  {
		this.processFinished_Conflict = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskInvolvedUser(string userId)
	  {
		this.taskInvolvedUser_Conflict = userId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskInvolvedGroup(string groupId)
	  {
		this.taskInvolvedGroup_Conflict = groupId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskHadCandidateUser(string userId)
	  {
		this.taskHadCandidateUser_Conflict = userId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskHadCandidateGroup(string groupId)
	  {
		this.taskHadCandidateGroup_Conflict = groupId;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery withCandidateGroups()
	  {
		this.withCandidateGroups_Conflict = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery withoutCandidateGroups()
	  {
		this.withoutCandidateGroups_Conflict = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processUnfinished()
	  {
		this.processUnfinished_Conflict = true;
		return this;
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		VariableSerializers types = Context.ProcessEngineConfiguration.VariableSerializers;
		foreach (QueryVariableValue var in variables)
		{
		  var.initialize(types);
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
		addVariable(new TaskQueryVariableValue(name, value, @operator, isTaskVariable, isProcessInstanceVariable));
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

	  public virtual HistoricTaskInstanceQuery taskDueDate(DateTime dueDate)
	  {
		this.dueDate = dueDate;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDueAfter(DateTime dueAfter)
	  {
		this.dueAfter = dueAfter;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDueBefore(DateTime dueBefore)
	  {
		this.dueBefore = dueBefore;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskFollowUpDate(DateTime followUpDate)
	  {
		this.followUpDate = followUpDate;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskFollowUpBefore(DateTime followUpBefore)
	  {
		this.followUpBefore = followUpBefore;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskFollowUpAfter(DateTime followUpAfter)
	  {
		this.followUpAfter = followUpAfter;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery finishedAfter(DateTime date)
	  {
		finished_Conflict = true;
		this.finishedAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery finishedBefore(DateTime date)
	  {
		finished_Conflict = true;
		this.finishedBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery startedAfter(DateTime date)
	  {
		this.startedAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery startedBefore(DateTime date)
	  {
		this.startedBefore_Conflict = date;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || (finished_Conflict && unfinished_Conflict) || (processFinished_Conflict && processUnfinished_Conflict) || CompareUtil.areNotInAscendingOrder(startedAfter_Conflict, startedBefore_Conflict) || CompareUtil.areNotInAscendingOrder(finishedAfter_Conflict, finishedBefore_Conflict) || CompareUtil.areNotInAscendingOrder(dueAfter, dueDate, dueBefore) || CompareUtil.areNotInAscendingOrder(followUpAfter, followUpDate, followUpBefore) || CompareUtil.elementIsNotContainedInArray(processInstanceBusinessKey_Conflict, processInstanceBusinessKeys);
	  }

	  // ordering /////////////////////////////////////////////////////////////////

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.HISTORIC_TASK_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricActivityInstanceId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByProcessDefinitionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByProcessInstanceId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByExecutionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricTaskInstanceDuration()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.DURATION);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricTaskInstanceEndTime()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.END);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricActivityInstanceStartTime()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.START);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskName()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_NAME);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskDescription()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_DESCRIPTION);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskAssignee()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_ASSIGNEE);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskOwner()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_OWNER);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskDueDate()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_DUE_DATE);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskFollowUpDate()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_FOLLOW_UP_DATE);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByDeleteReason()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.DELETE_REASON);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskDefinitionKey()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_DEFINITION_KEY);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskPriority()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.TASK_PRIORITY);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByCaseDefinitionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.CASE_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByCaseInstanceId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.CASE_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByCaseExecutionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty_Fields.CASE_EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricTaskInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
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

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string[] ActivityInstanceIds
	  {
		  get
		  {
			return activityInstanceIds;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual bool? Assigned
	  {
		  get
		  {
			return assigned;
		  }
	  }

	  public virtual bool? Unassigned
	  {
		  get
		  {
			return unassigned;
		  }
	  }

	  public virtual bool? WithCandidateGroups
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
			return withoutCandidateGroups_Conflict;
		  }
	  }

	  public virtual bool Finished
	  {
		  get
		  {
			return finished_Conflict;
		  }
	  }

	  public virtual bool Unfinished
	  {
		  get
		  {
			return unfinished_Conflict;
		  }
	  }

	  public virtual string TaskName
	  {
		  get
		  {
			return taskName_Conflict;
		  }
	  }

	  public virtual string TaskNameLike
	  {
		  get
		  {
			return taskNameLike_Conflict;
		  }
	  }

	  public virtual string TaskDescription
	  {
		  get
		  {
			return taskDescription_Conflict;
		  }
	  }

	  public virtual string TaskDescriptionLike
	  {
		  get
		  {
			return taskDescriptionLike_Conflict;
		  }
	  }

	  public virtual string TaskDeleteReason
	  {
		  get
		  {
			return taskDeleteReason_Conflict;
		  }
	  }

	  public virtual string TaskDeleteReasonLike
	  {
		  get
		  {
			return taskDeleteReasonLike_Conflict;
		  }
	  }

	  public virtual string TaskAssignee
	  {
		  get
		  {
			return taskAssignee_Conflict;
		  }
	  }

	  public virtual string TaskAssigneeLike
	  {
		  get
		  {
			return taskAssigneeLike_Conflict;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string[] TaskDefinitionKeys
	  {
		  get
		  {
			return taskDefinitionKeys;
		  }
	  }

	  public virtual IList<TaskQueryVariableValue> Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual string TaskOwnerLike
	  {
		  get
		  {
			return taskOwnerLike_Conflict;
		  }
	  }

	  public virtual string TaskOwner
	  {
		  get
		  {
			return taskOwner_Conflict;
		  }
	  }

	  public virtual string TaskParentTaskId
	  {
		  get
		  {
			return taskParentTaskId_Conflict;
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

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Conflict;
		  }
	  }

	  public virtual DateTime FinishedAfter
	  {
		  get
		  {
			return finishedAfter_Conflict;
		  }
	  }

	  public virtual DateTime FinishedBefore
	  {
		  get
		  {
			return finishedBefore_Conflict;
		  }
	  }

	  public virtual DateTime StartedAfter
	  {
		  get
		  {
			return startedAfter_Conflict;
		  }
	  }

	  public virtual DateTime StartedBefore
	  {
		  get
		  {
			return startedBefore_Conflict;
		  }
	  }
	}

}