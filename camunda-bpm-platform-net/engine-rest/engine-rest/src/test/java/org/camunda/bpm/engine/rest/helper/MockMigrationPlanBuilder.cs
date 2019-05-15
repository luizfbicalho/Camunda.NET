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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationInstructionBuilder = org.camunda.bpm.engine.migration.MigrationInstructionBuilder;
	using MigrationInstructionsBuilder = org.camunda.bpm.engine.migration.MigrationInstructionsBuilder;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;

	public class MockMigrationPlanBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string sourceProcessDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string targetProcessDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<MigrationInstruction> instructions_Renamed = new List<MigrationInstruction>();

	  public virtual MockMigrationPlanBuilder sourceProcessDefinitionId(string sourceProcessDefinitionId)
	  {
		this.sourceProcessDefinitionId_Renamed = sourceProcessDefinitionId;
		return this;
	  }

	  public virtual MockMigrationPlanBuilder targetProcessDefinitionId(string targetProcessDefinitionId)
	  {
		this.targetProcessDefinitionId_Renamed = targetProcessDefinitionId;
		return this;
	  }

	  public virtual MockMigrationPlanBuilder instructions(IList<MigrationInstruction> instructions)
	  {
		this.instructions_Renamed = instructions;
		return this;
	  }

	  public virtual MockMigrationPlanBuilder instruction(MigrationInstruction instruction)
	  {
		instructions_Renamed.Add(instruction);
		return this;
	  }

	  public virtual MockMigrationPlanBuilder instruction(string sourceActivityId, string targetActivityId)
	  {
		MigrationInstruction instructionMock = (new MockMigrationInstructionBuilder()).sourceActivityId(sourceActivityId).targetActivityId(targetActivityId).build();
		return instruction(instructionMock);
	  }

	  public virtual MockMigrationPlanBuilder instruction(string sourceActivityId, string targetActivityId, bool? updateEventTrigger)
	  {
		MockMigrationInstructionBuilder instructionBuilder = (new MockMigrationInstructionBuilder()).sourceActivityId(sourceActivityId).targetActivityId(targetActivityId);

		if (true.Equals(updateEventTrigger))
		{
		  instructionBuilder = instructionBuilder.updateEventTrigger();
		}

		MigrationInstruction instructionMock = instructionBuilder.build();
		return instruction(instructionMock);
	  }

	  public virtual MigrationPlan build()
	  {
		MigrationPlan migrationPlanMock = mock(typeof(MigrationPlan));
		when(migrationPlanMock.SourceProcessDefinitionId).thenReturn(sourceProcessDefinitionId_Renamed);
		when(migrationPlanMock.TargetProcessDefinitionId).thenReturn(targetProcessDefinitionId_Renamed);
		when(migrationPlanMock.Instructions).thenReturn(instructions_Renamed);
		return migrationPlanMock;
	  }

	  public virtual JoinedMigrationPlanBuilderMock builder()
	  {
		MigrationPlan migrationPlanMock = build();
		JoinedMigrationPlanBuilderMock migrationPlanBuilderMock = mock(typeof(JoinedMigrationPlanBuilderMock), new FluentAnswer());

		when(migrationPlanBuilderMock.build()).thenReturn(migrationPlanMock);

		return migrationPlanBuilderMock;
	  }

	  public interface JoinedMigrationPlanBuilderMock : MigrationPlanBuilder, MigrationInstructionBuilder, MigrationInstructionsBuilder
	  {
		// Just an empty interface joining all migration plan builder interfaces together.
		// Allows it to mock all three with one mock instance, which in turn makes invocation verification easier.
		// Quite a hack that may break if the interfaces become incompatible in the future.
	  }

	}

}