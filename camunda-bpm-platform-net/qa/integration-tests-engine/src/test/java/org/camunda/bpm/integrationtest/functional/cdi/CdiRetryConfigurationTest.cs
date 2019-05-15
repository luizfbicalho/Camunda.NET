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
namespace org.camunda.bpm.integrationtest.functional.cdi
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using DependentScopedBean = org.camunda.bpm.integrationtest.functional.cdi.beans.DependentScopedBean;
	using ErrorDelegate = org.camunda.bpm.integrationtest.functional.cdi.beans.ErrorDelegate;
	using RetryConfig = org.camunda.bpm.integrationtest.functional.cdi.beans.RetryConfig;
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiRetryConfigurationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiRetryConfigurationTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeployment()
		public static WebArchive createProcessArchiveDeployment()
		{
		return initWebArchiveDeployment().addClass(typeof(ErrorDelegate)).addClass(typeof(RetryConfig)).addAsResource("org/camunda/bpm/integrationtest/functional/RetryConfigurationTest.testResolveRetryConfigBean.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(DependentScopedBean)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBean()
	  public virtual void testResolveBean()
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