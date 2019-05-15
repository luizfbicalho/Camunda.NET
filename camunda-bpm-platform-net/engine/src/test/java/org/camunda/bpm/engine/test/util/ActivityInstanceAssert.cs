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
namespace org.camunda.bpm.engine.test.util
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ActivityInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.ActivityInstanceImpl;
	using TransitionInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.TransitionInstanceImpl;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ActivityInstanceAssert
	{

	  public class ActivityInstanceAssertThatClause
	  {

		protected internal ActivityInstance actual;

		public ActivityInstanceAssertThatClause(ActivityInstance actual)
		{
		  this.actual = actual;
		}

		public virtual void hasStructure(ActivityInstance expected)
		{
		  assertTreeMatch(expected, actual);
		}

		protected internal virtual void assertTreeMatch(ActivityInstance expected, ActivityInstance actual)
		{
		  bool treesMatch = isTreeMatched(expected, actual);
		  if (!treesMatch)
		  {
			Assert.fail("Could not match expected tree \n" + expected + " \n\n with actual tree \n\n " + actual);
		  }

		}


		/// <summary>
		/// if anyone wants to improve this algorithm, feel welcome! </summary>
		protected internal virtual bool isTreeMatched(ActivityInstance expectedInstance, ActivityInstance actualInstance)
		{
		  if (!expectedInstance.ActivityId.Equals(actualInstance.ActivityId) || (!string.ReferenceEquals(expectedInstance.Id, null) && !expectedInstance.Id.Equals(actualInstance.Id)))
		  {
			return false;
		  }
		  else
		  {
			if (expectedInstance.ChildActivityInstances.Length != actualInstance.ChildActivityInstances.Length)
			{
			  return false;
			}
			else
			{

			  IList<ActivityInstance> unmatchedInstances = new List<ActivityInstance>(Arrays.asList(expectedInstance.ChildActivityInstances));
			  foreach (ActivityInstance actualChild in actualInstance.ChildActivityInstances)
			  {
				bool matchFound = false;
				foreach (ActivityInstance expectedChild in new List<ActivityInstance>(unmatchedInstances))
				{
				  if (isTreeMatched(expectedChild, actualChild))
				  {
					unmatchedInstances.Remove(actualChild);
					matchFound = true;
					break;
				  }
				}
				if (!matchFound)
				{
				  return false;
				}
			  }

			  if (expectedInstance.ChildTransitionInstances.Length != actualInstance.ChildTransitionInstances.Length)
			  {
				return false;
			  }

			  IList<TransitionInstance> unmatchedTransitionInstances = new List<TransitionInstance>(Arrays.asList(expectedInstance.ChildTransitionInstances));
			  foreach (TransitionInstance child in actualInstance.ChildTransitionInstances)
			  {
				IEnumerator<TransitionInstance> expectedTransitionInstanceIt = unmatchedTransitionInstances.GetEnumerator();

				bool matchFound = false;
				while (expectedTransitionInstanceIt.MoveNext() && !matchFound)
				{
				  TransitionInstance expectedChild = expectedTransitionInstanceIt.Current;
				  if (expectedChild.ActivityId.Equals(child.ActivityId))
				  {
					matchFound = true;
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
					expectedTransitionInstanceIt.remove();
				  }
				}

				if (!matchFound)
				{
				  return false;
				}
			  }

			}
			return true;

		  }
		}

	  }

	  public class ActivityInstanceTreeBuilder
	  {

		protected internal ActivityInstanceImpl rootInstance = null;
		protected internal Stack<ActivityInstanceImpl> activityInstanceStack = new Stack<ActivityInstanceImpl>();

		public ActivityInstanceTreeBuilder() : this(null)
		{
		}

		public ActivityInstanceTreeBuilder(string rootActivityId)
		{
		  rootInstance = new ActivityInstanceImpl();
		  rootInstance.ActivityId = rootActivityId;
		  activityInstanceStack.Push(rootInstance);
		}

		public virtual ActivityInstanceTreeBuilder beginScope(string activityId)
		{
		  return beginScope(activityId, null);
		}

		public virtual ActivityInstanceTreeBuilder beginScope(string activityId, string activityInstanceId)
		{
		  ActivityInstanceImpl newInstance = new ActivityInstanceImpl();
		  newInstance.ActivityId = activityId;
		  newInstance.Id = activityInstanceId;

		  ActivityInstanceImpl parentInstance = activityInstanceStack.Peek();
		  IList<ActivityInstance> childInstances = new List<ActivityInstance>(Arrays.asList(parentInstance.ChildActivityInstances));
		  childInstances.Add(newInstance);
		  parentInstance.ChildActivityInstances = childInstances.ToArray();

		  activityInstanceStack.Push(newInstance);

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder beginMiBody(string activityId)
		{
		  return beginScope(activityId + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, null);
		}

		public virtual ActivityInstanceTreeBuilder beginMiBody(string activityId, string activityInstanceId)
		{
		  return beginScope(activityId + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, activityInstanceId);
		}

		public virtual ActivityInstanceTreeBuilder activity(string activityId)
		{

		  return activity(activityId, null);
		}

		public virtual ActivityInstanceTreeBuilder activity(string activityId, string activityInstanceId)
		{

		  beginScope(activityId);
		  id(activityInstanceId);
		  endScope();

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder transition(string activityId)
		{

		  TransitionInstanceImpl newInstance = new TransitionInstanceImpl();
		  newInstance.ActivityId = activityId;
		  ActivityInstanceImpl parentInstance = activityInstanceStack.Peek();

		  IList<TransitionInstance> childInstances = new List<TransitionInstance>(Arrays.asList(parentInstance.ChildTransitionInstances));
		  childInstances.Add(newInstance);
		  parentInstance.ChildTransitionInstances = childInstances.ToArray();

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder endScope()
		{
		  activityInstanceStack.Pop();
		  return this;
		}

		public virtual ActivityInstance done()
		{
		  return rootInstance;
		}

		protected internal virtual ActivityInstanceTreeBuilder id(string expectedActivityInstanceId)
		{
		  ActivityInstanceImpl activityInstanceImpl = activityInstanceStack.Peek();
		  activityInstanceImpl.Id = expectedActivityInstanceId;
		  return this;
		}
	  }

	  public static ActivityInstanceTreeBuilder describeActivityInstanceTree()
	  {
		return new ActivityInstanceTreeBuilder();
	  }

	  public static ActivityInstanceTreeBuilder describeActivityInstanceTree(string rootActivityId)
	  {
		return new ActivityInstanceTreeBuilder(rootActivityId);
	  }

	  public static ActivityInstanceAssertThatClause assertThat(ActivityInstance actual)
	  {
		return new ActivityInstanceAssertThatClause(actual);
	  }

	}

}