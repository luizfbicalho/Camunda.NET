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
namespace org.camunda.bpm.integrationtest.functional.ejb.local
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BusinessInterface = org.camunda.bpm.integrationtest.functional.ejb.local.bean.BusinessInterface;
	using LocalSLSBClientDelegateBean = org.camunda.bpm.integrationtest.functional.ejb.local.bean.LocalSLSBClientDelegateBean;
	using LocalSLSBean = org.camunda.bpm.integrationtest.functional.ejb.local.bean.LocalSLSBean;
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
	/// This test verifies that a CDI Java Bean Delegate is able to inject and invoke the
	/// local business interface of a SLSB from a different application
	/// 
	/// Note:
	/// - works on Jboss
	/// - not implemented on Glassfish
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class LocalSLSBInvocationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class LocalSLSBInvocationTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa", order:2)]
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(LocalSLSBClientDelegateBean)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/local/LocalSLSBInvocationTest.testInvokeBean.bpmn20.xml").addAsWebInfResource("org/camunda/bpm/integrationtest/functional/ejb/local/jboss-deployment-structure.xml","jboss-deployment-structure.xml");
		}

	  [Deployment(order:1)]
	  public static WebArchive delegateDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "service.war").addAsLibraries(DeploymentHelper.EjbClient).addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(LocalSLSBean)).addClass(typeof(BusinessInterface)); // the business interface

		TestContainer.addContainerSpecificResourcesForNonPa(webArchive);

		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa") public void testInvokeBean() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInvokeBean()
	  {

		// this testcase first resolves the Bean synchronously and then from the JobExecutor

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testInvokeBean");

		Assert.assertEquals(runtimeService.getVariable(pi.Id, "result"), true);

		runtimeService.setVariable(pi.Id, "result", false);

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);

		waitForJobExecutorToProcessAllJobs();

		Assert.assertEquals(runtimeService.getVariable(pi.Id, "result"), true);

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleInvocations()
	  public virtual void testMultipleInvocations()
	  {

		// this is greater than any Datasource / EJB / Thread Pool size -> make sure all resources are released properly.
		int instances = 100;
		string[] ids = new string[instances];

		for (int i = 0; i < instances; i++)
		{
		  ids[i] = runtimeService.startProcessInstanceByKey("testInvokeBean").Id;
		  Assert.assertEquals(runtimeService.getVariable(ids[i], "result"), true);
		  runtimeService.setVariable(ids[i], "result", false);
		  taskService.complete(taskService.createTaskQuery().processInstanceId(ids[i]).singleResult().Id);
		}

		waitForJobExecutorToProcessAllJobs(60 * 1000);

		for (int i = 0; i < instances; i++)
		{
		  Assert.assertEquals(runtimeService.getVariable(ids[i], "result"), true);
		  taskService.complete(taskService.createTaskQuery().processInstanceId(ids[i]).singleResult().Id);
		}

	  }


	}

}