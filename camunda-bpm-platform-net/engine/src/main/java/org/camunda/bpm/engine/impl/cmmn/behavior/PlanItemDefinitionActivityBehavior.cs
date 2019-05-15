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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.NEW;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_REPETITION_RULE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_REQUIRED_RULE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CmmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.CmmnProperties;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class PlanItemDefinitionActivityBehavior : CmmnActivityBehavior
	{
		public abstract void fireExitCriteria(CmmnActivityExecution execution);
		public abstract void fireEntryCriteria(CmmnActivityExecution execution);
		public abstract void onReactivation(CmmnActivityExecution execution);
		public abstract void onParentResume(CmmnActivityExecution execution);
		public abstract void onResume(CmmnActivityExecution execution);
		public abstract void onParentSuspension(CmmnActivityExecution execution);
		public abstract void onSuspension(CmmnActivityExecution execution);
		public abstract void onOccur(CmmnActivityExecution execution);
		public abstract void onExit(CmmnActivityExecution execution);
		public abstract void onParentTermination(CmmnActivityExecution execution);
		public abstract void onTermination(CmmnActivityExecution execution);
		public abstract void onManualCompletion(CmmnActivityExecution execution);
		public abstract void onCompletion(CmmnActivityExecution execution);
		public abstract void onManualStart(CmmnActivityExecution execution);
		public abstract void onStart(CmmnActivityExecution execution);
		public abstract void onDisable(CmmnActivityExecution execution);
		public abstract void onReenable(CmmnActivityExecution execution);
		public abstract void onEnable(CmmnActivityExecution execution);
		public abstract void created(CmmnActivityExecution execution);

	  protected internal static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution execution) throws Exception
	  public virtual void execute(CmmnActivityExecution execution)
	  {
		// nothing to do!
	  }

	  // sentries //////////////////////////////////////////////////////////////////////////////

	  protected internal virtual bool isAtLeastOneEntryCriterionSatisfied(CmmnActivityExecution execution)
	  {
		if (execution.EntryCriterionSatisfied)
		{
		  return true;
		}

		CmmnActivity activity = getActivity(execution);
		IList<CmmnSentryDeclaration> criteria = activity.EntryCriteria;
		return !(criteria != null && criteria.Count > 0);
	  }

	  // rules (required and repetition rule) /////////////////////////////////////////

	  protected internal virtual void evaluateRequiredRule(CmmnActivityExecution execution)
	  {
		CmmnActivity activity = execution.Activity;

		object requiredRule = activity.getProperty(PROPERTY_REQUIRED_RULE);
		if (requiredRule != null)
		{
		  CaseControlRule rule = (CaseControlRule) requiredRule;
		  bool required = rule.evaluate(execution);
		  execution.Required = required;
		}
	  }

	  protected internal virtual bool evaluateRepetitionRule(CmmnActivityExecution execution)
	  {
		CmmnActivity activity = execution.Activity;

		object repetitionRule = activity.getProperty(PROPERTY_REPETITION_RULE);
		if (repetitionRule != null)
		{
		  CaseControlRule rule = (CaseControlRule) repetitionRule;
		  return rule.evaluate(execution);
		}

		return false;
	  }

	  // creation ///////////////////////////////////////////////////////////////

	  public virtual void onCreate(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, NEW, AVAILABLE, "create");
		creating(execution);
	  }


	  protected internal virtual void creating(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  // start /////////////////////////////////////////////////////////////////

	  public virtual void started(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  // completion //////////////////////////////////////////////////////////////

	  protected internal virtual void completing(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  protected internal virtual void manualCompleting(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  // close ///////////////////////////////////////////////////////////////////

	  public virtual void onClose(CmmnActivityExecution execution)
	  {
		string id = execution.Id;
		if (execution.CaseInstanceExecution)
		{

		  if (execution.Closed)
		  {
			throw LOG.alreadyClosedCaseException("close", id);
		  }

		  if (execution.Active)
		  {
			throw LOG.wrongCaseStateException("close", id, "[completed|terminated|suspended]", "active");
		  }

		}
		else
		{
		  throw LOG.notACaseInstanceException("close", id);
		}
	  }

	  // termination ////////////////////////////////////////////////////////////

	  protected internal virtual void performTerminate(CmmnActivityExecution execution)
	  {
		execution.performTerminate();
	  }

	  protected internal virtual void performParentTerminate(CmmnActivityExecution execution)
	  {
		execution.performParentTerminate();
	  }

	  protected internal virtual void performExit(CmmnActivityExecution execution)
	  {
		execution.performExit();
	  }

	  // suspension ///////////////////////////////////////////////////////////////

	  protected internal virtual void performSuspension(CmmnActivityExecution execution)
	  {
		execution.performSuspension();
	  }

	  protected internal virtual void performParentSuspension(CmmnActivityExecution execution)
	  {
		execution.performParentSuspension();
	  }

	  // resume /////////////////////////////////////////////////////////////////

	  protected internal virtual void resuming(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  public virtual void resumed(CmmnActivityExecution execution)
	  {
		if (execution.Available)
		{
		  // trigger created() to check whether an exit- or
		  // entryCriteria has been satisfied in the meantime.
		  created(execution);
		}
	  }

	  // re-activation ///////////////////////////////////////////////////////////

	  public virtual void reactivated(CmmnActivityExecution execution)
	  {
		// noop
	  }

	  // repetition ///////////////////////////////////////////////////////////////

	  public virtual void repeat(CmmnActivityExecution execution, string standardEvent)
	  {
		CmmnActivity activity = execution.Activity;
		bool repeat = false;

		if (activity.EntryCriteria.Count == 0)
		{
		  IList<string> events = activity.Properties.get(CmmnProperties.REPEAT_ON_STANDARD_EVENTS);
		  if (events != null && events.Contains(standardEvent))
		  {
			repeat = evaluateRepetitionRule(execution);
		  }
		}
		else
		{

		  if (ENABLE.Equals(standardEvent) || START.Equals(standardEvent) || OCCUR.Equals(standardEvent))
		  {
			repeat = evaluateRepetitionRule(execution);
		  }
		}

		if (repeat)
		{

		  CmmnActivityExecution parent = execution.Parent;

		  // instantiate a new instance of given activity
		  IList<CmmnExecution> children = parent.createChildExecutions(Arrays.asList(activity));
		  // start the lifecycle of the new instance
		  parent.triggerChildExecutionsLifecycle(children);
		}

	  }

	  // helper //////////////////////////////////////////////////////////////////////

	  protected internal virtual void ensureTransitionAllowed(CmmnActivityExecution execution, CaseExecutionState expected, CaseExecutionState target, string transition)
	  {
		string id = execution.Id;

		CaseExecutionState currentState = execution.CurrentState;

		// the state "suspending" or "terminating" will set immediately
		// inside the corresponding AtomicOperation, that's why the
		// previous state will be used to ensure that the transition
		// is allowed.
		if (execution.Terminating || execution.Suspending)
		{
		  currentState = execution.PreviousState;
		}

		// is the case execution already in the target state
		if (target.Equals(currentState))
		{
		  throw LOG.isAlreadyInStateException(transition, id, target);

		}
		else
		{
		// is the case execution in the expected state
		if (!expected.Equals(currentState))
		{
		  throw LOG.unexpectedStateException(transition, id, expected, currentState);
		}
		}
	  }

	  protected internal virtual void ensureNotCaseInstance(CmmnActivityExecution execution, string transition)
	  {
		if (execution.CaseInstanceExecution)
		{
		  string id = execution.Id;
		  throw LOG.impossibleTransitionException(transition, id);
		}
	  }

	  protected internal virtual CmmnActivity getActivity(CmmnActivityExecution execution)
	  {
		string id = execution.Id;
		CmmnActivity activity = execution.Activity;
		ensureNotNull(typeof(PvmException), "Case execution '" + id + "': has no current activity.", "activity", activity);

		return activity;
	  }

	}

}