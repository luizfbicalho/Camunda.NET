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
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
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
	public class GetHistoricDecisionInstancesForOptimizeTest
	{
		private bool InstanceFieldsInitialized = false;

		public GetHistoricDecisionInstancesForOptimizeTest()
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


	  public const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";
	  public const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		ProcessEngineConfigurationImpl config = engineRule.ProcessEngineConfiguration;
		optimizeService = config.OptimizeService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		authorizationService = engineRule.AuthorizationService;

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
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void getCompletedHistoricDecisionInstances()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void getCompletedHistoricDecisionInstances()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(pastDate(), null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
		assertThatDecisionsHaveAllImportantInformation(decisionInstances);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void decisionInputInstanceProperties()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void decisionInputInstanceProperties()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(pastDate(), null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
		HistoricDecisionInstance decisionInstance = decisionInstances[0];
		IList<HistoricDecisionInputInstance> inputs = decisionInstance.Inputs;
		assertThat(inputs, @is(notNullValue()));
		assertThat(inputs.Count, @is(1));

		HistoricDecisionInputInstance input = inputs[0];
		assertThat(input.DecisionInstanceId, @is(decisionInstance.Id));
		assertThat(input.ClauseId, @is("in"));
		assertThat(input.ClauseName, @is("input"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void decisionOutputInstanceProperties()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void decisionOutputInstanceProperties()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(pastDate(), null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
		HistoricDecisionInstance decisionInstance = decisionInstances[0];
		IList<HistoricDecisionOutputInstance> outputs = decisionInstance.Outputs;
		assertThat(outputs, @is(notNullValue()));
		assertThat(outputs.Count, @is(1));

		HistoricDecisionOutputInstance output = outputs[0];
		assertThat(output.DecisionInstanceId, @is(decisionInstance.Id));
		assertThat(output.ClauseId, @is("out"));
		assertThat(output.ClauseName, @is("output"));

		assertThat(output.RuleId, @is("rule"));
		assertThat(output.RuleOrder, @is(1));

		assertThat(output.VariableName, @is("result"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void fishedAfterParameterWorks()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void fishedAfterParameterWorks()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		ProcessInstance secondProcessInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(now, null, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
		HistoricDecisionInstance decisionInstance = decisionInstances[0];
		assertThat(decisionInstance.ProcessInstanceId, @is(secondProcessInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void fishedAtParameterWorks()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void fishedAtParameterWorks()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		ProcessInstance firstProcessInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(null, now, 10);

		// then
		assertThat(decisionInstances.Count, @is(1));
		HistoricDecisionInstance decisionInstance = decisionInstances[0];
		assertThat(decisionInstance.ProcessInstanceId, @is(firstProcessInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void fishedAfterAndFinishedAtParameterWorks()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void fishedAfterAndFinishedAtParameterWorks()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		DateTime now = DateTime.Now;
		DateTime nowMinus2Seconds = new DateTime(now.Ticks - 2000L);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);

		ClockUtil.CurrentTime = nowMinus2Seconds;
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		ClockUtil.CurrentTime = now;
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(now, now, 10);

		// then
		assertThat(decisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void maxResultsParameterWorks()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void maxResultsParameterWorks()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		runtimeService.startProcessInstanceByKey("testProcess", variables);
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(null, null, 2);

		// then
		assertThat(decisionInstances.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN}) public void resultIsSortedByEvaluationTime()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN})]
	  public virtual void resultIsSortedByEvaluationTime()
	  {
		// given start process and evaluate decision
		VariableMap variables = Variables.createVariables();
		variables.put("input1", null);
		DateTime now = DateTime.Now;
		DateTime nowMinus2Seconds = new DateTime(now.Ticks - 2000L);
		DateTime nowPlus2Seconds = new DateTime(now.Ticks + 2000L);

		ClockUtil.CurrentTime = nowMinus2Seconds;
		ProcessInstance firstProcessInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
		ClockUtil.CurrentTime = now;
		ProcessInstance secondProcessInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
		ClockUtil.CurrentTime = nowPlus2Seconds;
		ProcessInstance thirdProcessInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		// when
		IList<HistoricDecisionInstance> decisionInstances = optimizeService.getHistoricDecisionInstances(pastDate(), null, 3);

		// then
		assertThat(decisionInstances.Count, @is(3));
		assertThat(decisionInstances[0].ProcessInstanceId, @is(firstProcessInstance.Id));
		assertThat(decisionInstances[1].ProcessInstanceId, @is(secondProcessInstance.Id));
		assertThat(decisionInstances[2].ProcessInstanceId, @is(thirdProcessInstance.Id));
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

	  private void assertThatDecisionsHaveAllImportantInformation(IList<HistoricDecisionInstance> decisionInstances)
	  {
		assertThat(decisionInstances.Count, @is(1));
		HistoricDecisionInstance decisionInstance = decisionInstances[0];


		assertThat(decisionInstance, notNullValue());
		assertThat(decisionInstance.ProcessDefinitionKey, @is("testProcess"));
		assertThat(decisionInstance.ProcessDefinitionId, notNullValue());
		assertThat(decisionInstance.DecisionDefinitionId, notNullValue());
		assertThat(decisionInstance.DecisionDefinitionKey, @is("testDecision"));
		assertThat(decisionInstance.DecisionDefinitionName, @is("sample decision"));

		assertThat(decisionInstance.ActivityId, @is("task"));
		assertThat(decisionInstance.ActivityInstanceId, notNullValue());

		assertThat(decisionInstance.ProcessInstanceId, @is(notNullValue()));
		assertThat(decisionInstance.RootProcessInstanceId, @is(notNullValue()));

		assertThat(decisionInstance.CaseDefinitionKey, @is(nullValue()));
		assertThat(decisionInstance.CaseDefinitionId, @is(nullValue()));

		assertThat(decisionInstance.CaseInstanceId, @is(nullValue()));

		assertThat(decisionInstance.RootDecisionInstanceId, @is(nullValue()));
		assertThat(decisionInstance.DecisionRequirementsDefinitionId, @is(nullValue()));
		assertThat(decisionInstance.DecisionRequirementsDefinitionKey, @is(nullValue()));

		assertThat(decisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	}

}