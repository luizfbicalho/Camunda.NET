﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// Contains the oddities required by compensation due to the execution structures it creates.
	/// Anything that is a cross-cutting concern, but requires some extra compensation-specific conditions, should go here.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CompensationBehavior
	{

	  /// <summary>
	  /// With compensation, we have a dedicated scope execution for every handler, even if the handler is not
	  /// a scope activity; this must be respected when invoking end listeners, etc.
	  /// </summary>
	  public static bool executesNonScopeCompensationHandler(PvmExecutionImpl execution)
	  {
		ActivityImpl activity = execution.getActivity();

		return execution.Scope && activity != null && activity.CompensationHandler && !activity.Scope;
	  }

	  public static bool isCompensationThrowing(PvmExecutionImpl execution)
	  {
		ActivityImpl currentActivity = execution.getActivity();
		if (currentActivity != null)
		{
		  bool? isCompensationThrowing = (bool?) currentActivity.getProperty(BpmnParse.PROPERTYNAME_THROWS_COMPENSATION);
		  if (isCompensationThrowing != null && isCompensationThrowing)
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Determines whether an execution is responsible for default compensation handling.
	  /// 
	  /// This is the case if
	  /// <ul>
	  ///   <li>the execution has an activity
	  ///   <li>the execution is a scope
	  ///   <li>the activity is a scope
	  ///   <li>the execution has children
	  ///   <li>the execution does not throw compensation
	  /// </ul>
	  /// </summary>
	  public static bool executesDefaultCompensationHandler(PvmExecutionImpl scopeExecution)
	  {
		ActivityImpl currentActivity = scopeExecution.getActivity();

		if (currentActivity != null)
		{
		  return scopeExecution.Scope && currentActivity.Scope && scopeExecution.NonEventScopeExecutions.Count > 0 && !isCompensationThrowing(scopeExecution);
		}
		else
		{
		  return false;
		}
	  }

	  public static string getParentActivityInstanceId(PvmExecutionImpl execution)
	  {
		IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = execution.createActivityExecutionMapping();
		PvmExecutionImpl parentScopeExecution = activityExecutionMapping[execution.getActivity().FlowScope];

		return parentScopeExecution.ParentActivityInstanceId;
	  }

	}

}