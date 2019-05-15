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

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class IncidentStatisticsResultDto
	{

	  protected internal string incidentType;
	  protected internal int? incidentCount;

	  public IncidentStatisticsResultDto()
	  {
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
		  set
		  {
			this.incidentType = value;
		  }
	  }


	  public virtual int? IncidentCount
	  {
		  get
		  {
			return incidentCount;
		  }
		  set
		  {
			this.incidentCount = value;
		  }
	  }


	  public static IncidentStatisticsResultDto fromIncidentStatistics(IncidentStatistics statistics)
	  {
		IncidentStatisticsResultDto dto = new IncidentStatisticsResultDto();
		dto.IncidentType = statistics.IncidentType;
		dto.IncidentCount = statistics.IncidentCount;
		return dto;
	  }

	}

}