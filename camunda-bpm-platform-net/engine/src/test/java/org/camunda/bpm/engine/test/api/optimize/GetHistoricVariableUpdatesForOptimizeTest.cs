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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;



	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class GetHistoricVariableUpdatesForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetHistoricVariableUpdatesForOptimizeTest()
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
//ORIGINAL LINE: @Test public void getHistoricVariableUpdates()
	  public virtual void getHistoricVariableUpdates()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "foo";
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> historicVariableUpdates = optimizeService.getHistoricVariableUpdates(new DateTime(1L), null, 10);

		// then
		assertThat(historicVariableUpdates.Count, @is(1));
		assertThatUpdateHasAllImportantInformation(historicVariableUpdates[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterParameterWorks()
	  public virtual void occurredAfterParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "value1";
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		runtimeService.startProcessInstanceByKey("process", variables);
		DateTime nowPlus2Seconds = new DateTime((DateTime.Now).Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		variables["stringVar"] = "value2";
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(now, null, 10);

		// then
		assertThat(variableUpdates.Count, @is(1));
		assertThat(variableUpdates[0].Value.ToString(), @is("value2"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAtParameterWorks()
	  public virtual void occurredAtParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "value1";
		runtimeService.startProcessInstanceByKey("process", variables);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		variables["stringVar"] = "value2";
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(null, now, 10);

		// then
		assertThat(variableUpdates.Count, @is(1));
		assertThat(variableUpdates[0].Value.ToString(), @is("value1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void occurredAfterAndOccurredAtParameterWorks()
	  public virtual void occurredAfterAndOccurredAtParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "value1";
		runtimeService.startProcessInstanceByKey("process", variables);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		variables["stringVar"] = "value2";
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(now, now, 10);

		// then
		assertThat(variableUpdates.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void maxResultsParameterWorks()
	  public virtual void maxResultsParameterWorks()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "value1";
		variables["integerVar"] = 1;
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(pastDate(), null, 3);

		// then
		assertThat(variableUpdates.Count, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resultIsSortedByTime()
	  public virtual void resultIsSortedByTime()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);
		DateTime now = DateTime.Now;
		DateTime nowPlus1Second = new DateTime(now.Ticks + 1000L);
		ClockUtil.CurrentTime = nowPlus1Second;
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var1"] = "value1";
		  runtimeService.startProcessInstanceByKey("process", variables);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		variables.Clear();
		variables["var2"] = "value2";
		  runtimeService.startProcessInstanceByKey("process", variables);
		DateTime nowPlus4Seconds = new DateTime(nowPlus2Seconds.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus4Seconds;
		variables.Clear();
		variables["var3"] = "value3";
		  runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(now, null, 10);

		// then
		assertThat(variableUpdates.Count, @is(3));
		assertThat(variableUpdates[0].VariableName, @is("var1"));
		assertThat(variableUpdates[1].VariableName, @is("var2"));
		assertThat(variableUpdates[2].VariableName, @is("var3"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fetchOnlyVariableUpdates()
	  public virtual void fetchOnlyVariableUpdates()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		testHelper.deploy(simpleDefinition);
		runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();
		IDictionary<string, string> formFields = new Dictionary<string, string>();
		formFields["var"] = "foo";
		engineRule.FormService.submitTaskFormData(task.Id, formFields);
		long detailCount = engineRule.HistoryService.createHistoricDetailQuery().count();
		assertThat(detailCount, @is(2L)); // variable update + form property

		// when
		IList<HistoricVariableUpdate> variableUpdates = optimizeService.getHistoricVariableUpdates(pastDate(), null, 10);

		// then
		assertThat(variableUpdates.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getHistoricVariableByteArrayUpdates()
	  public virtual void getHistoricVariableByteArrayUpdates()
	  {
		 // given
		BpmnModelInstance simpleDefinition = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		testHelper.deploy(simpleDefinition);

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = serializable;

		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);
		runtimeService.startProcessInstanceByKey("process", variables);

		// when
		IList<HistoricVariableUpdate> historicVariableUpdates = optimizeService.getHistoricVariableUpdates(new DateTime(1L), null, 10);

		// then
		assertThat(historicVariableUpdates.Count, @is(4));

		foreach (HistoricVariableUpdate variableUpdate in historicVariableUpdates)
		{
		  ObjectValue typedValue = (ObjectValue) variableUpdate.TypedValue;
		  assertThat(typedValue.Deserialized, @is(false));
		  assertThat(typedValue.ValueSerialized, notNullValue());
		}

	  }

	  private DateTime pastDate()
	  {
		return new DateTime(2L);
	  }

	  protected internal virtual void createUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);
	  }

	  private void assertThatUpdateHasAllImportantInformation(HistoricVariableUpdate variableUpdate)
	  {
		assertThat(variableUpdate, notNullValue());
		assertThat(variableUpdate.Id, notNullValue());
		assertThat(variableUpdate.ProcessDefinitionKey, @is("process"));
		assertThat(variableUpdate.ProcessDefinitionId, notNullValue());
		assertThat(variableUpdate.VariableName, @is("stringVar"));
		assertThat(variableUpdate.Value.ToString(), @is("foo"));
		assertThat(variableUpdate.TypeName, @is("string"));
		assertThat(variableUpdate.Time, notNullValue());
	  }

	}

}