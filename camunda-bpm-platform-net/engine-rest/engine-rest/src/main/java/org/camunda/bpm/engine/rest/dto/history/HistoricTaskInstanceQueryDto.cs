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
namespace org.camunda.bpm.engine.rest.dto.history
{


	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricTaskInstanceQueryDto : AbstractQueryDto<HistoricTaskInstanceQuery>
	{

	  private const string SORT_BY_TASK_ID = "taskId";
	  private const string SORT_BY_ACT_INSTANCE_ID = "activityInstanceId";
	  private const string SORT_BY_PROC_DEF_ID = "processDefinitionId";
	  private const string SORT_BY_PROC_INST_ID = "processInstanceId";
	  private const string SORT_BY_EXEC_ID = "executionId";
	  private const string SORT_BY_CASE_DEF_ID = "caseDefinitionId";
	  private const string SORT_BY_CASE_INST_ID = "caseInstanceId";
	  private const string SORT_BY_CASE_EXEC_ID = "caseExecutionId";
	  private const string SORT_BY_TASK_DURATION = "duration";
	  private const string SORT_BY_END_TIME = "endTime";
	  private const string SORT_BY_START_TIME = "startTime";
	  private const string SORT_BY_TASK_NAME = "taskName";
	  private const string SORT_BY_TASK_DESC = "taskDescription";
	  private const string SORT_BY_ASSIGNEE = "assignee";
	  private const string SORT_BY_OWNER = "owner";
	  private const string SORT_BY_DUE_DATE = "dueDate";
	  private const string SORT_BY_FOLLOW_UP_DATE = "followUpDate";
	  private const string SORT_BY_DELETE_REASON = "deleteReason";
	  private const string SORT_BY_TASK_DEF_KEY = "taskDefinitionKey";
	  private const string SORT_BY_PRIORITY = "priority";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricTaskInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACT_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROC_DEF_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROC_INST_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXEC_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_DEF_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INST_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_EXEC_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_DURATION);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_DURATION);
		VALID_SORT_BY_VALUES.Add(SORT_BY_END_TIME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_START_TIME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_NAME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_DESC);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ASSIGNEE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_OWNER);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DUE_DATE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_FOLLOW_UP_DATE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DELETE_REASON);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_DEF_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PRIORITY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string taskId;
	  protected internal string taskParentTaskId;
	  protected internal string processInstanceId;
	  protected internal string processInstanceBusinessKey;
	  protected internal string[] processInstanceBusinessKeyIn;
	  protected internal string processInstanceBusinessKeyLike;
	  protected internal string executionId;
	  protected internal string[] activityInstanceIdIn;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionName;
	  protected internal string taskName;
	  protected internal string taskNameLike;
	  protected internal string taskDescription;
	  protected internal string taskDescriptionLike;
	  protected internal string taskDefinitionKey;
	  protected internal string[] taskDefinitionKeyIn;
	  protected internal string taskDeleteReason;
	  protected internal string taskDeleteReasonLike;
	  protected internal bool? assigned;
	  protected internal bool? unassigned;
	  protected internal string taskAssignee;
	  protected internal string taskAssigneeLike;
	  protected internal string taskOwner;
	  protected internal string taskOwnerLike;
	  protected internal int? taskPriority;
	  protected internal bool? finished;
	  protected internal bool? unfinished;
	  protected internal bool? processFinished;
	  protected internal bool? processUnfinished;
	  protected internal DateTime taskDueDate;
	  protected internal DateTime taskDueDateBefore;
	  protected internal DateTime taskDueDateAfter;
	  protected internal DateTime taskFollowUpDate;
	  protected internal DateTime taskFollowUpDateBefore;
	  protected internal DateTime taskFollowUpDateAfter;
	  private IList<string> tenantIds;

	  protected internal DateTime startedBefore;
	  protected internal DateTime startedAfter;
	  protected internal DateTime finishedBefore;
	  protected internal DateTime finishedAfter;

	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionName;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskInvolvedUser;
	  protected internal string taskInvolvedGroup;
	  protected internal string taskHadCandidateUser;
	  protected internal string taskHadCandidateGroup;
	  protected internal bool? withCandidateGroups;
	  protected internal bool? withoutCandidateGroups;
	  protected internal IList<VariableQueryParameterDto> taskVariables;
	  protected internal IList<VariableQueryParameterDto> processVariables;

	  public HistoricTaskInstanceQueryDto()
	  {
	  }

	  public HistoricTaskInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("taskId")]
	  public virtual string TaskId
	  {
		  set
		  {
			this.taskId = value;
		  }
	  }

	  [CamundaQueryParam("taskParentTaskId")]
	  public virtual string TaskParentTaskId
	  {
		  set
		  {
			this.taskParentTaskId = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKey")]
	  public virtual string ProcessInstanceBusinessKey
	  {
		  set
		  {
			this.processInstanceBusinessKey = value;
		  }
	  }

	  [CamundaQueryParam(value : "processInstanceBusinessKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessInstanceBusinessKeyIn
	  {
		  set
		  {
			this.processInstanceBusinessKeyIn = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKeyLike")]
	  public virtual string ProcessInstanceBusinessKeyLike
	  {
		  set
		  {
			this.processInstanceBusinessKeyLike = value;
		  }
	  }

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
	  }

	  [CamundaQueryParam(value:"activityInstanceIdIn", converter:org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityInstanceIdIn
	  {
		  set
		  {
			this.activityInstanceIdIn = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionKey")]
	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionName")]
	  public virtual string ProcessDefinitionName
	  {
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }

	  [CamundaQueryParam("taskName")]
	  public virtual string TaskName
	  {
		  set
		  {
			this.taskName = value;
		  }
	  }

	  [CamundaQueryParam("taskNameLike")]
	  public virtual string TaskNameLike
	  {
		  set
		  {
			this.taskNameLike = value;
		  }
	  }

	  [CamundaQueryParam("taskDescription")]
	  public virtual string TaskDescription
	  {
		  set
		  {
			this.taskDescription = value;
		  }
	  }

	  [CamundaQueryParam("taskDescriptionLike")]
	  public virtual string TaskDescriptionLike
	  {
		  set
		  {
			this.taskDescriptionLike = value;
		  }
	  }

	  [CamundaQueryParam("taskDefinitionKey")]
	  public virtual string TaskDefinitionKey
	  {
		  set
		  {
			this.taskDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskDefinitionKeyIn", converter:org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TaskDefinitionKeyIn
	  {
		  set
		  {
			this.taskDefinitionKeyIn = value;
		  }
	  }

	  [CamundaQueryParam("taskDeleteReason")]
	  public virtual string TaskDeleteReason
	  {
		  set
		  {
			this.taskDeleteReason = value;
		  }
	  }

	  [CamundaQueryParam("taskDeleteReasonLike")]
	  public virtual string TaskDeleteReasonLike
	  {
		  set
		  {
			this.taskDeleteReasonLike = value;
		  }
	  }

	  [CamundaQueryParam(value:"assigned", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Assigned
	  {
		  set
		  {
			this.assigned = value;
		  }
	  }

	  [CamundaQueryParam(value:"unassigned", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Unassigned
	  {
		  set
		  {
			this.unassigned = value;
		  }
	  }

	  [CamundaQueryParam("taskAssignee")]
	  public virtual string TaskAssignee
	  {
		  set
		  {
			this.taskAssignee = value;
		  }
	  }

	  [CamundaQueryParam("taskAssigneeLike")]
	  public virtual string TaskAssigneeLike
	  {
		  set
		  {
			this.taskAssigneeLike = value;
		  }
	  }

	  [CamundaQueryParam("taskOwner")]
	  public virtual string TaskOwner
	  {
		  set
		  {
			this.taskOwner = value;
		  }
	  }

	  [CamundaQueryParam("taskOwnerLike")]
	  public virtual string TaskOwnerLike
	  {
		  set
		  {
			this.taskOwnerLike = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskPriority", converter:org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? TaskPriority
	  {
		  set
		  {
			this.taskPriority = value;
		  }
	  }

	  [CamundaQueryParam(value:"finished", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Finished
	  {
		  set
		  {
			this.finished = value;
		  }
	  }

	  [CamundaQueryParam(value:"unfinished", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Unfinished
	  {
		  set
		  {
			this.unfinished = value;
		  }
	  }

	  [CamundaQueryParam(value:"processFinished", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? ProcessFinished
	  {
		  set
		  {
			this.processFinished = value;
		  }
	  }

	  [CamundaQueryParam(value:"processUnfinished", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? ProcessUnfinished
	  {
		  set
		  {
			this.processUnfinished = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskDueDate", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskDueDate
	  {
		  set
		  {
			this.taskDueDate = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskDueDateBefore", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskDueDateBefore
	  {
		  set
		  {
			this.taskDueDateBefore = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskDueDateAfter", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskDueDateAfter
	  {
		  set
		  {
			this.taskDueDateAfter = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskFollowUpDate", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskFollowUpDate
	  {
		  set
		  {
			this.taskFollowUpDate = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskFollowUpDateBefore", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskFollowUpDateBefore
	  {
		  set
		  {
			this.taskFollowUpDateBefore = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskFollowUpDateAfter", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime TaskFollowUpDateAfter
	  {
		  set
		  {
			this.taskFollowUpDateAfter = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> TaskVariables
	  {
		  set
		  {
			this.taskVariables = value;
		  }
	  }

	  [CamundaQueryParam(value:"processVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> ProcessVariables
	  {
		  set
		  {
			this.processVariables = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionId")]
	  public virtual string CaseDefinitionId
	  {
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionKey")]
	  public virtual string CaseDefinitionKey
	  {
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionName")]
	  public virtual string CaseDefinitionName
	  {
		  set
		  {
			this.caseDefinitionName = value;
		  }
	  }

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("caseExecutionId")]
	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> TenantIdIn
	  {
		  set
		  {
			this.tenantIds = value;
		  }
	  }

	  [CamundaQueryParam("taskInvolvedUser")]
	  public virtual string TaskInvolvedUser
	  {
		  set
		  {
			this.taskInvolvedUser = value;
		  }
	  }

	  [CamundaQueryParam("taskInvolvedGroup")]
	  public virtual string TaskInvolvedGroup
	  {
		  set
		  {
			this.taskInvolvedGroup = value;
		  }
	  }

	  [CamundaQueryParam("taskHadCandidateUser")]
	  public virtual string TaskHadCandidateUser
	  {
		  set
		  {
			this.taskHadCandidateUser = value;
		  }
	  }

	  [CamundaQueryParam("taskHadCandidateGroup")]
	  public virtual string TaskHadCandidateGroup
	  {
		  set
		  {
			this.taskHadCandidateGroup = value;
		  }
	  }

	  [CamundaQueryParam(value:"withCandidateGroups", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithCandidateGroups
	  {
		  set
		  {
			this.withCandidateGroups = value;
		  }
	  }

	  [CamundaQueryParam(value:"withoutCandidateGroups", converter:org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutCandidateGroups
	  {
		  set
		  {
			this.withoutCandidateGroups = value;
		  }
	  }

	  [CamundaQueryParam(value:"startedBefore", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedBefore
	  {
		  set
		  {
			this.startedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value:"startedAfter", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedAfter
	  {
		  set
		  {
			this.startedAfter = value;
		  }
	  }

	  [CamundaQueryParam(value:"finishedBefore", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FinishedBefore
	  {
		  set
		  {
			this.finishedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value:"finishedAfter", converter:org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FinishedAfter
	  {
		  set
		  {
			this.finishedAfter = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricTaskInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricTaskInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricTaskInstanceQuery query)
	  {
		if (!string.ReferenceEquals(taskId, null))
		{
		  query.taskId(taskId);
		}
		if (!string.ReferenceEquals(taskParentTaskId, null))
		{
		  query.taskParentTaskId(taskParentTaskId);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKey, null))
		{
		  query.processInstanceBusinessKey(processInstanceBusinessKey);
		}
		if (processInstanceBusinessKeyIn != null && processInstanceBusinessKeyIn.Length > 0)
		{
		  query.processInstanceBusinessKeyIn(processInstanceBusinessKeyIn);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKeyLike, null))
		{
		  query.processInstanceBusinessKeyLike(processInstanceBusinessKeyLike);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (activityInstanceIdIn != null && activityInstanceIdIn.Length > 0)
		{
		  query.activityInstanceIdIn(activityInstanceIdIn);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(processDefinitionName, null))
		{
		  query.processDefinitionName(processDefinitionName);
		}
		if (!string.ReferenceEquals(taskName, null))
		{
		  query.taskName(taskName);
		}
		if (!string.ReferenceEquals(taskNameLike, null))
		{
		  query.taskNameLike(taskNameLike);
		}
		if (!string.ReferenceEquals(taskDescription, null))
		{
		  query.taskDescription(taskDescription);
		}
		if (!string.ReferenceEquals(taskDescriptionLike, null))
		{
		  query.taskDescriptionLike(taskDescriptionLike);
		}
		if (!string.ReferenceEquals(taskDefinitionKey, null))
		{
		  query.taskDefinitionKey(taskDefinitionKey);
		}
		if (taskDefinitionKeyIn != null && taskDefinitionKeyIn.Length > 0)
		{
		  query.taskDefinitionKeyIn(taskDefinitionKeyIn);
		}
		if (!string.ReferenceEquals(taskDeleteReason, null))
		{
		  query.taskDeleteReason(taskDeleteReason);
		}
		if (!string.ReferenceEquals(taskDeleteReasonLike, null))
		{
		  query.taskDeleteReasonLike(taskDeleteReasonLike);
		}
		if (assigned != null)
		{
		  query.taskAssigned();
		}
		if (unassigned != null)
		{
		  query.taskUnassigned();
		}
		if (!string.ReferenceEquals(taskAssignee, null))
		{
		  query.taskAssignee(taskAssignee);
		}
		if (!string.ReferenceEquals(taskAssigneeLike, null))
		{
		  query.taskAssigneeLike(taskAssigneeLike);
		}
		if (!string.ReferenceEquals(taskOwner, null))
		{
		  query.taskOwner(taskOwner);
		}
		if (!string.ReferenceEquals(taskOwnerLike, null))
		{
		  query.taskOwnerLike(taskOwnerLike);
		}
		if (taskPriority != null)
		{
		  query.taskPriority(taskPriority);
		}
		if (finished != null)
		{
		  query.finished();
		}
		if (unfinished != null)
		{
		  query.unfinished();
		}
		if (processFinished != null)
		{
		  query.processFinished();
		}
		if (processUnfinished != null)
		{
		  query.processUnfinished();
		}
		if (taskDueDate != null)
		{
		  query.taskDueDate(taskDueDate);
		}
		if (taskDueDateBefore != null)
		{
		  query.taskDueBefore(taskDueDateBefore);
		}
		if (taskDueDateAfter != null)
		{
		  query.taskDueAfter(taskDueDateAfter);
		}
		if (taskFollowUpDate != null)
		{
		  query.taskFollowUpDate(taskFollowUpDate);
		}
		if (taskFollowUpDateBefore != null)
		{
		  query.taskFollowUpBefore(taskFollowUpDateBefore);
		}
		if (taskFollowUpDateAfter != null)
		{
		  query.taskFollowUpAfter(taskFollowUpDateAfter);
		}
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (!string.ReferenceEquals(caseDefinitionKey, null))
		{
		  query.caseDefinitionKey(caseDefinitionKey);
		}
		if (!string.ReferenceEquals(caseDefinitionName, null))
		{
		  query.caseDefinitionName(caseDefinitionName);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (!string.ReferenceEquals(taskInvolvedUser, null))
		{
		  query.taskInvolvedUser(taskInvolvedUser);
		}
		if (!string.ReferenceEquals(taskInvolvedGroup, null))
		{
		  query.taskInvolvedGroup(taskInvolvedGroup);
		}
		if (!string.ReferenceEquals(taskHadCandidateUser, null))
		{
		  query.taskHadCandidateUser(taskHadCandidateUser);
		}
		if (!string.ReferenceEquals(taskHadCandidateGroup, null))
		{
		  query.taskHadCandidateGroup(taskHadCandidateGroup);
		}
		if (withCandidateGroups != null)
		{
		  query.withCandidateGroups();
		}
		if (withoutCandidateGroups != null)
		{
		  query.withoutCandidateGroups();
		}

		if (finishedAfter != null)
		{
		  query.finishedAfter(finishedAfter);
		}

		if (finishedBefore != null)
		{
		  query.finishedBefore(finishedBefore);
		}

		if (startedAfter != null)
		{
		  query.startedAfter(startedAfter);
		}

		if (startedBefore != null)
		{
		  query.startedBefore(startedBefore);
		}

		if (taskVariables != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in taskVariables)
		  {
			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.taskVariableValueEquals(variableName, variableValue);
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid variable comparator specified: " + op);
			}
		  }
		}

		if (processVariables != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in processVariables)
		  {
			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueNotEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  query.processVariableValueGreaterThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueGreaterThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  query.processVariableValueLessThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueLessThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LIKE_OPERATOR_NAME))
			{
			  query.processVariableValueLike(variableName, variableValue.ToString());
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid process variable comparator specified: " + op);
			}
		  }
		}
	  }

	  protected internal override void applySortBy(HistoricTaskInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TASK_ID))
		{
		  query.orderByTaskId();
		}
		else if (sortBy.Equals(SORT_BY_ACT_INSTANCE_ID))
		{
		  query.orderByHistoricActivityInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROC_DEF_ID))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_PROC_INST_ID))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_EXEC_ID))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_TASK_DURATION))
		{
		  query.orderByHistoricTaskInstanceDuration();
		}
		else if (sortBy.Equals(SORT_BY_END_TIME))
		{
		  query.orderByHistoricTaskInstanceEndTime();
		}
		else if (sortBy.Equals(SORT_BY_START_TIME))
		{
		  query.orderByHistoricActivityInstanceStartTime();
		}
		else if (sortBy.Equals(SORT_BY_TASK_NAME))
		{
		  query.orderByTaskName();
		}
		else if (sortBy.Equals(SORT_BY_TASK_DESC))
		{
		  query.orderByTaskDescription();
		}
		else if (sortBy.Equals(SORT_BY_ASSIGNEE))
		{
		  query.orderByTaskAssignee();
		}
		else if (sortBy.Equals(SORT_BY_OWNER))
		{
		  query.orderByTaskOwner();
		}
		else if (sortBy.Equals(SORT_BY_DUE_DATE))
		{
		  query.orderByTaskDueDate();
		}
		else if (sortBy.Equals(SORT_BY_FOLLOW_UP_DATE))
		{
		  query.orderByTaskFollowUpDate();
		}
		else if (sortBy.Equals(SORT_BY_DELETE_REASON))
		{
		  query.orderByDeleteReason();
		}
		else if (sortBy.Equals(SORT_BY_TASK_DEF_KEY))
		{
		  query.orderByTaskDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_PRIORITY))
		{
		  query.orderByTaskPriority();
		}
		else if (sortBy.Equals(SORT_BY_CASE_DEF_ID))
		{
		  query.orderByCaseDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INST_ID))
		{
		  query.orderByCaseInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_EXEC_ID))
		{
		  query.orderByCaseExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}


}