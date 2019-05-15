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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_JOBS_PER_SEED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_JOB_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_TOTAL_JOBS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_INVOCATIONS_PER_BATCH_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_MONITOR_JOB_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_SEED_JOB_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TENANT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.NON_EXISTING_ACTIVITY_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockBatch;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.NoIntermediaryInvocation.immediatelyAfter;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.isNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl;
	using MigratingActivityInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingActivityInstanceValidationReport;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigratingProcessInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationReport;
	using MigratingTransitionInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingTransitionInstanceValidationReport;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationInstructionValidationReport = org.camunda.bpm.engine.migration.MigrationInstructionValidationReport;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;
	using MigrationPlanExecutionBuilder = org.camunda.bpm.engine.migration.MigrationPlanExecutionBuilder;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using MigrationPlanValidationReport = org.camunda.bpm.engine.migration.MigrationPlanValidationReport;
	using MigrationInstructionDto = org.camunda.bpm.engine.rest.dto.migration.MigrationInstructionDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using FluentAnswer = org.camunda.bpm.engine.rest.helper.FluentAnswer;
	using MockMigrationPlanBuilder = org.camunda.bpm.engine.rest.helper.MockMigrationPlanBuilder;
	using JoinedMigrationPlanBuilderMock = org.camunda.bpm.engine.rest.helper.MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using MigrationExecutionDtoBuilder = org.camunda.bpm.engine.rest.util.migration.MigrationExecutionDtoBuilder;
	using MigrationPlanDtoBuilder = org.camunda.bpm.engine.rest.util.migration.MigrationPlanDtoBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using Response = io.restassured.response.Response;
	using MigrationInstructionDtoBuilder = org.camunda.bpm.engine.rest.util.migration.MigrationInstructionDtoBuilder;

	public class MigrationRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string MIGRATION_URL = TEST_RESOURCE_ROOT_PATH + "/migration";
	  protected internal static readonly string GENERATE_MIGRATION_URL = MIGRATION_URL + "/generate";
	  protected internal static readonly string VALIDATE_MIGRATION_URL = MIGRATION_URL + "/validate";
	  protected internal static readonly string EXECUTE_MIGRATION_URL = MIGRATION_URL + "/execute";
	  protected internal static readonly string EXECUTE_MIGRATION_ASYNC_URL = MIGRATION_URL + "/executeAsync";

	  protected internal RuntimeService runtimeServiceMock;
	  protected internal MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilderMock;
	  protected internal MigrationPlanExecutionBuilder migrationPlanExecutionBuilderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		migrationPlanBuilderMock = (new MockMigrationPlanBuilder()).sourceProcessDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).targetProcessDefinitionId(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).builder();

		when(runtimeServiceMock.createMigrationPlan(eq(EXAMPLE_PROCESS_DEFINITION_ID), eq(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(migrationPlanBuilderMock);

		migrationPlanExecutionBuilderMock = mock(typeof(MigrationPlanExecutionBuilder));
		when(migrationPlanExecutionBuilderMock.processInstanceIds(anyListOf(typeof(string)))).thenReturn(migrationPlanExecutionBuilderMock);

		when(runtimeServiceMock.newMigration(any(typeof(MigrationPlan)))).thenReturn(migrationPlanExecutionBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithInitialEmptyInstructions()
	  public virtual void generateMigrationPlanWithInitialEmptyInstructions()
	  {
		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).instructions(System.Linq.Enumerable.Empty<IDictionary<string, object>>()).build();

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.OK.StatusCode).when().post(GENERATE_MIGRATION_URL);

		verifyGenerateMigrationPlanInteraction(migrationPlanBuilderMock, initialMigrationPlan);
		verifyGenerateMigrationPlanResponse(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithInitialNullInstructions()
	  public virtual void generateMigrationPlanWithInitialNullInstructions()
	  {
		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).instructions(null).build();

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.OK.StatusCode).when().post(GENERATE_MIGRATION_URL);

		verifyGenerateMigrationPlanInteraction(migrationPlanBuilderMock, initialMigrationPlan);
		verifyGenerateMigrationPlanResponse(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithNoInitialInstructions()
	  public virtual void generateMigrationPlanWithNoInitialInstructions()
	  {
		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).build();

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().statusCode(Status.OK.StatusCode).when().post(GENERATE_MIGRATION_URL);

		verifyGenerateMigrationPlanInteraction(migrationPlanBuilderMock, initialMigrationPlan);
		verifyGenerateMigrationPlanResponse(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanIgnoringInitialInstructions()
	  public virtual void generateMigrationPlanIgnoringInitialInstructions()
	  {
		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).instruction("ignored", "ignored").build();

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.OK.StatusCode).when().post(GENERATE_MIGRATION_URL);

		verifyGenerateMigrationPlanInteraction(migrationPlanBuilderMock, initialMigrationPlan);
		verifyGenerateMigrationPlanResponse(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithNullSourceProcessDefinition()
	  public virtual void generateMigrationPlanWithNullSourceProcessDefinition()
	  {
		string message = "source process definition id is null";
		MigrationPlanBuilder planBuilder = mock(typeof(MigrationPlanBuilder), Mockito.RETURNS_DEEP_STUBS);
		when(runtimeServiceMock.createMigrationPlan(isNull(typeof(string)), anyString())).thenReturn(planBuilder);

		when(planBuilder.mapEqualActivities().build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(null, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(GENERATE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithNonExistingSourceProcessDefinition()
	  public virtual void generateMigrationPlanWithNonExistingSourceProcessDefinition()
	  {
		string message = "source process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MigrationPlanBuilder migrationPlanBuilder = mock(typeof(MigrationPlanBuilder), Mockito.RETURNS_DEEP_STUBS);
		when(runtimeServiceMock.createMigrationPlan(eq(NON_EXISTING_PROCESS_DEFINITION_ID), anyString())).thenReturn(migrationPlanBuilder);

		when(migrationPlanBuilder.mapEqualActivities().build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(NON_EXISTING_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(GENERATE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithNullTargetProcessDefinition()
	  public virtual void generateMigrationPlanWithNullTargetProcessDefinition()
	  {
		string message = "target process definition id is null";
		MigrationPlanBuilder migrationPlanBuilder = mock(typeof(MigrationPlanBuilder), Mockito.RETURNS_DEEP_STUBS);
		when(runtimeServiceMock.createMigrationPlan(anyString(), isNull(typeof(string)))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.mapEqualActivities().build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, null)).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(GENERATE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generateMigrationPlanWithNonExistingTargetProcessDefinition()
	  public virtual void generateMigrationPlanWithNonExistingTargetProcessDefinition()
	  {
		string message = "target process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MigrationPlanBuilder migrationPlanBuilder = mock(typeof(MigrationPlanBuilder), Mockito.RETURNS_DEEP_STUBS);
		when(runtimeServiceMock.createMigrationPlan(anyString(), eq(NON_EXISTING_PROCESS_DEFINITION_ID))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.mapEqualActivities().build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> initialMigrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, NON_EXISTING_PROCESS_DEFINITION_ID)).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(initialMigrationPlan).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(GENERATE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generatePlanUpdateEventTriggers()
	  public virtual void generatePlanUpdateEventTriggers()
	  {
		migrationPlanBuilderMock = (new MockMigrationPlanBuilder()).sourceProcessDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).targetProcessDefinitionId(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID, true).builder();

		IDictionary<string, object> generationRequest = new Dictionary<string, object>();
		generationRequest["sourceProcessDefinitionId"] = EXAMPLE_PROCESS_DEFINITION_ID;
		generationRequest["targetProcessDefinitionId"] = ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID;
		generationRequest["updateEventTriggers"] = true;

		when(runtimeServiceMock.createMigrationPlan(anyString(), anyString())).thenReturn(migrationPlanBuilderMock);

		given().contentType(POST_JSON_CONTENT_TYPE).body(generationRequest).then().expect().statusCode(Status.OK.StatusCode).when().post(GENERATE_MIGRATION_URL);

		verify(runtimeServiceMock).createMigrationPlan(eq(EXAMPLE_PROCESS_DEFINITION_ID), eq(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID));

		InOrder inOrder = Mockito.inOrder(migrationPlanBuilderMock);

		// the map equal activities method should be called
		inOrder.verify(migrationPlanBuilderMock).mapEqualActivities();
		inOrder.verify(migrationPlanBuilderMock, immediatelyAfter()).updateEventTriggers();
		verify(migrationPlanBuilderMock, never()).mapActivities(anyString(), anyString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void generatePlanUpdateEventTriggerResponse()
	  public virtual void generatePlanUpdateEventTriggerResponse()
	  {
		migrationPlanBuilderMock = (new MockMigrationPlanBuilder()).sourceProcessDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).targetProcessDefinitionId(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID, true).builder();
		when(runtimeServiceMock.createMigrationPlan(anyString(), anyString())).thenReturn(migrationPlanBuilderMock);

		IDictionary<string, object> generationRequest = new Dictionary<string, object>();
		  generationRequest["sourceProcessDefinitionId"] = EXAMPLE_PROCESS_DEFINITION_ID;
		  generationRequest["targetProcessDefinitionId"] = ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(generationRequest).then().expect().statusCode(Status.OK.StatusCode).body("instructions[0].sourceActivityIds[0]", equalTo(EXAMPLE_ACTIVITY_ID)).body("instructions[0].targetActivityIds[0]", equalTo(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("instructions[0].updateEventTrigger", equalTo(true)).when().post(GENERATE_MIGRATION_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlan()
	  public virtual void executeMigrationPlan()
	  {
		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MIGRATION_URL);

		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, (IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN]);
		verifyMigrationPlanExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithProcessInstanceQuery()
	  public virtual void executeMigrationPlanWithProcessInstanceQuery()
	  {
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());

		ProcessInstanceQueryDto processInstanceQuery = new ProcessInstanceQueryDto();
		processInstanceQuery.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstanceQuery(processInstanceQuery).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MIGRATION_URL);


		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, (IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN]);
		verifyMigrationPlanExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanSkipListeners()
	  public virtual void executeMigrationPlanSkipListeners()
	  {

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID).skipCustomListeners(true).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MIGRATION_URL);

		verifyMigrationPlanExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanSkipIoMappings()
	  public virtual void executeMigrationPlanSkipIoMappings()
	  {

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID).skipIoMappings(true).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MIGRATION_URL);

		verifyMigrationPlanExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNullInstructions()
	  public virtual void executeMigrationPlanWithNullInstructions()
	  {
		MigrationInstructionValidationReport instructionReport = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport.MigrationInstruction).thenReturn(null);
		when(instructionReport.Failures).thenReturn(Collections.singletonList("failure"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Collections.singletonList(instructionReport));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(1)).body("validationReport.instructionReports[0].instruction", nullValue()).body("validationReport.instructionReports[0].failures", hasSize(1)).body("validationReport.instructionReports[0].failures[0]", @is("failure")).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithEmptyInstructions()
	  public virtual void executeMigrationPlanWithEmptyInstructions()
	  {
		MigrationInstructionValidationReport instructionReport = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport.MigrationInstruction).thenReturn(null);
		when(instructionReport.Failures).thenReturn(Collections.singletonList("failure"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Collections.singletonList(instructionReport));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		((IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN])[MigrationPlanDtoBuilder.PROP_INSTRUCTIONS] = System.Linq.Enumerable.Empty<MigrationInstructionDto>();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(1)).body("validationReport.instructionReports[0].instruction", nullValue()).body("validationReport.instructionReports[0].failures", hasSize(1)).body("validationReport.instructionReports[0].failures[0]", @is("failure")).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNullSourceProcessInstanceId()
	  public virtual void executeMigrationPlanWithNullSourceProcessInstanceId()
	  {
		string message = "source process definition id is null";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(isNull(typeof(string)), anyString())).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(null, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNonExistingSourceProcessInstanceId()
	  public virtual void executeMigrationPlanWithNonExistingSourceProcessInstanceId()
	  {
		string message = "source process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(eq(NON_EXISTING_PROCESS_DEFINITION_ID), anyString())).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(NON_EXISTING_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNullTargetProcessInstanceId()
	  public virtual void executeMigrationPlanWithNullTargetProcessInstanceId()
	  {
		string message = "target process definition id is null";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(anyString(), isNull(typeof(string)))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, null).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNonExistingTargetProcessInstanceId()
	  public virtual void executeMigrationPlanWithNonExistingTargetProcessInstanceId()
	  {
		string message = "target process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(anyString(), eq(NON_EXISTING_PROCESS_DEFINITION_ID))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, NON_EXISTING_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNullSourceActivityId()
	  public virtual void executeMigrationPlanWithNullSourceActivityId()
	  {
		string message = "sourceActivityId is null";
		when(migrationPlanBuilderMock.mapActivities(isNull(typeof(string)), anyString())).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(null, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNonExistingSourceActivityId()
	  public virtual void executeMigrationPlanWithNonExistingSourceActivityId()
	  {
		string message = "sourceActivity is null";
		when(migrationPlanBuilderMock.mapActivities(eq(NON_EXISTING_ACTIVITY_ID), anyString())).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(NON_EXISTING_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNullTargetActivityId()
	  public virtual void executeMigrationPlanWithNullTargetActivityId()
	  {
		string message = "targetActivityId is null";
		when(migrationPlanBuilderMock.mapActivities(anyString(), isNull(typeof(string)))).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, null).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanWithNonExistingTargetActivityId()
	  public virtual void executeMigrationPlanWithNonExistingTargetActivityId()
	  {
		string message = "targetActivity is null";
		when(migrationPlanBuilderMock.mapActivities(anyString(), eq(NON_EXISTING_ACTIVITY_ID))).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, NON_EXISTING_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanValidationException()
	  public virtual void executeMigrationPlanValidationException()
	  {

		MigrationInstruction migrationInstruction = mock(typeof(MigrationInstruction));
		when(migrationInstruction.SourceActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(migrationInstruction.TargetActivityId).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);

		MigrationInstructionValidationReport instructionReport1 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport1.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport1.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationInstructionValidationReport instructionReport2 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport2.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport2.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Arrays.asList(instructionReport1, instructionReport2));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(2)).body("validationReport.instructionReports[0].instruction.sourceActivityIds", hasSize(1)).body("validationReport.instructionReports[0].instruction.sourceActivityIds[0]", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.instructionReports[0].instruction.targetActivityIds", hasSize(1)).body("validationReport.instructionReports[0].instruction.targetActivityIds[0]", @is(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("validationReport.instructionReports[0].failures", hasSize(2)).body("validationReport.instructionReports[0].failures[0]", @is("failure1")).body("validationReport.instructionReports[0].failures[1]", @is("failure2")).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigratingProcessInstanceValidationException()
	  public virtual void executeMigratingProcessInstanceValidationException()
	  {

		MigrationInstruction migrationInstruction = mock(typeof(MigrationInstruction));
		when(migrationInstruction.SourceActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(migrationInstruction.TargetActivityId).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);

		MigratingActivityInstanceValidationReport instanceReport1 = mock(typeof(MigratingActivityInstanceValidationReport));
		when(instanceReport1.ActivityInstanceId).thenReturn(EXAMPLE_ACTIVITY_INSTANCE_ID);
		when(instanceReport1.MigrationInstruction).thenReturn(migrationInstruction);
		when(instanceReport1.SourceScopeId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(instanceReport1.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigratingTransitionInstanceValidationReport instanceReport2 = mock(typeof(MigratingTransitionInstanceValidationReport));
		when(instanceReport2.TransitionInstanceId).thenReturn("transitionInstanceId");
		when(instanceReport2.MigrationInstruction).thenReturn(migrationInstruction);
		when(instanceReport2.SourceScopeId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(instanceReport2.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigratingProcessInstanceValidationReport processInstanceReport = mock(typeof(MigratingProcessInstanceValidationReport));
		when(processInstanceReport.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(processInstanceReport.Failures).thenReturn(Arrays.asList("failure1", "failure2"));
		when(processInstanceReport.ActivityInstanceReports).thenReturn(Arrays.asList(instanceReport1));
		when(processInstanceReport.TransitionInstanceReports).thenReturn(Arrays.asList(instanceReport2));

		doThrow(new MigratingProcessInstanceValidationException("fooo", processInstanceReport)).when(migrationPlanExecutionBuilderMock).execute();

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigratingProcessInstanceValidationException).Name)).body("message", @is("fooo")).body("validationReport.processInstanceId", @is(EXAMPLE_PROCESS_INSTANCE_ID)).body("validationReport.failures", hasSize(2)).body("validationReport.failures[0]", @is("failure1")).body("validationReport.failures[1]", @is("failure2")).body("validationReport.activityInstanceValidationReports", hasSize(1)).body("validationReport.activityInstanceValidationReports[0].migrationInstruction.sourceActivityIds", hasSize(1)).body("validationReport.activityInstanceValidationReports[0].migrationInstruction.sourceActivityIds[0]", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.activityInstanceValidationReports[0].migrationInstruction.targetActivityIds", hasSize(1)).body("validationReport.activityInstanceValidationReports[0].migrationInstruction.targetActivityIds[0]", @is(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("validationReport.activityInstanceValidationReports[0].activityInstanceId", @is(EXAMPLE_ACTIVITY_INSTANCE_ID)).body("validationReport.activityInstanceValidationReports[0].sourceScopeId", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.activityInstanceValidationReports[0].failures", hasSize(2)).body("validationReport.activityInstanceValidationReports[0].failures[0]", @is("failure1")).body("validationReport.activityInstanceValidationReports[0].failures[1]", @is("failure2")).body("validationReport.transitionInstanceValidationReports", hasSize(1)).body("validationReport.transitionInstanceValidationReports[0].migrationInstruction.sourceActivityIds", hasSize(1)).body("validationReport.transitionInstanceValidationReports[0].migrationInstruction.sourceActivityIds[0]", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.transitionInstanceValidationReports[0].migrationInstruction.targetActivityIds", hasSize(1)).body("validationReport.transitionInstanceValidationReports[0].migrationInstruction.targetActivityIds[0]", @is(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("validationReport.transitionInstanceValidationReports[0].transitionInstanceId", @is("transitionInstanceId")).body("validationReport.transitionInstanceValidationReports[0].sourceScopeId", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.transitionInstanceValidationReports[0].failures", hasSize(2)).body("validationReport.transitionInstanceValidationReports[0].failures[0]", @is("failure1")).body("validationReport.transitionInstanceValidationReports[0].failures[1]", @is("failure2")).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsync()
	  public virtual void executeMigrationPlanAsync()
	  {
		Batch batchMock = createMockBatch();
		when(migrationPlanExecutionBuilderMock.executeAsync()).thenReturn(batchMock);

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.OK.StatusCode).body("id", @is(EXAMPLE_BATCH_ID)).body("type", @is(EXAMPLE_BATCH_TYPE)).body("totalJobs", @is(EXAMPLE_BATCH_TOTAL_JOBS)).body("batchJobsPerSeed", @is(EXAMPLE_BATCH_JOBS_PER_SEED)).body("invocationsPerBatchJob", @is(EXAMPLE_INVOCATIONS_PER_BATCH_JOB)).body("seedJobDefinitionId", @is(EXAMPLE_SEED_JOB_DEFINITION_ID)).body("monitorJobDefinitionId", @is(EXAMPLE_MONITOR_JOB_DEFINITION_ID)).body("batchJobDefinitionId", @is(EXAMPLE_BATCH_JOB_DEFINITION_ID)).body("tenantId", @is(EXAMPLE_TENANT_ID)).when().post(EXECUTE_MIGRATION_ASYNC_URL);

		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, (IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN]);
		verifyMigrationPlanAsyncExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithProcessInstanceQuery()
	  public virtual void executeMigrationPlanAsyncWithProcessInstanceQuery()
	  {
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());

		ProcessInstanceQueryDto processInstanceQuery = new ProcessInstanceQueryDto();
		processInstanceQuery.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		Batch batchMock = createMockBatch();
		when(migrationPlanExecutionBuilderMock.executeAsync()).thenReturn(batchMock);

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstanceQuery(processInstanceQuery).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.OK.StatusCode).body("id", @is(EXAMPLE_BATCH_ID)).body("type", @is(EXAMPLE_BATCH_TYPE)).body("totalJobs", @is(EXAMPLE_BATCH_TOTAL_JOBS)).body("batchJobsPerSeed", @is(EXAMPLE_BATCH_JOBS_PER_SEED)).body("invocationsPerBatchJob", @is(EXAMPLE_INVOCATIONS_PER_BATCH_JOB)).body("seedJobDefinitionId", @is(EXAMPLE_SEED_JOB_DEFINITION_ID)).body("monitorJobDefinitionId", @is(EXAMPLE_MONITOR_JOB_DEFINITION_ID)).body("batchJobDefinitionId", @is(EXAMPLE_BATCH_JOB_DEFINITION_ID)).body("tenantId", @is(EXAMPLE_TENANT_ID)).when().post(EXECUTE_MIGRATION_ASYNC_URL);

		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, (IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN]);
		verifyMigrationPlanAsyncExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncSkipListeners()
	  public virtual void executeMigrationPlanAsyncSkipListeners()
	  {
		Batch batchMock = createMockBatch();
		when(migrationPlanExecutionBuilderMock.executeAsync()).thenReturn(batchMock);

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID).skipCustomListeners(true).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MIGRATION_ASYNC_URL);

		verifyMigrationPlanAsyncExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncSkipIoMappings()
	  public virtual void executeMigrationPlanAsyncSkipIoMappings()
	  {
		Batch batchMock = createMockBatch();
		when(migrationPlanExecutionBuilderMock.executeAsync()).thenReturn(batchMock);

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID).skipIoMappings(true).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MIGRATION_ASYNC_URL);

		verifyMigrationPlanAsyncExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNullInstructions()
	  public virtual void executeMigrationPlanAsyncWithNullInstructions()
	  {
		MigrationInstructionValidationReport instructionReport = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport.MigrationInstruction).thenReturn(null);
		when(instructionReport.Failures).thenReturn(Collections.singletonList("failure"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Collections.singletonList(instructionReport));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(1)).body("validationReport.instructionReports[0].instruction", nullValue()).body("validationReport.instructionReports[0].failures", hasSize(1)).body("validationReport.instructionReports[0].failures[0]", @is("failure")).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithEmptyInstructions()
	  public virtual void executeMigrationPlanAsyncWithEmptyInstructions()
	  {
		MigrationInstructionValidationReport instructionReport = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport.MigrationInstruction).thenReturn(null);
		when(instructionReport.Failures).thenReturn(Collections.singletonList("failure"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Collections.singletonList(instructionReport));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		((IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN])[MigrationPlanDtoBuilder.PROP_INSTRUCTIONS] = System.Linq.Enumerable.Empty<MigrationInstructionDto>();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(1)).body("validationReport.instructionReports[0].instruction", nullValue()).body("validationReport.instructionReports[0].failures", hasSize(1)).body("validationReport.instructionReports[0].failures[0]", @is("failure")).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNullSourceProcessDefinitionId()
	  public virtual void executeMigrationPlanAsyncWithNullSourceProcessDefinitionId()
	  {
		string message = "source process definition id is null";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(isNull(typeof(string)), anyString())).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(null, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNonExistingSourceProcessDefinitionId()
	  public virtual void executeMigrationPlanAsyncWithNonExistingSourceProcessDefinitionId()
	  {
		string message = "source process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(eq(NON_EXISTING_PROCESS_DEFINITION_ID), anyString())).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(NON_EXISTING_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNullTargetProcessDefinitionId()
	  public virtual void executeMigrationPlanAsyncWithNullTargetProcessDefinitionId()
	  {
		string message = "target process definition id is null";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(anyString(), isNull(typeof(string)))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, null).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNonExistingTargetProcessDefinitionId()
	  public virtual void executeMigrationPlanAsyncWithNonExistingTargetProcessDefinitionId()
	  {
		string message = "target process definition with id " + NON_EXISTING_PROCESS_DEFINITION_ID + " does not exist";
		MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilder = mock(typeof(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock), new FluentAnswer());
		when(runtimeServiceMock.createMigrationPlan(anyString(), eq(NON_EXISTING_PROCESS_DEFINITION_ID))).thenReturn(migrationPlanBuilder);
		when(migrationPlanBuilder.build()).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, NON_EXISTING_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNullSourceActivityId()
	  public virtual void executeMigrationPlanAsyncWithNullSourceActivityId()
	  {
		string message = "sourceActivityId is null";
		when(migrationPlanBuilderMock.mapActivities(isNull(typeof(string)), anyString())).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(null, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNonExistingSourceActivityId()
	  public virtual void executeMigrationPlanAsyncWithNonExistingSourceActivityId()
	  {
		string message = "sourceActivity is null";
		when(migrationPlanBuilderMock.mapActivities(eq(NON_EXISTING_ACTIVITY_ID), anyString())).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(NON_EXISTING_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNullTargetActivityId()
	  public virtual void executeMigrationPlanAsyncWithNullTargetActivityId()
	  {
		string message = "targetActivityId is null";
		when(migrationPlanBuilderMock.mapActivities(anyString(), isNull(typeof(string)))).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, null).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncWithNonExistingTargetActivityId()
	  public virtual void executeMigrationPlanAsyncWithNonExistingTargetActivityId()
	  {
		string message = "targetActivity is null";
		when(migrationPlanBuilderMock.mapActivities(anyString(), eq(NON_EXISTING_ACTIVITY_ID))).thenThrow(new BadUserRequestException(message));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, NON_EXISTING_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MIGRATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanAsyncValidationException()
	  public virtual void executeMigrationPlanAsyncValidationException()
	  {

		MigrationInstruction migrationInstruction = mock(typeof(MigrationInstruction));
		when(migrationInstruction.SourceActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(migrationInstruction.TargetActivityId).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);

		MigrationInstructionValidationReport instructionReport1 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport1.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport1.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationInstructionValidationReport instructionReport2 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport2.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport2.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Arrays.asList(instructionReport1, instructionReport2));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(MigrationPlanValidationException).Name)).body("message", @is("fooo")).body("validationReport.instructionReports", hasSize(2)).body("validationReport.instructionReports[0].instruction.sourceActivityIds", hasSize(1)).body("validationReport.instructionReports[0].instruction.sourceActivityIds[0]", @is(EXAMPLE_ACTIVITY_ID)).body("validationReport.instructionReports[0].instruction.targetActivityIds", hasSize(1)).body("validationReport.instructionReports[0].instruction.targetActivityIds[0]", @is(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("validationReport.instructionReports[0].failures", hasSize(2)).body("validationReport.instructionReports[0].failures[0]", @is("failure1")).body("validationReport.instructionReports[0].failures[1]", @is("failure2")).when().post(EXECUTE_MIGRATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeMigrationPlanUpdateEventTrigger()
	  public virtual void executeMigrationPlanUpdateEventTrigger()
	  {
		IDictionary<string, object> migrationExecution = (new MigrationExecutionDtoBuilder()).migrationPlan(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID, true).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID, false).done().processInstances(EXAMPLE_PROCESS_INSTANCE_ID, ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationExecution).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MIGRATION_URL);

		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, (IDictionary<string, object>) migrationExecution[MigrationExecutionDtoBuilder.PROP_MIGRATION_PLAN]);
		verifyMigrationPlanExecutionInteraction(migrationExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void validateMigrationPlan()
	  public virtual void validateMigrationPlan()
	  {
		IDictionary<string, object> migrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).instruction(ANOTHER_EXAMPLE_ACTIVITY_ID, EXAMPLE_ACTIVITY_ID, true).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationPlan).then().expect().statusCode(Status.OK.StatusCode).body("instructionReports", hasSize(0)).when().post(VALIDATE_MIGRATION_URL);

		verifyCreateMigrationPlanInteraction(migrationPlanBuilderMock, migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void validateMigrationPlanValidationException()
	  public virtual void validateMigrationPlanValidationException()
	  {
		MigrationInstruction migrationInstruction = mock(typeof(MigrationInstruction));
		when(migrationInstruction.SourceActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(migrationInstruction.TargetActivityId).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);

		MigrationInstructionValidationReport instructionReport1 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport1.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport1.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationInstructionValidationReport instructionReport2 = mock(typeof(MigrationInstructionValidationReport));
		when(instructionReport2.MigrationInstruction).thenReturn(migrationInstruction);
		when(instructionReport2.Failures).thenReturn(Arrays.asList("failure1", "failure2"));

		MigrationPlanValidationReport validationReport = mock(typeof(MigrationPlanValidationReport));
		when(validationReport.InstructionReports).thenReturn(Arrays.asList(instructionReport1, instructionReport2));

		when(migrationPlanBuilderMock.build()).thenThrow(new MigrationPlanValidationException("fooo", validationReport));

		IDictionary<string, object> migrationPlan = (new MigrationPlanDtoBuilder(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).instruction(EXAMPLE_ACTIVITY_ID, ANOTHER_EXAMPLE_ACTIVITY_ID).build();

		given().contentType(POST_JSON_CONTENT_TYPE).body(migrationPlan).then().expect().statusCode(Status.OK.StatusCode).body("instructionReports", hasSize(2)).body("instructionReports[0].instruction.sourceActivityIds", hasSize(1)).body("instructionReports[0].instruction.sourceActivityIds[0]", @is(EXAMPLE_ACTIVITY_ID)).body("instructionReports[0].instruction.targetActivityIds", hasSize(1)).body("instructionReports[0].instruction.targetActivityIds[0]", @is(ANOTHER_EXAMPLE_ACTIVITY_ID)).body("instructionReports[0].failures", hasSize(2)).body("instructionReports[0].failures[0]", @is("failure1")).body("instructionReports[0].failures[1]", @is("failure2")).when().post(VALIDATE_MIGRATION_URL);
	  }

	  protected internal virtual void verifyGenerateMigrationPlanResponse(Response response)
	  {
		string responseContent = response.asString();
		string sourceProcessDefinitionId = from(responseContent).getString("sourceProcessDefinitionId");
		string targetProcessDefinitionId = from(responseContent).getString("targetProcessDefinitionId");
		IList<IDictionary<string, object>> instructions = from(responseContent).getList("instructions");

		assertThat(sourceProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
		assertThat(targetProcessDefinitionId).isEqualTo(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);

		assertThat(instructions).hasSize(2);
		assertThat(instructions[0]).containsEntry("sourceActivityIds", Collections.singletonList(EXAMPLE_ACTIVITY_ID)).containsEntry("targetActivityIds", Collections.singletonList(ANOTHER_EXAMPLE_ACTIVITY_ID)).containsEntry("updateEventTrigger", false);

		assertThat(instructions[1]).containsEntry("sourceActivityIds", Collections.singletonList(ANOTHER_EXAMPLE_ACTIVITY_ID)).containsEntry("targetActivityIds", Collections.singletonList(EXAMPLE_ACTIVITY_ID)).containsEntry("updateEventTrigger", false);
	  }

	  protected internal virtual void verifyGenerateMigrationPlanInteraction(MigrationPlanBuilder migrationPlanBuilderMock, IDictionary<string, object> initialMigrationPlan)
	  {
		verify(runtimeServiceMock).createMigrationPlan(eq(initialMigrationPlan[MigrationPlanDtoBuilder.PROP_SOURCE_PROCESS_DEFINITION_ID].ToString()), eq(initialMigrationPlan[MigrationPlanDtoBuilder.PROP_TARGET_PROCESS_DEFINITION_ID].ToString()));
		// the map equal activities method should be called
		verify(migrationPlanBuilderMock).mapEqualActivities();
		// other instructions are ignored
		verify(migrationPlanBuilderMock, never()).mapActivities(anyString(), anyString());
	  }

	  protected internal virtual void verifyCreateMigrationPlanInteraction(MockMigrationPlanBuilder.JoinedMigrationPlanBuilderMock migrationPlanBuilderMock, IDictionary<string, object> migrationPlan)
	  {
		verify(runtimeServiceMock).createMigrationPlan(migrationPlan[MigrationPlanDtoBuilder.PROP_SOURCE_PROCESS_DEFINITION_ID].ToString(), migrationPlan[MigrationPlanDtoBuilder.PROP_TARGET_PROCESS_DEFINITION_ID].ToString());
		// the map equal activities method should not be called
		verify(migrationPlanBuilderMock, never()).mapEqualActivities();
		// all instructions are added
		IList<IDictionary<string, object>> instructions = (IList<IDictionary<string, object>>) migrationPlan[MigrationPlanDtoBuilder.PROP_INSTRUCTIONS];
		if (instructions != null)
		{
		  foreach (IDictionary<string, object> migrationInstructionDto in instructions)
		  {

			InOrder inOrder = Mockito.inOrder(migrationPlanBuilderMock);
			string sourceActivityId = ((IList<string>) migrationInstructionDto[MigrationInstructionDtoBuilder.PROP_SOURCE_ACTIVITY_IDS])[0];
			string targetActivityId = ((IList<string>) migrationInstructionDto[MigrationInstructionDtoBuilder.PROP_TARGET_ACTIVITY_IDS])[0];
			inOrder.verify(migrationPlanBuilderMock).mapActivities(eq(sourceActivityId), eq(targetActivityId));
			bool? updateEventTrigger = (bool?) migrationInstructionDto[MigrationInstructionDtoBuilder.PROP_UPDATE_EVENT_TRIGGER];
			if (true.Equals(updateEventTrigger))
			{
			  inOrder.verify(migrationPlanBuilderMock, immediatelyAfter()).updateEventTrigger();
			}
		  }
		}
	  }

	  protected internal virtual void verifyMigrationPlanExecutionInteraction(IDictionary<string, object> migrationExecution)
	  {
		InOrder inOrder = inOrder(runtimeServiceMock, migrationPlanExecutionBuilderMock);

		inOrder.verify(runtimeServiceMock).newMigration(any(typeof(MigrationPlan)));

		verifyMigrationExecutionBuilderInteraction(inOrder, migrationExecution);
		inOrder.verify(migrationPlanExecutionBuilderMock).execute();

		inOrder.verifyNoMoreInteractions();
	  }

	  protected internal virtual void verifyMigrationPlanAsyncExecutionInteraction(IDictionary<string, object> migrationExecution)
	  {
		InOrder inOrder = inOrder(runtimeServiceMock, migrationPlanExecutionBuilderMock);

		inOrder.verify(runtimeServiceMock).newMigration(any(typeof(MigrationPlan)));

		verifyMigrationExecutionBuilderInteraction(inOrder, migrationExecution);
		inOrder.verify(migrationPlanExecutionBuilderMock).executeAsync();

		Mockito.verifyNoMoreInteractions(migrationPlanExecutionBuilderMock);
	  }

	  protected internal virtual void verifyMigrationExecutionBuilderInteraction(InOrder inOrder, IDictionary<string, object> migrationExecution)
	  {
		IList<string> processInstanceIds = ((IList<string>) migrationExecution[MigrationExecutionDtoBuilder.PROP_PROCESS_INSTANCE_IDS]);

		inOrder.verify(migrationPlanExecutionBuilderMock).processInstanceIds(eq(processInstanceIds));
		ProcessInstanceQueryDto processInstanceQuery = (ProcessInstanceQueryDto) migrationExecution[MigrationExecutionDtoBuilder.PROP_PROCESS_INSTANCE_QUERY];
		if (processInstanceQuery != null)
		{
		  verifyMigrationPlanExecutionProcessInstanceQuery(inOrder);
		}
		bool? skipCustomListeners = (bool?) migrationExecution[MigrationExecutionDtoBuilder.PROP_SKIP_CUSTOM_LISTENERS];
		if (true.Equals(skipCustomListeners))
		{
		  inOrder.verify(migrationPlanExecutionBuilderMock).skipCustomListeners();
		}
		bool? skipIoMappings = (bool?) migrationExecution[MigrationExecutionDtoBuilder.PROP_SKIP_IO_MAPPINGS];
		if (true.Equals(skipIoMappings))
		{
		  inOrder.verify(migrationPlanExecutionBuilderMock).skipIoMappings();
		}
	  }

	  protected internal virtual void verifyMigrationPlanExecutionProcessInstanceQuery(InOrder inOrder)
	  {
		ArgumentCaptor<ProcessInstanceQuery> queryCapture = ArgumentCaptor.forClass(typeof(ProcessInstanceQuery));
		inOrder.verify(migrationPlanExecutionBuilderMock).processInstanceQuery(queryCapture.capture());

		ProcessInstanceQueryImpl actualQuery = (ProcessInstanceQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

	}

}