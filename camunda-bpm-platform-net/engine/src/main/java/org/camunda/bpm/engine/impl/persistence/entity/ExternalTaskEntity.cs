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

	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ExternalTaskActivityBehavior;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using IncidentHandler = org.camunda.bpm.engine.impl.incident.IncidentHandler;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ExceptionUtil.createExceptionByteArray;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.StringUtil.toByteArray;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Askar Akhmerov
	/// 
	/// </summary>
	public class ExternalTaskEntity : ExternalTask, DbEntity, HasDbRevision, HasDbReferences
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  private const string EXCEPTION_NAME = "externalTask.exceptionByteArray";

	  /// <summary>
	  /// Note: <seealso cref="String#length()"/> counts Unicode supplementary
	  /// characters twice, so for a String consisting only of those,
	  /// the limit is effectively MAX_EXCEPTION_MESSAGE_LENGTH / 2
	  /// </summary>
	  public const int MAX_EXCEPTION_MESSAGE_LENGTH = 666;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string topicName;
	  protected internal string workerId;
	  protected internal DateTime lockExpirationTime;
	  protected internal int? retries;
	  protected internal string errorMessage;

	  protected internal ByteArrayEntity errorDetailsByteArray;
	  protected internal string errorDetailsByteArrayId;

	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;
	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string activityId;
	  protected internal string activityInstanceId;
	  protected internal string tenantId;
	  protected internal long priority;

	  protected internal ExecutionEntity execution;

	  protected internal string businessKey;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
		  set
		  {
			this.topicName = value;
		  }
	  }
	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
		  set
		  {
			this.workerId = value;
		  }
	  }
	  public virtual DateTime LockExpirationTime
	  {
		  get
		  {
			return lockExpirationTime;
		  }
		  set
		  {
			this.lockExpirationTime = value;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }
	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }
	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }
	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }
	  public virtual int SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }
	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }
	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }
	  public virtual int? Retries
	  {
		  get
		  {
			return retries;
		  }
		  set
		  {
			this.retries = value;
		  }
	  }
	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
		  set
		  {
			if (!string.ReferenceEquals(value, null) && value.Length > MAX_EXCEPTION_MESSAGE_LENGTH)
			{
			  this.errorMessage = value.Substring(0, MAX_EXCEPTION_MESSAGE_LENGTH);
			}
			else
			{
			  this.errorMessage = value;
			}
		  }
	  }

	  public virtual bool areRetriesLeft()
	  {
		return retries == null || retries > 0;
	  }

	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
		  set
		  {
			this.priority = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["topic"] = topicName;
			persistentState["workerId"] = workerId;
			persistentState["lockExpirationTime"] = lockExpirationTime;
			persistentState["retries"] = retries;
			persistentState["errorMessage"] = errorMessage;
			persistentState["executionId"] = executionId;
			persistentState["processInstanceId"] = processInstanceId;
			persistentState["processDefinitionId"] = processDefinitionId;
			persistentState["processDefinitionKey"] = processDefinitionKey;
			persistentState["activityId"] = activityId;
			persistentState["activityInstanceId"] = activityInstanceId;
			persistentState["suspensionState"] = suspensionState;
			persistentState["tenantId"] = tenantId;
			persistentState["priority"] = priority;
    
			if (!string.ReferenceEquals(errorDetailsByteArrayId, null))
			{
			  persistentState["errorDetailsByteArrayId"] = errorDetailsByteArrayId;
			}
    
			return persistentState;
		  }
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.ExternalTaskManager.insert(this);

		Execution.addExternalTask(this);
	  }

	  /// <summary>
	  /// Method implementation relies on the command context object,
	  /// therefore should be invoked from the commands only
	  /// </summary>
	  /// <returns> error details persisted in byte array table </returns>
	  public virtual string ErrorDetails
	  {
		  get
		  {
			ByteArrayEntity byteArray = ErrorByteArray;
			return ExceptionUtil.getExceptionStacktrace(byteArray);
		  }
		  set
		  {
			EnsureUtil.ensureNotNull("exception", value);
    
			sbyte[] exceptionBytes = toByteArray(value);
    
			ByteArrayEntity byteArray = ErrorByteArray;
    
			if (byteArray == null)
			{
			  byteArray = createExceptionByteArray(EXCEPTION_NAME,exceptionBytes, ResourceTypes.RUNTIME);
			  errorDetailsByteArrayId = byteArray.Id;
			  errorDetailsByteArray = byteArray;
			}
			else
			{
			  byteArray.Bytes = exceptionBytes;
			}
		  }
	  }



	  public virtual string ErrorDetailsByteArrayId
	  {
		  get
		  {
			return errorDetailsByteArrayId;
		  }
	  }

	  protected internal virtual ByteArrayEntity ErrorByteArray
	  {
		  get
		  {
			ensureErrorByteArrayInitialized();
			return errorDetailsByteArray;
		  }
	  }

	  protected internal virtual void ensureErrorByteArrayInitialized()
	  {
		if (errorDetailsByteArray == null && !string.ReferenceEquals(errorDetailsByteArrayId, null))
		{
		  errorDetailsByteArray = Context.CommandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), errorDetailsByteArrayId);
		}
	  }

	  public virtual void delete()
	  {
		deleteFromExecutionAndRuntimeTable();
		produceHistoricExternalTaskDeletedEvent();
	  }

	  protected internal virtual void deleteFromExecutionAndRuntimeTable()
	  {
		Execution.removeExternalTask(this);

		CommandContext commandContext = Context.CommandContext;

		commandContext.ExternalTaskManager.delete(this);

		// Also delete the external tasks's error details byte array
		if (!string.ReferenceEquals(errorDetailsByteArrayId, null))
		{
		  commandContext.ByteArrayManager.deleteByteArrayById(errorDetailsByteArrayId);
		}
	  }

	  public virtual void complete(IDictionary<string, object> variables, IDictionary<string, object> localVariables)
	  {
		ensureActive();

		ExecutionEntity associatedExecution = Execution;

		if (variables != null)
		{
		  associatedExecution.Variables = variables;
		}

		if (localVariables != null)
		{
		  associatedExecution.VariablesLocal = localVariables;
		}

		deleteFromExecutionAndRuntimeTable();

		produceHistoricExternalTaskSuccessfulEvent();

		associatedExecution.signal(null, null);
	  }

	  /// <summary>
	  /// process failed state, make sure that binary entity is created for the errorMessage, shortError
	  /// message does not exceed limit, handle properly retry counts and incidents
	  /// </summary>
	  /// <param name="errorMessage"> - short error message text </param>
	  /// <param name="errorDetails"> - full error details </param>
	  /// <param name="retries"> - updated value of retries left </param>
	  /// <param name="retryDuration"> - used for lockExpirationTime calculation </param>
	  public virtual void failed(string errorMessage, string errorDetails, int retries, long retryDuration)
	  {
		ensureActive();

		this.ErrorMessage = errorMessage;
		if (!string.ReferenceEquals(errorDetails, null))
		{
		  ErrorDetails = errorDetails;
		}
		this.lockExpirationTime = new DateTime(ClockUtil.CurrentTime.Ticks + retryDuration);
		RetriesAndManageIncidents = retries;
		produceHistoricExternalTaskFailedEvent();
	  }

	  public virtual void bpmnError(string errorCode, string errorMessage, IDictionary<string, object> variables)
	  {
		ensureActive();
		ActivityExecution activityExecution = Execution;
		BpmnError bpmnError = null;
		if (!string.ReferenceEquals(errorMessage, null))
		{
		  bpmnError = new BpmnError(errorCode, errorMessage);
		}
		else
		{
		  bpmnError = new BpmnError(errorCode);
		}
		try
		{
		  ExternalTaskActivityBehavior behavior = ((ExternalTaskActivityBehavior) activityExecution.Activity.ActivityBehavior);
		  if (variables != null && variables.Count > 0)
		  {
			activityExecution.Variables = variables;
		  }
		  behavior.propagateBpmnError(bpmnError, activityExecution);
		}
		catch (Exception ex)
		{
		  throw ProcessEngineLogger.CMD_LOGGER.exceptionBpmnErrorPropagationFailed(errorCode, ex);
		}
	  }

	  public virtual int RetriesAndManageIncidents
	  {
		  set
		  {
    
			if (areRetriesLeft() && value <= 0)
			{
			  createIncident();
			}
			else if (!areRetriesLeft() && value > 0)
			{
			  removeIncident();
			}
    
			Retries = value;
		  }
	  }

	  protected internal virtual void createIncident()
	  {
		IncidentHandler incidentHandler = Context.ProcessEngineConfiguration.getIncidentHandler(org.camunda.bpm.engine.runtime.Incident_Fields.EXTERNAL_TASK_HANDLER_TYPE);

		incidentHandler.handleIncident(createIncidentContext(), errorMessage);
	  }

	  protected internal virtual void removeIncident()
	  {
		IncidentHandler handler = Context.ProcessEngineConfiguration.getIncidentHandler(org.camunda.bpm.engine.runtime.Incident_Fields.EXTERNAL_TASK_HANDLER_TYPE);

		handler.resolveIncident(createIncidentContext());
	  }

	  protected internal virtual IncidentContext createIncidentContext()
	  {
		IncidentContext context = new IncidentContext();
		context.ProcessDefinitionId = processDefinitionId;
		context.ExecutionId = executionId;
		context.ActivityId = activityId;
		context.TenantId = tenantId;
		context.Configuration = id;
		return context;
	  }

	  public virtual void @lock(string workerId, long lockDuration)
	  {
		this.workerId = workerId;
		this.lockExpirationTime = new DateTime(ClockUtil.CurrentTime.Ticks + lockDuration);
	  }

	  public virtual ExecutionEntity Execution
	  {
		  get
		  {
			ensureExecutionInitialized();
			return execution;
		  }
		  set
		  {
			this.execution = value;
		  }
	  }


	  protected internal virtual void ensureExecutionInitialized()
	  {
		if (execution == null)
		{
		  execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
		  EnsureUtil.ensureNotNull("Cannot find execution with id " + executionId + " for external task " + id, "execution", execution);
		}
	  }

	  protected internal virtual void ensureActive()
	  {
		if (suspensionState == SuspensionState_Fields.SUSPENDED.StateCode)
		{
		  throw LOG.suspendedEntityException(EntityTypes.EXTERNAL_TASK, id);
		}
	  }

	  public override string ToString()
	  {
		return "ExternalTaskEntity ["
			+ "id=" + id + ", revision=" + revision + ", topicName=" + topicName + ", workerId=" + workerId + ", lockExpirationTime=" + lockExpirationTime + ", priority=" + priority + ", errorMessage=" + errorMessage + ", errorDetailsByteArray=" + errorDetailsByteArray + ", errorDetailsByteArrayId=" + errorDetailsByteArrayId + ", executionId=" + executionId + "]";
	  }

	  public virtual void unlock()
	  {
		workerId = null;
		lockExpirationTime = null;

		Context.CommandContext.ExternalTaskManager.fireExternalTaskAvailableEvent();
	  }

	  public static ExternalTaskEntity createAndInsert(ExecutionEntity execution, string topic, long priority)
	  {
		ExternalTaskEntity externalTask = new ExternalTaskEntity();

		externalTask.TopicName = topic;
		externalTask.ExecutionId = execution.Id;
		externalTask.ProcessInstanceId = execution.ProcessInstanceId;
		externalTask.ProcessDefinitionId = execution.ProcessDefinitionId;
		externalTask.ActivityId = execution.ActivityId;
		externalTask.ActivityInstanceId = execution.ActivityInstanceId;
		externalTask.TenantId = execution.TenantId;
		externalTask.Priority = priority;

		ProcessDefinitionEntity processDefinition = execution.getProcessDefinition();
		externalTask.ProcessDefinitionKey = processDefinition.Key;

		externalTask.insert();
		externalTask.produceHistoricExternalTaskCreatedEvent();

		return externalTask;
	  }

	  protected internal virtual void produceHistoricExternalTaskCreatedEvent()
	  {
		CommandContext commandContext = Context.CommandContext;
		commandContext.HistoricExternalTaskLogManager.fireExternalTaskCreatedEvent(this);
	  }

	  protected internal virtual void produceHistoricExternalTaskFailedEvent()
	  {
		CommandContext commandContext = Context.CommandContext;
		commandContext.HistoricExternalTaskLogManager.fireExternalTaskFailedEvent(this);
	  }

	  protected internal virtual void produceHistoricExternalTaskSuccessfulEvent()
	  {
		CommandContext commandContext = Context.CommandContext;
		commandContext.HistoricExternalTaskLogManager.fireExternalTaskSuccessfulEvent(this);
	  }

	  protected internal virtual void produceHistoricExternalTaskDeletedEvent()
	  {
		CommandContext commandContext = Context.CommandContext;
		commandContext.HistoricExternalTaskLogManager.fireExternalTaskDeletedEvent(this);
	  }

	  public virtual void extendLock(long newLockExpirationTime)
	  {
		ensureActive();
		long newTime = ClockUtil.CurrentTime.Ticks + newLockExpirationTime;
		this.lockExpirationTime = new DateTime(newTime);
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(executionId, null))
			{
			  referenceIdAndClass[executionId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(errorDetailsByteArrayId, null))
			{
			  referenceIdAndClass[errorDetailsByteArrayId] = typeof(ByteArrayEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }
	}

}