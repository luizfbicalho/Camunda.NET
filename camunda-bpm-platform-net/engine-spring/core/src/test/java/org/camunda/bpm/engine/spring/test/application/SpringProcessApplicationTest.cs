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
namespace org.camunda.bpm.engine.spring.test.application
{
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using SpringProcessApplication = org.camunda.bpm.engine.spring.application.SpringProcessApplication;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using AbstractApplicationContext = org.springframework.context.support.AbstractApplicationContext;
	using ClassPathXmlApplicationContext = org.springframework.context.support.ClassPathXmlApplicationContext;

	/// <summary>
	/// <para>Testcases for <seealso cref="SpringProcessApplication"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpringProcessApplicationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessApplicationDeployment()
	  public virtual void testProcessApplicationDeployment()
	  {

		// initially no applications are deployed:
		Assert.assertEquals(0, BpmPlatform.ProcessApplicationService.ProcessApplicationNames.Count);

		// start a spring application context
		AbstractApplicationContext applicationContext = new ClassPathXmlApplicationContext("org/camunda/bpm/engine/spring/test/application/SpringProcessApplicationDeploymentTest-context.xml");
		applicationContext.start();

		// assert that there is a process application deployed with the name of the process application bean
		Assert.assertNotNull(BpmPlatform.ProcessApplicationService.getProcessApplicationInfo("myProcessApplication"));

		// close the spring application context
		applicationContext.close();

		// after closing the application context, the process application is undeployed.
		Assert.assertNull(BpmPlatform.ProcessApplicationService.getProcessApplicationInfo("myProcessApplication"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {

		// start a spring application context
		AbstractApplicationContext applicationContext = new ClassPathXmlApplicationContext("org/camunda/bpm/engine/spring/test/application/SpringProcessArchiveDeploymentTest-context.xml");
		applicationContext.start();

		// assert the process archive is deployed:
		ProcessEngine processEngine = BpmPlatform.DefaultProcessEngine;
		Assert.assertNotNull(processEngine.RepositoryService.createDeploymentQuery().deploymentName("pa").singleResult());

		applicationContext.close();

		// assert the process is undeployed
		Assert.assertNull(processEngine.RepositoryService.createDeploymentQuery().deploymentName("pa").singleResult());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostDeployRegistrationPa()
	  public virtual void testPostDeployRegistrationPa()
	  {
		// this test verifies that a process application is able to register a deployment from the @PostDeploy callback:

		AbstractApplicationContext applicationContext = new ClassPathXmlApplicationContext("org/camunda/bpm/engine/spring/test/application/PostDeployRegistrationPaTest-context.xml");
		applicationContext.start();

		ProcessEngine processEngine = BpmPlatform.DefaultProcessEngine;

		// create a manual deployment:
		Deployment deployment = processEngine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/spring/test/application/process.bpmn20.xml").deploy();

		// lookup the process application spring bean:
		PostDeployRegistrationPa processApplication = applicationContext.getBean("customProcessApplicaiton", typeof(PostDeployRegistrationPa));

		Assert.assertFalse(processApplication.PostDeployInvoked);
		processApplication.deploy();
		Assert.assertTrue(processApplication.PostDeployInvoked);

		// the process application was not invoked
		Assert.assertFalse(processApplication.Invoked);

		// start process instance:
		processEngine.RuntimeService.startProcessInstanceByKey("startToEnd");

		// now the process application was invoked:
		Assert.assertTrue(processApplication.Invoked);

		// undeploy PA
		Assert.assertFalse(processApplication.PreUndeployInvoked);
		processApplication.undeploy();
		Assert.assertTrue(processApplication.PreUndeployInvoked);

		// manually undeploy the process
		processEngine.RepositoryService.deleteDeployment(deployment.Id, true);

		applicationContext.close();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostDeployWithNestedContext()
	  public virtual void testPostDeployWithNestedContext()
	  {
		/*
		 * This test case checks if the process application deployment is done when
		 * application context is refreshed, but not when child contexts are
		 * refreshed.
		 * 
		 * As a side test it checks if events thrown in the PostDeploy-method are
		 * catched by the main application context.
		 */

		AbstractApplicationContext applicationContext = new ClassPathXmlApplicationContext("org/camunda/bpm/engine/spring/test/application/PostDeployWithNestedContext-context.xml");
		applicationContext.start();

		// lookup the process application spring bean:
		PostDeployWithNestedContext processApplication = applicationContext.getBean("customProcessApplicaiton", typeof(PostDeployWithNestedContext));

		Assert.assertFalse(processApplication.DeployOnChildRefresh);
		Assert.assertTrue(processApplication.LateEventTriggered);

		processApplication.undeploy();
		applicationContext.close();
	  }

	}

}