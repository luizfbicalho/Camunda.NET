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
namespace org.camunda.bpm.engine.impl.pvm
{

	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public interface PvmScope : PvmProcessElement
	{

	  /// <summary>
	  /// Indicates whether this is a local scope for variables and events
	  /// if true, there will _always_ be a scope execution created for it.
	  /// <para>
	  /// Note: the fact that this is a scope does not mean that it is also a
	  /// <seealso cref="isSubProcessScope() sub process scope."/>
	  /// 
	  /// @returns true if this activity is a scope
	  /// </para>
	  /// </summary>
	  bool Scope {get;}

	  /// <summary>
	  /// Indicates whether this scope is a sub process scope.
	  /// A sub process scope is a scope which contains "normal flow".Scopes which are flow scopes but not sub process scopes:
	  /// <ul>
	  /// <li>a multi instance body scope</li>
	  /// <li>leaf scope activities which are pure event scopes (Example: User task with attached boundary event)</li>
	  /// </ul>
	  /// </summary>
	  /// <returns> true if this is a sub process scope </returns>
	  bool SubProcessScope {get;}

	  /// <summary>
	  /// The event scope for an activity is the scope in which the activity listens for events.
	  /// This may or may not be the <seealso cref="getFlowScope() flow scope."/>.
	  /// Consider: boundary events have a different event scope than flow scope.
	  /// <para>
	  /// The event scope is always a <seealso cref="isScope() scope"/>.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the event scope of the activity </returns>
	  PvmScope EventScope {get;}

	  /// <summary>
	  /// The flow scope of the activity. The scope in which the activity itself is executed.
	  /// <para>
	  /// Note: in order to ensure backwards compatible behavior,  a flow scope is not necessarily
	  /// a <seealso cref="isScope() a scope"/>. Example: event sub processes.
	  /// </para>
	  /// </summary>
	  ScopeImpl FlowScope {get;}

	  /// <summary>
	  /// The "level of subprocess scope" as defined in bpmn: this is the subprocess
	  /// containing the activity. Usually this is the same as the flow scope, instead if
	  /// the activity is multi instance: in that case the activity is nested inside a
	  /// mutli instance body but "at the same level of subprocess" as other activities which
	  /// are siblings to the mi-body.
	  /// </summary>
	  /// <returns> the level of subprocess scope as defined in bpmn </returns>
	  PvmScope LevelOfSubprocessScope {get;}

	  /// <summary>
	  /// Returns the flow activities of this scope. This is the list of activities for which this scope is
	  /// the <seealso cref="PvmActivity.getFlowScope() flow scope"/>.
	  /// </summary>
	  /// <returns> the list of flow activities for this scope. </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends PvmActivity> getActivities();
	  IList<PvmActivity> Activities {get;}

	  /// <summary>
	  /// Recursively finds a flow activity. This is an activitiy which is in the hierarchy of flow activities.
	  /// </summary>
	  /// <param name="activityId"> the id of the activity to find. </param>
	  /// <returns> the activity or null </returns>
	  PvmActivity findActivity(string activityId);

	  /// <summary>
	  /// Finds an activity at the same level of subprocess.
	  /// </summary>
	  /// <param name="activityId"> the id of the activity to find. </param>
	  /// <returns> the activity or null </returns>
	  PvmActivity findActivityAtLevelOfSubprocess(string activityId);

	  /// <summary>
	  /// Recursively finds a transition. </summary>
	  /// <param name="transitionId"> the transiton to find </param>
	  /// <returns> the transition or null </returns>
	  TransitionImpl findTransition(string transitionId);

	}

}