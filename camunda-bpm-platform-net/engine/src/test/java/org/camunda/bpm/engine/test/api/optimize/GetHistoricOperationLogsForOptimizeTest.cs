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
namespace org.camunda.bpm.engine.test.api.optimize
{
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ASSIGN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CLAIM;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.greaterThan;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class GetHistoricOperationLogsForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetHistoricOperationLogsForOptimizeTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  private OptimizeService optimizeService;

	  protected internal string userId = "test";

	  private IdentityService identityService;
	  private RuntimeService runtimeService;
	  private AuthorizationService authorizationService;
	  private TaskService taskService;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		ProcessEngineConfigurationImpl config = engineRule.ProcessEngineConfiguration;
		optimizeService = config.OptimizeService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		authorizationService = engineRule.AuthorizationService;
		taskService = engineRule.TaskService;

		createUser(userId);
		identityService.AuthenticatedUserId = userId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
		ClockUtil.reset();
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getHistoricUserOperationLogs()
	  public virtual void getHistoricUserOperationLogs()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").name("task").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		claimAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(pastDate(), null, 10);

		// then
		assertThat(userOperationsLog.Count, @is(1));
		assertThatTasksHaveAllImportantInformation(userOperationsLog[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterParameterWorks()
	  public virtual void occurredAfterParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").camundaAssignee(userId).endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		unclaimAllUserTasks();

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		claimAllUserTasks();

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		completeAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(now, null, 10);

		// then
		ISet<string> allowedOperationsTypes = new HashSet<string>(Arrays.asList(OPERATION_TYPE_CLAIM, OPERATION_TYPE_COMPLETE));
		assertThat(userOperationsLog.Count, @is(2));
		assertTrue(allowedOperationsTypes.Contains(userOperationsLog[0].OperationType));
		assertTrue(allowedOperationsTypes.Contains(userOperationsLog[1].OperationType));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAtParameterWorks()
	  public virtual void occurredAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		claimAllUserTasks();

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		completeAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(null, now, 10);

		// then
		assertThat(userOperationsLog.Count, @is(1));
		assertThat(userOperationsLog[0].OperationType, @is(OPERATION_TYPE_CLAIM));
		assertThat(userOperationsLog[0].Category, @is(CATEGORY_TASK_WORKER));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterAndOccurredAtParameterWorks()
	  public virtual void occurredAfterAndOccurredAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		claimAllUserTasks();

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		completeAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(now, now, 10);

		// then
		assertThat(userOperationsLog.Count, @is(0));
	  }
	//
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().userTask().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		claimAndCompleteAllUserTasks();
		claimAndCompleteAllUserTasks();
		claimAndCompleteAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(pastDate(), null, 3);

		// then
		assertThat(userOperationsLog.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByTimestamp()
	  public virtual void resultIsSortedByTimestamp()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").camundaAssignee(userId).endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		unclaimAllUserTasks();

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		claimAllUserTasks();

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		completeAllUserTasks();

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(pastDate(), null, 4);

		// then
		assertThat(userOperationsLog.Count, @is(3));
		assertThat(userOperationsLog[0].OperationType, @is(OPERATION_TYPE_ASSIGN));
		assertThat(userOperationsLog[1].OperationType, @is(OPERATION_TYPE_CLAIM));
		assertThat(userOperationsLog[2].OperationType, @is(OPERATION_TYPE_COMPLETE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fetchOnlyUserTaskBasedLogEntries()
	  public virtual void fetchOnlyUserTaskBasedLogEntries()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");
		createLogEntriesThatShouldNotBeReturned(processInstance.Id);
		assertThat(engineRule.HistoryService.createUserOperationLogQuery().count(), greaterThan(0L));

		// when
		IList<UserOperationLogEntry> userOperationsLog = optimizeService.getHistoricUserOperationLogs(pastDate(), null, 10);

		// then
		assertThat(userOperationsLog.Count, @is(0));
	  }

	  private void createLogEntriesThatShouldNotBeReturned(string processInstanceId)
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		string processTaskId = taskService.createTaskQuery().singleResult().Id;

		// create and remove some links
		taskService.addCandidateUser(processTaskId, "er");
		taskService.deleteCandidateUser(processTaskId, "er");
		taskService.addCandidateGroup(processTaskId, "wir");
		taskService.deleteCandidateGroup(processTaskId, "wir");

		// assign and reassign the owner
		taskService.setOwner(processTaskId, "icke");

		// change priority of task
		taskService.setPriority(processTaskId, 10);

		// add and delete an attachment
		Attachment attachment = taskService.createAttachment("image/ico", processTaskId, processInstanceId, "favicon.ico", "favicon", "http://camunda.com/favicon.ico");
		taskService.deleteAttachment(attachment.Id);
		runtimeService.deleteProcessInstance(processInstanceId, "that's why");

		// create a standalone userTask
		Task userTask = taskService.newTask();
		userTask.Name = "to do";
		taskService.saveTask(userTask);

		// change some properties manually to create an update event
		ClockUtil.CurrentTime = DateTime.Now;
		userTask.Description = "desc";
		userTask.Owner = "icke";
		userTask.Assignee = "er";
		userTask.DueDate = DateTime.Now;
		taskService.saveTask(userTask);

		taskService.deleteTask(userTask.Id, true);
	  }


	  private DateTime pastDate()
	  {
		return new DateTime(2L);
	  }

	  private void claimAndCompleteAllUserTasks()
	  {
		IList<Task> list = taskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  taskService.claim(task.Id, userId);
		  taskService.complete(task.Id);
		}
	  }

	  private void completeAllUserTasks()
	  {
		IList<Task> list = taskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  taskService.complete(task.Id);
		}
	  }

	  private void claimAllUserTasks()
	  {
		IList<Task> list = taskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  taskService.claim(task.Id, userId);
		}
	  }

	  private void unclaimAllUserTasks()
	  {
		IList<Task> list = taskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  taskService.setAssignee(task.Id, null);
		}
	  }

	  protected internal virtual void createUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);
	  }

	  private void assertThatTasksHaveAllImportantInformation(UserOperationLogEntry userOperationLogEntry)
	  {
		assertThat(userOperationLogEntry, notNullValue());
		assertThat(userOperationLogEntry.Id, notNullValue());
		assertThat(userOperationLogEntry.OperationType, @is(OPERATION_TYPE_CLAIM));
		assertThat(userOperationLogEntry.OrgValue, nullValue());
		assertThat(userOperationLogEntry.NewValue, @is(userId));
		assertThat(userOperationLogEntry.Timestamp, notNullValue());
		assertThat(userOperationLogEntry.ProcessDefinitionKey, @is("process"));
		assertThat(userOperationLogEntry.ProcessDefinitionId, notNullValue());
		assertThat(userOperationLogEntry.UserId, @is(userId));
		assertThat(userOperationLogEntry.TaskId, @is(taskService.createTaskQuery().singleResult().Id));
		assertThat(userOperationLogEntry.Category, @is(CATEGORY_TASK_WORKER));
	  }

	}

}