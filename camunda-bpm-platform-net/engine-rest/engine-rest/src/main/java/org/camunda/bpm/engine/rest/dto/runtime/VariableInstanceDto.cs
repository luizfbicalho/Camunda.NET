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
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class VariableInstanceDto : VariableValueDto
	{

	  protected internal string id;
	  protected internal string name;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskId;
	  protected internal string activityInstanceId;
	  protected internal string errorMessage;
	  private string tenantId;

	  public VariableInstanceDto()
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }


	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
		  set
		  {
			this.errorMessage = value;
		  }
	  }


	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public static VariableInstanceDto fromVariableInstance(VariableInstance variableInstance)
	  {
		VariableInstanceDto dto = new VariableInstanceDto();

		dto.id = variableInstance.Id;
		dto.name = variableInstance.Name;
		dto.processInstanceId = variableInstance.ProcessInstanceId;
		dto.executionId = variableInstance.ExecutionId;

		dto.caseExecutionId = variableInstance.CaseExecutionId;
		dto.caseInstanceId = variableInstance.CaseInstanceId;

		dto.taskId = variableInstance.TaskId;
		dto.activityInstanceId = variableInstance.ActivityInstanceId;

		dto.tenantId = variableInstance.TenantId;

		if (string.ReferenceEquals(variableInstance.ErrorMessage, null))
		{
		  VariableValueDto.fromTypedValue(dto, variableInstance.TypedValue);
		}
		else
		{
		  dto.errorMessage = variableInstance.ErrorMessage;
		  dto.type = VariableValueDto.toRestApiTypeName(variableInstance.TypeName);
		}

		return dto;
	  }

	}

}