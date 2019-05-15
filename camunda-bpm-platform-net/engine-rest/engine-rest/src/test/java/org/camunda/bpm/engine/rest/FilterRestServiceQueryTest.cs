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
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using FilterQueryDto = org.camunda.bpm.engine.rest.dto.runtime.FilterQueryDto;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string FILTER_QUERY_URL = TEST_RESOURCE_ROOT_PATH + FilterRestService_Fields.PATH;
	  protected internal static readonly string SINGLE_FILTER_URL = FILTER_QUERY_URL + "/{id}";
	  protected internal static readonly string FILTER_COUNT_QUERY_URL = FILTER_QUERY_URL + "/count";

	  protected internal FilterQuery mockedQuery;
	  protected internal Filter mockedFilter;
	  protected internal int mockedFilterItemCount;
	  protected internal Filter anotherMockedFilter;
	  protected internal int anotherMockedFilterItemCount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = MockProvider.createMockFilterQuery();
		mockedFilter = MockProvider.createMockFilter(MockProvider.EXAMPLE_FILTER_ID);
		mockedFilterItemCount = 13;
		anotherMockedFilter = MockProvider.createMockFilter(MockProvider.ANOTHER_EXAMPLE_FILTER_ID);
		anotherMockedFilterItemCount = 42;

		FilterService filterService = processEngine.FilterService;

		when(filterService.createFilterQuery()).thenReturn(mockedQuery);
		when(filterService.getFilter(eq(MockProvider.EXAMPLE_FILTER_ID))).thenReturn(mockedFilter);
		when(filterService.count(eq(MockProvider.EXAMPLE_FILTER_ID))).thenReturn((long) mockedFilterItemCount);
		when(filterService.getFilter(eq(MockProvider.ANOTHER_EXAMPLE_FILTER_ID))).thenReturn(anotherMockedFilter);
		when(filterService.count(eq(MockProvider.ANOTHER_EXAMPLE_FILTER_ID))).thenReturn((long) anotherMockedFilterItemCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryFilterId = "";

		given().queryParam("filterId", queryFilterId).then().expect().statusCode(Status.OK.StatusCode).when().get(FILTER_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(FILTER_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCountyQuery()
	  public virtual void testCountyQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(FILTER_COUNT_QUERY_URL);

		verify(mockedQuery).count();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterRetrieval()
	  public virtual void testFilterRetrieval()
	  {
		Response response = given().queryParam("filterId", MockProvider.EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(FILTER_QUERY_URL);

		verifyFilterMock(response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParameters()
	  public virtual void testMultipleParameters()
	  {
		given().queryParams(QueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(FILTER_QUERY_URL);

		verifyQueryMockMultipleParameters();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("filterId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "filterId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(FILTER_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(FILTER_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		executeAndVerifyPagination(0, 10, Status.OK);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		executeAndVerifyPagination(null, 10, Status.OK);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		executeAndVerifyPagination(0, null, Status.OK);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSingleFilterWithItemCount()
	  public virtual void testSingleFilterWithItemCount()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_FILTER_ID).queryParam("itemCount", true).then().expect().statusCode(Status.OK.StatusCode).body("containsKey('itemCount')", @is(true)).body("itemCount", equalTo(mockedFilterItemCount)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSingleFilterWithoutItemCount()
	  public virtual void testSingleFilterWithoutItemCount()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("containsKey('itemCount')", @is(false)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterQueryWithItemCount()
	  public virtual void testFilterQueryWithItemCount()
	  {
		given().queryParam("itemCount", true).then().expect().statusCode(Status.OK.StatusCode).body("$.size", equalTo(2)).body("any { it.containsKey('itemCount') }", @is(true)).body("[0].itemCount", equalTo(mockedFilterItemCount)).body("[1].itemCount", equalTo(anotherMockedFilterItemCount)).when().get(FILTER_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterQueryWithoutItemCount()
	  public virtual void testFilterQueryWithoutItemCount()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).body("$.size", equalTo(2)).body("any { it.containsKey('itemCount') }", @is(false)).when().get(FILTER_QUERY_URL);
	  }

	  protected internal virtual IDictionary<string, string> QueryParameters
	  {
		  get
		  {
			IDictionary<string, string> @params = new Dictionary<string, string>();
    
			@params["filterId"] = MockProvider.EXAMPLE_FILTER_ID;
			@params["resourceType"] = MockProvider.EXAMPLE_FILTER_RESOURCE_TYPE;
			@params["name"] = MockProvider.EXAMPLE_FILTER_NAME;
			@params["nameLike"] = MockProvider.EXAMPLE_FILTER_NAME;
			@params["owner"] = MockProvider.EXAMPLE_FILTER_OWNER;
    
			return @params;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void verifyFilterMock(io.restassured.response.Response response)
	  protected internal virtual void verifyFilterMock(Response response)
	  {
		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).filterId(MockProvider.EXAMPLE_FILTER_ID);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> filters = from(content).getList("");

		assertThat(filters).hasSize(2);

		assertThat(filters[0]).NotNull;

		string returnedFilterId = from(content).getString("[0].id");
		string returnedResourceType = from(content).getString("[0].resourceType");
		string returnedName = from(content).getString("[0].name");
		string returnedOwner = from(content).getString("[0].owner");
		IDictionary<string, object> returnedQuery = from(content).getJsonObject("[0].query");
		IDictionary<string, object> returnedProperties = from(content).getJsonObject("[0].properties");

		IDictionary<string, string> expectedVariable = new Dictionary<string, string>();
		expectedVariable["name"] = "foo";
		expectedVariable["value"] = "bar";
		expectedVariable["operator"] = "eq";

		assertThat(returnedFilterId).isEqualTo(MockProvider.EXAMPLE_FILTER_ID);
		assertThat(returnedResourceType).isEqualTo(MockProvider.EXAMPLE_FILTER_RESOURCE_TYPE);
		assertThat(returnedName).isEqualTo(MockProvider.EXAMPLE_FILTER_NAME);
		assertThat(returnedOwner).isEqualTo(MockProvider.EXAMPLE_FILTER_OWNER);
		assertThat(returnedQuery["name"]).isEqualTo(MockProvider.EXAMPLE_FILTER_QUERY_DTO.Name);
		assertThat((IList<IDictionary<string, string>>) returnedQuery["processVariables"]).hasSize(1).containsExactly(expectedVariable);
		assertThat((IList<IDictionary<string, string>>) returnedQuery["taskVariables"]).hasSize(1).containsExactly(expectedVariable);
		assertThat((IList<IDictionary<string, string>>) returnedQuery["caseInstanceVariables"]).hasSize(1).containsExactly(expectedVariable);
		assertThat(returnedProperties).isEqualTo(MockProvider.EXAMPLE_FILTER_PROPERTIES);

		assertThat(filters[1]).NotNull;

		returnedFilterId = from(content).getString("[1].id");
		returnedResourceType = from(content).getString("[1].resourceType");
		returnedName = from(content).getString("[1].name");
		returnedOwner = from(content).getString("[1].owner");
		returnedQuery = from(content).getJsonObject("[1].query");
		returnedProperties = from(content).getJsonObject("[1].properties");

		assertThat(returnedFilterId).isEqualTo(MockProvider.ANOTHER_EXAMPLE_FILTER_ID);
		assertThat(returnedResourceType).isEqualTo(MockProvider.EXAMPLE_FILTER_RESOURCE_TYPE);
		assertThat(returnedName).isEqualTo(MockProvider.EXAMPLE_FILTER_NAME);
		assertThat(returnedOwner).isEqualTo(MockProvider.EXAMPLE_FILTER_OWNER);
		assertThat(returnedQuery["name"]).isEqualTo(MockProvider.EXAMPLE_FILTER_QUERY_DTO.Name);
		assertThat(returnedProperties).isEqualTo(MockProvider.EXAMPLE_FILTER_PROPERTIES);
	  }

	  protected internal virtual void verifyQueryMockMultipleParameters()
	  {
		verify(mockedQuery).filterId(MockProvider.EXAMPLE_FILTER_ID);
		verify(mockedQuery).filterResourceType(MockProvider.EXAMPLE_FILTER_RESOURCE_TYPE);
		verify(mockedQuery).filterName(MockProvider.EXAMPLE_FILTER_NAME);
		verify(mockedQuery).filterNameLike(MockProvider.EXAMPLE_FILTER_NAME);
		verify(mockedQuery).filterOwner(MockProvider.EXAMPLE_FILTER_OWNER);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["sortBy"] = sortBy;
		@params["sortOrder"] = sortOrder;

		given().queryParams(@params).then().expect().statusCode(expectedStatus.StatusCode).when().get(FILTER_QUERY_URL);

		if (expectedStatus.Equals(Status.OK))
		{
		  verifyQueryMockSorting(sortBy, sortOrder);
		}
	  }

	  protected internal virtual void verifyQueryMockSorting(string sortBy, string sortOrder)
	  {
		InOrder inOrder = inOrder(mockedQuery);
		if (sortBy.Equals(FilterQueryDto.SORT_BY_ID_VALUE))
		{
		  inOrder.verify(mockedQuery).orderByFilterId();
		}
		else if (sortBy.Equals(FilterQueryDto.SORT_BY_RESOURCE_TYPE_VALUE))
		{
		  inOrder.verify(mockedQuery).orderByFilterResourceType();
		}
		else if (sortBy.Equals(FilterQueryDto.SORT_BY_NAME_VALUE))
		{
		  inOrder.verify(mockedQuery).orderByFilterName();
		}
		else if (sortBy.Equals(FilterQueryDto.SORT_BY_OWNER_VALUE))
		{
		  inOrder.verify(mockedQuery).orderByFilterOwner();
		}

		if (sortOrder.Equals(AbstractQuery.SORTORDER_ASC))
		{
		  inOrder.verify(mockedQuery).asc();
		}
		else if (sortOrder.Equals(AbstractQuery.SORTORDER_DESC))
		{
		  inOrder.verify(mockedQuery).desc();
		}
	  }

	  protected internal virtual void executeAndVerifyPagination(int? firstResult, int? maxResults, Status expectedStatus)
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		if (firstResult != null)
		{
		  @params["firstResult"] = firstResult.ToString();
		}
		if (maxResults != null)
		{
		  @params["maxResults"] = maxResults.ToString();
		}

		given().queryParams(@params).then().expect().statusCode(expectedStatus.StatusCode).when().get(FILTER_QUERY_URL);

		verifyQueryMockPagination(firstResult, maxResults);

	  }

	  protected internal virtual void verifyQueryMockPagination(int? firstResult, int? maxResults)
	  {
		if (firstResult == null)
		{
		  firstResult = 0;
		}
		if (maxResults == null)
		{
		  maxResults = int.MaxValue;
		}

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

	}

}