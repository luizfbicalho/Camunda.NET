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
namespace org.camunda.bpm.engine.impl.interceptor
{
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;

	/// <summary>
	/// An invocation of an atomic operation
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AtomicOperationInvocation
	{

	  private static readonly ContextLogger LOG = ProcessEngineLogger.CONTEXT_LOGGER;

	  protected internal AtomicOperation operation;

	  protected internal ExecutionEntity execution;

	  protected internal bool performAsync;

	  // for logging
	  protected internal string applicationContextName = null;
	  protected internal string activityId = null;
	  protected internal string activityName = null;

	  public AtomicOperationInvocation(AtomicOperation operation, ExecutionEntity execution, bool performAsync)
	  {
		init(operation, execution, performAsync);
	  }

	  protected internal virtual void init(AtomicOperation operation, ExecutionEntity execution, bool performAsync)
	  {
		this.operation = operation;
		this.execution = execution;
		this.performAsync = performAsync;
	  }

	  public virtual void execute(BpmnStackTrace stackTrace)
	  {

		if (operation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CANCEL_SCOPE && operation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_INTERRUPT_SCOPE && operation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CONCURRENT && operation != org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE)
		{
		  // execution might be replaced in the meantime:
		  ExecutionEntity replacedBy = execution.ReplacedBy;
		  if (replacedBy != null)
		  {
			execution = replacedBy;
		  }
		}

		//execution was canceled for example via terminate end event
		if (execution.Canceled && (operation == org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END || operation == org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_NOTIFY_LISTENER_END))
		{
		  return;
		}

		// execution might have ended in the meanwhile
		if (execution.Ended && (operation == org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE || operation == org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE))
		{
		  return;
		}

		ProcessApplicationReference currentPa = Context.CurrentProcessApplication;
		if (currentPa != null)
		{
		  applicationContextName = currentPa.Name;
		}
		activityId = execution.ActivityId;
		activityName = execution.CurrentActivityName;
		stackTrace.add(this);

		try
		{
		  Context.ExecutionContext = execution;
		  if (!performAsync)
		  {
			LOG.debugExecutingAtomicOperation(operation, execution);
			operation.execute(execution);
		  }
		  else
		  {
			execution.scheduleAtomicOperationAsync(this);
		  }
		}
		finally
		{
		  Context.removeExecutionContext();
		}
	  }

	  // getters / setters ////////////////////////////////////

	  public virtual AtomicOperation Operation
	  {
		  get
		  {
			return operation;
		  }
	  }

	  public virtual ExecutionEntity Execution
	  {
		  get
		  {
			return execution;
		  }
	  }

	  public virtual bool PerformAsync
	  {
		  get
		  {
			return performAsync;
		  }
	  }

	  public virtual string ApplicationContextName
	  {
		  get
		  {
			return applicationContextName;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	}

}