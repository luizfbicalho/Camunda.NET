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
namespace org.camunda.bpm.engine.test.standalone.pvm
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ActivityInstanceVerification : Assert, ExecutionListener
	{

	  internal class ActivityInstance
	  {
		  private readonly ActivityInstanceVerification outerInstance;


		internal string id;
		internal string parentId;
		internal string executionId;
		internal bool isCompleteScope;

		public ActivityInstance(ActivityInstanceVerification outerInstance, string executionId, string actInstanceId, string parentId, bool isCompleteScope)
		{
			this.outerInstance = outerInstance;
		  this.id = actInstanceId;
		  this.executionId = executionId;
		  this.parentId = parentId;
		  this.isCompleteScope = isCompleteScope;
		}

		public override string ToString()
		{
		  return id + " by " + executionId + " parent: " + parentId;
		}

	  }

	  protected internal IDictionary<string, IList<ActivityInstance>> startedActivityInstances = new Dictionary<string, IList<ActivityInstance>>();
	  protected internal IDictionary<string, IList<ActivityInstance>> endedActivityInstances = new Dictionary<string, IList<ActivityInstance>>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution e) throws Exception
	  public virtual void notify(DelegateExecution e)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution = (org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution) e;
		ActivityExecution execution = (ActivityExecution) e;

		if (string.ReferenceEquals(execution.ActivityInstanceId, null))
		{
		  return;
		}

		if (execution.EventName.Equals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START))
		{
		  addActivityInstanceId(execution, startedActivityInstances);

		}
		else if (execution.EventName.Equals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END))
		{
		  addActivityInstanceId(execution, endedActivityInstances);
		}

	  }

	  private void addActivityInstanceId(ActivityExecution execution, IDictionary<string, IList<ActivityInstance>> instanceMap)
	  {

		string actId = execution.Activity.Id;
		string actInstanceId = execution.ActivityInstanceId;
		string parentActInstanceId = execution.ParentActivityInstanceId;
		string executionId = System.identityHashCode(execution).ToString();

		// add to instance map
		IList<ActivityInstance> instancesForThisAct = instanceMap[actId];
		if (instancesForThisAct == null)
		{
		  instancesForThisAct = new List<ActivityInstance>();
		  instanceMap[actId] = instancesForThisAct;
		}
		ActivityInstance activityInstance = new ActivityInstance(this, executionId, actInstanceId, parentActInstanceId, execution.CompleteScope);
		instancesForThisAct.Add(activityInstance);
	  }

	  // assertions //////////////////////////////

	  public virtual void assertStartInstanceCount(int count, string actId)
	  {

		IList<ActivityInstance> startInstancesForThisAct = startedActivityInstances[actId];
		if (count == 0 && startInstancesForThisAct == null)
		{
		  return;
		}

		assertNotNull(startInstancesForThisAct);
		assertEquals(count, startInstancesForThisAct.Count);

		IList<ActivityInstance> endInstancesForThisAct = endedActivityInstances[actId];
		assertNotNull(endInstancesForThisAct);

		foreach (ActivityInstance startedActInstance in startInstancesForThisAct)
		{

		  assertNotNull("activityInstanceId cannot be null for " + startedActInstance, startedActInstance.id);
		  assertNotNull("executionId cannot be null for " + startedActInstance, startedActInstance.executionId);
		  assertNotNull("parentId cannot be null for " + startedActInstance, startedActInstance.parentId);

		  bool foundMatchingEnd = false;
		  foreach (ActivityInstance endedActInstance in endInstancesForThisAct)
		  {
			if (startedActInstance.id.Equals(endedActInstance.id))
			{
			  assertEquals(startedActInstance.parentId, endedActInstance.parentId);
			  foundMatchingEnd = true;
			}
		  }
		  if (!foundMatchingEnd)
		  {
			fail("cannot find matching end activity instance for start activity instance " + startedActInstance.id);
		  }
		}
	  }

	  public virtual void assertProcessInstanceParent(string actId, PvmProcessInstance processInstance)
	  {
		assertParentActInstance(actId, System.identityHashCode(processInstance).ToString());
	  }

	  public virtual void assertParentActInstance(string actId, string actInstId)
	  {
		IList<ActivityInstance> actInstanceList = startedActivityInstances[actId];

		foreach (ActivityInstance activityInstance in actInstanceList)
		{
		  assertEquals(actInstId, activityInstance.parentId);
		}

		actInstanceList = endedActivityInstances[actId];
		foreach (ActivityInstance activityInstance in actInstanceList)
		{
		  assertEquals(actInstId, activityInstance.parentId);
		}

	  }

	  public virtual void assertParent(string actId, string parentId)
	  {
		IList<ActivityInstance> actInstanceList = startedActivityInstances[actId];
		IList<ActivityInstance> parentInstances = startedActivityInstances[parentId];

		foreach (ActivityInstance activityInstance in actInstanceList)
		{
		  bool found = false;
		  foreach (ActivityInstance parentIntance in parentInstances)
		  {
			if (activityInstance.parentId.Equals(parentIntance.id))
			{
			  found = true;
			}
		  }
		  if (!found)
		  {
			fail("every instance of '" + actId + "' must have a parent which is an instance of '" + parentId);
		  }
		}
	  }

	  public virtual void assertIsCompletingActivityInstance(string activityId)
	  {
		assertIsCompletingActivityInstance(activityId, -1);
	  }

	  public virtual void assertIsCompletingActivityInstance(string activityId, int count)
	  {
		assertCorrectCompletingState(activityId, count, true);
	  }

	  public virtual void assertNonCompletingActivityInstance(string activityId)
	  {
		assertNonCompletingActivityInstance(activityId, -1);
	  }

	  public virtual void assertNonCompletingActivityInstance(string activityId, int count)
	  {
		assertCorrectCompletingState(activityId, count, false);
	  }

	  private void assertCorrectCompletingState(string activityId, int expectedCount, bool completing)
	  {
		IList<ActivityInstance> endActivityInstances = endedActivityInstances[activityId];
		assertNotNull(endActivityInstances);

		foreach (ActivityInstance instance in endActivityInstances)
		{
		  assertEquals(completing, instance.isCompleteScope);
		}

		if (expectedCount != -1)
		{
		  assertEquals(expectedCount, endActivityInstances.Count);
		}
	  }

	}

}