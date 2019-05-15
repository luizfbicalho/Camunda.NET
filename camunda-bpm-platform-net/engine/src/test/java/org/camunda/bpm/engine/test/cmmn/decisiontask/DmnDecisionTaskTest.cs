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
namespace org.camunda.bpm.engine.test.cmmn.decisiontask
{
	using DecisionDefinitionNotFoundException = org.camunda.bpm.engine.exception.dmn.DecisionDefinitionNotFoundException;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DmnDecisionTaskTest : CmmnProcessEngineTestCase
	{

	  public const string CMMN_CALL_DECISION_CONSTANT = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionAsConstant.cmmn";
	  public const string CMMN_CALL_DECISION_CONSTANT_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionAsConstantWithManualActiovation.cmmn";
	  public const string CMMN_CALL_DECISION_EXPRESSION = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionAsExpressionStartsWithDollar.cmmn";
	  public const string CMMN_CALL_DECISION_EXPRESSION_WITH_MANUAL_ACTIVATION = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionAsExpressionStartsWithDollarWithManualActiovation.cmmn";

	  public const string DECISION_OKAY_DMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testDecisionOkay.dmn11.xml";
	  public const string DECISION_NOT_OKAY_DMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testDecisionNotOkay.dmn11.xml";
	  public const string DECISION_POJO_DMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testPojo.dmn11.xml";

	  public const string DECISION_LITERAL_EXPRESSION_DMN = "org/camunda/bpm/engine/test/dmn/deployment/DecisionWithLiteralExpression.dmn";
	  public const string DRD_DISH_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal readonly string CASE_KEY = "case";
	  protected internal readonly string DECISION_TASK = "PI_DecisionTask_1";
	  protected internal readonly string DECISION_KEY = "testDecision";

	  [Deployment(resources : {CMMN_CALL_DECISION_CONSTANT, DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionAsConstant()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY);

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("okay", getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { CMMN_CALL_DECISION_EXPRESSION, DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionAsExpressionStartsWithDollar()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("testDecision", "testDecision"));

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("okay", getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionAsExpressionStartsWithHash.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionAsExpressionStartsWithHash()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("testDecision", "testDecision"));

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("okay", getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallLatestDecision.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallLatestCase()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_NOT_OKAY_DMN).deploy().Id;

		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY);

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("not okay", getDecisionResult(caseInstance));

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionByDeployment.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionByDeployment()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_NOT_OKAY_DMN).deploy().Id;

		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY);

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("okay", getDecisionResult(caseInstance));

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionByVersion.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionByVersion()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_NOT_OKAY_DMN).deploy().Id;

		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY);

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("not okay", getDecisionResult(caseInstance));

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionByVersionAsExpressionStartsWithDollar.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionByVersionAsExpressionStartsWithDollar()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_NOT_OKAY_DMN).deploy().Id;

		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("myVersion", 2));

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("not okay", getDecisionResult(caseInstance));

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTaskTest.testCallDecisionByVersionAsExpressionStartsWithHash.cmmn", DECISION_OKAY_DMN })]
	  public virtual void testCallDecisionByVersionAsExpressionStartsWithHash()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(DECISION_NOT_OKAY_DMN).deploy().Id;

		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("myVersion", 2));

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("not okay", getDecisionResult(caseInstance));

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  [Deployment(resources : CMMN_CALL_DECISION_CONSTANT_WITH_MANUAL_ACTIVATION)]
	  public virtual void testDecisionNotFound()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string decisionTaskId = queryCaseExecutionByActivityId(DECISION_TASK).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(decisionTaskId).manualStart();
		  fail("It should not be possible to evaluate a not existing decision.");
		}
		catch (DecisionDefinitionNotFoundException)
		{
		}
	  }

	  [Deployment(resources : { CMMN_CALL_DECISION_CONSTANT, DECISION_POJO_DMN })]
	  public virtual void testPojo()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37));
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, variables);

		assertEquals("okay", getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { CMMN_CALL_DECISION_CONSTANT, DECISION_OKAY_DMN })]
	  public virtual void testIgnoreNonBlockingFlag()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY);

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("okay", getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { CMMN_CALL_DECISION_EXPRESSION_WITH_MANUAL_ACTIVATION, DECISION_LITERAL_EXPRESSION_DMN})]
	  public virtual void testCallDecisionWithLiteralExpression()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("testDecision", "decisionLiteralExpression").putValue("a", 2).putValue("b", 3));

		string decisionTaskId = queryCaseExecutionByActivityId(DECISION_TASK).Id;

		// when
		caseService.withCaseExecution(decisionTaskId).manualStart();

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals(5, getDecisionResult(caseInstance));
	  }

	  [Deployment(resources : { CMMN_CALL_DECISION_EXPRESSION, DRD_DISH_RESOURCE })]
	  public virtual void testCallDecisionWithRequiredDecisions()
	  {
		// given
		CaseInstance caseInstance = createCaseInstanceByKey(CASE_KEY, Variables.createVariables().putValue("testDecision", "dish-decision").putValue("temperature", 32).putValue("dayType", "Weekend"));

		// then
		assertNull(queryCaseExecutionByActivityId(DECISION_TASK));
		assertEquals("Light salad", getDecisionResult(caseInstance));
	  }

	  protected internal virtual object getDecisionResult(CaseInstance caseInstance)
	  {
		return caseService.getVariable(caseInstance.Id, "result");
	  }

	}

}