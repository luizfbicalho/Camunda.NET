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
namespace org.camunda.bpm.engine.rest.dto.management
{
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobDefinitionDto
	{

	  protected internal string id;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string jobType;
	  protected internal string jobConfiguration;
	  protected internal string activityId;
	  protected internal bool suspended;
	  protected internal long? overridingJobPriority;
	  protected internal string tenantId;

	  public JobDefinitionDto()
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }
	  public virtual string JobType
	  {
		  get
		  {
			return jobType;
		  }
	  }
	  public virtual string JobConfiguration
	  {
		  get
		  {
			return jobConfiguration;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }
	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }
	  public virtual long? OverridingJobPriority
	  {
		  get
		  {
			return overridingJobPriority;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public static JobDefinitionDto fromJobDefinition(JobDefinition definition)
	  {
		JobDefinitionDto dto = new JobDefinitionDto();

		dto.id = definition.Id;
		dto.processDefinitionId = definition.ProcessDefinitionId;
		dto.processDefinitionKey = definition.ProcessDefinitionKey;
		dto.jobType = definition.JobType;
		dto.jobConfiguration = definition.JobConfiguration;
		dto.activityId = definition.ActivityId;
		dto.suspended = definition.Suspended;
		dto.overridingJobPriority = definition.OverridingJobPriority;
		dto.tenantId = definition.TenantId;

		return dto;
	  }

	}

}