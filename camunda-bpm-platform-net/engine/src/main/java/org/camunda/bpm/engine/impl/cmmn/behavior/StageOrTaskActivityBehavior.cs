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
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE;

	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class StageOrTaskActivityBehavior : PlanItemDefinitionActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  // creation /////////////////////////////////////////////////////////

	  protected internal override void creating(CmmnActivityExecution execution)
	  {
		evaluateRequiredRule(execution);
	  }

	  public override void created(CmmnActivityExecution execution)
	  {
		if (execution.Available && isAtLeastOneEntryCriterionSatisfied(execution))
		{
		  fireEntryCriteria(execution);
		}
	  }

	  // enable ////////////////////////////////////////////////////////////

	  public override void onEnable(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "enable");
		ensureTransitionAllowed(execution, AVAILABLE, ENABLED, "enable");
	  }

	  // re-enable /////////////////////////////////////////////////////////

	  public override void onReenable(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "re-enable");
		ensureTransitionAllowed(execution, DISABLED, ENABLED, "re-enable");
	  }

	  // disable ///////////////////////////////////////////////////////////

	  public override void onDisable(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "disable");
		ensureTransitionAllowed(execution, ENABLED, DISABLED, "disable");
	  }

	  // start /////////////////////////////////////////////////////////////

	  public override void onStart(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "start");
		ensureTransitionAllowed(execution, AVAILABLE, ACTIVE, "start");
	  }

	  public override void onManualStart(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "manualStart");
		ensureTransitionAllowed(execution, ENABLED, ACTIVE, "start");
	  }

	  public override void started(CmmnActivityExecution execution)
	  {
		// only perform start behavior, when this case execution is
		// still active.
		// it can happen that a exit sentry will be triggered, so that
		// the given case execution will be terminated, in that case we
		// do not need to perform the start behavior
		if (execution.Active)
		{
		  performStart(execution);
		}
	  }

	  protected internal abstract void performStart(CmmnActivityExecution execution);

	  // completion ////////////////////////////////////////////////////////

	  public override void onCompletion(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, COMPLETED, "complete");
		completing(execution);
	  }

	  public override void onManualCompletion(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, COMPLETED, "complete");
		manualCompleting(execution);
	  }

	  // termination //////////////////////////////////////////////////////

	  public override void onTermination(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, TERMINATED, "terminate");
		performTerminate(execution);
	  }

	  public override void onParentTermination(CmmnActivityExecution execution)
	  {
		string id = execution.Id;
		throw LOG.illegalStateTransitionException("parentTerminate", id, TypeName);
	  }

	  public override void onExit(CmmnActivityExecution execution)
	  {
		string id = execution.Id;

		if (execution.Terminated)
		{
		  throw LOG.alreadyTerminatedException("exit", id);
		}

		if (execution.Completed)
		{
		  throw LOG.wrongCaseStateException("exit", id, "[available|enabled|disabled|active|failed|suspended]", "completed");
		}

		performExit(execution);
	  }

	  // suspension ///////////////////////////////////////////////////////////

	  public override void onSuspension(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, ACTIVE, SUSPENDED, "suspend");
		performSuspension(execution);
	  }

	  public override void onParentSuspension(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "parentSuspension");

		string id = execution.Id;

		if (execution.Suspended)
		{
		  throw LOG.alreadySuspendedException("parentSuspend", id);
		}

		if (execution.Completed || execution.Terminated)
		{
		  throw LOG.wrongCaseStateException("parentSuspend", id, "suspend", "[available|enabled|disabled|active]", execution.CurrentState.ToString());
		}

		performParentSuspension(execution);
	  }

	  // resume /////////////////////////////////////////////////////////////////

	  public override void onResume(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "resume");
		ensureTransitionAllowed(execution, SUSPENDED, ACTIVE, "resume");

		CmmnActivityExecution parent = execution.Parent;
		if (parent != null)
		{
		  if (!parent.Active)
		  {
			string id = execution.Id;
			throw LOG.resumeInactiveCaseException("resume", id);
		  }
		}

		resuming(execution);

	  }

	  public override void onParentResume(CmmnActivityExecution execution)
	  {
		ensureNotCaseInstance(execution, "parentResume");
		string id = execution.Id;

		if (!execution.Suspended)
		{
		  throw LOG.wrongCaseStateException("parentResume", id, "resume", "suspended", execution.CurrentState.ToString());
		}

		CmmnActivityExecution parent = execution.Parent;
		if (parent != null)
		{
		  if (!parent.Active)
		  {
			throw LOG.resumeInactiveCaseException("parentResume", id);
		  }
		}

		resuming(execution);

	  }

	  // occur ////////////////////////////////////////////////////////

	  public override void onOccur(CmmnActivityExecution execution)
	  {
		string id = execution.Id;
		throw LOG.illegalStateTransitionException("occur", id, TypeName);
	  }

	  // sentry ///////////////////////////////////////////////////////////////

	  public override void fireEntryCriteria(CmmnActivityExecution execution)
	  {
		bool manualActivation = evaluateManualActivationRule(execution);
		if (manualActivation)
		{
		  execution.enable();

		}
		else
		{
		  execution.start();
		}
	  }

	  // manual activation rule //////////////////////////////////////////////

	  protected internal virtual bool evaluateManualActivationRule(CmmnActivityExecution execution)
	  {
		bool manualActivation = false;
		CmmnActivity activity = execution.Activity;
		object manualActivationRule = activity.getProperty(PROPERTY_MANUAL_ACTIVATION_RULE);
		if (manualActivationRule != null)
		{
		  CaseControlRule rule = (CaseControlRule) manualActivationRule;
		  manualActivation = rule.evaluate(execution);
		}
		return manualActivation;
	  }

	  // helper ///////////////////////////////////////////////////////////

	  protected internal abstract string TypeName {get;}
	}

}