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
namespace org.camunda.bpm.engine.test.dmn.businessruletask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using DecisionDefinitionNotFoundException = org.camunda.bpm.engine.exception.dmn.DecisionDefinitionNotFoundException;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class DmnBusinessRuleTaskTest
	{
		private bool InstanceFieldsInitialized = false;

		public DmnBusinessRuleTaskTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  public const string DECISION_PROCESS = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml";
	  public const string DECISION_PROCESS_EXPRESSION = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRefExpression.bpmn20.xml";
	  public const string DECISION_PROCESS_COMPOSITEEXPRESSION = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRefCompositeExpression.bpmn20.xml";
	  public const string DECISION_PROCESS_LATEST = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRefLatestBinding.bpmn20.xml";
	  public const string DECISION_PROCESS_DEPLOYMENT = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRefDeploymentBinding.bpmn20.xml";
	  public const string DECISION_PROCESS_VERSION = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRefVersionBinding.bpmn20.xml";
	  public const string DECISION_OKAY_DMN = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionOkay.dmn11.xml";
	  public const string DECISION_NOT_OKAY_DMN = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionNotOkay.dmn11.xml";
	  public const string DECISION_VERSION_TAG_OKAY_DMN = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionVersionTagOkay.dmn11.xml";
	  public const string DECISION_POJO_DMN = "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testPojo.dmn11.xml";

	  public const string DECISION_LITERAL_EXPRESSION_DMN = "org/camunda/bpm/engine/test/dmn/deployment/DecisionWithLiteralExpression.dmn";
	  public const string DRD_DISH_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  public static readonly BpmnModelInstance BPMN_VERSION_TAG_BINDING = Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("versionTag").camundaDecisionRefVersionTag("0.0.2").camundaMapDecisionResult("singleEntry").camundaResultVariable("result").endEvent().camundaAsyncBefore().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DECISION_PROCESS, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN }) @Test public void decisionRef()
	  [Deployment(resources : { DECISION_PROCESS, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN })]
	  public virtual void decisionRef()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		assertEquals("okay", getDecisionResult(processInstance));

		processInstance = startExpressionProcess("testDecision", 1);
		assertEquals("okay", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DECISION_PROCESS) @Test public void noDecisionFound()
	  [Deployment(resources : DECISION_PROCESS)]
	  public virtual void noDecisionFound()
	  {
		thrown.expect(typeof(DecisionDefinitionNotFoundException));
		thrown.expectMessage("no decision definition deployed with key 'testDecision'");

		runtimeService.startProcessInstanceByKey("testProcess");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DECISION_PROCESS_EXPRESSION) @Test public void noDecisionFoundRefByExpression()
	  [Deployment(resources : DECISION_PROCESS_EXPRESSION)]
	  public virtual void noDecisionFoundRefByExpression()
	  {
		thrown.expect(typeof(DecisionDefinitionNotFoundException));
		thrown.expectMessage("no decision definition deployed with key = 'testDecision', version = '1' and tenant-id 'null");

	   startExpressionProcess("testDecision", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DECISION_PROCESS_LATEST, DECISION_OKAY_DMN }) @Test public void decisionRefLatestBinding()
	  [Deployment(resources : { DECISION_PROCESS_LATEST, DECISION_OKAY_DMN })]
	  public virtual void decisionRefLatestBinding()
	  {
		testRule.deploy(DECISION_NOT_OKAY_DMN);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		assertEquals("not okay", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DECISION_PROCESS_DEPLOYMENT, DECISION_OKAY_DMN }) @Test public void decisionRefDeploymentBinding()
	  [Deployment(resources : { DECISION_PROCESS_DEPLOYMENT, DECISION_OKAY_DMN })]
	  public virtual void decisionRefDeploymentBinding()
	  {
		testRule.deploy(DECISION_NOT_OKAY_DMN);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		assertEquals("okay", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DECISION_PROCESS_VERSION, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN }) @Test public void decisionRefVersionBinding()
	  [Deployment(resources : { DECISION_PROCESS_VERSION, DECISION_PROCESS_EXPRESSION, DECISION_OKAY_DMN })]
	  public virtual void decisionRefVersionBinding()
	  {
		testRule.deploy(DECISION_NOT_OKAY_DMN);
		testRule.deploy(DECISION_OKAY_DMN);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		assertEquals("not okay", getDecisionResult(processInstance));

		processInstance = startExpressionProcess("testDecision", 2);
		assertEquals("not okay", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRefVersionTagBinding()
	  public virtual void decisionRefVersionTagBinding()
	  {
		// given
		testRule.deploy(DECISION_VERSION_TAG_OKAY_DMN);
		testRule.deploy(BPMN_VERSION_TAG_BINDING);

		// when
		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey("process").setVariable("status", "gold").execute();

		// then
		assertEquals("A", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRefVersionTagBindingExpression()
	  public virtual void decisionRefVersionTagBindingExpression()
	  {
		// given
		testRule.deploy(DECISION_VERSION_TAG_OKAY_DMN);
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decision").camundaDecisionRefBinding("versionTag").camundaDecisionRefVersionTag("${versionTagExpr}").camundaMapDecisionResult("singleEntry").camundaResultVariable("result").endEvent().camundaAsyncBefore().done());

		// when
		VariableMap variables = Variables.createVariables().putValue("versionTagExpr", "0.0.2").putValue("status", "gold");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);

		// then
		assertEquals("A", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRefVersionTagBindingWithoutVersionTag()
	  public virtual void decisionRefVersionTagBindingWithoutVersionTag()
	  {
		// expected
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Could not parse BPMN process.");

		// when
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("testDecision").camundaDecisionRefBinding("versionTag").camundaMapDecisionResult("singleEntry").camundaResultVariable("result").endEvent().camundaAsyncBefore().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRefVersionTagBindingNoneDecisionDefinition()
	  public virtual void decisionRefVersionTagBindingNoneDecisionDefinition()
	  {
		// expected
		thrown.expect(typeof(DecisionDefinitionNotFoundException));
		thrown.expectMessage("no decision definition deployed with key = 'decision', versionTag = '0.0.2' and tenant-id 'null'");

		// given
		testRule.deploy(BPMN_VERSION_TAG_BINDING);

		// when
		runtimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void decisionRefVersionTagBindingTwoDecisionDefinitions()
	  public virtual void decisionRefVersionTagBindingTwoDecisionDefinitions()
	  {
		// expected
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Found more than one decision definition for key 'decision' and versionTag '0.0.2'");

		// given
		testRule.deploy(DECISION_VERSION_TAG_OKAY_DMN);
		testRule.deploy(DECISION_VERSION_TAG_OKAY_DMN);
		testRule.deploy(BPMN_VERSION_TAG_BINDING);

		// when
		runtimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {DECISION_PROCESS, DECISION_POJO_DMN}) @Test public void testPojo()
	  [Deployment(resources : {DECISION_PROCESS, DECISION_POJO_DMN})]
	  public virtual void testPojo()
	  {
		VariableMap variables = Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37));
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		assertEquals("okay", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DECISION_LITERAL_EXPRESSION_DMN) @Test public void evaluateDecisionWithLiteralExpression()
	  [Deployment(resources : DECISION_LITERAL_EXPRESSION_DMN)]
	  public virtual void evaluateDecisionWithLiteralExpression()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("decisionLiteralExpression").camundaResultVariable("result").camundaMapDecisionResult("singleEntry").endEvent().camundaAsyncBefore().done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("a", 2).putValue("b", 3));

		assertEquals(5, getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DRD_DISH_RESOURCE) @Test public void evaluateDecisionWithRequiredDecisions()
	  [Deployment(resources : DRD_DISH_RESOURCE)]
	  public virtual void evaluateDecisionWithRequiredDecisions()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().businessRuleTask().camundaDecisionRef("dish-decision").camundaResultVariable("result").camundaMapDecisionResult("singleEntry").endEvent().camundaAsyncBefore().done());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		assertEquals("Light salad", getDecisionResult(processInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DECISION_PROCESS_COMPOSITEEXPRESSION, DECISION_OKAY_DMN}) @Test public void decisionRefWithCompositeExpression()
	  [Deployment(resources : { DECISION_PROCESS_COMPOSITEEXPRESSION, DECISION_OKAY_DMN})]
	  public virtual void decisionRefWithCompositeExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("version", 1);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcessCompositeExpression", variables);

		assertEquals("okay", getDecisionResult(processInstance));
	  }

	  protected internal virtual ProcessInstance startExpressionProcess(object decisionKey, object version)
	  {
		VariableMap variables = Variables.createVariables().putValue("decision", decisionKey).putValue("version", version);
		return runtimeService.startProcessInstanceByKey("testProcessExpression", variables);
	  }

	  protected internal virtual object getDecisionResult(ProcessInstance processInstance)
	  {
		// the single entry of the single result of the decision result is stored as process variable
		return runtimeService.getVariable(processInstance.Id, "result");
	  }

	}

}