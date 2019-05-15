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
namespace org.camunda.bpm.engine.rest.sub.repository
{


	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using EvaluateDecisionDto = org.camunda.bpm.engine.rest.dto.dmn.EvaluateDecisionDto;
	using DecisionDefinitionDiagramDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDiagramDto;
	using DecisionDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDto;

	public interface DecisionDefinitionResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDto getDecisionDefinition();
	  DecisionDefinitionDto DecisionDefinition {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/xml") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDiagramDto getDecisionDefinitionDmnXml();
	  DecisionDefinitionDiagramDto DecisionDefinitionDmnXml {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/diagram") javax.ws.rs.core.Response getDecisionDefinitionDiagram();
	  Response DecisionDefinitionDiagram {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/evaluate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<java.util.Map<String, org.camunda.bpm.engine.rest.dto.VariableValueDto>> evaluateDecision(@Context UriInfo context, org.camunda.bpm.engine.rest.dto.dmn.EvaluateDecisionDto parameters);
	  IList<IDictionary<string, VariableValueDto>> evaluateDecision(UriInfo context, EvaluateDecisionDto parameters);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/history-time-to-live") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateHistoryTimeToLive(org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto historyTimeToLiveDto);
	  void updateHistoryTimeToLive(HistoryTimeToLiveDto historyTimeToLiveDto);

	}

}