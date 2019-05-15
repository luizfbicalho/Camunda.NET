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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyZeroInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;

	using ContentType = io.restassured.http.ContentType;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;

	public class ProcessEngineRestServiceTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string ENGINES_URL = TEST_RESOURCE_ROOT_PATH + "/engine";
	  protected internal static readonly string SINGLE_ENGINE_URL = ENGINES_URL + "/{name}";
	  protected internal static readonly string PROCESS_DEFINITION_URL = SINGLE_ENGINE_URL + "/process-definition/{id}";
	  protected internal static readonly string PROCESS_INSTANCE_URL = SINGLE_ENGINE_URL + "/process-instance/{id}";
	  protected internal static readonly string TASK_URL = SINGLE_ENGINE_URL + "/task/{id}";
	  protected internal static readonly string IDENTITY_GROUPS_URL = SINGLE_ENGINE_URL + "/identity/groups";
	  protected internal static readonly string MESSAGE_URL = SINGLE_ENGINE_URL + MessageRestService_Fields.PATH;

	  protected internal static readonly string EXECUTION_URL = SINGLE_ENGINE_URL + "/execution";
	  protected internal static readonly string VARIABLE_INSTANCE_URL = SINGLE_ENGINE_URL + "/variable-instance";
	  protected internal static readonly string USER_URL = SINGLE_ENGINE_URL + "/user";
	  protected internal static readonly string GROUP_URL = SINGLE_ENGINE_URL + "/group";
	  protected internal static readonly string INCIDENT_URL = SINGLE_ENGINE_URL + "/incident";
	  protected internal static readonly string AUTHORIZATION_URL = SINGLE_ENGINE_URL + AuthorizationRestService_Fields.PATH;
	  protected internal static readonly string AUTHORIZATION_CHECK_URL = AUTHORIZATION_URL + "/check";
	  protected internal static readonly string DEPLOYMENT_REST_SERVICE_URL = SINGLE_ENGINE_URL + DeploymentRestService_Fields.PATH;
	  protected internal static readonly string DEPLOYMENT_URL = DEPLOYMENT_REST_SERVICE_URL + "/{id}";
	  protected internal static readonly string EXTERNAL_TASKS_URL = SINGLE_ENGINE_URL + "/external-task";

	  protected internal static readonly string JOB_DEFINITION_URL = SINGLE_ENGINE_URL + "/job-definition";

	  protected internal static readonly string CASE_DEFINITION_URL = SINGLE_ENGINE_URL + "/case-definition";
	  protected internal static readonly string CASE_INSTANCE_URL = SINGLE_ENGINE_URL + "/case-instance";
	  protected internal static readonly string CASE_EXECUTION_URL = SINGLE_ENGINE_URL + "/case-execution";

	  protected internal static readonly string FILTER_URL = SINGLE_ENGINE_URL + "/filter";

	  protected internal static readonly string HISTORY_URL = SINGLE_ENGINE_URL + "/history";
	  protected internal static readonly string HISTORY_ACTIVITY_INSTANCE_URL = HISTORY_URL + "/activity-instance";
	  protected internal static readonly string HISTORY_PROCESS_INSTANCE_URL = HISTORY_URL + "/process-instance";
	  protected internal static readonly string HISTORY_VARIABLE_INSTANCE_URL = HISTORY_URL + "/variable-instance";
	  protected internal static readonly string HISTORY_BINARY_VARIABLE_INSTANCE_URL = HISTORY_VARIABLE_INSTANCE_URL + "/{id}/data";
	  protected internal static readonly string HISTORY_ACTIVITY_STATISTICS_URL = HISTORY_URL + "/process-definition/{id}/statistics";
	  protected internal static readonly string HISTORY_DETAIL_URL = HISTORY_URL + "/detail";
	  protected internal static readonly string HISTORY_BINARY_DETAIL_URL = HISTORY_DETAIL_URL + "/data";
	  protected internal static readonly string HISTORY_TASK_INSTANCE_URL = HISTORY_URL + "/task";
	  protected internal static readonly string HISTORY_INCIDENT_URL = HISTORY_URL + "/incident";
	  protected internal static readonly string HISTORY_JOB_LOG_URL = HISTORY_URL + "/job-log";
	  protected internal static readonly string HISTORY_EXTERNAL_TASK_LOG_URL = HISTORY_URL + "/external-task-log";

	  protected internal const string EXAMPLE_ENGINE_NAME = "anEngineName";

	  private ProcessEngine namedProcessEngine;
	  private RepositoryService mockRepoService;
	  private RuntimeService mockRuntimeService;
	  private TaskService mockTaskService;
	  private IdentityService mockIdentityService;
	  private ManagementService mockManagementService;
	  private HistoryService mockHistoryService;
	  private ExternalTaskService mockExternalTaskService;
	  private CaseService mockCaseService;
	  private FilterService mockFilterService;
	  private MessageCorrelationBuilder mockMessageCorrelationBuilder;
	  private MessageCorrelationResult mockMessageCorrelationResult;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		namedProcessEngine = getProcessEngine(EXAMPLE_ENGINE_NAME);
		mockRepoService = mock(typeof(RepositoryService));
		mockRuntimeService = mock(typeof(RuntimeService));
		mockTaskService = mock(typeof(TaskService));
		mockIdentityService = mock(typeof(IdentityService));
		mockManagementService = mock(typeof(ManagementService));
		mockHistoryService = mock(typeof(HistoryService));
		mockCaseService = mock(typeof(CaseService));
		mockFilterService = mock(typeof(FilterService));
		mockExternalTaskService = mock(typeof(ExternalTaskService));

		when(namedProcessEngine.RepositoryService).thenReturn(mockRepoService);
		when(namedProcessEngine.RuntimeService).thenReturn(mockRuntimeService);
		when(namedProcessEngine.TaskService).thenReturn(mockTaskService);
		when(namedProcessEngine.IdentityService).thenReturn(mockIdentityService);
		when(namedProcessEngine.ManagementService).thenReturn(mockManagementService);
		when(namedProcessEngine.HistoryService).thenReturn(mockHistoryService);
		when(namedProcessEngine.CaseService).thenReturn(mockCaseService);
		when(namedProcessEngine.FilterService).thenReturn(mockFilterService);
		when(namedProcessEngine.ExternalTaskService).thenReturn(mockExternalTaskService);

		createProcessDefinitionMock();
		createProcessInstanceMock();
		createTaskMock();
		createIdentityMocks();
		createExecutionMock();
		createVariableInstanceMock();
		createJobDefinitionMock();
		createIncidentMock();
		createDeploymentMock();
		createMessageCorrelationBuilderMock();
		createCaseDefinitionMock();
		createCaseInstanceMock();
		createCaseExecutionMock();
		createFilterMock();
		createExternalTaskMock();

		createHistoricActivityInstanceMock();
		createHistoricProcessInstanceMock();
		createHistoricVariableInstanceMock();
		createHistoricActivityStatisticsMock();
		createHistoricDetailMock();
		createHistoricTaskInstanceMock();
		createHistoricIncidentMock();
		createHistoricJobLogMock();
		createHistoricExternalTaskLogMock();
	  }

	  private void createProcessDefinitionMock()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();

		when(mockRepoService.getProcessDefinition(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(mockDefinition);
	  }


	  private void createCaseDefinitionMock()
	  {
		IList<CaseDefinition> caseDefinitions = new List<CaseDefinition>();
		CaseDefinition mockCaseDefinition = MockProvider.createMockCaseDefinition();
		caseDefinitions.Add(mockCaseDefinition);

		CaseDefinitionQuery mockCaseDefinitionQuery = mock(typeof(CaseDefinitionQuery));
		when(mockCaseDefinitionQuery.list()).thenReturn(caseDefinitions);
		when(mockRepoService.createCaseDefinitionQuery()).thenReturn(mockCaseDefinitionQuery);
	  }

	  private void createCaseInstanceMock()
	  {
		IList<CaseInstance> caseInstances = new List<CaseInstance>();
		CaseInstance mockCaseInstance = MockProvider.createMockCaseInstance();
		caseInstances.Add(mockCaseInstance);

		CaseInstanceQuery mockCaseInstanceQuery = mock(typeof(CaseInstanceQuery));
		when(mockCaseInstanceQuery.list()).thenReturn(caseInstances);
		when(mockCaseService.createCaseInstanceQuery()).thenReturn(mockCaseInstanceQuery);
	  }

	  private void createCaseExecutionMock()
	  {
		IList<CaseExecution> caseExecutions = new List<CaseExecution>();
		CaseExecution mockCaseExecution = MockProvider.createMockCaseExecution();
		caseExecutions.Add(mockCaseExecution);

		CaseExecutionQuery mockCaseExecutionQuery = mock(typeof(CaseExecutionQuery));
		when(mockCaseExecutionQuery.list()).thenReturn(caseExecutions);
		when(mockCaseService.createCaseExecutionQuery()).thenReturn(mockCaseExecutionQuery);
	  }

	  private void createProcessInstanceMock()
	  {
		ProcessInstance mockInstance = MockProvider.createMockInstance();

		ProcessInstanceQuery mockInstanceQuery = mock(typeof(ProcessInstanceQuery));
		when(mockInstanceQuery.processInstanceId(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID))).thenReturn(mockInstanceQuery);
		when(mockInstanceQuery.singleResult()).thenReturn(mockInstance);
		when(mockRuntimeService.createProcessInstanceQuery()).thenReturn(mockInstanceQuery);
	  }

	  private void createExecutionMock()
	  {
		Execution mockExecution = MockProvider.createMockExecution();

		ExecutionQuery mockExecutionQuery = mock(typeof(ExecutionQuery));
		when(mockExecutionQuery.processInstanceId(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID))).thenReturn(mockExecutionQuery);
		when(mockExecutionQuery.singleResult()).thenReturn(mockExecution);
		when(mockRuntimeService.createExecutionQuery()).thenReturn(mockExecutionQuery);
	  }

	  private void createTaskMock()
	  {
		Task mockTask = MockProvider.createMockTask();

		TaskQuery mockTaskQuery = mock(typeof(TaskQuery));
		when(mockTaskQuery.taskId(eq(MockProvider.EXAMPLE_TASK_ID))).thenReturn(mockTaskQuery);
		when(mockTaskQuery.initializeFormKeys()).thenReturn(mockTaskQuery);
		when(mockTaskQuery.singleResult()).thenReturn(mockTask);
		when(mockTaskService.createTaskQuery()).thenReturn(mockTaskQuery);
	  }

	  private void createIdentityMocks()
	  {
		// identity
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		IList<User> mockUsers = new List<User>();
		User mockUser = MockProvider.createMockUser();
		mockUsers.Add(mockUser);

		// user query
		when(sampleUserQuery.memberOfGroup(anyString())).thenReturn(sampleUserQuery);
		when(sampleUserQuery.list()).thenReturn(mockUsers);

		// group query
		IList<Group> mockGroups = MockProvider.createMockGroups();
		when(sampleGroupQuery.groupMember(anyString())).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.orderByGroupName()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.asc()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.list()).thenReturn(mockGroups);

		when(mockIdentityService.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(mockIdentityService.createUserQuery()).thenReturn(sampleUserQuery);
	  }

	  private void createVariableInstanceMock()
	  {
		IList<VariableInstance> variables = new List<VariableInstance>();
		VariableInstance mockInstance = MockProvider.createMockVariableInstance();
		variables.Add(mockInstance);

		VariableInstanceQuery mockVariableInstanceQuery = mock(typeof(VariableInstanceQuery));
		when(mockVariableInstanceQuery.list()).thenReturn(variables);
		when(mockRuntimeService.createVariableInstanceQuery()).thenReturn(mockVariableInstanceQuery);
	  }

	  private void createJobDefinitionMock()
	  {
		IList<JobDefinition> jobDefinitions = new List<JobDefinition>();
		JobDefinition mockJobDefinition = MockProvider.createMockJobDefinition();
		jobDefinitions.Add(mockJobDefinition);

		JobDefinitionQuery mockJobDefinitionQuery = mock(typeof(JobDefinitionQuery));
		when(mockJobDefinitionQuery.list()).thenReturn(jobDefinitions);
		when(mockManagementService.createJobDefinitionQuery()).thenReturn(mockJobDefinitionQuery);
	  }

	  private void createIncidentMock()
	  {
		IncidentQuery mockIncidentQuery = mock(typeof(IncidentQuery));
		IList<Incident> incidents = MockProvider.createMockIncidents();
		when(mockIncidentQuery.list()).thenReturn(incidents);
		when(mockRuntimeService.createIncidentQuery()).thenReturn(mockIncidentQuery);
	  }

	  private void createMessageCorrelationBuilderMock()
	  {
		mockMessageCorrelationBuilder = mock(typeof(MessageCorrelationBuilder));
		mockMessageCorrelationResult = mock(typeof(MessageCorrelationResult));

		when(mockRuntimeService.createMessageCorrelation(anyString())).thenReturn(mockMessageCorrelationBuilder);
		when(mockMessageCorrelationBuilder.correlateWithResult()).thenReturn(mockMessageCorrelationResult);
		when(mockMessageCorrelationBuilder.processInstanceId(anyString())).thenReturn(mockMessageCorrelationBuilder);
		when(mockMessageCorrelationBuilder.processInstanceBusinessKey(anyString())).thenReturn(mockMessageCorrelationBuilder);
		when(mockMessageCorrelationBuilder.processInstanceVariableEquals(anyString(), any())).thenReturn(mockMessageCorrelationBuilder);
		when(mockMessageCorrelationBuilder.setVariables(Matchers.any<IDictionary<string, object>>())).thenReturn(mockMessageCorrelationBuilder);
		when(mockMessageCorrelationBuilder.setVariable(anyString(), any())).thenReturn(mockMessageCorrelationBuilder);

	  }

	  private void createHistoricActivityInstanceMock()
	  {
		IList<HistoricActivityInstance> activities = new List<HistoricActivityInstance>();
		HistoricActivityInstance mockInstance = MockProvider.createMockHistoricActivityInstance();
		activities.Add(mockInstance);

		HistoricActivityInstanceQuery mockHistoricActivityInstanceQuery = mock(typeof(HistoricActivityInstanceQuery));
		when(mockHistoricActivityInstanceQuery.list()).thenReturn(activities);
		when(mockHistoryService.createHistoricActivityInstanceQuery()).thenReturn(mockHistoricActivityInstanceQuery);
	  }

	  private void createHistoricProcessInstanceMock()
	  {
		IList<HistoricProcessInstance> processes = new List<HistoricProcessInstance>();
		HistoricProcessInstance mockInstance = MockProvider.createMockHistoricProcessInstance();
		processes.Add(mockInstance);

		HistoricProcessInstanceQuery mockHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(mockHistoricProcessInstanceQuery.list()).thenReturn(processes);
		when(mockHistoryService.createHistoricProcessInstanceQuery()).thenReturn(mockHistoricProcessInstanceQuery);
	  }

	  private void createHistoricVariableInstanceMock()
	  {
		IList<HistoricVariableInstance> variables = new List<HistoricVariableInstance>();
		HistoricVariableInstance mockInstance = MockProvider.createMockHistoricVariableInstance();
		variables.Add(mockInstance);

		HistoricVariableInstanceQuery mockHistoricVariableInstanceQuery = mock(typeof(HistoricVariableInstanceQuery));
		when(mockHistoricVariableInstanceQuery.list()).thenReturn(variables);
		when(mockHistoryService.createHistoricVariableInstanceQuery()).thenReturn(mockHistoricVariableInstanceQuery);
	  }

	  private void createHistoricActivityStatisticsMock()
	  {
		IList<HistoricActivityStatistics> statistics = MockProvider.createMockHistoricActivityStatistics();

		HistoricActivityStatisticsQuery query = mock(typeof(HistoricActivityStatisticsQuery));
		when(mockHistoryService.createHistoricActivityStatisticsQuery(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(query);
		when(query.list()).thenReturn(statistics);
	  }

	  private void createHistoricDetailMock()
	  {
		IList<HistoricDetail> details = MockProvider.createMockHistoricDetails();

		HistoricDetailQuery query = mock(typeof(HistoricDetailQuery));
		when(mockHistoryService.createHistoricDetailQuery()).thenReturn(query);
		when(query.list()).thenReturn(details);
	  }

	  private void createHistoricTaskInstanceMock()
	  {
		IList<HistoricTaskInstance> tasks = MockProvider.createMockHistoricTaskInstances();

		HistoricTaskInstanceQuery query = mock(typeof(HistoricTaskInstanceQuery));
		when(mockHistoryService.createHistoricTaskInstanceQuery()).thenReturn(query);
		when(query.list()).thenReturn(tasks);
	  }

	  private void createHistoricIncidentMock()
	  {
		HistoricIncidentQuery mockHistoricIncidentQuery = mock(typeof(HistoricIncidentQuery));
		IList<HistoricIncident> historicIncidents = MockProvider.createMockHistoricIncidents();
		when(mockHistoricIncidentQuery.list()).thenReturn(historicIncidents);
		when(mockHistoryService.createHistoricIncidentQuery()).thenReturn(mockHistoricIncidentQuery);
	  }

	  private void createDeploymentMock()
	  {
		Deployment mockDeployment = MockProvider.createMockDeployment();

		DeploymentQuery deploymentQueryMock = mock(typeof(DeploymentQuery));
		when(deploymentQueryMock.deploymentId(anyString())).thenReturn(deploymentQueryMock);
		when(deploymentQueryMock.singleResult()).thenReturn(mockDeployment);

		when(mockRepoService.createDeploymentQuery()).thenReturn(deploymentQueryMock);
	  }

	  private void createFilterMock()
	  {
		IList<Filter> filters = new List<Filter>();
		Filter mockFilter = MockProvider.createMockFilter();
		filters.Add(mockFilter);

		FilterQuery mockFilterQuery = mock(typeof(FilterQuery));
		when(mockFilterQuery.list()).thenReturn(filters);
		when(mockFilterService.createFilterQuery()).thenReturn(mockFilterQuery);
	  }

	  private void createHistoricJobLogMock()
	  {
		HistoricJobLogQuery mockHistoricJobLogQuery = mock(typeof(HistoricJobLogQuery));
		IList<HistoricJobLog> historicJobLogs = MockProvider.createMockHistoricJobLogs();
		when(mockHistoricJobLogQuery.list()).thenReturn(historicJobLogs);
		when(mockHistoryService.createHistoricJobLogQuery()).thenReturn(mockHistoricJobLogQuery);
	  }

	  private void createExternalTaskMock()
	  {
		ExternalTaskQuery query = mock(typeof(ExternalTaskQuery));
		IList<ExternalTask> tasks = MockProvider.createMockExternalTasks();
		when(query.list()).thenReturn(tasks);
		when(mockExternalTaskService.createExternalTaskQuery()).thenReturn(query);
	  }

	  private void createHistoricExternalTaskLogMock()
	  {
		HistoricExternalTaskLogQuery mockHistoricExternalTaskLogQuery = mock(typeof(HistoricExternalTaskLogQuery));
		IList<HistoricExternalTaskLog> historicExternalTaskLogs = MockProvider.createMockHistoricExternalTaskLogs();
		when(mockHistoricExternalTaskLogQuery.list()).thenReturn(historicExternalTaskLogs);
		when(mockHistoryService.createHistoricExternalTaskLogQuery()).thenReturn(mockHistoricExternalTaskLogQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingEngineAccess()
	  public virtual void testNonExistingEngineAccess()
	  {
		given().pathParam("name", MockProvider.NON_EXISTING_PROCESS_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No process engine available")).when().get(PROCESS_DEFINITION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEngineNamesList()
	  public virtual void testEngineNamesList()
	  {
		expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("name", hasItems(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME, MockProvider.ANOTHER_EXAMPLE_PROCESS_ENGINE_NAME)).when().get(ENGINES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionServiceEngineAccess()
	  public virtual void testProcessDefinitionServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_URL);

		verify(mockRepoService).getProcessDefinition(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceServiceEngineAccess()
	  public virtual void testProcessInstanceServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_URL);

		verify(mockRuntimeService).createProcessInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskServiceEngineAccess()
	  public virtual void testTaskServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_URL);

		verify(mockTaskService).createTaskQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIdentityServiceEngineAccess()
	  public virtual void testIdentityServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).queryParam("userId", "someId").then().expect().statusCode(Status.OK.StatusCode).when().get(IDENTITY_GROUPS_URL);

		verify(mockIdentityService).createUserQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageServiceEngineAccess()
	  public virtual void testMessageServiceEngineAccess()
	  {
		string messageName = "aMessage";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(MESSAGE_URL);

		verify(mockRuntimeService).createMessageCorrelation(eq(messageName));
		verify(mockMessageCorrelationBuilder).correlateWithResult();
		verifyNoMoreInteractions(mockMessageCorrelationBuilder);
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageWithResultServiceEngineAccess()
	  public virtual void testMessageWithResultServiceEngineAccess()
	  {
		string messageName = "aMessage";
		IDictionary<string, object> messageParameters = new Dictionary<string, object>();
		messageParameters["messageName"] = messageName;
		messageParameters["resultEnabled"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(messageParameters).pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().contentType(ContentType.JSON).statusCode(Status.OK.StatusCode).when().post(MESSAGE_URL);

		verify(mockRuntimeService).createMessageCorrelation(eq(messageName));
		verify(mockMessageCorrelationBuilder).correlateWithResult();
		verifyNoMoreInteractions(mockMessageCorrelationBuilder);
		verifyZeroInteractions(processEngine);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutionServiceEngineAccess()
	  public virtual void testExecutionServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_URL);

		verify(mockRuntimeService).createExecutionQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableInstanceServiceEngineAccess()
	  public virtual void testVariableInstanceServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_URL);

		verify(mockRuntimeService).createVariableInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserServiceEngineAccess()
	  public virtual void testUserServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(USER_URL);

		verify(mockIdentityService).createUserQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupServiceEngineAccess()
	  public virtual void testGroupServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(GROUP_URL);

		verify(mockIdentityService).createGroupQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationServiceEngineAccess()
	  public virtual void testAuthorizationServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(AUTHORIZATION_CHECK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricActivityInstance()
	  public virtual void testHistoryServiceEngineAccess_HistoricActivityInstance()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_ACTIVITY_INSTANCE_URL);

		verify(mockHistoryService).createHistoricActivityInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricProcessInstance()
	  public virtual void testHistoryServiceEngineAccess_HistoricProcessInstance()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_PROCESS_INSTANCE_URL);

		verify(mockHistoryService).createHistoricProcessInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricVariableInstance()
	  public virtual void testHistoryServiceEngineAccess_HistoricVariableInstance()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_VARIABLE_INSTANCE_URL);

		verify(mockHistoryService).createHistoricVariableInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test public void testHistoryServiceEngineAccess_HistoricVariableInstanceBinaryFile()
	  public virtual void testHistoryServiceEngineAccess_HistoricVariableInstanceBinaryFile()
	  {

		HistoricVariableInstanceQuery query = mock(typeof(HistoricVariableInstanceQuery));
		HistoricVariableInstance instance = mock(typeof(HistoricVariableInstance));
		string filename = "test.txt";
		sbyte[] byteContent = "test".GetBytes();
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();
		when(instance.TypedValue).thenReturn(variableValue);
		when(query.singleResult()).thenReturn(instance);
		when(mockHistoryService.createHistoricVariableInstanceQuery()).thenReturn(query);

		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).and().header("Content-Disposition", "attachment; filename=" + filename).contentType(CoreMatchers.either<string>(equalTo(ContentType.TEXT.ToString() + ";charset=UTF-8")).or(equalTo(ContentType.TEXT.ToString() + " ;charset=UTF-8"))).when().get(HISTORY_BINARY_VARIABLE_INSTANCE_URL);

		verify(mockHistoryService).createHistoricVariableInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricActivityStatistics()
	  public virtual void testHistoryServiceEngineAccess_HistoricActivityStatistics()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_ACTIVITY_STATISTICS_URL);

		verify(mockHistoryService).createHistoricActivityStatisticsQuery(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionAccess()
	  public virtual void testJobDefinitionAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_URL);

		verify(mockManagementService).createJobDefinitionQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricDetail()
	  public virtual void testHistoryServiceEngineAccess_HistoricDetail()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_DETAIL_URL);

		verify(mockHistoryService).createHistoricDetailQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test public void testHistoryServiceEngineAccess_HistoricDetailBinaryFile()
	  public virtual void testHistoryServiceEngineAccess_HistoricDetailBinaryFile()
	  {
		HistoricDetailQuery query = mock(typeof(HistoricDetailQuery));
		HistoricVariableUpdate instance = mock(typeof(HistoricVariableUpdate));
		string filename = "test.txt";
		sbyte[] byteContent = "test".GetBytes();
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();
		when(instance.TypedValue).thenReturn(variableValue);
		when(query.singleResult()).thenReturn(instance);
		when(mockHistoryService.createHistoricDetailQuery()).thenReturn(query);

		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).and().header("Content-Disposition", "attachment; filename=" + filename).contentType(CoreMatchers.either<string>(equalTo(ContentType.TEXT.ToString() + ";charset=UTF-8")).or(equalTo(ContentType.TEXT.ToString() + " ;charset=UTF-8"))).when().get(HISTORY_BINARY_DETAIL_URL);

		verify(mockHistoryService).createHistoricDetailQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricTaskInstance()
	  public virtual void testHistoryServiceEngineAccess_HistoricTaskInstance()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_TASK_INSTANCE_URL);

		verify(mockHistoryService).createHistoricTaskInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_Incident()
	  public virtual void testHistoryServiceEngineAccess_Incident()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_URL);

		verify(mockRuntimeService).createIncidentQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricIncident()
	  public virtual void testHistoryServiceEngineAccess_HistoricIncident()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_INCIDENT_URL);

		verify(mockHistoryService).createHistoricIncidentQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentRestServiceEngineAccess()
	  public virtual void testDeploymentRestServiceEngineAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).pathParam("id", MockProvider.EXAMPLE_DEPLOYMENT_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_URL);

		verify(mockRepoService).createDeploymentQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDefinitionAccess()
	  public virtual void testCaseDefinitionAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_DEFINITION_URL);

		verify(mockRepoService).createCaseDefinitionQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceAccess()
	  public virtual void testCaseInstanceAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_URL);

		verify(mockCaseService).createCaseInstanceQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseExecutionAccess()
	  public virtual void testCaseExecutionAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_EXECUTION_URL);

		verify(mockCaseService).createCaseExecutionQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterAccess()
	  public virtual void testFilterAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(FILTER_URL);

		verify(mockFilterService).createFilterQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricJobLog()
	  public virtual void testHistoryServiceEngineAccess_HistoricJobLog()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_JOB_LOG_URL);

		verify(mockHistoryService).createHistoricJobLogQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExternalTaskAccess()
	  public virtual void testExternalTaskAccess()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASKS_URL);

		verify(mockExternalTaskService).createExternalTaskQuery();
		verifyZeroInteractions(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryServiceEngineAccess_HistoricExternalTaskLog()
	  public virtual void testHistoryServiceEngineAccess_HistoricExternalTaskLog()
	  {
		given().pathParam("name", EXAMPLE_ENGINE_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_EXTERNAL_TASK_LOG_URL);

		verify(mockHistoryService).createHistoricExternalTaskLogQuery();
		verifyZeroInteractions(processEngine);
	  }
	}

}