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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;

	public class MockTaskBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string name_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string assignee_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime createTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime dueDate_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime followUpDate_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DelegationState delegationState_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string description_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string owner_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string parentTaskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int priority_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string taskDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseExecutionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string formKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string tenantId_Conflict;

	  public virtual MockTaskBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockTaskBuilder name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual MockTaskBuilder assignee(string assignee)
	  {
		this.assignee_Conflict = assignee;
		return this;
	  }

	  public virtual MockTaskBuilder createTime(DateTime createTime)
	  {
		this.createTime_Conflict = createTime;
		return this;
	  }

	  public virtual MockTaskBuilder dueDate(DateTime dueDate)
	  {
		this.dueDate_Conflict = dueDate;
		return this;
	  }

	  public virtual MockTaskBuilder followUpDate(DateTime followUpDate)
	  {
		this.followUpDate_Conflict = followUpDate;
		return this;
	  }

	  public virtual MockTaskBuilder delegationState(DelegationState delegationState)
	  {
		this.delegationState_Conflict = delegationState;
		return this;
	  }

	  public virtual MockTaskBuilder description(string description)
	  {
		this.description_Conflict = description;
		return this;
	  }

	  public virtual MockTaskBuilder executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual MockTaskBuilder owner(string owner)
	  {
		this.owner_Conflict = owner;
		return this;
	  }

	  public virtual MockTaskBuilder parentTaskId(string parentTaskId)
	  {
		this.parentTaskId_Conflict = parentTaskId;
		return this;
	  }

	  public virtual MockTaskBuilder priority(int priority)
	  {
		this.priority_Conflict = priority;
		return this;
	  }

	  public virtual MockTaskBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockTaskBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual MockTaskBuilder taskDefinitionKey(string taskDefinitionKey)
	  {
		this.taskDefinitionKey_Conflict = taskDefinitionKey;
		return this;
	  }

	  public virtual MockTaskBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual MockTaskBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual MockTaskBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual MockTaskBuilder formKey(string exampleFormKey)
	  {
		this.formKey_Conflict = exampleFormKey;
		return this;
	  }

	  public virtual MockTaskBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual Task build()
	  {
		Task mockTask = mock(typeof(Task));
		when(mockTask.Id).thenReturn(id_Conflict);
		when(mockTask.Name).thenReturn(name_Conflict);
		when(mockTask.Assignee).thenReturn(assignee_Conflict);
		when(mockTask.CreateTime).thenReturn(createTime_Conflict);
		when(mockTask.DueDate).thenReturn(dueDate_Conflict);
		when(mockTask.FollowUpDate).thenReturn(followUpDate_Conflict);
		when(mockTask.DelegationState).thenReturn(delegationState_Conflict);
		when(mockTask.Description).thenReturn(description_Conflict);
		when(mockTask.ExecutionId).thenReturn(executionId_Conflict);
		when(mockTask.Owner).thenReturn(owner_Conflict);
		when(mockTask.ParentTaskId).thenReturn(parentTaskId_Conflict);
		when(mockTask.Priority).thenReturn(priority_Conflict);
		when(mockTask.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(mockTask.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(mockTask.TaskDefinitionKey).thenReturn(taskDefinitionKey_Conflict);
		when(mockTask.CaseDefinitionId).thenReturn(caseDefinitionId_Conflict);
		when(mockTask.CaseInstanceId).thenReturn(caseInstanceId_Conflict);
		when(mockTask.CaseExecutionId).thenReturn(caseExecutionId_Conflict);
		when(mockTask.FormKey).thenReturn(formKey_Conflict);
		when(mockTask.TenantId).thenReturn(tenantId_Conflict);
		return mockTask;
	  }

	}

}