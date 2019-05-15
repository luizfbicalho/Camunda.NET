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
	using SignalEventReceivedBuilderImpl = org.camunda.bpm.engine.impl.SignalEventReceivedBuilderImpl;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using SignalEventReceivedBuilder = org.camunda.bpm.engine.runtime.SignalEventReceivedBuilder;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsEqual.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
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

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SignalRestServiceTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string SIGNAL_URL = TEST_RESOURCE_ROOT_PATH + SignalRestService_Fields.PATH;

	  private RuntimeService runtimeServiceMock;
	  private SignalEventReceivedBuilder signalBuilderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupMocks()
	  public virtual void setupMocks()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		signalBuilderMock = mock(typeof(SignalEventReceivedBuilderImpl));
		when(runtimeServiceMock.createSignalEvent(anyString())).thenReturn(signalBuilderMock);
		when(signalBuilderMock.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(signalBuilderMock);
		when(signalBuilderMock.executionId(anyString())).thenReturn(signalBuilderMock);
		when(signalBuilderMock.tenantId(anyString())).thenReturn(signalBuilderMock);
		when(signalBuilderMock.withoutTenantId()).thenReturn(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcast()
	  public virtual void shouldBroadcast()
	  {
		IDictionary<string, string> requestBody = new Dictionary<string, string>();
		requestBody["name"] = "aSignalName";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent(requestBody["name"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcastWithVariables()
	  public virtual void shouldBroadcastWithVariables()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcastWithTenant()
	  public virtual void shouldBroadcastWithTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["tenantId"] = "aTenantId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).tenantId((string) requestBody["tenantId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcastWithVariablesAndTenant()
	  public virtual void shouldBroadcastWithVariablesAndTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;
		requestBody["tenantId"] = "aTenantId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).tenantId((string) requestBody["tenantId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcastWithoutTenant()
	  public virtual void shouldBroadcastWithoutTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).withoutTenantId();
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBroadcastWithoutTenantAndWithVariables()
	  public virtual void shouldBroadcastWithoutTenantAndWithVariables()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;
		requestBody["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).withoutTenantId();
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecution()
	  public virtual void shouldDeliverToSingleExecution()
	  {
		IDictionary<string, string> requestBody = new Dictionary<string, string>();
		requestBody["name"] = "aSignalName";
		requestBody["executionId"] = "anExecutionId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent(requestBody["name"]);
		verify(signalBuilderMock).executionId(requestBody["executionId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecutionWithVariables()
	  public virtual void shouldDeliverToSingleExecutionWithVariables()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["executionId"] = "anExecutionId";
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).executionId((string) requestBody["executionId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecutionWithTenant()
	  public virtual void shouldDeliverToSingleExecutionWithTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["tenantId"] = "aTenantId";
		requestBody["executionId"] = "anExecutionId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).tenantId((string) requestBody["tenantId"]);
		verify(signalBuilderMock).executionId((string) requestBody["executionId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecutionWithVariablesAndTenant()
	  public virtual void shouldDeliverToSingleExecutionWithVariablesAndTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["executionId"] = "anExecutionId";
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;
		requestBody["tenantId"] = "aTenantId";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).executionId((string) requestBody["executionId"]);
		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).tenantId((string) requestBody["tenantId"]);
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecutionWithoutTenant()
	  public virtual void shouldDeliverToSingleExecutionWithoutTenant()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["executionId"] = "anExecutionId";
		requestBody["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).executionId((string) requestBody["executionId"]);
		verify(signalBuilderMock).withoutTenantId();
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDeliverToSingleExecutionWithoutTenantAndWithVariables()
	  public virtual void shouldDeliverToSingleExecutionWithoutTenantAndWithVariables()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";
		requestBody["executionId"] = "anExecutionId";
		requestBody["withoutTenantId"] = true;
		requestBody["variables"] = VariablesBuilder.create().variable("total", 420).variable("invoiceId", "ABC123").Variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_URL);

		verify(runtimeServiceMock).createSignalEvent((string) requestBody["name"]);
		verify(signalBuilderMock).executionId((string) requestBody["executionId"]);
		verify(signalBuilderMock).withoutTenantId();
		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["total"] = 420;
		expectedVariables["invoiceId"] = "ABC123";
		verify(signalBuilderMock).Variables = expectedVariables;
		verify(signalBuilderMock).send();
		verifyNoMoreInteractions(signalBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByMissingName()
	  public virtual void shouldThrowExceptionByMissingName()
	  {
		IDictionary<string, object> requestBody = new Dictionary<string, object>();

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No signal name given")).when().post(SIGNAL_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestException()
	  public virtual void shouldThrowBadUserRequestException()
	  {
		string message = "expected exception";
		doThrow(new BadUserRequestException(message)).when(signalBuilderMock).send();

		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo(message)).when().post(SIGNAL_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowAuthorizationException()
	  public virtual void shouldThrowAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(signalBuilderMock).send();

		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SIGNAL_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowProcessEngineException()
	  public virtual void shouldThrowProcessEngineException()
	  {
		string message = "expected exception";
		doThrow(new ProcessEngineException(message)).when(signalBuilderMock).send();

		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["name"] = "aSignalName";

		given().contentType(POST_JSON_CONTENT_TYPE).body(requestBody).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo(message)).when().post(SIGNAL_URL);
	  }

	}

}