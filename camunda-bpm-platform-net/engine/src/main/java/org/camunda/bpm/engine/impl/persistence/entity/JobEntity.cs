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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ExceptionUtil.createJobExceptionByteArray;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.StringUtil.toByteArray;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using IncidentHandler = org.camunda.bpm.engine.impl.incident.IncidentHandler;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// Stub of the common parts of a Job. You will normally work with a subclass of
	/// JobEntity, such as <seealso cref="TimerEntity"/> or <seealso cref="MessageEntity"/>.
	/// 
	/// @author Tom Baeyens
	/// @author Nick Burch
	/// @author Dave Syer
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public abstract class JobEntity : Job, DbEntity, HasDbRevision, HasDbReferences
	{

	  private static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public const bool DEFAULT_EXCLUSIVE = true;
	  public const int DEFAULT_RETRIES = 3;

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal DateTime duedate;

	  protected internal string lockOwner = null;
	  protected internal DateTime lockExpirationTime = null;

	  protected internal string executionId = null;
	  protected internal string processInstanceId = null;

	  protected internal string processDefinitionId = null;
	  protected internal string processDefinitionKey = null;

	  protected internal bool isExclusive = DEFAULT_EXCLUSIVE;

	  protected internal int retries = DEFAULT_RETRIES;

	  // entity is active by default
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;

	  protected internal string jobHandlerType = null;
	  protected internal string jobHandlerConfiguration = null;

	  protected internal ByteArrayEntity exceptionByteArray;
	  protected internal string exceptionByteArrayId;

	  protected internal string exceptionMessage;

	  protected internal string deploymentId;

	  protected internal string jobDefinitionId;

	  protected internal long priority = DefaultJobPriorityProvider.DEFAULT_PRIORITY;

	  protected internal string tenantId;

	  protected internal DateTime createTime;

	  // runtime state /////////////////////////////
	  protected internal string activityId;
	  protected internal JobDefinition jobDefinition;
	  protected internal ExecutionEntity execution;

	  // sequence counter //////////////////////////
	  protected internal long sequenceCounter = 1;

	  public virtual void execute(CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(executionId, null))
		{
		  ExecutionEntity execution = Execution;
		  ensureNotNull("Cannot find execution with id '" + executionId + "' referenced from job '" + this + "'", "execution", execution);
		}

		// initialize activity id
		ActivityId;

		// increment sequence counter before job execution
		incrementSequenceCounter();

		preExecute(commandContext);
		JobHandler jobHandler = JobHandler;
		JobHandlerConfiguration configuration = JobHandlerConfiguration;
		ensureNotNull("Cannot find job handler '" + jobHandlerType + "' from job '" + this + "'", "jobHandler", jobHandler);
		jobHandler.execute(configuration, execution, commandContext, tenantId);
		postExecute(commandContext);
	  }

	  protected internal virtual void preExecute(CommandContext commandContext)
	  {
		// nothing to do
	  }

	  protected internal virtual void postExecute(CommandContext commandContext)
	  {
		LOG.debugJobExecuted(this);
		delete(true);
		commandContext.HistoricJobLogManager.fireJobSuccessfulEvent(this);
	  }

	  public virtual void init(CommandContext commandContext)
	  {
		// nothing to do
	  }

	  public virtual void insert()
	  {
		CommandContext commandContext = Context.CommandContext;

		// add link to execution and deployment
		ExecutionEntity execution = Execution;
		if (execution != null)
		{
		  execution.addJob(this);

		  ProcessDefinitionImpl processDefinition = execution.getProcessDefinition();
		  this.deploymentId = processDefinition.DeploymentId;
		}

		commandContext.JobManager.insertJob(this);
	  }

	  public virtual void delete()
	  {
		delete(false);
	  }

	  public virtual void delete(bool incidentResolved)
	  {
		CommandContext commandContext = Context.CommandContext;

		incrementSequenceCounter();

		// clean additional data related to this job
		JobHandler jobHandler = JobHandler;
		if (jobHandler != null)
		{
		  jobHandler.onDelete(JobHandlerConfiguration, this);
		}

		// fire delete event if this job is not being executed
		bool executingJob = this.Equals(commandContext.CurrentJob);
		commandContext.JobManager.deleteJob(this, !executingJob);

		// Also delete the job's exception byte array
		if (!string.ReferenceEquals(exceptionByteArrayId, null))
		{
		  commandContext.ByteArrayManager.deleteByteArrayById(exceptionByteArrayId);
		}

		// remove link to execution
		ExecutionEntity execution = Execution;
		if (execution != null)
		{
		  execution.removeJob(this);
		}

		removeFailedJobIncident(incidentResolved);
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["executionId"] = executionId;
			persistentState["lockOwner"] = lockOwner;
			persistentState["lockExpirationTime"] = lockExpirationTime;
			persistentState["retries"] = retries;
			persistentState["duedate"] = duedate;
			persistentState["exceptionMessage"] = exceptionMessage;
			persistentState["suspensionState"] = suspensionState;
			persistentState["processDefinitionId"] = processDefinitionId;
			persistentState["jobDefinitionId"] = jobDefinitionId;
			persistentState["deploymentId"] = deploymentId;
			persistentState["jobHandlerConfiguration"] = jobHandlerConfiguration;
			persistentState["priority"] = priority;
			persistentState["tenantId"] = tenantId;
			if (!string.ReferenceEquals(exceptionByteArrayId, null))
			{
			  persistentState["exceptionByteArrayId"] = exceptionByteArrayId;
			}
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual ExecutionEntity Execution
	  {
		  set
		  {
			if (value != null)
			{
			  this.execution = value;
			  executionId = value.Id;
			  processInstanceId = value.ProcessInstanceId;
			  this.execution.addJob(this);
			}
			else
			{
			  this.execution.removeJob(this);
			  this.execution = value;
			  processInstanceId = null;
			  executionId = null;
			}
		  }
		  get
		  {
			ensureExecutionInitialized();
			return execution;
		  }
	  }

	  // sequence counter /////////////////////////////////////////////////////////

	  public virtual long SequenceCounter
	  {
		  get
		  {
			return sequenceCounter;
		  }
		  set
		  {
			this.sequenceCounter = value;
		  }
	  }


	  public virtual void incrementSequenceCounter()
	  {
		sequenceCounter++;
	  }

	  // getters and setters //////////////////////////////////////////////////////

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



	  protected internal virtual void ensureExecutionInitialized()
	  {
		if (execution == null && !string.ReferenceEquals(executionId, null))
		{
		  execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
		}
	  }

	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
		  set
		  {
			// if value should be set to a negative value set it to 0
			if (value < 0)
			{
			  value = 0;
			}
    
			// Assuming: if the number of value will
			// be changed from 0 to x (x >= 1), means
			// that the corresponding incident is resolved.
			if (this.retries == 0 && value > 0)
			{
			  removeFailedJobIncident(true);
			}
    
			// If the value will be set to 0, an
			// incident has to be created.
			if (value == 0 && this.retries > 0)
			{
			  createFailedJobIncident();
			}
			this.retries = value;
		  }
	  }


	  // special setter for MyBatis which does not influence incidents
	  public virtual int RetriesFromPersistence
	  {
		  set
		  {
			this.retries = value;
		  }
	  }

	  protected internal virtual void createFailedJobIncident()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		if (processEngineConfiguration.CreateIncidentOnFailedJobEnabled)
		{

		  string incidentHandlerType = org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE;

		  // make sure job has an ID set:
		  if (string.ReferenceEquals(id, null))
		  {
			id = processEngineConfiguration.IdGenerator.NextId;

		  }
		  else
		  {
			// check whether there exists already an incident
			// for this job
			IList<Incident> failedJobIncidents = Context.CommandContext.IncidentManager.findIncidentByConfigurationAndIncidentType(id, incidentHandlerType);

			if (failedJobIncidents.Count > 0)
			{
			  return;
			}

		  }

		  IncidentContext incidentContext = createIncidentContext();
		  incidentContext.ActivityId = ActivityId;

		  processEngineConfiguration.getIncidentHandler(incidentHandlerType).handleIncident(incidentContext, exceptionMessage);

		}
	  }

	  protected internal virtual void removeFailedJobIncident(bool incidentResolved)
	  {
		IncidentHandler handler = Context.ProcessEngineConfiguration.getIncidentHandler(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE);

		IncidentContext incidentContext = createIncidentContext();

		if (incidentResolved)
		{
		  handler.resolveIncident(incidentContext);
		}
		else
		{
		  handler.deleteIncident(incidentContext);
		}
	  }

	  protected internal virtual IncidentContext createIncidentContext()
	  {
		IncidentContext incidentContext = new IncidentContext();
		incidentContext.ProcessDefinitionId = processDefinitionId;
		incidentContext.ExecutionId = executionId;
		incidentContext.TenantId = tenantId;
		incidentContext.Configuration = id;
		incidentContext.JobDefinitionId = jobDefinitionId;

		return incidentContext;
	  }

	  public virtual string ExceptionStacktrace
	  {
		  get
		  {
			ByteArrayEntity byteArray = ExceptionByteArray;
			return ExceptionUtil.getExceptionStacktrace(byteArray);
		  }
		  set
		  {
			sbyte[] exceptionBytes = toByteArray(value);
    
			ByteArrayEntity byteArray = ExceptionByteArray;
    
			if (byteArray == null)
			{
			  byteArray = createJobExceptionByteArray(exceptionBytes, ResourceTypes.RUNTIME);
			  exceptionByteArrayId = byteArray.Id;
			  exceptionByteArray = byteArray;
			}
			else
			{
			  byteArray.Bytes = exceptionBytes;
			}
		  }
	  }

	  public virtual int SuspensionState
	  {
		  set
		  {
			this.suspensionState = value;
		  }
		  get
		  {
			return suspensionState;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }

	  public virtual string LockOwner
	  {
		  get
		  {
			return lockOwner;
		  }
		  set
		  {
			this.lockOwner = value;
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

	  public virtual bool Exclusive
	  {
		  get
		  {
			return isExclusive;
		  }
		  set
		  {
			this.isExclusive = value;
		  }
	  }


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


	  public virtual DateTime Duedate
	  {
		  get
		  {
			return duedate;
		  }
		  set
		  {
			this.duedate = value;
		  }
	  }



	  protected internal virtual JobHandler JobHandler
	  {
		  get
		  {
			IDictionary<string, JobHandler> jobHandlers = Context.ProcessEngineConfiguration.JobHandlers;
			return jobHandlers[jobHandlerType];
		  }
	  }

	  public virtual JobHandlerConfiguration JobHandlerConfiguration
	  {
		  get
		  {
			return JobHandler.newConfiguration(jobHandlerConfiguration);
		  }
		  set
		  {
			this.jobHandlerConfiguration = value.toCanonicalString();
		  }
	  }


	  public virtual string JobHandlerType
	  {
		  get
		  {
			return jobHandlerType;
		  }
		  set
		  {
			this.jobHandlerType = value;
		  }
	  }


	  public virtual string JobHandlerConfigurationRaw
	  {
		  get
		  {
			return jobHandlerConfiguration;
		  }
		  set
		  {
			this.jobHandlerConfiguration = value;
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


	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage;
		  }
		  set
		  {
			this.exceptionMessage = StringUtil.trimToMaximumLengthAllowed(value);
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }


	  public virtual JobDefinition JobDefinition
	  {
		  get
		  {
			ensureJobDefinitionInitialized();
			return jobDefinition;
		  }
		  set
		  {
			this.jobDefinition = value;
			if (value != null)
			{
			  jobDefinitionId = value.Id;
			}
			else
			{
			  jobDefinitionId = null;
			}
		  }
	  }


	  protected internal virtual void ensureJobDefinitionInitialized()
	  {
		if (jobDefinition == null && !string.ReferenceEquals(jobDefinitionId, null))
		{
		  jobDefinition = Context.CommandContext.JobDefinitionManager.findById(jobDefinitionId);
		}
	  }


	  public virtual string ExceptionByteArrayId
	  {
		  get
		  {
			return exceptionByteArrayId;
		  }
	  }

	  protected internal virtual ByteArrayEntity ExceptionByteArray
	  {
		  get
		  {
			ensureExceptionByteArrayInitialized();
			return exceptionByteArray;
		  }
	  }

	  protected internal virtual void ensureExceptionByteArrayInitialized()
	  {
		if (exceptionByteArray == null && !string.ReferenceEquals(exceptionByteArrayId, null))
		{
		  exceptionByteArray = Context.CommandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), exceptionByteArrayId);
		}
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual bool InInconsistentLockState
	  {
		  get
		  {
			return (!string.ReferenceEquals(lockOwner, null) && lockExpirationTime == null) || (retries == 0 && (!string.ReferenceEquals(lockOwner, null) || lockExpirationTime != null));
		  }
	  }

	  public virtual void resetLock()
	  {
		this.lockOwner = null;
		this.lockExpirationTime = null;
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			ensureActivityIdInitialized();
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
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


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  protected internal virtual void ensureActivityIdInitialized()
	  {
		if (string.ReferenceEquals(activityId, null))
		{
		  JobDefinition jobDefinition = JobDefinition;
		  if (jobDefinition != null)
		  {
			activityId = jobDefinition.ActivityId;
		  }
		  else
		  {
			ExecutionEntity execution = Execution;
			if (execution != null)
			{
			  activityId = execution.ActivityId;
			}
		  }
		}
	  }

	  /// 
	  /// <summary>
	  /// Unlock from current lock owner
	  /// 
	  /// </summary>

	  public virtual void unlock()
	  {
		this.lockOwner = null;
		this.lockExpirationTime = null;
	  }

	  public abstract string Type {get;}

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(id, null)) ? 0 : id.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		JobEntity other = (JobEntity) obj;
		if (string.ReferenceEquals(id, null))
		{
		  if (!string.ReferenceEquals(other.id, null))
		  {
			return false;
		  }
		}
		else if (!id.Equals(other.id))
		{
		  return false;
		}
		return true;
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
    
			if (!string.ReferenceEquals(exceptionByteArrayId, null))
			{
			  referenceIdAndClass[exceptionByteArrayId] = typeof(ByteArrayEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", duedate=" + duedate + ", lockOwner=" + lockOwner + ", lockExpirationTime=" + lockExpirationTime + ", executionId=" + executionId + ", processInstanceId=" + processInstanceId + ", isExclusive=" + isExclusive + ", isExclusive=" + isExclusive + ", jobDefinitionId=" + jobDefinitionId + ", jobHandlerType=" + jobHandlerType + ", jobHandlerConfiguration=" + jobHandlerConfiguration + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + exceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + deploymentId + ", priority=" + priority + ", tenantId=" + tenantId + "]";
	  }
	}

}