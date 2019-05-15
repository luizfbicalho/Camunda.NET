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
namespace org.camunda.bpm.engine.test.history.dmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class HistoricDecisionInstanceDecisionServiceEvaluationTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceDecisionServiceEvaluationTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDecisionInstanceDecisionServiceEvaluationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
		}


	  protected internal const string DECISION_PROCESS_WITH_DECISION_SERVICE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideDelegation.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_START_LISTENER = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideStartListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_END_LISTENER = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideEndListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_WITH_TAKE_LISTENER = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideTakeListener.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_INSIDE_EXPRESSION = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideExpression.bpmn20.xml";
	  protected internal const string DECISION_PROCESS_INSIDE_DELEGATE_EXPRESSION = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testDecisionEvaluatedWithDecisionServiceInsideDelegateExpression.bpmn20.xml";

	  protected internal const string DECISION_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";

	  protected internal const string DECISION_DEFINITION_KEY = "testDecision";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {DECISION_PROCESS_WITH_DECISION_SERVICE, "task"},
			new object[] {DECISION_PROCESS_WITH_START_LISTENER, "task"},
			new object[] {DECISION_PROCESS_WITH_END_LISTENER, "task"},
			new object[] {DECISION_PROCESS_INSIDE_EXPRESSION, "task"},
			new object[] {DECISION_PROCESS_INSIDE_DELEGATE_EXPRESSION, "task"},
			new object[] {DECISION_PROCESS_WITH_TAKE_LISTENER, "start"}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String process;
	  public string process;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public String activityId;
	  public string activityId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testRule = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testRule;

	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		testRule.deploy(DECISION_DMN, process);

		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionWithDecisionService()
	  public virtual void evaluateDecisionWithDecisionService()
	  {

		runtimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("input1", null).putValue("myBean", new DecisionServiceDelegate()));

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();
		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		// references to process instance should be set since the decision is evaluated while executing a process instance
		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(processDefinition.Key));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(processDefinition.Id));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(processInstance.Id));
		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.ActivityId, @is(activityId));
		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	}

}