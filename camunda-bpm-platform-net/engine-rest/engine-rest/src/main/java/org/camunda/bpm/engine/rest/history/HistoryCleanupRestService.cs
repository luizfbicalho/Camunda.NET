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
namespace org.camunda.bpm.engine.rest.history
{


	using HistoryCleanupConfigurationDto = org.camunda.bpm.engine.rest.dto.history.HistoryCleanupConfigurationDto;
	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;

	public interface HistoryCleanupRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.JobDto cleanupAsync(@QueryParam("immediatelyDue") @DefaultValue("true") boolean immediatelyDue);
	  JobDto cleanupAsync(bool immediatelyDue);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/job") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.JobDto findCleanupJob();
	  JobDto findCleanupJob();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/jobs") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.JobDto> findCleanupJobs();
	  IList<JobDto> findCleanupJobs();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/configuration") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.history.HistoryCleanupConfigurationDto getHistoryCleanupConfiguration();
	  HistoryCleanupConfigurationDto HistoryCleanupConfiguration {get;}
	}

	public static class HistoryCleanupRestService_Fields
	{
	  public const string PATH = "/cleanup";
	}

}