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
	using HistoricDecisionInstanceStatistics = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatistics;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Mockito = org.mockito.Mockito;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class HistoricDecisionStatisticsRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history";
	  protected internal static readonly string HISTORIC_DECISION_STATISTICS_URL = HISTORY_URL + "/decision-requirements-definition/{id}/statistics";

	  private HistoricDecisionInstanceStatisticsQuery historicDecisionInstanceStatisticsQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoricDecisionStatisticsMock();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		Mockito.reset(processEngine.HistoryService, historicDecisionInstanceStatisticsQuery);
	  }

	  private void setupHistoricDecisionStatisticsMock()
	  {
		IList<HistoricDecisionInstanceStatistics> mocks = MockProvider.createMockHistoricDecisionStatistics();

		historicDecisionInstanceStatisticsQuery = mock(typeof(HistoricDecisionInstanceStatisticsQuery));
		when(processEngine.HistoryService.createHistoricDecisionInstanceStatisticsQuery(eq(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID))).thenReturn(historicDecisionInstanceStatisticsQuery);

		when(historicDecisionInstanceStatisticsQuery.decisionInstanceId(MockProvider.EXAMPLE_DECISION_INSTANCE_ID)).thenReturn(historicDecisionInstanceStatisticsQuery);
		when(historicDecisionInstanceStatisticsQuery.list()).thenReturn(mocks);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDefinitionInstanceStatisticsRetrieval()
	  public virtual void testHistoricDefinitionInstanceStatisticsRetrieval()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).then().expect().statusCode(Response.Status.OK.StatusCode).body("$.size()", @is(2)).body("decisionDefinitionKey", hasItems(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY, MockProvider.ANOTHER_DECISION_DEFINITION_KEY)).body("evaluations", hasItems(1, 2)).when().get(HISTORIC_DECISION_STATISTICS_URL);

		verify(processEngine.HistoryService).createHistoricDecisionInstanceStatisticsQuery(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDefinitionInstanceStatisticsRetrievalWithDefinitionInstance()
	  public virtual void testHistoricDefinitionInstanceStatisticsRetrievalWithDefinitionInstance()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).queryParam("decisionInstanceId", MockProvider.EXAMPLE_DECISION_INSTANCE_ID).then().expect().statusCode(Response.Status.OK.StatusCode).body("$.size()", @is(2)).body("decisionDefinitionKey", hasItems(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY, MockProvider.ANOTHER_DECISION_DEFINITION_KEY)).body("evaluations", hasItems(1, 2)).when().get(HISTORIC_DECISION_STATISTICS_URL);

		verify(processEngine.HistoryService).createHistoricDecisionInstanceStatisticsQuery(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		verify(historicDecisionInstanceStatisticsQuery).decisionInstanceId(MockProvider.EXAMPLE_DECISION_INSTANCE_ID);
	  }
	}

}