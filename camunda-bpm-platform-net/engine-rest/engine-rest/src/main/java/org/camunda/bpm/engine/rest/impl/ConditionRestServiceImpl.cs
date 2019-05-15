using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.impl
{

	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using EvaluationConditionDto = org.camunda.bpm.engine.rest.dto.condition.EvaluationConditionDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ConditionEvaluationBuilder = org.camunda.bpm.engine.runtime.ConditionEvaluationBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ConditionRestServiceImpl : AbstractRestProcessEngineAware, ConditionRestService
	{

	  public ConditionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual IList<ProcessInstanceDto> evaluateCondition(EvaluationConditionDto conditionDto)
	  {
		if (!string.ReferenceEquals(conditionDto.TenantId, null) && conditionDto.WithoutTenantId)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Parameter 'tenantId' cannot be used together with parameter 'withoutTenantId'.");
		}
		ConditionEvaluationBuilder builder = createConditionEvaluationBuilder(conditionDto);
		IList<ProcessInstance> processInstances = builder.evaluateStartConditions();

		IList<ProcessInstanceDto> result = new List<ProcessInstanceDto>();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  result.Add(ProcessInstanceDto.fromProcessInstance(processInstance));
		}
		return result;
	  }

	  protected internal virtual ConditionEvaluationBuilder createConditionEvaluationBuilder(EvaluationConditionDto conditionDto)
	  {
		RuntimeService runtimeService = processEngine.RuntimeService;

		ObjectMapper objectMapper = ObjectMapper;

		VariableMap variables = VariableValueDto.toMap(conditionDto.Variables, processEngine, objectMapper);

		ConditionEvaluationBuilder builder = runtimeService.createConditionEvaluation();

		if (variables != null && !variables.Empty)
		{
		  builder.Variables = variables;
		}

		if (!string.ReferenceEquals(conditionDto.BusinessKey, null))
		{
		  builder.processInstanceBusinessKey(conditionDto.BusinessKey);
		}

		if (!string.ReferenceEquals(conditionDto.ProcessDefinitionId, null))
		{
		  builder.processDefinitionId(conditionDto.ProcessDefinitionId);
		}

		if (!string.ReferenceEquals(conditionDto.TenantId, null))
		{
		  builder.tenantId(conditionDto.TenantId);
		}
		else if (conditionDto.WithoutTenantId)
		{
		  builder.withoutTenantId();
		}

		return builder;
	  }

	}

}