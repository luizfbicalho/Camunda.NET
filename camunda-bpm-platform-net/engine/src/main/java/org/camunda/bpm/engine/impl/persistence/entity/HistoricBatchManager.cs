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

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using HistoricBatchQueryImpl = org.camunda.bpm.engine.impl.batch.history.HistoricBatchQueryImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	public class HistoricBatchManager : AbstractManager
	{

	  public virtual long findBatchCountByQueryCriteria(HistoricBatchQueryImpl historicBatchQuery)
	  {
		configureQuery(historicBatchQuery);
		return (long?) DbEntityManager.selectOne("selectHistoricBatchCountByQueryCriteria", historicBatchQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.batch.history.HistoricBatch> findBatchesByQueryCriteria(org.camunda.bpm.engine.impl.batch.history.HistoricBatchQueryImpl historicBatchQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricBatch> findBatchesByQueryCriteria(HistoricBatchQueryImpl historicBatchQuery, Page page)
	  {
		configureQuery(historicBatchQuery);
		return DbEntityManager.selectList("selectHistoricBatchesByQueryCriteria", historicBatchQuery, page);
	  }

	  public virtual HistoricBatchEntity findHistoricBatchById(string batchId)
	  {
		return DbEntityManager.selectById(typeof(HistoricBatchEntity), batchId);
	  }

	  public virtual HistoricBatchEntity findHistoricBatchByJobId(string jobId)
	  {
		return (HistoricBatchEntity) DbEntityManager.selectOne("selectHistoricBatchByJobId", jobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findHistoricBatchIdsForCleanup(System.Nullable<int> batchSize, java.util.Map<String, int> batchOperationsForHistoryCleanup, int minuteFrom, int minuteTo)
	  public virtual IList<string> findHistoricBatchIdsForCleanup(int? batchSize, IDictionary<string, int> batchOperationsForHistoryCleanup, int minuteFrom, int minuteTo)
	  {
		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["currentTimestamp"] = ClockUtil.CurrentTime;
		queryParameters["map"] = batchOperationsForHistoryCleanup;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  queryParameters["minuteFrom"] = minuteFrom;
		  queryParameters["minuteTo"] = minuteTo;
		}
		ListQueryParameterObject parameterObject = new ListQueryParameterObject(queryParameters, 0, batchSize.Value);
		parameterObject.OrderingProperties.Add(new QueryOrderingProperty(new QueryPropertyImpl("END_TIME_"), Direction.ASCENDING));

		return (IList<string>) DbEntityManager.selectList("selectHistoricBatchIdsForCleanup", parameterObject);
	  }

	  public virtual void deleteHistoricBatchById(string id)
	  {
		DbEntityManager.delete(typeof(HistoricBatchEntity), "deleteHistoricBatchById", id);
	  }

	  public virtual void deleteHistoricBatchesByIds(IList<string> historicBatchIds)
	  {
		CommandContext commandContext = Context.CommandContext;

		commandContext.HistoricIncidentManager.deleteHistoricIncidentsByBatchId(historicBatchIds);
		commandContext.HistoricJobLogManager.deleteHistoricJobLogByBatchIds(historicBatchIds);

		DbEntityManager.deletePreserveOrder(typeof(HistoricBatchEntity), "deleteHistoricBatchByIds", historicBatchIds);

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void createHistoricBatch(final org.camunda.bpm.engine.impl.batch.BatchEntity batch)
	  public virtual void createHistoricBatch(BatchEntity batch)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.BATCH_START, batch))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, batch));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricBatchManager outerInstance;

		  private BatchEntity batch;

		  public HistoryEventCreatorAnonymousInnerClass(HistoricBatchManager outerInstance, BatchEntity batch)
		  {
			  this.outerInstance = outerInstance;
			  this.batch = batch;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createBatchStartEvent(batch);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void completeHistoricBatch(final org.camunda.bpm.engine.impl.batch.BatchEntity batch)
	  public virtual void completeHistoricBatch(BatchEntity batch)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.BATCH_END, batch))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this, batch));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricBatchManager outerInstance;

		  private BatchEntity batch;

		  public HistoryEventCreatorAnonymousInnerClass2(HistoricBatchManager outerInstance, BatchEntity batch)
		  {
			  this.outerInstance = outerInstance;
			  this.batch = batch;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createBatchEndEvent(batch);
		  }
	  }

	  protected internal virtual void configureQuery(HistoricBatchQueryImpl query)
	  {
		AuthorizationManager.configureHistoricBatchQuery(query);
		TenantManager.configureQuery(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult> findCleanableHistoricBatchesReportByCriteria(org.camunda.bpm.engine.impl.CleanableHistoricBatchReportImpl query, org.camunda.bpm.engine.impl.Page page, java.util.Map<String, int> batchOperationsForHistoryCleanup)
	  public virtual IList<CleanableHistoricBatchReportResult> findCleanableHistoricBatchesReportByCriteria(CleanableHistoricBatchReportImpl query, Page page, IDictionary<string, int> batchOperationsForHistoryCleanup)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		query.Parameter = batchOperationsForHistoryCleanup;
		query.OrderingProperties.add(new QueryOrderingProperty(new QueryPropertyImpl("TYPE_"), Direction.ASCENDING));
		if (batchOperationsForHistoryCleanup.Count == 0)
		{
		  return DbEntityManager.selectList("selectOnlyFinishedBatchesReportEntities", query, page);
		}
		else
		{
		  return DbEntityManager.selectList("selectFinishedBatchesReportEntities", query, page);
		}
	  }

	  public virtual long findCleanableHistoricBatchesReportCountByCriteria(CleanableHistoricBatchReportImpl query, IDictionary<string, int> batchOperationsForHistoryCleanup)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		query.Parameter = batchOperationsForHistoryCleanup;
		if (batchOperationsForHistoryCleanup.Count == 0)
		{
		  return (long?) DbEntityManager.selectOne("selectOnlyFinishedBatchesReportEntitiesCount", query).Value;
		}
		else
		{
		  return (long?) DbEntityManager.selectOne("selectFinishedBatchesReportEntitiesCount", query).Value;
		}
	  }

	  public virtual DbOperation deleteHistoricBatchesByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricBatchEntity), "deleteHistoricBatchesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  public virtual void addRemovalTimeById(string id, DateTime removalTime)
	  {
		CommandContext commandContext = Context.CommandContext;

		commandContext.HistoricIncidentManager.addRemovalTimeToHistoricIncidentsByBatchId(id, removalTime);

		commandContext.HistoricJobLogManager.addRemovalTimeToJobLogByBatchId(id, removalTime);

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["id"] = id;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricBatchEntity), "updateHistoricBatchRemovalTimeById", parameters);
	  }

	}

}