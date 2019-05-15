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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using SignalDto = org.camunda.bpm.engine.rest.dto.SignalDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using SignalEventReceivedBuilder = org.camunda.bpm.engine.runtime.SignalEventReceivedBuilder;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SignalRestServiceImpl : AbstractRestProcessEngineAware, SignalRestService
	{

	  public SignalRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual void throwSignal(SignalDto dto)
	  {
		string name = dto.Name;
		if (string.ReferenceEquals(name, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "No signal name given");
		}

		SignalEventReceivedBuilder signalEvent = createSignalEventReceivedBuilder(dto);
		signalEvent.send();
	  }

	  protected internal virtual SignalEventReceivedBuilder createSignalEventReceivedBuilder(SignalDto dto)
	  {
		RuntimeService runtimeService = processEngine.RuntimeService;
		string name = dto.Name;
		SignalEventReceivedBuilder signalEvent = runtimeService.createSignalEvent(name);

		string executionId = dto.ExecutionId;
		if (!string.ReferenceEquals(executionId, null))
		{
		  signalEvent.executionId(executionId);
		}

		IDictionary<string, VariableValueDto> variablesDto = dto.Variables;
		if (variablesDto != null)
		{
		  IDictionary<string, object> variables = VariableValueDto.toMap(variablesDto, processEngine, objectMapper);
		  signalEvent.Variables = variables;
		}

		string tenantId = dto.TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  signalEvent.tenantId(tenantId);
		}

		bool isWithoutTenantId = dto.WithoutTenantId;
		if (isWithoutTenantId)
		{
		  signalEvent.withoutTenantId();
		}

		return signalEvent;
	  }
	}

}