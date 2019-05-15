using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.dmn.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using org.camunda.bpm.engine.repository;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Dmn = org.camunda.bpm.model.dmn.Dmn;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;
	using HitPolicy = org.camunda.bpm.model.dmn.HitPolicy;
	using DmnModelConstants = org.camunda.bpm.model.dmn.impl.DmnModelConstants;
	using Decision = org.camunda.bpm.model.dmn.instance.Decision;
	using DecisionTable = org.camunda.bpm.model.dmn.instance.DecisionTable;
	using Definitions = org.camunda.bpm.model.dmn.instance.Definitions;
	using Input = org.camunda.bpm.model.dmn.instance.Input;
	using InputExpression = org.camunda.bpm.model.dmn.instance.InputExpression;
	using Output = org.camunda.bpm.model.dmn.instance.Output;
	using Text = org.camunda.bpm.model.dmn.instance.Text;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class DecisionDefinitionDeployerTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionDefinitionDeployerTest()
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


	  protected internal const string DMN_CHECK_ORDER_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDmnDeployment.dmn11.xml";
	  protected internal const string DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDmnDeployment.dmn";
	  protected internal const string DMN_SCORE_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/dmnScore.dmn11.xml";

	  protected internal const string DMN_DECISION_LITERAL_EXPRESSION = "org/camunda/bpm/engine/test/dmn/deployment/DecisionWithLiteralExpression.dmn";

	  protected internal const string DRD_SCORE_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string DRD_SCORE_V2_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdScore_v2.dmn11.xml";
	  protected internal const string DRD_DISH_RESOURCE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dmnDeployment()
	  public virtual void dmnDeployment()
	  {
		string deploymentId = testRule.deploy(DMN_CHECK_ORDER_RESOURCE).Id;

		// there should be decision deployment
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.count());

		// there should be one decision definition
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();
		assertEquals(1, query.count());

		DecisionDefinition decisionDefinition = query.singleResult();

		assertTrue(decisionDefinition.Id.StartsWith("decision:1:", StringComparison.Ordinal));
		assertEquals("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
		assertEquals("CheckOrder", decisionDefinition.Name);
		assertEquals("decision", decisionDefinition.Key);
		assertEquals(1, decisionDefinition.Version);
		assertEquals(DMN_CHECK_ORDER_RESOURCE, decisionDefinition.ResourceName);
		assertEquals(deploymentId, decisionDefinition.DeploymentId);
		assertNull(decisionDefinition.DiagramResourceName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dmnDeploymentWithDmnSuffix()
	  public virtual void dmnDeploymentWithDmnSuffix()
	  {
		string deploymentId = testRule.deploy(DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX).Id;

		// there should be one deployment
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.count());

		// there should be one case definition
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();
		assertEquals(1, query.count());

		DecisionDefinition decisionDefinition = query.singleResult();

		assertTrue(decisionDefinition.Id.StartsWith("decision:1:", StringComparison.Ordinal));
		assertEquals("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
		assertEquals("CheckOrder", decisionDefinition.Name);
		assertEquals("decision", decisionDefinition.Key);
		assertEquals(1, decisionDefinition.Version);
		assertEquals(DMN_CHECK_ORDER_RESOURCE_DMN_SUFFIX, decisionDefinition.ResourceName);
		assertEquals(deploymentId, decisionDefinition.DeploymentId);
		assertNull(decisionDefinition.DiagramResourceName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dmnDeploymentWithDecisionLiteralExpression()
	  public virtual void dmnDeploymentWithDecisionLiteralExpression()
	  {
		string deploymentId = testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION).Id;

		// there should be decision deployment
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();
		assertEquals(1, deploymentQuery.count());

		// there should be one decision definition
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();
		assertEquals(1, query.count());

		DecisionDefinition decisionDefinition = query.singleResult();

		assertTrue(decisionDefinition.Id.StartsWith("decisionLiteralExpression:1:", StringComparison.Ordinal));
		assertEquals("http://camunda.org/schema/1.0/dmn", decisionDefinition.Category);
		assertEquals("decisionLiteralExpression", decisionDefinition.Key);
		assertEquals("Decision with Literal Expression", decisionDefinition.Name);
		assertEquals(1, decisionDefinition.Version);
		assertEquals(DMN_DECISION_LITERAL_EXPRESSION, decisionDefinition.ResourceName);
		assertEquals(deploymentId, decisionDefinition.DeploymentId);
		assertNull(decisionDefinition.DiagramResourceName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void longDecisionDefinitionKey()
	  public virtual void longDecisionDefinitionKey()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		assertFalse(decisionDefinition.Id.StartsWith("o123456789", StringComparison.Ordinal));
		assertEquals("o123456789o123456789o123456789o123456789o123456789o123456789o123456789", decisionDefinition.Key);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void duplicateIdInDeployment()
	  public virtual void duplicateIdInDeployment()
	  {
		string resourceName1 = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDuplicateIdInDeployment.dmn11.xml";
		string resourceName2 = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDuplicateIdInDeployment2.dmn11.xml";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("duplicateDecision");

		repositoryService.createDeployment().addClasspathResource(resourceName1).addClasspathResource(resourceName2).name("duplicateIds").deploy();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDiagramResource.dmn11.xml", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDiagramResource.png" }) @Test public void getDecisionDiagramResource()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDiagramResource.dmn11.xml", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDiagramResource.png" })]
	  public virtual void getDecisionDiagramResource()
	  {
		string resourcePrefix = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDiagramResource";

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		assertEquals(resourcePrefix + ".dmn11.xml", decisionDefinition.ResourceName);
		assertEquals("decision", decisionDefinition.Key);

		string diagramResourceName = decisionDefinition.DiagramResourceName;
		assertEquals(resourcePrefix + ".png", diagramResourceName);

		Stream diagramStream = repositoryService.getResourceAsStream(decisionDefinition.DeploymentId, diagramResourceName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] diagramBytes = org.camunda.bpm.engine.impl.util.IoUtil.readInputStream(diagramStream, "diagram stream");
		sbyte[] diagramBytes = IoUtil.readInputStream(diagramStream, "diagram stream");
		assertEquals(2540, diagramBytes.Length);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.dmn11.xml", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision1.png", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision2.png", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision3.png" }) @Test public void multipleDiagramResourcesProvided()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.dmn11.xml", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision1.png", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision2.png", "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.decision3.png" })]
	  public virtual void multipleDiagramResourcesProvided()
	  {
		string resourcePrefix = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testMultipleDecisionDiagramResource.";

		DecisionDefinitionQuery decisionDefinitionQuery = repositoryService.createDecisionDefinitionQuery();
		assertEquals(3, decisionDefinitionQuery.count());

		foreach (DecisionDefinition decisionDefinition in decisionDefinitionQuery.list())
		{
		  assertEquals(resourcePrefix + decisionDefinition.Key + ".png", decisionDefinition.DiagramResourceName);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void drdDeployment()
	  public virtual void drdDeployment()
	  {
		string deploymentId = testRule.deploy(DRD_SCORE_RESOURCE).Id;

		// there should be one decision requirements definition
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();
		assertEquals(1, query.count());

		DecisionRequirementsDefinition decisionRequirementsDefinition = query.singleResult();

		assertTrue(decisionRequirementsDefinition.Id.StartsWith("score:1:", StringComparison.Ordinal));
		assertEquals("score", decisionRequirementsDefinition.Key);
		assertEquals("Score", decisionRequirementsDefinition.Name);
		assertEquals("test-drd-1", decisionRequirementsDefinition.Category);
		assertEquals(1, decisionRequirementsDefinition.Version);
		assertEquals(DRD_SCORE_RESOURCE, decisionRequirementsDefinition.ResourceName);
		assertEquals(deploymentId, decisionRequirementsDefinition.DeploymentId);
		assertNull(decisionRequirementsDefinition.DiagramResourceName);

		// both decisions should have a reference to the decision requirements definition
		IList<DecisionDefinition> decisions = repositoryService.createDecisionDefinitionQuery().orderByDecisionDefinitionKey().asc().list();
		assertEquals(2, decisions.Count);

		DecisionDefinition firstDecision = decisions[0];
		assertEquals("score-decision", firstDecision.Key);
		assertEquals(decisionRequirementsDefinition.Id, firstDecision.DecisionRequirementsDefinitionId);
		assertEquals("score", firstDecision.DecisionRequirementsDefinitionKey);

		DecisionDefinition secondDecision = decisions[1];
		assertEquals("score-result", secondDecision.Key);
		assertEquals(decisionRequirementsDefinition.Id, secondDecision.DecisionRequirementsDefinitionId);
		assertEquals("score", secondDecision.DecisionRequirementsDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_CHECK_ORDER_RESOURCE) @Test public void noDrdForSingleDecisionDeployment()
	  [Deployment(resources : DMN_CHECK_ORDER_RESOURCE)]
	  public virtual void noDrdForSingleDecisionDeployment()
	  {
		// when the DMN file contains only a single decision definition
		assertEquals(1, repositoryService.createDecisionDefinitionQuery().count());

		// then no decision requirements definition should be created
		assertEquals(0, repositoryService.createDecisionRequirementsDefinitionQuery().count());
		// and the decision should not be linked to a decision requirements definition
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();
		assertNull(decisionDefinition.DecisionRequirementsDefinitionId);
		assertNull(decisionDefinition.DecisionRequirementsDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DRD_SCORE_RESOURCE, DRD_DISH_RESOURCE }) @Test public void multipleDrdDeployment()
	  [Deployment(resources : { DRD_SCORE_RESOURCE, DRD_DISH_RESOURCE })]
	  public virtual void multipleDrdDeployment()
	  {
		// there should be two decision requirements definitions
		IList<DecisionRequirementsDefinition> decisionRequirementsDefinitions = repositoryService.createDecisionRequirementsDefinitionQuery().orderByDecisionRequirementsDefinitionCategory().asc().list();

		assertEquals(2, decisionRequirementsDefinitions.Count);
		assertEquals("score", decisionRequirementsDefinitions[0].Key);
		assertEquals("dish", decisionRequirementsDefinitions[1].Key);

		// the decisions should have a reference to the decision requirements definition
		IList<DecisionDefinition> decisions = repositoryService.createDecisionDefinitionQuery().orderByDecisionDefinitionCategory().asc().list();
		assertEquals(5, decisions.Count);
		assertEquals(decisionRequirementsDefinitions[0].Id, decisions[0].DecisionRequirementsDefinitionId);
		assertEquals(decisionRequirementsDefinitions[0].Id, decisions[1].DecisionRequirementsDefinitionId);
		assertEquals(decisionRequirementsDefinitions[1].Id, decisions[2].DecisionRequirementsDefinitionId);
		assertEquals(decisionRequirementsDefinitions[1].Id, decisions[3].DecisionRequirementsDefinitionId);
		assertEquals(decisionRequirementsDefinitions[1].Id, decisions[4].DecisionRequirementsDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void duplicateDrdIdInDeployment()
	  public virtual void duplicateDrdIdInDeployment()
	  {

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("definitions");

		repositoryService.createDeployment().addClasspathResource(DRD_SCORE_RESOURCE).addClasspathResource(DRD_SCORE_V2_RESOURCE).name("duplicateIds").deploy();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deployMultipleDecisionsWithSameDrdId()
	  public virtual void deployMultipleDecisionsWithSameDrdId()
	  {
		// when deploying two decision with the same drd id `definitions`
		testRule.deploy(DMN_SCORE_RESOURCE, DMN_CHECK_ORDER_RESOURCE);

		// then create two decision definitions and
		// ignore the duplicated drd id since no drd is created
		assertEquals(2, repositoryService.createDecisionDefinitionQuery().count());
		assertEquals(0, repositoryService.createDecisionRequirementsDefinitionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deployDecisionIndependentFromDrd()
	  public virtual void deployDecisionIndependentFromDrd()
	  {
		string deploymentIdDecision = testRule.deploy(DMN_SCORE_RESOURCE).Id;
		string deploymentIdDrd = testRule.deploy(DRD_SCORE_RESOURCE).Id;

		// there should be one decision requirements definition
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();
		assertEquals(1, query.count());

		DecisionRequirementsDefinition decisionRequirementsDefinition = query.singleResult();
		assertEquals(1, decisionRequirementsDefinition.Version);
		assertEquals(deploymentIdDrd, decisionRequirementsDefinition.DeploymentId);

		// and two deployed decisions with different versions
		IList<DecisionDefinition> decisions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("score-decision").orderByDecisionDefinitionVersion().asc().list();

		assertEquals(2, decisions.Count);

		DecisionDefinition firstDecision = decisions[0];
		assertEquals(1, firstDecision.Version);
		assertEquals(deploymentIdDecision, firstDecision.DeploymentId);
		assertNull(firstDecision.DecisionRequirementsDefinitionId);

		DecisionDefinition secondDecision = decisions[1];
		assertEquals(2, secondDecision.Version);
		assertEquals(deploymentIdDrd, secondDecision.DeploymentId);
		assertEquals(decisionRequirementsDefinition.Id,secondDecision.DecisionRequirementsDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDmnModelInstance() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployDmnModelInstance()
	  {
		// given
		DmnModelInstance dmnModelInstance = createDmnModelInstance();

		// when
		testRule.deploy(repositoryService.createDeployment().addModelInstance("foo.dmn", dmnModelInstance));

		// then
		assertNotNull(repositoryService.createDecisionDefinitionQuery().decisionDefinitionResourceName("foo.dmn").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDmnModelInstanceNegativeHistoryTimeToLive() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployDmnModelInstanceNegativeHistoryTimeToLive()
	  {
		// given
		DmnModelInstance dmnModelInstance = createDmnModelInstanceNegativeHistoryTimeToLive();

		try
		{
		  testRule.deploy(repositoryService.createDeployment().addModelInstance("foo.dmn", dmnModelInstance));
		  fail("Exception for negative time to live value is expected.");
		}
		catch (ProcessEngineException ex)
		{
		  assertTrue(ex.InnerException.Message.contains("negative value is not allowed"));
		}
	  }

	  protected internal static DmnModelInstance createDmnModelInstanceNegativeHistoryTimeToLive()
	  {
		DmnModelInstance modelInstance = Dmn.createEmptyModel();
		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.Id = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Name = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Namespace = DmnModelConstants.CAMUNDA_NS;
		modelInstance.Definitions = definitions;

		Decision decision = modelInstance.newInstance(typeof(Decision));
		decision.Id = "Decision-1";
		decision.Name = "foo";
		decision.CamundaHistoryTimeToLive = -5;
		modelInstance.Definitions.addChildElement(decision);

		return modelInstance;
	  }

	  protected internal static DmnModelInstance createDmnModelInstance()
	  {
		DmnModelInstance modelInstance = Dmn.createEmptyModel();
		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.Id = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Name = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Namespace = DmnModelConstants.CAMUNDA_NS;
		modelInstance.Definitions = definitions;

		Decision decision = modelInstance.newInstance(typeof(Decision));
		decision.Id = "Decision-1";
		decision.Name = "foo";
		decision.CamundaHistoryTimeToLive = 5;
		modelInstance.Definitions.addChildElement(decision);

		DecisionTable decisionTable = modelInstance.newInstance(typeof(DecisionTable));
		decisionTable.Id = DmnModelConstants.DMN_ELEMENT_DECISION_TABLE;
		decisionTable.HitPolicy = HitPolicy.FIRST;
		decision.addChildElement(decisionTable);

		Input input = modelInstance.newInstance(typeof(Input));
		input.Id = "Input-1";
		input.Label = "Input";
		decisionTable.addChildElement(input);

		InputExpression inputExpression = modelInstance.newInstance(typeof(InputExpression));
		inputExpression.Id = "InputExpression-1";
		Text inputExpressionText = modelInstance.newInstance(typeof(Text));
		inputExpressionText.TextContent = "input";
		inputExpression.Text = inputExpressionText;
		inputExpression.TypeRef = "string";
		input.InputExpression = inputExpression;

		Output output = modelInstance.newInstance(typeof(Output));
		output.Name = "output";
		output.Label = "Output";
		output.TypeRef = "string";
		decisionTable.addChildElement(output);

		return modelInstance;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployAndGetDecisionDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployAndGetDecisionDefinition()
	  {

		// given decision model
		DmnModelInstance dmnModelInstance = createDmnModelInstance();

		// when decision model is deployed
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addModelInstance("foo.dmn", dmnModelInstance);
		DeploymentWithDefinitions deployment = testRule.deploy(deploymentBuilder);

		// then deployment contains definition
		IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		assertEquals(1, deployedDecisionDefinitions.Count);
		assertNull(deployment.DeployedDecisionRequirementsDefinitions);
		assertNull(deployment.DeployedProcessDefinitions);
		assertNull(deployment.DeployedCaseDefinitions);

		// and persisted definition are equal to deployed definition
		DecisionDefinition persistedDecisionDef = repositoryService.createDecisionDefinitionQuery().decisionDefinitionResourceName("foo.dmn").singleResult();
		assertEquals(persistedDecisionDef.Id, deployedDecisionDefinitions[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployEmptyDecisionDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployEmptyDecisionDefinition()
	  {

		// given empty decision model
		DmnModelInstance modelInstance = Dmn.createEmptyModel();
		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.Id = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Name = DmnModelConstants.DMN_ELEMENT_DEFINITIONS;
		definitions.Namespace = DmnModelConstants.CAMUNDA_NS;
		modelInstance.Definitions = definitions;

		// when decision model is deployed
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addModelInstance("foo.dmn", modelInstance);
		DeploymentWithDefinitions deployment = testRule.deploy(deploymentBuilder);

		// then deployment contains no definitions
		assertNull(deployment.DeployedDecisionDefinitions);
		assertNull(deployment.DeployedDecisionRequirementsDefinitions);

		// and there are no persisted definitions
		assertNull(repositoryService.createDecisionDefinitionQuery().decisionDefinitionResourceName("foo.dmn").singleResult());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployAndGetDRDDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployAndGetDRDDefinition()
	  {

		// when decision requirement graph is deployed
		DeploymentWithDefinitions deployment = testRule.deploy(DRD_SCORE_RESOURCE);

		// then deployment contains definitions
		IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		assertEquals(2, deployedDecisionDefinitions.Count);

		IList<DecisionRequirementsDefinition> deployedDecisionRequirementsDefinitions = deployment.DeployedDecisionRequirementsDefinitions;
		assertEquals(1, deployedDecisionRequirementsDefinitions.Count);

		assertNull(deployment.DeployedProcessDefinitions);
		assertNull(deployment.DeployedCaseDefinitions);

		// and persisted definitions are equal to deployed definitions
		DecisionRequirementsDefinition persistedDecisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionResourceName(DRD_SCORE_RESOURCE).singleResult();
		assertEquals(persistedDecisionRequirementsDefinition.Id, deployedDecisionRequirementsDefinitions[0].Id);

		IList<DecisionDefinition> persistedDecisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionResourceName(DRD_SCORE_RESOURCE).list();
		assertEquals(deployedDecisionDefinitions.Count, persistedDecisionDefinitions.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDecisionDefinitionWithIntegerHistoryTimeToLive()
	  public virtual void testDeployDecisionDefinitionWithIntegerHistoryTimeToLive()
	  {
		// when
		DeploymentWithDefinitions deployment = testRule.deploy("org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDefinitionWithIntegerHistoryTimeToLive.dmn11.xml");

		// then
		IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		assertEquals(deployedDecisionDefinitions.Count, 1);
		int? historyTimeToLive = deployedDecisionDefinitions[0].HistoryTimeToLive;
		assertNotNull(historyTimeToLive);
		assertEquals((int) historyTimeToLive, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDecisionDefinitionWithStringHistoryTimeToLive()
	  public virtual void testDeployDecisionDefinitionWithStringHistoryTimeToLive()
	  {
		// when
		DeploymentWithDefinitions deployment = testRule.deploy("org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDefinitionWithStringHistoryTimeToLive.dmn11.xml");

		// then
		IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		assertEquals(deployedDecisionDefinitions.Count, 1);
		int? historyTimeToLive = deployedDecisionDefinitions[0].HistoryTimeToLive;
		assertNotNull(historyTimeToLive);
		assertEquals((int) historyTimeToLive, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDecisionDefinitionWithMalformedStringHistoryTimeToLive()
	  public virtual void testDeployDecisionDefinitionWithMalformedStringHistoryTimeToLive()
	  {
		try
		{
		  testRule.deploy("org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDefinitionWithMalformedHistoryTimeToLive.dmn11.xml");
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.InnerException.Message.contains("Cannot parse historyTimeToLive"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployDecisionDefinitionWithEmptyHistoryTimeToLive()
	  public virtual void testDeployDecisionDefinitionWithEmptyHistoryTimeToLive()
	  {
		  DeploymentWithDefinitions deployment = testRule.deploy("org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDecisionDefinitionWithEmptyHistoryTimeToLive.dmn11.xml");

		  // then
		  IList<DecisionDefinition> deployedDecisionDefinitions = deployment.DeployedDecisionDefinitions;
		  assertEquals(deployedDecisionDefinitions.Count, 1);
		  int? historyTimeToLive = deployedDecisionDefinitions[0].HistoryTimeToLive;
		  assertNull(historyTimeToLive);
	  }

	}

}