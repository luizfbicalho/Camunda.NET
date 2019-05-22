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
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using BpmnExceptionHandler = org.camunda.bpm.engine.impl.bpmn.helper.BpmnExceptionHandler;
	using ErrorPropagationException = org.camunda.bpm.engine.impl.bpmn.helper.ErrorPropagationException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using VariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using VariableStore = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore;
	using VariablesProvider = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore.VariablesProvider;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandContextListener = org.camunda.bpm.engine.impl.interceptor.CommandContextListener;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using TaskListenerInvocation = org.camunda.bpm.engine.impl.task.@delegate.TaskListenerInvocation;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Deivarayan Azhagappan
	/// </summary>
	[Serializable]
	public class TaskEntity : AbstractVariableScope, Task, DelegateTask, DbEntity, HasDbRevision, HasDbReferences, CommandContextListener, VariableStore.VariablesProvider<VariableInstanceEntity>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			variableStore = new VariableStore<VariableInstanceEntity>(this, new TaskEntityReferencer(this));
		}


	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public const string DELETE_REASON_COMPLETED = "completed";
	  public const string DELETE_REASON_DELETED = "deleted";

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string owner;
	  protected internal string assignee;
	  protected internal DelegationState delegationState;

	  protected internal string parentTaskId;
	  [NonSerialized]
	  protected internal TaskEntity parentTask;

	  protected internal string name;
	  protected internal string description;
	  protected internal int priority = org.camunda.bpm.engine.task.Task_Fields.PRIORITY_NORMAL;
	  protected internal DateTime createTime; // The time when the task has been created
	  protected internal DateTime dueDate;
	  protected internal DateTime followUpDate;
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;
	  protected internal string tenantId;

	  protected internal bool isIdentityLinksInitialized = false;
	  [NonSerialized]
	  protected internal IList<IdentityLinkEntity> taskIdentityLinkEntities = new List<IdentityLinkEntity>();

	  // execution
	  protected internal string executionId;
	  [NonSerialized]
	  protected internal ExecutionEntity execution;

	  protected internal string processInstanceId;
	  [NonSerialized]
	  protected internal ExecutionEntity processInstance;

	  protected internal string processDefinitionId;

	  // caseExecution
	  protected internal string caseExecutionId;
	  [NonSerialized]
	  protected internal CaseExecutionEntity caseExecution;

	  protected internal string caseInstanceId;
	  protected internal string caseDefinitionId;

	  // taskDefinition
	  [NonSerialized]
	  protected internal TaskDefinition taskDefinition;
	  protected internal string taskDefinitionKey;

	  protected internal bool isDeleted;
	  protected internal string deleteReason;

	  protected internal string eventName;
	  protected internal bool isFormKeyInitialized = false;
	  protected internal string formKey;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) protected transient org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<VariableInstanceEntity> variableStore = new org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<>(this, new TaskEntityReferencer(this));
	  [NonSerialized]
	  protected internal VariableStore<VariableInstanceEntity> variableStore;


	  [NonSerialized]
	  protected internal bool skipCustomListeners = false;

	  /// <summary>
	  /// contains all changed properties of this entity
	  /// </summary>
	  [NonSerialized]
	  protected internal IDictionary<string, PropertyChange> propertyChanges = new Dictionary<string, PropertyChange>();

	  [NonSerialized]
	  protected internal IList<PropertyChange> identityLinkChanges = new List<PropertyChange>();

	  // name references of tracked properties
	  public const string ASSIGNEE = "assignee";
	  public const string DELEGATION = "delegation";
	  public const string DELETE = "delete";
	  public const string DESCRIPTION = "description";
	  public const string DUE_DATE = "dueDate";
	  public const string FOLLOW_UP_DATE = "followUpDate";
	  public const string NAME = "name";
	  public const string OWNER = "owner";
	  public const string PARENT_TASK = "parentTask";
	  public const string PRIORITY = "priority";
	  public const string CASE_INSTANCE_ID = "caseInstanceId";

	  public TaskEntity() : this(null)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public TaskEntity(string taskId)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		this.id = taskId;
	  }

	  /// <summary>
	  /// creates and initializes a new persistent task. </summary>
	  public static TaskEntity createAndInsert(VariableScope execution)
	  {
		TaskEntity task = create();

		if (execution is ExecutionEntity)
		{
		  ExecutionEntity executionEntity = (ExecutionEntity) execution;
		  task.setExecution(executionEntity);
		  task.skipCustomListeners = executionEntity.SkipCustomListeners;
		  task.insert(executionEntity);
		  return task;

		}
		else if (execution is CaseExecutionEntity)
		{
		  task.setCaseExecution((DelegateCaseExecution) execution);
		}

		task.insert(null);
		return task;
	  }

	  public virtual void insert(ExecutionEntity execution)
	  {
		ensureParentTaskActive();
		propagateExecutionTenantId(execution);
		propagateParentTaskTenantId();

		CommandContext commandContext = Context.CommandContext;
		TaskManager taskManager = commandContext.TaskManager;
		taskManager.insertTask(this);

		if (execution != null)
		{
		  execution.addTask(this);
		}
	  }

	  protected internal virtual void propagateExecutionTenantId(ExecutionEntity execution)
	  {
		if (execution != null)
		{
		  TenantId = execution.TenantId;
		}
	  }

	  protected internal virtual void propagateParentTaskTenantId()
	  {
		if (!string.ReferenceEquals(parentTaskId, null))
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TaskEntity parentTask = org.camunda.bpm.engine.impl.context.Context.getCommandContext().getTaskManager().findTaskById(parentTaskId);
		  TaskEntity parentTask = Context.CommandContext.TaskManager.findTaskById(parentTaskId);

		  if (!string.ReferenceEquals(tenantId, null) && !tenantIdIsSame(parentTask))
		  {
			throw LOG.cannotSetDifferentTenantIdOnSubtask(parentTaskId, parentTask.TenantId, tenantId);
		  }

		  TenantId = parentTask.TenantId;
		}
	  }

	  public virtual void update()
	  {
		ensureTenantIdNotChanged();

		registerCommandContextCloseListener();

		CommandContext commandContext = Context.CommandContext;
		DbEntityManager dbEntityManger = commandContext.DbEntityManager;

		dbEntityManger.merge(this);
	  }

	  protected internal virtual void ensureTenantIdNotChanged()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TaskEntity persistentTask = org.camunda.bpm.engine.impl.context.Context.getCommandContext().getTaskManager().findTaskById(id);
		TaskEntity persistentTask = Context.CommandContext.TaskManager.findTaskById(id);

		if (persistentTask != null)
		{

		  bool changed = !tenantIdIsSame(persistentTask);

		  if (changed)
		  {
			throw LOG.cannotChangeTenantIdOfTask(id, persistentTask.tenantId, tenantId);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected boolean tenantIdIsSame(final TaskEntity otherTask)
	  protected internal virtual bool tenantIdIsSame(TaskEntity otherTask)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String otherTenantId = otherTask.getTenantId();
		string otherTenantId = otherTask.TenantId;

		if (string.ReferenceEquals(otherTenantId, null))
		{
		  return string.ReferenceEquals(tenantId, null);
		}
		else
		{
		  return otherTenantId.Equals(tenantId);
		}
	  }

	  /// <summary>
	  /// new task.  Embedded state and create time will be initialized.
	  /// But this task still will have to be persisted with
	  /// TransactionContext
	  ///     .getCurrent()
	  ///     .getPersistenceSession()
	  ///     .insert(task);
	  /// </summary>
	  public static TaskEntity create()
	  {
		TaskEntity task = new TaskEntity();
		task.isIdentityLinksInitialized = true;
		task.CreateTime = ClockUtil.CurrentTime;
		return task;
	  }

	  public virtual void complete()
	  {

		if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE.Equals(this.eventName) || org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE.Equals(this.eventName))
		{
		  throw LOG.invokeTaskListenerException(new System.InvalidOperationException("invalid task state"));
		}
		// if the task is associated with a case
		// execution then call complete on the
		// associated case execution. The case
		// execution handles the completion of
		// the task.
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  getCaseExecution().manualComplete();
		  return;
		}

		// in the other case:

		// ensure the the Task is not suspended
		ensureTaskActive();

		// trigger TaskListener.complete event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean listenersSuccessful = fireEvent(org.camunda.bpm.engine.delegate.TaskListener_Fields.EVENTNAME_COMPLETE);
		bool listenersSuccessful = fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE);

		if (listenersSuccessful)
		{
		  // delete the task
		  Context.CommandContext.TaskManager.deleteTask(this, TaskEntity.DELETE_REASON_COMPLETED, false, skipCustomListeners);

		  // if the task is associated with a
		  // execution (and not a case execution)
		  // and it's still in the same activity
		  // then call signal an the associated
		  // execution.
		  if (!string.ReferenceEquals(executionId, null))
		  {
			ExecutionEntity execution = getExecution();
			execution.removeTask(this);
			execution.signal(null, null);
		  }
		}
	  }

	  public virtual void caseExecutionCompleted()
	  {
		// ensure the the Task is not suspended
		ensureTaskActive();

		// trigger TaskListener.complete event
		fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE);

		// delete the task
		Context.CommandContext.TaskManager.deleteTask(this, TaskEntity.DELETE_REASON_COMPLETED, false, false);
	  }

	  public virtual void delete(string deleteReason, bool cascade)
	  {
		this.deleteReason = deleteReason;
		fireEvent(EVENTNAME_DELETE);

		Context.CommandContext.TaskManager.deleteTask(this, deleteReason, cascade, skipCustomListeners);

		if (!string.ReferenceEquals(executionId, null))
		{
		  ExecutionEntity execution = getExecution();
		  execution.removeTask(this);
		}
	  }

	  public virtual void delete(string deleteReason, bool cascade, bool skipCustomListeners)
	  {
		this.skipCustomListeners = skipCustomListeners;
		delete(deleteReason, cascade);
	  }

	  public virtual void @delegate(string userId)
	  {
		DelegationState = DelegationState.PENDING;
		if (string.ReferenceEquals(Owner, null))
		{
		  Owner = Assignee;
		}
		Assignee = userId;
	  }

	  public virtual void resolve()
	  {
		DelegationState = DelegationState.RESOLVED;
		Assignee = this.owner;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["assignee"] = this.assignee;
			persistentState["owner"] = this.owner;
			persistentState["name"] = this.name;
			persistentState["priority"] = this.priority;
			if (!string.ReferenceEquals(executionId, null))
			{
			  persistentState["executionId"] = this.executionId;
			}
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  persistentState["processDefinitionId"] = this.processDefinitionId;
			}
			if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  persistentState["caseExecutionId"] = this.caseExecutionId;
			}
			if (!string.ReferenceEquals(caseInstanceId, null))
			{
			  persistentState["caseInstanceId"] = this.caseInstanceId;
			}
			if (!string.ReferenceEquals(caseDefinitionId, null))
			{
			  persistentState["caseDefinitionId"] = this.caseDefinitionId;
			}
			if (createTime != null)
			{
			  persistentState["createTime"] = this.createTime;
			}
			if (!string.ReferenceEquals(description, null))
			{
			  persistentState["description"] = this.description;
			}
			if (dueDate != null)
			{
			  persistentState["dueDate"] = this.dueDate;
			}
			if (followUpDate != null)
			{
			  persistentState["followUpDate"] = this.followUpDate;
			}
			if (!string.ReferenceEquals(parentTaskId, null))
			{
			  persistentState["parentTaskId"] = this.parentTaskId;
			}
			if (delegationState != null)
			{
			  persistentState["delegationState"] = this.delegationState;
			}
			if (!string.ReferenceEquals(tenantId, null))
			{
			  persistentState["tenantId"] = this.tenantId;
			}
    
			persistentState["suspensionState"] = this.suspensionState;
    
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

	  protected internal virtual void ensureParentTaskActive()
	  {
		if (!string.ReferenceEquals(parentTaskId, null))
		{
		  TaskEntity parentTask = Context.CommandContext.TaskManager.findTaskById(parentTaskId);

		  ensureNotNull(typeof(NullValueException), "Parent task with id '" + parentTaskId + "' does not exist", "parentTask", parentTask);

		  if (parentTask.suspensionState == SuspensionState_Fields.SUSPENDED.StateCode)
		  {
			throw LOG.suspendedEntityException("parent task", id);
		  }
		}
	  }

	  protected internal virtual void ensureTaskActive()
	  {
		if (suspensionState == SuspensionState_Fields.SUSPENDED.StateCode)
		{
		  throw LOG.suspendedEntityException("task", id);
		}
	  }

	  public virtual UserTask BpmnModelElementInstance
	  {
		  get
		  {
			BpmnModelInstance bpmnModelInstance = BpmnModelInstance;
			if (bpmnModelInstance != null)
			{
			  ModelElementInstance modelElementInstance = bpmnModelInstance.getModelElementById(taskDefinitionKey);
			  try
			  {
				return (UserTask) modelElementInstance;
			  }
			  catch (System.InvalidCastException e)
			  {
				ModelElementType elementType = modelElementInstance.ElementType;
				throw LOG.castModelInstanceException(modelElementInstance, "UserTask", elementType.TypeName, elementType.TypeNamespace, e);
			  }
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual BpmnModelInstance BpmnModelInstance
	  {
		  get
		  {
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return Context.ProcessEngineConfiguration.DeploymentCache.findBpmnModelInstanceForProcessDefinition(processDefinitionId);
    
			}
			else
			{
			  return null;
    
			}
		  }
	  }

	  // variables ////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) protected org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableStore()
	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return (VariableStore) variableStore;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) protected org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableInstanceFactory()
	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) VariableInstanceEntityFactory.INSTANCE;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) protected java.util.List<org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance>> getVariableInstanceLifecycleListeners()
	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
			return Arrays.asList<VariableInstanceLifecycleListener<CoreVariableInstance>>((VariableInstanceLifecycleListener) VariableInstanceEntityPersistenceListener.INSTANCE, (VariableInstanceLifecycleListener) VariableInstanceSequenceCounterListener.INSTANCE, (VariableInstanceLifecycleListener) VariableInstanceHistoryListener.INSTANCE);
		  }
	  }

	  public override void dispatchEvent(VariableEvent variableEvent)
	  {
		if (execution != null && string.ReferenceEquals(variableEvent.VariableInstance.TaskId, null))
		{
		  execution.handleConditionalEventOnVariableChange(variableEvent);
		}
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables()
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByTaskId(id);
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables(ICollection<string> variableNames)
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByTaskIdAndVariableNames(id, variableNames);
	  }

	  public override AbstractVariableScope ParentVariableScope
	  {
		  get
		  {
			if (getExecution() != null)
			{
			  return execution;
			}
			if (getCaseExecution() != null)
			{
			  return caseExecution;
			}
			if (ParentTask != null)
			{
			  return parentTask;
			}
			return null;
		  }
	  }

	  public override string VariableScopeKey
	  {
		  get
		  {
			return "task";
		  }
	  }

	  // execution ////////////////////////////////////////////////////////////////

	  public virtual TaskEntity ParentTask
	  {
		  get
		  {
			if (parentTask == null && !string.ReferenceEquals(parentTaskId, null))
			{
			  this.parentTask = Context.CommandContext.TaskManager.findTaskById(parentTaskId);
			}
			return parentTask;
		  }
	  }

	  public virtual ExecutionEntity getExecution()
	  {
		if ((execution == null) && (!string.ReferenceEquals(executionId, null)))
		{
		  this.execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
		}
		return execution;
	  }

	  public virtual void setExecution(PvmExecutionImpl execution)
	  {
		if (execution != null)
		{

		  this.execution = (ExecutionEntity) execution;
		  this.executionId = this.execution.Id;
		  this.processInstanceId = this.execution.ProcessInstanceId;
		  this.processDefinitionId = this.execution.ProcessDefinitionId;

		  // get the process instance
		  ExecutionEntity instance = this.execution.getProcessInstance();
		  if (instance != null)
		  {
			// set case instance id on this task
			this.caseInstanceId = instance.CaseInstanceId;
		  }

		}
		else
		{
		  this.execution = null;
		  this.executionId = null;
		  this.processInstanceId = null;
		  this.processDefinitionId = null;
		  this.caseInstanceId = null;
		}
	  }

	  // case execution ////////////////////////////////////////////////////////////////

	  public virtual CaseExecutionEntity getCaseExecution()
	  {
		ensureCaseExecutionInitialized();
		return caseExecution;
	  }

	  protected internal virtual void ensureCaseExecutionInitialized()
	  {
		if ((caseExecution == null) && (!string.ReferenceEquals(caseExecutionId, null)))
		{
		  caseExecution = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);
		}
	  }

	  public virtual void setCaseExecution(DelegateCaseExecution caseExecution)
	  {
		if (caseExecution != null)
		{

		  this.caseExecution = (CaseExecutionEntity) caseExecution;
		  this.caseExecutionId = this.caseExecution.Id;
		  this.caseInstanceId = this.caseExecution.CaseInstanceId;
		  this.caseDefinitionId = this.caseExecution.CaseDefinitionId;
		  this.tenantId = this.caseExecution.TenantId;

		}
		else
		{
		  this.caseExecution = null;
		  this.caseExecutionId = null;
		  this.caseInstanceId = null;
		  this.caseDefinitionId = null;
		  this.tenantId = null;

		}
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(CASE_INSTANCE_ID, this.caseInstanceId, value);
			this.caseInstanceId = value;
		  }
	  }


	  /* plain setter for persistence */
	  public virtual string CaseInstanceIdWithoutCascade
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }

	  public virtual CaseDefinitionEntity CaseDefinition
	  {
		  get
		  {
			if (!string.ReferenceEquals(caseDefinitionId, null))
			{
			  return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);
			}
			return null;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  // task assignment //////////////////////////////////////////////////////////

	  public virtual IdentityLinkEntity addIdentityLink(string userId, string groupId, string type)
	  {
		ensureTaskActive();

		IdentityLinkEntity identityLink = newIdentityLink(userId, groupId, type);
		identityLink.insert();
		IdentityLinks.Add(identityLink);

		fireAddIdentityLinkAuthorizationProvider(type, userId, groupId);
		return identityLink;
	  }

	  public virtual void fireIdentityLinkHistoryEvents(string userId, string groupId, string type, HistoryEventTypes historyEventType)
	  {
		IdentityLinkEntity identityLinkEntity = newIdentityLink(userId, groupId, type);
		identityLinkEntity.fireHistoricIdentityLinkEvent(historyEventType);
	  }

	  public virtual IdentityLinkEntity newIdentityLink(string userId, string groupId, string type)
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		identityLinkEntity.Task = this;
		identityLinkEntity.UserId = userId;
		identityLinkEntity.GroupId = groupId;
		identityLinkEntity.Type = type;
		identityLinkEntity.TenantId = TenantId;
		return identityLinkEntity;
	  }

	  public virtual void deleteIdentityLink(string userId, string groupId, string type)
	  {
		ensureTaskActive();

		IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkManager.findIdentityLinkByTaskUserGroupAndType(id, userId, groupId, type);

		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  fireDeleteIdentityLinkAuthorizationProvider(type, userId, groupId);
		  identityLink.delete();
		}
	  }

	  public virtual void deleteIdentityLinks(bool withHistory)
	  {
		IList<IdentityLinkEntity> identityLinkEntities = IdentityLinks;
		foreach (IdentityLinkEntity identityLinkEntity in identityLinkEntities)
		{
		  fireDeleteIdentityLinkAuthorizationProvider(identityLinkEntity.Type, identityLinkEntity.UserId, identityLinkEntity.GroupId);
		  identityLinkEntity.delete(withHistory);
		}
		isIdentityLinksInitialized = false;
	  }

	  public virtual ISet<IdentityLink> Candidates
	  {
		  get
		  {
			ISet<IdentityLink> potentialOwners = new HashSet<IdentityLink>();
			foreach (IdentityLinkEntity identityLinkEntity in IdentityLinks)
			{
			  if (IdentityLinkType.CANDIDATE.Equals(identityLinkEntity.Type))
			  {
				potentialOwners.Add(identityLinkEntity);
			  }
			}
			return potentialOwners;
		  }
	  }

	  public virtual void addCandidateUser(string userId)
	  {
		addIdentityLink(userId, null, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void addCandidateUsers(ICollection<string> candidateUsers)
	  {
		foreach (string candidateUser in candidateUsers)
		{
		  addCandidateUser(candidateUser);
		}
	  }

	  public virtual void addCandidateGroup(string groupId)
	  {
		addIdentityLink(null, groupId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void addCandidateGroups(ICollection<string> candidateGroups)
	  {
		foreach (string candidateGroup in candidateGroups)
		{
		  addCandidateGroup(candidateGroup);
		}
	  }

	  public virtual void addGroupIdentityLink(string groupId, string identityLinkType)
	  {
		addIdentityLink(null, groupId, identityLinkType);
	  }

	  public virtual void addUserIdentityLink(string userId, string identityLinkType)
	  {
		addIdentityLink(userId, null, identityLinkType);
	  }

	  public virtual void deleteCandidateGroup(string groupId)
	  {
		deleteGroupIdentityLink(groupId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void deleteCandidateUser(string userId)
	  {
		deleteUserIdentityLink(userId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void deleteGroupIdentityLink(string groupId, string identityLinkType)
	  {
		if (!string.ReferenceEquals(groupId, null))
		{
		  deleteIdentityLink(null, groupId, identityLinkType);
		}
	  }

	  public virtual void deleteUserIdentityLink(string userId, string identityLinkType)
	  {
		if (!string.ReferenceEquals(userId, null))
		{
		  deleteIdentityLink(userId, null, identityLinkType);
		}
	  }

	  public virtual IList<IdentityLinkEntity> IdentityLinks
	  {
		  get
		  {
			if (!isIdentityLinksInitialized)
			{
			  taskIdentityLinkEntities = Context.CommandContext.IdentityLinkManager.findIdentityLinksByTaskId(id);
			  isIdentityLinksInitialized = true;
			}
    
			return taskIdentityLinkEntities;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, Object> getActivityInstanceVariables()
	  public virtual IDictionary<string, object> ActivityInstanceVariables
	  {
		  get
		  {
			if (execution != null)
			{
			  return execution.Variables;
			}
			return Collections.EMPTY_MAP;
		  }
	  }

	  public virtual IDictionary<string, object> ExecutionVariables
	  {
		  set
		  {
			AbstractVariableScope scope = ParentVariableScope;
			if (scope != null)
			{
			  scope.setVariables(value);
			}
		  }
	  }

	  public override string ToString()
	  {
		return "Task[" + id + "]";
	  }

	  // special setters //////////////////////////////////////////////////////////

	  public virtual string Name
	  {
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(NAME, this.name, value);
			this.name = value;
		  }
		  get
		  {
			return name;
		  }
	  }

	  /* plain setter for persistence */
	  public virtual string NameWithoutCascade
	  {
		  set
		  {
			this.name = value;
		  }
	  }

	  public virtual string Description
	  {
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(DESCRIPTION, this.description, value);
			this.description = value;
		  }
		  get
		  {
			return description;
		  }
	  }

	  /* plain setter for persistence */
	  public virtual string DescriptionWithoutCascade
	  {
		  set
		  {
			this.description = value;
		  }
	  }

	  public virtual string Assignee
	  {
		  set
		  {
			ensureTaskActive();
			registerCommandContextCloseListener();
    
			string oldAssignee = this.assignee;
			if (string.ReferenceEquals(value, null) && string.ReferenceEquals(oldAssignee, null))
			{
			  return;
			}
    
			addIdentityLinkChanges(IdentityLinkType.ASSIGNEE, oldAssignee, value);
			propertyChanged(ASSIGNEE, oldAssignee, value);
			this.assignee = value;
    
			CommandContext commandContext = Context.CommandContext;
			// if there is no command context, then it means that the user is calling the
			// setAssignee outside a service method.  E.g. while creating a new task.
			if (commandContext != null)
			{
			  fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT);
			  if (commandContext.DbEntityManager.contains(this))
			  {
				fireAssigneeAuthorizationProvider(oldAssignee, value);
				fireHistoricIdentityLinks();
			  }
			}
		  }
		  get
		  {
			return assignee;
		  }
	  }

	  /* plain setter for persistence */
	  public virtual string AssigneeWithoutCascade
	  {
		  set
		  {
			this.assignee = value;
		  }
	  }

	  public virtual string Owner
	  {
		  set
		  {
			ensureTaskActive();
			registerCommandContextCloseListener();
    
			string oldOwner = this.owner;
			if (string.ReferenceEquals(value, null) && string.ReferenceEquals(oldOwner, null))
			{
			  return;
			}
    
			addIdentityLinkChanges(IdentityLinkType.OWNER, oldOwner, value);
			propertyChanged(OWNER, oldOwner, value);
			this.owner = value;
    
			CommandContext commandContext = Context.CommandContext;
			// if there is no command context, then it means that the user is calling the
			// setOwner outside a service method.  E.g. while creating a new task.
			if (commandContext != null && commandContext.DbEntityManager.contains(this))
			{
			  fireOwnerAuthorizationProvider(oldOwner, value);
			  this.fireHistoricIdentityLinks();
			}
    
		  }
		  get
		  {
			return owner;
		  }
	  }

	  /* plain setter for persistence */
	  public virtual string OwnerWithoutCascade
	  {
		  set
		  {
			this.owner = value;
		  }
	  }

	  public virtual DateTime DueDate
	  {
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(DUE_DATE, this.dueDate, value);
			this.dueDate = value;
		  }
		  get
		  {
			return dueDate;
		  }
	  }

	  public virtual DateTime DueDateWithoutCascade
	  {
		  set
		  {
			this.dueDate = value;
		  }
	  }

	  public virtual int Priority
	  {
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(PRIORITY, this.priority, value);
			this.priority = value;
		  }
		  get
		  {
			return priority;
		  }
	  }

	  public virtual int PriorityWithoutCascade
	  {
		  set
		  {
			this.priority = value;
		  }
	  }

	  public virtual string ParentTaskId
	  {
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(PARENT_TASK, this.parentTaskId, value);
			this.parentTaskId = value;
		  }
		  get
		  {
			return parentTaskId;
		  }
	  }

	  public virtual string ParentTaskIdWithoutCascade
	  {
		  set
		  {
			this.parentTaskId = value;
		  }
	  }

	  public virtual string TaskDefinitionKeyWithoutCascade
	  {
		  set
		  {
			this.taskDefinitionKey = value;
		  }
	  }

	  /// <returns> true if invoking the listener was successful;
	  ///   if not successful, either false is returned (case: BPMN error propagation)
	  ///   or an exception is thrown </returns>
	  public virtual bool fireEvent(string taskEventName)
	  {

		IList<TaskListener> taskEventListeners = getListenersForEvent(taskEventName);

		if (taskEventListeners != null)
		{
		  foreach (TaskListener taskListener in taskEventListeners)
		  {
			CoreExecution execution = getExecution();
			if (execution == null)
			{
			  execution = getCaseExecution();
			}

			if (execution != null)
			{
			  EventName = taskEventName;
			}
			try
			{
			  bool success = invokeListener(execution, taskEventName, taskListener);
			  if (!success)
			  {
				return false;
			  }
			}
			catch (Exception e)
			{
			  throw LOG.invokeTaskListenerException(e);
			}
		  }
		}

		return true;
	  }

	  /// <returns> true if the next listener can be invoked; false if not </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean invokeListener(org.camunda.bpm.engine.impl.core.instance.CoreExecution currentExecution, String eventName, org.camunda.bpm.engine.delegate.TaskListener taskListener) throws Exception
	  protected internal virtual bool invokeListener(CoreExecution currentExecution, string eventName, TaskListener taskListener)
	  {
		bool isBpmnTask = currentExecution is ActivityExecution && currentExecution != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.task.delegate.TaskListenerInvocation listenerInvocation = new org.camunda.bpm.engine.impl.task.delegate.TaskListenerInvocation(taskListener, this, currentExecution);
		TaskListenerInvocation listenerInvocation = new TaskListenerInvocation(taskListener, this, currentExecution);

		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(listenerInvocation);
		}
		catch (Exception ex)
		{
		  // exceptions on delete events are never handled as BPMN errors
		  if (isBpmnTask && !eventName.Equals(EVENTNAME_DELETE))
		  {
			try
			{
			  BpmnExceptionHandler.propagateException((ActivityExecution) currentExecution, ex);
			  return false;
			}
			catch (ErrorPropagationException)
			{
			  // exception has been logged by thrower
			  // re-throw the original exception so that it is logged
			  // and set as cause of the failure
			  throw ex;
			}
		  }
		  else
		  {
			throw ex;
		  }
		}
		return true;
	  }

	  protected internal virtual IList<TaskListener> getListenersForEvent(string @event)
	  {
		TaskDefinition resolvedTaskDefinition = TaskDefinition;
		if (resolvedTaskDefinition != null)
		{
		  if (skipCustomListeners)
		  {
			return resolvedTaskDefinition.getBuiltinTaskListeners(@event);
		  }
		  else
		  {
			return resolvedTaskDefinition.getTaskListeners(@event);
		  }

		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Tracks a property change. Therefore the original and new value are stored in a map.
	  /// It tracks multiple changes and if a property finally is changed back to the original
	  /// value, then the change is removed.
	  /// </summary>
	  /// <param name="propertyName"> </param>
	  /// <param name="orgValue"> </param>
	  /// <param name="newValue"> </param>
	  protected internal virtual void propertyChanged(string propertyName, object orgValue, object newValue)
	  {
		if (propertyChanges.ContainsKey(propertyName))
		{ // update an existing change to save the original value
		  object oldOrgValue = propertyChanges[propertyName].OrgValue;
		  if ((oldOrgValue == null && newValue == null) || (oldOrgValue != null && oldOrgValue.Equals(newValue)))
		  { // remove this change
			propertyChanges.Remove(propertyName);
		  }
		  else
		  {
			propertyChanges[propertyName].NewValue = newValue;
		  }
		}
		else
		{ // save this change
		  if ((orgValue == null && newValue != null) || (orgValue != null && newValue == null) || (orgValue != null && !orgValue.Equals(newValue))) // value change
		  {
			propertyChanges[propertyName] = new PropertyChange(propertyName, orgValue, newValue);
		  }
		}
	  }

	  // authorizations ///////////////////////////////////////////////////////////

	  public virtual void fireAuthorizationProvider()
	  {
		PropertyChange assigneePropertyChange = propertyChanges[ASSIGNEE];
		if (assigneePropertyChange != null)
		{
		  string oldAssignee = assigneePropertyChange.OrgValueString;
		  string newAssignee = assigneePropertyChange.NewValueString;
		  fireAssigneeAuthorizationProvider(oldAssignee, newAssignee);
		}

		PropertyChange ownerPropertyChange = propertyChanges[OWNER];
		if (ownerPropertyChange != null)
		{
		  string oldOwner = ownerPropertyChange.OrgValueString;
		  string newOwner = ownerPropertyChange.NewValueString;
		  fireOwnerAuthorizationProvider(oldOwner, newOwner);
		}
	  }

	  public virtual void fireEvents()
	  {
		PropertyChange assigneePropertyChange = propertyChanges[ASSIGNEE];
		if (assigneePropertyChange != null)
		{
		  fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT);
		}
	  }

	  protected internal virtual void fireAssigneeAuthorizationProvider(string oldAssignee, string newAssignee)
	  {
		fireAuthorizationProvider(ASSIGNEE, oldAssignee, newAssignee);
	  }

	  protected internal virtual void fireOwnerAuthorizationProvider(string oldOwner, string newOwner)
	  {
		fireAuthorizationProvider(OWNER, oldOwner, newOwner);
	  }

	  protected internal virtual void fireAuthorizationProvider(string property, string oldValue, string newValue)
	  {
		if (AuthorizationEnabled && string.ReferenceEquals(caseExecutionId, null))
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

		  AuthorizationEntity[] authorizations = null;
		  if (ASSIGNEE.Equals(property))
		  {
			authorizations = provider.newTaskAssignee(this, oldValue, newValue);
		  }
		  else if (OWNER.Equals(property))
		  {
			authorizations = provider.newTaskOwner(this, oldValue, newValue);
		  }

		  saveAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void fireAddIdentityLinkAuthorizationProvider(string type, string userId, string groupId)
	  {
		if (AuthorizationEnabled && string.ReferenceEquals(caseExecutionId, null))
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

		  AuthorizationEntity[] authorizations = null;
		  if (!string.ReferenceEquals(userId, null))
		  {
			authorizations = provider.newTaskUserIdentityLink(this, userId, type);
		  }
		  else if (!string.ReferenceEquals(groupId, null))
		  {
			authorizations = provider.newTaskGroupIdentityLink(this, groupId, type);
		  }

		  saveAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void fireDeleteIdentityLinkAuthorizationProvider(string type, string userId, string groupId)
	  {
		if (AuthorizationEnabled && string.ReferenceEquals(caseExecutionId, null))
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

		  AuthorizationEntity[] authorizations = null;
		  if (!string.ReferenceEquals(userId, null))
		  {
			authorizations = provider.deleteTaskUserIdentityLink(this, userId, type);
		  }
		  else if (!string.ReferenceEquals(groupId, null))
		  {
			authorizations = provider.deleteTaskGroupIdentityLink(this, groupId, type);
		  }

		  deleteAuthorizations(authorizations);
		}
	  }

	  protected internal virtual ResourceAuthorizationProvider ResourceAuthorizationProvider
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			return processEngineConfiguration.ResourceAuthorizationProvider;
		  }
	  }

	  protected internal virtual void saveAuthorizations(AuthorizationEntity[] authorizations)
	  {
		CommandContext commandContext = Context.CommandContext;
		TaskManager taskManager = commandContext.TaskManager;
		taskManager.saveDefaultAuthorizations(authorizations);
	  }

	  protected internal virtual void deleteAuthorizations(AuthorizationEntity[] authorizations)
	  {
		CommandContext commandContext = Context.CommandContext;
		TaskManager taskManager = commandContext.TaskManager;
		taskManager.deleteDefaultAuthorizations(authorizations);
	  }

	  protected internal virtual bool AuthorizationEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.AuthorizationEnabled;
		  }
	  }

	  // modified getters and setters /////////////////////////////////////////////

	  public virtual TaskDefinition TaskDefinition
	  {
		  set
		  {
			this.taskDefinition = value;
			this.taskDefinitionKey = value.Key;
		  }
		  get
		  {
			if (taskDefinition == null && !string.ReferenceEquals(taskDefinitionKey, null))
			{
    
			  IDictionary<string, TaskDefinition> taskDefinitions = null;
			  if (!string.ReferenceEquals(processDefinitionId, null))
			  {
				ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
    
				taskDefinitions = processDefinition.TaskDefinitions;
    
			  }
			  else
			  {
				CaseDefinitionEntity caseDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);
    
				taskDefinitions = caseDefinition.TaskDefinitions;
			  }
    
			  taskDefinition = taskDefinitions[taskDefinitionKey];
			}
			return taskDefinition;
		  }
	  }


	  // getters and setters //////////////////////////////////////////////////////

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

	  public virtual ProcessDefinitionEntity ProcessDefinition
	  {
		  get
		  {
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
			}
			return null;
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

	  public virtual void initializeFormKey()
	  {
		isFormKeyInitialized = true;
		if (!string.ReferenceEquals(taskDefinitionKey, null))
		{
		  TaskDefinition taskDefinition = TaskDefinition;
		  if (taskDefinition != null)
		  {
			Expression formKey = taskDefinition.FormKey;
			if (formKey != null)
			{
			  this.formKey = (string) formKey.getValue(this);
			}
		  }
		}
	  }

	  public virtual string FormKey
	  {
		  get
		  {
			if (!isFormKeyInitialized)
			{
			  throw LOG.uninitializedFormKeyException();
			}
			return formKey;
		  }
	  }



	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
		  set
		  {
			if ((string.ReferenceEquals(value, null) && !string.ReferenceEquals(this.taskDefinitionKey, null)) || (!string.ReferenceEquals(value, null) && !value.Equals(this.taskDefinitionKey)))
			{
			  this.taskDefinition = null;
			  this.formKey = null;
			  this.isFormKeyInitialized = false;
			}
    
			this.taskDefinitionKey = value;
		  }
	  }


	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }



	  public virtual ExecutionEntity ProcessInstance
	  {
		  get
		  {
			if (this.processInstance == null && !string.ReferenceEquals(this.processInstanceId, null))
			{
			  this.processInstance = Context.CommandContext.ExecutionManager.findExecutionById(this.processInstanceId);
			}
			return this.processInstance;
		  }
		  set
		  {
			this.processInstance = value;
		  }
	  }




	  public virtual DelegationState DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
		  set
		  {
			propertyChanged(DELEGATION, this.delegationState, value);
			this.delegationState = value;
		  }
	  }


	  public virtual DelegationState DelegationStateWithoutCascade
	  {
		  set
		  {
			this.delegationState = value;
		  }
	  }

	  public virtual string DelegationStateString
	  {
		  get
		  {
			return (delegationState != null ? delegationState.ToString() : null);
		  }
		  set
		  {
			if (string.ReferenceEquals(value, null))
			{
			  DelegationStateWithoutCascade = null;
			}
			else
			{
			  DelegationStateWithoutCascade = Enum.Parse(typeof(DelegationState), value);
			}
		  }
	  }


	  public virtual bool Deleted
	  {
		  get
		  {
			return isDeleted;
		  }
		  set
		  {
			propertyChanged(DELETE, this.isDeleted, value);
			this.isDeleted = value;
		  }
	  }

	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
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

	  public virtual DateTime FollowUpDate
	  {
		  get
		  {
			return followUpDate;
		  }
		  set
		  {
			registerCommandContextCloseListener();
			propertyChanged(FOLLOW_UP_DATE, this.followUpDate, value);
			this.followUpDate = value;
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



	  public virtual DateTime FollowUpDateWithoutCascade
	  {
		  set
		  {
			this.followUpDate = value;
		  }
	  }

	  public virtual ICollection<VariableInstanceEntity> VariablesInternal
	  {
		  get
		  {
			return variableStore.Variables;
		  }
	  }

	  public virtual void onCommandContextClose(CommandContext commandContext)
	  {
		if (commandContext.DbEntityManager.isDirty(this))
		{
		  commandContext.HistoricTaskInstanceManager.updateHistoricTaskInstance(this);
		}
	  }

	  public virtual void onCommandFailed(CommandContext commandContext, Exception t)
	  {
		// ignore
	  }

	  protected internal virtual void registerCommandContextCloseListener()
	  {
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  commandContext.registerCommandContextListener(this);
		}
	  }

	  public virtual IDictionary<string, PropertyChange> PropertyChanges
	  {
		  get
		  {
			return propertyChanges;
		  }
	  }

	  public virtual void createHistoricTaskDetails(string operation)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  IList<PropertyChange> values = new List<PropertyChange>(propertyChanges.Values);
		  commandContext.OperationLogManager.logTaskOperations(operation, this, values);
		  fireHistoricIdentityLinks();
		  propertyChanges.Clear();
		}
	  }

	  public virtual void fireHistoricIdentityLinks()
	  {
		foreach (PropertyChange propertyChange in identityLinkChanges)
		{
		  string oldValue = propertyChange.OrgValueString;
		  string propertyName = propertyChange.PropertyName;
		  if (!string.ReferenceEquals(oldValue, null))
		  {
			fireIdentityLinkHistoryEvents(oldValue, null, propertyName, HistoryEventTypes.IDENTITY_LINK_DELETE);
		  }
		  string newValue = propertyChange.NewValueString;
		  if (!string.ReferenceEquals(newValue, null))
		  {
			fireIdentityLinkHistoryEvents(newValue, null, propertyName, HistoryEventTypes.IDENTITY_LINK_ADD);
		  }
		}
		identityLinkChanges.Clear();
	  }

	  public virtual ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

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
		TaskEntity other = (TaskEntity) obj;
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

	  public virtual void executeMetrics(string metricsName)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration.MetricsEnabled)
		{
		  processEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.ACTIVTY_INSTANCE_START);
		}
	  }

	  public virtual void addIdentityLinkChanges(string type, string oldProperty, string newProperty)
	  {
		identityLinkChanges.Add(new PropertyChange(type, oldProperty, newProperty));
	  }

	  public override IDictionary<T1> VariablesLocal<T1>
	  {
		  set
		  {
			base.setVariablesLocal(value);
			Context.CommandContext.DbEntityManager.forceUpdate(this);
		  }
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
    
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  referenceIdAndClass[processDefinitionId] = typeof(ProcessDefinitionEntity);
			}
			if (!string.ReferenceEquals(processInstanceId, null))
			{
			  referenceIdAndClass[processInstanceId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(executionId, null))
			{
			  referenceIdAndClass[executionId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(caseDefinitionId, null))
			{
			  referenceIdAndClass[caseDefinitionId] = typeof(CaseDefinitionEntity);
			}
			if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  referenceIdAndClass[caseExecutionId] = typeof(CaseExecutionEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }
	}

}