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
namespace org.camunda.bpm.engine.test.concurrency
{

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbEntityManagerFactory = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManagerFactory;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class DbDeadlockTest : ConcurrencyTestCase
	{

	  private ThreadControl thread1;
	  private ThreadControl thread2;

	  /// <summary>
	  /// In this test, we run two transactions concurrently.
	  /// The transactions have the following behavior:
	  /// 
	  /// (1) INSERT row into a table
	  /// (2) SELECT ALL rows from that table
	  /// 
	  /// We execute it with two threads in the following interleaving:
	  /// 
	  ///      Thread 1             Thread 2
	  ///      ========             ========
	  /// ------INSERT---------------------------   |
	  /// ---------------------------INSERT------   |
	  /// ---------------------------SELECT------   v time
	  /// ------SELECT---------------------------
	  /// 
	  /// Deadlocks may occur if readers are not properly isolated from writers.
	  /// 
	  /// </summary>
	  public virtual void testTransactionIsolation()
	  {

		thread1 = executeControllableCommand(new TestCommand("p1"));

		// wait for Thread 1 to perform INSERT
		thread1.waitForSync();

		thread2 = executeControllableCommand(new TestCommand("p2"));

		// wait for Thread 2 to perform INSERT
		thread2.waitForSync();

		// wait for Thread 2 to perform SELECT
		thread2.makeContinue();

		// wait for Thread 1  to perform same SELECT => deadlock
		thread1.makeContinue();

		thread2.waitForSync();
		thread1.waitForSync();

	  }

	  internal class TestCommand : ControllableCommand<Void>
	  {

		protected internal string id;

		public TestCommand(string id)
		{
		  this.id = id;
		}

		public override Void execute(CommandContext commandContext)
		{
		  DbEntityManagerFactory dbEntityManagerFactory = new DbEntityManagerFactory(Context.ProcessEngineConfiguration.IdGenerator);
		  DbEntityManager newEntityManager = dbEntityManagerFactory.openSession();

		  HistoricProcessInstanceEventEntity hpi = new HistoricProcessInstanceEventEntity();
		  hpi.Id = id;
		  hpi.ProcessInstanceId = id;
		  hpi.ProcessDefinitionId = "someProcDefId";
		  hpi.StartTime = DateTime.Now;
		  hpi.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE;

		  newEntityManager.insert(hpi);
		  newEntityManager.flush();

		  monitor.sync();

		  DbEntityManager cmdEntityManager = commandContext.DbEntityManager;
		  cmdEntityManager.createHistoricProcessInstanceQuery().list();

		  monitor.sync();

		  return null;
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {

		// end interaction with Thread 2
		thread2.waitUntilDone();

		// end interaction with Thread 1
		thread1.waitUntilDone();

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly DbDeadlockTest outerInstance;

		  public CommandAnonymousInnerClass(DbDeadlockTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			IList<HistoricProcessInstance> list = commandContext.DbEntityManager.createHistoricProcessInstanceQuery().list();
			foreach (HistoricProcessInstance historicProcessInstance in list)
			{
			  commandContext.DbEntityManager.delete(typeof(HistoricProcessInstanceEventEntity), "deleteHistoricProcessInstance", historicProcessInstance.Id);
			}
			return null;
		  }

	  }

	}

}