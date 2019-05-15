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
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
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


	using ConditionEvaluationBuilderImpl = org.camunda.bpm.engine.impl.ConditionEvaluationBuilderImpl;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ConditionEvaluationBuilder = org.camunda.bpm.engine.runtime.ConditionEvaluationBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;

	using Response = io.restassured.response.Response;

	public class ConditionRestServiceTest : AbstractRestServiceTest
	{

	  protected internal static readonly string CONDITION_URL = TEST_RESOURCE_ROOT_PATH + ConditionRestService_Fields.PATH;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  private RuntimeService runtimeServiceMock;
	  private ConditionEvaluationBuilder conditionEvaluationBuilderMock;
	  private IList<ProcessInstance> processInstancesMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupMocks()
	  public virtual void setupMocks()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		conditionEvaluationBuilderMock = mock(typeof(ConditionEvaluationBuilderImpl));

		when(runtimeServiceMock.createConditionEvaluation()).thenReturn(conditionEvaluationBuilderMock);
		when(conditionEvaluationBuilderMock.processDefinitionId(anyString())).thenReturn(conditionEvaluationBuilderMock);
		when(conditionEvaluationBuilderMock.processInstanceBusinessKey(anyString())).thenReturn(conditionEvaluationBuilderMock);
		when(conditionEvaluationBuilderMock.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(conditionEvaluationBuilderMock);
		when(conditionEvaluationBuilderMock.setVariable(anyString(), any())).thenReturn(conditionEvaluationBuilderMock);

		processInstancesMock = MockProvider.createAnotherMockProcessInstanceList();
		when(conditionEvaluationBuilderMock.evaluateStartConditions()).thenReturn(processInstancesMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationOnlyVariables()
	  public virtual void testConditionEvaluationOnlyVariables()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CONDITION_URL);

		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);
		checkResult(content);

		verify(runtimeServiceMock).createConditionEvaluation();
		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["foo"] = "bar";
		verify(conditionEvaluationBuilderMock).Variables = expectedVariables;
		verify(conditionEvaluationBuilderMock).evaluateStartConditions();
		verifyNoMoreInteractions(conditionEvaluationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationWithProcessDefinition()
	  public virtual void testConditionEvaluationWithProcessDefinition()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;
		parameters["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CONDITION_URL);

		verify(runtimeServiceMock).createConditionEvaluation();
		verify(conditionEvaluationBuilderMock).processDefinitionId(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(conditionEvaluationBuilderMock).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationWithBusinessKey()
	  public virtual void testConditionEvaluationWithBusinessKey()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;
		parameters["businessKey"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CONDITION_URL);

		verify(runtimeServiceMock).createConditionEvaluation();
		verify(conditionEvaluationBuilderMock).processInstanceBusinessKey(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationWithTenantId()
	  public virtual void testConditionEvaluationWithTenantId()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;
		parameters["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CONDITION_URL);

		verify(runtimeServiceMock).createConditionEvaluation();
		verify(conditionEvaluationBuilderMock).tenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(conditionEvaluationBuilderMock).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationWithoutTenantId()
	  public virtual void testConditionEvaluationWithoutTenantId()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;
		parameters["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CONDITION_URL);

		verify(runtimeServiceMock).createConditionEvaluation();
		verify(conditionEvaluationBuilderMock).withoutTenantId();
		verify(conditionEvaluationBuilderMock).evaluateStartConditions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationFailingInvalidTenantParameters()
	  public virtual void testConditionEvaluationFailingInvalidTenantParameters()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;
		parameters["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;
		parameters["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Parameter 'tenantId' cannot be used together with parameter 'withoutTenantId'.")).when().post(CONDITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConditionEvaluationThrowsAuthorizationException()
	  public virtual void testConditionEvaluationThrowsAuthorizationException()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable("foo", "bar").Variables;
		parameters["variables"] = variables;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(conditionEvaluationBuilderMock).evaluateStartConditions();

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(CONDITION_URL);
	  }

	  protected internal virtual void checkResult(string content)
	  {
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, from(content).get("[" + 0 + "].id"));
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, from(content).get("[" + 0 + "].definitionId"));
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, from(content).get("[" + 1 + "].id"));
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, from(content).get("[" + 1 + "].definitionId"));
	  }

	}

}