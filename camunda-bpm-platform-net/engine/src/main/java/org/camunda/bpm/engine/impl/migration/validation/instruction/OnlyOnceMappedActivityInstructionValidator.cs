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

	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;

	public class OnlyOnceMappedActivityInstructionValidator : MigrationInstructionValidator
	{

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		IList<ValidatingMigrationInstruction> instructionsForSourceActivity = instructions.getInstructionsBySourceScope(sourceActivity);

		if (instructionsForSourceActivity.Count > 1)
		{
		  addFailure(sourceActivity.Id, instructionsForSourceActivity, report);
		}
	  }

	  protected internal virtual void addFailure(string sourceActivityId, IList<ValidatingMigrationInstruction> migrationInstructions, MigrationInstructionValidationReportImpl report)
	  {
		report.addFailure("There are multiple mappings for source activity id '" + sourceActivityId + "': " + StringUtil.join(new StringIteratorAnonymousInnerClass(this, migrationInstructions.GetEnumerator())));
	  }

	  private class StringIteratorAnonymousInnerClass : StringUtil.StringIterator<ValidatingMigrationInstruction>
	  {
		  private readonly OnlyOnceMappedActivityInstructionValidator outerInstance;

		  public StringIteratorAnonymousInnerClass(OnlyOnceMappedActivityInstructionValidator outerInstance, UnknownType iterator) : base(iterator)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string next()
		  {
			return iterator.next().ToString();
		  }
	  }

	}

}