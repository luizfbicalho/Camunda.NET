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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using PriorityBean = org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobPrioritizationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobPrioritizationTest : AbstractFoxPlatformIntegrationTest
	{

	  protected internal ProcessInstance processInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
	  public static WebArchive createDeployment()
	  {
		return initWebArchiveDeployment().addClass(typeof(PriorityBean)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.priorityProcess.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.serviceTask.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.userTask.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.intermediateMessage.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (processInstance != null)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnProcessElement()
	  public virtual void testPriorityOnProcessElement()
	  {
		// given
		processInstance = runtimeService.startProcessInstanceByKey("priorityProcess");

		Job job = managementService.createJobQuery().singleResult();

		// then
		Assert.assertEquals(PriorityBean.PRIORITY, job.Priority);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnProcessStart()
	  public virtual void testPriorityOnProcessStart()
	  {

		// given
		processInstance = runtimeService.startProcessInstanceByKey("serviceTaskProcess");

		Job job = managementService.createJobQuery().singleResult();

		// then
		Assert.assertEquals(PriorityBean.PRIORITY, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnModification()
	  public virtual void testPriorityOnModification()
	  {

		// given
		processInstance = runtimeService.startProcessInstanceByKey("serviceTaskProcess");

		TransitionInstance transitionInstance = runtimeService.getActivityInstance(processInstance.Id).getTransitionInstances("serviceTask")[0];

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("serviceTask").cancelTransitionInstance(transitionInstance.Id).execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(PriorityBean.PRIORITY, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnInstantiationAtActivity()
	  public virtual void testPriorityOnInstantiationAtActivity()
	  {

		// when
		processInstance = runtimeService.createProcessInstanceByKey("serviceTaskProcess").startBeforeActivity("serviceTask").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(PriorityBean.PRIORITY, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnAsyncAfterUserTask()
	  public virtual void testPriorityOnAsyncAfterUserTask()
	  {
		// given
		processInstance = runtimeService.startProcessInstanceByKey("userTaskProcess");
		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.complete(task.Id);

		// then
		Job asyncAfterJob = managementService.createJobQuery().singleResult();
		Assert.assertEquals(PriorityBean.PRIORITY, asyncAfterJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPriorityOnAsyncAfterIntermediateCatchEvent()
	  public virtual void testPriorityOnAsyncAfterIntermediateCatchEvent()
	  {
		// given
		processInstance = runtimeService.startProcessInstanceByKey("intermediateMessageProcess");

		// when
		runtimeService.correlateMessage("Message");

		// then
		Job asyncAfterJob = managementService.createJobQuery().singleResult();
		Assert.assertEquals(PriorityBean.PRIORITY, asyncAfterJob.Priority);
	  }

	}

}