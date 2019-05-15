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

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricJobLogManager : AbstractHistoricManager
	{

	  // select /////////////////////////////////////////////////////////////////

	  public virtual HistoricJobLogEventEntity findHistoricJobLogById(string historicJobLogId)
	  {
		return (HistoricJobLogEventEntity) DbEntityManager.selectOne("selectHistoricJobLog", historicJobLogId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricJobLog> findHistoricJobLogsByDeploymentId(String deploymentId)
	  public virtual IList<HistoricJobLog> findHistoricJobLogsByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectHistoricJobLogByDeploymentId", deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricJobLog> findHistoricJobLogsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricJobLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricJobLog> findHistoricJobLogsByQueryCriteria(HistoricJobLogQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectHistoricJobLogByQueryCriteria", query, page);
	  }

	  public virtual long findHistoricJobLogsCountByQueryCriteria(HistoricJobLogQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectHistoricJobLogCountByQueryCriteria", query).Value;
	  }

	  // update ///////////////////////////////////////////////////////////////////

	  public virtual void addRemovalTimeToJobLogByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricJobLogEventEntity), "updateJobLogByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToJobLogByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricJobLogEventEntity), "updateJobLogByProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToJobLogByBatchId(string batchId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["batchId"] = batchId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricJobLogEventEntity), "updateJobLogByBatchId", parameters);

		DbEntityManager.updatePreserveOrder(typeof(ByteArrayEntity), "updateByteArraysByBatchId", parameters);
	  }

	  // delete ///////////////////////////////////////////////////////////////////

	  public virtual void deleteHistoricJobLogById(string id)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("id", id);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogById", id);
		}
	  }

	  public virtual void deleteHistoricJobLogByJobId(string jobId)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("jobId", jobId);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByJobId", jobId);
		}
	  }

	  public virtual void deleteHistoricJobLogsByProcessInstanceIds(IList<string> processInstanceIds)
	  {
		deleteExceptionByteArrayByParameterMap("processInstanceIdIn", processInstanceIds.ToArray());
		DbEntityManager.deletePreserveOrder(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByProcessInstanceIds", processInstanceIds);
	  }

	  public virtual void deleteHistoricJobLogsByProcessDefinitionId(string processDefinitionId)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("processDefinitionId", processDefinitionId);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByProcessDefinitionId", processDefinitionId);
		}
	  }

	  public virtual void deleteHistoricJobLogsByDeploymentId(string deploymentId)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("deploymentId", deploymentId);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByDeploymentId", deploymentId);
		}
	  }

	  public virtual void deleteHistoricJobLogsByHandlerType(string handlerType)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("handlerType", handlerType);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByHandlerType", handlerType);
		}
	  }

	  public virtual void deleteHistoricJobLogsByJobDefinitionId(string jobDefinitionId)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("jobDefinitionId", jobDefinitionId);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByJobDefinitionId", jobDefinitionId);
		}
	  }

	  public virtual void deleteHistoricJobLogByBatchIds(IList<string> historicBatchIds)
	  {
		if (HistoryEnabled)
		{
		  deleteExceptionByteArrayByParameterMap("historicBatchIdIn", historicBatchIds);
		  DbEntityManager.delete(typeof(HistoricJobLogEventEntity), "deleteHistoricJobLogByBatchIds", historicBatchIds);
		}
	  }

	  public virtual DbOperation deleteJobLogByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricJobLogEventEntity), "deleteJobLogByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  // byte array delete ////////////////////////////////////////////////////////

	  protected internal virtual void deleteExceptionByteArrayByParameterMap(string key, object value)
	  {
		EnsureUtil.ensureNotNull(key, value);
		IDictionary<string, object> parameterMap = new Dictionary<string, object>();
		parameterMap[key] = value;
		DbEntityManager.delete(typeof(ByteArrayEntity), "deleteExceptionByteArraysByIds", parameterMap);
	  }

	  // fire history events ///////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireJobCreatedEvent(final org.camunda.bpm.engine.runtime.Job job)
	  public virtual void fireJobCreatedEvent(Job job)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.JOB_CREATE, job))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, job));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricJobLogManager outerInstance;

		  private Job job;

		  public HistoryEventCreatorAnonymousInnerClass(HistoricJobLogManager outerInstance, Job job)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricJobLogCreateEvt(job);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireJobFailedEvent(final org.camunda.bpm.engine.runtime.Job job, final Throwable exception)
	  public virtual void fireJobFailedEvent(Job job, Exception exception)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.JOB_FAIL, job))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this, job, exception));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricJobLogManager outerInstance;

		  private Job job;
		  private Exception exception;

		  public HistoryEventCreatorAnonymousInnerClass2(HistoricJobLogManager outerInstance, Job job, Exception exception)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
			  this.exception = exception;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricJobLogFailedEvt(job, exception);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireJobSuccessfulEvent(final org.camunda.bpm.engine.runtime.Job job)
	  public virtual void fireJobSuccessfulEvent(Job job)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.JOB_SUCCESS, job))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass3(this, job));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass3 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricJobLogManager outerInstance;

		  private Job job;

		  public HistoryEventCreatorAnonymousInnerClass3(HistoricJobLogManager outerInstance, Job job)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricJobLogSuccessfulEvt(job);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void fireJobDeletedEvent(final org.camunda.bpm.engine.runtime.Job job)
	  public virtual void fireJobDeletedEvent(Job job)
	  {
		if (isHistoryEventProduced(HistoryEventTypes.JOB_DELETE, job))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass4(this, job));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass4 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly HistoricJobLogManager outerInstance;

		  private Job job;

		  public HistoryEventCreatorAnonymousInnerClass4(HistoricJobLogManager outerInstance, Job job)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricJobLogDeleteEvt(job);
		  }
	  }


	  // helper /////////////////////////////////////////////////////////

	  protected internal virtual bool isHistoryEventProduced(HistoryEventType eventType, Job job)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;
		return historyLevel.isHistoryEventProduced(eventType, job);
	  }

	  protected internal virtual void configureQuery(HistoricJobLogQueryImpl query)
	  {
		AuthorizationManager.configureHistoricJobLogQuery(query);
		TenantManager.configureQuery(query);
	  }

	}

}