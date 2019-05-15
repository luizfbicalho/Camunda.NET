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
namespace org.camunda.bpm.engine.test.bpmn.executionlistener
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;

	/// <summary>
	/// Simple <seealso cref="ExecutionListener"/> that sets the current activity id and name attributes on the execution.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
	public class CurrentActivityExecutionListener : ExecutionListener
	{

	  private static IList<CurrentActivity> currentActivities = new List<CurrentActivity>();

	  public class CurrentActivity
	  {
		internal readonly string activityId;
		internal readonly string activityName;

		public CurrentActivity(string activityId, string activityName)
		{
		  this.activityId = activityId;
		  this.activityName = activityName;
		}

		public virtual string ActivityId
		{
			get
			{
			  return activityId;
			}
		}

		public virtual string ActivityName
		{
			get
			{
			  return activityName;
			}
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		currentActivities.Add(new CurrentActivity(execution.CurrentActivityId, execution.CurrentActivityName));
	  }

	  public static IList<CurrentActivity> CurrentActivities
	  {
		  get
		  {
			return currentActivities;
		  }
	  }

	  public static void clear()
	  {
		currentActivities.Clear();
	  }
	}

}