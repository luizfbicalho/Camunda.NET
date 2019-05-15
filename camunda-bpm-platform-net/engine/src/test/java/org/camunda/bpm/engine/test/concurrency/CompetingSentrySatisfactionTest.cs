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
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CompleteCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.CompleteCaseExecutionCmd;
	using ManualStartCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.ManualStartCaseExecutionCmd;
	using StateTransitionCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.StateTransitionCaseExecutionCmd;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CompetingSentrySatisfactionTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;

	  public abstract class SingleThread : ControllableThread
	  {
		  private readonly CompetingSentrySatisfactionTest outerInstance;


		internal string caseExecutionId;
		internal OptimisticLockingException exception;
		protected internal StateTransitionCaseExecutionCmd cmd;

		public SingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId, StateTransitionCaseExecutionCmd cmd)
		{
			this.outerInstance = outerInstance;
		  this.caseExecutionId = caseExecutionId;
		  this.cmd = cmd;
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
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, cmd));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

	  public class CompletionSingleThread : SingleThread
	  {
		  private readonly CompetingSentrySatisfactionTest outerInstance;


		public CompletionSingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new CompleteCaseExecutionCmd(caseExecutionId, null, null, null, null))
		{
			this.outerInstance = outerInstance;
		}

	  }

	  public class ManualStartSingleThread : SingleThread
	  {
		  private readonly CompetingSentrySatisfactionTest outerInstance;


		public ManualStartSingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new ManualStartCaseExecutionCmd(caseExecutionId, null, null, null, null))
		{
			this.outerInstance = outerInstance;
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingSentrySatisfactionTest.testEntryCriteriaWithAndSentry.cmmn"})]
	  public virtual void testEntryCriteriaWithAndSentry()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		string message = threadTwo.exception.Message;
		assertTextPresent("CaseSentryPartEntity", message);
		assertTextPresent("was updated by another transaction concurrently", message);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingSentrySatisfactionTest.testExitCriteriaWithAndSentry.cmmn"})]
	  public virtual void testExitCriteriaWithAndSentry()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		string message = threadTwo.exception.Message;
		assertTextPresent("CaseSentryPartEntity", message);
		assertTextPresent("was updated by another transaction concurrently", message);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingSentrySatisfactionTest.testEntryCriteriaWithOrSentry.cmmn"})]
	  public virtual void testEntryCriteriaWithOrSentry()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		string message = threadTwo.exception.Message;
		assertTextPresent("CaseExecutionEntity", message);
		assertTextPresent("was updated by another transaction concurrently", message);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingSentrySatisfactionTest.testExitCriteriaWithOrSentry.cmmn", "org/camunda/bpm/engine/test/concurrency/CompetingSentrySatisfactionTest.oneTaskProcess.bpmn20.xml"})]
	  public virtual void testExitCriteriaWithOrSentry()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		CaseExecution thirdTask = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("ProcessTask_3").singleResult();
		caseService.manuallyStartCaseExecution(thirdTask.Id);

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		string message = threadTwo.exception.Message;
		assertTextPresent("CaseExecutionEntity", message);
		assertTextPresent("was updated by another transaction concurrently", message);
	  }

	}

}