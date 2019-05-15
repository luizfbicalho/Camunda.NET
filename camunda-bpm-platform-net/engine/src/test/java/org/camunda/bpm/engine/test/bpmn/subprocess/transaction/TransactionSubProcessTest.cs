using System;
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
namespace org.camunda.bpm.engine.test.bpmn.subprocess.transaction
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;


	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ActivityInstanceAssert = org.camunda.bpm.engine.test.util.ActivityInstanceAssert;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class TransactionSubProcessTest : PluggableProcessEngineTestCase
	{


	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testSimpleCase.bpmn20.xml"})]
	  public virtual void testSimpleCaseTxSuccessful()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(5, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookHotel").count());
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// the task is present:
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// making the tx succeed:
		taskService.setVariable(task.Id, "confirmed", true);
		taskService.complete(task.Id);

		// now the process instance execution is sitting in the 'afterSuccess' task
		// -> has left the transaction using the "normal" sequence flow
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterSuccess"));

		// there is a compensate event subscription for the transaction under the process instance
		EventSubscriptionEntity eventSubscriptionEntity = (EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("tx").executionId(processInstance.Id).singleResult();

		// there is an event-scope execution associated with the event-subscription:
		assertNotNull(eventSubscriptionEntity.Configuration);
		Execution eventScopeExecution = runtimeService.createExecutionQuery().executionId(eventSubscriptionEntity.Configuration).singleResult();
		assertNotNull(eventScopeExecution);

		// there is a compensate event subscription for the miBody of 'bookHotel' activity
		EventSubscriptionEntity miBodyEventSubscriptionEntity = (EventSubscriptionEntity) runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("bookHotel" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX).executionId(eventScopeExecution.Id).singleResult();
		assertNotNull(miBodyEventSubscriptionEntity);
		string miBodyEventScopeExecutionId = miBodyEventSubscriptionEntity.Configuration;

		// we still have compensate event subscriptions for the compensation handlers, only now they are part of the event scope
		assertEquals(5, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookHotel").executionId(miBodyEventScopeExecutionId).count());
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").executionId(eventScopeExecution.Id).count());
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoChargeCard").executionId(eventScopeExecution.Id).count());

		// assert that the compensation handlers have not been invoked:
		assertNull(runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertNull(runtimeService.getVariable(processInstance.Id, "undoBookFlight"));
		assertNull(runtimeService.getVariable(processInstance.Id, "undoChargeCard"));

		// end the process instance
		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testSimpleCase.bpmn20.xml"})]
	  public virtual void testActivityInstanceTreeAfterSuccessfulCompletion()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// the tx task is present
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// making the tx succeed
		taskService.setVariable(task.Id, "confirmed", true);
		taskService.complete(task.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		ActivityInstanceAssert.assertThat(tree).hasStructure(ActivityInstanceAssert.describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("afterSuccess").done());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testWaitstateCompensationHandler.bpmn20.xml"})]
	  public virtual void testWaitstateCompensationHandler()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(5, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookHotel").count());
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// the task is present:
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// making the tx fail:
		taskService.setVariable(task.Id, "confirmed", false);
		taskService.complete(task.Id);

		// now there are two user task instances (the compensation handlers):

		IList<Task> undoBookHotel = taskService.createTaskQuery().taskDefinitionKey("undoBookHotel").list();
		IList<Task> undoBookFlight = taskService.createTaskQuery().taskDefinitionKey("undoBookFlight").list();

		assertEquals(5,undoBookHotel.Count);
		assertEquals(1,undoBookFlight.Count);

		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(processInstance.Id);
		IList<ActivityInstance> undoBookHotelInstances = getInstancesForActivityId(rootActivityInstance, "undoBookHotel");
		IList<ActivityInstance> undoBookFlightInstances = getInstancesForActivityId(rootActivityInstance, "undoBookFlight");
		assertEquals(5, undoBookHotelInstances.Count);
		assertEquals(1, undoBookFlightInstances.Count);

		assertThat(describeActivityInstanceTree(processInstance.Id).beginScope("tx").activity("failure").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookFlight").done());

		foreach (Task t in undoBookHotel)
		{
		  taskService.complete(t.Id);
		}
		taskService.complete(undoBookFlight[0].Id);

		// now the process instance execution is sitting in the 'afterCancellation' task
		// -> has left the transaction using the cancel boundary event
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterCancellation"));

		// we have no more compensate event subscriptions
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("compensate").count());

		// end the process instance
		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testSimpleCase.bpmn20.xml"})]
	  public virtual void testSimpleCaseTxCancelled()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(5, runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookHotel").count());
		assertEquals(1,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// the task is present:
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// making the tx fail:
		taskService.setVariable(task.Id, "confirmed", false);
		taskService.complete(task.Id);

		// we have no more compensate event subscriptions
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").count());

		// assert that the compensation handlers have been invoked:
		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoBookFlight"));
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoChargeCard"));

		// signal compensation handler completion
		IList<Execution> compensationHandlerExecutions = collectExecutionsFor("undoBookHotel", "undoBookFlight", "undoChargeCard");
		foreach (Execution execution in compensationHandlerExecutions)
		{
		  runtimeService.signal(execution.Id);
		}

		// now the process instance execution is sitting in the 'afterCancellation' task
		// -> has left the transaction using the cancel boundary event
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterCancellation"));

		// if we have history, we check that the invocation of the compensation handlers is recorded in history.
		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("undoBookFlight").count());

		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());

		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("undoChargeCard").count());
		}

		// end the process instance
		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelEndConcurrent()
	  public virtual void testCancelEndConcurrent()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(5,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookHotel").count());
		assertEquals(1,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// the task is present:
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// making the tx fail:
		taskService.setVariable(task.Id, "confirmed", false);
		taskService.complete(task.Id);

		// we have no more compensate event subscriptions
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").count());

		// assert that the compensation handlers have been invoked:
		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoBookFlight"));

		// signal compensation handler completion
		IList<Execution> compensationHandlerExecutions = collectExecutionsFor("undoBookHotel", "undoBookFlight");
		foreach (Execution execution in compensationHandlerExecutions)
		{
		  runtimeService.signal(execution.Id);
		}

		// now the process instance execution is sitting in the 'afterCancellation' task
		// -> has left the transaction using the cancel boundary event
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterCancellation"));

		// if we have history, we check that the invocation of the compensation handlers is recorded in history.
		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("undoBookFlight").count());

		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		}

		// end the process instance
		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedCancelInner()
	  public virtual void testNestedCancelInner()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());
		assertEquals(5,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookHotel").count());
		assertEquals(1,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookFlight").count());

		// the tasks are present:
		Task taskInner = taskService.createTaskQuery().taskDefinitionKey("innerTxaskCustomer").singleResult();
		Task taskOuter = taskService.createTaskQuery().taskDefinitionKey("bookFlight").singleResult();
		assertNotNull(taskInner);
		assertNotNull(taskOuter);

		// making the tx fail:
		taskService.setVariable(taskInner.Id, "confirmed", false);
		taskService.complete(taskInner.Id);

		// we have no more compensate event subscriptions for the inner tx
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookHotel").count());
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookFlight").count());

		// we do not have a subscription or the outer tx yet
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// assert that the compensation handlers have been invoked:
		assertEquals(5, runtimeService.getVariable(processInstance.Id, "innerTxundoBookHotel"));
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "innerTxundoBookFlight"));

		// signal compensation handler completion
		IList<Execution> compensationHandlerExecutions = collectExecutionsFor("innerTxundoBookFlight", "innerTxundoBookHotel");
		foreach (Execution execution in compensationHandlerExecutions)
		{
		  runtimeService.signal(execution.Id);
		}

		// now the process instance execution is sitting in the 'afterInnerCancellation' task
		// -> has left the transaction using the cancel boundary event
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterInnerCancellation"));

		// if we have history, we check that the invocation of the compensation handlers is recorded in history.
		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("innerTxundoBookHotel").count());

		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("innerTxundoBookFlight").count());
		}

		// complete the task in the outer tx
		taskService.complete(taskOuter.Id);

		// end the process instance (signal the execution still sitting in afterInnerCancellation)
		runtimeService.signal(runtimeService.createExecutionQuery().activityId("afterInnerCancellation").singleResult().Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedCancelOuter()
	  public virtual void testNestedCancelOuter()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// after the process is started, we have compensate event subscriptions:
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());
		assertEquals(5,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookHotel").count());
		assertEquals(1,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookFlight").count());

		// the tasks are present:
		Task taskInner = taskService.createTaskQuery().taskDefinitionKey("innerTxaskCustomer").singleResult();
		Task taskOuter = taskService.createTaskQuery().taskDefinitionKey("bookFlight").singleResult();
		assertNotNull(taskInner);
		assertNotNull(taskOuter);

		// making the outer tx fail (invokes cancel end event)
		taskService.complete(taskOuter.Id);

		// now the process instance is sitting in 'afterOuterCancellation'
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);
		assertTrue(activeActivityIds.Contains("afterOuterCancellation"));

		// we have no more compensate event subscriptions
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookHotel").count());
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("innerTxundoBookFlight").count());
		assertEquals(0,runtimeService.createEventSubscriptionQuery().eventType("compensate").activityId("undoBookFlight").count());

		// the compensation handlers of the inner tx have not been invoked
		assertNull(runtimeService.getVariable(processInstance.Id, "innerTxundoBookHotel"));
		assertNull(runtimeService.getVariable(processInstance.Id, "innerTxundoBookFlight"));

		// the compensation handler in the outer tx has been invoked
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoBookFlight"));

		// end the process instance (signal the execution still sitting in afterOuterCancellation)
		runtimeService.signal(runtimeService.createExecutionQuery().activityId("afterOuterCancellation").singleResult().Id);

		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());

	  }

	  /*
	   * The cancel end event cancels all instances, compensation is performed for all instances
	   *
	   * see spec page 470:
	   * "If the cancelActivity attribute is set, the Activity the Event is attached to is then
	   * cancelled (in case of a multi-instance, all its instances are cancelled);"
	   */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultiInstanceTx()
	  public virtual void testMultiInstanceTx()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// there are now 5 instances of the transaction:

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().eventType("compensate").list();

		// there are 10 compensation event subscriptions
		assertEquals(10, eventSubscriptions.Count);

		Task task = taskService.createTaskQuery().listPage(0, 1).get(0);

		// canceling one instance triggers compensation for all other instances:
		taskService.setVariable(task.Id, "confirmed", false);
		taskService.complete(task.Id);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertEquals(1, runtimeService.getVariable(processInstance.Id, "undoBookFlight"));

		runtimeService.signal(runtimeService.createExecutionQuery().activityId("afterCancellation").singleResult().Id);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testMultiInstanceTx.bpmn20.xml"})]
	  public virtual void testMultiInstanceTxSuccessful()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("transactionProcess");

		// there are now 5 instances of the transaction:

		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().eventType("compensate").list();

		// there are 10 compensation event subscriptions
		assertEquals(10, eventSubscriptions.Count);

		// first complete the inner user-tasks
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.setVariable(task.Id, "confirmed", true);
		  taskService.complete(task.Id);
		}

		// now complete the inner receive tasks
		IList<Execution> executions = runtimeService.createExecutionQuery().activityId("receive").list();
		foreach (Execution execution in executions)
		{
		  runtimeService.signal(execution.Id);
		}

		runtimeService.signal(runtimeService.createExecutionQuery().activityId("afterSuccess").singleResult().Id);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateSubprocess()
	  public virtual void testCompensateSubprocess()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("txProcess");

		Task innerTask = taskService.createTaskQuery().singleResult();
		taskService.complete(innerTask.Id);

		// when the transaction is cancelled
		runtimeService.setVariable(instance.Id, "cancelTx", true);
		runtimeService.setVariable(instance.Id, "compensate", false);
		Task beforeCancelTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCancelTask.Id);

		// then compensation is triggered
		Task compensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationTask);
		assertEquals("undoInnerTask", compensationTask.TaskDefinitionKey);
		taskService.complete(compensationTask.Id);

		// and the process instance ends successfully
		Task afterBoundaryTask = taskService.createTaskQuery().singleResult();
		assertEquals("afterCancel", afterBoundaryTask.TaskDefinitionKey);
		taskService.complete(afterBoundaryTask.Id);
		assertProcessEnded(instance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateTransactionWithEventSubprocess()
	  public virtual void testCompensateTransactionWithEventSubprocess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("txProcess");
		Task beforeCancelTask = taskService.createTaskQuery().singleResult();

		// when the transaction is cancelled and handled by an event subprocess
		taskService.complete(beforeCancelTask.Id);

		// then completing compensation works
		Task compensationHandler = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationHandler);
		assertEquals("blackBoxCompensationHandler", compensationHandler.TaskDefinitionKey);

		taskService.complete(compensationHandler.Id);

		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testCompensateTransactionWithEventSubprocess.bpmn20.xml")]
	  public virtual void testCompensateTransactionWithEventSubprocessActivityInstanceTree()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("txProcess");
		Task beforeCancelTask = taskService.createTaskQuery().singleResult();

		// when the transaction is cancelled and handled by an event subprocess
		taskService.complete(beforeCancelTask.Id);

		// then the activity instance tree is correct
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("cancelEnd").beginScope("innerSubProcess").activity("blackBoxCompensationHandler").beginScope("eventSubProcess").activity("eventSubProcessThrowCompensation").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testCompensateSubprocess.bpmn20.xml")]
	  public virtual void testCompensateSubprocessNotTriggered()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("txProcess");

		Task innerTask = taskService.createTaskQuery().singleResult();
		taskService.complete(innerTask.Id);

		// when the transaction is not cancelled
		runtimeService.setVariable(instance.Id, "cancelTx", false);
		runtimeService.setVariable(instance.Id, "compensate", false);
		Task beforeEndTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeEndTask.Id);

		// then
		Task afterTxTask = taskService.createTaskQuery().singleResult();
		assertEquals("afterTx", afterTxTask.TaskDefinitionKey);

		// and the process has ended
		taskService.complete(afterTxTask.Id);
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testCompensateSubprocess.bpmn20.xml")]
	  public virtual void testCompensateSubprocessAfterTxCompletion()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("txProcess");

		Task innerTask = taskService.createTaskQuery().singleResult();
		taskService.complete(innerTask.Id);

		// when the transaction is not cancelled
		runtimeService.setVariable(instance.Id, "cancelTx", false);
		runtimeService.setVariable(instance.Id, "compensate", true);
		Task beforeTxEndTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeTxEndTask.Id);

		// but when compensation is thrown after the tx has completed successfully
		Task afterTxTask = taskService.createTaskQuery().singleResult();
		taskService.complete(afterTxTask.Id);

		// then compensation for the subprocess is triggered
		Task compensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationTask);
		assertEquals("undoInnerTask", compensationTask.TaskDefinitionKey);
		taskService.complete(compensationTask.Id);

		// and the process has ended
		assertProcessEnded(instance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILURE_testMultipleCompensationOfCancellationOfMultipleTx()
	  public virtual void FAILURE_testMultipleCompensationOfCancellationOfMultipleTx()
	  {
		// when
		IList<string> devices = new List<string>();
		  devices.Add("device1");
		devices.Add("device2");
		devices.Add("fail");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("order", Variables.putValue("devices", devices));

		// then the compensation should be triggered three times
		int expected = 3;
		int actual = historyService.createHistoricActivityInstanceQuery().activityId("ServiceTask_CompensateConfiguration").list().size();
		assertEquals(expected, actual);
	  }

	  public virtual void testMultipleCancelBoundaryFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testMultipleCancelBoundaryFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  if (!e.Message.contains("multiple boundary events with cancelEventDefinition not supported on same transaction"))
		  {
			fail("different exception expected");
		  }
		}
	  }

	  public virtual void testCancelBoundaryNoTransactionFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testCancelBoundaryNoTransactionFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  if (!e.Message.contains("boundary event with cancelEventDefinition only supported on transaction subprocesses"))
		  {
			fail("different exception expected");
		  }
		}
	  }

	  public virtual void testCancelEndNoTransactionFails()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/subprocess/transaction/TransactionSubProcessTest.testCancelEndNoTransactionFails.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  if (!e.Message.contains("end event with cancelEventDefinition only supported inside transaction subprocess"))
		  {
			fail("different exception expected");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseWithDI()
	  public virtual void testParseWithDI()
	  {

		// this test simply makes sure we can parse a transaction subprocess with DI information
		// the actual transaction behavior is tested by other testcases

		//// failing case

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("TransactionSubProcessTest");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariable(task.Id, "confirmed", false);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);


		////// success case

		processInstance = runtimeService.startProcessInstanceByKey("TransactionSubProcessTest");

		task = taskService.createTaskQuery().singleResult();
		taskService.setVariable(task.Id, "confirmed", true);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

	  protected internal virtual IList<Execution> collectExecutionsFor(params string[] activityIds)
	  {
		IList<Execution> executions = new List<Execution>();

		foreach (string activityId in activityIds)
		{
		  ((IList<Execution>)executions).AddRange(runtimeService.createExecutionQuery().activityId(activityId).list());
		}

		return executions;
	  }
	}

}