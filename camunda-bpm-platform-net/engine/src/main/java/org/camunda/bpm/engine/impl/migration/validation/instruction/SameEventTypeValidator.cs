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
	using BoundaryEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using EventSubProcessStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SameEventTypeValidator : MigrationInstructionValidator
	{

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		ActivityImpl targetActivity = instruction.TargetActivity;

		if (isEvent(sourceActivity) && isEvent(targetActivity))
		{
		  string sourceType = sourceActivity.Properties.get(BpmnProperties.TYPE);
		  string targetType = targetActivity.Properties.get(BpmnProperties.TYPE);

		  if (!sourceType.Equals(targetType))
		  {
			report.addFailure("Events are not of the same type (" + sourceType + " != " + targetType + ")");
		  }
		}
	  }

	  protected internal virtual bool isEvent(ActivityImpl activity)
	  {
		ActivityBehavior behavior = activity.ActivityBehavior;
		return behavior is BoundaryEventActivityBehavior || behavior is EventSubProcessStartEventActivityBehavior;
	  }

	}

}