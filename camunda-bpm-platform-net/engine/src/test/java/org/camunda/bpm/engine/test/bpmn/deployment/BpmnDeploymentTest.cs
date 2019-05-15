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
namespace org.camunda.bpm.engine.test.bpmn.deployment
{

	using RepositoryServiceImpl = org.camunda.bpm.engine.impl.RepositoryServiceImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ReadOnlyProcessDefinition = org.camunda.bpm.engine.impl.pvm.ReadOnlyProcessDefinition;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Resource = org.camunda.bpm.engine.repository.Resource;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Test = org.junit.Test;


	/// <summary>
	/// @author Joram Barrez
	/// @author Thorben Lindhauer
	/// </summary>
	public class BpmnDeploymentTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetBpmnXmlFileThroughService()
	  public virtual void testGetBpmnXmlFileThroughService()
	  {
		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;
		IList<string> deploymentResources = repositoryService.getDeploymentResourceNames(deploymentId);

		// verify bpmn file name
		assertEquals(1, deploymentResources.Count);
		string bpmnResourceName = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml";
		assertEquals(bpmnResourceName, deploymentResources[0]);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertEquals(bpmnResourceName, processDefinition.ResourceName);
		assertNull(processDefinition.DiagramResourceName);
		assertFalse(processDefinition.hasStartFormKey());

		ReadOnlyProcessDefinition readOnlyProcessDefinition = ((RepositoryServiceImpl)repositoryService).getDeployedProcessDefinition(processDefinition.Id);
		assertNull(readOnlyProcessDefinition.DiagramResourceName);

		// verify content
		Stream deploymentInputStream = repositoryService.getResourceAsStream(deploymentId, bpmnResourceName);
		string contentFromDeployment = readInputStreamToString(deploymentInputStream);
		assertTrue(contentFromDeployment.Length > 0);
		assertTrue(contentFromDeployment.Contains("process id=\"emptyProcess\""));

		Stream fileInputStream = ReflectUtil.getResourceAsStream("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml");
		string contentFromFile = readInputStreamToString(fileInputStream);
		assertEquals(contentFromFile, contentFromDeployment);
	  }

	  private string readInputStreamToString(Stream inputStream)
	  {
		sbyte[] bytes = IoUtil.readInputStream(inputStream, "input stream");
		return StringHelper.NewString(bytes);
	  }

	  public virtual void FAILING_testViolateProcessDefinitionIdMaximumLength()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/deployment/processWithLongId.bpmn20.xml").deploy();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("id can be maximum 64 characters", e.Message);
		}

		// Verify that nothing is deployed
		assertEquals(0, repositoryService.createDeploymentQuery().count());
	  }

	  public virtual void testDeploySameFileTwice()
	  {
		string bpmnResourceName = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml";
		repositoryService.createDeployment().enableDuplicateFiltering().addClasspathResource(bpmnResourceName).name("twice").deploy();

		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;
		IList<string> deploymentResources = repositoryService.getDeploymentResourceNames(deploymentId);

		// verify bpmn file name
		assertEquals(1, deploymentResources.Count);
		assertEquals(bpmnResourceName, deploymentResources[0]);

		repositoryService.createDeployment().enableDuplicateFiltering().addClasspathResource(bpmnResourceName).name("twice").deploy();
		IList<org.camunda.bpm.engine.repository.Deployment> deploymentList = repositoryService.createDeploymentQuery().list();
		assertEquals(1, deploymentList.Count);

		repositoryService.deleteDeployment(deploymentId);
	  }

	  public virtual void testPartialChangesDeployAll()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();
		org.camunda.bpm.engine.repository.Deployment deployment1 = repositoryService.createDeployment().enableDuplicateFiltering().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).name("twice").deploy();

		IList<string> deploymentResources = repositoryService.getDeploymentResourceNames(deployment1.Id);
		assertEquals(2, deploymentResources.Count);

		BpmnModelInstance changedModel2 = Bpmn.createExecutableProcess("process2").startEvent().done();

		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().enableDuplicateFiltering().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", changedModel2).name("twice").deploy();
		IList<org.camunda.bpm.engine.repository.Deployment> deploymentList = repositoryService.createDeploymentQuery().list();
		assertEquals(2, deploymentList.Count);

		// there should be new versions of both processes
		assertEquals(2, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").count());

		repositoryService.deleteDeployment(deployment1.Id);
		repositoryService.deleteDeployment(deployment2.Id);
	  }

	  public virtual void testPartialChangesDeployChangedOnly()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();
		org.camunda.bpm.engine.repository.Deployment deployment1 = repositoryService.createDeployment().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).name("thrice").deploy();

		IList<string> deploymentResources = repositoryService.getDeploymentResourceNames(deployment1.Id);
		assertEquals(2, deploymentResources.Count);

		BpmnModelInstance changedModel2 = Bpmn.createExecutableProcess("process2").startEvent().done();

		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().enableDuplicateFiltering(true).addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", changedModel2).name("thrice").deploy();

		IList<org.camunda.bpm.engine.repository.Deployment> deploymentList = repositoryService.createDeploymentQuery().list();
		assertEquals(2, deploymentList.Count);

		// there should be only one version of process 1
		ProcessDefinition process1Definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult();
		assertNotNull(process1Definition);
		assertEquals(1, process1Definition.Version);
		assertEquals(deployment1.Id, process1Definition.DeploymentId);

		// there should be two versions of process 2
		assertEquals(2, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").count());

		BpmnModelInstance anotherChangedModel2 = Bpmn.createExecutableProcess("process2").startEvent().endEvent().done();

		// testing with a third deployment to ensure the change check is not only performed against
		// the last version of the deployment
		org.camunda.bpm.engine.repository.Deployment deployment3 = repositoryService.createDeployment().enableDuplicateFiltering(true).addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", anotherChangedModel2).name("thrice").deploy();

		// there should still be one version of process 1
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").count());

		// there should be three versions of process 2
		assertEquals(3, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").count());

		repositoryService.deleteDeployment(deployment1.Id);
		repositoryService.deleteDeployment(deployment2.Id);
		repositoryService.deleteDeployment(deployment3.Id);
	  }


	  public virtual void testPartialChangesRedeployOldVersion()
	  {
		// deployment 1 deploys process version 1
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		org.camunda.bpm.engine.repository.Deployment deployment1 = repositoryService.createDeployment().addModelInstance("process1.bpmn20.xml", model1).name("deployment").deploy();

		// deployment 2 deploys process version 2
		BpmnModelInstance changedModel1 = Bpmn.createExecutableProcess("process1").startEvent().done();
		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().enableDuplicateFiltering(true).addModelInstance("process1.bpmn20.xml", changedModel1).name("deployment").deploy();

		// deployment 3 deploys process version 1 again
		org.camunda.bpm.engine.repository.Deployment deployment3 = repositoryService.createDeployment().enableDuplicateFiltering(true).addModelInstance("process1.bpmn20.xml", model1).name("deployment").deploy();

		// should result in three process definitions
		assertEquals(3, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").count());

		repositoryService.deleteDeployment(deployment1.Id);
		repositoryService.deleteDeployment(deployment2.Id);
		repositoryService.deleteDeployment(deployment3.Id);
	  }

	  public virtual void testDeployTwoProcessesWithDuplicateIdAtTheSameTime()
	  {
		try
		{
		  string bpmnResourceName = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml";
		  string bpmnResourceName2 = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService2.bpmn20.xml";
		  repositoryService.createDeployment().enableDuplicateFiltering().addClasspathResource(bpmnResourceName).addClasspathResource(bpmnResourceName2).name("duplicateAtTheSameTime").deploy();
		  fail();
		}
		catch (Exception)
		{
		  // Verify that nothing is deployed
		  assertEquals(0, repositoryService.createDeploymentQuery().count());
		}
	  }

	  public virtual void testDeployDifferentFiles()
	  {
		string bpmnResourceName = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml";
		repositoryService.createDeployment().enableDuplicateFiltering(false).addClasspathResource(bpmnResourceName).name("twice").deploy();

		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;
		IList<string> deploymentResources = repositoryService.getDeploymentResourceNames(deploymentId);

		// verify bpmn file name
		assertEquals(1, deploymentResources.Count);
		assertEquals(bpmnResourceName, deploymentResources[0]);

		bpmnResourceName = "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.bpmn20.xml";
		repositoryService.createDeployment().enableDuplicateFiltering().addClasspathResource(bpmnResourceName).name("twice").deploy();
		IList<org.camunda.bpm.engine.repository.Deployment> deploymentList = repositoryService.createDeploymentQuery().list();
		assertEquals(2, deploymentList.Count);

		deleteDeployments(deploymentList);
	  }

	  public virtual void testDiagramCreationDisabled()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/parse/BpmnParseTest.testParseDiagramInterchangeElements.bpmn20.xml").deploy();

		// Graphical information is not yet exposed publicly, so we need to do some plumbing
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		ProcessDefinitionEntity processDefinitionEntity = commandExecutor.execute(new CommandAnonymousInnerClass(this));

		assertNotNull(processDefinitionEntity);
		assertEquals(7, processDefinitionEntity.Activities.Count);

		// Check that no diagram has been created
		IList<string> resourceNames = repositoryService.getDeploymentResourceNames(processDefinitionEntity.DeploymentId);
		assertEquals(1, resourceNames.Count);

		repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
	  }

	  private class CommandAnonymousInnerClass : Command<ProcessDefinitionEntity>
	  {
		  private readonly BpmnDeploymentTest outerInstance;

		  public CommandAnonymousInnerClass(BpmnDeploymentTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessDefinitionEntity execute(CommandContext commandContext)
		  {
			return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedLatestProcessDefinitionByKey("myProcess");
		  }
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.jpg" })]
	  public virtual void testProcessDiagramResource()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		assertEquals("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.bpmn20.xml", processDefinition.ResourceName);
		assertTrue(processDefinition.hasStartFormKey());

		string diagramResourceName = processDefinition.DiagramResourceName;
		assertEquals("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.jpg", diagramResourceName);

		Stream diagramStream = repositoryService.getResourceAsStream(deploymentId, "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testProcessDiagramResource.jpg");
		sbyte[] diagramBytes = IoUtil.readInputStream(diagramStream, "diagram stream");
		assertEquals(33343, diagramBytes.Length);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.a.jpg", "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.b.jpg", "org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.c.jpg" })]
	  public virtual void testMultipleDiagramResourcesProvided()
	  {
		ProcessDefinition processA = repositoryService.createProcessDefinitionQuery().processDefinitionKey("a").singleResult();
		ProcessDefinition processB = repositoryService.createProcessDefinitionQuery().processDefinitionKey("b").singleResult();
		ProcessDefinition processC = repositoryService.createProcessDefinitionQuery().processDefinitionKey("c").singleResult();

		assertEquals("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.a.jpg", processA.DiagramResourceName);
		assertEquals("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.b.jpg", processB.DiagramResourceName);
		assertEquals("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testMultipleDiagramResourcesProvided.c.jpg", processC.DiagramResourceName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessDefinitionDescription()
	  public virtual void testProcessDefinitionDescription()
	  {
		string id = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		ReadOnlyProcessDefinition processDefinition = ((RepositoryServiceImpl) repositoryService).getDeployedProcessDefinition(id);
		assertEquals("This is really good process documentation!", processDefinition.Description);
	  }

	  public virtual void testDeployInvalidExpression()
	  {
		// ACT-1391: Deploying a process with invalid expressions inside should cause the deployment to fail, since
		// the process is not deployed and useless...
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testInvalidExpression.bpmn20.xml").deploy();

		  fail("Expected exception when deploying process with invalid expression.");
		}
		catch (ProcessEngineException expected)
		{
		  // Check if no deployments are made
		  assertEquals(0, repositoryService.createDeploymentQuery().count());
		  assertTextPresent("ENGINE-01009 Error while parsing process", expected.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/deployment/BpmnDeploymentTest.testGetBpmnXmlFileThroughService.bpmn20.xml"})]
	  public virtual void testDeploymentIdOfResource()
	  {
		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		IList<Resource> resources = repositoryService.getDeploymentResources(deploymentId);
		assertEquals(1, resources.Count);

		Resource resource = resources[0];
		assertEquals(deploymentId, resource.DeploymentId);
	  }

	  private void deleteDeployments(IList<org.camunda.bpm.engine.repository.Deployment> deploymentList)
	  {
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in deploymentList)
		{
		  repositoryService.deleteDeployment(deployment.Id);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployBpmnModelInstance() throws Exception
	  public virtual void testDeployBpmnModelInstance()
	  {

		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("foo").startEvent().userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().userTask().endEvent().done();

		// when
		deploymentWithBuilder(repositoryService.createDeployment().addModelInstance("foo.bpmn", modelInstance));

		// then
		assertNotNull(repositoryService.createProcessDefinitionQuery().processDefinitionResourceName("foo.bpmn").singleResult());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployAndGetProcessDefinition() throws Exception
	  public virtual void testDeployAndGetProcessDefinition()
	  {

		// given process model
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("foo").startEvent().userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent().userTask().endEvent().done();

		// when process model is deployed
		DeploymentWithDefinitions deployment = repositoryService.createDeployment().addModelInstance("foo.bpmn", modelInstance).deployWithResult();
		deploymentIds.Add(deployment.Id);

		// then deployment contains deployed process definitions
		IList<ProcessDefinition> deployedProcessDefinitions = deployment.DeployedProcessDefinitions;
		assertEquals(1, deployedProcessDefinitions.Count);
		assertNull(deployment.DeployedCaseDefinitions);
		assertNull(deployment.DeployedDecisionDefinitions);
		assertNull(deployment.DeployedDecisionRequirementsDefinitions);

		// and persisted process definition is equal to deployed process definition
		ProcessDefinition persistedProcDef = repositoryService.createProcessDefinitionQuery().processDefinitionResourceName("foo.bpmn").singleResult();
		assertEquals(persistedProcDef.Id, deployedProcessDefinitions[0].Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployNonExecutableProcess() throws Exception
	  public virtual void testDeployNonExecutableProcess()
	  {

		// given non executable process definition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createProcess("foo").startEvent().userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createProcess("foo").startEvent().userTask().endEvent().done();

		// when process model is deployed
		DeploymentWithDefinitions deployment = repositoryService.createDeployment().addModelInstance("foo.bpmn", modelInstance).deployWithResult();
		deploymentIds.Add(deployment.Id);

		// then deployment contains no deployed process definition
		assertNull(deployment.DeployedProcessDefinitions);

		// and there exist no persisted process definitions
		assertNull(repositoryService.createProcessDefinitionQuery().processDefinitionResourceName("foo.bpmn").singleResult());
	  }

	}

}