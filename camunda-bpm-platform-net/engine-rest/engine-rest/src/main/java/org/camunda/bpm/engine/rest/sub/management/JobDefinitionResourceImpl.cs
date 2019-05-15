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
namespace org.camunda.bpm.engine.rest.sub.management
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto;
	using JobDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto;
	using JobDefinitionPriorityDto = org.camunda.bpm.engine.rest.dto.runtime.JobDefinitionPriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobDefinitionResourceImpl : JobDefinitionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string jobDefinitionId;

	  public JobDefinitionResourceImpl(ProcessEngine engine, string jobDefinitionId)
	  {
		this.engine = engine;
		this.jobDefinitionId = jobDefinitionId;
	  }

	  public virtual JobDefinitionDto JobDefinition
	  {
		  get
		  {
			ManagementService managementService = engine.ManagementService;
			JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinitionId).singleResult();
    
			if (jobDefinition == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Job Definition with id " + jobDefinitionId + " does not exist");
			}
    
			return JobDefinitionDto.fromJobDefinition(jobDefinition);
		  }
	  }

	  public virtual void updateSuspensionState(JobDefinitionSuspensionStateDto dto)
	  {
		try
		{
		  dto.JobDefinitionId = jobDefinitionId;
		  dto.updateSuspensionState(engine);

		}
		catch (System.ArgumentException e)
		{
		  string message = string.Format("The suspension state of Job Definition with id {0} could not be updated due to: {1}", jobDefinitionId, e.Message);
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, message);
		}

	  }

	  public virtual RetriesDto JobRetries
	  {
		  set
		  {
			try
			{
			  ManagementService managementService = engine.ManagementService;
			  managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, value.Retries.Value);
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

	  public virtual JobDefinitionPriorityDto JobPriority
	  {
		  set
		  {
			try
			{
			  ManagementService managementService = engine.ManagementService;
    
			  if (value.Priority != null)
			  {
				managementService.setOverridingJobPriorityForJobDefinition(jobDefinitionId, value.Priority.Value, value.IncludeJobs);
			  }
			  else
			  {
				if (value.IncludeJobs)
				{
				  throw new InvalidRequestException(Status.BAD_REQUEST, "Cannot reset priority for job definition " + jobDefinitionId + " with includeJobs=true");
				}
    
				managementService.clearOverridingJobPriorityForJobDefinition(jobDefinitionId);
			  }
    
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

	}

}