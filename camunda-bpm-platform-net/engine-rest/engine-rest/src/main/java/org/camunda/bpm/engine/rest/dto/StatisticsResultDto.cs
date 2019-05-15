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
namespace org.camunda.bpm.engine.rest.dto
{
	using ActivityStatisticsResultDto = org.camunda.bpm.engine.rest.dto.repository.ActivityStatisticsResultDto;
	using IncidentStatisticsResultDto = org.camunda.bpm.engine.rest.dto.repository.IncidentStatisticsResultDto;
	using ProcessDefinitionStatisticsResultDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionStatisticsResultDto;

	using JsonSubTypes = com.fasterxml.jackson.annotation.JsonSubTypes;
	using JsonTypeInfo = com.fasterxml.jackson.annotation.JsonTypeInfo;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.CLASS) @JsonSubTypes({ @JsonSubTypes.Type(value = ActivityStatisticsResultDto.class), @JsonSubTypes.Type(value = ProcessDefinitionStatisticsResultDto.class) }) public abstract class StatisticsResultDto
	public abstract class StatisticsResultDto
	{

	  protected internal string id;
	  protected internal int? instances;
	  protected internal int? failedJobs;
	  protected internal IList<IncidentStatisticsResultDto> incidents;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual int? Instances
	  {
		  get
		  {
			return instances;
		  }
		  set
		  {
			this.instances = value;
		  }
	  }
	  public virtual int? FailedJobs
	  {
		  get
		  {
			return failedJobs;
		  }
		  set
		  {
			this.failedJobs = value;
		  }
	  }
	  public virtual IList<IncidentStatisticsResultDto> Incidents
	  {
		  get
		  {
			return incidents;
		  }
		  set
		  {
			this.incidents = value;
		  }
	  }

	}

}