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
namespace org.camunda.bpm.engine.impl.cmd
{

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using MigrationInstructionGenerator = org.camunda.bpm.engine.impl.migration.MigrationInstructionGenerator;
	using MigrationLogger = org.camunda.bpm.engine.impl.migration.MigrationLogger;
	using MigrationPlanBuilderImpl = org.camunda.bpm.engine.impl.migration.MigrationPlanBuilderImpl;
	using MigrationPlanImpl = org.camunda.bpm.engine.impl.migration.MigrationPlanImpl;
	using MigrationInstructionValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationInstructionValidationReportImpl;
	using MigrationInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationInstructionValidator;
	using MigrationPlanValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationPlanValidationReportImpl;
	using ValidatingMigrationInstruction = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstruction;
	using ValidatingMigrationInstructionImpl = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructionImpl;
	using ValidatingMigrationInstructions = org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructions;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using EngineUtilLogger = org.camunda.bpm.engine.impl.util.EngineUtilLogger;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CreateMigrationPlanCmd : Command<MigrationPlan>
	{

	  public static readonly MigrationLogger LOG = EngineUtilLogger.MIGRATION_LOGGER;

	  protected internal MigrationPlanBuilderImpl migrationBuilder;

	  public CreateMigrationPlanCmd(MigrationPlanBuilderImpl migrationPlanBuilderImpl)
	  {
		this.migrationBuilder = migrationPlanBuilderImpl;
	  }

	  public virtual MigrationPlan execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity sourceProcessDefinition = getProcessDefinition(commandContext, migrationBuilder.SourceProcessDefinitionId, "Source");
		ProcessDefinitionEntity targetProcessDefinition = getProcessDefinition(commandContext, migrationBuilder.TargetProcessDefinitionId, "Target");

		checkAuthorization(commandContext, sourceProcessDefinition, targetProcessDefinition);

		MigrationPlanImpl migrationPlan = new MigrationPlanImpl(sourceProcessDefinition.Id, targetProcessDefinition.Id);
		IList<MigrationInstruction> instructions = new List<MigrationInstruction>();

		if (migrationBuilder.MapEqualActivities)
		{
		  ((IList<MigrationInstruction>)instructions).AddRange(generateInstructions(commandContext, sourceProcessDefinition, targetProcessDefinition, migrationBuilder.UpdateEventTriggersForGeneratedInstructions));
		}

		((IList<MigrationInstruction>)instructions).AddRange(migrationBuilder.ExplicitMigrationInstructions);
		migrationPlan.Instructions = instructions;

		validateMigrationPlan(commandContext, migrationPlan, sourceProcessDefinition, targetProcessDefinition);

		return migrationPlan;
	  }

	  protected internal virtual ProcessDefinitionEntity getProcessDefinition(CommandContext commandContext, string id, string type)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), type + " process definition id", id);

		try
		{
		  return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(id);
		}
		catch (NullValueException)
		{
		  throw LOG.processDefinitionDoesNotExist(id, type);
		}
	  }

	  protected internal virtual void checkAuthorization(CommandContext commandContext, ProcessDefinitionEntity sourceProcessDefinition, ProcessDefinitionEntity targetProcessDefinition)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateMigrationPlan(sourceProcessDefinition, targetProcessDefinition);
		}
	  }

	  protected internal virtual IList<MigrationInstruction> generateInstructions(CommandContext commandContext, ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition, bool updateEventTriggers)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;

		// generate instructions
		MigrationInstructionGenerator migrationInstructionGenerator = processEngineConfiguration.MigrationInstructionGenerator;
		ValidatingMigrationInstructions generatedInstructions = migrationInstructionGenerator.generate(sourceProcessDefinition, targetProcessDefinition, updateEventTriggers);

		// filter only valid instructions
		generatedInstructions.filterWith(processEngineConfiguration.MigrationInstructionValidators);

		return generatedInstructions.asMigrationInstructions();
	  }

	  protected internal virtual void validateMigrationPlan(CommandContext commandContext, MigrationPlanImpl migrationPlan, ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition)
	  {
		IList<MigrationInstructionValidator> migrationInstructionValidators = commandContext.ProcessEngineConfiguration.MigrationInstructionValidators;

		MigrationPlanValidationReportImpl planReport = new MigrationPlanValidationReportImpl(migrationPlan);
		ValidatingMigrationInstructions validatingMigrationInstructions = wrapMigrationInstructions(migrationPlan, sourceProcessDefinition, targetProcessDefinition, planReport);

		foreach (ValidatingMigrationInstruction validatingMigrationInstruction in validatingMigrationInstructions.Instructions)
		{
		  MigrationInstructionValidationReportImpl instructionReport = validateInstruction(validatingMigrationInstruction, validatingMigrationInstructions, migrationInstructionValidators);
		  if (instructionReport.hasFailures())
		  {
			planReport.addInstructionReport(instructionReport);
		  }
		}

		if (planReport.hasInstructionReports())
		{
		  throw LOG.failingMigrationPlanValidation(planReport);
		}

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

	  protected internal virtual ValidatingMigrationInstructions wrapMigrationInstructions(MigrationPlan migrationPlan, ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition, MigrationPlanValidationReportImpl planReport)
	  {
		ValidatingMigrationInstructions validatingMigrationInstructions = new ValidatingMigrationInstructions();
		foreach (MigrationInstruction migrationInstruction in migrationPlan.Instructions)
		{
		  MigrationInstructionValidationReportImpl instructionReport = new MigrationInstructionValidationReportImpl(migrationInstruction);

		  string sourceActivityId = migrationInstruction.SourceActivityId;
		  string targetActivityId = migrationInstruction.TargetActivityId;
		  if (!string.ReferenceEquals(sourceActivityId, null) && !string.ReferenceEquals(targetActivityId, null))
		  {
			ActivityImpl sourceActivity = sourceProcessDefinition.findActivity(sourceActivityId);
			ActivityImpl targetActivity = targetProcessDefinition.findActivity(migrationInstruction.TargetActivityId);

			if (sourceActivity != null && targetActivity != null)
			{
			  validatingMigrationInstructions.addInstruction(new ValidatingMigrationInstructionImpl(sourceActivity, targetActivity, migrationInstruction.UpdateEventTrigger));
			}
			else
			{
			  if (sourceActivity == null)
			  {
				instructionReport.addFailure("Source activity '" + sourceActivityId + "' does not exist");
			  }
			  if (targetActivity == null)
			  {
				instructionReport.addFailure("Target activity '" + targetActivityId + "' does not exist");
			  }
			}
		  }
		  else
		  {
			if (string.ReferenceEquals(sourceActivityId, null))
			{
			  instructionReport.addFailure("Source activity id is null");
			}
			if (string.ReferenceEquals(targetActivityId, null))
			{
			  instructionReport.addFailure("Target activity id is null");
			}
		  }

		  if (instructionReport.hasFailures())
		  {
			planReport.addInstructionReport(instructionReport);
		  }
		}
		return validatingMigrationInstructions;
	  }

	}

}