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

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExecutionTreeProcessDefinitionIdAssertion : ExecutionTreeAssertion
	{

	  protected internal string expectedProcessDefinitionId;

	  public virtual void assertExecution(ExecutionTree tree)
	  {
		IList<Execution> nonMatchingExecutions = matches(tree);

		if (nonMatchingExecutions.Count > 0)
		{
		  StringBuilder sb = new StringBuilder();
		  sb.Append("Expected all executions to have process definition id " + expectedProcessDefinitionId + "\n");
		  sb.Append("Actual Tree: \n");
		  sb.Append(tree);
		  sb.Append("\nExecutions with unexpected process definition id:\n");
		  sb.Append("[\n");
		  foreach (Execution execution in nonMatchingExecutions)
		  {
			sb.Append(execution);
			sb.Append("\n");
		  }
		  sb.Append("]\n");
		  Assert.fail(sb.ToString());
		}
	  }

	  /// <summary>
	  /// returns umatched executions in the tree
	  /// </summary>
	  protected internal virtual IList<Execution> matches(ExecutionTree tree)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) tree.Execution;
		IList<Execution> unmatchedExecutions = new List<Execution>();

		if (!expectedProcessDefinitionId.Equals(executionEntity.ProcessDefinitionId))
		{
		  unmatchedExecutions.Add(tree.Execution);
		}
		foreach (ExecutionTree child in tree.Executions)
		{
		  ((IList<Execution>)unmatchedExecutions).AddRange(matches(child));
		}

		return unmatchedExecutions;
	  }

	  public static ExecutionTreeProcessDefinitionIdAssertion processDefinitionId(string expectedProcessDefinitionId)
	  {
		ExecutionTreeProcessDefinitionIdAssertion assertion = new ExecutionTreeProcessDefinitionIdAssertion();
		assertion.expectedProcessDefinitionId = expectedProcessDefinitionId;

		return assertion;
	  }

	}

}