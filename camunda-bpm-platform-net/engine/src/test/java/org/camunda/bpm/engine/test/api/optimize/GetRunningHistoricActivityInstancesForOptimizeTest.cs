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
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
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
	public class GetRunningHistoricActivityInstancesForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetRunningHistoricActivityInstancesForOptimizeTest()
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


	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal string userId = "test";
	  private OptimizeService optimizeService;
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
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getRunningHistoricActivityInstances()
	  public virtual void getRunningHistoricActivityInstances()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").name("start").userTask("userTask").name("task").endEvent("endEvent").name("end").done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(pastDate(), null, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(1));
		HistoricActivityInstance activityInstance = runningHistoricActivityInstances[0];
		assertThatActivitiesHaveAllImportantInformation(activityInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAfterParameterWorks()
	  public virtual void startedAfterParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		ClockUtil.CurrentTime = nowPlus2Seconds;
		ProcessInstance secondProcessInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(now, null, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(1));
		assertThat(runningHistoricActivityInstances[0].ProcessInstanceId, @is(secondProcessInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAtParameterWorks()
	  public virtual void startedAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = now;
		ProcessInstance firstProcessInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		ClockUtil.CurrentTime = nowPlus2Seconds;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(null, now, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(1));
		assertThat(runningHistoricActivityInstances[0].ProcessInstanceId, @is(firstProcessInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAfterAndStartedAtParameterWorks()
	  public virtual void startedAfterAndStartedAtParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		ClockUtil.CurrentTime = nowPlus2Seconds;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(now, now, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(pastDate(), null, 3);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByStartTime()
	  public virtual void resultIsSortedByStartTime()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		ClockUtil.CurrentTime = DateTime.Now;
		ProcessInstance firstProcessInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");
		shiftTimeByOneMinute();
		ProcessInstance secondProcessInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");
		shiftTimeByOneMinute();
		ProcessInstance thirdProcessInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(pastDate(), null, 4);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(3));
		assertThat(runningHistoricActivityInstances[0].ProcessInstanceId, @is(firstProcessInstance.Id));
		assertThat(runningHistoricActivityInstances[1].ProcessInstanceId, @is(secondProcessInstance.Id));
		assertThat(runningHistoricActivityInstances[2].ProcessInstanceId, @is(thirdProcessInstance.Id));
	  }

	  public virtual void shiftTimeByOneMinute()
	  {
		long? oneMinute = 1000L * 60L;
		DateTime shiftedTimeByOneMinute = new DateTime(ClockUtil.CurrentTime.Ticks + oneMinute);
		ClockUtil.CurrentTime = shiftedTimeByOneMinute;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fetchOnlyRunningActivities()
	  public virtual void fetchOnlyRunningActivities()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask("userTask").endEvent().done();
		testHelper.deploy(simpleDefinition);
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(pastDate(), null, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(1));
		assertThat(runningHistoricActivityInstances[0].ActivityId, @is("userTask"));

		// when
		completeAllUserTasks();
		runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(pastDate(), null, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(0));
	  }

	  private DateTime pastDate()
	  {
		return new DateTime(2L);
	  }

	  // test fetches only completed, even if there are still running activities

	  private void completeAllUserTasks()
	  {
		IList<Task> list = taskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  taskService.claim(task.Id, userId);
		  taskService.complete(task.Id);
		}
	  }

	  protected internal virtual void createUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);
	  }

	  private void assertThatActivitiesHaveAllImportantInformation(HistoricActivityInstance activityInstance)
	  {
		assertThat(activityInstance, notNullValue());
		assertThat(activityInstance.ActivityName, @is("task"));
		assertThat(activityInstance.ActivityType, @is("userTask"));
		assertThat(activityInstance.StartTime, notNullValue());
		assertThat(activityInstance.EndTime, nullValue());
		assertThat(activityInstance.ProcessDefinitionKey, @is("process"));
		assertThat(activityInstance.ProcessDefinitionId, notNullValue());
	  }

	}

}