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

	using HistoryLevelSetupCommand = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;

	/// <summary>
	/// <para>Tests cluster scenario with two nodes trying to write the history level property in parallel.</para>
	/// 
	/// <para><b>Note:</b> the test is not executed on H2 because it doesn't support the
	/// exclusive lock on table.</para>
	/// 
	/// </summary>
	public class ConcurrentHistoryLevelTest : ConcurrencyTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		TestHelper.deleteHistoryLevel(processEngineConfiguration);
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
//ORIGINAL LINE: public void test() throws InterruptedException
	  public virtual void test()
	  {
		ThreadControl thread1 = executeControllableCommand(new ControllableUpdateHistoryLevelCommand());
		thread1.waitForSync();

		ThreadControl thread2 = executeControllableCommand(new ControllableUpdateHistoryLevelCommand());
		thread2.waitForSync();

		thread1.makeContinue();
		thread1.waitForSync();

		thread2.makeContinue();

		Thread.Sleep(2000);

		thread1.waitUntilDone();

		thread2.waitForSync();
		thread2.waitUntilDone();

		assertNull(thread1.exception);
		assertNull(thread2.exception);
		HistoryLevel historyLevel = processEngineConfiguration.HistoryLevel;
		assertEquals("full", historyLevel.Name);
	  }

	  protected internal class ControllableUpdateHistoryLevelCommand : ControllableCommand<Void>
	  {

		public override Void execute(CommandContext commandContext)
		{

		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  (new HistoryLevelSetupCommand()).execute(commandContext);

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}

	  }
	}

}