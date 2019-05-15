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
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime lockExpirationTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? retries_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string topicName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string workerId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
	  protected internal VariableMap variables = Variables.createVariables();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long priority_Renamed;

	  public virtual MockExternalTaskBuilder activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Renamed = errorMessage;
		return this;
	  }

	  public virtual MockExternalTaskBuilder executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockExternalTaskBuilder lockExpirationTime(DateTime lockExpirationTime)
	  {
		this.lockExpirationTime_Renamed = lockExpirationTime;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual MockExternalTaskBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder retries(int? retries)
	  {
		this.retries_Renamed = retries;
		return this;
	  }

	  public virtual MockExternalTaskBuilder suspended(bool suspended)
	  {
		this.suspended_Renamed = suspended;
		return this;
	  }

	  public virtual MockExternalTaskBuilder topicName(string topicName)
	  {
		this.topicName_Renamed = topicName;
		return this;
	  }

	  public virtual MockExternalTaskBuilder workerId(string workerId)
	  {
		this.workerId_Renamed = workerId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockExternalTaskBuilder variable(string variableName, TypedValue value)
	  {
		this.variables.putValueTyped(variableName, value);
		return this;
	  }

	  public virtual MockExternalTaskBuilder priority(long priority)
	  {
		this.priority_Renamed = priority;
		return this;
	  }

	  public virtual ExternalTask buildExternalTask()
	  {
		ExternalTask task = mock(typeof(ExternalTask));
		when(task.ActivityId).thenReturn(activityId_Renamed);
		when(task.ActivityInstanceId).thenReturn(activityInstanceId_Renamed);
		when(task.ErrorMessage).thenReturn(errorMessage_Renamed);
		when(task.ExecutionId).thenReturn(executionId_Renamed);
		when(task.Id).thenReturn(id_Renamed);
		when(task.LockExpirationTime).thenReturn(lockExpirationTime_Renamed);
		when(task.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(task.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
		when(task.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(task.Retries).thenReturn(retries_Renamed);
		when(task.Suspended).thenReturn(suspended_Renamed);
		when(task.TopicName).thenReturn(topicName_Renamed);
		when(task.WorkerId).thenReturn(workerId_Renamed);
		when(task.TenantId).thenReturn(tenantId_Renamed);
		when(task.Priority).thenReturn(priority_Renamed);

		return task;
	  }

	  public virtual LockedExternalTask buildLockedExternalTask()
	  {
		LockedExternalTask task = mock(typeof(LockedExternalTask));
		when(task.ActivityId).thenReturn(activityId_Renamed);
		when(task.ActivityInstanceId).thenReturn(activityInstanceId_Renamed);
		when(task.ErrorMessage).thenReturn(errorMessage_Renamed);
		when(task.ExecutionId).thenReturn(executionId_Renamed);
		when(task.Id).thenReturn(id_Renamed);
		when(task.LockExpirationTime).thenReturn(lockExpirationTime_Renamed);
		when(task.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(task.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
		when(task.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(task.Retries).thenReturn(retries_Renamed);
		when(task.TopicName).thenReturn(topicName_Renamed);
		when(task.WorkerId).thenReturn(workerId_Renamed);
		when(task.TenantId).thenReturn(tenantId_Renamed);
		when(task.Variables).thenReturn(variables);
		when(task.Priority).thenReturn(priority_Renamed);

		return task;

	  }

	}

}