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
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ByteArrayManager : AbstractManager
	{

	  /// <summary>
	  /// Deletes the <seealso cref="ByteArrayEntity"/> with the given id from the database.
	  /// Important: this operation will NOT do any optimistic locking, to avoid loading the
	  /// bytes in memory. So use this method only in conjunction with an entity that has
	  /// optimistic locking!.
	  /// </summary>
	  public virtual void deleteByteArrayById(string byteArrayEntityId)
	  {
		DbEntityManager.delete(typeof(ByteArrayEntity), "deleteByteArrayNoRevisionCheck", byteArrayEntityId);
	  }

	  public virtual void insertByteArray(ByteArrayEntity arr)
	  {
		arr.CreateTime = ClockUtil.CurrentTime;
		DbEntityManager.insert(arr);
	  }

	  public virtual void addRemovalTimeToByteArraysByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(ByteArrayEntity), "updateByteArraysByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToByteArraysByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(ByteArrayEntity), "updateByteArraysByProcessInstanceId", parameters);
	  }

	  public virtual DbOperation deleteByteArraysByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteByteArraysByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}

}