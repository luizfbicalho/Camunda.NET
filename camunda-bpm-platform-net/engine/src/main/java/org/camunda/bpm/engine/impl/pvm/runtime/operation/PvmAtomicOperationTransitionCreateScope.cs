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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmAtomicOperationTransitionCreateScope : PvmAtomicOperationCreateScope
	{

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		PvmActivity activity = execution.getActivity();
		return activity.AsyncBefore;
	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "transition-create-scope";
		  }
	  }

	  protected internal override void scopeCreated(PvmExecutionImpl execution)
	  {
		execution.performOperation(PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_START);

	  }

	  public override bool AsyncCapable
	  {
		  get
		  {
			return true;
		  }
	  }
	}

}