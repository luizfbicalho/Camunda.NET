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
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	public abstract class MigratingJobInstance : MigratingInstance, RemovingInstance
	{

	  protected internal JobEntity jobEntity;
	  protected internal JobDefinitionEntity targetJobDefinitionEntity;
	  protected internal ScopeImpl targetScope;

	  protected internal IList<MigratingInstance> migratingDependentInstances = new List<MigratingInstance>();

	  public MigratingJobInstance(JobEntity jobEntity, JobDefinitionEntity jobDefinitionEntity, ScopeImpl targetScope)
	  {
		this.jobEntity = jobEntity;
		this.targetJobDefinitionEntity = jobDefinitionEntity;
		this.targetScope = targetScope;
	  }

	  public MigratingJobInstance(JobEntity jobEntity) : this(jobEntity, null, null)
	  {
	  }

	  public virtual JobEntity JobEntity
	  {
		  get
		  {
			return jobEntity;
		  }
	  }

	  public virtual void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
		migratingDependentInstances.Add(migratingInstance);
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(jobEntity.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		jobEntity.Execution = null;

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.detachState();
		}
	  }

	  public virtual void attachState(MigratingScopeInstance newOwningInstance)
	  {
		attachTo(newOwningInstance.resolveRepresentativeExecution());

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.attachState(newOwningInstance);
		}
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		attachTo(targetTransitionInstance.resolveRepresentativeExecution());

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.attachState(targetTransitionInstance);
		}
	  }

	  protected internal virtual void attachTo(ExecutionEntity execution)
	  {
		jobEntity.Execution = execution;
	  }

	  public virtual void migrateState()
	  {
		// update activity reference
		string activityId = targetScope.Id;
		jobEntity.ActivityId = activityId;
		migrateJobHandlerConfiguration();

		if (targetJobDefinitionEntity != null)
		{
		  jobEntity.JobDefinition = targetJobDefinitionEntity;
		}

		// update process definition reference
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) targetScope.ProcessDefinition;
		jobEntity.ProcessDefinitionId = processDefinition.Id;
		jobEntity.ProcessDefinitionKey = processDefinition.Key;

		// update deployment reference
		jobEntity.DeploymentId = processDefinition.DeploymentId;
	  }

	  public virtual void migrateDependentEntities()
	  {
		foreach (MigratingInstance migratingDependentInstance in migratingDependentInstances)
		{
		  migratingDependentInstance.migrateState();
		}
	  }

	  public virtual void remove()
	  {
		jobEntity.delete();
	  }

	  public virtual bool migrates()
	  {
		return targetScope != null;
	  }

	  public virtual ScopeImpl TargetScope
	  {
		  get
		  {
			return targetScope;
		  }
	  }

	  public virtual JobDefinitionEntity TargetJobDefinitionEntity
	  {
		  get
		  {
			return targetJobDefinitionEntity;
		  }
	  }

	  protected internal abstract void migrateJobHandlerConfiguration();

	}

}