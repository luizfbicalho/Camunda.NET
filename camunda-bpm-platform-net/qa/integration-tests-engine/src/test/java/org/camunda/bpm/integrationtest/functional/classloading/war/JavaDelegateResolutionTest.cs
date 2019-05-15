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
namespace org.camunda.bpm.integrationtest.functional.classloading.war
{
	using ExampleDelegate = org.camunda.bpm.integrationtest.functional.classloading.beans.ExampleDelegate;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// <para>Deploys two different WAR applications, a process archive and a client application.</para>
	/// 
	/// <para>This test ensures that when the process is started from the client,
	/// it is able to make the context switch to the process archvie and resolve classes from the
	/// process archive.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JavaDelegateResolutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JavaDelegateResolutionTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeplyoment()
		public static WebArchive createProcessArchiveDeplyoment()
		{
		return initWebArchiveDeployment().addClass(typeof(ExampleDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/classloading/JavaDelegateResolutionTest.testResolveClass.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/classloading/JavaDelegateResolutionTest.testResolveClassFromJobExecutor.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(webArchive);

		return webArchive;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveClass()
	  public virtual void testResolveClass()
	  {
		// assert that we cannot load the delegate here:
		try
		{
		  Type.GetType("org.camunda.bpm.integrationtest.functional.classloading.ExampleDelegate");
		  Assert.fail("CNFE expected");
		}
		catch (ClassNotFoundException)
		{
		  // expected
		}

		// but the process can since it performs context switch to the process archive for execution
		runtimeService.startProcessInstanceByKey("testResolveClass");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveClassFromJobExecutor() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testResolveClassFromJobExecutor()
	  {

		runtimeService.startProcessInstanceByKey("testResolveClassFromJobExecutor");

		Assert.assertEquals(1, runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveClassFromJobExecutor").count());

		waitForJobExecutorToProcessAllJobs();

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("testResolveClassFromJobExecutor").count());

	  }

	}

}