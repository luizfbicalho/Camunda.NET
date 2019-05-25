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

	using Job = org.camunda.bpm.engine.runtime.Job;

	public class MockJobBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal DateTime dueDate_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string exceptionMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal int retries_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal bool suspended_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal long priority_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string jobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal DateTime createTime_Conflict;

		public virtual MockJobBuilder id(string id)
		{
			this.id_Conflict = id;
			return this;
		}

		public virtual MockJobBuilder dueDate(DateTime dueDate)
		{
			this.dueDate_Conflict = dueDate;
			return this;
		}

		public virtual MockJobBuilder exceptionMessage(string exceptionMessage)
		{
			this.exceptionMessage_Conflict = exceptionMessage;
			return this;
		}

		public virtual MockJobBuilder executionId(string executionId)
		{
			this.executionId_Conflict = executionId;
			return this;
		}

		public virtual MockJobBuilder processInstanceId(string processInstanceId)
		{
			this.processInstanceId_Conflict = processInstanceId;
			return this;
		}

	  public virtual MockJobBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockJobBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual MockJobBuilder suspended(bool suspended)
	  {
		this.suspended_Conflict = suspended;
		return this;
	  }

	  public virtual MockJobBuilder priority(long priority)
	  {
		this.priority_Conflict = priority;
		return this;
	  }

		public virtual MockJobBuilder retries(int retries)
		{
			this.retries_Conflict = retries;
			return this;
		}

		public virtual MockJobBuilder jobDefinitionId(string jobDefinitionId)
		{
		  this.jobDefinitionId_Conflict = jobDefinitionId;
		  return this;
		}

		public virtual MockJobBuilder tenantId(string tenantId)
		{
		  this.tenantId_Conflict = tenantId;
		  return this;
		}

		public virtual MockJobBuilder createTime(DateTime createTime)
		{
			this.createTime_Conflict = createTime;
			return this;
		}

		public virtual Job build()
		{
			Job mockJob = mock(typeof(Job));
			when(mockJob.Id).thenReturn(id_Conflict);
			when(mockJob.Duedate).thenReturn(dueDate_Conflict);
			when(mockJob.ExceptionMessage).thenReturn(exceptionMessage_Conflict);
			when(mockJob.ExecutionId).thenReturn(executionId_Conflict);
			when(mockJob.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
			when(mockJob.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
			when(mockJob.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
			when(mockJob.Retries).thenReturn(retries_Conflict);
			when(mockJob.Suspended).thenReturn(suspended_Conflict);
			when(mockJob.Priority).thenReturn(priority_Conflict);
			when(mockJob.JobDefinitionId).thenReturn(jobDefinitionId_Conflict);
			when(mockJob.TenantId).thenReturn(tenantId_Conflict);
			when(mockJob.CreateTime).thenReturn(createTime_Conflict);
			return mockJob;
		}

	}

}