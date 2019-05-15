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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultBatchWindowManager = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.DefaultBatchWindowManager;
	using HistoryCleanupHelper = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHelper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using JacksonConfigurator = org.camunda.bpm.engine.rest.mapper.JacksonConfigurator;
	using DateTimeUtils = org.camunda.bpm.engine.rest.util.DateTimeUtils;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ContentType = io.restassured.http.ContentType;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;


	public class HistoryCleanupRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_CLEANUP_URL = TEST_RESOURCE_ROOT_PATH + "/history/cleanup";
	  protected internal static readonly string FIND_HISTORY_CLEANUP_JOB_URL = HISTORY_CLEANUP_URL + "/job";
	  protected internal static readonly string FIND_HISTORY_CLEANUP_JOBS_URL = HISTORY_CLEANUP_URL + "/jobs";
	  protected internal static readonly string CONFIGURATION_URL = HISTORY_CLEANUP_URL + "/configuration";

	  private HistoryService historyServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historyServiceMock = mock(typeof(HistoryService));
		Job mockJob = MockProvider.createMockJob();
		IList<Job> mockJobs = MockProvider.createMockJobs();
		when(historyServiceMock.cleanUpHistoryAsync(anyBoolean())).thenReturn(mockJob);
		when(historyServiceMock.findHistoryCleanupJob()).thenReturn(mockJob);
		when(historyServiceMock.findHistoryCleanupJobs()).thenReturn(mockJobs);

		// runtime service
		when(processEngine.HistoryService).thenReturn(historyServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindHistoryCleanupJob()
	  public virtual void testFindHistoryCleanupJob()
	  {
		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(FIND_HISTORY_CLEANUP_JOB_URL);

	   verify(historyServiceMock).findHistoryCleanupJob();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindNoHistoryCleanupJob()
	  public virtual void testFindNoHistoryCleanupJob()
	  {
		when(historyServiceMock.findHistoryCleanupJob()).thenReturn(null);

		given().contentType(ContentType.JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(FIND_HISTORY_CLEANUP_JOB_URL);

	   verify(historyServiceMock).findHistoryCleanupJob();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindHistoryCleanupJobs()
	  public virtual void testFindHistoryCleanupJobs()
	  {
		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(FIND_HISTORY_CLEANUP_JOBS_URL);

	   verify(historyServiceMock).findHistoryCleanupJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindNoHistoryCleanupJobs()
	  public virtual void testFindNoHistoryCleanupJobs()
	  {
		when(historyServiceMock.findHistoryCleanupJobs()).thenReturn(null);

		given().contentType(ContentType.JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(FIND_HISTORY_CLEANUP_JOBS_URL);

	   verify(historyServiceMock).findHistoryCleanupJobs();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupImmediatelyDueDefault()
	  public virtual void testHistoryCleanupImmediatelyDueDefault()
	  {
		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORY_CLEANUP_URL);

		verify(historyServiceMock).cleanUpHistoryAsync(true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupImmediatelyDue()
	  public virtual void testHistoryCleanupImmediatelyDue()
	  {
		given().contentType(ContentType.JSON).queryParam("immediatelyDue", true).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORY_CLEANUP_URL);

		verify(historyServiceMock).cleanUpHistoryAsync(true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanup()
	  public virtual void testHistoryCleanup()
	  {
		given().contentType(ContentType.JSON).queryParam("immediatelyDue", false).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORY_CLEANUP_URL);

		verify(historyServiceMock).cleanUpHistoryAsync(false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryConfigurationOutsideBatchWindow() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoryConfigurationOutsideBatchWindow()
	  {
		ProcessEngineConfigurationImpl processEngineConfigurationImplMock = mock(typeof(ProcessEngineConfigurationImpl));
		DateTime startDate = HistoryCleanupHelper.parseTimeConfiguration("23:59+0200");
		DateTime endDate = HistoryCleanupHelper.parseTimeConfiguration("00:00+0200");
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationImplMock);
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowStartTime).thenReturn("23:59+0200");
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowEndTime).thenReturn("00:00+0200");
		when(processEngineConfigurationImplMock.BatchWindowManager).thenReturn(new DefaultBatchWindowManager());

		SimpleDateFormat sdf = new SimpleDateFormat(JacksonConfigurator.dateFormatString);
		DateTime now = sdf.parse("2017-09-01T22:00:00.000+0200");

		ClockUtil.CurrentTime = now;
		DateTime today = new DateTime();
		today = new DateTime(now);
		DateTime tomorrow = new DateTime();
		tomorrow = new DateTime(DateTimeUtils.addDays(now, 1));

		DateTime dateToday = DateTimeUtils.updateTime(today, startDate);
		DateTime dateTomorrow = DateTimeUtils.updateTime(tomorrow, endDate);

		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).body("batchWindowStartTime", containsString(sdf.format(dateToday))).body("batchWindowEndTime", containsString(sdf.format(dateTomorrow))).when().get(CONFIGURATION_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryConfigurationWithinBatchWindow() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoryConfigurationWithinBatchWindow()
	  {
		ProcessEngineConfigurationImpl processEngineConfigurationImplMock = mock(typeof(ProcessEngineConfigurationImpl));
		DateTime startDate = HistoryCleanupHelper.parseTimeConfiguration("22:00+0200");
		DateTime endDate = HistoryCleanupHelper.parseTimeConfiguration("23:00+0200");
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationImplMock);
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowStartTime).thenReturn("22:00+0200");
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowEndTime).thenReturn("23:00+0200");
		when(processEngineConfigurationImplMock.BatchWindowManager).thenReturn(new DefaultBatchWindowManager());

		SimpleDateFormat sdf = new SimpleDateFormat(JacksonConfigurator.dateFormatString);
		DateTime now = sdf.parse("2017-09-01T22:00:00.000+0200");
		ClockUtil.CurrentTime = now;

		DateTime today = new DateTime();
		today = new DateTime(now);

		DateTime dateToday = DateTimeUtils.updateTime(today, startDate);
		DateTime dateTomorrow = DateTimeUtils.updateTime(today, endDate);

		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).body("batchWindowStartTime", containsString(sdf.format(dateToday))).body("batchWindowEndTime", containsString(sdf.format(dateTomorrow))).when().get(CONFIGURATION_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryConfigurationWhenBatchNotDefined()
	  public virtual void testHistoryConfigurationWhenBatchNotDefined()
	  {
		ProcessEngineConfigurationImpl processEngineConfigurationImplMock = mock(typeof(ProcessEngineConfigurationImpl));
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationImplMock);
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowStartTime).thenReturn(null);
		when(processEngineConfigurationImplMock.HistoryCleanupBatchWindowEndTime).thenReturn(null);
		when(processEngineConfigurationImplMock.BatchWindowManager).thenReturn(new DefaultBatchWindowManager());

		given().contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).body("batchWindowStartTime", equalTo(null)).body("batchWindowEndTime", equalTo(null)).when().get(CONFIGURATION_URL);

	  }

	}

}