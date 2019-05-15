using System;

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
namespace org.camunda.bpm.engine.cdi
{
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// Signifies an event that is happening / has happened during the execution of a
	/// business process.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface BusinessProcessEvent
	{

	  /// <returns> the process definition in which the event is happening / has
	  ///         happened or null the event was not related to a process definition </returns>
	  ProcessDefinition ProcessDefinition {get;}

	  /// <returns> the id of the activity the process is currently in / was in at the
	  ///         moment the event was fired. </returns>
	  string ActivityId {get;}

	  /// <returns> the name of the transition being taken / that was taken. (null, if
	  ///         this event is not of type <seealso cref="BusinessProcessEventType#TAKE"/> </returns>
	  string TransitionName {get;}

	  /// <returns> the id of the <seealso cref="ProcessInstance"/> this event corresponds to </returns>
	  string ProcessInstanceId {get;}

	  /// <returns> the id of the <seealso cref="Execution"/> this event corresponds to </returns>
	  string ExecutionId {get;}

	  /// <returns> the type of the event </returns>
	  BusinessProcessEventType Type {get;}

	  /// <returns> the timestamp indicating the local time at which the event was
	  ///         fired. </returns>
	  DateTime TimeStamp {get;}

	  /// <returns> the delegate task if this is a task event. </returns>
	  DelegateTask Task {get;}

	  /// <returns> the task id of the current task or null if this is not a task event. </returns>
	  string TaskId {get;}

	  /// <returns> the id of the task in the process definition (BPMN XML) or null if this is not a task event. </returns>
	  string TaskDefinitionKey {get;}
	}

}