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
namespace org.camunda.bpm.engine.impl.cmmn.operation
{
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CmmnAtomicOperation : CoreAtomicOperation<CmmnExecution>
	{

	  // lifecycle

	  // terminating

	  // suspending

	  // sentry


	  // delete cascade a case execution

	  void execute(CmmnExecution execution);

	  bool isAsync(CmmnExecution execution);

	}

	public static class CmmnAtomicOperation_Fields
	{
	  public static readonly CmmnAtomicOperation CASE_INSTANCE_CREATE = new AtomicOperationCaseInstanceCreate();
	  public static readonly CmmnAtomicOperation CASE_INSTANCE_CLOSE = new AtomicOperationCaseInstanceClose();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_CREATE = new AtomicOperationCaseExecutionCreate();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_CREATED = new AtomicOperationCaseExecutionCreated();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_ENABLE = new AtomicOperationCaseExecutionEnable();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_RE_ENABLE = new AtomicOperationCaseExecutionReenable();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_DISABLE = new AtomicOperationCaseExecutionDisable();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_START = new AtomicOperationCaseExecutionStart();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_MANUAL_START = new AtomicOperationCaseExecutionManualStart();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_COMPLETE = new AtomicOperationCaseExecutionComplete();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_MANUAL_COMPLETE = new AtomicOperationCaseExecutionManualComplete();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_OCCUR = new AtomicOperationCaseExecutionOccur();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_TERMINATE = new AtomicOperationCaseExecutionTerminate();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_PARENT_TERMINATE = new AtomicOperationCaseExecutionParentTerminate();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_EXIT = new AtomicOperationCaseExecutionExit();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_SUSPEND = new AtomicOperationCaseExecutionSuspend();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_PARENT_SUSPEND = new AtomicOperationCaseExecutionParentSuspend();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_RESUME = new AtomicOperationCaseExecutionResume();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_PARENT_RESUME = new AtomicOperationCaseExecutionParentResume();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_RE_ACTIVATE = new AtomicOperationCaseExecutionReactivate();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_TERMINATING_ON_TERMINATION = new AtomicOperationCaseExecutionTerminatingOnTermination();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_TERMINATING_ON_PARENT_TERMINATION = new AtomicOperationCaseExecutionTerminatingOnParentTermination();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_TERMINATING_ON_EXIT = new AtomicOperationCaseExecutionTerminatingOnExit();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_PARENT_COMPLETE = new AtomicOperationCaseExecutionParentComplete();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_SUSPENDING_ON_SUSPENSION = new AtomicOperationCaseExecutionSuspendingOnSuspension();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_SUSPENDING_ON_PARENT_SUSPENSION = new AtomicOperationCaseExecutionSuspendingOnParentSuspension();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_FIRE_ENTRY_CRITERIA = new AtomicOperationCaseExecutionFireEntryCriteria();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_FIRE_EXIT_CRITERIA = new AtomicOperationCaseExecutionFireExitCriteria();
	  public static readonly CmmnAtomicOperation CASE_EXECUTION_DELETE_CASCADE = new AtomicOperationCaseExecutionDeleteCascade();
	}

}