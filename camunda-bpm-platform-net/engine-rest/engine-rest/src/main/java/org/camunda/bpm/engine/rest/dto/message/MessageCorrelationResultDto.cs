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
namespace org.camunda.bpm.engine.rest.dto.message
{
	using ExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MessageCorrelationResultDto
	{

	  private MessageCorrelationResultType resultType;

	  //restul type execution
	  private ExecutionDto execution;

	  //result type process definition
	  private ProcessInstanceDto processInstance;

	  public static MessageCorrelationResultDto fromMessageCorrelationResult(MessageCorrelationResult result)
	  {
		MessageCorrelationResultDto dto = new MessageCorrelationResultDto();
		if (result != null)
		{
		  dto.resultType = result.ResultType;
		  if (result.ProcessInstance != null)
		  {
			dto.processInstance = ProcessInstanceDto.fromProcessInstance(result.ProcessInstance);
		  }
		  else if (result.Execution != null)
		  {
			dto.execution = ExecutionDto.fromExecution(result.Execution);
		  }
		}
		return dto;
	  }

	  public virtual MessageCorrelationResultType ResultType
	  {
		  get
		  {
			return resultType;
		  }
		  set
		  {
			this.resultType = value;
		  }
	  }


	  public virtual ExecutionDto Execution
	  {
		  get
		  {
			return execution;
		  }
		  set
		  {
			this.execution = value;
		  }
	  }


	  public virtual ProcessInstanceDto ProcessInstance
	  {
		  get
		  {
			return processInstance;
		  }
		  set
		  {
			this.processInstance = value;
		  }
	  }


	}

}