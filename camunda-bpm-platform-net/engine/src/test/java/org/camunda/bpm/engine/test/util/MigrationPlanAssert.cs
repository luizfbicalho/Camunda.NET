using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.test.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using MigrationInstructionImpl = org.camunda.bpm.engine.impl.migration.MigrationInstructionImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	public class MigrationPlanAssert
	{

	  protected internal MigrationPlan actual;

	  public MigrationPlanAssert(MigrationPlan actual)
	  {
		this.actual = actual;
	  }

	  public virtual MigrationPlanAssert NotNull
	  {
		  get
		  {
			assertNotNull("The migration plan is null", actual);
    
			return this;
		  }
	  }

	  public virtual MigrationPlanAssert hasSourceProcessDefinition(ProcessDefinition sourceProcessDefinition)
	  {
		return hasSourceProcessDefinitionId(sourceProcessDefinition.Id);
	  }

	  public virtual MigrationPlanAssert hasSourceProcessDefinitionId(string sourceProcessDefinitionId)
	  {
		NotNull;
		assertEquals("The source process definition id does not match", sourceProcessDefinitionId, actual.SourceProcessDefinitionId);

		return this;
	  }

	  public virtual MigrationPlanAssert hasTargetProcessDefinition(ProcessDefinition targetProcessDefinition)
	  {
		return hasTargetProcessDefinitionId(targetProcessDefinition.Id);
	  }

	  public virtual MigrationPlanAssert hasTargetProcessDefinitionId(string targetProcessDefinitionId)
	  {
		NotNull;
		assertEquals("The target process definition id does not match", targetProcessDefinitionId, actual.TargetProcessDefinitionId);

		return this;
	  }

	  public virtual MigrationPlanAssert hasInstructions(params MigrationInstructionAssert[] instructionAsserts)
	  {
		NotNull;

		IList<MigrationInstruction> notExpected = new List<MigrationInstruction>(actual.Instructions);
		IList<MigrationInstructionAssert> notFound = new List<MigrationInstructionAssert>();
		Collections.addAll(notFound, instructionAsserts);

		foreach (MigrationInstructionAssert instructionAssert in instructionAsserts)
		{
		  foreach (MigrationInstruction instruction in actual.Instructions)
		  {
			if (instructionAssert.sourceActivityId.Equals(instruction.SourceActivityId))
			{
			  notFound.Remove(instructionAssert);
			  notExpected.Remove(instruction);
			  assertEquals("Target activity ids do not match for instruction " + instruction, instructionAssert.targetActivityId, instruction.TargetActivityId);
			  if (instructionAssert.updateEventTrigger_Conflict != null)
			  {
				assertEquals("Expected instruction to update event trigger: " + instructionAssert.updateEventTrigger_Conflict + " but is: " + instruction.UpdateEventTrigger, instructionAssert.updateEventTrigger_Conflict, instruction.UpdateEventTrigger);
			  }
			}
		  }
		}

		if (notExpected.Count > 0 || notFound.Count > 0)
		{
		  StringBuilder builder = new StringBuilder();
		  builder.Append("\nActual migration instructions:\n\t").Append(actual.Instructions).Append("\n");
		  if (notExpected.Count > 0)
		  {
			builder.Append("Unexpected migration instructions:\n\t").Append(notExpected).Append("\n");
		  }
		  if (notFound.Count > 0)
		  {
			builder.Append("Migration instructions missing:\n\t").Append(notFound);
		  }
		  fail(builder.ToString());
		}

		return this;
	  }

	  public virtual MigrationPlanAssert hasEmptyInstructions()
	  {
		NotNull;

		IList<MigrationInstruction> instructions = actual.Instructions;
		assertTrue("Expected migration plan has no instructions but has: " + instructions, instructions.Count == 0);

		return this;
	  }

	  public static MigrationPlanAssert assertThat(MigrationPlan migrationPlan)
	  {
		return new MigrationPlanAssert(migrationPlan);
	  }

	  public static MigrationInstructionAssert migrate(string sourceActivityId)
	  {
		return (new MigrationInstructionAssert()).from(sourceActivityId);
	  }

	  public class MigrationInstructionAssert
	  {
		protected internal string sourceActivityId;
		protected internal string targetActivityId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal bool? updateEventTrigger_Conflict;

		public virtual MigrationInstructionAssert from(string sourceActivityId)
		{
		  this.sourceActivityId = sourceActivityId;
		  return this;
		}

		public virtual MigrationInstructionAssert to(string targetActivityId)
		{
		  this.targetActivityId = targetActivityId;
		  return this;
		}

		public virtual MigrationInstructionAssert updateEventTrigger(bool updateEventTrigger)
		{
		  this.updateEventTrigger_Conflict = updateEventTrigger;
		  return this;
		}

		public override string ToString()
		{
		  return (new MigrationInstructionImpl(sourceActivityId, targetActivityId)).ToString();
		}

	  }

	}

}