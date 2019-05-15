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
namespace org.camunda.bpm.integrationtest.functional.ejb.request
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using InvocationCounter = org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounter;
	using InvocationCounterDelegateBean = org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounterDelegateBean;
	using InvocationCounterService = org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounterService;
	using InvocationCounterServiceBean = org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounterServiceBean;
	using InvocationCounterServiceLocal = org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounterServiceLocal;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
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
	/// This test verifies that if a delegate bean invoked from the Job Executor
	/// calls a REMOTE SLSB from a different deployment, the RequestContext is active there as well.
	/// 
	/// NOTE:
	/// - does not work on Jboss AS with a remote invocation (Bug in Jboss AS?) SEE HEMERA-2453
	/// - works on Glassfish
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobExecutorRequestContextRemoteInvocationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobExecutorRequestContextRemoteInvocationTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa", order:2)]
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(InvocationCounterDelegateBean)).addClass(typeof(InvocationCounterService)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/request/JobExecutorRequestContextRemoteInvocationTest.testContextPropagationEjbRemote.bpmn20.xml");
		}

	  [Deployment(order:1)]
	  public static WebArchive delegateDeployment()
	  {
		return ShrinkWrap.create(typeof(WebArchive), "service.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(InvocationCounter)).addClass(typeof(InvocationCounterService)).addClass(typeof(InvocationCounterServiceLocal)).addClass(typeof(InvocationCounterServiceBean)); // @Stateless ejb
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa") public void testRequestContextPropagationEjbRemote()
	  public virtual void testRequestContextPropagationEjbRemote()
	  {

		// This test verifies that if a delegate bean invoked from the Job Executor
		// calls an EJB from a different deployment, the RequestContext is active there as well.

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testContextPropagationEjbRemote");

		waitForJobExecutorToProcessAllJobs();

		object variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		Assert.assertEquals(1, variable);

		// set the variable back to 0
		runtimeService.setVariable(pi.Id, "invocationCounter", 0);

		Task task = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).singleResult();
		taskService.complete(task.Id);

		waitForJobExecutorToProcessAllJobs();

		variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		// now it's '1' again! -> new instance of the bean
		Assert.assertEquals(1, variable);
	  }
	}

}