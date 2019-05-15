using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.standalone.pvm.activities
{

	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class EmbeddedSubProcess : CompositeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		IList<PvmActivity> startActivities = new List<PvmActivity>();
		foreach (PvmActivity activity in execution.Activity.Activities)
		{
		  if (activity.IncomingTransitions.Count == 0)
		  {
			startActivities.Add(activity);
		  }
		}

		foreach (PvmActivity startActivity in startActivities)
		{
		  execution.executeActivity(startActivity);
		}
	  }

	  public virtual void concurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {
		endedExecution.remove();
		scopeExecution.tryPruneLastConcurrentChild();
	  }

	  public virtual void complete(ActivityExecution execution)
	  {
		IList<PvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;
		if (outgoingTransitions.Count == 0)
		{
		  execution.end(true);
		}
		else
		{
		  execution.leaveActivityViaTransitions(outgoingTransitions, Collections.EMPTY_LIST);
		}
	  }

	}

}