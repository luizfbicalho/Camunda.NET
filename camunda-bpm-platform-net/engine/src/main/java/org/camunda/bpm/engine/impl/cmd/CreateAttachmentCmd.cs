using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using Attachment = org.camunda.bpm.engine.task.Attachment;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	// Not Serializable
	public class CreateAttachmentCmd : Command<Attachment>
	{

	  protected internal string taskId;
	  protected internal string attachmentType;
	  protected internal string processInstanceId;
	  protected internal string attachmentName;
	  protected internal string attachmentDescription;
	  protected internal Stream content;
	  protected internal string url;
	  private TaskEntity task;
	  protected internal ExecutionEntity processInstance;

	  public CreateAttachmentCmd(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, Stream content, string url)
	  {
		this.attachmentType = attachmentType;
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.attachmentName = attachmentName;
		this.attachmentDescription = attachmentDescription;
		this.content = content;
		this.url = url;
	  }

	  public virtual Attachment execute(CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(taskId, null))
		{
		  task = commandContext.TaskManager.findTaskById(taskId);
		}
		else
		{
		  ensureNotNull("taskId or processInstanceId has to be provided", this.processInstanceId);
		  IList<ExecutionEntity> executionsByProcessInstanceId = commandContext.ExecutionManager.findExecutionsByProcessInstanceId(processInstanceId);
		  processInstance = executionsByProcessInstanceId[0];
		}

		AttachmentEntity attachment = new AttachmentEntity();
		attachment.Name = attachmentName;
		attachment.Description = attachmentDescription;
		attachment.Type = attachmentType;
		attachment.TaskId = taskId;
		attachment.ProcessInstanceId = processInstanceId;
		attachment.Url = url;
		attachment.CreateTime = ClockUtil.CurrentTime;

		if (task != null)
		{
		  ExecutionEntity execution = task.getExecution();
		  if (execution != null)
		  {
			attachment.RootProcessInstanceId = execution.RootProcessInstanceId;
		  }
		}
		else if (processInstance != null)
		{
		  attachment.RootProcessInstanceId = processInstance.RootProcessInstanceId;
		}

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(attachment);
		}

		DbEntityManager dbEntityManger = commandContext.DbEntityManager;
		dbEntityManger.insert(attachment);

		if (content != null)
		{
		  sbyte[] bytes = IoUtil.readInputStream(content, attachmentName);
		  ByteArrayEntity byteArray = new ByteArrayEntity(bytes, ResourceTypes.HISTORY);

		  byteArray.RootProcessInstanceId = attachment.RootProcessInstanceId;
		  byteArray.RemovalTime = attachment.RemovalTime;

		  commandContext.ByteArrayManager.insertByteArray(byteArray);
		  attachment.ContentId = byteArray.Id;
		}

		PropertyChange propertyChange = new PropertyChange("name", null, attachmentName);

		if (task != null)
		{
		  commandContext.OperationLogManager.logAttachmentOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_ATTACHMENT, task, propertyChange);
		}
		else if (processInstance != null)
		{
		  commandContext.OperationLogManager.logAttachmentOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_ATTACHMENT, processInstance, propertyChange);
		}

		return attachment;
	  }

	  protected internal virtual bool HistoryRemovalTimeStrategyStart
	  {
		  get
		  {
			return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(HistoryRemovalTimeStrategy);
		  }
	  }

	  protected internal virtual string HistoryRemovalTimeStrategy
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
		  }
	  }

	  protected internal virtual HistoricProcessInstanceEventEntity getHistoricRootProcessInstance(string rootProcessInstanceId)
	  {
		return Context.CommandContext.DbEntityManager.selectById(typeof(HistoricProcessInstanceEventEntity), rootProcessInstanceId);
	  }

	  protected internal virtual void provideRemovalTime(AttachmentEntity attachment)
	  {
		string rootProcessInstanceId = attachment.RootProcessInstanceId;
		if (!string.ReferenceEquals(rootProcessInstanceId, null))
		{
		  HistoricProcessInstanceEventEntity historicRootProcessInstance = getHistoricRootProcessInstance(rootProcessInstanceId);

		  if (historicRootProcessInstance != null)
		  {
			DateTime removalTime = historicRootProcessInstance.RemovalTime;
			attachment.RemovalTime = removalTime;
		  }
		}
	  }

	}

}