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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{

	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using EventListener = org.camunda.bpm.model.cmmn.instance.EventListener;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using IfPart = org.camunda.bpm.model.cmmn.instance.IfPart;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItemOnPart = org.camunda.bpm.model.cmmn.instance.PlanItemOnPart;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;
	using UserEventListener = org.camunda.bpm.model.cmmn.instance.UserEventListener;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CmmnActivityExecution : DelegateCaseExecution
	{

	  /// <summary>
	  /// <para>Returns the parent of <code>this</code> case execution, or null
	  /// if there is no parent.</para>
	  /// </summary>
	  /// <returns> a <seealso cref="CmmnActivityExecution parent"/> or null. </returns>
	  CmmnActivityExecution Parent {get;}

	  /// <summary>
	  /// <para>Returns <code>true</code> if <code>this</code> case execution
	  /// is a case instance. If <code>this</code> case execution is not a
	  /// case instance then <code>false</code> will be returned.</para>
	  /// </summary>
	  /// <returns> whether <code>this</code> case execution is a case instance or not. </returns>
	  bool CaseInstanceExecution {get;}

	  /// <summary>
	  /// <para>Returns the <seealso cref="CaseExecutionState current state"/> of <code>this</code>
	  /// case execution.</para>
	  /// </summary>
	  /// <returns> the <seealso cref="CaseExecutionState current state"/> </returns>
	  CaseExecutionState CurrentState {get;set;}


	  /// <summary>
	  /// <para>Returns the <seealso cref="CaseExecutionState previous state"/> of <code>this</code>
	  /// case execution.</para>
	  /// </summary>
	  /// <returns> the <seealso cref="CaseExecutionState previous state"/> </returns>
	  CaseExecutionState PreviousState {get;}

	  /// <summary>
	  /// <para>Returns <code>true</code> iff:<br>
	  ///  <code><seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.NEW"/></code>
	  /// </para>
	  /// </summary>
	  /// <returns> whether <code>this</code> case execution has as current state <seealso cref="CaseExecutionState.NEW"/> </returns>
	  bool New {get;}

	  /// <summary>
	  /// <para>Returns <code>true</code> iff:<br>
	  ///  <code><seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.TERMINATING_ON_TERMINATION"/>
	  ///        || <seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.TERMINATING_ON_PARENT_TERMINATION"/>
	  ///        || <seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.TERMINATING_ON_EXIT"/></code>
	  /// </para>
	  /// </summary>
	  /// <returns> whether <code>this</code> case execution has as current state <seealso cref="CaseExecutionState.TERMINATING_ON_TERMINATION"/>,
	  ///         <seealso cref="CaseExecutionState.TERMINATING_ON_PARENT_TERMINATION"/> or <seealso cref="CaseExecutionState.TERMINATING_ON_EXIT"/> </returns>
	  bool Terminating {get;}

	  /// <summary>
	  /// <para>Returns <code>true</code> iff:<br>
	  ///  <code><seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.SUSPENDING_ON_SUSPENSION"/>
	  ///        || <seealso cref="getCurrentState()"/> == <seealso cref="CaseExecutionState.SUSPENDING_ON_PARENT_SUSPENSION"/></code>
	  /// </para>
	  /// </summary>
	  /// <returns> whether <code>this</code> case execution has as current state
	  ///        <seealso cref="CaseExecutionState.SUSPENDING_ON_SUSPENSION"/> or <seealso cref="CaseExecutionState.SUSPENDING_ON_PARENT_SUSPENSION"/> </returns>
	  bool Suspending {get;}

	  /// <summary>
	  /// <para>Returns the <seealso cref="CmmnActivity activity"/> which is associated with
	  /// <code>this</code> case execution.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the associated <seealso cref="CmmnActivity activity"/> </returns>
	  CmmnActivity Activity {get;}

	  /// <summary>
	  /// <para>Creates new child case executions for each given <seealso cref="CmmnActivity"/>.</para>
	  /// 
	  /// <para>Afterwards the method <seealso cref="triggerChildExecutionsLifecycle(System.Collections.IList)"/> must be called
	  /// to execute each created case executions (ie. the create listener will be
	  /// notified etc.).</para>
	  /// 
	  /// <para>According to the CMMN 1.0 specification:<br>
	  /// This method can be called when <code>this</code> case execution (which
	  /// represents a <seealso cref="Stage"/>) transitions to <code>ACTIVE</code> state.
	  /// The passed collection of <seealso cref="CmmnActivity activities"/> are the planned
	  /// items that should be executed in this <seealso cref="Stage"/>. So that for each
	  /// given <seealso cref="CmmnActivity"/> a new case execution will be instantiated.
	  /// Furthermore for each created child execution there happens a transition
	  /// to the initial state <code>AVAILABLE</code>.
	  /// </para>
	  /// </summary>
	  /// <param name="activities"> a collection of <seealso cref="CmmnActivity activities"/> of planned items
	  ///                   to execute inside <code>this</code> case execution </param>
	  IList<CmmnExecution> createChildExecutions(IList<CmmnActivity> activities);

	  /// <summary>
	  /// <para>This method triggers for each given case execution the lifecycle.</para>
	  /// 
	  /// <para>This method must be called after <seealso cref="createChildExecutions(System.Collections.IList)"/>.</para>
	  /// </summary>
	  /// <param name="children"> a collection of <seealso cref="CmmnExecution case execution"/> to
	  ///                 trigger for each given case execution the lifecycle </param>
	  void triggerChildExecutionsLifecycle(IList<CmmnExecution> children);

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.ENABLED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.AVAILABLE"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to enable a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.AVAILABLE"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void enable();

	  /// <summary>
	  /// <para>Transition to  state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.ENABLED"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to disable a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>If <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// disabled. This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.ENABLED"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void disable();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.ENABLED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.DISABLED"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to re-enable a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.DISABLED"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void reenable();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.ACTIVE"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.ENABLED"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to start a case execution manually which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.ENABLED"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void manualStart();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.ACTIVE"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.AVAILABLE"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to start a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.AVAILABLE"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void start();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.COMPLETED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.ACTIVE"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to complete a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>If <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// completed. This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// 
	  /// <para>In case of a <seealso cref="Stage"/> the completion can only be performed when the following
	  /// criteria are fulfilled:<br>
	  /// <ul>
	  ///  <li>there are no children in the state <seealso cref="CaseExecutionState.ACTIVE"/> or <seealso cref="CaseExecutionState.NEW"/></li>
	  ///  <li>if the property <code>autoComplete</code> of the associated <seealso cref="Stage"/> is set to <strong><code>true</code></strong>:
	  ///    <ul>
	  ///      <li>all required (<code>requiredRule</code> evaluates to <code>true</code>) children are in state
	  ///        <ul>
	  ///          <li><seealso cref="CaseExecutionState.DISABLED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.COMPLETED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.TERMINATED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  ///        </ul>
	  ///      </li>
	  ///    </ul>
	  ///  </li>
	  ///  <li>if the property <code>autoComplete</code> of the associated <seealso cref="Stage"/> is set to <strong><code>false</code></strong>:
	  ///    <ul>
	  ///      <li>all children are in
	  ///        <ul>
	  ///          <li><seealso cref="CaseExecutionState.DISABLED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.COMPLETED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.TERMINATED"/></li>
	  ///          <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  ///        </ul>
	  ///      </li>
	  ///    </ul>
	  ///  </li>
	  /// </ul>
	  /// </para>
	  /// 
	  /// <para>For a <seealso cref="Task"/> instance, this means its purpose has been accomplished:<br>
	  ///  <ul>
	  ///    <li><seealso cref="HumanTask"/> have been completed by human.</li>
	  ///    <li><seealso cref="CaseTask"/> have launched a new <seealso cref="CaseInstance"/> and if output parameters
	  ///        are required and/or the property <code>isBlocking</code> is set to <code>true</code>,
	  ///        then the launched <seealso cref="CaseInstance"/> has completed and returned the
	  ///        output parameters.</li>
	  ///    <li><seealso cref="ProcessTask"/> have launched a new <seealso cref="ProcessInstance"/> and if output parameters
	  ///        are required and/or the property <code>isBlocking</code> is set to <code>true</code>,
	  ///        then the launched <seealso cref="ProcessInstance"/> has completed and returned the
	  ///        output parameters.</li>
	  ///  </ul>
	  /// </para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.ACTIVE"/> or when the case execution cannot be
	  ///         completed. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void complete();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.COMPLETED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.ACTIVE"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to complete a case execution manually which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>If <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// completed. This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// 
	  /// <para>In case of a <seealso cref="Stage"/> the completion can only be performed when the following
	  /// criteria are fulfilled:<br>
	  /// <ul>
	  ///  <li>there are no children in the state <seealso cref="CaseExecutionState.ACTIVE"/> or <seealso cref="CaseExecutionState.NEW"/></li>
	  ///  <li>all required (<code>requiredRule</code> evaluates to <code>true</code>) children are in state
	  ///    <ul>
	  ///      <li><seealso cref="CaseExecutionState.DISABLED"/></li>
	  ///      <li><seealso cref="CaseExecutionState.COMPLETED"/></li>
	  ///      <li><seealso cref="CaseExecutionState.TERMINATED"/></li>
	  ///      <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  ///    </ul>
	  ///  </li>
	  /// </ul>
	  /// </para>
	  /// 
	  /// <para>For a <seealso cref="Task"/> instance, this means its purpose has been accomplished:<br>
	  ///  <ul>
	  ///    <li><seealso cref="HumanTask"/> have been completed by human.</li>
	  ///  </ul>
	  /// </para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.ACTIVE"/> or when the case execution cannot be
	  ///         completed. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void manualComplete();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.COMPLETED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.AVAILABLE"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>For <seealso cref="EventListener event listener"/> transitions when the event being listened by the
	  /// <seealso cref="EventListener event listener"/> instance does occur. For a {@link UserEventListener user event
	  /// listener} instance this transition happens when a human decides to raise the event.</para>
	  /// 
	  /// </p>For <seealso cref="Milestone"/> instance transitions when one of the achieving <seealso cref="Sentry sentries"/>
	  /// (entry criteria) is satisfied.</p>
	  /// 
	  /// <para>If <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// completed (ie.the event or milestone occured). This can lead to a completion of
	  /// the parent case execution, for more details when the parent case execution can
	  /// be completed see <seealso cref="complete()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not <seealso cref="CaseExecutionState.AVAILABLE"/>. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void occur();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATING_ON_TERMINATION"/> state.</para>
	  /// 
	  /// <para>If <code>this</code> case execution is associated with a <seealso cref="Stage"/> or
	  /// <seealso cref="Task"/>, then <code>this</code> case execution must be in <seealso cref="CaseExecutionState.ACTIVE"/>
	  /// state to be able to do this transition.<br>
	  /// And if <code>this</code> case execution is association with <seealso cref="EventListener EventListener"/>
	  /// or a <seealso cref="Milestone"/>, then <code>this</code> case execution must be in
	  /// <seealso cref="CaseExecutionState.AVAILABLE"/> state to be able to do this transition.</para>
	  /// 
	  /// <para>For a <seealso cref="Stage"/> instance the termination of <code>this</code> case execution
	  /// will be propagated down to all its contained <seealso cref="EventListener EventListener"/>, <seealso cref="Milestone"/>,
	  /// <seealso cref="Stage"/>, and <seealso cref="Task"/> instances.</para>
	  /// 
	  /// <para>In case of a <seealso cref="Stage"/> this corresponding case execution stays in this state until
	  /// all children notified this case execution, that they terminated successfully. Afterwards the
	  /// method <seealso cref="performTerminate()"/> must be called to complete the transition into the state
	  /// <seealso cref="CaseExecutionState.TERMINATED"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void terminate();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATED"/> state.</para>
	  /// 
	  /// <para>If <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// terminated. This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void performTerminate();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATING_ON_PARENT_TERMINATION"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.AVAILABLE"/>
	  /// or <seealso cref="CaseExecutionState.SUSPENDED"/> state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to execute a parent termination on a case execution which is
	  /// associated with a <seealso cref="EventListener"/> or <seealso cref="Milestone"/>.</para>
	  /// 
	  /// <para>Afterwards the method <seealso cref="performParentTerminate()"/> must be called to complete
	  /// the transition into the state <seealso cref="CaseExecutionState.TERMINATED"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void parentTerminate();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATED"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.AVAILABLE"/>
	  /// or <seealso cref="CaseExecutionState.SUSPENDED"/> state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to execute a parent termination on a case execution which is
	  /// associated with a <seealso cref="EventListener"/> or <seealso cref="Milestone"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void performParentTerminate();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATING_ON_EXIT"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in one of the following state to
	  /// be able to do this transition:
	  /// <ul>
	  ///   <li><seealso cref="CaseExecutionState.AVAILABLE"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.ENABLED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.DISABLED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.ACTIVE"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.SUSPENDED"/> or</li>
	  ///   <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  /// </ul>
	  /// 
	  /// </para>
	  /// <para>It is only possible to execute an exit on a case execution which is
	  /// associated with a <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>Afterwards the method <seealso cref="performExit()"/> must be called to complete
	  /// the transition into the state <seealso cref="CaseExecutionState.TERMINATED"/>.</para>
	  /// 
	  /// <para>If this transition is triggered by a fulfilled exit criteria and if
	  /// <code>this</code> case execution has a parent case execution, that parent
	  /// case execution will be notified that <code>this</code> case execution has been
	  /// terminated. This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void exit();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATED"/> state.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void parentComplete();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.TERMINATED"/> state.</para>
	  /// 
	  /// <para>This can lead to a completion of the parent case execution, for more
	  /// details when the parent case execution can be completed see <seealso cref="complete()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void performExit();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.SUSPENDING_ON_SUSPENSION"/> state.</para>
	  /// 
	  /// <para>If <code>this</code> case execution is associated with a <seealso cref="Stage"/> or
	  /// <seealso cref="Task"/>, then <code>this</code> case execution must be in <seealso cref="CaseExecutionState.ACTIVE"/>
	  /// state to be able to do this transition.<br>
	  /// And if <code>this</code> case execution is association with <seealso cref="EventListener EventListener"/>
	  /// or a <seealso cref="Milestone"/>, then <code>this</code> case execution must be in
	  /// <seealso cref="CaseExecutionState.AVAILABLE"/> state to be able to do this transition.</para>
	  /// 
	  /// <para>For a <seealso cref="Stage"/> instance the suspension of <code>this</code> case execution
	  /// will be propagated down to all its contained <seealso cref="EventListener EventListener"/>, <seealso cref="Milestone"/>,
	  /// <seealso cref="Stage"/>, and <seealso cref="Task"/> instances.</para>
	  /// 
	  /// <para>Afterwards the method <seealso cref="performSuspension()"/> must be called to complete
	  /// the transition into the state <seealso cref="CaseExecutionState.SUSPENDED"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void suspend();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.SUSPENDED"/> state.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void performSuspension();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.SUSPENDING_ON_PARENT_SUSPENSION"/> state.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in one of the following state to
	  /// be able to do this transition:
	  /// <ul>
	  ///   <li><seealso cref="CaseExecutionState.AVAILABLE"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.ENABLED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.DISABLED"/> or</li>
	  ///   <li><seealso cref="CaseExecutionState.ACTIVE"/></li>
	  /// </ul>
	  /// 
	  /// </para>
	  /// <para>It is only possible to execute a parent suspension on a case execution which is
	  /// associated with a <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>Afterwards the method <seealso cref="performParentSuspension()"/> must be called to complete
	  /// the transition into the state <seealso cref="CaseExecutionState.SUSPENDED"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void parentSuspend();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.SUSPENDED"/> state.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void performParentSuspension();

	  /// <summary>
	  /// <para>Transition to either to <seealso cref="CaseExecutionState.ACTIVE"/> state, if <code>this</code>
	  /// case execution is associated with a <seealso cref="Stage"/> or <seealso cref="Task"/>, or to <seealso cref="CaseExecutionState.AVAILABE"/>,
	  /// if <code>this</code> case execution is associated with a <seealso cref="EventListener"/> or <seealso cref="Milestone"/>.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.SUSPENDED"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>For a <seealso cref="Stage"/> instance the resume of <code>this</code> case execution
	  /// will be propagated down to all its contained <seealso cref="EventListener EventListener"/>, <seealso cref="Milestone"/>,
	  /// <seealso cref="Stage"/>, and <seealso cref="Task"/> instances.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void resume();

	  /// <summary>
	  /// <para>Transition to the previous state (<seealso cref="CaseExecutionState.AVAILABLE"/>,
	  /// <seealso cref="CaseExecutionState.ENABLED"/>, <seealso cref="CaseExecutionState.DISABLED"/> or
	  /// <seealso cref="CaseExecutionState.ACTIVE"/>) when the parent <seealso cref="Stage"/> transitions
	  /// out of <seealso cref="CaseExecutionState.SUSPENDED"/>.</para>
	  /// 
	  /// <para><code>This</code> case execution must be in <seealso cref="CaseExecutionState.SUSPENDED"/>
	  /// state to be able to do this transition.</para>
	  /// 
	  /// <para>It is only possible to execute a parent resume on a case execution which is
	  /// associated with a <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void parentResume();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.ACTIVE"/> state.</para>
	  /// 
	  /// <para>If <code>this</code> case execution is associated with a <seealso cref="Stage"/> or
	  /// <seealso cref="Task"/> and is not a case instance, then <code>this</code> case execution
	  /// must be in <seealso cref="CaseExecutionState.FAILED"/> state to be able to do this transition.<br>
	  /// And if <code>this</code> case execution is a case instance, then <code>this</code>
	  /// case instance must be in one of the following state to perform this transition:
	  /// <ul>
	  ///   <li><seealso cref="CaseExecutionState.COMPLETED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.SUSPENDED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.TERMINATED"/> or</li>
	  ///   <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  /// </ul>
	  /// </para>
	  /// 
	  /// <para>In case of a case instance the transition out of <seealso cref="CaseExecutionState.SUSPENDED"/> state
	  /// the resume will be propagated down to all its contained <seealso cref="EventListener EventListener"/>,
	  /// <seealso cref="Milestone"/>, <seealso cref="Stage"/>, and <seealso cref="Task"/> instances, see <seealso cref="resume()"/> and
	  /// <seealso cref="parentResume()"/>.</para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void reactivate();

	  /// <summary>
	  /// <para>Transition to <seealso cref="CaseExecutionState.CLOSED"/> state when no further
	  /// work or modifications should be allowed for this case instance.</para>
	  /// 
	  /// <para>It is only possible to close a case instance which is in one of the following
	  /// states:
	  /// <ul>
	  ///   <li><seealso cref="CaseExecutionState.COMPLETED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.SUSPENDED"/>,</li>
	  ///   <li><seealso cref="CaseExecutionState.TERMINATED"/> or</li>
	  ///   <li><seealso cref="CaseExecutionState.FAILED"/></li>
	  /// </ul>
	  /// </para>
	  /// </summary>
	  /// <exception cref="CaseIllegalStateTransitionException"> will be thrown, if <code>this</code> case execution
	  ///         is not in the expected state. </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void close();

	  /// <summary>
	  /// <para>Returns true, if <code>this</code> case execution is required.</para>
	  /// </summary>
	  /// <returns> true if <code>this</code> case execution is required. </returns>
	  bool Required {get;set;}


	  /// <summary>
	  /// <para>Removes <code>this</code> case execution from the parent case execution.</para>
	  /// </summary>
	  void remove();

	  /// <summary>
	  /// <para>Returns a <seealso cref="System.Collections.IList"/> of child case executions. If <code>this</code> case
	  /// execution has no child case executions an empty <seealso cref="System.Collections.IList"/> will be returned.</para>
	  /// </summary>
	  /// <returns> a <seealso cref="System.Collections.IList"/> of child case executions. </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends CmmnExecution> getCaseExecutions();
	  IList<CmmnExecution> CaseExecutions {get;}

	  /// <summary>
	  /// <para>Creates a new task.</para>
	  /// 
	  /// <para><code>This</code></para> case execution will be the case execution of the
	  /// created task.</p>
	  /// </summary>
	  /// <param name="taskDecorator"> the task decorator used to create task </param>
	   org.camunda.bpm.engine.task.Task createTask(TaskDecorator taskDecorator);

	  /// <summary>
	  /// <para>Creates a new sub process instance.</para>
	  /// 
	  /// <para><code>This</code> case execution will be the super case execution of the
	  /// created sub process instance.</para>
	  /// </summary>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition);

	  /// <summary>
	  /// <para>Creates a new sub process instance.</para>
	  /// 
	  /// <para><code>This</code> case execution will be the super case execution of the
	  /// created sub process instance.</para>
	  /// </summary>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  /// <param name="businessKey"> The businessKey to be set on sub process instance. </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey);

	  /// <summary>
	  /// <para>Creates a new sub process instance.</para>
	  /// 
	  /// <para><code>This</code> case execution will be the super case execution of the
	  /// created sub process instance.</para>
	  /// </summary>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  /// <param name="businessKey"> The businessKey to be set on sub process instance. </param>
	  /// <param name="caseInstanceId"> The caseInstanceId to be set on sub process instance. </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId);

	  /// <summary>
	  /// <para>Creates a new sub case instance.</para>
	  /// 
	  /// <para><code>This</code> case execution will be the super case execution of the
	  /// created sub case instance.</para>
	  /// </summary>
	  /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition"/> of the sub case instance. </param>
	  CmmnCaseInstance createSubCaseInstance(CmmnCaseDefinition caseDefinition);

	  /// <summary>
	  /// <para>Creates a new sub case instance.</para>
	  /// 
	  /// <para><code>This</code> case execution will be the super case execution of the
	  /// created sub case instance.</para>
	  /// </summary>
	  /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition"/> of the sub case instance. </param>
	  /// <param name="businessKey"> The businessKey to be set on sub case instance. </param>
	  CmmnCaseInstance createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey);

	  /// <summary>
	  /// <para>Creates for each defined <seealso cref="PlanItemOnPart"/> and <seealso cref="IfPart"/> inside
	  /// the specified <seealso cref="Sentry Sentries"/> a <seealso cref="CmmnSentryPart"/>.</para>
	  /// </summary>
	  void createSentryParts();

	  /// <summary>
	  /// <para>Returns <code>true</code>, if each <seealso cref="CmmnSentryPart"/> of the given
	  /// <code>sentryId</code> is satisfied.</para>
	  /// </summary>
	  /// <param name="sentryId"> the id of the sentry to check
	  /// </param>
	  /// <returns> <code>true</code> if the sentry is satisfied. </returns>
	  bool isSentrySatisfied(string sentryId);

	  /// <summary>
	  /// <para>The flag <code>entryCriterionSatisfied</code> will only be set to
	  /// <code>true</code>, when <code>this</code> <seealso cref="CmmnActivityExecution"/>
	  /// stays in state <seealso cref="CaseExecutionState.NEW"/>.</para>
	  /// 
	  /// <para>For example:</para>
	  /// 
	  /// <para>There exists:</para>
	  /// <ul>
	  ///   <li>a <seealso cref="Stage"/>,</li>
	  ///   <li>the <seealso cref="Stage"/> contains two tasks (A and B) and</li>
	  ///   <li>task B has an entry criterion which is satisfied,
	  ///       when task A performs the transition <code>create</code></li>
	  /// </ul>
	  /// 
	  /// <para>When the <seealso cref="Stage"/> instance becomes active, two child case executions
	  /// will be created for task A and task B. Both tasks are in the state <seealso cref="CaseExecutionState.NEW"/>.
	  /// Now task A performs the <code>create</code> transition and so that the given sentry is triggered,
	  /// that this is satisfied. Afterwards the sentry will be reseted, that the sentry is not satisfied anymore.</para>
	  /// <para>But task B is still in the state <seealso cref="CaseExecutionState.NEW"/> and will not be
	  /// notified, that its' entry criterion has been satisfied. That's why the the flag <code>entryCriterionSatisfied</code>
	  /// will be set to <code>true</code> on the case execution of task B in such a situation. When
	  /// task B performs the transition into the state <seealso cref="CaseExecutionState.AVAILABLE"/> it can perform
	  /// the next transition because the entry criterion has been already satisfied.</para>
	  /// </summary>
	  bool EntryCriterionSatisfied {get;}

	  /// <summary>
	  /// Fire sentries that consist only out of ifPart, are not satisfied yet, but do satisfy condition.
	  /// </summary>
	  void fireIfOnlySentryParts();
	}

}