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
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// Cancel scope operation performed when an execution starts at an <seealso cref="ActivityImpl.isCancelActivity()"/>
	/// activity. This is used when an execution is set to the activity without entering it through a transition.
	/// See <seealso cref="PvmAtomicOperationCancelActivity"/> for more details on "cancel scope" behavior.
	/// 
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class PvmAtomicOperationActivityStartCancelScope : PvmAtomicOperationCancelActivity
	{

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "activity-start-cancel-scope";
		  }
	  }

	  protected internal override void activityCancelled(PvmExecutionImpl execution)
	  {
		execution.ActivityInstanceId = null;
		execution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE);
	  }

	  protected internal virtual PvmActivity getCancellingActivity(PvmExecutionImpl execution)
	  {
		return execution.NextActivity;
	  }

	  public override bool AsyncCapable
	  {
		  get
		  {
			return false;
		  }
	  }

	}

}