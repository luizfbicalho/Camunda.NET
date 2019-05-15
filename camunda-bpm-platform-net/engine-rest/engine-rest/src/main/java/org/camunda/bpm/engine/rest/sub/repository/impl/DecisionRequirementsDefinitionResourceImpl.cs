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


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionDto;
	using DecisionRequirementsDefinitionXmlDto = org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionXmlDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class DecisionRequirementsDefinitionResourceImpl : DecisionRequirementsDefinitionResource
	{

	  protected internal ProcessEngine engine;
	  protected internal string decisionRequirementsDefinitionId;

	  public DecisionRequirementsDefinitionResourceImpl(ProcessEngine engine, string decisionDefinitionId)
	  {
		this.engine = engine;
		this.decisionRequirementsDefinitionId = decisionDefinitionId;
	  }

	  public virtual DecisionRequirementsDefinitionDto DecisionRequirementsDefinition
	  {
		  get
		  {
			RepositoryService repositoryService = engine.RepositoryService;
    
			DecisionRequirementsDefinition definition = null;
    
			try
			{
			  definition = repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);
    
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
    
			return DecisionRequirementsDefinitionDto.fromDecisionRequirementsDefinition(definition);
		  }
	  }

	  public virtual DecisionRequirementsDefinitionXmlDto DecisionRequirementsDefinitionDmnXml
	  {
		  get
		  {
			Stream decisionRequirementsModelInputStream = null;
			try
			{
			  decisionRequirementsModelInputStream = engine.RepositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);
    
			  sbyte[] decisionRequirementsModel = IoUtil.readInputStream(decisionRequirementsModelInputStream, "decisionRequirementsModelDmnXml");
			  return DecisionRequirementsDefinitionXmlDto.create(decisionRequirementsDefinitionId, StringHelper.NewString(decisionRequirementsModel, "UTF-8"));
    
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
			  IoUtil.closeSilently(decisionRequirementsModelInputStream);
			}
		  }
	  }

	  public virtual Response DecisionRequirementsDefinitionDiagram
	  {
		  get
		  {
			DecisionRequirementsDefinition definition = engine.RepositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);
			Stream decisionRequirementsDiagram = engine.RepositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);
			if (decisionRequirementsDiagram == null)
			{
			  return Response.noContent().build();
			}
			else
			{
			  string fileName = definition.DiagramResourceName;
			  return Response.ok(decisionRequirementsDiagram).header("Content-Disposition", "attachment; filename=" + fileName).type(ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix(fileName)).build();
			}
		  }
	  }
	}

}