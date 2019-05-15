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
	public class MigratingCompensationEventSubscriptionInstance : MigratingProcessElementInstance, RemovingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal EventSubscriptionEntity eventSubscription;

	  public MigratingCompensationEventSubscriptionInstance(MigrationInstruction migrationInstruction, ScopeImpl sourceScope, ScopeImpl targetScope, EventSubscriptionEntity eventSubscription)
	  {
		this.migrationInstruction = migrationInstruction;
		this.eventSubscription = eventSubscription;
		this.sourceScope = sourceScope;
		this.targetScope = targetScope;
		this.currentScope = sourceScope;
	  }

	  public override bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(eventSubscription.ExecutionId, null);
		  }
	  }

	  public override void detachState()
	  {
		eventSubscription.Execution = null;
	  }

	  public override void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);

	  }

	  public override void migrateState()
	  {
		eventSubscription.Activity = (ActivityImpl) targetScope;
		currentScope = targetScope;

	  }

	  public override void migrateDependentEntities()
	  {
	  }

	  public override void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
	  }

	  public override ExecutionEntity resolveRepresentativeExecution()
	  {
		return null;
	  }

	  public override void attachState(MigratingScopeInstance targetActivityInstance)
	  {
		Parent = targetActivityInstance;

		ExecutionEntity representativeExecution = targetActivityInstance.resolveRepresentativeExecution();
		eventSubscription.Execution = representativeExecution;
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

	  public virtual void remove()
	  {
		eventSubscription.delete();
	  }

	}

}