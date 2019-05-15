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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;

	using FlowNodeActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.FlowNodeActivityBehavior;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class PvmAtomicOperationActivityLeave : PvmAtomicOperation
	{

	  private static readonly PvmLogger LOG = PvmLogger.PVM_LOGGER;

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		return false;
	  }

	  public virtual void execute(PvmExecutionImpl execution)
	  {

		execution.activityInstanceDone();

		ActivityBehavior activityBehavior = getActivityBehavior(execution);

		if (activityBehavior is FlowNodeActivityBehavior)
		{
		  FlowNodeActivityBehavior behavior = (FlowNodeActivityBehavior) activityBehavior;

		  ActivityImpl activity = execution.getActivity();
		  string activityInstanceId = execution.ActivityInstanceId;
		  if (!string.ReferenceEquals(activityInstanceId, null))
		  {
			LOG.debugLeavesActivityInstance(execution, activityInstanceId);
		  }

		  try
		  {
			behavior.doLeave(execution);
		  }
		  catch (Exception e)
		  {
			throw e;
		  }
		  catch (Exception e)
		  {
			throw new PvmException("couldn't leave activity <" + activity.getProperty("type") + " id=\"" + activity.Id + "\" ...>: " + e.Message, e);
		  }
		}
		else
		{
		  throw new PvmException("Behavior of current activity is not an instance of " + typeof(FlowNodeActivityBehavior).Name + ". Execution " + execution);
		}
	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "activity-leave";
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