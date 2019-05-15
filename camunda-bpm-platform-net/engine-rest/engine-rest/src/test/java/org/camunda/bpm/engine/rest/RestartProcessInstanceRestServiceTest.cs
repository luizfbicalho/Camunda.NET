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
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockBatch;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	using ContentType = io.restassured.http.ContentType;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ModificationInstructionBuilder = org.camunda.bpm.engine.rest.util.ModificationInstructionBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	public class RestartProcessInstanceRestServiceTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_URL = PROCESS_DEFINITION_URL + "/{id}";
	  protected internal static readonly string RESTART_PROCESS_INSTANCE_URL = SINGLE_PROCESS_DEFINITION_URL + "/restart";
	  protected internal static readonly string RESTART_PROCESS_INSTANCE_ASYNC_URL = SINGLE_PROCESS_DEFINITION_URL + "/restart-async";

	  internal RuntimeService runtimeServiceMock;
	  internal HistoryService historyServiceMock;
	  internal RestartProcessInstanceBuilder builderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		historyServiceMock = mock(typeof(HistoryService));
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

		builderMock = mock(typeof(RestartProcessInstanceBuilder));
		when(builderMock.startAfterActivity(anyString())).thenReturn(builderMock);
		when(builderMock.startBeforeActivity(anyString())).thenReturn(builderMock);
		when(builderMock.startTransition(anyString())).thenReturn(builderMock);
		when(builderMock.processInstanceIds(anyListOf(typeof(string)))).thenReturn(builderMock);
		when(builderMock.historicProcessInstanceQuery(any(typeof(HistoricProcessInstanceQuery)))).thenReturn(builderMock);
		when(builderMock.skipCustomListeners()).thenReturn(builderMock);
		when(builderMock.skipIoMappings()).thenReturn(builderMock);
		when(builderMock.initialSetOfVariables()).thenReturn(builderMock);
		when(builderMock.withoutBusinessKey()).thenReturn(builderMock);

		Batch batchMock = createMockBatch();
		when(builderMock.executeAsync()).thenReturn(batchMock);

		when(runtimeServiceMock.restartProcessInstances(anyString())).thenReturn(builderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceSync()
	  public virtual void testRestartProcessInstanceSync()
	  {

		Dictionary<string, object> json = new Dictionary<string, object>();
		List<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["processInstanceIds"] = Arrays.asList("processInstanceId1", "processInstanceId2");

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).restartProcessInstances(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(builderMock).startAfterActivity("activityId");
		verify(builderMock).processInstanceIds(Arrays.asList("processInstanceId1", "processInstanceId2"));
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceAsync()
	  public virtual void testRestartProcessInstanceAsync()
	  {
		Dictionary<string, object> json = new Dictionary<string, object>();
		List<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["processInstanceIds"] = Arrays.asList("processInstanceId1", "processInstanceId2");

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(runtimeServiceMock).restartProcessInstances(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(builderMock).startAfterActivity("activityId");
		verify(builderMock).processInstanceIds(Arrays.asList("processInstanceId1", "processInstanceId2"));
		verify(builderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithNullProcessInstanceIdsSync()
	  public virtual void testRestartProcessInstanceWithNullProcessInstanceIdsSync()
	  {
		doThrow(new BadUserRequestException("processInstanceIds is null")).when(builderMock).execute();

		Dictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithNullProcessInstanceIdsAsync()
	  public virtual void testRestartProcessInstanceWithNullProcessInstanceIdsAsync()
	  {
		doThrow(new BadUserRequestException("processInstanceIds is null")).when(builderMock).executeAsync();

		Dictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithHistoricProcessInstanceQuerySync()
	  public virtual void testRestartProcessInstanceWithHistoricProcessInstanceQuerySync()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());
		Dictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.ProcessInstanceBusinessKey = "businessKey";

		json["historicProcessInstanceQuery"] = query;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).restartProcessInstances(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).historicProcessInstanceQuery(query.toQuery(processEngine));
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithHistoricProcessInstanceQueryAsync()
	  public virtual void testRestartProcessInstanceWithHistoricProcessInstanceQueryAsync()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());
		Dictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.ProcessInstanceBusinessKey = "businessKey";

		json["historicProcessInstanceQuery"] = query;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(runtimeServiceMock).restartProcessInstances(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).historicProcessInstanceQuery(query.toQuery(processEngine));
		verify(builderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithNullInstructionsSync()
	  public virtual void testRestartProcessInstanceWithNullInstructionsSync()
	  {
		doThrow(new BadUserRequestException("instructions is null")).when(builderMock).execute();

		Dictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIds"] = "processInstanceId";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithNullInstructionsAsync()
	  public virtual void testRestartProcessInstanceWithNullInstructionsAsync()
	  {
		doThrow(new BadUserRequestException("instructions is null")).when(builderMock).executeAsync();

		Dictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIds"] = "processInstanceId";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartAfterSync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartAfterSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startAfterActivity': 'activityId' must be set")).when().post(RESTART_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartAfterAsync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartAfterAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startAfterActivity': 'activityId' must be set")).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartBeforeSync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartBeforeSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startBeforeActivity': 'activityId' must be set")).when().post(RESTART_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartBeforeAsync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartBeforeAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startBeforeActivity': 'activityId' must be set")).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartTransitionSync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartTransitionSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startTransition': 'transitionId' must be set")).when().post(RESTART_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInvalidModificationInstructionForStartTransitionAsync()
	  public virtual void testRestartProcessInstanceWithInvalidModificationInstructionForStartTransitionAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startTransition': 'transitionId' must be set")).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInitialVariablesAsync()
	  public virtual void testRestartProcessInstanceWithInitialVariablesAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["initialVariables"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).initialSetOfVariables();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithInitialVariablesSync()
	  public virtual void testRestartProcessInstanceWithInitialVariablesSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["initialVariables"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).initialSetOfVariables();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithSkipCustomListenersAsync()
	  public virtual void testRestartProcessInstanceWithSkipCustomListenersAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["skipCustomListeners"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).skipCustomListeners();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithSkipCustomListenersSync()
	  public virtual void testRestartProcessInstanceWithSkipCustomListenersSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["skipCustomListeners"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).skipCustomListeners();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithSkipIoMappingsAsync()
	  public virtual void testRestartProcessInstanceWithSkipIoMappingsAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["skipIoMappings"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).skipIoMappings();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithSkipIoMappingsSync()
	  public virtual void testRestartProcessInstanceWithSkipIoMappingsSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["skipIoMappings"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).skipIoMappings();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithoutBusinessKey()
	  public virtual void testRestartProcessInstanceWithoutBusinessKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["processInstanceIds"] = Arrays.asList("processInstance1", "processInstance2");
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;
		json["withoutBusinessKey"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(builderMock).processInstanceIds(Arrays.asList("processInstance1", "processInstance2"));
		verify(builderMock).withoutBusinessKey();
		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithoutProcessInstanceIdsSync()
	  public virtual void testRestartProcessInstanceWithoutProcessInstanceIdsSync()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.Finished = true;
		json["historicProcessInstanceQuery"] = query;
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESTART_PROCESS_INSTANCE_URL);

		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).historicProcessInstanceQuery(query.toQuery(processEngine));
		verify(builderMock).execute();
		verifyNoMoreInteractions(builderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestartProcessInstanceWithoutProcessInstanceIdsAsync()
	  public virtual void testRestartProcessInstanceWithoutProcessInstanceIdsAsync()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.Finished = true;
		json["historicProcessInstanceQuery"] = query;
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(RESTART_PROCESS_INSTANCE_ASYNC_URL);

		verify(builderMock).startBeforeActivity("activityId");
		verify(builderMock).historicProcessInstanceQuery(query.toQuery(processEngine));
		verify(builderMock).executeAsync();
		verifyNoMoreInteractions(builderMock);
	  }
	}

}