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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil.SIGNAL_COMPENSATION_DONE;


	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ErrorEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ErrorEventDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using ActivityExecutionHierarchyWalker = org.camunda.bpm.engine.impl.tree.ActivityExecutionHierarchyWalker;
	using ActivityExecutionMappingCollector = org.camunda.bpm.engine.impl.tree.ActivityExecutionMappingCollector;
	using ActivityExecutionTuple = org.camunda.bpm.engine.impl.tree.ActivityExecutionTuple;
	using OutputVariablesPropagator = org.camunda.bpm.engine.impl.tree.OutputVariablesPropagator;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;


	/// <summary>
	/// Denotes an 'activity' in the sense of BPMN 2.0:
	/// a parent class for all tasks, subprocess and callActivity.
	/// 
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class AbstractBpmnActivityBehavior : FlowNodeActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  /// <summary>
	  /// Subclasses that call leave() will first pass through this method, before
	  /// the regular <seealso cref="FlowNodeActivityBehavior#leave(ActivityExecution)"/> is
	  /// called.
	  /// </summary>
	  public override void doLeave(ActivityExecution execution)
	  {

		PvmActivity currentActivity = execution.Activity;
		ActivityImpl compensationHandler = ((ActivityImpl) currentActivity).findCompensationHandler();

		// subscription for compensation event subprocess is already created
		if (compensationHandler != null && !isCompensationEventSubprocess(compensationHandler))
		{
		  createCompensateEventSubscription(execution, compensationHandler);
		}
		base.doLeave(execution);
	  }

	  protected internal virtual bool isCompensationEventSubprocess(ActivityImpl activity)
	  {
		return activity.CompensationHandler && activity.SubProcessScope && activity.TriggeredByEvent;
	  }

	  protected internal virtual void createCompensateEventSubscription(ActivityExecution execution, ActivityImpl compensationHandler)
	  {
		// the compensate event subscription is created at subprocess or miBody of the the current activity
		PvmActivity currentActivity = execution.Activity;
		ActivityExecution scopeExecution = execution.findExecutionForFlowScope(currentActivity.FlowScope);

		EventSubscriptionEntity.createAndInsert((ExecutionEntity) scopeExecution, EventType.COMPENSATE, compensationHandler);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void propagateExceptionAsError(Exception exception, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void propagateExceptionAsError(Exception exception, ActivityExecution execution)
	  {
		if (isProcessEngineExceptionWithoutCause(exception) || TransactionNotActive)
		{
		  throw exception;
		}
		else
		{
		  propagateError(null, exception.Message,exception, execution);
		}
	  }

	  /// <summary>
	  /// Takes an <seealso cref="ActivityExecution"/> and an <seealso cref="Callable"/> and wraps
	  /// the call to the Callable with the proper error propagation. This method
	  /// also makes sure that exceptions not caught by following activities in the
	  /// process will be thrown and not propagated.
	  /// </summary>
	  /// <param name="execution"> </param>
	  /// <param name="toExecute"> </param>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void executeWithErrorPropagation(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, java.util.concurrent.Callable<Void> toExecute) throws Exception
	  protected internal virtual void executeWithErrorPropagation(ActivityExecution execution, Callable<Void> toExecute)
	  {
		string activityInstanceId = execution.ActivityInstanceId;
		try
		{
		  toExecute.call();
		}
		catch (Exception ex)
		{
		  if (activityInstanceId.Equals(execution.ActivityInstanceId))
		  {

			try
			{
			  propagateException(execution, ex);
			}
			catch (ErrorPropagationException e)
			{
			  LOG.errorPropagationException(activityInstanceId, e.InnerException);
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
	  }

	  /// <summary>
	  /// Decides how to propagate the exception properly, e.g. as bpmn error or "normal" error. </summary>
	  /// <param name="execution"> the current execution </param>
	  /// <param name="ex"> the exception to propagate </param>
	  /// <exception cref="Exception"> if no error handler could be found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void propagateException(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, Exception ex) throws Exception
	  protected internal virtual void propagateException(ActivityExecution execution, Exception ex)
	  {
		BpmnError bpmnError = checkIfCauseOfExceptionIsBpmnError(ex);
		if (bpmnError != null)
		{
		  propagateBpmnError(bpmnError, execution);
		}
		else
		{
		  propagateExceptionAsError(ex, execution);
		}
	  }

	  /// <summary>
	  /// Searches recursively through the exception to see if the exception itself
	  /// or one of its causes is a <seealso cref="BpmnError"/>.
	  /// </summary>
	  /// <param name="e">
	  ///          the exception to check </param>
	  /// <returns> the BpmnError that was the cause of this exception or null if no
	  ///         BpmnError was found </returns>
	  protected internal virtual BpmnError checkIfCauseOfExceptionIsBpmnError(Exception e)
	  {
		if (e is BpmnError)
		{
		  return (BpmnError) e;
		}
		else if (e.InnerException == null)
		{
		  return null;
		}
		return checkIfCauseOfExceptionIsBpmnError(e.InnerException);
	  }

	  protected internal virtual bool TransactionNotActive
	  {
		  get
		  {
			return !Context.CommandContext.TransactionContext.TransactionActive;
		  }
	  }

	  protected internal virtual bool isProcessEngineExceptionWithoutCause(Exception exception)
	  {
		return exception is ProcessEngineException && exception.InnerException == null;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void propagateBpmnError(org.camunda.bpm.engine.delegate.BpmnError error, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void propagateBpmnError(BpmnError error, ActivityExecution execution)
	  {
		propagateError(error.ErrorCode, error.Message, null, execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void propagateError(String errorCode, String errorMessage, Exception origException, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void propagateError(string errorCode, string errorMessage, Exception origException, ActivityExecution execution)
	  {

		ActivityExecutionHierarchyWalker walker = new ActivityExecutionHierarchyWalker(execution);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder = new ErrorDeclarationForProcessInstanceFinder(origException, errorCode, execution.getActivity());
		ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder = new ErrorDeclarationForProcessInstanceFinder(this, origException, errorCode, execution.Activity);
		ActivityExecutionMappingCollector activityExecutionMappingCollector = new ActivityExecutionMappingCollector(execution);

		walker.addScopePreVisitor(errorDeclarationFinder);
		walker.addExecutionPreVisitor(activityExecutionMappingCollector);
		// map variables to super executions in the hierarchy of called process instances
		walker.addExecutionPreVisitor(new OutputVariablesPropagator());

		try
		{

		  walker.walkUntil(new WalkConditionAnonymousInnerClass(this, errorDeclarationFinder));

		}
		catch (Exception e)
		{
		  // separate the exception handling to support a fail-safe error propagation
		  throw new ErrorPropagationException(this, e);
		}

		PvmActivity errorHandlingActivity = errorDeclarationFinder.ErrorHandlerActivity;

		// process the error
		if (errorHandlingActivity == null)
		{
		  if (origException == null)
		  {

			if (Context.CommandContext.ProcessEngineConfiguration.EnableExceptionsAfterUnhandledBpmnError)
			{
			  throw LOG.missingBoundaryCatchEventError(execution.Activity.Id, errorCode);
			}
			else
			{
			  LOG.missingBoundaryCatchEvent(execution.Activity.Id, errorCode);
			  execution.end(true);
			}
		  }
		  else
		  {
			// throw original exception
			throw origException;
		  }
		}
		else
		{

		  ErrorEventDefinition errorDefinition = errorDeclarationFinder.ErrorEventDefinition;
		  PvmExecutionImpl errorHandlingExecution = activityExecutionMappingCollector.getExecutionForScope(errorHandlingActivity.EventScope);

		  if (!string.ReferenceEquals(errorDefinition.ErrorCodeVariable, null))
		  {
			errorHandlingExecution.setVariable(errorDefinition.ErrorCodeVariable, errorCode);
		  }
		  if (!string.ReferenceEquals(errorDefinition.ErrorMessageVariable, null))
		  {
			errorHandlingExecution.setVariable(errorDefinition.ErrorMessageVariable, errorMessage);
		  }
		  errorHandlingExecution.executeActivity(errorHandlingActivity);
		}
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ActivityExecutionTuple>
	  {
		  private readonly AbstractBpmnActivityBehavior outerInstance;

		  private org.camunda.bpm.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior.ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder;

		  public WalkConditionAnonymousInnerClass(AbstractBpmnActivityBehavior outerInstance, org.camunda.bpm.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior.ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder)
		  {
			  this.outerInstance = outerInstance;
			  this.errorDeclarationFinder = errorDeclarationFinder;
		  }


		  public bool isFulfilled(ActivityExecutionTuple element)
		  {
			return errorDeclarationFinder.ErrorEventDefinition != null || element == null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		if (SIGNAL_COMPENSATION_DONE.Equals(signalName))
		{
		  signalCompensationDone(execution);
		}
		else
		{
		  base.signal(execution, signalName, signalData);
		}
	  }

	  protected internal virtual void signalCompensationDone(ActivityExecution execution)
	  {
		// default behavior is to join compensating executions and propagate the signal if all executions have compensated

		// only wait for non-event-scope executions cause a compensation event subprocess consume the compensation event and
		// do not have to compensate embedded subprocesses (which are still non-event-scope executions)

		if (((PvmExecutionImpl) execution).NonEventScopeExecutions.Count == 0)
		{
		  if (execution.Parent != null)
		  {
			ActivityExecution parent = execution.Parent;
			execution.remove();
			parent.signal(SIGNAL_COMPENSATION_DONE, null);
		  }
		}
		else
		{
		  ((ExecutionEntity)execution).forceUpdate();
		}

	  }

	  public class ErrorDeclarationForProcessInstanceFinder : TreeVisitor<PvmScope>
	  {
		  private readonly AbstractBpmnActivityBehavior outerInstance;


		protected internal Exception exception;
		protected internal string errorCode;
		protected internal PvmActivity errorHandlerActivity;
		protected internal ErrorEventDefinition errorEventDefinition;
		protected internal PvmActivity currentActivity;

		public ErrorDeclarationForProcessInstanceFinder(AbstractBpmnActivityBehavior outerInstance, Exception exception, string errorCode, PvmActivity currentActivity)
		{
			this.outerInstance = outerInstance;
		  this.exception = exception;
		  this.errorCode = errorCode;
		  this.currentActivity = currentActivity;
		}

		public virtual void visit(PvmScope scope)
		{
		  IList<ErrorEventDefinition> errorEventDefinitions = scope.Properties.get(BpmnProperties.ERROR_EVENT_DEFINITIONS);
		  foreach (ErrorEventDefinition errorEventDefinition in errorEventDefinitions)
		  {
			PvmActivity activityHandler = scope.ProcessDefinition.findActivity(errorEventDefinition.HandlerActivityId);
			if ((!isReThrowingErrorEventSubprocess(activityHandler)) && ((exception != null && errorEventDefinition.catchesException(exception)) || (exception == null && errorEventDefinition.catchesError(errorCode))))
			{

			  errorHandlerActivity = activityHandler;
			  this.errorEventDefinition = errorEventDefinition;
			  break;
			}
		  }
		}

		protected internal virtual bool isReThrowingErrorEventSubprocess(PvmActivity activityHandler)
		{
		  ScopeImpl activityHandlerScope = (ScopeImpl)activityHandler;
		  return activityHandlerScope.isAncestorFlowScopeOf((ScopeImpl)currentActivity);
		}

		public virtual PvmActivity ErrorHandlerActivity
		{
			get
			{
			  return errorHandlerActivity;
			}
		}

		public virtual ErrorEventDefinition ErrorEventDefinition
		{
			get
			{
			  return errorEventDefinition;
			}
		}
	  }

	  protected internal class ErrorPropagationException : Exception
	  {
		  private readonly AbstractBpmnActivityBehavior outerInstance;


		internal const long serialVersionUID = 1L;

		public ErrorPropagationException(AbstractBpmnActivityBehavior outerInstance, Exception cause) : base(cause)
		{
			this.outerInstance = outerInstance;
		}
	  }

	}

}