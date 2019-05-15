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
	  private string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string assignee_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime createTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime dueDate_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DateTime followUpDate_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private DelegationState delegationState_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string description_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string owner_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string parentTaskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int priority_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string taskDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string formKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private string tenantId_Renamed;

	  public virtual MockTaskBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockTaskBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockTaskBuilder assignee(string assignee)
	  {
		this.assignee_Renamed = assignee;
		return this;
	  }

	  public virtual MockTaskBuilder createTime(DateTime createTime)
	  {
		this.createTime_Renamed = createTime;
		return this;
	  }

	  public virtual MockTaskBuilder dueDate(DateTime dueDate)
	  {
		this.dueDate_Renamed = dueDate;
		return this;
	  }

	  public virtual MockTaskBuilder followUpDate(DateTime followUpDate)
	  {
		this.followUpDate_Renamed = followUpDate;
		return this;
	  }

	  public virtual MockTaskBuilder delegationState(DelegationState delegationState)
	  {
		this.delegationState_Renamed = delegationState;
		return this;
	  }

	  public virtual MockTaskBuilder description(string description)
	  {
		this.description_Renamed = description;
		return this;
	  }

	  public virtual MockTaskBuilder executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual MockTaskBuilder owner(string owner)
	  {
		this.owner_Renamed = owner;
		return this;
	  }

	  public virtual MockTaskBuilder parentTaskId(string parentTaskId)
	  {
		this.parentTaskId_Renamed = parentTaskId;
		return this;
	  }

	  public virtual MockTaskBuilder priority(int priority)
	  {
		this.priority_Renamed = priority;
		return this;
	  }

	  public virtual MockTaskBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockTaskBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual MockTaskBuilder taskDefinitionKey(string taskDefinitionKey)
	  {
		this.taskDefinitionKey_Renamed = taskDefinitionKey;
		return this;
	  }

	  public virtual MockTaskBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Renamed = caseDefinitionId;
		return this;
	  }

	  public virtual MockTaskBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual MockTaskBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual MockTaskBuilder formKey(string exampleFormKey)
	  {
		this.formKey_Renamed = exampleFormKey;
		return this;
	  }

	  public virtual MockTaskBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual Task build()
	  {
		Task mockTask = mock(typeof(Task));
		when(mockTask.Id).thenReturn(id_Renamed);
		when(mockTask.Name).thenReturn(name_Renamed);
		when(mockTask.Assignee).thenReturn(assignee_Renamed);
		when(mockTask.CreateTime).thenReturn(createTime_Renamed);
		when(mockTask.DueDate).thenReturn(dueDate_Renamed);
		when(mockTask.FollowUpDate).thenReturn(followUpDate_Renamed);
		when(mockTask.DelegationState).thenReturn(delegationState_Renamed);
		when(mockTask.Description).thenReturn(description_Renamed);
		when(mockTask.ExecutionId).thenReturn(executionId_Renamed);
		when(mockTask.Owner).thenReturn(owner_Renamed);
		when(mockTask.ParentTaskId).thenReturn(parentTaskId_Renamed);
		when(mockTask.Priority).thenReturn(priority_Renamed);
		when(mockTask.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(mockTask.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(mockTask.TaskDefinitionKey).thenReturn(taskDefinitionKey_Renamed);
		when(mockTask.CaseDefinitionId).thenReturn(caseDefinitionId_Renamed);
		when(mockTask.CaseInstanceId).thenReturn(caseInstanceId_Renamed);
		when(mockTask.CaseExecutionId).thenReturn(caseExecutionId_Renamed);
		when(mockTask.FormKey).thenReturn(formKey_Renamed);
		when(mockTask.TenantId).thenReturn(tenantId_Renamed);
		return mockTask;
	  }

	}

}