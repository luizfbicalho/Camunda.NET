using System;
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
namespace org.camunda.bpm.engine.test.bpmn.multiinstance
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DelegateEvent : DelegateExecution
	{

	  protected internal static readonly IList<DelegateEvent> RECORDED_EVENTS = new List<DelegateEvent>();

	  protected internal string activityInstanceId;
	  protected internal string businessKey;
	  protected internal string currentActivityId;
	  protected internal string currentActivityName;
	  protected internal string currentTransitionId;
	  protected internal string eventName;
	  protected internal string id;
	  protected internal string parentActivityInstanceId;
	  protected internal string parentId;
	  protected internal string processBusinessKey;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string tenantId;
	  protected internal string variableScopeKey;

	  public static DelegateEvent fromExecution(DelegateExecution delegateExecution)
	  {
		DelegateEvent @event = new DelegateEvent();

		@event.activityInstanceId = delegateExecution.ActivityInstanceId;
		@event.businessKey = delegateExecution.BusinessKey;
		@event.currentActivityId = delegateExecution.CurrentActivityId;
		@event.currentActivityName = delegateExecution.CurrentActivityName;
		@event.currentTransitionId = delegateExecution.CurrentTransitionId;
		@event.eventName = delegateExecution.EventName;
		@event.id = delegateExecution.Id;
		@event.parentActivityInstanceId = delegateExecution.ParentActivityInstanceId;
		@event.parentId = delegateExecution.ParentId;
		@event.processBusinessKey = delegateExecution.ProcessBusinessKey;
		@event.processDefinitionId = delegateExecution.ProcessDefinitionId;
		@event.processInstanceId = delegateExecution.ProcessInstanceId;
		@event.tenantId = delegateExecution.TenantId;
		@event.variableScopeKey = delegateExecution.VariableScopeKey;

		return @event;
	  }

	  public static void clearEvents()
	  {
		RECORDED_EVENTS.Clear();
	  }

	  public static void recordEventFor(DelegateExecution execution)
	  {
		RECORDED_EVENTS.Add(DelegateEvent.fromExecution(execution));
	  }

	  public static IList<DelegateEvent> Events
	  {
		  get
		  {
			return RECORDED_EVENTS;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string VariableScopeKey
	  {
		  get
		  {
			return variableScopeKey;
		  }
	  }

	  protected internal virtual Exception notYetImplemented()
	  {
		return new Exception("Recording this method is not implemented");
	  }

	  protected internal virtual Exception cannotModifyState()
	  {
		return new Exception("This event is read-only; cannot modify state");
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			throw notYetImplemented();
		  }
		  set
		  {
			throw cannotModifyState();
		  }
	  }

	  public virtual VariableMap VariablesTyped
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual VariableMap getVariablesTyped(bool deserializeValues)
	  {
		throw notYetImplemented();
	  }

	  public virtual IDictionary<string, object> VariablesLocal
	  {
		  get
		  {
			throw notYetImplemented();
		  }
		  set
		  {
			throw cannotModifyState();
		  }
	  }

	  public virtual VariableMap VariablesLocalTyped
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual VariableMap getVariablesLocalTyped(bool deserializeValues)
	  {
		throw notYetImplemented();
	  }

	  public virtual object getVariable(string variableName)
	  {
		throw notYetImplemented();
	  }

	  public virtual object getVariableLocal(string variableName)
	  {
		throw notYetImplemented();
	  }

	  public virtual T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		throw notYetImplemented();
	  }

	  public virtual T getVariableTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		throw notYetImplemented();
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		throw notYetImplemented();
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		throw notYetImplemented();
	  }

	  public virtual ISet<string> VariableNames
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual ISet<string> VariableNamesLocal
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		throw cannotModifyState();
	  }

	  public virtual void setVariableLocal(string variableName, object value)
	  {
		throw cannotModifyState();
	  }



	  public virtual bool hasVariables()
	  {
		throw notYetImplemented();
	  }

	  public virtual bool hasVariablesLocal()
	  {
		throw notYetImplemented();
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		throw notYetImplemented();
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		throw notYetImplemented();
	  }

	  public virtual void removeVariable(string variableName)
	  {
		throw cannotModifyState();
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		throw cannotModifyState();
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		throw cannotModifyState();
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		throw cannotModifyState();
	  }

	  public virtual void removeVariables()
	  {
		throw cannotModifyState();
	  }

	  public virtual void removeVariablesLocal()
	  {
		throw cannotModifyState();
	  }

	  public virtual BpmnModelInstance BpmnModelInstance
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual FlowElement BpmnModelElementInstance
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string CurrentActivityId
	  {
		  get
		  {
			return currentActivityId;
		  }
	  }

	  public virtual string CurrentActivityName
	  {
		  get
		  {
			return currentActivityName;
		  }
	  }

	  public virtual string CurrentTransitionId
	  {
		  get
		  {
			return currentTransitionId;
		  }
	  }

	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			return parentActivityInstanceId;
		  }
	  }

	  public virtual string ParentId
	  {
		  get
		  {
			return parentId;
		  }
	  }

	  public virtual string ProcessBusinessKey
	  {
		  get
		  {
			return processBusinessKey;
		  }
		  set
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual void setVariable(string variableName, object value, string activityId)
	  {
		this.cannotModifyState();
	  }

	  public virtual DelegateExecution ProcessInstance
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual DelegateExecution SuperExecution
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual bool Canceled
	  {
		  get
		  {
			throw notYetImplemented();
		  }
	  }

	  public virtual Incident createIncident(string incidentType, string configuration)
	  {
		throw notYetImplemented();
	  }

	  public virtual void resolveIncident(string incidentId)
	  {
		throw notYetImplemented();
	  }

	  public virtual Incident createIncident(string incidentType, string configuration, string message)
	  {
		throw notYetImplemented();
	  }

	}
}