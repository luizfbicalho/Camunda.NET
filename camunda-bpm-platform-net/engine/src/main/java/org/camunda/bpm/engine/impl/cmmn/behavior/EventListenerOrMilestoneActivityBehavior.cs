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
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;

	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class EventListenerOrMilestoneActivityBehavior : PlanItemDefinitionActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  // enable /////////////////////////////////////////////////////////////

	  public override void onEnable(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("enable", execution);
	  }

	  // re-enable //////////////////////////////////////////////////////////

	  public override void onReenable(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("reenable", execution);
	  }

	  // disable ///////////////////////////////////////////////////////////

	  public override void onDisable(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("disable", execution);
	  }

	  // start /////////////////////////////////////////////////////////////

	  public override void onStart(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("start", execution);
	  }

	  public override void onManualStart(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("manualStart", execution);
	  }

	  // completion /////////////////////////////////////////////////////////

	  public override void onCompletion(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("complete", execution);
	  }

	  public override void onManualCompletion(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("complete", execution);
	  }

	  // termination ////////////////////////////////////////////////////////

	  public override void onTermination(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, AVAILABLE, TERMINATED, "terminate");
		performTerminate(execution);
	  }

	  public override void onParentTermination(CmmnActivityExecution execution)
	  {

		if (execution.Completed)
		{
		  string id = execution.Id;
		  throw LOG.executionAlreadyCompletedException("parentTerminate", id);
		}

		performParentTerminate(execution);
	  }

	  public override void onExit(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("exit", execution);
	  }

	  // occur /////////////////////////////////////////////////////////////////

	  public override void onOccur(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, AVAILABLE, COMPLETED, "occur");
	  }

	  // suspension ////////////////////////////////////////////////////////////

	  public override void onSuspension(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, AVAILABLE, SUSPENDED, "suspend");
		performSuspension(execution);
	  }

	  public override void onParentSuspension(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("parentSuspend", execution);
	  }

	  // resume ////////////////////////////////////////////////////////////////

	  public override void onResume(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, SUSPENDED, AVAILABLE, "resume");

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
		throw createIllegalStateTransitionException("parentResume", execution);
	  }

	  // re-activation ////////////////////////////////////////////////////////

	  public override void onReactivation(CmmnActivityExecution execution)
	  {
		throw createIllegalStateTransitionException("reactivate", execution);
	  }

	  // sentry ///////////////////////////////////////////////////////////////

	  protected internal virtual bool isAtLeastOneExitCriterionSatisfied(CmmnActivityExecution execution)
	  {
		return false;
	  }

	  public override void fireExitCriteria(CmmnActivityExecution execution)
	  {
		throw LOG.criteriaNotAllowedForEventListenerOrMilestonesException("exit", execution.Id);
	  }

	  // helper ////////////////////////////////////////////////////////////////

	  protected internal virtual CaseIllegalStateTransitionException createIllegalStateTransitionException(string transition, CmmnActivityExecution execution)
	  {
		string id = execution.Id;
		return LOG.illegalStateTransitionException(transition, id, TypeName);
	  }

	  protected internal abstract string TypeName {get;}

	}

}