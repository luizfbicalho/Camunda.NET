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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil.SIGNAL_COMPENSATION_DONE;

	using BpmnExceptionHandler = org.camunda.bpm.engine.impl.bpmn.helper.BpmnExceptionHandler;
	using ErrorPropagationException = org.camunda.bpm.engine.impl.bpmn.helper.ErrorPropagationException;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;


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
			  BpmnExceptionHandler.propagateException(execution, ex);
			}
			catch (ErrorPropagationException)
			{
			  // exception has been logged by thrower
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

	}

}