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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// Represents the conditional event definition corresponding to the
	/// ConditionalEvent defined by the BPMN 2.0 spec.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[Serializable]
	public class ConditionalEventDefinition : EventSubscriptionDeclaration
	{

	  private const long serialVersionUID = 1L;

	  protected internal string conditionAsString;
	  protected internal readonly Condition condition;
	  protected internal bool interrupting;
	  protected internal string variableName;
	  protected internal ISet<string> variableEvents;
	  protected internal ActivityImpl conditionalActivity;

	  public ConditionalEventDefinition(Condition condition, ActivityImpl conditionalActivity) : base(null, EventType.CONDITONAL)
	  {
		this.activityId = conditionalActivity.ActivityId;
		this.conditionalActivity = conditionalActivity;
		this.condition = condition;
	  }

	  public virtual ActivityImpl ConditionalActivity
	  {
		  get
		  {
			return conditionalActivity;
		  }
		  set
		  {
			this.conditionalActivity = value;
		  }
	  }


	  public virtual bool Interrupting
	  {
		  get
		  {
			return interrupting;
		  }
		  set
		  {
			this.interrupting = value;
		  }
	  }


	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
		  set
		  {
			this.variableName = value;
		  }
	  }


	  public virtual ISet<string> VariableEvents
	  {
		  get
		  {
			return variableEvents;
		  }
		  set
		  {
			this.variableEvents = value;
		  }
	  }


	  public virtual string ConditionAsString
	  {
		  get
		  {
			return conditionAsString;
		  }
		  set
		  {
			this.conditionAsString = value;
		  }
	  }


	  public virtual bool shouldEvaluateForVariableEvent(VariableEvent @event)
	  {
		return ((string.ReferenceEquals(variableName, null) || @event.VariableInstance.Name.Equals(variableName)) && ((variableEvents == null || variableEvents.Count == 0) || variableEvents.Contains(@event.EventName)));
	  }

	  public virtual bool evaluate(DelegateExecution execution)
	  {
		if (condition != null)
		{
		  return condition.evaluate(execution, execution);
		}
		throw new System.InvalidOperationException("Conditional event must have a condition!");
	  }

	  public virtual bool tryEvaluate(DelegateExecution execution)
	  {
		if (condition != null)
		{
		  return condition.tryEvaluate(execution, execution);
		}
		throw new System.InvalidOperationException("Conditional event must have a condition!");
	  }

	  public virtual bool tryEvaluate(VariableEvent variableEvent, DelegateExecution execution)
	  {
		return (variableEvent == null || shouldEvaluateForVariableEvent(variableEvent)) && tryEvaluate(execution);
	  }
	}

}