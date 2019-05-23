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

	/// <summary>
	/// <para>An activity instance represents an instance of an activity.</para>
	/// 
	/// <para>For documentation, see <seealso cref="RuntimeService.getActivityInstance(string)"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ActivityInstance : ProcessElementInstance
	{

	  /// <summary>
	  /// the id of the activity </summary>
	  string ActivityId {get;}

	  /// <summary>
	  /// the name of the activity </summary>
	  string ActivityName {get;}

	  /// <summary>
	  /// Type of the activity, corresponds to BPMN element name in XML (e.g. 'userTask').
	  /// The type of the Root activity instance (the one corresponding to the process instance will be 'processDefinition'.
	  /// </summary>
	  string ActivityType {get;}

	  /// <summary>
	  /// Returns the child activity instances.
	  /// Returns an empty list if there are no child instances 
	  /// </summary>
	  ActivityInstance[] ChildActivityInstances {get;}

	  /// <summary>
	  /// Returns the child transition instances.
	  /// Returns an empty list if there are no child transition instances 
	  /// </summary>
	  TransitionInstance[] ChildTransitionInstances {get;}

	  /// <summary>
	  /// the list of executions that are currently waiting in this activity instance </summary>
	  string[] ExecutionIds {get;}

	  /// <summary>
	  /// all descendant (children, grandchildren, etc.) activity instances that are instances of the supplied activity
	  /// </summary>
	  ActivityInstance[] getActivityInstances(string activityId);

	  /// <summary>
	  /// all descendant (children, grandchildren, etc.) transition instances that are leaving or entering the supplied activity
	  /// </summary>
	  TransitionInstance[] getTransitionInstances(string activityId);

	}

}