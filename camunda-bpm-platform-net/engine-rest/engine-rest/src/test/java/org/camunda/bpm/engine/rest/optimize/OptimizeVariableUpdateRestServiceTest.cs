﻿using System;

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
namespace org.camunda.bpm.engine.rest.optimize
{
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using MockHistoricVariableUpdateBuilder = org.camunda.bpm.engine.rest.helper.MockHistoricVariableUpdateBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;



//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	public class OptimizeVariableUpdateRestServiceTest : AbstractRestServiceTest
	{

	  public static readonly string OPTIMIZE_VARIABLE_UPDATE_PATH = TEST_RESOURCE_ROOT_PATH + "/optimize/variable-update";

	  protected internal OptimizeService mockedOptimizeService;
	  protected internal ProcessEngine namedProcessEngine;

	  protected internal HistoricVariableUpdate historicUpdateMock;
	  protected internal MockHistoricVariableUpdateBuilder historicUpdateBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historicUpdateBuilder = MockProvider.mockHistoricVariableUpdate();
		historicUpdateMock = historicUpdateBuilder.build();

		mockedOptimizeService = mock(typeof(OptimizeService));
		ProcessEngineConfigurationImpl mockedConfig = mock(typeof(ProcessEngineConfigurationImpl));

		when(mockedOptimizeService.getHistoricVariableUpdates(any(typeof(DateTime)), any(typeof(DateTime)), anyInt())).thenReturn(Arrays.asList(historicUpdateMock));

		namedProcessEngine = getProcessEngine(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
		when(namedProcessEngine.ProcessEngineConfiguration).thenReturn(mockedConfig);
		when(mockedConfig.OptimizeService).thenReturn(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoQueryParameters()
	  public virtual void testNoQueryParameters()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(null, null, int.MaxValue);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOccurredAfterQueryParameter()
	  public virtual void testOccurredAfterQueryParameter()
	  {
		DateTime now = DateTime.Now;
		given().queryParam("occurredAfter", DATE_FORMAT_WITH_TIMEZONE.format(now)).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(now, null, int.MaxValue);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOccurredAtQueryParameter()
	  public virtual void testOccurredAtQueryParameter()
	  {
		DateTime now = DateTime.Now;
		given().queryParam("occurredAt", DATE_FORMAT_WITH_TIMEZONE.format(now)).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(null, now, int.MaxValue);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMaxResultsQueryParameter()
	  public virtual void testMaxResultsQueryParameter()
	  {
		given().queryParam("maxResults", 10).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(null, null, 10);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryParameterCombination()
	  public virtual void testQueryParameterCombination()
	  {
		DateTime now = DateTime.Now;
		given().queryParam("occurredAfter", DATE_FORMAT_WITH_TIMEZONE.format(now)).queryParam("occurredAt", DATE_FORMAT_WITH_TIMEZONE.format(now)).queryParam("maxResults", 10).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(now, now, 10);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWhenFileWasDeleted()
	  public virtual void testQueryWhenFileWasDeleted()
	  {
		doThrow(new System.ArgumentException("Parameter 'filename' is null")).when(historicUpdateMock).TypedValue;

		given().then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(OPTIMIZE_VARIABLE_UPDATE_PATH);

		verify(mockedOptimizeService).getHistoricVariableUpdates(null, null, int.MaxValue);
		verifyNoMoreInteractions(mockedOptimizeService);
	  }
	}

}