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
namespace org.camunda.bpm.engine.rest.sub.runtime.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto;
	using CaseExecutionTriggerDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto;
	using TriggerVariableValueDto = org.camunda.bpm.engine.rest.dto.runtime.TriggerVariableValueDto;
	using VariableNameDto = org.camunda.bpm.engine.rest.dto.runtime.VariableNameDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionCommandBuilder = org.camunda.bpm.engine.runtime.CaseExecutionCommandBuilder;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionResourceImpl : CaseExecutionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string caseExecutionId;
	  protected internal ObjectMapper objectMapper;

	  public CaseExecutionResourceImpl(ProcessEngine engine, string caseExecutionId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.caseExecutionId = caseExecutionId;
		this.objectMapper = objectMapper;
	  }

	  public virtual CaseExecutionDto CaseExecution
	  {
		  get
		  {
			CaseService caseService = engine.CaseService;
    
			CaseExecution execution = caseService.createCaseExecutionQuery().caseExecutionId(caseExecutionId).singleResult();
    
			if (execution == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Case execution with id " + caseExecutionId + " does not exist.");
			}
    
			CaseExecutionDto result = CaseExecutionDto.fromCaseExecution(execution);
			return result;
		  }
	  }

	  public virtual void manualStart(CaseExecutionTriggerDto triggerDto)
	  {
		try
		{
		  CaseService caseService = engine.CaseService;
		  CaseExecutionCommandBuilder commandBuilder = caseService.withCaseExecution(caseExecutionId);

		  initializeCommand(commandBuilder, triggerDto, "start manually");

		  commandBuilder.manualStart();

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("manualStart", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("manualStart", Status.BAD_REQUEST, e);

		}
		catch (NotAllowedException e)
		{
		  throw createInvalidRequestException("manualStart", Status.FORBIDDEN, e);

		}
		catch (ProcessEngineException e)
		{
		  throw createRestException("manualStart", Status.INTERNAL_SERVER_ERROR, e);

		}

	  }

	  public virtual void disable(CaseExecutionTriggerDto triggerDto)
	  {
		try
		{
		  CaseService caseService = engine.CaseService;
		  CaseExecutionCommandBuilder commandBuilder = caseService.withCaseExecution(caseExecutionId);

		  initializeCommand(commandBuilder, triggerDto, "disable");

		  commandBuilder.disable();

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("disable", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("disable", Status.BAD_REQUEST, e);

		}
		catch (NotAllowedException e)
		{
		  throw createInvalidRequestException("disable", Status.FORBIDDEN, e);

		}
		catch (ProcessEngineException e)
		{
		  throw createRestException("disable", Status.INTERNAL_SERVER_ERROR, e);

		}

	  }

	  public virtual void reenable(CaseExecutionTriggerDto triggerDto)
	  {
		try
		{
		  CaseService caseService = engine.CaseService;
		  CaseExecutionCommandBuilder commandBuilder = caseService.withCaseExecution(caseExecutionId);

		  initializeCommand(commandBuilder, triggerDto, "reenable");

		  commandBuilder.reenable();

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("reenable", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("reenable", Status.BAD_REQUEST, e);

		}
		catch (NotAllowedException e)
		{
		  throw createInvalidRequestException("reenable", Status.FORBIDDEN, e);

		}
		catch (ProcessEngineException e)
		{
		  throw createRestException("reenable", Status.INTERNAL_SERVER_ERROR, e);

		}
	  }

	  public virtual void complete(CaseExecutionTriggerDto triggerDto)
	  {
		try
		{
		  CaseService caseService = engine.CaseService;
		  CaseExecutionCommandBuilder commandBuilder = caseService.withCaseExecution(caseExecutionId);

		  initializeCommand(commandBuilder, triggerDto, "complete");

		  commandBuilder.complete();

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("complete", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("complete", Status.BAD_REQUEST, e);

		}
		catch (NotAllowedException e)
		{
		  throw createInvalidRequestException("complete", Status.FORBIDDEN, e);

		}
		catch (ProcessEngineException e)
		{
		  throw createRestException("complete", Status.INTERNAL_SERVER_ERROR, e);

		}
	  }

	  public virtual void terminate(CaseExecutionTriggerDto triggerDto)
	  {
		try
		{
		  CaseService caseService = engine.CaseService;
		  CaseExecutionCommandBuilder commandBuilder = caseService.withCaseExecution(caseExecutionId);

		  initializeCommand(commandBuilder, triggerDto, "terminate");

		  commandBuilder.terminate();

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("terminate", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("terminate", Status.BAD_REQUEST, e);

		}
		catch (NotAllowedException e)
		{
		  throw createInvalidRequestException("terminate", Status.FORBIDDEN, e);

		}
		catch (ProcessEngineException e)
		{
		  throw createRestException("terminate", Status.INTERNAL_SERVER_ERROR, e);

		}
	  }

	  protected internal virtual InvalidRequestException createInvalidRequestException(string transition, Status status, ProcessEngineException cause)
	  {
		string errorMessage = string.Format("Cannot {0} case execution {1}: {2}", transition, caseExecutionId, cause.Message);
		return new InvalidRequestException(status, cause, errorMessage);
	  }

	  protected internal virtual RestException createRestException(string transition, Status status, ProcessEngineException cause)
	  {
		string errorMessage = string.Format("Cannot {0} case execution {1}: {2}", transition, caseExecutionId, cause.Message);
		return new RestException(status, cause, errorMessage);
	  }

	  protected internal virtual void initializeCommand(CaseExecutionCommandBuilder commandBuilder, CaseExecutionTriggerDto triggerDto, string transition)
	  {
		IDictionary<string, TriggerVariableValueDto> variables = triggerDto.Variables;
		if (variables != null && variables.Count > 0)
		{
		  initializeCommandWithVariables(commandBuilder, variables, transition);
		}

		IList<VariableNameDto> deletions = triggerDto.Deletions;
		if (deletions != null && deletions.Count > 0)
		{
		  initializeCommandWithDeletions(commandBuilder, deletions, transition);
		}
	  }

	  protected internal virtual void initializeCommandWithVariables(CaseExecutionCommandBuilder commandBuilder, IDictionary<string, TriggerVariableValueDto> variables, string transition)
	  {
		foreach (string variableName in variables.Keys)
		{
		  try
		  {
			TriggerVariableValueDto variableValue = variables[variableName];
			TypedValue typedValue = variableValue.toTypedValue(engine, objectMapper);

			if (variableValue.Local)
			{
			  commandBuilder.setVariableLocal(variableName, typedValue);

			}
			else
			{
			  commandBuilder.setVariable(variableName, typedValue);
			}

		  }
		  catch (RestException e)
		  {
			string errorMessage = string.Format("Cannot {0} case execution {1} due to invalid variable {2}: {3}", transition, caseExecutionId, variableName, e.Message);
			throw new RestException(e.Status, e, errorMessage);

		  }
		}
	  }

	  protected internal virtual void initializeCommandWithDeletions(CaseExecutionCommandBuilder commandBuilder, IList<VariableNameDto> deletions, string transition)
	  {
		foreach (VariableNameDto variableName in deletions)
		{
		  if (variableName.Local)
		  {
			commandBuilder.removeVariableLocal(variableName.Name);
		  }
		  else
		  {
			commandBuilder.removeVariable(variableName.Name);
		  }
		}
	  }

	  public virtual VariableResource VariablesLocal
	  {
		  get
		  {
			return new LocalCaseExecutionVariablesResource(engine, caseExecutionId, objectMapper);
		  }
	  }

	  public virtual VariableResource Variables
	  {
		  get
		  {
			return new CaseExecutionVariablesResource(engine, caseExecutionId, objectMapper);
		  }
	  }

	}

}