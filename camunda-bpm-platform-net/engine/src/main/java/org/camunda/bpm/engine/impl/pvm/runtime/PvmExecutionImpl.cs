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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using DefaultIncidentHandler = org.camunda.bpm.engine.impl.incident.DefaultIncidentHandler;
	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using IncidentHandler = org.camunda.bpm.engine.impl.incident.IncidentHandler;
	using DelayedVariableEvent = org.camunda.bpm.engine.impl.persistence.entity.DelayedVariableEvent;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using org.camunda.bpm.engine.impl.pvm;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ModificationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ModificationObserverBehavior;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using org.camunda.bpm.engine.impl.pvm.process;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;
	using org.camunda.bpm.engine.impl.tree;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using Incident = org.camunda.bpm.engine.runtime.Incident;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil.SIGNAL_COMPENSATION_DONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.ENDING;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public abstract class PvmExecutionImpl : CoreExecution, ActivityExecution, PvmProcessInstance
	{
		public abstract void start(IDictionary<string, object> variables);
		public abstract org.camunda.bpm.engine.ProcessEngine ProcessEngine {get;}
		public abstract org.camunda.bpm.engine.ProcessEngineServices ProcessEngineServices {get;}
		public abstract org.camunda.bpm.model.bpmn.instance.FlowElement BpmnModelElementInstance {get;}
		public abstract org.camunda.bpm.model.bpmn.BpmnModelInstance BpmnModelInstance {get;}
		public abstract string ProcessDefinitionId {get;}
		public abstract string ProcessInstanceId {get;}
		public abstract void forceUpdate();
		public abstract void leaveActivityViaTransitions<T1>(IList<org.camunda.bpm.engine.impl.pvm.PvmTransition> outgoingTransitions, IList<T1> joinedExecutions) where T1 : org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	  private const long serialVersionUID = 1L;

	  private static readonly PvmLogger LOG = ProcessEngineLogger.PVM_LOGGER;

	  [NonSerialized]
	  protected internal ProcessDefinitionImpl processDefinition;

	  [NonSerialized]
	  protected internal ExecutionStartContext startContext;

	  // current position /////////////////////////////////////////////////////////

	  /// <summary>
	  /// current activity
	  /// </summary>
	  [NonSerialized]
	  protected internal ActivityImpl activity;

	  /// <summary>
	  /// the activity which is to be started next
	  /// </summary>
	  [NonSerialized]
	  protected internal PvmActivity nextActivity;

	  /// <summary>
	  /// the transition that is currently being taken
	  /// </summary>
	  [NonSerialized]
	  protected internal TransitionImpl transition;

	  /// <summary>
	  /// A list of outgoing transitions from the current activity
	  /// that are going to be taken
	  /// </summary>
	  [NonSerialized]
	  protected internal IList<PvmTransition> transitionsToTake = null;

	  /// <summary>
	  /// the unique id of the current activity instance
	  /// </summary>
	  protected internal string activityInstanceId;

	  /// <summary>
	  /// the id of a case associated with this execution
	  /// </summary>
	  protected internal string caseInstanceId;

	  protected internal PvmExecutionImpl replacedBy;

	  // cascade deletion ////////////////////////////////////////////////////////

	  protected internal bool deleteRoot;
	  protected internal string deleteReason;
	  protected internal bool externallyTerminated;

	  //state/type of execution //////////////////////////////////////////////////

	  /// <summary>
	  /// indicates if this execution represents an active path of execution.
	  /// Executions are made inactive in the following situations:
	  /// <ul>
	  /// <li>an execution enters a nested scope</li>
	  /// <li>an execution is split up into multiple concurrent executions, then the parent is made inactive.</li>
	  /// <li>an execution has arrived in a parallel gateway or join and that join has not yet activated/fired.</li>
	  /// <li>an execution is ended.</li>
	  /// </ul>
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool isActive_Conflict = true;
	  protected internal bool isScope = true;
	  protected internal bool isConcurrent = false;
	  protected internal bool isEnded = false;
	  protected internal bool isEventScope = false;

	  /// <summary>
	  /// transient; used for process instance modification to preserve a scope from getting deleted
	  /// </summary>
	  protected internal bool preserveScope = false;

	  /// <summary>
	  /// marks the current activity instance
	  /// </summary>
	  protected internal int activityInstanceState = ActivityInstanceState_Fields.DEFAULT.StateCode;

	  protected internal bool activityInstanceEndListenersFailed = false;

	  // sequence counter ////////////////////////////////////////////////////////
	  protected internal long sequenceCounter = 0;

	  public PvmExecutionImpl()
	  {
	  }

	  // API ////////////////////////////////////////////////

	  /// <summary>
	  /// creates a new execution. properties processDefinition, processInstance and activity will be initialized.
	  /// </summary>
	  public virtual PvmExecutionImpl createExecution()
	  {
		return createExecution(false);
	  }

	  public override abstract PvmExecutionImpl createExecution(bool initStartContext);

	  // sub process instance

	  public virtual PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition)
	  {
		return createSubProcessInstance(processDefinition, null);
	  }

	  public virtual PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey)
	  {
		PvmExecutionImpl processInstance = ProcessInstance;

		string caseInstanceId = null;
		if (processInstance != null)
		{
		  caseInstanceId = processInstance.CaseInstanceId;
		}

		return createSubProcessInstance(processDefinition, businessKey, caseInstanceId);
	  }

	  public virtual PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId)
	  {
		PvmExecutionImpl subProcessInstance = newExecution();

		// manage bidirectional super-subprocess relation
		subProcessInstance.SuperExecution = this;
		this.SubProcessInstance = subProcessInstance;

		// Initialize the new execution
		subProcessInstance.ProcessDefinition = (ProcessDefinitionImpl) processDefinition;
		subProcessInstance.ProcessInstance = subProcessInstance;
		subProcessInstance.setActivity(processDefinition.Initial);

		if (!string.ReferenceEquals(businessKey, null))
		{
		  subProcessInstance.BusinessKey = businessKey;
		}

		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  subProcessInstance.CaseInstanceId = caseInstanceId;
		}

		return subProcessInstance;
	  }

	  protected internal abstract PvmExecutionImpl newExecution();

	  // sub case instance

	  public override abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition);

	  public override abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey);

	  public abstract void initialize();

	  public abstract void initializeTimerDeclarations();

	  public virtual void executeIoMapping()
	  {
		// execute Input Mappings (if they exist).
		ScopeImpl currentScope = ScopeActivity;
		if (currentScope != currentScope.ProcessDefinition)
		{
		  ActivityImpl currentActivity = (ActivityImpl) currentScope;

		  if (currentActivity != null && currentActivity.IoMapping != null && !skipIoMapping)
		  {
			currentActivity.IoMapping.executeInputParameters(this);
		  }
		}

	  }

	  public virtual void start()
	  {
		start(null);
	  }

	  public virtual void start(IDictionary<string, object> variables)
	  {
		startContext = new ProcessInstanceStartContext(getActivity());

		initialize();

		if (variables != null)
		{
		  Variables = variables;
		}

		initializeTimerDeclarations();

		fireHistoricProcessStartEvent();

		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START);
	  }

	  /// <summary>
	  /// perform starting behavior but don't execute the initial activity
	  /// </summary>
	  /// <param name="variables"> the variables which are used for the start </param>
	  public virtual void startWithoutExecuting(IDictionary<string, object> variables)
	  {
		initialize();
		initializeTimerDeclarations();
		fireHistoricProcessStartEvent();
		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.FIRE_PROCESS_START);

		setActivity(null);
		ActivityInstanceId = Id;

		// set variables
		Variables = variables;
	  }

	  public abstract void fireHistoricProcessStartEvent();

	  public virtual void destroy()
	  {
		LOG.destroying(this);
		Scope = false;
	  }

	  public virtual void removeAllTasks()
	  {
	  }

	  protected internal virtual void removeEventScopes()
	  {
		IList<PvmExecutionImpl> childExecutions = new List<PvmExecutionImpl>(EventScopeExecutions);
		foreach (PvmExecutionImpl childExecution in childExecutions)
		{
		  LOG.removingEventScope(childExecution);
		  childExecution.destroy();
		  childExecution.remove();
		}
	  }

	  public virtual void clearScope(string reason, bool skipCustomListeners, bool skipIoMappings)
	  {
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMapping = skipIoMappings;

		if (SubProcessInstance != null)
		{
		  SubProcessInstance.deleteCascade(reason, skipCustomListeners, skipIoMappings);
		}

		// remove all child executions and sub process instances:
		IList<PvmExecutionImpl> executions = new List<PvmExecutionImpl>(NonEventScopeExecutions);
		foreach (PvmExecutionImpl childExecution in executions)
		{
		  if (childExecution.SubProcessInstance != null)
		  {
			childExecution.SubProcessInstance.deleteCascade(reason, skipCustomListeners, skipIoMappings);
		  }
		  childExecution.deleteCascade(reason, skipCustomListeners, skipIoMappings);
		}

		// fire activity end on active activity
		PvmActivity activity = getActivity();
		if (isActive_Conflict && activity != null)
		{
		  // set activity instance state to cancel
		  if (activityInstanceState != ENDING.StateCode)
		  {
			Canceled = true;
			performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.FIRE_ACTIVITY_END);
		  }
		  // set activity instance state back to 'default'
		  // -> execution will be reused for executing more activities and we want the state to
		  // be default initially.
		  activityInstanceState = ActivityInstanceState_Fields.DEFAULT.StateCode;
		}
	  }

	  /// <summary>
	  /// Interrupts an execution
	  /// </summary>
	  public virtual void interrupt(string reason)
	  {
		interrupt(reason, false, false);
	  }

	  public virtual void interrupt(string reason, bool skipCustomListeners, bool skipIoMappings)
	  {
		LOG.interruptingExecution(reason, skipCustomListeners);

		clearScope(reason, skipCustomListeners, skipIoMappings);
	  }

	  /// <summary>
	  /// Ends an execution. Invokes end listeners for the current activity and notifies the flow scope execution
	  /// of this happening which may result in the flow scope ending.
	  /// </summary>
	  /// <param name="completeScope"> true if ending the execution contributes to completing the BPMN 2.0 scope </param>
	  public virtual void end(bool completeScope)
	  {


		CompleteScope = completeScope;

		isActive_Conflict = false;
		isEnded = true;

		if (hasReplacedParent())
		{
		  Parent.replacedBy = null;
		}

		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_NOTIFY_LISTENER_END);

	  }

	  public virtual void endCompensation()
	  {
		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.FIRE_ACTIVITY_END);
		remove();

		PvmExecutionImpl parent = Parent;

		if (parent.getActivity() == null)
		{
		  parent.setActivity((PvmActivity) getActivity().FlowScope);
		}

		parent.signal(SIGNAL_COMPENSATION_DONE, null);
	  }

	  /// <summary>
	  /// <para>Precondition: execution is already ended but this has not been propagated yet.</para>
	  /// <para>
	  /// </para>
	  /// <para>Propagates the ending of this execution to the flowscope execution; currently only supports
	  /// the process instance execution</para>
	  /// </summary>
	  public virtual void propagateEnd()
	  {
		if (!Ended)
		{
		  throw new ProcessEngineException(ToString() + " must have ended before ending can be propagated");
		}

		if (ProcessInstanceExecution)
		{
		  performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_END);
		}
		else
		{
		  // not supported yet
		}
	  }

	  public virtual void remove()
	  {
		PvmExecutionImpl parent = Parent;
		if (parent != null)
		{
		  parent.Executions.Remove(this);

		  // if the sequence counter is greater than the
		  // sequence counter of the parent, then set
		  // the greater sequence counter on the parent.
		  long parentSequenceCounter = parent.SequenceCounter;
		  long mySequenceCounter = SequenceCounter;
		  if (mySequenceCounter > parentSequenceCounter)
		  {
			parent.SequenceCounter = mySequenceCounter;
		  }

		  // propagate skipping configuration upwards, if it was not initially set on
		  // the root execution
		  parent.skipCustomListeners |= this.skipCustomListeners;
		  parent.skipIoMapping |= this.skipIoMapping;

		}

		isActive_Conflict = false;
		isEnded = true;

		if (hasReplacedParent())
		{
		  Parent.replacedBy = null;
		}

		removeEventScopes();
	  }

	  public virtual PvmExecutionImpl createConcurrentExecution()
	  {
		if (!Scope)
		{
		  throw new ProcessEngineException("Cannot create concurrent execution for " + this);
		}

		// The following covers the three cases in which a concurrent execution may be created
		// (this execution is the root in each scenario).
		//
		// Note: this should only consider non-event-scope executions. Event-scope executions
		// are not relevant for the tree structure and should remain under their original parent.
		//
		//
		// (1) A compacted tree:
		//
		// Before:               After:
		//       -------               -------
		//       |  e1  |              |  e1 |
		//       -------               -------
		//                             /     \
		//                         -------  -------
		//                         |  e2 |  |  e3 |
		//                         -------  -------
		//
		// e2 replaces e1; e3 is the new root for the activity stack to instantiate
		//
		//
		// (2) A single child that is a scope execution
		// Before:               After:
		//       -------               -------
		//       |  e1 |               |  e1 |
		//       -------               -------
		//          |                  /     \
		//       -------           -------  -------
		//       |  e2 |           |  e3 |  |  e4 |
		//       -------           -------  -------
		//                            |
		//                         -------
		//                         |  e2 |
		//                         -------
		//
		//
		// e3 is created and is concurrent;
		// e4 is the new root for the activity stack to instantiate
		//
		// (3) Existing concurrent execution(s)
		// Before:               After:
		//       -------                    ---------
		//       |  e1 |                    |   e1  |
		//       -------                    ---------
		//       /     \                   /    |    \
		//  -------    -------      -------  -------  -------
		//  |  e2 | .. |  eX |      |  e2 |..|  eX |  | eX+1|
		//  -------    -------      -------  -------  -------
		//
		// eX+1 is concurrent and the new root for the activity stack to instantiate
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<? extends PvmExecutionImpl> children = this.getNonEventScopeExecutions();
		IList<PvmExecutionImpl> children = this.NonEventScopeExecutions;

		// whenever we change the set of child executions we have to force an update
		// on the scope executions to avoid concurrent modifications (e.g. tree compaction)
		// that go unnoticed
		forceUpdate();

		if (children.Count == 0)
		{
		  // (1)
		  PvmExecutionImpl replacingExecution = this.createExecution();
		  replacingExecution.Concurrent = true;
		  replacingExecution.Scope = false;
		  replacingExecution.replace(this);
		  this.inactivate();
		  this.setActivity(null);

		}
		else if (children.Count == 1)
		{
		  // (2)
		  PvmExecutionImpl child = children[0];

		  PvmExecutionImpl concurrentReplacingExecution = this.createExecution();
		  concurrentReplacingExecution.Concurrent = true;
		  concurrentReplacingExecution.Scope = false;
		  concurrentReplacingExecution.Active = false;
		  concurrentReplacingExecution.onConcurrentExpand(this);
		  child.Parent = concurrentReplacingExecution;
		  this.leaveActivityInstance();
		  this.setActivity(null);
		}

		// (1), (2), and (3)
		PvmExecutionImpl concurrentExecution = this.createExecution();
		concurrentExecution.Concurrent = true;
		concurrentExecution.Scope = false;

		return concurrentExecution;
	  }

	  public virtual bool tryPruneLastConcurrentChild()
	  {
		if (NonEventScopeExecutions.Count == 1)
		{
		  PvmExecutionImpl lastConcurrent = NonEventScopeExecutions[0];
		  if (lastConcurrent.Concurrent)
		  {
			if (!lastConcurrent.Scope)
			{
			  setActivity(lastConcurrent.getActivity());
			  setTransition(lastConcurrent.getTransition());
			  this.replace(lastConcurrent);

			  // Move children of lastConcurrent one level up
			  if (lastConcurrent.hasChildren())
			  {
				foreach (PvmExecutionImpl childExecution in lastConcurrent.ExecutionsAsCopy)
				{
				  childExecution.Parent = this;
				}
			  }

			  // Make sure parent execution is re-activated when the last concurrent
			  // child execution is active
			  if (!Active && lastConcurrent.Active)
			  {
				Active = true;
			  }

			  lastConcurrent.remove();
			}
			else
			{
			  // legacy behavior
			  LegacyBehavior.pruneConcurrentScope(lastConcurrent);
			}
			return true;
		  }
		}

		return false;

	  }

	  public virtual void deleteCascade(string deleteReason)
	  {
		deleteCascade(deleteReason, false);
	  }

	  public virtual void deleteCascade(string deleteReason, bool skipCustomListeners)
	  {
		deleteCascade(deleteReason, skipCustomListeners, false);
	  }

	  public virtual void deleteCascade(string deleteReason, bool skipCustomListeners, bool skipIoMappings)
	  {
		deleteCascade(deleteReason, skipCustomListeners, skipIoMappings, false, false);
	  }

	  public virtual void deleteCascade(string deleteReason, bool skipCustomListeners, bool skipIoMappings, bool externallyTerminated, bool skipSubprocesses)
	  {
		this.deleteReason = deleteReason;
		DeleteRoot = true;
		this.isEnded = true;
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMapping = skipIoMappings;
		this.externallyTerminated = externallyTerminated;
		this.skipSubprocesses = skipSubprocesses;
		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE);
	  }

	  public virtual void executeEventHandlerActivity(ActivityImpl eventHandlerActivity)
	  {

		// the target scope
		ScopeImpl flowScope = eventHandlerActivity.FlowScope;

		// the event scope (the current activity)
		ScopeImpl eventScope = eventHandlerActivity.EventScope;

		if (eventHandlerActivity.ActivityStartBehavior == ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE && flowScope != eventScope)
		{
		  // the current scope is the event scope of the activity
		  findExecutionForScope(eventScope, flowScope).executeActivity(eventHandlerActivity);
		}
		else
		{
		  executeActivity(eventHandlerActivity);
		}
	  }

	  // tree compaction & expansion ///////////////////////////////////////////

	  /// <summary>
	  /// <para>Returns an execution that has replaced this execution for executing activities in their shared scope.</para>
	  /// <para>Invariant: this execution and getReplacedBy() execute in the same scope.</para>
	  /// </summary>
	  public abstract PvmExecutionImpl ReplacedBy {get;}

	  /// <summary>
	  /// Instead of <seealso cref="getReplacedBy()"/>, which returns the execution that this execution was directly replaced with,
	  /// this resolves the chain of replacements (i.e. in the case the replacedBy execution itself was replaced again)
	  /// </summary>
	  public virtual PvmExecutionImpl resolveReplacedBy()
	  {
		// follow the links of execution replacement;
		// note: this can be at most two hops:
		// case 1:
		//   this execution is a scope execution
		//     => tree may have expanded meanwhile
		//     => scope execution references replacing execution directly (one hop)
		//
		// case 2:
		//   this execution is a concurrent execution
		//     => tree may have compacted meanwhile
		//     => concurrent execution references scope execution directly (one hop)
		//
		// case 3:
		//   this execution is a concurrent execution
		//     => tree may have compacted/expanded/compacted/../expanded any number of times
		//     => the concurrent execution has been removed and therefore references the scope execution (first hop)
		//     => the scope execution may have been replaced itself again with another concurrent execution (second hop)
		//   note that the scope execution may have a long "history" of replacements, but only the last replacement is relevant here
		PvmExecutionImpl replacingExecution = ReplacedBy;

		if (replacingExecution != null)
		{
		  PvmExecutionImpl secondHopReplacingExecution = replacingExecution.ReplacedBy;
		  if (secondHopReplacingExecution != null)
		  {
			replacingExecution = secondHopReplacingExecution;
		  }
		}

		return replacingExecution;
	  }

	  public virtual bool hasReplacedParent()
	  {
		return Parent != null && Parent.ReplacedBy == this;
	  }

	  public virtual bool ReplacedByParent
	  {
		  get
		  {
			return ReplacedBy != null && ReplacedBy == this.Parent;
		  }
	  }

	  /// <summary>
	  /// <para>Replace an execution by this execution. The replaced execution has a pointer (<seealso cref="getReplacedBy()"/>) to this execution.
	  /// This pointer is maintained until the replaced execution is removed or this execution is removed/ended.</para>
	  /// <para>
	  /// </para>
	  /// <para>This is used for two cases: Execution tree expansion and execution tree compaction</para>
	  /// <ul>
	  /// <li><b>expansion</b>: Before:
	  /// <pre>
	  ///       -------
	  ///       |  e1 |  scope
	  ///       -------
	  ///     </pre>
	  /// After:
	  /// <pre>
	  ///       -------
	  ///       |  e1 |  scope
	  ///       -------
	  ///          |
	  ///       -------
	  ///       |  e2 |  cc (no scope)
	  ///       -------
	  ///     </pre>
	  /// e2 replaces e1: it should receive all entities associated with the activity currently executed
	  /// by e1; these are tasks, (local) variables, jobs (specific for the activity, not the scope)
	  /// </li>
	  /// <li><b>compaction</b>: Before:
	  /// <pre>
	  ///       -------
	  ///       |  e1 |  scope
	  ///       -------
	  ///          |
	  ///       -------
	  ///       |  e2 |  cc (no scope)
	  ///       -------
	  ///     </pre>
	  /// After:
	  /// <pre>
	  ///       -------
	  ///       |  e1 |  scope
	  ///       -------
	  ///     </pre>
	  /// e1 replaces e2: it should receive all entities associated with the activity currently executed
	  /// by e2; these are tasks, (all) variables, all jobs
	  /// </li>
	  /// </ul>
	  /// </summary>
	  /// <seealso cref= #createConcurrentExecution() </seealso>
	  /// <seealso cref= #tryPruneLastConcurrentChild() </seealso>
	  public virtual void replace(PvmExecutionImpl execution)
	  {
		// activity instance id handling
		this.activityInstanceId = execution.ActivityInstanceId;
		this.isActive_Conflict = execution.isActive_Conflict;

		this.replacedBy = null;
		execution.replacedBy = this;

		this.transitionsToTake = execution.transitionsToTake;

		execution.leaveActivityInstance();
	  }

	  /// <summary>
	  /// Callback on tree expansion when this execution is used as the concurrent execution
	  /// where the argument's children become a subordinate to. Note that this case is not the inverse
	  /// of replace because replace has the semantics that the replacing execution can be used to continue
	  /// execution of this execution's activity instance.
	  /// </summary>
	  public virtual void onConcurrentExpand(PvmExecutionImpl scopeExecution)
	  {
		// by default, do nothing
	  }

	  // methods that translate to operations /////////////////////////////////////

	  public virtual void signal(string signalName, object signalData)
	  {
		if (getActivity() == null)
		{
		  throw new PvmException("cannot signal execution " + this.id + ": it has no current activity");
		}

		SignallableActivityBehavior activityBehavior = (SignallableActivityBehavior) activity.ActivityBehavior;
		try
		{
		  activityBehavior.signal(this, signalName, signalData);
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new PvmException("couldn't process signal '" + signalName + "' on activity '" + activity.Id + "': " + e.Message, e);
		}
	  }

	  public virtual void take()
	  {
		if (this.transition == null)
		{
		  throw new PvmException(ToString() + ": no transition to take specified");
		}
		TransitionImpl transitionImpl = transition;
		setActivity(transitionImpl.Source);
		// while executing the transition, the activityInstance is 'null'
		// (we are not executing an activity)
		ActivityInstanceId = null;
		Active = true;
		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE);
	  }

	  /// <summary>
	  /// Execute an activity which is not contained in normal flow (having no incoming sequence flows).
	  /// Cannot be called for activities contained in normal flow.
	  /// <para>
	  /// First, the ActivityStartBehavior is evaluated.
	  /// In case the start behavior is not <seealso cref="ActivityStartBehavior.DEFAULT"/>, the corresponding start
	  /// behavior is executed before executing the activity.
	  /// </para>
	  /// <para>
	  /// For a given activity, the execution on which this method must be called depends on the type of the start behavior:
	  /// <ul>
	  /// <li>CONCURRENT_IN_FLOW_SCOPE: scope execution for <seealso cref="PvmActivity.getFlowScope()"/></li>
	  /// <li>INTERRUPT_EVENT_SCOPE: scope execution for <seealso cref="PvmActivity.getEventScope()"/></li>
	  /// <li>CANCEL_EVENT_SCOPE: scope execution for <seealso cref="PvmActivity.getEventScope()"/></li>
	  /// </ul>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="activity"> the activity to start </param>
	  public virtual void executeActivity(PvmActivity activity)
	  {
		if (activity.IncomingTransitions.Count > 0)
		{
		  throw new ProcessEngineException("Activity is contained in normal flow and cannot be executed using executeActivity().");
		}

		ActivityStartBehavior activityStartBehavior = activity.ActivityStartBehavior;
		if (!Scope && ActivityStartBehavior.DEFAULT != activityStartBehavior)
		{
		  throw new ProcessEngineException("Activity '" + activity + "' with start behavior '" + activityStartBehavior + "'" + "cannot be executed by non-scope execution.");
		}

		PvmActivity activityImpl = activity;
		this.isEnded = false;
		this.isActive_Conflict = true;

		switch (activityStartBehavior)
		{
		  case org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE:
			this.nextActivity = activityImpl;
			performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CONCURRENT);
			break;

		  case org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior.CANCEL_EVENT_SCOPE:
			this.nextActivity = activityImpl;
			performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CANCEL_SCOPE);
			break;

		  case org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior.INTERRUPT_EVENT_SCOPE:
			this.nextActivity = activityImpl;
			performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_INTERRUPT_SCOPE);
			break;

		  default:
			setActivity(activityImpl);
			ActivityInstanceId = null;
			performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE);
			break;
		}
	  }

	  /// <summary>
	  /// Instantiates the given activity stack under this execution.
	  /// Sets the variables for the execution responsible to execute the most deeply nested
	  /// activity.
	  /// </summary>
	  /// <param name="activityStack"> The most deeply nested activity is the last element in the list </param>
	  public virtual void executeActivitiesConcurrent(IList<PvmActivity> activityStack, PvmActivity targetActivity, PvmTransition targetTransition, IDictionary<string, object> variables, IDictionary<string, object> localVariables, bool skipCustomListeners, bool skipIoMappings)
	  {


		ScopeImpl flowScope = null;
		if (activityStack.Count > 0)
		{
		  flowScope = activityStack[0].FlowScope;
		}
		else if (targetActivity != null)
		{
		  flowScope = targetActivity.FlowScope;
		}
		else if (targetTransition != null)
		{
		  flowScope = targetTransition.Source.FlowScope;
		}

		PvmExecutionImpl propagatingExecution = null;
		if (flowScope.ActivityBehavior is ModificationObserverBehavior)
		{
		  ModificationObserverBehavior flowScopeBehavior = (ModificationObserverBehavior) flowScope.ActivityBehavior;
		  propagatingExecution = (PvmExecutionImpl) flowScopeBehavior.createInnerInstance(this);
		}
		else
		{
		  propagatingExecution = createConcurrentExecution();
		}

		propagatingExecution.executeActivities(activityStack, targetActivity, targetTransition, variables, localVariables, skipCustomListeners, skipIoMappings);
	  }

	  /// <summary>
	  /// Instantiates the given set of activities and returns the execution for the bottom-most activity
	  /// </summary>
	  public virtual IDictionary<PvmActivity, PvmExecutionImpl> instantiateScopes(IList<PvmActivity> activityStack, bool skipCustomListeners, bool skipIoMappings)
	  {

		if (activityStack.Count == 0)
		{
		  return java.util.Collections.emptyMap();
		}

		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMapping = skipIoMappings;

		ExecutionStartContext executionStartContext = new ExecutionStartContext(false);

		InstantiationStack instantiationStack = new InstantiationStack(new LinkedList<>(activityStack));
		executionStartContext.InstantiationStack = instantiationStack;
		StartContext = executionStartContext;

		performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_INIT_STACK_AND_RETURN);

		IDictionary<PvmActivity, PvmExecutionImpl> createdExecutions = new Dictionary<PvmActivity, PvmExecutionImpl>();

		PvmExecutionImpl currentExecution = this;
		foreach (PvmActivity instantiatedActivity in activityStack)
		{
		  // there must exactly one child execution
		  currentExecution = currentExecution.NonEventScopeExecutions[0];
		  if (currentExecution.Concurrent)
		  {
			// there may be a non-scope execution that we have to skip (e.g. multi-instance)
			currentExecution = currentExecution.NonEventScopeExecutions[0];
		  }

		  createdExecutions[instantiatedActivity] = currentExecution;
		}

		return createdExecutions;
	  }

	  /// <summary>
	  /// Instantiates the given activity stack. Uses this execution to execute the
	  /// highest activity in the stack.
	  /// Sets the variables for the execution responsible to execute the most deeply nested
	  /// activity.
	  /// </summary>
	  /// <param name="activityStack"> The most deeply nested activity is the last element in the list </param>
	  public virtual void executeActivities(IList<PvmActivity> activityStack, PvmActivity targetActivity, PvmTransition targetTransition, IDictionary<string, object> variables, IDictionary<string, object> localVariables, bool skipCustomListeners, bool skipIoMappings)
	  {


		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMapping = skipIoMappings;
		this.activityInstanceId = null;
		this.isEnded = false;

		if (activityStack.Count > 0)
		{
		  ExecutionStartContext executionStartContext = new ExecutionStartContext(false);

		  InstantiationStack instantiationStack = new InstantiationStack(activityStack, targetActivity, targetTransition);
		  executionStartContext.InstantiationStack = instantiationStack;
		  executionStartContext.Variables = variables;
		  executionStartContext.VariablesLocal = localVariables;
		  StartContext = executionStartContext;

		  performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_INIT_STACK);

		}
		else if (targetActivity != null)
		{
		  Variables = variables;
		  VariablesLocal = localVariables;
		  setActivity(targetActivity);
		  performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE);

		}
		else if (targetTransition != null)
		{
		  Variables = variables;
		  VariablesLocal = localVariables;
		  setActivity(targetTransition.Source);
		  setTransition(targetTransition);
		  performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_START_NOTIFY_LISTENER_TAKE);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({"rawtypes", "unchecked"}) public List<org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity)
	  public virtual IList<ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity)
	  {
		IList<PvmExecutionImpl> inactiveConcurrentExecutionsInActivity = new List<PvmExecutionImpl>();
		if (Concurrent)
		{
		  return Parent.findInactiveChildExecutions(activity);
		}
		else if (!Active)
		{
		  inactiveConcurrentExecutionsInActivity.Add(this);
		}

		return (System.Collections.IList) inactiveConcurrentExecutionsInActivity;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({"rawtypes", "unchecked"}) public List<org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution> findInactiveChildExecutions(PvmActivity activity)
	  public virtual IList<ActivityExecution> findInactiveChildExecutions(PvmActivity activity)
	  {
		IList<PvmExecutionImpl> inactiveConcurrentExecutionsInActivity = new List<PvmExecutionImpl>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<? extends PvmExecutionImpl> concurrentExecutions = getAllChildExecutions();
		IList<PvmExecutionImpl> concurrentExecutions = AllChildExecutions;
		foreach (PvmExecutionImpl concurrentExecution in concurrentExecutions)
		{
		  if (concurrentExecution.getActivity() == activity && !concurrentExecution.Active)
		  {
			inactiveConcurrentExecutionsInActivity.Add(concurrentExecution);
		  }
		}

		return (System.Collections.IList) inactiveConcurrentExecutionsInActivity;
	  }

	  protected internal virtual IList<PvmExecutionImpl> AllChildExecutions
	  {
		  get
		  {
			IList<PvmExecutionImpl> childExecutions = new List<PvmExecutionImpl>();
			foreach (PvmExecutionImpl childExecution in Executions)
			{
			  childExecutions.Add(childExecution);
			  ((IList<PvmExecutionImpl>)childExecutions).AddRange(childExecution.AllChildExecutions);
			}
			return childExecutions;
		  }
	  }

	  public virtual void leaveActivityViaTransition(PvmTransition outgoingTransition)
	  {
		leaveActivityViaTransitions(new IList<PvmTransition> {outgoingTransition}, System.Linq.Enumerable.Empty<ActivityExecution>());
	  }

	  public virtual void leaveActivityViaTransitions<T1>(IList<PvmTransition> _transitions, IList<T1> _recyclableExecutions) where T1 : org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<? extends org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution> recyclableExecutions = Collections.emptyList();
		IList<ActivityExecution> recyclableExecutions = java.util.Collections.emptyList();
		if (_recyclableExecutions != null)
		{
		  recyclableExecutions = new List<>(_recyclableExecutions);
		}

		// if recyclable executions size is greater
		// than 1, then the executions are joined and
		// the activity is left with 'this' execution,
		// if it is not not the last concurrent execution.
		// therefore it is necessary to remove the local
		// variables (event if it is the last concurrent
		// execution).
		if (recyclableExecutions.Count > 1)
		{
		  removeVariablesLocalInternal();
		}

		// mark all recyclable executions as ended
		// if the list of recyclable executions also
		// contains 'this' execution, then 'this' execution
		// is also marked as ended. (if 'this' execution is
		// pruned, then the local variables are not copied
		// to the parent execution)
		// this is a workaround to not delete all recyclable
		// executions and create a new execution which leaves
		// the activity.
		foreach (ActivityExecution execution in recyclableExecutions)
		{
		  execution.Ended = true;
		}

		// remove 'this' from recyclable executions to
		// leave the activity with 'this' execution
		// (when 'this' execution is the last concurrent
		// execution, then 'this' execution will be pruned,
		// and the activity is left with the scope
		// execution)
		recyclableExecutions.Remove(this);
		foreach (ActivityExecution execution in recyclableExecutions)
		{
		  execution.end(_transitions.Count == 0);
		}

		PvmExecutionImpl propagatingExecution = this;
		if (ReplacedBy != null)
		{
		  propagatingExecution = ReplacedBy;
		}

		propagatingExecution.isActive_Conflict = true;
		propagatingExecution.isEnded = false;

		if (_transitions.Count == 0)
		{
		  propagatingExecution.end(!propagatingExecution.Concurrent);
		}
		else
		{
		  propagatingExecution.TransitionsToTake = _transitions;
		  propagatingExecution.performOperation(org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END);
		}
	  }

	  protected internal abstract void removeVariablesLocalInternal();

	  public virtual bool isActive(string activityId)
	  {
		return findExecution(activityId) != null;
	  }

	  public virtual void inactivate()
	  {
		this.isActive_Conflict = false;
	  }

	  // executions ///////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override public abstract List<? extends PvmExecutionImpl> getExecutions();
	  public override abstract IList<PvmExecutionImpl> Executions {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract List<? extends PvmExecutionImpl> getExecutionsAsCopy();
	  public abstract IList<PvmExecutionImpl> ExecutionsAsCopy {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public List<? extends PvmExecutionImpl> getNonEventScopeExecutions()
	  public virtual IList<PvmExecutionImpl> NonEventScopeExecutions
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: List<? extends PvmExecutionImpl> children = getExecutions();
			IList<PvmExecutionImpl> children = Executions;
			IList<PvmExecutionImpl> result = new List<PvmExecutionImpl>();
    
			foreach (PvmExecutionImpl child in children)
			{
			  if (!child.EventScope)
			  {
				result.Add(child);
			  }
			}
    
			return result;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public List<? extends PvmExecutionImpl> getEventScopeExecutions()
	  public virtual IList<PvmExecutionImpl> EventScopeExecutions
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: List<? extends PvmExecutionImpl> children = getExecutions();
			IList<PvmExecutionImpl> children = Executions;
			IList<PvmExecutionImpl> result = new List<PvmExecutionImpl>();
    
			foreach (PvmExecutionImpl child in children)
			{
			  if (child.EventScope)
			  {
				result.Add(child);
			  }
			}
    
			return result;
		  }
	  }

	  public virtual PvmExecutionImpl findExecution(string activityId)
	  {
		if ((getActivity() != null) && (getActivity().Id.Equals(activityId)))
		{
		  return this;
		}
		foreach (PvmExecutionImpl nestedExecution in Executions)
		{
		  PvmExecutionImpl result = nestedExecution.findExecution(activityId);
		  if (result != null)
		  {
			return result;
		  }
		}
		return null;
	  }

	  public virtual IList<PvmExecution> findExecutions(string activityId)
	  {
		IList<PvmExecution> matchingExecutions = new List<PvmExecution>();
		collectExecutions(activityId, matchingExecutions);

		return matchingExecutions;
	  }

	  protected internal virtual void collectExecutions(string activityId, IList<PvmExecution> executions)
	  {
		if ((getActivity() != null) && (getActivity().Id.Equals(activityId)))
		{
		  executions.Add(this);
		}

		foreach (PvmExecutionImpl nestedExecution in Executions)
		{
		  nestedExecution.collectExecutions(activityId, executions);
		}
	  }

	  public virtual IList<string> findActiveActivityIds()
	  {
		IList<string> activeActivityIds = new List<string>();
		collectActiveActivityIds(activeActivityIds);
		return activeActivityIds;
	  }

	  protected internal virtual void collectActiveActivityIds(IList<string> activeActivityIds)
	  {
		ActivityImpl activity = getActivity();
		if (isActive_Conflict && activity != null)
		{
		  activeActivityIds.Add(activity.Id);
		}

		foreach (PvmExecutionImpl execution in Executions)
		{
		  execution.collectActiveActivityIds(activeActivityIds);
		}
	  }

	  // business key /////////////////////////////////////////

	  public virtual string ProcessBusinessKey
	  {
		  get
		  {
			return ProcessInstance.BusinessKey;
		  }
		  set
		  {
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final PvmExecutionImpl processInstance = getProcessInstance();
			PvmExecutionImpl processInstance = ProcessInstance;
			processInstance.BusinessKey = value;
    
			HistoryLevel historyLevel = Context.CommandContext.ProcessEngineConfiguration.HistoryLevel;
			if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_UPDATE, processInstance))
			{
    
			  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, processInstance));
			}
		  }
	  }


	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl processInstance;

		  public HistoryEventCreatorAnonymousInnerClass(PvmExecutionImpl outerInstance, org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl processInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstance = processInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createProcessInstanceUpdateEvt(processInstance);
		  }
	  }

	  public override string BusinessKey
	  {
		  get
		  {
			if (this.ProcessInstanceExecution)
			{
			  return businessKey;
			}
			else
			{
				return ProcessBusinessKey;
			}
		  }
	  }

	  // process definition ///////////////////////////////////////////////////////

	  public virtual ProcessDefinitionImpl ProcessDefinition
	  {
		  set
		  {
			this.processDefinition = value;
		  }
		  get
		  {
			return processDefinition;
		  }
	  }


	  // process instance /////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process instance.
	  /// </summary>
	  public override abstract PvmExecutionImpl ProcessInstance {get;set;}


	  // case instance id /////////////////////////////////////////////////////////

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  // activity /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the activity
	  /// </summary>
	  public virtual ActivityImpl getActivity()
	  {
		return activity;
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			ActivityImpl activity = getActivity();
			if (activity != null)
			{
			  return activity.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string CurrentActivityName
	  {
		  get
		  {
			ActivityImpl activity = getActivity();
			if (activity != null)
			{
			  return activity.Name;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string CurrentActivityId
	  {
		  get
		  {
			return ActivityId;
		  }
	  }

	  public virtual void setActivity(PvmActivity activity)
	  {
		this.activity = (ActivityImpl) activity;
	  }

	  public virtual void enterActivityInstance()
	  {
		ActivityImpl activity = getActivity();
		activityInstanceId = generateActivityInstanceId(activity.Id);

		LOG.debugEnterActivityInstance(this, ParentActivityInstanceId);

		// <LEGACY>: in general, io mappings may only exist when the activity is scope
		// however, for multi instance activities, the inner activity does not become a scope
		// due to the presence of an io mapping. In that case, it is ok to execute the io mapping
		// anyway because the multi-instance body already ensures variable isolation
		executeIoMapping();

		if (activity.Scope)
		{
		  initializeTimerDeclarations();
		}

		activityInstanceEndListenersFailed = false;

	  }

	  public virtual void activityInstanceStarting()
	  {
		this.activityInstanceState = ActivityInstanceState_Fields.STARTING.StateCode;
	  }


	  public virtual void activityInstanceStarted()
	  {
		this.activityInstanceState = ActivityInstanceState_Fields.DEFAULT.StateCode;
	  }

	  public virtual void activityInstanceDone()
	  {
		this.activityInstanceState = ENDING.StateCode;
	  }

	  public virtual void activityInstanceEndListenerFailure()
	  {
		this.activityInstanceEndListenersFailed = true;
	  }

	  protected internal abstract string generateActivityInstanceId(string activityId);

	  public virtual void leaveActivityInstance()
	  {
		if (!string.ReferenceEquals(activityInstanceId, null))
		{
		  LOG.debugLeavesActivityInstance(this, activityInstanceId);
		}
		activityInstanceId = ParentActivityInstanceId;

		activityInstanceState = ActivityInstanceState_Fields.DEFAULT.StateCode;
		activityInstanceEndListenersFailed = false;
	  }

	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			if (ProcessInstanceExecution)
			{
			  return Id;
    
			}
			else
			{
			  return Parent.ActivityInstanceId;
			}
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  set
		  {
			this.activityInstanceId = value;
		  }
		  get
		  {
			return activityInstanceId;
		  }
	  }


	  // parent ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the parent
	  /// </summary>
	  public override abstract PvmExecutionImpl Parent {get;}

	  public virtual string ParentId
	  {
		  get
		  {
			PvmExecutionImpl parent = Parent;
			if (parent != null)
			{
			  return parent.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual bool hasChildren()
	  {
		return Executions.Count > 0;
	  }

	  /// <summary>
	  /// Sets the execution's parent and updates the old and new parents' set of
	  /// child executions
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void setParent(PvmExecutionImpl parent)
	  public virtual PvmExecutionImpl Parent
	  {
		  set
		  {
			PvmExecutionImpl currentParent = Parent;
    
			ParentExecution = value;
    
			if (currentParent != null)
			{
			  currentParent.Executions.Remove(this);
			}
    
			if (value != null)
			{
			  ((IList<PvmExecutionImpl>) value.Executions).Add(this);
			}
		  }
	  }

	  /// <summary>
	  /// Use #setParent to also update the child execution sets
	  /// </summary>
	  public abstract PvmExecutionImpl ParentExecution {set;}

	  // super- and subprocess executions /////////////////////////////////////////

	  public override abstract PvmExecutionImpl SuperExecution {get;set;}


	  public abstract PvmExecutionImpl SubProcessInstance {get;set;}


	  // super case execution /////////////////////////////////////////////////////

	  public abstract CmmnExecution SuperCaseExecution {get;set;}


	  // sub case execution ///////////////////////////////////////////////////////

	  public abstract CmmnExecution SubCaseInstance {get;set;}


	  // scopes ///////////////////////////////////////////////////////////////////

	  protected internal virtual ScopeImpl ScopeActivity
	  {
		  get
		  {
			ScopeImpl scope = null;
			// this if condition is important during process instance startup
			// where the activity of the process instance execution may not be aligned
			// with the execution tree
			if (ProcessInstanceExecution)
			{
			  scope = ProcessDefinition;
			}
			else
			{
			  scope = getActivity();
			}
			return scope;
		  }
	  }

	  public virtual bool Scope
	  {
		  get
		  {
			return isScope;
		  }
		  set
		  {
			this.isScope = value;
		  }
	  }



	  /// <summary>
	  /// For a given target flow scope, this method returns the corresponding scope execution.
	  /// <para>
	  /// Precondition: the execution is active and executing an activity.
	  /// Can be invoked for scope and non scope executions.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="targetFlowScope"> scope activity or process definition for which the scope execution should be found </param>
	  /// <returns> the scope execution for the provided targetFlowScope </returns>
	  public virtual PvmExecutionImpl findExecutionForFlowScope(PvmScope targetFlowScope)
	  {
		// if this execution is not a scope execution, use the parent
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PvmExecutionImpl scopeExecution = isScope() ? this : getParent();
		PvmExecutionImpl scopeExecution = Scope ? this : Parent;

		ScopeImpl currentActivity = getActivity();
		EnsureUtil.ensureNotNull("activity of current execution", currentActivity);

		// if this is a scope execution currently executing a non scope activity
		currentActivity = currentActivity.Scope ? currentActivity : currentActivity.FlowScope;

		return scopeExecution.findExecutionForScope(currentActivity, (ScopeImpl) targetFlowScope);
	  }


	  public virtual PvmExecutionImpl findExecutionForScope(ScopeImpl currentScope, ScopeImpl targetScope)
	  {

		if (!targetScope.Scope)
		{
		  throw new ProcessEngineException("Target scope must be a scope.");
		}

		IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = createActivityExecutionMapping(currentScope);
		PvmExecutionImpl scopeExecution = activityExecutionMapping[targetScope];
		if (scopeExecution == null)
		{
		  // the target scope is scope but no corresponding execution was found
		  // => legacy behavior
		  scopeExecution = LegacyBehavior.getScopeExecution(targetScope, activityExecutionMapping);
		}
		return scopeExecution;
	  }

	  public virtual IDictionary<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping(ScopeImpl currentScope)
	  {
		if (!Scope)
		{
		  throw new ProcessEngineException("Execution must be a scope execution");
		}
		if (!currentScope.Scope)
		{
		  throw new ProcessEngineException("Current scope must be a scope.");
		}

		// A single path in the execution tree from a leaf (no child executions) to the root
		// may in fact contain multiple executions that correspond to leaves in the activity instance hierarchy.
		//
		// This is because compensation throwing executions have child executions. In that case, the
		// flow scope hierarchy is not aligned with the scope execution hierarchy: There is a scope
		// execution for a compensation-throwing event that is an ancestor of this execution,
		// while these events are not ancestor scopes of currentScope.
		//
		// The strategy to deal with this situation is as follows:
		// 1. Determine all executions that correspond to leaf activity instances
		// 2. Order the leaf executions in top-to-bottom fashion
		// 3. Iteratively build the activity execution mapping based on the leaves in top-to-bottom order
		//    3.1. For the first leaf, create the activity execution mapping regularly
		//    3.2. For every following leaf, rebuild the mapping but reuse any scopes and scope executions
		//         that are part of the mapping created in the previous iteration
		//
		// This process ensures that the resulting mapping does not contain scopes that are not ancestors
		// of currentScope and that it does not contain scope executions for such scopes.
		// For any execution hierarchy that does not involve compensation, the number of iterations in step 3
		// should be 1, i.e. there are no other leaf activity instance executions in the hierarchy.

		// 1. Find leaf activity instance executions
		LeafActivityInstanceExecutionCollector leafCollector = new LeafActivityInstanceExecutionCollector();
		(new ExecutionWalker(this)).addPreVisitor(leafCollector).walkUntil();

		IList<PvmExecutionImpl> leaves = leafCollector.Leaves;
		leaves.Remove(this);

		// 2. Order them from top to bottom
		leaves.Reverse();

		// 3. Iteratively extend the mapping for every additional leaf
		IDictionary<ScopeImpl, PvmExecutionImpl> mapping = new Dictionary<ScopeImpl, PvmExecutionImpl>();
		foreach (PvmExecutionImpl leaf in leaves)
		{
		  ScopeImpl leafFlowScope = leaf.FlowScope;
		  PvmExecutionImpl leafFlowScopeExecution = leaf.FlowScopeExecution;

		  mapping = leafFlowScopeExecution.createActivityExecutionMapping(leafFlowScope, mapping);
		}

		// finally extend the mapping for the current execution
		// (note that the current execution need not be a leaf itself)
		mapping = this.createActivityExecutionMapping(currentScope, mapping);

		return mapping;
	  }

	  public virtual IDictionary<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping()
	  {
		ScopeImpl currentActivity = getActivity();
		EnsureUtil.ensureNotNull("activity of current execution", currentActivity);

		ScopeImpl flowScope = FlowScope;
		PvmExecutionImpl flowScopeExecution = FlowScopeExecution;

		return flowScopeExecution.createActivityExecutionMapping(flowScope);
	  }

	  protected internal virtual PvmExecutionImpl FlowScopeExecution
	  {
		  get
		  {
			if (!isScope || CompensationBehavior.executesNonScopeCompensationHandler(this))
			{
			  // LEGACY: a correct implementation should also skip a compensation-throwing parent scope execution
			  // (since compensation throwing activities are scopes), but this cannot be done for backwards compatibility
			  // where a compensation throwing activity was no scope (and we would wrongly skip an execution in that case)
			  return Parent.FlowScopeExecution;
    
			}
			else
			{
			  return this;
			}
		  }
	  }

	  protected internal virtual ScopeImpl FlowScope
	  {
		  get
		  {
			ActivityImpl activity = getActivity();
    
			if (!activity.Scope || string.ReferenceEquals(activityInstanceId, null) || (activity.Scope && !Scope && activity.ActivityBehavior is CompositeActivityBehavior))
			{
			  // if
			  // - this is a scope execution currently executing a non scope activity
			  // - or it is not scope but the current activity is (e.g. can happen during activity end, when the actual
			  //   scope execution has been removed and the concurrent parent has been set to the scope activity)
			  // - or it is asyncBefore/asyncAfter
    
			  return activity.FlowScope;
			}
			else
			{
			  return activity;
			}
		  }
	  }

	  /// <summary>
	  /// Creates an extended mapping based on this execution and the given existing mapping.
	  /// Any entry <code>mapping</code> in mapping that corresponds to an ancestor scope of
	  /// <code>currentScope</code> is reused.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected Map<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping(ScopeImpl currentScope, final Map<ScopeImpl, PvmExecutionImpl> mapping)
	  protected internal virtual IDictionary<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping(ScopeImpl currentScope, IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
	  {
		if (!Scope)
		{
		  throw new ProcessEngineException("Execution must be a scope execution");
		}
		if (!currentScope.Scope)
		{
		  throw new ProcessEngineException("Current scope must be a scope.");
		}

		// collect all ancestor scope executions unless one is encountered that is already in "mapping"
		ScopeExecutionCollector scopeExecutionCollector = new ScopeExecutionCollector();
		(new ExecutionWalker(this)).addPreVisitor(scopeExecutionCollector).walkWhile(new WalkConditionAnonymousInnerClass(this, mapping));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<PvmExecutionImpl> scopeExecutions = scopeExecutionCollector.getScopeExecutions();
		IList<PvmExecutionImpl> scopeExecutions = scopeExecutionCollector.ScopeExecutions;

		// collect all ancestor scopes unless one is encountered that is already in "mapping"
		ScopeCollector scopeCollector = new ScopeCollector();
		(new FlowScopeWalker(currentScope)).addPreVisitor(scopeCollector).walkWhile(new WalkConditionAnonymousInnerClass2(this, mapping));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<ScopeImpl> scopes = scopeCollector.getScopes();
		IList<ScopeImpl> scopes = scopeCollector.Scopes;

		// add all ancestor scopes and scopeExecutions that are already in "mapping"
		// and correspond to ancestors of the topmost previously collected scope
		ScopeImpl topMostScope = scopes[scopes.Count - 1];
		(new FlowScopeWalker(topMostScope.FlowScope)).addPreVisitor(new TreeVisitorAnonymousInnerClass(this, mapping, scopeExecutions, scopes))
		 .walkWhile();

		if (scopes.Count == scopeExecutions.Count)
		{
		  // the trees are in sync
		  IDictionary<ScopeImpl, PvmExecutionImpl> result = new Dictionary<ScopeImpl, PvmExecutionImpl>();
		  for (int i = 0; i < scopes.Count; i++)
		  {
			result[scopes[i]] = scopeExecutions[i];
		  }
		  return result;
		}
		else
		{
		  // Wounderful! The trees are out of sync. This is due to legacy behavior
		  return LegacyBehavior.createActivityExecutionMapping(scopeExecutions, scopes);
		}
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<PvmExecutionImpl>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private IDictionary<ScopeImpl, PvmExecutionImpl> mapping;

		  public WalkConditionAnonymousInnerClass(PvmExecutionImpl outerInstance, IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
		  {
			  this.outerInstance = outerInstance;
			  this.mapping = mapping;
		  }

		  public bool isFulfilled(PvmExecutionImpl element)
		  {
			return element == null || mapping.ContainsValue(element);
		  }
	  }

	  private class WalkConditionAnonymousInnerClass2 : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private IDictionary<ScopeImpl, PvmExecutionImpl> mapping;

		  public WalkConditionAnonymousInnerClass2(PvmExecutionImpl outerInstance, IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
		  {
			  this.outerInstance = outerInstance;
			  this.mapping = mapping;
		  }

		  public bool isFulfilled(ScopeImpl element)
		  {
			return element == null || mapping.ContainsKey(element);
		  }
	  }

	  private class TreeVisitorAnonymousInnerClass : TreeVisitor<ScopeImpl>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private IDictionary<ScopeImpl, PvmExecutionImpl> mapping;
		  private IList<PvmExecutionImpl> scopeExecutions;
		  private IList<ScopeImpl> scopes;

		  public TreeVisitorAnonymousInnerClass(PvmExecutionImpl outerInstance, IDictionary<ScopeImpl, PvmExecutionImpl> mapping, IList<PvmExecutionImpl> scopeExecutions, IList<ScopeImpl> scopes)
		  {
			  this.outerInstance = outerInstance;
			  this.mapping = mapping;
			  this.scopeExecutions = scopeExecutions;
			  this.scopes = scopes;
		  }

		  public void visit(ScopeImpl obj)
		  {
			scopes.Add(obj);
			PvmExecutionImpl priorMappingExecution = mapping[obj];

			if (priorMappingExecution != null && !scopeExecutions.Contains(priorMappingExecution))
			{
			  scopeExecutions.Add(priorMappingExecution);
			}
		  }
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

	  protected internal virtual string ToStringIdentity
	  {
		  get
		  {
			return id;
		  }
	  }

	  // variables ////////////////////////////////////////////

	  public override string VariableScopeKey
	  {
		  get
		  {
			return "execution";
		  }
	  }

	  public override AbstractVariableScope ParentVariableScope
	  {
		  get
		  {
			return Parent;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
	  public virtual void setVariable(string variableName, object value, string targetActivityId)
	  {
		string activityId = ActivityId;
		if (!string.ReferenceEquals(activityId, null) && activityId.Equals(targetActivityId))
		{
		  setVariableLocal(variableName, value);
		}
		else
		{
		  PvmExecutionImpl executionForFlowScope = findExecutionForFlowScope(targetActivityId);
		  if (executionForFlowScope != null)
		  {
			executionForFlowScope.setVariableLocal(variableName, value);
		  }
		}
	  }

	  /// <param name="targetScopeId"> - destination scope to be found in current execution tree </param>
	  /// <returns> execution with activity id corresponding to targetScopeId </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected PvmExecutionImpl findExecutionForFlowScope(final String targetScopeId)
	  protected internal virtual PvmExecutionImpl findExecutionForFlowScope(string targetScopeId)
	  {
		EnsureUtil.ensureNotNull("target scope id", targetScopeId);

		ScopeImpl currentActivity = getActivity();
		EnsureUtil.ensureNotNull("activity of current execution", currentActivity);

		FlowScopeWalker walker = new FlowScopeWalker(currentActivity);
		ScopeImpl targetFlowScope = walker.walkUntil(new WalkConditionAnonymousInnerClass3(this, targetScopeId));

		if (targetFlowScope == null)
		{
		  throw LOG.scopeNotFoundException(targetScopeId, this.Id);
		}

		return findExecutionForFlowScope(targetFlowScope);
	  }

	  private class WalkConditionAnonymousInnerClass3 : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private string targetScopeId;

		  public WalkConditionAnonymousInnerClass3(PvmExecutionImpl outerInstance, string targetScopeId)
		  {
			  this.outerInstance = outerInstance;
			  this.targetScopeId = targetScopeId;
		  }


		  public bool isFulfilled(ScopeImpl scope)
		  {
			return scope == null || scope.Id.Equals(targetScopeId);
		  }

	  }


	  // sequence counter ///////////////////////////////////////////////////////////

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

	  // Getter / Setters ///////////////////////////////////


	  public virtual bool ExternallyTerminated
	  {
		  get
		  {
			return externallyTerminated;
		  }
		  set
		  {
			this.externallyTerminated = value;
		  }
	  }


	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
		  set
		  {
			this.deleteReason = value;
		  }
	  }


	  public virtual bool DeleteRoot
	  {
		  get
		  {
			return deleteRoot;
		  }
		  set
		  {
			this.deleteRoot = value;
		  }
	  }


	  public virtual TransitionImpl getTransition()
	  {
		return transition;
	  }

	  public virtual IList<PvmTransition> TransitionsToTake
	  {
		  get
		  {
			return transitionsToTake;
		  }
		  set
		  {
			this.transitionsToTake = value;
		  }
	  }


	  public virtual string CurrentTransitionId
	  {
		  get
		  {
			TransitionImpl transition = getTransition();
			if (transition != null)
			{
			  return transition.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual void setTransition(PvmTransition transition)
	  {
		this.transition = (TransitionImpl) transition;
	  }

	  public virtual bool Concurrent
	  {
		  get
		  {
			return isConcurrent;
		  }
		  set
		  {
			this.isConcurrent = value;
		  }
	  }


	  public virtual bool Active
	  {
		  get
		  {
			return isActive_Conflict;
		  }
		  set
		  {
			this.isActive_Conflict = value;
		  }
	  }


	  public virtual bool Ended
	  {
		  set
		  {
			this.isEnded = value;
		  }
		  get
		  {
			return isEnded;
		  }
	  }


	  public virtual bool Canceled
	  {
		  get
		  {
			return ActivityInstanceState_Fields.CANCELED.StateCode == activityInstanceState;
		  }
		  set
		  {
			if (value)
			{
			  activityInstanceState = ActivityInstanceState_Fields.CANCELED.StateCode;
			}
		  }
	  }


	  public virtual bool CompleteScope
	  {
		  get
		  {
			return ActivityInstanceState_Fields.SCOPE_COMPLETE.StateCode == activityInstanceState;
		  }
		  set
		  {
			if (value && !Canceled)
			{
			  activityInstanceState = ActivityInstanceState_Fields.SCOPE_COMPLETE.StateCode;
			}
		  }
	  }


	  public virtual bool PreserveScope
	  {
		  set
		  {
			this.preserveScope = value;
		  }
		  get
		  {
			return preserveScope;
		  }
	  }


	  public virtual int ActivityInstanceState
	  {
		  get
		  {
			return activityInstanceState;
		  }
	  }

	  public virtual bool isInState(ActivityInstanceState state)
	  {
		return activityInstanceState == state.StateCode;
	  }

	  public virtual bool hasFailedOnEndListeners()
	  {
		return activityInstanceEndListenersFailed;
	  }

	  public virtual bool EventScope
	  {
		  get
		  {
			return isEventScope;
		  }
		  set
		  {
			this.isEventScope = value;
		  }
	  }


	  public virtual ExecutionStartContext ExecutionStartContext
	  {
		  get
		  {
			return startContext;
		  }
	  }

	  public virtual void disposeProcessInstanceStartContext()
	  {
		startContext = null;
	  }

	  public virtual void disposeExecutionStartContext()
	  {
		startContext = null;
	  }

	  public virtual PvmActivity NextActivity
	  {
		  get
		  {
			return nextActivity;
		  }
		  set
		  {
			this.nextActivity = value;
		  }
	  }

	  public virtual bool ProcessInstanceExecution
	  {
		  get
		  {
			return Parent == null;
		  }
	  }

	  public virtual ProcessInstanceStartContext ProcessInstanceStartContext
	  {
		  get
		  {
			if (startContext != null && startContext is ProcessInstanceStartContext)
			{
			  return (ProcessInstanceStartContext) startContext;
			}
			return null;
		  }
	  }

	  public virtual bool hasProcessInstanceStartContext()
	  {
		return startContext != null && startContext is ProcessInstanceStartContext;
	  }

	  public virtual ExecutionStartContext StartContext
	  {
		  set
		  {
			this.startContext = value;
		  }
	  }


	  public virtual PvmExecutionImpl getParentScopeExecution(bool considerSuperExecution)
	  {
		if (ProcessInstanceExecution)
		{
		  if (considerSuperExecution && SuperExecution != null)
		  {
			PvmExecutionImpl superExecution = SuperExecution;
			if (superExecution.Scope)
			{
			  return superExecution;
			}
			else
			{
			  return superExecution.Parent;
			}
		  }
		  else
		  {
			return null;
		  }
		}
		else
		{
		  PvmExecutionImpl parent = Parent;
		  if (parent.Scope)
		  {
			return parent;
		  }
		  else
		  {
			return parent.Parent;
		  }
		}
	  }

	  /// <summary>
	  /// Contains the delayed variable events, which will be dispatched on a save point.
	  /// </summary>
	  [NonSerialized]
	  protected internal IList<DelayedVariableEvent> delayedEvents = new List<DelayedVariableEvent>();

	  /// <summary>
	  /// Delays a given variable event with the given target scope.
	  /// </summary>
	  /// <param name="targetScope">   the target scope of the variable event </param>
	  /// <param name="variableEvent"> the variable event which should be delayed </param>
	  public virtual void delayEvent(PvmExecutionImpl targetScope, VariableEvent variableEvent)
	  {
		DelayedVariableEvent delayedVariableEvent = new DelayedVariableEvent(targetScope, variableEvent);
		delayEvent(delayedVariableEvent);
	  }

	  /// <summary>
	  /// Delays and stores the given DelayedVariableEvent on the process instance.
	  /// </summary>
	  /// <param name="delayedVariableEvent"> the DelayedVariableEvent which should be store on the process instance </param>
	  public virtual void delayEvent(DelayedVariableEvent delayedVariableEvent)
	  {

		//if process definition has no conditional events the variable events does not have to be delayed
		bool? hasConditionalEvents = this.ProcessDefinition.Properties.get(BpmnProperties.HAS_CONDITIONAL_EVENTS);
		if (hasConditionalEvents == null || !hasConditionalEvents.Equals(true))
		{
		  return;
		}

		if (ProcessInstanceExecution)
		{
		  delayedEvents.Add(delayedVariableEvent);
		}
		else
		{
		  ProcessInstance.delayEvent(delayedVariableEvent);
		}
	  }

	  /// <summary>
	  /// The current delayed variable events.
	  /// </summary>
	  /// <returns> a list of DelayedVariableEvent objects </returns>
	  public virtual IList<DelayedVariableEvent> DelayedEvents
	  {
		  get
		  {
			if (ProcessInstanceExecution)
			{
			  return delayedEvents;
			}
			return ProcessInstance.DelayedEvents;
		  }
	  }

	  /// <summary>
	  /// Cleares the current delayed variable events.
	  /// </summary>
	  public virtual void clearDelayedEvents()
	  {
		if (ProcessInstanceExecution)
		{
		  delayedEvents.Clear();
		}
		else
		{
		  ProcessInstance.clearDelayedEvents();
		}
	  }


	  /// <summary>
	  /// Dispatches the current delayed variable events and performs the given atomic operation
	  /// if the current state was not changed.
	  /// </summary>
	  /// <param name="atomicOperation"> the atomic operation which should be executed </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void dispatchDelayedEventsAndPerformOperation(final org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation atomicOperation)
	  public virtual void dispatchDelayedEventsAndPerformOperation(PvmAtomicOperation atomicOperation)
	  {
		dispatchDelayedEventsAndPerformOperation(new CallbackAnonymousInnerClass(this, atomicOperation));
	  }

	  private class CallbackAnonymousInnerClass : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private PvmAtomicOperation atomicOperation;

		  public CallbackAnonymousInnerClass(PvmExecutionImpl outerInstance, PvmAtomicOperation atomicOperation)
		  {
			  this.outerInstance = outerInstance;
			  this.atomicOperation = atomicOperation;
		  }

		  public Void callback(PvmExecutionImpl param)
		  {
			param.performOperation(atomicOperation);
			return null;
		  }
	  }

	  /// <summary>
	  /// Dispatches the current delayed variable events and performs the given atomic operation
	  /// if the current state was not changed.
	  /// </summary>
	  /// <param name="continuation"> the atomic operation continuation which should be executed </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void dispatchDelayedEventsAndPerformOperation(final Callback<PvmExecutionImpl, Void> continuation)
	  public virtual void dispatchDelayedEventsAndPerformOperation(Callback<PvmExecutionImpl, Void> continuation)
	  {
		PvmExecutionImpl execution = this;

		if (execution.DelayedEvents.Count == 0)
		{
		  continueExecutionIfNotCanceled(continuation, execution);
		  return;
		}

		continueIfExecutionDoesNotAffectNextOperation(new CallbackAnonymousInnerClass2(this, execution)
	   , new CallbackAnonymousInnerClass3(this, continuation, execution)
	   , execution);
	  }

	  private class CallbackAnonymousInnerClass2 : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass2(PvmExecutionImpl outerInstance, org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void callback(PvmExecutionImpl execution)
		  {
			outerInstance.dispatchScopeEvents(execution);
			return null;
		  }
	  }

	  private class CallbackAnonymousInnerClass3 : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmExecutionImpl outerInstance;

		  private org.camunda.bpm.engine.impl.pvm.runtime.Callback<PvmExecutionImpl, Void> continuation;
		  private org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass3(PvmExecutionImpl outerInstance, org.camunda.bpm.engine.impl.pvm.runtime.Callback<PvmExecutionImpl, Void> continuation, org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.continuation = continuation;
			  this.execution = execution;
		  }

		  public Void callback(PvmExecutionImpl execution)
		  {
			outerInstance.continueExecutionIfNotCanceled(continuation, execution);
			return null;
		  }
	  }

	  /// <summary>
	  /// Executes the given depending operations with the given execution.
	  /// The execution state will be checked with the help of the activity instance id and activity id of the execution before and after
	  /// the dispatching callback call. If the id's are not changed the
	  /// continuation callback is called.
	  /// </summary>
	  /// <param name="dispatching">         the callback to dispatch the variable events </param>
	  /// <param name="continuation">        the callback to continue with the next atomic operation </param>
	  /// <param name="execution">           the execution which is used for the execution </param>
	  public virtual void continueIfExecutionDoesNotAffectNextOperation(Callback<PvmExecutionImpl, Void> dispatching, Callback<PvmExecutionImpl, Void> continuation, PvmExecutionImpl execution)
	  {

		string lastActivityId = execution.ActivityId;
		string lastActivityInstanceId = getActivityInstanceId(execution);

		dispatching.callback(execution);

		execution = execution.ReplacedBy != null ? execution.ReplacedBy : execution;
		string currentActivityInstanceId = getActivityInstanceId(execution);
		string currentActivityId = execution.ActivityId;

		//if execution was canceled or was changed during the dispatch we should not execute the next operation
		//since another atomic operation was executed during the dispatching
		if (!execution.Canceled && isOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId, currentActivityId))
		{
		  continuation.callback(execution);
		}
	  }

	  protected internal virtual void continueExecutionIfNotCanceled(Callback<PvmExecutionImpl, Void> continuation, PvmExecutionImpl execution)
	  {
		if (continuation != null && !execution.Canceled)
		{
		  continuation.callback(execution);
		}
	  }

	  /// <summary>
	  /// Dispatches the current delayed variable events on the scope of the given execution.
	  /// </summary>
	  /// <param name="execution"> the execution on which scope the delayed variable should be dispatched </param>
	  protected internal virtual void dispatchScopeEvents(PvmExecutionImpl execution)
	  {
		PvmExecutionImpl scopeExecution = execution.Scope ? execution : execution.Parent;

		IList<DelayedVariableEvent> delayedEvents = new List<DelayedVariableEvent>(scopeExecution.DelayedEvents);
		scopeExecution.clearDelayedEvents();

		IDictionary<PvmExecutionImpl, string> activityInstanceIds = new Dictionary<PvmExecutionImpl, string>();
		IDictionary<PvmExecutionImpl, string> activityIds = new Dictionary<PvmExecutionImpl, string>();
		initActivityIds(delayedEvents, activityInstanceIds, activityIds);

		//For each delayed variable event we have to check if the delayed event can be dispatched,
		//the check will be done with the help of the activity id and activity instance id.
		//That means it will be checked if the dispatching changed the execution tree in a way that we can't dispatch the
		//the other delayed variable events. We have to check the target scope with the last activity id and activity instance id
		//and also the replace pointer if it exist. Because on concurrency the replace pointer will be set on which we have
		//to check the latest state.
		foreach (DelayedVariableEvent @event in delayedEvents)
		{
		  PvmExecutionImpl targetScope = @event.TargetScope;
		  PvmExecutionImpl replaced = targetScope.ReplacedBy != null ? targetScope.ReplacedBy : targetScope;
		  dispatchOnSameActivity(targetScope, replaced, activityIds, activityInstanceIds, @event);
		}
	  }

	  /// <summary>
	  /// Initializes the given maps with the target scopes and current activity id's and activity instance id's.
	  /// </summary>
	  /// <param name="delayedEvents">       the delayed events which contains the information about the target scope </param>
	  /// <param name="activityInstanceIds"> the map which maps target scope to activity instance id </param>
	  /// <param name="activityIds">         the map which maps target scope to activity id </param>
	  protected internal virtual void initActivityIds(IList<DelayedVariableEvent> delayedEvents, IDictionary<PvmExecutionImpl, string> activityInstanceIds, IDictionary<PvmExecutionImpl, string> activityIds)
	  {

		foreach (DelayedVariableEvent @event in delayedEvents)
		{
		  PvmExecutionImpl targetScope = @event.TargetScope;

		  string targetScopeActivityInstanceId = getActivityInstanceId(targetScope);
		  activityInstanceIds[targetScope] = targetScopeActivityInstanceId;
		  activityIds[targetScope] = targetScope.ActivityId;
		}
	  }

	  /// <summary>
	  /// Dispatches the delayed variable event, if the target scope and replaced by scope (if target scope was replaced) have the
	  /// same activity Id's and activity instance id's.
	  /// </summary>
	  /// <param name="targetScope">          the target scope on which the event should be dispatched </param>
	  /// <param name="replacedBy">           the replaced by pointer which should have the same state </param>
	  /// <param name="activityIds">          the map which maps scope to activity id </param>
	  /// <param name="activityInstanceIds">  the map which maps scope to activity instance id </param>
	  /// <param name="delayedVariableEvent"> the delayed variable event which should be dispatched </param>
	  private void dispatchOnSameActivity(PvmExecutionImpl targetScope, PvmExecutionImpl replacedBy, IDictionary<PvmExecutionImpl, string> activityIds, IDictionary<PvmExecutionImpl, string> activityInstanceIds, DelayedVariableEvent delayedVariableEvent)
	  {
		//check if the target scope has the same activity id and activity instance id
		//since the dispatching was started
		string currentActivityInstanceId = getActivityInstanceId(targetScope);
		string currentActivityId = targetScope.ActivityId;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String lastActivityInstanceId = activityInstanceIds.get(targetScope);
		string lastActivityInstanceId = activityInstanceIds[targetScope];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String lastActivityId = activityIds.get(targetScope);
		string lastActivityId = activityIds[targetScope];

		bool onSameAct = isOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId, currentActivityId);

		//If not we have to check the replace pointer,
		//which was set if a concurrent execution was created during the dispatching.
		if (targetScope != replacedBy && !onSameAct)
		{
		  currentActivityInstanceId = getActivityInstanceId(replacedBy);
		  currentActivityId = replacedBy.ActivityId;
		  onSameAct = isOnSameActivity(lastActivityInstanceId, lastActivityId, currentActivityInstanceId, currentActivityId);
		}

		//dispatching
		if (onSameAct && isOnDispatchableState(targetScope))
		{
		  targetScope.dispatchEvent(delayedVariableEvent.Event);
		}
	  }

	  /// <summary>
	  /// Checks if the given execution is on a dispatchable state.
	  /// That means if the current activity is not a leaf in the activity tree OR
	  /// it is a leaf but not a scope OR it is a leaf, a scope
	  /// and the execution is in state DEFAULT, which means not in state
	  /// Starting, Execute or Ending. For this states it is
	  /// prohibited to trigger conditional events, otherwise unexpected behavior can appear.
	  /// </summary>
	  /// <returns> true if the execution is on a dispatchable state, false otherwise </returns>
	  private bool isOnDispatchableState(PvmExecutionImpl targetScope)
	  {
		ActivityImpl targetActivity = targetScope.getActivity();
		return string.ReferenceEquals(targetScope.ActivityId, null) || !targetActivity.Scope || (targetScope.isInState(ActivityInstanceState_Fields.DEFAULT));
	  }


	  /// <summary>
	  /// Compares the given activity instance id's and activity id's to check if the execution is on the same
	  /// activity as before an operation was executed. The activity instance id's can be null on transitions.
	  /// In this case the activity Id's have to be equal, otherwise the execution changed.
	  /// </summary>
	  /// <param name="lastActivityInstanceId">    the last activity instance id </param>
	  /// <param name="lastActivityId">            the last activity id </param>
	  /// <param name="currentActivityInstanceId"> the current activity instance id </param>
	  /// <param name="currentActivityId">         the current activity id </param>
	  /// <returns> true if the execution is on the same activity, otherwise false </returns>
	  private bool isOnSameActivity(string lastActivityInstanceId, string lastActivityId, string currentActivityInstanceId, string currentActivityId)
	  {
		return ((string.ReferenceEquals(lastActivityInstanceId, null) && string.ReferenceEquals(lastActivityInstanceId, currentActivityInstanceId) && lastActivityId.Equals(currentActivityId)) || (!string.ReferenceEquals(lastActivityInstanceId, null) && lastActivityInstanceId.Equals(currentActivityInstanceId) && (string.ReferenceEquals(lastActivityId, null) || lastActivityId.Equals(currentActivityId))));

	  }

	  /// <summary>
	  /// Returns the activity instance id for the given execution.
	  /// </summary>
	  /// <param name="targetScope"> the execution for which the activity instance id should be returned </param>
	  /// <returns> the activity instance id </returns>
	  private string getActivityInstanceId(PvmExecutionImpl targetScope)
	  {
		if (targetScope.Concurrent)
		{
		  return targetScope.ActivityInstanceId;
		}
		else
		{
		  ActivityImpl targetActivity = targetScope.getActivity();
		  if ((targetActivity != null && targetActivity.Activities.Count == 0))
		  {
			return targetScope.ActivityInstanceId;
		  }
		  else
		  {
			return targetScope.ParentActivityInstanceId;
		  }
		}
	  }

	  /// <summary>
	  /// Returns the newest incident in this execution
	  /// </summary>
	  /// <param name="incidentType"> the type of new incident </param>
	  /// <param name="configuration"> configuration of the incident </param>
	  /// <returns> new incident </returns>
	  public virtual Incident createIncident(string incidentType, string configuration)
	  {
		return createIncident(incidentType, configuration, null);
	  }

	  public virtual Incident createIncident(string incidentType, string configuration, string message)
	  {
		IncidentContext incidentContext = new IncidentContext();

		incidentContext.TenantId = this.TenantId;
		incidentContext.ProcessDefinitionId = this.ProcessDefinitionId;
		incidentContext.ExecutionId = this.Id;
		incidentContext.ActivityId = this.ActivityId;
		incidentContext.Configuration = configuration;

		IncidentHandler incidentHandler = findIncidentHandler(incidentType);

		if (incidentHandler == null)
		{
		  incidentHandler = new DefaultIncidentHandler(incidentType);
		}
		return incidentHandler.handleIncident(incidentContext, message);
	  }


	  /// <summary>
	  /// Resolves an incident with given id.
	  /// </summary>
	  /// <param name="incidentId"> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void resolveIncident(final String incidentId)
	  public virtual void resolveIncident(string incidentId)
	  {
		IncidentEntity incident = (IncidentEntity) Context.CommandContext.IncidentManager.findIncidentById(incidentId);

		IncidentHandler incidentHandler = findIncidentHandler(incident.IncidentType);

		if (incidentHandler == null)
		{
		  incidentHandler = new DefaultIncidentHandler(incident.IncidentType);
		}
		IncidentContext incidentContext = new IncidentContext(incident);
		incidentHandler.resolveIncident(incidentContext);
	  }

	  public virtual IncidentHandler findIncidentHandler(string incidentType)
	  {
		IDictionary<string, IncidentHandler> incidentHandlers = Context.ProcessEngineConfiguration.IncidentHandlers;
		return incidentHandlers[incidentType];
	  }
	}

}