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
namespace org.camunda.bpm.engine.impl.pvm.@delegate
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public interface ActivityExecution : DelegateExecution
	{

	  /* Process instance/activity/transition retrieval */

	  /// <summary>
	  /// returns the current <seealso cref="PvmActivity"/> of the execution.
	  /// </summary>
	  PvmActivity Activity {get;set;}

	  /// <summary>
	  /// invoked to notify the execution that a new activity instance is started </summary>
	  void enterActivityInstance();

	  /// <summary>
	  /// invoked to notify the execution that an activity instance is ended. </summary>
	  void leaveActivityInstance();

	  string ActivityInstanceId {set;get;}


	  /// <summary>
	  /// return the Id of the parent activity instance currently executed by this execution </summary>
	  string ParentActivityInstanceId {get;}

	  /* Execution management */

	  /// <summary>
	  /// creates a new execution. This execution will be the parent of the newly created execution.
	  /// properties processDefinition, processInstance and activity will be initialized.
	  /// </summary>
	  ActivityExecution createExecution();

	  /// <summary>
	  /// creates a new execution. This execution will be the parent of the newly created execution.
	  /// properties processDefinition, processInstance and activity will be initialized.
	  /// </summary>
	  ActivityExecution createExecution(bool initializeExecutionStartContext);

	  /// <summary>
	  /// creates a new sub process instance.
	  /// The current execution will be the super execution of the created execution.
	  /// </summary>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition);

	  /// <seealso cref= #createSubProcessInstance(PvmProcessDefinition)
	  /// </seealso>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  /// <param name="businessKey"> the business key of the process instance </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey);

	  /// <seealso cref= #createSubProcessInstance(PvmProcessDefinition)
	  /// </seealso>
	  /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
	  /// <param name="businessKey"> the business key of the process instance </param>
	  /// <param name="caseInstanceId"> the case instance id of the process instance </param>
	  PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId);

	  /// <summary>
	  /// <para>Creates a new sub case instance.</para>
	  /// 
	  /// <para><code>This</code> execution will be the super execution of the
	  /// created sub case instance.</para>
	  /// </summary>
	  /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition"/> of the sub case instance. </param>
	  CmmnCaseInstance createSubCaseInstance(CmmnCaseDefinition caseDefinition);

	  /// <summary>
	  /// <para>Creates a new sub case instance.</para>
	  /// 
	  /// <para><code>This</code> execution will be the super execution of the
	  /// created sub case instance.</para>
	  /// </summary>
	  /// <param name="caseDefinition"> The <seealso cref="CmmnCaseDefinition"/> of the sub case instance. </param>
	  /// <param name="businessKey"> The businessKey to be set on sub case instance. </param>
	  CmmnCaseInstance createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey);

	  /// <summary>
	  /// returns the parent of this execution, or null if there no parent.
	  /// </summary>
	  ActivityExecution Parent {get;}

	  /// <summary>
	  /// returns the list of execution of which this execution the parent of.
	  /// This is a copy of the actual list, so a modification has no direct effect.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends ActivityExecution> getExecutions();
	  IList<ActivityExecution> Executions {get;}

	  /// <summary>
	  /// returns child executions that are not event scope executions.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends ActivityExecution> getNonEventScopeExecutions();
	  IList<ActivityExecution> NonEventScopeExecutions {get;}

	  /// <returns> true if this execution has child executions (event scope executions or not) </returns>
	  bool hasChildren();

	  /// <summary>
	  /// ends this execution.
	  /// </summary>
	  void end(bool isScopeComplete);

	  /// <summary>
	  /// Execution finished compensation. Removes this
	  /// execution and notifies listeners.
	  /// </summary>
	  void endCompensation();

	  /* State management */

	  /// <summary>
	  /// makes this execution active or inactive.
	  /// </summary>
	  bool Active {set;get;}


	  /// <summary>
	  /// returns whether this execution has ended or not.
	  /// </summary>
	  bool Ended {get;set;}

	  /// <summary>
	  /// changes the concurrent indicator on this execution.
	  /// </summary>
	  bool Concurrent {set;get;}


	  /// <summary>
	  /// returns whether this execution is a process instance or not.
	  /// </summary>
	  bool ProcessInstanceExecution {get;}

	  /// <summary>
	  /// Inactivates this execution.
	  /// This is useful for example in a join: the execution
	  /// still exists, but it is not longer active.
	  /// </summary>
	  void inactivate();

	  /// <summary>
	  /// Returns whether this execution is a scope.
	  /// </summary>
	  bool Scope {get;set;}


	  /// <summary>
	  /// Returns whether this execution completed the parent scope.
	  /// </summary>
	  bool CompleteScope {get;}

	  /// <summary>
	  /// Retrieves all executions which are concurrent and inactive at the given activity.
	  /// </summary>
	  IList<ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity);

	  IList<ActivityExecution> findInactiveChildExecutions(PvmActivity activity);

	  /// <summary>
	  /// Takes the given outgoing transitions, and potentially reusing
	  /// the given list of executions that were previously joined.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: void leaveActivityViaTransitions(java.util.List<org.camunda.bpm.engine.impl.pvm.PvmTransition> outgoingTransitions, java.util.List<? extends ActivityExecution> joinedExecutions);
	  void leaveActivityViaTransitions<T1>(IList<PvmTransition> outgoingTransitions, IList<T1> joinedExecutions);

	  void leaveActivityViaTransition(PvmTransition outgoingTransition);

	  /// <summary>
	  /// Executes the <seealso cref="ActivityBehavior"/> associated with the given activity.
	  /// </summary>
	  void executeActivity(PvmActivity activity);

	  /// <summary>
	  /// Called when an execution is interrupted. This will remove all associated entities
	  /// such as event subscriptions, jobs, ...
	  /// </summary>
	  void interrupt(string reason);

	  /// <summary>
	  /// An activity which is to be started next. </summary>
	  PvmActivity NextActivity {get;}


	  void remove();
	  void destroy();

	  void signal(string @string, object signalData);


	  bool tryPruneLastConcurrentChild();

	  void forceUpdate();

	  TransitionImpl Transition {get;}

	  /// <summary>
	  /// Assumption: the current execution is active and executing an activity (<seealso cref="#getActivity()"/> is not null).
	  /// 
	  /// For a given target scope, this method returns the scope execution.
	  /// </summary>
	  /// <param name="targetScope"> scope activity or process definition for which the scope execution should be found;
	  ///   must be an ancestor of the execution's current activity
	  /// @return </param>
	  ActivityExecution findExecutionForFlowScope(PvmScope targetScope);

	  /// <summary>
	  /// Returns a mapping from scope activities to scope executions for all scopes that
	  /// are ancestors of the activity currently executed by this execution.
	  /// 
	  /// Assumption: the current execution is active and executing an activity (<seealso cref="#getActivity()"/> is not null).
	  /// </summary>
	  IDictionary<ScopeImpl, PvmExecutionImpl> createActivityExecutionMapping();


	}

}