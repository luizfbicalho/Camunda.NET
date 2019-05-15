﻿using System.Collections.Generic;

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
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;

	public class MessageCorrelationResultWithVariableDto : MessageCorrelationResultDto
	{

	  private IDictionary<string, VariableValueDto> variables;

	  public static MessageCorrelationResultWithVariableDto fromMessageCorrelationResultWithVariables(MessageCorrelationResultWithVariables result)
	  {
		MessageCorrelationResultWithVariableDto dto = new MessageCorrelationResultWithVariableDto();

		if (result != null)
		{
		  dto.ResultType = result.ResultType;
		  if (result.ProcessInstance != null)
		  {
			dto.ProcessInstance = ProcessInstanceDto.fromProcessInstance(result.ProcessInstance);
		  }
		  else if (result.Execution != null)
		  {
			dto.Execution = ExecutionDto.fromExecution(result.Execution);
		  }

		  dto.variables = VariableValueDto.fromMap(result.Variables, true);
		}
		return dto;
	  }

	  public virtual IDictionary<string, VariableValueDto> Variables
	  {
		  get
		  {
			return variables;
		  }
	  }
	}

}