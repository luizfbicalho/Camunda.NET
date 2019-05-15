using System;
using System.Collections.Generic;
using System.IO;

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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using StatisticsResultDto = org.camunda.bpm.engine.rest.dto.StatisticsResultDto;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using ActivityStatisticsResultDto = org.camunda.bpm.engine.rest.dto.repository.ActivityStatisticsResultDto;
	using ProcessDefinitionDiagramDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDiagramDto;
	using ProcessDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto;
	using ProcessDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using ProcessInstanceWithVariablesDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceWithVariablesDto;
	using RestartProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.RestartProcessInstanceDto;
	using StartProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.StartProcessInstanceDto;
	using ProcessInstanceModificationInstructionDto = org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationInstructionDto;
	using FormDto = org.camunda.bpm.engine.rest.dto.task.FormDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using ApplicationContextPathUtil = org.camunda.bpm.engine.rest.util.ApplicationContextPathUtil;
	using EncodingUtil = org.camunda.bpm.engine.rest.util.EncodingUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;


	public class ProcessDefinitionResourceImpl : ProcessDefinitionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string processDefinitionId;
	  protected internal string rootResourcePath;
	  protected internal ObjectMapper objectMapper;

	  public ProcessDefinitionResourceImpl(ProcessEngine engine, string processDefinitionId, string rootResourcePath, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.processDefinitionId = processDefinitionId;
		this.rootResourcePath = rootResourcePath;
		this.objectMapper = objectMapper;
	  }

	  public virtual ProcessDefinitionDto ProcessDefinition
	  {
		  get
		  {
			RepositoryService repoService = engine.RepositoryService;
    
			ProcessDefinition definition;
			try
			{
			  definition = repoService.getProcessDefinition(processDefinitionId);
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, e, "No matching definition with id " + processDefinitionId);
			}
    
			ProcessDefinitionDto result = ProcessDefinitionDto.fromProcessDefinition(definition);
    
			return result;
		  }
	  }

	  public virtual Response deleteProcessDefinition(bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		RepositoryService repositoryService = engine.RepositoryService;

		try
		{
		  repositoryService.deleteProcessDefinition(processDefinitionId, cascade, skipCustomListeners, skipIoMappings);
		}
		catch (NotFoundException nfe)
		{
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, nfe, nfe.Message);
		}
		return Response.ok().build();
	  }

	  public virtual ProcessInstanceDto startProcessInstance(UriInfo context, StartProcessInstanceDto parameters)
	  {
		ProcessInstanceWithVariables instance = null;
		try
		{
		  instance = startProcessInstanceAtActivities(parameters);
		}
		catch (AuthorizationException e)
		{
		  throw e;

		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot instantiate process definition {0}: {1}", processDefinitionId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot instantiate process definition {0}: {1}", processDefinitionId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}

		ProcessInstanceDto result;
		if (parameters.WithVariablesInReturn)
		{
		  result = ProcessInstanceWithVariablesDto.fromProcessInstance(instance);
		}
		else
		{
		 result = ProcessInstanceDto.fromProcessInstance(instance);
		}

		URI uri = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.ProcessInstanceRestService_Fields.PATH).path(instance.Id).build();

		result.addReflexiveLink(uri, HttpMethod.GET, "self");

		return result;
	  }

	  protected internal virtual ProcessInstanceWithVariables startProcessInstanceAtActivities(StartProcessInstanceDto dto)
	  {
		IDictionary<string, object> processInstanceVariables = VariableValueDto.toMap(dto.Variables, engine, objectMapper);
		string businessKey = dto.BusinessKey;
		string caseInstanceId = dto.CaseInstanceId;

		ProcessInstantiationBuilder instantiationBuilder = engine.RuntimeService.createProcessInstanceById(processDefinitionId).businessKey(businessKey).caseInstanceId(caseInstanceId).setVariables(processInstanceVariables);

		if (dto.StartInstructions != null && dto.StartInstructions.Count > 0)
		{
		  foreach (ProcessInstanceModificationInstructionDto instruction in dto.StartInstructions)
		  {
			instruction.applyTo(instantiationBuilder, engine, objectMapper);
		  }
		}

		return instantiationBuilder.executeWithVariablesInReturn(dto.SkipCustomListeners, dto.SkipIoMappings);
	  }

	  public virtual ProcessInstanceDto submitForm(UriInfo context, StartProcessInstanceDto parameters)
	  {
		FormService formService = engine.FormService;

		ProcessInstance instance = null;
		try
		{
		  IDictionary<string, object> variables = VariableValueDto.toMap(parameters.Variables, engine, objectMapper);
		  string businessKey = parameters.BusinessKey;
		  if (!string.ReferenceEquals(businessKey, null))
		  {
			instance = formService.submitStartForm(processDefinitionId, businessKey, variables);
		  }
		  else
		  {
			instance = formService.submitStartForm(processDefinitionId, variables);
		  }

		}
		catch (AuthorizationException e)
		{
		  throw e;

		}
		catch (FormFieldValidationException e)
		{
		  string errorMessage = string.Format("Cannot instantiate process definition {0}: {1}", processDefinitionId, e.Message);
		  throw new RestException(Response.Status.BAD_REQUEST, e, errorMessage);

		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot instantiate process definition {0}: {1}", processDefinitionId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot instantiate process definition {0}: {1}", processDefinitionId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}

		ProcessInstanceDto result = ProcessInstanceDto.fromProcessInstance(instance);

		URI uri = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.ProcessInstanceRestService_Fields.PATH).path(instance.Id).build();

		result.addReflexiveLink(uri, HttpMethod.GET, "self");

		return result;
	  }


	  public virtual IList<StatisticsResultDto> getActivityStatistics(bool? includeFailedJobs, bool? includeIncidents, string includeIncidentsForType)
	  {
		if (includeIncidents != null && !string.ReferenceEquals(includeIncidentsForType, null))
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Only one of the query parameter includeIncidents or includeIncidentsForType can be set.");
		}

		ManagementService mgmtService = engine.ManagementService;
		ActivityStatisticsQuery query = mgmtService.createActivityStatisticsQuery(processDefinitionId);

		if (includeFailedJobs != null && includeFailedJobs)
		{
		  query.includeFailedJobs();
		}

		if (includeIncidents != null && includeIncidents)
		{
		  query.includeIncidents();
		}
		else if (!string.ReferenceEquals(includeIncidentsForType, null))
		{
		  query.includeIncidentsForType(includeIncidentsForType);
		}

		IList<ActivityStatistics> queryResults = query.list();

		IList<StatisticsResultDto> results = new List<StatisticsResultDto>();
		foreach (ActivityStatistics queryResult in queryResults)
		{
		  StatisticsResultDto dto = ActivityStatisticsResultDto.fromActivityStatistics(queryResult);
		  results.Add(dto);
		}

		return results;
	  }

	  public virtual ProcessDefinitionDiagramDto ProcessDefinitionBpmn20Xml
	  {
		  get
		  {
			Stream processModelIn = null;
			try
			{
			  processModelIn = engine.RepositoryService.getProcessModel(processDefinitionId);
			  sbyte[] processModel = IoUtil.readInputStream(processModelIn, "processModelBpmn20Xml");
			  return ProcessDefinitionDiagramDto.create(processDefinitionId, StringHelper.NewString(processModel, "UTF-8"));
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, "No matching definition with id " + processDefinitionId);
			}
			catch (UnsupportedEncodingException e)
			{
			  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e);
			}
			finally
			{
			  IoUtil.closeSilently(processModelIn);
			}
		  }
	  }

	  public virtual Response ProcessDefinitionDiagram
	  {
		  get
		  {
			ProcessDefinition definition = engine.RepositoryService.getProcessDefinition(processDefinitionId);
			Stream processDiagram = engine.RepositoryService.getProcessDiagram(processDefinitionId);
			if (processDiagram == null)
			{
			  return Response.noContent().build();
			}
			else
			{
			  string fileName = definition.DiagramResourceName;
			  return Response.ok(processDiagram).header("Content-Disposition", "attachment; filename=" + fileName).type(getMediaTypeForFileSuffix(fileName)).build();
			}
		  }
	  }

	  /// <summary>
	  /// Determines an IANA media type based on the file suffix.
	  /// Hint: as of Java 7 the method Files.probeContentType() provides an implementation based on file type detection.
	  /// </summary>
	  /// <param name="fileName"> </param>
	  /// <returns> content type, defaults to octet-stream </returns>
	  public static string getMediaTypeForFileSuffix(string fileName)
	  {
		string mediaType = "application/octet-stream"; // default
		if (!string.ReferenceEquals(fileName, null))
		{
		  fileName = fileName.ToLower();
		  if (fileName.EndsWith(".png", StringComparison.Ordinal))
		  {
			mediaType = "image/png";
		  }
		  else if (fileName.EndsWith(".svg", StringComparison.Ordinal))
		  {
			mediaType = "image/svg+xml";
		  }
		  else if (fileName.EndsWith(".jpg", StringComparison.Ordinal) || fileName.EndsWith(".jpeg", StringComparison.Ordinal))
		  {
			mediaType = "image/jpeg";
		  }
		  else if (fileName.EndsWith(".gif", StringComparison.Ordinal))
		  {
			mediaType = "image/gif";
		  }
		  else if (fileName.EndsWith(".bmp", StringComparison.Ordinal))
		  {
			mediaType = "image/bmp";
		  }
		}
		return mediaType;
	  }

	  public virtual FormDto StartForm
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.camunda.bpm.engine.FormService formService = engine.getFormService();
			FormService formService = engine.FormService;
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.camunda.bpm.engine.form.StartFormData formData;
			StartFormData formData;
			try
			{
			  formData = formService.getStartFormData(processDefinitionId);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, "Cannot get start form data for process definition " + processDefinitionId);
			}
			FormDto dto = FormDto.fromFormData(formData);
			if (string.ReferenceEquals(dto.Key, null) || dto.Key.Length == 0)
			{
			  if (formData != null && formData.FormFields != null && formData.FormFields.Count > 0)
			  {
				dto.Key = "embedded:engine://engine/:engine/process-definition/" + processDefinitionId + "/rendered-form";
			  }
			}
			dto.ContextPath = ApplicationContextPathUtil.getApplicationPathByProcessDefinitionId(engine, processDefinitionId);
    
			return dto;
		  }
	  }

	  public virtual Response RenderedForm
	  {
		  get
		  {
			FormService formService = engine.FormService;
    
			object startForm = formService.getRenderedStartForm(processDefinitionId);
			if (startForm != null)
			{
			  string content = startForm.ToString();
			  Stream stream = new MemoryStream(content.GetBytes(EncodingUtil.DEFAULT_ENCODING));
			  return Response.ok(stream).type(MediaType.APPLICATION_XHTML_XML).build();
			}
    
			throw new InvalidRequestException(Response.Status.NOT_FOUND, "No matching rendered start form for process definition with the id " + processDefinitionId + " found.");
		  }
	  }

	  public virtual void updateSuspensionState(ProcessDefinitionSuspensionStateDto dto)
	  {
		try
		{
		  dto.ProcessDefinitionId = processDefinitionId;
		  dto.updateSuspensionState(engine);

		}
		catch (System.ArgumentException e)
		{
		  string message = string.Format("The suspension state of Process Definition with id {0} could not be updated due to: {1}", processDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, message);
		}
	  }

	  public virtual void updateHistoryTimeToLive(HistoryTimeToLiveDto historyTimeToLiveDto)
	  {
		engine.RepositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitionId, historyTimeToLiveDto.HistoryTimeToLive);
	  }

	  public virtual IDictionary<string, VariableValueDto> getFormVariables(string variableNames, bool deserializeValues)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.FormService formService = engine.getFormService();
		FormService formService = engine.FormService;
		IList<string> formVariables = null;

		if (!string.ReferenceEquals(variableNames, null))
		{
		  StringListConverter stringListConverter = new StringListConverter();
		  formVariables = stringListConverter.convertQueryParameterToType(variableNames);
		}

		VariableMap startFormVariables = formService.getStartFormVariables(processDefinitionId, formVariables, deserializeValues);

		return VariableValueDto.fromMap(startFormVariables);
	  }

	  public virtual void restartProcessInstance(RestartProcessInstanceDto restartProcessInstanceDto)
	  {
		try
		{
		  createRestartProcessInstanceBuilder(restartProcessInstanceDto).execute();
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto restartProcessInstanceAsync(RestartProcessInstanceDto restartProcessInstanceDto)
	  {
		Batch batch = null;
		try
		{
		   batch = createRestartProcessInstanceBuilder(restartProcessInstanceDto).executeAsync();
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
		}
		return BatchDto.fromBatch(batch);
	  }

	  private RestartProcessInstanceBuilder createRestartProcessInstanceBuilder(RestartProcessInstanceDto restartProcessInstanceDto)
	  {
		RuntimeService runtimeService = engine.RuntimeService;
		RestartProcessInstanceBuilder builder = runtimeService.restartProcessInstances(processDefinitionId);

		if (restartProcessInstanceDto.ProcessInstanceIds != null)
		{
		  builder.processInstanceIds(restartProcessInstanceDto.ProcessInstanceIds);
		}

		if (restartProcessInstanceDto.HistoricProcessInstanceQuery != null)
		{
		  builder.historicProcessInstanceQuery(restartProcessInstanceDto.HistoricProcessInstanceQuery.toQuery(engine));
		}

		if (restartProcessInstanceDto.InitialVariables)
		{
		  builder.initialSetOfVariables();
		}

		if (restartProcessInstanceDto.WithoutBusinessKey)
		{
		  builder.withoutBusinessKey();
		}

		if (restartProcessInstanceDto.SkipCustomListeners)
		{
		  builder.skipCustomListeners();
		}

		if (restartProcessInstanceDto.SkipIoMappings)
		{
		  builder.skipIoMappings();
		}
		restartProcessInstanceDto.applyTo(builder, engine, objectMapper);
		return builder;
	  }

	  public virtual Response DeployedStartForm
	  {
		  get
		  {
			Stream deployedStartForm = null;
			try
			{
			  deployedStartForm = engine.FormService.getDeployedStartForm(processDefinitionId);
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, e.Message);
			}
			catch (NullValueException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
			}
			catch (AuthorizationException e)
			{
			  throw new InvalidRequestException(Response.Status.FORBIDDEN, e.Message);
			}
			catch (BadUserRequestException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
			}
			return Response.ok(deployedStartForm, MediaType.APPLICATION_XHTML_XML).build();
		  }
	  }
	}

}