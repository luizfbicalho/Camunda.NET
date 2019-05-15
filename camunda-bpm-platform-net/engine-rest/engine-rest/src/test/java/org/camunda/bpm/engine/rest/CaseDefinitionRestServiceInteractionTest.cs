using System.Collections.Generic;
using System.IO;

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
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
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
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsVariableMap = org.camunda.bpm.engine.rest.helper.EqualsVariableMap;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using ProcessDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.ProcessDefinitionResourceImpl;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionRestServiceInteractionTest : AbstractRestServiceTest
	{

	  protected internal static readonly string CASE_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/case-definition";
	  protected internal static readonly string SINGLE_CASE_DEFINITION_URL = CASE_DEFINITION_URL + "/{id}";
	  protected internal static readonly string SINGLE_CASE_DEFINITION_BY_KEY_URL = CASE_DEFINITION_URL + "/key/{key}";
	  protected internal static readonly string SINGLE_CASE_DEFINITION_BY_KEY_AND_TENANT_ID_URL = CASE_DEFINITION_URL + "/key/{key}/tenant-id/{tenant-id}";

	  protected internal static readonly string XML_DEFINITION_URL = SINGLE_CASE_DEFINITION_URL + "/xml";
	  protected internal static readonly string XML_DEFINITION_BY_KEY_URL = SINGLE_CASE_DEFINITION_BY_KEY_URL + "/xml";

	  protected internal static readonly string CREATE_INSTANCE_URL = SINGLE_CASE_DEFINITION_URL + "/create";
	  protected internal static readonly string CREATE_INSTANCE_BY_KEY_URL = SINGLE_CASE_DEFINITION_BY_KEY_URL + "/create";
	  protected internal static readonly string CREATE_INSTANCE_BY_KEY_AND_TENANT_ID_URL = SINGLE_CASE_DEFINITION_BY_KEY_AND_TENANT_ID_URL + "/create";

	  protected internal static readonly string DIAGRAM_DEFINITION_URL = SINGLE_CASE_DEFINITION_URL + "/diagram";

	  protected internal static readonly string UPDATE_HISTORY_TIME_TO_LIVE_URL = SINGLE_CASE_DEFINITION_URL + "/history-time-to-live";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  private RepositoryService repositoryServiceMock;
	  private CaseService caseServiceMock;
	  private CaseDefinitionQuery caseDefinitionQueryMock;
	  private CaseInstanceBuilder caseInstanceBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntime()
	  public virtual void setUpRuntime()
	  {
		CaseDefinition mockCaseDefinition = MockProvider.createMockCaseDefinition();

		UpRuntimeData = mockCaseDefinition;

		caseServiceMock = mock(typeof(CaseService));

		when(processEngine.CaseService).thenReturn(caseServiceMock);

		caseInstanceBuilder = mock(typeof(CaseInstanceBuilder));
		CaseInstance mockCaseInstance = MockProvider.createMockCaseInstance();

		when(caseServiceMock.withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(caseInstanceBuilder);
		when(caseInstanceBuilder.businessKey(anyString())).thenReturn(caseInstanceBuilder);
		when(caseInstanceBuilder.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(caseInstanceBuilder);
		when(caseInstanceBuilder.create()).thenReturn(mockCaseInstance);
	  }

	  private CaseDefinition UpRuntimeData
	  {
		  set
		  {
			repositoryServiceMock = mock(typeof(RepositoryService));
    
			when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
			when(repositoryServiceMock.getCaseDefinition(eq(MockProvider.EXAMPLE_CASE_DEFINITION_ID))).thenReturn(value);
			when(repositoryServiceMock.getCaseModel(eq(MockProvider.EXAMPLE_CASE_DEFINITION_ID))).thenReturn(createMockCaseDefinitionCmmnXml());
    
			caseDefinitionQueryMock = mock(typeof(CaseDefinitionQuery));
			when(caseDefinitionQueryMock.caseDefinitionKey(MockProvider.EXAMPLE_CASE_DEFINITION_KEY)).thenReturn(caseDefinitionQueryMock);
			when(caseDefinitionQueryMock.tenantIdIn(anyString())).thenReturn(caseDefinitionQueryMock);
			when(caseDefinitionQueryMock.withoutTenantId()).thenReturn(caseDefinitionQueryMock);
			when(caseDefinitionQueryMock.latestVersion()).thenReturn(caseDefinitionQueryMock);
			when(caseDefinitionQueryMock.singleResult()).thenReturn(value);
			when(caseDefinitionQueryMock.list()).thenReturn(Collections.singletonList(value));
			when(repositoryServiceMock.createCaseDefinitionQuery()).thenReturn(caseDefinitionQueryMock);
		  }
	  }

	  private Stream createMockCaseDefinitionCmmnXml()
	  {
		// do not close the input stream, will be done in implementation
		Stream cmmnXmlInputStream = ReflectUtil.getResourceAsStream("cases/case-model.cmmn");
		Assert.assertNotNull(cmmnXmlInputStream);
		return cmmnXmlInputStream;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDefinitionCmmnXmlRetrieval()
	  public virtual void testCaseDefinitionCmmnXmlRetrieval()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_CASE_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval()
	  public virtual void testDefinitionRetrieval()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(null)).when().get(SINGLE_CASE_DEFINITION_URL);

		verify(repositoryServiceMock).getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDefinitionCmmnXmlRetrieval_ByKey()
	  public virtual void testCaseDefinitionCmmnXmlRetrieval_ByKey()
	  {
		Response response = given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_BY_KEY_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_CASE_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKey()
	  public virtual void testDefinitionRetrieval_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(null)).when().get(SINGLE_CASE_DEFINITION_BY_KEY_URL);

		verify(caseDefinitionQueryMock).withoutTenantId();
		verify(repositoryServiceMock).getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingCaseDefinitionRetrieval_ByKey()
	  public virtual void testNonExistingCaseDefinitionRetrieval_ByKey()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";

		when(repositoryServiceMock.createCaseDefinitionQuery().caseDefinitionKey(nonExistingKey)).thenReturn(caseDefinitionQueryMock);
		when(caseDefinitionQueryMock.latestVersion()).thenReturn(caseDefinitionQueryMock);
		when(caseDefinitionQueryMock.singleResult()).thenReturn(null);
		when(caseDefinitionQueryMock.list()).thenReturn(System.Linq.Enumerable.Empty<CaseDefinition> ());

		given().pathParam("key", nonExistingKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching case definition with key: " + nonExistingKey)).when().get(SINGLE_CASE_DEFINITION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testDefinitionRetrieval_ByKeyAndTenantId()
	  {
		CaseDefinition mockDefinition = MockProvider.mockCaseDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeData = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_CASE_DEFINITION_BY_KEY_AND_TENANT_ID_URL);

		verify(caseDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(repositoryServiceMock).getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingCaseDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testNonExistingCaseDefinitionRetrieval_ByKeyAndTenantId()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";
		string nonExistingTenantId = "aNonExistingTenantId";

		when(repositoryServiceMock.createCaseDefinitionQuery().caseDefinitionKey(nonExistingKey)).thenReturn(caseDefinitionQueryMock);
		when(caseDefinitionQueryMock.singleResult()).thenReturn(null);

		given().pathParam("key", nonExistingKey).pathParam("tenant-id", nonExistingTenantId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching case definition with key: " + nonExistingKey + " and tenant-id: " + nonExistingTenantId)).when().get(SINGLE_CASE_DEFINITION_BY_KEY_AND_TENANT_ID_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionKeyAndTenantId()
	  public virtual void testCreateCaseInstanceByCaseDefinitionKeyAndTenantId()
	  {
		CaseDefinition mockDefinition = MockProvider.mockCaseDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeData = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_BY_KEY_AND_TENANT_ID_URL);

		verify(caseDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).create();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionId()
	  public virtual void testCreateCaseInstanceByCaseDefinitionId()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_URL);

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(null);
		verify(caseInstanceBuilder).Variables = null;
		verify(caseInstanceBuilder).create();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionKey()
	  public virtual void testCreateCaseInstanceByCaseDefinitionKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_BY_KEY_URL);

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(null);
		verify(caseInstanceBuilder).Variables = null;
		verify(caseInstanceBuilder).create();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionIdWithBusinessKey()
	  public virtual void testCreateCaseInstanceByCaseDefinitionIdWithBusinessKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["businessKey"] = MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_URL);

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		verify(caseInstanceBuilder).Variables = null;
		verify(caseInstanceBuilder).create();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionKeyWithBusinessKey()
	  public virtual void testCreateCaseInstanceByCaseDefinitionKeyWithBusinessKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["businessKey"] = MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY;

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_BY_KEY_URL);

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		verify(caseInstanceBuilder).Variables = null;
		verify(caseInstanceBuilder).create();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionIdWithVariables()
	  public virtual void testCreateCaseInstanceByCaseDefinitionIdWithVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = VariablesBuilder.getVariableValueMap("abc", ValueType.STRING.Name);
		variables["anotherVariableName"] = VariablesBuilder.getVariableValueMap(900, ValueType.INTEGER.Name);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_URL);

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(null);
		verify(caseInstanceBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("aVariableName", EqualsPrimitiveValue.stringValue("abc")).matcher("anotherVariableName", EqualsPrimitiveValue.integerValue(900)));
		verify(caseInstanceBuilder).create();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionKeyWithVariables()
	  public virtual void testCreateCaseInstanceByCaseDefinitionKeyWithVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = VariablesBuilder.getVariableValueMap("abc", null);
		variables["anotherVariableName"] = VariablesBuilder.getVariableValueMap(900, null);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["variables"] = variables;

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_BY_KEY_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariableName"] = "abc";
		expectedVariables["anotherVariableName"] = 999;

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey(null);
		verify(caseInstanceBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("aVariableName", EqualsUntypedValue.matcher().value("abc")).matcher("anotherVariableName", EqualsUntypedValue.matcher().value(900)));
		verify(caseInstanceBuilder).create();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionIdWithBusinessKeyAndVariables()
	  public virtual void testCreateCaseInstanceByCaseDefinitionIdWithBusinessKeyAndVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = VariablesBuilder.getVariableValueMap("abc", null);
		variables["anotherVariableName"] = VariablesBuilder.getVariableValueMap(900, null);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["variables"] = variables;
		@params["businessKey"] = "aBusinessKey";

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariableName"] = "abc";
		expectedVariables["anotherVariableName"] = 999;

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey("aBusinessKey");
		verify(caseInstanceBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("aVariableName", EqualsUntypedValue.matcher().value("abc")).matcher("anotherVariableName", EqualsUntypedValue.matcher().value(900)));
		verify(caseInstanceBuilder).create();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByCaseDefinitionKeyWithBusinessKeyAndVariables()
	  public virtual void testCreateCaseInstanceByCaseDefinitionKeyWithBusinessKeyAndVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = VariablesBuilder.getVariableValueMap("abc", null);
		variables["anotherVariableName"] = VariablesBuilder.getVariableValueMap(900, null);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["variables"] = variables;
		@params["businessKey"] = "aBusinessKey";

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).when().post(CREATE_INSTANCE_BY_KEY_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariableName"] = "abc";
		expectedVariables["anotherVariableName"] = 999;

		verify(caseServiceMock).withCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(caseInstanceBuilder).businessKey("aBusinessKey");
		verify(caseInstanceBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("aVariableName", EqualsUntypedValue.matcher().value("abc")).matcher("anotherVariableName", EqualsUntypedValue.matcher().value(900)));
		verify(caseInstanceBuilder).create();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByInvalidCaseDefinitionId()
	  public virtual void testCreateCaseInstanceByInvalidCaseDefinitionId()
	  {
		when(caseInstanceBuilder.create()).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", containsString("Cannot instantiate case definition aCaseDefnitionId: expected exception")).when().post(CREATE_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCaseInstanceByInvalidCaseDefinitionKey()
	  public virtual void testCreateCaseInstanceByInvalidCaseDefinitionKey()
	  {
		when(caseInstanceBuilder.create()).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("key", MockProvider.EXAMPLE_CASE_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", containsString("Cannot instantiate case definition aCaseDefnitionId: expected exception")).when().post(CREATE_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDiagramRetrieval() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCaseDiagramRetrieval()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("image/png").header("Content-Disposition", "attachment; filename=" + MockProvider.EXAMPLE_CASE_DEFINITION_DIAGRAM_RESOURCE_NAME).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(repositoryServiceMock).getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "case diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDiagramNullFilename() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCaseDiagramNullFilename()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID).DiagramResourceName).thenReturn(null);
		when(repositoryServiceMock.getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("application/octet-stream").header("Content-Disposition", "attachment; filename=" + null).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "case diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDiagramNotExist()
	  public virtual void testCaseDiagramNotExist()
	  {
		// setup additional mock behavior
		when(repositoryServiceMock.getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(null);

		// call method
		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().get(DIAGRAM_DEFINITION_URL);

		// verify service interaction
		verify(repositoryServiceMock).getCaseDefinition(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		verify(repositoryServiceMock).getCaseDiagram(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDiagramMediaType()
	  public virtual void testProcessDiagramMediaType()
	  {
		Assert.assertEquals("image/png", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.png"));
		Assert.assertEquals("image/png", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.PNG"));
		Assert.assertEquals("image/svg+xml", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.svg"));
		Assert.assertEquals("image/jpeg", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.jpeg"));
		Assert.assertEquals("image/jpeg", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.jpg"));
		Assert.assertEquals("image/gif", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.gif"));
		Assert.assertEquals("image/bmp", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.bmp"));
		Assert.assertEquals("application/octet-stream", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("process.UNKNOWN"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLive()
	  public virtual void testUpdateHistoryTimeToLive()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_CASE_DEFINITION_ID, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNullValue()
	  public virtual void testUpdateHistoryTimeToLiveNullValue()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).content(new HistoryTimeToLiveDto()).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_CASE_DEFINITION_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNegativeValue()
	  public virtual void testUpdateHistoryTimeToLiveNegativeValue()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new BadUserRequestException(expectedMessage)).when(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_CASE_DEFINITION_ID), eq(-1));

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).content(new HistoryTimeToLiveDto(-1)).contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(BadUserRequestException).Name)).body("message", containsString(expectedMessage)).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_CASE_DEFINITION_ID, -1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveAuthorizationException()
	  public virtual void testUpdateHistoryTimeToLiveAuthorizationException()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new AuthorizationException(expectedMessage)).when(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_CASE_DEFINITION_ID), eq(5));

		given().pathParam("id", MockProvider.EXAMPLE_CASE_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", containsString(expectedMessage)).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateCaseDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_CASE_DEFINITION_ID, 5);
	  }
	}

}