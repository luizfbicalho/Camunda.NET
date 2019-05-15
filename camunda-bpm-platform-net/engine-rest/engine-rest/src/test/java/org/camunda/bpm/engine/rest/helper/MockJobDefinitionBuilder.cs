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
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobConfiguration_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? jobPriority_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;

	  public virtual MockJobDefinitionBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobConfiguration(string jobConfiguration)
	  {
		this.jobConfiguration_Renamed = jobConfiguration;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobType(string jobType)
	  {
		this.jobType_Renamed = jobType;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder jobPriority(long? priority)
	  {
		this.jobPriority_Renamed = priority;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder suspended(bool suspended)
	  {
		this.suspended_Renamed = suspended;
		return this;
	  }

	  public virtual MockJobDefinitionBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual JobDefinition build()
	  {
		JobDefinition mockJobDefinition = mock(typeof(JobDefinition));
		when(mockJobDefinition.Id).thenReturn(id_Renamed);
		when(mockJobDefinition.ActivityId).thenReturn(activityId_Renamed);
		when(mockJobDefinition.JobConfiguration).thenReturn(jobConfiguration_Renamed);
		when(mockJobDefinition.OverridingJobPriority).thenReturn(jobPriority_Renamed);
		when(mockJobDefinition.JobType).thenReturn(jobType_Renamed);
		when(mockJobDefinition.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(mockJobDefinition.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
		when(mockJobDefinition.Suspended).thenReturn(suspended_Renamed);
		when(mockJobDefinition.TenantId).thenReturn(tenantId_Renamed);
		return mockJobDefinition;
	  }

	}

}