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
namespace org.camunda.bpm.engine.rest.sub.task.impl
{


	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using AttachmentDto = org.camunda.bpm.engine.rest.dto.task.AttachmentDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MultipartFormData = org.camunda.bpm.engine.rest.mapper.MultipartFormData;
	using FormPart = org.camunda.bpm.engine.rest.mapper.MultipartFormData.FormPart;
	using Attachment = org.camunda.bpm.engine.task.Attachment;

	public class TaskAttachmentResourceImpl : TaskAttachmentResource
	{

	  private ProcessEngine engine;
	  private string taskId;
	  private string rootResourcePath;

	  public TaskAttachmentResourceImpl(ProcessEngine engine, string taskId, string rootResourcePath)
	  {
		this.engine = engine;
		this.taskId = taskId;
		this.rootResourcePath = rootResourcePath;
	  }

	  public virtual IList<AttachmentDto> Attachments
	  {
		  get
		  {
			if (!HistoryEnabled)
			{
			  return Collections.emptyList();
			}
    
			ensureTaskExists(Status.NOT_FOUND);
    
			IList<Attachment> taskAttachments = engine.TaskService.getTaskAttachments(taskId);
    
			IList<AttachmentDto> attachments = new List<AttachmentDto>();
			foreach (Attachment attachment in taskAttachments)
			{
			  attachments.Add(AttachmentDto.fromAttachment(attachment));
			}
    
			return attachments;
		  }
	  }

	  public virtual AttachmentDto getAttachment(string attachmentId)
	  {
		ensureHistoryEnabled(Status.NOT_FOUND);

		Attachment attachment = engine.TaskService.getTaskAttachment(taskId, attachmentId);

		if (attachment == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Task attachment with id " + attachmentId + " does not exist for task id '" + taskId + "'.");
		}

		return AttachmentDto.fromAttachment(attachment);
	  }

	  public virtual Stream getAttachmentData(string attachmentId)
	  {
		ensureHistoryEnabled(Status.NOT_FOUND);

		Stream attachmentData = engine.TaskService.getTaskAttachmentContent(taskId, attachmentId);

		if (attachmentData != null)
		{
		  return attachmentData;
		}
		else
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Attachment content for attachment with id '" + attachmentId + "' does not exist for task id '" + taskId + "'.");
		}
	  }

	  public virtual void deleteAttachment(string attachmentId)
	  {
		ensureHistoryEnabled(Status.FORBIDDEN);

		try
		{
		  engine.TaskService.deleteTaskAttachment(taskId, attachmentId);
		}
		catch (ProcessEngineException)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Deletion is not possible. No attachment exists for task id '" + taskId + "' and attachment id '" + attachmentId + "'.");
		}
	  }

	  public virtual AttachmentDto addAttachment(UriInfo uriInfo, MultipartFormData payload)
	  {
		ensureHistoryEnabled(Status.FORBIDDEN);
		ensureTaskExists(Status.BAD_REQUEST);

		MultipartFormData.FormPart attachmentNamePart = payload.getNamedPart("attachment-name");
		MultipartFormData.FormPart attachmentTypePart = payload.getNamedPart("attachment-type");
		MultipartFormData.FormPart attachmentDescriptionPart = payload.getNamedPart("attachment-description");
		MultipartFormData.FormPart contentPart = payload.getNamedPart("content");
		MultipartFormData.FormPart urlPart = payload.getNamedPart("url");

		if (urlPart == null && contentPart == null)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "No content or url to remote content exists to create the task attachment.");
		}

		string attachmentName = null;
		string attachmentDescription = null;
		string attachmentType = null;
		if (attachmentNamePart != null)
		{
		  attachmentName = attachmentNamePart.TextContent;
		}
		if (attachmentDescriptionPart != null)
		{
		  attachmentDescription = attachmentDescriptionPart.TextContent;
		}
		if (attachmentTypePart != null)
		{
		  attachmentType = attachmentTypePart.TextContent;
		}

		Attachment attachment = null;
		try
		{
		  if (contentPart != null)
		  {
			MemoryStream byteArrayInputStream = new MemoryStream(contentPart.BinaryContent);
			attachment = engine.TaskService.createAttachment(attachmentType, taskId, null, attachmentName, attachmentDescription, byteArrayInputStream);
		  }
		  else if (urlPart != null)
		  {
			attachment = engine.TaskService.createAttachment(attachmentType, taskId, null, attachmentName, attachmentDescription, urlPart.TextContent);
		  }
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Task id is null");
		}

		URI uri = uriInfo.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path(taskId + "/attachment/" + attachment.Id).build();

		AttachmentDto attachmentDto = AttachmentDto.fromAttachment(attachment);

		// GET /
		attachmentDto.addReflexiveLink(uri, HttpMethod.GET, "self");

		return attachmentDto;
	  }

	  private bool HistoryEnabled
	  {
		  get
		  {
			IdentityService identityService = engine.IdentityService;
			Authentication currentAuthentication = identityService.CurrentAuthentication;
			try
			{
			  identityService.clearAuthentication();
			  int historyLevel = engine.ManagementService.HistoryLevel;
			  return historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE;
			}
			finally
			{
			  identityService.Authentication = currentAuthentication;
			}
		  }
	  }

	  private void ensureHistoryEnabled(Status status)
	  {
		if (!HistoryEnabled)
		{
		  throw new InvalidRequestException(status, "History is not enabled");
		}
	  }

	  private void ensureTaskExists(Status status)
	  {
		HistoricTaskInstance historicTaskInstance = engine.HistoryService.createHistoricTaskInstanceQuery().taskId(taskId).singleResult();
		if (historicTaskInstance == null)
		{
		  throw new InvalidRequestException(status, "No task found for task id " + taskId);
		}
	  }

	}

}