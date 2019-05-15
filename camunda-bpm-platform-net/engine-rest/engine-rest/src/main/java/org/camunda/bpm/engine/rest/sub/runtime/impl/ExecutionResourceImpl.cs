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

	using CreateIncidentDto = org.camunda.bpm.engine.rest.dto.CreateIncidentDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using ExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto;
	using ExecutionTriggerDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionTriggerDto;
	using IncidentDto = org.camunda.bpm.engine.rest.dto.runtime.IncidentDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ExecutionResourceImpl : ExecutionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string executionId;
	  protected internal ObjectMapper objectMapper;

	  public ExecutionResourceImpl(ProcessEngine engine, string executionId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.executionId = executionId;
		this.objectMapper = objectMapper;
	  }

	  public virtual ExecutionDto Execution
	  {
		  get
		  {
			RuntimeService runtimeService = engine.RuntimeService;
			Execution execution = runtimeService.createExecutionQuery().executionId(executionId).singleResult();
    
			if (execution == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Execution with id " + executionId + " does not exist");
			}
    
			return ExecutionDto.fromExecution(execution);
		  }
	  }

	  public virtual void signalExecution(ExecutionTriggerDto triggerDto)
	  {
		RuntimeService runtimeService = engine.RuntimeService;
		try
		{
		  VariableMap variables = VariableValueDto.toMap(triggerDto.Variables, engine, objectMapper);
		  runtimeService.signal(executionId, variables);

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot signal execution {0}: {1}", executionId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}
		catch (AuthorizationException e)
		{
		  throw e;

		}
		catch (ProcessEngineException e)
		{
		  throw new RestException(Status.INTERNAL_SERVER_ERROR, e, "Cannot signal execution " + executionId + ": " + e.Message);

		}
	  }

	  public virtual VariableResource LocalVariables
	  {
		  get
		  {
			return new LocalExecutionVariablesResource(engine, executionId, objectMapper);
		  }
	  }

	  public virtual EventSubscriptionResource getMessageEventSubscription(string messageName)
	  {
		return new MessageEventSubscriptionResource(engine, executionId, messageName, objectMapper);
	  }

	  public virtual IncidentDto createIncident(CreateIncidentDto createIncidentDto)
	  {
		Incident newIncident = null;

		try
		{
		  newIncident = engine.RuntimeService.createIncident(createIncidentDto.IncidentType, executionId, createIncidentDto.Configuration, createIncidentDto.Message);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
		return IncidentDto.fromIncident(newIncident);
	  }
	}

}