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
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal DateTime dueDate_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string exceptionMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal int retries_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal bool suspended_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal long priority_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string jobDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal DateTime createTime_Renamed;

		public virtual MockJobBuilder id(string id)
		{
			this.id_Renamed = id;
			return this;
		}

		public virtual MockJobBuilder dueDate(DateTime dueDate)
		{
			this.dueDate_Renamed = dueDate;
			return this;
		}

		public virtual MockJobBuilder exceptionMessage(string exceptionMessage)
		{
			this.exceptionMessage_Renamed = exceptionMessage;
			return this;
		}

		public virtual MockJobBuilder executionId(string executionId)
		{
			this.executionId_Renamed = executionId;
			return this;
		}

		public virtual MockJobBuilder processInstanceId(string processInstanceId)
		{
			this.processInstanceId_Renamed = processInstanceId;
			return this;
		}

	  public virtual MockJobBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockJobBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual MockJobBuilder suspended(bool suspended)
	  {
		this.suspended_Renamed = suspended;
		return this;
	  }

	  public virtual MockJobBuilder priority(long priority)
	  {
		this.priority_Renamed = priority;
		return this;
	  }

		public virtual MockJobBuilder retries(int retries)
		{
			this.retries_Renamed = retries;
			return this;
		}

		public virtual MockJobBuilder jobDefinitionId(string jobDefinitionId)
		{
		  this.jobDefinitionId_Renamed = jobDefinitionId;
		  return this;
		}

		public virtual MockJobBuilder tenantId(string tenantId)
		{
		  this.tenantId_Renamed = tenantId;
		  return this;
		}

		public virtual MockJobBuilder createTime(DateTime createTime)
		{
			this.createTime_Renamed = createTime;
			return this;
		}

		public virtual Job build()
		{
			Job mockJob = mock(typeof(Job));
			when(mockJob.Id).thenReturn(id_Renamed);
			when(mockJob.Duedate).thenReturn(dueDate_Renamed);
			when(mockJob.ExceptionMessage).thenReturn(exceptionMessage_Renamed);
			when(mockJob.ExecutionId).thenReturn(executionId_Renamed);
			when(mockJob.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
			when(mockJob.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
			when(mockJob.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
			when(mockJob.Retries).thenReturn(retries_Renamed);
			when(mockJob.Suspended).thenReturn(suspended_Renamed);
			when(mockJob.Priority).thenReturn(priority_Renamed);
			when(mockJob.JobDefinitionId).thenReturn(jobDefinitionId_Renamed);
			when(mockJob.TenantId).thenReturn(tenantId_Renamed);
			when(mockJob.CreateTime).thenReturn(createTime_Renamed);
			return mockJob;
		}

	}

}