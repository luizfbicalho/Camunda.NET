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
namespace org.camunda.bpm.engine.migration
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface MigrationPlanBuilder
	{

	  /// <summary>
	  /// Automatically adds a set of instructions for activities that are <em>equivalent</em> in both
	  /// process definitions. By default, this is given if two activities are both user tasks, are on the same
	  /// level of sub process, and have the same id.
	  /// </summary>
	  MigrationInstructionsBuilder mapEqualActivities();

	  /// <summary>
	  /// Adds a migration instruction that maps activity instances of the source activity (of the source process definition)
	  /// to activity instances of the target activity (of the target process definition)
	  /// </summary>
	  MigrationInstructionBuilder mapActivities(string sourceActivityId, string targetActivityId);

	  /// <returns> a migration plan with all previously specified instructions
	  /// </returns>
	  /// <exception cref="MigrationPlanValidationException"> if the migration plan contains instructions that are not valid </exception>
	  /// <exception cref="AuthorizationException">
	  ///         if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
	  ///         for both, source and target process definition. </exception>
	  MigrationPlan build();


	}

}