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
//	import static org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class GetCompletedHistoricActivityInstancesForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetCompletedHistoricActivityInstancesForOptimizeTest()
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
//ORIGINAL LINE: @Test public void getCompletedHistoricActivityInstances()
	  public virtual void getCompletedHistoricActivityInstances()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").name("start").endEvent("endEvent").name("end").done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(pastDate(), null, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(2));
		assertThatActivitiesHaveAllImportantInformation(completedHistoricActivityInstances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fishedAfterParameterWorks()
	  public virtual void fishedAfterParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		ClockUtil.CurrentTime = nowPlus2Seconds;
		completeAllUserTasks();
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(now, null, 10);

		// then
		ISet<string> allowedActivityIds = new HashSet<string>(Arrays.asList("userTask", "endEvent"));
		assertThat(completedHistoricActivityInstances.Count, @is(2));
		assertTrue(allowedActivityIds.Contains(completedHistoricActivityInstances[0].ActivityId));
		assertTrue(allowedActivityIds.Contains(completedHistoricActivityInstances[1].ActivityId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fishedAtParameterWorks()
	  public virtual void fishedAtParameterWorks()
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
		completeAllUserTasks();
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(null, now, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(1));
		assertThat(completedHistoricActivityInstances[0].ActivityId, @is("startEvent"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fishedAfterAndFinishedAtParameterWorks()
	  public virtual void fishedAfterAndFinishedAtParameterWorks()
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
		completeAllUserTasks();
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(now, now, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").serviceTask().camundaExpression("${true}").serviceTask().camundaExpression("${true}").serviceTask().camundaExpression("${true}").serviceTask().camundaExpression("${true}").endEvent("endEvent").done();
		testHelper.deploy(simpleDefinition);
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(pastDate(), null, 3);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByEndTime()
	  public virtual void resultIsSortedByEndTime()
	  {
		 // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").serviceTask("ServiceTask1").camundaExpression("${true}").camundaExecutionListenerClass(EVENTNAME_START, typeof(ShiftTimeByOneMinuteListener).FullName).serviceTask("ServiceTask2").camundaExpression("${true}").camundaExecutionListenerClass(EVENTNAME_START, typeof(ShiftTimeByOneMinuteListener).FullName).serviceTask("ServiceTask3").camundaExpression("${true}").camundaExecutionListenerClass(EVENTNAME_START, typeof(ShiftTimeByOneMinuteListener).FullName).endEvent("endEvent").camundaExecutionListenerClass(EVENTNAME_START, typeof(ShiftTimeByOneMinuteListener).FullName).done();
		testHelper.deploy(simpleDefinition);
		ClockUtil.CurrentTime = DateTime.Now;
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		ClockUtil.reset();

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(pastDate(), null, 4);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(4));
		assertThat(completedHistoricActivityInstances[0].ActivityId, @is("startEvent"));
		assertThat(completedHistoricActivityInstances[1].ActivityId, @is("ServiceTask1"));
		assertThat(completedHistoricActivityInstances[2].ActivityId, @is("ServiceTask2"));
		assertThat(completedHistoricActivityInstances[3].ActivityId, @is("ServiceTask3"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fetchOnlyCompletedActivities()
	  public virtual void fetchOnlyCompletedActivities()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent("startEvent").userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(pastDate(), null, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(1));
		assertThat(completedHistoricActivityInstances[0].ActivityId, @is("startEvent"));
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

	  private void assertThatActivitiesHaveAllImportantInformation(IList<HistoricActivityInstance> completedHistoricActivityInstances)
	  {
		HistoricActivityInstance startEvent = null, endEvent = null;
		foreach (HistoricActivityInstance completedHistoricActivityInstance in completedHistoricActivityInstances)
		{
		  if (completedHistoricActivityInstance.ActivityId.Equals("startEvent"))
		  {
			startEvent = completedHistoricActivityInstance;
		  }
		  else if (completedHistoricActivityInstance.ActivityId.Equals("endEvent"))
		  {
			endEvent = completedHistoricActivityInstance;
		  }
		}
		assertThat(startEvent, notNullValue());
		assertThat(startEvent.ActivityName, @is("start"));
		assertThat(startEvent.ActivityType, @is("startEvent"));
		assertThat(startEvent.StartTime, notNullValue());
		assertThat(startEvent.EndTime, notNullValue());
		assertThat(startEvent.ProcessDefinitionKey, @is("process"));
		assertThat(startEvent.ProcessDefinitionId, notNullValue());

		assertThat(endEvent, notNullValue());
		assertThat(endEvent.ActivityName, @is("end"));
		assertThat(endEvent.ActivityType, @is("noneEndEvent"));
		assertThat(endEvent.StartTime, notNullValue());
		assertThat(endEvent.EndTime, notNullValue());
		assertThat(endEvent.ProcessDefinitionKey, @is("process"));
		assertThat(endEvent.ProcessDefinitionId, notNullValue());
	  }

	}

}