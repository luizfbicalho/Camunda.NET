using System;
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
	using Response = io.restassured.response.Response;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using org.camunda.bpm.engine;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DeleteProcessDefinitionsBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsBuilder;
	using DeleteProcessDefinitionsSelectBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsSelectBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using org.camunda.bpm.engine.rest.helper;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using ProcessDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.ProcessDefinitionResourceImpl;
	using EncodingUtil = org.camunda.bpm.engine.rest.util.EncodingUtil;
	using ModificationInstructionBuilder = org.camunda.bpm.engine.rest.util.ModificationInstructionBuilder;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Assertions = org.assertj.core.api.Assertions;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Matchers = org.mockito.Matchers;
	using InvocationOnMock = org.mockito.invocation.InvocationOnMock;
	using Answer = org.mockito.stubbing.Answer;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockSerializedVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
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
//	import static org.mockito.Matchers.isNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	public class ProcessDefinitionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_URL = PROCESS_DEFINITION_URL + "/{id}";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_URL = PROCESS_DEFINITION_URL + "/key/{key}";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_URL = PROCESS_DEFINITION_URL + "/key/{key}/tenant-id/{tenant-id}";

	  protected internal static readonly string START_PROCESS_INSTANCE_URL = SINGLE_PROCESS_DEFINITION_URL + "/start";
	  protected internal static readonly string START_PROCESS_INSTANCE_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/start";
	  protected internal static readonly string START_PROCESS_INSTANCE_BY_KEY_AND_TENANT_ID_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_URL + "/start";

	  protected internal static readonly string XML_DEFINITION_URL = SINGLE_PROCESS_DEFINITION_URL + "/xml";
	  protected internal static readonly string XML_DEFINITION_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/xml";
	  protected internal static readonly string DIAGRAM_DEFINITION_URL = SINGLE_PROCESS_DEFINITION_URL + "/diagram";
	  protected internal static readonly string DIAGRAM_DEFINITION_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/diagram";

	  protected internal static readonly string START_FORM_URL = SINGLE_PROCESS_DEFINITION_URL + "/startForm";
	  protected internal static readonly string START_FORM_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/startForm";
	  protected internal static readonly string DEPLOYED_START_FORM_URL = SINGLE_PROCESS_DEFINITION_URL + "/deployed-start-form";
	  protected internal static readonly string DEPLOYED_START_FORM_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/deployed-start-form";
	  protected internal static readonly string RENDERED_FORM_URL = SINGLE_PROCESS_DEFINITION_URL + "/rendered-form";
	  protected internal static readonly string RENDERED_FORM_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/rendered-form";
	  protected internal static readonly string SUBMIT_FORM_URL = SINGLE_PROCESS_DEFINITION_URL + "/submit-form";
	  protected internal static readonly string SUBMIT_FORM_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/submit-form";
	  protected internal static readonly string START_FORM_VARIABLES_URL = SINGLE_PROCESS_DEFINITION_URL + "/form-variables";
	  protected internal static readonly string START_FORM_VARIABLES_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/form-variables";

	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_SUSPENDED_URL = SINGLE_PROCESS_DEFINITION_URL + "/suspended";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/suspended";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_HISTORY_TIMETOLIVE_URL = SINGLE_PROCESS_DEFINITION_URL + "/history-time-to-live";
	  protected internal static readonly string PROCESS_DEFINITION_SUSPENDED_URL = PROCESS_DEFINITION_URL + "/suspended";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/delete";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_URL + "/delete";

	  private RuntimeService runtimeServiceMock;
	  private RepositoryService repositoryServiceMock;
	  private FormService formServiceMock;
	  private ManagementService managementServiceMock;
	  private ProcessDefinitionQuery processDefinitionQueryMock;
	  private ProcessInstanceWithVariables mockInstance;
	  private ProcessInstantiationBuilder mockInstantiationBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		UpRuntimeDataForDefinition = mockDefinition;

		managementServiceMock = mock(typeof(ManagementService));
		when(processEngine.ManagementService).thenReturn(managementServiceMock);
		when(managementServiceMock.getProcessApplicationForDeployment(MockProvider.EXAMPLE_DEPLOYMENT_ID)).thenReturn(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME);

		// replace the runtime container delegate & process application service with a mock

		ProcessApplicationService processApplicationService = mock(typeof(ProcessApplicationService));
		ProcessApplicationInfo appMock = MockProvider.createMockProcessApplicationInfo();
		when(processApplicationService.getProcessApplicationInfo(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME)).thenReturn(appMock);

		RuntimeContainerDelegate @delegate = mock(typeof(RuntimeContainerDelegate));
		when(@delegate.ProcessApplicationService).thenReturn(processApplicationService);
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.set(@delegate);
	  }

	  private ProcessDefinition UpRuntimeDataForDefinition
	  {
		  set
		  {
			mockInstance = MockProvider.createMockInstanceWithVariables();
    
			// we replace this mock with every test in order to have a clean one (in terms of invocations) for verification
			runtimeServiceMock = mock(typeof(RuntimeService));
			when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);
			when(runtimeServiceMock.startProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), Matchers.any<IDictionary<string, object>>())).thenReturn(mockInstance);
			when(runtimeServiceMock.startProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), anyString(), anyString(), Matchers.any<IDictionary<string, object>>())).thenReturn(mockInstance);
    
    
			mockInstantiationBuilder = setUpMockInstantiationBuilder();
			when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenReturn(mockInstance);
			when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);
    
    
			repositoryServiceMock = mock(typeof(RepositoryService));
			when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
			when(repositoryServiceMock.getProcessDefinition(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(value);
			when(repositoryServiceMock.getProcessModel(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(createMockProcessDefinionBpmn20Xml());
    
			DeleteProcessDefinitionsSelectBuilder deleteProcessDefinitionsSelectBuilder = mock(typeof(DeleteProcessDefinitionsSelectBuilder), RETURNS_DEEP_STUBS);
			when(repositoryServiceMock.deleteProcessDefinitions()).thenReturn(deleteProcessDefinitionsSelectBuilder);
    
			UpMockDefinitionQuery = value;
    
			StartFormData formDataMock = MockProvider.createMockStartFormData(value);
			formServiceMock = mock(typeof(FormService));
			when(processEngine.FormService).thenReturn(formServiceMock);
			when(formServiceMock.getStartFormData(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(formDataMock);
			when(formServiceMock.submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), Matchers.any<IDictionary<string, object>>())).thenReturn(mockInstance);
			when(formServiceMock.submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), anyString(), Matchers.any<IDictionary<string, object>>())).thenReturn(mockInstance);
    
			VariableMap startFormVariablesMock = MockProvider.createMockFormVariables();
			when(formServiceMock.getStartFormVariables(eq(EXAMPLE_PROCESS_DEFINITION_ID), Matchers.any<ICollection<string>>(), anyBoolean())).thenReturn(startFormVariablesMock);
		  }
	  }

	  private Stream createMockProcessDefinionBpmn20Xml()
	  {
		// do not close the input stream, will be done in implementation
		Stream bpmn20XmlIn = null;
		bpmn20XmlIn = ReflectUtil.getResourceAsStream("processes/fox-invoice_en_long_id.bpmn");
		Assert.assertNotNull(bpmn20XmlIn);
		return bpmn20XmlIn;
	  }

	  private ProcessDefinition UpMockDefinitionQuery
	  {
		  set
		  {
			processDefinitionQueryMock = mock(typeof(ProcessDefinitionQuery));
			when(processDefinitionQueryMock.processDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).thenReturn(processDefinitionQueryMock);
			when(processDefinitionQueryMock.processDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(processDefinitionQueryMock);
			when(processDefinitionQueryMock.tenantIdIn(anyString())).thenReturn(processDefinitionQueryMock);
			when(processDefinitionQueryMock.withoutTenantId()).thenReturn(processDefinitionQueryMock);
			when(processDefinitionQueryMock.latestVersion()).thenReturn(processDefinitionQueryMock);
			when(processDefinitionQueryMock.singleResult()).thenReturn(value);
			when(processDefinitionQueryMock.count()).thenReturn(1L);
			when(processDefinitionQueryMock.list()).thenReturn(Collections.singletonList(value));
			when(repositoryServiceMock.createProcessDefinitionQuery()).thenReturn(processDefinitionQueryMock);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceResourceLinkResult()
	  public virtual void testInstanceResourceLinkResult()
	  {
		string fullInstanceUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/process-instance/" + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullInstanceUrl)).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceResourceLinkWithEnginePrefix()
	  public virtual void testInstanceResourceLinkWithEnginePrefix()
	  {
		string startInstanceOnExplicitEngineUrl = TEST_RESOURCE_ROOT_PATH + "/engine/default/process-definition/{id}/start";

		string fullInstanceUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/engine/default/process-instance/" + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullInstanceUrl)).when().post(startInstanceOnExplicitEngineUrl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionBpmn20XmlRetrieval()
	  public virtual void testProcessDefinitionBpmn20XmlRetrieval()
	  {
		// Rest-assured has problems with extracting json with escaped quotation marks, i.e. the xml content in our case
		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDiagramRetrieval() throws FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessDiagramRetrieval()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("image/png").header("Content-Disposition", "attachment; filename=" + MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(repositoryServiceMock).getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "process diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDiagramNullFilename() throws FileNotFoundException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessDiagramNullFilename()
	  {
		// setup additional mock behavior
		File file = getFile("/processes/todo-process.png");
		when(repositoryServiceMock.getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).DiagramResourceName).thenReturn(null);
		when(repositoryServiceMock.getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(new FileStream(file, FileMode.Open, FileAccess.Read));

		// call method
		sbyte[] actual = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).contentType("application/octet-stream").header("Content-Disposition", "attachment; filename=" + null).when().get(DIAGRAM_DEFINITION_URL).Body.asByteArray();

		// verify service interaction
		verify(repositoryServiceMock).getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);

		// compare input stream with response body bytes
		sbyte[] expected = IoUtil.readInputStream(new FileStream(file, FileMode.Open, FileAccess.Read), "process diagram");
		Assert.assertArrayEquals(expected, actual);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDiagramNotExist()
	  public virtual void testProcessDiagramNotExist()
	  {
		// setup additional mock behavior
		when(repositoryServiceMock.getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(null);

		// call method
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().get(DIAGRAM_DEFINITION_URL);

		// verify service interaction
		verify(repositoryServiceMock).getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(repositoryServiceMock).getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
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
//ORIGINAL LINE: @Test public void testGetProcessDiagramGetDefinitionThrowsAuthorizationException()
	  public virtual void testGetProcessDiagramGetDefinitionThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(DIAGRAM_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetProcessDiagramThrowsAuthorizationException()
	  public virtual void testGetProcessDiagramThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(DIAGRAM_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetProcessDiagramGetDefinitionThrowsAuthorizationException_ByKey()
	  public virtual void testGetProcessDiagramGetDefinitionThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(DIAGRAM_DEFINITION_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetProcessDiagramThrowsAuthorizationException_ByKey()
	  public virtual void testGetProcessDiagramThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessDiagram(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(DIAGRAM_DEFINITION_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormData()
	  public virtual void testGetStartFormData()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo(MockProvider.EXAMPLE_FORM_KEY)).when().get(START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartForm_shouldReturnKeyContainingTaskId()
	  public virtual void testGetStartForm_shouldReturnKeyContainingTaskId()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		StartFormData mockStartFormData = MockProvider.createMockStartFormDataUsingFormFieldsWithoutFormKey(mockDefinition);
		when(formServiceMock.getStartFormData(mockDefinition.Id)).thenReturn(mockStartFormData);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo("embedded:engine://engine/:engine/process-definition/" + mockDefinition.Id + "/rendered-form")).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartForm_StartFormDataEqualsNull()
	  public virtual void testGetStartForm_StartFormDataEqualsNull()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		when(formServiceMock.getStartFormData(mockDefinition.Id)).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormThrowsAuthorizationException()
	  public virtual void testGetStartFormThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(formServiceMock.getStartFormData(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartForm()
	  public virtual void testGetRenderedStartForm()
	  {
		string expectedResult = "<formField>anyContent</formField>";

		when(formServiceMock.getRenderedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(expectedResult);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(XHTML_XML_CONTENT_TYPE).when().get(RENDERED_FORM_URL);

		string responseContent = response.asString();
		Assertions.assertThat(responseContent).isEqualTo(expectedResult);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormForDifferentPlatformEncoding() throws NoSuchFieldException, IllegalAccessException, UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testGetRenderedStartFormForDifferentPlatformEncoding()
	  {
		string expectedResult = "<formField>unicode symbol: \u2200</formField>";
		when(formServiceMock.getRenderedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(expectedResult);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(XHTML_XML_CONTENT_TYPE).when().get(RENDERED_FORM_URL);

		string responseContent = new string(response.asByteArray(), EncodingUtil.DEFAULT_ENCODING);
		Assertions.assertThat(responseContent).isEqualTo(expectedResult);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormReturnsNotFound()
	  public virtual void testGetRenderedStartFormReturnsNotFound()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching rendered start form for process definition with the id " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + " found.")).when().get(RENDERED_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormThrowsAuthorizationException()
	  public virtual void testGetRenderedStartFormThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(formServiceMock.getRenderedStartForm(anyString())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(RENDERED_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartForm()
	  public virtual void testSubmitStartForm()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithParameters()
	  public virtual void testSubmitStartFormWithParameters()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithSerializedVariableValue()
	  public virtual void testSubmitStartFormWithSerializedVariableValue()
	  {

		string jsonValue = "{}";

		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("aSerializedVariable", ValueType.OBJECT.Name, jsonValue, "aFormat", "aRootType").Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), argThat(new EqualsVariableMap()
				  .matcher("aVariable", EqualsUntypedValue.matcher().value("aStringValue")).matcher("aSerializedVariable", EqualsObjectValue.objectValueMatcher().serializedValue(jsonValue).serializationFormat("aFormat").objectTypeName("aRootType"))));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithBase64EncodedBytes()
	  public virtual void testSubmitStartFormWithBase64EncodedBytes()
	  {

		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", Base64.encodeBase64String("someBytes".GetBytes()), ValueType.BYTES.Name).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), argThat(new EqualsVariableMap()
				  .matcher("aVariable", EqualsPrimitiveValue.bytesValue("someBytes".GetBytes()))));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithBusinessKey()
	  public virtual void testSubmitStartFormWithBusinessKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, "myBusinessKey", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithBusinessKeyAndParameters()
	  public virtual void testSubmitStartFormWithBusinessKeyAndParameters()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		json["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq("myBusinessKey"), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableIntegerVariable()
	  public virtual void testSubmitStartFormWithUnparseableIntegerVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableShortVariable()
	  public virtual void testSubmitStartFormWithUnparseableShortVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableLongVariable()
	  public virtual void testSubmitStartFormWithUnparseableLongVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableDoubleVariable()
	  public virtual void testSubmitStartFormWithUnparseableDoubleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableDateVariable()
	  public virtual void testSubmitStartFormWithUnparseableDateVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithNotSupportedVariableType()
	  public virtual void testSubmitStartFormWithNotSupportedVariableType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: Unsupported value type 'X'")).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulSubmitStartForm()
	  public virtual void testUnsuccessfulSubmitStartForm()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot instantiate process definition " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + ": expected exception")).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormByIdThrowsAuthorizationException()
	  public virtual void testSubmitFormByIdThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormByIdThrowsFormFieldValidationException()
	  public virtual void testSubmitFormByIdThrowsFormFieldValidationException()
	  {
		string message = "expected exception";
		doThrow(new FormFieldValidationException("form-exception", message)).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot instantiate process definition " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + ": " + message)).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariables()
	  public virtual void testGetStartFormVariables()
	  {

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Type))).when().get(START_FORM_VARIABLES_URL).body();

		verify(formServiceMock, times(1)).getStartFormVariables(EXAMPLE_PROCESS_DEFINITION_ID, null, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesVarNames()
	  public virtual void testGetStartFormVariablesVarNames()
	  {

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).queryParam("variableNames", "a,b,c").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(START_FORM_VARIABLES_URL);

		verify(formServiceMock, times(1)).getStartFormVariables(EXAMPLE_PROCESS_DEFINITION_ID, Arrays.asList(new string[]{"a", "b", "c"}), true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesAndDoNotDeserializeVariables()
	  public virtual void testGetStartFormVariablesAndDoNotDeserializeVariables()
	  {

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).queryParam("deserializeValues", false).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Type))).when().get(START_FORM_VARIABLES_URL).body();

		verify(formServiceMock, times(1)).getStartFormVariables(EXAMPLE_PROCESS_DEFINITION_ID, null, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesVarNamesAndDoNotDeserializeVariables()
	  public virtual void testGetStartFormVariablesVarNamesAndDoNotDeserializeVariables()
	  {

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).queryParam("deserializeValues", false).queryParam("variableNames", "a,b,c").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(START_FORM_VARIABLES_URL);

		verify(formServiceMock, times(1)).getStartFormVariables(EXAMPLE_PROCESS_DEFINITION_ID, Arrays.asList(new string[]{"a", "b", "c"}), false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesThrowsAuthorizationException()
	  public virtual void testGetStartFormVariablesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(formServiceMock.getStartFormVariables(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, null, true)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(START_FORM_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleProcessInstantiation()
	  public virtual void testSimpleProcessInstantiation()
	  {
	   given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleProcessInstantiationWithVariables()
	  public virtual void testSimpleProcessInstantiationWithVariables()
	  {
		//mock process instance
		ProcessInstanceWithVariables mockInstance = MockProvider.createMockInstanceWithVariables();
		ProcessInstantiationBuilder mockInstantiationBuilder = setUpMockInstantiationBuilder();
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenReturn(mockInstance);
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		//given request with parameter withVariables to get variables in return
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["withVariablesInReturn"] = true;

		//when request then return process instance with serialized variables
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(List<object>).FullName)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(object).FullName)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithParameters() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithParameters()
	  {
		IDictionary<string, object> parameters = VariablesBuilder.create().variable("aBoolean", true).variable("aString", "aStringVariableValue").variable("anInteger", 42).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = parameters;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		IDictionary<string, object> expectedParameters = new Dictionary<string, object>();
		expectedParameters["aBoolean"] = true;
		expectedParameters["aString"] = "aStringVariableValue";
		expectedParameters["anInteger"] = 42;

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).Variables = argThat(new EqualsMap(expectedParameters));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithBusinessKey() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithBusinessKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithBusinessKeyAndParameters() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithBusinessKeyAndParameters()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		IDictionary<string, object> parameters = VariablesBuilder.create().variable("aBoolean", true).variable("aString", "aStringVariableValue").variable("anInteger", 42).Variables;

		json["variables"] = parameters;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		IDictionary<string, object> expectedParameters = new Dictionary<string, object>();
		expectedParameters["aBoolean"] = true;
		expectedParameters["aString"] = "aStringVariableValue";
		expectedParameters["anInteger"] = 42;

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).Variables = argThat(new EqualsMap(expectedParameters));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithTransientVariables() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithTransientVariables()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["variables"] = VariablesBuilder.create().variableTransient("foo", "bar", "string").Variables;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.variable.VariableMap varMap = new org.camunda.bpm.engine.variable.impl.VariableMapImpl();
		VariableMap varMap = new VariableMapImpl();

		when(mockInstantiationBuilder.setVariables(anyMapOf(typeof(string), typeof(object)))).thenAnswer(new AnswerAnonymousInnerClass(this, varMap));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		VariableMap expectedVariables = Variables.createVariables().putValueTyped("foo", Variables.stringValue("bar", true));
		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).Variables = expectedVariables;
		assertEquals(expectedVariables.getValueTyped("foo").Transient, varMap.getValueTyped("foo").Transient);
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

	  private class AnswerAnonymousInnerClass : Answer<ProcessInstantiationBuilder>
	  {
		  private readonly ProcessDefinitionRestServiceInteractionTest outerInstance;

		  private VariableMap varMap;

		  public AnswerAnonymousInnerClass(ProcessDefinitionRestServiceInteractionTest outerInstance, VariableMap varMap)
		  {
			  this.outerInstance = outerInstance;
			  this.varMap = varMap;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder answer(org.mockito.invocation.InvocationOnMock invocation) throws Throwable
		  public override ProcessInstantiationBuilder answer(InvocationOnMock invocation)
		  {
			varMap.putAll((VariableMap) invocation.Arguments[0]);
			return outerInstance.mockInstantiationBuilder;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationAtActivitiesById()
	  public virtual void testProcessInstantiationAtActivitiesById()
	  {

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("processVariable", "aString", "String").Variables;
		json["businessKey"] = "aBusinessKey";
		json["caseInstanceId"] = "aCaseInstanceId";

		IList<IDictionary<string, object>> startInstructions = new List<IDictionary<string, object>>();

		startInstructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").variables(VariablesBuilder.create().variable("var", "value", "String", false).variable("varLocal", "valueLocal", "String", true).Variables).Json);
		startInstructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").variables(VariablesBuilder.create().variable("var", 52, "Integer", false).variable("varLocal", 74, "Integer", true).Variables).Json);
		startInstructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").variables(VariablesBuilder.create().variable("var", 53, "Integer", false).variable("varLocal", 75, "Integer", true).Variables).Json);


		json["startInstructions"] = startInstructions;

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(EXAMPLE_PROCESS_DEFINITION_ID));

		InOrder inOrder = inOrder(mockInstantiationBuilder);

		inOrder.verify(mockInstantiationBuilder).businessKey("aBusinessKey");
		inOrder.verify(mockInstantiationBuilder).caseInstanceId("aCaseInstanceId");
		inOrder.verify(mockInstantiationBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("processVariable", EqualsPrimitiveValue.stringValue("aString")));

		inOrder.verify(mockInstantiationBuilder).startBeforeActivity("activityId");

		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.stringValue("valueLocal")));
		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.stringValue("value")));

		inOrder.verify(mockInstantiationBuilder).startAfterActivity("activityId");

		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.integerValue(52)));
		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.integerValue(74)));

		inOrder.verify(mockInstantiationBuilder).startTransition("transitionId");

		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.integerValue(53)));
		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.integerValue(75)));

		inOrder.verify(mockInstantiationBuilder).executeWithVariablesInReturn(false, false);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationAtActivitiesByIdWithVariablesInReturn()
	  public virtual void testProcessInstantiationAtActivitiesByIdWithVariablesInReturn()
	  {
		//set up variables and parameters
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("processVariable", "aString", "String").Variables;
		json["businessKey"] = "aBusinessKey";
		json["caseInstanceId"] = "aCaseInstanceId";

		VariableMap variables = createMockSerializedVariables().putValueTyped("processVariable", Variables.stringValue("aString")).putValueTyped("var", Variables.stringValue("value")).putValueTyped("varLocal", Variables.stringValue("valueLocal"));

		//mock process instance and instantiation builder
		ProcessInstanceWithVariables mockInstance = MockProvider.createMockInstanceWithVariables();
		when(mockInstance.Variables).thenReturn(variables);

		ProcessInstantiationBuilder mockInstantiationBuilder = setUpMockInstantiationBuilder();
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenReturn(mockInstance);
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		//create instructions
		IList<IDictionary<string, object>> startInstructions = new List<IDictionary<string, object>>();

		startInstructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").variables(VariablesBuilder.create().variable("var", "value", "String", false).variable("varLocal", "valueLocal", "String", true).Variables).Json);

		json["startInstructions"] = startInstructions;
		json["withVariablesInReturn"] = true;

		//request which response should contain serialized variables of process instance
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(List<object>).FullName)).body("variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(object).FullName)).body("variables." + MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).body("variables.processVariable.type", equalTo("String")).body("variables.processVariable.value", equalTo("aString")).body("variables.var.type", equalTo("String")).body("variables.var.value", equalTo("value")).body("variables.varLocal.type", equalTo("String")).body("variables.varLocal.value", equalTo("valueLocal")).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(EXAMPLE_PROCESS_DEFINITION_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationAtActivitiesByKey()
	  public virtual void testProcessInstantiationAtActivitiesByKey()
	  {
		ProcessInstantiationBuilder mockInstantiationBuilder = setUpMockInstantiationBuilder();
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = VariablesBuilder.create().variable("processVariable", "aString", "String").Variables;
		json["businessKey"] = "aBusinessKey";
		json["caseInstanceId"] = "aCaseInstanceId";

		IList<IDictionary<string, object>> startInstructions = new List<IDictionary<string, object>>();

		startInstructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").variables(VariablesBuilder.create().variable("var", "value", "String", false).variable("varLocal", "valueLocal", "String", true).Variables).Json);
		startInstructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").variables(VariablesBuilder.create().variable("var", 52, "Integer", false).variable("varLocal", 74, "Integer", true).Variables).Json);
		startInstructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").variables(VariablesBuilder.create().variable("var", 53, "Integer", false).variable("varLocal", 75, "Integer", true).Variables).Json);


		json["startInstructions"] = startInstructions;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));

		InOrder inOrder = inOrder(mockInstantiationBuilder);

		inOrder.verify(mockInstantiationBuilder).businessKey("aBusinessKey");
		inOrder.verify(mockInstantiationBuilder).caseInstanceId("aCaseInstanceId");
		inOrder.verify(mockInstantiationBuilder).Variables = argThat(EqualsVariableMap.matches().matcher("processVariable", EqualsPrimitiveValue.stringValue("aString")));

		inOrder.verify(mockInstantiationBuilder).startBeforeActivity("activityId");

		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.stringValue("valueLocal")));
		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.stringValue("value")));

		inOrder.verify(mockInstantiationBuilder).startAfterActivity("activityId");

		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.integerValue(52)));
		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.integerValue(74)));

		inOrder.verify(mockInstantiationBuilder).startTransition("transitionId");

		verify(mockInstantiationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.integerValue(53)));
		verify(mockInstantiationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.integerValue(75)));

		inOrder.verify(mockInstantiationBuilder).executeWithVariablesInReturn(false, false);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationAtActivitiesSkipIoMappingsAndListeners()
	  public virtual void testProcessInstantiationAtActivitiesSkipIoMappingsAndListeners()
	  {
		ProcessInstantiationBuilder mockInstantiationBuilder = setUpMockInstantiationBuilder();
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> startInstructions = new List<IDictionary<string, object>>();

		startInstructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);

		json["startInstructions"] = startInstructions;
		json["skipIoMappings"] = true;
		json["skipCustomListeners"] = true;

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(EXAMPLE_PROCESS_DEFINITION_ID));

		InOrder inOrder = inOrder(mockInstantiationBuilder);

		inOrder.verify(mockInstantiationBuilder).startBeforeActivity("activityId");
		inOrder.verify(mockInstantiationBuilder).executeWithVariablesInReturn(true, true);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidInstantiationAtActivities()
	  public virtual void testInvalidInstantiationAtActivities()
	  {
		ProcessInstantiationBuilder mockInstantiationBuilder = setUpMockInstantiationBuilder();
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		IDictionary<string, object> json = new Dictionary<string, object>();

		// start before: missing activity id
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["startInstructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(START_PROCESS_INSTANCE_URL);

		// start after: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["startInstructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(START_PROCESS_INSTANCE_URL);

		// start transition: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["startInstructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'transitionId' must be set")).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder setUpMockInstantiationBuilder()
	  protected internal virtual ProcessInstantiationBuilder setUpMockInstantiationBuilder()
	  {
		ProcessInstanceWithVariables resultInstanceWithVariables = MockProvider.createMockInstanceWithVariables();
		ProcessInstantiationBuilder mockInstantiationBuilder = mock(typeof(ProcessInstantiationBuilder));

		when(mockInstantiationBuilder.startAfterActivity(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.startBeforeActivity(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.startTransition(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.setVariables(any(typeof(System.Collections.IDictionary)))).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.setVariablesLocal(any(typeof(System.Collections.IDictionary)))).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.businessKey(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.caseInstanceId(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.execute(anyBoolean(), anyBoolean())).thenReturn(resultInstanceWithVariables);
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenReturn(resultInstanceWithVariables);

		return mockInstantiationBuilder;
	  }

	  /// <summary>
	  /// <seealso cref="RuntimeService.startProcessInstanceById(string, System.Collections.IDictionary)"/> throws an <seealso cref="ProcessEngineException"/>, if a definition with the given id does not exist.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulInstantiation()
	  public virtual void testUnsuccessfulInstantiation()
	  {
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", containsString("Cannot instantiate process definition")).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByIdThrowsAuthorizationException()
	  public virtual void testStartProcessInstanceByIdThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval()
	  public virtual void testDefinitionRetrieval()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME)).body("description", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME)).body("diagram", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED)).body("tenantId", nullValue()).when().get(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingProcessDefinitionRetrieval()
	  public virtual void testNonExistingProcessDefinitionRetrieval()
	  {
		string nonExistingId = "aNonExistingDefinitionId";
		when(repositoryServiceMock.getProcessDefinition(eq(nonExistingId))).thenThrow(new ProcessEngineException("no matching definition"));

		given().pathParam("id", "aNonExistingDefinitionId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching definition with id " + nonExistingId)).when().get(SINGLE_PROCESS_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingProcessDefinitionBpmn20XmlRetrieval()
	  public virtual void testNonExistingProcessDefinitionBpmn20XmlRetrieval()
	  {
		string nonExistingId = "aNonExistingDefinitionId";
		when(repositoryServiceMock.getProcessModel(eq(nonExistingId))).thenThrow(new ProcessEngineException("no matching process definition found."));

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching definition with id " + nonExistingId)).when().get(XML_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetProcessDefinitionBpmn20XmlThrowsAuthorizationException()
	  public virtual void testGetProcessDefinitionBpmn20XmlThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessModel(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(XML_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeployment()
	  public virtual void testDeleteDeployment()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascade()
	  public virtual void testDeleteDeploymentCascade()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("cascade", true).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascadeNonsense()
	  public virtual void testDeleteDeploymentCascadeNonsense()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("cascade", "bla").expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentCascadeFalse()
	  public virtual void testDeleteDeploymentCascadeFalse()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("cascade", false).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListeners()
	  public virtual void testDeleteDeploymentSkipCustomListeners()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("skipCustomListeners", true).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersNonsense()
	  public virtual void testDeleteDeploymentSkipCustomListenersNonsense()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("skipCustomListeners", "bla").expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersFalse()
	  public virtual void testDeleteDeploymentSkipCustomListenersFalse()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("skipCustomListeners", false).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentSkipCustomListenersAndCascade()
	  public virtual void testDeleteDeploymentSkipCustomListenersAndCascade()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("cascade", true).queryParam("skipCustomListeners", true).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingDeployment()
	  public virtual void testDeleteNonExistingDeployment()
	  {

		doThrow(new NotFoundException("No process definition found with id 'NON_EXISTING_ID'")).when(repositoryServiceMock).deleteProcessDefinition("NON_EXISTING_ID", false, false, false);

		given().pathParam("id", "NON_EXISTING_ID").expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("No process definition found with id 'NON_EXISTING_ID'")).when().delete(SINGLE_PROCESS_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeploymentThrowsAuthorizationException()
	  public virtual void testDeleteDeploymentThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, false);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().delete(SINGLE_PROCESS_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionSkipIoMappings()
	  public virtual void testDeleteDefinitionSkipIoMappings()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("skipIoMappings", true).expect().statusCode(Status.OK.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_URL);

		verify(repositoryServiceMock).deleteProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, false, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKey()
	  public virtual void testDeleteDefinitionsByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeyCascade()
	  public virtual void testDeleteDefinitionsByKeyCascade()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("cascade", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).cascade();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeySkipCustomListeners()
	  public virtual void testDeleteDefinitionsByKeySkipCustomListeners()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("skipCustomListeners", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).skipCustomListeners();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeySkipIoMappings()
	  public virtual void testDeleteDefinitionsByKeySkipIoMappings()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("skipIoMappings", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).skipIoMappings();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeySkipCustomListenersAndCascade()
	  public virtual void testDeleteDefinitionsByKeySkipCustomListenersAndCascade()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("cascade", true).queryParam("skipCustomListeners", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).skipCustomListeners().cascade();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeyNotExistingKey()
	  public virtual void testDeleteDefinitionsByKeyNotExistingKey()
	  {
		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey("NOT_EXISTING_KEY");

		doThrow(new NotFoundException("No process definition found with key 'NOT_EXISTING_KEY'")).when(builder).delete();

		given().pathParam("key", "NOT_EXISTING_KEY").expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("No process definition found with key 'NOT_EXISTING_KEY'")).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_DELETE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeyWithTenantId()
	  public virtual void testDeleteDefinitionsByKeyWithTenantId()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).withTenantId(MockProvider.EXAMPLE_TENANT_ID);

		verify(builder).delete();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeyCascadeWithTenantId()
	  public virtual void testDeleteDefinitionsByKeyCascadeWithTenantId()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).queryParam("cascade", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).withTenantId(MockProvider.EXAMPLE_TENANT_ID).cascade();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeySkipCustomListenersWithTenantId()
	  public virtual void testDeleteDefinitionsByKeySkipCustomListenersWithTenantId()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).queryParam("skipCustomListeners", true).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).withTenantId(MockProvider.EXAMPLE_TENANT_ID).skipCustomListeners();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeySkipCustomListenersAndCascadeWithTenantId()
	  public virtual void testDeleteDefinitionsByKeySkipCustomListenersAndCascadeWithTenantId()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("skipCustomListeners", true).queryParam("cascade", true).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL);

		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).withTenantId(MockProvider.EXAMPLE_TENANT_ID).skipCustomListeners().cascade();

		verify(builder).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDefinitionsByKeyNoPermissions()
	  public virtual void testDeleteDefinitionsByKeyNoPermissions()
	  {
		DeleteProcessDefinitionsBuilder builder = repositoryServiceMock.deleteProcessDefinitions().byKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).withTenantId(MockProvider.EXAMPLE_TENANT_ID);

		doThrow(new AuthorizationException("No permission to delete process definitions")).when(builder).delete();

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("No permission to delete process definitions")).when().delete(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_DELETE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormDataForNonExistingProcessDefinition()
	  public virtual void testGetStartFormDataForNonExistingProcessDefinition()
	  {
		when(formServiceMock.getStartFormData(anyString())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingProcessDefinitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot get start form data for process definition")).when().get(START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableIntegerVariable()
	  public virtual void testUnparseableIntegerVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableShortVariable()
	  public virtual void testUnparseableShortVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableLongVariable()
	  public virtual void testUnparseableLongVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableDoubleVariable()
	  public virtual void testUnparseableDoubleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableDateVariable()
	  public virtual void testUnparseableDateVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotSupportedTypeVariable()
	  public virtual void testNotSupportedTypeVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: Unsupported value type 'X'")).when().post(START_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionExcludingInstances()
	  public virtual void testActivateProcessDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionExcludingInstances()
	  public virtual void testDelayedActivateProcessDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionIncludingInstances()
	  public virtual void testActivateProcessDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionIncludingInstances()
	  public virtual void testDelayedActivateProcessDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsProcessEngineException()
	  public virtual void testActivateThrowsProcessEngineException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(repositoryServiceMock).activateProcessDefinitionById(eq(MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID), eq(false), isNull(typeof(DateTime)));

		given().pathParam("id", MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateNonParseableDateFormat()
	  public virtual void testActivateNonParseableDateFormat()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Process Definition with id " + MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("id", MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionThrowsAuthorizationException()
	  public virtual void testActivateProcessDefinitionThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionExcludingInstances()
	  public virtual void testSuspendProcessDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionExcludingInstances()
	  public virtual void testDelayedSuspendProcessDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionIncludingInstances()
	  public virtual void testSuspendProcessDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionIncludingInstances()
	  public virtual void testDelayedSuspendProcessDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsProcessEngineException()
	  public virtual void testSuspendThrowsProcessEngineException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(repositoryServiceMock).suspendProcessDefinitionById(eq(MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID), eq(false), isNull(typeof(DateTime)));

		given().pathParam("id", MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendNonParseableDateFormat()
	  public virtual void testSuspendNonParseableDateFormat()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Process Definition with id " + MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("id", MockProvider.NON_EXISTING_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendWithMultipleByParameters()
	  public virtual void testSuspendWithMultipleByParameters()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "Only one of processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionThrowsAuthorizationException()
	  public virtual void testSuspendProcessDefinitionThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByKey()
	  public virtual void testActivateProcessDefinitionByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByKeyIncludingInstaces()
	  public virtual void testActivateProcessDefinitionByKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, true, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionByKey()
	  public virtual void testDelayedActivateProcessDefinitionByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionByKeyIncludingInstaces()
	  public virtual void testDelayedActivateProcessDefinitionByKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByKeyWithUnparseableDate()
	  public virtual void testActivateProcessDefinitionByKeyWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Process Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByKeyWithException()
	  public virtual void testActivateProcessDefinitionByKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByKeyThrowsAuthorizationException()
	  public virtual void testActivateProcessDefinitionByKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).activateProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByKey()
	  public virtual void testSuspendProcessDefinitionByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByKeyIncludingInstaces()
	  public virtual void testSuspendProcessDefinitionByKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, true, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionByKey()
	  public virtual void testDelayedSuspendProcessDefinitionByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionByKeyIncludingInstaces()
	  public virtual void testDelayedSuspendProcessDefinitionByKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_DEFINITION_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByKeyWithUnparseableDate()
	  public virtual void testSuspendProcessDefinitionByKeyWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Process Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByKeyWithException()
	  public virtual void testSuspendProcessDefinitionByKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByKeyThrowsAuthorizationException()
	  public virtual void testSuspendProcessDefinitionByKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).suspendProcessDefinitionByKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, false, null);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByIdShouldThrowException()
	  public virtual void testActivateProcessDefinitionByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string message = "Only processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionByNothing()
	  public virtual void testActivateProcessDefinitionByNothing()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;

		string message = "Either processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByIdShouldThrowException()
	  public virtual void testSuspendProcessDefinitionByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string message = "Only processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionByNothing()
	  public virtual void testSuspendProcessDefinitionByNothing()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "Either processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionThrowsAuthorizationExcpetion()
	  public virtual void testSuspendProcessDefinitionThrowsAuthorizationExcpetion()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "Either processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_DEFINITION_SUSPENDED_URL);
	  }

	  /// 
	  /// <summary>
	  /// ********************************* test cases for operations of the latest process definition ********************************
	  /// get the latest process definition by key
	  /// 
	  /// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceResourceLinkResult_ByKey()
	  public virtual void testInstanceResourceLinkResult_ByKey()
	  {
		string fullInstanceUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/process-instance/" + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullInstanceUrl)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceResourceLinkWithEnginePrefix_ByKey()
	  public virtual void testInstanceResourceLinkWithEnginePrefix_ByKey()
	  {
		string startInstanceOnExplicitEngineUrl = TEST_RESOURCE_ROOT_PATH + "/engine/default/process-definition/key/{key}/start";

		string fullInstanceUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/engine/default/process-instance/" + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullInstanceUrl)).when().post(startInstanceOnExplicitEngineUrl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionBpmn20XmlRetrieval_ByKey()
	  public virtual void testProcessDefinitionBpmn20XmlRetrieval_ByKey()
	  {
		// Rest-assured has problems with extracting json with escaped quotation marks, i.e. the xml content in our case
		Response response = given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).when().get(XML_DEFINITION_BY_KEY_URL);

		string responseContent = response.asString();
		Assert.assertTrue(responseContent.Contains(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		Assert.assertTrue(responseContent.Contains("<?xml"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetProcessDefinitionBpmn20XmlThrowsAuthorizationException_ByKey()
	  public virtual void testGetProcessDefinitionBpmn20XmlThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(repositoryServiceMock.getProcessModel(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(XML_DEFINITION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormData_ByKey()
	  public virtual void testGetStartFormData_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo(MockProvider.EXAMPLE_FORM_KEY)).when().get(START_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormThrowsAuthorizationException_ByKey()
	  public virtual void testGetStartFormThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(formServiceMock.getStartFormData(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(START_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartForm_shouldReturnKeyContainingTaskId_ByKey()
	  public virtual void testGetStartForm_shouldReturnKeyContainingTaskId_ByKey()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		StartFormData mockStartFormData = MockProvider.createMockStartFormDataUsingFormFieldsWithoutFormKey(mockDefinition);
		when(formServiceMock.getStartFormData(mockDefinition.Id)).thenReturn(mockStartFormData);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo("embedded:engine://engine/:engine/process-definition/" + mockDefinition.Id + "/rendered-form")).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(START_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartForm_ByKey()
	  public virtual void testGetRenderedStartForm_ByKey()
	  {
		string expectedResult = "<formField>anyContent</formField>";

		when(formServiceMock.getRenderedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(expectedResult);

		Response response = given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(XHTML_XML_CONTENT_TYPE).when().get(RENDERED_FORM_BY_KEY_URL);

		string responseContent = response.asString();
		Assertions.assertThat(responseContent).isEqualTo(expectedResult);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormReturnsNotFound_ByKey()
	  public virtual void testGetRenderedStartFormReturnsNotFound_ByKey()
	  {
		when(formServiceMock.getRenderedStartForm(anyString(), anyString())).thenReturn(null);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching rendered start form for process definition with the id " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + " found.")).when().get(RENDERED_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedStartFormThrowsAuthorizationException_ByKey()
	  public virtual void testGetRenderedStartFormThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(formServiceMock.getRenderedStartForm(anyString())).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(RENDERED_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartForm_ByKey()
	  public virtual void testSubmitStartForm_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_BY_KEY_URL);

		verify(formServiceMock).submitStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithParameters_ByKey()
	  public virtual void testSubmitStartFormWithParameters_ByKey()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_BY_KEY_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithBusinessKey_ByKey()
	  public virtual void testSubmitStartFormWithBusinessKey_ByKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_BY_KEY_URL);

		verify(formServiceMock).submitStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, "myBusinessKey", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithBusinessKeyAndParameters_ByKey()
	  public virtual void testSubmitStartFormWithBusinessKeyAndParameters_ByKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		json["variables"] = variables;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).when().post(SUBMIT_FORM_BY_KEY_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(formServiceMock).submitStartForm(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq("myBusinessKey"), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableIntegerVariable_ByKey()
	  public virtual void testSubmitStartFormWithUnparseableIntegerVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableShortVariable_ByKey()
	  public virtual void testSubmitStartFormWithUnparseableShortVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableLongVariable_ByKey()
	  public virtual void testSubmitStartFormWithUnparseableLongVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableDoubleVariable_ByKey()
	  public virtual void testSubmitStartFormWithUnparseableDoubleVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithUnparseableDateVariable_ByKey()
	  public virtual void testSubmitStartFormWithUnparseableDateVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitStartFormWithNotSupportedVariableType_ByKey()
	  public virtual void testSubmitStartFormWithNotSupportedVariableType_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: Unsupported value type 'X'")).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulSubmitStartForm_ByKey()
	  public virtual void testUnsuccessfulSubmitStartForm_ByKey()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot instantiate process definition " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + ": expected exception")).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormByKeyThrowsAuthorizationException()
	  public virtual void testSubmitFormByKeyThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormByKeyThrowsFormFieldValidationException()
	  public virtual void testSubmitFormByKeyThrowsFormFieldValidationException()
	  {
		string message = "expected exception";
		doThrow(new FormFieldValidationException("form-exception", message)).when(formServiceMock).submitStartForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot instantiate process definition " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + ": " + message)).when().post(SUBMIT_FORM_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleProcessInstantiation_ByKey()
	  public virtual void testSimpleProcessInstantiation_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithParameters_ByKey() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithParameters_ByKey()
	  {
		IDictionary<string, object> parameters = VariablesBuilder.create().variable("aBoolean", true).variable("aString", "aStringVariableValue").variable("anInteger", 42).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = parameters;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);

		IDictionary<string, object> expectedParameters = new Dictionary<string, object>();
		expectedParameters["aBoolean"] = true;
		expectedParameters["aString"] = "aStringVariableValue";
		expectedParameters["anInteger"] = 42;

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).Variables = argThat(new EqualsMap(expectedParameters));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithBusinessKey_ByKey() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithBusinessKey_ByKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithBusinessKeyAndParameters_ByKey() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithBusinessKeyAndParameters_ByKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["businessKey"] = "myBusinessKey";

		IDictionary<string, object> parameters = VariablesBuilder.create().variable("aBoolean", true).variable("aString", "aStringVariableValue").variable("anInteger", 42).Variables;

		json["variables"] = parameters;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);

		IDictionary<string, object> expectedParameters = new Dictionary<string, object>();
		expectedParameters["aBoolean"] = true;
		expectedParameters["aString"] = "aStringVariableValue";
		expectedParameters["anInteger"] = 42;

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).Variables = argThat(new EqualsMap(expectedParameters));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

	  /// <summary>
	  /// <seealso cref="RuntimeService.startProcessInstanceById(string, System.Collections.IDictionary)"/> throws an <seealso cref="ProcessEngineException"/>, if a definition with the given id does not exist.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulInstantiation_ByKey()
	  public virtual void testUnsuccessfulInstantiation_ByKey()
	  {
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", containsString("Cannot instantiate process definition")).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceByKeyThrowsAuthorizationException()
	  public virtual void testStartProcessInstanceByKeyThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKey()
	  public virtual void testDefinitionRetrieval_ByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME)).body("description", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME)).body("diagram", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED)).body("tenantId", nullValue()).when().get(SINGLE_PROCESS_DEFINITION_BY_KEY_URL);

		verify(processDefinitionQueryMock).withoutTenantId();
		verify(repositoryServiceMock).getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingProcessDefinitionRetrieval_ByKey()
	  public virtual void testNonExistingProcessDefinitionRetrieval_ByKey()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";

		when(repositoryServiceMock.createProcessDefinitionQuery().processDefinitionKey(nonExistingKey)).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.latestVersion()).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.singleResult()).thenReturn(null);
		when(processDefinitionQueryMock.list()).thenReturn(System.Linq.Enumerable.Empty<ProcessDefinition>());
		when(processDefinitionQueryMock.count()).thenReturn(0L);

		given().pathParam("key", nonExistingKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching process definition with key: " + nonExistingKey + " and no tenant-id")).when().get(SINGLE_PROCESS_DEFINITION_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testDefinitionRetrieval_ByKeyAndTenantId()
	  {
		ProcessDefinition mockDefinition = MockProvider.mockDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeDataForDefinition = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("key", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("category", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY)).body("name", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME)).body("description", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION)).body("deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("version", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION)).body("resource", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME)).body("diagram", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_URL);

		verify(processDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(repositoryServiceMock).getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingProcessDefinitionRetrieval_ByKeyAndTenantId()
	  public virtual void testNonExistingProcessDefinitionRetrieval_ByKeyAndTenantId()
	  {
		string nonExistingKey = "aNonExistingDefinitionKey";
		string nonExistingTenantId = "aNonExistingTenantId";

		when(repositoryServiceMock.createProcessDefinitionQuery().processDefinitionKey(nonExistingKey)).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.singleResult()).thenReturn(null);

		given().pathParam("key", nonExistingKey).pathParam("tenant-id", nonExistingTenantId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", containsString("No matching process definition with key: " + nonExistingKey + " and tenant-id: " + nonExistingTenantId)).when().get(SINGLE_PROCESS_DEFINITION_BY_KEY_AND_TENANT_ID_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleProcessInstantiation_ByKeyAndTenantId()
	  public virtual void testSimpleProcessInstantiation_ByKeyAndTenantId()
	  {
		ProcessDefinition mockDefinition = MockProvider.mockDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build();
		UpRuntimeDataForDefinition = mockDefinition;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).pathParam("tenant-id", MockProvider.EXAMPLE_TENANT_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_BY_KEY_AND_TENANT_ID_URL);

		verify(processDefinitionQueryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableIntegerVariable_ByKey()
	  public virtual void testUnparseableIntegerVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableShortVariable_ByKey()
	  public virtual void testUnparseableShortVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableLongVariable_ByKey()
	  public virtual void testUnparseableLongVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableDoubleVariable_ByKey()
	  public virtual void testUnparseableDoubleVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnparseableDateVariable_ByKey()
	  public virtual void testUnparseableDateVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotSupportedTypeVariable_ByKey()
	  public virtual void testNotSupportedTypeVariable_ByKey()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot instantiate process definition aProcDefId: Unsupported value type 'X'")).when().post(START_PROCESS_INSTANCE_BY_KEY_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLive()
	  public virtual void testUpdateHistoryTimeToLive()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_HISTORY_TIMETOLIVE_URL);

		verify(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNullValue()
	  public virtual void testUpdateHistoryTimeToLiveNullValue()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).content(new HistoryTimeToLiveDto()).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_HISTORY_TIMETOLIVE_URL);

		verify(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveNegativeValue()
	  public virtual void testUpdateHistoryTimeToLiveNegativeValue()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new BadUserRequestException(expectedMessage)).when(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq(-1));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).content(new HistoryTimeToLiveDto(-1)).contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(BadUserRequestException).Name)).body("message", containsString(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_HISTORY_TIMETOLIVE_URL);

		verify(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, -1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateHistoryTimeToLiveAuthorizationException()
	  public virtual void testUpdateHistoryTimeToLiveAuthorizationException()
	  {
		string expectedMessage = "expectedMessage";

		doThrow(new AuthorizationException(expectedMessage)).when(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq(5));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).content(new HistoryTimeToLiveDto(5)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", containsString(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_HISTORY_TIMETOLIVE_URL);

		verify(repositoryServiceMock).updateProcessDefinitionHistoryTimeToLive(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionExcludingInstances_ByKey()
	  public virtual void testActivateProcessDefinitionExcludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionExcludingInstances_ByKey()
	  public virtual void testDelayedActivateProcessDefinitionExcludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionIncludingInstances_ByKey()
	  public virtual void testActivateProcessDefinitionIncludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateProcessDefinitionIncludingInstances_ByKey()
	  public virtual void testDelayedActivateProcessDefinitionIncludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsProcessEngineException_ByKey()
	  public virtual void testActivateThrowsProcessEngineException_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(repositoryServiceMock).activateProcessDefinitionById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq(false), isNull(typeof(DateTime)));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", containsString(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateNonParseableDateFormat_ByKey()
	  public virtual void testActivateNonParseableDateFormat_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Process Definition with id " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessDefinitionThrowsAuthorizationException_ByKey()
	  public virtual void testActivateProcessDefinitionThrowsAuthorizationException_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).activateProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionExcludingInstances_ByKey()
	  public virtual void testSuspendProcessDefinitionExcludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionExcludingInstances_ByKey()
	  public virtual void testDelayedSuspendProcessDefinitionExcludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionIncludingInstances_ByKey()
	  public virtual void testSuspendProcessDefinitionIncludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, null);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendProcessDefinitionIncludingInstances_ByKey()
	  public virtual void testDelayedSuspendProcessDefinitionIncludingInstances_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);

		verify(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, true, executionDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsProcessEngineException_ByKey()
	  public virtual void testSuspendThrowsProcessEngineException_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(repositoryServiceMock).suspendProcessDefinitionById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID), eq(false), isNull(typeof(DateTime)));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", containsString(expectedMessage)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendNonParseableDateFormat_ByKey()
	  public virtual void testSuspendNonParseableDateFormat_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeProcessInstances"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Process Definition with id " + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessDefinitionThrowsAuthorizationException_ByKey()
	  public virtual void testSuspendProcessDefinitionThrowsAuthorizationException_ByKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(repositoryServiceMock).suspendProcessDefinitionById(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, false, null);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_DEFINITION_BY_KEY_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithCaseInstanceId() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithCaseInstanceId()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceId"] = "myCaseInstanceId";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).caseInstanceId("myCaseInstanceId");
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithCaseInstanceIdAndBusinessKey() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithCaseInstanceIdAndBusinessKey()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceId"] = "myCaseInstanceId";
		json["businessKey"] = "myBusinessKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).caseInstanceId("myCaseInstanceId");
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstantiationWithCaseInstanceIdAndBusinessKeyAndParameters() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessInstantiationWithCaseInstanceIdAndBusinessKeyAndParameters()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceId"] = "myCaseInstanceId";
		json["businessKey"] = "myBusinessKey";

		IDictionary<string, object> parameters = VariablesBuilder.create().variable("aBoolean", true).variable("aString", "aStringVariableValue").variable("anInteger", 42).Variables;

		json["variables"] = parameters;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).when().post(START_PROCESS_INSTANCE_URL);

		IDictionary<string, object> expectedParameters = new Dictionary<string, object>();
		expectedParameters["aBoolean"] = true;
		expectedParameters["aString"] = "aStringVariableValue";
		expectedParameters["anInteger"] = 42;

		verify(runtimeServiceMock).createProcessInstanceById(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verify(mockInstantiationBuilder).businessKey("myBusinessKey");
		verify(mockInstantiationBuilder).caseInstanceId("myCaseInstanceId");
		verify(mockInstantiationBuilder).Variables = argThat(new EqualsMap(expectedParameters));
		verify(mockInstantiationBuilder).executeWithVariablesInReturn(anyBoolean(), anyBoolean());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesThrowsAuthorizationException_ByKey()
	  public virtual void testGetStartFormVariablesThrowsAuthorizationException_ByKey()
	  {
		string message = "expected exception";
		when(formServiceMock.getStartFormVariables(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, null, true)).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(START_FORM_VARIABLES_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartForm_ByKey()
	  public virtual void testGetDeployedStartForm_ByKey()
	  {
		Stream deployedStartFormMock = new MemoryStream("Test".GetBytes());
		when(formServiceMock.getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(deployedStartFormMock);

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body(equalTo("Test")).when().get(DEPLOYED_START_FORM_BY_KEY_URL);

		verify(formServiceMock).getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartForm()
	  public virtual void testGetDeployedStartForm()
	  {
		Stream deployedStartFormMock = new MemoryStream("Test".GetBytes());
		when(formServiceMock.getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(deployedStartFormMock);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body(equalTo("Test")).when().get(DEPLOYED_START_FORM_URL);

		verify(formServiceMock).getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartFormWithoutAuthorization()
	  public virtual void testGetDeployedStartFormWithoutAuthorization()
	  {
		string message = "unauthorized";
		when(formServiceMock.getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("message", equalTo(message)).when().get(DEPLOYED_START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartFormWithWrongFormKeyFormat()
	  public virtual void testGetDeployedStartFormWithWrongFormKeyFormat()
	  {
		string message = "wrong key format";
		when(formServiceMock.getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new BadUserRequestException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", equalTo(message)).when().get(DEPLOYED_START_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartFormWithUnexistingForm()
	  public virtual void testGetDeployedStartFormWithUnexistingForm()
	  {
		string message = "not found";
		when(formServiceMock.getDeployedStartForm(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenThrow(new NotFoundException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("message", equalTo(message)).when().get(DEPLOYED_START_FORM_URL);
	  }

	}

}