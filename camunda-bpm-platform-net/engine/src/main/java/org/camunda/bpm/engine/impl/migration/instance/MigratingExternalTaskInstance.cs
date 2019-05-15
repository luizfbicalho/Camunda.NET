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
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingExternalTaskInstance : MigratingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal ExternalTaskEntity externalTask;
	  protected internal MigratingActivityInstance migratingActivityInstance;

	  protected internal IList<MigratingInstance> dependentInstances = new List<MigratingInstance>();

	  public MigratingExternalTaskInstance(ExternalTaskEntity externalTask, MigratingActivityInstance migratingActivityInstance)
	  {
		this.externalTask = externalTask;
		this.migratingActivityInstance = migratingActivityInstance;
	  }

	  public virtual void migrateDependentEntities()
	  {
		foreach (MigratingInstance migratingDependentInstance in dependentInstances)
		{
		  migratingDependentInstance.migrateState();
		}
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(externalTask.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		externalTask.Execution.removeExternalTask(externalTask);
		externalTask.Execution = null;
	  }

	  public virtual void attachState(MigratingScopeInstance owningInstance)
	  {
		ExecutionEntity representativeExecution = owningInstance.resolveRepresentativeExecution();
		representativeExecution.addExternalTask(externalTask);

		externalTask.Execution = representativeExecution;
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public virtual void migrateState()
	  {
		ScopeImpl targetActivity = migratingActivityInstance.TargetScope;
		ProcessDefinition targetProcessDefinition = (ProcessDefinition) targetActivity.ProcessDefinition;

		externalTask.ActivityId = targetActivity.Id;
		externalTask.ProcessDefinitionId = targetProcessDefinition.Id;
		externalTask.ProcessDefinitionKey = targetProcessDefinition.Key;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return externalTask.Id;
		  }
	  }

	  public virtual ScopeImpl TargetScope
	  {
		  get
		  {
			return migratingActivityInstance.TargetScope;
		  }
	  }

	  public virtual void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
		dependentInstances.Add(migratingInstance);
	  }
	}

}