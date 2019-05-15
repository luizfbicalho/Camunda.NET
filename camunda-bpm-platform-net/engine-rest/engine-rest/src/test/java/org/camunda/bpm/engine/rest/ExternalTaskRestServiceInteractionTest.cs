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
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyLong;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyMapOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;



	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ExternalTaskQueryTopicBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using UpdateExternalTaskRetriesBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesBuilder;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using ExternalTaskQueryImpl = org.camunda.bpm.engine.impl.ExternalTaskQueryImpl;
	using HistoricProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl;
	using HistoryServiceImpl = org.camunda.bpm.engine.impl.HistoryServiceImpl;
	using ProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using ExternalTaskQueryDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskQueryDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsVariableMap = org.camunda.bpm.engine.rest.helper.EqualsVariableMap;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string EXTERNAL_TASK_URL = TEST_RESOURCE_ROOT_PATH + "/external-task";
	  protected internal static readonly string FETCH_EXTERNAL_TASK_URL = EXTERNAL_TASK_URL + "/fetchAndLock";
	  protected internal static readonly string SINGLE_EXTERNAL_TASK_URL = EXTERNAL_TASK_URL + "/{id}";
	  protected internal static readonly string COMPLETE_EXTERNAL_TASK_URL = SINGLE_EXTERNAL_TASK_URL + "/complete";
	  protected internal static readonly string GET_EXTERNAL_TASK_ERROR_DETAILS_URL = SINGLE_EXTERNAL_TASK_URL + "/errorDetails";
	  protected internal static readonly string HANDLE_EXTERNAL_TASK_FAILURE_URL = SINGLE_EXTERNAL_TASK_URL + "/failure";
	  protected internal static readonly string HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL = SINGLE_EXTERNAL_TASK_URL + "/bpmnError";
	  protected internal static readonly string UNLOCK_EXTERNAL_TASK_URL = SINGLE_EXTERNAL_TASK_URL + "/unlock";
	  protected internal static readonly string RETRIES_EXTERNAL_TASK_URL = SINGLE_EXTERNAL_TASK_URL + "/retries";
	  protected internal static readonly string RETRIES_EXTERNAL_TASK_SYNC_URL = EXTERNAL_TASK_URL + "/retries";
	  protected internal static readonly string RETRIES_EXTERNAL_TASKS_ASYNC_URL = EXTERNAL_TASK_URL + "/retries-async";
	  protected internal static readonly string PRIORITY_EXTERNAL_TASK_URL = SINGLE_EXTERNAL_TASK_URL + "/priority";
	  protected internal static readonly string EXTEND_LOCK_ON_EXTERNAL_TASK = SINGLE_EXTERNAL_TASK_URL + "/extendLock";


	  protected internal ExternalTaskService externalTaskService;
	  protected internal RuntimeServiceImpl runtimeServiceMock;
	  protected internal HistoryServiceImpl historyServiceMock;

	  protected internal LockedExternalTask lockedExternalTaskMock;
	  protected internal ExternalTaskQueryTopicBuilder fetchTopicBuilder;

	  protected internal ExternalTask externalTaskMock;
	  protected internal ExternalTaskQuery externalTaskQueryMock;

	  protected internal UpdateExternalTaskRetriesBuilder updateRetriesBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		externalTaskService = mock(typeof(ExternalTaskService));
		when(processEngine.ExternalTaskService).thenReturn(externalTaskService);

		runtimeServiceMock = mock(typeof(RuntimeServiceImpl));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		historyServiceMock = mock(typeof(HistoryServiceImpl));
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

		// locked external task
		lockedExternalTaskMock = MockProvider.createMockLockedExternalTask();

		// fetching
		fetchTopicBuilder = mock(typeof(ExternalTaskQueryTopicBuilder));
		when(externalTaskService.fetchAndLock(anyInt(), any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(externalTaskService.fetchAndLock(anyInt(), any(typeof(string)), any(typeof(Boolean)))).thenReturn(fetchTopicBuilder);

		when(fetchTopicBuilder.topic(any(typeof(string)), anyLong())).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.variables(anyListOf(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.variables(any(typeof(string[])))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.enableCustomObjectDeserialization()).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.localVariables()).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.topic(any(typeof(string)), anyLong())).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.businessKey(any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.processDefinitionId(any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.processDefinitionIdIn(any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.processDefinitionKey(any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.processDefinitionKeyIn(any(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.processInstanceVariableEquals(anyMapOf(typeof(string), typeof(object)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.withoutTenantId()).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.tenantIdIn(any(typeof(string)))).thenReturn(fetchTopicBuilder);

		Batch batch = createMockBatch();
		updateRetriesBuilder = mock(typeof(UpdateExternalTaskRetriesBuilder));
		when(externalTaskService.updateRetries()).thenReturn(updateRetriesBuilder);

		when(updateRetriesBuilder.externalTaskIds(anyListOf(typeof(string)))).thenReturn(updateRetriesBuilder);
		when(updateRetriesBuilder.processInstanceIds(anyListOf(typeof(string)))).thenReturn(updateRetriesBuilder);
		when(updateRetriesBuilder.externalTaskQuery(any(typeof(ExternalTaskQuery)))).thenReturn(updateRetriesBuilder);
		when(updateRetriesBuilder.processInstanceQuery(any(typeof(ProcessInstanceQuery)))).thenReturn(updateRetriesBuilder);
		when(updateRetriesBuilder.historicProcessInstanceQuery(any(typeof(HistoricProcessInstanceQuery)))).thenReturn(updateRetriesBuilder);
		when(updateRetriesBuilder.setAsync(anyInt())).thenReturn(batch);

		// querying
		externalTaskQueryMock = mock(typeof(ExternalTaskQuery));
		when(externalTaskQueryMock.externalTaskId(any(typeof(string)))).thenReturn(externalTaskQueryMock);
		when(externalTaskService.createExternalTaskQuery()).thenReturn(externalTaskQueryMock);

		// external task
		externalTaskMock = MockProvider.createMockExternalTask();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLock()
	  public virtual void testFetchAndLock()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";
		parameters["usePriority"] = true;

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["lockDuration"] = 12354L;
		topicParameter["variables"] = Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);
		parameters["topics"] = Arrays.asList(topicParameter);

		executePost(parameters);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).variables(Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithBusinessKey()
	  public virtual void testFetchAndLockWithBusinessKey()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";
		parameters["usePriority"] = true;

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["businessKey"] = EXAMPLE_BUSINESS_KEY;
		topicParameter["lockDuration"] = 12354L;
		topicParameter["variables"] = Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);
		parameters["topics"] = Arrays.asList(topicParameter);

		executePost(parameters);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).businessKey(EXAMPLE_BUSINESS_KEY);
		inOrder.verify(fetchTopicBuilder).variables(Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithProcessDefinition()
	  public virtual void testFetchAndLockWithProcessDefinition()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";
		parameters["usePriority"] = true;

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["processDefinitionId"] = EXAMPLE_PROCESS_DEFINITION_ID;
		topicParameter["processDefinitionIdIn"] = Arrays.asList(EXAMPLE_PROCESS_DEFINITION_ID);
		topicParameter["processDefinitionKey"] = EXAMPLE_PROCESS_DEFINITION_KEY;
		topicParameter["processDefinitionKeyIn"] = Arrays.asList(EXAMPLE_PROCESS_DEFINITION_KEY);
		topicParameter["lockDuration"] = 12354L;
		parameters["topics"] = Arrays.asList(topicParameter);

		executePost(parameters);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID);
		inOrder.verify(fetchTopicBuilder).processDefinitionIdIn(EXAMPLE_PROCESS_DEFINITION_ID);
		inOrder.verify(fetchTopicBuilder).processDefinitionKey(EXAMPLE_PROCESS_DEFINITION_KEY);
		inOrder.verify(fetchTopicBuilder).processDefinitionKeyIn(EXAMPLE_PROCESS_DEFINITION_KEY);
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithVariableValue()
	  public virtual void testFetchAndLockWithVariableValue()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";
		parameters["usePriority"] = true;

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["businessKey"] = EXAMPLE_BUSINESS_KEY;
		topicParameter["lockDuration"] = 12354L;
		topicParameter["variables"] = Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);

		IDictionary<string, object> variableValueParameter = new Dictionary<string, object>();
		variableValueParameter[MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME] = MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value;
		topicParameter["processVariables"] = variableValueParameter;

		parameters["topics"] = Arrays.asList(topicParameter);

		executePost(parameters);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).businessKey(EXAMPLE_BUSINESS_KEY);
		inOrder.verify(fetchTopicBuilder).variables(Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).processInstanceVariableEquals(variableValueParameter);
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchWithoutVariables()
	  public virtual void testFetchWithoutVariables()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["lockDuration"] = 12354L;
		parameters["topics"] = Arrays.asList(topicParameter);

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("[0].id", equalTo(MockProvider.EXTERNAL_TASK_ID)).when().post(FETCH_EXTERNAL_TASK_URL);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", false);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithTenant()
	  public virtual void testFetchAndLockWithTenant()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";
		parameters["usePriority"] = true;

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["withoutTenantId"] = true;
		topicParameter["tenantId"] = "tenant1";
		topicParameter["tenantIdIn"] = Arrays.asList("tenant2");
		topicParameter["lockDuration"] = 12354L;
		parameters["topics"] = Arrays.asList(topicParameter);

		executePost(parameters);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).withoutTenantId();
		inOrder.verify(fetchTopicBuilder).tenantIdIn("tenant2");
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEnableCustomObjectDeserialization()
	  public virtual void testEnableCustomObjectDeserialization()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["lockDuration"] = 12354L;
		topicParameter["variables"] = Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);
		topicParameter["deserializeValues"] = true;
		parameters["topics"] = Arrays.asList(topicParameter);

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", false);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).variables(Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).enableCustomObjectDeserialization();
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariables()
	  public virtual void testLocalVariables()
	  {
		// given
		when(fetchTopicBuilder.execute()).thenReturn(Arrays.asList(lockedExternalTaskMock));

		// when
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["maxTasks"] = 5;
		parameters["workerId"] = "aWorkerId";

		IDictionary<string, object> topicParameter = new Dictionary<string, object>();
		topicParameter["topicName"] = "aTopicName";
		topicParameter["lockDuration"] = 12354L;
		topicParameter["variables"] = Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);
		topicParameter["localVariables"] = true;
		parameters["topics"] = Arrays.asList(topicParameter);

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", false);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).variables(Arrays.asList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).localVariables();
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testComplete()
	  public virtual void testComplete()
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["workerId"] = "aWorkerId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(COMPLETE_EXTERNAL_TASK_URL);

		verify(externalTaskService).complete("anExternalTaskId", "aWorkerId", null, null);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithVariables()
	  public virtual void testCompleteWithVariables()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";

		IDictionary<string, object> variables = VariablesBuilder.create().variable("var1", "val1").variable("var2", "val2", "String").variable("var3", ValueType.OBJECT.Name, "val3", "aFormat", "aRootType").Variables;
		parameters["variables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(COMPLETE_EXTERNAL_TASK_URL);

		verify(externalTaskService).complete(eq("anExternalTaskId"), eq("aWorkerId"), argThat(EqualsVariableMap.matches().matcher("var1", EqualsUntypedValue.matcher().value("val1")).matcher("var2", EqualsPrimitiveValue.stringValue("val2")).matcher("var3", EqualsObjectValue.objectValueMatcher().type(ValueType.OBJECT).serializedValue("val3").serializationFormat("aFormat").objectTypeName("aRootType"))), eq((IDictionary<string, object>) null));

		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithLocalVariables()
	  public virtual void testCompleteWithLocalVariables()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";

		IDictionary<string, object> variables = VariablesBuilder.create().variable("var1", "val1").variable("var2", "val2", "String").variable("var3", ValueType.OBJECT.Name, "val3", "aFormat", "aRootType").Variables;
		parameters["localVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(COMPLETE_EXTERNAL_TASK_URL);

		verify(externalTaskService).complete(eq("anExternalTaskId"), eq("aWorkerId"), eq((IDictionary<string, object>) null), argThat(EqualsVariableMap.matches().matcher("var1", EqualsUntypedValue.matcher().value("val1")).matcher("var2", EqualsPrimitiveValue.stringValue("val2")).matcher("var3", EqualsObjectValue.objectValueMatcher().type(ValueType.OBJECT).serializedValue("val3").serializationFormat("aFormat").objectTypeName("aRootType"))));

		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteNonExistingTask()
	  public virtual void testCompleteNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).complete(any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["workerId"] = "aWorkerId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().post(COMPLETE_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteThrowsAuthorizationException()
	  public virtual void testCompleteThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).complete(any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["workerId"] = "aWorkerId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().post(COMPLETE_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteThrowsBadUserRequestException()
	  public virtual void testCompleteThrowsBadUserRequestException()
	  {
		doThrow(new BadUserRequestException("aMessage")).when(externalTaskService).complete(any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["workerId"] = "aWorkerId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("aMessage")).when().post(COMPLETE_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlock()
	  public virtual void testUnlock()
	  {
		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(UNLOCK_EXTERNAL_TASK_URL);

		verify(externalTaskService).unlock("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockNonExistingTask()
	  public virtual void testUnlockNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).unlock(any(typeof(string)));

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().post(UNLOCK_EXTERNAL_TASK_URL);

		verify(externalTaskService).unlock("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockThrowsAuthorizationException()
	  public virtual void testUnlockThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).unlock(any(typeof(string)));

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().post(UNLOCK_EXTERNAL_TASK_URL);

		verify(externalTaskService).unlock("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetails()
	  public virtual void testGetErrorDetails()
	  {
		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().get(GET_EXTERNAL_TASK_ERROR_DETAILS_URL);

		verify(externalTaskService).getExternalTaskErrorDetails("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsNonExistingTask()
	  public virtual void testGetErrorDetailsNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).getExternalTaskErrorDetails(any(typeof(string)));

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().get(GET_EXTERNAL_TASK_ERROR_DETAILS_URL);

		verify(externalTaskService).getExternalTaskErrorDetails("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsThrowsAuthorizationException()
	  public virtual void testGetErrorDetailsThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).getExternalTaskErrorDetails(any(typeof(string)));

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().get(GET_EXTERNAL_TASK_ERROR_DETAILS_URL);

		verify(externalTaskService).getExternalTaskErrorDetails("anExternalTaskId");
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailure()
	  public virtual void testHandleFailure()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorMessage"] = "anErrorMessage";
		parameters["retries"] = 5;
		parameters["retryTimeout"] = 12345;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(HANDLE_EXTERNAL_TASK_FAILURE_URL);

		verify(externalTaskService).handleFailure("anExternalTaskId", "aWorkerId", "anErrorMessage", null, 5, 12345);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithStackTrace()
	  public virtual void testHandleFailureWithStackTrace()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorMessage"] = "anErrorMessage";
		parameters["errorDetails"] = "aStackTrace";
		parameters["retries"] = 5;
		parameters["retryTimeout"] = 12345;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(HANDLE_EXTERNAL_TASK_FAILURE_URL);

		verify(externalTaskService).handleFailure("anExternalTaskId", "aWorkerId", "anErrorMessage","aStackTrace", 5, 12345);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureNonExistingTask()
	  public virtual void testHandleFailureNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).handleFailure(any(typeof(string)), any(typeof(string)), any(typeof(string)),any(typeof(string)), anyInt(), anyLong());

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorMessage"] = "anErrorMessage";
		parameters["retries"] = 5;
		parameters["retryTimeout"] = 12345;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().post(HANDLE_EXTERNAL_TASK_FAILURE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureThrowsAuthorizationException()
	  public virtual void testHandleFailureThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).handleFailure(any(typeof(string)), any(typeof(string)), any(typeof(string)),any(typeof(string)), anyInt(), anyLong());

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorMessage"] = "anErrorMessage";
		parameters["retries"] = 5;
		parameters["retryTimeout"] = 12345;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().post(HANDLE_EXTERNAL_TASK_FAILURE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureThrowsBadUserRequestException()
	  public virtual void testHandleFailureThrowsBadUserRequestException()
	  {
		doThrow(new BadUserRequestException("aMessage")).when(externalTaskService).handleFailure(any(typeof(string)), any(typeof(string)), any(typeof(string)),any(typeof(string)), anyInt(), anyLong());

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorMessage"] = "anErrorMessage";
		parameters["retries"] = 5;
		parameters["retryTimeout"] = 12345;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("aMessage")).when().post(HANDLE_EXTERNAL_TASK_FAILURE_URL);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBpmnError()
	  public virtual void testHandleBpmnError()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorCode"] = "anErrorCode";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL);

		verify(externalTaskService).handleBpmnError("anExternalTaskId", "aWorkerId", "anErrorCode", null, null);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBpmnErrorWithVariables()
	  public virtual void testHandleBpmnErrorWithVariables()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorCode"] = "anErrorCode";
		parameters["errorMessage"] = "anErrorMessage";
		IDictionary<string, object> variables = VariablesBuilder.create().variable("var1", "val1").variable("var2", "val2", "String").variable("var3", ValueType.OBJECT.Name, "val3", "aFormat", "aRootType").Variables;
		parameters["variables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL);

		verify(externalTaskService).handleBpmnError(eq("anExternalTaskId"), eq("aWorkerId"), eq("anErrorCode"), eq("anErrorMessage"), argThat(EqualsVariableMap.matches().matcher("var1", EqualsUntypedValue.matcher().value("val1")).matcher("var2", EqualsPrimitiveValue.stringValue("val2")).matcher("var3", EqualsObjectValue.objectValueMatcher().type(ValueType.OBJECT).serializedValue("val3").serializationFormat("aFormat").objectTypeName("aRootType"))));
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBpmnErrorNonExistingTask()
	  public virtual void testHandleBpmnErrorNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).handleBpmnError(any(typeof(string)), any(typeof(string)), any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorCode"] = "errorCode";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().post(HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBpmnErrorThrowsAuthorizationException()
	  public virtual void testHandleBpmnErrorThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).handleBpmnError(any(typeof(string)), any(typeof(string)), any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorCode"] = "errorCode";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().post(HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBpmnErrorThrowsBadUserRequestException()
	  public virtual void testHandleBpmnErrorThrowsBadUserRequestException()
	  {
		doThrow(new BadUserRequestException("aMessage")).when(externalTaskService).handleBpmnError(any(typeof(string)), any(typeof(string)), any(typeof(string)), any(typeof(string)), anyMapOf(typeof(string), typeof(object)));

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "aWorkerId";
		parameters["errorCode"] = "errorCode";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("aMessage")).when().post(HANDLE_EXTERNAL_TASK_BPMN_ERROR_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetries()
	  public virtual void testSetRetries()
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["retries"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(RETRIES_EXTERNAL_TASK_URL);

		verify(externalTaskService).setRetries("anExternalTaskId", 5);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesNonExistingTask()
	  public virtual void testSetRetriesNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).setRetries(any(typeof(string)), anyInt());

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["retries"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().put(RETRIES_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesThrowsAuthorizationException()
	  public virtual void testSetRetriesThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).setRetries(any(typeof(string)), anyInt());

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["retries"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().put(RETRIES_EXTERNAL_TASK_URL);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriority()
	  public virtual void testSetPriority()
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["priority"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PRIORITY_EXTERNAL_TASK_URL);

		verify(externalTaskService).setPriority("anExternalTaskId", 5);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityNonExistingTask()
	  public virtual void testSetPriorityNonExistingTask()
	  {
		doThrow(new NotFoundException()).when(externalTaskService).setPriority(any(typeof(string)), anyInt());

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["priority"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().put(PRIORITY_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityThrowsAuthorizationException()
	  public virtual void testSetPriorityThrowsAuthorizationException()
	  {
		doThrow(new AuthorizationException("aMessage")).when(externalTaskService).setPriority(any(typeof(string)), anyInt());

		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["priority"] = "5";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("aMessage")).when().put(PRIORITY_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleExternalTask()
	  public virtual void testGetSingleExternalTask()
	  {
		when(externalTaskQueryMock.singleResult()).thenReturn(externalTaskMock);

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.OK.StatusCode).body("activityId", equalTo(MockProvider.EXAMPLE_ACTIVITY_ID)).body("activityInstanceId", equalTo(MockProvider.EXAMPLE_ACTIVITY_INSTANCE_ID)).body("errorMessage", equalTo(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE)).body("executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("id", equalTo(MockProvider.EXTERNAL_TASK_ID)).body("lockExpirationTime", equalTo(MockProvider.EXTERNAL_TASK_LOCK_EXPIRATION_TIME)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("processDefinitionKey", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("retries", equalTo(MockProvider.EXTERNAL_TASK_RETRIES)).body("suspended", equalTo(MockProvider.EXTERNAL_TASK_SUSPENDED)).body("topicName", equalTo(MockProvider.EXTERNAL_TASK_TOPIC_NAME)).body("workerId", equalTo(MockProvider.EXTERNAL_TASK_WORKER_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("priority", equalTo(MockProvider.EXTERNAL_TASK_PRIORITY)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).when().get(SINGLE_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingExternalTask()
	  public virtual void testGetNonExistingExternalTask()
	  {
		when(externalTaskQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("External task with id anExternalTaskId does not exist")).when().get(SINGLE_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksAsync()
	  public virtual void testSetRetriesForExternalTasksAsync()
	  {
		IList<string> externalTaskIds = Arrays.asList("externalTaskId1", "externalTaskId2");
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["externalTaskIds"] = externalTaskIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds(externalTaskIds);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksSync()
	  public virtual void testSetRetriesForExternalTasksSync()
	  {
		IList<string> externalTaskIds = Arrays.asList("externalTaskId1", "externalTaskId2");
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["externalTaskIds"] = externalTaskIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds(externalTaskIds);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksAsyncByProcessInstanceIds()
	  public virtual void testSetRetriesForExternalTasksAsyncByProcessInstanceIds()
	  {
		IList<string> processInstanceIds = Arrays.asList("123", "456");
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["processInstanceIds"] = processInstanceIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds(processInstanceIds);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksSyncByProcessInstanceIds()
	  public virtual void testSetRetriesForExternalTasksSyncByProcessInstanceIds()
	  {
		IList<string> processInstanceIds = Arrays.asList("123", "456");
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["processInstanceIds"] = processInstanceIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(RETRIES_EXTERNAL_TASK_SYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds(processInstanceIds);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).set(5);
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksWithNullExternalTaskIdsAsync()
	  public virtual void testSetRetriesForExternalTasksWithNullExternalTaskIdsAsync()
	  {
		doThrow(typeof(BadUserRequestException)).when(updateRetriesBuilder).Async = anyInt();

		IList<string> externalTaskIds = null;
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["externalTaskIds"] = externalTaskIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds(externalTaskIds);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNegativeRetriesForExternalTasksAsync()
	  public virtual void testSetNegativeRetriesForExternalTasksAsync()
	  {
		doThrow(typeof(BadUserRequestException)).when(updateRetriesBuilder).Async = anyInt();

		IList<string> externalTaskIds = null;
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "-5";
		parameters["externalTaskIds"] = externalTaskIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds(externalTaskIds);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = -5;
		verifyNoMoreInteractions(updateRetriesBuilder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNullRetriesForExternalTasks()
	  public virtual void testSetNullRetriesForExternalTasks()
	  {
		IList<string> externalTaskIds = null;
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = null;
		parameters["externalTaskIds"] = externalTaskIds;

		// test set retries to null synchronous
		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().put(RETRIES_EXTERNAL_TASK_SYNC_URL);

		verify(updateRetriesBuilder, never()).set(anyInt());

		// test set retries to null asynchronous
		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		verify(updateRetriesBuilder, never()).Async = anyInt();

		// test set retries to null on single task
		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).pathParam("id", "anExternalTaskId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().put(RETRIES_EXTERNAL_TASK_URL);

		verify(externalTaskService, never()).setRetries(anyString(),anyInt());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksAsyncWithProcessInstanceQuery()
	  public virtual void testSetRetriesForExternalTasksAsyncWithProcessInstanceQuery()
	  {
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());

		ProcessInstanceQueryDto processInstanceQuery = new ProcessInstanceQueryDto();
		processInstanceQuery.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["processInstanceQuery"] = processInstanceQuery;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		ArgumentCaptor<ProcessInstanceQuery> queryCapture = ArgumentCaptor.forClass(typeof(ProcessInstanceQuery));

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(queryCapture.capture());
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);

		ProcessInstanceQueryImpl actualQuery = (ProcessInstanceQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesForExternalTasksAsyncWithHistoricProcessInstanceQuery()
	  public virtual void testSetRetriesForExternalTasksAsyncWithHistoricProcessInstanceQuery()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());

		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["historicProcessInstanceQuery"] = query;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		ArgumentCaptor<HistoricProcessInstanceQuery> queryCapture = ArgumentCaptor.forClass(typeof(HistoricProcessInstanceQuery));

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(queryCapture.capture());
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);

		HistoricProcessInstanceQueryImpl actualQuery = (HistoricProcessInstanceQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithProcessInstanceQuery()
	  public virtual void testSetRetriesWithProcessInstanceQuery()
	  {
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(new ProcessInstanceQueryImpl());

		ProcessInstanceQueryDto processInstanceQuery = new ProcessInstanceQueryDto();
		processInstanceQuery.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["processInstanceQuery"] = processInstanceQuery;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(RETRIES_EXTERNAL_TASK_SYNC_URL);

		ArgumentCaptor<ProcessInstanceQuery> queryCapture = ArgumentCaptor.forClass(typeof(ProcessInstanceQuery));

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(queryCapture.capture());
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).set(5);
		verifyNoMoreInteractions(updateRetriesBuilder);

		ProcessInstanceQueryImpl actualQuery = (ProcessInstanceQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithHistoricProcessInstanceQuery()
	  public virtual void testSetRetriesWithHistoricProcessInstanceQuery()
	  {
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(new HistoricProcessInstanceQueryImpl());

		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		query.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["historicProcessInstanceQuery"] = query;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(RETRIES_EXTERNAL_TASK_SYNC_URL);

		ArgumentCaptor<HistoricProcessInstanceQuery> queryCapture = ArgumentCaptor.forClass(typeof(HistoricProcessInstanceQuery));

		verify(externalTaskService).updateRetries();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(null);
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(queryCapture.capture());
		verify(updateRetriesBuilder).set(5);
		verifyNoMoreInteractions(updateRetriesBuilder);

		HistoricProcessInstanceQueryImpl actualQuery = (HistoricProcessInstanceQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesSyncWithExternalTaskQuery()
	  public virtual void testSetRetriesSyncWithExternalTaskQuery()
	  {
		when(externalTaskService.createExternalTaskQuery()).thenReturn(new ExternalTaskQueryImpl());

		ExternalTaskQueryDto query = new ExternalTaskQueryDto();
		query.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["externalTaskQuery"] = query;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(RETRIES_EXTERNAL_TASK_SYNC_URL);

		ArgumentCaptor<ExternalTaskQuery> queryCapture = ArgumentCaptor.forClass(typeof(ExternalTaskQuery));

		verify(externalTaskService).updateRetries();
		verify(externalTaskService).createExternalTaskQuery();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(queryCapture.capture());
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).set(5);
		verifyNoMoreInteractions(updateRetriesBuilder);

		ExternalTaskQueryImpl actualQuery = (ExternalTaskQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesAsyncWithExternalTaskQuery()
	  public virtual void testSetRetriesAsyncWithExternalTaskQuery()
	  {
		when(externalTaskService.createExternalTaskQuery()).thenReturn(new ExternalTaskQueryImpl());

		ExternalTaskQueryDto query = new ExternalTaskQueryDto();
		query.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["retries"] = "5";
		parameters["externalTaskQuery"] = query;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(RETRIES_EXTERNAL_TASKS_ASYNC_URL);

		ArgumentCaptor<ExternalTaskQuery> queryCapture = ArgumentCaptor.forClass(typeof(ExternalTaskQuery));

		verify(externalTaskService).updateRetries();
		verify(externalTaskService).createExternalTaskQuery();
		verifyNoMoreInteractions(externalTaskService);

		verify(updateRetriesBuilder).externalTaskIds((IList<string>) null);
		verify(updateRetriesBuilder).processInstanceIds((IList<string>) null);
		verify(updateRetriesBuilder).externalTaskQuery(queryCapture.capture());
		verify(updateRetriesBuilder).processInstanceQuery(null);
		verify(updateRetriesBuilder).historicProcessInstanceQuery(null);
		verify(updateRetriesBuilder).Async = 5;
		verifyNoMoreInteractions(updateRetriesBuilder);

		ExternalTaskQueryImpl actualQuery = (ExternalTaskQueryImpl) queryCapture.Value;
		assertThat(actualQuery).NotNull;
		assertThat(actualQuery.ProcessDefinitionId).isEqualTo(EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtendLockOnExternalTask()
	  public virtual void testExtendLockOnExternalTask()
	  {

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "workerId";
		parameters["newDuration"] = "1000";

		given().pathParam("id", MockProvider.EXTERNAL_TASK_ID).contentType(ContentType.JSON).body(parameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXTEND_LOCK_ON_EXTERNAL_TASK);

		verify(externalTaskService).extendLock(MockProvider.EXTERNAL_TASK_ID, "workerId", 1000);
		verifyNoMoreInteractions(externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtendLockOnExternalTaskFailed()
	  public virtual void testExtendLockOnExternalTaskFailed()
	  {

		doThrow(typeof(BadUserRequestException)).when(externalTaskService).extendLock(anyString(), anyString(), anyLong());
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["workerId"] = "workerId";
		parameters["newDuration"] = -1;

		given().pathParam("id", MockProvider.EXTERNAL_TASK_ID).contentType(ContentType.JSON).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXTEND_LOCK_ON_EXTERNAL_TASK);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtendLockOnUnexistingExternalTask()
	  public virtual void testExtendLockOnUnexistingExternalTask()
	  {
		doThrow(typeof(NotFoundException)).when(externalTaskService).extendLock(anyString(), anyString(), anyLong());

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["workerId"] = "workerId";
		json["newDuration"] = 1000;

		given().pathParam("id", MockProvider.EXTERNAL_TASK_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().post(EXTEND_LOCK_ON_EXTERNAL_TASK);
	  }

	  protected internal virtual void executePost(IDictionary<string, object> parameters)
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("[0].id", equalTo(MockProvider.EXTERNAL_TASK_ID)).body("[0].topicName", equalTo(MockProvider.EXTERNAL_TASK_TOPIC_NAME)).body("[0].workerId", equalTo(MockProvider.EXTERNAL_TASK_WORKER_ID)).body("[0].lockExpirationTime", equalTo(MockProvider.EXTERNAL_TASK_LOCK_EXPIRATION_TIME)).body("[0].processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("[0].executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("[0].activityId", equalTo(MockProvider.EXAMPLE_ACTIVITY_ID)).body("[0].activityInstanceId", equalTo(MockProvider.EXAMPLE_ACTIVITY_INSTANCE_ID)).body("[0].processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("[0].processDefinitionKey", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("[0].tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("[0].retries", equalTo(MockProvider.EXTERNAL_TASK_RETRIES)).body("[0].errorMessage", equalTo(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE)).body("[0].errorMessage", equalTo(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE)).body("[0].priority", equalTo(MockProvider.EXTERNAL_TASK_PRIORITY)).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME, notNullValue()).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("String")).when().post(FETCH_EXTERNAL_TASK_URL);
	  }

	}

}