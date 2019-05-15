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
namespace org.camunda.bpm.engine.impl.pvm.runtime.operation
{
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmAtomicOperationActivityExecute : PvmAtomicOperation
	{

	  private static readonly PvmLogger LOG = PvmLogger.PVM_LOGGER;

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		return false;
	  }

	  public virtual void execute(PvmExecutionImpl execution)
	  {
		execution.activityInstanceStarted();

		execution.continueIfExecutionDoesNotAffectNextOperation(new CallbackAnonymousInnerClass(this, execution)
	   , new CallbackAnonymousInnerClass2(this, execution)
	   , execution);
	  }

	  private class CallbackAnonymousInnerClass : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmAtomicOperationActivityExecute outerInstance;

		  private PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass(PvmAtomicOperationActivityExecute outerInstance, PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void callback(PvmExecutionImpl execution)
		  {
			if (execution.getActivity().Scope)
			{
			  execution.dispatchEvent(null);
			}
			return null;
		  }
	  }

	  private class CallbackAnonymousInnerClass2 : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmAtomicOperationActivityExecute outerInstance;

		  private PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass2(PvmAtomicOperationActivityExecute outerInstance, PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }


		  public Void callback(PvmExecutionImpl execution)
		  {

			ActivityBehavior activityBehavior = getActivityBehavior(execution);

			ActivityImpl activity = execution.getActivity();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LOG.debugExecutesActivity(execution, activity, activityBehavior.GetType().FullName);

			try
			{
			  activityBehavior.execute(execution);
			}
			catch (Exception e)
			{
			  throw e;
			}
			catch (Exception e)
			{
			  throw new PvmException("couldn't execute activity <" + activity.getProperty("type") + " id=\"" + activity.Id + "\" ...>: " + e.Message, e);
			}
			return null;
		  }
	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "activity-execute";
		  }
	  }

	  public virtual bool AsyncCapable
	  {
		  get
		  {
			return false;
		  }
	  }
	}

}