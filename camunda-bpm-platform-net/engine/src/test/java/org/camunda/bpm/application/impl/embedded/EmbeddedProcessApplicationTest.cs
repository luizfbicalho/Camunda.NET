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
namespace org.camunda.bpm.application.impl.embedded
{

	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using Resource = org.camunda.bpm.engine.repository.Resource;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EmbeddedProcessApplicationTest : PluggableProcessEngineTestCase
	{

	  protected internal RuntimeContainerDelegate runtimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
	  protected internal bool defaultEngineRegistered;

	  public virtual void registerProcessEngine()
	  {
		runtimeContainerDelegate.registerProcessEngine(processEngine);
		defaultEngineRegistered = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		defaultEngineRegistered = false;
	  }

	  public override void tearDown()
	  {
		if (defaultEngineRegistered)
		{
		  runtimeContainerDelegate.unregisterProcessEngine(processEngine);
		}
	  }

	  public virtual void testDeployAppWithoutEngine()
	  {

		TestApplicationWithoutEngine processApplication = new TestApplicationWithoutEngine();
		processApplication.deploy();

		processApplication.undeploy();

	  }

	  public virtual void testDeployAppWithoutProcesses()
	  {

		registerProcessEngine();

		TestApplicationWithoutProcesses processApplication = new TestApplicationWithoutProcesses();
		processApplication.deploy();

		ProcessEngine processEngine = BpmPlatform.ProcessEngineService.DefaultProcessEngine;
		long deployments = processEngine.RepositoryService.createDeploymentQuery().count();
		assertEquals(0, deployments);

		processApplication.undeploy();

	  }

	  public virtual void testDeployAppWithCustomEngine()
	  {

		TestApplicationWithCustomEngine processApplication = new TestApplicationWithCustomEngine();
		processApplication.deploy();

		ProcessEngine processEngine = BpmPlatform.ProcessEngineService.getProcessEngine("embeddedEngine");
		assertNotNull(processEngine);
		assertEquals("embeddedEngine", processEngine.Name);

		ProcessEngineConfiguration configuration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;

		// assert engine properties specified
		assertTrue(configuration.JobExecutorDeploymentAware);
		assertTrue(configuration.JobExecutorPreferTimerJobs);
		assertTrue(configuration.JobExecutorAcquireByDueDate);
		assertEquals(5, configuration.JdbcMaxActiveConnections);

		processApplication.undeploy();

	  }

	  public virtual void testDeployAppWithCustomDefaultEngine()
	  {

		// Test if it's possible to set a custom default engine name.
		// This might happen when the "default" ProcessEngine is not available,
		// but a ProcessApplication doesn't define a ProcessEngine to deploy to.
		string processApplicationName = "test-app";
		string customEngineName = "customDefaultEngine";
		TestApplicationWithCustomDefaultEngine processApplication = new TestApplicationWithCustomDefaultEngine();

		processApplication.deploy();

		string deployedToProcessEngineName = runtimeContainerDelegate.ProcessApplicationService.getProcessApplicationInfo(processApplicationName).DeploymentInfo[0].ProcessEngineName;

		assertEquals(customEngineName, processApplication.DefaultDeployToEngineName);
		assertEquals(customEngineName, deployedToProcessEngineName);

		processApplication.undeploy();
	  }

	  public virtual void testDeployAppReusingExistingEngine()
	  {

		registerProcessEngine();

		TestApplicationReusingExistingEngine processApplication = new TestApplicationReusingExistingEngine();
		processApplication.deploy();

		assertEquals(1, repositoryService.createDeploymentQuery().count());

		processApplication.undeploy();

		assertEquals(0, repositoryService.createDeploymentQuery().count());

	  }

	  public virtual void testDeployAppWithAdditionalResourceSuffixes()
	  {
		registerProcessEngine();

		TestApplicationWithAdditionalResourceSuffixes processApplication = new TestApplicationWithAdditionalResourceSuffixes();
		processApplication.deploy();


		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertNotNull(deployment);

		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deployment.Id);
		assertEquals(4, deploymentResources.Count);

		processApplication.undeploy();
		assertEquals(0, repositoryService.createDeploymentQuery().count());
	  }

	  public virtual void testDeployAppWithResources()
	  {
		registerProcessEngine();

		TestApplicationWithResources processApplication = new TestApplicationWithResources();
		processApplication.deploy();

		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertNotNull(deployment);

		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deployment.Id);
		assertEquals(4, deploymentResources.Count);

		processApplication.undeploy();
		assertEquals(0, repositoryService.createDeploymentQuery().count());
	  }

	  public virtual void testDeploymentSourceProperty()
	  {
		registerProcessEngine();

		TestApplicationWithResources processApplication = new TestApplicationWithResources();
		processApplication.deploy();

		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertNotNull(deployment);
		assertEquals(org.camunda.bpm.engine.repository.ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE, deployment.Source);

		processApplication.undeploy();
	  }

	  public virtual void testDeployProcessApplicationWithNameAttribute()
	  {
		TestApplicationWithCustomName pa = new TestApplicationWithCustomName();

		pa.deploy();

		ISet<string> deployedPAs = runtimeContainerDelegate.ProcessApplicationService.ProcessApplicationNames;
		assertEquals(1, deployedPAs.Count);
		assertTrue(deployedPAs.Contains(TestApplicationWithCustomName.NAME));

		pa.undeploy();
	  }

	  public virtual void testDeployWithTenantIds()
	  {
		registerProcessEngine();

		TestApplicationWithTenantId processApplication = new TestApplicationWithTenantId();
		processApplication.deploy();

		IList<Deployment> deployments = repositoryService.createDeploymentQuery().orderByTenantId().asc().list();

		assertEquals(2, deployments.Count);
		assertEquals("tenant1", deployments[0].TenantId);
		assertEquals("tenant2", deployments[1].TenantId);

		processApplication.undeploy();
	  }

	  public virtual void testDeployWithoutTenantId()
	  {
		registerProcessEngine();

		TestApplicationWithResources processApplication = new TestApplicationWithResources();
		processApplication.deploy();

		Deployment deployment = repositoryService.createDeploymentQuery().singleResult();

		assertNotNull(deployment);
		assertNull(deployment.TenantId);

		processApplication.undeploy();
	  }

	}

}