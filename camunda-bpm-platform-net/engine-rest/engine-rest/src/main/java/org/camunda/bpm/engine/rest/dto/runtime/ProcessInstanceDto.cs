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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	public class ProcessInstanceDto : LinkableDto
	{

	  private string id;
	  private string definitionId;
	  private string businessKey;
	  private string caseInstanceId;
	  private bool ended;
	  private bool suspended;
	  private string tenantId;

	  public ProcessInstanceDto()
	  {
	  }

	  public ProcessInstanceDto(ProcessInstance instance)
	  {
		this.id = instance.Id;
		this.definitionId = instance.ProcessDefinitionId;
		this.businessKey = instance.BusinessKey;
		this.caseInstanceId = instance.CaseInstanceId;
		this.ended = instance.Ended;
		this.suspended = instance.Suspended;
		this.tenantId = instance.TenantId;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string DefinitionId
	  {
		  get
		  {
			return definitionId;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual bool Ended
	  {
		  get
		  {
			return ended;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public static ProcessInstanceDto fromProcessInstance(ProcessInstance instance)
	  {
		return new ProcessInstanceDto(instance);
	  }

	}

}