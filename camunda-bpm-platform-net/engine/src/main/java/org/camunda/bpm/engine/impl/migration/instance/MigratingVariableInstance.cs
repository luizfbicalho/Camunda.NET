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
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingVariableInstance : MigratingInstance
	{

	  protected internal VariableInstanceEntity variable;
	  protected internal bool isConcurrentLocalInParentScope;

	  public MigratingVariableInstance(VariableInstanceEntity variable, bool isConcurrentLocalInParentScope)
	  {
		this.variable = variable;
		this.isConcurrentLocalInParentScope = isConcurrentLocalInParentScope;
	  }

	  public virtual bool Detached
	  {
		  get
		  {
			return string.ReferenceEquals(variable.ExecutionId, null);
		  }
	  }

	  public virtual void detachState()
	  {
		variable.Execution.removeVariableInternal(variable);
	  }

	  public virtual void attachState(MigratingScopeInstance owningActivityInstance)
	  {
		ExecutionEntity representativeExecution = owningActivityInstance.resolveRepresentativeExecution();
		ScopeImpl currentScope = owningActivityInstance.CurrentScope;

		ExecutionEntity newOwningExecution = representativeExecution;

		if (currentScope.Scope && isConcurrentLocalInParentScope)
		{
		  newOwningExecution = representativeExecution.Parent;
		}

		newOwningExecution.addVariableInternal(variable);
	  }

	  public virtual void attachState(MigratingTransitionInstance owningActivityInstance)
	  {
		ExecutionEntity representativeExecution = owningActivityInstance.resolveRepresentativeExecution();

		representativeExecution.addVariableInternal(variable);
	  }

	  public virtual void migrateState()
	  {
		migrateHistory();
	  }

	  protected internal virtual void migrateHistory()
	  {
		HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;

		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.VARIABLE_INSTANCE_MIGRATE, this))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly MigratingVariableInstance outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(MigratingVariableInstance outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricVariableMigrateEvt(outerInstance.variable);
		  }
	  }

	  public virtual void migrateDependentEntities()
	  {
		// nothing to do
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variable.Name;
		  }
	  }

	}

}