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
	using SignalCmd = org.camunda.bpm.engine.impl.cmd.SignalCmd;
	using SuspendProcessDefinitionCmd = org.camunda.bpm.engine.impl.cmd.SuspendProcessDefinitionCmd;
	using UpdateProcessDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.repository.UpdateProcessDefinitionSuspensionStateBuilderImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class CompetingSuspensionTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal static ControllableThread activeThread;

	  internal class SuspendProcessDefinitionThread : ControllableThread
	  {
		  private readonly CompetingSuspensionTest outerInstance;


		internal string processDefinitionId;
		internal OptimisticLockingException exception;

		public SuspendProcessDefinitionThread(CompetingSuspensionTest outerInstance, string processDefinitionId)
		{
			this.outerInstance = outerInstance;
		  this.processDefinitionId = processDefinitionId;
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
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand<Void>(activeThread, createSuspendCommand()));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}

		protected internal virtual SuspendProcessDefinitionCmd createSuspendCommand()
		{
		  UpdateProcessDefinitionSuspensionStateBuilderImpl builder = (new UpdateProcessDefinitionSuspensionStateBuilderImpl()).byProcessDefinitionId(processDefinitionId).includeProcessInstances(true);

		  return new SuspendProcessDefinitionCmd(builder);
		}
	  }

	  internal class SignalThread : ControllableThread
	  {
		  private readonly CompetingSuspensionTest outerInstance;


		internal string executionId;
		internal OptimisticLockingException exception;

		public SignalThread(CompetingSuspensionTest outerInstance, string executionId)
		{
			this.outerInstance = outerInstance;
		  this.executionId = executionId;
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
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new SignalCmd(executionId, null, null, null)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

	  /// <summary>
	  /// Ensures that suspending a process definition and its process instances will also increase the revision of the executions
	  /// such that concurrent updates fail with an OptimisticLockingException.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompetingSuspension()
	  public virtual void testCompetingSuspension()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("CompetingSuspensionProcess").singleResult();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).activityId("wait1").singleResult();

		SuspendProcessDefinitionThread suspensionThread = new SuspendProcessDefinitionThread(this, processDefinition.Id);
		suspensionThread.startAndWaitUntilControlIsReturned();

		SignalThread signalExecutionThread = new SignalThread(this, execution.Id);
		signalExecutionThread.startAndWaitUntilControlIsReturned();

		suspensionThread.proceedAndWaitTillDone();
		assertNull(suspensionThread.exception);

		signalExecutionThread.proceedAndWaitTillDone();
		assertNotNull(signalExecutionThread.exception);
	  }
	}

}