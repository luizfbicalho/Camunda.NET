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
namespace org.camunda.bpm.engine.rest.impl
{


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using TaskDto = org.camunda.bpm.engine.rest.dto.task.TaskDto;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using HalTaskList = org.camunda.bpm.engine.rest.hal.task.HalTaskList;
	using TaskReportResource = org.camunda.bpm.engine.rest.sub.task.TaskReportResource;
	using TaskResource = org.camunda.bpm.engine.rest.sub.task.TaskResource;
	using TaskReportResourceImpl = org.camunda.bpm.engine.rest.sub.task.impl.TaskReportResourceImpl;
	using TaskResourceImpl = org.camunda.bpm.engine.rest.sub.task.impl.TaskResourceImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class TaskRestServiceImpl : AbstractRestProcessEngineAware, TaskRestService
	{

	  public static readonly IList<Variant> VARIANTS = Variant.mediaTypes(MediaType.APPLICATION_JSON_TYPE, Hal.APPLICATION_HAL_JSON_TYPE).add().build();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public TaskRestServiceImpl(String engineName, final com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public TaskRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual object getTasks(Request request, UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return getJsonTasks(uriInfo, firstResult, maxResults);
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return getHalTasks(uriInfo, firstResult, maxResults);
		  }
		}
		throw new InvalidRequestException(Response.Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual IList<TaskDto> getJsonTasks(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		TaskQueryDto queryDto = new TaskQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryTasks(queryDto, firstResult, maxResults);
	  }

	  public virtual HalTaskList getHalTasks(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		TaskQueryDto queryDto = new TaskQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		TaskQuery query = queryDto.toQuery(engine);

		// get list of tasks
		IList<Task> matchingTasks = executeTaskQuery(firstResult, maxResults, query);

		// get total count
		long count = query.count();

		return HalTaskList.generate(matchingTasks, count, engine);
	  }

	  public virtual IList<TaskDto> queryTasks(TaskQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		TaskQuery query = queryDto.toQuery(engine);

		IList<Task> matchingTasks = executeTaskQuery(firstResult, maxResults, query);

		IList<TaskDto> tasks = new List<TaskDto>();
		foreach (Task task in matchingTasks)
		{
		  TaskDto returnTask = TaskDto.fromEntity(task);
		  tasks.Add(returnTask);
		}

		return tasks;
	  }

	  protected internal virtual IList<Task> executeTaskQuery(int? firstResult, int? maxResults, TaskQuery query)
	  {

		// enable initialization of form key:
		query.initializeFormKeys();

		IList<Task> matchingTasks;
		if (firstResult != null || maxResults != null)
		{
		  matchingTasks = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingTasks = query.list();
		}
		return matchingTasks;
	  }

	  protected internal virtual IList<Task> executePaginatedQuery(TaskQuery query, int? firstResult, int? maxResults)
	  {
		if (firstResult == null)
		{
		  firstResult = 0;
		}
		if (maxResults == null)
		{
		  maxResults = int.MaxValue;
		}
		return query.listPage(firstResult, maxResults);
	  }

	  public virtual CountResultDto getTasksCount(UriInfo uriInfo)
	  {
		TaskQueryDto queryDto = new TaskQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryTasksCount(queryDto);
	  }

	  public virtual CountResultDto queryTasksCount(TaskQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		TaskQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual TaskResource getTask(string id)
	  {
		return new TaskResourceImpl(ProcessEngine, id, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual void createTask(TaskDto taskDto)
	  {
		ProcessEngine engine = ProcessEngine;
		TaskService taskService = engine.TaskService;

		Task newTask = taskService.newTask(taskDto.Id);
		taskDto.updateTask(newTask);

		try
		{
		  taskService.saveTask(newTask);

		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, "Could not save task: " + e.Message);
		}

	  }

	  public virtual TaskReportResource TaskReportResource
	  {
		  get
		  {
			return new TaskReportResourceImpl(ProcessEngine);
		  }
	  }
	}

}