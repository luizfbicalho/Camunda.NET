using System;

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
	using SetTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.SetTaskVariablesCmd;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DatabaseHelper = org.camunda.bpm.engine.test.util.DatabaseHelper;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ConcurrentVariableUpdateTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal static ControllableThread activeThread;

	  internal class SetTaskVariablesThread : ControllableThread
	  {
		  private readonly ConcurrentVariableUpdateTest outerInstance;


		internal OptimisticLockingException optimisticLockingException;
		internal Exception exception;

		protected internal object variableValue;
		protected internal string taskId;
		protected internal string variableName;

		public SetTaskVariablesThread(ConcurrentVariableUpdateTest outerInstance, string taskId, string variableName, object variableValue)
		{
			this.outerInstance = outerInstance;
		  this.taskId = taskId;
		  this.variableName = variableName;
		  this.variableValue = variableValue;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}

		public virtual void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new SetTaskVariablesCmd(taskId, Collections.singletonMap(variableName, variableValue), false)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.optimisticLockingException = e;
		  }
		  catch (Exception e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {

		string databaseType = DatabaseHelper.getDatabaseType(processEngineConfiguration);

		if (DbSqlSessionFactory.DB2.Equals(databaseType) && "testConcurrentVariableCreate".Equals(Name))
		{
		  // skip test method - if database is DB2
		}
		else
		{
		  // invoke the test method
		  base.runTest();
		}
	  }

	  // Test is skipped when testing on DB2.
	  // Please update the IF condition in #runTest, if the method name is changed.
	  [Deployment(resources:"org/camunda/bpm/engine/test/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml")]
	  public virtual void testConcurrentVariableCreate()
	  {

		runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("varName1", "someValue"));

		string variableName = "varName";
		string taskId = taskService.createTaskQuery().singleResult().Id;

		SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, variableName, "someString");
		thread1.startAndWaitUntilControlIsReturned();

		// this should fail with integrity constraint violation
		SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, variableName, "someString");
		thread2.startAndWaitUntilControlIsReturned();

		thread1.proceedAndWaitTillDone();
		assertNull(thread1.exception);
		assertNull(thread1.optimisticLockingException);

		thread2.proceedAndWaitTillDone();
		assertNull(thread2.exception);
		assertNotNull(thread2.optimisticLockingException);

		// should not fail with FK violation because one of the variables is not deleted.
		taskService.complete(taskId);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml")]
	  public virtual void testConcurrentVariableUpdate()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;
		string variableName = "varName";

		taskService.setVariable(taskId, variableName, "someValue");

		SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, variableName, "someString");
		thread1.startAndWaitUntilControlIsReturned();

		// this fails with an optimistic locking exception
		SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, variableName, "someOtherString");
		thread2.startAndWaitUntilControlIsReturned();

		thread1.proceedAndWaitTillDone();
		thread2.proceedAndWaitTillDone();

		assertNull(thread1.optimisticLockingException);
		assertNotNull(thread2.optimisticLockingException);

		// succeeds
		taskService.complete(taskId);
	  }


	  [Deployment(resources:"org/camunda/bpm/engine/test/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml")]
	  public virtual void testConcurrentVariableUpdateTypeChange()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;
		string variableName = "varName";

		taskService.setVariable(taskId, variableName, "someValue");

		SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, variableName, 100l);
		thread1.startAndWaitUntilControlIsReturned();

		// this fails with an optimistic locking exception
		SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, variableName, "someOtherString");
		thread2.startAndWaitUntilControlIsReturned();

		thread1.proceedAndWaitTillDone();
		thread2.proceedAndWaitTillDone();

		assertNull(thread1.optimisticLockingException);
		assertNotNull(thread2.optimisticLockingException);

		// succeeds
		taskService.complete(taskId);
	  }

	}

}