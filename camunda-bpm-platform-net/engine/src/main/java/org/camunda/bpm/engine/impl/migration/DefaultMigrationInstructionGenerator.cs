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

	using ConditionalEventBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ConditionalEventBehavior;
	using MigrationActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.MigrationActivityValidator;
	using CannotAddMultiInstanceInnerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.CannotAddMultiInstanceInnerActivityValidator;
	using CannotRemoveMultiInstanceInnerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.CannotRemoveMultiInstanceInnerActivityValidator;
	using MigrationInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationInstructionValidator;
	using UpdateEventTriggersValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.UpdateEventTriggersValidator;
	using ValidatingMigrationInstruction = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstruction;
	using ValidatingMigrationInstructionImpl = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructionImpl;
	using ValidatingMigrationInstructions = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructions;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class DefaultMigrationInstructionGenerator : MigrationInstructionGenerator
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<MigrationActivityValidator> migrationActivityValidators_Conflict = new List<MigrationActivityValidator>();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<MigrationInstructionValidator> migrationInstructionValidators_Conflict = new List<MigrationInstructionValidator>();
	  protected internal MigrationActivityMatcher migrationActivityMatcher;

	  public DefaultMigrationInstructionGenerator(MigrationActivityMatcher migrationActivityMatcher)
	  {
		this.migrationActivityMatcher = migrationActivityMatcher;
	  }

	  public virtual MigrationInstructionGenerator migrationActivityValidators(IList<MigrationActivityValidator> migrationActivityValidators)
	  {
		this.migrationActivityValidators_Conflict = migrationActivityValidators;
		return this;
	  }

	  public virtual MigrationInstructionGenerator migrationInstructionValidators(IList<MigrationInstructionValidator> migrationInstructionValidators)
	  {

		this.migrationInstructionValidators_Conflict = new List<MigrationInstructionValidator>();
		foreach (MigrationInstructionValidator validator in migrationInstructionValidators)
		{
		  // ignore the following two validators during generation. Enables multi-instance bodies to be mapped.
		  // this procedure is fine because these validators are again applied after all instructions have been generated
		  if (!(validator is CannotAddMultiInstanceInnerActivityValidator || validator is CannotRemoveMultiInstanceInnerActivityValidator))
		  {
			this.migrationInstructionValidators_Conflict.Add(validator);
		  }
		}

		return this;
	  }

	  public virtual ValidatingMigrationInstructions generate(ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition, bool updateEventTriggers)
	  {
		ValidatingMigrationInstructions migrationInstructions = new ValidatingMigrationInstructions();
		generate(sourceProcessDefinition, targetProcessDefinition, sourceProcessDefinition, targetProcessDefinition, migrationInstructions, updateEventTriggers);
		return migrationInstructions;
	  }

	  protected internal virtual IList<ValidatingMigrationInstruction> generateInstructionsForActivities(ICollection<ActivityImpl> sourceActivities, ICollection<ActivityImpl> targetActivities, bool updateEventTriggers, ValidatingMigrationInstructions existingInstructions)
	  {

		IList<ValidatingMigrationInstruction> generatedInstructions = new List<ValidatingMigrationInstruction>();

		foreach (ActivityImpl sourceActivity in sourceActivities)
		{
		  if (!existingInstructions.containsInstructionForSourceScope(sourceActivity))
		  {
			foreach (ActivityImpl targetActivity in targetActivities)
			{


			  if (isValidActivity(sourceActivity) && isValidActivity(targetActivity) && migrationActivityMatcher.matchActivities(sourceActivity, targetActivity))
			  {

				//for conditional events the update event trigger must be set
				bool updateEventTriggersForInstruction = sourceActivity.ActivityBehavior is ConditionalEventBehavior || updateEventTriggers && UpdateEventTriggersValidator.definesPersistentEventTrigger(sourceActivity);

				ValidatingMigrationInstruction generatedInstruction = new ValidatingMigrationInstructionImpl(sourceActivity, targetActivity, updateEventTriggersForInstruction);
				generatedInstructions.Add(generatedInstruction);
			  }
			}
		  }
		}

		return generatedInstructions;
	  }

	  public virtual void generate(ScopeImpl sourceScope, ScopeImpl targetScope, ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition, ValidatingMigrationInstructions existingInstructions, bool updateEventTriggers)
	  {

		IList<ValidatingMigrationInstruction> flowScopeInstructions = generateInstructionsForActivities(sourceScope.Activities, targetScope.Activities, updateEventTriggers, existingInstructions);

		existingInstructions.addAll(flowScopeInstructions);

		IList<ValidatingMigrationInstruction> eventScopeInstructions = generateInstructionsForActivities(sourceScope.EventActivities, targetScope.EventActivities, updateEventTriggers, existingInstructions);

		existingInstructions.addAll(eventScopeInstructions);

		existingInstructions.filterWith(migrationInstructionValidators_Conflict);

		foreach (ValidatingMigrationInstruction generatedInstruction in flowScopeInstructions)
		{
		  if (existingInstructions.contains(generatedInstruction))
		  {
			generate(generatedInstruction.SourceActivity, generatedInstruction.TargetActivity, sourceProcessDefinition, targetProcessDefinition, existingInstructions, updateEventTriggers);
		  }
		}
	  }

	  protected internal virtual bool isValidActivity(ActivityImpl activity)
	  {
		foreach (MigrationActivityValidator migrationActivityValidator in migrationActivityValidators_Conflict)
		{
		  if (!migrationActivityValidator.valid(activity))
		  {
			return false;
		  }
		}
		return true;
	  }

	}

}