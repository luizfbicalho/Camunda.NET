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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using org.camunda.bpm.engine.impl.history.@event;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;



	public class HistoricExternalTaskLogManager : AbstractManager
	{

	  // select /////////////////////////////////////////////////////////////////

	  public virtual HistoricExternalTaskLogEntity findHistoricExternalTaskLogById(string HistoricExternalTaskLogId)
	  {
		return (HistoricExternalTaskLogEntity) DbEntityManager.selectOne("selectHistoricExternalTaskLog", HistoricExternalTaskLogId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricExternalTaskLog> findHistoricExternalTaskLogsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricExternalTaskLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricExternalTaskLog> findHistoricExternalTaskLogsByQueryCriteria(HistoricExternalTaskLogQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectHistoricExternalTaskLogByQueryCriteria", query, page);
	  }

	  public virtual long findHistoricExternalTaskLogsCountByQueryCriteria(HistoricExternalTaskLogQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectHistoricExternalTaskLogCountByQueryCriteria", query).Value;
	  }

	  // update ///////////////////////////////////////////////////////////////////

	  public virtual void addRemovalTimeToExternalTaskLogByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricExternalTaskLogEntity), "updateExternalTaskLogByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToExternalTaskLogByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricExternalTaskLogEntity), "updateExternalTaskLogByProcessInstanceId", parameters);
	  }

	  // delete ///////////////////////////////////////////////////////////////////

	  public virtual void deleteHistoricExternalTaskLogsByProcessInstanceIds(IList<string> processInstanceIds)
	  {
		deleteExceptionByteArrayByParameterMap("processInstanceIdIn", processInstanceIds.ToArray());
		DbEntityManager.deletePreserveOrder(typeof(HistoricExternalTaskLogEntity), "deleteHistoricExternalTaskLogByProcessInstanceIds", processInstanceIds);
	  }

	  public virtual DbOperation deleteExternalTaskLogByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricExternalTaskLogEntity), "deleteExternalTaskLogByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  // byte array delete ////////////////////////////////////////////////////////

	  protected internal virtual void deleteExceptionByteArrayByParameterMap(string key, object value)
	  {
		EnsureUtil.ensureNotNull(key, value);
		IDictionary<string, object> parameterMap = new Dictionary<string, object>();
		parameterMap[key] = value;
		DbEntityManager.delete(typeof(ByteArrayEntity), "deleteErrorDetailsByteArraysByIds", parameterMap);
	  }

	  // fire history events ///////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireExternalTaskCreatedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
	  public virtual void fireExternalTaskCreatedEvent(ExternalTask externalTask)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.EXTERNAL_TASK_CREATE, externalTask))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, externalTask));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricExternalTaskLogManager outerInstance;

		  private ExternalTask externalTask;

		  public HistoryEventCreatorAnonymousInnerClass(HistoricExternalTaskLogManager outerInstance, ExternalTask externalTask)
		  {
			  this.outerInstance = outerInstance;
			  this.externalTask = externalTask;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricExternalTaskLogCreatedEvt(externalTask);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireExternalTaskFailedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
	  public virtual void fireExternalTaskFailedEvent(ExternalTask externalTask)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.EXTERNAL_TASK_FAIL, externalTask))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this, externalTask));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricExternalTaskLogManager outerInstance;

		  private ExternalTask externalTask;

		  public HistoryEventCreatorAnonymousInnerClass2(HistoricExternalTaskLogManager outerInstance, ExternalTask externalTask)
		  {
			  this.outerInstance = outerInstance;
			  this.externalTask = externalTask;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricExternalTaskLogFailedEvt(externalTask);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireExternalTaskSuccessfulEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
	  public virtual void fireExternalTaskSuccessfulEvent(ExternalTask externalTask)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.EXTERNAL_TASK_SUCCESS, externalTask))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass3(this, externalTask));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass3 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricExternalTaskLogManager outerInstance;

		  private ExternalTask externalTask;

		  public HistoryEventCreatorAnonymousInnerClass3(HistoricExternalTaskLogManager outerInstance, ExternalTask externalTask)
		  {
			  this.outerInstance = outerInstance;
			  this.externalTask = externalTask;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricExternalTaskLogSuccessfulEvt(externalTask);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireExternalTaskDeletedEvent(final org.camunda.bpm.engine.externaltask.ExternalTask externalTask)
	  public virtual void fireExternalTaskDeletedEvent(ExternalTask externalTask)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.EXTERNAL_TASK_DELETE, externalTask))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass4(this, externalTask));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass4 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricExternalTaskLogManager outerInstance;

		  private ExternalTask externalTask;

		  public HistoryEventCreatorAnonymousInnerClass4(HistoricExternalTaskLogManager outerInstance, ExternalTask externalTask)
		  {
			  this.outerInstance = outerInstance;
			  this.externalTask = externalTask;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricExternalTaskLogDeletedEvt(externalTask);
		  }
	  }

	  // helper /////////////////////////////////////////////////////////

	  protected internal virtual bool isHistoryEventProduced(HistoryEventType eventType, ExternalTask externalTask)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;
		return historyLevel.isHistoryEventProduced(eventType, externalTask);
	  }

	  protected internal virtual void configureQuery(HistoricExternalTaskLogQueryImpl query)
	  {
		AuthorizationManager.configureHistoricExternalTaskLogQuery(query);
		TenantManager.configureQuery(query);
	  }
	}

}