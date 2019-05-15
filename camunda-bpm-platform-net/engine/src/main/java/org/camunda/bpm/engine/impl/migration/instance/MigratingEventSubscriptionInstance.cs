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
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	public class MigratingEventSubscriptionInstance : MigratingInstance, RemovingInstance, EmergingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal EventSubscriptionEntity eventSubscriptionEntity;
	  protected internal ScopeImpl targetScope;
	  protected internal bool updateEvent;
	  protected internal EventSubscriptionDeclaration targetDeclaration;

	  protected internal EventSubscriptionDeclaration eventSubscriptionDeclaration;

	  public MigratingEventSubscriptionInstance(EventSubscriptionEntity eventSubscriptionEntity, ScopeImpl targetScope, bool updateEvent, EventSubscriptionDeclaration targetDeclaration)
	  {
		this.eventSubscriptionEntity = eventSubscriptionEntity;
		this.targetScope = targetScope;
		this.updateEvent = updateEvent;
		this.targetDeclaration = targetDeclaration;
	  }

	  public MigratingEventSubscriptionInstance(EventSubscriptionEntity eventSubscriptionEntity) : this(eventSubscriptionEntity, null, false, null)
	  {
	  }

	  public MigratingEventSubscriptionInstance(EventSubscriptionDeclaration eventSubscriptionDeclaration)
	  {
		this.eventSubscriptionDeclaration = eventSubscriptionDeclaration;
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(eventSubscriptionEntity.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		eventSubscriptionEntity.Execution = null;
	  }

	  public virtual void attachState(MigratingScopeInstance newOwningInstance)
	  {
		eventSubscriptionEntity.Execution = newOwningInstance.resolveRepresentativeExecution();
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public virtual void migrateState()
	  {
		if (updateEvent)
		{
		  targetDeclaration.updateSubscription(eventSubscriptionEntity);
		}
		eventSubscriptionEntity.Activity = (ActivityImpl) targetScope;
	  }

	  public virtual void migrateDependentEntities()
	  {
		// do nothing
	  }

	  public virtual void create(ExecutionEntity scopeExecution)
	  {
		eventSubscriptionDeclaration.createSubscriptionForExecution(scopeExecution);
	  }

	  public virtual void remove()
	  {
		eventSubscriptionEntity.delete();
	  }
	}

}