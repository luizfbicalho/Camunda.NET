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
	using CorrelationMessageDto = org.camunda.bpm.engine.rest.dto.message.CorrelationMessageDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using MessageCorrelationResultDto = org.camunda.bpm.engine.rest.dto.message.MessageCorrelationResultDto;
	using MessageCorrelationResultWithVariableDto = org.camunda.bpm.engine.rest.dto.message.MessageCorrelationResultWithVariableDto;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;

	public class MessageRestServiceImpl : AbstractRestProcessEngineAware, MessageRestService
	{

	  public MessageRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual Response deliverMessage(CorrelationMessageDto messageDto)
	  {
		if (string.ReferenceEquals(messageDto.MessageName, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "No message name supplied");
		}
		if (!string.ReferenceEquals(messageDto.TenantId, null) && messageDto.WithoutTenantId)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Parameter 'tenantId' cannot be used together with parameter 'withoutTenantId'.");
		}
		bool variablesInResultEnabled = messageDto.VariablesInResultEnabled;
		if (!messageDto.ResultEnabled && variablesInResultEnabled)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Parameter 'variablesInResultEnabled' cannot be used without 'resultEnabled' set to true.");
		}

		IList<MessageCorrelationResultDto> resultDtos = new List<MessageCorrelationResultDto>();
		try
		{
		  MessageCorrelationBuilder correlation = createMessageCorrelationBuilder(messageDto);
			if (!variablesInResultEnabled)
			{
			  ((IList<MessageCorrelationResultDto>)resultDtos).AddRange(correlate(messageDto, correlation));
			}
			else
			{
			  ((IList<MessageCorrelationResultDto>)resultDtos).AddRange(correlateWithVariablesEnabled(messageDto, correlation));
			}
		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot deliver message: {0}", e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}
		catch (MismatchingMessageCorrelationException e)
		{
		  throw new RestException(Status.BAD_REQUEST, e);
		}
		return createResponse(resultDtos, messageDto);
	  }

	  protected internal virtual IList<MessageCorrelationResultDto> correlate(CorrelationMessageDto messageDto, MessageCorrelationBuilder correlation)
	  {
		IList<MessageCorrelationResultDto> resultDtos = new List<MessageCorrelationResultDto>();
		if (!messageDto.All)
		{
		  MessageCorrelationResult result = correlation.correlateWithResult();
		  resultDtos.Add(MessageCorrelationResultDto.fromMessageCorrelationResult(result));
		}
		else
		{
		  IList<MessageCorrelationResult> results = correlation.correlateAllWithResult();
		  foreach (MessageCorrelationResult result in results)
		  {
			resultDtos.Add(MessageCorrelationResultDto.fromMessageCorrelationResult(result));
		  }
		}
		return resultDtos;
	  }

	  protected internal virtual IList<MessageCorrelationResultWithVariableDto> correlateWithVariablesEnabled(CorrelationMessageDto messageDto, MessageCorrelationBuilder correlation)
	  {
		IList<MessageCorrelationResultWithVariableDto> resultDtos = new List<MessageCorrelationResultWithVariableDto>();
		if (!messageDto.All)
		{
		  MessageCorrelationResultWithVariables result = correlation.correlateWithResultAndVariables(false);
		  resultDtos.Add(MessageCorrelationResultWithVariableDto.fromMessageCorrelationResultWithVariables(result));
		}
		else
		{
		  IList<MessageCorrelationResultWithVariables> results = correlation.correlateAllWithResultAndVariables(false);
		  foreach (MessageCorrelationResultWithVariables result in results)
		  {
			resultDtos.Add(MessageCorrelationResultWithVariableDto.fromMessageCorrelationResultWithVariables(result));
		  }
		}
		return resultDtos;
	  }


	  protected internal virtual Response createResponse(IList<MessageCorrelationResultDto> resultDtos, CorrelationMessageDto messageDto)
	  {
		Response.ResponseBuilder response = Response.noContent();
		if (messageDto.ResultEnabled)
		{
		  response = Response.ok(resultDtos, MediaType.APPLICATION_JSON);
		}
		return response.build();
	  }

	  protected internal virtual MessageCorrelationBuilder createMessageCorrelationBuilder(CorrelationMessageDto messageDto)
	  {
		RuntimeService runtimeService = processEngine.RuntimeService;

		ObjectMapper objectMapper = ObjectMapper;
		IDictionary<string, object> correlationKeys = VariableValueDto.toMap(messageDto.CorrelationKeys, processEngine, objectMapper);
		IDictionary<string, object> localCorrelationKeys = VariableValueDto.toMap(messageDto.LocalCorrelationKeys, processEngine, objectMapper);
		IDictionary<string, object> processVariables = VariableValueDto.toMap(messageDto.ProcessVariables, processEngine, objectMapper);
		IDictionary<string, object> processVariablesLocal = VariableValueDto.toMap(messageDto.ProcessVariablesLocal, processEngine, objectMapper);

		MessageCorrelationBuilder builder = runtimeService.createMessageCorrelation(messageDto.MessageName);

		if (processVariables != null)
		{
		  builder.Variables = processVariables;
		}
		if (processVariablesLocal != null)
		{
		  builder.VariablesLocal = processVariablesLocal;
		}
		if (!string.ReferenceEquals(messageDto.BusinessKey, null))
		{
		  builder.processInstanceBusinessKey(messageDto.BusinessKey);
		}

		if (correlationKeys != null && correlationKeys.Count > 0)
		{
		  foreach (KeyValuePair<string, object> correlationKey in correlationKeys.SetOfKeyValuePairs())
		  {
			string name = correlationKey.Key;
			object value = correlationKey.Value;
			builder.processInstanceVariableEquals(name, value);
		  }
		}

		if (localCorrelationKeys != null && localCorrelationKeys.Count > 0)
		{
		  foreach (KeyValuePair<string, object> correlationKey in localCorrelationKeys.SetOfKeyValuePairs())
		  {
			string name = correlationKey.Key;
			object value = correlationKey.Value;
			builder.localVariableEquals(name, value);
		  }
		}

		if (!string.ReferenceEquals(messageDto.TenantId, null))
		{
		  builder.tenantId(messageDto.TenantId);

		}
		else if (messageDto.WithoutTenantId)
		{
		  builder.withoutTenantId();
		}

		string processInstanceId = messageDto.ProcessInstanceId;
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  builder.processInstanceId(processInstanceId);
		}

		return builder;
	  }

	}

}