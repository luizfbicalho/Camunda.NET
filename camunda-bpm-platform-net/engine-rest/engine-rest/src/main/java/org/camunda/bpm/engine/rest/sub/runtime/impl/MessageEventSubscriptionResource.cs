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
namespace org.camunda.bpm.engine.rest.sub.runtime.impl
{

	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using EventSubscriptionDto = org.camunda.bpm.engine.rest.dto.runtime.EventSubscriptionDto;
	using ExecutionTriggerDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionTriggerDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class MessageEventSubscriptionResource : EventSubscriptionResource
	{

	  protected internal const string MESSAGE_EVENT_TYPE = "message";

	  protected internal ProcessEngine engine;
	  protected internal string executionId;
	  protected internal string messageName;

	  protected internal ObjectMapper objectMapper;

	  public MessageEventSubscriptionResource(ProcessEngine engine, string executionId, string messageName, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.executionId = executionId;
		this.messageName = messageName;
		this.objectMapper = objectMapper;
	  }

	  public virtual EventSubscriptionDto EventSubscription
	  {
		  get
		  {
			RuntimeService runtimeService = engine.RuntimeService;
			EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().executionId(executionId).eventName(messageName).eventType(MESSAGE_EVENT_TYPE).singleResult();
    
			if (eventSubscription == null)
			{
			  string errorMessage = string.Format("Message event subscription for execution {0} named {1} does not exist", executionId, messageName);
			  throw new InvalidRequestException(Status.NOT_FOUND, errorMessage);
			}
    
			return EventSubscriptionDto.fromEventSubscription(eventSubscription);
		  }
	  }

	  public virtual void triggerEvent(ExecutionTriggerDto triggerDto)
	  {
		RuntimeService runtimeService = engine.RuntimeService;


		try
		{
		  VariableMap variables = VariableValueDto.toMap(triggerDto.Variables, engine, objectMapper);
		  runtimeService.messageEventReceived(messageName, executionId, variables);

		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  throw new RestException(Status.INTERNAL_SERVER_ERROR, e, string.Format("Cannot trigger message {0} for execution {1}: {2}", messageName, executionId, e.Message));

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot trigger message {0} for execution {1}: {2}", messageName, executionId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}

	  }

	}

}