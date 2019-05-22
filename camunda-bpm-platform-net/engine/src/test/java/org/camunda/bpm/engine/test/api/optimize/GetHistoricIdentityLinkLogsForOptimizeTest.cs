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
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using OptimizeHistoricIdentityLinkLogEntity = org.camunda.bpm.engine.impl.persistence.entity.optimize.OptimizeHistoricIdentityLinkLogEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class GetHistoricIdentityLinkLogsForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetHistoricIdentityLinkLogsForOptimizeTest()
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


	  public const string IDENTITY_LINK_ADD = "add";
	  public const string IDENTITY_LINK_DELETE = "delete";
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  private OptimizeService optimizeService;

	  protected internal const string userId = "testUser";
	  protected internal const string assignerId = "testAssigner";
	  protected internal const string groupId = "testGroup";

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
		createGroup();
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
//ORIGINAL LINE: @Test public void allNecessaryInformationIsAvailable()
	  public virtual void allNecessaryInformationIsAvailable()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").name("task").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.AuthenticatedUserId = assignerId;
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(pastDate(), null, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(1));
		assertThatIdentityLinksHaveAllImportantInformation(identityLinkLogs[0], processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void performCandidateOperations()
	  public virtual void performCandidateOperations()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").name("task").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.AuthenticatedUserId = assignerId;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		taskService.addCandidateUser(taskId, userId);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		taskService.deleteCandidateUser(taskId, userId);
		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		taskService.addCandidateGroup(taskId, groupId);
		DateTime nowPlus6Seconds = new DateTime(now.Ticks + 6000L);
		ClockUtil.CurrentTime = nowPlus6Seconds;
		taskService.deleteCandidateGroup(taskId, groupId);
		DateTime nowPlus8Seconds = new DateTime(now.Ticks + 8000L);
		ClockUtil.CurrentTime = nowPlus8Seconds;

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(pastDate(), null, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(4));
		assertThat(identityLinkLogs[0].UserId, @is(userId));
		assertThat(identityLinkLogs[0].OperationType, @is(IDENTITY_LINK_ADD));
		assertThat(identityLinkLogs[0].Type, @is(IdentityLinkType.CANDIDATE));
		assertThat(identityLinkLogs[1].UserId, @is(userId));
		assertThat(identityLinkLogs[1].OperationType, @is(IDENTITY_LINK_DELETE));
		assertThat(identityLinkLogs[1].Type, @is(IdentityLinkType.CANDIDATE));
		assertThat(identityLinkLogs[2].GroupId, @is(groupId));
		assertThat(identityLinkLogs[2].OperationType, @is(IDENTITY_LINK_ADD));
		assertThat(identityLinkLogs[2].Type, @is(IdentityLinkType.CANDIDATE));
		assertThat(identityLinkLogs[3].GroupId, @is(groupId));
		assertThat(identityLinkLogs[3].OperationType, @is(IDENTITY_LINK_DELETE));
		assertThat(identityLinkLogs[3].Type, @is(IdentityLinkType.CANDIDATE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void performAssigneeOperations()
	  public virtual void performAssigneeOperations()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").name("task").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		identityService.AuthenticatedUserId = assignerId;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		claimAllUserTasks();
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		unclaimAllUserTasks();

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(pastDate(), null, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(2));
		assertThat(identityLinkLogs[0].UserId, @is(userId));
		assertThat(identityLinkLogs[0].OperationType, @is(IDENTITY_LINK_ADD));
		assertThat(identityLinkLogs[0].Type, @is(IdentityLinkType.ASSIGNEE));
		assertThat(identityLinkLogs[1].UserId, @is(userId));
		assertThat(identityLinkLogs[1].OperationType, @is(IDENTITY_LINK_DELETE));
		assertThat(identityLinkLogs[0].Type, @is(IdentityLinkType.ASSIGNEE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterParameterWorks()
	  public virtual void occurredAfterParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").camundaAssignee(userId).endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		taskService.addCandidateUser(taskId, userId);

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		taskService.deleteCandidateUser(taskId, userId);

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(now, null, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAtParameterWorks()
	  public virtual void occurredAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		taskService.addCandidateUser(taskId, userId);

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		taskService.deleteCandidateUser(taskId, userId);

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(null, now, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterAndOccurredAtParameterWorks()
	  public virtual void occurredAfterAndOccurredAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		taskService.addCandidateUser(taskId, userId);

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		taskService.deleteCandidateUser(taskId, userId);

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(now, now, 10);

		// then
		assertThat(identityLinkLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.addCandidateUser(taskId, userId);
		taskService.deleteCandidateUser(taskId, userId);
		taskService.addCandidateUser(taskId, userId);
		taskService.deleteCandidateUser(taskId, userId);
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(pastDate(), null, 3);

		// then
		assertThat(identityLinkLogs.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByTimestamp()
	  public virtual void resultIsSortedByTimestamp()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		taskService.addCandidateUser(taskId, userId);

		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		taskService.deleteCandidateUser(taskId, userId);

		DateTime nowPlus4Seconds = new DateTime(now.Ticks + 4000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		taskService.addCandidateUser(taskId, userId);

		// when
		IList<OptimizeHistoricIdentityLinkLogEntity> identityLinkLogs = optimizeService.getHistoricIdentityLinkLogs(pastDate(), null, 4);

		// then
		assertThat(identityLinkLogs.Count, @is(3));
		assertThat(identityLinkLogs[0].OperationType, @is(IDENTITY_LINK_ADD));
		assertThat(identityLinkLogs[1].OperationType, @is(IDENTITY_LINK_DELETE));
		assertThat(identityLinkLogs[2].OperationType, @is(IDENTITY_LINK_ADD));
	  }

	  private DateTime pastDate()
	  {
		return new DateTime(2L);
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

	  protected internal virtual void createGroup()
	  {
		Group group = identityService.newGroup(GetHistoricIdentityLinkLogsForOptimizeTest.groupId);
		identityService.saveGroup(group);
	  }

	  private void assertThatIdentityLinksHaveAllImportantInformation(OptimizeHistoricIdentityLinkLogEntity identityLinkLog, ProcessInstance processInstance)
	  {
		assertThat(identityLinkLog, notNullValue());
		assertThat(identityLinkLog.UserId, @is(userId));
		assertThat(identityLinkLog.TaskId, @is(taskService.createTaskQuery().singleResult().Id));
		assertThat(identityLinkLog.Type, @is(IdentityLinkType.CANDIDATE));
		assertThat(identityLinkLog.AssignerId, @is(assignerId));
		assertThat(identityLinkLog.GroupId, nullValue());
		assertThat(identityLinkLog.OperationType, @is(IDENTITY_LINK_ADD));
		assertThat(identityLinkLog.ProcessDefinitionId, @is(processInstance.ProcessDefinitionId));
		assertThat(identityLinkLog.ProcessDefinitionKey, @is("process"));
		assertThat(identityLinkLog.ProcessInstanceId, @is(processInstance.Id));
	  }

	}

}