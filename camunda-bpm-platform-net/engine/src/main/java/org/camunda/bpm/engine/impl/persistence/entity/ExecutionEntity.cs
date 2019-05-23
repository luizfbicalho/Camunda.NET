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

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NoneStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.NoneStartEventActivityBehavior;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using org.camunda.bpm.engine.impl.core.variable.scope;
	using VariablesProvider = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore.VariablesProvider;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using AtomicOperationInvocation = org.camunda.bpm.engine.impl.interceptor.AtomicOperationInvocation;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using FormPropertyStartContext = org.camunda.bpm.engine.impl.persistence.entity.util.FormPropertyStartContext;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ActivityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState;
	using AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation;
	using ExecutionStartContext = org.camunda.bpm.engine.impl.pvm.runtime.ExecutionStartContext;
	using ProcessInstanceStartContext = org.camunda.bpm.engine.impl.pvm.runtime.ProcessInstanceStartContext;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;
	using ExecutionTopDownWalker = org.camunda.bpm.engine.impl.tree.ExecutionTopDownWalker;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;
	using BitMaskUtil = org.camunda.bpm.engine.impl.util.BitMaskUtil;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using VariableDeclaration = org.camunda.bpm.engine.impl.variable.VariableDeclaration;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class ExecutionEntity : PvmExecutionImpl, Execution, ProcessInstance, DbEntity, HasDbRevision, HasDbReferences, VariablesProvider<VariableInstanceEntity>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			variableStore = new VariableStore<VariableInstanceEntity>(this, new ExecutionEntityReferencer(this));
		}


	  private const long serialVersionUID = 1L;

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  // Persistent refrenced entities state //////////////////////////////////////
	  public const int EVENT_SUBSCRIPTIONS_STATE_BIT = 1;
	  public const int TASKS_STATE_BIT = 2;
	  public const int JOBS_STATE_BIT = 3;
	  public const int INCIDENT_STATE_BIT = 4;
	  public const int VARIABLES_STATE_BIT = 5;
	  public const int SUB_PROCESS_INSTANCE_STATE_BIT = 6;
	  public const int SUB_CASE_INSTANCE_STATE_BIT = 7;
	  public const int EXTERNAL_TASKS_BIT = 8;

	  // current position /////////////////////////////////////////////////////////

	  /// <summary>
	  /// the process instance. this is the root of the execution tree. the
	  /// processInstance of a process instance is a self reference.
	  /// </summary>
	  [NonSerialized]
	  protected internal ExecutionEntity processInstance;

	  /// <summary>
	  /// the parent execution </summary>
	  [NonSerialized]
	  protected internal ExecutionEntity parent;

	  /// <summary>
	  /// nested executions representing scopes or concurrent paths </summary>
	  [NonSerialized]
	  protected internal IList<ExecutionEntity> executions;

	  /// <summary>
	  /// super execution, not-null if this execution is part of a subprocess </summary>
	  [NonSerialized]
	  protected internal ExecutionEntity superExecution;

	  /// <summary>
	  /// super case execution, not-null if this execution is part of a case
	  /// execution
	  /// </summary>
	  [NonSerialized]
	  protected internal CaseExecutionEntity superCaseExecution;

	  /// <summary>
	  /// reference to a subprocessinstance, not-null if currently subprocess is
	  /// started from this execution
	  /// </summary>
	  [NonSerialized]
	  protected internal ExecutionEntity subProcessInstance;

	  /// <summary>
	  /// reference to a subcaseinstance, not-null if currently subcase is started
	  /// from this execution
	  /// </summary>
	  [NonSerialized]
	  protected internal CaseExecutionEntity subCaseInstance;

	  protected internal bool shouldQueryForSubprocessInstance = false;

	  protected internal bool shouldQueryForSubCaseInstance = false;

	  // associated entities /////////////////////////////////////////////////////

	  // (we cache associated entities here to minimize db queries)
	  [NonSerialized]
	  protected internal IList<EventSubscriptionEntity> eventSubscriptions;
	  [NonSerialized]
	  protected internal IList<JobEntity> jobs;
	  [NonSerialized]
	  protected internal IList<TaskEntity> tasks;
	  [NonSerialized]
	  protected internal IList<ExternalTaskEntity> externalTasks;
	  [NonSerialized]
	  protected internal IList<IncidentEntity> incidents;
	  protected internal int cachedEntityState;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected transient VariableStore<VariableInstanceEntity> variableStore = new VariableStore<>(this, new ExecutionEntityReferencer(this));
	  [NonSerialized]
	  protected internal VariableStore<VariableInstanceEntity> variableStore;


	  // replaced by //////////////////////////////////////////////////////////////

	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;

	  // Persistence //////////////////////////////////////////////////////////////

	  protected internal int revision = 1;

	  /// <summary>
	  /// persisted reference to the processDefinition.
	  /// </summary>
	  /// <seealso cref= #processDefinition </seealso>
	  /// <seealso cref= #setProcessDefinition(ProcessDefinitionImpl) </seealso>
	  /// <seealso cref= #getProcessDefinition() </seealso>
	  protected internal string processDefinitionId;

	  /// <summary>
	  /// persisted reference to the current position in the diagram within the
	  /// <seealso cref="processDefinition"/>.
	  /// </summary>
	  /// <seealso cref= #activity </seealso>
	  /// <seealso cref= #getActivity() </seealso>
	  protected internal string activityId;

	  /// <summary>
	  /// The name of the current activity position
	  /// </summary>
	  protected internal string activityName;

	  /// <summary>
	  /// persisted reference to the process instance.
	  /// </summary>
	  /// <seealso cref= #getProcessInstance() </seealso>
	  protected internal string processInstanceId;

	  /// <summary>
	  /// persisted reference to the parent of this execution.
	  /// </summary>
	  /// <seealso cref= #getParent() </seealso>
	  protected internal string parentId;

	  /// <summary>
	  /// persisted reference to the super execution of this execution
	  /// 
	  /// @See <seealso cref="getSuperExecution()"/> </summary>
	  /// <seealso cref= <code>setSuperExecution(ExecutionEntity)</code> </seealso>
	  protected internal string superExecutionId;

	  /// <summary>
	  /// persisted reference to the root process instance.
	  /// </summary>
	  /// <seealso cref= #getRootProcessInstanceId() </seealso>
	  protected internal string rootProcessInstanceId;

	  /// <summary>
	  /// persisted reference to the super case execution of this execution
	  /// 
	  /// @See <seealso cref="getSuperCaseExecution()"/> </summary>
	  /// <seealso cref= <code>setSuperCaseExecution(ExecutionEntity)</code> </seealso>
	  protected internal string superCaseExecutionId;

	  /// <summary>
	  /// Contains observers which are observe the execution.
	  /// @since 7.6
	  /// </summary>
	  [NonSerialized]
	  protected internal IList<ExecutionObserver> executionObservers = new List<ExecutionObserver>();

	  [NonSerialized]
	  protected internal IList<VariableInstanceLifecycleListener<VariableInstanceEntity>> registeredVariableListeners = new List<VariableInstanceLifecycleListener<VariableInstanceEntity>>();

	  public ExecutionEntity()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public override ExecutionEntity createExecution()
	  {
		return createExecution(false);
	  }

	  /// <summary>
	  /// creates a new execution. properties processDefinition, processInstance and
	  /// activity will be initialized.
	  /// </summary>
	  public override ExecutionEntity createExecution(bool initializeExecutionStartContext)
	  {
		// create the new child execution
		ExecutionEntity createdExecution = createNewExecution();

		// initialize sequence counter
		createdExecution.SequenceCounter = SequenceCounter;

		// manage the bidirectional parent-child relation
		createdExecution.Parent = this;

		// initialize the new execution
		createdExecution.setProcessDefinition(getProcessDefinition());
		createdExecution.setProcessInstance(getProcessInstance());
		createdExecution.setActivity(getActivity());
		createdExecution.SuspensionState = SuspensionState;

		// make created execution start in same activity instance
		createdExecution.activityInstanceId = activityInstanceId;

		// inherit the tenant id from parent execution
		if (!string.ReferenceEquals(tenantId, null))
		{
		  createdExecution.TenantId = tenantId;
		}

		// with the fix of CAM-9249 we presume that the parent and the child have the same startContext
		if (initializeExecutionStartContext)
		{
		  createdExecution.StartContext = new ExecutionStartContext();
		}
		else if (startContext != null)
		{
		  createdExecution.StartContext = startContext;
		}

		createdExecution.skipCustomListeners = this.skipCustomListeners;
		createdExecution.skipIoMapping = this.skipIoMapping;

		LOG.createChildExecution(createdExecution, this);

		return createdExecution;
	  }

	  // sub process instance
	  // /////////////////////////////////////////////////////////////

	  public override ExecutionEntity createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId)
	  {
		shouldQueryForSubprocessInstance = true;

		ExecutionEntity subProcessInstance = (ExecutionEntity) base.createSubProcessInstance(processDefinition, businessKey, caseInstanceId);

		// inherit the tenant-id from the process definition
		string tenantId = ((ProcessDefinitionEntity) processDefinition).TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  subProcessInstance.TenantId = tenantId;
		}
		else
		{
		  // if process definition has no tenant id, inherit this process instance's tenant id
		  subProcessInstance.TenantId = this.tenantId;
		}

		fireHistoricActivityInstanceUpdate();

		return subProcessInstance;
	  }

	  protected internal static ExecutionEntity createNewExecution()
	  {
		ExecutionEntity newExecution = new ExecutionEntity();
		initializeAssociations(newExecution);
		newExecution.insert();

		return newExecution;
	  }

	  protected internal override PvmExecutionImpl newExecution()
	  {
		return createNewExecution();
	  }

	  // sub case instance ////////////////////////////////////////////////////////

	  public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition)
	  {
		return createSubCaseInstance(caseDefinition, null);
	  }

	  public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey)
	  {
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) caseDefinition.createCaseInstance(businessKey);

		// inherit the tenant-id from the case definition
		string tenantId = ((CaseDefinitionEntity) caseDefinition).TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  subCaseInstance.TenantId = tenantId;
		}
		else
		{
		  // if case definition has no tenant id, inherit this process instance's tenant id
		  subCaseInstance.TenantId = this.tenantId;
		}

		// manage bidirectional super-process-sub-case-instances relation
		subCaseInstance.setSuperExecution(this);
		setSubCaseInstance(subCaseInstance);

		fireHistoricActivityInstanceUpdate();

		return subCaseInstance;
	  }

	  // helper ///////////////////////////////////////////////////////////////////

	  public virtual void fireHistoricActivityInstanceUpdate()
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.ACTIVITY_INSTANCE_UPDATE, this))
		{
		  // publish update event for current activity instance (containing the id
		  // of the sub process/case)
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly ExecutionEntity outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(ExecutionEntity outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createActivityInstanceUpdateEvt(outerInstance);
		  }
	  }

	  // scopes ///////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void initialize()
	  public override void initialize()
	  {
		LOG.initializeExecution(this);

		ScopeImpl scope = ScopeActivity;
		ensureParentInitialized();

		IList<VariableDeclaration> variableDeclarations = (IList<VariableDeclaration>) scope.getProperty(BpmnParse.PROPERTYNAME_VARIABLE_DECLARATIONS);
		if (variableDeclarations != null)
		{
		  foreach (VariableDeclaration variableDeclaration in variableDeclarations)
		  {
			variableDeclaration.initialize(this, parent);
		  }
		}

		if (ProcessInstanceExecution)
		{
		  string initiatorVariableName = (string) processDefinition.getProperty(BpmnParse.PROPERTYNAME_INITIATOR_VARIABLE_NAME);
		  if (!string.ReferenceEquals(initiatorVariableName, null))
		  {
			string authenticatedUserId = Context.CommandContext.AuthenticatedUserId;
			setVariable(initiatorVariableName, authenticatedUserId);
		  }
		}

		// create event subscriptions for the current scope
		foreach (EventSubscriptionDeclaration declaration in EventSubscriptionDeclaration.getDeclarationsForScope(scope).Values)
		{
		  if (!declaration.StartEvent)
		  {
			declaration.createSubscriptionForExecution(this);
		  }
		}
	  }

	  public override void initializeTimerDeclarations()
	  {
		LOG.initializeTimerDeclaration(this);
		ScopeImpl scope = ScopeActivity;
		ICollection<TimerDeclarationImpl> timerDeclarations = TimerDeclarationImpl.getDeclarationsForScope(scope).Values;
		foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
		{
		  timerDeclaration.createTimerInstance(this);
		}
	  }

	  protected internal static void initializeAssociations(ExecutionEntity execution)
	  {
		// initialize the lists of referenced objects (prevents db queries)
		execution.executions = new List<>();
		execution.variableStore.setVariablesProvider(VariableCollectionProvider.emptyVariables<VariableInstanceEntity>());
		execution.variableStore.forceInitialization();
		execution.eventSubscriptions = new List<>();
		execution.jobs = new List<>();
		execution.tasks = new List<>();
		execution.externalTasks = new List<>();
		execution.incidents = new List<>();

		// Cached entity-state initialized to null, all bits are zero, indicating NO
		// entities present
		execution.cachedEntityState = 0;
	  }

	  public override void start(IDictionary<string, object> variables)
	  {
		if (getSuperExecution() == null)
		{
		  RootProcessInstanceId = processInstanceId;
		}
		else
		{
		  ExecutionEntity superExecution = getSuperExecution();
		  RootProcessInstanceId = superExecution.RootProcessInstanceId;
		}

		// determine tenant Id if null
		provideTenantId(variables);
		base.start(variables);
	  }

	  public override void startWithoutExecuting(IDictionary<string, object> variables)
	  {
		RootProcessInstanceId = ProcessInstanceId;
		provideTenantId(variables);
		base.startWithoutExecuting(variables);
	  }

	  protected internal virtual void provideTenantId(IDictionary<string, object> variables)
	  {
		if (string.ReferenceEquals(tenantId, null))
		{
		  TenantIdProvider tenantIdProvider = Context.ProcessEngineConfiguration.TenantIdProvider;

		  if (tenantIdProvider != null)
		  {
			VariableMap variableMap = Variables.fromMap(variables);
			ProcessDefinition processDefinition = getProcessDefinition();

			TenantIdProviderProcessInstanceContext ctx;
			if (!string.ReferenceEquals(superExecutionId, null))
			{
			  ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap, getSuperExecution());
			}
			else if (!string.ReferenceEquals(superCaseExecutionId, null))
			{
			  ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap, getSuperCaseExecution());
			}
			else
			{
			  ctx = new TenantIdProviderProcessInstanceContext(processDefinition, variableMap);
			}

			tenantId = tenantIdProvider.provideTenantIdForProcessInstance(ctx);
		  }
		}
	  }

	  public virtual void startWithFormProperties(VariableMap properties)
	  {
		RootProcessInstanceId = ProcessInstanceId;
		provideTenantId(properties);
		if (ProcessInstanceExecution)
		{
		  ActivityImpl initial = processDefinition.Initial;
		  ProcessInstanceStartContext processInstanceStartContext = ProcessInstanceStartContext;
		  if (processInstanceStartContext != null)
		  {
			initial = processInstanceStartContext.Initial;
		  }
		  FormPropertyStartContext formPropertyStartContext = new FormPropertyStartContext(initial);
		  formPropertyStartContext.FormProperties = properties;
		  startContext = formPropertyStartContext;

		  initialize();
		  initializeTimerDeclarations();
		  fireHistoricProcessStartEvent();
		}

		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START);
	  }

	  public override void fireHistoricProcessStartEvent()
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;
		// TODO: This smells bad, as the rest of the history is done via the
		// ParseListener
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_START, processInstance))
		{

		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly ExecutionEntity outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass2(ExecutionEntity outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createProcessInstanceStartEvt(outerInstance.processInstance);
		  }
	  }

	  /// <summary>
	  /// Method used for destroying a scope in a way that the execution can be
	  /// removed afterwards.
	  /// </summary>
	  public override void destroy()
	  {

		ensureParentInitialized();

		// execute Output Mappings (if they exist).
		ensureActivityInitialized();
		if (activity != null && activity.IoMapping != null && !skipIoMapping)
		{
		  activity.IoMapping.executeOutputParameters(this);
		}

		clearExecution();

		base.destroy();

		removeEventSubscriptionsExceptCompensation();
	  }

	  public override void removeAllTasks()
	  {
		// delete all the tasks
		removeTasks(null);

		// delete external tasks
		removeExternalTasks();
	  }

	  protected internal virtual void clearExecution()
	  {
		//call the onRemove method of the execution observers
		//so they can do some clean up before
		foreach (ExecutionObserver observer in executionObservers)
		{
		  observer.onClear(this);
		}

		// delete all the tasks and external tasks
		removeAllTasks();

		// delete all the variable instances
		removeVariablesLocalInternal();

		// remove all jobs
		removeJobs();

		// remove all incidents
		removeIncidents();
	  }

	  public override void removeVariablesLocalInternal()
	  {
		foreach (VariableInstanceEntity variableInstance in variableStore.Variables)
		{
		  invokeVariableLifecycleListenersDelete(variableInstance, this, Collections.singletonList(VariablePersistenceListener));
		  removeVariableInternal(variableInstance);
		}
	  }

	  public override void interrupt(string reason, bool skipCustomListeners, bool skipIoMappings)
	  {

		// remove Jobs
		if (preserveScope)
		{
		  removeActivityJobs(reason);
		}
		else
		{
		  removeJobs();
		  removeEventSubscriptionsExceptCompensation();
		}

		removeTasks(reason);

		base.interrupt(reason, skipCustomListeners, skipIoMappings);
	  }

	  protected internal virtual void removeActivityJobs(string reason)
	  {
		if (!string.ReferenceEquals(activityId, null))
		{
		  foreach (JobEntity job in Jobs)
		  {
			if (activityId.Equals(job.ActivityId))
			{
			  job.delete();
			  removeJob(job);
			}
		  }

		}

	  }

	  // methods that translate to operations /////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("deprecation") public <T extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> void performOperation(org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation<T> operation)
	  public override void performOperation<T>(CoreAtomicOperation<T> operation) where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	  {
		if (operation is AtomicOperation)
		{
		  performOperation((AtomicOperation) operation);
		}
		else
		{
		  base.performOperation(operation);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("deprecation") public <T extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> void performOperationSync(org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation<T> operation)
	  public override void performOperationSync<T>(CoreAtomicOperation<T> operation) where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	  {
		if (operation is AtomicOperation)
		{
		  performOperationSync((AtomicOperation) operation);
		}
		else
		{
		  base.performOperationSync(operation);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void performOperation(org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation executionOperation)
	  public virtual void performOperation(AtomicOperation executionOperation)
	  {
		bool async = executionOperation.isAsync(this);

		if (!async && requiresUnsuspendedExecution(executionOperation))
		{
		  ensureNotSuspended();
		}

		Context.CommandInvocationContext.performOperation(executionOperation, this, async);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void performOperationSync(org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation executionOperation)
	  public virtual void performOperationSync(AtomicOperation executionOperation)
	  {
		if (requiresUnsuspendedExecution(executionOperation))
		{
		  ensureNotSuspended();
		}

		Context.CommandInvocationContext.performOperation(executionOperation, this);
	  }

	  protected internal virtual void ensureNotSuspended()
	  {
		if (Suspended)
		{
		  throw LOG.suspendedEntityException("Execution", id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") protected boolean requiresUnsuspendedExecution(org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation executionOperation)
	  protected internal virtual bool requiresUnsuspendedExecution(AtomicOperation executionOperation)
	  {
		if (executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_DESTROY_SCOPE && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_START && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE && executionOperation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END)
		{
		  return true;
		}

		return false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked"}) public void scheduleAtomicOperationAsync(org.camunda.bpm.engine.impl.interceptor.AtomicOperationInvocation executionOperationInvocation)
	  public virtual void scheduleAtomicOperationAsync(AtomicOperationInvocation executionOperationInvocation)
	  {

		MessageJobDeclaration messageJobDeclaration = null;

		IList<MessageJobDeclaration> messageJobDeclarations = (IList<MessageJobDeclaration>) getActivity().getProperty(BpmnParse.PROPERTYNAME_MESSAGE_JOB_DECLARATION);
		if (messageJobDeclarations != null)
		{
		  foreach (MessageJobDeclaration declaration in messageJobDeclarations)
		  {
			if (declaration.isApplicableForOperation(executionOperationInvocation.Operation))
			{
			  messageJobDeclaration = declaration;
			  break;
			}
		  }
		}

		if (messageJobDeclaration != null)
		{
		  MessageEntity message = messageJobDeclaration.createJobInstance(executionOperationInvocation);
		  Context.CommandContext.JobManager.send(message);

		}
		else
		{
		  throw LOG.requiredAsyncContinuationException(getActivity().Id);
		}
	  }

	  public override bool isActive(string activityId)
	  {
		return findExecution(activityId) != null;
	  }

	  public override void inactivate()
	  {
		this.isActive_Renamed = false;
	  }

	  // executions ///////////////////////////////////////////////////////////////

	  public virtual void addExecutionObserver(ExecutionObserver observer)
	  {
		executionObservers.Add(observer);
	  }

	  public virtual void removeExecutionObserver(ExecutionObserver observer)
	  {
		executionObservers.Remove(observer);
	  }

	  public override IList<ExecutionEntity> Executions
	  {
		  get
		  {
			ensureExecutionsInitialized();
			return executions;
		  }
		  set
		  {
			this.executions = value;
		  }
	  }

	  public override IList<ExecutionEntity> ExecutionsAsCopy
	  {
		  get
		  {
			return new List<>(Executions);
		  }
	  }

	  protected internal virtual void ensureExecutionsInitialized()
	  {
		if (executions == null)
		{
		  if (ExecutionTreePrefetchEnabled)
		  {
			ensureExecutionTreeInitialized();

		  }
		  else
		  {
			this.executions = Context.CommandContext.ExecutionManager.findChildExecutionsByParentExecutionId(id);
		  }

		}
	  }

	  /// <returns> true if execution tree prefetching is enabled </returns>
	  protected internal virtual bool ExecutionTreePrefetchEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ExecutionTreePrefetchEnabled;
		  }
	  }


	  // bussiness key ////////////////////////////////////////////////////////////

	  public override string ProcessBusinessKey
	  {
		  get
		  {
			return getProcessInstance().BusinessKey;
		  }
	  }

	  // process definition ///////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process definition. </summary>
	  public override ProcessDefinitionEntity getProcessDefinition()
	  {
		ensureProcessDefinitionInitialized();
		return (ProcessDefinitionEntity) processDefinition;
	  }

	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
		  get
		  {
			return processDefinitionId;
		  }
	  }


	  /// <summary>
	  /// for setting the process definition, this setter must be used as subclasses
	  /// can override
	  /// </summary>
	  protected internal virtual void ensureProcessDefinitionInitialized()
	  {
		if ((processDefinition == null) && (!string.ReferenceEquals(processDefinitionId, null)))
		{
		  ProcessDefinitionEntity deployedProcessDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		  setProcessDefinition(deployedProcessDefinition);
		}
	  }

	  public override void setProcessDefinition(ProcessDefinitionImpl processDefinition)
	  {
		this.processDefinition = processDefinition;
		if (processDefinition != null)
		{
		  this.processDefinitionId = processDefinition.Id;
		}
		else
		{
		  this.processDefinitionId = null;
		}

	  }

	  // process instance /////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process instance. </summary>
	  public override ExecutionEntity getProcessInstance()
	  {
		ensureProcessInstanceInitialized();
		return processInstance;
	  }

	  protected internal virtual void ensureProcessInstanceInitialized()
	  {
		if ((processInstance == null) && (!string.ReferenceEquals(processInstanceId, null)))
		{

		  if (ExecutionTreePrefetchEnabled)
		  {
			ensureExecutionTreeInitialized();

		  }
		  else
		  {
			processInstance = Context.CommandContext.ExecutionManager.findExecutionById(processInstanceId);
		  }

		}
	  }

	  public override void setProcessInstance(PvmExecutionImpl processInstance)
	  {
		this.processInstance = (ExecutionEntity) processInstance;
		if (processInstance != null)
		{
		  this.processInstanceId = this.processInstance.Id;
		}
	  }

	  public override bool ProcessInstanceExecution
	  {
		  get
		  {
			return string.ReferenceEquals(parentId, null);
		  }
	  }

	  // activity /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the activity </summary>
	  public override ActivityImpl getActivity()
	  {
		ensureActivityInitialized();
		return base.getActivity();
	  }

	  public override string ActivityId
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

	  /// <summary>
	  /// must be called before the activity member field or getActivity() is called </summary>
	  protected internal virtual void ensureActivityInitialized()
	  {
		if ((activity == null) && (!string.ReferenceEquals(activityId, null)))
		{
		  setActivity(getProcessDefinition().findActivity(activityId));
		}
	  }

	  public override void setActivity(PvmActivity activity)
	  {
		base.setActivity(activity);
		if (activity != null)
		{
		  this.activityId = activity.Id;
		  this.activityName = (string) activity.getProperty("name");
		}
		else
		{
		  this.activityId = null;
		  this.activityName = null;
		}

	  }

	  /// <summary>
	  /// generates an activity instance id
	  /// </summary>
	  protected internal override string generateActivityInstanceId(string activityId)
	  {

		if (activityId.Equals(processDefinitionId))
		{
		  return processInstanceId;

		}
		else
		{

		  string nextId = Context.ProcessEngineConfiguration.IdGenerator.NextId;

		  string compositeId = activityId + ":" + nextId;
		  if (compositeId.Length > 64)
		  {
			return nextId.ToString();
		  }
		  else
		  {
			return compositeId;
		  }
		}
	  }

	  // parent ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the parent </summary>
	  public override ExecutionEntity Parent
	  {
		  get
		  {
			ensureParentInitialized();
			return parent;
		  }
	  }

	  protected internal virtual void ensureParentInitialized()
	  {
		if (parent == null && !string.ReferenceEquals(parentId, null))
		{
		  if (ExecutionTreePrefetchEnabled)
		  {
			ensureExecutionTreeInitialized();

		  }
		  else
		  {
			parent = Context.CommandContext.ExecutionManager.findExecutionById(parentId);
		  }
		}
	  }

	  public override PvmExecutionImpl ParentExecution
	  {
		  set
		  {
			this.parent = (ExecutionEntity) value;
    
			if (value != null)
			{
			  this.parentId = value.Id;
			}
			else
			{
			  this.parentId = null;
			}
		  }
	  }

	  // super- and subprocess executions /////////////////////////////////////////

	  public virtual string SuperExecutionId
	  {
		  get
		  {
			return superExecutionId;
		  }
		  set
		  {
			this.superExecutionId = value;
		  }
	  }

	  public override ExecutionEntity getSuperExecution()
	  {
		ensureSuperExecutionInitialized();
		return superExecution;
	  }

	  public override void setSuperExecution(PvmExecutionImpl superExecution)
	  {
		if (!string.ReferenceEquals(this.superExecutionId, null))
		{
		  ensureSuperExecutionInitialized();
		  this.superExecution.setSubProcessInstance(null);
		}

		this.superExecution = (ExecutionEntity) superExecution;

		if (superExecution != null)
		{
		  this.superExecutionId = superExecution.Id;
		  this.superExecution.setSubProcessInstance(this);
		}
		else
		{
		  this.superExecutionId = null;
		}
	  }

	  protected internal virtual void ensureSuperExecutionInitialized()
	  {
		if (superExecution == null && !string.ReferenceEquals(superExecutionId, null))
		{
		  superExecution = Context.CommandContext.ExecutionManager.findExecutionById(superExecutionId);
		}
	  }

	  public override ExecutionEntity getSubProcessInstance()
	  {
		ensureSubProcessInstanceInitialized();
		return subProcessInstance;
	  }

	  public override void setSubProcessInstance(PvmExecutionImpl subProcessInstance)
	  {
		shouldQueryForSubprocessInstance = subProcessInstance != null;
		this.subProcessInstance = (ExecutionEntity) subProcessInstance;
	  }

	  protected internal virtual void ensureSubProcessInstanceInitialized()
	  {
		if (shouldQueryForSubprocessInstance && subProcessInstance == null)
		{
		  subProcessInstance = Context.CommandContext.ExecutionManager.findSubProcessInstanceBySuperExecutionId(id);
		}
	  }

	  // super case executions ///////////////////////////////////////////////////

	  public virtual string SuperCaseExecutionId
	  {
		  get
		  {
			return superCaseExecutionId;
		  }
		  set
		  {
			this.superCaseExecutionId = value;
		  }
	  }


	  public override CaseExecutionEntity getSuperCaseExecution()
	  {
		ensureSuperCaseExecutionInitialized();
		return superCaseExecution;
	  }

	  public override void setSuperCaseExecution(CmmnExecution superCaseExecution)
	  {
		this.superCaseExecution = (CaseExecutionEntity) superCaseExecution;

		if (superCaseExecution != null)
		{
		  this.superCaseExecutionId = superCaseExecution.Id;
		  this.caseInstanceId = superCaseExecution.CaseInstanceId;
		}
		else
		{
		  this.superCaseExecutionId = null;
		  this.caseInstanceId = null;
		}
	  }

	  protected internal virtual void ensureSuperCaseExecutionInitialized()
	  {
		if (superCaseExecution == null && !string.ReferenceEquals(superCaseExecutionId, null))
		{
		  superCaseExecution = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(superCaseExecutionId);
		}
	  }

	  // sub case execution //////////////////////////////////////////////////////

	  public override CaseExecutionEntity getSubCaseInstance()
	  {
		ensureSubCaseInstanceInitialized();
		return subCaseInstance;

	  }

	  public override void setSubCaseInstance(CmmnExecution subCaseInstance)
	  {
		shouldQueryForSubCaseInstance = subCaseInstance != null;
		this.subCaseInstance = (CaseExecutionEntity) subCaseInstance;
	  }

	  protected internal virtual void ensureSubCaseInstanceInitialized()
	  {
		if (shouldQueryForSubCaseInstance && subCaseInstance == null)
		{
		  subCaseInstance = Context.CommandContext.CaseExecutionManager.findSubCaseInstanceBySuperExecutionId(id);
		}
	  }

	  // customized persistence behavior /////////////////////////////////////////

	  public override void remove()
	  {
		base.remove();

		// removes jobs, incidents and tasks, and
		// clears the variable store
		clearExecution();

		// remove all event subscriptions for this scope, if the scope has event
		// subscriptions:
		removeEventSubscriptions();

		// finally delete this execution
		Context.CommandContext.ExecutionManager.deleteExecution(this);
	  }

	  protected internal virtual void removeEventSubscriptionsExceptCompensation()
	  {
		// remove event subscriptions which are not compensate event subscriptions
		IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptions;
		foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
		{
		  if (!EventType.COMPENSATE.name().Equals(eventSubscriptionEntity.EventType))
		  {
			eventSubscriptionEntity.delete();
		  }
		}
	  }

	  public virtual void removeEventSubscriptions()
	  {
		foreach (EventSubscriptionEntity eventSubscription in EventSubscriptions)
		{
		  if (ReplacedBy != null)
		  {
			eventSubscription.Execution = ReplacedBy;
		  }
		  else
		  {
			eventSubscription.delete();
		  }
		}
	  }

	  private void removeJobs()
	  {
		foreach (Job job in Jobs)
		{
		  if (ReplacedByParent)
		  {
			((JobEntity) job).Execution = ReplacedBy;
		  }
		  else
		  {
			((JobEntity) job).delete();
		  }
		}
	  }

	  private void removeIncidents()
	  {
		foreach (IncidentEntity incident in Incidents)
		{
		  if (ReplacedByParent)
		  {
			incident.Execution = ReplacedBy;
		  }
		  else
		  {
			incident.delete();
		  }
		}
	  }

	  protected internal virtual void removeTasks(string reason)
	  {
		if (string.ReferenceEquals(reason, null))
		{
		  reason = TaskEntity.DELETE_REASON_DELETED;
		}
		foreach (TaskEntity task in Tasks)
		{
		  if (ReplacedByParent)
		  {
			if (task.getExecution() == null || task.getExecution() != replacedBy)
			{
			  // All tasks should have been moved when "replacedBy" has been set.
			  // Just in case tasks where added,
			  // wo do an additional check here and move it
			  task.setExecution(replacedBy);
			  this.ReplacedBy.addTask(task);
			}
		  }
		  else
		  {
			task.delete(reason, false, skipCustomListeners);
		  }
		}
	  }

	  protected internal virtual void removeExternalTasks()
	  {
		foreach (ExternalTaskEntity externalTask in ExternalTasks)
		{
		  externalTask.delete();
		}
	  }

	  public override ExecutionEntity ReplacedBy
	  {
		  get
		  {
			return (ExecutionEntity) replacedBy;
		  }
	  }

	  public override ExecutionEntity resolveReplacedBy()
	  {
		return (ExecutionEntity) base.resolveReplacedBy();
	  }

	  public override void replace(PvmExecutionImpl execution)
	  {
		ExecutionEntity replacedExecution = (ExecutionEntity) execution;

		ListenerIndex = replacedExecution.ListenerIndex;
		replacedExecution.ListenerIndex = 0;

		// update the related tasks
		replacedExecution.moveTasksTo(this);

		replacedExecution.moveExternalTasksTo(this);

		// update those jobs that are directly related to the argument execution's
		// current activity
		replacedExecution.moveActivityLocalJobsTo(this);

		if (!replacedExecution.Ended)
		{
		  // on compaction, move all variables
		  if (replacedExecution.Parent == this)
		  {
			replacedExecution.moveVariablesTo(this);
		  }
		  // on expansion, move only concurrent local variables
		  else
		  {
			replacedExecution.moveConcurrentLocalVariablesTo(this);
		  }
		}

		// note: this method not move any event subscriptions since concurrent
		// executions
		// do not have event subscriptions (and either one of the executions
		// involved in this
		// operation is concurrent)

		base.replace(replacedExecution);
	  }

	  public override void onConcurrentExpand(PvmExecutionImpl scopeExecution)
	  {
		ExecutionEntity scopeExecutionEntity = (ExecutionEntity) scopeExecution;
		scopeExecutionEntity.moveConcurrentLocalVariablesTo(this);
		base.onConcurrentExpand(scopeExecutionEntity);
	  }

	  protected internal virtual void moveTasksTo(ExecutionEntity other)
	  {
		// update the related tasks
		foreach (TaskEntity task in TasksInternal)
		{
		  task.setExecution(other);

		  // update the related local task variables
		  ICollection<VariableInstanceEntity> variables = task.VariablesInternal;

		  foreach (VariableInstanceEntity variable in variables)
		  {
			variable.Execution = other;
		  }

		  other.addTask(task);
		}
		TasksInternal.Clear();
	  }

	  protected internal virtual void moveExternalTasksTo(ExecutionEntity other)
	  {
		foreach (ExternalTaskEntity externalTask in ExternalTasksInternal)
		{
		  externalTask.ExecutionId = other.Id;
		  externalTask.Execution = other;

		  other.addExternalTask(externalTask);
		}

		ExternalTasksInternal.Clear();
	  }

	  protected internal virtual void moveActivityLocalJobsTo(ExecutionEntity other)
	  {
		if (!string.ReferenceEquals(activityId, null))
		{
		  foreach (JobEntity job in Jobs)
		  {

			if (activityId.Equals(job.ActivityId))
			{
			  removeJob(job);
			  job.Execution = other;
			}
		  }
		}
	  }

	  protected internal virtual void moveVariablesTo(ExecutionEntity other)
	  {
		IList<VariableInstanceEntity> variables = variableStore.Variables;
		variableStore.removeVariables();

		foreach (VariableInstanceEntity variable in variables)
		{
		  moveVariableTo(variable, other);
		}
	  }

	  protected internal virtual void moveVariableTo(VariableInstanceEntity variable, ExecutionEntity other)
	  {
		if (other.variableStore.containsKey(variable.Name))
		{
		  CoreVariableInstance existingInstance = other.variableStore.getVariable(variable.Name);
		  existingInstance.Value = variable.getTypedValue(false);
		  invokeVariableLifecycleListenersUpdate(existingInstance, this);
		  invokeVariableLifecycleListenersDelete(variable, this, Collections.singletonList(VariablePersistenceListener));
		}
		else
		{
		  other.variableStore.addVariable(variable);
		}
	  }

	  protected internal virtual void moveConcurrentLocalVariablesTo(ExecutionEntity other)
	  {
		IList<VariableInstanceEntity> variables = variableStore.Variables;

		foreach (VariableInstanceEntity variable in variables)
		{
		  if (variable.ConcurrentLocal)
		  {
			moveVariableTo(variable, other);
		  }
		}
	  }

	  // variables ////////////////////////////////////////////////////////////////

	  public virtual void addVariableListener(VariableInstanceLifecycleListener<VariableInstanceEntity> listener)
	  {
		registeredVariableListeners.Add(listener);
	  }

	  public virtual void removeVariableListener(VariableInstanceLifecycleListener<VariableInstanceEntity> listener)
	  {
		registeredVariableListeners.Remove(listener);
	  }

	  public virtual bool ExecutingScopeLeafActivity
	  {
		  get
		  {
			return isActive_Renamed && getActivity() != null && getActivity().Scope && !string.ReferenceEquals(activityInstanceId, null) && !(getActivity().ActivityBehavior is CompositeActivityBehavior);
		  }
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables()
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByExecutionId(id);
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables(ICollection<string> variableNames)
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByExecutionIdAndVariableNames(id, variableNames);
	  }

	  protected internal virtual bool AutoFireHistoryEvents
	  {
		  get
		  {
			// as long as the process instance is starting (i.e. before activity instance
			// of the selected initial (start event) is created), the variable scope should
			// not automatic fire history events for variable updates.
    
			// firing the events is triggered by the processInstanceStart context after
			// the initial activity has been initialized. The effect is that the activity instance id of the
			// historic variable instances will be the activity instance id of the start event.
    
			// if a variable is updated while the process instance is starting then the
			// update history event is lost and the updated value is handled as initial value.
    
			ActivityImpl currentActivity = getActivity();
    
			return (startContext == null || !startContext.DelayFireHistoricVariableEvents) && (currentActivity == null || isActivityNoStartEvent(currentActivity) || isStartEventInValidStateOrNotAsync(currentActivity));
		  }
	  }

	  protected internal virtual bool isActivityNoStartEvent(ActivityImpl currentActivity)
	  {
		return !(currentActivity.ActivityBehavior is NoneStartEventActivityBehavior);
	  }

	  protected internal virtual bool isStartEventInValidStateOrNotAsync(ActivityImpl currentActivity)
	  {
		return ActivityInstanceState != org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.DEFAULT.StateCode || !currentActivity.AsyncBefore;
	  }

	  public virtual void fireHistoricVariableInstanceCreateEvents()
	  {
		// this method is called by the start context and batch-fires create events
		// for all variable instances
		IList<VariableInstanceEntity> variableInstances = variableStore.Variables;
		VariableInstanceHistoryListener historyListener = new VariableInstanceHistoryListener();
		if (variableInstances != null)
		{
		  foreach (VariableInstanceEntity variable in variableInstances)
		  {
			historyListener.onCreate(variable, this);
		  }
		}
	  }

	  /// <summary>
	  /// Fetch all the executions inside the same process instance as list and then
	  /// reconstruct the complete execution tree.
	  /// 
	  /// In many cases this is an optimization over fetching the execution tree
	  /// lazily. Usually we need all executions anyway and it is preferable to fetch
	  /// more data in a single query (maybe even too much data) then to run multiple
	  /// queries, each returning a fraction of the data.
	  /// 
	  /// The most important consideration here is network roundtrip: If the process
	  /// engine and database run on separate hosts, network roundtrip has to be
	  /// added to each query. Economizing on the number of queries economizes on
	  /// network roundtrip. The tradeoff here is network roundtrip vs. throughput:
	  /// multiple roundtrips carrying small chucks of data vs. a single roundtrip
	  /// carrying more data.
	  /// 
	  /// </summary>
	  protected internal virtual void ensureExecutionTreeInitialized()
	  {
		IList<ExecutionEntity> executions = Context.CommandContext.ExecutionManager.findExecutionsByProcessInstanceId(processInstanceId);

		ExecutionEntity processInstance = ProcessInstanceExecution ? this : null;

		if (processInstance == null)
		{
		  foreach (ExecutionEntity execution in executions)
		  {
			if (execution.ProcessInstanceExecution)
			{
			  processInstance = execution;
			}
		  }
		}

		processInstance.restoreProcessInstance(executions, null, null, null, null, null, null);
	  }

	  /// <summary>
	  /// Restores a complete process instance tree including referenced entities.
	  /// </summary>
	  /// <param name="executions">
	  ///   the list of all executions that are part of this process instance.
	  ///   Cannot be null, must include the process instance execution itself. </param>
	  /// <param name="eventSubscriptions">
	  ///   the list of all event subscriptions that are linked to executions which is part of this process instance
	  ///   If null, event subscriptions are not initialized and lazy loaded on demand </param>
	  /// <param name="variables">
	  ///   the list of all variables that are linked to executions which are part of this process instance
	  ///   If null, variables are not initialized and are lazy loaded on demand </param>
	  /// <param name="jobs"> </param>
	  /// <param name="tasks"> </param>
	  /// <param name="incidents"> </param>
	  public virtual void restoreProcessInstance(ICollection<ExecutionEntity> executions, ICollection<EventSubscriptionEntity> eventSubscriptions, ICollection<VariableInstanceEntity> variables, ICollection<TaskEntity> tasks, ICollection<JobEntity> jobs, ICollection<IncidentEntity> incidents, ICollection<ExternalTaskEntity> externalTasks)
	  {

		if (!ProcessInstanceExecution)
		{
		  throw LOG.restoreProcessInstanceException(this);
		}

		// index executions by id
		IDictionary<string, ExecutionEntity> executionsMap = new Dictionary<string, ExecutionEntity>();
		foreach (ExecutionEntity execution in executions)
		{
		  executionsMap[execution.Id] = execution;
		}

		IDictionary<string, IList<VariableInstanceEntity>> variablesByScope = new Dictionary<string, IList<VariableInstanceEntity>>();
		if (variables != null)
		{
		  foreach (VariableInstanceEntity variable in variables)
		  {
			CollectionUtil.addToMapOfLists(variablesByScope, variable.VariableScopeId, variable);
		  }
		}

		// restore execution tree
		foreach (ExecutionEntity execution in executions)
		{
		  if (execution.executions == null)
		  {
			execution.executions = new List<>();
		  }
		  if (execution.eventSubscriptions == null && eventSubscriptions != null)
		  {
			execution.eventSubscriptions = new List<>();
		  }
		  if (variables != null)
		  {
			execution.variableStore.setVariablesProvider(new VariableCollectionProvider<>(variablesByScope[execution.id]));
		  }
		  string parentId = execution.ParentId;
		  ExecutionEntity parent = executionsMap[parentId];
		  if (!execution.ProcessInstanceExecution)
		  {
			if (parent == null)
			{
			  throw LOG.resolveParentOfExecutionFailedException(parentId, execution.Id);
			}
			execution.processInstance = this;
			execution.parent = parent;
			if (parent.executions == null)
			{
			  parent.executions = new List<>();
			}
			parent.executions.Add(execution);
		  }
		  else
		  {
			execution.processInstance = execution;
		  }
		}

		if (eventSubscriptions != null)
		{
		  // add event subscriptions to the right executions in the tree
		  foreach (EventSubscriptionEntity eventSubscription in eventSubscriptions)
		  {
			ExecutionEntity executionEntity = executionsMap[eventSubscription.ExecutionId];
			if (executionEntity != null)
			{
			  executionEntity.addEventSubscription(eventSubscription);
			}
			else
			{
			  throw LOG.executionNotFoundException(eventSubscription.ExecutionId);
			}
		  }
		}

		if (jobs != null)
		{
		  foreach (JobEntity job in jobs)
		  {
			ExecutionEntity execution = executionsMap[job.ExecutionId];
			job.Execution = execution;
		  }
		}

		if (tasks != null)
		{
		  foreach (TaskEntity task in tasks)
		  {
			ExecutionEntity execution = executionsMap[task.ExecutionId];
			task.setExecution(execution);
			execution.addTask(task);

			if (variables != null)
			{
			  task.variableStore.setVariablesProvider(new VariableCollectionProvider<>(variablesByScope[task.id]));
			}
		  }
		}


		if (incidents != null)
		{
		  foreach (IncidentEntity incident in incidents)
		  {
			ExecutionEntity execution = executionsMap[incident.ExecutionId];
			incident.Execution = execution;
		  }
		}

		if (externalTasks != null)
		{
		  foreach (ExternalTaskEntity externalTask in externalTasks)
		  {
			ExecutionEntity execution = executionsMap[externalTask.ExecutionId];
			externalTask.Execution = execution;
			execution.addExternalTask(externalTask);
		  }
		}
	  }


	  // persistent state /////////////////////////////////////////////////////////

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["processDefinitionId"] = this.processDefinitionId;
			persistentState["businessKey"] = businessKey;
			persistentState["activityId"] = this.activityId;
			persistentState["activityInstanceId"] = this.activityInstanceId;
			persistentState["isActive"] = this.isActive_Renamed;
			persistentState["isConcurrent"] = this.isConcurrent;
			persistentState["isScope"] = this.isScope;
			persistentState["isEventScope"] = this.isEventScope;
			persistentState["parentId"] = parentId;
			persistentState["superExecution"] = this.superExecutionId;
			persistentState["superCaseExecutionId"] = this.superCaseExecutionId;
			persistentState["caseInstanceId"] = this.caseInstanceId;
			persistentState["suspensionState"] = this.suspensionState;
			persistentState["cachedEntityState"] = CachedEntityState;
			persistentState["sequenceCounter"] = SequenceCounter;
			return persistentState;
		  }
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.ExecutionManager.insertExecution(this);
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public override void forceUpdate()
	  {
		Context.CommandContext.DbEntityManager.forceUpdate(this);
	  }

	  // toString /////////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		if (ProcessInstanceExecution)
		{
		  return "ProcessInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return (isConcurrent ? "Concurrent" : "") + (isScope ? "Scope" : "") + "Execution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal override string ToStringIdentity
	  {
		  get
		  {
			return id;
		  }
	  }

	  // event subscription support //////////////////////////////////////////////

	  public virtual IList<EventSubscriptionEntity> EventSubscriptionsInternal
	  {
		  get
		  {
			ensureEventSubscriptionsInitialized();
			return eventSubscriptions;
		  }
	  }

	  public virtual IList<EventSubscriptionEntity> EventSubscriptions
	  {
		  get
		  {
			return new List<>(EventSubscriptionsInternal);
		  }
	  }

	  public virtual IList<EventSubscriptionEntity> CompensateEventSubscriptions
	  {
		  get
		  {
			IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
			IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>(eventSubscriptions.Count);
			foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
			{
			  if (eventSubscriptionEntity.isSubscriptionForEventType(EventType.COMPENSATE))
			  {
				result.Add(eventSubscriptionEntity);
			  }
			}
			return result;
		  }
	  }

	  public virtual IList<EventSubscriptionEntity> getCompensateEventSubscriptions(string activityId)
	  {
		IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
		IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>(eventSubscriptions.Count);
		foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
		{
		  if (eventSubscriptionEntity.isSubscriptionForEventType(EventType.COMPENSATE) && activityId.Equals(eventSubscriptionEntity.ActivityId))
		  {
			  result.Add(eventSubscriptionEntity);
		  }
		}
		return result;
	  }

	  protected internal virtual void ensureEventSubscriptionsInitialized()
	  {
		if (eventSubscriptions == null)
		{

		  eventSubscriptions = Context.CommandContext.EventSubscriptionManager.findEventSubscriptionsByExecution(id);
		}
	  }

	  public virtual void addEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
	  {
		IList<EventSubscriptionEntity> eventSubscriptionsInternal = EventSubscriptionsInternal;
		if (!eventSubscriptionsInternal.Contains(eventSubscriptionEntity))
		{
		  eventSubscriptionsInternal.Add(eventSubscriptionEntity);
		}
	  }

	  public virtual void removeEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
	  {
		EventSubscriptionsInternal.Remove(eventSubscriptionEntity);
	  }

	  // referenced job entities //////////////////////////////////////////////////

	  protected internal virtual void ensureJobsInitialized()
	  {
		if (jobs == null)
		{
		  jobs = Context.CommandContext.JobManager.findJobsByExecutionId(id);
		}
	  }

	  protected internal virtual IList<JobEntity> JobsInternal
	  {
		  get
		  {
			ensureJobsInitialized();
			return jobs;
		  }
	  }

	  public virtual IList<JobEntity> Jobs
	  {
		  get
		  {
			return new List<>(JobsInternal);
		  }
	  }

	  public virtual void addJob(JobEntity jobEntity)
	  {
		IList<JobEntity> jobsInternal = JobsInternal;
		if (!jobsInternal.Contains(jobEntity))
		{
		  jobsInternal.Add(jobEntity);
		}
	  }

	  public virtual void removeJob(JobEntity job)
	  {
		JobsInternal.Remove(job);
	  }

	  // referenced incidents entities
	  // //////////////////////////////////////////////

	  protected internal virtual void ensureIncidentsInitialized()
	  {
		if (incidents == null)
		{
		  incidents = Context.CommandContext.IncidentManager.findIncidentsByExecution(id);
		}
	  }

	  protected internal virtual IList<IncidentEntity> IncidentsInternal
	  {
		  get
		  {
			ensureIncidentsInitialized();
			return incidents;
		  }
	  }

	  public virtual IList<IncidentEntity> Incidents
	  {
		  get
		  {
			return new List<>(IncidentsInternal);
		  }
	  }

	  public virtual void addIncident(IncidentEntity incident)
	  {
		IList<IncidentEntity> incidentsInternal = IncidentsInternal;
		if (!incidentsInternal.Contains(incident))
		{
		  incidentsInternal.Add(incident);
		}
	  }

	  public virtual void removeIncident(IncidentEntity incident)
	  {
		IncidentsInternal.Remove(incident);
	  }

	  public virtual IncidentEntity getIncidentByCauseIncidentId(string causeIncidentId)
	  {
		foreach (IncidentEntity incident in Incidents)
		{
		  if (!string.ReferenceEquals(incident.CauseIncidentId, null) && incident.CauseIncidentId.Equals(causeIncidentId))
		  {
			return incident;
		  }
		}
		return null;
	  }

	  // referenced task entities
	  // ///////////////////////////////////////////////////

	  protected internal virtual void ensureTasksInitialized()
	  {
		if (tasks == null)
		{
		  tasks = Context.CommandContext.TaskManager.findTasksByExecutionId(id);
		}
	  }

	  protected internal virtual IList<TaskEntity> TasksInternal
	  {
		  get
		  {
			ensureTasksInitialized();
			return tasks;
		  }
	  }

	  public virtual IList<TaskEntity> Tasks
	  {
		  get
		  {
			return new List<>(TasksInternal);
		  }
	  }

	  public virtual void addTask(TaskEntity taskEntity)
	  {
		IList<TaskEntity> tasksInternal = TasksInternal;
		if (!tasksInternal.Contains(taskEntity))
		{
		  tasksInternal.Add(taskEntity);
		}
	  }

	  public virtual void removeTask(TaskEntity task)
	  {
		TasksInternal.Remove(task);
	  }

	  // external tasks

	  protected internal virtual void ensureExternalTasksInitialized()
	  {
		if (externalTasks == null)
		{
		  externalTasks = Context.CommandContext.ExternalTaskManager.findExternalTasksByExecutionId(id);
		}
	  }

	  protected internal virtual IList<ExternalTaskEntity> ExternalTasksInternal
	  {
		  get
		  {
			ensureExternalTasksInitialized();
			return externalTasks;
		  }
	  }

	  public virtual void addExternalTask(ExternalTaskEntity externalTask)
	  {
		ExternalTasksInternal.Add(externalTask);
	  }

	  public virtual void removeExternalTask(ExternalTaskEntity externalTask)
	  {
		ExternalTasksInternal.Remove(externalTask);
	  }

	  public virtual IList<ExternalTaskEntity> ExternalTasks
	  {
		  get
		  {
			return new List<>(ExternalTasksInternal);
		  }
	  }

	  // variables /////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "rawtypes", "unchecked" }) protected VariableStore<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableStore()
	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return (VariableStore) variableStore;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "rawtypes", "unchecked" }) protected VariableInstanceFactory<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableInstanceFactory()
	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) VariableInstanceEntityFactory.INSTANCE;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "rawtypes", "unchecked" }) protected java.util.List<VariableInstanceLifecycleListener<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance>> getVariableInstanceLifecycleListeners()
	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
    
			IList<VariableInstanceLifecycleListener<CoreVariableInstance>> listeners = new List<VariableInstanceLifecycleListener<CoreVariableInstance>>();
    
			listeners.Add(VariablePersistenceListener);
			listeners.Add((VariableInstanceLifecycleListener) new VariableInstanceConcurrentLocalInitializer(this));
			listeners.Add((VariableInstanceLifecycleListener) VariableInstanceSequenceCounterListener.INSTANCE);
    
			if (AutoFireHistoryEvents)
			{
			  listeners.Add((VariableInstanceLifecycleListener) VariableInstanceHistoryListener.INSTANCE);
			}
    
			listeners.Add((VariableInstanceLifecycleListener) new VariableListenerInvocationListener(this));
    
			((IList<VariableInstanceLifecycleListener<CoreVariableInstance>>)listeners).AddRange((System.Collections.IList) registeredVariableListeners);
    
			return listeners;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public VariableInstanceLifecycleListener<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariablePersistenceListener()
	  public virtual VariableInstanceLifecycleListener<CoreVariableInstance> VariablePersistenceListener
	  {
		  get
		  {
			return (VariableInstanceLifecycleListener) VariableInstanceEntityPersistenceListener.INSTANCE;
		  }
	  }

	  public virtual ICollection<VariableInstanceEntity> VariablesInternal
	  {
		  get
		  {
			return variableStore.Variables;
		  }
	  }

	  public virtual void removeVariableInternal(VariableInstanceEntity variable)
	  {
		if (variableStore.containsValue(variable))
		{
		  variableStore.removeVariable(variable.Name);
		}
	  }

	  public virtual void addVariableInternal(VariableInstanceEntity variable)
	  {
		if (variableStore.containsKey(variable.Name))
		{
		  VariableInstanceEntity existingVariable = variableStore.getVariable(variable.Name);
		  existingVariable.setValue(variable.TypedValue);
		  variable.delete();
		}
		else
		{
		  variableStore.addVariable(variable);
		}
	  }

	  public virtual void handleConditionalEventOnVariableChange(VariableEvent variableEvent)
	  {
		IList<EventSubscriptionEntity> subScriptions = EventSubscriptions;
		foreach (EventSubscriptionEntity subscription in subScriptions)
		{
		  if (EventType.CONDITONAL.name().Equals(subscription.EventType))
		  {
			subscription.processEventSync(variableEvent);
		  }
		}
	  }

	  public override void dispatchEvent(VariableEvent variableEvent)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<ExecutionEntity> execs = new java.util.ArrayList<>();
		IList<ExecutionEntity> execs = new List<ExecutionEntity>();
		(new ExecutionTopDownWalker(this)).addPreVisitor(new TreeVisitorAnonymousInnerClass(this, execs))
	   .walkUntil();
		foreach (ExecutionEntity execution in execs)
		{
		  execution.handleConditionalEventOnVariableChange(variableEvent);
		}
	  }

	  private class TreeVisitorAnonymousInnerClass : TreeVisitor<ExecutionEntity>
	  {
		  private readonly ExecutionEntity outerInstance;

		  private IList<ExecutionEntity> execs;

		  public TreeVisitorAnonymousInnerClass(ExecutionEntity outerInstance, IList<ExecutionEntity> execs)
		  {
			  this.outerInstance = outerInstance;
			  this.execs = execs;
		  }

		  public void visit(ExecutionEntity obj)
		  {
			if (obj.EventSubscriptions.Count > 0 && (obj.isInState(org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.DEFAULT) || (!obj.getActivity().Scope)))
			{ // state is default or tree is compacted
			  execs.Add(obj);
			}
		  }
	  }



	  // getters and setters //////////////////////////////////////////////////////

	  public virtual int CachedEntityState
	  {
		  set
		  {
			this.cachedEntityState = value;
    
			// Check for flags that are down. These lists can be safely initialized as
			// empty, preventing
			// additional queries that end up in an empty list anyway
			if (jobs == null && !BitMaskUtil.isBitOn(value, JOBS_STATE_BIT))
			{
			  jobs = new List<>();
			}
			if (tasks == null && !BitMaskUtil.isBitOn(value, TASKS_STATE_BIT))
			{
			  tasks = new List<>();
			}
			if (eventSubscriptions == null && !BitMaskUtil.isBitOn(value, EVENT_SUBSCRIPTIONS_STATE_BIT))
			{
			  eventSubscriptions = new List<>();
			}
			if (incidents == null && !BitMaskUtil.isBitOn(value, INCIDENT_STATE_BIT))
			{
			  incidents = new List<>();
			}
			if (!variableStore.Initialized && !BitMaskUtil.isBitOn(value, VARIABLES_STATE_BIT))
			{
			  variableStore.setVariablesProvider(VariableCollectionProvider.emptyVariables<VariableInstanceEntity>());
			  variableStore.forceInitialization();
			}
			if (externalTasks == null && !BitMaskUtil.isBitOn(value, EXTERNAL_TASKS_BIT))
			{
			  externalTasks = new List<>();
			}
			shouldQueryForSubprocessInstance = BitMaskUtil.isBitOn(value, SUB_PROCESS_INSTANCE_STATE_BIT);
			shouldQueryForSubCaseInstance = BitMaskUtil.isBitOn(value, SUB_CASE_INSTANCE_STATE_BIT);
		  }
		  get
		  {
			cachedEntityState = 0;
    
			// Only mark a flag as false when the list is not-null and empty. If null,
			// we can't be sure there are no entries in it since
			// the list hasn't been initialized/queried yet.
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, TASKS_STATE_BIT, (tasks == null || tasks.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, EVENT_SUBSCRIPTIONS_STATE_BIT, (eventSubscriptions == null || eventSubscriptions.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, JOBS_STATE_BIT, (jobs == null || jobs.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, INCIDENT_STATE_BIT, (incidents == null || incidents.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, VARIABLES_STATE_BIT, (!variableStore.Initialized || !variableStore.Empty));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, SUB_PROCESS_INSTANCE_STATE_BIT, shouldQueryForSubprocessInstance);
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, SUB_CASE_INSTANCE_STATE_BIT, shouldQueryForSubCaseInstance);
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, EXTERNAL_TASKS_BIT, (externalTasks == null || externalTasks.Count > 0));
    
			return cachedEntityState;
		  }
	  }


	  public virtual int CachedEntityStateRaw
	  {
		  get
		  {
			return cachedEntityState;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			if (ProcessInstanceExecution)
			{
			  return rootProcessInstanceId;
			}
			else
			{
			  ExecutionEntity processInstance = getProcessInstance();
			  return processInstance.rootProcessInstanceId;
			}
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }

	  public virtual string RootProcessInstanceIdRaw
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
	  }


	  public override string ProcessInstanceId
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


	  public override string ParentId
	  {
		  get
		  {
			return parentId;
		  }
		  set
		  {
			this.parentId = value;
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




	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referenceIds = new HashSet<string>();
    
			if (!string.ReferenceEquals(superExecutionId, null))
			{
			  referenceIds.Add(superExecutionId);
			}
			if (!string.ReferenceEquals(parentId, null))
			{
			  referenceIds.Add(parentId);
			}
    
			return referenceIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(superExecutionId, null))
			{
			  referenceIdAndClass[this.superExecutionId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(parentId, null))
			{
			  referenceIdAndClass[this.parentId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(processInstanceId, null))
			{
			  referenceIdAndClass[this.processInstanceId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  referenceIdAndClass[this.processDefinitionId] = typeof(ProcessDefinitionEntity);
			}
    
			return referenceIdAndClass;
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

	  public override ProcessInstanceStartContext ProcessInstanceStartContext
	  {
		  get
		  {
			if (ProcessInstanceExecution)
			{
			  if (startContext == null)
			  {
    
				ActivityImpl activity = getActivity();
				startContext = new ProcessInstanceStartContext(activity);
    
			  }
			}
			return base.ProcessInstanceStartContext;
		  }
	  }

	  public override string CurrentActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public override string CurrentActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public override FlowElement BpmnModelElementInstance
	  {
		  get
		  {
			BpmnModelInstance bpmnModelInstance = BpmnModelInstance;
			if (bpmnModelInstance != null)
			{
    
			  ModelElementInstance modelElementInstance = null;
			  if (org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE.Equals(eventName))
			  {
				modelElementInstance = bpmnModelInstance.getModelElementById(transition.Id);
			  }
			  else
			  {
				modelElementInstance = bpmnModelInstance.getModelElementById(activityId);
			  }
    
			  try
			  {
				return (FlowElement) modelElementInstance;
    
			  }
			  catch (System.InvalidCastException e)
			  {
				ModelElementType elementType = modelElementInstance.ElementType;
				throw LOG.castModelInstanceException(modelElementInstance, "FlowElement", elementType.TypeName, elementType.TypeNamespace, e);
			  }
    
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public override BpmnModelInstance BpmnModelInstance
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

	  public override ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public override ProcessEngine ProcessEngine
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }
	}

}