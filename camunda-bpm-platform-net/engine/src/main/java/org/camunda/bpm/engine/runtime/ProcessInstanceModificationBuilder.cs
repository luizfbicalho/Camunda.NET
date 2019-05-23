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
namespace org.camunda.bpm.engine.runtime
{
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;

	/// <summary>
	/// <para>A fluent builder to specify a modification of process instance state in terms
	/// of cancellation of activity instances and instantiations of activities and sequence flows.
	/// Allows to specify an ordered set of instructions that are all executed within one
	/// transaction. Individual instructions are executed in the order of their specification.</para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public interface ProcessInstanceModificationBuilder : InstantiationBuilder<ProcessInstanceModificationInstantiationBuilder>
	{

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Start before the specified activity. Instantiate the given activity
	  /// as a descendant of the given ancestor activity instance.</para>
	  /// 
	  /// <para>In particular:
	  ///   <ul>
	  ///     <li>Instantiate all activities between the ancestor activity and the activity to execute</li>
	  ///     <li>Instantiate and execute the given activity (respects the asyncBefore
	  ///       attribute of the activity)</li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="activityId"> the activity to instantiate </param>
	  /// <param name="ancestorActivityInstanceId"> the ID of an existing activity instance under which the new
	  ///   activity instance should be created </param>
	  ProcessInstanceModificationInstantiationBuilder startBeforeActivity(string activityId, string ancestorActivityInstanceId);

	  /// <summary>
	  /// Submits an instruction that behaves like <seealso cref="startTransition(string,string)"/> and always instantiates
	  /// the single outgoing sequence flow of the given activity. Does not consider asyncAfter.
	  /// </summary>
	  /// <param name="activityId"> the activity for which the outgoing flow should be executed </param>
	  /// <exception cref="ProcessEngineException"> if the activity has 0 or more than 1 outgoing sequence flows </exception>
	  ProcessInstanceModificationInstantiationBuilder startAfterActivity(string activityId, string ancestorActivityInstanceId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Start the specified sequence flow. Instantiate the given sequence flow
	  /// as a descendant of the given ancestor activity instance.</para>
	  /// 
	  /// <para>In particular:
	  ///   <ul>
	  ///     <li>Instantiate all activities between the ancestor activity and the activity to execute</li>
	  ///     <li>Execute the given transition (does not consider sequence flow conditions)</li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="transitionId"> the sequence flow to execute </param>
	  /// <param name="ancestorActivityInstanceId"> the ID of an existing activity instance under which the new
	  ///   transition should be executed </param>
	  ProcessInstanceModificationInstantiationBuilder startTransition(string transitionId, string ancestorActivityInstanceId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Cancel an activity instance in a process. If this instance has child activity instances
	  /// (e.g. in a subprocess instance), these children, their grandchildren, etc. are cancelled as well.</para>
	  /// 
	  /// <para>Process instance cancellation will propagate upward, removing any parent process instances that are
	  /// only waiting on the cancelled process to complete.</para>
	  /// </summary>
	  /// <param name="activityInstanceId"> the id of the activity instance to cancel </param>
	  ProcessInstanceModificationBuilder cancelActivityInstance(string activityInstanceId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Cancel a transition instance (i.e. an async continuation) in a process.</para>
	  /// </summary>
	  /// <param name="transitionInstanceId"> the id of the transition instance to cancel </param>
	  ProcessInstanceModificationBuilder cancelTransitionInstance(string transitionInstanceId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Cancel all instances of the given activity in an arbitrary order, which are:
	  /// <ul>
	  ///   <li>activity instances of that activity
	  ///   <li>transition instances entering or leaving that activity
	  /// </ul></para>
	  /// 
	  /// <para>Therefore behaves like <seealso cref="cancelActivityInstance(string)"/> for each individual
	  /// activity instance and like <seealso cref="cancelTransitionInstance(string)"/> for each
	  /// individual transition instance.</para>
	  /// 
	  /// <para>The cancellation order of the instances is arbitrary</para>
	  /// </summary>
	  /// <param name="activityId"> the activity for which all instances should be cancelled </param>
	  ProcessInstanceModificationBuilder cancelAllForActivity(string activityId);

	  /// <summary>
	  /// Execute all instructions. Custom execution and task listeners, as well as task input output mappings
	  /// are executed.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  ///          if the process instance will be delete and the user has no <seealso cref="Permissions.DELETE"/> permission
	  ///          on <seealso cref="Resources.PROCESS_INSTANCE"/> or no <seealso cref="Permissions.DELETE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void execute();

	  /// <param name="skipCustomListeners"> specifies whether custom listeners (task and execution)
	  ///   should be invoked when executing the instructions </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked
	  ///   throughout the transaction when executing the instructions
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  ///          if the process instance will be delete and the user has no <seealso cref="Permissions.DELETE"/> permission
	  ///          on <seealso cref="Resources.PROCESS_INSTANCE"/> or no <seealso cref="Permissions.DELETE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void execute(bool skipCustomListeners, bool skipIoMappings);

	  /// <summary>
	  /// Execute all instructions asynchronously. Custom execution and task listeners, as well as task input output mappings
	  /// are executed.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///              if the user has no <seealso cref="Permissions.CREATE"/> or
	  ///              <seealso cref="BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES"/> permission on <seealso cref="Resources.BATCH"/>.
	  /// </exception>
	  /// <returns> a batch job to be executed by the executor </returns>
	  Batch executeAsync();

	  /// <param name="skipCustomListeners"> specifies whether custom listeners (task and execution)
	  ///   should be invoked when executing the instructions </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked
	  ///   throughout the transaction when executing the instructions
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///               if the user has no <seealso cref="Permissions.CREATE"/> or
	  ///               <seealso cref="BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES"/> permission on <seealso cref="Resources.BATCH"/>.
	  /// </exception>
	  /// <returns> a batch job to be executed by the executor </returns>
	  Batch executeAsync(bool skipCustomListeners, bool skipIoMappings);

	}

}