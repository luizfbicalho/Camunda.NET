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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;




	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	[Obsolete]
	public interface AtomicOperation : CoreAtomicOperation<PvmExecutionImpl>
	{

	  bool AsyncCapable {get;}
	}

	public static class AtomicOperation_Fields
	{
	  public static readonly AtomicOperation PROCESS_START = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START;
	  public static readonly AtomicOperation PROCESS_START_INITIAL = PvmAtomicOperation.PROCESS_START_INITIAL;
	  public static readonly AtomicOperation PROCESS_END = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_END;
	  public static readonly AtomicOperation ACTIVITY_START = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START;
	  public static readonly AtomicOperation ACTIVITY_START_CONCURRENT = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CONCURRENT;
	  public static readonly AtomicOperation ACTIVITY_START_CANCEL_SCOPE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CANCEL_SCOPE;
	  public static readonly AtomicOperation ACTIVITY_EXECUTE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_EXECUTE;
	  public static readonly AtomicOperation ACTIVITY_END = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_END;
	  public static readonly AtomicOperation FIRE_ACTIVITY_END = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.FIRE_ACTIVITY_END;
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_END = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END;
	  public static readonly AtomicOperation TRANSITION_DESTROY_SCOPE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_DESTROY_SCOPE;
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_TAKE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE;
	  public static readonly AtomicOperation TRANSITION_CREATE_SCOPE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE;
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_START = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_START;
	  public static readonly AtomicOperation DELETE_CASCADE = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE;
	  public static readonly AtomicOperation DELETE_CASCADE_FIRE_ACTIVITY_END = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END;
	}

}