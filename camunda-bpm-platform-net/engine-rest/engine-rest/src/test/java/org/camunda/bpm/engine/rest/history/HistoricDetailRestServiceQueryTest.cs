using System;
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
namespace org.camunda.bpm.engine.rest.history
{
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockHistoricVariableUpdateBuilder = org.camunda.bpm.engine.rest.helper.MockHistoricVariableUpdateBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.reset;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Nikola Koevski
	/// </summary>
	public class HistoricDetailRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_DETAIL_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/detail";

	  protected internal static readonly string HISTORIC_DETAIL_COUNT_RESOURCE_URL = HISTORIC_DETAIL_RESOURCE_URL + "/count";

	  protected internal HistoricDetailQuery mockedQuery;

	  protected internal HistoricVariableUpdate historicUpdateMock;
	  protected internal MockHistoricVariableUpdateBuilder historicUpdateBuilder;

	  protected internal HistoricFormField historicFormFieldMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		IList<HistoricDetail> details = new List<HistoricDetail>();

		historicUpdateBuilder = MockProvider.mockHistoricVariableUpdate();
		historicUpdateMock = historicUpdateBuilder.build();
		historicFormFieldMock = MockProvider.createMockHistoricFormField();

		details.Add(historicUpdateMock);
		details.Add(historicFormFieldMock);

		mockedQuery = setUpMockedDetailsQuery(details);
	  }

	  protected internal virtual HistoricDetailQuery setUpMockedDetailsQuery(IList<HistoricDetail> detailMocks)
	  {
		HistoricDetailQuery mock = mock(typeof(HistoricDetailQuery));
		when(mock.list()).thenReturn(detailMocks);
		when(mock.count()).thenReturn((long) detailMocks.Count);

		when(processEngine.HistoryService.createHistoricDetailQuery()).thenReturn(mock);
		return mock;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		// GET
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verifyNoMoreInteractions(mockedQuery);

		reset(mockedQuery);

		// POST
		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryDisableObjectDeserialization()
	  public virtual void testNoParametersQueryDisableObjectDeserialization()
	  {
		// GET
		given().queryParam("deserializeValues", false).expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);

		reset(mockedQuery);

		// POST
		given().queryParam("deserializeValues", false).contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySortingAsPost(OrderingBuilder.create().orderBy("processInstanceId").desc().orderBy("variableName").asc().orderBy("variableType").desc().orderBy("variableRevision").asc().orderBy("formPropertyId").desc().orderBy("time").asc().orderBy("occurrence").desc().orderBy("tenantId").asc().Json, Status.OK);

		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).asc();
		inOrder.verify(mockedQuery).orderByVariableType();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByVariableRevision();
		inOrder.verify(mockedQuery).asc();
		inOrder.verify(mockedQuery).orderByFormPropertyId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByTime();
		inOrder.verify(mockedQuery).asc();
		inOrder.verify(mockedQuery).orderPartiallyByOccurrence();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("processInstanceId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSecondarySortingOptions()
	  public virtual void testInvalidSecondarySortingOptions()
	  {
		executeAndVerifySortingAsPost(OrderingBuilder.create().orderBy("processInstanceId").desc().orderBy("invalidParameter").asc().Json, Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingSecondarySortingOptions()
	  public virtual void testMissingSecondarySortingOptions()
	  {
		executeAndVerifySortingAsPost(OrderingBuilder.create().orderBy("processInstanceId").desc().orderBy("variableName").Json, Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);
	  }

	  protected internal virtual void executeAndVerifySortingAsPost(IList<IDictionary<string, object>> sortingJson, Status expectedStatus)
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = sortingJson;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(json).then().expect().statusCode(expectedStatus.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_DETAIL_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("formPropertyId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByFormPropertyId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("formPropertyId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByFormPropertyId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableType();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableRevision", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableRevision();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableRevision", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableRevision();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("time", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("time", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("occurrence", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderPartiallyByOccurrence();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("occurrence", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderPartiallyByOccurrence();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		// GET
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);

		reset(mockedQuery);

		// POST
		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		// GET
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);

		reset(mockedQuery);

		// POST
		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).queryParam("maxResults", maxResults).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		// GET
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);

		reset(mockedQuery);

		// POST
		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).queryParam("firstResult", firstResult).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_DETAIL_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricActivityQuery()
	  public virtual void testSimpleHistoricActivityQuery()
	  {
		// GET
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).and().body("[0].id", equalTo(historicUpdateBuilder.Id)).body("[0].variableName", equalTo(historicUpdateBuilder.Name)).body("[0].variableInstanceId", equalTo(historicUpdateBuilder.VariableInstanceId)).body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(historicUpdateBuilder.TypedValue.Type))).body("[0].value", equalTo(historicUpdateBuilder.TypedValue.Value)).body("[0].processDefinitionKey", equalTo(historicUpdateBuilder.ProcessDefinitionKey)).body("[0].processDefinitionId", equalTo(historicUpdateBuilder.ProcessDefinitionId)).body("[0].processInstanceId", equalTo(historicUpdateBuilder.ProcessInstanceId)).body("[0].errorMessage", equalTo(historicUpdateBuilder.ErrorMessage)).body("[0].activityInstanceId", equalTo(historicUpdateBuilder.ActivityInstanceId)).body("[0].revision", equalTo(historicUpdateBuilder.Revision)).body("[0].time", equalTo(historicUpdateBuilder.Time)).body("[0].taskId", equalTo(historicUpdateBuilder.TaskId)).body("[0].executionId", equalTo(historicUpdateBuilder.ExecutionId)).body("[0].type", equalTo("variableUpdate")).body("[0].caseDefinitionKey", equalTo(historicUpdateBuilder.CaseDefinitionKey)).body("[0].caseDefinitionId", equalTo(historicUpdateBuilder.CaseDefinitionId)).body("[0].caseInstanceId", equalTo(historicUpdateBuilder.CaseInstanceId)).body("[0].caseExecutionId", equalTo(historicUpdateBuilder.CaseExecutionId)).body("[0].tenantId", equalTo(historicUpdateBuilder.TenantId)).body("[0].userOperationId", equalTo(historicUpdateBuilder.UserOperationId)).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		verifySimpleHistoricActivityQueryResponse(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricActivityQueryPost()
	  public virtual void testSimpleHistoricActivityQueryPost()
	  {
		// POST
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).and().body("[0].id", equalTo(historicUpdateBuilder.Id)).body("[0].variableName", equalTo(historicUpdateBuilder.Name)).body("[0].variableInstanceId", equalTo(historicUpdateBuilder.VariableInstanceId)).body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(historicUpdateBuilder.TypedValue.Type))).body("[0].value", equalTo(historicUpdateBuilder.TypedValue.Value)).body("[0].processDefinitionKey", equalTo(historicUpdateBuilder.ProcessDefinitionKey)).body("[0].processDefinitionId", equalTo(historicUpdateBuilder.ProcessDefinitionId)).body("[0].processInstanceId", equalTo(historicUpdateBuilder.ProcessInstanceId)).body("[0].errorMessage", equalTo(historicUpdateBuilder.ErrorMessage)).body("[0].activityInstanceId", equalTo(historicUpdateBuilder.ActivityInstanceId)).body("[0].revision", equalTo(historicUpdateBuilder.Revision)).body("[0].time", equalTo(historicUpdateBuilder.Time)).body("[0].taskId", equalTo(historicUpdateBuilder.TaskId)).body("[0].executionId", equalTo(historicUpdateBuilder.ExecutionId)).body("[0].type", equalTo("variableUpdate")).body("[0].caseDefinitionKey", equalTo(historicUpdateBuilder.CaseDefinitionKey)).body("[0].caseDefinitionId", equalTo(historicUpdateBuilder.CaseDefinitionId)).body("[0].caseInstanceId", equalTo(historicUpdateBuilder.CaseInstanceId)).body("[0].caseExecutionId", equalTo(historicUpdateBuilder.CaseExecutionId)).body("[0].tenantId", equalTo(historicUpdateBuilder.TenantId)).body("[0].userOperationId", equalTo(historicUpdateBuilder.UserOperationId)).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		verifySimpleHistoricActivityQueryResponse(response);
	  }

	  private void verifySimpleHistoricActivityQueryResponse(Response response)
	  {
		string content = response.asString();
		IList<string> details = from(content).getList("");
		Assert.assertEquals("There should be two activity instance returned.", 2, details.Count);
		Assert.assertNotNull("The returned details should not be null.", details[0]);
		Assert.assertNotNull("The returned details should not be null.", details[1]);

		// note: element [0] is asserted as part of the fluent rest-assured invocation

		string returnedId2 = from(content).getString("[1].id");
		string returnedProcessDefinitionKey2 = from(content).getString("[1].processDefinitionKey");
		string returnedProcessDefinitionId2 = from(content).getString("[1].processDefinitionId");
		string returnedProcessInstanceId2 = from(content).getString("[1].processInstanceId");
		string returnedActivityInstanceId2 = from(content).getString("[1].activityInstanceId");
		string returnedExecutionId2 = from(content).getString("[1].executionId");
		string returnedTaskId2 = from(content).getString("[1].taskId");
		DateTime returnedTime2 = DateTimeUtil.parseDate(from(content).getString("[1].time"));
		DateTime returnedRemovalTime = DateTimeUtil.parseDate(from(content).getString("[1].removalTime"));
		string returnedFieldId = from(content).getString("[1].fieldId");
		string returnedFieldValue = from(content).getString("[1].fieldValue");
		string returnedType = from(content).getString("[1].type");
		string returnedCaseDefinitionKey2 = from(content).getString("[1].caseDefinitionKey");
		string returnedCaseDefinitionId2 = from(content).getString("[1].caseDefinitionId");
		string returnedCaseInstanceId2 = from(content).getString("[1].caseInstanceId");
		string returnedCaseExecutionId2 = from(content).getString("[1].caseExecutionId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");
		string returnedOperationId2 = from(content).getString("[1].userOperationId");
		string returnedRootProcessInstanceId = from(content).getString("[1].rootProcessInstanceId");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_ID, returnedId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_KEY, returnedProcessDefinitionKey2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_ID, returnedProcessDefinitionId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_PROC_INST_ID, returnedProcessInstanceId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_ACT_INST_ID, returnedActivityInstanceId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_EXEC_ID, returnedExecutionId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_TASK_ID, returnedTaskId2);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_TIME), returnedTime2);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_TIME), returnedRemovalTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_FIELD_ID, returnedFieldId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_VALUE, returnedFieldValue);
		Assert.assertEquals("formField", returnedType);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_ID, returnedCaseDefinitionId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_KEY, returnedCaseDefinitionKey2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_CASE_INST_ID, returnedCaseInstanceId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_CASE_EXEC_ID, returnedCaseExecutionId2);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_EXEC_ID, returnedExecutionId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_FIELD_OPERATION_ID, returnedOperationId2);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_FORM_ROOT_PROCESS_INSTANCE_ID, returnedRootProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSerializableVariableInstanceRetrieval()
	  public virtual void testSerializableVariableInstanceRetrieval()
	  {
		ObjectValue serializedValue = Variables.serializedObjectValue("a serialized value").create();
		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate().typedValue(serializedValue);

		IList<HistoricDetail> details = new List<HistoricDetail>();
		details.Add(builder.build());

		// GET
		mockedQuery = setUpMockedDetailsQuery(details);

		given().then().expect().statusCode(Status.OK.StatusCode).and().body("[0].value", equalTo("a serialized value")).body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(serializedValue.Type))).body("[0].errorMessage", nullValue()).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		// should not resolve custom objects but existing API requires it
	//  verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();

		// POST
		mockedQuery = setUpMockedDetailsQuery(details);

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).and().body("[0].value", equalTo("a serialized value")).body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(serializedValue.Type))).body("[0].errorMessage", nullValue()).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		// should not resolve custom objects but existing API requires it
		//  verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSpinVariableInstanceRetrieval()
	  public virtual void testSpinVariableInstanceRetrieval()
	  {
		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate().typedValue(Variables.serializedObjectValue("aSerializedValue").serializationDataFormat("aDataFormat").objectTypeName("aRootType").create());

		IList<HistoricDetail> details = new List<HistoricDetail>();
		details.Add(builder.build());

		// GET
		mockedQuery = setUpMockedDetailsQuery(details);

		given().then().expect().statusCode(Status.OK.StatusCode).and().body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT))).body("[0].errorMessage", nullValue()).body("[0].value", equalTo("aSerializedValue")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo("aRootType")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("aDataFormat")).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		// POST
		mockedQuery = setUpMockedDetailsQuery(details);

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).and().body("[0].variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT))).body("[0].errorMessage", nullValue()).body("[0].value", equalTo("aSerializedValue")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo("aRootType")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("aDataFormat")).when().post(HISTORIC_DETAIL_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		string processInstanceId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_PROC_INST_ID;

		// GET
		given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).processInstanceId(processInstanceId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["processInstanceId"] = processInstanceId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).processInstanceId(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionId()
	  public virtual void testQueryByExecutionId()
	  {
		string executionId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_EXEC_ID;

		// GET
		given().queryParam("executionId", executionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).executionId(executionId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["executionId"] = executionId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).executionId(executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByOperationId()
	  public virtual void testQueryByOperationId()
	  {
		string operationId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_OPERATION_ID;

		// GET
		given().queryParam("userOperationId", operationId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).userOperationId(operationId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["userOperationId"] = operationId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).userOperationId(operationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceId()
	  public virtual void testQueryByActivityInstanceId()
	  {
		string activityInstanceId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ACT_INST_ID;

		// GET
		given().queryParam("activityInstanceId", activityInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).activityInstanceId(activityInstanceId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["activityInstanceId"] = activityInstanceId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).activityInstanceId(activityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskId()
	  public virtual void testQueryByTaskId()
	  {
		string taskId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TASK_ID;

		// GET
		given().queryParam("taskId", taskId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).taskId(taskId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["taskId"] = taskId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).taskId(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByVariableInstanceId()
	  public virtual void testQueryByVariableInstanceId()
	  {
		string variableInstanceId = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID;

		// GET
		given().queryParam("variableInstanceId", variableInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableInstanceId(variableInstanceId);

		reset(mockedQuery);

		// POST
		IDictionary<string, string> jsonBody = new Dictionary<string, string>();
		jsonBody["variableInstanceId"] = variableInstanceId;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableInstanceId(variableInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByVariableTypeIn()
	  public virtual void testQueryByVariableTypeIn()
	  {
		string aVariableType = "string";
		string anotherVariableType = "integer";

		// GET
		given().queryParam("variableTypeIn", aVariableType + "," + anotherVariableType).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableTypeIn(aVariableType, anotherVariableType);

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		string[] variableTypeIn = new string[] {aVariableType, anotherVariableType};
		jsonBody["variableTypeIn"] = variableTypeIn;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableTypeIn(aVariableType, anotherVariableType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFormFields()
	  public virtual void testQueryByFormFields()
	  {
		// GET
		given().queryParam("formFields", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).formFields();

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["formFields"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).formFields();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByVariableUpdates()
	  public virtual void testQueryByVariableUpdates()
	  {
		// GET
		given().queryParam("variableUpdates", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableUpdates();

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["variableUpdates"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).variableUpdates();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExcludeTaskDetails()
	  public virtual void testQueryByExcludeTaskDetails()
	  {
		// GET
		given().queryParam("excludeTaskDetails", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).excludeTaskDetails();

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["excludeTaskDetails"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).excludeTaskDetails();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseInstanceId()
	  public virtual void testQueryByCaseInstanceId()
	  {
		// GET
		given().queryParam("caseInstanceId", MockProvider.EXAMPLE_CASE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(MockProvider.EXAMPLE_CASE_INSTANCE_ID);

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["caseInstanceId"] = MockProvider.EXAMPLE_CASE_INSTANCE_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseExecutionId()
	  public virtual void testQueryByCaseExecutionId()
	  {
		// GET
		given().queryParam("caseExecutionId", MockProvider.EXAMPLE_CASE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).caseExecutionId(MockProvider.EXAMPLE_CASE_EXECUTION_ID);

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["caseExecutionId"] = MockProvider.EXAMPLE_CASE_EXECUTION_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).caseExecutionId(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockedDetailsQuery(createMockHistoricDetailsTwoTenants());

		// GET
		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).queryParam("variableUpdates", "true").queryParam("formFields", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).variableUpdates();
		verify(mockedQuery).formFields();
		verify(mockedQuery).list();

		verifyTenantIdListParameterResponse(response);

		mockedQuery = setUpMockedDetailsQuery(createMockHistoricDetailsTwoTenants());

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		string[] exampleTenantIdList = new string[] {MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID};
		jsonBody["tenantIdIn"] = exampleTenantIdList;
		jsonBody["variableUpdates"] = true;
		jsonBody["formFields"] = true;

		response = given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).variableUpdates();
		verify(mockedQuery).formFields();
		verify(mockedQuery).list();

		verifyTenantIdListParameterResponse(response);
	  }

	  private void verifyTenantIdListParameterResponse(Response response)
	  {
		string content = response.asString();
		IList<string> historicDetails = from(content).getList("");
		assertThat(historicDetails).hasSize(4);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");
		string returnedTenantId3 = from(content).getString("[2].tenantId");
		string returnedTenantId4 = from(content).getString("[3].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId3).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId4).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testByProcessInstanceIdIn()
	   public virtual void testByProcessInstanceIdIn()
	   {
		string aProcessInstanceId = "aProcessInstanceId";
		string anotherProcessInstanceId = "anotherProcessInstanceId";

		// GET
		given().queryParam("processInstanceIdIn", aProcessInstanceId + "," + anotherProcessInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId);

		reset(mockedQuery);

		 // POST
		 IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		 string[] processInstanceIdIn = new string[] {aProcessInstanceId, anotherProcessInstanceId};
		 jsonBody["processInstanceIdIn"] = processInstanceIdIn;

		 given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		 verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId);
	   }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testByOccurredBefore()
	  public virtual void testByOccurredBefore()
	  {
		// GET
		given().queryParam("occurredBefore", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).occurredBefore(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME));

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["occurredBefore"] = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).occurredBefore(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testByOccurredAfter()
	  public virtual void testByOccurredAfter()
	  {
		// GET
		given().queryParam("occurredAfter", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).occurredAfter(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME));

		reset(mockedQuery);

		// POST
		IDictionary<string, object> jsonBody = new Dictionary<string, object>();
		jsonBody["occurredAfter"] = MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(jsonBody).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).occurredAfter(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_TIME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetQueryWhereFileWasDeleted()
	  public virtual void testGetQueryWhereFileWasDeleted()
	  {
		doThrow(new System.ArgumentException("Parameter 'filename' is null")).when(historicUpdateMock).TypedValue;

		// GET
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostQueryWhereFileWasDeleted()
	  public virtual void testPostQueryWhereFileWasDeleted()
	  {
		doThrow(new System.ArgumentException("Parameter 'filename' is null")).when(historicUpdateMock).TypedValue;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(Collections.emptyMap()).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DETAIL_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verifyNoMoreInteractions(mockedQuery);
	  }

	  private IList<HistoricDetail> createMockHistoricDetailsTwoTenants()
	  {
		IList<HistoricDetail> mockHistoricDetails = MockProvider.createMockHistoricDetails(MockProvider.EXAMPLE_TENANT_ID);
		IList<HistoricDetail> mockHistoricDetails2 = MockProvider.createMockHistoricDetails(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		((IList<HistoricDetail>)mockHistoricDetails).AddRange(mockHistoricDetails2);
		return mockHistoricDetails;
	  }

	}

}