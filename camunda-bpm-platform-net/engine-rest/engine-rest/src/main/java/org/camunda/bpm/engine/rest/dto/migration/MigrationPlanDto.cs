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
namespace org.camunda.bpm.engine.rest.dto.migration
{

	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationInstructionBuilder = org.camunda.bpm.engine.migration.MigrationInstructionBuilder;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	public class MigrationPlanDto
	{

	  protected internal string sourceProcessDefinitionId;
	  protected internal string targetProcessDefinitionId;
	  protected internal IList<MigrationInstructionDto> instructions;

	  public virtual string SourceProcessDefinitionId
	  {
		  get
		  {
			return sourceProcessDefinitionId;
		  }
		  set
		  {
			this.sourceProcessDefinitionId = value;
		  }
	  }


	  public virtual string TargetProcessDefinitionId
	  {
		  get
		  {
			return targetProcessDefinitionId;
		  }
		  set
		  {
			this.targetProcessDefinitionId = value;
		  }
	  }


	  public virtual IList<MigrationInstructionDto> Instructions
	  {
		  get
		  {
			return instructions;
		  }
		  set
		  {
			this.instructions = value;
		  }
	  }


	  public static MigrationPlanDto from(MigrationPlan migrationPlan)
	  {
		MigrationPlanDto dto = new MigrationPlanDto();

		dto.SourceProcessDefinitionId = migrationPlan.SourceProcessDefinitionId;
		dto.TargetProcessDefinitionId = migrationPlan.TargetProcessDefinitionId;

		List<MigrationInstructionDto> instructionDtos = new List<MigrationInstructionDto>();
		if (migrationPlan.Instructions != null)
		{
		  foreach (MigrationInstruction migrationInstruction in migrationPlan.Instructions)
		  {
			MigrationInstructionDto migrationInstructionDto = MigrationInstructionDto.from(migrationInstruction);
			instructionDtos.Add(migrationInstructionDto);
		  }
		}
		dto.Instructions = instructionDtos;

		return dto;
	  }

	  public static MigrationPlan toMigrationPlan(ProcessEngine processEngine, MigrationPlanDto migrationPlanDto)
	  {
		MigrationPlanBuilder migrationPlanBuilder = processEngine.RuntimeService.createMigrationPlan(migrationPlanDto.SourceProcessDefinitionId, migrationPlanDto.TargetProcessDefinitionId);

		if (migrationPlanDto.Instructions != null)
		{
		  foreach (MigrationInstructionDto migrationInstructionDto in migrationPlanDto.Instructions)
		  {
			MigrationInstructionBuilder migrationInstructionBuilder = migrationPlanBuilder.mapActivities(migrationInstructionDto.SourceActivityIds[0], migrationInstructionDto.TargetActivityIds[0]);
			if (true.Equals(migrationInstructionDto.UpdateEventTrigger))
			{
			  migrationInstructionBuilder = migrationInstructionBuilder.updateEventTrigger();
			}

			migrationPlanBuilder = migrationInstructionBuilder;
		  }
		}

		return migrationPlanBuilder.build();
	  }

	}

}