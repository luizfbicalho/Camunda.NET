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
namespace org.camunda.bpm.engine.rest.sub.externaltask.impl
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using CompleteExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.CompleteExternalTaskDto;
	using ExtendLockOnExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExtendLockOnExternalTaskDto;
	using ExternalTaskBpmnError = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskBpmnError;
	using ExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto;
	using ExternalTaskFailureDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskFailureDto;
	using PriorityDto = org.camunda.bpm.engine.rest.dto.runtime.PriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskResourceImpl : ExternalTaskResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string externalTaskId;
	  protected internal ObjectMapper objectMapper;

	  public ExternalTaskResourceImpl(ProcessEngine engine, string externalTaskId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.externalTaskId = externalTaskId;
		this.objectMapper = objectMapper;
	  }

	  public virtual ExternalTaskDto ExternalTask
	  {
		  get
		  {
			ExternalTask task = engine.ExternalTaskService.createExternalTaskQuery().externalTaskId(externalTaskId).singleResult();
    
			if (task == null)
			{
			  throw new RestException(Status.NOT_FOUND, "External task with id " + externalTaskId + " does not exist");
			}
    
			return ExternalTaskDto.fromExternalTask(task);
		  }
	  }

	  public virtual string ErrorDetails
	  {
		  get
		  {
			ExternalTaskService externalTaskService = engine.ExternalTaskService;
    
			try
			{
			  return externalTaskService.getExternalTaskErrorDetails(externalTaskId);
			}
			catch (NotFoundException e)
			{
			  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
			}
		  }
	  }

	  public virtual RetriesDto Retries
	  {
		  set
		  {
			ExternalTaskService externalTaskService = engine.ExternalTaskService;
			int? retries = value.Retries;
    
			if (retries == null)
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "The number of retries cannot be null.");
			}
    
			try
			{
			  externalTaskService.setRetries(externalTaskId, retries);
			}
			catch (NotFoundException e)
			{
			  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
			}
		  }
	  }

	  public virtual PriorityDto Priority
	  {
		  set
		  {
			ExternalTaskService externalTaskService = engine.ExternalTaskService;
    
			try
			{
			  externalTaskService.setPriority(externalTaskId, value.Priority.Value);
			}
			catch (NotFoundException e)
			{
			  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
			}
		  }
	  }

	  public virtual void complete(CompleteExternalTaskDto dto)
	  {
		ExternalTaskService externalTaskService = engine.ExternalTaskService;

		VariableMap variables = VariableValueDto.toMap(dto.Variables, engine, objectMapper);
		VariableMap localVariables = VariableValueDto.toMap(dto.LocalVariables, engine, objectMapper);

		try
		{
		  externalTaskService.complete(externalTaskId, dto.WorkerId, variables, localVariables);
		}
		catch (NotFoundException e)
		{
		  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
		}
		catch (BadUserRequestException e)
		{
		  throw new RestException(Status.BAD_REQUEST, e, e.Message);
		}

	  }

	  public virtual void handleFailure(ExternalTaskFailureDto dto)
	  {
		ExternalTaskService externalTaskService = engine.ExternalTaskService;

		try
		{
		  externalTaskService.handleFailure(externalTaskId, dto.WorkerId, dto.ErrorMessage, dto.ErrorDetails, dto.Retries, dto.RetryTimeout);
		}
		catch (NotFoundException e)
		{
		  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
		}
		catch (BadUserRequestException e)
		{
		  throw new RestException(Status.BAD_REQUEST, e, e.Message);
		}
	  }

	  public virtual void handleBpmnError(ExternalTaskBpmnError dto)
	  {
		ExternalTaskService externalTaskService = engine.ExternalTaskService;

		try
		{
		  externalTaskService.handleBpmnError(externalTaskId, dto.WorkerId, dto.ErrorCode, dto.ErrorMessage, VariableValueDto.toMap(dto.Variables, engine, objectMapper));
		}
		catch (NotFoundException e)
		{
		  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
		}
		catch (BadUserRequestException e)
		{
		  throw new RestException(Status.BAD_REQUEST, e, e.Message);
		}
	  }

	  public virtual void unlock()
	  {
		ExternalTaskService externalTaskService = engine.ExternalTaskService;

		try
		{
		  externalTaskService.unlock(externalTaskId);
		}
		catch (NotFoundException e)
		{
		  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
		}
	  }

	  public virtual void extendLock(ExtendLockOnExternalTaskDto extendLockDto)
	  {
		ExternalTaskService externalTaskService = engine.ExternalTaskService;

		try
		{
		  externalTaskService.extendLock(externalTaskId, extendLockDto.WorkerId, extendLockDto.NewDuration);
		}
		catch (NotFoundException e)
		{
		  throw new RestException(Status.NOT_FOUND, e, "External task with id " + externalTaskId + " does not exist");
		}
		catch (BadUserRequestException e)
		{
		  throw new RestException(Status.BAD_REQUEST, e, e.Message);
		}
	  }
	}

}