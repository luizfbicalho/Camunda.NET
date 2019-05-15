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
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyMapOf;
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
//	import static org.mockito.Mockito.when;


	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnEngineException = org.camunda.bpm.dmn.engine.DmnEngineException;
	using DecisionsEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionsEvaluationBuilder;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MockDecisionResultBuilder = org.camunda.bpm.engine.rest.helper.MockDecisionResultBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ProcessDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.ProcessDefinitionResourceImpl;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class DecisionDefinitionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string DECISION_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/decision-definition";
	  protected internal static readonly string SINGLE_DECISION_DEFINITION_URL = DECISION_DEFINITION_URL + "/{id}";
	  protected internal static readonly string SINGLE_DECISION_DEFINITION_BY_KEY_URL = DECISION_DEFINITION_URL + "/key/{key}";
	  protected internal static readonly string SINGLE_DECISION_DEFINITION_BY_KEY_AND_TENANT_ID_URL = DECISION_DEFINITION_URL + "/key/{key}/tenant-id/{tenant-id}";

	  protected internal static readonly string XML_DEFINITION_URL = SINGLE_DECISION_DEFINITION_URL + "/xml";
	  protected internal static readonly string XML_DEFINITION_BY_KEY_URL = SINGLE_DECISION_DEFINITION_BY_KEY_URL + "/xml";

	  protected internal static readonly string DIAGRAM_DEFINITION_URL = SINGLE_DECISION_DEFINITION_URL + "/diagram";

	  protected internal static readonly string EVALUATE_DECISION_URL = SINGLE_DECISION_DEFINITION_URL + "/evaluate";
	  protected internal static readonly string EVALUATE_DECISION_BY_KEY_URL = SINGLE_DECISION_DEFINITION_BY_KEY_URL + "/evaluate";
	  protected internal static readonly string EVALUATE_DECISION_BY_KEY_AND_TENANT_ID_URL = SINGLE_DECISION_DEFINITION_BY_KEY_AND_TENANT_ID_URL + "/evaluate";
	  protected internal static readonly string UPDATE_HISTORY_TIME_TO_LIVE_URL = SINGLE_DECISION_DEFINITION_URL + "/history-time-to-live";

	  private RepositoryService repositoryServiceMock;
	  private DecisionDefinitionQuery decisionDefinitionQueryMock;
	  private DecisionService decisionServiceMock;
	  private DecisionsEvaluationBuilder decisionEvaluationBuilderMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntime()
	  public virtual void setUpRuntime()
	  {
		DecisionDefinition mockDecisionDefinition = MockProvider.createMockDecisionDefinition();

		UpRuntimeData = mockDecisionDefinition;
		setUpDecisionService();
	  }

	  private DecisionDefinition UpRuntimeData
	  {
		  set
		  {
			repositoryServiceMock = mock(typeof(RepositoryService));
    
			when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
			when(repositoryServiceMock.getDecisionDefinition(eq(MockProvider.EXAMPLE_DECISION_DEFINITION_ID))).thenReturn(value);
			when(repositoryServiceMock.getDecisionModel(eq(MockProvider.EXAMPLE_DECISION_DEFINITION_ID))).thenReturn(createMockDecisionDefinitionDmnXml());
    
			decisionDefinitionQueryMock = mock(typeof(DecisionDefinitionQuery));
			when(decisionDefinitionQueryMock.decisionDefinitionKey(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY)).thenReturn(decisionDefinitionQueryMock);
			when(decisionDefinitionQueryMock.tenantIdIn(anyString())).thenReturn(decisionDefinitionQueryMock);
			when(decisionDefinitionQueryMock.withoutTenantId()).thenReturn(decisionDefinitionQueryMock);
			when(decisionDefinitionQueryMock.latestVersion()).thenReturn(decisionDefinitionQueryMock);
			when(decisionDefinitionQueryMock.singleResult()).thenReturn(value);
			when(decisionDefinitionQueryMock.list()).thenReturn(Collections.singletonList(value));
			when(repositoryServiceMock.createDecisionDefinitionQuery()).thenReturn(decisionDefinitionQueryMock);
		  }
	  }

	  private Stream createMockDecisionDefinitionDmnXml()
	  {
		// do not close the input stream, will be done in implementation
		Stream dmnXmlInputStream = ReflectUtil.getResourceAsStream("decisions/decision-model.dmn");
		Assert.assertNotNull(dmnXmlInputStream);
		return dmnXmlInputStream;
	  }

	  private void setUpDecisionService()
	  {
		decisionEvaluationBuilderMock = mock(typeof(DecisionsEvaluationBuilder));
		when(decisionEvaluationBuilderMock.variables(anyMapOf(typeof(string), typeof(object)))).thenReturn(decisionEvaluationBuilderMock);

		decisionServiceMock = mock(typeof(DecisionService));
		when(decisionServiceMock.evaluateDecisionById(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).thenReturn(decisionEvaluationBuilderMock);
		when(decisionServiceMock.evaluateDecisionByKey(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY)).thenReturn(decisionEvaluationBuilderMock);

		when(processEngine.DecisionService).thenReturn(decisionServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionDmnXmlRetrieval()
	  public virtual void testDecisionDefinitionDmnXmlRetrieval()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_DECISION_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval()
	  public virtual void testDefinitionRetrieval()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME)).body("decisionRequirementsDefinitionId", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("decisionRequirementsDefinitionKey", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("tenantId", equalTo(null)).when().get(SINGLE_DECISION_DEFINITION_URL);

		verify(repositoryServiceMock).getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDefinitionDmnXmlRetrieval_ByKey()
	  public virtual void testDecisionDefinitionDmnXmlRetrieval_ByKey()
	  {
		Response response = given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_BY_KEY_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_DECISION_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKey()
	  public virtual void testDefinitionRetrieval_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME)).body("decisionRequirementsDefinitionId", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("decisionRequirementsDefinitionKey", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("tenantId", equalTo(null)).when().get(SINGLE_DECISION_DEFINITION_BY_KEY_URL);

		verify(decisionDefinitionQueryMock).withoutTenantId();
		verify(repositoryServiceMock).getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingDecisionDefinitionRetrieval_ByKey()
	  public virtual void testNonExistingDecisionDefinitionRetrieval_ByKey()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";

		when(repositoryServiceMock.createDecisionDefinitionQuery().decisionDefinitionKey(nonExistingKey)).thenReturn(decisionDefinitionQueryMock);
		when(decisionDefinitionQueryMock.latestVersion()).thenReturn(decisionDefinitionQueryMock);
		when(decisionDefinitionQueryMock.singleResult()).thenReturn(null);
		when(decisionDefinitionQueryMock.list()).thenReturn(System.Linq.Enumerable.Empty<DecisionDefinition> ());

		given().pathParam("key", nonExistingKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching decision definition with key: " + nonExistingKey)).when().get(SINGLE_DECISION_DEFINITION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testDefinitionRetrieval_ByKeyAndTenantId()
	  {
		DecisionDefinition mockDefinition = MockProvider.mockDecisionDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeData = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME)).body("decisionRequirementsDefinitionId", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("decisionRequirementsDefinitionKey", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_DECISION_DEFINITION_BY_KEY_AND_TENANT_ID_URL);

		verify(decisionDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(repositoryServiceMock).getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingDecisionDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testNonExistingDecisionDefinitionRetrieval_ByKeyAndTenantId()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";
		string nonExistingTenantId = "aNonExistingTenantId";

		when(repositoryServiceMock.createDecisionDefinitionQuery().decisionDefinitionKey(nonExistingKey)).thenReturn(decisionDefinitionQueryMock);
		when(decisionDefinitionQueryMock.singleResult()).thenReturn(null);

		given().pathParam("key", nonExistingKey).pathParam("tenant-id", nonExistingTenantId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching decision definition with key: " + nonExistingKey + " and tenant-id: " + nonExistingTenantId)).when().get(SINGLE_DECISION_DEFINITION_BY_KEY_AND_TENANT_ID_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKeyAndTenantId()
	  public virtual void testEvaluateDecisionByKeyAndTenantId()
	  {
		DecisionDefinition mockDefinition = MockProvider.mockDecisionDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeData = mockDefinition;

		DmnDecisionResult decisionResult = MockProvider.createMockDecisionResult();
		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("amount", 420).variable("invoiceCategory", "MISC").Variables;

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EVALUATE_DECISION_BY_KEY_AND_TENANT_ID_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["amount"] = 420;
		expectedVariables["invoiceCategory"] = "MISC";

		verify(decisionDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(decisionEvaluationBuilderMock).variables(expectedVariables);
		verify(decisionEvaluationBuilderMock).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDiagramRetrieval() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDecisionDiagramRetrieval()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("image/png").header("Content-Disposition", "attachment; filename=" + MockProvider.EXAMPLE_DECISION_DEFINITION_DIAGRAM_RESOURCE_NAME).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
		verify(repositoryServiceMock).getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "decision diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDiagramNullFilename() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDecisionDiagramNullFilename()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID).DiagramResourceName).thenReturn(null);
		when(repositoryServiceMock.getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("application/octet-stream").header("Content-Disposition", "attachment; filename=" + null).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "decision diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDiagramNotExist()
	  public virtual void testDecisionDiagramNotExist()
	  {
		// setup additional mock behavior
		when(repositoryServiceMock.getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID)).thenReturn(null);

		// call method
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().get(DIAGRAM_DEFINITION_URL);

		// verify service interaction
		verify(repositoryServiceMock).getDecisionDefinition(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
		verify(repositoryServiceMock).getDecisionDiagram(MockProvider.EXAMPLE_DECISION_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecisionDiagramMediaType()
	  public virtual void testDecisionDiagramMediaType()
	  {
		Assert.assertEquals("image/png", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.png"));
		Assert.assertEquals("image/png", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.PNG"));
		Assert.assertEquals("image/svg+xml", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.svg"));
		Assert.assertEquals("image/jpeg", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.jpeg"));
		Assert.assertEquals("image/jpeg", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.jpg"));
		Assert.assertEquals("image/gif", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.gif"));
		Assert.assertEquals("image/bmp", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.bmp"));
		Assert.assertEquals("application/octet-stream", ProcessDefinitionResourceImpl.getMediaTypeForFileSuffix("decision.UNKNOWN"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey()
	  public virtual void testEvaluateDecisionByKey()
	  {
		DmnDecisionResult decisionResult = MockProvider.createMockDecisionResult();

		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("amount", 420).variable("invoiceCategory", "MISC").Variables;

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EVALUATE_DECISION_BY_KEY_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["amount"] = 420;
		expectedVariables["invoiceCategory"] = "MISC";

		verify(decisionEvaluationBuilderMock).variables(expectedVariables);
		verify(decisionEvaluationBuilderMock).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionById()
	  public virtual void testEvaluateDecisionById()
	  {
		DmnDecisionResult decisionResult = MockProvider.createMockDecisionResult();

		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("amount", 420).variable("invoiceCategory", "MISC").Variables;

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EVALUATE_DECISION_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["amount"] = 420;
		expectedVariables["invoiceCategory"] = "MISC";

		verify(decisionEvaluationBuilderMock).variables(expectedVariables);
		verify(decisionEvaluationBuilderMock).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionSingleDecisionOutput()
	  public virtual void testEvaluateDecisionSingleDecisionOutput()
	  {
		DmnDecisionResult decisionResult = (new MockDecisionResultBuilder()).resultEntries().entry("status", Variables.stringValue("gold")).build();

		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("size()", @is(1)).body("[0].size()", @is(1)).body("[0].status", @is(notNullValue())).body("[0].status.value", @is("gold")).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionMultipleDecisionOutputs()
	  public virtual void testEvaluateDecisionMultipleDecisionOutputs()
	  {
		DmnDecisionResult decisionResult = (new MockDecisionResultBuilder()).resultEntries().entry("status", Variables.stringValue("gold")).resultEntries().entry("assignee", Variables.stringValue("manager")).build();

		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("size()", @is(2)).body("[0].size()", @is(1)).body("[0].status.value", @is("gold")).body("[1].size()", @is(1)).body("[1].assignee.value", @is("manager")).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionMultipleDecisionValues()
	  public virtual void testEvaluateDecisionMultipleDecisionValues()
	  {
		DmnDecisionResult decisionResult = (new MockDecisionResultBuilder()).resultEntries().entry("status", Variables.stringValue("gold")).entry("assignee", Variables.stringValue("manager")).build();

		when(decisionEvaluationBuilderMock.evaluate()).thenReturn(decisionResult);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("size()", @is(1)).body("[0].size()", @is(2)).body("[0].status.value", @is("gold")).body("[0].assignee.value", @is("manager")).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecision_NotFound()
	  public virtual void testEvaluateDecision_NotFound()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new NotFoundException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey_NotFound()
	  public virtual void testEvaluateDecisionByKey_NotFound()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new NotFoundException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecision_NotValid()
	  public virtual void testEvaluateDecision_NotValid()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new NotValidException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey_NotValid()
	  public virtual void testEvaluateDecisionByKey_NotValid()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new NotValidException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecision_NotAuthorized()
	  public virtual void testEvaluateDecision_NotAuthorized()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new AuthorizationException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey_NotAuthorized()
	  public virtual void testEvaluateDecisionByKey_NotAuthorized()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new AuthorizationException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecision_ProcessEngineException()
	  public virtual void testEvaluateDecision_ProcessEngineException()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new ProcessEngineException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey_ProcessEngineException()
	  public virtual void testEvaluateDecisionByKey_ProcessEngineException()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new ProcessEngineException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecision_DmnEngineException()
	  public virtual void testEvaluateDecision_DmnEngineException()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new DmnEngineException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEvaluateDecisionByKey_DmnEngineException()
	  public virtual void testEvaluateDecisionByKey_DmnEngineException()
	  {
		string message = "expected message";
		when(decisionEvaluationBuilderMock.evaluate()).thenThrow(new DmnEngineException(message));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = Collections.emptyMap();

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString(message)).when().post(EVALUATE_DECISION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLive()
	  public virtual void testUpdateHistoryTimeToLive()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_DECISION_DEFINITION_ID, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNullValue()
	  public virtual void testUpdateHistoryTimeToLiveNullValue()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).content(new HistoryTimeToLiveDto()).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_DECISION_DEFINITION_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNegativeValue()
	  public virtual void testUpdateHistoryTimeToLiveNegativeValue()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new BadUserRequestException(expectedMessage)).when(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_DECISION_DEFINITION_ID), eq(-1));

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).content(new HistoryTimeToLiveDto(-1)).contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(BadUserRequestException).Name)).body("message", containsString(expectedMessage)).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_DECISION_DEFINITION_ID, -1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveAuthorizationException()
	  public virtual void testUpdateHistoryTimeToLiveAuthorizationException()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new AuthorizationException(expectedMessage)).when(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_DECISION_DEFINITION_ID), eq(5));

		given().pathParam("id", MockProvider.EXAMPLE_DECISION_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", containsString(expectedMessage)).when().put(UPDATE_HISTORY_TIME_TO_LIVE_URL);

		verify(repositoryServiceMock).updateDecisionDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_DECISION_DEFINITION_ID, 5);
	  }

	}

}