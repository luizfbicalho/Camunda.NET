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

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbEntityManagerFactory = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManagerFactory;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;

	/// <summary>
	///  @author Philipp Ossler
	/// </summary>
	public class JdbcStatementTimeoutTest : ConcurrencyTestCase
	{

	  private const int STATEMENT_TIMEOUT_IN_SECONDS = 1;
	  // some databases (like mysql and oracle) need more time to cancel the statement
	  private const int TEST_TIMEOUT_IN_MILLIS = 10000;
	  private const string JOB_ENTITY_ID = "42";

	  private ThreadControl thread1;
	  private ThreadControl thread2;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if ((DbSqlSessionFactory.DB2.Equals(databaseType) || DbSqlSessionFactory.MARIADB.Equals(databaseType)) && processEngine.ProcessEngineConfiguration.JdbcBatchProcessing)
		{
		  // skip test method - if database is DB2 and MariaDB and Batch mode on
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

	  protected internal override void initializeProcessEngine()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml").setJdbcStatementTimeout(STATEMENT_TIMEOUT_IN_SECONDS).buildProcessEngine();
	  }

	  protected internal override void closeDownProcessEngine()
	  {
		processEngine.close();
		processEngine = null;
	  }

	  public virtual void testTimeoutOnUpdate()
	  {
		createJobEntity();

		thread1 = executeControllableCommand(new UpdateJobCommand("p1"));
		// wait for thread 1 to perform UPDATE
		thread1.waitForSync();

		thread2 = executeControllableCommand(new UpdateJobCommand("p2"));
		// wait for thread 2 to perform UPDATE
		thread2.waitForSync();

		// perform FLUSH for thread 1 (but no commit of transaction)
		thread1.makeContinue();
		// wait for thread 1 to perform FLUSH
		thread1.waitForSync();

		// perform FLUSH for thread 2
		thread2.makeContinue();
		// wait for thread 2 to cancel FLUSH because of timeout
		thread2.reportInterrupts();
		thread2.waitForSync(TEST_TIMEOUT_IN_MILLIS);

		assertNotNull("expected timeout exception", thread2.Exception);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		if (thread1 != null)
		{
		  thread1.waitUntilDone();
		  deleteJobEntities();
		}
	  }

	  private void createJobEntity()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<JobEntity>
	  {
		  private readonly JdbcStatementTimeoutTest outerInstance;

		  public CommandAnonymousInnerClass(JdbcStatementTimeoutTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public JobEntity execute(CommandContext commandContext)
		  {
			MessageEntity jobEntity = new MessageEntity();
			jobEntity.Id = JOB_ENTITY_ID;
			jobEntity.insert();

			return jobEntity;
		  }
	  }

	  private void deleteJobEntities()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly JdbcStatementTimeoutTest outerInstance;

		  public CommandAnonymousInnerClass2(JdbcStatementTimeoutTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			IList<Job> jobs = commandContext.DbEntityManager.createJobQuery().list();
			foreach (Job job in jobs)
			{
			  commandContext.JobManager.deleteJob((JobEntity) job, false);
			}

			foreach (HistoricJobLog jobLog in commandContext.DbEntityManager.createHistoricJobLogQuery().list())
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogById(jobLog.Id);
			}

			return null;
		  }

	  }

	  internal class UpdateJobCommand : ControllableCommand<Void>
	  {

		protected internal string lockOwner;

		public UpdateJobCommand(string lockOwner)
		{
		  this.lockOwner = lockOwner;
		}

		public override Void execute(CommandContext commandContext)
		{
		  DbEntityManagerFactory dbEntityManagerFactory = new DbEntityManagerFactory(Context.ProcessEngineConfiguration.IdGenerator);
		  DbEntityManager entityManager = dbEntityManagerFactory.openSession();

		  JobEntity job = entityManager.selectById(typeof(JobEntity), JOB_ENTITY_ID);
		  job.LockOwner = lockOwner;
		  entityManager.forceUpdate(job);

		  monitor.sync();

		  // flush the changed entity and create a lock for the table
		  entityManager.flush();

		  monitor.sync();

		  // commit transaction and remove the lock
		  commandContext.TransactionContext.commit();

		  return null;
		}

	  }
	}

}