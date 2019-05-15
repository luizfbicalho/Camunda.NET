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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryCleanupCmd = org.camunda.bpm.engine.impl.cmd.HistoryCleanupCmd;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;

	/// <summary>
	/// <para>Tests the call to history cleanup simultaneously.</para>
	/// 
	/// <para><b>Note:</b> the test is not executed on H2 because it doesn't support the
	/// exclusive lock on table.</para>
	/// 
	/// @author Svetlana Dorokhova
	/// </summary>
	public class ConcurrentHistoryCleanupTest : ConcurrencyTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		((ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		base.tearDown();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ConcurrentHistoryCleanupTest outerInstance;

		  public CommandAnonymousInnerClass(ConcurrentHistoryCleanupTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.processEngine.ManagementService.createJobQuery().list();
			if (jobs.Count > 0)
			{
			  string jobId = jobs[0].Id;
			  commandContext.JobManager.deleteJob((JobEntity) jobs[0]);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> transactionIsolationLevel = org.camunda.bpm.engine.test.util.DatabaseHelper.getTransactionIsolationLevel(processEngineConfiguration);
		int? transactionIsolationLevel = DatabaseHelper.getTransactionIsolationLevel(processEngineConfiguration);
		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if (DbSqlSessionFactory.H2.Equals(databaseType) || DbSqlSessionFactory.MARIADB.Equals(databaseType) || (transactionIsolationLevel != null && !transactionIsolationLevel.Equals(Connection.TRANSACTION_READ_COMMITTED)))
		{
		  // skip test method - if database is H2
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRunTwoHistoryCleanups() throws InterruptedException
	  public virtual void testRunTwoHistoryCleanups()
	  {
		ThreadControl thread1 = executeControllableCommand(new ControllableHistoryCleanupCommand());
		thread1.waitForSync();

		ThreadControl thread2 = executeControllableCommand(new ControllableHistoryCleanupCommand());
		thread2.waitForSync();

		thread1.makeContinue();
		thread1.waitForSync();

		thread2.makeContinue();

		Thread.Sleep(2000);

		thread1.waitUntilDone();

		thread2.waitForSync();
		thread2.waitUntilDone();

		//only one history cleanup job exists -> no exception
		IList<Job> historyCleanupJobs = processEngine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);

		assertNull(thread1.Exception);
		assertNull(thread2.Exception);

	  }

	  protected internal class ControllableHistoryCleanupCommand : ControllableCommand<Void>
	  {

		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  (new HistoryCleanupCmd(true)).execute(commandContext);

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}

	  }

	}

}