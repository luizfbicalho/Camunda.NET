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
	using MultiInstanceActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// Validates that the target process definition cannot add a new inner activity to a migrating multi-instance body.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CannotAddMultiInstanceInnerActivityValidator : MigrationInstructionValidator
	{

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl targetActivity = instruction.TargetActivity;

		if (isMultiInstance(targetActivity))
		{
		  ActivityImpl innerActivity = getInnerActivity(targetActivity);

		  if (instructions.getInstructionsByTargetScope(innerActivity).Count == 0)
		  {
			report.addFailure("Must map the inner activity of a multi-instance body when the body is mapped");
		  }
		}
	  }

	  protected internal virtual bool isMultiInstance(ScopeImpl scope)
	  {
		return scope.ActivityBehavior is MultiInstanceActivityBehavior;
	  }

	  protected internal virtual ActivityImpl getInnerActivity(ActivityImpl multiInstanceBody)
	  {
		MultiInstanceActivityBehavior activityBehavior = (MultiInstanceActivityBehavior) multiInstanceBody.ActivityBehavior;
		return activityBehavior.getInnerActivity(multiInstanceBody);
	  }

	}

}