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

	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	public class MockJobDefinitionBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobConfiguration_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? jobPriority_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;

	  public virtual MockJobDefinitionBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder activityId(string activityId)
	  {
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobConfiguration(string jobConfiguration)
	  {
		this.jobConfiguration_Conflict = jobConfiguration;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobType(string jobType)
	  {
		this.jobType_Conflict = jobType;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobPriority(long? priority)
	  {
		this.jobPriority_Conflict = priority;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder suspended(bool suspended)
	  {
		this.suspended_Conflict = suspended;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual JobDefinition build()
	  {
		JobDefinition mockJobDefinition = mock(typeof(JobDefinition));
		when(mockJobDefinition.Id).thenReturn(id_Conflict);
		when(mockJobDefinition.ActivityId).thenReturn(activityId_Conflict);
		when(mockJobDefinition.JobConfiguration).thenReturn(jobConfiguration_Conflict);
		when(mockJobDefinition.OverridingJobPriority).thenReturn(jobPriority_Conflict);
		when(mockJobDefinition.JobType).thenReturn(jobType_Conflict);
		when(mockJobDefinition.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(mockJobDefinition.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
		when(mockJobDefinition.Suspended).thenReturn(suspended_Conflict);
		when(mockJobDefinition.TenantId).thenReturn(tenantId_Conflict);
		return mockJobDefinition;
	  }

	}

}