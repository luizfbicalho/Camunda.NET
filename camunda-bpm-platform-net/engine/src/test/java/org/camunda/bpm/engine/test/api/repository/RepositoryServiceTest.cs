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
namespace org.camunda.bpm.engine.test.api.repository
{
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using RepositoryServiceImpl = org.camunda.bpm.engine.impl.RepositoryServiceImpl;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using UserOperationLogEntryEventEntity = org.camunda.bpm.engine.impl.history.@event.UserOperationLogEntryEventEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ReadOnlyProcessDefinition = org.camunda.bpm.engine.impl.pvm.ReadOnlyProcessDefinition;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderTaskListener = org.camunda.bpm.engine.test.bpmn.tasklistener.util.RecorderTaskListener;
	using TestExecutionListener = org.camunda.bpm.engine.test.util.TestExecutionListener;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;


	/// <summary>
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// @author Roman Smirnov
	/// </summary>
	public class RepositoryServiceTest : PluggableProcessEngineTestCase
	{

	  private const string NAMESPACE = "xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL'";
	  private static readonly string TARGET_NAMESPACE = "targetNamespace='" + BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS + "'";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly RepositoryServiceTest outerInstance;

		  public CommandAnonymousInnerClass(RepositoryServiceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkDeployedBytes(java.io.InputStream deployedResource, byte[] utf8Bytes) throws java.io.IOException
	  private void checkDeployedBytes(Stream deployedResource, sbyte[] utf8Bytes)
	  {
		sbyte[] deployedBytes = new sbyte[utf8Bytes.Length];
		deployedResource.Read(deployedBytes, 0, deployedBytes.Length);

		for (int i = 0; i < utf8Bytes.Length; i++)
		{
		  assertEquals(utf8Bytes[i], deployedBytes[i]);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testUTF8DeploymentMethod() throws java.io.IOException
	  public virtual void testUTF8DeploymentMethod()
	  {
		//given utf8 charset
		Charset utf8Charset = Charset.forName("UTF-8");
		Charset defaultCharset = processEngineConfiguration.DefaultCharset;
		processEngineConfiguration.DefaultCharset = utf8Charset;

		//and model instance with umlauts
		string umlautsString = "äöüÄÖÜß";
		string resourceName = "deployment.bpmn";
		BpmnModelInstance instance = Bpmn.createExecutableProcess("umlautsProcess").startEvent(umlautsString).done();
		string instanceAsString = Bpmn.convertToString(instance);

		//when instance is deployed via addString method
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addString(resourceName, instanceAsString).deploy();

		//then bytes are saved in utf-8 format
		Stream inputStream = repositoryService.getResourceAsStream(deployment.Id, resourceName);
		sbyte[] utf8Bytes = instanceAsString.GetBytes(utf8Charset);
		checkDeployedBytes(inputStream, utf8Bytes);
		repositoryService.deleteDeployment(deployment.Id);


		//when model instance is deployed via addModelInstance method
		deployment = repositoryService.createDeployment().addModelInstance(resourceName, instance).deploy();

		//then also the bytes are saved in utf-8 format
		inputStream = repositoryService.getResourceAsStream(deployment.Id, resourceName);
		checkDeployedBytes(inputStream, utf8Bytes);

		repositoryService.deleteDeployment(deployment.Id);
		processEngineConfiguration.DefaultCharset = defaultCharset;
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartProcessInstanceById()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, processDefinitions.Count);

		ProcessDefinition processDefinition = processDefinitions[0];
		assertEquals("oneTaskProcess", processDefinition.Key);
		assertNotNull(processDefinition.Id);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testFindProcessDefinitionById()
	  {
		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, definitions.Count);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionId(definitions[0].Id).singleResult();
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertNotNull(processDefinition);
		assertEquals("oneTaskProcess", processDefinition.Key);
		assertEquals("The One Task Process", processDefinition.Name);

		processDefinition = repositoryService.getProcessDefinition(definitions[0].Id);
		assertEquals("This is a process for testing purposes", processDefinition.Description);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteDeploymentWithRunningInstances()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, processDefinitions.Count);
		ProcessDefinition processDefinition = processDefinitions[0];

		runtimeService.startProcessInstanceById(processDefinition.Id);

		// Try to delete the deployment
		try
		{
		  repositoryService.deleteDeployment(processDefinition.DeploymentId);
		  fail("Exception expected");
		}
		catch (ProcessEngineException pee)
		{
		  // Exception expected when deleting deployment with running process
		  assert(pee.Message.contains("Deletion of process definition without cascading failed."));
		}
	  }

	  public virtual void testDeleteDeploymentSkipCustomListeners()
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeleteProcessInstanceSkipCustomListeners.bpmn20.xml");

		string deploymentId = deploymentBuilder.deploy().Id;

		runtimeService.startProcessInstanceByKey("testProcess");

		repositoryService.deleteDeployment(deploymentId, true, false);
		assertEquals(1, TestExecutionListener.collectedEvents.Count);
		TestExecutionListener.reset();

		deploymentId = deploymentBuilder.deploy().Id;

		runtimeService.startProcessInstanceByKey("testProcess");

		repositoryService.deleteDeployment(deploymentId, true, true);
		assertTrue(TestExecutionListener.collectedEvents.Count == 0);
		TestExecutionListener.reset();

	  }

	  public virtual void testDeleteDeploymentSkipCustomTaskListeners()
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeleteProcessInstanceSkipCustomTaskListeners.bpmn20.xml");

		string deploymentId = deploymentBuilder.deploy().Id;

		runtimeService.startProcessInstanceByKey("testProcess");

		RecorderTaskListener.RecordedEvents.Clear();

		repositoryService.deleteDeployment(deploymentId, true, false);
		assertEquals(1, RecorderTaskListener.RecordedEvents.Count);
		RecorderTaskListener.clear();

		deploymentId = deploymentBuilder.deploy().Id;

		runtimeService.startProcessInstanceByKey("testProcess");

		repositoryService.deleteDeployment(deploymentId, true, true);
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);
		RecorderTaskListener.clear();
	  }

	  public virtual void testDeleteDeploymentSkipIoMappings()
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeleteDeploymentSkipIoMappings.bpmn20.xml");

		string deploymentId = deploymentBuilder.deploy().Id;
		runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// Try to delete the deployment
		try
		{
		  repositoryService.deleteDeployment(deploymentId, true, false, true);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Exception is not expected when deleting deployment with running process", e);
		}
	  }

	  public virtual void testDeleteDeploymentWithoutSkipIoMappings()
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeleteDeploymentSkipIoMappings.bpmn20.xml");

		string deploymentId = deploymentBuilder.deploy().Id;
		runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// Try to delete the deployment
		try
		{
		  repositoryService.deleteDeployment(deploymentId, true, false, false);
		  fail("Exception expected");
		}
		catch (Exception e)
		{
		  // Exception expected when deleting deployment with running process
		  // assert (e.getMessage().contains("Exception when output mapping is executed"));
		  assertTextPresent("Exception when output mapping is executed", e.Message);
		}

		repositoryService.deleteDeployment(deploymentId, true, false, true);
	  }

	  public virtual void testDeleteDeploymentNullDeploymentId()
	  {
		try
		{
		  repositoryService.deleteDeployment(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("deploymentId is null", ae.Message);
		}
	  }

	  public virtual void testDeleteDeploymentCascadeNullDeploymentId()
	  {
		try
		{
		  repositoryService.deleteDeployment(null, true);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("deploymentId is null", ae.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteDeploymentCascadeWithRunningInstances()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, processDefinitions.Count);
		ProcessDefinition processDefinition = processDefinitions[0];

		runtimeService.startProcessInstanceById(processDefinition.Id);

		// Try to delete the deployment, no exception should be thrown
		repositoryService.deleteDeployment(processDefinition.DeploymentId, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/repository/one.cmmn"})]
	  public virtual void testDeleteDeploymentClearsCache()
	  {

		// fetch definition ids
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;
		// fetch CMMN model to be placed to in the cache
		repositoryService.getCmmnModelInstance(caseDefinitionId);

		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// ensure definitions and models are part of the cache
		assertNotNull(deploymentCache.ProcessDefinitionCache.get(processDefinitionId));
		assertNotNull(deploymentCache.BpmnModelInstanceCache.get(processDefinitionId));
		assertNotNull(deploymentCache.CaseDefinitionCache.get(caseDefinitionId));
		assertNotNull(deploymentCache.CmmnModelInstanceCache.get(caseDefinitionId));

		// when the deployment is deleted
		repositoryService.deleteDeployment(deploymentId, true);

		// then the definitions and models are removed from the cache
		assertNull(deploymentCache.ProcessDefinitionCache.get(processDefinitionId));
		assertNull(deploymentCache.BpmnModelInstanceCache.get(processDefinitionId));
		assertNull(deploymentCache.CaseDefinitionCache.get(caseDefinitionId));
		assertNull(deploymentCache.CmmnModelInstanceCache.get(caseDefinitionId));
	  }

	  public virtual void testFindDeploymentResourceNamesNullDeploymentId()
	  {
		try
		{
		  repositoryService.getDeploymentResourceNames(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("deploymentId is null", ae.Message);
		}
	  }

	  public virtual void testFindDeploymentResourcesNullDeploymentId()
	  {
		try
		{
		  repositoryService.getDeploymentResources(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("deploymentId is null", e.Message);
		}
	  }

	  public virtual void testDeploymentWithDelayedProcessDefinitionActivation()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		DateTime inThreeDays = new DateTime(startTime.Ticks + (3 * 24 * 60 * 60 * 1000));

		// Deploy process, but activate after three days
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml").activateProcessDefinitionsOn(inThreeDays).deploy();

		assertEquals(1, repositoryService.createDeploymentQuery().count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());

		// Shouldn't be able to start a process instance
		try
		{
		  runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("suspended", e.Message);
		}

		IList<Job> jobs = managementService.createJobQuery().list();
		managementService.executeJob(jobs[0].Id);
		managementService.executeJob(jobs[1].Id);

		assertEquals(1, repositoryService.createDeploymentQuery().count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().active().count());

		// Should be able to start process instance
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// Cleanup
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testDeploymentWithDelayedProcessDefinitionAndJobDefinitionActivation()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		DateTime inThreeDays = new DateTime(startTime.Ticks + (3 * 24 * 60 * 60 * 1000));

		// Deploy process, but activate after three days
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/oneAsyncTask.bpmn").activateProcessDefinitionsOn(inThreeDays).deploy();

		assertEquals(1, repositoryService.createDeploymentQuery().count());

		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());

		assertEquals(1, managementService.createJobDefinitionQuery().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());
		assertEquals(0, managementService.createJobDefinitionQuery().active().count());

		// Shouldn't be able to start a process instance
		try
		{
		  runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("suspended", e.Message);
		}

		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		assertEquals(1, repositoryService.createDeploymentQuery().count());

		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());

		assertEquals(1, managementService.createJobDefinitionQuery().count());
		assertEquals(0, managementService.createJobDefinitionQuery().suspended().count());
		assertEquals(1, managementService.createJobDefinitionQuery().active().count());

		// Should be able to start process instance
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// Cleanup
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testGetResourceAsStreamUnexistingResourceInExistingDeployment()
	  {
		// Get hold of the deployment id
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		try
		{
		  repositoryService.getResourceAsStream(deployment.Id, "org/camunda/bpm/engine/test/api/unexistingProcess.bpmn.xml");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("no resource found with name", ae.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testGetResourceAsStreamUnexistingDeployment()
	  {

		try
		{
		  repositoryService.getResourceAsStream("unexistingdeployment", "org/camunda/bpm/engine/test/api/unexistingProcess.bpmn.xml");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("no resource found with name", ae.Message);
		}
	  }


	  public virtual void testGetResourceAsStreamNullArguments()
	  {
		try
		{
		  repositoryService.getResourceAsStream(null, "resource");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("deploymentId is null", ae.Message);
		}

		try
		{
		  repositoryService.getResourceAsStream("deployment", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("resourceName is null", ae.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/one.cmmn" })]
	  public virtual void testGetCaseDefinition()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		CaseDefinition caseDefinition = query.singleResult();
		string caseDefinitionId = caseDefinition.Id;

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinitionId);

		assertNotNull(definition);
		assertEquals(caseDefinitionId, definition.Id);
	  }

	  public virtual void testGetCaseDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.getCaseDefinition("invalid");
		}
		catch (NotFoundException e)
		{
		  assertTextPresent("no deployed case definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getCaseDefinition(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("caseDefinitionId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/repository/one.cmmn" }) public void testGetCaseModel() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/one.cmmn" })]
	  public virtual void testGetCaseModel()
	  {
		CaseDefinitionQuery query = repositoryService.createCaseDefinitionQuery();

		CaseDefinition caseDefinition = query.singleResult();
		string caseDefinitionId = caseDefinition.Id;

		Stream caseModel = repositoryService.getCaseModel(caseDefinitionId);

		assertNotNull(caseModel);

		sbyte[] readInputStream = IoUtil.readInputStream(caseModel, "caseModel");
		string model = StringHelper.NewString(readInputStream, "UTF-8");

		assertTrue(model.Contains("<case id=\"one\" name=\"One\">"));

		IoUtil.closeSilently(caseModel);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testGetCaseModelByInvalidId() throws Exception
	  public virtual void testGetCaseModelByInvalidId()
	  {
		try
		{
		  repositoryService.getCaseModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no deployed case definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getCaseModel(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("caseDefinitionId is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/one.dmn" })]
	  public virtual void testGetDecisionDefinition()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		DecisionDefinition decisionDefinition = query.singleResult();
		string decisionDefinitionId = decisionDefinition.Id;

		DecisionDefinition definition = repositoryService.getDecisionDefinition(decisionDefinitionId);

		assertNotNull(definition);
		assertEquals(decisionDefinitionId, definition.Id);
	  }

	  public virtual void testGetDecisionDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.getDecisionDefinition("invalid");
		  fail();
		}
		catch (NotFoundException e)
		{
		  assertTextPresent("no deployed decision definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getDecisionDefinition(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("decisionDefinitionId is null", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/drg.dmn" })]
	  public virtual void testGetDecisionRequirementsDefinition()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		DecisionRequirementsDefinition decisionRequirementsDefinition = query.singleResult();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		DecisionRequirementsDefinition definition = repositoryService.getDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

		assertNotNull(definition);
		assertEquals(decisionRequirementsDefinitionId, definition.Id);
	  }

	  public virtual void testGetDecisionRequirementsDefinitionByInvalidId()
	  {
		try
		{
		  repositoryService.getDecisionRequirementsDefinition("invalid");
		  fail();
		}
		catch (Exception e)
		{
		  assertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getDecisionRequirementsDefinition(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/repository/one.dmn" }) public void testGetDecisionModel() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/one.dmn" })]
	  public virtual void testGetDecisionModel()
	  {
		DecisionDefinitionQuery query = repositoryService.createDecisionDefinitionQuery();

		DecisionDefinition decisionDefinition = query.singleResult();
		string decisionDefinitionId = decisionDefinition.Id;

		Stream decisionModel = repositoryService.getDecisionModel(decisionDefinitionId);

		assertNotNull(decisionModel);

		sbyte[] readInputStream = IoUtil.readInputStream(decisionModel, "decisionModel");
		string model = StringHelper.NewString(readInputStream, "UTF-8");

		assertTrue(model.Contains("<decision id=\"one\" name=\"One\">"));

		IoUtil.closeSilently(decisionModel);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testGetDecisionModelByInvalidId() throws Exception
	  public virtual void testGetDecisionModelByInvalidId()
	  {
		try
		{
		  repositoryService.getDecisionModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no deployed decision definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getDecisionModel(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("decisionDefinitionId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/repository/drg.dmn" }) public void testGetDecisionRequirementsModel() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/drg.dmn" })]
	  public virtual void testGetDecisionRequirementsModel()
	  {
		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		DecisionRequirementsDefinition decisionRequirementsDefinition = query.singleResult();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		Stream decisionRequirementsModel = repositoryService.getDecisionRequirementsModel(decisionRequirementsDefinitionId);

		assertNotNull(decisionRequirementsModel);

		sbyte[] readInputStream = IoUtil.readInputStream(decisionRequirementsModel, "decisionRequirementsModel");
		string model = StringHelper.NewString(readInputStream, "UTF-8");

		assertTrue(model.Contains("<definitions id=\"dish\" name=\"Dish\" namespace=\"test-drg\""));
		IoUtil.closeSilently(decisionRequirementsModel);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testGetDecisionRequirementsModelByInvalidId() throws Exception
	  public virtual void testGetDecisionRequirementsModelByInvalidId()
	  {
		try
		{
		  repositoryService.getDecisionRequirementsModel("invalid");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getDecisionRequirementsModel(null);
		  fail();
		}
		catch (NotValidException e)
		{
		  assertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/repository/drg.dmn", "org/camunda/bpm/engine/test/repository/drg.png" }) public void testGetDecisionRequirementsDiagram() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/repository/drg.dmn", "org/camunda/bpm/engine/test/repository/drg.png" })]
	  public virtual void testGetDecisionRequirementsDiagram()
	  {

		DecisionRequirementsDefinitionQuery query = repositoryService.createDecisionRequirementsDefinitionQuery();

		DecisionRequirementsDefinition decisionRequirementsDefinition = query.singleResult();
		string decisionRequirementsDefinitionId = decisionRequirementsDefinition.Id;

		Stream actualDrd = repositoryService.getDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

		assertNotNull(actualDrd);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testGetDecisionRequirementsDiagramByInvalidId() throws Exception
	  public virtual void testGetDecisionRequirementsDiagramByInvalidId()
	  {
		try
		{
		  repositoryService.getDecisionRequirementsDiagram("invalid");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no deployed decision requirements definition found with id 'invalid'", e.Message);
		}

		try
		{
		  repositoryService.getDecisionRequirementsDiagram(null);
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("decisionRequirementsDefinitionId is null", e.Message);
		}
	  }

	  public virtual void testDeployRevisedProcessAfterDeleteOnOtherProcessEngine()
	  {

		// Setup both process engines
		ProcessEngine processEngine1 = (new StandaloneProcessEngineConfiguration()).setProcessEngineName("reboot-test-schema").setDatabaseSchemaUpdate(ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE).setJdbcUrl("jdbc:h2:mem:activiti-process-cache-test;DB_CLOSE_DELAY=1000").setJobExecutorActivate(false).buildProcessEngine();
		RepositoryService repositoryService1 = processEngine1.RepositoryService;

		ProcessEngine processEngine2 = (new StandaloneProcessEngineConfiguration()).setProcessEngineName("reboot-test").setDatabaseSchemaUpdate(ProcessEngineConfiguration.DB_SCHEMA_UPDATE_FALSE).setJdbcUrl("jdbc:h2:mem:activiti-process-cache-test;DB_CLOSE_DELAY=1000").setJobExecutorActivate(false).buildProcessEngine();
		RepositoryService repositoryService2 = processEngine2.RepositoryService;
		RuntimeService runtimeService2 = processEngine2.RuntimeService;
		TaskService taskService2 = processEngine2.TaskService;

		// Deploy first version of process: start->originalTask->end on first process engine
		string deploymentId = repositoryService1.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeployRevisedProcessAfterDeleteOnOtherProcessEngine.v1.bpmn20.xml").deploy().Id;

		// Start process instance on second engine
		string processDefinitionId = repositoryService2.createProcessDefinitionQuery().singleResult().Id;
		runtimeService2.startProcessInstanceById(processDefinitionId);
		Task task = taskService2.createTaskQuery().singleResult();
		assertEquals("original task", task.Name);

		// Delete the deployment on second process engine
		repositoryService2.deleteDeployment(deploymentId, true);
		assertEquals(0, repositoryService2.createDeploymentQuery().count());
		assertEquals(0, runtimeService2.createProcessInstanceQuery().count());

		// deploy a revised version of the process: start->revisedTask->end on first process engine
		//
		// Before the bugfix, this would set the cache on the first process engine,
		// but the second process engine still has the original process definition in his cache.
		// Since there is a deployment delete in between, the new generated process definition id is the same
		// as in the original deployment, making the second process engine using the old cached process definition.
		deploymentId = repositoryService1.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/RepositoryServiceTest.testDeployRevisedProcessAfterDeleteOnOtherProcessEngine.v2.bpmn20.xml").deploy().Id;

		// Start process instance on second process engine -> must use revised process definition
		processDefinitionId = repositoryService2.createProcessDefinitionQuery().singleResult().Id;
		runtimeService2.startProcessInstanceByKey("oneTaskProcess");
		task = taskService2.createTaskQuery().singleResult();
		assertEquals("revised task", task.Name);

		// cleanup
		repositoryService1.deleteDeployment(deploymentId, true);
		processEngine1.close();
		processEngine2.close();
	  }

	  public virtual void testDeploymentPersistence()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().name("strings").addString("org/camunda/bpm/engine/test/test/HelloWorld.string", "hello world").addString("org/camunda/bpm/engine/test/test/TheAnswer.string", "42").deploy();

		IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		assertEquals(1, deployments.Count);
		deployment = deployments[0];

		assertEquals("strings", deployment.Name);
		assertNotNull(deployment.DeploymentTime);

		string deploymentId = deployment.Id;
		IList<string> resourceNames = repositoryService.getDeploymentResourceNames(deploymentId);
		ISet<string> expectedResourceNames = new HashSet<string>();
		expectedResourceNames.Add("org/camunda/bpm/engine/test/test/HelloWorld.string");
		expectedResourceNames.Add("org/camunda/bpm/engine/test/test/TheAnswer.string");
		assertEquals(expectedResourceNames, new HashSet<string>(resourceNames));

		Stream resourceStream = repositoryService.getResourceAsStream(deploymentId, "org/camunda/bpm/engine/test/test/HelloWorld.string");
		assertTrue(Arrays.Equals("hello world".GetBytes(), IoUtil.readInputStream(resourceStream, "test")));

		resourceStream = repositoryService.getResourceAsStream(deploymentId, "org/camunda/bpm/engine/test/test/TheAnswer.string");
		assertTrue(Arrays.Equals("42".GetBytes(), IoUtil.readInputStream(resourceStream, "test")));

		repositoryService.deleteDeployment(deploymentId);
	  }

	  public virtual void testProcessDefinitionPersistence()
	  {
		string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml").deploy().Id;

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(2, processDefinitions.Count);

		repositoryService.deleteDeployment(deploymentId);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/dmn/Example.dmn"})]
	  public virtual void testDecisionDefinitionUpdateTimeToLiveWithUserOperationLog()
	  {
		//given
		identityService.AuthenticatedUserId = "userId";
		DecisionDefinition decisionDefinition = findOnlyDecisionDefinition();
		int? orgTtl = decisionDefinition.HistoryTimeToLive;

		//when
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 6);

		//then
		decisionDefinition = findOnlyDecisionDefinition();
		assertEquals(6, decisionDefinition.HistoryTimeToLive.Value);

		UserOperationLogQuery operationLogQuery = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE).entityType(EntityTypes.DECISION_DEFINITION);

		UserOperationLogEntry ttlEntry = operationLogQuery.property("historyTimeToLive").singleResult();
		UserOperationLogEntry definitionIdEntry = operationLogQuery.property("decisionDefinitionId").singleResult();
		UserOperationLogEntry definitionKeyEntry = operationLogQuery.property("decisionDefinitionKey").singleResult();

		assertNotNull(ttlEntry);
		assertNotNull(definitionIdEntry);
		assertNotNull(definitionKeyEntry);

		assertEquals(orgTtl.ToString(), ttlEntry.OrgValue);
		assertEquals("6", ttlEntry.NewValue);
		assertEquals(decisionDefinition.Id, definitionIdEntry.NewValue);
		assertEquals(decisionDefinition.Key, definitionKeyEntry.NewValue);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, ttlEntry.Category);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, definitionIdEntry.Category);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, definitionKeyEntry.Category);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/dmn/Example.dmn"})]
	  public virtual void testDecisionDefinitionUpdateTimeToLiveNull()
	  {
		//given
		DecisionDefinition decisionDefinition = findOnlyDecisionDefinition();

		//when
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, null);

		//then
		decisionDefinition = (DecisionDefinitionEntity) repositoryService.getDecisionDefinition(decisionDefinition.Id);
		assertEquals(null, decisionDefinition.HistoryTimeToLive);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/dmn/Example.dmn"})]
	  public virtual void testDecisionDefinitionUpdateTimeToLiveNegative()
	  {
		//given
		DecisionDefinition decisionDefinition = findOnlyDecisionDefinition();

		//when
		try
		{
		  repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, -1);
		  fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  assertTrue(ex.Message.contains("greater than"));
		}

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessDefinitionUpdateTimeToLive()
	  {
		//given
		ProcessDefinition processDefinition = findOnlyProcessDefinition();

		//when
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinition.Id, 6);

		//then
		processDefinition = findOnlyProcessDefinition();
		assertEquals(6, processDefinition.HistoryTimeToLive.Value);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessDefinitionUpdateTimeToLiveNull()
	  {
		//given
		ProcessDefinition processDefinition = findOnlyProcessDefinition();

		//when
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinition.Id, null);

		//then
		processDefinition = findOnlyProcessDefinition();
		assertEquals(null, processDefinition.HistoryTimeToLive);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessDefinitionUpdateTimeToLiveNegative()
	  {
		//given
		ProcessDefinition processDefinition = findOnlyProcessDefinition();

		//when
		try
		{
		  repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinition.Id, -1);
		  fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  assertTrue(ex.Message.contains("greater than"));
		}

	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessDefinitionUpdateHistoryTimeToLiveWithUserOperationLog()
	  {
		//given
		ProcessDefinition processDefinition = findOnlyProcessDefinition();
		int? timeToLiveOrgValue = processDefinition.HistoryTimeToLive;
		processEngine.IdentityService.AuthenticatedUserId = "userId";

		//when
		int? timeToLiveNewValue = 6;
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinition.Id, timeToLiveNewValue);

		//then
		IList<UserOperationLogEntry> opLogEntries = processEngine.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(1, opLogEntries.Count);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.history.event.UserOperationLogEntryEventEntity userOperationLogEntry = (org.camunda.bpm.engine.impl.history.event.UserOperationLogEntryEventEntity)opLogEntries.get(0);
		UserOperationLogEntryEventEntity userOperationLogEntry = (UserOperationLogEntryEventEntity)opLogEntries[0];

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE, userOperationLogEntry.OperationType);
		assertEquals(processDefinition.Key, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(processDefinition.Id, userOperationLogEntry.ProcessDefinitionId);
		assertEquals("historyTimeToLive", userOperationLogEntry.Property);
		assertEquals(timeToLiveOrgValue, Convert.ToInt32(userOperationLogEntry.OrgValue));
		assertEquals(timeToLiveNewValue, Convert.ToInt32(userOperationLogEntry.NewValue));

	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCaseDefinitionUpdateHistoryTimeToLiveWithUserOperationLog()
	  {
		// given
		identityService.AuthenticatedUserId = "userId";

		// there exists a deployment containing a case definition with key "oneTaskCase"
		CaseDefinition caseDefinition = findOnlyCaseDefinition();

		// when
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, 6);

		// then
		caseDefinition = findOnlyCaseDefinition();

		assertEquals(6, caseDefinition.HistoryTimeToLive.Value);

		UserOperationLogQuery operationLogQuery = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE).entityType(EntityTypes.CASE_DEFINITION).caseDefinitionId(caseDefinition.Id);

		UserOperationLogEntry ttlEntry = operationLogQuery.property("historyTimeToLive").singleResult();
		UserOperationLogEntry definitionKeyEntry = operationLogQuery.property("caseDefinitionKey").singleResult();

		assertNotNull(ttlEntry);
		assertNotNull(definitionKeyEntry);

		// original time-to-live value is null
		assertNull(ttlEntry.OrgValue);
		assertEquals("6", ttlEntry.NewValue);
		assertEquals(caseDefinition.Key, definitionKeyEntry.NewValue);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, ttlEntry.Category);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, definitionKeyEntry.Category);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testUpdateHistoryTimeToLiveNull()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		CaseDefinition caseDefinition = findOnlyCaseDefinition();

		// when
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, null);

		// then
		caseDefinition = findOnlyCaseDefinition();

		assertEquals(null, caseDefinition.HistoryTimeToLive);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testUpdateHistoryTimeToLiveNegative()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		CaseDefinition caseDefinition = findOnlyCaseDefinition();

		// when
		try
		{
		  repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, -1);
		  fail("Exception is expected, that negative value is not allowed.");
		}
		catch (BadUserRequestException ex)
		{
		  assertTrue(ex.Message.contains("greater than"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testUpdateHistoryTimeToLiveInCache()
	  {
		// given
		// there exists a deployment containing a case definition with key "oneTaskCase"

		CaseDefinition caseDefinition = findOnlyCaseDefinition();

		// assume
		assertNull(caseDefinition.HistoryTimeToLive);

		// when
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinition.Id, 10);

		CaseDefinition definition = repositoryService.getCaseDefinition(caseDefinition.Id);
		assertEquals(Convert.ToInt32(10), definition.HistoryTimeToLive);
	  }

	  private CaseDefinition findOnlyCaseDefinition()
	  {
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().list();
		assertNotNull(caseDefinitions);
		assertEquals(1, caseDefinitions.Count);
		return caseDefinitions[0];
	  }

	  private ProcessDefinition findOnlyProcessDefinition()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);
		return processDefinitions[0];
	  }

	  private DecisionDefinition findOnlyDecisionDefinition()
	  {
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().list();
		assertNotNull(decisionDefinitions);
		assertEquals(1, decisionDefinitions.Count);
		return decisionDefinitions[0];
	  }

	  public virtual void testProcessDefinitionIntrospection()
	  {
		string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml").deploy().Id;

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		ReadOnlyProcessDefinition processDefinition = ((RepositoryServiceImpl)repositoryService).getDeployedProcessDefinition(procDefId);

		assertEquals(procDefId, processDefinition.Id);
		assertEquals("Process One", processDefinition.Name);
		assertEquals("the first process", processDefinition.getProperty("documentation"));

		PvmActivity start = processDefinition.findActivity("start");
		assertNotNull(start);
		assertEquals("start", start.Id);
		assertEquals("S t a r t", start.getProperty("name"));
		assertEquals("the start event", start.getProperty("documentation"));
		assertEquals(Collections.EMPTY_LIST, start.Activities);
		IList<PvmTransition> outgoingTransitions = start.OutgoingTransitions;
		assertEquals(1, outgoingTransitions.Count);
		assertEquals("${a == b}", outgoingTransitions[0].getProperty(BpmnParse.PROPERTYNAME_CONDITION_TEXT));

		PvmActivity end = processDefinition.findActivity("end");
		assertNotNull(end);
		assertEquals("end", end.Id);

		PvmTransition transition = outgoingTransitions[0];
		assertEquals("flow1", transition.Id);
		assertEquals("Flow One", transition.getProperty("name"));
		assertEquals("The only transitions in the process", transition.getProperty("documentation"));
		assertSame(start, transition.Source);
		assertSame(end, transition.Destination);

		repositoryService.deleteDeployment(deploymentId);
	  }

	  public virtual void testProcessDefinitionQuery()
	  {
		string deployment1Id = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml").deploy().Id;

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionName().asc().orderByProcessDefinitionVersion().asc().list();

		assertEquals(2, processDefinitions.Count);

		string deployment2Id = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml").deploy().Id;

		assertEquals(4, repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionName().asc().count());
		assertEquals(2, repositoryService.createProcessDefinitionQuery().latestVersion().orderByProcessDefinitionName().asc().count());

		deleteDeployments(Arrays.asList(deployment1Id, deployment2Id));
	  }

	  public virtual void testGetProcessDefinitions()
	  {
		IList<string> deploymentIds = new List<string>();
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='IDR' name='Insurance Damage Report 1' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='IDR' name='Insurance Damage Report 2' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='IDR' name='Insurance Damage Report 3' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='EN' name='Expense Note 1' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='EN' name='Expense Note 2' isExecutable='true' />" + "</definitions>")));

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionKey().asc().orderByProcessDefinitionVersion().desc().list();

		assertNotNull(processDefinitions);

		assertEquals(5, processDefinitions.Count);

		ProcessDefinition processDefinition = processDefinitions[0];
		assertEquals("EN", processDefinition.Key);
		assertEquals("Expense Note 2", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("EN:2", StringComparison.Ordinal));
		assertEquals(2, processDefinition.Version);

		processDefinition = processDefinitions[1];
		assertEquals("EN", processDefinition.Key);
		assertEquals("Expense Note 1", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("EN:1", StringComparison.Ordinal));
		assertEquals(1, processDefinition.Version);

		processDefinition = processDefinitions[2];
		assertEquals("IDR", processDefinition.Key);
		assertEquals("Insurance Damage Report 3", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("IDR:3", StringComparison.Ordinal));
		assertEquals(3, processDefinition.Version);

		processDefinition = processDefinitions[3];
		assertEquals("IDR", processDefinition.Key);
		assertEquals("Insurance Damage Report 2", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("IDR:2", StringComparison.Ordinal));
		assertEquals(2, processDefinition.Version);

		processDefinition = processDefinitions[4];
		assertEquals("IDR", processDefinition.Key);
		assertEquals("Insurance Damage Report 1", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("IDR:1", StringComparison.Ordinal));
		assertEquals(1, processDefinition.Version);

		deleteDeployments(deploymentIds);
	  }

	  public virtual void testDeployIdenticalProcessDefinitions()
	  {
		IList<string> deploymentIds = new List<string>();
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='IDR' name='Insurance Damage Report' isExecutable='true' />" + "</definitions>")));
		deploymentIds.Add(deployProcessString(("<definitions " + NAMESPACE + " " + TARGET_NAMESPACE + ">" + "  <process id='IDR' name='Insurance Damage Report' isExecutable='true' />" + "</definitions>")));

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionKey().asc().orderByProcessDefinitionVersion().desc().list();

		assertNotNull(processDefinitions);
		assertEquals(2, processDefinitions.Count);

		ProcessDefinition processDefinition = processDefinitions[0];
		assertEquals("IDR", processDefinition.Key);
		assertEquals("Insurance Damage Report", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("IDR:2", StringComparison.Ordinal));
		assertEquals(2, processDefinition.Version);

		processDefinition = processDefinitions[1];
		assertEquals("IDR", processDefinition.Key);
		assertEquals("Insurance Damage Report", processDefinition.Name);
		assertTrue(processDefinition.Id.StartsWith("IDR:1", StringComparison.Ordinal));
		assertEquals(1, processDefinition.Version);

		deleteDeployments(deploymentIds);
	  }

	  private string deployProcessString(string processString)
	  {
		string resourceName = "xmlString." + BpmnDeployer.BPMN_RESOURCE_SUFFIXES[0];
		return repositoryService.createDeployment().addString(resourceName, processString).deploy().Id;
	  }

	  private void deleteDeployments(ICollection<string> deploymentIds)
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  repositoryService.deleteDeployment(deploymentId);
		}
	  }

	}

}