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
namespace org.camunda.bpm.engine.rest.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using MigrationInstructionsBuilder = org.camunda.bpm.engine.migration.MigrationInstructionsBuilder;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanExecutionBuilder = org.camunda.bpm.engine.migration.MigrationPlanExecutionBuilder;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using MigrationExecutionDto = org.camunda.bpm.engine.rest.dto.migration.MigrationExecutionDto;
	using MigrationPlanDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanDto;
	using MigrationPlanGenerationDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanGenerationDto;
	using MigrationPlanReportDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanReportDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class MigrationRestServiceImpl : AbstractRestProcessEngineAware, MigrationRestService
	{

	  public MigrationRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual MigrationPlanDto generateMigrationPlan(MigrationPlanGenerationDto generationDto)
	  {
		RuntimeService runtimeService = processEngine.RuntimeService;

		string sourceProcessDefinitionId = generationDto.SourceProcessDefinitionId;
		string targetProcessDefinitionId = generationDto.TargetProcessDefinitionId;

		try
		{
		  MigrationInstructionsBuilder instructionsBuilder = runtimeService.createMigrationPlan(sourceProcessDefinitionId, targetProcessDefinitionId).mapEqualActivities();

		  if (generationDto.UpdateEventTriggers)
		  {
			instructionsBuilder = instructionsBuilder.updateEventTriggers();
		  }

		  MigrationPlan migrationPlan = instructionsBuilder.build();

		  return MigrationPlanDto.from(migrationPlan);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, e.Message);
		}
	  }

	  public virtual MigrationPlanReportDto validateMigrationPlan(MigrationPlanDto migrationPlanDto)
	  {
		try
		{
		  createMigrationPlan(migrationPlanDto);
		  // return an empty report if not errors are found
		  return MigrationPlanReportDto.emptyReport();
		}
		catch (MigrationPlanValidationException e)
		{
		 return MigrationPlanReportDto.form(e.ValidationReport);
		}
	  }

	  public virtual void executeMigrationPlan(MigrationExecutionDto migrationExecution)
	  {
		createMigrationPlanExecutionBuilder(migrationExecution).execute();
	  }

	  public virtual BatchDto executeMigrationPlanAsync(MigrationExecutionDto migrationExecution)
	  {
		Batch batch = createMigrationPlanExecutionBuilder(migrationExecution).executeAsync();
		return BatchDto.fromBatch(batch);
	  }

	  protected internal virtual MigrationPlanExecutionBuilder createMigrationPlanExecutionBuilder(MigrationExecutionDto migrationExecution)
	  {
		MigrationPlan migrationPlan = createMigrationPlan(migrationExecution.MigrationPlan);
		IList<string> processInstanceIds = migrationExecution.ProcessInstanceIds;

		MigrationPlanExecutionBuilder executionBuilder = processEngine.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstanceIds);

		ProcessInstanceQueryDto processInstanceQueryDto = migrationExecution.ProcessInstanceQuery;
		if (processInstanceQueryDto != null)
		{
		  ProcessInstanceQuery processInstanceQuery = processInstanceQueryDto.toQuery(ProcessEngine);
		  executionBuilder.processInstanceQuery(processInstanceQuery);
		}

		if (migrationExecution.SkipCustomListeners)
		{
		  executionBuilder.skipCustomListeners();
		}

		if (migrationExecution.SkipIoMappings)
		{
		  executionBuilder.skipIoMappings();
		}

		return executionBuilder;
	  }

	  protected internal virtual MigrationPlan createMigrationPlan(MigrationPlanDto migrationPlanDto)
	  {
		try
		{
		  return MigrationPlanDto.toMigrationPlan(processEngine, migrationPlanDto);
		}
		catch (MigrationPlanValidationException e)
		{
		  throw e;
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, e.Message);
		}
	  }

	}

}