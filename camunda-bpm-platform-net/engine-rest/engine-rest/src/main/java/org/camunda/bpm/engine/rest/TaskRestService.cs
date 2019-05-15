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
namespace org.camunda.bpm.engine.rest
{
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using TaskDto = org.camunda.bpm.engine.rest.dto.task.TaskDto;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using TaskReportResource = org.camunda.bpm.engine.rest.sub.task.TaskReportResource;
	using TaskResource = org.camunda.bpm.engine.rest.sub.task.TaskResource;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface TaskRestService
	public interface TaskRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.task.TaskResource getTask(@PathParam("id") String id);
	  TaskResource getTask(string id);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) Object getTasks(@Context Request request, @Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  object getTasks(Request request, UriInfo uriInfo, int? firstResult, int? maxResults);

	  /// <summary>
	  /// Expects the same parameters as <seealso cref="TaskRestService#getTasks(UriInfo, Integer, Integer)"/> (as
	  /// JSON message body) and allows more than one variable check. </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.task.TaskDto> queryTasks(org.camunda.bpm.engine.rest.dto.task.TaskQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<TaskDto> queryTasks(TaskQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getTasksCount(@Context UriInfo uriInfo);
	  CountResultDto getTasksCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryTasksCount(org.camunda.bpm.engine.rest.dto.task.TaskQueryDto query);
	  CountResultDto queryTasksCount(TaskQueryDto query);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/create") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void createTask(org.camunda.bpm.engine.rest.dto.task.TaskDto taskDto);
	  void createTask(TaskDto taskDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/report") org.camunda.bpm.engine.rest.sub.task.TaskReportResource getTaskReportResource();
	  TaskReportResource TaskReportResource {get;}


	}

	public static class TaskRestService_Fields
	{
	  public const string PATH = "/task";
	}

}