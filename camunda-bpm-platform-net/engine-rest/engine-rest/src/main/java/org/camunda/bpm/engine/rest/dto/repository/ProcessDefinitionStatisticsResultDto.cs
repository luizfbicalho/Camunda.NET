﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.dto.repository
{
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;

	public class ProcessDefinitionStatisticsResultDto : StatisticsResultDto
	{

	  private ProcessDefinitionDto definition;

	  public virtual ProcessDefinitionDto Definition
	  {
		  get
		  {
			return definition;
		  }
		  set
		  {
			this.definition = value;
		  }
	  }


	  public static ProcessDefinitionStatisticsResultDto fromProcessDefinitionStatistics(ProcessDefinitionStatistics statistics)
	  {
		ProcessDefinitionStatisticsResultDto dto = new ProcessDefinitionStatisticsResultDto();

		dto.definition = ProcessDefinitionDto.fromProcessDefinition(statistics);
		dto.id = statistics.Id;
		dto.instances = statistics.Instances;
		dto.failedJobs = statistics.FailedJobs;

		dto.incidents = new List<IncidentStatisticsResultDto>();
		foreach (IncidentStatistics incident in statistics.IncidentStatistics)
		{
		  IncidentStatisticsResultDto incidentDto = IncidentStatisticsResultDto.fromIncidentStatistics(incident);
		  dto.incidents.Add(incidentDto);
		}

		return dto;
	  }
	}

}