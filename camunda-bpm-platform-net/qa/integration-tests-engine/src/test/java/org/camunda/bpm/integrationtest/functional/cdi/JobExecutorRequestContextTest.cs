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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RequestScopedDelegateBean = org.camunda.bpm.integrationtest.functional.cdi.beans.RequestScopedDelegateBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// These test cases verify that the CDI RequestContext is active,
	/// when the job executor executes a job and is scoped as expected
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobExecutorRequestContextTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobExecutorRequestContextTest : AbstractFoxPlatformIntegrationTest
	{

	  /// 
	  private const int _6000 = 6000;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
	  public static WebArchive processArchive()
	  {
		return initWebArchiveDeployment().addClass(typeof(RequestScopedDelegateBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/JobExecutorRequestContextTest.testResolveBean.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/JobExecutorRequestContextTest.testScoping.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/JobExecutorRequestContextTest.testScopingExclusiveJobs.bpmn20.xml");
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveBean()
	  public virtual void testResolveBean()
	  {

		// verifies that @RequestScoped Beans can be resolved

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testResolveBean");

		waitForJobExecutorToProcessAllJobs();

		object variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		Assert.assertEquals(1, variable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScoping()
	  public virtual void testScoping()
	  {

		// verifies that if the same @RequestScoped Bean is invoked multiple times
		// in the context of the same job, we get the same instance

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testScoping");

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
//ORIGINAL LINE: @Test public void testScopingExclusiveJobs()
	  public virtual void testScopingExclusiveJobs()
	  {

		// verifies that if the same @RequestScoped Bean is invoked
		// in the context of two subsequent exclusive jobs, we have
		// seperate requests for each job, eben if the jobs are executed
		// subsequently by the same thread.

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testScopingExclusiveJobs");

		waitForJobExecutorToProcessAllJobs();

		object variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		// -> seperate requests
		Assert.assertEquals(1, variable);

		Task task = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).singleResult();
		taskService.complete(task.Id);

		waitForJobExecutorToProcessAllJobs();

		variable = runtimeService.getVariable(pi.Id, "invocationCounter");
		Assert.assertEquals(1, variable);

	  }

	}

}