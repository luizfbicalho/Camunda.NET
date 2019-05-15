using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.test.concurrency
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using MessageCorrelationBuilderImpl = org.camunda.bpm.engine.impl.MessageCorrelationBuilderImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using MessageEventReceivedCmd = org.camunda.bpm.engine.impl.cmd.MessageEventReceivedCmd;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompetingMessageCorrelationTest : ConcurrencyTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		((ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration).CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass(this));

		assertEquals(0, processEngine.HistoryService.createHistoricJobLogQuery().list().size());

		base.tearDown();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CompetingMessageCorrelationTest outerInstance;

		  public CommandAnonymousInnerClass(CompetingMessageCorrelationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<HistoricJobLog> jobLogs = outerInstance.processEngine.HistoryService.createHistoricJobLogQuery().list();
			foreach (HistoricJobLog jobLog in jobLogs)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogById(jobLog.Id);
			}

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if (DbSqlSessionFactory.H2.Equals(databaseType) && Name.Equals("testConcurrentExclusiveCorrelation"))
		{
		  // skip test method - if database is H2
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void testConcurrentCorrelationFailsWithOptimisticLockingException()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// both threads correlate
		thread1.makeContinue();
		thread2.makeContinue();

		thread1.waitForSync();
		thread2.waitForSync();

		// the service task was executed twice
		assertEquals(2, InvocationLogListener.Invocations);

		// the first thread ends its transcation
		thread1.waitUntilDone();
		assertNull(thread1.Exception);

		Task afterMessageTask = taskService.createTaskQuery().singleResult();
		assertEquals(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

		// the second thread ends its transaction and fails with optimistic locking exception
		thread2.waitUntilDone();
		assertTrue(thread2.Exception != null);
		assertTrue(thread2.Exception is OptimisticLockingException);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml") public void testConcurrentExclusiveCorrelation() throws InterruptedException
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void testConcurrentExclusiveCorrelation()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// thread one correlates and acquires the exclusive lock
		thread1.makeContinue();
		thread1.waitForSync();

		// the service task was executed once
		assertEquals(1, InvocationLogListener.Invocations);

		// thread two attempts to acquire the exclusive lock but can't since thread 1 hasn't released it yet
		thread2.makeContinue();
		Thread.Sleep(2000);

		// let the first thread ends its transaction
		thread1.makeContinue();
		assertNull(thread1.Exception);

		// thread 2 can't continue because the event subscription it tried to lock was deleted
		thread2.waitForSync();
		assertTrue(thread2.Exception != null);
		assertTrue(thread2.Exception is ProcessEngineException);
		assertTextPresent("does not have a subscription to a message event with name 'Message'", thread2.Exception.Message);

		// the first thread ended successfully without an exception
		thread1.join();
		assertNull(thread1.Exception);

		// the follow-up task was reached
		Task afterMessageTask = taskService.createTaskQuery().singleResult();
		assertEquals(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

		// the service task was not executed a second time
		assertEquals(1, InvocationLogListener.Invocations);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml") public void testConcurrentExclusiveCorrelationToDifferentExecutions() throws InterruptedException
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void testConcurrentExclusiveCorrelationToDifferentExecutions()
	  {
		InvocationLogListener.reset();

		// given a process instance
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("testProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel to each of the two instances
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", instance1.Id, true));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", instance2.Id, true));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// thread one correlates and acquires the exclusive lock on the event subscription of instance1
		thread1.makeContinue();
		thread1.waitForSync();

		// the service task was executed once
		assertEquals(1, InvocationLogListener.Invocations);

		// thread two correlates and acquires the exclusive lock on the event subscription of instance2
		// depending on the database and locking used, this may block thread2
		thread2.makeContinue();

		// thread 1 completes successfully
		thread1.waitUntilDone();
		assertNull(thread1.Exception);

		// thread2 should be able to continue at least after thread1 has finished and released its lock
		thread2.waitForSync();

		// the service task was executed the second time
		assertEquals(2, InvocationLogListener.Invocations);

		// thread 2 completes successfully
		thread2.waitUntilDone();
		assertNull(thread2.Exception);

		// the follow-up task was reached in both instances
		assertEquals(2, taskService.createTaskQuery().taskDefinitionKey("afterMessageUserTask").count());
	  }

	  /// <summary>
	  /// Fails at least on mssql; mssql appears to lock more than the actual event subscription row
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml") public void FAILING_testConcurrentExclusiveCorrelationToDifferentExecutionsCase2() throws InterruptedException
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void FAILING_testConcurrentExclusiveCorrelationToDifferentExecutionsCase2()
	  {
		InvocationLogListener.reset();

		// given a process instance
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("testProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel to each of the two instances
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", instance1.Id, true));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", instance2.Id, true));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// thread one correlates and acquires the exclusive lock on the event subscription of instance1
		thread1.makeContinue();
		thread1.waitForSync();

		// the service task was executed once
		assertEquals(1, InvocationLogListener.Invocations);

		// thread two correlates and acquires the exclusive lock on the event subscription of instance2
		thread2.makeContinue();
		// FIXME: this does not return on sql server due to locking
		thread2.waitForSync();

		// the service task was executed the second time
		assertEquals(2, InvocationLogListener.Invocations);

		// thread 2 completes successfully, even though it acquired its lock after thread 1
		thread2.waitUntilDone();
		assertNull(thread2.Exception);

		// thread 1 completes successfully
		thread1.waitUntilDone();
		assertNull(thread1.Exception);

		// the follow-up task was reached in both instances
		assertEquals(2, taskService.createTaskQuery().taskDefinitionKey("afterMessageUserTask").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml") public void testConcurrentMixedCorrelation() throws InterruptedException
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void testConcurrentMixedCorrelation()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel (one exclusive, one non-exclusive)
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// thread one correlates and acquires the exclusive lock
		thread1.makeContinue();
		thread1.waitForSync();

		// thread two correlates since it does not need a pessimistic lock
		thread2.makeContinue();
		thread2.waitForSync();

		// the service task was executed twice
		assertEquals(2, InvocationLogListener.Invocations);

		// the first thread ends its transaction and releases the lock; the event subscription is now gone
		thread1.waitUntilDone();
		assertNull(thread1.Exception);

		Task afterMessageTask = taskService.createTaskQuery().singleResult();
		assertEquals(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

		// thread two attempts to end its transaction and fails with optimistic locking
		thread2.makeContinue();
		thread2.waitForSync();

		assertTrue(thread2.Exception != null);
		assertTrue(thread2.Exception is OptimisticLockingException);
	  }

	  /// <summary>
	  /// <para>
	  ///   At least on MySQL, this test case fails with deadlock exceptions.
	  ///   The reason is the combination of our flush with the locking of the event
	  ///   subscription documented in the ticket CAM-3636.
	  /// </para> </summary>
	  /// <exception cref="InterruptedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml") public void FAILING_testConcurrentMixedCorrelationCase2() throws InterruptedException
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.catchMessageProcess.bpmn20.xml")]
	  public virtual void FAILING_testConcurrentMixedCorrelationCase2()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel (one exclusive, one non-exclusive)
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", true));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// thread one correlates and acquires no lock
		thread1.makeContinue();
		thread1.waitForSync();

		// thread two acquires a lock and succeeds because thread one hasn't acquired one
		thread2.makeContinue();
		thread2.waitForSync();

		// the service task was executed twice
		assertEquals(2, InvocationLogListener.Invocations);

		// thread one ends its transaction and blocks on flush when it attempts to delete the event subscription
		thread1.makeContinue();
		Thread.Sleep(5000);
		assertNull(thread1.Exception);

		assertEquals(0, taskService.createTaskQuery().count());

		// thread 2 flushes successfully and releases the lock
		thread2.waitUntilDone();
		assertNull(thread2.Exception);

		Task afterMessageTask = taskService.createTaskQuery().singleResult();
		assertNotNull(afterMessageTask);
		assertEquals(afterMessageTask.TaskDefinitionKey, "afterMessageUserTask");

		// thread 1 flush fails with optimistic locking
		thread1.join();
		assertTrue(thread1.Exception != null);
		assertTrue(thread1.Exception is OptimisticLockingException);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.eventSubprocess.bpmn")]
	  public virtual void testEventSubprocess()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		// and two threads correlating in parallel
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageCorrelationCommand("incoming", false));
		thread1.reportInterrupts();
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageCorrelationCommand("incoming", false));
		thread2.reportInterrupts();

		// both threads open a transaction and wait before correlating the message
		thread1.waitForSync();
		thread2.waitForSync();

		// both threads correlate
		thread1.makeContinue();
		thread2.makeContinue();

		thread1.waitForSync();
		thread2.waitForSync();

		// the first thread ends its transaction
		thread1.waitUntilDone();
		assertNull(thread1.Exception);

		// the second thread ends its transaction and fails with optimistic locking exception
		thread2.waitUntilDone();
		assertTrue(thread2.Exception != null);
		assertTrue(thread2.Exception is OptimisticLockingException);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentMessageCorrelationAndTreeCompaction()
	  public virtual void testConcurrentMessageCorrelationAndTreeCompaction()
	  {
		runtimeService.startProcessInstanceByKey("process");

		// trigger non-interrupting boundary event and wait before flush
		ThreadControl correlateThread = executeControllableCommand(new ControllableMessageCorrelationCommand("Message", false));
		correlateThread.reportInterrupts();

		// stop correlation right before the flush
		correlateThread.waitForSync();
		correlateThread.makeContinueAndWaitForSync();

		// trigger tree compaction
		IList<Task> tasks = taskService.createTaskQuery().list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		// flush correlation
		correlateThread.waitUntilDone();

		// the correlation should not have succeeded
		Exception exception = correlateThread.Exception;
		assertNotNull(exception);
		assertTrue(exception is OptimisticLockingException);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingMessageCorrelationTest.testConcurrentMessageCorrelationAndTreeCompaction.bpmn20.xml")]
	  public virtual void testConcurrentTreeCompactionAndMessageCorrelation()
	  {
		runtimeService.startProcessInstanceByKey("process");
		IList<Task> tasks = taskService.createTaskQuery().list();

		// trigger tree compaction and wait before flush
		ThreadControl taskCompletionThread = executeControllableCommand(new ControllableCompleteTaskCommand(tasks));
		taskCompletionThread.reportInterrupts();

		// stop task completion right before flush
		taskCompletionThread.waitForSync();

		// perform message correlation to non-interrupting boundary event
		// (i.e. adds another concurrent execution to the scope execution)
		runtimeService.correlateMessage("Message");

		// flush task completion and tree compaction
		taskCompletionThread.waitUntilDone();

		// then it should not have succeeded
		Exception exception = taskCompletionThread.Exception;
		assertNotNull(exception);
		assertTrue(exception is OptimisticLockingException);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentMessageCorrelationTwiceAndTreeCompaction()
	  public virtual void testConcurrentMessageCorrelationTwiceAndTreeCompaction()
	  {
		runtimeService.startProcessInstanceByKey("process");

		// trigger non-interrupting boundary event 1 that ends in a none end event immediately
		runtimeService.correlateMessage("Message2");

		// trigger non-interrupting boundary event 2 and wait before flush
		ThreadControl correlateThread = executeControllableCommand(new ControllableMessageCorrelationCommand("Message1", false));
		correlateThread.reportInterrupts();

		// stop correlation right before the flush
		correlateThread.waitForSync();
		correlateThread.makeContinueAndWaitForSync();

		// trigger tree compaction
		IList<Task> tasks = taskService.createTaskQuery().list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		// flush correlation
		correlateThread.waitUntilDone();

		// the correlation should not have succeeded
		Exception exception = correlateThread.Exception;
		assertNotNull(exception);
		assertTrue(exception is OptimisticLockingException);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentEndExecutionListener()
	  public virtual void testConcurrentEndExecutionListener()
	  {
		InvocationLogListener.reset();

		// given a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		IList<Execution> tasks = runtimeService.createExecutionQuery().messageEventSubscriptionName("Message").list();
		// two tasks waiting for the message
		assertEquals(2, tasks.Count);

		// start first thread and wait in the second execution end listener
		ThreadControl thread1 = executeControllableCommand(new ControllableMessageEventReceivedCommand(tasks[0].Id, "Message", true));
		thread1.reportInterrupts();
		thread1.waitForSync();

		// the counting execution listener was executed on task 1
		assertEquals(1, InvocationLogListener.Invocations);

		// start second thread and complete the task
		ThreadControl thread2 = executeControllableCommand(new ControllableMessageEventReceivedCommand(tasks[1].Id, "Message", false));
		thread2.waitForSync();
		thread2.waitUntilDone();

		// the counting execution listener was executed on task 1 and 2
		assertEquals(2, InvocationLogListener.Invocations);

		// continue with thread 1
		thread1.makeContinueAndWaitForSync();

		// the counting execution listener was not executed again
		assertEquals(2, InvocationLogListener.Invocations);

		// try to complete thread 1
		thread1.waitUntilDone();

		// thread 1 was rolled back with an optimistic locking exception
		Exception exception = thread1.Exception;
		assertNotNull(exception);
		assertTrue(exception is OptimisticLockingException);

		// the execution listener was not executed again
		assertEquals(2, InvocationLogListener.Invocations);
	  }

	  public class InvocationLogListener : JavaDelegate
	  {

		protected internal static AtomicInteger invocations = new AtomicInteger(0);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  invocations.incrementAndGet();
		}

		public static void reset()
		{
		  invocations.set(0);
		}

		public static int Invocations
		{
			get
			{
			  return invocations.get();
			}
		}
	  }

	  public class WaitingListener : ExecutionListener
	  {

		protected internal static ThreadControl monitor;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  if (WaitingListener.monitor != null)
		  {
			ThreadControl localMonitor = WaitingListener.monitor;
			WaitingListener.monitor = null;
			localMonitor.sync();
		  }
		}

		public static ThreadControl Monitor
		{
			set
			{
			  WaitingListener.monitor = value;
			}
		}
	  }

	  protected internal class ControllableMessageCorrelationCommand : ControllableCommand<Void>
	  {

		protected internal string messageName;
		protected internal bool exclusive;
		protected internal string processInstanceId;

		public ControllableMessageCorrelationCommand(string messageName, bool exclusive)
		{
		  this.messageName = messageName;
		  this.exclusive = exclusive;
		}

		public ControllableMessageCorrelationCommand(string messageName, string processInstanceId, bool exclusive) : this(messageName, exclusive)
		{
		  this.processInstanceId = processInstanceId;
		}

		public override Void execute(CommandContext commandContext)
		{

		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  MessageCorrelationBuilderImpl correlationBuilder = new MessageCorrelationBuilderImpl(commandContext, messageName);
		  if (!string.ReferenceEquals(processInstanceId, null))
		  {
			correlationBuilder.processInstanceId(processInstanceId);
		  }

		  if (exclusive)
		  {
			correlationBuilder.correlateExclusively();
		  }
		  else
		  {
			correlationBuilder.correlate();
		  }

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}

	  }

	  protected internal class ControllableMessageEventReceivedCommand : ControllableCommand<Void>
	  {

		protected internal readonly string executionId;
		protected internal readonly string messageName;
		protected internal readonly bool shouldWaitInListener;

		public ControllableMessageEventReceivedCommand(string executionId, string messageName, bool shouldWaitInListener)
		{
		  this.executionId = executionId;
		  this.messageName = messageName;
		  this.shouldWaitInListener = shouldWaitInListener;
		}

		public override Void execute(CommandContext commandContext)
		{

		  if (shouldWaitInListener)
		  {
			WaitingListener.Monitor = monitor;
		  }

		  MessageEventReceivedCmd receivedCmd = new MessageEventReceivedCmd(messageName, executionId, null);

		  receivedCmd.execute(commandContext);

		  monitor.sync();

		  return null;
		}
	  }

	  public class ControllableCompleteTaskCommand : ControllableCommand<Void>
	  {

		protected internal IList<Task> tasks;

		public ControllableCompleteTaskCommand(IList<Task> tasks)
		{
		  this.tasks = tasks;
		}

		public override Void execute(CommandContext commandContext)
		{

		  foreach (Task task in tasks)
		  {
			CompleteTaskCmd completeTaskCmd = new CompleteTaskCmd(task.Id, null);
			completeTaskCmd.execute(commandContext);
		  }

		  monitor.sync();

		  return null;
		}

	  }

	}

}