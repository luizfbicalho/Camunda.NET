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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public class CustomJacksonDateFormatTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/process-instance";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_URL = PROCESS_INSTANCE_URL + "/{id}";
	  protected internal static readonly string PROCESS_INSTANCE_VARIABLES_URL = SINGLE_PROCESS_INSTANCE_URL + "/variables";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_VARIABLE_URL = PROCESS_INSTANCE_VARIABLES_URL + "/{varId}";

	  protected internal static readonly SimpleDateFormat testDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
	  protected internal static readonly DateTime testDate = new DateTime(1450282812000L);
	  protected internal static readonly string testDateFormatted = testDateFormat.format(testDate);

	  protected internal RuntimeServiceImpl runtimeServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeServiceImpl));

		when(runtimeServiceMock.getVariableTyped(eq(EXAMPLE_PROCESS_INSTANCE_ID), eq(EXAMPLE_VARIABLE_KEY), eq(true))).thenReturn(Variables.dateValue(testDate));

		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDateVariable()
	  public virtual void testGetDateVariable()
	  {
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(testDateFormatted)).body("type", @is("Date")).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetDateVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetDateVariable()
	  {
		string variableValue = testDateFormat.format(testDate);

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, "Date");

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(EXAMPLE_PROCESS_INSTANCE_ID), eq(EXAMPLE_VARIABLE_KEY), argThat(EqualsPrimitiveValue.dateValue(testDate)));
	  }

	}

}