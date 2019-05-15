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
namespace org.camunda.bpm.engine.rest.sub.externaltask
{

	using CompleteExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.CompleteExternalTaskDto;
	using ExtendLockOnExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExtendLockOnExternalTaskDto;
	using ExternalTaskBpmnError = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskBpmnError;
	using ExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto;
	using ExternalTaskFailureDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskFailureDto;
	using PriorityDto = org.camunda.bpm.engine.rest.dto.runtime.PriorityDto;
	using RetriesDto = org.camunda.bpm.engine.rest.dto.runtime.RetriesDto;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Askar Akhmerov
	/// 
	/// </summary>
	public interface ExternalTaskResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto getExternalTask();
	  ExternalTaskDto ExternalTask {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/errorDetails") @Produces(javax.ws.rs.core.MediaType.TEXT_PLAIN) String getErrorDetails();
	  string ErrorDetails {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setRetries(org.camunda.bpm.engine.rest.dto.runtime.RetriesDto dto);
	  RetriesDto Retries {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/priority") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setPriority(org.camunda.bpm.engine.rest.dto.runtime.PriorityDto dto);
	  PriorityDto Priority {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/complete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void complete(org.camunda.bpm.engine.rest.dto.externaltask.CompleteExternalTaskDto dto);
	  void complete(CompleteExternalTaskDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/failure") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void handleFailure(org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskFailureDto dto);
	  void handleFailure(ExternalTaskFailureDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/bpmnError") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void handleBpmnError(org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskBpmnError dto);
	  void handleBpmnError(ExternalTaskBpmnError dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/unlock") void unlock();
	  void unlock();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/extendLock") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void extendLock(org.camunda.bpm.engine.rest.dto.externaltask.ExtendLockOnExternalTaskDto extendLockDto);
	  void extendLock(ExtendLockOnExternalTaskDto extendLockDto);
	}

}