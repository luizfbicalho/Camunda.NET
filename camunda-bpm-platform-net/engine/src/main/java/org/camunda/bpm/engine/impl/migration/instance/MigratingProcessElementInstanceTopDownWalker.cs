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
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;

	/// <summary>
	/// Walks the hierarchy of <seealso cref="MigratingProcessElementInstance"/>s in a top-down-fashion.
	/// Maintains a context of the current instance and the <seealso cref="MigratingScopeInstanceBranch"/>
	/// that it is in.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MigratingProcessElementInstanceTopDownWalker : ReferenceWalker<MigrationContext>
	{

	  public MigratingProcessElementInstanceTopDownWalker(MigratingActivityInstance activityInstance) : base(new MigrationContext(activityInstance, new MigratingScopeInstanceBranch()))
	  {
	  }

	  protected internal override ICollection<MigrationContext> nextElements()
	  {

		ICollection<MigrationContext> nextElements = new LinkedList<MigrationContext>();

		MigrationContext currentElement = CurrentElement;

		// continue migration for non-leaf instances (i.e. scopes)
		if (currentElement.processElementInstance is MigratingScopeInstance)
		{
		  // Child instances share the same scope instance branch;
		  // This ensures "once-per-parent" instantiation semantics,
		  // i.e. if a new parent scope is added to more than one child, all those children
		  // will share the same new parent instance.
		  // By changing the way how the branches are created here, it should be possible
		  // to implement other strategies, e.g. "once-per-child" semantics
		  MigratingScopeInstanceBranch childrenScopeBranch = currentElement.scopeInstanceBranch.copy();
		  MigratingScopeInstanceBranch childrenCompensationScopeBranch = currentElement.scopeInstanceBranch.copy();

		  MigratingScopeInstance scopeInstance = (MigratingScopeInstance) currentElement.processElementInstance;

		  childrenScopeBranch.visited(scopeInstance);
		  childrenCompensationScopeBranch.visited(scopeInstance);

		  foreach (MigratingProcessElementInstance child in scopeInstance.Children)
		  {
			MigratingScopeInstanceBranch instanceBranch = null;

			// compensation and non-compensation scopes cannot share the same scope instance branch
			// e.g. when adding a sub process, we want to create a new activity instance as well
			// as a new event scope instance for that sub process
			if (child is MigratingEventScopeInstance || child is MigratingCompensationEventSubscriptionInstance)
			{
			  instanceBranch = childrenCompensationScopeBranch;
			}
			else
			{
			  instanceBranch = childrenScopeBranch;
			}
			nextElements.Add(new MigrationContext(child, instanceBranch));
		  }
		}

		return nextElements;
	  }

	  public class MigrationContext
	  {
		protected internal MigratingProcessElementInstance processElementInstance;
		protected internal MigratingScopeInstanceBranch scopeInstanceBranch;

		public MigrationContext(MigratingProcessElementInstance processElementInstance, MigratingScopeInstanceBranch scopeInstanceBranch)
		{
		  this.processElementInstance = processElementInstance;
		  this.scopeInstanceBranch = scopeInstanceBranch;
		}

		public virtual MigratingProcessElementInstance ProcessElementInstance
		{
			get
			{
			  return processElementInstance;
			}
		}

		public virtual MigratingScopeInstanceBranch ScopeInstanceBranch
		{
			get
			{
			  return scopeInstanceBranch;
			}
		}
	  }

	}

}