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
	using FlowScopeWalker = org.camunda.bpm.engine.impl.tree.FlowScopeWalker;
	using WalkCondition = org.camunda.bpm.engine.impl.tree.ReferenceWalker.WalkCondition;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;

	/// <summary>
	/// Validates that the target process definition cannot add a migrating multi-instance body.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CannotAddMultiInstanceBodyValidator : MigrationInstructionValidator
	{

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void validate(ValidatingMigrationInstruction instruction, final ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl targetActivity = instruction.TargetActivity;

		FlowScopeWalker flowScopeWalker = new FlowScopeWalker(targetActivity.FlowScope);
		MiBodyCollector miBodyCollector = new MiBodyCollector();
		flowScopeWalker.addPreVisitor(miBodyCollector);

		// walk until a target scope is found that is mapped
		flowScopeWalker.walkWhile(new WalkConditionAnonymousInnerClass(this, instructions));

		if (miBodyCollector.firstMiBody != null)
		{
		  report.addFailure("Target activity '" + targetActivity.Id + "' is a descendant of multi-instance body '" + miBodyCollector.firstMiBody.Id + "' that is not mapped from the source process definition.");
		}
	  }

	  private class WalkConditionAnonymousInnerClass : WalkCondition<ScopeImpl>
	  {
		  private readonly CannotAddMultiInstanceBodyValidator outerInstance;

		  private org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructions instructions;

		  public WalkConditionAnonymousInnerClass(CannotAddMultiInstanceBodyValidator outerInstance, org.camunda.bpm.engine.impl.migration.validation.instruction.ValidatingMigrationInstructions instructions)
		  {
			  this.outerInstance = outerInstance;
			  this.instructions = instructions;
		  }

		  public bool isFulfilled(ScopeImpl element)
		  {
			return element == null || instructions.getInstructionsByTargetScope(element).Count > 0;
		  }
	  }

	  public class MiBodyCollector : TreeVisitor<ScopeImpl>
	  {

		protected internal ScopeImpl firstMiBody;

		public virtual void visit(ScopeImpl obj)
		{
		  if (firstMiBody == null && obj != null && isMiBody(obj))
		  {
			firstMiBody = obj;
		  }
		}

		protected internal virtual bool isMiBody(ScopeImpl scope)
		{
		  return scope.ActivityBehavior is MultiInstanceActivityBehavior;
		}
	  }

	}

}