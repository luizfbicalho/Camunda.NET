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
namespace org.camunda.bpm.engine.impl.migration.validation.instruction
{
	/// <summary>
	/// Checks that a migration instruction is valid for the
	/// migration plan. For example if the instruction migrates
	/// an activity to a different type.
	/// </summary>
	public interface MigrationInstructionValidator
	{

	  /// <summary>
	  /// Check that a migration instruction is valid for a migration plan. If it is invalid
	  /// a failure has to added to the validation report.
	  /// </summary>
	  ///  <param name="instruction"> the instruction to validate </param>
	  /// <param name="instructions"> the complete migration plan to validate </param>
	  /// <param name="report"> the validation report </param>
	  void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report);

	}

}