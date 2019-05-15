using System;

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
namespace org.camunda.bpm.integrationtest.functional.spring
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ErrorDelegate = org.camunda.bpm.integrationtest.functional.spring.beans.ErrorDelegate;
	using RetryConfig = org.camunda.bpm.integrationtest.functional.spring.beans.RetryConfig;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>Integration test that makes sure the shared container managed process engine is able to resolve
	/// Spring beans form a process application</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class SpringRetryConfigurationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class SpringRetryConfigurationTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{

		// deploy spring Process Application (does not include ejb-client nor cdi modules)
		return ShrinkWrap.create(typeof(WebArchive), "test.war").addClass(typeof(ErrorDelegate)).addClass(typeof(RetryConfig)).addAsResource("org/camunda/bpm/integrationtest/functional/RetryConfigurationTest.testResolveRetryConfigBean.bpmn20.xml").addClass(typeof(CustomServletProcessApplication)).addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addAsWebInfResource("org/camunda/bpm/integrationtest/functional/spring/web.xml", "web.xml").addAsWebInfResource("org/camunda/bpm/integrationtest/functional/spring/SpringRetryConfigurationTest-context.xml", "applicationContext.xml").addAsLibraries(DeploymentHelper.EngineSpring).addAsManifestResource("org/camunda/bpm/integrationtest/functional/spring/jboss-deployment-structure.xml", "jboss-deployment-structure.xml");
		}


	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {

		// the test is deployed as a seperate deployment

		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveRetryConfigBean()
	  public virtual void testResolveRetryConfigBean()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testRetry");

		JobQuery query = managementService.createJobQuery().processInstanceId(processInstance.Id);

		Job job = query.singleResult();

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = query.singleResult();
		Assert.assertEquals(6, job.Retries);
	  }

	}

}