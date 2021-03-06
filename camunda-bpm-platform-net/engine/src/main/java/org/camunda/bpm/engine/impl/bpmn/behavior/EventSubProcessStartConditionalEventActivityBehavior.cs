﻿/*
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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using ConditionalEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ConditionalEventDefinition;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class EventSubProcessStartConditionalEventActivityBehavior : EventSubProcessStartEventActivityBehavior, ConditionalEventBehavior
	{

	  protected internal readonly ConditionalEventDefinition conditionalEvent;

	  public EventSubProcessStartConditionalEventActivityBehavior(ConditionalEventDefinition conditionalEvent)
	  {
		this.conditionalEvent = conditionalEvent;
	  }

	  public virtual ConditionalEventDefinition ConditionalEventDefinition
	  {
		  get
		  {
			return conditionalEvent;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void leaveOnSatisfiedCondition(final org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity eventSubscription, final org.camunda.bpm.engine.impl.core.variable.event.VariableEvent variableEvent)
	  public virtual void leaveOnSatisfiedCondition(EventSubscriptionEntity eventSubscription, VariableEvent variableEvent)
	  {
		PvmExecutionImpl execution = eventSubscription.Execution;

		if (execution != null && !execution.Ended && execution.Scope && conditionalEvent.tryEvaluate(variableEvent, execution))
		{
		  ActivityImpl activity = eventSubscription.Activity;
		  activity = (ActivityImpl) activity.FlowScope;
		  execution.executeEventHandlerActivity(activity);
		}
	  }

	}

}