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
namespace org.camunda.bpm.engine.impl.pvm.process
{
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// Defines the start behavior for <seealso cref="ActivityImpl activities"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public enum ActivityStartBehavior
	{

	  /// <summary>
	  /// Default start behavior for an activity is to "do nothing special". Meaning:
	  /// the activity is executed by the execution which enters it.
	  /// 
	  /// NOTE: Only activities contained in normal flow can have DEFALUT start behavior.
	  /// </summary>
	  DEFAULT,

	  /// <summary>
	  /// Used for activities which <seealso cref="PvmExecutionImpl#interrupt(String) interrupt"/>
	  /// their <seealso cref="PvmActivity#getFlowScope() flow scope"/>. Examples:
	  /// - Terminate end event
	  /// - Cancel end event
	  /// 
	  /// NOTE: can only be used for activities contained in normal flow
	  /// </summary>
	  INTERRUPT_FLOW_SCOPE,

	  /// <summary>
	  /// Used for activities which are executed concurrently to activities
	  /// within the same <seealso cref="ActivityImpl#getFlowScope() flowScope"/>.
	  /// </summary>
	  CONCURRENT_IN_FLOW_SCOPE,

	  /// <summary>
	  /// Used for activities which <seealso cref="PvmExecutionImpl#interrupt(String) interrupt"/>
	  /// their <seealso cref="PvmActivity#getEventScope() event scope"/>
	  /// 
	  /// NOTE: cannot only be used for activities contained in normal flow
	  /// </summary>
	  INTERRUPT_EVENT_SCOPE,

	  /// <summary>
	  /// Used for activities which cancel their <seealso cref="PvmActivity#getEventScope() event scope"/>.
	  /// - Boundary events with cancelActivity=true
	  /// 
	  /// NOTE: cannot only be used for activities contained in normal flow
	  /// </summary>
	  CANCEL_EVENT_SCOPE

	}

}