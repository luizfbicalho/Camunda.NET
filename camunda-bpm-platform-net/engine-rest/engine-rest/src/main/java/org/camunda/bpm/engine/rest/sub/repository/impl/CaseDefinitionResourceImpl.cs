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
	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using CaseDefinitionDiagramDto = org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionDiagramDto;
	using CaseDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionDto;
	using CaseInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.CaseInstanceDto;
	using CreateCaseInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.CreateCaseInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;


	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionResourceImpl : CaseDefinitionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string caseDefinitionId;
	  protected internal string rootResourcePath;
	  protected internal ObjectMapper objectMapper;

	  public CaseDefinitionResourceImpl(ProcessEngine engine, string caseDefinitionId, string rootResourcePath, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.caseDefinitionId = caseDefinitionId;
		this.rootResourcePath = rootResourcePath;
		this.objectMapper = objectMapper;
	  }

	  public virtual CaseDefinitionDto CaseDefinition
	  {
		  get
		  {
			RepositoryService repositoryService = engine.RepositoryService;
    
			CaseDefinition definition = null;
    
			try
			{
			  definition = repositoryService.getCaseDefinition(caseDefinitionId);
    
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
    
			return CaseDefinitionDto.fromCaseDefinition(definition);
		  }
	  }

	  public virtual CaseDefinitionDiagramDto CaseDefinitionCmmnXml
	  {
		  get
		  {
			Stream caseModelInputStream = null;
			try
			{
			  caseModelInputStream = engine.RepositoryService.getCaseModel(caseDefinitionId);
    
			  sbyte[] caseModel = IoUtil.readInputStream(caseModelInputStream, "caseModelCmmnXml");
			  return CaseDefinitionDiagramDto.create(caseDefinitionId, StringHelper.NewString(caseModel, "UTF-8"));
    
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
			  IoUtil.closeSilently(caseModelInputStream);
			}
		  }
	  }

	  public virtual CaseInstanceDto createCaseInstance(UriInfo context, CreateCaseInstanceDto parameters)
	  {
		CaseService caseService = engine.CaseService;

		CaseInstance instance = null;
		try
		{

		  string businessKey = parameters.BusinessKey;
		  VariableMap variables = VariableValueDto.toMap(parameters.Variables, engine, objectMapper);

		  instance = caseService.withCaseDefinition(caseDefinitionId).businessKey(businessKey).setVariables(variables).create();

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot instantiate case definition {0}: {1}", caseDefinitionId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}
		catch (NotFoundException e)
		{
		  string errorMessage = string.Format("Cannot instantiate case definition {0}: {1}", caseDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, e, errorMessage);

		}
		catch (NotValidException e)
		{
		  string errorMessage = string.Format("Cannot instantiate case definition {0}: {1}", caseDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, errorMessage);

		}
		catch (NotAllowedException e)
		{
		  string errorMessage = string.Format("Cannot instantiate case definition {0}: {1}", caseDefinitionId, e.Message);
		  throw new InvalidRequestException(Response.Status.FORBIDDEN, e, errorMessage);

		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot instantiate case definition {0}: {1}", caseDefinitionId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);

		}

		CaseInstanceDto result = CaseInstanceDto.fromCaseInstance(instance);

		URI uri = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.CaseInstanceRestService_Fields.PATH).path(instance.Id).build();

		result.addReflexiveLink(uri, HttpMethod.GET, "self");

		return result;
	  }

	  public virtual Response CaseDefinitionDiagram
	  {
		  get
		  {
			CaseDefinition definition = engine.RepositoryService.getCaseDefinition(caseDefinitionId);
			Stream caseDiagram = engine.RepositoryService.getCaseDiagram(caseDefinitionId);
			if (caseDiagram == null)
			{
			  return Response.noContent().build();
			}
			else
			{
			  string fileName = definition.DiagramResourceName;
			  return Response.ok(caseDiagram).header("Content-Disposition", "attachment; filename=" + fileName).type(ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix(fileName)).build();
			}
		  }
	  }

	  public virtual void updateHistoryTimeToLive(HistoryTimeToLiveDto historyTimeToLiveDto)
	  {
		engine.RepositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitionId, historyTimeToLiveDto.HistoryTimeToLive);
	  }

	}

}