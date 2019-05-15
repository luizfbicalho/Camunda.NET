using System;
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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using Attachment = org.camunda.bpm.engine.task.Attachment;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AttachmentManager : AbstractHistoricManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Attachment> findAttachmentsByProcessInstanceId(String processInstanceId)
	  public virtual IList<Attachment> findAttachmentsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		return DbEntityManager.selectList("selectAttachmentsByProcessInstanceId", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Attachment> findAttachmentsByTaskId(String taskId)
	  public virtual IList<Attachment> findAttachmentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		return DbEntityManager.selectList("selectAttachmentsByTaskId", taskId);
	  }

	  public virtual void addRemovalTimeToAttachmentsByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(AttachmentEntity), "updateAttachmentsByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToAttachmentsByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(AttachmentEntity), "updateAttachmentsByProcessInstanceId", parameters);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteAttachmentsByTaskId(String taskId)
	  public virtual void deleteAttachmentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		IList<AttachmentEntity> attachments = DbEntityManager.selectList("selectAttachmentsByTaskId", taskId);
		foreach (AttachmentEntity attachment in attachments)
		{
		  string contentId = attachment.ContentId;
		  if (!string.ReferenceEquals(contentId, null))
		  {
			ByteArrayManager.deleteByteArrayById(contentId);
		  }
		  DbEntityManager.delete(attachment);
		}
	  }

	  public virtual void deleteAttachmentsByProcessInstanceIds(IList<string> processInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceIds"] = processInstanceIds;
		deleteAttachments(parameters);
	  }

	  public virtual void deleteAttachmentsByTaskProcessInstanceIds(IList<string> processInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskProcessInstanceIds"] = processInstanceIds;
		deleteAttachments(parameters);
	  }

	  public virtual void deleteAttachmentsByTaskCaseInstanceIds(IList<string> caseInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["caseInstanceIds"] = caseInstanceIds;
		deleteAttachments(parameters);
	  }

	  protected internal virtual void deleteAttachments(IDictionary<string, object> parameters)
	  {
		DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteAttachmentByteArraysByIds", parameters);
		DbEntityManager.deletePreserveOrder(typeof(AttachmentEntity), "deleteAttachmentByIds", parameters);
	  }

	  public virtual Attachment findAttachmentByTaskIdAndAttachmentId(string taskId, string attachmentId)
	  {
		checkHistoryEnabled();

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["taskId"] = taskId;
		parameters["id"] = attachmentId;

		return (AttachmentEntity) DbEntityManager.selectOne("selectAttachmentByTaskIdAndAttachmentId", parameters);
	  }

	  public virtual DbOperation deleteAttachmentsByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(AttachmentEntity), "deleteAttachmentsByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}


}