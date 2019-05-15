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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ModificationInstructionBuilder = org.camunda.bpm.engine.rest.util.ModificationInstructionBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ModificationBuilder = org.camunda.bpm.engine.runtime.ModificationBuilder;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public class ModificationRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/modification";
	  protected internal static readonly string EXECUTE_MODIFICATION_SYNC_URL = PROCESS_INSTANCE_URL + "/execute";
	  protected internal static readonly string EXECUTE_MODIFICATION_ASYNC_URL = PROCESS_INSTANCE_URL + "/executeAsync";

	  protected internal RuntimeService runtimeServiceMock;
	  protected internal ModificationBuilder modificationBuilderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		modificationBuilderMock = mock(typeof(ModificationBuilder));
		when(modificationBuilderMock.cancelAllForActivity(anyString())).thenReturn(modificationBuilderMock);
		when(modificationBuilderMock.startAfterActivity(anyString())).thenReturn(modificationBuilderMock);
		when(modificationBuilderMock.startBeforeActivity(anyString())).thenReturn(modificationBuilderMock);
		when(modificationBuilderMock.startTransition(anyString())).thenReturn(modificationBuilderMock);
		when(modificationBuilderMock.processInstanceIds(anyListOf(typeof(string)))).thenReturn(modificationBuilderMock);

		Batch batchMock = createMockBatch();
		when(modificationBuilderMock.executeAsync()).thenReturn(batchMock);

		when(runtimeServiceMock.createModification(anyString())).thenReturn(modificationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationSync()
	  public virtual void executeModificationSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["skipCustomListeners"] = true;
		json["skipIoMappings"] = true;
		json["processDefinitionId"] = "processDefinitionId";
		json["processInstanceIds"] = Arrays.asList("100", "20");
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);

		json["instructions"] = instructions;

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

		verify(runtimeServiceMock).createModification("processDefinitionId");
		verify(modificationBuilderMock).processInstanceIds(eq(Arrays.asList("100", "20")));
		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).startBeforeActivity("activityId");
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).startTransition("transitionId");
		verify(modificationBuilderMock).skipCustomListeners();
		verify(modificationBuilderMock).skipIoMappings();
		verify(modificationBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullProcessDefinitionIdAsync()
	  public virtual void executeModificationWithNullProcessDefinitionIdAsync()
	  {
		doThrow(new BadUserRequestException("processDefinitionId must be set")).when(modificationBuilderMock).executeAsync();

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["skipCustomListeners"] = true;
		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("100", "20");
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);

		json["instructions"] = instructions;

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);

		verify(runtimeServiceMock).createModification(null);
		verify(modificationBuilderMock).processInstanceIds(eq(Arrays.asList("100", "20")));
		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).startBeforeActivity("activityId");
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).startTransition("transitionId");
		verify(modificationBuilderMock).skipCustomListeners();
		verify(modificationBuilderMock).skipIoMappings();
		verify(modificationBuilderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullProcessDefinitionIdSync()
	  public virtual void executeModificationWithNullProcessDefinitionIdSync()
	  {
		doThrow(new BadUserRequestException("processDefinitionId must be set")).when(modificationBuilderMock).execute();

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["skipCustomListeners"] = true;
		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("100", "20");
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);

		json["instructions"] = instructions;

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

		verify(runtimeServiceMock).createModification(null);
		verify(modificationBuilderMock).processInstanceIds(eq(Arrays.asList("100", "20")));
		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).startBeforeActivity("activityId");
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).startTransition("transitionId");
		verify(modificationBuilderMock).skipCustomListeners();
		verify(modificationBuilderMock).skipIoMappings();
		verify(modificationBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullProcessInstanceIdsSync()
	  public virtual void executeModificationWithNullProcessInstanceIdsSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		string message = "Process instance ids is null";
		doThrow(new BadUserRequestException(message)).when(modificationBuilderMock).execute();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId(EXAMPLE_ACTIVITY_ID).Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);
		json["processDefinitionId"] = "processDefinitionId";
		json["instructions"] = instructions;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationAsync()
	  public virtual void executeModificationAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		json["processDefinitionId"] = "processDefinitionId";
		json["instructions"] = instructions;
		json["processInstanceIds"] = Arrays.asList("100", "20");

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);

		verify(runtimeServiceMock).createModification("processDefinitionId");
		verify(modificationBuilderMock).processInstanceIds(eq(Arrays.asList("100", "20")));
		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).startBeforeActivity("activityId");
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).startTransition("transitionId");
		verify(modificationBuilderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullProcessInstanceIdsAsync()
	  public virtual void executeModificationWithNullProcessInstanceIdsAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		string message = "Process instance ids is null";
		doThrow(new BadUserRequestException(message)).when(modificationBuilderMock).executeAsync();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId(EXAMPLE_ACTIVITY_ID).Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);

		json["instructions"] = instructions;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", @is(message)).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithValidProcessInstanceQuerySync()
	  public virtual void executeModificationWithValidProcessInstanceQuerySync()
	  {

		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		json["processDefinitionId"] = "processDefinitionId";

		ProcessInstanceQueryDto processInstanceQueryDto = new ProcessInstanceQueryDto();
		processInstanceQueryDto.BusinessKey = "foo";

		json["processInstanceQuery"] = processInstanceQueryDto;
		json["instructions"] = instructions;

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

		verify(runtimeServiceMock, times(1)).createProcessInstanceQuery();
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).processInstanceQuery(processInstanceQueryDto.toQuery(processEngine));
		verify(modificationBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithValidProcessInstanceQueryAsync()
	  public virtual void executeModificationWithValidProcessInstanceQueryAsync()
	  {

		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);

		ProcessInstanceQueryDto processInstanceQueryDto = new ProcessInstanceQueryDto();
		processInstanceQueryDto.BusinessKey = "foo";

		json["processInstanceQuery"] = processInstanceQueryDto;
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);

		verify(runtimeServiceMock, times(1)).createProcessInstanceQuery();
		verify(modificationBuilderMock).startAfterActivity("activityId");
		verify(modificationBuilderMock).processInstanceQuery(processInstanceQueryDto.toQuery(processEngine));
		verify(modificationBuilderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithInvalidProcessInstanceQuerySync()
	  public virtual void executeModificationWithInvalidProcessInstanceQuerySync()
	  {

		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();

		string message = "Process instance ids is null";
		doThrow(new BadUserRequestException(message)).when(modificationBuilderMock).execute();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("acivityId").Json);

		ProcessInstanceQueryDto processInstanceQueryDto = new ProcessInstanceQueryDto();
		processInstanceQueryDto.BusinessKey = "foo";
		json["processInstanceQuery"] = processInstanceQueryDto;
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithInvalidProcessInstanceQueryAsync()
	  public virtual void executeModificationWithInvalidProcessInstanceQueryAsync()
	  {

		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("acivityId").Json);

		ProcessInstanceQueryDto processInstanceQueryDto = new ProcessInstanceQueryDto();
		processInstanceQueryDto.BusinessKey = "foo";
		json["processInstanceQuery"] = processInstanceQueryDto;
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullInstructionsSync()
	  public virtual void executeModificationWithNullInstructionsSync()
	  {
		doThrow(new BadUserRequestException("Instructions must be set")).when(modificationBuilderMock).execute();

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIds"] = Arrays.asList("200", "11");
		json["skipIoMappings"] = true;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Instructions must be set")).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationWithNullInstructionsAsync()
	  public virtual void executeModificationWithNullInstructionsAsync()
	  {
		doThrow(new BadUserRequestException("Instructions must be set")).when(modificationBuilderMock).executeAsync();
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIds"] = Arrays.asList("200", "11");
		json["skipIoMappings"] = true;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Instructions must be set")).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModificationThrowsAuthorizationException()
	  public virtual void executeModificationThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(modificationBuilderMock).executeAsync();

		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);

		json["instructions"] = instructions;
		json["processInstanceIds"] = Arrays.asList("200", "323");
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartAfterSync()
	  public virtual void executeInvalidModificationForStartAfterSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startAfterActivity': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartAfterAsync()
	  public virtual void executeInvalidModificationForStartAfterAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startAfterActivity': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartBeforeSync()
	  public virtual void executeInvalidModificationForStartBeforeSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startBeforeActivity': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartBeforeAsync()
	  public virtual void executeInvalidModificationForStartBeforeAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startBeforeActivity': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartTransitionSync()
	  public virtual void executeInvalidModificationForStartTransitionSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startTransition': 'transitionId' must be set")).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForStartTransitionAsync()
	  public virtual void executeInvalidModificationForStartTransitionAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'startTransition': 'transitionId' must be set")).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForCancelAllSync()
	  public virtual void executeInvalidModificationForCancelAllSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'cancel': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_SYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeInvalidModificationForCancelAllAsync()
	  public virtual void executeInvalidModificationForCancelAllAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("For instruction type 'cancel': 'activityId' must be set")).when().post(EXECUTE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeCancellationWithActiveFlagSync()
	  public virtual void executeCancellationWithActiveFlagSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").cancelCurrentActiveActivityInstances(true).Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

		verify(modificationBuilderMock).cancelAllForActivity("activityId", true);
		verify(modificationBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeCancellationWithActiveFlagAsync()
	  public virtual void executeCancellationWithActiveFlagAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").cancelCurrentActiveActivityInstances(true).Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);

		verify(modificationBuilderMock).cancelAllForActivity("activityId", true);
		verify(modificationBuilderMock).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeCancellationWithoutActiveFlagSync()
	  public virtual void executeCancellationWithoutActiveFlagSync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").cancelCurrentActiveActivityInstances(false).Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTE_MODIFICATION_SYNC_URL);

		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeCancellationWithoutActiveFlagAsync()
	  public virtual void executeCancellationWithoutActiveFlagAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		json["skipIoMappings"] = true;
		json["processInstanceIds"] = Arrays.asList("200", "100");
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").cancelCurrentActiveActivityInstances(false).Json);
		json["instructions"] = instructions;
		json["processDefinitionId"] = "processDefinitionId";

		given().contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_MODIFICATION_ASYNC_URL);

		verify(modificationBuilderMock).cancelAllForActivity("activityId");
		verify(modificationBuilderMock).executeAsync();
	  }
	}

}