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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class BpmnBehaviorLogger : ProcessEngineLogger
	{


	  public virtual void missingBoundaryCatchEvent(string executionId, string errorCode)
	  {
		logInfo("001", "Execution with id '{}' throws an error event with errorCode '{}', but no catching boundary event was defined. " + "Execution is ended (none end event semantics).", executionId, errorCode);
	  }

	  public virtual void leavingActivity(string activityId)
	  {
		logDebug("002", "Leaving activity '{}'.", activityId);
	  }

	  public virtual void missingOutgoingSequenceFlow(string activityId)
	  {
		logDebug("003", "No outgoing sequence flow found for activity '{}'. Ending execution.", activityId);
	  }

	  public virtual ProcessEngineException stuckExecutionException(string activityId)
	  {
		return new ProcessEngineException(exceptionMessage("004", "No outgoing sequence flow for the element with id '{}' could be selected for continuing the process.", activityId));
	  }

	  public virtual ProcessEngineException missingDefaultFlowException(string activityId, string defaultSequenceFlow)
	  {
		return new ProcessEngineException(exceptionMessage("005", "Default sequence flow '{}' for element with id '{}' could not be not found.", defaultSequenceFlow, activityId));
	  }

	  public virtual ProcessEngineException missingConditionalFlowException(string activityId)
	  {
		return new ProcessEngineException(exceptionMessage("006", "No conditional sequence flow leaving the Flow Node '{}' could be selected for continuing the process.", activityId));
	  }

	  public virtual ProcessEngineException incorrectlyUsedSignalException(string className)
	  {
		return new ProcessEngineException(exceptionMessage("007", "signal() can only be called on a '{}' instance.", className));
	  }

	  public virtual ProcessEngineException missingDelegateParentClassException(string className, string javaDelegate, string activityBehavior)
	  {

		return new ProcessEngineException(exceptionMessage("008", "Class '{}' doesn't implement '{}' nor '{}'.", className, javaDelegate, activityBehavior));
	  }

	  public virtual void outgoingSequenceFlowSelected(string sequenceFlowId)
	  {
		logDebug("009", "Sequence flow with id '{}' was selected as outgoing sequence flow.", sequenceFlowId);
	  }

	  public virtual ProcessEngineException unsupportedSignalException(string activityId)
	  {
		return new ProcessEngineException(exceptionMessage("010", "The activity with id '{}' doesn't accept signals.", activityId));
	  }

	  public virtual void activityActivation(string activityId)
	  {
		logDebug("011", "Element with id '{}' activates.", activityId);
	  }

	  public virtual void noActivityActivation(string activityId)
	  {
		logDebug("012", "Element with id '{}' does not activate.", activityId);
	  }

	  public virtual void ignoringEventSubscription(EventSubscriptionEntity eventSubscription, string processDefinitionId)
	  {
		logDebug("014", "Found event subscription '{}' but process definition with id '{}' could not be found.", eventSubscription.ToString(), processDefinitionId);
	  }

	  public virtual ProcessEngineException sendingEmailException(string recipient, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("015", "Unable to send email to recipient '{}'.", recipient), cause);
	  }

	  public virtual ProcessEngineException emailFormatException()
	  {
		return new ProcessEngineException(exceptionMessage("016", "'html' or 'text' is required to be defined as mail format when using the mail activity."));
	  }

	  public virtual ProcessEngineException emailCreationException(string format, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("017", "Unable to create a mail with format '{}'.", format), cause);
	  }

	  public virtual ProcessEngineException addRecipientException(string recipient, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("018", "Unable to add '{}' as recipient.", recipient), cause);
	  }

	  public virtual ProcessEngineException missingRecipientsException()
	  {
		return new ProcessEngineException(exceptionMessage("019", "No recipient could be found for sending email."));
	  }

	  public virtual ProcessEngineException addSenderException(string sender, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("020", "Could not set '{}' as from address in email.", sender), cause);
	  }

	  public virtual ProcessEngineException addCcException(string cc, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("021", "Could not add '{}' as cc recipient.", cc), cause);
	  }

	  public virtual ProcessEngineException addBccException(string bcc, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("022", "Could not add '{}' as bcc recipient.", bcc), cause);
	  }

	  public virtual ProcessEngineException invalidAmountException(string type, int amount)
	  {
		return new ProcessEngineException(exceptionMessage("023", "Invalid number of '{}': must be positive integer value or zero, but was '{}'.", type, amount));
	  }

	  public virtual ProcessEngineException unresolvableExpressionException(string expression, string type)
	  {
		return new ProcessEngineException(exceptionMessage("024", "Expression '{}' didn't resolve to type '{}'.", expression, type));
	  }

	  public virtual ProcessEngineException invalidVariableTypeException(string variable, string type)
	  {
		return new ProcessEngineException(exceptionMessage("025", "Variable '{}' is not from type '{}'.", variable, type));
	  }

	  public virtual ProcessEngineException resolveCollectionExpressionOrVariableReferenceException()
	  {
		return new ProcessEngineException(exceptionMessage("026", "Couldn't resolve collection expression nor variable reference"));
	  }

	  public virtual ProcessEngineException expressionNotANumberException(string type, string expression)
	  {
		return new ProcessEngineException(exceptionMessage("027", "Could not resolve expression from type '{}'. Expression '{}' needs to be a number or number String.", type, expression));
	  }

	  public virtual ProcessEngineException expressionNotBooleanException(string type, string expression)
	  {
		return new ProcessEngineException(exceptionMessage("028", "Could not resolve expression from type '{}'. Expression '{}' needs to evaluate to a boolean value.", type, expression));
	  }

	  public virtual void multiInstanceCompletionConditionState(bool? state)
	  {
		logDebug("029", "Completion condition of multi-instance satisfied: '{}'", state);
	  }

	  public virtual void activityActivation(string activityId, int joinedExecutions, int availableExecution)
	  {
		logDebug("030", "Element with id '{}' activates. Joined '{}' of '{}' available executions.", activityId, joinedExecutions, availableExecution);
	  }

	  public virtual void noActivityActivation(string activityId, int joinedExecutions, int availableExecution)
	  {
		logDebug("031", "Element with id '{}' does not activate. Joined '{}' of '{}' available executions.", activityId, joinedExecutions, availableExecution);
	  }

	  public virtual ProcessEngineException unsupportedConcurrencyException(string scopeExecutionId, string className)
	  {
		return new ProcessEngineException(exceptionMessage("032", "Execution '{}' with execution behavior of class '{}' cannot have concurrency.", scopeExecutionId, className));
	  }

	  public virtual ProcessEngineException resolveDelegateExpressionException(Expression expression, Type parentClass, Type javaDelegateClass)
	  {
			  javaDelegateClass = typeof(JavaDelegate);
		return new ProcessEngineException(exceptionMessage("033", "Delegate Expression '{}' did neither resolve to an implementation of '{}' nor '{}'.", expression, parentClass, javaDelegateClass));
	  }

	  public virtual ProcessEngineException shellExecutionException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("034", "Could not execute shell command."), cause);
	  }

	  public virtual void errorPropagationException(string activityId, Exception cause)
	  {
		logError("035", "caught an exception while propagate error in activity with id '{}'", activityId, cause);
	  }

	  public virtual void debugConcurrentScopeIsPruned(PvmExecutionImpl execution)
	  {
		logDebug("036", "Concurrent scope is pruned {}", execution);
	  }

	  public virtual void debugCancelConcurrentScopeExecution(PvmExecutionImpl execution)
	  {
		logDebug("037", "Cancel concurrent scope execution {}", execution);
	  }

	  public virtual void destroyConcurrentScopeExecution(PvmExecutionImpl execution)
	  {
		logDebug("038", "Destroy concurrent scope execution", execution);
	  }

	  public virtual void completeNonScopeEventSubprocess()
	  {
		logDebug("039", "Destroy non-socpe event subprocess");
	  }

	  public virtual void endConcurrentExecutionInEventSubprocess()
	  {
		logDebug("040", "End concurrent execution in event subprocess");
	  }

	  public virtual ProcessEngineException missingDelegateVariableMappingParentClassException(string className, string delegateVarMapping)
	  {
		return new ProcessEngineException(exceptionMessage("041", "Class '{}' doesn't implement '{}'.", className, delegateVarMapping));
	  }

	  public virtual ProcessEngineException missingBoundaryCatchEventError(string executionId, string errorCode)
	  {
		return new ProcessEngineException(exceptionMessage("042", "Execution with id '{}' throws an error event with errorCode '{}', but no error handler was defined. ", executionId, errorCode));
	  }

	}

}