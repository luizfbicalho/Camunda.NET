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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasEntry;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using HistoricDecisionInputInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInputInstanceDto;
	using HistoricDecisionOutputInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionOutputInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricDecisionInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_DECISION_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/decision-instance";
	  protected internal static readonly string HISTORIC_DECISION_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_DECISION_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricDecisionInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricDecisionInstanceQuery(MockProvider.createMockHistoricDecisionInstances());
	  }

	  protected internal virtual HistoricDecisionInstanceQuery setUpMockHistoricDecisionInstanceQuery(IList<HistoricDecisionInstance> mockedHistoricDecisionInstances)
	  {
		HistoricDecisionInstanceQuery mockedHistoricDecisionInstanceQuery = mock(typeof(HistoricDecisionInstanceQuery));
		when(mockedHistoricDecisionInstanceQuery.list()).thenReturn(mockedHistoricDecisionInstances);
		when(mockedHistoricDecisionInstanceQuery.count()).thenReturn((long) mockedHistoricDecisionInstances.Count);

		when(processEngine.HistoryService.createHistoricDecisionInstanceQuery()).thenReturn(mockedHistoricDecisionInstanceQuery);

		return mockedHistoricDecisionInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("caseDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("definitionId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "evaluationTime").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("evaluationTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByEvaluationTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("evaluationTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByEvaluationTime();
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

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_DECISION_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricDecisionInstanceQuery()
	  public virtual void testSimpleHistoricDecisionInstanceQuery()
	  {
		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		Response response = given().queryParam("decisionDefinitionId", decisionDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedHistoricDecisionInstanceId = from(content).getString("[0].id");
		string returnedDecisionDefinitionId = from(content).getString("[0].decisionDefinitionId");
		string returnedDecisionDefinitionKey = from(content).getString("[0].decisionDefinitionKey");
		string returnedDecisionDefinitionName = from(content).getString("[0].decisionDefinitionName");
		string returnedEvaluationTime = from(content).getString("[0].evaluationTime");
		string returnedRemovalTime = from(content).getString("[0].removalTime");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedCaseDefinitionKey = from(content).getString("[0].caseDefinitionKey");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedActivityInstanceId = from(content).getString("[0].activityInstanceId");
		IList<HistoricDecisionInputInstanceDto> returnedInputs = from(content).getList("[0].inputs");
		IList<HistoricDecisionOutputInstanceDto> returnedOutputs = from(content).getList("[0].outputs");
		double? returnedCollectResultValue = from(content).getDouble("[0].collectResultValue");
		string returnedTenantId = from(content).getString("[0].tenantId");
		string returnedRootDecisionInstanceId = from(content).getString("[0].rootDecisionInstanceId");
		string returnedRootProcessInstanceId = from(content).getString("[0].rootProcessInstanceId");
		string returnedDecisionRequirementsDefinitionId = from(content).getString("[0].decisionRequirementsDefinitionId");
		string returnedDecisionRequirementsDefinitionKey = from(content).getString("[0].decisionRequirementsDefinitionKey");

		assertThat(returnedHistoricDecisionInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		assertThat(returnedDecisionDefinitionId, @is(MockProvider.EXAMPLE_DECISION_DEFINITION_ID));
		assertThat(returnedDecisionDefinitionKey, @is(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY));
		assertThat(returnedDecisionDefinitionName, @is(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME));
		assertThat(returnedEvaluationTime, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATION_TIME));
		assertThat(returnedRemovalTime, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_REMOVAL_TIME));
		assertThat(returnedProcessDefinitionId, @is(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		assertThat(returnedProcessDefinitionKey, @is(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY));
		assertThat(returnedProcessInstanceId, @is(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID));
		assertThat(returnedCaseDefinitionId, @is(MockProvider.EXAMPLE_CASE_DEFINITION_ID));
		assertThat(returnedCaseDefinitionKey, @is(MockProvider.EXAMPLE_CASE_DEFINITION_KEY));
		assertThat(returnedCaseInstanceId, @is(MockProvider.EXAMPLE_CASE_INSTANCE_ID));
		assertThat(returnedActivityId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID));
		assertThat(returnedActivityInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID));
		assertThat(returnedInputs, @is(nullValue()));
		assertThat(returnedOutputs, @is(nullValue()));
		assertThat(returnedCollectResultValue, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_COLLECT_RESULT_VALUE));
		assertThat(returnedTenantId, @is(MockProvider.EXAMPLE_TENANT_ID));
		assertThat(returnedRootDecisionInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		assertThat(returnedRootProcessInstanceId, @is(MockProvider.EXAMPLE_ROOT_HISTORIC_PROCESS_INSTANCE_ID));
		assertThat(returnedDecisionRequirementsDefinitionId, @is(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID));
		assertThat(returnedDecisionRequirementsDefinitionKey, @is(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeInputs()
	  public virtual void testIncludeInputs()
	  {
		mockedQuery = setUpMockHistoricDecisionInstanceQuery(Collections.singletonList(MockProvider.createMockHistoricDecisionInstanceWithInputs()));

		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		Response response = given().queryParam("decisionDefinitionId", decisionDefinitionId).queryParam("includeInputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery).includeInputs();
		inOrder.verify(mockedQuery, never()).includeOutputs();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("[0].inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("[0].outputs");

		assertThat(returnedInputs, @is(notNullValue()));
		assertThat(returnedOutputs, @is(nullValue()));

		verifyHistoricDecisionInputInstances(returnedInputs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeOutputs()
	  public virtual void testIncludeOutputs()
	  {
		mockedQuery = setUpMockHistoricDecisionInstanceQuery(Collections.singletonList(MockProvider.createMockHistoricDecisionInstanceWithOutputs()));

		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		Response response = given().queryParam("decisionDefinitionId", decisionDefinitionId).queryParam("includeOutputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery, never()).includeInputs();
		inOrder.verify(mockedQuery).includeOutputs();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("[0].inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("[0].outputs");

		assertThat(returnedInputs, @is(nullValue()));
		assertThat(returnedOutputs, @is(notNullValue()));

		verifyHistoricDecisionOutputInstances(returnedOutputs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeInputsAndOutputs()
	  public virtual void testIncludeInputsAndOutputs()
	  {
		mockedQuery = setUpMockHistoricDecisionInstanceQuery(Collections.singletonList(MockProvider.createMockHistoricDecisionInstanceWithInputsAndOutputs()));

		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		Response response = given().queryParam("decisionDefinitionId", decisionDefinitionId).queryParam("includeInputs", true).queryParam("includeOutputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery).includeInputs();
		inOrder.verify(mockedQuery).includeOutputs();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("[0].inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("[0].outputs");

		assertThat(returnedInputs, @is(notNullValue()));
		assertThat(returnedOutputs, @is(notNullValue()));

		verifyHistoricDecisionInputInstances(returnedInputs);
		verifyHistoricDecisionOutputInstances(returnedOutputs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultBinaryFetching()
	  public virtual void testDefaultBinaryFetching()
	  {
		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		given().queryParam("decisionDefinitionId", decisionDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery, never()).disableBinaryFetching();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableBinaryFetching()
	  public virtual void testDisableBinaryFetching()
	  {
		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		given().queryParam("decisionDefinitionId", decisionDefinitionId).queryParam("disableBinaryFetching", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery).disableBinaryFetching();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultCustomObjectDeserialization()
	  public virtual void testDefaultCustomObjectDeserialization()
	  {
		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		given().queryParam("decisionDefinitionId", decisionDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery, never()).disableCustomObjectDeserialization();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableCustomObjectDeserialization()
	  public virtual void testDisableCustomObjectDeserialization()
	  {
		string decisionDefinitionId = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;

		given().queryParam("decisionDefinitionId", decisionDefinitionId).queryParam("disableCustomObjectDeserialization", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionId(decisionDefinitionId);
		inOrder.verify(mockedQuery).disableCustomObjectDeserialization();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRootDecisionInstancesOnly()
	  public virtual void testRootDecisionInstancesOnly()
	  {

		given().queryParam("rootDecisionInstancesOnly", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).rootDecisionInstancesOnly();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricDecisionInstanceQuery(createMockHistoricDecisionInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicDecisionInstances = from(content).getList("");
		assertThat(historicDecisionInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

	  private IList<HistoricDecisionInstance> createMockHistoricDecisionInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricDecisionInstanceBase(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricDecisionInstanceBase(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

	  protected internal virtual IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["decisionInstanceId"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID;
			parameters["decisionInstanceIdIn"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID_IN;
			parameters["decisionDefinitionId"] = MockProvider.EXAMPLE_DECISION_DEFINITION_ID;
			parameters["decisionDefinitionIdIn"] = MockProvider.EXAMPLE_DECISION_DEFINITION_ID_IN;
			parameters["decisionDefinitionKey"] = MockProvider.EXAMPLE_DECISION_DEFINITION_KEY;
			parameters["decisionDefinitionKeyIn"] = MockProvider.EXAMPLE_DECISION_DEFINITION_KEY_IN;
			parameters["decisionDefinitionName"] = MockProvider.EXAMPLE_DECISION_DEFINITION_NAME;
			parameters["decisionDefinitionNameLike"] = MockProvider.EXAMPLE_DECISION_DEFINITION_NAME_LIKE;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
			parameters["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
			parameters["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;
			parameters["caseDefinitionId"] = MockProvider.EXAMPLE_CASE_DEFINITION_ID;
			parameters["caseDefinitionKey"] = MockProvider.EXAMPLE_CASE_DEFINITION_KEY;
			parameters["caseInstanceId"] = MockProvider.EXAMPLE_CASE_INSTANCE_ID;
			parameters["activityIdIn"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID_IN;
			parameters["activityInstanceIdIn"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID_IN;
			parameters["evaluatedBefore"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATED_BEFORE;
			parameters["evaluatedAfter"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATED_AFTER;
			parameters["userId"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_USER_ID;
			parameters["rootDecisionInstanceId"] = MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID;
			parameters["decisionRequirementsDefinitionId"] = MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID;
			parameters["decisionRequirementsDefinitionKey"] = MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;
		StringArrayConverter stringArrayConverter = new StringArrayConverter();

		verify(mockedQuery).decisionInstanceId(stringQueryParameters["decisionInstanceId"]);
		verify(mockedQuery).decisionInstanceIdIn(stringArrayConverter.convertQueryParameterToType(stringQueryParameters["decisionInstanceIdIn"]));
		verify(mockedQuery).decisionDefinitionId(stringQueryParameters["decisionDefinitionId"]);
		verify(mockedQuery).decisionDefinitionIdIn(stringArrayConverter.convertQueryParameterToType(stringQueryParameters["decisionDefinitionIdIn"]));
		verify(mockedQuery).decisionDefinitionKey(stringQueryParameters["decisionDefinitionKey"]);
		verify(mockedQuery).decisionDefinitionKeyIn(stringArrayConverter.convertQueryParameterToType(stringQueryParameters["decisionDefinitionKeyIn"]));
		verify(mockedQuery).decisionDefinitionName(stringQueryParameters["decisionDefinitionName"]);
		verify(mockedQuery).decisionDefinitionNameLike(stringQueryParameters["decisionDefinitionNameLike"]);
		verify(mockedQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionKey(stringQueryParameters["processDefinitionKey"]);
		verify(mockedQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockedQuery).caseDefinitionId(stringQueryParameters["caseDefinitionId"]);
		verify(mockedQuery).caseDefinitionKey(stringQueryParameters["caseDefinitionKey"]);
		verify(mockedQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);
		verify(mockedQuery).activityIdIn(stringArrayConverter.convertQueryParameterToType(stringQueryParameters["activityIdIn"]));
		verify(mockedQuery).activityInstanceIdIn(stringArrayConverter.convertQueryParameterToType(stringQueryParameters["activityInstanceIdIn"]));
		verify(mockedQuery).evaluatedBefore(DateTimeUtil.parseDate(stringQueryParameters["evaluatedBefore"]));
		verify(mockedQuery).evaluatedAfter(DateTimeUtil.parseDate(stringQueryParameters["evaluatedAfter"]));
		verify(mockedQuery).userId(stringQueryParameters["userId"]);
		verify(mockedQuery).rootDecisionInstanceId(stringQueryParameters["rootDecisionInstanceId"]);
		verify(mockedQuery).decisionRequirementsDefinitionId(stringQueryParameters["decisionRequirementsDefinitionId"]);
		verify(mockedQuery).decisionRequirementsDefinitionKey(stringQueryParameters["decisionRequirementsDefinitionKey"]);

		verify(mockedQuery).list();
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_DECISION_INSTANCE_RESOURCE_URL);
	  }

	  protected internal virtual void verifyHistoricDecisionInputInstances(IList<IDictionary<string, object>> returnedInputs)
	  {
		assertThat(returnedInputs, hasSize(3));

		// verify common properties
		foreach (IDictionary<string, object> returnedInput in returnedInputs)
		{
		  assertThat(returnedInput, hasEntry("id", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_ID));
		  assertThat(returnedInput, hasEntry("decisionInstanceId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		  assertThat(returnedInput, hasEntry("clauseId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_ID));
		  assertThat(returnedInput, hasEntry("clauseName", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_NAME));
		  assertThat(returnedInput, hasEntry("errorMessage", null));
		  assertThat(returnedInput, hasEntry("createTime", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CREATE_TIME));
		  assertThat(returnedInput, hasEntry("removalTime", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_REMOVAL_TIME));
		  assertThat(returnedInput, hasEntry("rootProcessInstanceId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INPUT_ROOT_PROCESS_INSTANCE_ID));
		}

		verifyStringValue(returnedInputs[0]);
		verifyByteArrayValue(returnedInputs[1]);
		verifySerializedValue(returnedInputs[2]);

	  }

	  protected internal virtual void verifyHistoricDecisionOutputInstances(IList<IDictionary<string, object>> returnedOutputs)
	  {
		assertThat(returnedOutputs, hasSize(3));

		// verify common properties
		foreach (IDictionary<string, object> returnedOutput in returnedOutputs)
		{
		  assertThat(returnedOutput, hasEntry("id", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_ID));
		  assertThat(returnedOutput, hasEntry("decisionInstanceId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		  assertThat(returnedOutput, hasEntry("clauseId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_ID));
		  assertThat(returnedOutput, hasEntry("clauseName", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_NAME));
		  assertThat(returnedOutput, hasEntry("ruleId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ID));
		  assertThat(returnedOutput, hasEntry("ruleOrder", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ORDER));
		  assertThat(returnedOutput, hasEntry("variableName", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_VARIABLE_NAME));
		  assertThat(returnedOutput, hasEntry("errorMessage", null));
		  assertThat(returnedOutput, hasEntry("createTime", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CREATE_TIME));
		  assertThat(returnedOutput, hasEntry("removalTime", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_REMOVAL_TIME));
		  assertThat(returnedOutput, hasEntry("rootProcessInstanceId", (object) MockProvider.EXAMPLE_HISTORIC_DECISION_OUTPUT_ROOT_PROCESS_INSTANCE_ID));
		}

		verifyStringValue(returnedOutputs[0]);
		verifyByteArrayValue(returnedOutputs[1]);
		verifySerializedValue(returnedOutputs[2]);
	  }

	  protected internal virtual void verifyStringValue(IDictionary<string, object> stringValue)
	  {
		StringValue exampleValue = MockProvider.EXAMPLE_HISTORIC_DECISION_STRING_VALUE;
		assertThat(stringValue, hasEntry("type", (object) VariableValueDto.toRestApiTypeName(exampleValue.Type.Name)));
		assertThat(stringValue, hasEntry("value", (object) exampleValue.Value));
		assertThat(stringValue, hasEntry("valueInfo", (object) Collections.emptyMap()));
	  }

	  protected internal virtual void verifyByteArrayValue(IDictionary<string, object> byteArrayValue)
	  {
		BytesValue exampleValue = MockProvider.EXAMPLE_HISTORIC_DECISION_BYTE_ARRAY_VALUE;
		assertThat(byteArrayValue, hasEntry("type", (object) VariableValueDto.toRestApiTypeName(exampleValue.Type.Name)));
		string byteString = Base64.encodeBase64String(exampleValue.Value).Trim();
		assertThat(byteArrayValue, hasEntry("value", (object) byteString));
		assertThat(byteArrayValue, hasEntry("valueInfo", (object) Collections.emptyMap()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void verifySerializedValue(java.util.Map<String, Object> serializedValue)
	  protected internal virtual void verifySerializedValue(IDictionary<string, object> serializedValue)
	  {
		ObjectValue exampleValue = MockProvider.EXAMPLE_HISTORIC_DECISION_SERIALIZED_VALUE;
		assertThat(serializedValue, hasEntry("type", (object) VariableValueDto.toRestApiTypeName(exampleValue.Type.Name)));
		assertThat(serializedValue, hasEntry("value", exampleValue.Value));
		IDictionary<string, string> valueInfo = (IDictionary<string, string>) serializedValue["valueInfo"];
		assertThat(valueInfo, hasEntry("serializationDataFormat", exampleValue.SerializationDataFormat));
		assertThat(valueInfo, hasEntry("objectTypeName", exampleValue.ObjectTypeName));

	  }

	}

}