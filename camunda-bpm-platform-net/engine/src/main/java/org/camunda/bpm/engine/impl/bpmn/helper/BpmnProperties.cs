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
namespace org.camunda.bpm.engine.impl.bpmn.helper
{
	using ErrorEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ErrorEventDefinition;
	using EscalationEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.EscalationEventDefinition;
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using PropertyKey = org.camunda.bpm.engine.impl.core.model.PropertyKey;
	using PropertyListKey = org.camunda.bpm.engine.impl.core.model.PropertyListKey;
	using PropertyMapKey = org.camunda.bpm.engine.impl.core.model.PropertyMapKey;
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse.PROPERTYNAME_HAS_CONDITIONAL_EVENTS;

	using ConditionalEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ConditionalEventDefinition;

	/// <summary>
	/// Properties of bpmn elements.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	/// <seealso cref= Properties
	///  </seealso>
	public class BpmnProperties
	{

	  public static readonly PropertyKey<string> TYPE = new PropertyKey<string>("type");

	  public static readonly PropertyListKey<EscalationEventDefinition> ESCALATION_EVENT_DEFINITIONS = new PropertyListKey<EscalationEventDefinition>("escalationEventDefinitions");

	  public static readonly PropertyListKey<ErrorEventDefinition> ERROR_EVENT_DEFINITIONS = new PropertyListKey<ErrorEventDefinition>("errorEventDefinitions");

	  /// <summary>
	  /// Declaration indexed by activity that is triggered by the event; assumes that there is at most one such declaration per activity.
	  /// There is code that relies on this assumption (e.g. when determining which declaration matches a job in the migration logic).
	  /// </summary>
	  public static readonly PropertyMapKey<string, TimerDeclarationImpl> TIMER_DECLARATIONS = new PropertyMapKey<string, TimerDeclarationImpl>("timerDeclarations", false);

	  /// <summary>
	  /// Declaration indexed by activity that is triggered by the event; assumes that there is at most one such declaration per activity.
	  /// There is code that relies on this assumption (e.g. when determining which declaration matches a job in the migration logic).
	  /// </summary>
	  public static readonly PropertyMapKey<string, EventSubscriptionDeclaration> EVENT_SUBSCRIPTION_DECLARATIONS = new PropertyMapKey<string, EventSubscriptionDeclaration>("eventDefinitions", false);

	  public static readonly PropertyKey<ActivityImpl> COMPENSATION_BOUNDARY_EVENT = new PropertyKey<ActivityImpl>("compensationBoundaryEvent");

	  public static readonly PropertyKey<ActivityImpl> INITIAL_ACTIVITY = new PropertyKey<ActivityImpl>("initial");

	  public static readonly PropertyKey<bool> TRIGGERED_BY_EVENT = new PropertyKey<bool>("triggeredByEvent");

	  public static readonly PropertyKey<bool> HAS_CONDITIONAL_EVENTS = new PropertyKey<bool>(PROPERTYNAME_HAS_CONDITIONAL_EVENTS);

	  public static readonly PropertyKey<ConditionalEventDefinition> CONDITIONAL_EVENT_DEFINITION = new PropertyKey<ConditionalEventDefinition>("conditionalEventDefinition");

	}

}