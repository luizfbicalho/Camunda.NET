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
namespace org.camunda.bpm.engine.rest
{

	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using MigrationExecutionDto = org.camunda.bpm.engine.rest.dto.migration.MigrationExecutionDto;
	using MigrationPlanDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanDto;
	using MigrationPlanGenerationDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanGenerationDto;
	using MigrationPlanReportDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanReportDto;

	public interface MigrationRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/generate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.migration.MigrationPlanDto generateMigrationPlan(org.camunda.bpm.engine.rest.dto.migration.MigrationPlanGenerationDto generationDto);
	  MigrationPlanDto generateMigrationPlan(MigrationPlanGenerationDto generationDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/validate") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.migration.MigrationPlanReportDto validateMigrationPlan(org.camunda.bpm.engine.rest.dto.migration.MigrationPlanDto migrationPlanDto);
	  MigrationPlanReportDto validateMigrationPlan(MigrationPlanDto migrationPlanDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/execute") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void executeMigrationPlan(org.camunda.bpm.engine.rest.dto.migration.MigrationExecutionDto migrationPlan);
	  void executeMigrationPlan(MigrationExecutionDto migrationPlan);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/executeAsync") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto executeMigrationPlanAsync(org.camunda.bpm.engine.rest.dto.migration.MigrationExecutionDto migrationPlan);
	  BatchDto executeMigrationPlanAsync(MigrationExecutionDto migrationPlan);

	}

	public static class MigrationRestService_Fields
	{
	  public const string PATH = "/migration";
	}

}