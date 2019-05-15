using System;

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

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;
	using JobDuedateDto = org.camunda.bpm.engine.rest.dto.runtime.JobDuedateDto;
	using PriorityDto = org.camunda.bpm.engine.rest.dto.runtime.PriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;
	using JobSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using Job = org.camunda.bpm.engine.runtime.Job;

	public class JobResourceImpl : JobResource
	{

	  private ProcessEngine engine;
	  private string jobId;

	  public JobResourceImpl(ProcessEngine engine, string jobId)
	  {
		this.engine = engine;
		this.jobId = jobId;
	  }

	  public virtual JobDto Job
	  {
		  get
		  {
			ManagementService managementService = engine.ManagementService;
			Job job = managementService.createJobQuery().jobId(jobId).singleResult();
    
			if (job == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Job with id " + jobId + " does not exist");
			}
    
			return JobDto.fromJob(job);
		  }
	  }

	  public virtual string Stacktrace
	  {
		  get
		  {
			try
			{
			  ManagementService managementService = engine.ManagementService;
			  string stacktrace = managementService.getJobExceptionStacktrace(jobId);
			  return stacktrace;
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
			}
		  }
	  }

	  public virtual RetriesDto JobRetries
	  {
		  set
		  {
			try
			{
			  ManagementService managementService = engine.ManagementService;
			  managementService.setJobRetries(jobId, value.Retries);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, e.Message);
			}
		  }
	  }

	  public virtual void executeJob()
	  {
		try
		{
		  ManagementService managementService = engine.ManagementService;
		  managementService.executeJob(this.jobId);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
		}
		catch (Exception r)
		{
		  throw new RestException(Status.INTERNAL_SERVER_ERROR, r.Message);
		}
	  }

	  public virtual JobDuedateDto JobDuedate
	  {
		  set
		  {
			try
			{
			  ManagementService managementService = engine.ManagementService;
			  managementService.setJobDuedate(jobId, value.Duedate);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, e.Message);
			}
		  }
	  }

	  public virtual void recalculateDuedate(bool creationDateBased)
	  {
		try
		{
		  ManagementService managementService = engine.ManagementService;
		  managementService.recalculateJobDuedate(jobId, creationDateBased);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (NotFoundException e)
		{ // rewrite status code from bad request (400) to not found (404)
		  throw new InvalidRequestException(Status.NOT_FOUND, e, e.Message);
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, e.Message);
		}
	  }

	  public virtual void updateSuspensionState(JobSuspensionStateDto dto)
	  {
		dto.JobId = jobId;
		dto.updateSuspensionState(engine);
	  }

	  public virtual PriorityDto JobPriority
	  {
		  set
		  {
			if (value.Priority == null)
			{
			  throw new RestException(Status.BAD_REQUEST, "Priority for job '" + jobId + "' cannot be null.");
			}
    
			try
			{
			  ManagementService managementService = engine.ManagementService;
			  managementService.setJobPriority(jobId, value.Priority.Value);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
			}
			catch (ProcessEngineException e)
			{
			  throw new RestException(Status.INTERNAL_SERVER_ERROR, e.Message);
			}
		  }
	  }

	  public virtual void deleteJob()
	  {
		try
		{
		  engine.ManagementService.deleteJob(jobId);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (NullValueException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
		}
		catch (ProcessEngineException e)
		{
		  throw new RestException(Status.INTERNAL_SERVER_ERROR, e.Message);
		}
	  }

	}

}