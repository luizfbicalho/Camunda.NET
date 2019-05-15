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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_CASE_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_CASE_EXECUTION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_CASE_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_GROUP_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_GROUP_ID2;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ATTACHMENT_DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ATTACHMENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ATTACHMENT_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ATTACHMENT_TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ATTACHMENT_URL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_COMMENT_FULL_MESSAGE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_COMMENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_COMMENT_ROOT_PROCESS_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_COMMENT_TIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_EXECUTION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_OWNER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_PARENT_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_USER_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.NON_EXISTING_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricTaskInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.endsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasItem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
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
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EncodingUtil = org.camunda.bpm.engine.rest.util.EncodingUtil;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using Assertions = org.assertj.core.api.Assertions;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using Matchers = org.mockito.Matchers;


	using ContentType = io.restassured.http.ContentType;
	using JsonPath = io.restassured.path.json.JsonPath;
	using Response = io.restassured.response.Response;

	public class TaskRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string TASK_SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/task";
	  protected internal static readonly string SINGLE_TASK_URL = TASK_SERVICE_URL + "/{id}";
	  protected internal static readonly string CLAIM_TASK_URL = SINGLE_TASK_URL + "/claim";
	  protected internal static readonly string UNCLAIM_TASK_URL = SINGLE_TASK_URL + "/unclaim";
	  protected internal static readonly string COMPLETE_TASK_URL = SINGLE_TASK_URL + "/complete";
	  protected internal static readonly string RESOLVE_TASK_URL = SINGLE_TASK_URL + "/resolve";
	  protected internal static readonly string DELEGATE_TASK_URL = SINGLE_TASK_URL + "/delegate";
	  protected internal static readonly string ASSIGNEE_TASK_URL = SINGLE_TASK_URL + "/assignee";
	  protected internal static readonly string TASK_IDENTITY_LINKS_URL = SINGLE_TASK_URL + "/identity-links";

	  protected internal static readonly string TASK_FORM_URL = SINGLE_TASK_URL + "/form";
	  protected internal static readonly string DEPLOYED_TASK_FORM_URL = SINGLE_TASK_URL + "/deployed-form";
	  protected internal static readonly string RENDERED_FORM_URL = SINGLE_TASK_URL + "/rendered-form";
	  protected internal static readonly string SUBMIT_FORM_URL = SINGLE_TASK_URL + "/submit-form";

	  protected internal static readonly string FORM_VARIABLES_URL = SINGLE_TASK_URL + "/form-variables";

	  protected internal static readonly string SINGLE_TASK_ADD_COMMENT_URL = SINGLE_TASK_URL + "/comment/create";
	  protected internal static readonly string SINGLE_TASK_COMMENTS_URL = SINGLE_TASK_URL + "/comment";
	  protected internal static readonly string SINGLE_TASK_SINGLE_COMMENT_URL = SINGLE_TASK_COMMENTS_URL + "/{commentId}";

	  protected internal static readonly string SINGLE_TASK_ADD_ATTACHMENT_URL = SINGLE_TASK_URL + "/attachment/create";
	  protected internal static readonly string SINGLE_TASK_ATTACHMENTS_URL = SINGLE_TASK_URL + "/attachment";
	  protected internal static readonly string SINGLE_TASK_SINGLE_ATTACHMENT_URL = SINGLE_TASK_ATTACHMENTS_URL + "/{attachmentId}";
	  protected internal static readonly string SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL = SINGLE_TASK_SINGLE_ATTACHMENT_URL;
	  protected internal static readonly string SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL = SINGLE_TASK_ATTACHMENTS_URL + "/{attachmentId}/data";

	  protected internal static readonly string TASK_CREATE_URL = TASK_SERVICE_URL + "/create";

	  private Task mockTask;
	  private TaskService taskServiceMock;
	  private TaskQuery mockQuery;
	  private FormService formServiceMock;
	  private ManagementService managementServiceMock;
	  private RepositoryService repositoryServiceMock;

	  private IdentityLink mockAssigneeIdentityLink;
	  private IdentityLink mockOwnerIdentityLink;
	  private IdentityLink mockCandidateGroupIdentityLink;
	  private IdentityLink mockCandidateGroup2IdentityLink;

	  private HistoricTaskInstanceQuery historicTaskInstanceQueryMock;

	  private Comment mockTaskComment;
	  private IList<Comment> mockTaskComments;

	  private Attachment mockTaskAttachment;
	  private IList<Attachment> mockTaskAttachments;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		taskServiceMock = mock(typeof(TaskService));
		when(processEngine.TaskService).thenReturn(taskServiceMock);

		mockTask = MockProvider.createMockTask();
		mockQuery = mock(typeof(TaskQuery));
		when(mockQuery.initializeFormKeys()).thenReturn(mockQuery);
		when(mockQuery.taskId(anyString())).thenReturn(mockQuery);
		when(mockQuery.singleResult()).thenReturn(mockTask);
		when(taskServiceMock.createTaskQuery()).thenReturn(mockQuery);

		IList<IdentityLink> identityLinks = new List<IdentityLink>();
		mockAssigneeIdentityLink = MockProvider.createMockUserAssigneeIdentityLink();
		identityLinks.Add(mockAssigneeIdentityLink);
		mockOwnerIdentityLink = MockProvider.createMockUserOwnerIdentityLink();
		identityLinks.Add(mockOwnerIdentityLink);
		mockCandidateGroupIdentityLink = MockProvider.createMockCandidateGroupIdentityLink();
		identityLinks.Add(mockCandidateGroupIdentityLink);
		mockCandidateGroup2IdentityLink = MockProvider.createAnotherMockCandidateGroupIdentityLink();
		identityLinks.Add(mockCandidateGroup2IdentityLink);
		when(taskServiceMock.getIdentityLinksForTask(EXAMPLE_TASK_ID)).thenReturn(identityLinks);

		mockTaskComment = MockProvider.createMockTaskComment();
		when(taskServiceMock.getTaskComment(EXAMPLE_TASK_ID, EXAMPLE_TASK_COMMENT_ID)).thenReturn(mockTaskComment);
		mockTaskComments = MockProvider.createMockTaskComments();
		when(taskServiceMock.getTaskComments(EXAMPLE_TASK_ID)).thenReturn(mockTaskComments);
		when(taskServiceMock.createComment(EXAMPLE_TASK_ID, null, EXAMPLE_TASK_COMMENT_FULL_MESSAGE)).thenReturn(mockTaskComment);

		mockTaskAttachment = MockProvider.createMockTaskAttachment();
		when(taskServiceMock.getTaskAttachment(EXAMPLE_TASK_ID, EXAMPLE_TASK_ATTACHMENT_ID)).thenReturn(mockTaskAttachment);
		mockTaskAttachments = MockProvider.createMockTaskAttachments();
		when(taskServiceMock.getTaskAttachments(EXAMPLE_TASK_ID)).thenReturn(mockTaskAttachments);
		when(taskServiceMock.createAttachment(anyString(), anyString(), anyString(), anyString(), anyString(), anyString())).thenReturn(mockTaskAttachment);
		when(taskServiceMock.createAttachment(anyString(), anyString(), anyString(), anyString(), anyString(), any(typeof(Stream)))).thenReturn(mockTaskAttachment);
		when(taskServiceMock.getTaskAttachmentContent(EXAMPLE_TASK_ID, EXAMPLE_TASK_ATTACHMENT_ID)).thenReturn(new MemoryStream(createMockByteData()));

		formServiceMock = mock(typeof(FormService));
		when(processEngine.FormService).thenReturn(formServiceMock);
		TaskFormData mockFormData = MockProvider.createMockTaskFormData();
		when(formServiceMock.getTaskFormData(anyString())).thenReturn(mockFormData);

		VariableMap variablesMock = MockProvider.createMockFormVariables();
		when(formServiceMock.getTaskFormVariables(eq(EXAMPLE_TASK_ID), Matchers.any<ICollection<string>>(), anyBoolean())).thenReturn(variablesMock);

		repositoryServiceMock = mock(typeof(RepositoryService));
		when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		when(repositoryServiceMock.getProcessDefinition(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(mockDefinition);

		managementServiceMock = mock(typeof(ManagementService));
		when(processEngine.ManagementService).thenReturn(managementServiceMock);
		when(managementServiceMock.getProcessApplicationForDeployment(MockProvider.EXAMPLE_DEPLOYMENT_ID)).thenReturn(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME);
		when(managementServiceMock.HistoryLevel).thenReturn(ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL);

		HistoryService historyServiceMock = mock(typeof(HistoryService));
		when(processEngine.HistoryService).thenReturn(historyServiceMock);
		historicTaskInstanceQueryMock = mock(typeof(HistoricTaskInstanceQuery));
		when(historyServiceMock.createHistoricTaskInstanceQuery()).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.taskId(eq(EXAMPLE_TASK_ID))).thenReturn(historicTaskInstanceQueryMock);
		HistoricTaskInstance historicTaskInstanceMock = createMockHistoricTaskInstance();
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(historicTaskInstanceMock);

		// replace the runtime container delegate & process application service with a mock

		ProcessApplicationService processApplicationService = mock(typeof(ProcessApplicationService));
		ProcessApplicationInfo appMock = MockProvider.createMockProcessApplicationInfo();
		when(processApplicationService.getProcessApplicationInfo(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME)).thenReturn(appMock);

		RuntimeContainerDelegate @delegate = mock(typeof(RuntimeContainerDelegate));
		when(@delegate.ProcessApplicationService).thenReturn(processApplicationService);
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.set(@delegate);
	  }

	  public virtual void mockHistoryDisabled()
	  {
		when(managementServiceMock.HistoryLevel).thenReturn(ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE);
	  }

	  private sbyte[] createMockByteData()
	  {
		return "someContent".GetBytes();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTask()
	  public virtual void testGetSingleTask()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(EXAMPLE_TASK_ID)).body("name", equalTo(MockProvider.EXAMPLE_TASK_NAME)).body("assignee", equalTo(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME)).body("created", equalTo(MockProvider.EXAMPLE_TASK_CREATE_TIME)).body("due", equalTo(MockProvider.EXAMPLE_TASK_DUE_DATE)).body("delegationState", equalTo(MockProvider.EXAMPLE_TASK_DELEGATION_STATE.ToString())).body("description", equalTo(MockProvider.EXAMPLE_TASK_DESCRIPTION)).body("executionId", equalTo(MockProvider.EXAMPLE_TASK_EXECUTION_ID)).body("owner", equalTo(MockProvider.EXAMPLE_TASK_OWNER)).body("parentTaskId", equalTo(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID)).body("priority", equalTo(MockProvider.EXAMPLE_TASK_PRIORITY)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("taskDefinitionKey", equalTo(MockProvider.EXAMPLE_TASK_DEFINITION_KEY)).body("suspended", equalTo(MockProvider.EXAMPLE_TASK_SUSPENSION_STATE)).body("caseExecutionId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).body("caseInstanceId", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).body("caseDefinitionId", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void testGetSingleTaskHal()
	  public virtual void testGetSingleTaskHal()
	  {

		// setup user query mock
		IList<User> mockUsers = Arrays.asList(MockProvider.mockUser().id(EXAMPLE_TASK_ASSIGNEE_NAME).build(), MockProvider.mockUser().id(EXAMPLE_TASK_OWNER).build());
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(sampleUserQuery.userIdIn(eq(EXAMPLE_TASK_ASSIGNEE_NAME), eq(EXAMPLE_TASK_OWNER))).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userIdIn(eq(EXAMPLE_TASK_OWNER), eq(EXAMPLE_TASK_ASSIGNEE_NAME))).thenReturn(sampleUserQuery);
		when(sampleUserQuery.listPage(eq(0), eq(2))).thenReturn(mockUsers);
		when(sampleUserQuery.count()).thenReturn((long) mockUsers.Count);
		when(processEngine.IdentityService.createUserQuery()).thenReturn(sampleUserQuery);

		// setup group query mock
		IList<Group> mockGroups = Arrays.asList(MockProvider.mockGroup().id(mockCandidateGroupIdentityLink.GroupId).build(), MockProvider.mockGroup().id(mockCandidateGroup2IdentityLink.GroupId).build());
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(sampleGroupQuery.groupIdIn(eq(EXAMPLE_GROUP_ID), eq(EXAMPLE_GROUP_ID2))).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupIdIn(eq(EXAMPLE_GROUP_ID2), eq(EXAMPLE_GROUP_ID))).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.listPage(eq(0), eq(2))).thenReturn(mockGroups);
		when(sampleGroupQuery.count()).thenReturn((long) mockGroups.Count);
		when(processEngine.IdentityService.createGroupQuery()).thenReturn(sampleGroupQuery);

		// setup process definition query mock
		IList<ProcessDefinition> mockDefinitions = MockProvider.createMockDefinitions();
		ProcessDefinitionQuery sampleProcessDefinitionQuery = mock(typeof(ProcessDefinitionQuery));
		when(sampleProcessDefinitionQuery.listPage(0, 1)).thenReturn(mockDefinitions);
		when(sampleProcessDefinitionQuery.processDefinitionIdIn(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(sampleProcessDefinitionQuery);
		when(sampleProcessDefinitionQuery.count()).thenReturn(1l);
		when(processEngine.RepositoryService.createProcessDefinitionQuery()).thenReturn(sampleProcessDefinitionQuery);

		// setup case definition query mock
		IList<CaseDefinition> mockCaseDefinitions = MockProvider.createMockCaseDefinitions();
		CaseDefinitionQuery sampleCaseDefinitionQuery = mock(typeof(CaseDefinitionQuery));
		when(sampleCaseDefinitionQuery.listPage(0, 1)).thenReturn(mockCaseDefinitions);
		when(sampleCaseDefinitionQuery.caseDefinitionIdIn(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(sampleCaseDefinitionQuery);
		when(sampleCaseDefinitionQuery.count()).thenReturn(1l);
		when(processEngine.RepositoryService.createCaseDefinitionQuery()).thenReturn(sampleCaseDefinitionQuery);

		Response response = given().header("accept", Hal.APPLICATION_HAL_JSON).pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(EXAMPLE_TASK_ID)).body("name", equalTo(MockProvider.EXAMPLE_TASK_NAME)).body("assignee", equalTo(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME)).body("created", equalTo(MockProvider.EXAMPLE_TASK_CREATE_TIME)).body("due", equalTo(MockProvider.EXAMPLE_TASK_DUE_DATE)).body("delegationState", equalTo(MockProvider.EXAMPLE_TASK_DELEGATION_STATE.ToString())).body("description", equalTo(MockProvider.EXAMPLE_TASK_DESCRIPTION)).body("executionId", equalTo(MockProvider.EXAMPLE_TASK_EXECUTION_ID)).body("owner", equalTo(MockProvider.EXAMPLE_TASK_OWNER)).body("parentTaskId", equalTo(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID)).body("priority", equalTo(MockProvider.EXAMPLE_TASK_PRIORITY)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("taskDefinitionKey", equalTo(MockProvider.EXAMPLE_TASK_DEFINITION_KEY)).body("suspended", equalTo(MockProvider.EXAMPLE_TASK_SUSPENSION_STATE)).body("caseExecutionId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).body("caseInstanceId", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).body("caseDefinitionId", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("_links.assignee.href", endsWith(EXAMPLE_TASK_ASSIGNEE_NAME)).body("_links.caseDefinition.href", endsWith(EXAMPLE_CASE_DEFINITION_ID)).body("_links.caseExecution.href", endsWith(EXAMPLE_CASE_EXECUTION_ID)).body("_links.caseInstance.href", endsWith(EXAMPLE_CASE_INSTANCE_ID)).body("_links.execution.href", endsWith(EXAMPLE_TASK_EXECUTION_ID)).body("_links.owner.href", endsWith(EXAMPLE_TASK_OWNER)).body("_links.parentTask.href", endsWith(EXAMPLE_TASK_PARENT_TASK_ID)).body("_links.processDefinition.href", endsWith(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("_links.processInstance.href", endsWith(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("_links.identityLink.href", endsWith("/task/" + EXAMPLE_TASK_ID + "/identity-links")).body("_links.self.href", endsWith(EXAMPLE_TASK_ID)).when().get(SINGLE_TASK_URL);

		string content = response.asString();

		// validate embedded users:
		IList<IDictionary<string, object>> embeddedUsers = from(content).getList("_embedded.user");
		Assert.assertEquals("There should be two users returned.", 2, embeddedUsers.Count);

		IDictionary<string, object> embeddedUser = embeddedUsers[0];
		assertNotNull("The returned user should not be null.", embeddedUser);
		assertEquals(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME, embeddedUser["id"]);
		assertEquals(MockProvider.EXAMPLE_USER_FIRST_NAME, embeddedUser["firstName"]);
		assertEquals(MockProvider.EXAMPLE_USER_LAST_NAME, embeddedUser["lastName"]);
		assertEquals(MockProvider.EXAMPLE_USER_EMAIL, embeddedUser["email"]);
		assertNull(embeddedUser["_embedded"]);
		IDictionary<string, object> links = (IDictionary<string, object>) embeddedUser["_links"];
		assertEquals(1, links.Count);
		assertHalLink(links, "self", UserRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME);

		embeddedUser = embeddedUsers[1];
		assertNotNull("The returned user should not be null.", embeddedUser);
		assertEquals(MockProvider.EXAMPLE_TASK_OWNER, embeddedUser["id"]);
		assertEquals(MockProvider.EXAMPLE_USER_FIRST_NAME, embeddedUser["firstName"]);
		assertEquals(MockProvider.EXAMPLE_USER_LAST_NAME, embeddedUser["lastName"]);
		assertEquals(MockProvider.EXAMPLE_USER_EMAIL, embeddedUser["email"]);
		assertNull(embeddedUser["_embedded"]);
		links = (IDictionary<string, object>) embeddedUser["_links"];
		assertEquals(1, links.Count);
		assertHalLink(links, "self", UserRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_TASK_OWNER);

		// validate embedded groups:
		IList<IDictionary<string, object>> embeddedGroups = from(content).getList("_embedded.group");
		Assert.assertEquals("There should be two groups returned.", 2, embeddedGroups.Count);

		IDictionary<string, object> embeddedGroup = embeddedGroups[0];
		assertNotNull("The returned group should not be null.", embeddedGroup);
		assertEquals(MockProvider.EXAMPLE_GROUP_ID, embeddedGroup["id"]);
		assertEquals(MockProvider.EXAMPLE_GROUP_NAME, embeddedGroup["name"]);
		assertEquals(MockProvider.EXAMPLE_GROUP_TYPE, embeddedGroup["type"]);
		assertNull(embeddedGroup["_embedded"]);
		links = (IDictionary<string, object>) embeddedGroup["_links"];
		assertEquals(1, links.Count);
		assertHalLink(links, "self", GroupRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_GROUP_ID);

		embeddedGroup = embeddedGroups[1];
		assertNotNull("The returned group should not be null.", embeddedGroup);
		assertEquals(MockProvider.EXAMPLE_GROUP_ID2, embeddedGroup["id"]);
		assertEquals(MockProvider.EXAMPLE_GROUP_NAME, embeddedGroup["name"]);
		assertEquals(MockProvider.EXAMPLE_GROUP_TYPE, embeddedGroup["type"]);
		assertNull(embeddedGroup["_embedded"]);
		links = (IDictionary<string, object>) embeddedGroup["_links"];
		assertEquals(1, links.Count);
		assertHalLink(links, "self", GroupRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_GROUP_ID2);

		// validate embedded processDefinitions:
		IList<IDictionary<string, object>> embeddedDefinitions = from(content).getList("_embedded.processDefinition");
		Assert.assertEquals("There should be one processDefinition returned.", 1, embeddedDefinitions.Count);
		IDictionary<string, object> embeddedProcessDefinition = embeddedDefinitions[0];
		Assert.assertNotNull("The returned processDefinition should not be null.", embeddedProcessDefinition);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, embeddedProcessDefinition["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, embeddedProcessDefinition["key"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY, embeddedProcessDefinition["category"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME, embeddedProcessDefinition["name"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION, embeddedProcessDefinition["description"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION, embeddedProcessDefinition["version"]);
		Assert.assertEquals(MockProvider.EXAMPLE_VERSION_TAG, embeddedProcessDefinition["versionTag"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME, embeddedProcessDefinition["resource"]);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, embeddedProcessDefinition["deploymentId"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME, embeddedProcessDefinition["diagram"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED, embeddedProcessDefinition["suspended"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH, embeddedProcessDefinition["contextPath"]);

		links = (IDictionary<string, object>) embeddedProcessDefinition["_links"];
		Assert.assertEquals(3, links.Count);
		assertHalLink(links, "self", "/process-definition/" + MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		assertHalLink(links, "deployment", "/deployment/" + MockProvider.EXAMPLE_DEPLOYMENT_ID);
		assertHalLink(links, "resource", "/deployment/" + MockProvider.EXAMPLE_DEPLOYMENT_ID + "/resources/" + MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME);


		// validate embedded caseDefinitions:
		IList<IDictionary<string, object>> embeddedCaseDefinitions = from(content).getList("_embedded.caseDefinition");
		Assert.assertEquals("There should be one caseDefinition returned.", 1, embeddedCaseDefinitions.Count);
		IDictionary<string, object> embeddedCaseDefinition = embeddedCaseDefinitions[0];
		Assert.assertNotNull("The returned caseDefinition should not be null.", embeddedCaseDefinition);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, embeddedCaseDefinition["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_KEY, embeddedCaseDefinition["key"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_CATEGORY, embeddedCaseDefinition["category"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_NAME, embeddedCaseDefinition["name"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_VERSION, embeddedCaseDefinition["version"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME, embeddedCaseDefinition["resource"]);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, embeddedCaseDefinition["deploymentId"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH, embeddedCaseDefinition["contextPath"]);

		links = (IDictionary<string, object>) embeddedCaseDefinition["_links"];
		Assert.assertEquals(3, links.Count);
		assertHalLink(links, "self", "/case-definition/" + MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		assertHalLink(links, "deployment", "/deployment/" + MockProvider.EXAMPLE_DEPLOYMENT_ID);
		assertHalLink(links, "resource", "/deployment/" + MockProvider.EXAMPLE_DEPLOYMENT_ID + "/resources/" + MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME);

		// validate embedded identity links
		IList<IDictionary<string, object>> embeddedIdentityLinks = from(content).getList("_embedded.identityLink");
		assertEquals("There should be three identityLink returned", 4, embeddedIdentityLinks.Count);
		assertEmbeddedIdentityLink(mockAssigneeIdentityLink, embeddedIdentityLinks[0]);
		assertEmbeddedIdentityLink(mockOwnerIdentityLink, embeddedIdentityLinks[1]);
		assertEmbeddedIdentityLink(mockCandidateGroupIdentityLink, embeddedIdentityLinks[2]);
		assertEmbeddedIdentityLink(mockCandidateGroup2IdentityLink, embeddedIdentityLinks[3]);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void assertHalLink(java.util.Map<String, Object> links, String key, String expectedLink)
	  protected internal virtual void assertHalLink(IDictionary<string, object> links, string key, string expectedLink)
	  {
		IDictionary<string, object> linkObject = (IDictionary<string, object>) links[key];
		Assert.assertNotNull(linkObject);

		string actualLink = (string) linkObject["href"];
		Assert.assertEquals(expectedLink, actualLink);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void assertEmbeddedIdentityLink(org.camunda.bpm.engine.task.IdentityLink expected, java.util.Map<String, Object> actual)
	  protected internal virtual void assertEmbeddedIdentityLink(IdentityLink expected, IDictionary<string, object> actual)
	  {
		assertNotNull("Embedded indentity link should not be null", actual);
		assertEquals(expected.Type, actual["type"]);
		assertEquals(expected.UserId, actual["userId"]);
		assertEquals(expected.GroupId, actual["groupId"]);
		assertEquals(expected.TaskId, actual["taskId"]);
		assertNull(actual["_embedded"]);

		IDictionary<string, object> links = (IDictionary<string, object>) actual["_links"];
		if (!string.ReferenceEquals(expected.UserId, null))
		{
		  assertHalLink(links, "user", UserRestService_Fields.PATH + "/" + expected.UserId);
		}
		if (!string.ReferenceEquals(expected.GroupId, null))
		{
		  assertHalLink(links, "group", GroupRestService_Fields.PATH + "/" + expected.GroupId);
		}
		if (!string.ReferenceEquals(expected.TaskId, null))
		{
		  assertHalLink(links, "task", TaskRestService_Fields.PATH + "/" + expected.TaskId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetForm()
	  public virtual void testGetForm()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo(MockProvider.EXAMPLE_FORM_KEY)).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(TASK_FORM_URL);
	  }

	  /// <summary>
	  /// Assuming the task belongs to a deployment that does not belong to any process application
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFormForNonRegisteredDeployment()
	  public virtual void testGetFormForNonRegisteredDeployment()
	  {
		when(managementServiceMock.getProcessApplicationForDeployment(MockProvider.EXAMPLE_DEPLOYMENT_ID)).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo(MockProvider.EXAMPLE_FORM_KEY)).body("contextPath", nullValue()).when().get(TASK_FORM_URL);
	  }

	  /// <summary>
	  /// Assuming that the task belongs to no process definition
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getFormForIndependentTask()
	  public virtual void getFormForIndependentTask()
	  {
		when(mockTask.ProcessDefinitionId).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo(MockProvider.EXAMPLE_FORM_KEY)).body("contextPath", nullValue()).when().get(TASK_FORM_URL);

		verify(repositoryServiceMock, never()).getProcessDefinition(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetForm_shouldReturnKeyContainingTaskId()
	  public virtual void testGetForm_shouldReturnKeyContainingTaskId()
	  {
		TaskFormData mockTaskFormData = MockProvider.createMockTaskFormDataUsingFormFieldsWithoutFormKey();
		when(formServiceMock.getTaskFormData(EXAMPLE_TASK_ID)).thenReturn(mockTaskFormData);

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("key", equalTo("embedded:engine://engine/:engine/task/" + EXAMPLE_TASK_ID + "/rendered-form")).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetForm__FormDataEqualsNull()
	  public virtual void testGetForm__FormDataEqualsNull()
	  {
		when(formServiceMock.getTaskFormData(EXAMPLE_TASK_ID)).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("contextPath", equalTo(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH)).when().get(TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFormThrowsAuthorizationException()
	  public virtual void testGetFormThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).getTaskFormData(anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedForm()
	  public virtual void testGetRenderedForm()
	  {
		string expectedResult = "<formField>anyContent</formField>";

		when(formServiceMock.getRenderedTaskForm(EXAMPLE_TASK_ID)).thenReturn(expectedResult);

		Response response = given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(XHTML_XML_CONTENT_TYPE).when().get(RENDERED_FORM_URL);

		string responseContent = response.asString();
		Assertions.assertThat(responseContent).isEqualTo(expectedResult);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedFormForDifferentPlatformEncoding() throws NoSuchFieldException, IllegalAccessException, java.io.UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testGetRenderedFormForDifferentPlatformEncoding()
	  {
		string expectedResult = "<formField>unicode symbol: \u2200</formField>";
		when(formServiceMock.getRenderedTaskForm(MockProvider.EXAMPLE_TASK_ID)).thenReturn(expectedResult);

		Response response = given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(XHTML_XML_CONTENT_TYPE).when().get(RENDERED_FORM_URL);

		string responseContent = new string(response.asByteArray(), EncodingUtil.DEFAULT_ENCODING);
		Assertions.assertThat(responseContent).isEqualTo(expectedResult);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedFormReturnsNotFound()
	  public virtual void testGetRenderedFormReturnsNotFound()
	  {
		when(formServiceMock.getRenderedTaskForm(anyString(), anyString())).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching rendered form for task with the id " + EXAMPLE_TASK_ID + " found.")).when().get(RENDERED_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetRenderedFormThrowsAuthorizationException()
	  public virtual void testGetRenderedFormThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).getRenderedTaskForm(anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(RENDERED_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitForm()
	  public virtual void testSubmitForm()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitTaskForm(EXAMPLE_TASK_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithVariablesInReturn()
	  public virtual void testSubmitFormWithVariablesInReturn()
	  {
		VariableMap variables = MockProvider.createMockSerializedVariables();
		when(formServiceMock.submitTaskFormWithVariablesInReturn(EXAMPLE_TASK_ID, null, false)).thenReturn(variables);

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withVariablesInReturn"] = true;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(List<object>).FullName)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(object).FullName)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitTaskFormWithVariablesInReturn(EXAMPLE_TASK_ID, null, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithParameters()
	  public virtual void testSubmitFormWithParameters()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SUBMIT_FORM_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(formServiceMock).submitTaskForm(eq(EXAMPLE_TASK_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormWithBase64EncodedBytes()
	  public virtual void testSubmitTaskFormWithBase64EncodedBytes()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", Base64.encodeBase64String("someBytes".GetBytes()), "Bytes").Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SUBMIT_FORM_URL);

		verify(formServiceMock).submitTaskForm(eq(EXAMPLE_TASK_ID), argThat(new EqualsMap()
		  .matcher("aVariable", EqualsPrimitiveValue.bytesValue("someBytes".GetBytes()))));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) @Test public void testSubmitTaskFormWithFileValue()
	  public virtual void testSubmitTaskFormWithFileValue()
	  {
		string variableKey = "aVariable";
		string filename = "test.txt";
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, Base64.encodeBase64String("someBytes".GetBytes()), "File").Variables;
		((IDictionary<string, object>)variables[variableKey])["valueInfo"] = Collections.singletonMap<string, object>("filename", filename);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SUBMIT_FORM_URL);

		ArgumentCaptor<VariableMap> captor = ArgumentCaptor.forClass(typeof(VariableMap));
		verify(formServiceMock).submitTaskForm(eq(EXAMPLE_TASK_ID), captor.capture());
		VariableMap map = captor.Value;
		FileValue fileValue = (FileValue) map.getValueTyped(variableKey);
		assertThat(fileValue, @is(notNullValue()));
		assertThat(fileValue.Filename, @is(filename));
		assertThat(IoUtil.readInputStream(fileValue.Value, null), @is("someBytes".GetBytes()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithUnparseableIntegerVariable()
	  public virtual void testSubmitFormWithUnparseableIntegerVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithUnparseableShortVariable()
	  public virtual void testSubmitFormWithUnparseableShortVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithUnparseableLongVariable()
	  public virtual void testSubmitFormWithUnparseableLongVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithUnparseableDoubleVariable()
	  public virtual void testSubmitFormWithUnparseableDoubleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithUnparseableDateVariable()
	  public virtual void testSubmitFormWithUnparseableDateVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormWithNotSupportedVariableType()
	  public virtual void testSubmitFormWithNotSupportedVariableType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot submit task form anId: Unsupported value type 'X'")).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulSubmitForm()
	  public virtual void testUnsuccessfulSubmitForm()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(formServiceMock).submitTaskForm(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot submit task form " + EXAMPLE_TASK_ID + ": expected exception")).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitFormThrowsAuthorizationException()
	  public virtual void testSubmitFormThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).submitTaskForm(anyString(), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SUBMIT_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormThrowsFormFieldValidationException()
	  public virtual void testSubmitTaskFormThrowsFormFieldValidationException()
	  {
		string message = "expected exception";
		doThrow(new FormFieldValidationException("form-exception", message)).when(formServiceMock).submitTaskForm(anyString(), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot submit task form " + EXAMPLE_TASK_ID + ": " + message)).when().post(SUBMIT_FORM_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariables()
	  public virtual void testGetTaskFormVariables()
	  {

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Type))).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).when().get(FORM_VARIABLES_URL).body();

		verify(formServiceMock, times(1)).getTaskFormVariables(EXAMPLE_TASK_ID, null, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesVarNames()
	  public virtual void testGetTaskFormVariablesVarNames()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).queryParam("variableNames", "a,b,c").header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(FORM_VARIABLES_URL);

		verify(formServiceMock, times(1)).getTaskFormVariables(EXAMPLE_TASK_ID, Arrays.asList(new string[]{"a", "b", "c"}), true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesAndDoNotDeserializeVariables()
	  public virtual void testGetTaskFormVariablesAndDoNotDeserializeVariables()
	  {

		given().pathParam("id", EXAMPLE_TASK_ID).queryParam("deserializeValues", false).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Type))).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).when().get(FORM_VARIABLES_URL).body();

		verify(formServiceMock, times(1)).getTaskFormVariables(EXAMPLE_TASK_ID, null, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesVarNamesAndDoNotDeserializeVariables()
	  public virtual void testGetTaskFormVariablesVarNamesAndDoNotDeserializeVariables()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).queryParam("deserializeValues", false).queryParam("variableNames", "a,b,c").header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(FORM_VARIABLES_URL);

		verify(formServiceMock, times(1)).getTaskFormVariables(EXAMPLE_TASK_ID, Arrays.asList(new string[]{"a", "b", "c"}), false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesThrowsAuthorizationException()
	  public virtual void testGetTaskFormVariablesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(formServiceMock).getTaskFormVariables(anyString(), Matchers.any<ICollection<string>>(), anyBoolean());

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(FORM_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimTask()
	  public virtual void testClaimTask()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = EXAMPLE_USER_ID;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CLAIM_TASK_URL);

		verify(taskServiceMock).claim(EXAMPLE_TASK_ID, EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingUserId()
	  public virtual void testMissingUserId()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = null;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(CLAIM_TASK_URL);

		verify(taskServiceMock).claim(EXAMPLE_TASK_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulClaimTask()
	  public virtual void testUnsuccessfulClaimTask()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).claim(any(typeof(string)), any(typeof(string)));

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().post(CLAIM_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimTaskThrowsAuthorizationException()
	  public virtual void testClaimTaskThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).claim(anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(CLAIM_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnclaimTask()
	  public virtual void testUnclaimTask()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(UNCLAIM_TASK_URL);

		verify(taskServiceMock).setAssignee(EXAMPLE_TASK_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulUnclaimTask()
	  public virtual void testUnsuccessfulUnclaimTask()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).setAssignee(any(typeof(string)), any(typeof(string)));

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().post(UNCLAIM_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnclaimTaskThrowsAuthorizationException()
	  public virtual void testUnclaimTaskThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).setAssignee(anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(UNCLAIM_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAssignee()
	  public virtual void testSetAssignee()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = EXAMPLE_USER_ID;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(ASSIGNEE_TASK_URL);

		verify(taskServiceMock).setAssignee(EXAMPLE_TASK_ID, EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingUserIdSetAssignee()
	  public virtual void testMissingUserIdSetAssignee()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = null;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(ASSIGNEE_TASK_URL);

		verify(taskServiceMock).setAssignee(EXAMPLE_TASK_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulSetAssignee()
	  public virtual void testUnsuccessfulSetAssignee()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).setAssignee(any(typeof(string)), any(typeof(string)));

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().post(ASSIGNEE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAssigneeThrowsAuthorizationException()
	  public virtual void testSetAssigneeThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).setAssignee(anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(ASSIGNEE_TASK_URL);
	  }

	  protected internal virtual IDictionary<string, object> toExpectedJsonMap(IdentityLink identityLink)
	  {
		IDictionary<string, object> result = new Dictionary<string, object>();
		result["userId"] = identityLink.UserId;
		result["groupId"] = identityLink.GroupId;
		result["type"] = identityLink.Type;
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinks()
	  public virtual void testGetIdentityLinks()
	  {
		IDictionary<string, object> expectedAssigneeIdentityLink = toExpectedJsonMap(mockAssigneeIdentityLink);
		IDictionary<string, object> expectedOwnerIdentityLink = toExpectedJsonMap(mockOwnerIdentityLink);
		IDictionary<string, object> expectedGroupIdentityLink = toExpectedJsonMap(mockCandidateGroupIdentityLink);
		IDictionary<string, object> expectedGroupIdentityLink2 = toExpectedJsonMap(mockCandidateGroup2IdentityLink);

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(4)).body("$", hasItem(expectedAssigneeIdentityLink)).body("$", hasItem(expectedOwnerIdentityLink)).body("$", hasItem(expectedGroupIdentityLink)).body("$", hasItem(expectedGroupIdentityLink2)).when().get(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock).getIdentityLinksForTask(EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksByType()
	  public virtual void testGetIdentityLinksByType()
	  {
		IDictionary<string, object> expectedGroupIdentityLink = toExpectedJsonMap(mockCandidateGroupIdentityLink);
		IDictionary<string, object> expectedGroupIdentityLink2 = toExpectedJsonMap(mockCandidateGroup2IdentityLink);

		given().pathParam("id", EXAMPLE_TASK_ID).queryParam("type", IdentityLinkType.CANDIDATE).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(2)).body("$", hasItem(expectedGroupIdentityLink)).body("$", hasItem(expectedGroupIdentityLink2)).when().get(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock).getIdentityLinksForTask(EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksThrowsAuthorizationException()
	  public virtual void testGetIdentityLinksThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).getIdentityLinksForTask(anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_IDENTITY_LINKS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUserIdentityLink()
	  public virtual void testAddUserIdentityLink()
	  {
		string userId = "someUserId";
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = userId;
		json["taskId"] = taskId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock).addUserIdentityLink(taskId, userId, type);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGroupIdentityLink()
	  public virtual void testAddGroupIdentityLink()
	  {
		string groupId = "someGroupId";
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["groupId"] = groupId;
		json["taskId"] = taskId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock).addGroupIdentityLink(taskId, groupId, type);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidAddIdentityLink()
	  public virtual void testInvalidAddIdentityLink()
	  {
		string groupId = "someGroupId";
		string userId = "someUserId";
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["groupId"] = groupId;
		json["userId"] = userId;
		json["taskId"] = taskId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Identity Link requires userId or groupId, but not both")).when().post(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock, never()).addGroupIdentityLink(anyString(), anyString(), anyString());
		verify(taskServiceMock, never()).addGroupIdentityLink(anyString(), anyString(), anyString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnderspecifiedAddIdentityLink()
	  public virtual void testUnderspecifiedAddIdentityLink()
	  {
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskId"] = taskId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Identity Link requires userId or groupId")).when().post(TASK_IDENTITY_LINKS_URL);

		verify(taskServiceMock, never()).addGroupIdentityLink(anyString(), anyString(), anyString());
		verify(taskServiceMock, never()).addGroupIdentityLink(anyString(), anyString(), anyString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGroupIdentityLinkThrowsAuthorizationException()
	  public virtual void testAddGroupIdentityLinkThrowsAuthorizationException()
	  {
		string groupId = "someGroupId";
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["groupId"] = groupId;
		json["taskId"] = taskId;
		json["type"] = type;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).addGroupIdentityLink(anyString(), anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TASK_IDENTITY_LINKS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUserIdentityLinkThrowsAuthorizationException()
	  public virtual void testAddUserIdentityLinkThrowsAuthorizationException()
	  {
		string userId = "someUserId";
		string taskId = EXAMPLE_TASK_ID;
		string type = "someType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = userId;
		json["taskId"] = taskId;
		json["type"] = type;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).addUserIdentityLink(anyString(), anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TASK_IDENTITY_LINKS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUserIdentityLink()
	  public virtual void testDeleteUserIdentityLink()
	  {
		string deleteIdentityLinkUrl = TASK_IDENTITY_LINKS_URL + "/delete";

		string taskId = EXAMPLE_TASK_ID;
		string userId = EXAMPLE_USER_ID;
		string type = "someIdentityLinkType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = userId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(deleteIdentityLinkUrl);

		verify(taskServiceMock).deleteUserIdentityLink(taskId, userId, type);
		verify(taskServiceMock, never()).deleteGroupIdentityLink(anyString(), anyString(), anyString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupIdentityLink()
	  public virtual void testDeleteGroupIdentityLink()
	  {
		string deleteIdentityLinkUrl = TASK_IDENTITY_LINKS_URL + "/delete";

		string taskId = EXAMPLE_TASK_ID;
		string groupId = MockProvider.EXAMPLE_GROUP_ID;
		string type = "someIdentityLinkType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["groupId"] = groupId;
		json["type"] = type;

		given().pathParam("id", taskId).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(deleteIdentityLinkUrl);

		verify(taskServiceMock).deleteGroupIdentityLink(taskId, groupId, type);
		verify(taskServiceMock, never()).deleteUserIdentityLink(anyString(), anyString(), anyString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupIdentityLinkThrowsAuthorizationException()
	  public virtual void testDeleteGroupIdentityLinkThrowsAuthorizationException()
	  {
		string deleteIdentityLinkUrl = TASK_IDENTITY_LINKS_URL + "/delete";

		string taskId = EXAMPLE_TASK_ID;
		string groupId = MockProvider.EXAMPLE_GROUP_ID;
		string type = "someIdentityLinkType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["groupId"] = groupId;
		json["type"] = type;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).deleteGroupIdentityLink(anyString(), anyString(), anyString());

		given().pathParam("id", taskId).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(deleteIdentityLinkUrl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUserIdentityLinkThrowsAuthorizationException()
	  public virtual void testDeleteUserIdentityLinkThrowsAuthorizationException()
	  {
		string deleteIdentityLinkUrl = TASK_IDENTITY_LINKS_URL + "/delete";

		string taskId = EXAMPLE_TASK_ID;
		string userId = MockProvider.EXAMPLE_USER_ID;
		string type = "someIdentityLinkType";

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = userId;
		json["type"] = type;

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).deleteUserIdentityLink(anyString(), anyString(), anyString());

		given().pathParam("id", taskId).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(deleteIdentityLinkUrl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTask()
	  public virtual void testCompleteTask()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(COMPLETE_TASK_URL);

		verify(taskServiceMock).complete(EXAMPLE_TASK_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithParameters()
	  public virtual void testCompleteWithParameters()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(COMPLETE_TASK_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(taskServiceMock).complete(eq(EXAMPLE_TASK_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskWithVariablesInReturn()
	  public virtual void testCompleteTaskWithVariablesInReturn()
	  {
		VariableMap variables = MockProvider.createMockSerializedVariables();
		when(taskServiceMock.completeWithVariablesInReturn(EXAMPLE_TASK_ID, null, false)).thenReturn(variables);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["withVariablesInReturn"] = true;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(List<object>).FullName)).body(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".type", equalTo("Object")).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.objectTypeName", equalTo(typeof(object).FullName)).body(MockProvider.EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME + ".valueInfo.serializationDataFormat", equalTo(MockProvider.FORMAT_APPLICATION_JSON)).when().post(COMPLETE_TASK_URL);

		verify(taskServiceMock).completeWithVariablesInReturn(EXAMPLE_TASK_ID, null, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithUnparseableIntegerVariable()
	  public virtual void testCompleteWithUnparseableIntegerVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithUnparseableShortVariable()
	  public virtual void testCompleteWithUnparseableShortVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithUnparseableLongVariable()
	  public virtual void testCompleteWithUnparseableLongVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithUnparseableDoubleVariable()
	  public virtual void testCompleteWithUnparseableDoubleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithUnparseableDateVariable()
	  public virtual void testCompleteWithUnparseableDateVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithNotSupportedVariableType()
	  public virtual void testCompleteWithNotSupportedVariableType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot complete task anId: Unsupported value type 'X'")).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulCompleteTask()
	  public virtual void testUnsuccessfulCompleteTask()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).complete(any(typeof(string)), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot complete task " + EXAMPLE_TASK_ID + ": expected exception")).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskThrowsAuthorizationException()
	  public virtual void testCompleteTaskThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).complete(anyString(), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(COMPLETE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTask()
	  public virtual void testResolveTask()
	  {
		IDictionary<string, object> variables = VariablesBuilder.create().variable("aVariable", "aStringValue").variable("anotherVariable", 42).variable("aThirdValue", true).Variables;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(RESOLVE_TASK_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["aVariable"] = "aStringValue";
		expectedVariables["anotherVariable"] = 42;
		expectedVariables["aThirdValue"] = true;

		verify(taskServiceMock).resolveTask(eq(EXAMPLE_TASK_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithUnparseableIntegerVariable()
	  public virtual void testResolveTaskWithUnparseableIntegerVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithUnparseableShortVariable()
	  public virtual void testResolveTaskWithUnparseableShortVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithUnparseableLongVariable()
	  public virtual void testResolveTaskWithUnparseableLongVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithUnparseableDoubleVariable()
	  public virtual void testResolveTaskWithUnparseableDoubleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithUnparseableDateVariable()
	  public virtual void testResolveTaskWithUnparseableDateVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithNotSupportedVariableType()
	  public virtual void testResolveTaskWithNotSupportedVariableType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["variables"] = variableJson;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(variables).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot resolve task anId: Unsupported value type 'X'")).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskThrowsAuthorizationException()
	  public virtual void testResolveTaskThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).resolveTask(anyString(), Matchers.any<IDictionary<string, object>>());

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulResolving()
	  public virtual void testUnsuccessfulResolving()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).resolveTask(any(typeof(string)), any(typeof(System.Collections.IDictionary)));

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().post(RESOLVE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingTask()
	  public virtual void testGetNonExistingTask()
	  {
		when(mockQuery.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No matching task with id " + NON_EXISTING_ID)).when().get(SINGLE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingForm()
	  public virtual void testGetNonExistingForm()
	  {
		when(formServiceMock.getTaskFormData(anyString())).thenThrow(new ProcessEngineException("Expected exception: task does not exist."));

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot get form for task " + NON_EXISTING_ID)).when().get(TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateTask()
	  public virtual void testDelegateTask()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = EXAMPLE_USER_ID;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(DELEGATE_TASK_URL);

		verify(taskServiceMock).delegateTask(EXAMPLE_TASK_ID, EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulDelegateTask()
	  public virtual void testUnsuccessfulDelegateTask()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(taskServiceMock).delegateTask(any(typeof(string)), any(typeof(string)));

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["userId"] = EXAMPLE_USER_ID;

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().post(DELEGATE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateTaskThrowsAuthorizationException()
	  public virtual void testDelegateTaskThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).delegateTask(anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(DELEGATE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskComment()
	  public virtual void testGetSingleTaskComment()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("commentId", EXAMPLE_TASK_COMMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("id", equalTo(EXAMPLE_TASK_COMMENT_ID)).body("taskId", equalTo(EXAMPLE_TASK_ID)).body("userId", equalTo(EXAMPLE_USER_ID)).body("time", equalTo(EXAMPLE_TASK_COMMENT_TIME)).body("message", equalTo(EXAMPLE_TASK_COMMENT_FULL_MESSAGE)).body("removalTime", equalTo(EXAMPLE_TASK_COMMENT_TIME)).body("rootProcessInstanceId", equalTo(EXAMPLE_TASK_COMMENT_ROOT_PROCESS_INSTANCE_ID)).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskCommentWithHistoryDisabled()
	  public virtual void testGetSingleTaskCommentWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("commentId", EXAMPLE_TASK_COMMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskCommentForNonExistingComment()
	  public virtual void testGetSingleTaskCommentForNonExistingComment()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("commentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Task comment with id " + NON_EXISTING_ID + " does not exist for task id '" + EXAMPLE_TASK_ID + "'.")).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskCommentForNonExistingCommentWithHistoryDisabled()
	  public virtual void testGetSingleTaskCommentForNonExistingCommentWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("commentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskCommentForNonExistingTask()
	  public virtual void testGetSingleTaskCommentForNonExistingTask()
	  {
		given().pathParam("id", NON_EXISTING_ID).pathParam("commentId", EXAMPLE_TASK_COMMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Task comment with id " + EXAMPLE_TASK_COMMENT_ID + " does not exist for task id '" + NON_EXISTING_ID + "'")).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskCommentForNonExistingTaskWithHistoryDisabled()
	  public virtual void testGetSingleTaskCommentForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).pathParam("commentId", EXAMPLE_TASK_COMMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskComments()
	  public virtual void testGetTaskComments()
	  {
		Response response = given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(1)).when().get(SINGLE_TASK_COMMENTS_URL);

		verifyTaskComments(mockTaskComments, response);
		verify(taskServiceMock).getTaskComments(EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskCommentsWithHistoryDisabled()
	  public virtual void testGetTaskCommentsWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_COMMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskNonExistingComments()
	  public virtual void testGetTaskNonExistingComments()
	  {
		when(taskServiceMock.getTaskComments(EXAMPLE_TASK_ID)).thenReturn(System.Linq.Enumerable.Empty<Comment>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_COMMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskNonExistingCommentsWithHistoryDisabled()
	  public virtual void testGetTaskNonExistingCommentsWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_COMMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskCommentsForNonExistingTask()
	  public virtual void testGetTaskCommentsForNonExistingTask()
	  {
		when(historicTaskInstanceQueryMock.taskId(NON_EXISTING_ID)).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body(containsString("No task found for task id " + NON_EXISTING_ID)).when().get(SINGLE_TASK_COMMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskCommentsForNonExistingTaskWithHistoryDisabled()
	  public virtual void testGetTaskCommentsForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_COMMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCompleteTaskComment()
	  public virtual void testAddCompleteTaskComment()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["message"] = EXAMPLE_TASK_COMMENT_FULL_MESSAGE;

		Response response = given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().post(SINGLE_TASK_ADD_COMMENT_URL);

		verify(taskServiceMock).createComment(EXAMPLE_TASK_ID, null, EXAMPLE_TASK_COMMENT_FULL_MESSAGE);

		verifyCreatedTaskComment(mockTaskComment, response);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCompleteTaskCommentWithHistoryDisabled()
	  public virtual void testAddCompleteTaskCommentWithHistoryDisabled()
	  {

		mockHistoryDisabled();

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["message"] = EXAMPLE_TASK_COMMENT_FULL_MESSAGE;

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().post(SINGLE_TASK_ADD_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCommentToNonExistingTask()
	  public virtual void testAddCommentToNonExistingTask()
	  {
		when(historicTaskInstanceQueryMock.taskId(eq(NON_EXISTING_ID))).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(null);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["message"] = EXAMPLE_TASK_COMMENT_FULL_MESSAGE;

		given().pathParam("id", NON_EXISTING_ID).contentType(ContentType.JSON).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("No task found for task id " + NON_EXISTING_ID)).when().post(SINGLE_TASK_ADD_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCommentToNonExistingTaskWithHistoryDisabled()
	  public virtual void testAddCommentToNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["message"] = EXAMPLE_TASK_COMMENT_FULL_MESSAGE;

		given().pathParam("id", NON_EXISTING_ID).contentType(ContentType.JSON).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().post(SINGLE_TASK_ADD_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTaskCommentWithoutBody()
	  public virtual void testAddTaskCommentWithoutBody()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.UNSUPPORTED_MEDIA_TYPE.StatusCode).when().post(SINGLE_TASK_ADD_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTaskCommentWithoutMessage()
	  public virtual void testAddTaskCommentWithoutMessage()
	  {

		doThrow(new ProcessEngineException("Message is null")).when(taskServiceMock).createComment(EXAMPLE_TASK_ID, null, null);

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Not enough parameters submitted")).when().post(SINGLE_TASK_ADD_COMMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachment()
	  public virtual void testGetSingleTaskAttachment()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("attachmentId", MockProvider.EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("id", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_ID)).body("taskId", equalTo(MockProvider.EXAMPLE_TASK_ID)).body("description", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_DESCRIPTION)).body("type", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_TYPE)).body("name", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_NAME)).body("url", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_URL)).body("createTime", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_CREATE_DATE)).body("removalTime", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_REMOVAL_DATE)).body("rootProcessInstanceId", equalTo(MockProvider.EXAMPLE_TASK_ATTACHMENT_ROOT_PROCESS_INSTANCE_ID)).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentForNonExistingAttachmentId()
	  public virtual void testGetSingleTaskAttachmentForNonExistingAttachmentId()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body(containsString("Task attachment with id " + NON_EXISTING_ID + " does not exist for task id '" + EXAMPLE_TASK_ID + "'.")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentForNonExistingAttachmentIdWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentForNonExistingAttachmentIdWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentForNonExistingTask()
	  public virtual void testGetSingleTaskAttachmentForNonExistingTask()
	  {
		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Task attachment with id " + EXAMPLE_TASK_ATTACHMENT_ID + " does not exist for task id '" + NON_EXISTING_ID + "'")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentForNonExistingTaskWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachments()
	  public virtual void testGetTaskAttachments()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(1)).when().get(SINGLE_TASK_ATTACHMENTS_URL);

		verifyTaskAttachments(mockTaskAttachments, response);
		verify(taskServiceMock).getTaskAttachments(MockProvider.EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsWithHistoryDisabled()
	  public virtual void testGetTaskAttachmentsWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_ATTACHMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsForNonExistingTaskId()
	  public virtual void testGetTaskAttachmentsForNonExistingTaskId()
	  {
		when(historicTaskInstanceQueryMock.taskId(NON_EXISTING_ID)).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body(containsString("No task found for task id " + NON_EXISTING_ID)).when().get(SINGLE_TASK_ATTACHMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsForNonExistingTaskWithHistoryDisabled()
	  public virtual void testGetTaskAttachmentsForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_ATTACHMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsForNonExistingAttachments()
	  public virtual void testGetTaskAttachmentsForNonExistingAttachments()
	  {
		when(taskServiceMock.getTaskAttachments(EXAMPLE_TASK_ID)).thenReturn(System.Linq.Enumerable.Empty<Attachment>());

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_ATTACHMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsForNonExistingAttachmentsWithHistoryDisabled()
	  public virtual void testGetTaskAttachmentsForNonExistingAttachmentsWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("$.size()", equalTo(0)).when().get(SINGLE_TASK_ATTACHMENTS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteTaskAttachmentWithContent()
	  public virtual void testCreateCompleteTaskAttachmentWithContent()
	  {
		Response response = given().pathParam("id", EXAMPLE_TASK_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("content", createMockByteData()).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);

		verifyCreatedTaskAttachment(mockTaskAttachment, response, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskAttachmentWithContentToNonExistingTask()
	  public virtual void testCreateTaskAttachmentWithContentToNonExistingTask()
	  {
		when(historicTaskInstanceQueryMock.taskId(eq(NON_EXISTING_ID))).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("content", createMockByteData()).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("No task found for task id " + NON_EXISTING_ID)).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteTaskAttachmentWithUrl()
	  public virtual void testCreateCompleteTaskAttachmentWithUrl()
	  {
		Response response = given().pathParam("id", EXAMPLE_TASK_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("url", EXAMPLE_TASK_ATTACHMENT_URL).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);

		verifyCreatedTaskAttachment(mockTaskAttachment, response, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateCompleteTaskAttachmentWithUrlWithHistoryDisabled()
	  public virtual void testCreateCompleteTaskAttachmentWithUrlWithHistoryDisabled()
	  {

		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("url", EXAMPLE_TASK_ATTACHMENT_URL).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskAttachmentWithUrlToNonExistingTask()
	  public virtual void testCreateTaskAttachmentWithUrlToNonExistingTask()
	  {
		when(historicTaskInstanceQueryMock.taskId(eq(NON_EXISTING_ID))).thenReturn(historicTaskInstanceQueryMock);
		when(historicTaskInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", NON_EXISTING_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("url", EXAMPLE_TASK_ATTACHMENT_URL).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("No task found for task id " + NON_EXISTING_ID)).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskAttachmentWithUrlToNonExistingTaskWithHistoryDisabled()
	  public virtual void testCreateTaskAttachmentWithUrlToNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).multiPart("attachment-name", EXAMPLE_TASK_ATTACHMENT_NAME).multiPart("attachment-description", EXAMPLE_TASK_ATTACHMENT_DESCRIPTION).multiPart("attachment-type", EXAMPLE_TASK_ATTACHMENT_TYPE).multiPart("url", EXAMPLE_TASK_ATTACHMENT_URL).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTaskAttachmentWithoutMultiparts()
	  public virtual void testCreateTaskAttachmentWithoutMultiparts()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.UNSUPPORTED_MEDIA_TYPE.StatusCode).when().post(SINGLE_TASK_ADD_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContent()
	  public virtual void testGetSingleTaskAttachmentContent()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("attachmentId", MockProvider.EXAMPLE_TASK_ATTACHMENT_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);

		sbyte[] responseContent = IoUtil.readInputStream(response.asInputStream(), "attachmentContent");
		assertEquals("someContent", StringHelper.NewString(responseContent));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContentWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentContentWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContentForNonExistingAttachmentId()
	  public virtual void testGetSingleTaskAttachmentContentForNonExistingAttachmentId()
	  {
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Attachment content for attachment with id '" + NON_EXISTING_ID + "' does not exist for task id '" + EXAMPLE_TASK_ID + "'.")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContentForNonExistingAttachmentIdWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentContentForNonExistingAttachmentIdWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContentForNonExistingTask()
	  public virtual void testGetSingleTaskAttachmentContentForNonExistingTask()
	  {
		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Attachment content for attachment with id '" + EXAMPLE_TASK_ATTACHMENT_ID + "' does not exist for task id '" + NON_EXISTING_ID + "'.")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleTaskAttachmentContentForNonExistingTaskWithHistoryDisabled()
	  public virtual void testGetSingleTaskAttachmentContentForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("History is not enabled")).when().get(SINGLE_TASK_SINGLE_ATTACHMENT_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachment()
	  public virtual void testDeleteSingleTaskAttachment()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("attachmentId", MockProvider.EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachmentWithHistoryDisabled()
	  public virtual void testDeleteSingleTaskAttachmentWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachmentForNonExistingAttachmentId()
	  public virtual void testDeleteSingleTaskAttachmentForNonExistingAttachmentId()
	  {
		doThrow(new ProcessEngineException()).when(taskServiceMock).deleteTaskAttachment(EXAMPLE_TASK_ID, NON_EXISTING_ID);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body(containsString("Deletion is not possible. No attachment exists for task id '" + EXAMPLE_TASK_ID + "' and attachment id '" + NON_EXISTING_ID + "'.")).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachmentForNonExistingAttachmentIdWithHistoryDisabled()
	  public virtual void testDeleteSingleTaskAttachmentForNonExistingAttachmentIdWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("attachmentId", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachmentForNonExistingTask()
	  public virtual void testDeleteSingleTaskAttachmentForNonExistingTask()
	  {
		doThrow(new ProcessEngineException()).when(taskServiceMock).deleteTaskAttachment(NON_EXISTING_ID, EXAMPLE_TASK_ATTACHMENT_ID);

		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Deletion is not possible. No attachment exists for task id '" + NON_EXISTING_ID + "' and attachment id '" + EXAMPLE_TASK_ATTACHMENT_ID + "'.")).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleTaskAttachmentForNonExistingTaskWithHistoryDisabled()
	  public virtual void testDeleteSingleTaskAttachmentForNonExistingTaskWithHistoryDisabled()
	  {
		mockHistoryDisabled();

		given().pathParam("id", NON_EXISTING_ID).pathParam("attachmentId", EXAMPLE_TASK_ATTACHMENT_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body(containsString("History is not enabled")).when().delete(SINGLE_TASK_DELETE_SINGLE_ATTACHMENT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteDeleteTask()
	  public virtual void testDeleteDeleteTask()
	  {

		given().pathParam("id", EXAMPLE_TASK_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_TASK_URL);

		verify(taskServiceMock).deleteTask(EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTask()
	  public virtual void testPostCreateTask()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["id"] = "anyTaskId";
		json["name"] = "A Task";
		json["description"] = "Some description";
		json["priority"] = 30;
		json["assignee"] = "demo";
		json["owner"] = "mary";
		json["delegationState"] = "PENDING";
		json["due"] = withTimezone("2014-01-01T00:00:00");
		json["followUp"] = withTimezone("2014-01-01T00:00:00");
		json["parentTaskId"] = "aParentTaskId";
		json["caseInstanceId"] = "aCaseInstanceId";
		json["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_CREATE_URL);

		verify(taskServiceMock).newTask((string) json["id"]);
		verify(newTask).Name = (string) json["name"];
		verify(newTask).Description = (string) json["description"];
		verify(newTask).Priority = (int?) json["priority"];
		verify(newTask).Assignee = (string) json["assignee"];
		verify(newTask).Owner = (string) json["owner"];
		verify(newTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(newTask).DueDate = any(typeof(DateTime));
		verify(newTask).FollowUpDate = any(typeof(DateTime));
		verify(newTask).ParentTaskId = (string) json["parentTaskId"];
		verify(newTask).CaseInstanceId = (string) json["caseInstanceId"];
		verify(newTask).TenantId = (string) json["tenantId"];
		verify(taskServiceMock).saveTask(newTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskPartialProperties()
	  public virtual void testPostCreateTaskPartialProperties()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["name"] = "A Task";
		json["description"] = "Some description";
		json["assignee"] = "demo";
		json["owner"] = "mary";
		json["due"] = withTimezone("2014-01-01T00:00:00");
		json["parentTaskId"] = "aParentTaskId";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_CREATE_URL);

		verify(taskServiceMock).newTask(null);
		verify(newTask).Name = (string) json["name"];
		verify(newTask).Description = (string) json["description"];
		verify(newTask).Priority = 0;
		verify(newTask).Assignee = (string) json["assignee"];
		verify(newTask).Owner = (string) json["owner"];
		verify(newTask).DelegationState = null;
		verify(newTask).DueDate = any(typeof(DateTime));
		verify(newTask).FollowUpDate = null;
		verify(newTask).ParentTaskId = (string) json["parentTaskId"];
		verify(newTask).CaseInstanceId = null;
		verify(newTask).TenantId = null;
		verify(taskServiceMock).saveTask(newTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskDelegationStateResolved()
	  public virtual void testPostCreateTaskDelegationStateResolved()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "RESOLVED";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_CREATE_URL);

		verify(taskServiceMock).newTask(null);
		verify(newTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(taskServiceMock).saveTask(newTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskDelegationStatePending()
	  public virtual void testPostCreateTaskDelegationStatePending()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "PENDING";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_CREATE_URL);

		verify(taskServiceMock).newTask(null);
		verify(newTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(taskServiceMock).saveTask(newTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskUnsupportedDelegationState()
	  public virtual void testPostCreateTaskUnsupportedDelegationState()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "unsupported";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Valid values for property 'delegationState' are 'PENDING' or 'RESOLVED', but was 'unsupported'")).when().post(TASK_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskLowercaseDelegationState()
	  public virtual void testPostCreateTaskLowercaseDelegationState()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "pending";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TASK_CREATE_URL);

		verify(taskServiceMock).newTask(null);
		verify(newTask).DelegationState = DelegationState.PENDING;
		verify(taskServiceMock).saveTask(newTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTask_NotValidValueException()
	  public virtual void testPostCreateTask_NotValidValueException()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["id"] = "anyTaskId";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		doThrow(new NotValidException("parent task is null")).when(taskServiceMock).saveTask(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Could not save task: parent task is null")).when().post(TASK_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostCreateTaskThrowsAuthorizationException()
	  public virtual void testPostCreateTaskThrowsAuthorizationException()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["id"] = "anyTaskId";

		string message = "expected exception";
		when(taskServiceMock.newTask(anyString())).thenThrow(new AuthorizationException(message));

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TASK_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveNewTaskThrowsAuthorizationException()
	  public virtual void testSaveNewTaskThrowsAuthorizationException()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["id"] = "anyTaskId";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).saveTask(newTask);

		given().body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TASK_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTask()
	  public virtual void testPutUpdateTask()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["id"] = "anyTaskId";
		json["name"] = "A Task";
		json["description"] = "Some description";
		json["priority"] = 30;
		json["assignee"] = "demo";
		json["owner"] = "mary";
		json["delegationState"] = "PENDING";
		json["due"] = withTimezone("2014-01-01T00:00:00");
		json["followUp"] = withTimezone("2014-01-01T00:00:00");
		json["parentTaskId"] = "aParentTaskId";
		json["caseInstanceId"] = "aCaseInstanceId";
		json["tenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_URL);

		verify(mockTask).Name = (string) json["name"];
		verify(mockTask).Description = (string) json["description"];
		verify(mockTask).Priority = (int?) json["priority"];
		verify(mockTask).Assignee = (string) json["assignee"];
		verify(mockTask).Owner = (string) json["owner"];
		verify(mockTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(mockTask).DueDate = any(typeof(DateTime));
		verify(mockTask).FollowUpDate = any(typeof(DateTime));
		verify(mockTask).ParentTaskId = (string) json["parentTaskId"];
		verify(mockTask).CaseInstanceId = (string) json["caseInstanceId"];
		verify(mockTask).TenantId = (string) json["tenantId"];
		verify(taskServiceMock).saveTask(mockTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskPartialProperties()
	  public virtual void testPutUpdateTaskPartialProperties()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["name"] = "A Task";
		json["description"] = "Some description";
		json["assignee"] = "demo";
		json["owner"] = "mary";
		json["due"] = withTimezone("2014-01-01T00:00:00");
		json["parentTaskId"] = "aParentTaskId";

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_URL);

		verify(mockTask).Name = (string) json["name"];
		verify(mockTask).Description = (string) json["description"];
		verify(mockTask).Priority = 0;
		verify(mockTask).Assignee = (string) json["assignee"];
		verify(mockTask).Owner = (string) json["owner"];
		verify(mockTask).DelegationState = null;
		verify(mockTask).DueDate = any(typeof(DateTime));
		verify(mockTask).FollowUpDate = null;
		verify(mockTask).ParentTaskId = (string) json["parentTaskId"];
		verify(mockTask).CaseInstanceId = null;
		verify(mockTask).TenantId = null;
		verify(taskServiceMock).saveTask(mockTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskNotFound()
	  public virtual void testPutUpdateTaskNotFound()
	  {
		when(mockQuery.singleResult()).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).body(EMPTY_JSON_OBJECT).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("No matching task with id " + EXAMPLE_TASK_ID)).when().put(SINGLE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskDelegationStateResolved()
	  public virtual void testPutUpdateTaskDelegationStateResolved()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "RESOLVED";

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_URL);

		verify(mockTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(taskServiceMock).saveTask(mockTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskDelegationStatePending()
	  public virtual void testPutUpdateTaskDelegationStatePending()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "PENDING";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_URL);

		verify(mockTask).DelegationState = Enum.Parse(typeof(DelegationState), (string) json["delegationState"]);
		verify(taskServiceMock).saveTask(mockTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskUnsupportedDelegationState()
	  public virtual void testPutUpdateTaskUnsupportedDelegationState()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "unsupported";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Valid values for property 'delegationState' are 'PENDING' or 'RESOLVED', but was 'unsupported'")).when().put(SINGLE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskLowercaseDelegationState()
	  public virtual void testPutUpdateTaskLowercaseDelegationState()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["delegationState"] = "pending";

		Task newTask = mock(typeof(Task));
		when(taskServiceMock.newTask(anyString())).thenReturn(newTask);

		given().pathParam("id", EXAMPLE_TASK_ID).body(json).contentType(ContentType.JSON).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_URL);

		verify(mockTask).DelegationState = DelegationState.PENDING;
		verify(taskServiceMock).saveTask(mockTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutUpdateTaskThrowsAuthorizationException()
	  public virtual void testPutUpdateTaskThrowsAuthorizationException()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["delegationState"] = "pending";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).saveTask(any(typeof(Task)));

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskForm()
	  public virtual void testGetDeployedTaskForm()
	  {
		Stream deployedFormMock = new MemoryStream("Test".GetBytes());
		when(formServiceMock.getDeployedTaskForm(anyString())).thenReturn(deployedFormMock);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).then().expect().statusCode(Status.OK.StatusCode).body(equalTo("Test")).when().get(DEPLOYED_TASK_FORM_URL);

		verify(formServiceMock).getDeployedTaskForm(MockProvider.EXAMPLE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskFormWithoutAuthorization()
	  public virtual void testGetDeployedTaskFormWithoutAuthorization()
	  {
		string message = "unauthorized";
		when(formServiceMock.getDeployedTaskForm(anyString())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("message", equalTo(message)).when().get(DEPLOYED_TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskFormWithWrongFormKey()
	  public virtual void testGetDeployedTaskFormWithWrongFormKey()
	  {
		string message = "wrong key format";
		when(formServiceMock.getDeployedTaskForm(anyString())).thenThrow(new BadUserRequestException(message));

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("message", equalTo(message)).when().get(DEPLOYED_TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskFormWithUnexistingForm()
	  public virtual void testGetDeployedTaskFormWithUnexistingForm()
	  {
		string message = "not found";
		when(formServiceMock.getDeployedTaskForm(anyString())).thenThrow(new NotFoundException(message));

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("message",equalTo(message)).when().get(DEPLOYED_TASK_FORM_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) private void verifyTaskComments(java.util.List<org.camunda.bpm.engine.task.Comment> mockTaskComments, io.restassured.response.Response response)
	  private void verifyTaskComments(IList<Comment> mockTaskComments, Response response)
	  {
		System.Collections.IList list = response.@as(typeof(System.Collections.IList));
		assertEquals(1, list.Count);

		LinkedHashMap<string, string> resourceHashMap = (LinkedHashMap<string, string>) list[0];

		string returnedId = resourceHashMap.get("id");
		string returnedUserId = resourceHashMap.get("userId");
		string returnedTaskId = resourceHashMap.get("taskId");
		DateTime returnedTime = DateTimeUtil.parseDate(resourceHashMap.get("time"));
		string returnedFullMessage = resourceHashMap.get("message");

		Comment mockComment = mockTaskComments[0];

		assertEquals(mockComment.Id, returnedId);
		assertEquals(mockComment.TaskId, returnedTaskId);
		assertEquals(mockComment.UserId, returnedUserId);
		assertEquals(mockComment.Time, returnedTime);
		assertEquals(mockComment.FullMessage, returnedFullMessage);
	  }

	  private void verifyCreatedTaskComment(Comment mockTaskComment, Response response)
	  {
		string content = response.asString();
		verifyTaskCommentValues(mockTaskComment, content);
		verifyTaskCommentLink(mockTaskComment, content);
	  }

	  private void verifyTaskCommentValues(Comment mockTaskComment, string responseContent)
	  {
		JsonPath path = from(responseContent);
		string returnedId = path.get("id");
		string returnedUserId = path.get("userId");
		string returnedTaskId = path.get("taskId");
		DateTime returnedTime = DateTimeUtil.parseDate(path.get<string>("time"));
		string returnedFullMessage = path.get("message");

		assertEquals(mockTaskComment.Id, returnedId);
		assertEquals(mockTaskComment.TaskId, returnedTaskId);
		assertEquals(mockTaskComment.UserId, returnedUserId);
		assertEquals(mockTaskComment.Time, returnedTime);
		assertEquals(mockTaskComment.FullMessage, returnedFullMessage);
	  }

	  private void verifyTaskCommentLink(Comment mockTaskComment, string responseContent)
	  {
		IList<IDictionary<string, string>> returnedLinks = from(responseContent).getList("links");
		assertEquals(1, returnedLinks.Count);

		IDictionary<string, string> returnedLink = returnedLinks[0];
		assertEquals(HttpMethod.GET, returnedLink["method"]);
		assertTrue(returnedLink["href"].EndsWith(SINGLE_TASK_COMMENTS_URL.Replace("{id}", mockTaskComment.TaskId) + "/" + mockTaskComment.Id, StringComparison.Ordinal));
		assertEquals("self", returnedLink["rel"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) private void verifyTaskAttachments(java.util.List<org.camunda.bpm.engine.task.Attachment> mockTaskAttachments, io.restassured.response.Response response)
	  private void verifyTaskAttachments(IList<Attachment> mockTaskAttachments, Response response)
	  {
		System.Collections.IList list = response.@as(typeof(System.Collections.IList));
		assertEquals(1, list.Count);

		LinkedHashMap<string, string> resourceHashMap = (LinkedHashMap<string, string>) list[0];

		string returnedId = resourceHashMap.get("id");
		string returnedTaskId = resourceHashMap.get("taskId");
		string returnedName = resourceHashMap.get("name");
		string returnedType = resourceHashMap.get("type");
		string returnedDescription = resourceHashMap.get("description");
		string returnedUrl = resourceHashMap.get("url");

		Attachment mockAttachment = mockTaskAttachments[0];

		assertEquals(mockAttachment.Id, returnedId);
		assertEquals(mockAttachment.TaskId, returnedTaskId);
		assertEquals(mockAttachment.Name, returnedName);
		assertEquals(mockAttachment.Type, returnedType);
		assertEquals(mockAttachment.Description, returnedDescription);
		assertEquals(mockAttachment.Url, returnedUrl);
	  }

	  private void verifyCreatedTaskAttachment(Attachment mockTaskAttachment, Response response, bool urlExist)
	  {
		string content = response.asString();
		verifyTaskAttachmentValues(mockTaskAttachment, content, urlExist);
		verifyTaskAttachmentLink(mockTaskAttachment, content);
	  }

	  private void verifyTaskAttachmentValues(Attachment mockTaskAttachment, string responseContent, bool urlExist)
	  {
		JsonPath path = from(responseContent);
		string returnedId = path.get("id");
		string returnedTaskId = path.get("taskId");
		string returnedName = path.get("name");
		string returnedType = path.get("type");
		string returnedDescription = path.get("description");
		string returnedUrl = path.get("url");

		Attachment mockAttachment = mockTaskAttachments[0];

		assertEquals(mockAttachment.Id, returnedId);
		assertEquals(mockAttachment.TaskId, returnedTaskId);
		assertEquals(mockAttachment.Name, returnedName);
		assertEquals(mockAttachment.Type, returnedType);
		assertEquals(mockAttachment.Description, returnedDescription);
		if (urlExist)
		{
		  assertEquals(mockAttachment.Url, returnedUrl);
		}
	  }

	  private void verifyTaskAttachmentLink(Attachment mockTaskAttachment, string responseContent)
	  {
		IList<IDictionary<string, string>> returnedLinks = from(responseContent).getList("links");
		assertEquals(1, returnedLinks.Count);

		IDictionary<string, string> returnedLink = returnedLinks[0];
		assertEquals(HttpMethod.GET, returnedLink["method"]);
		assertTrue(returnedLink["href"].EndsWith(SINGLE_TASK_ATTACHMENTS_URL.Replace("{id}", mockTaskAttachment.TaskId) + "/" + mockTaskAttachment.Id, StringComparison.Ordinal));
		assertEquals("self", returnedLink["rel"]);
	  }

	}

}