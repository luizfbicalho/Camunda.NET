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
	using DisableCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.DisableCaseExecutionCmd;
	using StateTransitionCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.StateTransitionCaseExecutionCmd;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CompetingParentCompletionTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;

	  public abstract class SingleThread : ControllableThread
	  {
		  private readonly CompetingParentCompletionTest outerInstance;


		internal string caseExecutionId;
		internal OptimisticLockingException exception;
		protected internal StateTransitionCaseExecutionCmd cmd;

		public SingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId, StateTransitionCaseExecutionCmd cmd)
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
		  private readonly CompetingParentCompletionTest outerInstance;


		public CompletionSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new CompleteCaseExecutionCmd(caseExecutionId, null, null, null, null))
		{
			this.outerInstance = outerInstance;
		}

	  }

	  public class DisableSingleThread : SingleThread
	  {
		  private readonly CompetingParentCompletionTest outerInstance;


		public DisableSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new DisableCaseExecutionCmd(caseExecutionId, null, null, null, null))
		{
			this.outerInstance = outerInstance;
		}

	  }

	  public class TerminateSingleThread : SingleThread
	  {
		  private readonly CompetingParentCompletionTest outerInstance;


		public TerminateSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstancecaseExecutionId, new StateTransitionCaseExecutionCmd(caseExecutionId, null, null, null, null)
		{
			this.outerInstance = outerInstance;
			{

			private static final long serialVersionUID = 1L;

			protected void performStateTransition(CommandContext commandContext, CaseExecutionEntity caseExecution)
			{
			  caseExecution.terminate();
			}

			}
		 );
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingParentCompletionTest.testComplete.cmmn"})]
	  public virtual void testComplete()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new CompletionSingleThread(this, firstHumanTaskId);
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
		assertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingParentCompletionTest.testDisable.cmmn"})]
	  public virtual void testDisable()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new DisableSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new DisableSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);
		assertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingParentCompletionTest.testTerminate.cmmn"})]
	  public virtual void testTerminate()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string firstHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult().Id;

		string secondHumanTaskId = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_2").singleResult().Id;

		LOG.debug("test thread starts thread one");
		SingleThread threadOne = new TerminateSingleThread(this, firstHumanTaskId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SingleThread threadTwo = new TerminateSingleThread(this, secondHumanTaskId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);
		assertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

	  }

	}

}