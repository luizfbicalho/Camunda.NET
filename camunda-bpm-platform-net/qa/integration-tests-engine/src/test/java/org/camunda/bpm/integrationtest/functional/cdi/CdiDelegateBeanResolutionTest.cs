﻿using System;

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
	using BeanManagerLookup = org.camunda.bpm.engine.cdi.impl.util.BeanManagerLookup;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using ExampleDelegateBean = org.camunda.bpm.integrationtest.functional.cdi.beans.ExampleDelegateBean;
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
	/// <para>Deploys two different applications, a process archive and a cleint application.</para>
	/// 
	/// <para>This test ensures that when the process is started from the client,
	/// it is able to make the context switch to the process archvie and resolve cdi beans
	/// from the process archive.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiDelegateBeanResolutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiDelegateBeanResolutionTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(ExampleDelegateBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiDelegateBeanResolutionTest.testResolveBean.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiDelegateBeanResolutionTest.testResolveBeanFromJobExecutor.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		 WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(ProgrammaticBeanLookup)).addClass(typeof(BeanManagerLookup)).addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		 TestContainer.addContainerSpecificResources(webArchive);

		 return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBean()
	  public virtual void testResolveBean()
	  {
		try
		{
		  // assert that we cannot resolve the bean here:
		  ProgrammaticBeanLookup.lookup("exampleDelegateBean");
		  Assert.fail("exception expected");
		}
		catch (Exception)
		{
		  // expected
		}

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveBean").count());
		// but the process engine can:
		runtimeService.startProcessInstanceByKey("testResolveBean");

		Assert.assertEquals(0,runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveBean").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBeanFromJobExecutor()
	  public virtual void testResolveBeanFromJobExecutor()
	  {

		Assert.assertEquals(0,runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveBeanFromJobExecutor").count());
		runtimeService.startProcessInstanceByKey("testResolveBeanFromJobExecutor");
		Assert.assertEquals(1,runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveBeanFromJobExecutor").count());

		waitForJobExecutorToProcessAllJobs();

		Assert.assertEquals(0,runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveBeanFromJobExecutor").count());

	  }

	}

}