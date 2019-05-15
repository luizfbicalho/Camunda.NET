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
namespace org.camunda.bpm.engine.rest.dto.task
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;



	using QueryEntityRelationCondition = org.camunda.bpm.engine.impl.QueryEntityRelationCondition;
	using QueryOrderingProperty = org.camunda.bpm.engine.impl.QueryOrderingProperty;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using TaskQueryProperty = org.camunda.bpm.engine.impl.TaskQueryProperty;
	using TaskQueryVariableValue = org.camunda.bpm.engine.impl.TaskQueryVariableValue;
	using VariableInstanceQueryProperty = org.camunda.bpm.engine.impl.VariableInstanceQueryProperty;
	using VariableOrderProperty = org.camunda.bpm.engine.impl.VariableOrderProperty;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using Query = org.camunda.bpm.engine.query.Query;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using DelegationStateConverter = org.camunda.bpm.engine.rest.dto.converter.DelegationStateConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;

	using JsonInclude = com.fasterxml.jackson.annotation.JsonInclude;
	using Include = com.fasterxml.jackson.annotation.JsonInclude.Include;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonInclude(Include.NON_NULL) public class TaskQueryDto extends org.camunda.bpm.engine.rest.dto.AbstractQueryDto<org.camunda.bpm.engine.task.TaskQuery>
	public class TaskQueryDto : AbstractQueryDto<TaskQuery>
	{

	  public const string SORT_BY_PROCESS_INSTANCE_ID_VALUE = "instanceId";
	  public const string SORT_BY_CASE_INSTANCE_ID_VALUE = "caseInstanceId";
	  public const string SORT_BY_DUE_DATE_VALUE = "dueDate";
	  public const string SORT_BY_FOLLOW_UP_VALUE = "followUpDate";
	  public const string SORT_BY_EXECUTION_ID_VALUE = "executionId";
	  public const string SORT_BY_CASE_EXECUTION_ID_VALUE = "caseExecutionId";
	  public const string SORT_BY_ASSIGNEE_VALUE = "assignee";
	  public const string SORT_BY_CREATE_TIME_VALUE = "created";
	  public const string SORT_BY_DESCRIPTION_VALUE = "description";
	  public const string SORT_BY_ID_VALUE = "id";
	  public const string SORT_BY_NAME_VALUE = "name";
	  public const string SORT_BY_NAME_CASE_INSENSITIVE_VALUE = "nameCaseInsensitive";
	  public const string SORT_BY_PRIORITY_VALUE = "priority";
	  public const string SORT_BY_TENANT_ID_VALUE = "tenantId";

	  public const string SORT_BY_PROCESS_VARIABLE = "processVariable";
	  public const string SORT_BY_EXECUTION_VARIABLE = "executionVariable";
	  public const string SORT_BY_TASK_VARIABLE = "taskVariable";
	  public const string SORT_BY_CASE_INSTANCE_VARIABLE = "caseInstanceVariable";
	  public const string SORT_BY_CASE_EXECUTION_VARIABLE = "caseExecutionVariable";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;
	  static TaskQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DUE_DATE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_FOLLOW_UP_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ASSIGNEE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CREATE_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DESCRIPTION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_CASE_INSENSITIVE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PRIORITY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID_VALUE);
	  }

	  public const string SORT_PARAMETERS_VARIABLE_NAME = "variable";
	  public const string SORT_PARAMETERS_VALUE_TYPE = "type";

	  private string processInstanceBusinessKey;
	  private string processInstanceBusinessKeyExpression;
	  private string[] processInstanceBusinessKeyIn;
	  private string processInstanceBusinessKeyLike;
	  private string processInstanceBusinessKeyLikeExpression;
	  private string processDefinitionKey;
	  private string[] processDefinitionKeyIn;
	  private string processDefinitionId;
	  private string executionId;
	  private string[] activityInstanceIdIn;
	  private string processDefinitionName;
	  private string processDefinitionNameLike;
	  private string processInstanceId;
	  private string assignee;
	  private string assigneeExpression;
	  private string assigneeLike;
	  private string assigneeLikeExpression;
	  private string candidateGroup;
	  private string candidateGroupExpression;
	  private string candidateUser;
	  private string candidateUserExpression;
	  private bool? includeAssignedTasks;
	  private string taskDefinitionKey;
	  private string[] taskDefinitionKeyIn;
	  private string taskDefinitionKeyLike;
	  private string description;
	  private string descriptionLike;
	  private string involvedUser;
	  private string involvedUserExpression;
	  private int? maxPriority;
	  private int? minPriority;
	  private string name;
	  private string nameNotEqual;
	  private string nameLike;
	  private string nameNotLike;
	  private string owner;
	  private string ownerExpression;
	  private int? priority;
	  private string parentTaskId;
	  protected internal bool? assigned;
	  private bool? unassigned;
	  private bool? active;
	  private bool? suspended;

	  private string caseDefinitionKey;
	  private string caseDefinitionId;
	  private string caseDefinitionName;
	  private string caseDefinitionNameLike;
	  private string caseInstanceId;
	  private string caseInstanceBusinessKey;
	  private string caseInstanceBusinessKeyLike;
	  private string caseExecutionId;

	  private DateTime dueAfter;
	  private string dueAfterExpression;
	  private DateTime dueBefore;
	  private string dueBeforeExpression;
	  private DateTime dueDate;
	  private string dueDateExpression;
	  private DateTime followUpAfter;
	  private string followUpAfterExpression;
	  private DateTime followUpBefore;
	  private string followUpBeforeExpression;
	  private DateTime followUpBeforeOrNotExistent;
	  private string followUpBeforeOrNotExistentExpression;
	  private DateTime followUpDate;
	  private string followUpDateExpression;
	  private DateTime createdAfter;
	  private string createdAfterExpression;
	  private DateTime createdBefore;
	  private string createdBeforeExpression;
	  private DateTime createdOn;
	  private string createdOnExpression;

	  private string delegationState;

	  private string[] tenantIdIn;
	  private bool? withoutTenantId;

	  private IList<string> candidateGroups;
	  private string candidateGroupsExpression;
	  protected internal bool? withCandidateGroups;
	  protected internal bool? withoutCandidateGroups;
	  protected internal bool? withCandidateUsers;
	  protected internal bool? withoutCandidateUsers;

	  protected internal bool? variableNamesIgnoreCase;
	  protected internal bool? variableValuesIgnoreCase;

	  private IList<VariableQueryParameterDto> taskVariables;
	  private IList<VariableQueryParameterDto> processVariables;
	  private IList<VariableQueryParameterDto> caseInstanceVariables;

	  private IList<TaskQueryDto> orQueries;

	  public TaskQueryDto()
	  {

	  }

	  public TaskQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("orQueries")]
	  public virtual IList<TaskQueryDto> OrQueries
	  {
		  set
		  {
			this.orQueries = value;
		  }
		  get
		  {
			return orQueries;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKey")]
	  public virtual string ProcessInstanceBusinessKey
	  {
		  set
		  {
			this.processInstanceBusinessKey = value;
		  }
		  get
		  {
			return processInstanceBusinessKey;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKeyExpression")]
	  public virtual string ProcessInstanceBusinessKeyExpression
	  {
		  set
		  {
			this.processInstanceBusinessKeyExpression = value;
		  }
		  get
		  {
			return processInstanceBusinessKeyExpression;
		  }
	  }

	  [CamundaQueryParam(value : "processInstanceBusinessKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessInstanceBusinessKeyIn
	  {
		  set
		  {
			this.processInstanceBusinessKeyIn = value;
		  }
		  get
		  {
			return processInstanceBusinessKeyIn;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKeyLike")]
	  public virtual string ProcessInstanceBusinessKeyLike
	  {
		  set
		  {
			this.processInstanceBusinessKeyLike = value;
		  }
		  get
		  {
			return processInstanceBusinessKeyLike;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKeyLikeExpression")]
	  public virtual string ProcessInstanceBusinessKeyLikeExpression
	  {
		  set
		  {
			this.processInstanceBusinessKeyLikeExpression = value;
		  }
		  get
		  {
			return processInstanceBusinessKeyLikeExpression;
		  }
	  }

	  [CamundaQueryParam("processDefinitionKey")]
	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  [CamundaQueryParam(value : "processDefinitionKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessDefinitionKeyIn
	  {
		  set
		  {
			this.processDefinitionKeyIn = value;
		  }
		  get
		  {
			return processDefinitionKeyIn;
		  }
	  }

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
		  get
		  {
			return executionId;
		  }
	  }

	  [CamundaQueryParam(value:"activityInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityInstanceIdIn
	  {
		  set
		  {
			this.activityInstanceIdIn = value;
		  }
		  get
		  {
			return activityInstanceIdIn;
		  }
	  }

	  [CamundaQueryParam(value:"tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TenantIdIn
	  {
		  set
		  {
			this.tenantIdIn = value;
		  }
		  get
		  {
			return tenantIdIn;
		  }
	  }

	  [CamundaQueryParam(value : "withoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutTenantId
	  {
		  set
		  {
			this.withoutTenantId = value;
		  }
		  get
		  {
			return withoutTenantId;
		  }
	  }

	  [CamundaQueryParam("processDefinitionName")]
	  public virtual string ProcessDefinitionName
	  {
		  set
		  {
			this.processDefinitionName = value;
		  }
		  get
		  {
			return processDefinitionName;
		  }
	  }

	  [CamundaQueryParam("processDefinitionNameLike")]
	  public virtual string ProcessDefinitionNameLike
	  {
		  set
		  {
			this.processDefinitionNameLike = value;
		  }
		  get
		  {
			return processDefinitionNameLike;
		  }
	  }

	  [CamundaQueryParam("processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
		  get
		  {
			return processInstanceId;
		  }
	  }

	  [CamundaQueryParam("assignee")]
	  public virtual string Assignee
	  {
		  set
		  {
			this.assignee = value;
		  }
		  get
		  {
			return assignee;
		  }
	  }

	  [CamundaQueryParam("assigneeExpression")]
	  public virtual string AssigneeExpression
	  {
		  set
		  {
			this.assigneeExpression = value;
		  }
		  get
		  {
			return assigneeExpression;
		  }
	  }

	  [CamundaQueryParam("assigneeLike")]
	  public virtual string AssigneeLike
	  {
		  set
		  {
			this.assigneeLike = value;
		  }
		  get
		  {
			return assigneeLike;
		  }
	  }

	  [CamundaQueryParam("assigneeLikeExpression")]
	  public virtual string AssigneeLikeExpression
	  {
		  set
		  {
			this.assigneeLikeExpression = value;
		  }
		  get
		  {
			return assigneeLikeExpression;
		  }
	  }

	  [CamundaQueryParam("candidateGroup")]
	  public virtual string CandidateGroup
	  {
		  set
		  {
			this.candidateGroup = value;
		  }
		  get
		  {
			return candidateGroup;
		  }
	  }

	  [CamundaQueryParam("candidateGroupExpression")]
	  public virtual string CandidateGroupExpression
	  {
		  set
		  {
			this.candidateGroupExpression = value;
		  }
		  get
		  {
			return candidateGroupExpression;
		  }
	  }

	  [CamundaQueryParam(value : "withCandidateGroups", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithCandidateGroups
	  {
		  set
		  {
			this.withCandidateGroups = value;
		  }
	  }

	  [CamundaQueryParam(value : "withoutCandidateGroups", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutCandidateGroups
	  {
		  set
		  {
			this.withoutCandidateGroups = value;
		  }
	  }

	  [CamundaQueryParam(value : "withCandidateUsers", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithCandidateUsers
	  {
		  set
		  {
			this.withCandidateUsers = value;
		  }
	  }

	  [CamundaQueryParam(value : "withoutCandidateUsers", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutCandidateUsers
	  {
		  set
		  {
			this.withoutCandidateUsers = value;
		  }
	  }

	  [CamundaQueryParam("candidateUser")]
	  public virtual string CandidateUser
	  {
		  set
		  {
			this.candidateUser = value;
		  }
		  get
		  {
			return candidateUser;
		  }
	  }

	  [CamundaQueryParam("candidateUserExpression")]
	  public virtual string CandidateUserExpression
	  {
		  set
		  {
			this.candidateUserExpression = value;
		  }
		  get
		  {
			return candidateUserExpression;
		  }
	  }

	  [CamundaQueryParam(value : "includeAssignedTasks", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeAssignedTasks
	  {
		  set
		  {
			this.includeAssignedTasks = value;
		  }
		  get
		  {
			return includeAssignedTasks;
		  }
	  }

	  [CamundaQueryParam("taskDefinitionKey")]
	  public virtual string TaskDefinitionKey
	  {
		  set
		  {
			this.taskDefinitionKey = value;
		  }
		  get
		  {
			return taskDefinitionKey;
		  }
	  }

	  [CamundaQueryParam(value : "taskDefinitionKeyIn", converter: org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TaskDefinitionKeyIn
	  {
		  set
		  {
			this.taskDefinitionKeyIn = value;
		  }
		  get
		  {
			return taskDefinitionKeyIn;
		  }
	  }

	  [CamundaQueryParam("taskDefinitionKeyLike")]
	  public virtual string TaskDefinitionKeyLike
	  {
		  set
		  {
			this.taskDefinitionKeyLike = value;
		  }
		  get
		  {
			return taskDefinitionKeyLike;
		  }
	  }

	  [CamundaQueryParam("description")]
	  public virtual string Description
	  {
		  set
		  {
			this.description = value;
		  }
		  get
		  {
			return description;
		  }
	  }

	  [CamundaQueryParam("descriptionLike")]
	  public virtual string DescriptionLike
	  {
		  set
		  {
			this.descriptionLike = value;
		  }
		  get
		  {
			return descriptionLike;
		  }
	  }

	  [CamundaQueryParam("involvedUser")]
	  public virtual string InvolvedUser
	  {
		  set
		  {
			this.involvedUser = value;
		  }
		  get
		  {
			return involvedUser;
		  }
	  }

	  [CamundaQueryParam("involvedUserExpression")]
	  public virtual string InvolvedUserExpression
	  {
		  set
		  {
			this.involvedUserExpression = value;
		  }
		  get
		  {
			return involvedUserExpression;
		  }
	  }

	  [CamundaQueryParam(value : "maxPriority", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? MaxPriority
	  {
		  set
		  {
			this.maxPriority = value;
		  }
		  get
		  {
			return maxPriority;
		  }
	  }

	  [CamundaQueryParam(value : "minPriority", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? MinPriority
	  {
		  set
		  {
			this.minPriority = value;
		  }
		  get
		  {
			return minPriority;
		  }
	  }

	  [CamundaQueryParam("name")]
	  public virtual string Name
	  {
		  set
		  {
			this.name = value;
		  }
		  get
		  {
			return name;
		  }
	  }

	  [CamundaQueryParam("nameNotEqual")]
	  public virtual string NameNotEqual
	  {
		  set
		  {
			this.nameNotEqual = value;
		  }
		  get
		  {
			return nameNotEqual;
		  }
	  }

	  [CamundaQueryParam("nameLike")]
	  public virtual string NameLike
	  {
		  set
		  {
			this.nameLike = value;
		  }
		  get
		  {
			return nameLike;
		  }
	  }

	  [CamundaQueryParam("nameNotLike")]
	  public virtual string NameNotLike
	  {
		  set
		  {
			this.nameNotLike = value;
		  }
		  get
		  {
			return nameNotLike;
		  }
	  }

	  [CamundaQueryParam("owner")]
	  public virtual string Owner
	  {
		  set
		  {
			this.owner = value;
		  }
		  get
		  {
			return owner;
		  }
	  }

	  [CamundaQueryParam("ownerExpression")]
	  public virtual string OwnerExpression
	  {
		  set
		  {
			this.ownerExpression = value;
		  }
		  get
		  {
			return ownerExpression;
		  }
	  }

	  [CamundaQueryParam(value : "priority", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? Priority
	  {
		  set
		  {
			this.priority = value;
		  }
		  get
		  {
			return priority;
		  }
	  }

	  [CamundaQueryParam("parentTaskId")]
	  public virtual string ParentTaskId
	  {
		  set
		  {
			this.parentTaskId = value;
		  }
		  get
		  {
			return parentTaskId;
		  }
	  }

	  [CamundaQueryParam(value : "assigned", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Assigned
	  {
		  set
		  {
			this.assigned = value;
		  }
	  }

	  [CamundaQueryParam(value : "unassigned", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Unassigned
	  {
		  set
		  {
			this.unassigned = value;
		  }
		  get
		  {
			return unassigned;
		  }
	  }

	  [CamundaQueryParam(value : "active", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Active
	  {
		  set
		  {
			this.active = value;
		  }
		  get
		  {
			return active;
		  }
	  }

	  [CamundaQueryParam(value : "suspended", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Suspended
	  {
		  set
		  {
			this.suspended = value;
		  }
		  get
		  {
			return suspended;
		  }
	  }

	  [CamundaQueryParam(value : "dueAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DueAfter
	  {
		  set
		  {
			this.dueAfter = value;
		  }
		  get
		  {
			return dueAfter;
		  }
	  }

	  [CamundaQueryParam(value : "dueAfterExpression")]
	  public virtual string DueAfterExpression
	  {
		  set
		  {
			this.dueAfterExpression = value;
		  }
		  get
		  {
			return dueAfterExpression;
		  }
	  }

	  [CamundaQueryParam(value : "dueBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DueBefore
	  {
		  set
		  {
			this.dueBefore = value;
		  }
		  get
		  {
			return dueBefore;
		  }
	  }

	  [CamundaQueryParam(value : "dueBeforeExpression")]
	  public virtual string DueBeforeExpression
	  {
		  set
		  {
			this.dueBeforeExpression = value;
		  }
		  get
		  {
			return dueBeforeExpression;
		  }
	  }

	  [CamundaQueryParam(value : "dueDate", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DueDate
	  {
		  set
		  {
			this.dueDate = value;
		  }
		  get
		  {
			return dueDate;
		  }
	  }

	  [Obsolete, CamundaQueryParam(value : "due", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime Due
	  {
		  set
		  {
			this.dueDate = value;
		  }
	  }

	  [CamundaQueryParam(value : "dueDateExpression")]
	  public virtual string DueDateExpression
	  {
		  set
		  {
			this.dueDateExpression = value;
		  }
		  get
		  {
			return dueDateExpression;
		  }
	  }

	  [CamundaQueryParam(value : "followUpAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FollowUpAfter
	  {
		  set
		  {
			this.followUpAfter = value;
		  }
		  get
		  {
			return followUpAfter;
		  }
	  }

	  [CamundaQueryParam(value : "followUpAfterExpression")]
	  public virtual string FollowUpAfterExpression
	  {
		  set
		  {
			this.followUpAfterExpression = value;
		  }
		  get
		  {
			return followUpAfterExpression;
		  }
	  }

	  [CamundaQueryParam(value : "followUpBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FollowUpBefore
	  {
		  set
		  {
			this.followUpBefore = value;
		  }
		  get
		  {
			return followUpBefore;
		  }
	  }

	  [CamundaQueryParam(value : "followUpBeforeOrNotExistentExpression")]
	  public virtual string FollowUpBeforeOrNotExistentExpression
	  {
		  set
		  {
			this.followUpBeforeOrNotExistentExpression = value;
		  }
		  get
		  {
			return followUpBeforeOrNotExistentExpression;
		  }
	  }

	  [CamundaQueryParam(value : "followUpBeforeOrNotExistent", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FollowUpBeforeOrNotExistent
	  {
		  set
		  {
			this.followUpBeforeOrNotExistent = value;
		  }
		  get
		  {
			return followUpBeforeOrNotExistent;
		  }
	  }

	  [CamundaQueryParam(value : "followUpBeforeExpression")]
	  public virtual string FollowUpBeforeExpression
	  {
		  set
		  {
			this.followUpBeforeExpression = value;
		  }
		  get
		  {
			return followUpBeforeExpression;
		  }
	  }

	  [CamundaQueryParam(value : "followUpDate", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FollowUpDate
	  {
		  set
		  {
			this.followUpDate = value;
		  }
		  get
		  {
			return followUpDate;
		  }
	  }

	  [Obsolete, CamundaQueryParam(value : "followUp", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FollowUp
	  {
		  set
		  {
			this.followUpDate = value;
		  }
	  }

	  [CamundaQueryParam(value : "followUpDateExpression")]
	  public virtual string FollowUpDateExpression
	  {
		  set
		  {
			this.followUpDateExpression = value;
		  }
		  get
		  {
			return followUpDateExpression;
		  }
	  }

	  [CamundaQueryParam(value : "createdAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime CreatedAfter
	  {
		  set
		  {
			this.createdAfter = value;
		  }
		  get
		  {
			return createdAfter;
		  }
	  }

	  [CamundaQueryParam(value : "createdAfterExpression")]
	  public virtual string CreatedAfterExpression
	  {
		  set
		  {
			this.createdAfterExpression = value;
		  }
		  get
		  {
			return createdAfterExpression;
		  }
	  }

	  [CamundaQueryParam(value : "createdBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime CreatedBefore
	  {
		  set
		  {
			this.createdBefore = value;
		  }
		  get
		  {
			return createdBefore;
		  }
	  }

	  [CamundaQueryParam(value : "createdBeforeExpression")]
	  public virtual string CreatedBeforeExpression
	  {
		  set
		  {
			this.createdBeforeExpression = value;
		  }
		  get
		  {
			return createdBeforeExpression;
		  }
	  }

	  [CamundaQueryParam(value : "createdOn", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime CreatedOn
	  {
		  set
		  {
			this.createdOn = value;
		  }
		  get
		  {
			return createdOn;
		  }
	  }

	  [Obsolete, CamundaQueryParam(value : "created", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime Created
	  {
		  set
		  {
			this.createdOn = value;
		  }
	  }

	  [CamundaQueryParam(value : "createdOnExpression")]
	  public virtual string CreatedOnExpression
	  {
		  set
		  {
			this.createdOnExpression = value;
		  }
		  get
		  {
			return createdOnExpression;
		  }
	  }

	  [CamundaQueryParam(value : "delegationState")]
	  public virtual string DelegationState
	  {
		  set
		  {
			this.delegationState = value;
		  }
		  get
		  {
			return delegationState;
		  }
	  }

	  [CamundaQueryParam(value : "candidateGroups", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CandidateGroups
	  {
		  set
		  {
			this.candidateGroups = value;
		  }
		  get
		  {
			return candidateGroups;
		  }
	  }

	  [CamundaQueryParam(value : "candidateGroupsExpression")]
	  public virtual string CandidateGroupsExpression
	  {
		  set
		  {
			this.candidateGroupsExpression = value;
		  }
		  get
		  {
			return candidateGroupsExpression;
		  }
	  }

	  [CamundaQueryParam(value : "taskVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> TaskVariables
	  {
		  set
		  {
			this.taskVariables = value;
		  }
		  get
		  {
			return taskVariables;
		  }
	  }

	  [CamundaQueryParam(value : "processVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> ProcessVariables
	  {
		  set
		  {
			this.processVariables = value;
		  }
		  get
		  {
			return processVariables;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionId")]
	  public virtual string CaseDefinitionId
	  {
		  set
		  {
			this.caseDefinitionId = value;
		  }
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionKey")]
	  public virtual string CaseDefinitionKey
	  {
		  set
		  {
			this.caseDefinitionKey = value;
		  }
		  get
		  {
			return caseDefinitionKey;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionName")]
	  public virtual string CaseDefinitionName
	  {
		  set
		  {
			this.caseDefinitionName = value;
		  }
		  get
		  {
			return caseDefinitionName;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionNameLike")]
	  public virtual string CaseDefinitionNameLike
	  {
		  set
		  {
			this.caseDefinitionNameLike = value;
		  }
		  get
		  {
			return caseDefinitionNameLike;
		  }
	  }

	  [CamundaQueryParam("caseExecutionId")]
	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
		  }
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  [CamundaQueryParam("caseInstanceBusinessKey")]
	  public virtual string CaseInstanceBusinessKey
	  {
		  set
		  {
			this.caseInstanceBusinessKey = value;
		  }
		  get
		  {
			return caseInstanceBusinessKey;
		  }
	  }

	  [CamundaQueryParam("caseInstanceBusinessKeyLike")]
	  public virtual string CaseInstanceBusinessKeyLike
	  {
		  set
		  {
			this.caseInstanceBusinessKeyLike = value;
		  }
		  get
		  {
			return caseInstanceBusinessKeyLike;
		  }
	  }

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  [CamundaQueryParam(value : "caseInstanceVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> CaseInstanceVariables
	  {
		  set
		  {
			this.caseInstanceVariables = value;
		  }
		  get
		  {
			return caseInstanceVariables;
		  }
	  }

	  [CamundaQueryParam(value : "variableNamesIgnoreCase", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? VariableNamesIgnoreCase
	  {
		  set
		  {
			this.variableNamesIgnoreCase = value;
		  }
		  get
		  {
			return variableNamesIgnoreCase;
		  }
	  }

	  [CamundaQueryParam(value :"variableValuesIgnoreCase", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? VariableValuesIgnoreCase
	  {
		  set
		  {
			this.variableValuesIgnoreCase = value;
		  }
		  get
		  {
			return variableValuesIgnoreCase;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override TaskQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.TaskService.createTaskQuery();
	  }


















































































	  protected internal override void applyFilters(TaskQuery query)
	  {
		if (orQueries != null)
		{
		  foreach (TaskQueryDto orQueryDto in orQueries)
		  {
			TaskQueryImpl orQuery = new TaskQueryImpl();
			orQuery.setOrQueryActive();
			orQueryDto.applyFilters(orQuery);
			((TaskQueryImpl) query).addOrQuery(orQuery);
		  }
		}
		if (!string.ReferenceEquals(processInstanceBusinessKey, null))
		{
		  query.processInstanceBusinessKey(processInstanceBusinessKey);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKeyExpression, null))
		{
		  query.processInstanceBusinessKeyExpression(processInstanceBusinessKeyExpression);
		}
		if (processInstanceBusinessKeyIn != null && processInstanceBusinessKeyIn.Length > 0)
		{
		  query.processInstanceBusinessKeyIn(processInstanceBusinessKeyIn);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKeyLike, null))
		{
		  query.processInstanceBusinessKeyLike(processInstanceBusinessKeyLike);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKeyLikeExpression, null))
		{
		  query.processInstanceBusinessKeyLikeExpression(processInstanceBusinessKeyLikeExpression);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (processDefinitionKeyIn != null && processDefinitionKeyIn.Length > 0)
		{
		  query.processDefinitionKeyIn(processDefinitionKeyIn);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (activityInstanceIdIn != null && activityInstanceIdIn.Length > 0)
		{
		  query.activityInstanceIdIn(activityInstanceIdIn);
		}
		if (tenantIdIn != null && tenantIdIn.Length > 0)
		{
		  query.tenantIdIn(tenantIdIn);
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (!string.ReferenceEquals(processDefinitionName, null))
		{
		  query.processDefinitionName(processDefinitionName);
		}
		if (!string.ReferenceEquals(processDefinitionNameLike, null))
		{
		  query.processDefinitionNameLike(processDefinitionNameLike);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(assignee, null))
		{
		  query.taskAssignee(assignee);
		}
		if (!string.ReferenceEquals(assigneeExpression, null))
		{
		  query.taskAssigneeExpression(assigneeExpression);
		}
		if (!string.ReferenceEquals(assigneeLike, null))
		{
		  query.taskAssigneeLike(assigneeLike);
		}
		if (!string.ReferenceEquals(assigneeLikeExpression, null))
		{
		  query.taskAssigneeLikeExpression(assigneeLikeExpression);
		}
		if (!string.ReferenceEquals(candidateGroup, null))
		{
		  query.taskCandidateGroup(candidateGroup);
		}
		if (!string.ReferenceEquals(candidateGroupExpression, null))
		{
		  query.taskCandidateGroupExpression(candidateGroupExpression);
		}
		if (withCandidateGroups != null && withCandidateGroups)
		{
		  query.withCandidateGroups();
		}
		if (withoutCandidateGroups != null && withoutCandidateGroups)
		{
		  query.withoutCandidateGroups();
		}
		if (withCandidateUsers != null && withCandidateUsers)
		{
		  query.withCandidateUsers();
		}
		if (withoutCandidateUsers != null && withoutCandidateUsers)
		{
		  query.withoutCandidateUsers();
		}
		if (!string.ReferenceEquals(candidateUser, null))
		{
		  query.taskCandidateUser(candidateUser);
		}
		if (!string.ReferenceEquals(candidateUserExpression, null))
		{
		  query.taskCandidateUserExpression(candidateUserExpression);
		}
		if (taskDefinitionKeyIn != null && taskDefinitionKeyIn.Length > 0)
		{
		  query.taskDefinitionKeyIn(taskDefinitionKeyIn);
		}
		if (!string.ReferenceEquals(taskDefinitionKey, null))
		{
		  query.taskDefinitionKey(taskDefinitionKey);
		}
		if (!string.ReferenceEquals(taskDefinitionKeyLike, null))
		{
		  query.taskDefinitionKeyLike(taskDefinitionKeyLike);
		}
		if (!string.ReferenceEquals(description, null))
		{
		  query.taskDescription(description);
		}
		if (!string.ReferenceEquals(descriptionLike, null))
		{
		  query.taskDescriptionLike(descriptionLike);
		}
		if (!string.ReferenceEquals(involvedUser, null))
		{
		  query.taskInvolvedUser(involvedUser);
		}
		if (!string.ReferenceEquals(involvedUserExpression, null))
		{
		  query.taskInvolvedUserExpression(involvedUserExpression);
		}
		if (maxPriority != null)
		{
		  query.taskMaxPriority(maxPriority);
		}
		if (minPriority != null)
		{
		  query.taskMinPriority(minPriority);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.taskName(name);
		}
		if (!string.ReferenceEquals(nameNotEqual, null))
		{
		  query.taskNameNotEqual(nameNotEqual);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.taskNameLike(nameLike);
		}
		if (!string.ReferenceEquals(nameNotLike, null))
		{
		  query.taskNameNotLike(nameNotLike);
		}
		if (!string.ReferenceEquals(owner, null))
		{
		  query.taskOwner(owner);
		}
		if (!string.ReferenceEquals(ownerExpression, null))
		{
		  query.taskOwnerExpression(ownerExpression);
		}
		if (priority != null)
		{
		  query.taskPriority(priority);
		}
		if (!string.ReferenceEquals(parentTaskId, null))
		{
		  query.taskParentTaskId(parentTaskId);
		}
		if (assigned != null && assigned)
		{
		  query.taskAssigned();
		}
		if (unassigned != null && unassigned)
		{
		  query.taskUnassigned();
		}
		if (dueAfter != null)
		{
		  query.dueAfter(dueAfter);
		}
		if (!string.ReferenceEquals(dueAfterExpression, null))
		{
		  query.dueAfterExpression(dueAfterExpression);
		}
		if (dueBefore != null)
		{
		  query.dueBefore(dueBefore);
		}
		if (!string.ReferenceEquals(dueBeforeExpression, null))
		{
		  query.dueBeforeExpression(dueBeforeExpression);
		}
		if (dueDate != null)
		{
		  query.dueDate(dueDate);
		}
		if (!string.ReferenceEquals(dueDateExpression, null))
		{
		  query.dueDateExpression(dueDateExpression);
		}
		if (followUpAfter != null)
		{
		  query.followUpAfter(followUpAfter);
		}
		if (!string.ReferenceEquals(followUpAfterExpression, null))
		{
		  query.followUpAfterExpression(followUpAfterExpression);
		}
		if (followUpBefore != null)
		{
		  query.followUpBefore(followUpBefore);
		}
		if (!string.ReferenceEquals(followUpBeforeExpression, null))
		{
		  query.followUpBeforeExpression(followUpBeforeExpression);
		}
		if (followUpBeforeOrNotExistent != null)
		{
		  query.followUpBeforeOrNotExistent(followUpBeforeOrNotExistent);
		}
		if (!string.ReferenceEquals(followUpBeforeOrNotExistentExpression, null))
		{
		  query.followUpBeforeOrNotExistentExpression(followUpBeforeOrNotExistentExpression);
		}
		if (followUpDate != null)
		{
		  query.followUpDate(followUpDate);
		}
		if (!string.ReferenceEquals(followUpDateExpression, null))
		{
		  query.followUpDateExpression(followUpDateExpression);
		}
		if (createdAfter != null)
		{
		  query.taskCreatedAfter(createdAfter);
		}
		if (!string.ReferenceEquals(createdAfterExpression, null))
		{
		  query.taskCreatedAfterExpression(createdAfterExpression);
		}
		if (createdBefore != null)
		{
		  query.taskCreatedBefore(createdBefore);
		}
		if (!string.ReferenceEquals(createdBeforeExpression, null))
		{
		  query.taskCreatedBeforeExpression(createdBeforeExpression);
		}
		if (createdOn != null)
		{
		  query.taskCreatedOn(createdOn);
		}
		if (!string.ReferenceEquals(createdOnExpression, null))
		{
		  query.taskCreatedOnExpression(createdOnExpression);
		}
		if (!string.ReferenceEquals(delegationState, null))
		{
		  DelegationStateConverter converter = new DelegationStateConverter();
		  DelegationState state = converter.convertQueryParameterToType(delegationState);
		  query.taskDelegationState(state);
		}
		if (candidateGroups != null)
		{
		  query.taskCandidateGroupIn(candidateGroups);
		}
		if (!string.ReferenceEquals(candidateGroupsExpression, null))
		{
		  query.taskCandidateGroupInExpression(candidateGroupsExpression);
		}
		if (includeAssignedTasks != null && includeAssignedTasks)
		{
		  query.includeAssignedTasks();
		}
		if (active != null && active)
		{
		  query.active();
		}
		if (suspended != null && suspended)
		{
		  query.suspended();
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
		if (!string.ReferenceEquals(caseDefinitionNameLike, null))
		{
		  query.caseDefinitionNameLike(caseDefinitionNameLike);
		}
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}
		if (!string.ReferenceEquals(caseInstanceBusinessKey, null))
		{
		  query.caseInstanceBusinessKey(caseInstanceBusinessKey);
		}
		if (!string.ReferenceEquals(caseInstanceBusinessKeyLike, null))
		{
		  query.caseInstanceBusinessKeyLike(caseInstanceBusinessKeyLike);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (variableValuesIgnoreCase != null && variableValuesIgnoreCase)
		{
		  query.matchVariableValuesIgnoreCase();
		}
		if (variableNamesIgnoreCase != null && variableNamesIgnoreCase)
		{
		  query.matchVariableNamesIgnoreCase();
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
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.taskVariableValueNotEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  query.taskVariableValueGreaterThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.taskVariableValueGreaterThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  query.taskVariableValueLessThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.taskVariableValueLessThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LIKE_OPERATOR_NAME))
			{
			  query.taskVariableValueLike(variableName, variableValue.ToString());
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid task variable comparator specified: " + op);
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

		if (caseInstanceVariables != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in caseInstanceVariables)
		  {
			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueNotEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueGreaterThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueGreaterThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLessThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLessThanOrEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LIKE_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLike(variableName, variableValue.ToString());
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid case variable comparator specified: " + op);
			}
		  }
		}
	  }

	  protected internal override void applySortBy(TaskQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_ID_VALUE))
		{
		  query.orderByCaseInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_DUE_DATE_VALUE))
		{
		  query.orderByDueDate();
		}
		else if (sortBy.Equals(SORT_BY_FOLLOW_UP_VALUE))
		{
		  query.orderByFollowUpDate();
		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_ID_VALUE))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_EXECUTION_ID_VALUE))
		{
		  query.orderByCaseExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_ASSIGNEE_VALUE))
		{
		  query.orderByTaskAssignee();
		}
		else if (sortBy.Equals(SORT_BY_CREATE_TIME_VALUE))
		{
		  query.orderByTaskCreateTime();
		}
		else if (sortBy.Equals(SORT_BY_DESCRIPTION_VALUE))
		{
		  query.orderByTaskDescription();
		}
		else if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByTaskId();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByTaskName();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID_VALUE))
		{
		  query.orderByTenantId();
		}
		else if (sortBy.Equals(SORT_BY_NAME_CASE_INSENSITIVE_VALUE))
		{
		  query.orderByTaskNameCaseInsensitive();
		}
		else if (sortBy.Equals(SORT_BY_PRIORITY_VALUE))
		{
		  query.orderByTaskPriority();

		}
		else if (sortBy.Equals(SORT_BY_PROCESS_VARIABLE))
		{
		  string variableName = getVariableName(parameters);
		  string valueTypeName = getValueTypeName(parameters);
		  query.orderByProcessVariable(variableName, getValueTypeByName(valueTypeName, engine));

		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_VARIABLE))
		{
		  string variableName = getVariableName(parameters);
		  string valueTypeName = getValueTypeName(parameters);
		  query.orderByExecutionVariable(variableName, getValueTypeByName(valueTypeName, engine));

		}
		else if (sortBy.Equals(SORT_BY_TASK_VARIABLE))
		{
		  string variableName = getVariableName(parameters);
		  string valueTypeName = getValueTypeName(parameters);
		  query.orderByTaskVariable(variableName, getValueTypeByName(valueTypeName, engine));

		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_VARIABLE))
		{
		  string variableName = getVariableName(parameters);
		  string valueTypeName = getValueTypeName(parameters);
		  query.orderByCaseInstanceVariable(variableName, getValueTypeByName(valueTypeName, engine));

		}
		else if (sortBy.Equals(SORT_BY_CASE_EXECUTION_VARIABLE))
		{
		  string variableName = getVariableName(parameters);
		  string valueTypeName = getValueTypeName(parameters);
		  query.orderByCaseExecutionVariable(variableName, getValueTypeByName(valueTypeName, engine));
		}
	  }

	  protected internal virtual string getValueTypeName(IDictionary<string, object> parameters)
	  {
		string valueTypeName = (string) getValue(parameters, SORT_PARAMETERS_VALUE_TYPE);
		if (!string.ReferenceEquals(valueTypeName, null))
		{
		  valueTypeName = VariableValueDto.fromRestApiTypeName(valueTypeName);
		}
		return valueTypeName;
	  }

	  protected internal virtual string getVariableName(IDictionary<string, object> parameters)
	  {
		return (string) getValue(parameters, SORT_PARAMETERS_VARIABLE_NAME);
	  }

	  protected internal virtual object getValue(IDictionary<string, object> map, string key)
	  {
		if (map != null)
		{
		  return map[key];
		}
		return null;
	  }

	  protected internal virtual ValueType getValueTypeByName(string name, ProcessEngine engine)
	  {
		ValueTypeResolver valueTypeResolver = engine.ProcessEngineConfiguration.ValueTypeResolver;
		return valueTypeResolver.typeForName(name);
	  }

	  public static TaskQueryDto fromQuery<T1>(Query<T1> query)
	  {
		return fromQuery(query, false);
	  }

	  public static TaskQueryDto fromQuery<T1>(Query<T1> query, bool isOrQueryActive)
	  {
		TaskQueryImpl taskQuery = (TaskQueryImpl) query;

		TaskQueryDto dto = new TaskQueryDto();

		if (!isOrQueryActive)
		{
		  dto.orQueries = new List<TaskQueryDto>();
		  foreach (TaskQueryImpl orQuery in taskQuery.Queries)
		  {
			if (orQuery.OrQueryActive)
			{
			  dto.orQueries.Add(fromQuery(orQuery, true));
			}
		  }
		}

		dto.activityInstanceIdIn = taskQuery.ActivityInstanceIdIn;
		dto.caseDefinitionId = taskQuery.CaseDefinitionId;
		dto.caseDefinitionKey = taskQuery.CaseDefinitionKey;
		dto.caseDefinitionName = taskQuery.CaseDefinitionName;
		dto.caseDefinitionNameLike = taskQuery.CaseDefinitionNameLike;
		dto.caseExecutionId = taskQuery.CaseExecutionId;
		dto.caseInstanceBusinessKey = taskQuery.CaseInstanceBusinessKey;
		dto.caseInstanceBusinessKeyLike = taskQuery.CaseInstanceBusinessKeyLike;
		dto.caseInstanceId = taskQuery.CaseInstanceId;

		dto.candidateUser = taskQuery.CandidateUser;
		dto.candidateGroup = taskQuery.CandidateGroup;
		dto.candidateGroups = taskQuery.CandidateGroupsInternal;
		dto.includeAssignedTasks = taskQuery.IncludeAssignedTasksInternal;
		dto.withCandidateGroups = taskQuery.WithCandidateGroups;
		dto.withoutCandidateGroups = taskQuery.WithoutCandidateGroups;
		dto.withCandidateUsers = taskQuery.WithCandidateUsers;
		dto.withoutCandidateUsers = taskQuery.WithoutCandidateUsers;

		dto.processInstanceBusinessKey = taskQuery.ProcessInstanceBusinessKey;
		dto.processInstanceBusinessKeyLike = taskQuery.ProcessInstanceBusinessKeyLike;
		dto.processDefinitionKey = taskQuery.ProcessDefinitionKey;
		dto.processDefinitionKeyIn = taskQuery.ProcessDefinitionKeys;
		dto.processDefinitionId = taskQuery.ProcessDefinitionId;
		dto.executionId = taskQuery.ExecutionId;

		dto.processDefinitionName = taskQuery.ProcessDefinitionName;
		dto.processDefinitionNameLike = taskQuery.ProcessDefinitionNameLike;
		dto.processInstanceId = taskQuery.ProcessInstanceId;
		dto.assignee = taskQuery.Assignee;
		dto.assigneeLike = taskQuery.AssigneeLike;
		dto.taskDefinitionKey = taskQuery.Key;
		dto.taskDefinitionKeyIn = taskQuery.Keys;
		dto.taskDefinitionKeyLike = taskQuery.KeyLike;
		dto.description = taskQuery.Description;
		dto.descriptionLike = taskQuery.DescriptionLike;
		dto.involvedUser = taskQuery.InvolvedUser;
		dto.maxPriority = taskQuery.MaxPriority;
		dto.minPriority = taskQuery.MinPriority;
		dto.name = taskQuery.Name;
		dto.nameNotEqual = taskQuery.NameNotEqual;
		dto.nameLike = taskQuery.NameLike;
		dto.nameNotLike = taskQuery.NameNotLike;
		dto.owner = taskQuery.Owner;
		dto.priority = taskQuery.Priority;
		dto.assigned = taskQuery.AssignedInternal;
		dto.unassigned = taskQuery.UnassignedInternal;
		dto.parentTaskId = taskQuery.ParentTaskId;

		dto.dueAfter = taskQuery.DueAfter;
		dto.dueBefore = taskQuery.DueBefore;
		dto.dueDate = taskQuery.DueDate;
		dto.followUpAfter = taskQuery.FollowUpAfter;

		dto.variableNamesIgnoreCase = taskQuery.VariableNamesIgnoreCase;
		dto.variableValuesIgnoreCase = taskQuery.VariableValuesIgnoreCase;

		if (taskQuery.FollowUpNullAccepted)
		{
		  dto.followUpBeforeOrNotExistent = taskQuery.FollowUpBefore;
		}
		else
		{
		  dto.followUpBefore = taskQuery.FollowUpBefore;
		}
		dto.followUpDate = taskQuery.FollowUpDate;
		dto.createdAfter = taskQuery.CreateTimeAfter;
		dto.createdBefore = taskQuery.CreateTimeBefore;
		dto.createdOn = taskQuery.CreateTime;

		if (taskQuery.DelegationState != null)
		{
		  dto.delegationState = taskQuery.DelegationState.ToString();
		}

		if (taskQuery.TenantIdSet)
		{
		  if (taskQuery.TenantIds != null)
		  {
			dto.tenantIdIn = taskQuery.TenantIds;
		  }
		  else
		  {
			dto.withoutTenantId = true;
		  }
		}

		dto.processVariables = new List<VariableQueryParameterDto>();
		dto.taskVariables = new List<VariableQueryParameterDto>();
		dto.caseInstanceVariables = new List<VariableQueryParameterDto>();
		foreach (TaskQueryVariableValue variableValue in taskQuery.Variables)
		{
		  VariableQueryParameterDto variableValueDto = new VariableQueryParameterDto(variableValue);

		  if (variableValue.ProcessInstanceVariable)
		  {
			dto.processVariables.Add(variableValueDto);
		  }
		  else if (variableValue.Local)
		  {
			dto.taskVariables.Add(variableValueDto);
		  }
		  else
		  {
			dto.caseInstanceVariables.Add(variableValueDto);
		  }
		}

		if (taskQuery.SuspensionState == org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE)
		{
		  dto.active = true;
		}
		if (taskQuery.SuspensionState == org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED)
		{
		  dto.suspended = true;
		}

		// sorting
		IList<QueryOrderingProperty> orderingProperties = taskQuery.OrderingProperties;
		if (orderingProperties.Count > 0)
		{
		  dto.Sorting = convertQueryOrderingPropertiesToSortingDtos(orderingProperties);
		}

		// expressions
		IDictionary<string, string> expressions = taskQuery.Expressions;
		if (expressions.ContainsKey("taskAssignee"))
		{
		  dto.AssigneeExpression = expressions["taskAssignee"];
		}
		if (expressions.ContainsKey("taskAssigneeLike"))
		{
		  dto.AssigneeLikeExpression = expressions["taskAssigneeLike"];
		}
		if (expressions.ContainsKey("taskOwner"))
		{
		  dto.OwnerExpression = expressions["taskOwner"];
		}
		if (expressions.ContainsKey("taskCandidateUser"))
		{
		  dto.CandidateUserExpression = expressions["taskCandidateUser"];
		}
		if (expressions.ContainsKey("taskInvolvedUser"))
		{
		  dto.InvolvedUserExpression = expressions["taskInvolvedUser"];
		}
		if (expressions.ContainsKey("taskCandidateGroup"))
		{
		  dto.CandidateGroupExpression = expressions["taskCandidateGroup"];
		}
		if (expressions.ContainsKey("taskCandidateGroupIn"))
		{
		  dto.CandidateGroupsExpression = expressions["taskCandidateGroupIn"];
		}
		if (expressions.ContainsKey("taskCreatedOne"))
		{
		  dto.CreatedOnExpression = expressions["taskCreatedOne"];
		}
		if (expressions.ContainsKey("taskCreatedBefore"))
		{
		  dto.CreatedBeforeExpression = expressions["taskCreatedBefore"];
		}
		if (expressions.ContainsKey("taskCreatedAfter"))
		{
		  dto.CreatedAfterExpression = expressions["taskCreatedAfter"];
		}
		if (expressions.ContainsKey("dueDate"))
		{
		  dto.DueDateExpression = expressions["dueDate"];
		}
		if (expressions.ContainsKey("dueBefore"))
		{
		  dto.DueBeforeExpression = expressions["dueBefore"];
		}
		if (expressions.ContainsKey("dueAfter"))
		{
		  dto.DueAfterExpression = expressions["dueAfter"];
		}
		if (expressions.ContainsKey("followUpDate"))
		{
		  dto.FollowUpDateExpression = expressions["followUpDate"];
		}
		if (expressions.ContainsKey("followUpBefore"))
		{
		  dto.FollowUpBeforeExpression = expressions["followUpBefore"];
		}
		if (expressions.ContainsKey("followUpBeforeOrNotExistent"))
		{
		  dto.FollowUpBeforeOrNotExistentExpression = expressions["followUpBeforeOrNotExistent"];
		}
		if (expressions.ContainsKey("followUpAfter"))
		{
		  dto.FollowUpAfterExpression = expressions["followUpAfter"];
		}
		if (expressions.ContainsKey("processInstanceBusinessKey"))
		{
		  dto.ProcessInstanceBusinessKeyExpression = expressions["processInstanceBusinessKey"];
		}
		if (expressions.ContainsKey("processInstanceBusinessKeyLike"))
		{
		  dto.ProcessInstanceBusinessKeyLikeExpression = expressions["processInstanceBusinessKeyLike"];
		}

		return dto;
	  }

	  public static IList<SortingDto> convertQueryOrderingPropertiesToSortingDtos(IList<QueryOrderingProperty> orderingProperties)
	  {
		IList<SortingDto> sortingDtos = new List<SortingDto>();
		foreach (QueryOrderingProperty orderingProperty in orderingProperties)
		{
		  SortingDto sortingDto;
		  if (orderingProperty is VariableOrderProperty)
		  {
			sortingDto = convertVariableOrderPropertyToSortingDto((VariableOrderProperty) orderingProperty);
		  }
		  else
		  {
			sortingDto = convertQueryOrderingPropertyToSortingDto(orderingProperty);
		  }
		  sortingDtos.Add(sortingDto);
		}
		return sortingDtos;
	  }

	  public static SortingDto convertVariableOrderPropertyToSortingDto(VariableOrderProperty variableOrderProperty)
	  {
		SortingDto sortingDto = new SortingDto();
		sortingDto.SortBy = sortByValueForVariableOrderProperty(variableOrderProperty);
		sortingDto.SortOrder = sortOrderValueForDirection(variableOrderProperty.Direction);
		sortingDto.Parameters = sortParametersForVariableOrderProperty(variableOrderProperty);
		return sortingDto;
	  }

	  public static SortingDto convertQueryOrderingPropertyToSortingDto(QueryOrderingProperty orderingProperty)
	  {
		SortingDto sortingDto = new SortingDto();
		sortingDto.SortBy = sortByValueForQueryProperty(orderingProperty.QueryProperty);
		sortingDto.SortOrder = sortOrderValueForDirection(orderingProperty.Direction);
		return sortingDto;
	  }

	  public static string sortByValueForQueryProperty(QueryProperty queryProperty)
	  {
		if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.ASSIGNEE.Equals(queryProperty))
		{
		  return SORT_BY_ASSIGNEE_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.CASE_EXECUTION_ID.Equals(queryProperty))
		{
		  return SORT_BY_CASE_EXECUTION_ID_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.CASE_INSTANCE_ID.Equals(queryProperty))
		{
		  return SORT_BY_CASE_INSTANCE_ID_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.CREATE_TIME.Equals(queryProperty))
		{
		  return SORT_BY_CREATE_TIME_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.DESCRIPTION.Equals(queryProperty))
		{
		  return SORT_BY_DESCRIPTION_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.DUE_DATE.Equals(queryProperty))
		{
		  return SORT_BY_DUE_DATE_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.EXECUTION_ID.Equals(queryProperty))
		{
		  return SORT_BY_EXECUTION_ID_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.FOLLOW_UP_DATE.Equals(queryProperty))
		{
		  return SORT_BY_FOLLOW_UP_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Equals(queryProperty))
		{
		  return SORT_BY_NAME_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME_CASE_INSENSITIVE.Equals(queryProperty))
		{
		  return SORT_BY_NAME_CASE_INSENSITIVE_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.PRIORITY.Equals(queryProperty))
		{
		  return SORT_BY_PRIORITY_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.PROCESS_INSTANCE_ID.Equals(queryProperty))
		{
		  return SORT_BY_PROCESS_INSTANCE_ID_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.TASK_ID.Equals(queryProperty))
		{
		  return SORT_BY_ID_VALUE;
		}
		else if (org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.TENANT_ID.Equals(queryProperty))
		{
		  return SORT_BY_TENANT_ID_VALUE;
		}
		else
		{
		  throw new RestException("Unknown query property for task query " + queryProperty);
		}
	  }

	  public static string sortByValueForVariableOrderProperty(VariableOrderProperty variableOrderProperty)
	  {
		foreach (QueryEntityRelationCondition relationCondition in variableOrderProperty.RelationConditions)
		{
		  if (relationCondition.PropertyComparison)
		  {
			return sortByValueForQueryEntityRelationCondition(relationCondition);
		  }
		}

		// if no property comparison was found throw an exception
		throw new RestException("Unknown variable order property for task query " + variableOrderProperty);
	  }

	  public static string sortByValueForQueryEntityRelationCondition(QueryEntityRelationCondition relationCondition)
	  {
		QueryProperty property = relationCondition.Property;
		QueryProperty comparisonProperty = relationCondition.ComparisonProperty;
		if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.EXECUTION_ID.Equals(property) && org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.PROCESS_INSTANCE_ID.Equals(comparisonProperty))
		{
			return SORT_BY_PROCESS_VARIABLE;
		}
		else if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.EXECUTION_ID.Equals(property) && org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.EXECUTION_ID.Equals(comparisonProperty))
		{
		  return SORT_BY_EXECUTION_VARIABLE;
		}
		else if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.TASK_ID.Equals(property) && org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.TASK_ID.Equals(comparisonProperty))
		{
		  return SORT_BY_TASK_VARIABLE;
		}
		else if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.CASE_EXECUTION_ID.Equals(property) && org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.CASE_INSTANCE_ID.Equals(comparisonProperty))
		{
		  return SORT_BY_CASE_INSTANCE_VARIABLE;
		}
		else if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.CASE_EXECUTION_ID.Equals(property) && org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.CASE_EXECUTION_ID.Equals(comparisonProperty))
		{
		  return SORT_BY_CASE_EXECUTION_VARIABLE;
		}
		else
		{
		  throw new RestException("Unknown relation condition for task query  with query property " + property + " and comparison property " + comparisonProperty);
		}
	  }

	  public static IDictionary<string, object> sortParametersForVariableOrderProperty(VariableOrderProperty variableOrderProperty)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		foreach (QueryEntityRelationCondition relationCondition in variableOrderProperty.RelationConditions)
		{
		  QueryProperty property = relationCondition.Property;
		  if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.VARIABLE_NAME.Equals(property))
		  {
			parameters[SORT_PARAMETERS_VARIABLE_NAME] = relationCondition.ScalarValue;
		  }
		  else if (org.camunda.bpm.engine.impl.VariableInstanceQueryProperty_Fields.VARIABLE_TYPE.Equals(property))
		  {
			string type = VariableValueDto.toRestApiTypeName((string) relationCondition.ScalarValue);
			parameters[SORT_PARAMETERS_VALUE_TYPE] = type;
		  }
		}
		return parameters;
	  }
	}
}