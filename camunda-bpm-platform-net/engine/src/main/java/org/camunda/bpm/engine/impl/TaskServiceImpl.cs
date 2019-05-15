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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using AddCommentCmd = org.camunda.bpm.engine.impl.cmd.AddCommentCmd;
	using AddGroupIdentityLinkCmd = org.camunda.bpm.engine.impl.cmd.AddGroupIdentityLinkCmd;
	using AddUserIdentityLinkCmd = org.camunda.bpm.engine.impl.cmd.AddUserIdentityLinkCmd;
	using AssignTaskCmd = org.camunda.bpm.engine.impl.cmd.AssignTaskCmd;
	using ClaimTaskCmd = org.camunda.bpm.engine.impl.cmd.ClaimTaskCmd;
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using CreateAttachmentCmd = org.camunda.bpm.engine.impl.cmd.CreateAttachmentCmd;
	using CreateTaskCmd = org.camunda.bpm.engine.impl.cmd.CreateTaskCmd;
	using DelegateTaskCmd = org.camunda.bpm.engine.impl.cmd.DelegateTaskCmd;
	using DeleteAttachmentCmd = org.camunda.bpm.engine.impl.cmd.DeleteAttachmentCmd;
	using DeleteGroupIdentityLinkCmd = org.camunda.bpm.engine.impl.cmd.DeleteGroupIdentityLinkCmd;
	using DeleteTaskAttachmentCmd = org.camunda.bpm.engine.impl.cmd.DeleteTaskAttachmentCmd;
	using DeleteTaskCmd = org.camunda.bpm.engine.impl.cmd.DeleteTaskCmd;
	using DeleteUserIdentityLinkCmd = org.camunda.bpm.engine.impl.cmd.DeleteUserIdentityLinkCmd;
	using GetAttachmentCmd = org.camunda.bpm.engine.impl.cmd.GetAttachmentCmd;
	using GetAttachmentContentCmd = org.camunda.bpm.engine.impl.cmd.GetAttachmentContentCmd;
	using GetIdentityLinksForTaskCmd = org.camunda.bpm.engine.impl.cmd.GetIdentityLinksForTaskCmd;
	using GetProcessInstanceAttachmentsCmd = org.camunda.bpm.engine.impl.cmd.GetProcessInstanceAttachmentsCmd;
	using GetProcessInstanceCommentsCmd = org.camunda.bpm.engine.impl.cmd.GetProcessInstanceCommentsCmd;
	using GetSubTasksCmd = org.camunda.bpm.engine.impl.cmd.GetSubTasksCmd;
	using GetTaskAttachmentCmd = org.camunda.bpm.engine.impl.cmd.GetTaskAttachmentCmd;
	using GetTaskAttachmentContentCmd = org.camunda.bpm.engine.impl.cmd.GetTaskAttachmentContentCmd;
	using GetTaskAttachmentsCmd = org.camunda.bpm.engine.impl.cmd.GetTaskAttachmentsCmd;
	using GetTaskCommentCmd = org.camunda.bpm.engine.impl.cmd.GetTaskCommentCmd;
	using GetTaskCommentsCmd = org.camunda.bpm.engine.impl.cmd.GetTaskCommentsCmd;
	using GetTaskEventsCmd = org.camunda.bpm.engine.impl.cmd.GetTaskEventsCmd;
	using GetTaskVariableCmd = org.camunda.bpm.engine.impl.cmd.GetTaskVariableCmd;
	using GetTaskVariableCmdTyped = org.camunda.bpm.engine.impl.cmd.GetTaskVariableCmdTyped;
	using GetTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.GetTaskVariablesCmd;
	using PatchTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.PatchTaskVariablesCmd;
	using RemoveTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.RemoveTaskVariablesCmd;
	using ResolveTaskCmd = org.camunda.bpm.engine.impl.cmd.ResolveTaskCmd;
	using SaveAttachmentCmd = org.camunda.bpm.engine.impl.cmd.SaveAttachmentCmd;
	using SaveTaskCmd = org.camunda.bpm.engine.impl.cmd.SaveTaskCmd;
	using SetTaskOwnerCmd = org.camunda.bpm.engine.impl.cmd.SetTaskOwnerCmd;
	using SetTaskPriorityCmd = org.camunda.bpm.engine.impl.cmd.SetTaskPriorityCmd;
	using SetTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.SetTaskVariablesCmd;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Event = org.camunda.bpm.engine.task.Event;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using NativeTaskQuery = org.camunda.bpm.engine.task.NativeTaskQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using TaskReport = org.camunda.bpm.engine.task.TaskReport;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class TaskServiceImpl : ServiceImpl, TaskService
	{

	  public virtual Task newTask()
	  {
		return newTask(null);
	  }

	  public virtual Task newTask(string taskId)
	  {
		return commandExecutor.execute(new CreateTaskCmd(taskId));
	  }

	  public virtual void saveTask(Task task)
	  {
		commandExecutor.execute(new SaveTaskCmd(task));
	  }

	  public virtual void deleteTask(string taskId)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, null, false));
	  }

	  public virtual void deleteTasks(ICollection<string> taskIds)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, null, false));
	  }

	  public virtual void deleteTask(string taskId, bool cascade)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, null, cascade));
	  }

	  public virtual void deleteTasks(ICollection<string> taskIds, bool cascade)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, null, cascade));
	  }

	  public virtual void deleteTask(string taskId, string deleteReason)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, deleteReason, false));
	  }

	  public virtual void deleteTasks(ICollection<string> taskIds, string deleteReason)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, deleteReason, false));
	  }

	  public virtual void setAssignee(string taskId, string userId)
	  {
		commandExecutor.execute(new AssignTaskCmd(taskId, userId));
	  }

	  public virtual void setOwner(string taskId, string userId)
	  {
		commandExecutor.execute(new SetTaskOwnerCmd(taskId, userId));
	  }

	  public virtual void addCandidateUser(string taskId, string userId)
	  {
		commandExecutor.execute(new AddUserIdentityLinkCmd(taskId, userId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void addCandidateGroup(string taskId, string groupId)
	  {
		commandExecutor.execute(new AddGroupIdentityLinkCmd(taskId, groupId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void addUserIdentityLink(string taskId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new AddUserIdentityLinkCmd(taskId, userId, identityLinkType));
	  }

	  public virtual void addGroupIdentityLink(string taskId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new AddGroupIdentityLinkCmd(taskId, groupId, identityLinkType));
	  }

	  public virtual void deleteCandidateGroup(string taskId, string groupId)
	  {
		commandExecutor.execute(new DeleteGroupIdentityLinkCmd(taskId, groupId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void deleteCandidateUser(string taskId, string userId)
	  {
		commandExecutor.execute(new DeleteUserIdentityLinkCmd(taskId, userId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void deleteGroupIdentityLink(string taskId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteGroupIdentityLinkCmd(taskId, groupId, identityLinkType));
	  }

	  public virtual void deleteUserIdentityLink(string taskId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteUserIdentityLinkCmd(taskId, userId, identityLinkType));
	  }

	  public virtual IList<IdentityLink> getIdentityLinksForTask(string taskId)
	  {
		return commandExecutor.execute(new GetIdentityLinksForTaskCmd(taskId));
	  }

	  public virtual void claim(string taskId, string userId)
	  {
		commandExecutor.execute(new ClaimTaskCmd(taskId, userId));
	  }

	  public virtual void complete(string taskId)
	  {
		complete(taskId, null);
	  }

	  public virtual void complete(string taskId, IDictionary<string, object> variables)
	  {
		commandExecutor.execute(new CompleteTaskCmd(taskId, variables, false, false));
	  }

	  public virtual VariableMap completeWithVariablesInReturn(string taskId, IDictionary<string, object> variables, bool deserializeValues)
	  {
		return commandExecutor.execute(new CompleteTaskCmd(taskId, variables, true, deserializeValues));
	  }

	  public virtual void delegateTask(string taskId, string userId)
	  {
		commandExecutor.execute(new DelegateTaskCmd(taskId, userId));
	  }

	  public virtual void resolveTask(string taskId)
	  {
		commandExecutor.execute(new ResolveTaskCmd(taskId, null));
	  }

	  public virtual void resolveTask(string taskId, IDictionary<string, object> variables)
	  {
		commandExecutor.execute(new ResolveTaskCmd(taskId, variables));
	  }

	  public virtual void setPriority(string taskId, int priority)
	  {
		commandExecutor.execute(new SetTaskPriorityCmd(taskId, priority));
	  }

	  public virtual TaskQuery createTaskQuery()
	  {
		return new TaskQueryImpl(commandExecutor);
	  }

	  public virtual NativeTaskQuery createNativeTaskQuery()
	  {
		return new NativeTaskQueryImpl(commandExecutor);
	  }

	  public virtual VariableMap getVariables(string taskId)
	  {
		return getVariablesTyped(taskId);
	  }

	  public virtual VariableMap getVariablesTyped(string taskId)
	  {
		return getVariablesTyped(taskId, true);
	  }

	  public virtual VariableMap getVariablesTyped(string taskId, bool deserializeValues)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, null, false, deserializeValues));
	  }

	  public virtual VariableMap getVariablesLocal(string taskId)
	  {
		return getVariablesLocalTyped(taskId);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string taskId)
	  {
		return getVariablesLocalTyped(taskId, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string taskId, bool deserializeValues)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, null, true, deserializeValues));
	  }

	  public virtual VariableMap getVariables(string taskId, ICollection<string> variableNames)
	  {
		return getVariablesTyped(taskId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesTyped(string taskId, ICollection<string> variableNames, bool deserializeValues)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, variableNames, false, deserializeValues));
	  }

	  public virtual VariableMap getVariablesLocal(string taskId, ICollection<string> variableNames)
	  {
		return getVariablesLocalTyped(taskId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string taskId, ICollection<string> variableNames, bool deserializeValues)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, variableNames, true, deserializeValues));
	  }

	  public virtual object getVariable(string taskId, string variableName)
	  {
		return commandExecutor.execute(new GetTaskVariableCmd(taskId, variableName, false));
	  }

	  public virtual object getVariableLocal(string taskId, string variableName)
	  {
		return commandExecutor.execute(new GetTaskVariableCmd(taskId, variableName, true));
	  }

	  public virtual T getVariableTyped<T>(string taskId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(taskId, variableName, false, true);
	  }

	  public virtual T getVariableTyped<T>(string taskId, string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(taskId, variableName, false, deserializeValue);
	  }

	  public virtual T getVariableLocalTyped<T>(string taskId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(taskId, variableName, true, true);
	  }

	  public virtual T getVariableLocalTyped<T>(string taskId, string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(taskId, variableName, true, deserializeValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getVariableTyped(String taskId, String variableName, boolean isLocal, boolean deserializeValue)
	  protected internal virtual T getVariableTyped<T>(string taskId, string variableName, bool isLocal, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return (T) commandExecutor.execute(new GetTaskVariableCmdTyped(taskId, variableName, isLocal, deserializeValue));
	  }

	  public virtual void setVariable(string taskId, string variableName, object value)
	  {
		ensureNotNull("variableName", variableName);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		setVariables(taskId, variables, false);
	  }

	  public virtual void setVariableLocal(string taskId, string variableName, object value)
	  {
		ensureNotNull("variableName", variableName);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		setVariables(taskId, variables, true);
	  }

	  public virtual void setVariables<T1>(string taskId, IDictionary<T1> variables) where T1 : object
	  {
		setVariables(taskId, variables, false);
	  }

	  public virtual void setVariablesLocal<T1>(string taskId, IDictionary<T1> variables) where T1 : object
	  {
		setVariables(taskId, variables, true);
	  }

	  protected internal virtual void setVariables<T1>(string taskId, IDictionary<T1> variables, bool local) where T1 : object
	  {
		try
		{
		  commandExecutor.execute(new SetTaskVariablesCmd(taskId, variables, local));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkValueTooLongException(ex))
		  {
			throw new BadUserRequestException("Variable value is too long", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual void updateVariablesLocal<T1>(string taskId, IDictionary<T1> modifications, ICollection<string> deletions) where T1 : object
	  {
		updateVariables(taskId, modifications, deletions, true);
	  }

	  public virtual void updateVariables<T1>(string taskId, IDictionary<T1> modifications, ICollection<string> deletions) where T1 : object
	  {
		updateVariables(taskId, modifications, deletions, false);
	  }

	  protected internal virtual void updateVariables<T1>(string taskId, IDictionary<T1> modifications, ICollection<string> deletions, bool local) where T1 : object
	  {
		try
		{
		  commandExecutor.execute(new PatchTaskVariablesCmd(taskId, modifications, deletions, local));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkValueTooLongException(ex))
		  {
			throw new BadUserRequestException("Variable value is too long", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual void removeVariable(string taskId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
	  }

	  public virtual void removeVariableLocal(string taskId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>(1);
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
	  }

	  public virtual void removeVariables(string taskId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
	  }

	  public virtual void removeVariablesLocal(string taskId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
	  }

	  public virtual void addComment(string taskId, string processInstance, string message)
	  {
		createComment(taskId, processInstance, message);
	  }

	  public virtual Comment createComment(string taskId, string processInstance, string message)
	  {
		return commandExecutor.execute(new AddCommentCmd(taskId, processInstance, message));
	  }

	  public virtual IList<Comment> getTaskComments(string taskId)
	  {
		return commandExecutor.execute(new GetTaskCommentsCmd(taskId));
	  }

	  public virtual Comment getTaskComment(string taskId, string commentId)
	  {
		return commandExecutor.execute(new GetTaskCommentCmd(taskId, commentId));
	  }

	  public virtual IList<Event> getTaskEvents(string taskId)
	  {
		return commandExecutor.execute(new GetTaskEventsCmd(taskId));
	  }

	  public virtual IList<Comment> getProcessInstanceComments(string processInstanceId)
	  {
		return commandExecutor.execute(new GetProcessInstanceCommentsCmd(processInstanceId));
	  }

	  public virtual Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, Stream content)
	  {
		return commandExecutor.execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId, attachmentName, attachmentDescription, content, null));
	  }

	  public virtual Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, string url)
	  {
		return commandExecutor.execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId, attachmentName, attachmentDescription, null, url));
	  }

	  public virtual Stream getAttachmentContent(string attachmentId)
	  {
		return commandExecutor.execute(new GetAttachmentContentCmd(attachmentId));
	  }

	  public virtual Stream getTaskAttachmentContent(string taskId, string attachmentId)
	  {
		return commandExecutor.execute(new GetTaskAttachmentContentCmd(taskId, attachmentId));
	  }

	  public virtual void deleteAttachment(string attachmentId)
	  {
		commandExecutor.execute(new DeleteAttachmentCmd(attachmentId));
	  }

	  public virtual void deleteTaskAttachment(string taskId, string attachmentId)
	  {
		commandExecutor.execute(new DeleteTaskAttachmentCmd(taskId, attachmentId));
	  }

	  public virtual Attachment getAttachment(string attachmentId)
	  {
		return commandExecutor.execute(new GetAttachmentCmd(attachmentId));
	  }

	  public virtual Attachment getTaskAttachment(string taskId, string attachmentId)
	  {
		return commandExecutor.execute(new GetTaskAttachmentCmd(taskId, attachmentId));
	  }

	  public virtual IList<Attachment> getTaskAttachments(string taskId)
	  {
		return commandExecutor.execute(new GetTaskAttachmentsCmd(taskId));
	  }

	  public virtual IList<Attachment> getProcessInstanceAttachments(string processInstanceId)
	  {
		return commandExecutor.execute(new GetProcessInstanceAttachmentsCmd(processInstanceId));
	  }

	  public virtual void saveAttachment(Attachment attachment)
	  {
		commandExecutor.execute(new SaveAttachmentCmd(attachment));
	  }

	  public virtual IList<Task> getSubTasks(string parentTaskId)
	  {
		return commandExecutor.execute(new GetSubTasksCmd(parentTaskId));
	  }

	  public virtual TaskReport createTaskReport()
	  {
		return new TaskReportImpl(commandExecutor);
	  }

	}

}