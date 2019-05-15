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
	using CaseExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto;
	using CaseExecutionTriggerDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto;



	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseExecutionResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto getCaseExecution();
	  CaseExecutionDto CaseExecution {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/manual-start") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void manualStart(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto triggerDto);
	  void manualStart(CaseExecutionTriggerDto triggerDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/disable") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void disable(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto triggerDto);
	  void disable(CaseExecutionTriggerDto triggerDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/reenable") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void reenable(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto triggerDto);
	  void reenable(CaseExecutionTriggerDto triggerDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/complete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void complete(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto triggerDto);
	  void complete(CaseExecutionTriggerDto triggerDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/terminate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void terminate(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionTriggerDto triggerDto);
	  void terminate(CaseExecutionTriggerDto triggerDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/localVariables") org.camunda.bpm.engine.rest.sub.VariableResource getVariablesLocal();
	  VariableResource VariablesLocal {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/variables") org.camunda.bpm.engine.rest.sub.VariableResource getVariables();
	  VariableResource Variables {get;}


	}

}