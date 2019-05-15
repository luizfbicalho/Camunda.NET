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
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	public class MigratingIncident : MigratingInstance
	{

	  protected internal IncidentEntity incident;
	  protected internal ScopeImpl targetScope;
	  protected internal string targetJobDefinitionId;

	  public MigratingIncident(IncidentEntity incident, ScopeImpl targetScope)
	  {
		this.incident = incident;
		this.targetScope = targetScope;
	  }

	  public virtual string TargetJobDefinitionId
	  {
		  set
		  {
			this.targetJobDefinitionId = value;
		  }
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(incident.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		incident.Execution = null;
	  }

	  public virtual void attachState(MigratingScopeInstance newOwningInstance)
	  {
		attachTo(newOwningInstance.resolveRepresentativeExecution());
	  }

	  public virtual void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		attachTo(targetTransitionInstance.resolveRepresentativeExecution());
	  }

	  public virtual void migrateState()
	  {
		incident.ActivityId = targetScope.Id;
		incident.ProcessDefinitionId = targetScope.ProcessDefinition.Id;
		incident.JobDefinitionId = targetJobDefinitionId;

		migrateHistory();
	  }

	  protected internal virtual void migrateHistory()
	  {
		HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;

		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.INCIDENT_MIGRATE, this))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly MigratingIncident outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(MigratingIncident outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricIncidentMigrateEvt(outerInstance.incident);
		  }
	  }

	  public virtual void migrateDependentEntities()
	  {
		// nothing to do
	  }

	  protected internal virtual void attachTo(ExecutionEntity execution)
	  {
		incident.Execution = execution;
	  }
	}

}