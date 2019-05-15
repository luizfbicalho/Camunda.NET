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
namespace org.camunda.bpm.engine.test.api.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using EmbeddedProcessApplication = org.camunda.bpm.application.impl.EmbeddedProcessApplication;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentTest : PluggableProcessEngineTestCase
	{

	  private EmbeddedProcessApplication processApplication;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		processApplication = new EmbeddedProcessApplication();
	  }

	  public virtual void testEmptyDeployment()
	  {
		try
		{
		  repositoryService.createDeployment(processApplication.Reference).deploy();
		  fail("it should not be possible to deploy without deployment resources");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  repositoryService.createDeployment().deploy();
		  fail("it should not be possible to deploy without deployment resources");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  public virtual void testSimpleProcessApplicationDeployment()
	  {

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		// process is deployed:
		assertThatOneProcessIsDeployed();

		// registration was performed:
		ProcessApplicationRegistration registration = deployment.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment);
	  }

	  public virtual void testProcessApplicationDeploymentNoChanges()
	  {
		// create initial deployment
		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).name("deployment").addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		assertThatOneProcessIsDeployed();

		// deploy update with no changes:
		deployment = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(false).addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		// no changes
		assertThatOneProcessIsDeployed();
		ProcessApplicationRegistration registration = deployment.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment);
	  }

	  public virtual void testPartialChangesDeployAll()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();

		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).deploy();

		BpmnModelInstance changedModel2 = Bpmn.createExecutableProcess("process2").startEvent().done();

		// second deployment with partial changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(false).resumePreviousVersions().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", changedModel2).deploy();

		assertEquals(4, repositoryService.createProcessDefinitionQuery().count());

		IList<ProcessDefinition> processDefinitionsModel1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").orderByProcessDefinitionVersion().asc().list();

		// now there are two versions of process1 deployed
		assertEquals(2, processDefinitionsModel1.Count);
		assertEquals(1, processDefinitionsModel1[0].Version);
		assertEquals(2, processDefinitionsModel1[1].Version);

		// now there are two versions of process2 deployed
		IList<ProcessDefinition> processDefinitionsModel2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").orderByProcessDefinitionVersion().asc().list();

		assertEquals(2, processDefinitionsModel2.Count);
		assertEquals(1, processDefinitionsModel2[0].Version);
		assertEquals(2, processDefinitionsModel2[1].Version);

		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(2, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  /// <summary>
	  /// Test re-deployment of only those resources that have actually changed
	  /// </summary>
	  public virtual void testPartialChangesDeployChangedOnly()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();

		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).deploy();

		BpmnModelInstance changedModel2 = Bpmn.createExecutableProcess("process2").startEvent().done();

		// second deployment with partial changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(true).resumePreviousVersions().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", changedModel2).deploy();

		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());

		// there is one version of process1 deployed
		ProcessDefinition processDefinitionModel1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult();

		assertNotNull(processDefinitionModel1);
		assertEquals(1, processDefinitionModel1.Version);
		assertEquals(deployment1.Id, processDefinitionModel1.DeploymentId);

		// there are two versions of process2 deployed
		IList<ProcessDefinition> processDefinitionsModel2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").orderByProcessDefinitionVersion().asc().list();

		assertEquals(2, processDefinitionsModel2.Count);
		assertEquals(1, processDefinitionsModel2[0].Version);
		assertEquals(2, processDefinitionsModel2[1].Version);

		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(2, deploymentIds.Count);

		BpmnModelInstance anotherChangedModel2 = Bpmn.createExecutableProcess("process2").startEvent().endEvent().done();

		// testing with a third deployment to ensure the change check is not only performed against
		// the last version of the deployment
		ProcessApplicationDeployment deployment3 = repositoryService.createDeployment(processApplication.Reference).enableDuplicateFiltering(true).resumePreviousVersions().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", anotherChangedModel2).name("deployment").deploy();

		// there should still be one version of process 1
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").count());

		// there should be three versions of process 2
		assertEquals(3, repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").count());

		// old deployments are resumed
		registration = deployment3.ProcessApplicationRegistration;
		deploymentIds = registration.DeploymentIds;
		assertEquals(3, deploymentIds.Count);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  public virtual void testPartialChangesResumePreviousVersion()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();

		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn20.xml", model1).deploy();

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(true).resumePreviousVersions().addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).deploy();

		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		assertEquals(2, registration.DeploymentIds.Count);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersions()
	  {
		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		assertThatOneProcessIsDeployed();

		// deploy update with changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(false).resumePreviousVersions().addClasspathResource("org/camunda/bpm/engine/test/api/repository/version2.bpmn20.xml").deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// now there are 2 process definitions deployed
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(2, processDefinitions[1].Version);

		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(2, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersionsDifferentKeys()
	  {
		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		assertThatOneProcessIsDeployed();

		// deploy update with changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").resumePreviousVersions().addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// now there are 2 process definitions deployed
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(1, processDefinitions[1].Version);

		// and the old deployment was not resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertEquals(deployment2.Id, deploymentIds.GetEnumerator().next());
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentNoResume()
	  {
		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		assertThatOneProcessIsDeployed();

		// deploy update with changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(false).addClasspathResource("org/camunda/bpm/engine/test/api/repository/version2.bpmn20.xml").deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// now there are 2 process definitions deployed
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(2, processDefinitions[1].Version);

		// old deployment was NOT resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentName()
	  {
		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addClasspathResource("org/camunda/bpm/engine/test/api/repository/version1.bpmn20.xml").deploy();

		assertThatOneProcessIsDeployed();

		// deploy update with changes:
		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(false).resumePreviousVersions().resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME).addClasspathResource("org/camunda/bpm/engine/test/api/repository/version2.bpmn20.xml").deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// now there are 2 process definitions deployed
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(2, processDefinitions[1].Version);

		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(2, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentNameDeployDifferentProcesses()
	  {
		BpmnModelInstance process1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance process2 = Bpmn.createExecutableProcess("process2").done();
		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn", process1).deploy();

		assertThatOneProcessIsDeployed();

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").resumePreviousVersions().resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME).addModelInstance("process2.bpmn", process2).deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// now there are 2 process definitions deployed but both with version 1
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(1, processDefinitions[1].Version);

		// old deployment was resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(2, deploymentIds.Count);
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentResumePreviousVersionsByDeploymentNameNoResume()
	  {
		BpmnModelInstance process1 = Bpmn.createExecutableProcess("process1").done();
		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn", process1).deploy();

		assertThatOneProcessIsDeployed();

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("anotherDeployment").resumePreviousVersions().resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME).addModelInstance("process2.bpmn", process1).deploy();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc().list();
		// there is a new version of the process
		assertEquals(1, processDefinitions[0].Version);
		assertEquals(2, processDefinitions[1].Version);

		// but the old deployment was not resumed
		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		ISet<string> deploymentIds = registration.DeploymentIds;
		assertEquals(1, deploymentIds.Count);
		assertEquals(deployment2.Id, deploymentIds.GetEnumerator().next());
		assertEquals(processEngine.Name, registration.ProcessEngineName);

		deleteDeployments(deployment, deployment2);
	  }

	  public virtual void testPartialChangesResumePreviousVersionByDeploymentName()
	  {
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("process1").done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("process2").done();

		// create initial deployment
		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name("deployment").addModelInstance("process1.bpmn20.xml", model1).deploy();

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("deployment").enableDuplicateFiltering(true).resumePreviousVersions().resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME).addModelInstance("process1.bpmn20.xml", model1).addModelInstance("process2.bpmn20.xml", model2).deploy();

		ProcessApplicationRegistration registration = deployment2.ProcessApplicationRegistration;
		assertEquals(2, registration.DeploymentIds.Count);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testDeploymentSourceShouldBeNull()
	  {
		string key = "process";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		Deployment deployment1 = repositoryService.createDeployment().name("first-deployment-without-a-source").addModelInstance("process.bpmn", model).deploy();

		assertNull(deploymentQuery.deploymentName("first-deployment-without-a-source").singleResult().Source);

		Deployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("second-deployment-with-a-source").source(null).addModelInstance("process.bpmn", model).deploy();

		assertNull(deploymentQuery.deploymentName("second-deployment-with-a-source").singleResult().Source);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testDeploymentSourceShouldNotBeNull()
	  {
		string key = "process";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		Deployment deployment1 = repositoryService.createDeployment().name("first-deployment-without-a-source").source("my-first-deployment-source").addModelInstance("process.bpmn", model).deploy();

		assertEquals("my-first-deployment-source", deploymentQuery.deploymentName("first-deployment-without-a-source").singleResult().Source);

		Deployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("second-deployment-with-a-source").source("my-second-deployment-source").addModelInstance("process.bpmn", model).deploy();

		assertEquals("my-second-deployment-source", deploymentQuery.deploymentName("second-deployment-with-a-source").singleResult().Source);

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testDefaultDeploymentSource()
	  {
		string key = "process";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		Deployment deployment = repositoryService.createDeployment(processApplication.Reference).name("first-deployment-with-a-source").addModelInstance("process.bpmn", model).deploy();

		assertEquals(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE, deploymentQuery.deploymentName("first-deployment-with-a-source").singleResult().Source);

		deleteDeployments(deployment);
	  }

	  public virtual void testOverwriteDeploymentSource()
	  {
		string key = "process";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		Deployment deployment = repositoryService.createDeployment(processApplication.Reference).name("first-deployment-with-a-source").source("my-source").addModelInstance("process.bpmn", model).deploy();

		assertEquals("my-source", deploymentQuery.deploymentName("first-deployment-with-a-source").singleResult().Source);

		deleteDeployments(deployment);
	  }

	  public virtual void testNullDeploymentSourceAwareDuplicateFilter()
	  {
		// given
		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testNullAndProcessApplicationDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationAndNullDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testProcessApplicationDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testSameDeploymentSourceAwareDuplicateFilter()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source("cockpit").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name("my-deployment").source("cockpit").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testDifferentDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source("my-source1").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).source("my-source2").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(2, processDefinitionQuery.count());
		assertEquals(2, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testNullAndNotNullDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).source("my-source2").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(2, processDefinitionQuery.count());
		assertEquals(2, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testNotNullAndNullDeploymentSourceShouldDeployNewVersion()
	  {
		// given

		string key = "process";
		string name = "my-deployment";

		BpmnModelInstance model = Bpmn.createExecutableProcess(key).done();

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentName(name);

		// when

		ProcessApplicationDeployment deployment1 = repositoryService.createDeployment(processApplication.Reference).name(name).source("my-source1").addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		assertEquals(1, processDefinitionQuery.count());
		assertEquals(1, deploymentQuery.count());

		ProcessApplicationDeployment deployment2 = repositoryService.createDeployment(processApplication.Reference).name(name).source(null).addModelInstance("process.bpmn", model).enableDuplicateFiltering(true).deploy();

		// then

		assertEquals(2, processDefinitionQuery.count());
		assertEquals(2, deploymentQuery.count());

		deleteDeployments(deployment1, deployment2);
	  }

	  public virtual void testUnregisterProcessApplicationOnDeploymentDeletion()
	  {
		// given a deployment with a process application registration
		Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", Bpmn.createExecutableProcess("foo").done()).deploy();

		// and a process application registration
		managementService.registerProcessApplication(deployment.Id, processApplication.Reference);

		// when deleting the deploymen
		repositoryService.deleteDeployment(deployment.Id, true);

		// then the registration is removed
		assertNull(managementService.getProcessApplicationForDeployment(deployment.Id));



	  }

	  /// <summary>
	  /// Deletes the deployments cascading.
	  /// </summary>
	  private void deleteDeployments(params Deployment[] deployments)
	  {
		foreach (Deployment deployment in deployments)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  /// <summary>
	  /// Creates a process definition query and checks that only one process with version 1 is present.
	  /// </summary>
	  private void assertThatOneProcessIsDeployed()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertThat(processDefinition, @is(notNullValue()));
		assertEquals(1, processDefinition.Version);
	  }

	}

}