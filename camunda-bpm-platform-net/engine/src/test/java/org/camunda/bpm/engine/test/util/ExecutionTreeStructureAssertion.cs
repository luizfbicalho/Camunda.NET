using System.Collections.Generic;
using System.Text;

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

	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExecutionTreeStructureAssertion : ExecutionTreeAssertion
	{

	  protected internal string expectedActivityId;
	  protected internal bool? expectedIsScope = true;
	  protected internal bool? expectedIsConcurrent = false;
	  protected internal bool? expectedIsEventScope = false;
	  protected internal string expectedId;

	  protected internal IList<ExecutionTreeStructureAssertion> childAssertions = new List<ExecutionTreeStructureAssertion>();

	  public virtual void addChildAssertion(ExecutionTreeStructureAssertion childAssertion)
	  {
		this.childAssertions.Add(childAssertion);
	  }

	  public virtual string ExpectedActivityId
	  {
		  set
		  {
			this.expectedActivityId = value;
		  }
	  }

	  /// <summary>
	  /// This assumes that all children have been fetched
	  /// </summary>
	  protected internal virtual bool matches(ExecutionTree tree)
	  {
		// match activity id
		string actualActivityId = tree.ActivityId;
		if (string.ReferenceEquals(expectedActivityId, null) && !string.ReferenceEquals(actualActivityId, null))
		{
		  return false;
		}
		else if (!string.ReferenceEquals(expectedActivityId, null) && !expectedActivityId.Equals(tree.ActivityId))
		{
		  return false;
		}

		if (!string.ReferenceEquals(expectedId, null) && !expectedId.Equals(tree.Id))
		{
		  return false;
		}


		// match is scope
		if (expectedIsScope != null && !expectedIsScope.Equals(tree.Scope))
		{
		  return false;
		}

		if (expectedIsConcurrent != null && !expectedIsConcurrent.Equals(tree.Concurrent))
		{
		  return false;
		}

		if (expectedIsEventScope != null && !expectedIsEventScope.Equals(tree.EventScope))
		{
		  return false;
		}

		// match children
		if (tree.Executions.Count != childAssertions.Count)
		{
		  return false;
		}

		IList<ExecutionTreeStructureAssertion> unmatchedChildAssertions = new List<ExecutionTreeStructureAssertion>(childAssertions);
		foreach (ExecutionTree child in tree.Executions)
		{
		  foreach (ExecutionTreeStructureAssertion childAssertion in unmatchedChildAssertions)
		  {
			if (childAssertion.matches(child))
			{
			  unmatchedChildAssertions.Remove(childAssertion);
			  break;
			}
		  }
		}

		if (unmatchedChildAssertions.Count > 0)
		{
		  return false;
		}

		return true;
	  }

	  public virtual void assertExecution(ExecutionTree tree)
	  {
		bool matches = matches(tree);
		if (!matches)
		{
		  StringBuilder errorBuilder = new StringBuilder();
		  errorBuilder.Append("Expected tree: \n");
		  describe(this, "", errorBuilder);
		  errorBuilder.Append("Actual tree: \n");
		  errorBuilder.Append(tree);
		  Assert.fail(errorBuilder.ToString());
		}
	  }

	  public static void describe(ExecutionTreeStructureAssertion assertion, string prefix, StringBuilder errorBuilder)
	  {
		errorBuilder.Append(prefix);
		errorBuilder.Append(assertion);
		errorBuilder.Append("\n");
		foreach (ExecutionTreeStructureAssertion child in assertion.childAssertions)
		{
		  describe(child, prefix + "   ", errorBuilder);
		}
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("[activityId=");
		sb.Append(expectedActivityId);

		if (!string.ReferenceEquals(expectedId, null))
		{
		  sb.Append(", id=");
		  sb.Append(expectedId);
		}

		if (expectedIsScope != null)
		{
		  sb.Append(", isScope=");
		  sb.Append(expectedIsScope);
		}

		if (expectedIsConcurrent != null)
		{
		  sb.Append(", isConcurrent=");
		  sb.Append(expectedIsConcurrent);
		}

		if (expectedIsEventScope != null)
		{
		  sb.Append(", isEventScope=");
		  sb.Append(expectedIsEventScope);
		}

		sb.Append("]");

		return sb.ToString();
	  }

	  public virtual bool? ExpectedIsScope
	  {
		  set
		  {
			this.expectedIsScope = value;
    
		  }
	  }

	  public virtual bool? ExpectedIsConcurrent
	  {
		  set
		  {
			this.expectedIsConcurrent = value;
		  }
	  }

	  public virtual bool? ExpectedIsEventScope
	  {
		  set
		  {
			this.expectedIsEventScope = value;
		  }
	  }

	  public virtual string ExpectedId
	  {
		  set
		  {
			this.expectedId = value;
		  }
	  }

	}

}