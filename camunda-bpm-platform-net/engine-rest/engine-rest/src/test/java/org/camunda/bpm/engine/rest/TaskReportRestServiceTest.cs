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
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using TaskReportResultToCsvConverter = org.camunda.bpm.engine.rest.dto.converter.TaskReportResultToCsvConverter;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using TaskReport = org.camunda.bpm.engine.task.TaskReport;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_GROUP_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockTaskCountByCandidateGroupReport;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskReportRestServiceTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string TASK_REPORT_URL = TEST_RESOURCE_ROOT_PATH + "/task/report";
	  protected internal static readonly string CANDIDATE_GROUP_REPORT_URL = TASK_REPORT_URL + "/candidate-group-count";

	  protected internal TaskReport mockedReportQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedReportQuery = setUpMockHistoricProcessInstanceReportQuery();
	  }

	  private TaskReport setUpMockHistoricProcessInstanceReportQuery()
	  {
		TaskReport mockedReportQuery = mock(typeof(TaskReport));

		IList<TaskCountByCandidateGroupResult> taskCountByCandidateGroupResults = createMockTaskCountByCandidateGroupReport();
		when(mockedReportQuery.taskCountByCandidateGroup()).thenReturn(taskCountByCandidateGroupResults);

		when(processEngine.TaskService.createTaskReport()).thenReturn(mockedReportQuery);

		return mockedReportQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyReport()
	  public virtual void testEmptyReport()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(CANDIDATE_GROUP_REPORT_URL);

		verify(mockedReportQuery).taskCountByCandidateGroup();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.taskCountByCandidateGroup()).thenThrow(new AuthorizationException(message));

		given().then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(CANDIDATE_GROUP_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountByCandidateGroupReport()
	  public virtual void testTaskCountByCandidateGroupReport()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(CANDIDATE_GROUP_REPORT_URL);

		string content = response.asString();
		IList<string> reports = from(content).getList("");
		Assert.assertEquals("There should be one report returned.", 1, reports.Count);
		Assert.assertNotNull("The returned report should not be null.", reports[0]);

		string returnedGroup = from(content).getString("[0].groupName");
		int returnedCount = from(content).getInt("[0].taskCount");

		Assert.assertEquals(EXAMPLE_GROUP_ID, returnedGroup);
		Assert.assertEquals(EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP, returnedCount);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyCsvReport()
	  public virtual void testEmptyCsvReport()
	  {
		given().accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").when().get(CANDIDATE_GROUP_REPORT_URL);

		verify(mockedReportQuery).taskCountByCandidateGroup();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvTaskCountByCandidateGroupReport()
	  public virtual void testCsvTaskCountByCandidateGroupReport()
	  {
		Response response = given().accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=task-count-by-candidate-group.csv").when().get(CANDIDATE_GROUP_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(TaskReportResultToCsvConverter.CANDIDATE_GROUP_HEADER));
		assertTrue(responseContent.Contains(EXAMPLE_GROUP_ID));
		assertTrue(responseContent.Contains(EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testApplicationCsvTaskCountByCandidateGroupReport()
	  public virtual void testApplicationCsvTaskCountByCandidateGroupReport()
	  {
		Response response = given().accept("application/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("application/csv").header("Content-Disposition", "attachment; filename=task-count-by-candidate-group.csv").when().get(CANDIDATE_GROUP_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(TaskReportResultToCsvConverter.CANDIDATE_GROUP_HEADER));
		assertTrue(responseContent.Contains(EXAMPLE_GROUP_ID));
		assertTrue(responseContent.Contains(EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP.ToString()));
	  }
	}

}