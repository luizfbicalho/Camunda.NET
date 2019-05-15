using System;
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
namespace org.camunda.bpm.engine.rest.sub.runtime.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using Query = org.camunda.bpm.engine.query.Query;
	using AbstractQueryDto = org.camunda.bpm.engine.rest.dto.AbstractQueryDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using FilterDto = org.camunda.bpm.engine.rest.dto.runtime.FilterDto;
	using TaskDto = org.camunda.bpm.engine.rest.dto.task.TaskDto;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using org.camunda.bpm.engine.rest.hal;
	using HalTask = org.camunda.bpm.engine.rest.hal.task.HalTask;
	using HalTaskList = org.camunda.bpm.engine.rest.hal.task.HalTaskList;
	using AbstractAuthorizedRestResource = org.camunda.bpm.engine.rest.impl.AbstractAuthorizedRestResource;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	using static org.camunda.bpm.engine.authorization.Permissions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterResourceImpl : AbstractAuthorizedRestResource, FilterResource
	{

	  public static readonly Pattern EMPTY_JSON_BODY = Pattern.compile("\\s*\\{\\s*\\}\\s*");
	  public const string PROPERTIES_VARIABLES_KEY = "variables";
	  public const string PROPERTIES_VARIABLES_NAME_KEY = "name";
	  public static readonly IList<Variant> VARIANTS = Variant.mediaTypes(MediaType.APPLICATION_JSON_TYPE, Hal.APPLICATION_HAL_JSON_TYPE).add().build();

	  protected internal new string relativeRootResourcePath;
	  protected internal FilterService filterService;
	  protected internal Filter dbFilter;

	  public FilterResourceImpl(string processEngineName, ObjectMapper objectMapper, string filterId, string relativeRootResourcePath) : base(processEngineName, FILTER, filterId, objectMapper)
	  {
		this.relativeRootResourcePath = relativeRootResourcePath;
		filterService = processEngine.FilterService;
	  }

	  public virtual FilterDto getFilter(bool? itemCount)
	  {
		Filter filter = DbFilter;
		FilterDto dto = FilterDto.fromFilter(filter);
		if (itemCount != null && itemCount)
		{
		  dto.ItemCount = filterService.count(filter.Id);
		}
		return dto;
	  }

	  protected internal virtual Filter DbFilter
	  {
		  get
		  {
			if (dbFilter == null)
			{
			  dbFilter = filterService.getFilter(resourceId);
    
			  if (dbFilter == null)
			  {
				throw filterNotFound(null);
			  }
			}
			return dbFilter;
		  }
	  }

	  public virtual void deleteFilter()
	  {
		try
		{
		  filterService.deleteFilter(resourceId);
		}
		catch (NullValueException e)
		{
		  throw filterNotFound(e);
		}
	  }

	  public virtual void updateFilter(FilterDto filterDto)
	  {
		Filter filter = DbFilter;

		try
		{
		  filterDto.updateFilter(filter, processEngine);
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Unable to update filter with invalid content");
		}

		filterService.saveFilter(filter);
	  }

	  public virtual object executeSingleResult(Request request)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return executeJsonSingleResult();
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return executeHalSingleResult();
		  }
		}
		throw new InvalidRequestException(Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual object executeJsonSingleResult()
	  {
		return queryJsonSingleResult(null);
	  }

	  public virtual object querySingleResult(Request request, string extendingQuery)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return queryJsonSingleResult(extendingQuery);
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return queryHalSingleResult(extendingQuery);
		  }
		}
		throw new InvalidRequestException(Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual object queryJsonSingleResult(string extendingQuery)
	  {
		object entity = executeFilterSingleResult(extendingQuery);

		if (entity != null)
		{
		  return convertToDto(entity);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual HalResource executeHalSingleResult()
	  {
		return queryHalSingleResult(null);
	  }

	  public virtual HalResource queryHalSingleResult(string extendingQuery)
	  {
		object entity = executeFilterSingleResult(extendingQuery);

		if (entity != null)
		{
		  return convertToHalResource(entity);
		}
		else
		{
		  return EmptyHalResource.INSTANCE;
		}
	  }

	  protected internal virtual object executeFilterSingleResult(string extendingQuery)
	  {
		try
		{
		  return filterService.singleResult(resourceId, convertQuery(extendingQuery));
		}
		catch (NullValueException e)
		{
		  throw filterNotFound(e);
		}
		catch (NotValidException e)
		{
		  throw invalidQuery(e);
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Filter does not returns a valid single result");
		}
	  }

	  public virtual object executeList(Request request, int? firstResult, int? maxResults)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return executeJsonList(firstResult, maxResults);
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return executeHalList(firstResult, maxResults);
		  }
		}
		throw new InvalidRequestException(Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual IList<object> executeJsonList(int? firstResult, int? maxResults)
	  {
		return queryJsonList(null, firstResult, maxResults);
	  }

	  public virtual object queryList(Request request, string extendingQuery, int? firstResult, int? maxResults)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return queryJsonList(extendingQuery, firstResult,maxResults);
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return queryHalList(extendingQuery, firstResult, maxResults);
		  }
		}
		throw new InvalidRequestException(Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual IList<object> queryJsonList(string extendingQuery, int? firstResult, int? maxResults)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<?> entities = executeFilterList(extendingQuery, firstResult, maxResults);
		IList<object> entities = executeFilterList(extendingQuery, firstResult, maxResults);

		if (entities != null && entities.Count > 0)
		{
		  return convertToDtoList(entities);
		}
		else
		{
		  return java.util.Collections.emptyList();
		}
	  }

	  public virtual HalResource executeHalList(int? firstResult, int? maxResults)
	  {
		return queryHalList(null, firstResult, maxResults);
	  }

	  public virtual HalResource queryHalList(string extendingQuery, int? firstResult, int? maxResults)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<?> entities = executeFilterList(extendingQuery, firstResult, maxResults);
		IList<object> entities = executeFilterList(extendingQuery, firstResult, maxResults);
		long count = executeFilterCount(extendingQuery);

		if (entities != null && entities.Count > 0)
		{
		  return convertToHalCollection(entities, count);
		}
		else
		{
		  return new EmptyHalCollection(count);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected List<?> executeFilterList(String extendingQueryString, System.Nullable<int> firstResult, System.Nullable<int> maxResults)
	  protected internal virtual IList<object> executeFilterList(string extendingQueryString, int? firstResult, int? maxResults)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.query.Query<?, ?> extendingQuery = convertQuery(extendingQueryString);
		Query<object, ?> extendingQuery = convertQuery(extendingQueryString);
		try
		{
		  if (firstResult != null || maxResults != null)
		  {
			if (firstResult == null)
			{
			  firstResult = 0;
			}
			if (maxResults == null)
			{
			  maxResults = int.MaxValue;
			}
			return filterService.listPage(resourceId, extendingQuery, firstResult.Value, maxResults.Value);
		  }
		  else
		  {
			return filterService.list(resourceId, extendingQuery);
		  }
		}
		catch (NullValueException e)
		{
		  throw filterNotFound(e);
		}
		catch (NotValidException e)
		{
		  throw invalidQuery(e);
		}
	  }

	  public virtual CountResultDto executeCount()
	  {
		return queryCount(null);
	  }

	  public virtual CountResultDto queryCount(string extendingQuery)
	  {
		return new CountResultDto(executeFilterCount(extendingQuery));
	  }

	  protected internal virtual long executeFilterCount(string extendingQuery)
	  {
		try
		{
		  return filterService.count(resourceId, convertQuery(extendingQuery)).Value;
		}
		catch (NullValueException e)
		{
		  throw filterNotFound(e);
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Filter cannot be extended by an invalid query");
		}
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		ResourceOptionsDto dto = new ResourceOptionsDto();

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.FilterRestService_Fields.PATH).path(resourceId);

		URI baseUri = baseUriBuilder.build();

		if (isAuthorized(READ))
		{
		  dto.addReflexiveLink(baseUri, HttpMethod.GET, "self");

		  URI singleResultUri = baseUriBuilder.clone().path("/singleResult").build();
		  dto.addReflexiveLink(singleResultUri, HttpMethod.GET, "singleResult");
		  dto.addReflexiveLink(singleResultUri, HttpMethod.POST, "singleResult");

		  URI listUri = baseUriBuilder.clone().path("/list").build();
		  dto.addReflexiveLink(listUri, HttpMethod.GET, "list");
		  dto.addReflexiveLink(listUri, HttpMethod.POST, "list");

		  URI countUri = baseUriBuilder.clone().path("/count").build();
		  dto.addReflexiveLink(countUri, HttpMethod.GET, "count");
		  dto.addReflexiveLink(countUri, HttpMethod.POST, "count");
		}

		if (isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(baseUri, HttpMethod.DELETE, "delete");
		}

		if (isAuthorized(UPDATE))
		{
		  dto.addReflexiveLink(baseUri, HttpMethod.PUT, "update");
		}

		return dto;
	  }

	  protected internal virtual Query convertQuery(string queryString)
	  {
		if (isEmptyJson(queryString))
		{
		  return null;
		}
		else
		{
		  string resourceType = DbFilter.ResourceType;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.rest.dto.AbstractQueryDto<?> queryDto = getQueryDtoForQuery(queryString, resourceType);
		  AbstractQueryDto<object> queryDto = getQueryDtoForQuery(queryString, resourceType);
		  queryDto.ObjectMapper = ObjectMapper;
		  return queryDto.toQuery(processEngine);
		}
	  }

	  protected internal virtual object convertToDto(object entity)
	  {
		if (isEntityOfClass(entity, typeof(Task)))
		{
		  return TaskDto.fromEntity((Task) entity);
		}
		else
		{
		  throw unsupportedEntityClass(entity);
		}
	  }

	  protected internal virtual IList<object> convertToDtoList<T1>(IList<T1> entities)
	  {
		IList<object> dtoList = new List<object>();
		foreach (object entity in entities)
		{
		  dtoList.Add(convertToDto(entity));
		}
		return dtoList;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected HalResource<?> convertToHalResource(Object entity)
	  protected internal virtual HalResource<object> convertToHalResource(object entity)
	  {
		if (isEntityOfClass(entity, typeof(Task)))
		{
		  return convertToHalTask((Task) entity);
		}
		else
		{
		  throw unsupportedEntityClass(entity);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.rest.hal.task.HalTask convertToHalTask(org.camunda.bpm.engine.task.Task task)
	  protected internal virtual HalTask convertToHalTask(Task task)
	  {
		HalTask halTask = HalTask.generate(task, ProcessEngine);
		IDictionary<string, IList<VariableInstance>> variableInstances = getVariableInstancesForTasks(halTask);
		if (variableInstances != null)
		{
		  embedVariableValuesInHalTask(halTask, variableInstances);
		}
		return halTask;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected HalCollectionResource convertToHalCollection(List<?> entities, long count)
	  protected internal virtual HalCollectionResource convertToHalCollection<T1>(IList<T1> entities, long count)
	  {
		if (isEntityOfClass(entities[0], typeof(Task)))
		{
		  return convertToHalTaskList((IList<Task>) entities, count);
		}
		else
		{
		  throw unsupportedEntityClass(entities[0]);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.rest.hal.task.HalTaskList convertToHalTaskList(List<org.camunda.bpm.engine.task.Task> tasks, long count)
	  protected internal virtual HalTaskList convertToHalTaskList(IList<Task> tasks, long count)
	  {
		HalTaskList halTasks = HalTaskList.generate(tasks, count, ProcessEngine);
		IDictionary<string, IList<VariableInstance>> variableInstances = getVariableInstancesForTasks(halTasks);
		if (variableInstances != null)
		{
		  foreach (HalTask halTask in (IList<HalTask>) halTasks.getEmbedded("task"))
		  {
			embedVariableValuesInHalTask(halTask, variableInstances);
		  }
		}
		return halTasks;
	  }

	  protected internal virtual void embedVariableValuesInHalTask(HalTask halTask, IDictionary<string, IList<VariableInstance>> variableInstances)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<HalResource<?>> variableValues = getVariableValuesForTask(halTask, variableInstances);
		IList<HalResource<object>> variableValues = getVariableValuesForTask(halTask, variableInstances);
		halTask.addEmbedded("variable", variableValues);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.rest.dto.AbstractQueryDto<?> getQueryDtoForQuery(String queryString, String resourceType)
	  protected internal virtual AbstractQueryDto<object> getQueryDtoForQuery(string queryString, string resourceType)
	  {
		try
		{
		  if (EntityTypes.TASK.Equals(resourceType))
		  {
			return ObjectMapper.readValue(queryString, typeof(TaskQueryDto));
		  }
		  else
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "Queries for resource type '" + resourceType + "' are currently not supported by filters.");
		  }
		}
		catch (IOException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Invalid query for resource type '" + resourceType + "'");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected List<HalResource<?>> getVariableValuesForTask(org.camunda.bpm.engine.rest.hal.task.HalTask halTask, Map<String, List<org.camunda.bpm.engine.runtime.VariableInstance>> variableInstances)
	  protected internal virtual IList<HalResource<object>> getVariableValuesForTask(HalTask halTask, IDictionary<string, IList<VariableInstance>> variableInstances)
	  {
		// converted variables values
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<HalResource<?>> variableValues = new ArrayList<>();
		IList<HalResource<object>> variableValues = new List<HalResource<object>>();

		// variable scope ids to check, ordered by visibility
		LinkedHashSet<string> variableScopeIds = getVariableScopeIds(halTask);

		// names of already converted variables
		ISet<string> knownVariableNames = new HashSet<string>();

		foreach (string variableScopeId in variableScopeIds)
		{
		  if (variableInstances.ContainsKey(variableScopeId))
		  {
			foreach (VariableInstance variableInstance in variableInstances[variableScopeId])
			{
			  if (!knownVariableNames.Contains(variableInstance.Name))
			  {
				variableValues.Add(HalVariableValue.generateVariableValue(variableInstance, variableScopeId));
				knownVariableNames.Add(variableInstance.Name);
			  }
			}
		  }
		}

		return variableValues;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected Map<String, List<org.camunda.bpm.engine.runtime.VariableInstance>> getVariableInstancesForTasks(org.camunda.bpm.engine.rest.hal.task.HalTaskList halTaskList)
	  protected internal virtual IDictionary<string, IList<VariableInstance>> getVariableInstancesForTasks(HalTaskList halTaskList)
	  {
		IList<HalTask> halTasks = (IList<HalTask>) halTaskList.getEmbedded("task");
		return getVariableInstancesForTasks(halTasks.ToArray());
	  }

	  protected internal virtual IDictionary<string, IList<VariableInstance>> getVariableInstancesForTasks(params HalTask[] halTasks)
	  {
		if (halTasks != null && halTasks.Length > 0)
		{
		  IList<string> variableNames = FilterVariableNames;
		  if (variableNames != null && variableNames.Count > 0)
		  {
			LinkedHashSet<string> variableScopeIds = getVariableScopeIds(halTasks);
			return getSortedVariableInstances(variableNames, variableScopeIds);
		  }
		}
		return null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected List<String> getFilterVariableNames()
	  protected internal virtual IList<string> FilterVariableNames
	  {
		  get
		  {
			IDictionary<string, object> properties = DbFilter.Properties;
			if (properties != null)
			{
			  try
			  {
				IList<IDictionary<string, object>> variables = (IList<IDictionary<string, object>>) properties[PROPERTIES_VARIABLES_KEY];
				return collectVariableNames(variables);
			  }
			  catch (Exception e)
			  {
				throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, e, "Filter property '" + PROPERTIES_VARIABLES_KEY + "' has to be a list of variable definitions with a '" + PROPERTIES_VARIABLES_NAME_KEY + "' property");
			  }
			}
			else
			{
			  return null;
			}
		  }
	  }

	  private IList<string> collectVariableNames(IList<IDictionary<string, object>> variables)
	  {
		if (variables != null && variables.Count > 0)
		{
		  IList<string> variableNames = new List<string>();
		  foreach (IDictionary<string, object> variable in variables)
		  {
			variableNames.Add((string) variable[PROPERTIES_VARIABLES_NAME_KEY]);
		  }
		  return variableNames;
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual LinkedHashSet<string> getVariableScopeIds(params HalTask[] halTasks)
	  {
		// collect scope ids
		// the ordering is important because it specifies which variables are visible from a single task
		LinkedHashSet<string> variableScopeIds = new LinkedHashSet<string>();
		if (halTasks != null && halTasks.Length > 0)
		{
		  foreach (HalTask halTask in halTasks)
		  {
			variableScopeIds.add(halTask.Id);
			variableScopeIds.add(halTask.ExecutionId);
			variableScopeIds.add(halTask.ProcessInstanceId);
			variableScopeIds.add(halTask.CaseExecutionId);
			variableScopeIds.add(halTask.CaseInstanceId);
		  }
		}

		// remove null from set which was probably added due an unset id
		variableScopeIds.remove(null);

		return variableScopeIds;
	  }

	  protected internal virtual IDictionary<string, IList<VariableInstance>> getSortedVariableInstances(ICollection<string> variableNames, ICollection<string> variableScopeIds)
	  {
		IList<VariableInstance> variableInstances = queryVariablesInstancesByVariableScopeIds(variableNames, variableScopeIds);
		IDictionary<string, IList<VariableInstance>> sortedVariableInstances = new Dictionary<string, IList<VariableInstance>>();
		foreach (VariableInstance variableInstance in variableInstances)
		{
		  string variableScopeId = ((VariableInstanceEntity) variableInstance).VariableScopeId;
		  if (!sortedVariableInstances.ContainsKey(variableScopeId))
		  {
			sortedVariableInstances[variableScopeId] = new List<VariableInstance>();
		  }
		  sortedVariableInstances[variableScopeId].Add(variableInstance);
		}
		return sortedVariableInstances;
	  }

	  protected internal virtual IList<VariableInstance> queryVariablesInstancesByVariableScopeIds(ICollection<string> variableNames, ICollection<string> variableScopeIds)
	  {

		return ProcessEngine.RuntimeService.createVariableInstanceQuery().disableBinaryFetching().disableCustomObjectDeserialization().variableNameIn(variableNames.toArray(new string[variableNames.Count])).variableScopeIdIn(variableScopeIds.toArray(new string[variableScopeIds.Count])).list();

	  }

	  protected internal virtual bool isEntityOfClass(object entity, Type entityClass)
	  {
		return entityClass.IsAssignableFrom(entity.GetType());
	  }

	  protected internal virtual bool isEmptyJson(string jsonString)
	  {
		return string.ReferenceEquals(jsonString, null) || jsonString.Trim().Length == 0 || EMPTY_JSON_BODY.matcher(jsonString).matches();
	  }

	  protected internal virtual InvalidRequestException filterNotFound(Exception cause)
	  {
		return new InvalidRequestException(Status.NOT_FOUND, cause, "Filter with id '" + resourceId + "' does not exist.");
	  }

	  protected internal virtual InvalidRequestException invalidQuery(Exception cause)
	  {
		return new InvalidRequestException(Status.BAD_REQUEST, cause, "Filter cannot be extended by an invalid query");
	  }

	  protected internal virtual InvalidRequestException unsupportedEntityClass(object entity)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		return new InvalidRequestException(Status.BAD_REQUEST, "Entities of class '" + entity.GetType().FullName + "' are currently not supported by filters.");
	  }

	}

}