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
namespace org.camunda.bpm.engine.impl.pvm.runtime.operation
{
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public interface PvmAtomicOperation : CoreAtomicOperation<PvmExecutionImpl>, AtomicOperation
	{
	}

	public static class PvmAtomicOperation_Fields
	{
	  public static readonly PvmAtomicOperation PROCESS_START = new PvmAtomicOperationProcessStart();
	  public static readonly PvmAtomicOperation FIRE_PROCESS_START = new PvmAtomicOperationFireProcessStart();
	  public static readonly PvmAtomicOperation PROCESS_END = new PvmAtomicOperationProcessEnd();
	  public static readonly PvmAtomicOperation ACTIVITY_START = new PvmAtomicOperationActivityStart();
	  public static readonly PvmAtomicOperation ACTIVITY_START_CONCURRENT = new PvmAtomicOperationActivityStartConcurrent();
	  public static readonly PvmAtomicOperation ACTIVITY_START_CANCEL_SCOPE = new PvmAtomicOperationActivityStartCancelScope();
	  public static readonly PvmAtomicOperation ACTIVITY_START_INTERRUPT_SCOPE = new PvmAtomicOperationActivityStartInterruptEventScope();
	  public static readonly PvmAtomicOperation ACTIVITY_START_CREATE_SCOPE = new PvmAtomicOperationActivityStartCreateScope();
	  public static readonly PvmAtomicOperation ACTIVITY_INIT_STACK_NOTIFY_LISTENER_START = new PvmAtomicOperationActivityInitStackNotifyListenerStart();
	  public static readonly PvmAtomicOperation ACTIVITY_INIT_STACK_NOTIFY_LISTENER_RETURN = new PvmAtomicOperationActivityInitStackNotifyListenerReturn();
	  public static readonly PvmAtomicOperation ACTIVITY_INIT_STACK = new PvmAtomicOperationActivityInitStack(ACTIVITY_INIT_STACK_NOTIFY_LISTENER_START);
	  public static readonly PvmAtomicOperation ACTIVITY_INIT_STACK_AND_RETURN = new PvmAtomicOperationActivityInitStack(ACTIVITY_INIT_STACK_NOTIFY_LISTENER_RETURN);
	  public static readonly PvmAtomicOperation ACTIVITY_EXECUTE = new PvmAtomicOperationActivityExecute();
	  public static readonly PvmAtomicOperation ACTIVITY_NOTIFY_LISTENER_END = new PvmAtomicOperationActivityNotifyListenerEnd();
	  public static readonly PvmAtomicOperation ACTIVITY_END = new PvmAtomicOperationActivityEnd();
	  public static readonly PvmAtomicOperation FIRE_ACTIVITY_END = new PvmAtomicOperationFireActivityEnd();
	  public static readonly PvmAtomicOperation TRANSITION_NOTIFY_LISTENER_END = new PvmAtomicOperationTransitionNotifyListenerEnd();
	  public static readonly PvmAtomicOperation TRANSITION_DESTROY_SCOPE = new PvmAtomicOperationTransitionDestroyScope();
	  public static readonly PvmAtomicOperation TRANSITION_NOTIFY_LISTENER_TAKE = new PvmAtomicOperationTransitionNotifyListenerTake();
	  public static readonly PvmAtomicOperation TRANSITION_START_NOTIFY_LISTENER_TAKE = new PvmAtomicOperationStartTransitionNotifyListenerTake();
	  public static readonly PvmAtomicOperation TRANSITION_CREATE_SCOPE = new PvmAtomicOperationTransitionCreateScope();
	  public static readonly PvmAtomicOperation TRANSITION_INTERRUPT_FLOW_SCOPE = new PvmAtomicOperationsTransitionInterruptFlowScope();
	  public static readonly PvmAtomicOperation TRANSITION_NOTIFY_LISTENER_START = new PvmAtomicOperationTransitionNotifyListenerStart();
	  public static readonly PvmAtomicOperation DELETE_CASCADE = new PvmAtomicOperationDeleteCascade();
	  public static readonly PvmAtomicOperation DELETE_CASCADE_FIRE_ACTIVITY_END = new PvmAtomicOperationDeleteCascadeFireActivityEnd();
	  public static readonly PvmAtomicOperation ACTIVITY_LEAVE = new PvmAtomicOperationActivityLeave();
	}

}