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
namespace org.camunda.bpm.engine.impl.migration.validation.activity
{

	using BoundaryEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using EventSubProcessStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// Validator for events that passively wait for an event, i.e. without being activated by sequence flow (e.g. boundary events
	/// and event subprocess start events but not intermediate catch events).
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class SupportedPassiveEventTriggerActivityValidator : MigrationActivityValidator
	{

	  public static SupportedPassiveEventTriggerActivityValidator INSTANCE = new SupportedPassiveEventTriggerActivityValidator();

	  public static readonly IList<string> supportedTypes = Arrays.asList(ActivityTypes.BOUNDARY_MESSAGE, ActivityTypes.BOUNDARY_SIGNAL, ActivityTypes.BOUNDARY_TIMER, ActivityTypes.BOUNDARY_COMPENSATION, ActivityTypes.BOUNDARY_CONDITIONAL, ActivityTypes.START_EVENT_MESSAGE, ActivityTypes.START_EVENT_SIGNAL, ActivityTypes.START_EVENT_TIMER, ActivityTypes.START_EVENT_COMPENSATION, ActivityTypes.START_EVENT_CONDITIONAL);

	  public virtual bool valid(ActivityImpl activity)
	  {
		return activity != null && (!isPassivelyWaitingEvent(activity) || isSupportedEventType(activity));
	  }

	  public virtual bool isPassivelyWaitingEvent(ActivityImpl activity)
	  {
		return activity.ActivityBehavior is BoundaryEventActivityBehavior || activity.ActivityBehavior is EventSubProcessStartEventActivityBehavior;
	  }

	  public virtual bool isSupportedEventType(ActivityImpl activity)
	  {
		return supportedTypes.Contains(activity.Properties.get(BpmnProperties.TYPE));
	  }

	}

}