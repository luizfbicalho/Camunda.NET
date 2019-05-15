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
namespace org.camunda.bpm.engine.rest.sub.task
{
	using AttachmentDto = org.camunda.bpm.engine.rest.dto.task.AttachmentDto;
	using MultipartFormData = org.camunda.bpm.engine.rest.mapper.MultipartFormData;


	public interface TaskAttachmentResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.task.AttachmentDto> getAttachments();
	  IList<AttachmentDto> Attachments {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{attachmentId}") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.task.AttachmentDto getAttachment(@PathParam("attachmentId") String attachmentId);
	  AttachmentDto getAttachment(string attachmentId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{attachmentId}/data") @Produces(javax.ws.rs.core.MediaType.APPLICATION_OCTET_STREAM) java.io.InputStream getAttachmentData(@PathParam("attachmentId") String attachmentId);
	  Stream getAttachmentData(string attachmentId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Path("/{attachmentId}") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) void deleteAttachment(@PathParam("attachmentId") String attachmentId);
	  void deleteAttachment(string attachmentId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/create") @Consumes(javax.ws.rs.core.MediaType.MULTIPART_FORM_DATA) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.task.AttachmentDto addAttachment(@Context UriInfo uriInfo, org.camunda.bpm.engine.rest.mapper.MultipartFormData multipartFormData);
	  AttachmentDto addAttachment(UriInfo uriInfo, MultipartFormData multipartFormData);

	}
}