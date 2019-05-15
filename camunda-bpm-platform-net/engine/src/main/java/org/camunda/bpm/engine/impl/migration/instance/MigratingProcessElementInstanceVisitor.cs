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
namespace org.camunda.bpm.engine.impl.migration.instance
{

	using MigrationContext = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessElementInstanceTopDownWalker.MigrationContext;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using FlowScopeWalker = org.camunda.bpm.engine.impl.tree.FlowScopeWalker;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class MigratingProcessElementInstanceVisitor : TreeVisitor<MigrationContext>
	{
		public abstract void visit(T obj);

	  public virtual void visit(MigrationContext obj)
	  {
		if (canMigrate(obj.processElementInstance))
		{
		  migrateProcessElementInstance(obj.processElementInstance, obj.scopeInstanceBranch);
		}
	  }

	  protected internal abstract bool canMigrate(MigratingProcessElementInstance instance);

	  protected internal abstract void instantiateScopes(MigratingScopeInstance ancestorScopeInstance, MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate);

	  protected internal virtual void migrateProcessElementInstance(MigratingProcessElementInstance migratingInstance, MigratingScopeInstanceBranch migratingInstanceBranch)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MigratingScopeInstance parentMigratingInstance = migratingInstance.getParent();
		MigratingScopeInstance parentMigratingInstance = migratingInstance.Parent;

		ScopeImpl sourceScope = migratingInstance.SourceScope;
		ScopeImpl targetScope = migratingInstance.TargetScope;
		ScopeImpl targetFlowScope = targetScope.FlowScope;
		ScopeImpl parentActivityInstanceTargetScope = parentMigratingInstance != null ? parentMigratingInstance.TargetScope : null;

		if (sourceScope != sourceScope.ProcessDefinition && targetFlowScope != parentActivityInstanceTargetScope)
		{
		  // create intermediate scopes

		  // 1. manipulate execution tree

		  // determine the list of ancestor scopes (parent, grandparent, etc.) for which
		  //     no executions exist yet
		  IList<ScopeImpl> nonExistingScopes = collectNonExistingFlowScopes(targetFlowScope, migratingInstanceBranch);

		  // get the closest ancestor scope that is instantiated already
		  ScopeImpl existingScope = nonExistingScopes.Count == 0 ? targetFlowScope : nonExistingScopes[0].FlowScope;

		  // and its scope instance
		  MigratingScopeInstance ancestorScopeInstance = migratingInstanceBranch.getInstance(existingScope);

		  // Instantiate the scopes as children of the scope execution
		  instantiateScopes(ancestorScopeInstance, migratingInstanceBranch, nonExistingScopes);

		  MigratingScopeInstance targetFlowScopeInstance = migratingInstanceBranch.getInstance(targetFlowScope);

		  // 2. detach instance
		  // The order of steps 1 and 2 avoids intermediate execution tree compaction
		  // which in turn could overwrite some dependent instances (e.g. variables)
		  migratingInstance.detachState();

		  // 3. attach to newly created activity instance
		  migratingInstance.attachState(targetFlowScopeInstance);
		}

		// 4. update state (e.g. activity id)
		migratingInstance.migrateState();

		// 5. migrate instance state other than execution-tree structure
		migratingInstance.migrateDependentEntities();
	  }

	  /// <summary>
	  /// Returns a list of flow scopes from the given scope until a scope is reached that is already present in the given
	  /// <seealso cref="MigratingScopeInstanceBranch"/> (exclusive). The order of the returned list is top-down, i.e. the highest scope
	  /// is the first element of the list.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.pvm.process.ScopeImpl> collectNonExistingFlowScopes(org.camunda.bpm.engine.impl.pvm.process.ScopeImpl scope, final MigratingScopeInstanceBranch migratingExecutionBranch)
	  protected internal virtual IList<ScopeImpl> collectNonExistingFlowScopes(ScopeImpl scope, MigratingScopeInstanceBranch migratingExecutionBranch)
	  {
		FlowScopeWalker walker = new FlowScopeWalker(scope);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.impl.pvm.process.ScopeImpl> result = new java.util.LinkedList<org.camunda.bpm.engine.impl.pvm.process.ScopeImpl>();
		IList<ScopeImpl> result = new LinkedList<ScopeImpl>();
		walker.addPreVisitor(new TreeVisitorAnonymousInnerClass(this, result));

		walker.walkWhile(new WalkConditionAnonymousInnerClass(this, migratingExecutionBranch));

		return result;
	  }

	  private class TreeVisitorAnonymousInnerClass : TreeVisitor<ScopeImpl>
	  {
		  private readonly MigratingProcessElementInstanceVisitor outerInstance;

		  private IList<ScopeImpl> result;

		  public TreeVisitorAnonymousInnerClass(MigratingProcessElementInstanceVisitor outerInstance, IList<ScopeImpl> result)
		  {
			  this.outerInstance = outerInstance;
			  this.result = result;
		  }


		  public void visit(ScopeImpl obj)
		  {
			result.Insert(0, obj);
		  }
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ScopeImpl>
	  {
		  private readonly MigratingProcessElementInstanceVisitor outerInstance;

		  private org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstanceBranch migratingExecutionBranch;

		  public WalkConditionAnonymousInnerClass(MigratingProcessElementInstanceVisitor outerInstance, org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstanceBranch migratingExecutionBranch)
		  {
			  this.outerInstance = outerInstance;
			  this.migratingExecutionBranch = migratingExecutionBranch;
		  }


		  public bool isFulfilled(ScopeImpl element)
		  {
			return migratingExecutionBranch.hasInstance(element);
		  }
	  }

	}

}