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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableInstanceHistoryListener : VariableInstanceLifecycleListener<VariableInstanceEntity>
	{

	  public static readonly VariableInstanceHistoryListener INSTANCE = new VariableInstanceHistoryListener();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void onCreate(final VariableInstanceEntity variableInstance, final org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope sourceScope)
	  public virtual void onCreate(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.isHistoryEventProduced(HistoryEventTypes.VARIABLE_INSTANCE_CREATE, variableInstance) && !variableInstance.Transient)
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener outerInstance;

		  private org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance;
		  private AbstractVariableScope sourceScope;

		  public HistoryEventCreatorAnonymousInnerClass(VariableInstanceHistoryListener outerInstance, org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
		  {
			  this.outerInstance = outerInstance;
			  this.variableInstance = variableInstance;
			  this.sourceScope = sourceScope;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricVariableCreateEvt(variableInstance, sourceScope);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void onDelete(final VariableInstanceEntity variableInstance, final org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope sourceScope)
	  public virtual void onDelete(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.isHistoryEventProduced(HistoryEventTypes.VARIABLE_INSTANCE_DELETE, variableInstance) && !variableInstance.Transient)
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener outerInstance;

		  private org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance;
		  private AbstractVariableScope sourceScope;

		  public HistoryEventCreatorAnonymousInnerClass2(VariableInstanceHistoryListener outerInstance, org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
		  {
			  this.outerInstance = outerInstance;
			  this.variableInstance = variableInstance;
			  this.sourceScope = sourceScope;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricVariableDeleteEvt(variableInstance, sourceScope);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void onUpdate(final VariableInstanceEntity variableInstance, final org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope sourceScope)
	  public virtual void onUpdate(VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
	  {
		if (HistoryLevel.isHistoryEventProduced(HistoryEventTypes.VARIABLE_INSTANCE_UPDATE, variableInstance) && !variableInstance.Transient)
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass3(this, variableInstance, sourceScope));
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass3 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly VariableInstanceHistoryListener outerInstance;

		  private org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance;
		  private AbstractVariableScope sourceScope;

		  public HistoryEventCreatorAnonymousInnerClass3(VariableInstanceHistoryListener outerInstance, org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity variableInstance, AbstractVariableScope sourceScope)
		  {
			  this.outerInstance = outerInstance;
			  this.variableInstance = variableInstance;
			  this.sourceScope = sourceScope;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createHistoricVariableUpdateEvt(variableInstance, sourceScope);
		  }
	  }

	  protected internal virtual HistoryLevel HistoryLevel
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryLevel;
		  }
	  }
	}

}