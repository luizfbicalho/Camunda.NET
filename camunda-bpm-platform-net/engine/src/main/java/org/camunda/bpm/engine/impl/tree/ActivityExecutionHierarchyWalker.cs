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
namespace org.camunda.bpm.engine.impl.tree
{

	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Combination of flow scope and execution walker. Walks the flow scope
	/// hierarchy upwards from the given execution to the top level process instance.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class ActivityExecutionHierarchyWalker : SingleReferenceWalker<ActivityExecutionTuple>
	{

	  private IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping;

	  public ActivityExecutionHierarchyWalker(ActivityExecution execution) : base(createTupel(execution))
	  {

		activityExecutionMapping = execution.createActivityExecutionMapping();
	  }

	  protected internal override ActivityExecutionTuple nextElement()
	  {
		ActivityExecutionTuple currentElement = CurrentElement;

		PvmScope currentScope = currentElement.Scope;
		PvmExecutionImpl currentExecution = (PvmExecutionImpl) currentElement.Execution;

		PvmScope flowScope = currentScope.FlowScope;

		if (!currentExecution.Scope)
		{
		  currentExecution = activityExecutionMapping[currentScope];
		  return new ActivityExecutionTuple(currentScope, currentExecution);
		}
		else if (flowScope != null)
		{
		  // walk to parent scope
		  PvmExecutionImpl execution = activityExecutionMapping[flowScope];
		  return new ActivityExecutionTuple(flowScope, execution);
		}
		else
		{
		  // this is the process instance, look for parent
		  currentExecution = activityExecutionMapping[currentScope];
		  PvmExecutionImpl superExecution = currentExecution.SuperExecution;

		  if (superExecution != null)
		  {
			// walk to parent process instance
			activityExecutionMapping = superExecution.createActivityExecutionMapping();
			return createTupel(superExecution);

		  }
		  else
		  {
			// this is the top level process instance
			return null;
		  }
		}
	  }

	  protected internal static ActivityExecutionTuple createTupel(ActivityExecution execution)
	  {
		PvmScope flowScope = getCurrentFlowScope(execution);
		return new ActivityExecutionTuple(flowScope, execution);
	  }

	  protected internal static PvmScope getCurrentFlowScope(ActivityExecution execution)
	  {
		ScopeImpl scope = null;
		if (execution.Transition != null)
		{
		  scope = execution.Transition.Destination.FlowScope;
		}
		else
		{
		  scope = (ScopeImpl) execution.Activity;
		}

		if (scope.Scope)
		{
		  return scope;
		}
		else
		{
		  return scope.FlowScope;
		}
	  }

	  public virtual ReferenceWalker<ActivityExecutionTuple> addScopePreVisitor(TreeVisitor<PvmScope> visitor)
	  {
		return addPreVisitor(new ScopeVisitorWrapper(this, visitor));
	  }

	  public virtual ReferenceWalker<ActivityExecutionTuple> addScopePostVisitor(TreeVisitor<PvmScope> visitor)
	  {
		return addPostVisitor(new ScopeVisitorWrapper(this, visitor));
	  }

	  public virtual ReferenceWalker<ActivityExecutionTuple> addExecutionPreVisitor(TreeVisitor<ActivityExecution> visitor)
	  {
		return addPreVisitor(new ExecutionVisitorWrapper(this, visitor));
	  }

	  public virtual ReferenceWalker<ActivityExecutionTuple> addExecutionPostVisitor(TreeVisitor<ActivityExecution> visitor)
	  {
		return addPostVisitor(new ExecutionVisitorWrapper(this, visitor));
	  }

	  private class ExecutionVisitorWrapper : TreeVisitor<ActivityExecutionTuple>
	  {
		  private readonly ActivityExecutionHierarchyWalker outerInstance;


		internal readonly TreeVisitor<ActivityExecution> collector;

		public ExecutionVisitorWrapper(ActivityExecutionHierarchyWalker outerInstance, TreeVisitor<ActivityExecution> collector)
		{
			this.outerInstance = outerInstance;
		  this.collector = collector;
		}

		public virtual void visit(ActivityExecutionTuple tupel)
		{
		  collector.visit(tupel.Execution);
		}
	  }

	  private class ScopeVisitorWrapper : TreeVisitor<ActivityExecutionTuple>
	  {
		  private readonly ActivityExecutionHierarchyWalker outerInstance;


		internal readonly TreeVisitor<PvmScope> collector;

		public ScopeVisitorWrapper(ActivityExecutionHierarchyWalker outerInstance, TreeVisitor<PvmScope> collector)
		{
			this.outerInstance = outerInstance;
		  this.collector = collector;
		}

		public virtual void visit(ActivityExecutionTuple tupel)
		{
		  collector.visit(tupel.Scope);
		}
	  }

	}
}