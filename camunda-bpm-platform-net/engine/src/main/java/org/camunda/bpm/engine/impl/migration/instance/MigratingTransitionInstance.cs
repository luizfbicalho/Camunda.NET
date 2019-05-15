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

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingTransitionInstance : MigratingProcessElementInstance, MigratingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal ExecutionEntity representativeExecution;

	  protected internal TransitionInstance transitionInstance;
	  protected internal MigratingAsyncJobInstance jobInstance;
	  protected internal IList<MigratingInstance> migratingDependentInstances = new List<MigratingInstance>();
	  protected internal bool activeState;


	  public MigratingTransitionInstance(TransitionInstance transitionInstance, MigrationInstruction migrationInstruction, ScopeImpl sourceScope, ScopeImpl targetScope, ExecutionEntity asyncExecution)
	  {
		this.transitionInstance = transitionInstance;
		this.migrationInstruction = migrationInstruction;
		this.sourceScope = sourceScope;
		this.targetScope = targetScope;
		this.currentScope = sourceScope;
		this.representativeExecution = asyncExecution;
		this.activeState = representativeExecution.Active;
	  }

	  public override bool Detached
	  {
		  get
		  {
			return jobInstance.Detached;
		  }
	  }

	  public override MigratingActivityInstance getParent()
	  {
		return (MigratingActivityInstance) base.Parent;
	  }

	  public override void detachState()
	  {

		jobInstance.detachState();
		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.detachState();
		}

		ExecutionEntity execution = resolveRepresentativeExecution();
		execution.Active = false;
		getParent().destroyAttachableExecution(execution);

		setParent(null);
	  }

	  public override void attachState(MigratingScopeInstance scopeInstance)
	  {
		if (!(scopeInstance is MigratingActivityInstance))
		{
		  throw MIGRATION_LOGGER.cannotHandleChild(scopeInstance, this);
		}

		MigratingActivityInstance activityInstance = (MigratingActivityInstance) scopeInstance;

		setParent(activityInstance);

		representativeExecution = activityInstance.createAttachableExecution();
		representativeExecution.ActivityInstanceId = null;
		representativeExecution.Active = activeState;

		jobInstance.attachState(this);

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.attachState(this);
		}
	  }

	  public override ExecutionEntity resolveRepresentativeExecution()
	  {
		if (representativeExecution.ReplacedBy != null)
		{
		  return representativeExecution.resolveReplacedBy();
		}
		else
		{
		  return representativeExecution;
		}
	  }

	  public override void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {

		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public virtual MigratingAsyncJobInstance DependentJobInstance
	  {
		  set
		  {
			this.jobInstance = value;
		  }
	  }

	  public override void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
		migratingDependentInstances.Add(migratingInstance);
	  }

	  public virtual IList<MigratingInstance> MigratingDependentInstances
	  {
		  get
		  {
			return migratingDependentInstances;
		  }
	  }

	  public override void migrateState()
	  {
		ExecutionEntity representativeExecution = resolveRepresentativeExecution();

		representativeExecution.setProcessDefinition(targetScope.ProcessDefinition);
		representativeExecution.setActivity((PvmActivity) targetScope);
	  }

	  public override void migrateDependentEntities()
	  {
		jobInstance.migrateState();
		jobInstance.migrateDependentEntities();

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.migrateState();
		  dependentInstance.migrateDependentEntities();
		}
	  }

	  public virtual TransitionInstance TransitionInstance
	  {
		  get
		  {
			return transitionInstance;
		  }
	  }

	  /// <summary>
	  /// Else asyncBefore
	  /// </summary>
	  public virtual bool AsyncAfter
	  {
		  get
		  {
			return jobInstance.AsyncAfter;
		  }
	  }

	  public virtual bool AsyncBefore
	  {
		  get
		  {
			return jobInstance.AsyncBefore;
		  }
	  }

	  public virtual MigratingJobInstance JobInstance
	  {
		  get
		  {
			return jobInstance;
		  }
	  }

	  public override void setParent(MigratingScopeInstance parentInstance)
	  {
		if (parentInstance != null && !(parentInstance is MigratingActivityInstance))
		{
		  throw MIGRATION_LOGGER.cannotHandleChild(parentInstance, this);
		}

		MigratingActivityInstance parentActivityInstance = (MigratingActivityInstance) parentInstance;

		if (this.parentInstance != null)
		{
		  ((MigratingActivityInstance) this.parentInstance).removeChild(this);
		}

		this.parentInstance = parentActivityInstance;

		if (parentInstance != null)
		{
		  parentActivityInstance.addChild(this);
		}
	  }

	}

}