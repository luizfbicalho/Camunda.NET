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
namespace org.camunda.bpm.engine.test.cmmn.deployment
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using org.camunda.bpm.engine.repository;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnDeployerTest : PluggableProcessEngineTestCase
	{

	  public virtual void testCmmnDeployment()
	  {
		string deploymentId = processEngine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testSimpleDeployment.cmmn").deploy().Id;

		// there should be one deployment
		RepositoryService repositoryService = processEngine.RepositoryService;
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.count());

		// there should be one case definition
		CaseDefinitionQuery query = processEngine.RepositoryService.createCaseDefinitionQuery();
		assertEquals(1, query.count());

		CaseDefinition caseDefinition = query.singleResult();
		assertEquals("Case_1", caseDefinition.Key);

		processEngine.RepositoryService.deleteDeployment(deploymentId);
	  }

	  public virtual void testDeployTwoCasesWithDuplicateIdAtTheSameTime()
	  {
		try
		{
		  string cmmnResourceName1 = "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testSimpleDeployment.cmmn";
		  string cmmnResourceName2 = "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testSimpleDeployment2.cmmn";
		  repositoryService.createDeployment().addClasspathResource(cmmnResourceName1).addClasspathResource(cmmnResourceName2).name("duplicateAtTheSameTime").deploy();
		  fail();
		}
		catch (Exception)
		{
		  // Verify that nothing is deployed
		  assertEquals(0, repositoryService.createDeploymentQuery().count());
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.cmmn", "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.png" })]
	  public virtual void testCaseDiagramResource()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		assertEquals("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.cmmn", caseDefinition.ResourceName);
		assertEquals("Case_1", caseDefinition.Key);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String diagramResourceName = caseDefinition.getDiagramResourceName();
		string diagramResourceName = caseDefinition.DiagramResourceName;
		assertEquals("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.png", diagramResourceName);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream diagramStream = repositoryService.getResourceAsStream(deploymentId, "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.png");
		Stream diagramStream = repositoryService.getResourceAsStream(deploymentId, "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testCaseDiagramResource.png");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] diagramBytes = org.camunda.bpm.engine.impl.util.IoUtil.readInputStream(diagramStream, "diagram stream");
		sbyte[] diagramBytes = IoUtil.readInputStream(diagramStream, "diagram stream");
		assertEquals(2540, diagramBytes.Length);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.cmmn", "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.a.png", "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.b.png", "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.c.png" })]
	  public virtual void testMultipleDiagramResourcesProvided()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CaseDefinition caseA = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("a").singleResult();
		CaseDefinition caseA = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("a").singleResult();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CaseDefinition caseB = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("b").singleResult();
		CaseDefinition caseB = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("b").singleResult();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CaseDefinition caseC = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("c").singleResult();
		CaseDefinition caseC = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("c").singleResult();

		assertEquals("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.a.png", caseA.DiagramResourceName);
		assertEquals("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.b.png", caseB.DiagramResourceName);
		assertEquals("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testMultipleDiagramResourcesProvided.c.png", caseC.DiagramResourceName);
	  }

	  public virtual void testDeployCmmn10XmlFile()
	  {
		verifyCmmnResourceDeployed("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testDeployCmmn10XmlFile.cmmn10.xml");

	  }

	  public virtual void testDeployCmmn11XmlFile()
	  {
		verifyCmmnResourceDeployed("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testDeployCmmn11XmlFile.cmmn11.xml");
	  }

	  protected internal virtual void verifyCmmnResourceDeployed(string resourcePath)
	  {
		string deploymentId = processEngine.RepositoryService.createDeployment().addClasspathResource(resourcePath).deploy().Id;

		// there should be one deployment
		RepositoryService repositoryService = processEngine.RepositoryService;
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.count());

		// there should be one case definition
		CaseDefinitionQuery query = processEngine.RepositoryService.createCaseDefinitionQuery();
		assertEquals(1, query.count());

		CaseDefinition caseDefinition = query.singleResult();
		assertEquals("Case_1", caseDefinition.Key);

		processEngine.RepositoryService.deleteDeployment(deploymentId);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployCmmnModelInstance() throws Exception
	  public virtual void testDeployCmmnModelInstance()
	  {
		// given
		CmmnModelInstance modelInstance = createCmmnModelInstance();

		// when
		deploymentWithBuilder(repositoryService.createDeployment().addModelInstance("foo.cmmn", modelInstance));

		// then
		assertNotNull(repositoryService.createCaseDefinitionQuery().caseDefinitionResourceName("foo.cmmn").singleResult());
	  }

	  protected internal static CmmnModelInstance createCmmnModelInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.cmmn.CmmnModelInstance modelInstance = org.camunda.bpm.model.cmmn.Cmmn.createEmptyModel();
		CmmnModelInstance modelInstance = Cmmn.createEmptyModel();
		org.camunda.bpm.model.cmmn.instance.Definitions definitions = modelInstance.newInstance(typeof(org.camunda.bpm.model.cmmn.instance.Definitions));
		definitions.TargetNamespace = "http://camunda.org/examples";
		modelInstance.Definitions = definitions;

		Case caseElement = modelInstance.newInstance(typeof(Case));
		caseElement.Id = "a-case";
		definitions.addChildElement(caseElement);

		CasePlanModel casePlanModel = modelInstance.newInstance(typeof(CasePlanModel));
		caseElement.CasePlanModel = casePlanModel;

		Cmmn.writeModelToStream(System.out, modelInstance);

		return modelInstance;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployAndGetCaseDefinition() throws Exception
	  public virtual void testDeployAndGetCaseDefinition()
	  {
		// given case model
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.cmmn.CmmnModelInstance modelInstance = createCmmnModelInstance();
		CmmnModelInstance modelInstance = createCmmnModelInstance();

		// when case model is deployed
		DeploymentWithDefinitions deployment = repositoryService.createDeployment().addModelInstance("foo.cmmn", modelInstance).deployWithResult();
		deploymentIds.Add(deployment.Id);

		// then deployment contains deployed case definition
		IList<CaseDefinition> deployedCaseDefinitions = deployment.DeployedCaseDefinitions;
		assertEquals(1, deployedCaseDefinitions.Count);
		assertNull(deployment.DeployedProcessDefinitions);
		assertNull(deployment.DeployedDecisionDefinitions);
		assertNull(deployment.DeployedDecisionRequirementsDefinitions);

		// and persisted case definition is equal to deployed case definition
		CaseDefinition persistedCaseDefinition = repositoryService.createCaseDefinitionQuery().caseDefinitionResourceName("foo.cmmn").singleResult();
		assertEquals(persistedCaseDefinition.Id, deployedCaseDefinitions[0].Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployEmptyCaseDefinition() throws Exception
	  public virtual void testDeployEmptyCaseDefinition()
	  {

		// given empty case model
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.cmmn.CmmnModelInstance modelInstance = org.camunda.bpm.model.cmmn.Cmmn.createEmptyModel();
		CmmnModelInstance modelInstance = Cmmn.createEmptyModel();
		org.camunda.bpm.model.cmmn.instance.Definitions definitions = modelInstance.newInstance(typeof(org.camunda.bpm.model.cmmn.instance.Definitions));
		definitions.TargetNamespace = "http://camunda.org/examples";
		modelInstance.Definitions = definitions;

		// when case model is deployed
		DeploymentWithDefinitions deployment = repositoryService.createDeployment().addModelInstance("foo.cmmn", modelInstance).deployWithResult();
		deploymentIds.Add(deployment.Id);

		// then no case definition is deployed
		assertNull(deployment.DeployedCaseDefinitions);

		// and there exist not persisted case definition
		assertNull(repositoryService.createCaseDefinitionQuery().caseDefinitionResourceName("foo.cmmn").singleResult());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testDeployCaseDefinitionWithIntegerHistoryTimeToLive.cmmn")]
	  public virtual void testDeployCaseDefinitionWithIntegerHistoryTimeToLive()
	  {
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		int? historyTimeToLive = caseDefinition.HistoryTimeToLive;
		assertNotNull(historyTimeToLive);
		assertEquals((int) historyTimeToLive, 5);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testDeployCaseDefinitionWithStringHistoryTimeToLive.cmmn")]
	  public virtual void testDeployCaseDefinitionWithStringHistoryTimeToLive()
	  {
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		int? historyTimeToLive = caseDefinition.HistoryTimeToLive;
		assertNotNull(historyTimeToLive);
		assertEquals((int) historyTimeToLive, 5);
	  }

	  public virtual void testDeployCaseDefinitionWithMalformedHistoryTimeToLive()
	  {
		try
		{
		  deployment("org/camunda/bpm/engine/test/cmmn/deployment/CmmnDeploymentTest.testDeployCaseDefinitionWithMalformedHistoryTimeToLive.cmmn");
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.InnerException.Message.contains("Cannot parse historyTimeToLive"));
		}
	  }
	}

}