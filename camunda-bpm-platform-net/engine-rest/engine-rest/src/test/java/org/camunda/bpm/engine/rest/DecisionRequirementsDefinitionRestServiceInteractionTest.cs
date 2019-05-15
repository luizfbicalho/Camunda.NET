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
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>

	public class DecisionRequirementsDefinitionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string DECISION_REQUIREMENTS_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/decision-requirements-definition";
	  protected internal static readonly string SINGLE_DECISION_REQUIREMENTS_DEFINITION_ID_URL = DECISION_REQUIREMENTS_DEFINITION_URL + "/{id}";
	  protected internal static readonly string SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_URL = DECISION_REQUIREMENTS_DEFINITION_URL + "/key/{key}";
	  protected internal static readonly string SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_AND_TENANT_ID_URL = DECISION_REQUIREMENTS_DEFINITION_URL + "/key/{key}/tenant-id/{tenant-id}";

	  protected internal static readonly string XML_DEFINITION_URL = SINGLE_DECISION_REQUIREMENTS_DEFINITION_ID_URL + "/xml";

	  protected internal static readonly string DIAGRAM_DEFINITION_URL = SINGLE_DECISION_REQUIREMENTS_DEFINITION_ID_URL + "/diagram";

	  protected internal RepositoryService repositoryServiceMock;
	  protected internal DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionQueryMock;
	  protected internal DecisionService decisionServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntime() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUpRuntime()
	  {
		DecisionRequirementsDefinition mockDecisionRequirementsDefinition = MockProvider.createMockDecisionRequirementsDefinition();

		UpRuntimeData = mockDecisionRequirementsDefinition;
		decisionServiceMock = mock(typeof(DecisionService));
		when(processEngine.DecisionService).thenReturn(decisionServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDefinitionRetrievalById()
	  public virtual void decisionRequirementsDefinitionRetrievalById()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(null)).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_ID_URL);

		verify(repositoryServiceMock).getDecisionRequirementsDefinition(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nonExistingDecisionRequirementsDefinitionRetrieval()
	  public virtual void nonExistingDecisionRequirementsDefinitionRetrieval()
	  {
		string nonExistingId = "aNonExistingDefinitionId";

		when(repositoryServiceMock.getDecisionRequirementsDefinition(eq(nonExistingId))).thenThrow(new ProcessEngineException("No matching decision requirements definition"));

		given().pathParam("id", "aNonExistingDefinitionId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching decision requirements definition")).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_ID_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDefinitionRetrievalByKey()
	  public virtual void decisionRequirementsDefinitionRetrievalByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(null)).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_URL);

		verify(repositoryServiceMock).getDecisionRequirementsDefinition(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDefinitionRetrievalByNonExistingKey()
	  public virtual void decisionRequirementsDefinitionRetrievalByNonExistingKey()
	  {

		string nonExistingKey = "aNonExistingRequirementsDefinitionKey";

		when(repositoryServiceMock.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(nonExistingKey)).thenReturn(decisionRequirementsDefinitionQueryMock);

		when(decisionRequirementsDefinitionQueryMock.singleResult()).thenReturn(null);

		given().pathParam("key", nonExistingKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching decision requirements definition with key: " + nonExistingKey)).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDefinitionRetrievalByKeyAndTenantId() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void decisionRequirementsDefinitionRetrievalByKeyAndTenantId()
	  {
		DecisionRequirementsDefinition mockDefinition = MockProvider.mockDecisionRequirementsDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeData = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_AND_TENANT_ID_URL);

		verify(decisionRequirementsDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(repositoryServiceMock).getDecisionRequirementsDefinition(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nonExistingDecisionRequirementsDefinitionRetrievalByKeyAndTenantId()
	  public virtual void nonExistingDecisionRequirementsDefinitionRetrievalByKeyAndTenantId()
	  {
		string nonExistingKey = "aNonExistingDecisionDefinitionRequirementsDefinitionKey";
		string nonExistingTenantId = "aNonExistingTenantId";

		when(repositoryServiceMock.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(nonExistingKey)).thenReturn(decisionRequirementsDefinitionQueryMock);
		when(decisionRequirementsDefinitionQueryMock.singleResult()).thenReturn(null);

		given().pathParam("key", nonExistingKey).pathParam("tenant-id", nonExistingTenantId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching decision requirements definition with key: " + nonExistingKey + " and tenant-id: " + nonExistingTenantId)).when().get(SINGLE_DECISION_REQUIREMENTS_DEFINITION_KEY_AND_TENANT_ID_URL);
	  }

	  // dmn xml retrieval
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDefinitionDmnXmlRetrieval()
	  public virtual void decisionRequirementsDefinitionDmnXmlRetrieval()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

	  // DRD retrieval
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRequirementsDiagramRetrieval() throws java.io.FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void decisionRequirementsDiagramRetrieval()
	  {
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("image/png").header("Content-Disposition", "attachment; filename=" + MockProvider.EXAMPLE_DECISION_DEFINITION_DIAGRAM_RESOURCE_NAME).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		verify(repositoryServiceMock).getDecisionRequirementsDefinition(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		verify(repositoryServiceMock).getDecisionRequirementsDiagram(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);

		sbyte[] expected = IoUtil.readInputStream(new FileStream(File, FileMode.Open, FileAccess.Read), "decision requirements diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUpRuntimeData(org.camunda.bpm.engine.repository.DecisionRequirementsDefinition mockDecisionRequirementsDefinition) throws java.io.FileNotFoundException, java.net.URISyntaxException
	  protected internal virtual DecisionRequirementsDefinition UpRuntimeData
	  {
		  set
		  {
			repositoryServiceMock = mock(typeof(RepositoryService));
    
			when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
			when(repositoryServiceMock.getDecisionRequirementsDefinition(eq(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID))).thenReturn(value);
			when(repositoryServiceMock.getDecisionRequirementsModel(eq(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID))).thenReturn(createMockDecisionRequirementsDefinitionDmnXml());
			when(repositoryServiceMock.getDecisionRequirementsDiagram(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID)).thenReturn(createMockDecisionRequirementsDiagram());
    
			decisionRequirementsDefinitionQueryMock = mock(typeof(DecisionRequirementsDefinitionQuery));
			when(decisionRequirementsDefinitionQueryMock.decisionRequirementsDefinitionKey(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY)).thenReturn(decisionRequirementsDefinitionQueryMock);
			when(decisionRequirementsDefinitionQueryMock.tenantIdIn(anyString())).thenReturn(decisionRequirementsDefinitionQueryMock);
			when(decisionRequirementsDefinitionQueryMock.withoutTenantId()).thenReturn(decisionRequirementsDefinitionQueryMock);
			when(decisionRequirementsDefinitionQueryMock.latestVersion()).thenReturn(decisionRequirementsDefinitionQueryMock);
			when(decisionRequirementsDefinitionQueryMock.singleResult()).thenReturn(value);
			when(decisionRequirementsDefinitionQueryMock.list()).thenReturn(Collections.singletonList(value));
			when(repositoryServiceMock.createDecisionRequirementsDefinitionQuery()).thenReturn(decisionRequirementsDefinitionQueryMock);
		  }
	  }

	  protected internal virtual Stream createMockDecisionRequirementsDefinitionDmnXml()
	  {
		// do not close the input stream, will be done in implementation
		Stream dmnXmlInputStream = ReflectUtil.getResourceAsStream("decisions/decision-requirements-model.dmn");
		Assert.assertNotNull(dmnXmlInputStream);
		return dmnXmlInputStream;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.FileInputStream createMockDecisionRequirementsDiagram() throws java.net.URISyntaxException, java.io.FileNotFoundException
	  protected internal virtual FileStream createMockDecisionRequirementsDiagram()
	  {
		File file = File;
		return new FileStream(file, FileMode.Open, FileAccess.Read);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.File getFile() throws java.net.URISyntaxException
	  protected internal virtual File File
	  {
		  get
		  {
			return getFile("/decisions/decision-requirements-diagram.png");
		  }
	  }
	}

}