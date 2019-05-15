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
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class MigratingProcessElementInstance : MigratingInstance
	{
		public abstract void migrateDependentEntities();
		public abstract void migrateState();
		public abstract void attachState(MigratingTransitionInstance targetTransitionInstance);
		public abstract void attachState(MigratingScopeInstance targetActivityInstance);
		public abstract void detachState();
		public abstract bool Detached {get;}

	  protected internal MigrationInstruction migrationInstruction;

	  protected internal ScopeImpl sourceScope;
	  protected internal ScopeImpl targetScope;
	  // changes from source to target scope during migration
	  protected internal ScopeImpl currentScope;

	  protected internal MigratingScopeInstance parentInstance;

	  public virtual ScopeImpl SourceScope
	  {
		  get
		  {
			return sourceScope;
		  }
	  }

	  public virtual ScopeImpl TargetScope
	  {
		  get
		  {
			return targetScope;
		  }
	  }

	  public virtual ScopeImpl CurrentScope
	  {
		  get
		  {
			return currentScope;
		  }
	  }

	  public virtual MigrationInstruction MigrationInstruction
	  {
		  get
		  {
			return migrationInstruction;
		  }
	  }

	  public virtual MigratingScopeInstance Parent
	  {
		  get
		  {
			return parentInstance;
		  }
	  }

	  public virtual bool migratesTo(ScopeImpl other)
	  {
		return other == targetScope;
	  }

	  public abstract MigratingScopeInstance Parent {set;}

	  public abstract void addMigratingDependentInstance(MigratingInstance migratingInstance);

	  public abstract ExecutionEntity resolveRepresentativeExecution();

	  public virtual MigratingActivityInstance ClosestAncestorActivityInstance
	  {
		  get
		  {
			MigratingScopeInstance ancestorInstance = parentInstance;
    
			while (!(ancestorInstance is MigratingActivityInstance))
			{
			  ancestorInstance = ancestorInstance.Parent;
			}
    
			return (MigratingActivityInstance) ancestorInstance;
		  }
	  }

	}

}