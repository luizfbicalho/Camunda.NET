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
namespace org.camunda.bpm.engine.impl.migration.instance
{
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface MigratingInstance
	{

	  bool Detached {get;}

	  /// <summary>
	  /// Detach this instance's state from its owning instance and the execution tree
	  /// </summary>
	  void detachState();

	  /// <summary>
	  /// Restore this instance's state as a subordinate to the given activity instance
	  /// (e.g. in the execution tree).
	  /// Restoration should restore the state that was detached
	  /// before.
	  /// </summary>
	  void attachState(MigratingScopeInstance targetActivityInstance);

	  /// <summary>
	  /// Restore this instance's state as a subordinate to the given transition instance
	  /// (e.g. in the execution tree).
	  /// Restoration should restore the state that was detached
	  /// before.
	  /// </summary>
	  void attachState(MigratingTransitionInstance targetTransitionInstance);

	  /// <summary>
	  /// Migrate state from the source process definition
	  /// to the target process definition.
	  /// </summary>
	  void migrateState();

	  /// <summary>
	  /// Migrate instances that are aggregated by this instance
	  /// (e.g. an activity instance aggregates task instances).
	  /// </summary>
	  void migrateDependentEntities();

	}

}