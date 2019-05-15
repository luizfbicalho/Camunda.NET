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

	using InclusiveGatewayActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.InclusiveGatewayActivityBehavior;
	using ParallelGatewayActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ParallelGatewayActivityBehavior;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// <para>For synchronizing gateways (inclusive; parallel), the situation in which
	///  more tokens end up at the target gateway than there are incoming sequence flows
	///  must be avoided. Else, the migrated process instance may appear as broken to users
	///  since the migration logic cannot trigger these gateways immediately.
	/// 
	/// </para>
	///  <para>Such situations can be avoided by enforcing that
	///  <ul>
	///  <li>the target gateway has at least the same number of incoming sequence flows
	///  <li>the target gateway's flow scope is not removed
	///  <li>there is not more than one instruction that maps to the target gateway
	/// 
	/// @author Thorben Lindhauer
	/// </para>
	/// </summary>
	public class GatewayMappingValidator : MigrationInstructionValidator
	{

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {

		ActivityImpl targetActivity = instruction.TargetActivity;

		if (isWaitStateGateway(targetActivity))
		{
		  validateIncomingSequenceFlows(instruction, instructions, report);
		  validateParentScopeMigrates(instruction, instructions, report);
		  validateSingleInstruction(instruction, instructions, report);
		}
	  }


	  protected internal virtual void validateIncomingSequenceFlows(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		ActivityImpl targetActivity = instruction.TargetActivity;

		int numSourceIncomingFlows = sourceActivity.IncomingTransitions.Count;
		int numTargetIncomingFlows = targetActivity.IncomingTransitions.Count;

		if (numSourceIncomingFlows > numTargetIncomingFlows)
		{
		  report.addFailure("The target gateway must have at least the same number " + "of incoming sequence flows that the source gateway has");
		}
	  }

	  protected internal virtual void validateParentScopeMigrates(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		ScopeImpl flowScope = sourceActivity.FlowScope;

		if (flowScope != flowScope.ProcessDefinition)
		{
		  if (instructions.getInstructionsBySourceScope(flowScope).Count == 0)
		  {
			report.addFailure("The gateway's flow scope '" + flowScope.Id + "' must be mapped");
		  }
		}
	  }

	  protected internal virtual void validateSingleInstruction(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl targetActivity = instruction.TargetActivity;
		IList<ValidatingMigrationInstruction> instructionsToTargetGateway = instructions.getInstructionsByTargetScope(targetActivity);

		if (instructionsToTargetGateway.Count > 1)
		{
		  report.addFailure("Only one gateway can be mapped to gateway '" + targetActivity.Id + "'");
		}
	  }

	  protected internal virtual bool isWaitStateGateway(ActivityImpl activity)
	  {
		ActivityBehavior behavior = activity.ActivityBehavior;
		return behavior is ParallelGatewayActivityBehavior || behavior is InclusiveGatewayActivityBehavior;
	  }


	}

}