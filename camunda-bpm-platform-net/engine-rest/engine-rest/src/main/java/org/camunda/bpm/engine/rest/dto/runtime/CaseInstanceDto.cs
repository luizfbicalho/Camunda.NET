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
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceDto : LinkableDto
	{

	  protected internal string id;
	  protected internal string caseDefinitionId;
	  protected internal string businessKey;
	  protected internal string tenantId;
	  protected internal bool active;
	  protected internal bool completed;
	  protected internal bool terminated;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return completed;
		  }
	  }

	  public virtual bool Terminated
	  {
		  get
		  {
			return terminated;
		  }
	  }

	  public static CaseInstanceDto fromCaseInstance(CaseInstance instance)
	  {
		CaseInstanceDto result = new CaseInstanceDto();

		result.id = instance.Id;
		result.caseDefinitionId = instance.CaseDefinitionId;
		result.businessKey = instance.BusinessKey;
		result.tenantId = instance.TenantId;
		result.active = instance.Active;
		result.completed = instance.Completed;
		result.terminated = instance.Terminated;

		return result;
	  }

	}

}