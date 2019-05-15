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
//	import static org.mockito.Mockito.when;


	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Matchers = org.mockito.Matchers;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class DecisionDefinitionRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string DECISION_DEFINITION_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/decision-definition";
	  protected internal static readonly string DECISION_DEFINITION_COUNT_QUERY_URL = DECISION_DEFINITION_QUERY_URL + "/count";

	  private DecisionDefinitionQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntime()
	  public virtual void setUpRuntime()
	  {
		mockedQuery = createMockDecisionDefinitionQuery(MockProvider.createMockDecisionDefinitions());
	  }

	  private DecisionDefinitionQuery createMockDecisionDefinitionQuery(IList<DecisionDefinition> mockedDefinitions)
	  {
		DecisionDefinitionQuery sampleDefinitionsQuery = mock(typeof(DecisionDefinitionQuery));

		when(sampleDefinitionsQuery.list()).thenReturn(mockedDefinitions);
		when(sampleDefinitionsQuery.count()).thenReturn((long) mockedDefinitions.Count);
		when(processEngine.RepositoryService.createDecisionDefinitionQuery()).thenReturn(sampleDefinitionsQuery);

		return sampleDefinitionsQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidNumericParameter()
	  public virtual void testInvalidNumericParameter()
	  {
		string anInvalidIntegerQueryParam = "aString";

		given().queryParam("version", anInvalidIntegerQueryParam).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot set query parameter 'version' to value 'aString': " + "Cannot convert value aString to java type java.lang.Integer")).when().get(DECISION_DEFINITION_QUERY_URL);
	  }

	  /// <summary>
	  /// We assume that boolean query parameters that are not "true"
	  /// or "false" are evaluated to "false" and don't cause a 400 error.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidBooleanParameter()
	  public virtual void testInvalidBooleanParameter()
	  {
		string anInvalidBooleanQueryParam = "neitherTrueNorFalse";
		given().queryParam("active", anInvalidBooleanQueryParam).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("id", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "id").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		// asc
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("id", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("name", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("version", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionVersion();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("key", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("category", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionCategory();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("deploymentId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		// desc
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("id", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("name", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("version", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionVersion();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("key", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("category", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDecisionDefinitionCategory();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("deploymentId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("versionTag", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVersionTag();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("versionTag", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVersionTag();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

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

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

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

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionRetrieval()
	  public virtual void testDecisionDefinitionRetrieval()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> decisionDefinitions = from(content).getList("");

		assertThat(decisionDefinitions).hasSize(1);
		assertThat(decisionDefinitions[0]).NotNull;

		string returnedId = from(content).getString("[0].id");
		string returnedKey = from(content).getString("[0].key");
		string returnedCategory = from(content).getString("[0].category");
		string returnedName = from(content).getString("[0].name");
		int returnedVersion = from(content).getInt("[0].version");
		string returnedResource = from(content).getString("[0].resource");
		string returnedDeploymentId = from(content).getString("[0].deploymentId");
		string returnedDecisionRequirementsDefinitionId = from(content).getString("[0].decisionRequirementsDefinitionId");
		string returnedDecisionRequirementsDefinitionKey = from(content).getString("[0].decisionRequirementsDefinitionKey");
		string returnedTenantId = from(content).getString("[0].tenantId");

		assertThat(returnedId).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
		assertThat(returnedKey).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY);
		assertThat(returnedCategory).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_CATEGORY);
		assertThat(returnedName).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME);
		assertThat(returnedVersion).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_VERSION);
		assertThat(returnedResource).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME);
		assertThat(returnedDeploymentId).isEqualTo(MockProvider.EXAMPLE_DEPLOYMENT_ID);
		assertThat(returnedDecisionRequirementsDefinitionId).isEqualTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		assertThat(returnedDecisionRequirementsDefinitionKey).isEqualTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY);
		assertThat(returnedTenantId).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionRetrievalByList()
	  public virtual void testDecisionDefinitionRetrievalByList()
	  {
		mockedQuery = createMockDecisionDefinitionQuery(MockProvider.createMockTwoDecisionDefinitions());

		Response response = given().queryParam("decisionDefinitionIdIn", MockProvider.EXAMPLE_DECISION_DEFINITION_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).decisionDefinitionIdIn(MockProvider.EXAMPLE_DECISION_DEFINITION_ID, MockProvider.ANOTHER_EXAMPLE_DECISION_DEFINITION_ID);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedDefinitionId1 = from(content).getString("[0].id");
		string returnedDefinitionId2 = from(content).getString("[1].id");

		assertThat(returnedDefinitionId1).isEqualTo(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
		assertThat(returnedDefinitionId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_DECISION_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionRetrievalByEmptyList()
	  public virtual void testDecisionDefinitionRetrievalByEmptyList()
	  {
		given().queryParam("decisionDefinitionIdIn", "").then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery, never()).decisionDefinitionIdIn(Matchers.anyVararg<string[]>());
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		// assert query invocation
		verify(mockedQuery).decisionDefinitionId(queryParameters["decisionDefinitionId"]);
		verify(mockedQuery).decisionDefinitionCategory(queryParameters["category"]);
		verify(mockedQuery).decisionDefinitionCategoryLike(queryParameters["categoryLike"]);
		verify(mockedQuery).decisionDefinitionName(queryParameters["name"]);
		verify(mockedQuery).decisionDefinitionNameLike(queryParameters["nameLike"]);
		verify(mockedQuery).deploymentId(queryParameters["deploymentId"]);
		verify(mockedQuery).decisionDefinitionKey(queryParameters["key"]);
		verify(mockedQuery).decisionDefinitionKeyLike(queryParameters["keyLike"]);
		verify(mockedQuery).decisionDefinitionVersion(int.Parse(queryParameters["version"]));
		verify(mockedQuery).latestVersion();
		verify(mockedQuery).decisionDefinitionResourceName(queryParameters["resourceName"]);
		verify(mockedQuery).decisionDefinitionResourceNameLike(queryParameters["resourceNameLike"]);
		verify(mockedQuery).decisionRequirementsDefinitionId(queryParameters["decisionRequirementsDefinitionId"]);
		verify(mockedQuery).decisionRequirementsDefinitionKey(queryParameters["decisionRequirementsDefinitionKey"]);
		verify(mockedQuery).versionTag(queryParameters["versionTag"]);
		verify(mockedQuery).versionTagLike(queryParameters["versionTagLike"]);
		verify(mockedQuery).withoutDecisionRequirementsDefinition();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionTenantIdList()
	  public virtual void testDecisionDefinitionTenantIdList()
	  {
		IList<DecisionDefinition> decisionDefinitions = Arrays.asList(MockProvider.mockDecisionDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockDecisionDefinition().id(MockProvider.ANOTHER_EXAMPLE_CASE_DEFINITION_ID).tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
		mockedQuery = createMockDecisionDefinitionQuery(decisionDefinitions);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

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
//ORIGINAL LINE: @Test public void testDecisionDefinitionWithoutTenantId()
	  public virtual void testDecisionDefinitionWithoutTenantId()
	  {
		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionTenantIdIncludeDefinitionsWithoutTenantid()
	  public virtual void testDecisionDefinitionTenantIdIncludeDefinitionsWithoutTenantid()
	  {
		IList<DecisionDefinition> decisionDefinitions = Arrays.asList(MockProvider.mockDecisionDefinition().tenantId(null).build(), MockProvider.mockDecisionDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockedQuery = createMockDecisionDefinitionQuery(decisionDefinitions);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID).queryParam("includeDecisionDefinitionsWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockedQuery).includeDecisionDefinitionsWithoutTenantId();
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
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(DECISION_DEFINITION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionVersionTag()
	  public virtual void testDecisionDefinitionVersionTag()
	  {
		IList<DecisionDefinition> decisionDefinitions = Arrays.asList(MockProvider.mockDecisionDefinition().versionTag(MockProvider.EXAMPLE_VERSION_TAG).build(), MockProvider.mockDecisionDefinition().id(MockProvider.ANOTHER_EXAMPLE_DECISION_DEFINITION_ID).versionTag(MockProvider.ANOTHER_EXAMPLE_VERSION_TAG).build());
		mockedQuery = createMockDecisionDefinitionQuery(decisionDefinitions);

		given().queryParam("versionTag", MockProvider.EXAMPLE_VERSION_TAG).then().expect().statusCode(Status.OK.StatusCode).when().get(DECISION_DEFINITION_QUERY_URL);

		verify(mockedQuery).versionTag(MockProvider.EXAMPLE_VERSION_TAG);
		verify(mockedQuery).list();
	  }

	  private IDictionary<string, string> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["decisionDefinitionId"] = "anId";
			parameters["category"] = "cat";
			parameters["categoryLike"] = "catlike";
			parameters["name"] = "name";
			parameters["nameLike"] = "namelike";
			parameters["deploymentId"] = "depId";
			parameters["key"] = "key";
			parameters["keyLike"] = "keylike";
			parameters["version"] = "1";
			parameters["latestVersion"] = "true";
			parameters["resourceName"] = "res";
			parameters["resourceNameLike"] = "resLike";
			parameters["decisionRequirementsDefinitionId"] = MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID;
			parameters["decisionRequirementsDefinitionKey"] = MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY;
			parameters["withoutDecisionRequirementsDefinition"] = "true";
			parameters["versionTag"] = "semVer";
			parameters["versionTagLike"] = "semVerLike";
    
			return parameters;
		  }
	  }

	}

}