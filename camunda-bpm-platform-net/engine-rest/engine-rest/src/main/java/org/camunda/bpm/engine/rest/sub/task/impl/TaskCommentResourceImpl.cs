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
namespace org.camunda.bpm.engine.rest.sub.task.impl
{


	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CommentDto = org.camunda.bpm.engine.rest.dto.task.CommentDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using Comment = org.camunda.bpm.engine.task.Comment;

	public class TaskCommentResourceImpl : TaskCommentResource
	{

	  private ProcessEngine engine;
	  private string taskId;
	  private string rootResourcePath;

	  public TaskCommentResourceImpl(ProcessEngine engine, string taskId, string rootResourcePath)
	  {
		this.engine = engine;
		this.taskId = taskId;
		this.rootResourcePath = rootResourcePath;
	  }

	  public virtual IList<CommentDto> Comments
	  {
		  get
		  {
			if (!HistoryEnabled)
			{
			  return Collections.emptyList();
			}
    
			ensureTaskExists(Status.NOT_FOUND);
    
			IList<Comment> taskComments = engine.TaskService.getTaskComments(taskId);
    
			IList<CommentDto> comments = new List<CommentDto>();
			foreach (Comment comment in taskComments)
			{
			  comments.Add(CommentDto.fromComment(comment));
			}
    
			return comments;
		  }
	  }

	  public virtual CommentDto getComment(string commentId)
	  {
		ensureHistoryEnabled(Status.NOT_FOUND);

		Comment comment = engine.TaskService.getTaskComment(taskId, commentId);
		if (comment == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Task comment with id " + commentId + " does not exist for task id '" + taskId + "'.");
		}

		return CommentDto.fromComment(comment);
	  }

	  public virtual CommentDto createComment(UriInfo uriInfo, CommentDto commentDto)
	  {
		ensureHistoryEnabled(Status.FORBIDDEN);
		ensureTaskExists(Status.BAD_REQUEST);

		Comment comment;

		try
		{
		  comment = engine.TaskService.createComment(taskId, null, commentDto.Message);
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Not enough parameters submitted");
		}

		URI uri = uriInfo.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.TaskRestService_Fields.PATH).path(taskId + "/comment/" + comment.Id).build();

		CommentDto resultDto = CommentDto.fromComment(comment);

		// GET /
		resultDto.addReflexiveLink(uri, HttpMethod.GET, "self");

		return resultDto;
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