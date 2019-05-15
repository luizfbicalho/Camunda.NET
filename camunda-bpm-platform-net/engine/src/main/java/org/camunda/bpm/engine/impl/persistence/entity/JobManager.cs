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
	using org.camunda.bpm.engine.impl;
	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using org.camunda.bpm.engine.impl.jobexecutor;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.JOB_HANDLER_CONFIG_PROPERTY_DELIMITER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.TimerEventJobHandler.JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class JobManager : AbstractManager
	{

	  public static QueryOrderingProperty JOB_PRIORITY_ORDERING_PROPERTY = new QueryOrderingProperty(null, JobQueryProperty_Fields.PRIORITY);
	  public static QueryOrderingProperty JOB_TYPE_ORDERING_PROPERTY = new QueryOrderingProperty(null, JobQueryProperty_Fields.TYPE);
	  public static QueryOrderingProperty JOB_DUEDATE_ORDERING_PROPERTY = new QueryOrderingProperty(null, JobQueryProperty_Fields.DUEDATE);

	  static JobManager()
	  {
		JOB_PRIORITY_ORDERING_PROPERTY.Direction = Direction.DESCENDING;
		JOB_TYPE_ORDERING_PROPERTY.Direction = Direction.DESCENDING;
		JOB_DUEDATE_ORDERING_PROPERTY.Direction = Direction.ASCENDING;
	  }

	  public virtual void updateJob(JobEntity job)
	  {
		DbEntityManager.merge(job);
	  }

	  public virtual void insertJob(JobEntity job)
	  {
		job.CreateTime = ClockUtil.CurrentTime;

		DbEntityManager.insert(job);
		HistoricJobLogManager.fireJobCreatedEvent(job);
	  }

	  public virtual void deleteJob(JobEntity job)
	  {
		deleteJob(job, true);
	  }

	  public virtual void deleteJob(JobEntity job, bool fireDeleteEvent)
	  {
		DbEntityManager.delete(job);

		if (fireDeleteEvent)
		{
		  HistoricJobLogManager.fireJobDeletedEvent(job);
		}

	  }

	  public virtual void insertAndHintJobExecutor(JobEntity jobEntity)
	  {
		jobEntity.insert();
		if (Context.ProcessEngineConfiguration.HintJobExecutor)
		{
		  hintJobExecutor(jobEntity);
		}
	  }

	  public virtual void send(MessageEntity message)
	  {
		message.insert();
		if (Context.ProcessEngineConfiguration.HintJobExecutor)
		{
		  hintJobExecutor(message);
		}
	  }

	  public virtual void schedule(TimerEntity timer)
	  {
		DateTime duedate = timer.Duedate;
		ensureNotNull("duedate", duedate);
		timer.insert();
		hintJobExecutorIfNeeded(timer, duedate);
	  }

	  public virtual void reschedule(JobEntity jobEntity, DateTime newDuedate)
	  {
		((EverLivingJobEntity)jobEntity).init(Context.CommandContext, true);
		jobEntity.SuspensionState = SuspensionState_Fields.ACTIVE.StateCode;
		jobEntity.Duedate = newDuedate;
		hintJobExecutorIfNeeded(jobEntity, newDuedate);
	  }

	  private void hintJobExecutorIfNeeded(JobEntity jobEntity, DateTime duedate)
	  {
		// Check if this timer fires before the next time the job executor will check for new timers to fire.
		// This is highly unlikely because normally waitTimeInMillis is 5000 (5 seconds)
		// and timers are usually set further in the future
		JobExecutor jobExecutor = Context.ProcessEngineConfiguration.JobExecutor;
		int waitTimeInMillis = jobExecutor.WaitTimeInMillis;
		if (duedate.Ticks < (ClockUtil.CurrentTime.Ticks + waitTimeInMillis))
		{
		  hintJobExecutor(jobEntity);
		}
	  }

	  protected internal virtual void hintJobExecutor(JobEntity job)
	  {
		JobExecutor jobExecutor = Context.ProcessEngineConfiguration.JobExecutor;
		if (!jobExecutor.Active)
		{
		  return;
		}

		JobExecutorContext jobExecutorContext = Context.JobExecutorContext;
		TransactionListener transactionListener = null;
		if (!job.Suspended && job.Exclusive && jobExecutorContext != null && jobExecutorContext.ExecutingExclusiveJob && areInSameProcessInstance(job, jobExecutorContext.CurrentJob))
		{
		  // lock job & add to the queue of the current processor
		  DateTime currentTime = ClockUtil.CurrentTime;
		  job.LockExpirationTime = new DateTime(currentTime.Ticks + jobExecutor.LockTimeInMillis);
		  job.LockOwner = jobExecutor.LockOwner;
		  transactionListener = new ExclusiveJobAddedNotification(job.Id, jobExecutorContext);
		}
		else
		{
		  // notify job executor:
		  transactionListener = new MessageAddedNotification(jobExecutor);
		}
		Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, transactionListener);
	  }

	  protected internal virtual bool areInSameProcessInstance(JobEntity job1, JobEntity job2)
	  {
		if (job1 == null || job2 == null)
		{
		  return false;
		}

		string instance1 = job1.ProcessInstanceId;
		string instance2 = job2.ProcessInstanceId;

		return !string.ReferenceEquals(instance1, null) && instance1.Equals(instance2);
	  }

	  public virtual void cancelTimers(ExecutionEntity execution)
	  {
		IList<TimerEntity> timers = Context.CommandContext.JobManager.findTimersByExecutionId(execution.Id);

		foreach (TimerEntity timer in timers)
		{
		  timer.delete();
		}
	  }

	  public virtual JobEntity findJobById(string jobId)
	  {
		return (JobEntity) DbEntityManager.selectOne("selectJob", jobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<JobEntity> findNextJobsToExecute(Page page)
	  public virtual IList<JobEntity> findNextJobsToExecute(Page page)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		DateTime now = ClockUtil.CurrentTime;
		@params["now"] = now;
		@params["alwaysSetDueDate"] = EnsureJobDueDateNotNull;
		@params["deploymentAware"] = Context.ProcessEngineConfiguration.JobExecutorDeploymentAware;
		if (Context.ProcessEngineConfiguration.JobExecutorDeploymentAware)
		{
		  ISet<string> registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
		  if (registeredDeployments.Count > 0)
		  {
			@params["deploymentIds"] = registeredDeployments;
		  }
		}

		IList<QueryOrderingProperty> orderingProperties = new List<QueryOrderingProperty>();
		if (Context.ProcessEngineConfiguration.JobExecutorAcquireByPriority)
		{
		  orderingProperties.Add(JOB_PRIORITY_ORDERING_PROPERTY);
		}
		if (Context.ProcessEngineConfiguration.JobExecutorPreferTimerJobs)
		{
		  orderingProperties.Add(JOB_TYPE_ORDERING_PROPERTY);
		}
		if (Context.ProcessEngineConfiguration.JobExecutorAcquireByDueDate)
		{
		  orderingProperties.Add(JOB_DUEDATE_ORDERING_PROPERTY);
		}

		@params["orderingProperties"] = orderingProperties;
		// don't apply default sorting
		@params["applyOrdering"] = orderingProperties.Count > 0;

		return DbEntityManager.selectList("selectNextJobsToExecute", @params, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<JobEntity> findJobsByExecutionId(String executionId)
	  public virtual IList<JobEntity> findJobsByExecutionId(string executionId)
	  {
		return DbEntityManager.selectList("selectJobsByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<JobEntity> findJobsByProcessInstanceId(String processInstanceId)
	  public virtual IList<JobEntity> findJobsByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectJobsByProcessInstanceId", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<JobEntity> findJobsByJobDefinitionId(String jobDefinitionId)
	  public virtual IList<JobEntity> findJobsByJobDefinitionId(string jobDefinitionId)
	  {
		return DbEntityManager.selectList("selectJobsByJobDefinitionId", jobDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<org.camunda.bpm.engine.runtime.Job> findJobsByHandlerType(String handlerType)
	  public virtual IList<Job> findJobsByHandlerType(string handlerType)
	  {
		return DbEntityManager.selectList("selectJobsByHandlerType", handlerType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<TimerEntity> findUnlockedTimersByDuedate(Date duedate, Page page)
	  public virtual IList<TimerEntity> findUnlockedTimersByDuedate(DateTime duedate, Page page)
	  {
		const string query = "selectUnlockedTimersByDuedate";
		return DbEntityManager.selectList(query, duedate, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<TimerEntity> findTimersByExecutionId(String executionId)
	  public virtual IList<TimerEntity> findTimersByExecutionId(string executionId)
	  {
		return DbEntityManager.selectList("selectTimersByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<org.camunda.bpm.engine.runtime.Job> findJobsByQueryCriteria(JobQueryImpl jobQuery, Page page)
	  public virtual IList<Job> findJobsByQueryCriteria(JobQueryImpl jobQuery, Page page)
	  {
		configureQuery(jobQuery);
		return DbEntityManager.selectList("selectJobByQueryCriteria", jobQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public List<JobEntity> findJobsByConfiguration(String jobHandlerType, String jobHandlerConfiguration, String tenantId)
	  public virtual IList<JobEntity> findJobsByConfiguration(string jobHandlerType, string jobHandlerConfiguration, string tenantId)
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["handlerType"] = jobHandlerType;
		@params["handlerConfiguration"] = jobHandlerConfiguration;
		@params["tenantId"] = tenantId;

		if (TimerCatchIntermediateEventJobHandler.TYPE.Equals(jobHandlerType) || TimerExecuteNestedActivityJobHandler.TYPE.Equals(jobHandlerType) || TimerStartEventJobHandler.TYPE.Equals(jobHandlerType) || TimerStartEventSubprocessJobHandler.TYPE.Equals(jobHandlerType))
		{

		  string queryValue = jobHandlerConfiguration + JOB_HANDLER_CONFIG_PROPERTY_DELIMITER + JOB_HANDLER_CONFIG_PROPERTY_FOLLOW_UP_JOB_CREATED;
		  @params["handlerConfigurationWithFollowUpJobCreatedProperty"] = queryValue;
		}

		return DbEntityManager.selectList("selectJobsByConfiguration", @params);
	  }

	  public virtual long findJobCountByQueryCriteria(JobQueryImpl jobQuery)
	  {
		configureQuery(jobQuery);
		return (long?) DbEntityManager.selectOne("selectJobCountByQueryCriteria", jobQuery).Value;
	  }

	  public virtual void updateJobSuspensionStateById(string jobId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["jobId"] = jobId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobSuspensionStateByJobDefinitionId(string jobDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["jobDefinitionId"] = jobDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobSuspensionStateByProcessInstanceId(string processInstanceId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateStartTimerJobSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = true;
		parameters["processDefinitionTenantId"] = processDefinitionTenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateStartTimerJobSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateStartTimerJobSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = true;
		parameters["processDefinitionTenantId"] = processDefinitionTenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		parameters["handlerType"] = TimerStartEventJobHandler.TYPE;
		DbEntityManager.update(typeof(JobEntity), "updateJobSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateFailedJobRetriesByJobDefinitionId(string jobDefinitionId, int retries)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["jobDefinitionId"] = jobDefinitionId;
		parameters["retries"] = retries;
		DbEntityManager.update(typeof(JobEntity), "updateFailedJobRetriesByParameters", parameters);
	  }

	  public virtual void updateJobPriorityByDefinitionId(string jobDefinitionId, long priority)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["jobDefinitionId"] = jobDefinitionId;
		parameters["priority"] = priority;
		DbEntityManager.update(typeof(JobEntity), "updateJobPriorityByDefinitionId", parameters);
	  }

	  protected internal virtual void configureQuery(JobQueryImpl query)
	  {
		AuthorizationManager.configureJobQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  protected internal virtual bool EnsureJobDueDateNotNull
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.EnsureJobDueDateNotNull;
		  }
	  }
	}

}