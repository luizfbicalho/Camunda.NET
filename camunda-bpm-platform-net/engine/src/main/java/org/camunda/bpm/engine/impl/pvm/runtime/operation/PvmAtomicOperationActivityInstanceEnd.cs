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
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class PvmAtomicOperationActivityInstanceEnd : AbstractPvmEventAtomicOperation
	{

	  private static readonly PvmLogger LOG = ProcessEngineLogger.PVM_LOGGER;

	  protected internal override PvmExecutionImpl eventNotificationsStarted(PvmExecutionImpl execution)
	  {
		execution.incrementSequenceCounter();

		// hack around execution tree structure not being in sync with activity instance concept:
		// if we end a scope activity, take remembered activity instance from parent and set on
		// execution before calling END listeners.
		PvmExecutionImpl parent = execution.Parent;
		PvmActivity activity = execution.getActivity();
		if (parent != null && execution.Scope && activity != null && activity.Scope && (activity.ActivityBehavior is CompositeActivityBehavior || (CompensationBehavior.isCompensationThrowing(execution)) && !LegacyBehavior.isCompensationThrowing(execution)))
		{

		  LOG.debugLeavesActivityInstance(execution, execution.ActivityInstanceId);

		  // use remembered activity instance id from parent
		  execution.ActivityInstanceId = parent.ActivityInstanceId;
		  // make parent go one scope up.
		  parent.leaveActivityInstance();


		}

		return execution;

	  }

	  protected internal override bool isSkipNotifyListeners(PvmExecutionImpl execution)
	  {
		// listeners are skipped if this execution is not part of an activity instance.
		return string.ReferenceEquals(execution.ActivityInstanceId, null);
	  }

	}

}