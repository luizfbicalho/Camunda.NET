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

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	public class SameEventScopeInstructionValidator : MigrationInstructionValidator
	{

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		if (isCompensationBoundaryEvent(sourceActivity))
		{
		  // this is not required for compensation boundary events since their
		  // event scopes need not be active at runtime
		  return;
		}

		ScopeImpl sourceEventScope = instruction.SourceActivity.EventScope;
		ScopeImpl targetEventScope = instruction.TargetActivity.EventScope;

		if (sourceEventScope == null || sourceEventScope == sourceActivity.FlowScope)
		{
		  // event scopes must only match if the event scopes are not the flow scopes
		  // => validation necessary for boundary events;
		  // => validation not necessary for event subprocesses
		  return;
		}

		if (targetEventScope == null)
		{
		  report.addFailure("The source activity's event scope (" + sourceEventScope.Id + ") must be mapped but the " + "target activity has no event scope");
		}
		else
		{
		  ScopeImpl mappedSourceEventScope = findMappedEventScope(sourceEventScope, instruction, instructions);
		  if (mappedSourceEventScope == null || !mappedSourceEventScope.Id.Equals(targetEventScope.Id))
		  {
			report.addFailure("The source activity's event scope (" + sourceEventScope.Id + ") " + "must be mapped to the target activity's event scope (" + targetEventScope.Id + ")");
		  }
		}
	  }

	  protected internal virtual bool isCompensationBoundaryEvent(ActivityImpl sourceActivity)
	  {
		string activityType = sourceActivity.Properties.get(BpmnProperties.TYPE);
		return ActivityTypes.BOUNDARY_COMPENSATION.Equals(activityType);
	  }

	  protected internal virtual ScopeImpl findMappedEventScope(ScopeImpl sourceEventScope, ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions)
	  {
		if (sourceEventScope != null)
		{
		  if (sourceEventScope == sourceEventScope.ProcessDefinition)
		  {
			return instruction.TargetActivity.ProcessDefinition;
		  }
		  else
		  {
			IList<ValidatingMigrationInstruction> eventScopeInstructions = instructions.getInstructionsBySourceScope(sourceEventScope);
			if (eventScopeInstructions.Count > 0)
			{
			  return eventScopeInstructions[0].TargetActivity;
			}
		  }
		}
		return null;
	  }

	  protected internal virtual void addFailure(ValidatingMigrationInstruction instruction, MigrationInstructionValidationReportImpl report, string sourceScopeId, string targetScopeId)
	  {
		report.addFailure("The source activity's event scope (" + sourceScopeId + ") " + "must be mapped to the target activity's event scope (" + targetScopeId + ")");
	  }

	}

}