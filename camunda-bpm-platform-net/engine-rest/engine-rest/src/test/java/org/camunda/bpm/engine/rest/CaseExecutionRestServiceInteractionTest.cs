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
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using VariableNameDto = org.camunda.bpm.engine.rest.dto.runtime.VariableNameDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using EqualsNullValue = org.camunda.bpm.engine.rest.helper.variable.EqualsNullValue;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionCommandBuilder = org.camunda.bpm.engine.runtime.CaseExecutionCommandBuilder;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using Matchers = org.mockito.Matchers;


	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using TypeFactory = com.fasterxml.jackson.databind.type.TypeFactory;
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string CASE_EXECUTION_URL = TEST_RESOURCE_ROOT_PATH + "/case-execution";
	  protected internal static readonly string SINGLE_CASE_EXECUTION_URL = CASE_EXECUTION_URL + "/{id}";

	  protected internal static readonly string CASE_EXECUTION_MANUAL_START_URL = SINGLE_CASE_EXECUTION_URL + "/manual-start";
	  protected internal static readonly string CASE_EXECUTION_REENABLE_URL = SINGLE_CASE_EXECUTION_URL + "/reenable";
	  protected internal static readonly string CASE_EXECUTION_DISABLE_URL = SINGLE_CASE_EXECUTION_URL + "/disable";
	  protected internal static readonly string CASE_EXECUTION_COMPLETE_URL = SINGLE_CASE_EXECUTION_URL + "/complete";
	  protected internal static readonly string CASE_EXECUTION_TERMINATE_URL = SINGLE_CASE_EXECUTION_URL + "/terminate";

	  protected internal static readonly string CASE_EXECUTION_LOCAL_VARIABLES_URL = SINGLE_CASE_EXECUTION_URL + "/localVariables";
	  protected internal static readonly string CASE_EXECUTION_VARIABLES_URL = SINGLE_CASE_EXECUTION_URL + "/variables";
	  protected internal static readonly string SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL = CASE_EXECUTION_LOCAL_VARIABLES_URL + "/{varId}";
	  protected internal static readonly string SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL = SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL + "/data";
	  protected internal static readonly string SINGLE_CASE_EXECUTION_VARIABLE_URL = CASE_EXECUTION_VARIABLES_URL + "/{varId}";
	  protected internal static readonly string SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL = SINGLE_CASE_EXECUTION_VARIABLE_URL + "/data";

	  private CaseService caseServiceMock;
	  private CaseExecutionQuery caseExecutionQueryMock;
	  private CaseExecutionCommandBuilder caseExecutionCommandBuilderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntime()
	  public virtual void setUpRuntime()
	  {
		CaseExecution mockCaseExecution = MockProvider.createMockCaseExecution();

		caseServiceMock = mock(typeof(CaseService));

		when(processEngine.CaseService).thenReturn(caseServiceMock);

		caseExecutionQueryMock = mock(typeof(CaseExecutionQuery));

		when(caseServiceMock.createCaseExecutionQuery()).thenReturn(caseExecutionQueryMock);
		when(caseExecutionQueryMock.caseExecutionId(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).thenReturn(caseExecutionQueryMock);
		when(caseExecutionQueryMock.singleResult()).thenReturn(mockCaseExecution);

		when(caseServiceMock.getVariableTyped(anyString(), anyString(), eq(true))).thenReturn(EXAMPLE_VARIABLE_VALUE);
		when(caseServiceMock.getVariablesTyped(anyString(), eq(true))).thenReturn(EXAMPLE_VARIABLES);

		when(caseServiceMock.getVariableLocalTyped(anyString(), eq(EXAMPLE_VARIABLE_KEY), anyBoolean())).thenReturn(EXAMPLE_VARIABLE_VALUE);
		when(caseServiceMock.getVariableLocalTyped(anyString(), eq(EXAMPLE_BYTES_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE_BYTES);
		when(caseServiceMock.getVariablesLocalTyped(anyString(), eq(true))).thenReturn(EXAMPLE_VARIABLES);

		when(caseServiceMock.getVariablesTyped(anyString(), Matchers.any<ICollection<string>>(), eq(true))).thenReturn(EXAMPLE_VARIABLES);
		when(caseServiceMock.getVariablesLocalTyped(anyString(), Matchers.any<ICollection<string>>(), eq(true))).thenReturn(EXAMPLE_VARIABLES);

		caseExecutionCommandBuilderMock = mock(typeof(CaseExecutionCommandBuilder));

		when(caseServiceMock.withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).thenReturn(caseExecutionCommandBuilderMock);

		when(caseExecutionCommandBuilderMock.setVariable(anyString(), any())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.setVariableLocal(anyString(), any())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.setVariablesLocal(Matchers.any<IDictionary<string, object>>())).thenReturn(caseExecutionCommandBuilderMock);

		when(caseExecutionCommandBuilderMock.removeVariable(anyString())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.removeVariableLocal(anyString())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.removeVariables(Matchers.any<ICollection<string>>())).thenReturn(caseExecutionCommandBuilderMock);
		when(caseExecutionCommandBuilderMock.removeVariablesLocal(Matchers.any<ICollection<string>>())).thenReturn(caseExecutionCommandBuilderMock);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseExecutionRetrieval()
	  public virtual void testCaseExecutionRetrieval()
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["id"] = MockProvider.EXAMPLE_CASE_EXECUTION_ID;
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).body("caseInstanceId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_CASE_INSTANCE_ID)).body("parentId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_PARENT_ID)).body("caseDefinitionId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_CASE_DEFINITION_ID)).body("activityId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ACTIVITY_ID)).body("activityName", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ACTIVITY_NAME)).body("activityType", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ACTIVITY_TYPE)).body("activityDescription", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ACTIVITY_DESCRIPTION)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("required", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_IS_REQUIRED)).body("active", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_IS_ACTIVE)).body("enabled", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_IS_ENABLED)).body("disabled", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_IS_DISABLED)).when().get(SINGLE_CASE_EXECUTION_URL);

		verify(caseServiceMock).createCaseExecutionQuery();
		verify(caseExecutionQueryMock).caseExecutionId(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionQueryMock).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStart()
	  public virtual void testManualStart()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulManualStart()
	  public virtual void testUnsuccessfulManualStart()
	  {
		doThrow(new NotValidException("expected exception")).when(caseExecutionCommandBuilderMock).manualStart();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot manualStart case execution " + MockProvider.EXAMPLE_CASE_EXECUTION_ID + ": expected exception")).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariable()
	  public virtual void testManualStartWithSetVariable()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableLocal()
	  public virtual void testManualStartWithSetVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer", true).variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableAndVariableLocal()
	  public virtual void testManualStartWithSetVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithRemoveVariable()
	  public virtual void testManualStartWithRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithRemoveVariableLocal()
	  public virtual void testManualStartWithRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey, true);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithRemoveVariableAndVariableLocal()
	  public virtual void testManualStartWithRemoveVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableAndRemoveVariable()
	  public virtual void testManualStartWithSetVariableAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableAndRemoveVariableLocal()
	  public virtual void testManualStartWithSetVariableAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableLocalAndRemoveVariable()
	  public virtual void testManualStartWithSetVariableLocalAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualStartWithSetVariableLocalAndRemoveVariableLocal()
	  public virtual void testManualStartWithSetVariableLocalAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_MANUAL_START_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).manualStart();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisable()
	  public virtual void testDisable()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulDisable()
	  public virtual void testUnsuccessfulDisable()
	  {
		doThrow(new NotValidException("expected exception")).when(caseExecutionCommandBuilderMock).disable();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot disable case execution " + MockProvider.EXAMPLE_CASE_EXECUTION_ID + ": expected exception")).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariable()
	  public virtual void testDisableWithSetVariable()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableLocal()
	  public virtual void testDisableWithSetVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer", true).variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableAndVariableLocal()
	  public virtual void testDisableWithSetVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithRemoveVariable()
	  public virtual void testDisableWithRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithRemoveVariableLocal()
	  public virtual void testDisableWithRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey, true);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithRemoveVariableAndVariableLocal()
	  public virtual void testDisableWithRemoveVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableAndRemoveVariable()
	  public virtual void testDisableWithSetVariableAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableAndRemoveVariableLocal()
	  public virtual void testDisableWithSetVariableAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableLocalAndRemoveVariable()
	  public virtual void testDisableWithSetVariableLocalAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableWithSetVariableLocalAndRemoveVariableLocal()
	  public virtual void testDisableWithSetVariableLocalAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_DISABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).disable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenable()
	  public virtual void testReenable()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulReenable()
	  public virtual void testUnsuccessfulReenable()
	  {
		doThrow(new NotValidException("expected exception")).when(caseExecutionCommandBuilderMock).reenable();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot reenable case execution " + MockProvider.EXAMPLE_CASE_EXECUTION_ID + ": expected exception")).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariable()
	  public virtual void testReenableWithSetVariable()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableLocal()
	  public virtual void testReenableWithSetVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer", true).variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableAndVariableLocal()
	  public virtual void testReenableWithSetVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithRemoveVariable()
	  public virtual void testReenableWithRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithRemoveVariableLocal()
	  public virtual void testReenableWithRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey, true);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithRemoveVariableAndVariableLocal()
	  public virtual void testReenableWithRemoveVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableAndRemoveVariable()
	  public virtual void testReenableWithSetVariableAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableAndRemoveVariableLocal()
	  public virtual void testReenableWithSetVariableAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableLocalAndRemoveVariable()
	  public virtual void testReenableWithSetVariableLocalAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReenableWithSetVariableLocalAndRemoveVariableLocal()
	  public virtual void testReenableWithSetVariableLocalAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_REENABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).reenable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariables()
	  public virtual void testGetLocalVariables()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value", equalTo(EXAMPLE_VARIABLE_VALUE.Value)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo("String")).when().get(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());

		verify(caseServiceMock).getVariablesLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariables()
	  public virtual void testGetVariables()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value", equalTo(EXAMPLE_VARIABLE_VALUE.Value)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(EXAMPLE_VARIABLE_VALUE.Type))).when().get(CASE_EXECUTION_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());

		verify(caseServiceMock).getVariablesTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalObjectVariables()
	  public virtual void testGetLocalObjectVariables()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(caseServiceMock.getVariablesLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo(payload)).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		// then
		verify(caseServiceMock).getVariablesLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalObjectVariablesSerialized()
	  public virtual void testGetLocalObjectVariablesSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(caseServiceMock.getVariablesLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).queryParam("deserializeValues", false).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo("a serialized value")).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		// then
		verify(caseServiceMock).getVariablesLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariablesForNonExistingExecution()
	  public virtual void testGetLocalVariablesForNonExistingExecution()
	  {
		when(caseServiceMock.getVariablesLocalTyped(anyString(), anyBoolean())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingExecutionId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().get(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		verify(caseServiceMock).getVariablesLocalTyped("aNonExistingExecutionId", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesForNonExistingExecution()
	  public virtual void testGetVariablesForNonExistingExecution()
	  {
		when(caseServiceMock.getVariablesTyped(anyString(), anyBoolean())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingExecutionId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().get(CASE_EXECUTION_VARIABLES_URL);

		verify(caseServiceMock).getVariablesTyped("aNonExistingExecutionId", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariable()
	  public virtual void testGetFileVariable()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(mimeType).create();

		when(caseServiceMock.getVariableTyped(MockProvider.EXAMPLE_CASE_INSTANCE_ID, variableKey, true)).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON.ToString()).and().body("valueInfo.mimeType", equalTo(mimeType)).body("valueInfo.filename", equalTo(filename)).body("value", nullValue()).when().get(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNullFileVariable()
	  public virtual void testGetNullFileVariable()
	  {
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).mimeType(mimeType).create();

		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(""))).when().get(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithType()
	  public virtual void testGetFileVariableDownloadWithType()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).create();

		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithTypeAndEncoding()
	  public virtual void testGetFileVariableDownloadWithTypeAndEncoding()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();

		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		string contentType = response.contentType().replaceAll(" ", "");
		assertThat(contentType, @is(ContentType.TEXT + ";charset=" + encoding));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithoutType()
	  public virtual void testGetFileVariableDownloadWithoutType()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).create();

		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).header("Content-Disposition", containsString(filename)).when().get(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotDownloadVariableOtherThanFile()
	  public virtual void testCannotDownloadVariableOtherThanFile()
	  {
		string variableKey = "aVariableKey";
		BooleanValue variableValue = Variables.booleanValue(true);

		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariableModification()
	  public virtual void testLocalVariableModification()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		IList<string> deletions = new List<string>();
		deletions.Add("deleteKey");
		messageBodyJson["deletions"] = deletions;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		IDictionary<string, object> expectedMap = new Dictionary<string, object>();
		expectedMap[variableKey] = variableValue;

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariablesLocal(eq(deletions));
		verify(caseExecutionCommandBuilderMock).VariablesLocal = eq(expectedMap);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModification()
	  public virtual void testVariableModification()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		IList<string> deletions = new List<string>();
		deletions.Add("deleteKey");
		messageBodyJson["deletions"] = deletions;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_VARIABLES_URL);

		IDictionary<string, object> expectedMap = new Dictionary<string, object>();
		expectedMap[variableKey] = variableValue;

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariables(eq(deletions));
		verify(caseExecutionCommandBuilderMock).Variables = eq(expectedMap);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariableModificationForNonExistingExecution()
	  public virtual void testLocalVariableModificationForNonExistingExecution()
	  {
		when(caseServiceMock.withCaseExecution("aNonExistingExecutionId")).thenReturn(caseExecutionCommandBuilderMock);

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;

		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", "aNonExistingExecutionId").contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot modify variables for case execution " + "aNonExistingExecutionId" + ": expected exception")).when().post(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		IDictionary<string, object> expectedMap = new Dictionary<string, object>();
		expectedMap[variableKey] = variableValue;

		verify(caseServiceMock).withCaseExecution("aNonExistingExecutionId");
		verify(caseExecutionCommandBuilderMock).removeVariablesLocal(null);
		verify(caseExecutionCommandBuilderMock).VariablesLocal = eq(expectedMap);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationForNonExistingExecution()
	  public virtual void testVariableModificationForNonExistingExecution()
	  {
		when(caseServiceMock.withCaseExecution("aNonExistingExecutionId")).thenReturn(caseExecutionCommandBuilderMock);

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;

		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", "aNonExistingExecutionId").contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot modify variables for case execution " + "aNonExistingExecutionId" + ": expected exception")).when().post(CASE_EXECUTION_VARIABLES_URL);

		IDictionary<string, object> expectedMap = new Dictionary<string, object>();
		expectedMap[variableKey] = variableValue;

		verify(caseServiceMock).withCaseExecution("aNonExistingExecutionId");
		verify(caseExecutionCommandBuilderMock).removeVariables(null);
		verify(caseExecutionCommandBuilderMock).Variables = eq(expectedMap);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyLocalVariableModification()
	  public virtual void testEmptyLocalVariableModification()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_LOCAL_VARIABLES_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariablesLocal(null);
		verify(caseExecutionCommandBuilderMock).VariablesLocal = null;
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyVariableModification()
	  public virtual void testEmptyVariableModification()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_VARIABLES_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariables(null);
		verify(caseExecutionCommandBuilderMock).Variables = null;
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariable()
	  public virtual void testGetSingleLocalVariable()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(EXAMPLE_VARIABLE_VALUE.Value)).body("type", @is(VariableTypeHelper.toExpectedValueTypeName(EXAMPLE_VARIABLE_VALUE.Type))).when().get(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, EXAMPLE_VARIABLE_KEY, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableData()
	  public virtual void testGetSingleLocalVariableData()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_BYTES_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).when().get(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, EXAMPLE_BYTES_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableDataNonExisting()
	  public virtual void testGetSingleLocalVariableDataNonExisting()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", "nonExisting").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("case execution variable with name " + "nonExisting" + " does not exist")).when().get(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, "nonExisting", false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariabledataNotBinary()
	  public virtual void testGetSingleLocalVariabledataNotBinary()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, EXAMPLE_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariable()
	  public virtual void testGetSingleVariable()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(EXAMPLE_VARIABLE_VALUE.Value)).body("type", @is(VariableTypeHelper.toExpectedValueTypeName(EXAMPLE_VARIABLE_VALUE.Type))).when().get(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).getVariableTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, EXAMPLE_VARIABLE_KEY, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalObjectVariable()
	  public virtual void testGetSingleLocalObjectVariable()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(caseServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo(payload)).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		// then
		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, variableKey, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalObjectVariableSerialized()
	  public virtual void testGetSingleLocalObjectVariableSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(caseServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo("a serialized value")).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		// then
		verify(caseServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_CASE_EXECUTION_ID, variableKey, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingLocalVariable()
	  public virtual void testNonExistingLocalVariable()
	  {
		when(caseServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(EXAMPLE_VARIABLE_KEY), eq(true))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("case execution variable with name " + EXAMPLE_VARIABLE_KEY + " does not exist")).when().get(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingVariable()
	  public virtual void testNonExistingVariable()
	  {
		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(EXAMPLE_VARIABLE_KEY), eq(true))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("case execution variable with name " + EXAMPLE_VARIABLE_KEY + " does not exist")).when().get(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariableForNonExistingExecution()
	  public virtual void testGetLocalVariableForNonExistingExecution()
	  {
		when(caseServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(EXAMPLE_VARIABLE_KEY), eq(true))).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot get case execution variable " + EXAMPLE_VARIABLE_KEY + ": expected exception")).when().get(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableForNonExistingExecution()
	  public virtual void testGetVariableForNonExistingExecution()
	  {
		when(caseServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_CASE_EXECUTION_ID), eq(EXAMPLE_VARIABLE_KEY), eq(true))).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot get case execution variable " + EXAMPLE_VARIABLE_KEY + ": expected exception")).when().get(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariable()
	  public virtual void testPutSingleLocalVariable()
	  {
		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(EXAMPLE_VARIABLE_VALUE.Value, EXAMPLE_VARIABLE_VALUE.Type.Name);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(EXAMPLE_VARIABLE_KEY, EXAMPLE_VARIABLE_VALUE);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariable()
	  public virtual void testPutSingleVariable()
	  {
		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(EXAMPLE_VARIABLE_VALUE.Value, EXAMPLE_VARIABLE_VALUE.Type.Name);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(EXAMPLE_VARIABLE_KEY, EXAMPLE_VARIABLE_VALUE);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeInteger()
	  public virtual void testPutSingleLocalVariableWithTypeInteger()
	  {
		string variableKey = "aVariableKey";
		int? variableValue = 123;
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.integerValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeInteger()
	  public virtual void testPutSingleVariableWithTypeInteger()
	  {
		string variableKey = "aVariableKey";
		int? variableValue = 123;
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.integerValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithUnparseableInteger()
	  public virtual void testPutSingleLocalVariableWithUnparseableInteger()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Integer)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableInteger()
	  public virtual void testPutSingleVariableWithUnparseableInteger()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Integer)))).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeShort()
	  public virtual void testPutSingleLocalVariableWithTypeShort()
	  {
		string variableKey = "aVariableKey";
		short? variableValue = 123;
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.shortValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeShort()
	  public virtual void testPutSingleVariableWithTypeShort()
	  {
		string variableKey = "aVariableKey";
		short? variableValue = 123;
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.shortValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithUnparseableShort()
	  public virtual void testPutSingleLocalVariableWithUnparseableShort()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Short)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableShort()
	  public virtual void testPutSingleVariableWithUnparseableShort()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Short)))).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeLong()
	  public virtual void testPutSingleLocalVariableWithTypeLong()
	  {
		string variableKey = "aVariableKey";
		long? variableValue = Convert.ToInt64(123);
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.longValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeLong()
	  public virtual void testPutSingleVariableWithTypeLong()
	  {
		string variableKey = "aVariableKey";
		long? variableValue = Convert.ToInt64(123);
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.longValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithUnparseableLong()
	  public virtual void testPutSingleLocalVariableWithUnparseableLong()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Long)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableLong()
	  public virtual void testPutSingleVariableWithUnparseableLong()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Long)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeDouble()
	  public virtual void testPutSingleLocalVariableWithTypeDouble()
	  {
		string variableKey = "aVariableKey";
		double? variableValue = 123.456;
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.doubleValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDouble()
	  public virtual void testPutSingleVariableWithTypeDouble()
	  {
		string variableKey = "aVariableKey";
		double? variableValue = 123.456;
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.doubleValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithUnparseableDouble()
	  public virtual void testPutSingleLocalVariableWithUnparseableDouble()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Double)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDouble()
	  public virtual void testPutSingleVariableWithUnparseableDouble()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Double)))).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeBoolean()
	  public virtual void testPutSingleLocalVariableWithTypeBoolean()
	  {
		string variableKey = "aVariableKey";
		bool? variableValue = true;
		string type = "Boolean";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.booleanValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeBoolean()
	  public virtual void testPutSingleVariableWithTypeBoolean()
	  {
		string variableKey = "aVariableKey";
		bool? variableValue = true;
		string type = "Boolean";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.booleanValue(variableValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithTypeDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalVariableWithTypeDate()
	  {
		DateTime now = DateTime.Now;

		string variableKey = "aVariableKey";
		string variableValue = DATE_FORMAT_WITH_TIMEZONE.format(now);
		string type = "Date";

		DateTime expectedValue = DATE_FORMAT_WITH_TIMEZONE.parse(variableValue);

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.dateValue(expectedValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableWithTypeDate()
	  {
		DateTime now = DateTime.Now;

		string variableKey = "aVariableKey";
		string variableValue = DATE_FORMAT_WITH_TIMEZONE.format(now);
		string type = "Date";

		DateTime expectedValue = DATE_FORMAT_WITH_TIMEZONE.parse(variableValue);

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.dateValue(expectedValue)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithUnparseableDate()
	  public virtual void testPutSingleLocalVariableWithUnparseableDate()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(DateTime)))).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDate()
	  public virtual void testPutSingleVariableWithUnparseableDate()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(DateTime)))).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithNotSupportedType()
	  public virtual void testPutSingleLocalVariableWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: Unsupported value type 'X'")).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNotSupportedType()
	  public virtual void testPutSingleVariableWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put case execution variable aVariableKey: Unsupported value type 'X'")).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalBinaryVariable()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleBinaryVariable()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes, MediaType.APPLICATION_OCTET_STREAM).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithValueType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalBinaryVariableWithValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "Bytes", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableWithValueType()
	  public virtual void testPutSingleBinaryVariableWithValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "Bytes", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithNoValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalBinaryVariableWithNoValue()
	  {
		sbyte[] bytes = new sbyte[0];

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableWithNoValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleBinaryVariableWithNoValue()
	  {
		sbyte[] bytes = new sbyte[0];

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalSerializableVariableFromJson() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalSerializableVariableFromJson()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, MediaType.APPLICATION_JSON).multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().Deserialized.value(serializable)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleSerializableVariableFormJson() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleSerializableVariableFormJson()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, MediaType.APPLICATION_JSON).multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().Deserialized.value(serializable)));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalSerializableVariableUnsupportedMediaType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalSerializableVariableUnsupportedMediaType()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, "unsupported").multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Unrecognized content type for serialized java type: unsupported")).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(caseServiceMock, never()).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleSerializableVariableUnsupportedMediaType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleSerializableVariableUnsupportedMediaType()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, "unsupported").multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Unrecognized content type for serialized java type: unsupported")).when().post(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		verify(caseServiceMock, never()).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithNoValue()
	  public virtual void testPutSingleLocalVariableWithNoValue()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), argThat(EqualsNullValue.matcher()));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNoValue()
	  public virtual void testPutSingleVariableWithNoValue()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), argThat(EqualsNullValue.matcher()));
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutLocalVariableForNonExistingExecution()
	  public virtual void testPutLocalVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot put case execution variable " + variableKey + ": expected exception")).when().put(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithEncodingAndMimeType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleLocalFileVariableWithEncodingAndMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype + "; encoding=" + encoding).multiPart("valueType", "File", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(encoding));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithMimeType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleLocalFileVariableWithMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithEncoding() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleLocalFileVariableWithEncoding()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, "encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableOnlyFilename() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleLocalFileVariableOnlyFilename()
	  {

		string variableKey = "aVariableKey";
		string filename = "test.txt";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, new sbyte[0]).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(MediaType.APPLICATION_OCTET_STREAM));
		assertThat(captured.Value.available(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariable()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype + "; encoding=" + encoding).header("accept", MediaType.APPLICATION_JSON).multiPart("valueType", "File", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_CASE_EXECUTION_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(encoding));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutVariableForNonExistingExecution()
	  public virtual void testPutVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot put case execution variable " + variableKey + ": expected exception")).when().put(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleLocalVariable()
	  public virtual void testDeleteSingleLocalVariable()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(variableKey);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleVariable()
	  public virtual void testDeleteSingleVariable()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_CASE_EXECUTION_VARIABLE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(variableKey);
		verify(caseExecutionCommandBuilderMock).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteLocalVariableForNonExistingExecution()
	  public virtual void testDeleteLocalVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot delete case execution variable " + variableKey + ": expected exception")).when().delete(SINGLE_CASE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteVariableForNonExistingExecution()
	  public virtual void testDeleteVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";

		doThrow(new ProcessEngineException("expected exception")).when(caseExecutionCommandBuilderMock).execute();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot delete case execution variable " + variableKey + ": expected exception")).when().delete(SINGLE_CASE_EXECUTION_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testComplete()
	  public virtual void testComplete()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulComplete()
	  public virtual void testUnsuccessfulComplete()
	  {
		doThrow(new NotValidException("expected exception")).when(caseExecutionCommandBuilderMock).complete();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete case execution " + MockProvider.EXAMPLE_CASE_EXECUTION_ID + ": expected exception")).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariable()
	  public virtual void testCompleteWithSetVariable()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableLocal()
	  public virtual void testCompleteWithSetVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer", true).variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableAndVariableLocal()
	  public virtual void testCompleteWithSetVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithRemoveVariable()
	  public virtual void testCompleteWithRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithRemoveVariableLocal()
	  public virtual void testCompleteWithRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey, true);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithRemoveVariableAndVariableLocal()
	  public virtual void testCompleteWithRemoveVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableAndRemoveVariable()
	  public virtual void testCompleteWithSetVariableAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableAndRemoveVariableLocal()
	  public virtual void testCompleteWithSetVariableAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableLocalAndRemoveVariable()
	  public virtual void testCompleteWithSetVariableLocalAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithSetVariableLocalAndRemoveVariableLocal()
	  public virtual void testCompleteWithSetVariableLocalAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_COMPLETE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).complete();
	  }

	  ///////////////////////////////////////////////////////////////
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminate()
	  public virtual void testTerminate()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulTerminate()
	  public virtual void testUnsuccessfulTerminate()
	  {
		doThrow(new NotValidException("expected exception")).when(caseExecutionCommandBuilderMock).terminate();

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot terminate case execution " + MockProvider.EXAMPLE_CASE_EXECUTION_ID + ": expected exception")).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariable()
	  public virtual void testTerminateWithSetVariable()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableLocal()
	 public virtual void testTerminateWithSetVariableLocal()
	 {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer", true).variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	 }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableAndVariableLocal()
	  public virtual void testTerminateWithSetVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		int aVariableValue = 123;

		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		IDictionary<string, object> variables = VariablesBuilder.create().variable(aVariableKey, aVariableValue, "Integer").variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(aVariableKey), argThat(EqualsPrimitiveValue.integerValue(aVariableValue)));
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithRemoveVariable()
	  public virtual void testTerminateWithRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithRemoveVariableLocal()
	  public virtual void testTerminateWithRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey, true);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithRemoveVariableAndVariableLocal()
	  public virtual void testTerminateWithRemoveVariableAndVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);
		VariableNameDto secondVariableName = new VariableNameDto(anotherVariableKey);
		variableNames.Add(secondVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).removeVariable(anotherVariableKey);
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableAndRemoveVariable()
	  public virtual void testTerminateWithSetVariableAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableAndRemoveVariableLocal()
	  public virtual void testTerminateWithSetVariableAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String").Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariable(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableLocalAndRemoveVariable()
	  public virtual void testTerminateWithSetVariableLocalAndRemoveVariable()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariable(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminateWithSetVariableLocalAndRemoveVariableLocal()
	  public virtual void testTerminateWithSetVariableLocalAndRemoveVariableLocal()
	  {
		string aVariableKey = "aKey";
		string anotherVariableKey = "anotherKey";
		string anotherVariableValue = "abc";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(anotherVariableKey, anotherVariableValue, "String", true).Variables;

		IList<VariableNameDto> variableNames = new List<VariableNameDto>();

		VariableNameDto firstVariableName = new VariableNameDto(aVariableKey, true);
		variableNames.Add(firstVariableName);

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();

		variablesJson["variables"] = variables;
		variablesJson["deletions"] = variableNames;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CASE_EXECUTION_TERMINATE_URL);

		verify(caseServiceMock).withCaseExecution(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		verify(caseExecutionCommandBuilderMock).removeVariableLocal(aVariableKey);
		verify(caseExecutionCommandBuilderMock).setVariableLocal(eq(anotherVariableKey), argThat(EqualsPrimitiveValue.stringValue(anotherVariableValue)));
		verify(caseExecutionCommandBuilderMock).terminate();
	  }
	}

}