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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingUserTaskInstance : MigratingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal TaskEntity userTask;
	  protected internal MigratingActivityInstance migratingActivityInstance;

	  public MigratingUserTaskInstance(TaskEntity userTask, MigratingActivityInstance migratingActivityInstance)
	  {
		this.userTask = userTask;
		this.migratingActivityInstance = migratingActivityInstance;
	  }

	  public virtual void migrateDependentEntities()
	  {
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(userTask.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		userTask.getExecution().removeTask(userTask);
		userTask.setExecution(null);
	  }

	  public virtual void attachState(MigratingScopeInstance owningInstance)
	  {
		ExecutionEntity representativeExecution = owningInstance.resolveRepresentativeExecution();
		representativeExecution.addTask(userTask);

		foreach (VariableInstanceEntity variable in userTask.VariablesInternal)
		{
		  variable.Execution = representativeExecution;
		}

		userTask.setExecution(representativeExecution);
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public virtual void migrateState()
	  {
		userTask.ProcessDefinitionId = migratingActivityInstance.TargetScope.ProcessDefinition.Id;
		userTask.TaskDefinitionKey = migratingActivityInstance.TargetScope.Id;

		migrateHistory();
	  }

	  protected internal virtual void migrateHistory()
	  {
		HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;

		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.TASK_INSTANCE_MIGRATE, this))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly MigratingUserTaskInstance outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(MigratingUserTaskInstance outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createTaskInstanceMigrateEvt(outerInstance.userTask);
		  }
	  }
	}

}