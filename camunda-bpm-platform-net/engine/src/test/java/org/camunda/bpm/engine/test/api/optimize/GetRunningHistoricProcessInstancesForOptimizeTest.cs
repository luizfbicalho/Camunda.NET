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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
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
	public class GetRunningHistoricProcessInstancesForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetRunningHistoricProcessInstancesForOptimizeTest()
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

	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";

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
//ORIGINAL LINE: @Test public void getRunningHistoricProcessInstances()
	  public virtual void getRunningHistoricProcessInstances()
	  {
		// given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(pastDate(), null, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(1));
		assertThatInstanceHasAllImportantInformation(runningHistoricProcessInstances[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAfterParameterWorks()
	  public virtual void startedAfterParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(now, null, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAtParameterWorks()
	  public virtual void startedAtParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(null, now, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(1));
		assertThat(runningHistoricProcessInstances[0].Id, @is(processInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startedAfterAndStartedAtParameterWorks()
	  public virtual void startedAfterAndStartedAtParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(now, now, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.startProcessInstanceByKey("process");
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(pastDate(), null, 3);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByStartTime()
	  public virtual void resultIsSortedByStartTime()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus1Second = new DateTime(now.Ticks + 1000L);
		ClockUtil.CurrentTime = nowPlus1Second;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process");
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process");
		DateTime nowPlus4Seconds = new DateTime(nowPlus2Seconds.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(new DateTime(now.Ticks), null, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(3));
		assertThat(runningHistoricProcessInstances[0].Id, @is(processInstance1.Id));
		assertThat(runningHistoricProcessInstances[1].Id, @is(processInstance2.Id));
		assertThat(runningHistoricProcessInstances[2].Id, @is(processInstance3.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fetchOnlyRunningProcessInstances()
	  public virtual void fetchOnlyRunningProcessInstances()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");
		completeAllUserTasks();
		ProcessInstance runningProcessInstance = runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(pastDate(), null, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(1));
		assertThat(runningHistoricProcessInstances[0].Id, @is(runningProcessInstance.Id));
	  }

	  private DateTime pastDate()
	  {
		return new DateTime(2L);
	  }

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

	  private void assertThatInstanceHasAllImportantInformation(HistoricProcessInstance historicProcessInstance)
	  {
		assertThat(historicProcessInstance, notNullValue());
		assertThat(historicProcessInstance.Id, notNullValue());
		assertThat(historicProcessInstance.ProcessDefinitionKey, @is("process"));
		assertThat(historicProcessInstance.ProcessDefinitionVersion, notNullValue());
		assertThat(historicProcessInstance.ProcessDefinitionId, notNullValue());
		assertThat(historicProcessInstance.StartTime, notNullValue());
		assertThat(historicProcessInstance.EndTime, nullValue());
	  }

	}

}