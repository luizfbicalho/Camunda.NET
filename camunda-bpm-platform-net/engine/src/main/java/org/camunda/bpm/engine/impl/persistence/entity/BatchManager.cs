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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchQueryImpl = org.camunda.bpm.engine.impl.batch.BatchQueryImpl;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;

	public class BatchManager : AbstractManager
	{

	  public virtual void insertBatch(BatchEntity batch)
	  {
		batch.CreateUserId = CommandContext.AuthenticatedUserId;
		DbEntityManager.insert(batch);
	  }

	  public virtual BatchEntity findBatchById(string id)
	  {
		return DbEntityManager.selectById(typeof(BatchEntity), id);
	  }

	  public virtual long findBatchCountByQueryCriteria(BatchQueryImpl batchQuery)
	  {
		configureQuery(batchQuery);
		return (long?) DbEntityManager.selectOne("selectBatchCountByQueryCriteria", batchQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.batch.Batch> findBatchesByQueryCriteria(org.camunda.bpm.engine.impl.batch.BatchQueryImpl batchQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<Batch> findBatchesByQueryCriteria(BatchQueryImpl batchQuery, Page page)
	  {
		configureQuery(batchQuery);
		return DbEntityManager.selectList("selectBatchesByQueryCriteria", batchQuery, page);
	  }

	  public virtual void updateBatchSuspensionStateById(string batchId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["batchId"] = batchId;
		parameters["suspensionState"] = suspensionState.StateCode;

		ListQueryParameterObject queryParameter = new ListQueryParameterObject();
		queryParameter.Parameter = parameters;

		DbEntityManager.update(typeof(BatchEntity), "updateBatchSuspensionStateByParameters", queryParameter);
	  }

	  protected internal virtual void configureQuery(BatchQueryImpl batchQuery)
	  {
		AuthorizationManager.configureBatchQuery(batchQuery);
		TenantManager.configureQuery(batchQuery);
	  }

	}

}