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

	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingEventScopeInstance : MigratingScopeInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal MigratingCompensationEventSubscriptionInstance migratingEventSubscription;

	  protected internal ExecutionEntity eventScopeExecution;

	  protected internal ISet<MigratingEventScopeInstance> childInstances = new HashSet<MigratingEventScopeInstance>();
	  protected internal ISet<MigratingCompensationEventSubscriptionInstance> childCompensationSubscriptionInstances = new HashSet<MigratingCompensationEventSubscriptionInstance>();
	  protected internal IList<MigratingInstance> migratingDependentInstances = new List<MigratingInstance>();

	  public MigratingEventScopeInstance(MigrationInstruction migrationInstruction, ExecutionEntity eventScopeExecution, ScopeImpl sourceScope, ScopeImpl targetScope, MigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription, ScopeImpl eventSubscriptionSourceScope, ScopeImpl eventSubscriptionTargetScope)
	  {
		this.migratingEventSubscription = new MigratingCompensationEventSubscriptionInstance(eventSubscriptionInstruction, eventSubscriptionSourceScope, eventSubscriptionTargetScope, eventSubscription);
		this.migrationInstruction = migrationInstruction;
		this.eventScopeExecution = eventScopeExecution;

		// compensation handlers (not boundary events)
		this.sourceScope = sourceScope;
		this.targetScope = targetScope;
	  }

	  /// <summary>
	  /// Creates an emerged scope
	  /// </summary>
	  public MigratingEventScopeInstance(EventSubscriptionEntity eventSubscription, ExecutionEntity eventScopeExecution, ScopeImpl targetScope)
	  {
		this.migratingEventSubscription = new MigratingCompensationEventSubscriptionInstance(null, null, targetScope, eventSubscription);
		this.eventScopeExecution = eventScopeExecution;

		// compensation handlers (not boundary events)
		// or parent flow scopes
		this.targetScope = targetScope;
		this.currentScope = targetScope;
	  }

	  public override bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(eventScopeExecution.ParentId, null);
		  }
	  }

	  public override void detachState()
	  {
		migratingEventSubscription.detachState();
		eventScopeExecution.Parent = null;
	  }

	  public override void attachState(MigratingScopeInstance targetActivityInstance)
	  {
		Parent = targetActivityInstance;

		migratingEventSubscription.attachState(targetActivityInstance);

		ExecutionEntity representativeExecution = targetActivityInstance.resolveRepresentativeExecution();
		eventScopeExecution.Parent = representativeExecution;
	  }

	  public override void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public override void migrateState()
	  {
		migratingEventSubscription.migrateState();

		eventScopeExecution.setActivity((ActivityImpl) targetScope);
		eventScopeExecution.setProcessDefinition(targetScope.ProcessDefinition);

		currentScope = targetScope;
	  }

	  public override void migrateDependentEntities()
	  {
		foreach (MigratingInstance dependentEntity in migratingDependentInstances)
		{
		  dependentEntity.migrateState();
		}
	  }

	  public override MigratingScopeInstance Parent
	  {
		  set
		  {
			if (this.parentInstance != null)
			{
			  this.parentInstance.removeChild(this);
			}
    
			this.parentInstance = value;
    
			if (value != null)
			{
			  value.addChild(this);
			}
		  }
	  }

	  public override void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
		migratingDependentInstances.Add(migratingInstance);
	  }

	  public override ExecutionEntity resolveRepresentativeExecution()
	  {
		return eventScopeExecution;
	  }

	  public override void removeChild(MigratingScopeInstance migratingScopeInstance)
	  {
		childInstances.remove(migratingScopeInstance);
	  }

	  public override void addChild(MigratingScopeInstance migratingScopeInstance)
	  {
		if (migratingScopeInstance is MigratingEventScopeInstance)
		{
		  childInstances.Add((MigratingEventScopeInstance) migratingScopeInstance);
		}
		else
		{
		  throw MIGRATION_LOGGER.cannotHandleChild(this, migratingScopeInstance);
		}
	  }

	  public override void addChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
	  {
		this.childCompensationSubscriptionInstances.Add(migratingEventSubscription);
	  }

	  public override void removeChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
	  {
		this.childCompensationSubscriptionInstances.remove(migratingEventSubscription);
	  }

	  public override bool migrates()
	  {
		return targetScope != null;
	  }

	  public override void detachChildren()
	  {
		ISet<MigratingProcessElementInstance> childrenCopy = new HashSet<MigratingProcessElementInstance>(Children);
		foreach (MigratingProcessElementInstance child in childrenCopy)
		{
		  child.detachState();
		}
	  }

	  public override void remove(bool skipCustomListeners, bool skipIoMappings)
	  {
		// never invokes listeners and io mappings because this does not remove an active
		// activity instance
		eventScopeExecution.remove();
		migratingEventSubscription.remove();
		Parent = null;
	  }

	  public override ICollection<MigratingProcessElementInstance> Children
	  {
		  get
		  {
			ISet<MigratingProcessElementInstance> children = new HashSet<MigratingProcessElementInstance>(childInstances);
			children.addAll(childCompensationSubscriptionInstances);
			return children;
		  }
	  }

	  public override ICollection<MigratingScopeInstance> ChildScopeInstances
	  {
		  get
		  {
			return new HashSet<MigratingScopeInstance>(childInstances);
		  }
	  }

	  public override void removeUnmappedDependentInstances()
	  {
	  }

	  public virtual MigratingCompensationEventSubscriptionInstance EventSubscription
	  {
		  get
		  {
			return migratingEventSubscription;
		  }
	  }

	}

}