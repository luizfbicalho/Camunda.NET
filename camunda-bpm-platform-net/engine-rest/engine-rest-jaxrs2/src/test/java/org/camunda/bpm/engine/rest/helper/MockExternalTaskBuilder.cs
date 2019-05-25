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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	public class MockExternalTaskBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime lockExpirationTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? retries_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string topicName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string workerId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
	  protected internal VariableMap variables = Variables.createVariables();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long priority_Conflict;

	  public virtual MockExternalTaskBuilder activityId(string activityId)
	  {
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Conflict = errorMessage;
		return this;
	  }

	  public virtual MockExternalTaskBuilder executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockExternalTaskBuilder lockExpirationTime(DateTime lockExpirationTime)
	  {
		this.lockExpirationTime_Conflict = lockExpirationTime;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder retries(int? retries)
	  {
		this.retries_Conflict = retries;
		return this;
	  }

	  public virtual MockExternalTaskBuilder suspended(bool suspended)
	  {
		this.suspended_Conflict = suspended;
		return this;
	  }

	  public virtual MockExternalTaskBuilder topicName(string topicName)
	  {
		this.topicName_Conflict = topicName;
		return this;
	  }

	  public virtual MockExternalTaskBuilder workerId(string workerId)
	  {
		this.workerId_Conflict = workerId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder variable(string variableName, TypedValue value)
	  {
		this.variables.putValueTyped(variableName, value);
		return this;
	  }

	  public virtual MockExternalTaskBuilder priority(long priority)
	  {
		this.priority_Conflict = priority;
		return this;
	  }

	  public virtual ExternalTask buildExternalTask()
	  {
		ExternalTask task = mock(typeof(ExternalTask));
		when(task.ActivityId).thenReturn(activityId_Conflict);
		when(task.ActivityInstanceId).thenReturn(activityInstanceId_Conflict);
		when(task.ErrorMessage).thenReturn(errorMessage_Conflict);
		when(task.ExecutionId).thenReturn(executionId_Conflict);
		when(task.Id).thenReturn(id_Conflict);
		when(task.LockExpirationTime).thenReturn(lockExpirationTime_Conflict);
		when(task.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(task.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
		when(task.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(task.Retries).thenReturn(retries_Conflict);
		when(task.Suspended).thenReturn(suspended_Conflict);
		when(task.TopicName).thenReturn(topicName_Conflict);
		when(task.WorkerId).thenReturn(workerId_Conflict);
		when(task.TenantId).thenReturn(tenantId_Conflict);
		when(task.Priority).thenReturn(priority_Conflict);

		return task;
	  }

	  public virtual LockedExternalTask buildLockedExternalTask()
	  {
		LockedExternalTask task = mock(typeof(LockedExternalTask));
		when(task.ActivityId).thenReturn(activityId_Conflict);
		when(task.ActivityInstanceId).thenReturn(activityInstanceId_Conflict);
		when(task.ErrorMessage).thenReturn(errorMessage_Conflict);
		when(task.ExecutionId).thenReturn(executionId_Conflict);
		when(task.Id).thenReturn(id_Conflict);
		when(task.LockExpirationTime).thenReturn(lockExpirationTime_Conflict);
		when(task.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(task.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
		when(task.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(task.Retries).thenReturn(retries_Conflict);
		when(task.TopicName).thenReturn(topicName_Conflict);
		when(task.WorkerId).thenReturn(workerId_Conflict);
		when(task.TenantId).thenReturn(tenantId_Conflict);
		when(task.Variables).thenReturn(variables);
		when(task.Priority).thenReturn(priority_Conflict);

		return task;

	  }

	}

}