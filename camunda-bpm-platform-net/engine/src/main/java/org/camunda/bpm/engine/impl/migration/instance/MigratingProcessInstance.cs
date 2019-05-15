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
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingProcessInstance
	{

	  protected internal static readonly MigrationLogger LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal string processInstanceId;
	  protected internal IList<MigratingActivityInstance> migratingActivityInstances;
	  protected internal IList<MigratingTransitionInstance> migratingTransitionInstances;
	  protected internal IList<MigratingEventScopeInstance> migratingEventScopeInstances;
	  protected internal IList<MigratingCompensationEventSubscriptionInstance> migratingCompensationSubscriptionInstances;
	  protected internal MigratingActivityInstance rootInstance;
	  protected internal ProcessDefinitionEntity sourceDefinition;
	  protected internal ProcessDefinitionEntity targetDefinition;

	  public MigratingProcessInstance(string processInstanceId, ProcessDefinitionEntity sourceDefinition, ProcessDefinitionEntity targetDefinition)
	  {
		this.processInstanceId = processInstanceId;
		this.migratingActivityInstances = new List<MigratingActivityInstance>();
		this.migratingTransitionInstances = new List<MigratingTransitionInstance>();
		this.migratingEventScopeInstances = new List<MigratingEventScopeInstance>();
		this.migratingCompensationSubscriptionInstances = new List<MigratingCompensationEventSubscriptionInstance>();
		this.sourceDefinition = sourceDefinition;
		this.targetDefinition = targetDefinition;
	  }

	  public virtual MigratingActivityInstance RootInstance
	  {
		  get
		  {
			return rootInstance;
		  }
		  set
		  {
			this.rootInstance = value;
		  }
	  }


	  public virtual ICollection<MigratingActivityInstance> MigratingActivityInstances
	  {
		  get
		  {
			return migratingActivityInstances;
		  }
	  }

	  public virtual ICollection<MigratingTransitionInstance> MigratingTransitionInstances
	  {
		  get
		  {
			return migratingTransitionInstances;
		  }
	  }

	  public virtual ICollection<MigratingEventScopeInstance> MigratingEventScopeInstances
	  {
		  get
		  {
			return migratingEventScopeInstances;
		  }
	  }

	  public virtual ICollection<MigratingCompensationEventSubscriptionInstance> MigratingCompensationSubscriptionInstances
	  {
		  get
		  {
			return migratingCompensationSubscriptionInstances;
		  }
	  }

	  public virtual ICollection<MigratingScopeInstance> MigratingScopeInstances
	  {
		  get
		  {
			ISet<MigratingScopeInstance> result = new HashSet<MigratingScopeInstance>();
    
			result.addAll(migratingActivityInstances);
			result.addAll(migratingEventScopeInstances);
    
			return result;
		  }
	  }

	  public virtual ProcessDefinitionEntity SourceDefinition
	  {
		  get
		  {
			return sourceDefinition;
		  }
	  }

	  public virtual ProcessDefinitionEntity TargetDefinition
	  {
		  get
		  {
			return targetDefinition;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual MigratingActivityInstance addActivityInstance(MigrationInstruction migrationInstruction, ActivityInstance activityInstance, ScopeImpl sourceScope, ScopeImpl targetScope, ExecutionEntity scopeExecution)
	  {

		MigratingActivityInstance migratingActivityInstance = new MigratingActivityInstance(activityInstance, migrationInstruction, sourceScope, targetScope, scopeExecution);

		migratingActivityInstances.Add(migratingActivityInstance);

		if (processInstanceId.Equals(activityInstance.Id))
		{
		  rootInstance = migratingActivityInstance;
		}

		return migratingActivityInstance;
	  }

	  public virtual MigratingTransitionInstance addTransitionInstance(MigrationInstruction migrationInstruction, TransitionInstance transitionInstance, ScopeImpl sourceScope, ScopeImpl targetScope, ExecutionEntity asyncExecution)
	  {

		MigratingTransitionInstance migratingTransitionInstance = new MigratingTransitionInstance(transitionInstance, migrationInstruction, sourceScope, targetScope, asyncExecution);

		migratingTransitionInstances.Add(migratingTransitionInstance);

		return migratingTransitionInstance;
	  }

	  public virtual MigratingEventScopeInstance addEventScopeInstance(MigrationInstruction migrationInstruction, ExecutionEntity eventScopeExecution, ScopeImpl sourceScope, ScopeImpl targetScope, MigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription, ScopeImpl eventSubscriptionSourceScope, ScopeImpl eventSubscriptionTargetScope)
	  {

		MigratingEventScopeInstance compensationInstance = new MigratingEventScopeInstance(migrationInstruction, eventScopeExecution, sourceScope, targetScope, eventSubscriptionInstruction, eventSubscription, eventSubscriptionSourceScope, eventSubscriptionTargetScope);

		migratingEventScopeInstances.Add(compensationInstance);

		return compensationInstance;
	  }

	  public virtual MigratingCompensationEventSubscriptionInstance addCompensationSubscriptionInstance(MigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription, ScopeImpl sourceScope, ScopeImpl targetScope)
	  {
		MigratingCompensationEventSubscriptionInstance compensationInstance = new MigratingCompensationEventSubscriptionInstance(eventSubscriptionInstruction, sourceScope, targetScope, eventSubscription);

		migratingCompensationSubscriptionInstances.Add(compensationInstance);

		return compensationInstance;
	  }

	}

}