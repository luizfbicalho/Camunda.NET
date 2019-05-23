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
namespace org.camunda.bpm.engine.runtime
{

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// <para>A fluent builder for defining a command to execute for a case execution.</para>
	/// 
	/// <para>This fluent builder offers different points to execute a defined command:
	///  <ul>
	///    <li><seealso cref="execute()"/></li>
	///    <li><seealso cref="manualStart()"/></li>
	///    <li><seealso cref="disable()"/></li>
	///    <li><seealso cref="reenable()"/></li>
	///    <li><seealso cref="complete()"/></li>
	///    <li><seealso cref="close()"/></li>
	///  </ul>
	/// </para>
	/// 
	/// <para>The entry point to use this fluent builder is <seealso cref="CaseService.withCaseExecution(string)"/>.
	/// It expects an id of a case execution as parameter.</para>
	/// 
	/// <para>This fluent builder can be used as follows:</para>
	/// 
	/// <para>(1) Set and remove case execution variables:</para>
	/// <code>
	/// &nbsp;&nbsp;caseService<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.withCaseExecution("aCaseDefinitionId")<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.setVariable("aVariableName", "aVariableValue")<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.setVariable("anotherVariableName", 999)<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.removeVariable("aVariableNameToRemove")<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.execute();
	/// </code>
	/// 
	/// <para>(2) Set case execution variable and start the case execution manually:</para>
	/// <code>
	/// &nbsp;&nbsp;caseService<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.withCaseExecution("aCaseDefinitionId")<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.setVariable("aVariableName", "aVariableValue")<br>
	/// &nbsp;&nbsp;&nbsp;&nbsp;.manualStart();
	/// </code>
	/// 
	/// <para>etc.</para>
	/// 
	/// <para><strong>Note:</strong> All defined changes for a case execution within this fluent
	/// builder will be performed in one command. So for example: if you set and remove
	/// variables of a case execution this happens in a single command. This has the effect
	/// that if anything went wrong the whole command will be rolled back.</para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseExecutionCommandBuilder
	{

	  /// <summary>
	  /// <para>Pass a variable to the case execution. If the variable is not already
	  /// existing, they will be created in the case instance (which is the root execution)
	  /// otherwise the existing variable will be updated.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to set </param>
	  /// <param name="variableValue"> the value of the variable to set
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given variable name is null or the same variable
	  ///   should be removed in the same command </exception>
	  CaseExecutionCommandBuilder setVariable(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Pass a map of variables to the case execution. If the variables are not already
	  /// existing, they will be created in the case instance (which is the root execution)
	  /// otherwise the existing variable will be updated.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variables"> the map of variables
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when one of the passed variables should be removed
	  ///         in the same command </exception>
	  CaseExecutionCommandBuilder setVariables(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Pass a local variable to the case execution (not considering parent scopes).</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to set </param>
	  /// <param name="variableValue"> the value of the variable to set
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given variable name is null or the same variable
	  ///   should be removed in the same command </exception>
	  CaseExecutionCommandBuilder setVariableLocal(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Pass a map of variables to the case execution (not considering parent scopes).</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variables"> the map of variables
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when one of the passed variables should be removed
	  ///         in the same command </exception>
	  CaseExecutionCommandBuilder setVariablesLocal(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Pass a variable name of a variable to be removed for a case execution.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variable names.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of a variable to remove
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given variable name is null or the same variable
	  ///         should be set in the same command </exception>
	  CaseExecutionCommandBuilder removeVariable(string variableName);

	  /// <summary>
	  /// <para>Pass a collection of variable names of variables to be removed for a
	  /// case execution.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variable names.</para>
	  /// </summary>
	  /// <param name="variableNames"> a collection of names of variables to remove
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when one of the passed variables should be set
	  ///         in the same command </exception>
	  CaseExecutionCommandBuilder removeVariables(ICollection<string> variableNames);

	  /// <summary>
	  /// <para>Pass a variable name of a local variable to be removed for a case execution
	  /// (not considering parent scopes).</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variable names.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of a variable to remove
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given variable name is null or the same
	  ///         variable should be set in same command </exception>
	  CaseExecutionCommandBuilder removeVariableLocal(string variableName);

	  /// <summary>
	  /// <para>Pass a collection of variable names of local variables to be removed for a
	  /// case execution (not considering parent scopes).</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variable names.</para>
	  /// </summary>
	  /// <param name="variableNames"> a collection of names of variables to remove
	  /// </param>
	  /// <returns> the builder
	  /// </returns>
	  /// <exception cref="NotValidException"> when one of the passed variables should be set
	  ///         in the same command </exception>
	  CaseExecutionCommandBuilder removeVariablesLocal(ICollection<string> variableNames);

	  /// <summary>
	  /// <para>Invoking this method will remove and/or set the passed variables.</para>
	  /// 
	  /// <para>This behaves as follows:</para>
	  /// 
	  /// <ol>
	  ///   <li>if at least one variable name of a variable to remove is passed, those
	  ///       variables will be removed.
	  ///   </li>
	  ///   <li>if at least one local variable name of a local variable to remove is
	  ///       passed, those local variables will be removed.
	  ///   </li>
	  ///   <li>if at least one variable to add or update is passed, those variables
	  ///       will be set for a case execution.
	  ///   </li>
	  ///   <li>if at least one local variable to add or update is passed, those
	  ///       variables will be set for a case execution.
	  ///   </li>
	  /// </ol>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void execute();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case execution will
	  /// be started manually. Therefore there happens a transition from the state
	  /// <code>ENABLED</code> to state <code>ACTIVE</code>.</para>
	  /// 
	  /// <para>According to CMMN 1.0 specification the state <code>ACTIVE</code> means,
	  /// that the with the case execution related <seealso cref="Stage"/> or <seealso cref="Task"/> is
	  /// executing in this state:
	  ///   <ul>
	  ///     <li><seealso cref="Task"/>: the <seealso cref="Task task"/> will be completed immediately</li>
	  ///     <li><seealso cref="HumanTask"/>: a new <seealso cref="org.camunda.bpm.engine.task.Task user task"/> will be instantiated</li>
	  ///     <li><seealso cref="ProcessTask"/>: a new <seealso cref="ProcessInstance process instance"/> will be instantiated</li>
	  ///     <li><seealso cref="CaseTask"/>: a new <seealso cref="CaseInstance case instance"/> will be instantiated</li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done or
	  ///      when the case execution is a case instance </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void manualStart();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case execution will
	  /// be disabled. Therefore there happens a transition from the state <code>ENABLED</code>
	  /// to state <code>DISABLED</code>.</para>
	  /// 
	  /// <para>According to CMMN 1.0 specification the state <code>DISABLED</code> means,
	  /// that the with the case execution related <seealso cref="Stage"/> or <seealso cref="Task"/> should
	  /// not be executed in the case instance.</para>
	  /// 
	  /// <para>If the given case execution has a parent case execution, that parent
	  /// case execution will be notified that the given case execution has been
	  /// disabled. This can lead to a completion of the parent case execution if
	  /// the completion criteria are fulfilled.</para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done or
	  ///      when the case execution is a case instance </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void disable();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case execution will
	  /// be re-enabled. Therefore there happens a transition from the state <code>DISABLED</code>
	  /// to state <code>ENABLED</code>.</para>
	  /// 
	  /// <para>According to CMMN 1.0 specification the state <code>DISABLED</code> means,
	  /// that the with the case execution related <seealso cref="Stage"/> or <seealso cref="Task"/> is waiting
	  /// for a decision to become <code>ACTIVE</code> or <code>DISABLED</code> once again.</para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done or
	  ///      when the case execution is a case instance </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void reenable();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case execution will
	  /// be completed. Therefore there happens a transition from the state <code>ACTIVE</code>
	  /// to state <code>COMPLETED</code>.</para>
	  /// 
	  /// <para>It is only possible to complete a case execution which is associated with a
	  /// <seealso cref="Stage"/> or <seealso cref="Task"/>.</para>
	  /// 
	  /// <para>In case of a <seealso cref="Stage"/> the completion can only be performed when the following
	  /// criteria are fulfilled:<br>
	  /// <ul>
	  ///  <li>there are no children in the state <code>ACTIVE</code></li>
	  /// </ul>
	  /// </para>
	  /// 
	  /// <para>For a <seealso cref="Task"/> instance, this means its purpose has been accomplished:<br>
	  ///  <ul>
	  ///    <li><seealso cref="HumanTask"/> has been completed by human.</li>
	  ///  </ul>
	  /// </para>
	  /// 
	  /// <para>If the given case execution has a parent case execution, that parent
	  /// case execution will be notified that the given case execution has been
	  /// completed. This can lead to a completion of the parent case execution if
	  /// the completion criteria are fulfilled.</para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void complete();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case execution will
	  /// be terminated. Therefore there happens a transition to state <code>TERMINATED</code>.</para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done or
	  ///      when the case execution is a case instance </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void terminate();

	  /// <summary>
	  /// <para>Additionally to <seealso cref="execute()"/> the associated case instance will
	  /// be closed, so that no further work or modifications is allowed for the
	  /// associated case instance. Therefore there happens a transition from the
	  /// state <code>COMPLETED</code> to state <code>CLOSED</code>.</para>
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case execution id is null </exception>
	  /// <exception cref="NotFoundException"> when no case execution is found for the
	  ///      given case execution id </exception>
	  /// <exception cref="NotAllowedException"> when the transition is not allowed to be done </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  void close();

	}

}