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
	using RequestScopedSFSBDelegate = org.camunda.bpm.integrationtest.functional.ejb.request.beans.RequestScopedSFSBDelegate;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// This test verifies that if the same @RequestScoped SFSB Bean is invoked multiple times
	/// in the context of the same job, we get the same instance.
	/// 
	/// NOTE:
	/// - works on Jboss AS
	/// - broken on Glassfish, see HEMERA-2454
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobExecutorRequestContextSFSBDelegateTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobExecutorRequestContextSFSBDelegateTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa", order:2)]
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(RequestScopedSFSBDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/request/JobExecutorRequestContextSFSBDelegateTest.testScopingSFSB.bpmn20.xml");
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa") public void testScopingSFSB()
	  public virtual void testScopingSFSB()
	  {

		// verifies that if the same @RequestScoped SFSB Bean is invoked multiple times
		// in the context of the same job, we get the same instance

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testScopingSFSB");

		waitForJobExecutorToProcessAllJobs();

		object variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		// -> the same bean instance was invoked 2 times!
		Assert.assertEquals(2, variable);

		Task task = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).singleResult();
		taskService.complete(task.Id);

		waitForJobExecutorToProcessAllJobs();

		variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		// now it's '1' again! -> new instance of the bean
		Assert.assertEquals(1, variable);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleInvocations()
	  public virtual void testMultipleInvocations()
	  {

		// this is greater than any Datasource- / EJB- / Thread-Pool size -> make sure all resources are released properly.
		int instances = 100;
		string[] ids = new string[instances];

		for (int i = 0; i < instances; i++)
		{
		  ids[i] = runtimeService.startProcessInstanceByKey("testScopingSFSB").Id;
		}

		waitForJobExecutorToProcessAllJobs(60 * 1000);

		for (int i = 0; i < instances; i++)
		{
		  object variable = runtimeService.getVariable(ids[i], "invocationCounter");
		  // -> the same bean instance was invoked 2 times!
		  Assert.assertEquals(2, variable);

		  taskService.complete(taskService.createTaskQuery().processInstanceId(ids[i]).singleResult().Id);
		}

		waitForJobExecutorToProcessAllJobs(60 * 1000);

		for (int i = 0; i < instances; i++)
		{
		  // now it's '1' again! -> new instance of the bean
		  Assert.assertEquals(1, runtimeService.getVariable(ids[i], "invocationCounter"));
		}


	  }

	}

}