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
namespace org.camunda.bpm.qa.upgrade.util
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
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

		public virtual void hasStructure(ExpectedActivityInstance expected)
		{
		  assertTreeMatch(expected, actual);
		}

		protected internal virtual void assertTreeMatch(ExpectedActivityInstance expected, ActivityInstance actual)
		{
		  bool treesMatch = isTreeMatched(expected, actual);
		  if (!treesMatch)
		  {
			Assert.fail("Could not match expected tree \n" + expected + " \n\n with actual tree \n\n " + actual);
		  }

		}


		/// <summary>
		/// if anyone wants to improve this algorithm, feel welcome! </summary>
		protected internal virtual bool isTreeMatched(ExpectedActivityInstance expectedInstance, ActivityInstance actualInstance)
		{
		  if (!expectedInstance.getActivityIds().Contains(actualInstance.ActivityId))
		  {
			return false;
		  }
		  else
		  {
			if (expectedInstance.ChildActivityInstances.Count != actualInstance.ChildActivityInstances.Length)
			{
			  return false;
			}
			else
			{

			  IList<ExpectedActivityInstance> unmatchedInstances = new List<ExpectedActivityInstance>(expectedInstance.ChildActivityInstances);
			  foreach (ActivityInstance child1 in actualInstance.ChildActivityInstances)
			  {
				bool matchFound = false;
				foreach (ExpectedActivityInstance child2 in new List<ExpectedActivityInstance>(unmatchedInstances))
				{
				  if (isTreeMatched(child2, child1))
				  {
					unmatchedInstances.Remove(child2);
					matchFound = true;
					break;
				  }
				}
				if (!matchFound)
				{
				  return false;
				}
			  }

			  IList<ExpectedTransitionInstance> unmatchedTransitionInstances = new List<ExpectedTransitionInstance>(expectedInstance.ChildTransitionInstances);
			  foreach (TransitionInstance child in actualInstance.ChildTransitionInstances)
			  {
				IEnumerator<ExpectedTransitionInstance> expectedTransitionInstanceIt = unmatchedTransitionInstances.GetEnumerator();

				bool matchFound = false;
				while (expectedTransitionInstanceIt.MoveNext() && !matchFound)
				{
				  ExpectedTransitionInstance expectedChild = expectedTransitionInstanceIt.Current;
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

		protected internal ExpectedActivityInstance rootInstance = null;
		protected internal Stack<ExpectedActivityInstance> activityInstanceStack = new Stack<ExpectedActivityInstance>();

		public ActivityInstanceTreeBuilder() : this(null)
		{
		}

		public ActivityInstanceTreeBuilder(string rootActivityId)
		{
		  rootInstance = new ExpectedActivityInstance();
		  rootInstance.ActivityId = rootActivityId;
		  activityInstanceStack.Push(rootInstance);
		}

		public virtual ActivityInstanceTreeBuilder beginScope(params string[] activityIds)
		{
		  ExpectedActivityInstance newInstance = new ExpectedActivityInstance();
		  newInstance.setActivityIds(activityIds);

		  ExpectedActivityInstance parentInstance = activityInstanceStack.Peek();
		  IList<ExpectedActivityInstance> childInstances = new List<ExpectedActivityInstance>(parentInstance.ChildActivityInstances);
		  childInstances.Add(newInstance);
		  parentInstance.ChildActivityInstances = childInstances;

		  activityInstanceStack.Push(newInstance);

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder beginMiBody(string activityId)
		{
		  return beginScope(activityId + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX);
		}

		public virtual ActivityInstanceTreeBuilder activity(string activityId)
		{

		  beginScope(activityId);
		  endScope();

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder transition(string activityId)
		{

		  ExpectedTransitionInstance newInstance = new ExpectedTransitionInstance();
		  newInstance.ActivityId = activityId;
		  ExpectedActivityInstance parentInstance = activityInstanceStack.Peek();

		  IList<ExpectedTransitionInstance> childInstances = new List<ExpectedTransitionInstance>(parentInstance.ChildTransitionInstances);
		  childInstances.Add(newInstance);
		  parentInstance.ChildTransitionInstances = childInstances;

		  return this;
		}

		public virtual ActivityInstanceTreeBuilder endScope()
		{
		  activityInstanceStack.Pop();
		  return this;
		}

		public virtual ExpectedActivityInstance done()
		{
		  return rootInstance;
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