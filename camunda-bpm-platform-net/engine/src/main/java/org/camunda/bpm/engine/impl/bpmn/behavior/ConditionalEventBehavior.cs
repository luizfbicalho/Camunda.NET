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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using ConditionalEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ConditionalEventDefinition;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;

	/// <summary>
	/// Represents an interface for the condition event behaviors.
	/// Makes it possible to leave the current activity if the condition of the
	/// conditional event is satisfied.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public interface ConditionalEventBehavior
	{

	  /// <summary>
	  /// Returns the current conditional event definition.
	  /// </summary>
	  /// <returns> the conditional event definition </returns>
	  ConditionalEventDefinition ConditionalEventDefinition {get;}

	  /// <summary>
	  /// Checks the condition, on satisfaction the activity is leaved.
	  /// </summary>
	  /// <param name="eventSubscription"> the event subscription which contains all necessary informations </param>
	  /// <param name="variableEvent"> the variableEvent to evaluate the condition </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: void leaveOnSatisfiedCondition(final org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity eventSubscription, final org.camunda.bpm.engine.impl.core.variable.event.VariableEvent variableEvent);
	  void leaveOnSatisfiedCondition(EventSubscriptionEntity eventSubscription, VariableEvent variableEvent);
	}

}