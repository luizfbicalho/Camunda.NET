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
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Event = org.camunda.bpm.engine.task.Event;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CommentManager : AbstractHistoricManager
	{

	  public override void delete(DbEntity dbEntity)
	  {
		checkHistoryEnabled();
		base.delete(dbEntity);
	  }

	  public override void insert(DbEntity dbEntity)
	  {
		checkHistoryEnabled();
		base.insert(dbEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Comment> findCommentsByTaskId(String taskId)
	  public virtual IList<Comment> findCommentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		return DbEntityManager.selectList("selectCommentsByTaskId", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Event> findEventsByTaskId(String taskId)
	  public virtual IList<Event> findEventsByTaskId(string taskId)
	  {
		checkHistoryEnabled();

		ListQueryParameterObject query = new ListQueryParameterObject();
		query.Parameter = taskId;
		query.OrderingProperties.Add(new QueryOrderingProperty(new QueryPropertyImpl("TIME_"), Direction.DESCENDING));

		return DbEntityManager.selectList("selectEventsByTaskId", query);
	  }

	  public virtual void deleteCommentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		DbEntityManager.delete(typeof(CommentEntity), "deleteCommentsByTaskId", taskId);
	  }

	  public virtual void deleteCommentsByProcessInstanceIds(IList<string> processInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceIds"] = processInstanceIds;
		deleteComments(parameters);
	  }

	  public virtual void deleteCommentsByTaskProcessInstanceIds(IList<string> processInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskProcessInstanceIds"] = processInstanceIds;
		deleteComments(parameters);
	  }

	  public virtual void deleteCommentsByTaskCaseInstanceIds(IList<string> caseInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskCaseInstanceIds"] = caseInstanceIds;
		deleteComments(parameters);
	  }

	  protected internal virtual void deleteComments(IDictionary<string, object> parameters)
	  {
		DbEntityManager.deletePreserveOrder(typeof(CommentEntity), "deleteCommentsByIds", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Comment> findCommentsByProcessInstanceId(String processInstanceId)
	  public virtual IList<Comment> findCommentsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		return DbEntityManager.selectList("selectCommentsByProcessInstanceId", processInstanceId);
	  }

	  public virtual CommentEntity findCommentByTaskIdAndCommentId(string taskId, string commentId)
	  {
		checkHistoryEnabled();

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["taskId"] = taskId;
		parameters["id"] = commentId;

		return (CommentEntity) DbEntityManager.selectOne("selectCommentByTaskIdAndCommentId", parameters);
	  }

	  public virtual void addRemovalTimeToCommentsByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(CommentEntity), "updateCommentsByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToCommentsByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(CommentEntity), "updateCommentsByProcessInstanceId", parameters);
	  }

	  public virtual DbOperation deleteCommentsByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(CommentEntity), "deleteCommentsByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}

}