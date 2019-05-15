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
namespace org.camunda.bpm.engine.impl.migration
{

	using MigrationActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.MigrationActivityValidator;
	using MigrationInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationInstructionValidator;
	using ValidatingMigrationInstructions = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructions;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;

	/// <summary>
	/// Generates all migration instructions which represent a direct one
	/// to one mapping of mapped entities in two process definitions. See
	/// also <seealso cref="MigrationActivityMatcher"/>.
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface MigrationInstructionGenerator
	{

	  /// <summary>
	  /// Sets the list of migration activity validators which validate that a activity
	  /// is a candidate for the migration.
	  /// </summary>
	  /// <param name="migrationActivityValidators"> the list of validators to check </param>
	  /// <returns> this generator instance </returns>
	  MigrationInstructionGenerator migrationActivityValidators(IList<MigrationActivityValidator> migrationActivityValidators);

	  /// <summary>
	  /// Sets the list of migration instruction validators currently used by the process engine.
	  /// Implementations may use these to restrict the search space.
	  /// </summary>
	  /// <returns> this </returns>
	  MigrationInstructionGenerator migrationInstructionValidators(IList<MigrationInstructionValidator> migrationInstructionValidators);

	  /// <summary>
	  /// Generate all migration instructions for mapped activities between two process definitions. A activity can be mapped
	  /// if the <seealso cref="MigrationActivityMatcher"/> matches it with an activity from the target process definition.
	  /// </summary>
	  /// <param name="sourceProcessDefinition"> the source process definition </param>
	  /// <param name="targetProcessDefinition"> the target process definiton </param>
	  /// <returns> the list of generated instructions </returns>
	  ValidatingMigrationInstructions generate(ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition, bool updateEventTriggers);

	}

}