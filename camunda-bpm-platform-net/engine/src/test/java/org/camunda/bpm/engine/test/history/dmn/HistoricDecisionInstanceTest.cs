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
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using DateTime = org.joda.time.DateTime;

	/// <summary>
	/// @author Philipp Ossler
	/// @author Ingo Richtsmeier
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceTest : PluggableProcessEngineTestCase
	{

	  public const string DECISION_CASE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.caseWithDecisionTask.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testCaseDecisionEvaluatedWithDecisionServiceInsideDelegate.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_RULE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testManualActivationRuleEvaluatesDecision.cmmn";
	  public const string DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_IF_PART = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.testIfPartEvaluatesDecision.cmmn";

	  public const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";

	  public const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";
	  public const string DECISION_MULTIPLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionMultipleOutput.dmn11.xml";
	  public const string DECISION_COMPOUND_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionCompoundOutput.dmn11.xml";
	  public const string DECISION_MULTIPLE_INPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionMultipleInput.dmn11.xml";
	  public const string DECISION_COLLECT_SUM_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionCollectSum.dmn11.xml";
	  public const string DECISION_RETURNS_TRUE = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.returnsTrue.dmn11.xml";

	  public const string DECISION_LITERAL_EXPRESSION_DMN = "org/camunda/bpm/engine/test/api/dmn/DecisionWithLiteralExpression.dmn";

	  public const string DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  public const string DECISION_DEFINITION_KEY = "testDecision";

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();
		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;
		string activityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("task").singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(processDefinition.Key));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(processDefinition.Id));

		assertThat(historicDecisionInstance.ProcessInstanceId, @is(processInstance.Id));

		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(nullValue()));

		assertThat(historicDecisionInstance.CaseInstanceId, @is(nullValue()));

		assertThat(historicDecisionInstance.ActivityId, @is("task"));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(activityInstanceId));

		assertThat(historicDecisionInstance.RootDecisionInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.DecisionRequirementsDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.DecisionRequirementsDefinitionKey, @is(nullValue()));

		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	  [Deployment(resources : { DECISION_CASE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testCaseDecisionInstanceProperties()
	  {

		CaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionId(caseInstance.CaseDefinitionId).singleResult();

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		string activityInstanceId = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_DecisionTask_1").singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(nullValue()));

		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(caseDefinition.Key));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(caseDefinition.Id));
		assertThat(historicDecisionInstance.CaseInstanceId, @is(caseInstance.Id));

		assertThat(historicDecisionInstance.ActivityId, @is("PI_DecisionTask_1"));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(activityInstanceId));

		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionInputInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().singleResult();
		IList<HistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		assertThat(inputs, @is(notNullValue()));
		assertThat(inputs.Count, @is(1));

		HistoricDecisionInputInstance input = inputs[0];
		assertThat(input.DecisionInstanceId, @is(historicDecisionInstance.Id));
		assertThat(input.ClauseId, @is("in"));
		assertThat(input.ClauseName, @is("input"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testMultipleDecisionInstances()
	  {

		startProcessInstanceAndEvaluateDecision("a");
		waitASignificantAmountOfTime();
		startProcessInstanceAndEvaluateDecision("b");

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().orderByEvaluationTime().asc().list();
		assertThat(historicDecisionInstances.Count, @is(2));

		IList<HistoricDecisionInputInstance> inputsOfFirstDecision = historicDecisionInstances[0].Inputs;
		assertThat(inputsOfFirstDecision.Count, @is(1));
		assertThat(inputsOfFirstDecision[0].Value, @is((object) "a"));

		IList<HistoricDecisionInputInstance> inputsOfSecondDecision = historicDecisionInstances[1].Inputs;
		assertThat(inputsOfSecondDecision.Count, @is(1));
		assertThat(inputsOfSecondDecision[0].Value, @is((object) "b"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_MULTIPLE_INPUT_DMN})]
	  public virtual void testMultipleDecisionInputInstances()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "a";
		variables["input2"] = 1;
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().singleResult();
		IList<HistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		assertThat(inputs.Count, @is(2));

		assertThat(inputs[0].Value, @is((object) "a"));
		assertThat(inputs[1].Value, @is((object) 1));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDisableDecisionInputInstanceByteValue()
	  {

		sbyte[] bytes = "object".GetBytes();
		startProcessInstanceAndEvaluateDecision(bytes);

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().disableBinaryFetching().singleResult();
		IList<HistoricDecisionInputInstance> inputs = historicDecisionInstance.Inputs;
		assertThat(inputs.Count, @is(1));

		HistoricDecisionInputInstance input = inputs[0];
		assertThat(input.TypeName, @is("bytes"));
		assertThat(input.Value, @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionOutputInstanceProperties()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().singleResult();
		IList<HistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		assertThat(outputs, @is(notNullValue()));
		assertThat(outputs.Count, @is(1));

		HistoricDecisionOutputInstance output = outputs[0];
		assertThat(output.DecisionInstanceId, @is(historicDecisionInstance.Id));
		assertThat(output.ClauseId, @is("out"));
		assertThat(output.ClauseName, @is("output"));

		assertThat(output.RuleId, @is("rule"));
		assertThat(output.RuleOrder, @is(1));

		assertThat(output.VariableName, @is("result"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_MULTIPLE_OUTPUT_DMN })]
	  public virtual void testMultipleDecisionOutputInstances()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().singleResult();
		IList<HistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		assertThat(outputs.Count, @is(2));

		HistoricDecisionOutputInstance firstOutput = outputs[0];
		assertThat(firstOutput.ClauseId, @is("out1"));
		assertThat(firstOutput.RuleId, @is("rule1"));
		assertThat(firstOutput.RuleOrder, @is(1));
		assertThat(firstOutput.VariableName, @is("result1"));
		assertThat(firstOutput.Value, @is((object) "okay"));

		HistoricDecisionOutputInstance secondOutput = outputs[1];
		assertThat(secondOutput.ClauseId, @is("out1"));
		assertThat(secondOutput.RuleId, @is("rule2"));
		assertThat(secondOutput.RuleOrder, @is(2));
		assertThat(secondOutput.VariableName, @is("result1"));
		assertThat(secondOutput.Value, @is((object) "not okay"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_COMPOUND_OUTPUT_DMN })]
	  public virtual void testCompoundDecisionOutputInstances()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().singleResult();
		IList<HistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		assertThat(outputs.Count, @is(2));

		HistoricDecisionOutputInstance firstOutput = outputs[0];
		assertThat(firstOutput.ClauseId, @is("out1"));
		assertThat(firstOutput.RuleId, @is("rule1"));
		assertThat(firstOutput.RuleOrder, @is(1));
		assertThat(firstOutput.VariableName, @is("result1"));
		assertThat(firstOutput.Value, @is((object) "okay"));

		HistoricDecisionOutputInstance secondOutput = outputs[1];
		assertThat(secondOutput.ClauseId, @is("out2"));
		assertThat(secondOutput.RuleId, @is("rule1"));
		assertThat(secondOutput.RuleOrder, @is(1));
		assertThat(secondOutput.VariableName, @is("result2"));
		assertThat(secondOutput.Value, @is((object) "not okay"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_COLLECT_SUM_DMN })]
	  public virtual void testCollectResultValue()
	  {

		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance.CollectResultValue, @is(notNullValue()));
		assertThat(historicDecisionInstance.CollectResultValue, @is(3.0));
	  }

	  [Deployment(resources : DECISION_LITERAL_EXPRESSION_DMN)]
	  public virtual void testDecisionInstancePropertiesOfDecisionLiteralExpression()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		decisionService.evaluateDecisionByKey("decision").variables(Variables.createVariables().putValue("sum", 2205)).evaluate();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().includeInputs().includeOutputs();
		assertThat(query.count(), @is(1L));

		HistoricDecisionInstance historicDecisionInstance = query.singleResult();

		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinition.Id));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is("decision"));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("Decision with Literal Expression"));
		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));

		assertThat(historicDecisionInstance.Inputs.Count, @is(0));

		IList<HistoricDecisionOutputInstance> outputs = historicDecisionInstance.Outputs;
		assertThat(outputs.Count, @is(1));

		HistoricDecisionOutputInstance output = outputs[0];
		assertThat(output.VariableName, @is("result"));
		assertThat(output.TypeName, @is("string"));
		assertThat((string) output.Value, @is("ok"));

		assertThat(output.ClauseId, @is(nullValue()));
		assertThat(output.ClauseName, @is(nullValue()));
		assertThat(output.RuleId, @is(nullValue()));
		assertThat(output.RuleOrder, @is(nullValue()));
	  }

	  [Deployment(resources : DRG_DMN)]
	  public virtual void testDecisionInstancePropertiesOfDrdDecision()
	  {

		decisionService.evaluateDecisionTableByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.count(), @is(3L));

		HistoricDecisionInstance rootHistoricDecisionInstance = query.decisionDefinitionKey("dish-decision").singleResult();
		HistoricDecisionInstance requiredHistoricDecisionInstance1 = query.decisionDefinitionKey("season").singleResult();
		HistoricDecisionInstance requiredHistoricDecisionInstance2 = query.decisionDefinitionKey("guestCount").singleResult();

		assertThat(rootHistoricDecisionInstance.RootDecisionInstanceId, @is(nullValue()));
		assertThat(rootHistoricDecisionInstance.DecisionRequirementsDefinitionId, @is(decisionRequirementsDefinition.Id));
		assertThat(rootHistoricDecisionInstance.DecisionRequirementsDefinitionKey, @is(decisionRequirementsDefinition.Key));

		assertThat(requiredHistoricDecisionInstance1.RootDecisionInstanceId, @is(rootHistoricDecisionInstance.Id));
		assertThat(requiredHistoricDecisionInstance1.DecisionRequirementsDefinitionId, @is(decisionRequirementsDefinition.Id));
		assertThat(requiredHistoricDecisionInstance1.DecisionRequirementsDefinitionKey, @is(decisionRequirementsDefinition.Key));

		assertThat(requiredHistoricDecisionInstance2.RootDecisionInstanceId, @is(rootHistoricDecisionInstance.Id));
		assertThat(requiredHistoricDecisionInstance2.DecisionRequirementsDefinitionId, @is(decisionRequirementsDefinition.Id));
		assertThat(requiredHistoricDecisionInstance2.DecisionRequirementsDefinitionKey, @is(decisionRequirementsDefinition.Key));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDeleteHistoricDecisionInstances()
	  {
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY);

		startProcessInstanceAndEvaluateDecision();

		assertThat(query.count(), @is(1L));

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();
		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinition.Id);

		assertThat(query.count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDeleteHistoricDecisionInstanceByInstanceId()
	  {

		// given
		startProcessInstanceAndEvaluateDecision();
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY);

		assertThat(query.count(), @is(1L));
		HistoricDecisionInstance historicDecisionInstance = query.includeInputs().includeOutputs().singleResult();

		// when
		historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		// then
		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testDeleteHistoricDecisionInstanceByUndeployment()
	  {
		string firstDeploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_PROCESS).addClasspathResource(DECISION_SINGLE_OUTPUT_DMN).deploy().Id;

		startProcessInstanceAndEvaluateDecision();

		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_PROCESS).addClasspathResource(DECISION_MULTIPLE_OUTPUT_DMN).deploy().Id;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();
		assertThat(query.count(), @is(1L));

		repositoryService.deleteDeployment(secondDeploymentId, true);
		assertThat(query.count(), @is(1L));

		repositoryService.deleteDeployment(firstDeploymentId, true);
		assertThat(query.count(), @is(0L));
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionEvaluatedWithDecisionService()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "test";
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
		// references to process instance should be null since the decision is not evaluated while executing a process instance
		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.ActivityId, @is(nullValue()));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(nullValue()));
		// the user should be null since no user was authenticated during evaluation
		assertThat(historicDecisionInstance.UserId, @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionEvaluatedWithAuthenticatedUser()
	  {
		identityService.AuthenticatedUserId = "demo";
		VariableMap variables = Variables.putValue("input1", "test");
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		// the user should be set since the decision was evaluated with the decision service
		assertThat(historicDecisionInstance.UserId, @is("demo"));
	  }

	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionEvaluatedWithAuthenticatedUserFromProcess()
	  {
		identityService.AuthenticatedUserId = "demo";
		startProcessInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		// the user should be null since the decision was evaluated by the process
		assertThat(historicDecisionInstance.UserId, @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_CASE_WITH_DECISION_SERVICE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDecisionEvaluatedWithAuthenticatedUserFromCase()
	  {
		identityService.AuthenticatedUserId = "demo";
		createCaseInstanceAndEvaluateDecision();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		// the user should be null since decision was evaluated by the case
		assertThat(historicDecisionInstance.UserId, @is(nullValue()));
	  }

	  [Deployment(resources : { DECISION_CASE_WITH_DECISION_SERVICE, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testCaseDecisionEvaluatedWithDecisionServiceInsideDelegate()
	  {

		CaseInstance caseInstance = createCaseInstanceAndEvaluateDecision();

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionId(caseInstance.CaseDefinitionId).singleResult();

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		string activityInstanceId = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		// references to case instance should be set since the decision is evaluated while executing a case instance
		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(caseDefinition.Key));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(caseDefinition.Id));
		assertThat(historicDecisionInstance.CaseInstanceId, @is(caseInstance.Id));
		assertThat(historicDecisionInstance.ActivityId, @is("PI_HumanTask_1"));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(activityInstanceId));
		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	  [Deployment(resources : { DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_RULE, DECISION_RETURNS_TRUE })]
	  public virtual void testManualActivationRuleEvaluatesDecision()
	  {

		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").setVariable("input1", null).setVariable("myBean", new DecisionServiceDelegate()).create();

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionId(caseInstance.CaseDefinitionId).singleResult();

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		string activityInstanceId = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		// references to case instance should be set since the decision is evaluated while executing a case instance
		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(caseDefinition.Key));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(caseDefinition.Id));
		assertThat(historicDecisionInstance.CaseInstanceId, @is(caseInstance.Id));
		assertThat(historicDecisionInstance.ActivityId, @is("PI_HumanTask_1"));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(activityInstanceId));
		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	  [Deployment(resources : { DECISION_CASE_WITH_DECISION_SERVICE_INSIDE_IF_PART, DECISION_RETURNS_TRUE })]
	  public virtual void testIfPartEvaluatesDecision()
	  {

		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").setVariable("input1", null).setVariable("myBean", new DecisionServiceDelegate()).create();

		string humanTask1 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;
		caseService.completeCaseExecution(humanTask1);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionId(caseInstance.CaseDefinitionId).singleResult();

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;

		string activityInstanceId = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult().Id;

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();

		assertThat(historicDecisionInstance, @is(notNullValue()));
		assertThat(historicDecisionInstance.DecisionDefinitionId, @is(decisionDefinitionId));
		assertThat(historicDecisionInstance.DecisionDefinitionKey, @is(DECISION_DEFINITION_KEY));
		assertThat(historicDecisionInstance.DecisionDefinitionName, @is("sample decision"));

		// references to case instance should be set since the decision is evaluated while executing a case instance
		assertThat(historicDecisionInstance.ProcessDefinitionKey, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessDefinitionId, @is(nullValue()));
		assertThat(historicDecisionInstance.ProcessInstanceId, @is(nullValue()));
		assertThat(historicDecisionInstance.CaseDefinitionKey, @is(caseDefinition.Key));
		assertThat(historicDecisionInstance.CaseDefinitionId, @is(caseDefinition.Id));
		assertThat(historicDecisionInstance.CaseInstanceId, @is(caseInstance.Id));
		assertThat(historicDecisionInstance.ActivityId, @is("PI_HumanTask_1"));
		assertThat(historicDecisionInstance.ActivityInstanceId, @is(activityInstanceId));
		assertThat(historicDecisionInstance.EvaluationTime, @is(notNullValue()));
	  }

	  public virtual void testTableNames()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		assertThat(managementService.getTableName(typeof(HistoricDecisionInstance)), @is(tablePrefix + "ACT_HI_DECINST"));

		assertThat(managementService.getTableName(typeof(HistoricDecisionInstanceEntity)), @is(tablePrefix + "ACT_HI_DECINST"));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecision()
	  {
		return startProcessInstanceAndEvaluateDecision(null);
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecision(object input)
	  {
		return runtimeService.startProcessInstanceByKey("testProcess", getVariables(input));
	  }

	  protected internal virtual CaseInstance createCaseInstanceAndEvaluateDecision()
	  {
		return caseService.withCaseDefinitionByKey("case").setVariables(getVariables("test")).create();
	  }

	  protected internal virtual VariableMap getVariables(object input)
	  {
		VariableMap variables = Variables.createVariables();
		variables.put("input1", input);
		return variables;
	  }

	  /// <summary>
	  /// Use between two rule evaluations to ensure the expected order by evaluation time.
	  /// </summary>
	  protected internal virtual void waitASignificantAmountOfTime()
	  {
		DateTime now = new DateTime(ClockUtil.CurrentTime);
		ClockUtil.CurrentTime = now.plusSeconds(10).toDate();
	  }

	}

}