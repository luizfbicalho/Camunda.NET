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
namespace org.camunda.bpm.engine.rest.sub.repository.impl
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using IncidentDto = org.camunda.bpm.engine.rest.dto.runtime.IncidentDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using IncidentResource = org.camunda.bpm.engine.rest.sub.runtime.IncidentResource;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class IncidentResourceImpl : IncidentResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string incidentId;
	  protected internal ObjectMapper objectMapper;

	  public IncidentResourceImpl(ProcessEngine engine, string incidentId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.incidentId = incidentId;
		this.objectMapper = objectMapper;
	  }

	  public virtual IncidentDto Incident
	  {
		  get
		  {
			Incident incident = engine.RuntimeService.createIncidentQuery().incidentId(incidentId).singleResult();
			if (incident == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "No matching incident with id " + incidentId);
			}
			return IncidentDto.fromIncident(incident);
		  }
	  }

	  public virtual void resolveIncident()
	  {
		try
		{
		  engine.RuntimeService.resolveIncident(incidentId);
		}
		catch (NotFoundException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }
	}

}