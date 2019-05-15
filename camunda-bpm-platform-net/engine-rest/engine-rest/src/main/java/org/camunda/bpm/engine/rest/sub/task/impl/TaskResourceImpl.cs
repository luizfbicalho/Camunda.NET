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


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using FormData = org.camunda.bpm.engine.form.FormData;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using CompleteTaskDto = org.camunda.bpm.engine.rest.dto.task.CompleteTaskDto;
	using FormDto = org.camunda.bpm.engine.rest.dto.task.FormDto;
	using IdentityLinkDto = org.camunda.bpm.engine.rest.dto.task.IdentityLinkDto;
	using TaskDto = org.camunda.bpm.engine.rest.dto.task.TaskDto;
	using UserIdDto = org.camunda.bpm.engine.rest.dto.task.UserIdDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using HalTask = org.camunda.bpm.engine.rest.hal.task.HalTask;
	using ApplicationContextPathUtil = org.camunda.bpm.engine.rest.util.ApplicationContextPathUtil;
	using EncodingUtil = org.camunda.bpm.engine.rest.util.EncodingUtil;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class TaskResourceImpl : TaskResource
	{

	  public static readonly IList<Variant> VARIANTS = Variant.mediaTypes(MediaType.APPLICATION_JSON_TYPE, Hal.APPLICATION_HAL_JSON_TYPE).add().build();

	  protected internal ProcessEngine engine;
	  protected internal string taskId;
	  protected internal string rootResourcePath;
	  protected internal ObjectMapper objectMapper;

	  public TaskResourceImpl(ProcessEngine engine, string taskId, string rootResourcePath, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.taskId = taskId;
		this.rootResourcePath = rootResourcePath;
		this.objectMapper = objectMapper;
	  }

	  public virtual void claim(UserIdDto dto)
	  {
		TaskService taskService = engine.TaskService;

		taskService.claim(taskId, dto.UserId);
	  }

	  public virtual void unclaim()
	  {
		engine.TaskService.setAssignee(taskId, null);
	  }

	  public virtual Response complete(CompleteTaskDto dto)
	  {
		TaskService taskService = engine.TaskService;

		try
		{
		  VariableMap variables = VariableValueDto.toMap(dto.Variables, engine, objectMapper);
		  if (dto.WithVariablesInReturn)
		  {
			VariableMap taskVariables = taskService.completeWithVariablesInReturn(taskId, variables, false);

			IDictionary<string, VariableValueDto> body = VariableValueDto.fromMap(taskVariables, true);

			return Response.ok(body).type(MediaType.APPLICATION_JSON).build();
		  }
		  else
		  {
			taskService.complete(taskId, variables);
			return Response.noContent().build();
		  }

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot complete task {0}: {1}", taskId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}
		catch (AuthorizationException e)
		{
		  throw e;

		}
		catch (FormFieldValidationException e)
		{
		  string errorMessage = string.Format("Cannot complete task {0}: {1}", taskId, e.Message);
		  throw new RestException(Response.Status.BAD_REQUEST, e, errorMessage);

		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot complete task {0}: {1}", taskId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}
	  }

	  public virtual Response submit(CompleteTaskDto dto)
	  {
		FormService formService = engine.FormService;

		try
		{
		  VariableMap variables = VariableValueDto.toMap(dto.Variables, engine, objectMapper);
		  if (dto.WithVariablesInReturn)
		  {
			VariableMap taskVariables = formService.submitTaskFormWithVariablesInReturn(taskId, variables, false);

			IDictionary<string, VariableValueDto> body = VariableValueDto.fromMap(taskVariables, true);
			return Response.ok(body).type(MediaType.APPLICATION_JSON).build();
		  }
		  else
		  {
			formService.submitTaskForm(taskId, variables);
			return Response.noContent().build();
		  }

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot submit task form {0}: {1}", taskId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}
		catch (AuthorizationException e)
		{
		  throw e;

		}
		catch (FormFieldValidationException e)
		{
		  string errorMessage = string.Format("Cannot submit task form {0}: {1}", taskId, e.Message);
		  throw new RestException(Response.Status.BAD_REQUEST, e, errorMessage);

		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot submit task form {0}: {1}", taskId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}

	  }

	  public virtual void @delegate(UserIdDto delegatedUser)
	  {
		engine.TaskService.delegateTask(taskId, delegatedUser.UserId);
	  }

	  public virtual object getTask(Request request)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  if (MediaType.APPLICATION_JSON_TYPE.Equals(variant.MediaType))
		  {
			return JsonTask;
		  }
		  else if (Hal.APPLICATION_HAL_JSON_TYPE.Equals(variant.MediaType))
		  {
			return HalTask;
		  }
		}
		throw new InvalidRequestException(Response.Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual TaskDto JsonTask
	  {
		  get
		  {
			Task task = getTaskById(taskId);
			if (task == null)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, "No matching task with id " + taskId);
			}
    
			return TaskDto.fromEntity(task);
		  }
	  }

	  public virtual HalTask HalTask
	  {
		  get
		  {
			Task task = getTaskById(taskId);
			if (task == null)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, "No matching task with id " + taskId);
			}
    
			return HalTask.generate(task, engine);
		  }
	  }

	  public virtual FormDto Form
	  {
		  get
		  {
			FormService formService = engine.FormService;
			Task task = getTaskById(taskId);
			FormData formData;
			try
			{
			  formData = formService.getTaskFormData(taskId);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new RestException(Response.Status.BAD_REQUEST, e, "Cannot get form for task " + taskId);
			}
    
			FormDto dto = FormDto.fromFormData(formData);
			if (string.ReferenceEquals(dto.Key, null) || dto.Key.Length == 0)
			{
			  if (formData != null && formData.FormFields != null && formData.FormFields.Count > 0)
			  {
				dto.Key = "embedded:engine://engine/:engine/task/" + taskId + "/rendered-form";
			  }
			}
    
			// to get the application context path it is necessary to
			// execute it without authentication (tries to fetch the
			// process definition), because:
			// - user 'demo' has READ permission on a specific task resource
			// - user 'demo' does not have a READ permission on the corresponding
			//   process definition
			// -> running the following lines with authorization would lead
			// to an AuthorizationException because the user 'demo' does not
			// have READ permission on the corresponding process definition
			IdentityService identityService = engine.IdentityService;
			Authentication currentAuthentication = identityService.CurrentAuthentication;
			try
			{
			  identityService.clearAuthentication();
			  string processDefinitionId = task.ProcessDefinitionId;
			  string caseDefinitionId = task.CaseDefinitionId;
			  if (!string.ReferenceEquals(processDefinitionId, null))
			  {
				dto.ContextPath = ApplicationContextPathUtil.getApplicationPathByProcessDefinitionId(engine, processDefinitionId);
    
			  }
			  else if (!string.ReferenceEquals(caseDefinitionId, null))
			  {
				dto.ContextPath = ApplicationContextPathUtil.getApplicationPathByCaseDefinitionId(engine, caseDefinitionId);
			  }
			}
			finally
			{
			  identityService.Authentication = currentAuthentication;
			}
    
			return dto;
		  }
	  }

	  public virtual Response RenderedForm
	  {
		  get
		  {
			FormService formService = engine.FormService;
    
			object renderedTaskForm = formService.getRenderedTaskForm(taskId);
			if (renderedTaskForm != null)
			{
			  string content = renderedTaskForm.ToString();
			  Stream stream = new MemoryStream(content.GetBytes(EncodingUtil.DEFAULT_ENCODING));
			  return Response.ok(stream).type(MediaType.APPLICATION_XHTML_XML).build();
			}
    
			throw new InvalidRequestException(Response.Status.NOT_FOUND, "No matching rendered form for task with the id " + taskId + " found.");
		  }
	  }

	  public virtual void resolve(CompleteTaskDto dto)
	  {
		TaskService taskService = engine.TaskService;

		try
		{
		  VariableMap variables = VariableValueDto.toMap(dto.Variables, engine, objectMapper);
		  taskService.resolveTask(taskId, variables);

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot resolve task {0}: {1}", taskId, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}

	  }


	  /// <summary>
	  /// Returns the task with the given id
	  /// </summary>
	  /// <param name="id">
	  /// @return </param>
	  protected internal virtual Task getTaskById(string id)
	  {
		return engine.TaskService.createTaskQuery().taskId(id).initializeFormKeys().singleResult();
	  }

	  public virtual UserIdDto Assignee
	  {
		  set
		  {
			TaskService taskService = engine.TaskService;
			taskService.setAssignee(taskId, value.UserId);
		  }
	  }

	  public virtual IList<IdentityLinkDto> getIdentityLinks(string type)
	  {
		TaskService taskService = engine.TaskService;
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);

		IList<IdentityLinkDto> result = new List<IdentityLinkDto>();
		foreach (IdentityLink link in identityLinks)
		{
		  if (string.ReferenceEquals(type, null) || type.Equals(link.Type))
		  {
			result.Add(IdentityLinkDto.fromIdentityLink(link));
		  }
		}

		return result;
	  }

	  public virtual void addIdentityLink(IdentityLinkDto identityLink)
	  {
		TaskService taskService = engine.TaskService;

		identityLink.validate();

		if (!string.ReferenceEquals(identityLink.UserId, null))
		{
		  taskService.addUserIdentityLink(taskId, identityLink.UserId, identityLink.Type);
		}
		else if (!string.ReferenceEquals(identityLink.GroupId, null))
		{
		  taskService.addGroupIdentityLink(taskId, identityLink.GroupId, identityLink.Type);
		}

	  }

	  public virtual void deleteIdentityLink(IdentityLinkDto identityLink)
	  {
		TaskService taskService = engine.TaskService;

		identityLink.validate();

		if (!string.ReferenceEquals(identityLink.UserId, null))
		{
		  taskService.deleteUserIdentityLink(taskId, identityLink.UserId, identityLink.Type);
		}
		else if (!string.ReferenceEquals(identityLink.GroupId, null))
		{
		  taskService.deleteGroupIdentityLink(taskId, identityLink.GroupId, identityLink.Type);
		}

	  }

	  public virtual TaskCommentResource TaskCommentResource
	  {
		  get
		  {
			return new TaskCommentResourceImpl(engine, taskId, rootResourcePath);
		  }
	  }

	  public virtual TaskAttachmentResource AttachmentResource
	  {
		  get
		  {
			return new TaskAttachmentResourceImpl(engine, taskId, rootResourcePath);
		  }
	  }

	  public virtual VariableResource LocalVariables
	  {
		  get
		  {
			return new LocalTaskVariablesResource(engine, taskId, objectMapper);
		  }
	  }

	  public virtual VariableResource Variables
	  {
		  get
		  {
			return new TaskVariablesResource(engine, taskId, objectMapper);
		  }
	  }

	  public virtual IDictionary<string, VariableValueDto> getFormVariables(string variableNames, bool deserializeValues)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.FormService formService = engine.getFormService();
		FormService formService = engine.FormService;
		IList<string> formVariables = null;

		if (!string.ReferenceEquals(variableNames, null))
		{
		  StringListConverter stringListConverter = new StringListConverter();
		  formVariables = stringListConverter.convertQueryParameterToType(variableNames);
		}

		VariableMap startFormVariables = formService.getTaskFormVariables(taskId, formVariables, deserializeValues);

		return VariableValueDto.fromMap(startFormVariables);
	  }

	  public virtual void updateTask(TaskDto taskDto)
	  {
		TaskService taskService = engine.TaskService;

		Task task = getTaskById(taskId);

		if (task == null)
		{
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, "No matching task with id " + taskId);
		}

		taskDto.updateTask(task);
		taskService.saveTask(task);
	  }

	  public virtual void deleteTask(string id)
	  {
		TaskService taskService = engine.TaskService;

		try
		{
		  taskService.deleteTask(id);
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, "Could not delete task: " + e.Message);
		}
	  }

	  public virtual Response DeployedForm
	  {
		  get
		  {
			Stream deployedTaskForm = null;
			try
			{
			  deployedTaskForm = engine.FormService.getDeployedTaskForm(taskId);
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, e.Message);
			}
			catch (NullValueException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
			}
			catch (AuthorizationException e)
			{
			  throw new InvalidRequestException(Response.Status.FORBIDDEN, e.Message);
			}
			catch (BadUserRequestException e)
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
			}
			return Response.ok(deployedTaskForm, MediaType.APPLICATION_XHTML_XML).build();
		  }
	  }
	}

}