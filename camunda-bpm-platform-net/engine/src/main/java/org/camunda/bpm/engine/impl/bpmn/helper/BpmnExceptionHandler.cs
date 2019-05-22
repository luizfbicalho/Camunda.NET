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
namespace org.camunda.bpm.engine.impl.bpmn.helper
{
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using BpmnBehaviorLogger = org.camunda.bpm.engine.impl.bpmn.behavior.BpmnBehaviorLogger;
	using ErrorEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ErrorEventDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using ActivityExecutionHierarchyWalker = org.camunda.bpm.engine.impl.tree.ActivityExecutionHierarchyWalker;
	using ActivityExecutionMappingCollector = org.camunda.bpm.engine.impl.tree.ActivityExecutionMappingCollector;
	using ActivityExecutionTuple = org.camunda.bpm.engine.impl.tree.ActivityExecutionTuple;
	using OutputVariablesPropagator = org.camunda.bpm.engine.impl.tree.OutputVariablesPropagator;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;

	/// <summary>
	/// Helper class handling the propagation of BPMN Errors.
	/// </summary>
	public class BpmnExceptionHandler
	{

	  private static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  /// <summary>
	  /// Decides how to propagate the exception properly, e.g. as bpmn error or "normal" error. </summary>
	  /// <param name="execution"> the current execution </param>
	  /// <param name="ex"> the exception to propagate </param>
	  /// <exception cref="Exception"> if no error handler could be found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void propagateException(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, Exception ex) throws Exception
	  public static void propagateException(ActivityExecution execution, Exception ex)
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


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void propagateExceptionAsError(Exception exception, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal static void propagateExceptionAsError(Exception exception, ActivityExecution execution)
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

	  protected internal static bool TransactionNotActive
	  {
		  get
		  {
			return !Context.CommandContext.TransactionContext.TransactionActive;
		  }
	  }

	  protected internal static bool isProcessEngineExceptionWithoutCause(Exception exception)
	  {
		return exception is ProcessEngineException && exception.InnerException == null;
	  }

	  /// <summary>
	  /// Searches recursively through the exception to see if the exception itself
	  /// or one of its causes is a <seealso cref="BpmnError"/>.
	  /// </summary>
	  /// <param name="e">
	  ///          the exception to check </param>
	  /// <returns> the BpmnError that was the cause of this exception or null if no
	  ///         BpmnError was found </returns>
	  protected internal static BpmnError checkIfCauseOfExceptionIsBpmnError(Exception e)
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


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void propagateBpmnError(org.camunda.bpm.engine.delegate.BpmnError error, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public static void propagateBpmnError(BpmnError error, ActivityExecution execution)
	  {
		propagateError(error.ErrorCode, error.Message, null, execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void propagateError(String errorCode, String errorMessage, Exception origException, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public static void propagateError(string errorCode, string errorMessage, Exception origException, ActivityExecution execution)
	  {

		ActivityExecutionHierarchyWalker walker = new ActivityExecutionHierarchyWalker(execution);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder = new ErrorDeclarationForProcessInstanceFinder(origException, errorCode, execution.getActivity());
		ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder = new ErrorDeclarationForProcessInstanceFinder(origException, errorCode, execution.Activity);
		ActivityExecutionMappingCollector activityExecutionMappingCollector = new ActivityExecutionMappingCollector(execution);

		walker.addScopePreVisitor(errorDeclarationFinder);
		walker.addExecutionPreVisitor(activityExecutionMappingCollector);
		// map variables to super executions in the hierarchy of called process instances
		walker.addExecutionPreVisitor(new OutputVariablesPropagator());

		try
		{

		  walker.walkUntil(new WalkConditionAnonymousInnerClass(errorDeclarationFinder));

		}
		catch (Exception e)
		{
		  LOG.errorPropagationException(execution.ActivityInstanceId, e);

		  // separate the exception handling to support a fail-safe error propagation
		  throw new ErrorPropagationException(e.InnerException);
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
		  private org.camunda.bpm.engine.impl.bpmn.helper.ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder;

		  public WalkConditionAnonymousInnerClass(org.camunda.bpm.engine.impl.bpmn.helper.ErrorDeclarationForProcessInstanceFinder errorDeclarationFinder)
		  {
			  this.errorDeclarationFinder = errorDeclarationFinder;
		  }


		  public bool isFulfilled(ActivityExecutionTuple element)
		  {
			return errorDeclarationFinder.ErrorEventDefinition != null || element == null;
		  }
	  }


	}
}