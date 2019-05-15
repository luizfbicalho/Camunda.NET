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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;

	using static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_AUTO_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureInstanceOf;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class StageActivityBehavior : StageOrTaskActivityBehavior, CmmnCompositeActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  // start /////////////////////////////////////////////////////////////////////

	  protected internal override void performStart(CmmnActivityExecution execution)
	  {
		CmmnActivity activity = execution.Activity;
		IList<CmmnActivity> childActivities = activity.Activities;

		if (childActivities != null && childActivities.Count > 0)
		{
		  IList<CmmnExecution> children = execution.createChildExecutions(childActivities);
		  execution.createSentryParts();
		  execution.triggerChildExecutionsLifecycle(children);


		  if (execution.Active)
		  {
			execution.fireIfOnlySentryParts();
			// if "autoComplete == true" and there are no
			// required nor active child activities,
			// then the stage will be completed.
			if (execution.Active)
			{
			  checkAndCompleteCaseExecution(execution);
			}
		  }

		}
		else
		{
		  execution.complete();
		}
	  }

	  // re-activation ////////////////////////////////////////////////////////////

	  public override void onReactivation(CmmnActivityExecution execution)
	  {
		string id = execution.Id;

		if (execution.Active)
		{
		  throw LOG.alreadyActiveException("reactivate", id);
		}

		if (execution.CaseInstanceExecution)
		{
		  if (execution.Closed)
		  {
			throw LOG.alreadyClosedCaseException("reactivate", id);
		  }
		}
		else
		{
		  ensureTransitionAllowed(execution, FAILED, ACTIVE, "reactivate");
		}
	  }

	  public override void reactivated(CmmnActivityExecution execution)
	  {
		if (execution.CaseInstanceExecution)
		{
		  CaseExecutionState previousState = execution.PreviousState;

		  if (SUSPENDED.Equals(previousState))
		  {
			resumed(execution);
		  }
		}

		// at the moment it is not possible to re-activate a case execution
		// because the state "FAILED" is not implemented.
	  }

	  // completion //////////////////////////////////////////////////////////////

	  public override void onCompletion(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, COMPLETED, "complete");
		canComplete(execution, true);
		completing(execution);
	  }

	  public override void onManualCompletion(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, COMPLETED, "complete");
		canComplete(execution, true, true);
		completing(execution);
	  }

	  protected internal override void completing(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;
		foreach (CmmnExecution child in children)
		{
		  if (!child.Disabled)
		  {
			child.parentComplete();
		  }
		  else
		  {
			child.remove();
		  }
		}
	  }

	  protected internal virtual bool canComplete(CmmnActivityExecution execution)
	  {
		return canComplete(execution, false);
	  }

	  protected internal virtual bool canComplete(CmmnActivityExecution execution, bool throwException)
	  {
		bool autoComplete = evaluateAutoComplete(execution);
		return canComplete(execution, throwException, autoComplete);
	  }

	  protected internal virtual bool canComplete(CmmnActivityExecution execution, bool throwException, bool autoComplete)
	  {
		string id = execution.Id;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;

		if (children == null || children.Count == 0)
		{
		  // if the stage does not contain any child
		  // then the stage can complete.
		  return true;
		}

		// verify there are no STATE_ACTIVE children
		foreach (CmmnExecution child in children)
		{
		  if (child.New || child.Active)
		  {

			if (throwException)
			{
			  throw LOG.remainingChildException("complete", id, child.Id, org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE);
			}

			return false;
		  }
		}

		if (autoComplete)
		{
		  // ensure that all required children are DISABLED, STATE_COMPLETED and/or TERMINATED
		  // available in the case execution tree.

		  foreach (CmmnExecution child in children)
		  {
			if (child.Required && !child.Disabled && !child.Completed && !child.Terminated)
			{

			  if (throwException)
			  {
				throw LOG.remainingChildException("complete", id, child.Id, child.CurrentState);
			  }

			  return false;
			}
		  }

		}
		else
		{ // autoComplete == false && manualCompletion == false
		  // ensure that ALL children are DISABLED, STATE_COMPLETED and/or TERMINATED

		  foreach (CmmnExecution child in children)
		  {
			if (!child.Disabled && !child.Completed && !child.Terminated)
			{

			  if (throwException)
			  {
				throw LOG.wrongChildStateException("complete", id, child.Id, "[available|enabled|suspended]");
			  }

			  return false;
			}
		  }

		  // TODO: are there any DiscretionaryItems?
		  // if yes, then it is not possible to complete
		  // this stage (NOTE: manualCompletion == false)!

		}

		return true;
	  }

	  protected internal virtual bool evaluateAutoComplete(CmmnActivityExecution execution)
	  {
		CmmnActivity activity = getActivity(execution);

		object autoCompleteProperty = activity.getProperty(PROPERTY_AUTO_COMPLETE);
		if (autoCompleteProperty != null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  string message = "Property autoComplete expression returns non-Boolean: " + autoCompleteProperty + " (" + autoCompleteProperty.GetType().FullName + ")";
		  ensureInstanceOf(message, "autoComplete", autoCompleteProperty, typeof(Boolean));

		  return (bool?) autoCompleteProperty.Value;
		}

		return false;
	  }

	  // termination //////////////////////////////////////////////////////////////

	  protected internal virtual bool isAbleToTerminate(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;

		if (children != null && children.Count > 0)
		{

		  foreach (CmmnExecution child in children)
		  {
			// the guard "!child.isCompleted()" is needed,
			// when an exitCriteria is triggered on a stage, and
			// the referenced sentry contains an onPart to a child
			// case execution which has defined as standardEvent "complete".
			// In that case the completed child case execution is still
			// in the list of child case execution of the parent case execution.
			if (!child.Terminated && !child.Completed)
			{
			  return false;
			}
		  }
		}

		return true;
	  }

	  protected internal override void performTerminate(CmmnActivityExecution execution)
	  {
		if (!isAbleToTerminate(execution))
		{
		  terminateChildren(execution);

		}
		else
		{
		  base.performTerminate(execution);
		}

	  }

	  protected internal override void performExit(CmmnActivityExecution execution)
	  {
		if (!isAbleToTerminate(execution))
		{
		  terminateChildren(execution);

		}
		else
		{
		  base.performExit(execution);
		}
	  }

	  protected internal virtual void terminateChildren(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;

		foreach (CmmnExecution child in children)
		{
		  terminateChild(child);
		}
	  }

	  protected internal virtual void terminateChild(CmmnExecution child)
	  {
		CmmnActivityBehavior behavior = getActivityBehavior(child);

		// "child.isTerminated()": during resuming the children, it can
		// happen that a sentry will be satisfied, so that a child
		// will terminated. these terminated child cannot be resumed,
		// so ignore it.
		// "child.isCompleted()": in case that an exitCriteria on caseInstance
		// (ie. casePlanModel) has been fired, when a child inside has been
		// completed, so ignore it.
		if (!child.Terminated && !child.Completed)
		{
		  if (behavior is StageOrTaskActivityBehavior)
		  {
			child.exit();

		  }
		  else
		  { // behavior instanceof EventListenerOrMilestoneActivityBehavior
			child.parentTerminate();
		  }
		}
	  }

	  // suspension /////////////////////////////////////////////////////////////////

	  protected internal override void performSuspension(CmmnActivityExecution execution)
	  {
		if (!isAbleToSuspend(execution))
		{
		  suspendChildren(execution);

		}
		else
		{
		  base.performSuspension(execution);
		}
	  }


	  protected internal override void performParentSuspension(CmmnActivityExecution execution)
	  {
		if (!isAbleToSuspend(execution))
		{
		  suspendChildren(execution);

		}
		else
		{
		  base.performParentSuspension(execution);
		}
	  }

	  protected internal virtual void suspendChildren(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;
		if (children != null && children.Count > 0)
		{

		  foreach (CmmnExecution child in children)
		  {

			CmmnActivityBehavior behavior = getActivityBehavior(child);

			// "child.isTerminated()": during resuming the children, it can
			// happen that a sentry will be satisfied, so that a child
			// will terminated. these terminated child cannot be resumed,
			// so ignore it.
			// "child.isSuspended()": maybe the child has been already
			// suspended, so ignore it.
			if (!child.Terminated && !child.Suspended)
			{
			  if (behavior is StageOrTaskActivityBehavior)
			  {
				child.parentSuspend();

			  }
			  else
			  { // behavior instanceof EventListenerOrMilestoneActivityBehavior
				child.suspend();
			  }
			}
		  }
		}
	  }

	  protected internal virtual bool isAbleToSuspend(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;

		if (children != null && children.Count > 0)
		{

		  foreach (CmmnExecution child in children)
		  {
			if (!child.Suspended)
			{
			  return false;
			}
		  }
		}

		return true;
	  }

	  // resume /////////////////////////////////////////////////////////////////////////

	  public override void resumed(CmmnActivityExecution execution)
	  {
		if (execution.Available)
		{
		  // trigger created() to check whether an exit- or
		  // entryCriteria has been satisfied in the meantime.
		  created(execution);

		}
		else if (execution.Active)
		{
		  // if the given case execution is active after resuming,
		  // then propagate it to the children.
		  resumeChildren(execution);
		}
	  }

	  protected internal virtual void resumeChildren(CmmnActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> children = execution.getCaseExecutions();
		IList<CmmnExecution> children = execution.CaseExecutions;

		if (children != null && children.Count > 0)
		{

		  foreach (CmmnExecution child in children)
		  {

			CmmnActivityBehavior behavior = getActivityBehavior(child);

			// during resuming the children, it can happen that a sentry
			// will be satisfied, so that a child will terminated. these
			// terminated child cannot be resumed, so ignore it.
			if (!child.Terminated)
			{
			  if (behavior is StageOrTaskActivityBehavior)
			  {
				child.parentResume();

			  }
			  else
			  { // behavior instanceof EventListenerOrMilestoneActivityBehavior
				child.resume();
			  }
			}
		  }
		}
	  }

	  // sentry ///////////////////////////////////////////////////////////////////////////////

	  protected internal override bool isAtLeastOneEntryCriterionSatisfied(CmmnActivityExecution execution)
	  {
		if (!execution.CaseInstanceExecution)
		{
		  return base.isAtLeastOneEntryCriterionSatisfied(execution);
		}

		return false;
	  }

	  public override void fireExitCriteria(CmmnActivityExecution execution)
	  {
		if (!execution.CaseInstanceExecution)
		{
		  execution.exit();
		}
		else
		{
		  execution.terminate();
		}
	  }

	  public override void fireEntryCriteria(CmmnActivityExecution execution)
	  {
		if (!execution.CaseInstanceExecution)
		{
		  base.fireEntryCriteria(execution);
		  return;
		}

		throw LOG.criteriaNotAllowedForCaseInstanceException("entry", execution.Id);
	  }

	  // handle child state changes ///////////////////////////////////////////////////////////

	  public virtual void handleChildCompletion(CmmnActivityExecution execution, CmmnActivityExecution child)
	  {
		fireForceUpdate(execution);

		if (execution.Active)
		{
		  checkAndCompleteCaseExecution(execution);
		}
	  }

	  public virtual void handleChildDisabled(CmmnActivityExecution execution, CmmnActivityExecution child)
	  {
		fireForceUpdate(execution);

		if (execution.Active)
		{
		  checkAndCompleteCaseExecution(execution);
		}
	  }

	  public virtual void handleChildSuspension(CmmnActivityExecution execution, CmmnActivityExecution child)
	  {
		// if the given execution is not suspending currently, then ignore this notification.
		if (execution.Suspending && isAbleToSuspend(execution))
		{
		  string id = execution.Id;
		  CaseExecutionState currentState = execution.CurrentState;

		  if (SUSPENDING_ON_SUSPENSION.Equals(currentState))
		  {
			execution.performSuspension();

		  }
		  else if (SUSPENDING_ON_PARENT_SUSPENSION.Equals(currentState))
		  {
			execution.performParentSuspension();

		  }
		  else
		  {
			throw LOG.suspendCaseException(id, currentState);
		  }
		}
	  }

	  public virtual void handleChildTermination(CmmnActivityExecution execution, CmmnActivityExecution child)
	  {
		fireForceUpdate(execution);

		if (execution.Active)
		{
		  checkAndCompleteCaseExecution(execution);

		}
		else if (execution.Terminating && isAbleToTerminate(execution))
		{
		  string id = execution.Id;
		  CaseExecutionState currentState = execution.CurrentState;

		  if (TERMINATING_ON_TERMINATION.Equals(currentState))
		  {
			execution.performTerminate();

		  }
		  else if (TERMINATING_ON_EXIT.Equals(currentState))
		  {
			execution.performExit();

		  }
		  else if (TERMINATING_ON_PARENT_TERMINATION.Equals(currentState))
		  {
			throw LOG.illegalStateTransitionException("parentTerminate", id, TypeName);

		  }
		  else
		  {
			throw LOG.terminateCaseException(id, currentState);

		  }
		}
	  }

	  protected internal virtual void checkAndCompleteCaseExecution(CmmnActivityExecution execution)
	  {
		if (canComplete(execution))
		{
		  execution.complete();
		}
	  }

	  protected internal virtual void fireForceUpdate(CmmnActivityExecution execution)
	  {
		if (execution is CaseExecutionEntity)
		{
		  CaseExecutionEntity entity = (CaseExecutionEntity) execution;
		  entity.forceUpdate();
		}
	  }

	  protected internal override string TypeName
	  {
		  get
		  {
			return "stage";
		  }
	  }

	}

}