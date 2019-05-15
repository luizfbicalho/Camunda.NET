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
namespace org.camunda.bpm.engine.impl.pvm
{

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityStartBehavior = org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior;


	/// <summary>
	/// Defines an activity insisde a process. Note that the term "activity" is meant to be
	/// understood in a broader sense than in BPMN: everything inside a process which can have incoming
	/// or outgoing sequence flows (transitions) are activities. Examples: events, tasks, gateways,
	/// subprocesses ...
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public interface PvmActivity : PvmScope
	{

	  /// <summary>
	  /// The inner behavior of an activity. The inner behavior is the logic which is executed after
	  /// the <seealso cref="ExecutionListener#EVENTNAME_START start"/> listeners have been executed.
	  /// 
	  /// In case the activity <seealso cref="#isScope() is scope"/>, a new execution will be created
	  /// </summary>
	  /// <returns> the inner behavior of the activity </returns>
	  ActivityBehavior ActivityBehavior {get;}

	  /// <summary>
	  /// The start behavior of an activity. The start behavior is executed before the
	  /// <seealso cref="ExecutionListener#EVENTNAME_START start"/> listeners of the activity are executed.
	  /// </summary>
	  /// <returns> the start behavior of an activity. </returns>
	  ActivityStartBehavior ActivityStartBehavior {get;}

	  /// <summary>
	  /// Finds and returns an outgoing sequence flow (transition) by it's id. </summary>
	  /// <param name="transitionId"> the id of the transition to find </param>
	  /// <returns> the transition or null in case it cannot be found </returns>
	  PvmTransition findOutgoingTransition(string transitionId);

	  /// <returns> the list of outgoing sequence flows (transitions) </returns>
	  IList<PvmTransition> OutgoingTransitions {get;}

	  /// <returns> the list of incoming sequence flows (transitions) </returns>
	  IList<PvmTransition> IncomingTransitions {get;}

	  /// <summary>
	  /// Indicates whether the activity is executed asynchronously.
	  /// This can be done <em>after</em> the <seealso cref="#getActivityStartBehavior() activity start behavior"/> and
	  /// <em>before</em> the <seealso cref="ExecutionListener#EVENTNAME_START start"/> listeners are invoked.
	  /// </summary>
	  /// <returns> true if the activity is executed asynchronously. </returns>
	  bool AsyncBefore {get;}

	  /// <summary>
	  /// Indicates whether execution after this execution should continue asynchronously.
	  /// This can be done <em>after</em> the <seealso cref="ExecutionListener#EVENTNAME_END end"/> listeners are invoked. </summary>
	  /// <returns> true if execution after this activity continues asynchronously. </returns>
	  bool AsyncAfter {get;}
	}

}