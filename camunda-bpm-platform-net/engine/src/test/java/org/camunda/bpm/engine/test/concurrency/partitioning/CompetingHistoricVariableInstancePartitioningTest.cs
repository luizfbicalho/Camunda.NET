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
namespace org.camunda.bpm.engine.test.concurrency.partitioning
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using Variables = org.camunda.bpm.engine.variable.Variables;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>

	public class CompetingHistoricVariableInstancePartitioningTest : AbstractPartitioningTest
	{

	  protected internal readonly string VARIABLE_NAME = "aVariableName";
	  protected internal readonly string VARIABLE_VALUE = "aVariableValue";
	  protected internal readonly string ANOTHER_VARIABLE_VALUE = "anotherVariableValue";

	  public virtual void testConcurrentFetchAndDelete()
	  {
		// given
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK, Variables.createVariables().putValue(VARIABLE_NAME, VARIABLE_VALUE)).Id;

		ThreadControl asyncThread = executeControllableCommand(new AsyncThread(this, processInstanceId));

		asyncThread.waitForSync();

		commandExecutor.execute(new CommandAnonymousInnerClass(this));

		// assume
		assertThat(historyService.createHistoricVariableInstanceQuery().singleResult(), nullValue());

		// when
		asyncThread.makeContinue();
		asyncThread.waitUntilDone();

		// then
		assertThat(runtimeService.createVariableInstanceQuery().singleResult().Name, @is(VARIABLE_NAME));
		assertThat((string) runtimeService.createVariableInstanceQuery().singleResult().Value, @is(ANOTHER_VARIABLE_VALUE));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CompetingHistoricVariableInstancePartitioningTest outerInstance;

		  public CommandAnonymousInnerClass(CompetingHistoricVariableInstancePartitioningTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			HistoricVariableInstanceEntity historicVariableInstanceEntity = (HistoricVariableInstanceEntity) outerInstance.historyService.createHistoricVariableInstanceQuery().singleResult();

			commandContext.DbEntityManager.delete(historicVariableInstanceEntity);

			return null;
		  }
	  }

	  public class AsyncThread : ControllableCommand<Void>
	  {
		  private readonly CompetingHistoricVariableInstancePartitioningTest outerInstance;


		internal string processInstanceId;

		internal AsyncThread(CompetingHistoricVariableInstancePartitioningTest outerInstance, string processInstanceId)
		{
			this.outerInstance = outerInstance;
		  this.processInstanceId = processInstanceId;
		}

		public override Void execute(CommandContext commandContext)
		{
		 outerInstance.historyService.createHistoricVariableInstanceQuery().singleResult().Id; // cache

		  monitor.sync();

		  commandContext.ProcessEngineConfiguration.RuntimeService.setVariable(processInstanceId, outerInstance.VARIABLE_NAME, outerInstance.ANOTHER_VARIABLE_VALUE);

		  return null;
		}

	  }

	}

}