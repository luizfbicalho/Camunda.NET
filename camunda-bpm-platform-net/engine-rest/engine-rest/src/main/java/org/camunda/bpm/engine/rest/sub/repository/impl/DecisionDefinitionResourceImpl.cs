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


	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnDecisionResultEntries = org.camunda.bpm.dmn.engine.DmnDecisionResultEntries;
	using DmnEngineException = org.camunda.bpm.dmn.engine.DmnEngineException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using EvaluateDecisionDto = org.camunda.bpm.engine.rest.dto.dmn.EvaluateDecisionDto;
	using DecisionDefinitionDiagramDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDiagramDto;
	using DecisionDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DecisionDefinitionResourceImpl : DecisionDefinitionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string decisionDefinitionId;
	  protected internal string rootResourcePath;
	  protected internal ObjectMapper objectMapper;

	  public DecisionDefinitionResourceImpl(ProcessEngine engine, string decisionDefinitionId, string rootResourcePath, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.decisionDefinitionId = decisionDefinitionId;
		this.rootResourcePath = rootResourcePath;
		this.objectMapper = objectMapper;
	  }

	  public virtual DecisionDefinitionDto DecisionDefinition
	  {
		  get
		  {
			RepositoryService repositoryService = engine.RepositoryService;
    
			DecisionDefinition definition = null;
    
			try
			{
			  definition = repositoryService.getDecisionDefinition(decisionDefinitionId);
    
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, e, e.Message);
    
			}
			catch (NotValidException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, e.Message);
    
			}
			catch (ProcessEngineException e)
			{
			  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e);
    
			}
    
			return DecisionDefinitionDto.fromDecisionDefinition(definition);
		  }
	  }

	  public virtual DecisionDefinitionDiagramDto DecisionDefinitionDmnXml
	  {
		  get
		  {
			Stream decisionModelInputStream = null;
			try
			{
			  decisionModelInputStream = engine.RepositoryService.getDecisionModel(decisionDefinitionId);
    
			  sbyte[] decisionModel = IoUtil.readInputStream(decisionModelInputStream, "decisionModelDmnXml");
			  return DecisionDefinitionDiagramDto.create(decisionDefinitionId, StringHelper.NewString(decisionModel, "UTF-8"));
    
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, e, e.Message);
    
			}
			catch (NotValidException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, e.Message);
    
			}
			catch (ProcessEngineException e)
			{
			  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e);
    
			}
			catch (UnsupportedEncodingException e)
			{
			  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e);
    
			}
			finally
			{
			  IoUtil.closeSilently(decisionModelInputStream);
			}
		  }
	  }

	  public virtual Response DecisionDefinitionDiagram
	  {
		  get
		  {
			DecisionDefinition definition = engine.RepositoryService.getDecisionDefinition(decisionDefinitionId);
			Stream decisionDiagram = engine.RepositoryService.getDecisionDiagram(decisionDefinitionId);
			if (decisionDiagram == null)
			{
			  return Response.noContent().build();
			}
			else
			{
			  string fileName = definition.DiagramResourceName;
			  return Response.ok(decisionDiagram).header("Content-Disposition", "attachment; filename=" + fileName).type(ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix(fileName)).build();
			}
		  }
	  }

	  public virtual IList<IDictionary<string, VariableValueDto>> evaluateDecision(UriInfo context, EvaluateDecisionDto parameters)
	  {
		DecisionService decisionService = engine.DecisionService;

		IDictionary<string, object> variables = VariableValueDto.toMap(parameters.Variables, engine, objectMapper);

		try
		{
		  DmnDecisionResult decisionResult = decisionService.evaluateDecisionById(decisionDefinitionId).variables(variables).evaluate();

		  return createDecisionResultDto(decisionResult);

		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (NotFoundException e)
		{
		  string errorMessage = string.Format("Cannot evaluate decision {0}: {1}", decisionDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, e, errorMessage);
		}
		catch (NotValidException e)
		{
		  string errorMessage = string.Format("Cannot evaluate decision {0}: {1}", decisionDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, errorMessage);
		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot evaluate decision {0}: {1}", decisionDefinitionId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}
		catch (DmnEngineException e)
		{
		  string errorMessage = string.Format("Cannot evaluate decision {0}: {1}", decisionDefinitionId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}
	  }

	  public virtual void updateHistoryTimeToLive(HistoryTimeToLiveDto historyTimeToLiveDto)
	  {
		engine.RepositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, historyTimeToLiveDto.HistoryTimeToLive);
	  }

	  protected internal virtual IList<IDictionary<string, VariableValueDto>> createDecisionResultDto(DmnDecisionResult decisionResult)
	  {
		IList<IDictionary<string, VariableValueDto>> dto = new List<IDictionary<string, VariableValueDto>>();

		foreach (DmnDecisionResultEntries entries in decisionResult)
		{
		  IDictionary<string, VariableValueDto> resultEntriesDto = createResultEntriesDto(entries);
		  dto.Add(resultEntriesDto);
		}

		return dto;
	  }

	  protected internal virtual IDictionary<string, VariableValueDto> createResultEntriesDto(DmnDecisionResultEntries entries)
	  {
		VariableMap variableMap = Variables.createVariables();

		foreach (string key in entries.Keys)
		{
		  TypedValue typedValue = entries.getEntryTyped(key);
		  variableMap.putValueTyped(key, typedValue);
		}

		return VariableValueDto.fromMap(variableMap);
	  }

	}

}