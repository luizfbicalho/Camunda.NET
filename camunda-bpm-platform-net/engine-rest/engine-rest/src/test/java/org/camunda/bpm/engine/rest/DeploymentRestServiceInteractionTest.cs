using System;
using System.Collections;
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
	using ContentType = io.restassured.http.ContentType;
	using JsonPath = io.restassured.path.json.JsonPath;
	using Response = io.restassured.response.Response;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using org.camunda.bpm.engine.repository;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
	using static org.camunda.bpm.engine.rest.helper.MockProvider;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	public class DeploymentRestServiceInteractionTest : AbstractRestServiceTest
	{

	  protected internal const string PROPERTY_DEPLOYED_PROCESS_DEFINITIONS = "deployedProcessDefinitions";
	  protected internal const string PROPERTY_DEPLOYED_CASE_DEFINITIONS = "deployedCaseDefinitions";
	  protected internal const string PROPERTY_DEPLOYED_DECISION_DEFINITIONS = "deployedDecisionDefinitions";
	  protected internal const string PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS = "deployedDecisionRequirementsDefinitions";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/deployment";
	  protected internal static readonly string DEPLOYMENT_URL = RESOURCE_URL + "/{id}";
	  protected internal static readonly string RESOURCES_URL = DEPLOYMENT_URL + "/resources";
	  protected internal static readonly string SINGLE_RESOURCE_URL = RESOURCES_URL + "/{resourceId}";
	  protected internal static readonly string SINGLE_RESOURCE_DATA_URL = SINGLE_RESOURCE_URL + "/data";
	  protected internal static readonly string CREATE_DEPLOYMENT_URL = RESOURCE_URL + "/create";
	  protected internal static readonly string REDEPLOY_DEPLOYMENT_URL = DEPLOYMENT_URL + "/redeploy";

	  protected internal RepositoryService mockRepositoryService;
	  protected internal Deployment mockDeployment;
	  protected internal DeploymentWithDefinitions mockDeploymentWithDefinitions;
	  protected internal IList<Resource> mockDeploymentResources;
	  protected internal Resource mockDeploymentResource;
	  protected internal DeploymentQuery mockDeploymentQuery;
	  protected internal DeploymentBuilder mockDeploymentBuilder;
	  protected internal ICollection<string> resourceNames = new List<string>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockRepositoryService = mock(typeof(RepositoryService));
		when(processEngine.RepositoryService).thenReturn(mockRepositoryService);

		mockDeployment = createMockDeployment();
		mockDeploymentWithDefinitions = createMockDeploymentWithDefinitions();
		mockDeploymentQuery = mock(typeof(DeploymentQuery));
		when(mockDeploymentQuery.deploymentId(EXAMPLE_DEPLOYMENT_ID)).thenReturn(mockDeploymentQuery);
		when(mockDeploymentQuery.singleResult()).thenReturn(mockDeployment);
		when(mockRepositoryService.createDeploymentQuery()).thenReturn(mockDeploymentQuery);

		mockDeploymentResources = createMockDeploymentResources();
		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(mockDeploymentResources);

		mockDeploymentResource = createMockDeploymentResource();

		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_RESOURCE_ID))).thenReturn(createMockDeploymentResourceBpmnData());

		mockDeploymentBuilder = mock(typeof(DeploymentBuilder));
		when(mockRepositoryService.createDeployment()).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.addInputStream(anyString(), any(typeof(Stream)))).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.addDeploymentResourcesById(anyString(), anyListOf(typeof(string)))).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.addDeploymentResourcesByName(anyString(), anyListOf(typeof(string)))).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.source(anyString())).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.tenantId(anyString())).thenReturn(mockDeploymentBuilder);
		when(mockDeploymentBuilder.ResourceNames).thenReturn(resourceNames);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeploymentWithDefinitions);
	  }

	  private sbyte[] createMockDeploymentResourceByteData()
	  {
		return "someContent".GetBytes();
	  }

	  private Stream createMockDeploymentResourceBpmnData()
	  {
		// do not close the input stream, will be done in implementation
		Stream bpmn20XmlIn = ReflectUtil.getResourceAsStream("processes/fox-invoice_en_long_id.bpmn");
		assertNotNull(bpmn20XmlIn);
		return bpmn20XmlIn;
	  }

	  private Stream createMockDeploymentResourceBpmnDataNonExecutableProcess()
	  {
		// do not close the input stream, will be done in implementation
		string model = Bpmn.convertToString(Bpmn.createProcess().startEvent().endEvent().done());
		Stream inputStream = new MemoryStream(model.GetBytes());
		return inputStream;
	  }

	  private Stream createMockDeploymentResourceSvgData()
	  {
		// do not close the input stream, will be done in implementation
		Stream image = ReflectUtil.getResourceAsStream("processes/diagram.svg");
		assertNotNull(image);
		return image;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleDeployment()
	  public virtual void testGetSingleDeployment()
	  {

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_URL);

		verifyDeployment(mockDeployment, response);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingSingleDeployment()
	  public virtual void testGetNonExistingSingleDeployment()
	  {

		when(mockDeploymentQuery.deploymentId(NON_EXISTING_DEPLOYMENT_ID)).thenReturn(mockDeploymentQuery);
		when(mockDeploymentQuery.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment with id '" + NON_EXISTING_DEPLOYMENT_ID + "' does not exist")).when().get(DEPLOYMENT_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResources()
	  public virtual void testGetDeploymentResources()
	  {

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(RESOURCES_URL);

		verifyDeploymentResources(mockDeploymentResources, response);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingDeploymentResources()
	  public virtual void testGetNonExistingDeploymentResources()
	  {

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resources for deployment id '" + NON_EXISTING_DEPLOYMENT_ID + "' do not exist.")).when().get(RESOURCES_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourcesThrowsAuthorizationException()
	  public virtual void testGetDeploymentResourcesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockRepositoryService.getDeploymentResources(EXAMPLE_DEPLOYMENT_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().get(RESOURCES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResource()
	  public virtual void testGetDeploymentResource()
	  {

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_RESOURCE_URL);

		verifyDeploymentResource(mockDeploymentResource, response);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingDeploymentResource()
	  public virtual void testGetNonExistingDeploymentResource()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", NON_EXISTING_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resource with resource id '" + NON_EXISTING_DEPLOYMENT_RESOURCE_ID + "' for deployment id '" + EXAMPLE_DEPLOYMENT_ID + "' does not exist.")).when().get(SINGLE_RESOURCE_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceWithNonExistingDeploymentId()
	  public virtual void testGetDeploymentResourceWithNonExistingDeploymentId()
	  {

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resources for deployment id '" + NON_EXISTING_DEPLOYMENT_ID + "' do not exist.")).when().get(SINGLE_RESOURCE_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceWithNonExistingDeploymentIdAndNonExistingResourceId()
	  public virtual void testGetDeploymentResourceWithNonExistingDeploymentIdAndNonExistingResourceId()
	  {

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).pathParam("resourceId", NON_EXISTING_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resources for deployment id '" + NON_EXISTING_DEPLOYMENT_ID + "' do not exist.")).when().get(SINGLE_RESOURCE_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceThrowsAuthorizationException()
	  public virtual void testGetDeploymentResourceThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockRepositoryService.getDeploymentResources(EXAMPLE_DEPLOYMENT_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().get(SINGLE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceData()
	  public virtual void testGetDeploymentResourceData()
	  {

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("application/octet-stream").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains("<?xml"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentSvgResourceData()
	  public virtual void testGetDeploymentSvgResourceData()
	  {
		Resource resource = createMockDeploymentSvgResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_SVG_RESOURCE_ID))).thenReturn(createMockDeploymentResourceSvgData());

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_SVG_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/svg+xml").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_SVG_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentPngResourceData()
	  public virtual void testGetDeploymentPngResourceData()
	  {
		Resource resource = createMockDeploymentPngResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_PNG_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_PNG_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/png").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_PNG_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentGifResourceData()
	  public virtual void testGetDeploymentGifResourceData()
	  {
		Resource resource = createMockDeploymentGifResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_GIF_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_GIF_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/gif").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_GIF_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJpgResourceData()
	  public virtual void testGetDeploymentJpgResourceData()
	  {
		Resource resource = createMockDeploymentJpgResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JPG_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JPG_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/jpeg").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JPG_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJpegResourceData()
	  public virtual void testGetDeploymentJpegResourceData()
	  {
		Resource resource = createMockDeploymentJpegResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/jpeg").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJpeResourceData()
	  public virtual void testGetDeploymentJpeResourceData()
	  {
		Resource resource = createMockDeploymentJpeResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JPE_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JPE_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/jpeg").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JPE_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentTifResourceData()
	  public virtual void testGetDeploymentTifResourceData()
	  {
		Resource resource = createMockDeploymentTifResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_TIF_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_TIF_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/tiff").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_TIF_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentTiffResourceData()
	  public virtual void testGetDeploymentTiffResourceData()
	  {
		Resource resource = createMockDeploymentTiffResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType("image/tiff").header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentBpmnResourceData()
	  public virtual void testGetDeploymentBpmnResourceData()
	  {
		Resource resource = createMockDeploymentBpmnResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_ID))).thenReturn(createMockDeploymentResourceBpmnData());

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentBpmnXmlResourceData()
	  public virtual void testGetDeploymentBpmnXmlResourceData()
	  {
		Resource resource = createMockDeploymentBpmnXmlResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_ID))).thenReturn(createMockDeploymentResourceBpmnData());

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentCmmnResourceData()
	  public virtual void testGetDeploymentCmmnResourceData()
	  {
		Resource resource = createMockDeploymentCmmnResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentCmmnXmlResourceData()
	  public virtual void testGetDeploymentCmmnXmlResourceData()
	  {
		Resource resource = createMockDeploymentCmmnXmlResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentDmnResourceData()
	  public virtual void testGetDeploymentDmnResourceData()
	  {
		Resource resource = createMockDeploymentDmnResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_DMN_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_DMN_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_DMN_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentDmnXmlResourceData()
	  public virtual void testGetDeploymentDmnXmlResourceData()
	  {
		Resource resource = createMockDeploymentDmnXmlResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentXmlResourceData()
	  public virtual void testGetDeploymentXmlResourceData()
	  {
		Resource resource = createMockDeploymentXmlResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_XML_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_XML_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_XML_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJsonResourceData()
	  public virtual void testGetDeploymentJsonResourceData()
	  {
		Resource resource = createMockDeploymentJsonResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JSON_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JSON_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JSON_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentGroovyResourceData()
	  public virtual void testGetDeploymentGroovyResourceData()
	  {
		Resource resource = createMockDeploymentGroovyResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJavaResourceData()
	  public virtual void testGetDeploymentJavaResourceData()
	  {
		Resource resource = createMockDeploymentJavaResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentJsResourceData()
	  public virtual void testGetDeploymentJsResourceData()
	  {
		Resource resource = createMockDeploymentJsResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_JS_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_JS_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_JS_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentPythonResourceData()
	  public virtual void testGetDeploymentPythonResourceData()
	  {
		Resource resource = createMockDeploymentPythonResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentRubyResourceData()
	  public virtual void testGetDeploymentRubyResourceData()
	  {
		Resource resource = createMockDeploymentRubyResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentPhpResourceData()
	  public virtual void testGetDeploymentPhpResourceData()
	  {
		Resource resource = createMockDeploymentPhpResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_PHP_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_PHP_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_PHP_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentHtmlpResourceData()
	  public virtual void testGetDeploymentHtmlpResourceData()
	  {
		Resource resource = createMockDeploymentHtmlResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_HTML_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_HTML_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.HTML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_HTML_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentTxtResourceData()
	  public virtual void testGetDeploymentTxtResourceData()
	  {
		Resource resource = createMockDeploymentTxtResource();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_TXT_RESOURCE_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_TXT_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_TXT_RESOURCE_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataFilename()
	  public virtual void testGetDeploymentResourceDataFilename()
	  {
		Resource resource = createMockDeploymentResourceFilename();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataFilenameBackslash()
	  public virtual void testGetDeploymentResourceDataFilenameBackslash()
	  {
		Resource resource = createMockDeploymentResourceFilenameBackslash();

		IList<Resource> resources = new List<Resource>();
		resources.Add(resource);

		Stream input = new MemoryStream(createMockDeploymentResourceByteData());

		when(mockRepositoryService.getDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID))).thenReturn(resources);
		when(mockRepositoryService.getResourceAsStreamById(eq(EXAMPLE_DEPLOYMENT_ID), eq(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID))).thenReturn(input);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.XML).header("Content-Disposition", "attachment; filename=" + EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_NAME).when().get(SINGLE_RESOURCE_DATA_URL);

		string responseContent = response.asString();
		assertNotNull(responseContent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataForNonExistingDeploymentId()
	  public virtual void testGetDeploymentResourceDataForNonExistingDeploymentId()
	  {

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resource '" + EXAMPLE_DEPLOYMENT_RESOURCE_ID + "' for deployment id '" + NON_EXISTING_DEPLOYMENT_ID + "' does not exist.")).when().get(SINGLE_RESOURCE_DATA_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataForNonExistingResourceId()
	  public virtual void testGetDeploymentResourceDataForNonExistingResourceId()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", NON_EXISTING_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resource '" + NON_EXISTING_DEPLOYMENT_RESOURCE_ID + "' for deployment id '" + EXAMPLE_DEPLOYMENT_ID + "' does not exist.")).when().get(SINGLE_RESOURCE_DATA_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataForNonExistingDeploymentIdAndNonExistingResourceId()
	  public virtual void testGetDeploymentResourceDataForNonExistingDeploymentIdAndNonExistingResourceId()
	  {

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).pathParam("resourceId", NON_EXISTING_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment resource '" + NON_EXISTING_DEPLOYMENT_RESOURCE_ID + "' for deployment id '" + NON_EXISTING_DEPLOYMENT_ID + "' does not exist.")).when().get(SINGLE_RESOURCE_DATA_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeploymentResourceDataThrowsAuthorizationException()
	  public virtual void testGetDeploymentResourceDataThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockRepositoryService.getResourceAsStreamById(EXAMPLE_DEPLOYMENT_ID, EXAMPLE_DEPLOYMENT_RESOURCE_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).pathParam("resourceId", EXAMPLE_DEPLOYMENT_RESOURCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().get(SINGLE_RESOURCE_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteDeployment() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteDeployment()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verifyCreatedDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteBpmnDeployment() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteBpmnDeployment()
	  {
		// given
		DeploymentWithDefinitions mockDeployment = createMockDeploymentWithDefinitions();
		when(mockDeployment.DeployedDecisionDefinitions).thenReturn(null);
		when(mockDeployment.DeployedCaseDefinitions).thenReturn(null);
		when(mockDeployment.DeployedDecisionRequirementsDefinitions).thenReturn(null);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeployment);

		// when
		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		// then
		verifyCreatedBpmnDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteCmmnDeployment() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteCmmnDeployment()
	  {
		// given
		DeploymentWithDefinitions mockDeployment = createMockDeploymentWithDefinitions();
		when(mockDeployment.DeployedDecisionDefinitions).thenReturn(null);
		when(mockDeployment.DeployedProcessDefinitions).thenReturn(null);
		when(mockDeployment.DeployedDecisionRequirementsDefinitions).thenReturn(null);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeployment);

		// when
		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		// then
		verifyCreatedCmmnDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteDmnDeployment() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteDmnDeployment()
	  {
		// given
		DeploymentWithDefinitions mockDeployment = createMockDeploymentWithDefinitions();
		when(mockDeployment.DeployedCaseDefinitions).thenReturn(null);
		when(mockDeployment.DeployedProcessDefinitions).thenReturn(null);
		when(mockDeployment.DeployedDecisionRequirementsDefinitions).thenReturn(null);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeployment);

		// when
		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		// then
		verifyCreatedDmnDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteDrdDeployment() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteDrdDeployment()
	  {
		// given
		DeploymentWithDefinitions mockDeployment = createMockDeploymentWithDefinitions();
		when(mockDeployment.DeployedCaseDefinitions).thenReturn(null);
		when(mockDeployment.DeployedProcessDefinitions).thenReturn(null);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeployment);

		// when
		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		// then
		verifyCreatedDrdDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentWithNonExecutableProcess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentWithNonExecutableProcess()
	  {

		// given
		DeploymentWithDefinitions mockDeployment = createMockDeploymentWithDefinitions();
		when(mockDeployment.DeployedDecisionDefinitions).thenReturn(null);
		when(mockDeployment.DeployedCaseDefinitions).thenReturn(null);
		when(mockDeployment.DeployedProcessDefinitions).thenReturn(null);
		when(mockDeployment.DeployedDecisionRequirementsDefinitions).thenReturn(null);
		when(mockDeploymentBuilder.deployWithResult()).thenReturn(mockDeployment);

		// when
		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnDataNonExecutableProcess()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		// then
		verifyCreatedEmptyDeployment(mockDeployment, response);

		verify(mockDeploymentBuilder).name(EXAMPLE_DEPLOYMENT_ID);
		verify(mockDeploymentBuilder).enableDuplicateFiltering(false);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteDeploymentDeployChangedOnly() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteDeploymentDeployChangedOnly()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("deploy-changed-only", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).enableDuplicateFiltering(true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteDeploymentConflictingDuplicateSetting() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateCompleteDeploymentConflictingDuplicateSetting()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		// deploy-changed-only should override enable-duplicate-filtering
		given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("enable-duplicate-filtering", "false").multiPart("deploy-changed-only", "true").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).enableDuplicateFiltering(true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentWithDeploymentSource() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentWithDeploymentSource()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		// deploy-changed-only should override enable-duplicate-filtering
		given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("enable-duplicate-filtering", "false").multiPart("deployment-source", "my-deployment-source").expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).source("my-deployment-source");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentWithTenantId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentWithTenantId()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("tenant-id", EXAMPLE_TENANT_ID).expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).tenantId(EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentOnlyWithBytes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentOnlyWithBytes()
	  {

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		Response response = given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).expect().statusCode(Status.OK.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

		verifyCreatedDeployment(mockDeployment, response);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentWithoutBytes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentWithoutBytes()
	  {

		given().multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentWithNonExistentPart() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateDeploymentWithNonExistentPart()
	  {

		given().multiPart("non-existent-body-part", EXAMPLE_DEPLOYMENT_ID).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(CREATE_DEPLOYMENT_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateDeploymentThrowsAuthorizationException()
	  public virtual void testCreateDeploymentThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockDeploymentBuilder.deployWithResult()).thenThrow(new AuthorizationException(message));

		resourceNames.addAll(Arrays.asList("data", "more-data"));

		given().multiPart("data", "unspecified", createMockDeploymentResourceByteData()).multiPart("more-data", "unspecified", createMockDeploymentResourceBpmnData()).multiPart("deployment-name", EXAMPLE_DEPLOYMENT_ID).multiPart("enable-duplicate-filtering", "true").then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().post(CREATE_DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeployment()
	  public virtual void testDeleteDeployment()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascade()
	  public virtual void testDeleteDeploymentCascade()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascadeNonsense()
	  public virtual void testDeleteDeploymentCascadeNonsense()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", "bla").expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascadeFalse()
	  public virtual void testDeleteDeploymentCascadeFalse()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", false).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListeners()
	  public virtual void testDeleteDeploymentSkipCustomListeners()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("skipCustomListeners", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersNonsense()
	  public virtual void testDeleteDeploymentSkipCustomListenersNonsense()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("skipCustomListeners", "bla").expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersFalse()
	  public virtual void testDeleteDeploymentSkipCustomListenersFalse()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("skipCustomListeners", false).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersAndCascade()
	  public virtual void testDeleteDeploymentSkipCustomListenersAndCascade()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", true).queryParam("skipCustomListeners", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, true, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipIoMappings()
	  public virtual void testDeleteDeploymentSkipIoMappings()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", true).queryParam("skipIoMappings", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, true, false, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipIoMappingsFalse()
	  public virtual void testDeleteDeploymentSkipIoMappingsFalse()
	  {

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).queryParam("cascade", true).queryParam("skipIoMappings", false).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(DEPLOYMENT_URL);

		verify(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingDeployment()
	  public virtual void testDeleteNonExistingDeployment()
	  {

		when(mockDeploymentQuery.deploymentId(NON_EXISTING_DEPLOYMENT_ID)).thenReturn(mockDeploymentQuery);
		when(mockDeploymentQuery.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_DEPLOYMENT_ID).expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deployment with id '" + NON_EXISTING_DEPLOYMENT_ID + "' do not exist")).when().delete(DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentThrowsAuthorizationException()
	  public virtual void testDeleteDeploymentThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockRepositoryService).deleteDeployment(EXAMPLE_DEPLOYMENT_ID, false, false, false);

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().delete(DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeployment()
	  public virtual void testRedeployDeployment()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<string> resourceIds = new List<string>();
		resourceIds.Add("first-resource-id");
		resourceIds.Add("second-resource-id");
		json["resourceIds"] = resourceIds;

		IList<string> resourceNames = new List<string>();
		resourceNames.Add("first-resource-name");
		resourceNames.Add("second-resource-name");
		json["resourceNames"] = resourceNames;

		json["source"] = EXAMPLE_DEPLOYMENT_SOURCE;

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder, never()).addDeploymentResources(anyString());
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder).addDeploymentResourcesById(eq(EXAMPLE_DEPLOYMENT_ID), eq(resourceIds));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder).addDeploymentResourcesByName(eq(EXAMPLE_DEPLOYMENT_ID), eq(resourceNames));
		verify(mockDeploymentBuilder).source(EXAMPLE_DEPLOYMENT_SOURCE);
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentWithoutRequestBody()
	  public virtual void testRedeployDeploymentWithoutRequestBody()
	  {
		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).addDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesById(anyString(), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesByName(anyString(), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder, never()).source(anyString());
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentEmptyRequestBody()
	  public virtual void testRedeployDeploymentEmptyRequestBody()
	  {
		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).body("{}").expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).addDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesById(eq(EXAMPLE_DEPLOYMENT_ID), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesByName(eq(EXAMPLE_DEPLOYMENT_ID), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder).source(null);
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentResourceIds()
	  public virtual void testRedeployDeploymentResourceIds()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<string> resourceIds = new List<string>();
		resourceIds.Add("first-resource-id");
		resourceIds.Add("second-resource-id");
		json["resourceIds"] = resourceIds;

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder, never()).addDeploymentResources(anyString());
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder).addDeploymentResourcesById(eq(EXAMPLE_DEPLOYMENT_ID), eq(resourceIds));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesByName(eq(EXAMPLE_DEPLOYMENT_ID), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder).source(null);
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentResourceNames()
	  public virtual void testRedeployDeploymentResourceNames()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<string> resourceNames = new List<string>();
		resourceNames.Add("first-resource-name");
		resourceNames.Add("second-resource-name");
		json["resourceNames"] = resourceNames;

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder, never()).addDeploymentResources(anyString());
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesById(eq(EXAMPLE_DEPLOYMENT_ID), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder).addDeploymentResourcesByName(eq(EXAMPLE_DEPLOYMENT_ID), eq(resourceNames));
		verify(mockDeploymentBuilder).source(null);
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentSource()
	  public virtual void testRedeployDeploymentSource()
	  {
		IDictionary<string, string> json = new Dictionary<string, string>();
		json["source"] = EXAMPLE_DEPLOYMENT_SOURCE;

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).addDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder).nameFromDeployment(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceById(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesById(anyString(), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder, never()).addDeploymentResourceByName(anyString(), anyString());
		verify(mockDeploymentBuilder, never()).addDeploymentResourcesByName(anyString(), anyListOf(typeof(string)));
		verify(mockDeploymentBuilder).source(eq(EXAMPLE_DEPLOYMENT_SOURCE));
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentWithoutTenantId()
	  public virtual void testRedeployDeploymentWithoutTenantId()
	  {
		when(mockDeployment.TenantId).thenReturn(null);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).addDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder, never()).tenantId(any(typeof(string)));
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployDeploymentWithTenantId()
	  public virtual void testRedeployDeploymentWithTenantId()
	  {
		when(mockDeployment.TenantId).thenReturn(EXAMPLE_TENANT_ID);

		Response response = given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(REDEPLOY_DEPLOYMENT_URL);

		verify(mockDeploymentBuilder).addDeploymentResources(eq(EXAMPLE_DEPLOYMENT_ID));
		verify(mockDeploymentBuilder).tenantId(eq(EXAMPLE_TENANT_ID));
		verify(mockDeploymentBuilder).deployWithResult();

		verifyDeployment(mockDeployment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployThrowsNotFoundException()
	  public virtual void testRedeployThrowsNotFoundException()
	  {
		string message = "deployment not found";
		doThrow(new NotFoundException(message)).when(mockDeploymentBuilder).deployWithResult();

		string expected = "Cannot redeploy deployment '" + EXAMPLE_DEPLOYMENT_ID + "': " + message;

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(expected)).when().post(REDEPLOY_DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployThrowsNotValidException()
	  public virtual void testRedeployThrowsNotValidException()
	  {
		string message = "not valid";
		doThrow(new NotValidException(message)).when(mockDeploymentBuilder).deployWithResult();

		string expected = "Cannot redeploy deployment '" + EXAMPLE_DEPLOYMENT_ID + "': " + message;

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(expected)).when().post(REDEPLOY_DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployThrowsProcessEngineException()
	  public virtual void testRedeployThrowsProcessEngineException()
	  {
		string message = "something went wrong";
		doThrow(new ProcessEngineException(message)).when(mockDeploymentBuilder).deployWithResult();

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(message)).when().post(REDEPLOY_DEPLOYMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRedeployThrowsAuthorizationException()
	  public virtual void testRedeployThrowsAuthorizationException()
	  {
		string message = "missing authorization";
		doThrow(new AuthorizationException(message)).when(mockDeploymentBuilder).deployWithResult();

		given().pathParam("id", EXAMPLE_DEPLOYMENT_ID).contentType(POST_JSON_CONTENT_TYPE).expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().post(REDEPLOY_DEPLOYMENT_URL);
	  }

	  private void verifyDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyDeploymentValues(mockDeployment, content);
	  }

	  private void verifyCreatedDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyDeploymentWithDefinitionsValues(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyCreatedBpmnDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyBpmnDeploymentValues(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyCreatedCmmnDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyCmmnDeploymentValues(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyCreatedDmnDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyDmnDeploymentValues(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyCreatedDrdDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyDrdDeploymentValues(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyCreatedEmptyDeployment(Deployment mockDeployment, Response response)
	  {
		string content = response.asString();
		verifyDeploymentValuesEmptyDefinitions(mockDeployment, content);
		verifyDeploymentLink(mockDeployment, content);
	  }

	  private void verifyDeploymentValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);
	  }

	  private void verifyDeploymentWithDefinitionsValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		IDictionary<string, Dictionary<string, object>> deployedProcessDefinitions = path.getMap(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS);
		IDictionary<string, Dictionary<string, object>> deployedCaseDefinitions = path.getMap(PROPERTY_DEPLOYED_CASE_DEFINITIONS);
		IDictionary<string, Dictionary<string, object>> deployedDecisionDefinitions = path.getMap(PROPERTY_DEPLOYED_DECISION_DEFINITIONS);
		IDictionary<string, Dictionary<string, object>> deployedDecisionRequirementsDefinitions = path.getMap(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS);

		assertEquals(1, deployedProcessDefinitions.Count);
		assertNotNull(deployedProcessDefinitions[EXAMPLE_PROCESS_DEFINITION_ID]);
		assertEquals(1, deployedCaseDefinitions.Count);
		assertNotNull(deployedCaseDefinitions[EXAMPLE_CASE_DEFINITION_ID]);
		assertEquals(1, deployedDecisionDefinitions.Count);
		assertNotNull(deployedDecisionDefinitions[EXAMPLE_DECISION_DEFINITION_ID]);
		assertEquals(1, deployedDecisionRequirementsDefinitions.Count);
		assertNotNull(deployedDecisionRequirementsDefinitions[EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID]);
	  }

	  private void verifyBpmnDeploymentValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		IDictionary<string, Dictionary<string, object>> deployedProcessDefinitionDtos = path.getMap(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS);

		assertEquals(1, deployedProcessDefinitionDtos.Count);
		Hashtable processDefinitionDto = deployedProcessDefinitionDtos[EXAMPLE_PROCESS_DEFINITION_ID];
		assertNotNull(processDefinitionDto);
		verifyBpmnDeployment(processDefinitionDto);

		assertNull(path.get(PROPERTY_DEPLOYED_CASE_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS));
	  }

	  private void verifyCmmnDeploymentValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		IDictionary<string, Dictionary<string, object>> deployedCaseDefinitions = path.getMap(PROPERTY_DEPLOYED_CASE_DEFINITIONS);

		assertEquals(1, deployedCaseDefinitions.Count);
		Hashtable caseDefinitionDto = deployedCaseDefinitions[EXAMPLE_CASE_DEFINITION_ID];
		assertNotNull(caseDefinitionDto);
		verifyCmnDeployment(caseDefinitionDto);

		assertNull(path.get(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS));
	  }

	  private void verifyDmnDeploymentValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		IDictionary<string, Dictionary<string, object>> deployedDecisionDefinitions = path.getMap(PROPERTY_DEPLOYED_DECISION_DEFINITIONS);

		assertEquals(1, deployedDecisionDefinitions.Count);
		Hashtable decisionDefinitionDto = deployedDecisionDefinitions[EXAMPLE_DECISION_DEFINITION_ID];
		assertNotNull(decisionDefinitionDto);
		verifyDmnDeployment(decisionDefinitionDto);

		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_CASE_DEFINITIONS));
	  }

	  private void verifyDrdDeploymentValues(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		IDictionary<string, Dictionary<string, object>> deployedDecisionDefinitions = path.getMap(PROPERTY_DEPLOYED_DECISION_DEFINITIONS);
		IDictionary<string, Dictionary<string, object>> deployedDecisionRequirementsDefinitions = path.getMap(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS);

		assertEquals(1, deployedDecisionDefinitions.Count);
		Hashtable decisionDefinitionDto = deployedDecisionDefinitions[EXAMPLE_DECISION_DEFINITION_ID];
		assertNotNull(decisionDefinitionDto);
		verifyDmnDeployment(decisionDefinitionDto);

		assertEquals(1, deployedDecisionRequirementsDefinitions.Count);
		Hashtable decisionRequirementsDefinitionDto = deployedDecisionRequirementsDefinitions[EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID];
		assertNotNull(decisionRequirementsDefinitionDto);
		verifyDrdDeployment(decisionRequirementsDefinitionDto);

		assertNull(path.get(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_CASE_DEFINITIONS));
	  }

	  private void verifyBpmnDeployment(Dictionary<string, object> dto)
	  {
		assertEquals(dto["id"], EXAMPLE_PROCESS_DEFINITION_ID);
		assertEquals(dto["category"], EXAMPLE_PROCESS_DEFINITION_CATEGORY);
		assertEquals(dto["name"], EXAMPLE_PROCESS_DEFINITION_NAME);
		assertEquals(dto["key"], EXAMPLE_PROCESS_DEFINITION_KEY);
		assertEquals(dto["description"], EXAMPLE_PROCESS_DEFINITION_DESCRIPTION);
		assertEquals(dto["version"], EXAMPLE_PROCESS_DEFINITION_VERSION);
		assertEquals(dto["resource"], EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME);
		assertEquals(dto["deploymentId"], EXAMPLE_DEPLOYMENT_ID);
		assertEquals(dto["diagram"], EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME);
		assertEquals(dto["suspended"], EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED);
	  }
	  private void verifyCmnDeployment(Dictionary<string, object> dto)
	  {
		assertEquals(dto["id"], EXAMPLE_CASE_DEFINITION_ID);
		assertEquals(dto["category"], EXAMPLE_CASE_DEFINITION_CATEGORY);
		assertEquals(dto["name"], EXAMPLE_CASE_DEFINITION_NAME);
		assertEquals(dto["key"], EXAMPLE_CASE_DEFINITION_KEY);
		assertEquals(dto["version"], EXAMPLE_CASE_DEFINITION_VERSION);
		assertEquals(dto["resource"], EXAMPLE_CASE_DEFINITION_RESOURCE_NAME);
		assertEquals(dto["deploymentId"], EXAMPLE_DEPLOYMENT_ID);
	  }

	  private void verifyDmnDeployment(Dictionary<string, object> dto)
	  {
		assertEquals(dto["id"], EXAMPLE_DECISION_DEFINITION_ID);
		assertEquals(dto["category"], EXAMPLE_DECISION_DEFINITION_CATEGORY);
		assertEquals(dto["name"], EXAMPLE_DECISION_DEFINITION_NAME);
		assertEquals(dto["key"], EXAMPLE_DECISION_DEFINITION_KEY);
		assertEquals(dto["version"], EXAMPLE_DECISION_DEFINITION_VERSION);
		assertEquals(dto["resource"], EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME);
		assertEquals(dto["deploymentId"], EXAMPLE_DEPLOYMENT_ID);
		assertEquals(dto["decisionRequirementsDefinitionId"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		assertEquals(dto["decisionRequirementsDefinitionKey"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY);
	  }

	  private void verifyDrdDeployment(Dictionary<string, object> dto)
	  {
		assertEquals(dto["id"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		assertEquals(dto["category"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY);
		assertEquals(dto["name"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME);
		assertEquals(dto["key"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY);
		assertEquals(dto["version"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION);
		assertEquals(dto["resource"], EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME);
		assertEquals(dto["deploymentId"], EXAMPLE_DEPLOYMENT_ID);
	  }

	  private void verifyDeploymentValuesEmptyDefinitions(Deployment mockDeployment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		verifyStandardDeploymentValues(mockDeployment, path);

		assertNull(path.get(PROPERTY_DEPLOYED_PROCESS_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_CASE_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_DEFINITIONS));
		assertNull(path.get(PROPERTY_DEPLOYED_DECISION_REQUIREMENTS_DEFINITIONS));
	  }

	  private void verifyStandardDeploymentValues(Deployment mockDeployment, JsonPath path)
	  {
		string returnedId = path.get("id");
		string returnedName = path.get("name");
		DateTime returnedDeploymentTime = DateTimeUtil.parseDate(path.get<string>("deploymentTime"));

		assertEquals(mockDeployment.Id, returnedId);
		assertEquals(mockDeployment.Name, returnedName);
		assertEquals(mockDeployment.DeploymentTime, returnedDeploymentTime);
	  }

	  private void verifyDeploymentLink(Deployment mockDeployment, string responseContent)
	  {
		IList<IDictionary<string, string>> returnedLinks = from(responseContent).getList("links");
		assertEquals(1, returnedLinks.Count);

		IDictionary<string, string> returnedLink = returnedLinks[0];
		assertEquals(HttpMethod.GET, returnedLink["method"]);
		assertTrue(returnedLink["href"].EndsWith(RESOURCE_URL + "/" + mockDeployment.Id, StringComparison.Ordinal));
		assertEquals("self", returnedLink["rel"]);
	  }

	  private void verifyDeploymentResource(Resource mockDeploymentResource, Response response)
	  {
		string content = response.asString();

		JsonPath path = from(content);
		string returnedId = path.get("id");
		string returnedName = path.get("name");
		string returnedDeploymentId = path.get("deploymentId");

		assertEquals(mockDeploymentResource.Id, returnedId);
		assertEquals(mockDeploymentResource.Name, returnedName);
		assertEquals(mockDeploymentResource.DeploymentId, returnedDeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void verifyDeploymentResources(List<Resource> mockDeploymentResources, io.restassured.response.Response response)
	  private void verifyDeploymentResources(IList<Resource> mockDeploymentResources, Response response)
	  {
		System.Collections.IList list = response.@as(typeof(System.Collections.IList));
		assertEquals(1, list.Count);

		LinkedHashMap<string, string> resourceHashMap = (LinkedHashMap<string, string>) list[0];

		string returnedId = resourceHashMap.get("id");
		string returnedName = resourceHashMap.get("name");
		string returnedDeploymentId = resourceHashMap.get("deploymentId");

		Resource mockDeploymentResource = mockDeploymentResources[0];

		assertEquals(mockDeploymentResource.Id, returnedId);
		assertEquals(mockDeploymentResource.Name, returnedName);
		assertEquals(mockDeploymentResource.DeploymentId, returnedDeploymentId);
	  }

	}

}