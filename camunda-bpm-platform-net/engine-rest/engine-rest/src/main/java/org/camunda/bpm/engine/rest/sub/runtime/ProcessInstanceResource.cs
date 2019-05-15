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

	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ActivityInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ActivityInstanceDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using ProcessInstanceSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto;
	using ProcessInstanceModificationDto = org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationDto;

	public interface ProcessInstanceResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto getProcessInstance();
	  ProcessInstanceDto ProcessInstance {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE void deleteProcessInstance(@QueryParam("skipCustomListeners") @DefaultValue("false") boolean skipCustomListeners, @QueryParam("skipIoMappings") @DefaultValue("false") boolean skipIoMappings, @QueryParam("skipSubprocesses") @DefaultValue("false") boolean skipSubprocesses, @QueryParam("failIfNotExists") @DefaultValue("true") boolean failIfNotExists);
	  void deleteProcessInstance(bool skipCustomListeners, bool skipIoMappings, bool skipSubprocesses, bool failIfNotExists);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/variables") org.camunda.bpm.engine.rest.sub.VariableResource getVariablesResource();
	  VariableResource VariablesResource {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/activity-instances") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.ActivityInstanceDto getActivityInstanceTree();
	  ActivityInstanceDto ActivityInstanceTree {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto dto);
	  void updateSuspensionState(ProcessInstanceSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/modification") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void modifyProcessInstance(org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationDto dto);
	  void modifyProcessInstance(ProcessInstanceModificationDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/modification-async") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto modifyProcessInstanceAsync(org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationDto dto);
	  BatchDto modifyProcessInstanceAsync(ProcessInstanceModificationDto dto);
	}

}