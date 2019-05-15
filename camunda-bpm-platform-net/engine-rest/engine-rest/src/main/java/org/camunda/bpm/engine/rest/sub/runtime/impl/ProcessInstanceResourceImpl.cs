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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ActivityInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ActivityInstanceDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using ProcessInstanceSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto;
	using ProcessInstanceModificationDto = org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ProcessInstanceResourceImpl : ProcessInstanceResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string processInstanceId;
	  protected internal ObjectMapper objectMapper;

	  public ProcessInstanceResourceImpl(ProcessEngine engine, string processInstanceId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.processInstanceId = processInstanceId;
		this.objectMapper = objectMapper;
	  }

	  public virtual ProcessInstanceDto ProcessInstance
	  {
		  get
		  {
			RuntimeService runtimeService = engine.RuntimeService;
			ProcessInstance instance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
    
			if (instance == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Process instance with id " + processInstanceId + " does not exist");
			}
    
			ProcessInstanceDto result = ProcessInstanceDto.fromProcessInstance(instance);
			return result;
		  }
	  }

	  public virtual void deleteProcessInstance(bool skipCustomListeners, bool skipIoMappings, bool skipSubprocesses, bool failIfNotExists)
	  {
		RuntimeService runtimeService = engine.RuntimeService;
		try
		{
		  if (failIfNotExists)
		  {
			runtimeService.deleteProcessInstance(processInstanceId, null, skipCustomListeners, true, skipIoMappings, skipSubprocesses);
		  }
		  else
		  {
			runtimeService.deleteProcessInstanceIfExists(processInstanceId, null, skipCustomListeners, true, skipIoMappings, skipSubprocesses);
		  }
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e, "Process instance with id " + processInstanceId + " does not exist");
		}

	  }

	  public virtual VariableResource VariablesResource
	  {
		  get
		  {
			return new ExecutionVariablesResource(engine, processInstanceId, true, objectMapper);
		  }
	  }

	  public virtual ActivityInstanceDto ActivityInstanceTree
	  {
		  get
		  {
			RuntimeService runtimeService = engine.RuntimeService;
    
			ActivityInstance activityInstance = null;
    
			try
			{
			  activityInstance = runtimeService.getActivityInstance(processInstanceId);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, e, e.Message);
			}
    
			if (activityInstance == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Process instance with id " + processInstanceId + " does not exist");
			}
    
			ActivityInstanceDto result = ActivityInstanceDto.fromActivityInstance(activityInstance);
			return result;
		  }
	  }

	  public virtual void updateSuspensionState(ProcessInstanceSuspensionStateDto dto)
	  {
		dto.ProcessInstanceId = processInstanceId;
		dto.updateSuspensionState(engine);
	  }

	  public virtual void modifyProcessInstance(ProcessInstanceModificationDto dto)
	  {
		if (dto.Instructions != null && dto.Instructions.Count > 0)
		{
		  ProcessInstanceModificationBuilder modificationBuilder = engine.RuntimeService.createProcessInstanceModification(processInstanceId);

		  dto.applyTo(modificationBuilder, engine, objectMapper);

		  modificationBuilder.execute(dto.SkipCustomListeners, dto.SkipIoMappings);
		}
	  }

	  public virtual BatchDto modifyProcessInstanceAsync(ProcessInstanceModificationDto dto)
	  {
		Batch batch = null;
		if (dto.Instructions != null && dto.Instructions.Count > 0)
		{
		  ProcessInstanceModificationBuilder modificationBuilder = engine.RuntimeService.createProcessInstanceModification(processInstanceId);

		  dto.applyTo(modificationBuilder, engine, objectMapper);

		  try
		  {
			batch = modificationBuilder.executeAsync(dto.SkipCustomListeners, dto.SkipIoMappings);
		  }
		  catch (BadUserRequestException e)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		  }
		  return BatchDto.fromBatch(batch);
		}

		throw new InvalidRequestException(Status.BAD_REQUEST, "The provided instuctions are invalid.");
	  }
	}

}