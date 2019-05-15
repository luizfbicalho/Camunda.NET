using System;

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
	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class CmmnBehaviorLogger : ProcessEngineLogger
	{

	  protected internal readonly string caseStateTransitionMessage = "Could not perform transition '{} on case execution with id '{}'.";

	  public virtual ProcessEngineException ruleExpressionNotBooleanException(object result)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new ProcessEngineException(exceptionMessage("001", "Rule expression returns a non-boolean value. Value: '{}', Class: '{}'", result, result.GetType().FullName));
	  }

	  public virtual CaseIllegalStateTransitionException forbiddenManualCompletitionException(string transition, string id, string type)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("002", caseStateTransitionMessage + "Reason: It is not possible to manually complete the case execution which is associated with an element of type {}.", transition, id, type));
	  }

	  public virtual CaseIllegalStateTransitionException criteriaNotAllowedException(string criteria, string id, string additionalMessage)
	  {

		return new CaseIllegalStateTransitionException(exceptionMessage("003", "Cannot trigger case execution with id '{}' because {} criteria is not allowed for {}.", id, criteria, additionalMessage));
	  }

	  public virtual CaseIllegalStateTransitionException criteriaNotAllowedForEventListenerOrMilestonesException(string criteria, string id)
	  {
		return criteriaNotAllowedException(criteria, id, "event listener or milestones");
	  }

	  public virtual CaseIllegalStateTransitionException criteriaNotAllowedForEventListenerException(string criteria, string id)
	  {
		return criteriaNotAllowedException(criteria, id, "event listener");
	  }

	  public virtual CaseIllegalStateTransitionException criteriaNotAllowedForCaseInstanceException(string criteria, string id)
	  {
		return criteriaNotAllowedException(criteria, id, "case instances");
	  }

	  internal virtual CaseIllegalStateTransitionException executionAlreadyCompletedException(string transition, string id)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("004", caseStateTransitionMessage + "Reason: Case execution must be available or suspended, but was completed.", transition, id));

	  }

	  public virtual CaseIllegalStateTransitionException resumeInactiveCaseException(string transition, string id)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("005", caseStateTransitionMessage + "Reason: It is not possible to resume the case execution which parent is not active.", transition, id));
	  }

	  public virtual CaseIllegalStateTransitionException illegalStateTransitionException(string transition, string id, string typeName)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("006", caseStateTransitionMessage + "Reason: It is not possible to {} the case execution which is associated with a {}", transition, id, transition, typeName));
	  }

	  public virtual CaseIllegalStateTransitionException alreadyStateCaseException(string transition, string id, string state)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("007", caseStateTransitionMessage + "Reason: The case instance is already {}.", transition, id, state));
	  }

	  public virtual CaseIllegalStateTransitionException alreadyClosedCaseException(string transition, string id)
	  {
		return alreadyStateCaseException(transition, id, "closed");
	  }

	  public virtual CaseIllegalStateTransitionException alreadyActiveException(string transition, string id)
	  {
		return alreadyStateCaseException(transition, id, "active");
	  }

	  public virtual CaseIllegalStateTransitionException alreadyTerminatedException(string transition, string id)
	  {
		return alreadyStateCaseException(transition, id, "terminated");
	  }

	  public virtual CaseIllegalStateTransitionException alreadySuspendedException(string transition, string id)
	  {
		return alreadyStateCaseException(transition, id, "suspended");
	  }

	  public virtual CaseIllegalStateTransitionException wrongCaseStateException(string transition, string id, string acceptedState, string currentState)
	  {
		return wrongCaseStateException(transition, id, transition, acceptedState, currentState);
	  }

	  public virtual CaseIllegalStateTransitionException wrongCaseStateException(string transition, string id, string altTransition, string acceptedState, string currentState)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("008", caseStateTransitionMessage + "Reason: The case instance must be in state '{}' to {} it, but the state is '{}'.", transition, id, acceptedState, transition, currentState));
	  }

	  public virtual CaseIllegalStateTransitionException notACaseInstanceException(string transition, string id)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("009", caseStateTransitionMessage + "Reason: It is not possible to close a case execution which is not a case instance.", transition, id));
	  }

	  public virtual CaseIllegalStateTransitionException isAlreadyInStateException(string transition, string id, CaseExecutionState state)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("010", caseStateTransitionMessage + "Reason: The case execution is already in state '{}'.", transition, id, state));
	  }

	  public virtual CaseIllegalStateTransitionException unexpectedStateException(string transition, string id, CaseExecutionState expectedState, CaseExecutionState currentState)
	  {

		return new CaseIllegalStateTransitionException(exceptionMessage("011", caseStateTransitionMessage + "Reason: The case execution must be in state '{}' to {}, but it was in state '{}'", transition, id, expectedState, transition, currentState));
	  }

	  public virtual CaseIllegalStateTransitionException impossibleTransitionException(string transition, string id)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("012", caseStateTransitionMessage + "Reason: The transition is not possible for this case instance.", transition, id));
	  }



	  public virtual CaseIllegalStateTransitionException remainingChildException(string transition, string id, string childId, CaseExecutionState childState)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("013", caseStateTransitionMessage + "Reason: There is a child case execution with id '{}' in state '{}'", transition, id, childId, childState));
	  }

	  public virtual CaseIllegalStateTransitionException wrongChildStateException(string transition, string id, string childId, string stateList)
	  {
		return new CaseIllegalStateTransitionException(exceptionMessage("014", caseStateTransitionMessage + "Reason: There is a child case execution with id '{}' which is in one of the following states: {}", transition, id, childId, stateList));
	  }

	  public virtual PvmException transitCaseException(string transition, string id, CaseExecutionState currentState)
	  {
		return new PvmException(exceptionMessage("015", caseStateTransitionMessage + "Reason: Expected case execution state to be {terminatingOnTermination|terminatingOnExit} but it was '{}'.", transition, id, currentState));
	  }

	  public virtual PvmException suspendCaseException(string id, CaseExecutionState currentState)
	  {
		return transitCaseException("suspend", id, currentState);
	  }

	  public virtual PvmException terminateCaseException(string id, CaseExecutionState currentState)
	  {
		return transitCaseException("terminate", id, currentState);
	  }

	  public virtual ProcessEngineException missingDelegateParentClassException(string className, string parentClass)
	  {
		return new ProcessEngineException(exceptionMessage("016", "Class '{}' doesn't implement '{}'.", className, parentClass));
	  }

	  public virtual System.NotSupportedException unsupportedTransientOperationException(string className)
	  {
		return new System.NotSupportedException(exceptionMessage("017", "Class '{}' is not supported in transient CaseExecutionImpl", className));
	  }

	  public virtual ProcessEngineException invokeVariableListenerException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("018", "Variable listener invocation failed. Reason: {}", cause.Message), cause);
	  }

	  public virtual ProcessEngineException decisionDefinitionEvaluationFailed(CmmnActivityExecution execution, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("019", "Could not evaluate decision in case execution '" + execution.Id + "'. Reason: {}", cause.Message), cause);
	  }

	}

}