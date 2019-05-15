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

	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExecutionAssert
	{

	  protected internal ExecutionTree tree;
	  protected internal CommandExecutor commandExecutor;

	  public static ExecutionAssert assertThat(ExecutionTree tree)
	  {

		ExecutionAssert assertion = new ExecutionAssert();
		assertion.tree = tree;
		return assertion;
	  }

	  public virtual ExecutionAssert matches(ExecutionTreeAssertion assertion)
	  {
		assertion.assertExecution(tree);
		return this;
	  }

	  public virtual ExecutionAssert hasProcessDefinitionId(string expectedProcessDefinitionId)
	  {
		ExecutionTreeAssertion assertion = ExecutionTreeProcessDefinitionIdAssertion.processDefinitionId(expectedProcessDefinitionId);
		matches(assertion);
		return this;
	  }

	  public class ExecutionTreeBuilder
	  {

		protected internal ExecutionTreeStructureAssertion rootAssertion = null;
		protected internal Stack<ExecutionTreeStructureAssertion> activityInstanceStack = new Stack<ExecutionTreeStructureAssertion>();

		public ExecutionTreeBuilder(string rootActivityInstanceId)
		{
		  rootAssertion = new ExecutionTreeStructureAssertion();
		  rootAssertion.ExpectedActivityId = rootActivityInstanceId;
		  activityInstanceStack.Push(rootAssertion);
		}

		public virtual ExecutionTreeBuilder child(string activityId)
		{
		  ExecutionTreeStructureAssertion newInstance = new ExecutionTreeStructureAssertion();
		  newInstance.ExpectedActivityId = activityId;

		  ExecutionTreeStructureAssertion parentInstance = activityInstanceStack.Peek();
		  parentInstance.addChildAssertion(newInstance);

		  activityInstanceStack.Push(newInstance);

		  return this;
		}

		public virtual ExecutionTreeBuilder scope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = activityInstanceStack.Peek();
		  currentAssertion.ExpectedIsScope = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder concurrent()
		{
		  ExecutionTreeStructureAssertion currentAssertion = activityInstanceStack.Peek();
		  currentAssertion.ExpectedIsConcurrent = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder eventScope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = activityInstanceStack.Peek();
		  currentAssertion.ExpectedIsEventScope = true;
		  return this;
		}

		public virtual ExecutionTreeBuilder noScope()
		{
		  ExecutionTreeStructureAssertion currentAssertion = activityInstanceStack.Peek();
		  currentAssertion.ExpectedIsScope = false;
		  return this;
		}

		public virtual ExecutionTreeBuilder id(string id)
		{
		  ExecutionTreeStructureAssertion currentAssertion = activityInstanceStack.Peek();
		  currentAssertion.ExpectedId = id;
		  return this;
		}

		public virtual ExecutionTreeBuilder up()
		{
		  activityInstanceStack.Pop();
		  return this;
		}

		public virtual ExecutionTreeStructureAssertion done()
		{
		  return rootAssertion;
		}
	  }

	  public static ExecutionTreeBuilder describeExecutionTree(string activityId)
	  {
		return new ExecutionTreeBuilder(activityId);
	  }

	}

}