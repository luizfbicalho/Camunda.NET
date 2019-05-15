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
	using Execution = org.camunda.bpm.engine.runtime.Execution;

	public class ExecutionDto
	{

	  private string id;
	  private string processInstanceId;
	  private bool ended;
	  private string tenantId;

	  public static ExecutionDto fromExecution(Execution execution)
	  {
		ExecutionDto dto = new ExecutionDto();
		dto.id = execution.Id;
		dto.processInstanceId = execution.ProcessInstanceId;
		dto.ended = execution.Ended;
		dto.tenantId = execution.TenantId;

		return dto;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual bool Ended
	  {
		  get
		  {
			return ended;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	}

}