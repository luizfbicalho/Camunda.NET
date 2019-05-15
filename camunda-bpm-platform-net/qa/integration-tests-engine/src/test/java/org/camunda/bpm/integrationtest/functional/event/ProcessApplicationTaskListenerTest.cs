using System.Collections.Generic;

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
namespace org.camunda.bpm.integrationtest.functional.@event
{
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskListenerProcessApplication = org.camunda.bpm.integrationtest.functional.@event.beans.TaskListenerProcessApplication;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ProcessApplicationTaskListenerTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class ProcessApplicationTaskListenerTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TaskListenerProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/functional/event/ProcessApplicationEventSupportTest.testTaskListener.bpmn20.xml");

		TestContainer.addContainerSpecificResourcesForNonPa(archive);

		return archive;

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskListener()
	  public virtual void testTaskListener()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE] = false;
		variables[org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT] = false;
		variables[org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE] = false;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		bool createEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE).Value;
		bool assignmentEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT).Value;
		bool completeEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE).Value;

		Assert.assertTrue(createEventFired);
		Assert.assertFalse(assignmentEventFired);
		Assert.assertFalse(completeEventFired);

		Task task = taskService.createTaskQuery().processDefinitionKey("testProcess").singleResult();
		taskService.claim(task.Id, "jonny");

		createEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE).Value;
		assignmentEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT).Value;
		completeEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE).Value;

		Assert.assertTrue(createEventFired);
		Assert.assertTrue(assignmentEventFired);
		Assert.assertFalse(completeEventFired);

		taskService.complete(task.Id);

		createEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE).Value;
		assignmentEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT).Value;
		completeEventFired = (bool?) runtimeService.getVariable(processInstance.Id, org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE).Value;

		Assert.assertTrue(createEventFired);
		Assert.assertTrue(assignmentEventFired);
		Assert.assertTrue(completeEventFired);

	  }

	}

}