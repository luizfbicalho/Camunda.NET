using System;
using System.Collections;
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
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using Assert = junit.framework.Assert;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;

	public class MessageRestServiceTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string MESSAGE_URL = TEST_RESOURCE_ROOT_PATH + MessageRestService_Fields.PATH;

	  private RuntimeService runtimeServiceMock;
	  private MessageCorrelationBuilder messageCorrelationBuilderMock;
	  private MessageCorrelationResult executionResult;
	  private MessageCorrelationResult procInstanceResult;
	  private IList<MessageCorrelationResult> executionResultList;
	  private IList<MessageCorrelationResult> procInstanceResultList;
	  private IList<MessageCorrelationResult> mixedResultList;

	  private MessageCorrelationResultWithVariables executionResultWithVariables;
	  private IList<MessageCorrelationResultWithVariables> execResultWithVariablesList;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupMocks()
	  public virtual void setupMocks()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		messageCorrelationBuilderMock = mock(typeof(MessageCorrelationBuilder));

		when(runtimeServiceMock.createMessageCorrelation(anyString())).thenReturn(messageCorrelationBuilderMock);
		when(messageCorrelationBuilderMock.processInstanceId(anyString())).thenReturn(messageCorrelationBuilderMock);
		when(messageCorrelationBuilderMock.processInstanceBusinessKey(anyString())).thenReturn(messageCorrelationBuilderMock);
		when(messageCorrelationBuilderMock.processInstanceVariableEquals(anyString(), any())).thenReturn(messageCorrelationBuilderMock);
		when(messageCorrelationBuilderMock.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(messageCorrelationBuilderMock);
		when(messageCorrelationBuilderMock.setVariable(anyString(), any())).thenReturn(messageCorrelationBuilderMock);

		executionResult = MockProvider.createMessageCorrelationResult(MessageCorrelationResultType.Execution);
		procInstanceResult = MockProvider.createMessageCorrelationResult(MessageCorrelationResultType.ProcessDefinition);
		executionResultList = MockProvider.createMessageCorrelationResultList(MessageCorrelationResultType.Execution);
		procInstanceResultList = MockProvider.createMessageCorrelationResultList(MessageCorrelationResultType.ProcessDefinition);
		mixedResultList = new List<>(executionResultList);
		((IList<MessageCorrelationResult>)mixedResultList).AddRange(procInstanceResultList);

		executionResultWithVariables = MockProvider.createMessageCorrelationResultWithVariables(MessageCorrelationResultType.Execution);
		execResultWithVariablesList = new List<>();
		execResultWithVariablesList.Add(executionResultWithVariables);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelation()
	  public virtual void testFullMessageCorrelation()
	  {
		string messageName = "aMessageName";
		string businessKey = "aBusinessKey";
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aKey", "aValue").Variables;
		IDictionary<string, object> variablesLocal = VariablesBuilder.create().variable("aKeyLocal", "aValueLocal").Variables;

		IDictionary<string, object> correlationKeys = VariablesBuilder.create().variable("aKey", "aValue").variable("anotherKey", 1).variable("aThirdKey", true).Variables;

		IDictionary<string, object> localCorrelationKeys = VariablesBuilder.create().variable("aLocalKey", "aValue").variable("anotherLocalKey", 1).variable("aThirdLocalKey", false).Variables;

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = correlationKeys;
		messageParameters["localCorrelationKeys"] = localCorrelationKeys;
		messageParameters["processVariables"] = variables;
		messageParameters["processVariablesLocal"] = variablesLocal;
		messageParameters["businessKey"] = businessKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		IDictionary<string, object> expectedCorrelationKeys = new Dictionary<string, object>();
		expectedCorrelationKeys["aKey"] = "aValue";
		expectedCorrelationKeys["anotherKey"] = 1;
		expectedCorrelationKeys["aThirdKey"] = true;

		IDictionary<string, object> expectedLocalCorrelationKeys = new Dictionary<string, object>();
		expectedLocalCorrelationKeys["aLocalKey"] = "aValue";
		expectedLocalCorrelationKeys["anotherLocalKey"] = 1;
		expectedLocalCorrelationKeys["aThirdLocalKey"] = false;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aKey"] = "aValue";
		IDictionary<string, object> expectedVariablesLocal = new Dictionary<string, object>();
		expectedVariablesLocal["aKeyLocal"] = "aValueLocal";

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).processInstanceBusinessKey(eq(businessKey));
		verify(messageCorrelationBuilderMock).Variables = argThat(new EqualsMap(expectedVariables));
		verify(messageCorrelationBuilderMock).VariablesLocal = argThat(new EqualsMap(expectedVariablesLocal));

		foreach (KeyValuePair<string, object> expectedKey in expectedCorrelationKeys.SetOfKeyValuePairs())
		{
		  string name = expectedKey.Key;
		  object value = expectedKey.Value;
		  verify(messageCorrelationBuilderMock).processInstanceVariableEquals(name, value);
		}

		foreach (KeyValuePair<string, object> expectedLocalKey in expectedLocalCorrelationKeys.SetOfKeyValuePairs())
		{
		  string name = expectedLocalKey.Key;
		  object value = expectedLocalKey.Value;
		  verify(messageCorrelationBuilderMock).localVariableEquals(name, value);
		}

		verify(messageCorrelationBuilderMock).correlateWithResult();

	//    verify(runtimeServiceMock).correlateMessage(eq(messageName), eq(businessKey),
	//        argThat(new EqualsMap(expectedCorrelationKeys)), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationWithExecutionResult()
	  public virtual void testFullMessageCorrelationWithExecutionResult()
	  {
		//given
		when(messageCorrelationBuilderMock.correlateWithResult()).thenReturn(executionResult);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["resultEnabled"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);
		checkExecutionResult(content, 0);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateWithResult();
	  }

	  protected internal virtual void checkExecutionResult(string content, int idx)
	  {
		//resultType should be execution
		string resultType = from(content).get("[" + idx + "].resultType").ToString();
		assertEquals(MessageCorrelationResultType.Execution.name(), resultType);
		//execution should be filled and process instance should be null
		assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, from(content).get("[" + idx + "].execution.id"));
		assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, from(content).get("[" + idx + "].execution.processInstanceId"));
		assertNull(from(content).get("[" + idx + "].processInstance"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationWithProcessDefinitionResult()
	  public virtual void testFullMessageCorrelationWithProcessDefinitionResult()
	  {
		//given
		when(messageCorrelationBuilderMock.correlateWithResult()).thenReturn(procInstanceResult);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["resultEnabled"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);
		checkProcessInstanceResult(content, 0);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateWithResult();
	  }

	  protected internal virtual void checkProcessInstanceResult(string content, int idx)
	  {
		//resultType should be set to process definition
		string resultType = from(content).get("[" + idx + "].resultType");
		Assert.assertEquals(MessageCorrelationResultType.ProcessDefinition.name(), resultType);

		//process instance should be filled and execution should be null
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, from(content).get("[" + idx + "].processInstance.id"));
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, from(content).get("[" + idx + "].processInstance.definitionId"));
		Assert.assertNull(from(content).get("[" + idx + "].execution"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationAll()
	  public virtual void testFullMessageCorrelationAll()
	  {
		string messageName = "aMessageName";
		string businessKey = "aBusinessKey";
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aKey", "aValue").Variables;
		IDictionary<string, object> variablesLocal = VariablesBuilder.create().variable("aKeyLocal", "aValueLocal").Variables;

		IDictionary<string, object> correlationKeys = VariablesBuilder.create().variable("aKey", "aValue").variable("anotherKey", 1).variable("aThirdKey", true).Variables;

		IDictionary<string, object> localCorrelationKeys = VariablesBuilder.create().variable("aLocalKey", "aValue").variable("anotherLocalKey", 1).variable("aThirdLocalKey", false).Variables;

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = correlationKeys;
		messageParameters["localCorrelationKeys"] = localCorrelationKeys;
		messageParameters["processVariables"] = variables;
		messageParameters["processVariablesLocal"] = variablesLocal;
		messageParameters["businessKey"] = businessKey;
		messageParameters["all"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		IDictionary<string, object> expectedCorrelationKeys = new Dictionary<string, object>();
		expectedCorrelationKeys["aKey"] = "aValue";
		expectedCorrelationKeys["anotherKey"] = 1;
		expectedCorrelationKeys["aThirdKey"] = true;

		IDictionary<string, object> expectedLocalCorrelationKeys = new Dictionary<string, object>();
		expectedLocalCorrelationKeys["aLocalKey"] = "aValue";
		expectedLocalCorrelationKeys["anotherLocalKey"] = 1;
		expectedLocalCorrelationKeys["aThirdLocalKey"] = false;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aKey"] = "aValue";
		IDictionary<string, object> expectedVariablesLocal = new Dictionary<string, object>();
		expectedVariablesLocal["aKeyLocal"] = "aValueLocal";

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).processInstanceBusinessKey(eq(businessKey));
		verify(messageCorrelationBuilderMock).Variables = argThat(new EqualsMap(expectedVariables));
		verify(messageCorrelationBuilderMock).VariablesLocal = argThat(new EqualsMap(expectedVariablesLocal));

		foreach (KeyValuePair<string, object> expectedKey in expectedCorrelationKeys.SetOfKeyValuePairs())
		{
		  string name = expectedKey.Key;
		  object value = expectedKey.Value;
		  verify(messageCorrelationBuilderMock).processInstanceVariableEquals(name, value);
		}

		foreach (KeyValuePair<string, object> expectedLocalKey in expectedLocalCorrelationKeys.SetOfKeyValuePairs())
		{
		  string name = expectedLocalKey.Key;
		  object value = expectedLocalKey.Value;
		  verify(messageCorrelationBuilderMock).localVariableEquals(name, value);
		}

		verify(messageCorrelationBuilderMock).correlateAllWithResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationAllWithExecutionResult()
	  public virtual void testFullMessageCorrelationAllWithExecutionResult()
	  {
		//given
		when(messageCorrelationBuilderMock.correlateAllWithResult()).thenReturn(executionResultList);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;
		messageParameters["resultEnabled"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);

		IList<Hashtable> results = from(content).getList("");
		assertEquals(2, results.Count);
		for (int i = 0; i < 2; i++)
		{
		  checkExecutionResult(content, i);
		}

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateAllWithResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationAllWithProcessInstanceResult()
	 public virtual void testFullMessageCorrelationAllWithProcessInstanceResult()
	 {
		//given
		when(messageCorrelationBuilderMock.correlateAllWithResult()).thenReturn(procInstanceResultList);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;
		messageParameters["resultEnabled"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);

		IList<Hashtable> results = from(content).getList("");
		assertEquals(2, results.Count);
		for (int i = 0; i < 2; i++)
		{
		  checkProcessInstanceResult(content, i);
		}

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateAllWithResult();
	 }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationAllWithMixedResult()
	  public virtual void testFullMessageCorrelationAllWithMixedResult()
	  {
		//given
		when(messageCorrelationBuilderMock.correlateAllWithResult()).thenReturn(mixedResultList);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;
		messageParameters["resultEnabled"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);

		IList<Hashtable> results = from(content).getList("");
		assertEquals(4, results.Count);
		for (int i = 0; i < 2; i++)
		{
		  string resultType = from(content).get("[" + i + "].resultType");
		  assertNotNull(resultType);
		  if (resultType.Equals(MessageCorrelationResultType.Execution.name()))
		  {
			checkExecutionResult(content, i);
		  }
		  else
		  {
			checkProcessInstanceResult(content, i);
		  }
		}

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateAllWithResult();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullMessageCorrelationAllWithNoResult()
	  public virtual void testFullMessageCorrelationAllWithNoResult()
	  {
		//given
		when(messageCorrelationBuilderMock.correlateAllWithResult()).thenReturn(mixedResultList);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;

		//when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		//then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length == 0);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateAllWithResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageNameOnlyCorrelation()
	  public virtual void testMessageNameOnlyCorrelation()
	  {
		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageNameAndBusinessKeyCorrelation()
	  public virtual void testMessageNameAndBusinessKeyCorrelation()
	  {
		string messageName = "aMessageName";
		string businessKey = "aBusinessKey";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["businessKey"] = businessKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

	//    verify(runtimeServiceMock).correlateMessage(eq(messageName), eq(businessKey),
	//        argThat(new EqualsMap(null)), argThat(new EqualsMap(null)));

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).processInstanceBusinessKey(eq(businessKey));
		verify(messageCorrelationBuilderMock).correlateWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageNameAndBusinessKeyCorrelationAll()
	  public virtual void testMessageNameAndBusinessKeyCorrelationAll()
	  {
		string messageName = "aMessageName";
		string businessKey = "aBusinessKey";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["businessKey"] = businessKey;
		messageParameters["all"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).processInstanceBusinessKey(eq(businessKey));
		verify(messageCorrelationBuilderMock).correlateAllWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMismatchingCorrelation()
	  public virtual void testMismatchingCorrelation()
	  {
		string messageName = "aMessage";

		doThrow(new MismatchingMessageCorrelationException(messageName, "Expected exception: cannot correlate")).when(messageCorrelationBuilderMock).correlateWithResult();

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", containsString("Expected exception: cannot correlate")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingInstantiation()
	  public virtual void testFailingInstantiation()
	  {
		string messageName = "aMessage";

		// thrown, if instantiation of the process or signalling the instance fails
		doThrow(new ProcessEngineException("Expected exception")).when(messageCorrelationBuilderMock).correlateWithResult();

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("Expected exception")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoMessageNameCorrelation()
	  public virtual void testNoMessageNameCorrelation()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No message name supplied")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageCorrelationWithTenantId()
	  public virtual void testMessageCorrelationWithTenantId()
	  {
		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).tenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(messageCorrelationBuilderMock).correlateWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageCorrelationWithoutTenantId()
	  public virtual void testMessageCorrelationWithoutTenantId()
	  {
		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).withoutTenantId();
		verify(messageCorrelationBuilderMock).correlateWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingInvalidTenantParameters()
	  public virtual void testFailingInvalidTenantParameters()
	  {
		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;
		messageParameters["withoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Parameter 'tenantId' cannot be used together with parameter 'withoutTenantId'.")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableIntegerInCorrelationKeys()
	  public virtual void testFailingDueToUnparseableIntegerInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableIntegerInLocalCorrelationKeys()
	  public virtual void testFailingDueToUnparseableIntegerInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableShortInCorrelationKeys()
	  public virtual void testFailingDueToUnparseableShortInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableShortInLocalCorrelationKeys()
	  public virtual void testFailingDueToUnparseableShortInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableLongInCorrelationKeys()
	  public virtual void testFailingDueToUnparseableLongInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableLongInLocalCorrelationKeys()
	  public virtual void testFailingDueToUnparseableLongInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDoubleInCorrelationKeys()
	  public virtual void testFailingDueToUnparseableDoubleInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDoubleInLocalCorrelationKeys()
	  public virtual void testFailingDueToUnparseableDoubleInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDateInCorrelationKeys()
	  public virtual void testFailingDueToUnparseableDateInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDateInLocalCorrelationKeys()
	  public virtual void testFailingDueToUnparseableDateInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToNotSupportedTypeInCorrelationKeys()
	  public virtual void testFailingDueToNotSupportedTypeInCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["correlationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: Unsupported value type 'X'")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToNotSupportedTypeInLocalCorrelationKeys()
	  public virtual void testFailingDueToNotSupportedTypeInLocalCorrelationKeys()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["localCorrelationKeys"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: Unsupported value type 'X'")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableIntegerInProcessVariables()
	  public virtual void testFailingDueToUnparseableIntegerInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableShortInProcessVariables()
	  public virtual void testFailingDueToUnparseableShortInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableLongInProcessVariables()
	  public virtual void testFailingDueToUnparseableLongInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDoubleInProcessVariables()
	  public virtual void testFailingDueToUnparseableDoubleInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDateInProcessVariables()
	  public virtual void testFailingDueToUnparseableDateInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToNotSupportedTypeInProcessVariables()
	  public virtual void testFailingDueToNotSupportedTypeInProcessVariables()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariables"] = variableJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: Unsupported value type 'X'")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableIntegerInProcessVariablesLocal()
	  public virtual void testFailingDueToUnparseableIntegerInProcessVariablesLocal()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableLocalJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariablesLocal"] = variableLocalJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToNotSupportedTypeInProcessVariablesLocal()
	  public virtual void testFailingDueToNotSupportedTypeInProcessVariablesLocal()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableLocalJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariablesLocal"] = variableLocalJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: Unsupported value type 'X'")).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingDueToUnparseableDateInProcessVariablesLocal()
	  public virtual void testFailingDueToUnparseableDateInProcessVariablesLocal()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableLocalJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processVariablesLocal"] = variableLocalJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot deliver message: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelateThrowsAuthorizationException()
	  public virtual void testCorrelateThrowsAuthorizationException()
	  {
		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(messageCorrelationBuilderMock).correlateWithResult();

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testcorrelateAllThrowsAuthorizationException()
	  public virtual void testcorrelateAllThrowsAuthorizationException()
	  {
		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(messageCorrelationBuilderMock).correlateAllWithResult();

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(MESSAGE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageCorrelationWithProcessInstanceId()
	  public virtual void testMessageCorrelationWithProcessInstanceId()
	  {
		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).processInstanceId(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID));

		verify(messageCorrelationBuilderMock).correlateWithResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageCorrelationWithoutBusinessKey()
	  public virtual void testMessageCorrelationWithoutBusinessKey()
	  {
		when(messageCorrelationBuilderMock.processInstanceBusinessKey(null)).thenThrow(new NullValueException());

		string messageName = "aMessageName";

		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));

		verify(messageCorrelationBuilderMock, Mockito.never()).processInstanceBusinessKey(anyString());
		verify(messageCorrelationBuilderMock).correlateWithResult();
		verifyNoMoreInteractions(messageCorrelationBuilderMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelationWithVariablesInResult()
	  public virtual void testCorrelationWithVariablesInResult()
	  {
		// given
		when(messageCorrelationBuilderMock.correlateWithResultAndVariables(false)).thenReturn(executionResultWithVariables);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["resultEnabled"] = true;
		messageParameters["variablesInResultEnabled"] = true;

		// when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		// then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);
		checkVariablesInResult(content, 0);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateWithResultAndVariables(false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCorrelationAllWithVariablesInResult()
	  public virtual void testCorrelationAllWithVariablesInResult()
	  {
		// given
		when(messageCorrelationBuilderMock.correlateAllWithResultAndVariables(false)).thenReturn(execResultWithVariablesList);

		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["all"] = true;
		messageParameters["resultEnabled"] = true;
		messageParameters["variablesInResultEnabled"] = true;

		// when
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		// then
		assertNotNull(response);
		string content = response.asString();
		assertTrue(content.Length > 0);

		IList<Dictionary<object, object>> results = from(content).getList("");
		assertEquals(1, results.Count);
		checkVariablesInResult(content, 0);

		verify(runtimeServiceMock).createMessageCorrelation(eq(messageName));
		verify(messageCorrelationBuilderMock).correlateAllWithResultAndVariables(false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingCorrelationWithVariablesInResultDueToDisabledResult()
	  public virtual void testFailingCorrelationWithVariablesInResultDueToDisabledResult()
	  {
		// given
		string messageName = "aMessageName";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["resultEnabled"] = false;
		messageParameters["variablesInResultEnabled"] = true;

		// when/
		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).then().expect().contentType(ContentType.JSON).statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Parameter 'variablesInResultEnabled' cannot be used without 'resultEnabled' set to true.")).when().post(MESSAGE_URL);
	  }


	  protected internal virtual void checkVariablesInResult(string content, int idx)
	  {
		IList<string> variableNames = new IList<string> {MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME, MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME};

		foreach (string variableName in variableNames)
		{
		  string variablePath = "[" + idx + "].variables." + variableName;
		  assertEquals(MockProvider.FORMAT_APPLICATION_JSON, from(content).getMap(variablePath + ".valueInfo").get("serializationDataFormat"));
		  assertEquals(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE, from(content).get(variablePath + ".value"));
		  assertEquals(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT), from(content).get(variablePath + ".type"));
		}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(List<object>).FullName, from(content).getMap("[" + idx + "].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo").get("objectTypeName"));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(object).FullName, from(content).getMap("[" + idx + "].variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo").get("objectTypeName"));
	  }

	}

}