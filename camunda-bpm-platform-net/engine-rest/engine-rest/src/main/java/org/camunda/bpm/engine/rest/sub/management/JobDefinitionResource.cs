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
	using JobDefinitionDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto;
	using JobDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto;
	using JobDefinitionPriorityDto = org.camunda.bpm.engine.rest.dto.runtime.JobDefinitionPriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public interface JobDefinitionResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto getJobDefinition();
	  JobDefinitionDto JobDefinition {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto dto);
	  void updateSuspensionState(JobDefinitionSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setJobRetries(org.camunda.bpm.engine.rest.dto.runtime.RetriesDto dto);
	  RetriesDto JobRetries {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/jobPriority") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setJobPriority(org.camunda.bpm.engine.rest.dto.runtime.JobDefinitionPriorityDto dto);
	  JobDefinitionPriorityDto JobPriority {set;}

	}

}