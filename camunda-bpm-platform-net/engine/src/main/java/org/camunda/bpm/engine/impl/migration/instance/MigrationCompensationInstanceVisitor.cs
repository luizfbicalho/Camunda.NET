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
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCompensationInstanceVisitor : MigratingProcessElementInstanceVisitor
	{

	  protected internal override bool canMigrate(MigratingProcessElementInstance instance)
	  {
		return instance is MigratingEventScopeInstance || instance is MigratingCompensationEventSubscriptionInstance;
	  }

	  protected internal override void instantiateScopes(MigratingScopeInstance ancestorScopeInstance, MigratingScopeInstanceBranch executionBranch, IList<ScopeImpl> scopesToInstantiate)
	  {

		if (scopesToInstantiate.Count == 0)
		{
		  return;
		}

		ExecutionEntity ancestorScopeExecution = ancestorScopeInstance.resolveRepresentativeExecution();

		ExecutionEntity parentExecution = ancestorScopeExecution;

		foreach (ScopeImpl scope in scopesToInstantiate)
		{
		  ExecutionEntity compensationScopeExecution = parentExecution.createExecution();
		  compensationScopeExecution.Scope = true;
		  compensationScopeExecution.EventScope = true;

		  compensationScopeExecution.setActivity((PvmActivity) scope);
		  compensationScopeExecution.Active = false;
		  compensationScopeExecution.activityInstanceStarting();
		  compensationScopeExecution.enterActivityInstance();

		  EventSubscriptionEntity eventSubscription = EventSubscriptionEntity.createAndInsert(parentExecution, EventType.COMPENSATE, (ActivityImpl) scope);
		  eventSubscription.Configuration = compensationScopeExecution.Id;

		  executionBranch.visited(new MigratingEventScopeInstance(eventSubscription, compensationScopeExecution, scope));

		  parentExecution = compensationScopeExecution;
		}

	  }

	}

}