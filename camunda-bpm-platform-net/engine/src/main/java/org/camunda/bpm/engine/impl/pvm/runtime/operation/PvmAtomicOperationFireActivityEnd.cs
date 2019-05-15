﻿/*
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
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// <para>This atomic operation simply fires the activity end event</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PvmAtomicOperationFireActivityEnd : AbstractPvmEventAtomicOperation
	{

	  public override string CanonicalName
	  {
		  get
		  {
			return "fire-activity-end";
		  }
	  }

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		return execution.getActivity();
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
		  }
	  }

	  protected internal virtual void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {
		// nothing to do
	  }

	}

}