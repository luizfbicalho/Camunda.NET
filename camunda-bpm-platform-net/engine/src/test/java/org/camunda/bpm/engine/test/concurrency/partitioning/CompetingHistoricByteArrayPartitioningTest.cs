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
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
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

	public class CompetingHistoricByteArrayPartitioningTest : AbstractPartitioningTest
	{

	  protected internal readonly string VARIABLE_NAME = "aVariableName";
	  protected internal readonly string VARIABLE_VALUE = "aVariableValue";
	  protected internal readonly string ANOTHER_VARIABLE_VALUE = "anotherVariableValue";

	  public virtual void testConcurrentFetchAndDelete()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK, org.camunda.bpm.engine.variable.Variables.createVariables().putValue(VARIABLE_NAME, org.camunda.bpm.engine.variable.Variables.byteArrayValue(VARIABLE_VALUE.getBytes()))).getId();
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK, Variables.createVariables().putValue(VARIABLE_NAME, Variables.byteArrayValue(VARIABLE_VALUE.GetBytes()))).Id;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] historicByteArrayId = new String[1];
		string[] historicByteArrayId = new string[1];
		commandExecutor.execute(new CommandAnonymousInnerClass(this, processInstanceId, historicByteArrayId));

		ThreadControl asyncThread = executeControllableCommand(new AsyncThread(this, processInstanceId, historicByteArrayId[0]));

		asyncThread.waitForSync();

		commandExecutor.execute(new CommandAnonymousInnerClass2(this, historicByteArrayId));

		commandExecutor.execute(new CommandAnonymousInnerClass3(this, historicByteArrayId));

		// when
		asyncThread.makeContinue();
		asyncThread.waitUntilDone();

		// then
		assertThat(runtimeService.createVariableInstanceQuery().singleResult().Name, @is(VARIABLE_NAME));
		assertThat(StringHelper.NewString((sbyte[]) runtimeService.createVariableInstanceQuery().singleResult().Value), @is(ANOTHER_VARIABLE_VALUE));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CompetingHistoricByteArrayPartitioningTest outerInstance;

		  private string processInstanceId;
		  private string[] historicByteArrayId;

		  public CommandAnonymousInnerClass(CompetingHistoricByteArrayPartitioningTest outerInstance, string processInstanceId, string[] historicByteArrayId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
			  this.historicByteArrayId = historicByteArrayId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(processInstanceId);

			VariableInstanceEntity varInstance = (VariableInstanceEntity) execution.getVariableInstance(outerInstance.VARIABLE_NAME);
			HistoricVariableInstanceEntity historicVariableInstance = commandContext.HistoricVariableInstanceManager.findHistoricVariableInstanceByVariableInstanceId(varInstance.Id);

			historicByteArrayId[0] = historicVariableInstance.ByteArrayValueId;

			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly CompetingHistoricByteArrayPartitioningTest outerInstance;

		  private string[] historicByteArrayId;

		  public CommandAnonymousInnerClass2(CompetingHistoricByteArrayPartitioningTest outerInstance, string[] historicByteArrayId)
		  {
			  this.outerInstance = outerInstance;
			  this.historicByteArrayId = historicByteArrayId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.ByteArrayManager.deleteByteArrayById(historicByteArrayId[0]);

			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly CompetingHistoricByteArrayPartitioningTest outerInstance;

		  private string[] historicByteArrayId;

		  public CommandAnonymousInnerClass3(CompetingHistoricByteArrayPartitioningTest outerInstance, string[] historicByteArrayId)
		  {
			  this.outerInstance = outerInstance;
			  this.historicByteArrayId = historicByteArrayId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			// assume
			assertThat(commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), historicByteArrayId[0]), nullValue());

			return null;
		  }
	  }

	  public class AsyncThread : ControllableCommand<Void>
	  {
		  private readonly CompetingHistoricByteArrayPartitioningTest outerInstance;


		internal string processInstanceId;
		internal string historicByteArrayId;

		internal AsyncThread(CompetingHistoricByteArrayPartitioningTest outerInstance, string processInstanceId, string historicByteArrayId)
		{
			this.outerInstance = outerInstance;
		  this.processInstanceId = processInstanceId;
		  this.historicByteArrayId = historicByteArrayId;
		}

		public override Void execute(CommandContext commandContext)
		{
		  commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), historicByteArrayId); // cache

		  monitor.sync();

		  outerInstance.runtimeService.setVariable(processInstanceId, outerInstance.VARIABLE_NAME, Variables.byteArrayValue(outerInstance.ANOTHER_VARIABLE_VALUE.GetBytes()));

		  return null;
		}

	  }

	}

}