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
namespace org.camunda.bpm.engine.rest.sub.task
{


	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using CompleteTaskDto = org.camunda.bpm.engine.rest.dto.task.CompleteTaskDto;
	using FormDto = org.camunda.bpm.engine.rest.dto.task.FormDto;
	using IdentityLinkDto = org.camunda.bpm.engine.rest.dto.task.IdentityLinkDto;
	using TaskDto = org.camunda.bpm.engine.rest.dto.task.TaskDto;
	using UserIdDto = org.camunda.bpm.engine.rest.dto.task.UserIdDto;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;

	public interface TaskResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) Object getTask(@Context Request request);
	  object getTask(Request request);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/form") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.task.FormDto getForm();
	  FormDto Form {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/submit-form") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) javax.ws.rs.core.Response submit(org.camunda.bpm.engine.rest.dto.task.CompleteTaskDto dto);
	  Response submit(CompleteTaskDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/rendered-form") @Produces(javax.ws.rs.core.MediaType.APPLICATION_XHTML_XML) javax.ws.rs.core.Response getRenderedForm();
	  Response RenderedForm {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/deployed-form") javax.ws.rs.core.Response getDeployedForm();
	  Response DeployedForm {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/claim") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void claim(org.camunda.bpm.engine.rest.dto.task.UserIdDto dto);
	  void claim(UserIdDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/unclaim") void unclaim();
	  void unclaim();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/complete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) javax.ws.rs.core.Response complete(org.camunda.bpm.engine.rest.dto.task.CompleteTaskDto dto);
	  Response complete(CompleteTaskDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/resolve") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void resolve(org.camunda.bpm.engine.rest.dto.task.CompleteTaskDto dto);
	  void resolve(CompleteTaskDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/delegate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void delegate(org.camunda.bpm.engine.rest.dto.task.UserIdDto delegatedUser);
	  void @delegate(UserIdDto delegatedUser);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/assignee") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setAssignee(org.camunda.bpm.engine.rest.dto.task.UserIdDto dto);
	  UserIdDto Assignee {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/identity-links") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.task.IdentityLinkDto> getIdentityLinks(@QueryParam("type") String type);
	  IList<IdentityLinkDto> getIdentityLinks(string type);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/identity-links") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void addIdentityLink(org.camunda.bpm.engine.rest.dto.task.IdentityLinkDto identityLink);
	  void addIdentityLink(IdentityLinkDto identityLink);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/identity-links/delete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void deleteIdentityLink(org.camunda.bpm.engine.rest.dto.task.IdentityLinkDto identityLink);
	  void deleteIdentityLink(IdentityLinkDto identityLink);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/comment") TaskCommentResource getTaskCommentResource();
	  TaskCommentResource TaskCommentResource {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/attachment") TaskAttachmentResource getAttachmentResource();
	  TaskAttachmentResource AttachmentResource {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/variables") org.camunda.bpm.engine.rest.sub.VariableResource getVariables();
	  VariableResource Variables {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/localVariables") org.camunda.bpm.engine.rest.sub.VariableResource getLocalVariables();
	  VariableResource LocalVariables {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/form-variables") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.Map<String, org.camunda.bpm.engine.rest.dto.VariableValueDto> getFormVariables(@QueryParam("variableNames") String variableNames, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeValues);
	  IDictionary<string, VariableValueDto> getFormVariables(string variableNames, bool deserializeValues);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) public void updateTask(org.camunda.bpm.engine.rest.dto.task.TaskDto task);
	  void updateTask(TaskDto task);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE void deleteTask(@PathParam("id") String id);
	  void deleteTask(string id);
	}

}