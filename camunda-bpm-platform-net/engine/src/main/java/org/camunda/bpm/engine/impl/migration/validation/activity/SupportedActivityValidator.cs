﻿using System;
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

	using org.camunda.bpm.engine.impl.bpmn.behavior;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// *Supported* refers to whether an activity instance of a certain activity type can be migrated.
	/// This validator is irrelevant for transition instances which can be migrated at any activity type.
	/// Thus, this validator is only used during migration instruction generation and migrating activity instance validation,
	/// not during migration instruction validation.
	/// </summary>
	public class SupportedActivityValidator : MigrationActivityValidator
	{

	  public static SupportedActivityValidator INSTANCE = new SupportedActivityValidator();

	  public static IList<Type> SUPPORTED_ACTIVITY_BEHAVIORS = new List<Type>();

	  static SupportedActivityValidator()
	  {
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(SubProcessActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(UserTaskActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(BoundaryEventActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(ParallelMultiInstanceActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(SequentialMultiInstanceActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(ReceiveTaskActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(CallActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(CaseCallActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(IntermediateCatchEventActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(EventBasedGatewayActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(EventSubProcessActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(EventSubProcessStartEventActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(ExternalTaskActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(ParallelGatewayActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(InclusiveGatewayActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(IntermediateConditionalEventBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(BoundaryConditionalEventActivityBehavior));
		SUPPORTED_ACTIVITY_BEHAVIORS.Add(typeof(EventSubProcessStartConditionalEventActivityBehavior));
	  }

	  public virtual bool valid(ActivityImpl activity)
	  {
		return activity != null && (isSupportedActivity(activity) || isAsync(activity));
	  }

	  public virtual bool isSupportedActivity(ActivityImpl activity)
	  {
		return SUPPORTED_ACTIVITY_BEHAVIORS.Contains(activity.ActivityBehavior.GetType());
	  }

	  protected internal virtual bool isAsync(ActivityImpl activity)
	  {
		return activity.AsyncBefore || activity.AsyncAfter;
	  }

	}

}