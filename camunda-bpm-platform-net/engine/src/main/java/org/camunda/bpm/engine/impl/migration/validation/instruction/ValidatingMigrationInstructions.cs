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
namespace org.camunda.bpm.engine.impl.migration.validation.instruction
{

	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ValidatingMigrationInstructions
	{

	  protected internal ICollection<ValidatingMigrationInstruction> instructions;
	  protected internal IDictionary<ScopeImpl, IList<ValidatingMigrationInstruction>> instructionsBySourceScope;
	  protected internal IDictionary<ScopeImpl, IList<ValidatingMigrationInstruction>> instructionsByTargetScope;

	  public ValidatingMigrationInstructions(ICollection<ValidatingMigrationInstruction> instructions)
	  {
		this.instructions = instructions;
		instructionsBySourceScope = new Dictionary<ScopeImpl, IList<ValidatingMigrationInstruction>>();
		instructionsByTargetScope = new Dictionary<ScopeImpl, IList<ValidatingMigrationInstruction>>();

		foreach (ValidatingMigrationInstruction instruction in instructions)
		{
		  indexInstruction(instruction);
		}
	  }

	  public ValidatingMigrationInstructions() : this(new HashSet<ValidatingMigrationInstruction>())
	  {
	  }

	  public virtual void addInstruction(ValidatingMigrationInstruction instruction)
	  {
		instructions.Add(instruction);
		indexInstruction(instruction);
	  }

	  public virtual void addAll(IList<ValidatingMigrationInstruction> instructions)
	  {
		foreach (ValidatingMigrationInstruction instruction in instructions)
		{
		  addInstruction(instruction);
		}
	  }

	  protected internal virtual void indexInstruction(ValidatingMigrationInstruction instruction)
	  {
		CollectionUtil.addToMapOfLists(instructionsBySourceScope, instruction.SourceActivity, instruction);
		CollectionUtil.addToMapOfLists(instructionsByTargetScope, instruction.TargetActivity, instruction);
	  }

	  public virtual IList<ValidatingMigrationInstruction> Instructions
	  {
		  get
		  {
			return new List<ValidatingMigrationInstruction>(instructions);
		  }
	  }

	  public virtual IList<ValidatingMigrationInstruction> getInstructionsBySourceScope(ScopeImpl scope)
	  {
		IList<ValidatingMigrationInstruction> instructions = instructionsBySourceScope[scope];

		if (instructions == null)
		{
		  return Collections.emptyList();
		}
		else
		{
		  return instructions;
		}
	  }

	  public virtual IList<ValidatingMigrationInstruction> getInstructionsByTargetScope(ScopeImpl scope)
	  {
		IList<ValidatingMigrationInstruction> instructions = instructionsByTargetScope[scope];

		if (instructions == null)
		{
		  return Collections.emptyList();
		}
		else
		{
		  return instructions;
		}
	  }

	  public virtual void filterWith(IList<MigrationInstructionValidator> validators)
	  {
		IList<ValidatingMigrationInstruction> validInstructions = new List<ValidatingMigrationInstruction>();

		foreach (ValidatingMigrationInstruction instruction in instructions)
		{
		  if (isValidInstruction(instruction, this, validators))
		  {
			validInstructions.Add(instruction);
		  }
		}

		instructionsBySourceScope.Clear();
		instructionsByTargetScope.Clear();
		instructions.Clear();

		foreach (ValidatingMigrationInstruction validInstruction in validInstructions)
		{
		  addInstruction(validInstruction);
		}
	  }

	  public virtual IList<MigrationInstruction> asMigrationInstructions()
	  {
		IList<MigrationInstruction> instructions = new List<MigrationInstruction>();

		foreach (ValidatingMigrationInstruction instruction in this.instructions)
		{
		  instructions.Add(instruction.toMigrationInstruction());
		}

		return instructions;
	  }

	  public virtual bool contains(ValidatingMigrationInstruction instruction)
	  {
		return instructions.Contains(instruction);
	  }

	  public virtual bool containsInstructionForSourceScope(ScopeImpl sourceScope)
	  {
		return instructionsBySourceScope.ContainsKey(sourceScope);
	  }

	  protected internal virtual bool isValidInstruction(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, IList<MigrationInstructionValidator> migrationInstructionValidators)
	  {
		return !validateInstruction(instruction, instructions, migrationInstructionValidators).hasFailures();
	  }

	  protected internal virtual MigrationInstructionValidationReportImpl validateInstruction(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, IList<MigrationInstructionValidator> migrationInstructionValidators)
	  {
		MigrationInstructionValidationReportImpl validationReport = new MigrationInstructionValidationReportImpl(instruction.toMigrationInstruction());
		foreach (MigrationInstructionValidator migrationInstructionValidator in migrationInstructionValidators)
		{
		  migrationInstructionValidator.validate(instruction, instructions, validationReport);
		}
		return validationReport;
	  }
	}

}