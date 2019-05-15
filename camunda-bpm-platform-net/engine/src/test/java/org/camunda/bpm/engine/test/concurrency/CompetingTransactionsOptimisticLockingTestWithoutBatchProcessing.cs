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
namespace org.camunda.bpm.engine.test.concurrency
{
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class CompetingTransactionsOptimisticLockingTestWithoutBatchProcessing : ResourceProcessEngineTestCase
	{

	  private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;
	  internal static ControllableThread activeThread;

	  public CompetingTransactionsOptimisticLockingTestWithoutBatchProcessing() : base("org/camunda/bpm/engine/test/concurrency/custombatchprocessing.camunda.cfg.xml")
	  {
	  }


	  public class TransactionThread : ControllableThread
	  {
		  private readonly CompetingTransactionsOptimisticLockingTestWithoutBatchProcessing outerInstance;

		internal string taskId;
		internal ProcessEngineException exception;

		public TransactionThread(CompetingTransactionsOptimisticLockingTestWithoutBatchProcessing outerInstance, string taskId)
		{
			this.outerInstance = outerInstance;
		  this.taskId = taskId;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}

		public override void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new CompleteTaskCmd(taskId, null)));

		  }
		  catch (ProcessEngineException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends.");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if (DbSqlSessionFactory.POSTGRES.Equals(databaseType))
		{
		  // skip test method - if database is PostgreSQL
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/concurrency/CompetingTransactionsOptimisticLockingTest.testCompetingTransactionsOptimisticLocking.bpmn20.xml") public void testCompetingTransactionsOptimisticLocking() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/concurrency/CompetingTransactionsOptimisticLockingTest.testCompetingTransactionsOptimisticLocking.bpmn20.xml")]
	  public virtual void testCompetingTransactionsOptimisticLocking()
	  {
		// given
		runtimeService.startProcessInstanceByKey("competingTransactionsProcess");
		IList<Task> tasks = taskService.createTaskQuery().list();

		assertEquals(2, tasks.Count);

		Task firstTask = "task1-1".Equals(tasks[0].TaskDefinitionKey) ? tasks[0] : tasks[1];
		Task secondTask = "task2-1".Equals(tasks[0].TaskDefinitionKey) ? tasks[0] : tasks[1];

		TransactionThread thread1 = new TransactionThread(this, firstTask.Id);
		thread1.startAndWaitUntilControlIsReturned();
		TransactionThread thread2 = new TransactionThread(this, secondTask.Id);
		thread2.startAndWaitUntilControlIsReturned();

		thread2.proceedAndWaitTillDone();
		assertNull(thread2.exception);

		thread1.proceedAndWaitTillDone();
		assertNotNull(thread1.exception);
		assertEquals(typeof(OptimisticLockingException), thread1.exception.GetType());
	  }
	}

}