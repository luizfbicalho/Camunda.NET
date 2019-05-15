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
namespace org.camunda.bpm.engine.rest.sub.runtime
{

	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;
	using JobDuedateDto = org.camunda.bpm.engine.rest.dto.runtime.JobDuedateDto;
	using PriorityDto = org.camunda.bpm.engine.rest.dto.runtime.PriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;
	using JobSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto;

	public interface JobResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.JobDto getJob();
	  JobDto Job {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/stacktrace") @Produces(javax.ws.rs.core.MediaType.TEXT_PLAIN) String getStacktrace();
	  string Stacktrace {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setJobRetries(org.camunda.bpm.engine.rest.dto.runtime.RetriesDto dto);
	  RetriesDto JobRetries {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/execute") void executeJob();
	  void executeJob();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/duedate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setJobDuedate(org.camunda.bpm.engine.rest.dto.runtime.JobDuedateDto dto);
	  JobDuedateDto JobDuedate {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/duedate/recalculate") void recalculateDuedate(@DefaultValue("true") @QueryParam("creationDateBased") boolean creationDateBased);
	  void recalculateDuedate(bool creationDateBased);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto dto);
	  void updateSuspensionState(JobSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/priority") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setJobPriority(org.camunda.bpm.engine.rest.dto.runtime.PriorityDto dto);
	  PriorityDto JobPriority {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE void deleteJob();
	  void deleteJob();

	}

}