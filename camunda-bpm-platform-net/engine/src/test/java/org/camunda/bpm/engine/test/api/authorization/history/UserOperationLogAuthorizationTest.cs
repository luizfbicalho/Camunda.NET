using System;
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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.OPERATION_LOG_CATEGORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.UserOperationLogCategoryPermissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.UserOperationLogCategoryPermissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER;

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Ignore = org.junit.Ignore;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class UserOperationLogAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
	  protected internal const string ONE_TASK_CASE_KEY = "oneTaskCase";
	  protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // standalone task ///////////////////////////////

	  public virtual void testQueryCreateStandaloneTaskUserOperationLogWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryCreateStandaloneTaskUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  // CAM-9888
	  public virtual void failing_testQueryCreateStandaloneTaskUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryCreateStandaloneTaskUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryCreateStandaloneTaskUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryCreateStandaloneTaskUserOperationLogWithReadPermissionOnAnyCategoryAndRevokeReadHistoryOnProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);
		createRevokeAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1); // "revoke specific process definition" has no effect since task log is not related to a definition

		deleteTask(taskId, true);
	  }

	  // CAM-9888
	  public virtual void failing_testQueryCreateStandaloneTaskUserOperationLogWithReadPermissionOnAnyCategoryAndRevokeReadHistoryOnAnyProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);
		createRevokeAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		// "grant all categories" should preceed over "revoke all process definitions" 
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLogWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLogWithReadPermissionOnProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  // CAM-9888
	  public virtual void failing_testQuerySetAssigneeStandaloneTaskUserOperationLogWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		deleteTask(taskId, true);
	  }

	  public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		deleteTask(taskId, true);
	  }

	  // (process) user task /////////////////////////////

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithMultiple()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadPermissionOnAnyCategoryAndRevokeOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);
		createRevokeAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0); // "revoke process definition" wins over "grant all categories" since task log is related to the definition
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadPermissionOnAnyCategoryAndRevokeOnUnrelatedProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);
		createRevokeAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_CASE_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2); // "revoke process definition" has no effect since task log is not related to the definition
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadPermissionOnAnyCategoryAndRevokeOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);
		createRevokeAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0); // "revoke all process definitions" wins over "grant all categories"
	  }

	  // (case) human task /////////////////////////////

	  public virtual void testQuerySetAssigneeHumanTaskUserOperationLogWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetAssigneeHumanTaskUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_CASE_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  // CAM-9888
	  public virtual void failing_testQuerySetAssigneeHumanTaskUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetAssigneeHumanTaskUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQuerySetAssigneeHumanTaskUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // standalone job ///////////////////////////////

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithoutAuthorization()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then only user operation log of non standalone jobs are visible
		verifyQueryResults(query, 1);
		assertEquals(ONE_TASK_PROCESS_KEY, query.singleResult().ProcessDefinitionKey);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("CAM-9888") public void testQuerySetStandaloneJobRetriesUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		disableAuthorization();
		identityService.clearAuthentication();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		identityService.setAuthentication(userId, null);
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then only non-stadalone jobs entries
		verifyQueryResults(query, 1);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_OPERATOR, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then expect 2 entries (due to necessary permission on 'Operator' category, the definition suspension can be seen as well)
		verifyQueryResults(query, 2);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLogWithReadPermissionOnWrongCategory()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  // job ///////////////////////////////

	  public virtual void testQuerySetJobRetriesUserOperationLogWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_OPERATOR, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // process definition ////////////////////////////////////////////

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithoutAuthorization()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHPermissionOnCategory()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_OPERATOR, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHPermissionOnAnyCategory()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  // process instance //////////////////////////////////////////////

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadPermissionOnCategory()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_OPERATOR, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadPermissionOnAnyCategory()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeploymentWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  public virtual void testQueryAfterDeletingDeploymentWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  public virtual void testQueryAfterDeletingDeploymentWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  public virtual void testQueryAfterDeletingDeploymentWithReadPermissionOnCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_OPERATOR, userId, READ);
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then expect 1 entry (start process instance)
		verifyQueryResults(query, 1);

		// and when
		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, READ);

		// then expect 3 entries (start process instance, set assignee, complete task)
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  public virtual void testQueryAfterDeletingDeploymentWithReadPermissionOnAnyCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, READ);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // delete user operation log (standalone) ////////////////////////

	  public virtual void testDeleteStandaloneEntryWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testDeleteStandaloneEntryWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, DELETE_HISTORY);

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testDeleteStandaloneEntryWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testDeleteStandaloneEntryWithDeletePermissionOnCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, DELETE);

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());

		deleteTask(taskId, true);
	  }

	  public virtual void testDeleteStandaloneEntryWithDeletePermissionOnAnyCategory()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, DELETE);

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());

		deleteTask(taskId, true);
	  }

	  // delete user operation log /////////////////////////////////////

	  public virtual void testDeleteEntryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE_HISTORY.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}
	  }

	  public virtual void testDeleteEntryWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, DELETE_HISTORY);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryWithDeletePermissionOnCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, DELETE);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryWithDeletePermissionOnAnyCategory()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(OPERATION_LOG_CATEGORY, ANY, userId, DELETE);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryAfterDeletingDeployment()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY, DELETE_HISTORY);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();

		disableAuthorization();
		historyService.deleteHistoricProcessInstance(processInstanceId);
		enableAuthorization();
	  }

	  // delete user operation log (case) //////////////////////////////

	  public virtual void testCaseDeleteEntryWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}
	  }

	  public virtual void testCaseDeleteEntryWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_CASE_KEY, userId, DELETE_HISTORY);

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}
	  }

	  public virtual void testCaseDeleteEntryWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		// when
		try
		{
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE.Name, message);
		  assertTextPresent(OPERATION_LOG_CATEGORY.resourceName(), message);
		  assertTextPresent(CATEGORY_TASK_WORKER, message);
		}
	  }

	  public virtual void testCaseDeleteEntryWithDeletePermissionOnCategory()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, CATEGORY_TASK_WORKER, userId, DELETE);

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());
	  }

	  public virtual void testCaseDeleteEntryWithDeletePermissionOnAnyCategory()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(OPERATION_LOG_CATEGORY, ANY, userId, DELETE);

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(UserOperationLogQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual Job selectSingleJob()
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().singleResult();
		enableAuthorization();
		return job;
	  }

	  protected internal virtual void clearDatabase()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly UserOperationLogAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(UserOperationLogAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			IList<HistoricIncident> incidents = Context.ProcessEngineConfiguration.HistoryService.createHistoricIncidentQuery().list();
			foreach (HistoricIncident incident in incidents)
			{
			  commandContext.HistoricIncidentManager.delete((HistoricIncidentEntity) incident);
			}
			return null;
		  }
	  }
	}

}