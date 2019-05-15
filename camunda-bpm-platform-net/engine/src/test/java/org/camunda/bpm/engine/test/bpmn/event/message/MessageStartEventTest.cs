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
namespace org.camunda.bpm.engine.test.bpmn.@event.message
{
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class MessageStartEventTest : PluggableProcessEngineTestCase
	{

	  public virtual void testDeploymentCreatesSubscriptions()
	  {
		string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").deploy().Id;

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(1, eventSubscriptions.Count);

		repositoryService.deleteDeployment(deploymentId);
	  }

	  public virtual void testSameMessageNameFails()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").deploy().Id;
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/otherProcessWithNewInvoiceMessage.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("there already is a message event subscription for the message with name"));
		}
		finally
		{
		  // clean db:
		  IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		  foreach (org.camunda.bpm.engine.repository.Deployment deployment in deployments)
		  {
			repositoryService.deleteDeployment(deployment.Id, true);
		  }
		  // Workaround for #CAM-4250: remove process definition of failed
		  // deployment from deployment cache
		  processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.clear();
		}
	  }

	  // SEE: https://app.camunda.com/jira/browse/CAM-1448
	  public virtual void testEmptyMessageNameFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testEmptyMessageNameFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Cannot have a message event subscription with an empty or missing name"));
		}
	  }

	  public virtual void testSameMessageNameInSameProcessFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/testSameMessageNameInSameProcessFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Cannot have more than one message event subscription with name 'newInvoiceMessage' for scope"));
		}
	  }

	  public virtual void testUpdateProcessVersionCancelsSubscriptions()
	  {
		string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").deploy().Id;

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, eventSubscriptions.Count);
		assertEquals(1, processDefinitions.Count);

		string newDeploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").deploy().Id;

		IList<EventSubscription> newEventSubscriptions = runtimeService.createEventSubscriptionQuery().list();
		IList<ProcessDefinition> newProcessDefinitions = repositoryService.createProcessDefinitionQuery().list();

		assertEquals(1, newEventSubscriptions.Count);
		assertEquals(2, newProcessDefinitions.Count);
		foreach (ProcessDefinition processDefinition in newProcessDefinitions)
		{
		  if (processDefinition.Version == 1)
		  {
			foreach (EventSubscription subscription in newEventSubscriptions)
			{
			  EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity) subscription;
			  assertFalse(subscriptionEntity.Configuration.Equals(processDefinition.Id));
			}
		  }
		  else
		  {
			foreach (EventSubscription subscription in newEventSubscriptions)
			{
			  EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity) subscription;
			  assertTrue(subscriptionEntity.Configuration.Equals(processDefinition.Id));
			}
		  }
		}
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: assertFalse(eventSubscriptions.equals(newEventSubscriptions));
		assertFalse(eventSubscriptions.SequenceEqual(newEventSubscriptions));

		repositoryService.deleteDeployment(deploymentId);
		repositoryService.deleteDeployment(newDeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleMessageStartEvent()
	  public virtual void testSingleMessageStartEvent()
	  {

		// using startProcessInstanceByMessage triggers the message start event

		ProcessInstance processInstance = runtimeService.startProcessInstanceByMessage("newInvoiceMessage");

		assertFalse(processInstance.Ended);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

		// using startProcessInstanceByKey also triggers the message event, if there is a single start event

		processInstance = runtimeService.startProcessInstanceByKey("singleMessageStartEvent");

		assertFalse(processInstance.Ended);

		task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageStartEventAndNoneStartEvent()
	  public virtual void testMessageStartEventAndNoneStartEvent()
	  {

		// using startProcessInstanceByKey triggers the none start event

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		assertFalse(processInstance.Ended);

		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterNoneStart").singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

		// using startProcessInstanceByMessage triggers the message start event

		processInstance = runtimeService.startProcessInstanceByMessage("newInvoiceMessage");

		assertFalse(processInstance.Ended);

		task = taskService.createTaskQuery().taskDefinitionKey("taskAfterMessageStart").singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleMessageStartEvents()
	  public virtual void testMultipleMessageStartEvents()
	  {

		// sending newInvoiceMessage

		ProcessInstance processInstance = runtimeService.startProcessInstanceByMessage("newInvoiceMessage");

		assertFalse(processInstance.Ended);

		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterMessageStart").singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

		// sending newInvoiceMessage2

		processInstance = runtimeService.startProcessInstanceByMessage("newInvoiceMessage2");

		assertFalse(processInstance.Ended);

		task = taskService.createTaskQuery().taskDefinitionKey("taskAfterMessageStart2").singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);

		// starting the process using startProcessInstanceByKey is not possible:
		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue("different exception expected, not " + e.Message, e.Message.contains("has no default start activity"));
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeployStartAndIntermediateEventWithSameMessageInSameProcess()
	  public virtual void testDeployStartAndIntermediateEventWithSameMessageInSameProcess()
	  {
		ProcessInstance pi = null;
		try
		{
		  runtimeService.startProcessInstanceByMessage("message");
		  pi = runtimeService.createProcessInstanceQuery().singleResult();
		  assertThat(pi.Ended, @is(false));

		  string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageInSameProcess.bpmn").name("deployment2").deploy().Id;
		  assertThat(repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult(), @is(notNullValue()));
		}
		finally
		{
		  // clean db:
		  runtimeService.deleteProcessInstance(pi.Id, "failure");
		  IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		  foreach (org.camunda.bpm.engine.repository.Deployment d in deployments)
		  {
			repositoryService.deleteDeployment(d.Id, true);
		  }
		  // Workaround for #CAM-4250: remove process definition of failed
		  // deployment from deployment cache

		  processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.clear();
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses.bpmn"})]
	  public virtual void testDeployStartAndIntermediateEventWithSameMessageDifferentProcessesFirstStartEvent()
	  {
		ProcessInstance pi = null;
		try
		{
		  runtimeService.startProcessInstanceByMessage("message");
		  pi = runtimeService.createProcessInstanceQuery().singleResult();
		  assertThat(pi.Ended, @is(false));

		  string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses2.bpmn").name("deployment2").deploy().Id;
		  assertThat(repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult(), @is(notNullValue()));
		}
		finally
		{
		  // clean db:
		  runtimeService.deleteProcessInstance(pi.Id, "failure");
		  IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		  foreach (org.camunda.bpm.engine.repository.Deployment d in deployments)
		  {
			repositoryService.deleteDeployment(d.Id, true);
		  }
		  // Workaround for #CAM-4250: remove process definition of failed
		  // deployment from deployment cache

		  processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.clear();
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses2.bpmn"})]
	  public virtual void testDeployStartAndIntermediateEventWithSameMessageDifferentProcessesFirstIntermediateEvent()
	  {
		ProcessInstance pi = null;
		try
		{
		  runtimeService.startProcessInstanceByKey("Process_2");
		  pi = runtimeService.createProcessInstanceQuery().singleResult();
		  assertThat(pi.Ended, @is(false));

		  string deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses.bpmn").name("deployment2").deploy().Id;
		  assertThat(repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult(), @is(notNullValue()));
		}
		finally
		{
		  // clean db:
		  runtimeService.deleteProcessInstance(pi.Id, "failure");
		  IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();
		  foreach (org.camunda.bpm.engine.repository.Deployment d in deployments)
		  {
			repositoryService.deleteDeployment(d.Id, true);
		  }
		  // Workaround for #CAM-4250: remove process definition of failed
		  // deployment from deployment cache

		  processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.clear();
		}
	  }

	  public virtual void testUsingExpressionWithDollarTagInMessageStartEventNameThrowsException()
	  {

		// given a process definition with a start message event that has a message name which contains an expression
		string processDefinition = "org/camunda/bpm/engine/test/bpmn/event/message/" +
				"MessageStartEventTest.testUsingExpressionWithDollarTagInMessageStartEventNameThrowsException.bpmn20.xml";
		try
		{
		  // when deploying the process
		  repositoryService.createDeployment().addClasspathResource(processDefinition).deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then a process engine exception should be thrown with a certain message
		  assertTrue(e.Message.contains("Invalid message name"));
		  assertTrue(e.Message.contains("expressions in the message start event name are not allowed!"));
		}
	  }

	  public virtual void testUsingExpressionWithHashTagInMessageStartEventNameThrowsException()
	  {

		// given a process definition with a start message event that has a message name which contains an expression
		string processDefinition = "org/camunda/bpm/engine/test/bpmn/event/message/" +
				"MessageStartEventTest.testUsingExpressionWithHashTagInMessageStartEventNameThrowsException.bpmn20.xml";
		try
		{
		  // when deploying the process
		  repositoryService.createDeployment().addClasspathResource(processDefinition).deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then a process engine exception should be thrown with a certain message
		  assertTrue(e.Message.contains("Invalid message name"));
		  assertTrue(e.Message.contains("expressions in the message start event name are not allowed!"));
		}
	  }
	}

}