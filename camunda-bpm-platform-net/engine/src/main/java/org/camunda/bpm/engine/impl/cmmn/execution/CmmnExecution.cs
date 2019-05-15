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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.FAILED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.NEW;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDING_ON_PARENT_SUSPENSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDING_ON_SUSPENSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATING_ON_EXIT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATING_ON_PARENT_TERMINATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATING_ON_TERMINATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration.IF_PART;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration.PLAN_ITEM_ON_PART;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration.VARIABLE_ON_PART;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_DELETE_CASCADE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_DISABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_ENABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_EXIT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_FIRE_ENTRY_CRITERIA;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_FIRE_EXIT_CRITERIA;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_MANUAL_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_MANUAL_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_OCCUR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_PARENT_RESUME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_PARENT_SUSPEND;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_PARENT_TERMINATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_RESUME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_RE_ACTIVATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_RE_ENABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_SUSPEND;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_SUSPENDING_ON_PARENT_SUSPENSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_SUSPENDING_ON_SUSPENSION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_TERMINATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_TERMINATING_ON_EXIT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_TERMINATING_ON_PARENT_TERMINATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_TERMINATING_ON_TERMINATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_INSTANCE_CLOSE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_INSTANCE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureInstanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using CmmnBehaviorLogger = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnBehaviorLogger;
	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnIfPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnIfPartDeclaration;
	using CmmnOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnOnPartDeclaration;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnVariableOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnVariableOnPartDeclaration;
	using CmmnAtomicOperation = org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using CaseVariableListenerInvocation = org.camunda.bpm.engine.impl.variable.listener.CaseVariableListenerInvocation;
	using DelegateCaseVariableInstanceImpl = org.camunda.bpm.engine.impl.variable.listener.DelegateCaseVariableInstanceImpl;
	using Task = org.camunda.bpm.engine.task.Task;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public abstract class CmmnExecution : CoreExecution, CmmnCaseInstance
	{
		public abstract org.camunda.bpm.model.cmmn.instance.CmmnElement CmmnModelElementInstance {get;}
		public abstract org.camunda.bpm.model.cmmn.CmmnModelInstance CmmnModelInstance {get;}
		public abstract org.camunda.bpm.engine.ProcessEngine ProcessEngine {get;}
		public abstract org.camunda.bpm.engine.ProcessEngineServices ProcessEngineServices {get;}
		public abstract string ActivityName {get;}
		public abstract string ActivityId {get;}
		public abstract string ParentId {get;}
		public abstract string CaseDefinitionId {get;}

	  protected internal static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  private const long serialVersionUID = 1L;

	  [NonSerialized]
	  protected internal CmmnCaseDefinition caseDefinition;

	  // current position //////////////////////////////////////

	  /// <summary>
	  /// current activity </summary>
	  [NonSerialized]
	  protected internal CmmnActivity activity;

	  protected internal bool required = false;

	  protected internal int previousState;

	  protected internal int currentState = NEW.StateCode;

	  protected internal LinkedList<VariableEvent> variableEventsQueue;

	  [NonSerialized]
	  protected internal TaskEntity task;

	  /// <summary>
	  /// This property will be used if <code>this</code>
	  /// <seealso cref="CmmnExecution"/> is in state <seealso cref="CaseExecutionState#NEW"/>
	  /// to note that an entry criterion is satisfied.
	  /// </summary>
	  protected internal bool entryCriterionSatisfied = false;

	  public CmmnExecution()
	  {
	  }

	  // plan items ///////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends CmmnExecution> getCaseExecutions();
	  public abstract IList<CmmnExecution> CaseExecutions {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.List<? extends CmmnExecution> getCaseExecutionsInternal();
	  protected internal abstract IList<CmmnExecution> CaseExecutionsInternal {get;}

	  public virtual CmmnExecution findCaseExecution(string activityId)
	  {
		if ((Activity != null) && (Activity.Id.Equals(activityId)))
		{
		 return this;
		}
	   foreach (CmmnExecution nestedExecution in CaseExecutions)
	   {
		 CmmnExecution result = nestedExecution.findCaseExecution(activityId);
		 if (result != null)
		 {
		   return result;
		 }
	   }
	   return null;
	  }

	  // task /////////////////////////////////////////////////////////////////////

	  public virtual TaskEntity getTask()
	  {
		return this.task;
	  }

	  public virtual void setTask(Task task)
	  {
		this.task = (TaskEntity) task;
	  }

	  public virtual TaskEntity createTask(TaskDecorator taskDecorator)
	  {
		TaskEntity task = TaskEntity.createAndInsert(this);

		setTask(task);

		taskDecorator.decorate(task, this);

		Context.CommandContext.HistoricTaskInstanceManager.createHistoricTask(task);

		// All properties set, now firing 'create' event
		task.fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE);

		return task;
	  }

	  // super execution  ////////////////////////////////////////////////////////

	  public abstract PvmExecutionImpl SuperExecution {get;set;}


	  // sub process instance ////////////////////////////////////////////////////

	  public abstract PvmExecutionImpl SubProcessInstance {get;set;}


	  public abstract PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition);

	  public abstract PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey);

	  public abstract PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId);

	  // sub-/super- case instance ////////////////////////////////////////////////////

	  public abstract CmmnExecution SubCaseInstance {get;set;}


	  public abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition);

	  public abstract CmmnExecution createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey);

	  public abstract CmmnExecution SuperCaseExecution {get;set;}


	  // sentry //////////////////////////////////////////////////////////////////

	  // sentry: (1) create and initialize sentry parts

	  protected internal abstract CmmnSentryPart newSentryPart();

	  protected internal abstract void addSentryPart(CmmnSentryPart sentryPart);

	  public virtual void createSentryParts()
	  {
		CmmnActivity activity = Activity;
		ensureNotNull("Case execution '" + id + "': has no current activity", "activity", activity);

		IList<CmmnSentryDeclaration> sentries = activity.Sentries;

		if (sentries != null && sentries.Count > 0)
		{

		  foreach (CmmnSentryDeclaration sentryDeclaration in sentries)
		  {

			CmmnIfPartDeclaration ifPartDeclaration = sentryDeclaration.IfPart;
			if (ifPartDeclaration != null)
			{
			  CmmnSentryPart ifPart = createIfPart(sentryDeclaration, ifPartDeclaration);
			  addSentryPart(ifPart);
			}

			IList<CmmnOnPartDeclaration> onPartDeclarations = sentryDeclaration.OnParts;

			foreach (CmmnOnPartDeclaration onPartDeclaration in onPartDeclarations)
			{
			  CmmnSentryPart onPart = createOnPart(sentryDeclaration, onPartDeclaration);
			  addSentryPart(onPart);
			}

			IList<CmmnVariableOnPartDeclaration> variableOnPartDeclarations = sentryDeclaration.VariableOnParts;
			foreach (CmmnVariableOnPartDeclaration variableOnPartDeclaration in variableOnPartDeclarations)
			{
			  CmmnSentryPart variableOnPart = createVariableOnPart(sentryDeclaration, variableOnPartDeclaration);
			  addSentryPart(variableOnPart);
			}

		  }
		}
	  }

	  protected internal virtual CmmnSentryPart createOnPart(CmmnSentryDeclaration sentryDeclaration, CmmnOnPartDeclaration onPartDeclaration)
	  {
		CmmnSentryPart sentryPart = createSentryPart(sentryDeclaration, PLAN_ITEM_ON_PART);

		// set the standard event
		string standardEvent = onPartDeclaration.StandardEvent;
		sentryPart.StandardEvent = standardEvent;

		// set source case execution
		CmmnActivity source = onPartDeclaration.Source;
		ensureNotNull("The source of sentry '" + sentryDeclaration.Id + "' is null.", "source", source);

		string sourceActivityId = source.Id;
		sentryPart.Source = sourceActivityId;

		// TODO: handle also sentryRef!!! (currently not implemented on purpose)

		return sentryPart;
	  }

	  protected internal virtual CmmnSentryPart createIfPart(CmmnSentryDeclaration sentryDeclaration, CmmnIfPartDeclaration ifPartDeclaration)
	  {
		return createSentryPart(sentryDeclaration, IF_PART);
	  }

	  protected internal virtual CmmnSentryPart createVariableOnPart(CmmnSentryDeclaration sentryDeclaration, CmmnVariableOnPartDeclaration variableOnPartDeclaration)
	  {
		CmmnSentryPart sentryPart = createSentryPart(sentryDeclaration, VARIABLE_ON_PART);

		// set the variable event
		string variableEvent = variableOnPartDeclaration.VariableEvent;
		sentryPart.VariableEvent = variableEvent;

		// set the variable name
		string variableName = variableOnPartDeclaration.VariableName;
		sentryPart.VariableName = variableName;

		return sentryPart;
	  }

	  protected internal virtual CmmnSentryPart createSentryPart(CmmnSentryDeclaration sentryDeclaration, string type)
	  {
		CmmnSentryPart newSentryPart = newSentryPart();

		// set the type
		newSentryPart.Type = type;

		// set the case instance and case execution
		newSentryPart.CaseInstance = CaseInstance;
		newSentryPart.CaseExecution = this;

		// set sentry id
		string sentryId = sentryDeclaration.Id;
		newSentryPart.SentryId = sentryId;

		return newSentryPart;
	  }

	  // sentry: (2) handle transitions

	  public virtual void handleChildTransition(CmmnExecution child, string transition)
	  {
		// Step 1: collect all affected sentries
		IList<string> affectedSentries = collectAffectedSentries(child, transition);

		// Step 2: fire force update on all case sentry part
		// contained by a affected sentry to provoke an
		// OptimisticLockingException
		forceUpdateOnSentries(affectedSentries);

		// Step 3: check each affected sentry whether it is satisfied.
		// the returned list contains all satisfied sentries
		IList<string> satisfiedSentries = getSatisfiedSentries(affectedSentries);

		// Step 4: reset sentries -> satisfied == false
		resetSentries(satisfiedSentries);

		// Step 5: fire satisfied sentries
		fireSentries(satisfiedSentries);

	  }

	  public virtual void fireIfOnlySentryParts()
	  {
		// the following steps are a workaround, because setVariable()
		// does not check nor fire a sentry!!!
		ISet<string> affectedSentries = new HashSet<string>();
		IList<CmmnSentryPart> sentryParts = collectSentryParts(Sentries);
		foreach (CmmnSentryPart sentryPart in sentryParts)
		{
		  if (isNotSatisfiedIfPartOnly(sentryPart))
		  {
			affectedSentries.Add(sentryPart.SentryId);
		  }
		}

		// Step 7: check each not affected sentry whether it is satisfied
		IList<string> satisfiedSentries = getSatisfiedSentries(new List<string>(affectedSentries));

		// Step 8: reset sentries -> satisfied == false
		resetSentries(satisfiedSentries);

		// Step 9: fire satisfied sentries
		fireSentries(satisfiedSentries);
	  }

	  public virtual void handleVariableTransition(string variableName, string transition)
	  {
		IDictionary<string, IList<CmmnSentryPart>> sentries = collectAllSentries();

		IList<CmmnSentryPart> sentryParts = collectSentryParts(sentries);

		IList<string> affectedSentries = collectAffectedSentriesWithVariableOnParts(variableName, transition, sentryParts);

		IList<CmmnSentryPart> affectedSentryParts = getAffectedSentryParts(sentries,affectedSentries);
		forceUpdateOnCaseSentryParts(affectedSentryParts);

		IList<string> allSentries = new List<string>(sentries.Keys);

		IList<string> satisfiedSentries = getSatisfiedSentriesInExecutionTree(allSentries, sentries);

		IList<CmmnSentryPart> satisfiedSentryParts = getAffectedSentryParts(sentries, satisfiedSentries);
		resetSentryParts(satisfiedSentryParts);

		fireSentries(satisfiedSentries);

	  }

	  protected internal virtual IList<string> collectAffectedSentries(CmmnExecution child, string transition)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnSentryPart> sentryParts = getCaseSentryParts();
		IList<CmmnSentryPart> sentryParts = CaseSentryParts;

		IList<string> affectedSentries = new List<string>();

		foreach (CmmnSentryPart sentryPart in sentryParts)
		{

		  // necessary for backward compatibility
		  string sourceCaseExecutionId = sentryPart.SourceCaseExecutionId;
		  string sourceRef = sentryPart.Source;
		  if (child.ActivityId.Equals(sourceRef) || child.Id.Equals(sourceCaseExecutionId))
		  {

			string standardEvent = sentryPart.StandardEvent;
			if (transition.Equals(standardEvent))
			{
			  addIdIfNotSatisfied(affectedSentries, sentryPart);
			}
		  }
		}

		return affectedSentries;
	  }

	  protected internal virtual bool isNotSatisfiedIfPartOnly(CmmnSentryPart sentryPart)
	  {
		return IF_PART.Equals(sentryPart.Type) && Sentries[sentryPart.SentryId].Count == 1 && !sentryPart.Satisfied;
	  }

	  protected internal virtual void addIdIfNotSatisfied(IList<string> affectedSentries, CmmnSentryPart sentryPart)
	  {
		if (!sentryPart.Satisfied)
		{
		  // if it is not already satisfied, then set the
		  // current case sentry part to satisfied (=true).
		  string sentryId = sentryPart.SentryId;
		  sentryPart.Satisfied = true;

		  // collect the id of affected sentry.
		  if (!affectedSentries.Contains(sentryId))
		  {
			affectedSentries.Add(sentryId);
		  }
		}
	  }

	  protected internal virtual IList<string> collectAffectedSentriesWithVariableOnParts(string variableName, string variableEvent, IList<CmmnSentryPart> sentryParts)
	  {

		IList<string> affectedSentries = new List<string>();

		foreach (CmmnSentryPart sentryPart in sentryParts)
		{

		  string sentryVariableName = sentryPart.VariableName;
		  string sentryVariableEvent = sentryPart.VariableEvent;
		  CmmnExecution execution = sentryPart.CaseExecution;
		  if (VARIABLE_ON_PART.Equals(sentryPart.Type) && sentryVariableName.Equals(variableName) && sentryVariableEvent.Equals(variableEvent) && !hasVariableWithSameNameInParent(execution, sentryVariableName))
		  {

			addIdIfNotSatisfied(affectedSentries, sentryPart);
		  }
		}

		return affectedSentries;
	  }

	  protected internal virtual bool hasVariableWithSameNameInParent(CmmnExecution execution, string variableName)
	  {
		while (execution != null)
		{
		  if (execution.Id.Equals(Id))
		  {
			return false;
		  }
		  TypedValue variableTypedValue = execution.getVariableLocalTyped(variableName);
		  if (variableTypedValue != null)
		  {
			return true;
		  }
		  execution = execution.Parent;
		}
		return false;
	  }

	  protected internal virtual IDictionary<string, IList<CmmnSentryPart>> collectAllSentries()
	  {
		IDictionary<string, IList<CmmnSentryPart>> sentries = new Dictionary<string, IList<CmmnSentryPart>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnExecution> caseExecutions = getCaseExecutions();
		IList<CmmnExecution> caseExecutions = CaseExecutions;
		foreach (CmmnExecution caseExecution in caseExecutions)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  sentries.putAll(caseExecution.collectAllSentries());
		}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		sentries.putAll(Sentries);
		return sentries;
	  }

	  protected internal virtual IList<CmmnSentryPart> getAffectedSentryParts(IDictionary<string, IList<CmmnSentryPart>> allSentries, IList<string> affectedSentries)
	  {
		IList<CmmnSentryPart> affectedSentryParts = new List<CmmnSentryPart>();
		foreach (string affectedSentryId in affectedSentries)
		{
		  ((IList<CmmnSentryPart>)affectedSentryParts).AddRange(allSentries[affectedSentryId]);
		}
		return affectedSentryParts;
	  }

	  protected internal virtual IList<CmmnSentryPart> collectSentryParts(IDictionary<string, IList<CmmnSentryPart>> sentries)
	  {
		IList<CmmnSentryPart> sentryParts = new List<CmmnSentryPart>();
		foreach (string sentryId in sentries.Keys)
		{
		  ((IList<CmmnSentryPart>)sentryParts).AddRange(sentries[sentryId]);
		}
		return sentryParts;
	  }

	  protected internal virtual void forceUpdateOnCaseSentryParts(IList<CmmnSentryPart> sentryParts)
	  {
		// set for each case sentry part forceUpdate flag to true to provoke
		// an OptimisticLockingException if different case sentry parts of the
		// same sentry has been satisfied concurrently.
		foreach (CmmnSentryPart sentryPart in sentryParts)
		{
		  if (sentryPart is CaseSentryPartEntity)
		  {
			CaseSentryPartEntity sentryPartEntity = (CaseSentryPartEntity) sentryPart;
			sentryPartEntity.forceUpdate();
		  }
		}
	  }

	  /// <summary>
	  /// Checks for each given sentry id whether the corresponding
	  /// sentry is satisfied.
	  /// </summary>
	  protected internal virtual IList<string> getSatisfiedSentries(IList<string> sentryIds)
	  {
		IList<string> result = new List<string>();

		if (sentryIds != null)
		{

		  foreach (string sentryId in sentryIds)
		  {

			if (isSentrySatisfied(sentryId))
			{
			  result.Add(sentryId);
			}
		  }
		}

		return result;
	  }

	  /// <summary>
	  /// Checks for each given sentry id in the execution tree whether the corresponding
	  /// sentry is satisfied.
	  /// </summary>
	  protected internal virtual IList<string> getSatisfiedSentriesInExecutionTree(IList<string> sentryIds, IDictionary<string, IList<CmmnSentryPart>> allSentries)
	  {
		IList<string> result = new List<string>();

		if (sentryIds != null)
		{

		  foreach (string sentryId in sentryIds)
		  {
			IList<CmmnSentryPart> sentryParts = allSentries[sentryId];
			if (isSentryPartsSatisfied(sentryId, sentryParts))
			{
			  result.Add(sentryId);
			}
		  }
		}

		return result;
	  }

	  protected internal virtual void forceUpdateOnSentries(IList<string> sentryIds)
	  {
		foreach (string sentryId in sentryIds)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnSentryPart> sentryParts = findSentry(sentryId);
		  IList<CmmnSentryPart> sentryParts = findSentry(sentryId);
		  // set for each case sentry part forceUpdate flag to true to provoke
		  // an OptimisticLockingException if different case sentry parts of the
		  // same sentry has been satisfied concurrently.
		  foreach (CmmnSentryPart sentryPart in sentryParts)
		  {
			if (sentryPart is CaseSentryPartEntity)
			{
			  CaseSentryPartEntity sentryPartEntity = (CaseSentryPartEntity) sentryPart;
			  sentryPartEntity.forceUpdate();
			}
		  }
		}
	  }

	  protected internal virtual void resetSentries(IList<string> sentries)
	  {
		foreach (string sentry in sentries)
		{
		  IList<CmmnSentryPart> parts = Sentries[sentry];
		  foreach (CmmnSentryPart part in parts)
		  {
			part.Satisfied = false;
		  }
		}
	  }

	  protected internal virtual void resetSentryParts(IList<CmmnSentryPart> parts)
	  {
		foreach (CmmnSentryPart part in parts)
		{
		  part.Satisfied = false;
		}
	  }

	  protected internal virtual void fireSentries(IList<string> satisfiedSentries)
	  {
		if (satisfiedSentries != null && satisfiedSentries.Count > 0)
		{
		  // if there are satisfied sentries, trigger the associated
		  // case executions

		  // 1. propagate to all child case executions ///////////////////////////////////////////

		  // collect the execution tree.
		  List<CmmnExecution> children = new List<CmmnExecution>();
		  collectCaseExecutionsInExecutionTree(children);

		  foreach (CmmnExecution currentChild in children)
		  {

			// check and fire first exitCriteria
			currentChild.checkAndFireExitCriteria(satisfiedSentries);

			// then trigger entryCriteria
			currentChild.checkAndFireEntryCriteria(satisfiedSentries);
		  }

		  // 2. check exit criteria of the case instance //////////////////////////////////////////

		  if (CaseInstanceExecution && Active)
		  {
			checkAndFireExitCriteria(satisfiedSentries);
		  }

		}
	  }

	  protected internal virtual void collectCaseExecutionsInExecutionTree(IList<CmmnExecution> children)
	  {
		foreach (CmmnExecution child in CaseExecutions)
		{
		  child.collectCaseExecutionsInExecutionTree(children);
		}
		((IList<CmmnExecution>)children).AddRange(CaseExecutions);
	  }

	  protected internal virtual void checkAndFireExitCriteria(IList<string> satisfiedSentries)
	  {
		if (Active)
		{
		  CmmnActivity activity = Activity;
		  ensureNotNull(typeof(PvmException), "Case execution '" + Id + "': has no current activity.", "activity", activity);

		  // trigger first exitCriteria
		  IList<CmmnSentryDeclaration> exitCriteria = activity.ExitCriteria;
		  foreach (CmmnSentryDeclaration sentryDeclaration in exitCriteria)
		  {

			if (sentryDeclaration != null && satisfiedSentries.Contains(sentryDeclaration.Id))
			{
			  fireExitCriteria();
			  break;
			}
		  }
		}
	  }

	  protected internal virtual void checkAndFireEntryCriteria(IList<string> satisfiedSentries)
	  {
		if (Available || New)
		{
		  // do that only, when this child case execution
		  // is available

		  CmmnActivity activity = Activity;
		  ensureNotNull(typeof(PvmException), "Case execution '" + Id + "': has no current activity.", "activity", activity);

		  IList<CmmnSentryDeclaration> criteria = activity.EntryCriteria;
		  foreach (CmmnSentryDeclaration sentryDeclaration in criteria)
		  {
			if (sentryDeclaration != null && satisfiedSentries.Contains(sentryDeclaration.Id))
			{
			  if (Available)
			  {
				fireEntryCriteria();
			  }
			  else
			  {
				entryCriterionSatisfied = true;
			  }
			  break;
			}
		  }
		}
	  }

	  public virtual void fireExitCriteria()
	  {
		performOperation(CASE_EXECUTION_FIRE_EXIT_CRITERIA);
	  }

	  public virtual void fireEntryCriteria()
	  {
		performOperation(CASE_EXECUTION_FIRE_ENTRY_CRITERIA);
	  }

	  // sentry: (3) helper

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends CmmnSentryPart> getCaseSentryParts();
	  public abstract IList<CmmnSentryPart> CaseSentryParts {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.List<? extends CmmnSentryPart> findSentry(String sentryId);
	  protected internal abstract IList<CmmnSentryPart> findSentry(string sentryId);

	  protected internal abstract IDictionary<string, IList<CmmnSentryPart>> Sentries {get;}

	  public virtual bool isSentrySatisfied(string sentryId)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnSentryPart> sentryParts = findSentry(sentryId);
		IList<CmmnSentryPart> sentryParts = findSentry(sentryId);
		return isSentryPartsSatisfied(sentryId, sentryParts);

	  }

	  protected internal virtual bool isSentryPartsSatisfied<T1>(string sentryId, IList<T1> sentryParts) where T1 : CmmnSentryPart
	  {
		// if part will be evaluated in the end
		CmmnSentryPart ifPart = null;

		if (sentryParts != null && sentryParts.Count > 0)
		{
		  foreach (CmmnSentryPart sentryPart in sentryParts)
		  {

			if (PLAN_ITEM_ON_PART.Equals(sentryPart.Type))
			{

			  if (!sentryPart.Satisfied)
			  {
				return false;
			  }

			}
			else if (VARIABLE_ON_PART.Equals(sentryPart.Type))
			{
			  if (!sentryPart.Satisfied)
			  {
				return false;
			  }
			}
			else
			{ // IF_PART.equals(sentryPart.getType) == true

			  ifPart = sentryPart;

			  // once the ifPart has been satisfied the whole sentry is satisfied
			  if (ifPart.Satisfied)
			  {
				return true;
			  }

			}

		  }
		}

		if (ifPart != null)
		{

		  CmmnExecution execution = ifPart.CaseExecution;
		  ensureNotNull("Case execution of sentry '" + ifPart.SentryId + "': is null", execution);

		  CmmnActivity activity = ifPart.CaseExecution.Activity;
		  ensureNotNull("Case execution '" + id + "': has no current activity", "activity", activity);

		  CmmnSentryDeclaration sentryDeclaration = activity.getSentry(sentryId);
		  ensureNotNull("Case execution '" + id + "': has no declaration for sentry '" + sentryId + "'", "sentryDeclaration", sentryDeclaration);

		  CmmnIfPartDeclaration ifPartDeclaration = sentryDeclaration.IfPart;
		  ensureNotNull("Sentry declaration '" + sentryId + "' has no definied ifPart, but there should be one defined for case execution '" + id + "'.", "ifPartDeclaration", ifPartDeclaration);

		  Expression condition = ifPartDeclaration.Condition;
		  ensureNotNull("A condition was expected for ifPart of Sentry declaration '" + sentryId + "' for case execution '" + id + "'.", "condition", condition);

		  object result = condition.getValue(this);
		  ensureInstanceOf("condition expression returns non-Boolean", "result", result, typeof(Boolean));

		  bool? booleanResult = (bool?) result;
		  ifPart.Satisfied = booleanResult.Value;
		  return booleanResult.Value;

		}

		// if all onParts are satisfied and there is no
		// ifPart then the whole sentry is satisfied.
		return true;
	  }

	  protected internal virtual bool containsIfPartAndExecutionActive(string sentryId, IDictionary<string, IList<CmmnSentryPart>> sentries)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnSentryPart> sentryParts = sentries.get(sentryId);
		IList<CmmnSentryPart> sentryParts = sentries[sentryId];

		foreach (CmmnSentryPart part in sentryParts)
		{
		  CmmnExecution caseExecution = part.CaseExecution;
		  if (IF_PART.Equals(part.Type) && caseExecution != null && caseExecution.Active)
		  {
			return true;
		  }
		}

		return false;
	  }

	  public virtual bool EntryCriterionSatisfied
	  {
		  get
		  {
			return entryCriterionSatisfied;
		  }
	  }

	  // business key ////////////////////////////////////////////////////////////

	  public virtual string CaseBusinessKey
	  {
		  get
		  {
			return CaseInstance.BusinessKey;
		  }
	  }

	  public override string BusinessKey
	  {
		  get
		  {
			if (this.CaseInstanceExecution)
			{
			  return businessKey;
			}
			else
			{
				return CaseBusinessKey;
			}
		  }
	  }

	  // case definition ///////////////////////////////////////////////////////

	  public virtual CmmnCaseDefinition CaseDefinition
	  {
		  get
		  {
			return caseDefinition;
		  }
		  set
		  {
			this.caseDefinition = value;
		  }
	  }


	  // case instance /////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process instance. </summary>
	  public abstract CmmnExecution CaseInstance {get;set;}


	  public virtual bool CaseInstanceExecution
	  {
		  get
		  {
			return Parent == null;
		  }
	  }

	  // case instance id /////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process instance. </summary>
	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return CaseInstance.Id;
		  }
	  }

	  // parent ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the parent </summary>
	  public abstract CmmnExecution Parent {get;set;}


	  // activity /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the activity </summary>
	  public virtual CmmnActivity Activity
	  {
		  get
		  {
			return activity;
		  }
		  set
		  {
			this.activity = value;
		  }
	  }


	  // variables ////////////////////////////////////////////

	  public override string VariableScopeKey
	  {
		  get
		  {
			return "caseExecution";
		  }
	  }

	  public override AbstractVariableScope ParentVariableScope
	  {
		  get
		  {
			return Parent;
		  }
	  }

	  //delete/remove /////////////////////////////////////////////////////

	  public virtual void deleteCascade()
	  {
	   performOperation(CASE_EXECUTION_DELETE_CASCADE);
	  }

	  public virtual void remove()
	  {
	   CmmnExecution parent = Parent;
	   if (parent != null)
	   {
		 parent.CaseExecutionsInternal.Remove(this);
	   }
	  }

	  // required //////////////////////////////////////////////////

	  public virtual bool Required
	  {
		  get
		  {
			return required;
		  }
		  set
		  {
			this.required = value;
		  }
	  }


	  // state /////////////////////////////////////////////////////

	  public virtual CaseExecutionState CurrentState
	  {
		  get
		  {
			return CaseExecutionState_Fields.CASE_EXECUTION_STATES[State];
		  }
		  set
		  {
			if (!Suspending && !Terminating)
			{
			  // do not reset the previous state, if this case execution
			  // is currently terminating or suspending. otherwise the
			  // "real" previous state is lost.
			  previousState = this.currentState;
			}
			this.currentState = value.StateCode;
		  }
	  }


	  public virtual int State
	  {
		  get
		  {
			return currentState;
		  }
		  set
		  {
			this.currentState = value;
		  }
	  }


	  public virtual bool New
	  {
		  get
		  {
			return currentState == NEW.StateCode;
		  }
	  }

	  public virtual bool Available
	  {
		  get
		  {
			return currentState == AVAILABLE.StateCode;
		  }
	  }

	  public virtual bool Enabled
	  {
		  get
		  {
			return currentState == ENABLED.StateCode;
		  }
	  }

	  public virtual bool Disabled
	  {
		  get
		  {
			return currentState == DISABLED.StateCode;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return currentState == ACTIVE.StateCode;
		  }
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return currentState == COMPLETED.StateCode;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return currentState == SUSPENDED.StateCode;
		  }
	  }

	  public virtual bool Suspending
	  {
		  get
		  {
			return currentState == SUSPENDING_ON_SUSPENSION.StateCode || currentState == SUSPENDING_ON_PARENT_SUSPENSION.StateCode;
		  }
	  }

	  public virtual bool Terminated
	  {
		  get
		  {
			return currentState == TERMINATED.StateCode;
		  }
	  }

	  public virtual bool Terminating
	  {
		  get
		  {
			return currentState == TERMINATING_ON_TERMINATION.StateCode || currentState == TERMINATING_ON_PARENT_TERMINATION.StateCode || currentState == TERMINATING_ON_EXIT.StateCode;
		  }
	  }

	  public virtual bool Failed
	  {
		  get
		  {
			return currentState == FAILED.StateCode;
		  }
	  }

	  public virtual bool Closed
	  {
		  get
		  {
			return currentState == CLOSED.StateCode;
		  }
	  }

	  // previous state /////////////////////////////////////////////

	  public virtual CaseExecutionState PreviousState
	  {
		  get
		  {
			return CaseExecutionState_Fields.CASE_EXECUTION_STATES[Previous];
		  }
	  }

	  public virtual int Previous
	  {
		  get
		  {
			return previousState;
		  }
		  set
		  {
			this.previousState = value;
		  }
	  }


	  // state transition ///////////////////////////////////////////

	  public virtual void create()
	  {
		create(null);
	  }

	  public virtual void create(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  Variables = variables;
		}

		performOperation(CASE_INSTANCE_CREATE);
	  }

	  public virtual IList<CmmnExecution> createChildExecutions(IList<CmmnActivity> activities)
	  {
		IList<CmmnExecution> children = new List<CmmnExecution>();

		// first create new child case executions
		foreach (CmmnActivity currentActivity in activities)
		{
		  CmmnExecution child = createCaseExecution(currentActivity);
		  children.Add(child);
		}

		return children;
	  }


	  public virtual void triggerChildExecutionsLifecycle(IList<CmmnExecution> children)
	  {
		// then notify create listener for each created
		// child case execution
		foreach (CmmnExecution child in children)
		{

		  if (Active)
		  {
			if (child.New)
			{
			  child.performOperation(CASE_EXECUTION_CREATE);
			}
		  }
		  else
		  {
			// if this case execution is not active anymore,
			// then stop notifying create listener and executing
			// of each child case execution
			break;
		  }
		}
	  }

	  protected internal abstract CmmnExecution createCaseExecution(CmmnActivity activity);

	  protected internal abstract CmmnExecution newCaseExecution();

	  public virtual void enable()
	  {
		performOperation(CASE_EXECUTION_ENABLE);
	  }

	  public virtual void disable()
	  {
		performOperation(CASE_EXECUTION_DISABLE);
	  }

	  public virtual void reenable()
	  {
		performOperation(CASE_EXECUTION_RE_ENABLE);
	  }

	  public virtual void manualStart()
	  {
		performOperation(CASE_EXECUTION_MANUAL_START);
	  }

	  public virtual void start()
	  {
		performOperation(CASE_EXECUTION_START);
	  }

	  public virtual void complete()
	  {
		performOperation(CASE_EXECUTION_COMPLETE);
	  }

	  public virtual void manualComplete()
	  {
		performOperation(CASE_EXECUTION_MANUAL_COMPLETE);
	  }

	  public virtual void occur()
	  {
		performOperation(CASE_EXECUTION_OCCUR);
	  }

	  public virtual void terminate()
	  {
		performOperation(CASE_EXECUTION_TERMINATING_ON_TERMINATION);
	  }

	  public virtual void performTerminate()
	  {
		performOperation(CASE_EXECUTION_TERMINATE);
	  }

	  public virtual void parentTerminate()
	  {
		performOperation(CASE_EXECUTION_TERMINATING_ON_PARENT_TERMINATION);
	  }

	  public virtual void performParentTerminate()
	  {
		performOperation(CASE_EXECUTION_PARENT_TERMINATE);
	  }

	  public virtual void exit()
	  {
		performOperation(CASE_EXECUTION_TERMINATING_ON_EXIT);
	  }

	  public virtual void parentComplete()
	  {
		performOperation(org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation_Fields.CASE_EXECUTION_PARENT_COMPLETE);
	  }

	  public virtual void performExit()
	  {
		performOperation(CASE_EXECUTION_EXIT);
	  }

	  public virtual void suspend()
	  {
		performOperation(CASE_EXECUTION_SUSPENDING_ON_SUSPENSION);
	  }

	  public virtual void performSuspension()
	  {
		performOperation(CASE_EXECUTION_SUSPEND);
	  }

	  public virtual void parentSuspend()
	  {
		performOperation(CASE_EXECUTION_SUSPENDING_ON_PARENT_SUSPENSION);
	  }

	  public virtual void performParentSuspension()
	  {
		performOperation(CASE_EXECUTION_PARENT_SUSPEND);
	  }

	  public virtual void resume()
	  {
		performOperation(CASE_EXECUTION_RESUME);
	  }

	  public virtual void parentResume()
	  {
		performOperation(CASE_EXECUTION_PARENT_RESUME);
	  }

	  public virtual void reactivate()
	  {
		performOperation(CASE_EXECUTION_RE_ACTIVATE);
	  }

	  public virtual void close()
	  {
		performOperation(CASE_INSTANCE_CLOSE);
	  }

	  // variable listeners
	  public override void dispatchEvent(VariableEvent variableEvent)
	  {
		bool invokeCustomListeners = Context.ProcessEngineConfiguration.InvokeCustomVariableListeners;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> listeners = getActivity().getVariableListeners(variableEvent.getEventName(), invokeCustomListeners);
		IDictionary<string, IList<VariableListener<object>>> listeners = Activity.getVariableListeners(variableEvent.EventName, invokeCustomListeners);

		// only attempt to invoke listeners if there are any (as this involves resolving the upwards execution hierarchy)
		if (listeners.Count > 0)
		{
		  CaseInstance.queueVariableEvent(variableEvent, invokeCustomListeners);
		}
	  }

	  protected internal virtual void queueVariableEvent(VariableEvent variableEvent, bool includeCustomerListeners)
	  {

		LinkedList<VariableEvent> variableEventsQueue = VariableEventQueue;

		variableEventsQueue.AddLast(variableEvent);

		// if this is the first event added, trigger listener invocation
		if (variableEventsQueue.Count == 1)
		{
		  invokeVariableListeners(includeCustomerListeners);
		}
	  }

	  protected internal virtual void invokeVariableListeners(bool includeCustomerListeners)
	  {
		LinkedList<VariableEvent> variableEventsQueue = VariableEventQueue;

		while (variableEventsQueue.Count > 0)
		{
		  // do not remove the event yet, as otherwise new events will immediately be dispatched
		  VariableEvent nextEvent = variableEventsQueue.First.Value;

		  CmmnExecution sourceExecution = (CmmnExecution) nextEvent.SourceScope;

		  DelegateCaseVariableInstanceImpl delegateVariable = DelegateCaseVariableInstanceImpl.fromVariableInstance(nextEvent.VariableInstance);
		  delegateVariable.EventName = nextEvent.EventName;
		  delegateVariable.SourceExecution = sourceExecution;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>>> listenersByActivity = sourceExecution.getActivity().getVariableListeners(delegateVariable.getEventName(), includeCustomerListeners);
		  IDictionary<string, IList<VariableListener<object>>> listenersByActivity = sourceExecution.Activity.getVariableListeners(delegateVariable.EventName, includeCustomerListeners);

		  CmmnExecution currentExecution = sourceExecution;
		  while (currentExecution != null)
		  {

			if (!string.ReferenceEquals(currentExecution.ActivityId, null))
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listeners = listenersByActivity.get(currentExecution.getActivityId());
			  IList<VariableListener<object>> listeners = listenersByActivity[currentExecution.ActivityId];

			  if (listeners != null)
			  {
				delegateVariable.ScopeExecution = currentExecution;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.delegate.VariableListener<?> listener : listeners)
				foreach (VariableListener<object> listener in listeners)
				{
				  try
				  {
					CaseVariableListener caseVariableListener = (CaseVariableListener) listener;
					CaseVariableListenerInvocation invocation = new CaseVariableListenerInvocation(caseVariableListener, delegateVariable, currentExecution);
					Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
				  }
				  catch (Exception e)
				  {
					throw LOG.invokeVariableListenerException(e);
				  }
				}
			  }
			}

			currentExecution = currentExecution.Parent;
		  }

		  // finally remove the event from the queue
		  variableEventsQueue.RemoveFirst();
		}
	  }

	  protected internal virtual LinkedList<VariableEvent> VariableEventQueue
	  {
		  get
		  {
			if (variableEventsQueue == null)
			{
			  variableEventsQueue = new LinkedList<VariableEvent>();
			}
    
			return variableEventsQueue;
		  }
	  }

	  // toString() //////////////////////////////////////  ///////////

	  public override string ToString()
	  {
		if (CaseInstanceExecution)
		{
		  return "CaseInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return "CmmnExecution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal virtual string ToStringIdentity
	  {
		  get
		  {
			return id;
		  }
	  }

	}

}