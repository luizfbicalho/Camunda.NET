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
namespace org.camunda.bpm.integrationtest.jobexecutor
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using PriorityBean = org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using InSequence = org.jboss.arquillian.junit.InSequence;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// Requires fix for CAM-3163
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) @Ignore public class JobPrioritizationDuringDeploymentTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobPrioritizationDuringDeploymentTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource protected org.jboss.arquillian.container.test.api.Deployer deployer;
		protected internal Deployer deployer;

	  public override void setupBeforeTest()
	  {
		// don't lookup the default engine since this method is not executed in the deployment
	  }

	  // deploy this manually
	  [Deployment(name:"timerStart", managed : false)]
	  public static WebArchive createTimerStartDeployment()
	  {
		return initWebArchiveDeployment().addClass(typeof(PriorityBean)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationDuringDeploymentTest.timerStart.bpmn20.xml");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @InSequence(1) public void testPriorityOnTimerStartEvent()
	  public virtual void testPriorityOnTimerStartEvent()
	  {
		// when
		try
		{
		  deployer.deploy("timerStart");

		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		  Assert.fail("deployment should be successful, i.e. bean for timer start event should get resolved");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("timerStart") @InSequence(2) public void testAssertPriority()
	  public virtual void testAssertPriority()
	  {

		// then the timer start event job has the priority resolved from the bean
		Job job = managementService.createJobQuery().activityId("timerStart").singleResult();

		Assert.assertNotNull(job);
		Assert.assertEquals(PriorityBean.PRIORITY, job.Priority);
	  }
	}

}