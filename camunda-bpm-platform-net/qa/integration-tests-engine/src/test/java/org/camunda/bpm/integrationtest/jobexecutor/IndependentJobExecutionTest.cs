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
namespace org.camunda.bpm.integrationtest.jobexecutor
{
	using ProcessApplicationDeploymentInfo = org.camunda.bpm.application.ProcessApplicationDeploymentInfo;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebAppDescriptor = org.jboss.arquillian.protocol.servlet.arq514hack.descriptors.api.web.WebAppDescriptor;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Descriptors = org.jboss.shrinkwrap.descriptor.api.Descriptors;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class IndependentJobExecutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class IndependentJobExecutionTest : AbstractFoxPlatformIntegrationTest
	{

	  private ProcessEngine engine1;
	  private ProcessEngineConfigurationImpl engine1Configuration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setEngines()
	  public virtual void setEngines()
	  {
		ProcessEngineService engineService = BpmPlatform.ProcessEngineService;
		engine1 = engineService.getProcessEngine("engine1");
		engine1Configuration = ((ProcessEngineImpl) engine1).ProcessEngineConfiguration;
	  }

	  [Deployment(order : 0, name:"pa1")]
	  public static WebArchive processArchive1()
	  {

		WebArchive deployment = initWebArchiveDeployment("pa1.war", "org/camunda/bpm/integrationtest/jobexecutor/IndependentJobExecutionTest.pa1.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/IndependentJobExecutionTest.process1.bpmn20.xml").setWebXML(new StringAsset(Descriptors.create(typeof(WebAppDescriptor)).version("3.0").exportAsString()));

		TestContainer.addContainerSpecificProcessEngineConfigurationClass(deployment);

		return deployment;

	  }

	  [Deployment(order : 1, name:"pa2")]
	  public static WebArchive processArchive2()
	  {

		return initWebArchiveDeployment("pa2.war", "org/camunda/bpm/integrationtest/jobexecutor/IndependentJobExecutionTest.pa2.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/IndependentJobExecutionTest.process2.bpmn20.xml").setWebXML(new StringAsset(Descriptors.create(typeof(WebAppDescriptor)).version("3.0").exportAsString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OperateOnDeployment("pa1") @Test public void testDeploymentRegistration()
	  public virtual void testDeploymentRegistration()
	  {
		ISet<string> registeredDeploymentsForEngine1 = engine1.ManagementService.RegisteredDeployments;
		ISet<string> registeredDeploymentsForDefaultEngine = processEngine.ManagementService.RegisteredDeployments;

		ProcessApplicationInfo pa1Info = getProcessApplicationDeploymentInfo("pa1");

		IList<ProcessApplicationDeploymentInfo> pa1DeploymentInfo = pa1Info.DeploymentInfo;

		Assert.assertEquals(1, pa1DeploymentInfo.Count);
		Assert.assertTrue(registeredDeploymentsForEngine1.Contains(pa1DeploymentInfo[0].DeploymentId));

		ProcessApplicationInfo pa2Info = getProcessApplicationDeploymentInfo("pa2");

		IList<ProcessApplicationDeploymentInfo> pa2DeploymentInfo = pa2Info.DeploymentInfo;
		Assert.assertEquals(1, pa2DeploymentInfo.Count);
		Assert.assertTrue(registeredDeploymentsForDefaultEngine.Contains(pa2DeploymentInfo[0].DeploymentId));
	  }

	  private ProcessApplicationInfo getProcessApplicationDeploymentInfo(string applicationName)
	  {
		ProcessApplicationInfo processApplicationInfo = BpmPlatform.ProcessApplicationService.getProcessApplicationInfo(applicationName);
		if (processApplicationInfo == null)
		{
		  processApplicationInfo = BpmPlatform.ProcessApplicationService.getProcessApplicationInfo("/" + applicationName);
		}
		return processApplicationInfo;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OperateOnDeployment("pa1") @Test public void testDeploymentAwareJobAcquisition()
	  public virtual void testDeploymentAwareJobAcquisition()
	  {
		JobExecutor jobExecutor1 = engine1Configuration.JobExecutor;

		ProcessInstance instance1 = engine1.RuntimeService.startProcessInstanceByKey("archive1Process");
		ProcessInstance instance2 = processEngine.RuntimeService.startProcessInstanceByKey("archive2Process");

		Job job1 = managementService.createJobQuery().processInstanceId(instance1.Id).singleResult();
		Job job2 = managementService.createJobQuery().processInstanceId(instance2.Id).singleResult();


		// the deployment aware configuration should only return the jobs of the registered deployments
		CommandExecutor commandExecutor = engine1Configuration.CommandExecutorTxRequired;
		AcquiredJobs acquiredJobs = commandExecutor.execute(new AcquireJobsCmd(jobExecutor1));

		Assert.assertEquals(1, acquiredJobs.size());
		Assert.assertTrue(acquiredJobs.contains(job1.Id));
		Assert.assertFalse(acquiredJobs.contains(job2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OperateOnDeployment("pa1") @Test public void testDeploymentUnawareJobAcquisition()
	  public virtual void testDeploymentUnawareJobAcquisition()
	  {
		JobExecutor defaultJobExecutor = processEngineConfiguration.JobExecutor;

		ProcessInstance instance1 = engine1.RuntimeService.startProcessInstanceByKey("archive1Process");
		ProcessInstance instance2 = processEngine.RuntimeService.startProcessInstanceByKey("archive2Process");

		Job job1 = managementService.createJobQuery().processInstanceId(instance1.Id).singleResult();
		Job job2 = managementService.createJobQuery().processInstanceId(instance2.Id).singleResult();

		// the deployment unaware configuration should return both jobs
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		processEngineConfiguration.JobExecutorDeploymentAware = false;
		try
		{
		  AcquiredJobs acquiredJobs = commandExecutor.execute(new AcquireJobsCmd(defaultJobExecutor));

		  Assert.assertEquals(2, acquiredJobs.size());
		  Assert.assertTrue(acquiredJobs.contains(job1.Id));
		  Assert.assertTrue(acquiredJobs.contains(job2.Id));
		}
		finally
		{
		  processEngineConfiguration.JobExecutorDeploymentAware = true;
		}
	  }
	}

}