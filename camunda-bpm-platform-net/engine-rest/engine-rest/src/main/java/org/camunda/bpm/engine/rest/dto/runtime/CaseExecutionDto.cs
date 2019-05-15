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
namespace org.camunda.bpm.engine.rest.dto.runtime
{
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionDto
	{

	  protected internal string id;
	  protected internal string caseInstanceId;
	  protected internal string caseDefinitionId;
	  protected internal string activityId;
	  protected internal string activityName;
	  protected internal string activityType;
	  protected internal string activityDescription;
	  protected internal string parentId;
	  protected internal string tenantId;
	  protected internal bool required;
	  protected internal bool enabled;
	  protected internal bool active;
	  protected internal bool disabled;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
	  }

	  public virtual string ActivityDescription
	  {
		  get
		  {
			return activityDescription;
		  }
	  }

	  public virtual string ParentId
	  {
		  get
		  {
			return parentId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual bool Required
	  {
		  get
		  {
			return required;
		  }
	  }

	  public virtual bool Enabled
	  {
		  get
		  {
			return enabled;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual bool Disabled
	  {
		  get
		  {
			return disabled;
		  }
	  }

	  public static CaseExecutionDto fromCaseExecution(CaseExecution caseExecution)
	  {
		CaseExecutionDto dto = new CaseExecutionDto();

		dto.id = caseExecution.Id;
		dto.caseInstanceId = caseExecution.CaseInstanceId;
		dto.caseDefinitionId = caseExecution.CaseDefinitionId;
		dto.activityId = caseExecution.ActivityId;
		dto.activityName = caseExecution.ActivityName;
		dto.activityType = caseExecution.ActivityType;
		dto.activityDescription = caseExecution.ActivityDescription;
		dto.parentId = caseExecution.ParentId;
		dto.tenantId = caseExecution.TenantId;
		dto.required = caseExecution.Required;
		dto.active = caseExecution.Active;
		dto.enabled = caseExecution.Enabled;
		dto.disabled = caseExecution.Disabled;

		return dto;
	  }

	}

}