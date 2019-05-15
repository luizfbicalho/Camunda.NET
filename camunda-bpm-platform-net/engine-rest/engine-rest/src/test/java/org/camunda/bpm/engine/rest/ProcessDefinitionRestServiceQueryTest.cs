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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
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


	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockDefinitionBuilder = org.camunda.bpm.engine.rest.helper.MockDefinitionBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Matchers = org.mockito.Matchers;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class ProcessDefinitionRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_DEFINITION_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition";
	  protected internal static readonly string PROCESS_DEFINITION_COUNT_QUERY_URL = PROCESS_DEFINITION_QUERY_URL + "/count";
	  private ProcessDefinitionQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockDefinitionQuery(MockProvider.createMockDefinitions());
	  }

	  private ProcessDefinitionQuery setUpMockDefinitionQuery(IList<ProcessDefinition> mockedDefinitions)
	  {
		ProcessDefinitionQuery sampleDefinitionsQuery = mock(typeof(ProcessDefinitionQuery));
		when(sampleDefinitionsQuery.list()).thenReturn(mockedDefinitions);
		when(sampleDefinitionsQuery.count()).thenReturn((long) mockedDefinitions.Count);
		when(processEngine.RepositoryService.createProcessDefinitionQuery()).thenReturn(sampleDefinitionsQuery);
		return sampleDefinitionsQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("keyLike", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidNumericParameter()
	  public virtual void testInvalidNumericParameter()
	  {
		string anInvalidIntegerQueryParam = "aString";
		given().queryParam("version", anInvalidIntegerQueryParam).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot set query parameter 'version' to value 'aString': " + "Cannot convert value aString to java type java.lang.Integer")).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidBooleanParameter()
	  public virtual void testInvalidBooleanParameter()
	  {
		string anInvalidBooleanQueryParam = "neitherTrueNorFalse";
		given().queryParam("active", anInvalidBooleanQueryParam).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot set query parameter 'active' to value 'neitherTrueNorFalse': " + "Cannot convert value neitherTrueNorFalse to java type java.lang.Boolean")).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifyFailingSorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST, typeof(InvalidRequestException).Name, "Cannot set query parameter 'sortBy' to value 'anInvalidSortByOption'");
		executeAndVerifyFailingSorting("category", "anInvalidSortOrderOption", Status.BAD_REQUEST, typeof(InvalidRequestException).Name, "Cannot set query parameter 'sortOrder' to value 'anInvalidSortOrderOption'");
	  }

	  protected internal virtual void executeAndVerifySuccessfulSorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

	  protected internal virtual void executeAndVerifyFailingSorting(string sortBy, string sortOrder, Status expectedStatus, string expectedErrorType, string expectedErrorMessage)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).contentType(ContentType.JSON).body("type", equalTo(expectedErrorType)).body("message", equalTo(expectedErrorMessage)).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "category").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(PROCESS_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionRetrieval()
	  public virtual void testProcessDefinitionRetrieval()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);

		string queryKey = "Key";
		Response response = given().queryParam("keyLike", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		// assert query invocation
		inOrder.verify(mockedQuery).processDefinitionKeyLike(queryKey);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		Assert.assertEquals("There should be one process definition returned.", 1, definitions.Count);
		Assert.assertNotNull("There should be one process definition returned", definitions[0]);

		string returnedDefinitionKey = from(content).getString("[0].key");
		string returnedDefinitionId = from(content).getString("[0].id");
		string returnedCategory = from(content).getString("[0].category");
		string returnedDefinitionName = from(content).getString("[0].name");
		string returnedDescription = from(content).getString("[0].description");
		int returnedVersion = from(content).getInt("[0].version");
		string returnedResourceName = from(content).getString("[0].resource");
		string returnedDeploymentId = from(content).getString("[0].deploymentId");
		string returnedDiagramResourceName = from(content).getString("[0].diagram");
		bool? returnedIsSuspended = from(content).getBoolean("[0].suspended");
		bool? returnedIsStartedInTasklist = from(content).getBoolean("[0].startableInTasklist");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY, returnedCategory);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME, returnedDefinitionName);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION, returnedDescription);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION, returnedVersion);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME, returnedResourceName);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, returnedDeploymentId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME, returnedDiagramResourceName);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED, returnedIsSuspended);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_STARTABLE, returnedIsStartedInTasklist);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionRetrievalByList()
	  public virtual void testProcessDefinitionRetrievalByList()
	  {
		mockedQuery = setUpMockDefinitionQuery(MockProvider.createMockTwoDefinitions());

		Response response = given().queryParam("processDefinitionIdIn", MockProvider.EXAMPLE_PROCESS_DEFINTION_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processDefinitionIdIn(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedDefinitionId1 = from(content).getString("[0].id");
		string returnedDefinitionId2 = from(content).getString("[1].id");

		assertThat(returnedDefinitionId1).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		assertThat(returnedDefinitionId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionRetrievalByEmptyList()
	  public virtual void testProcessDefinitionRetrievalByEmptyList()
	  {
		mockedQuery = setUpMockDefinitionQuery(MockProvider.createMockTwoDefinitions());

		Response response = given().queryParam("processDefinitionIdIn", "").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery, never()).processDefinitionIdIn(Matchers.anyVararg<string[]>());
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedDefinitionId1 = from(content).getString("[0].id");
		string returnedDefinitionId2 = from(content).getString("[1].id");

		assertThat(returnedDefinitionId1).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		assertThat(returnedDefinitionId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncompleteProcessDefinition()
	  public virtual void testIncompleteProcessDefinition()
	  {
		UpMockDefinitionQuery = createIncompleteMockDefinitions();
		Response response = expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		string content = response.asString();
		string returnedResourceName = from(content).getString("[0].resource");
		Assert.assertNull("Should be null, as it is also null in the original process definition on the server.", returnedResourceName);
	  }

	  private IList<ProcessDefinition> createIncompleteMockDefinitions()
	  {
		IList<ProcessDefinition> mocks = new List<ProcessDefinition>();

		MockDefinitionBuilder builder = new MockDefinitionBuilder();
		ProcessDefinition mockDefinition = builder.id(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).category(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY).name(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME).key(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).suspended(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED).version(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION).build();

		mocks.Add(mockDefinition);
		return mocks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		// assert query invocation
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionCategory(queryParameters["category"]);
		verify(mockedQuery).processDefinitionCategoryLike(queryParameters["categoryLike"]);
		verify(mockedQuery).processDefinitionName(queryParameters["name"]);
		verify(mockedQuery).processDefinitionNameLike(queryParameters["nameLike"]);
		verify(mockedQuery).deploymentId(queryParameters["deploymentId"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["key"]);
		verify(mockedQuery).processDefinitionKeyLike(queryParameters["keyLike"]);
		verify(mockedQuery).processDefinitionVersion(int.Parse(queryParameters["version"]));
		verify(mockedQuery).latestVersion();
		verify(mockedQuery).processDefinitionResourceName(queryParameters["resourceName"]);
		verify(mockedQuery).processDefinitionResourceNameLike(queryParameters["resourceNameLike"]);
		verify(mockedQuery).startableByUser(queryParameters["startableBy"]);
		verify(mockedQuery).active();
		verify(mockedQuery).suspended();
		verify(mockedQuery).incidentId(queryParameters["incidentId"]);
		verify(mockedQuery).incidentType(queryParameters["incidentType"]);
		verify(mockedQuery).incidentMessage(queryParameters["incidentMessage"]);
		verify(mockedQuery).incidentMessageLike(queryParameters["incidentMessageLike"]);
		verify(mockedQuery).versionTag(queryParameters["versionTag"]);
		verify(mockedQuery).versionTagLike(queryParameters["versionTagLike"]);
		verify(mockedQuery).startableInTasklist();
		verify(mockedQuery).list();
	  }

	  private IDictionary<string, string> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["processDefinitionId"] = "anId";
			parameters["category"] = "cat";
			parameters["categoryLike"] = "catlike";
			parameters["name"] = "name";
			parameters["nameLike"] = "namelike";
			parameters["deploymentId"] = "depId";
			parameters["key"] = "key";
			parameters["keyLike"] = "keylike";
			parameters["version"] = "0";
			parameters["latestVersion"] = "true";
			parameters["resourceName"] = "res";
			parameters["resourceNameLike"] = "resLike";
			parameters["startableBy"] = "kermit";
			parameters["suspended"] = "true";
			parameters["active"] = "true";
			parameters["incidentId"] = "incId";
			parameters["incidentType"] = "incType";
			parameters["incidentMessage"] = "incMessage";
			parameters["incidentMessageLike"] = "incMessageLike";
			parameters["versionTag"] = "semVer";
			parameters["versionTagLike"] = "semVerLike";
			parameters["startableInTasklist"] = "true";
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionTenantIdList()
	  public virtual void testProcessDefinitionTenantIdList()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockDefinition().id(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionKeysList()
	  public virtual void testProcessDefinitionKeysList()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().key(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).build(), MockProvider.mockDefinition().key(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_KEY).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		Response response = given().queryParam("keysIn", MockProvider.EXAMPLE_KEY_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).processDefinitionKeysIn(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedKey1 = from(content).getString("[0].key");
		string returnedKey2 = from(content).getString("[1].key");

		assertThat(returnedKey1).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		assertThat(returnedKey2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_KEY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionWithoutTenantId()
	  public virtual void testProcessDefinitionWithoutTenantId()
	  {
		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionTenantIdIncludeDefinitionsWithoutTenantid()
	  public virtual void testProcessDefinitionTenantIdIncludeDefinitionsWithoutTenantid()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().tenantId(null).build(), MockProvider.mockDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID).queryParam("includeProcessDefinitionsWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockedQuery).includeProcessDefinitionsWithoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionVersionTag()
	  public virtual void testProcessDefinitionVersionTag()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().versionTag(MockProvider.EXAMPLE_VERSION_TAG).build(), MockProvider.mockDefinition().id(MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).versionTag(MockProvider.ANOTHER_EXAMPLE_VERSION_TAG).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		given().queryParam("versionTag", MockProvider.EXAMPLE_VERSION_TAG).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).versionTag(MockProvider.EXAMPLE_VERSION_TAG);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotStartableInTasklist()
	  public virtual void testNotStartableInTasklist()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().isStartableInTasklist(false).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		given().queryParam("notStartableInTasklist", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).notStartableInTasklist();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartableInTasklistPermissionCheck()
	  public virtual void testStartableInTasklistPermissionCheck()
	  {
		IList<ProcessDefinition> processDefinitions = Arrays.asList(MockProvider.mockDefinition().isStartableInTasklist(false).build());
		mockedQuery = setUpMockDefinitionQuery(processDefinitions);

		given().queryParam("startablePermissionCheck", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).startablePermissionCheck();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("category", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionCategory();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("key", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("id", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("version", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionVersion();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("name", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("deploymentId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("versionTag", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVersionTag();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("versionTag", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVersionTag();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

	  /// <summary>
	  /// If parameter "firstResult" is missing, we expect 0 as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

	  /// <summary>
	  /// If parameter "maxResults" is missing, we expect Integer.MAX_VALUE as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(PROCESS_DEFINITION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

	}

}