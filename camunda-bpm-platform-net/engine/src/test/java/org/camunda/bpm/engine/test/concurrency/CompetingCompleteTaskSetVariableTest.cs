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
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using SetTaskVariablesCmd = org.camunda.bpm.engine.impl.cmd.SetTaskVariablesCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Ignore = org.junit.Ignore;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// 
	/// </summary>
	public class CompetingCompleteTaskSetVariableTest : ConcurrencyTestCase
	{

	  protected internal class ControllableCompleteTaskCommand : ConcurrencyTestCase.ControllableCommand<Void>
	  {

		protected internal string taskId;

		protected internal Exception exception;

		public ControllableCompleteTaskCommand(string taskId)
		{
		  this.taskId = taskId;
		}

		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  (new CompleteTaskCmd(taskId, null)).execute(commandContext);

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}

	  }

	  public class ControllableSetTaskVariablesCommand : ConcurrencyTestCase.ControllableCommand<Void>
	  {
		  private readonly CompetingCompleteTaskSetVariableTest outerInstance;


		protected internal string taskId;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, ? extends Object> variables;
		protected internal IDictionary<string, ? extends object> variables;

		protected internal Exception exception;

		public ControllableSetTaskVariablesCommand<T1>(CompetingCompleteTaskSetVariableTest outerInstance, string taskId, IDictionary<T1> variables) where T1 : object
		{
			this.outerInstance = outerInstance;
		  this.taskId = taskId;
		  this.variables = variables;
		}

		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync(); // thread will block here until makeContinue() is called form main thread

		  (new SetTaskVariablesCmd(taskId, variables, true)).execute(commandContext);

		  monitor.sync(); // thread will block here until waitUntilDone() is called form main thread

		  return null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompleteTaskSetLocalVariable()
	  public virtual void testCompleteTaskSetLocalVariable()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String taskId = taskService.createTaskQuery().singleResult().getId();
		string taskId = taskService.createTaskQuery().singleResult().Id;

		ConcurrencyTestCase.ThreadControl thread1 = executeControllableCommand(new ControllableSetTaskVariablesCommand(this, taskId, Variables.createVariables().putValue("var", "value")));
		thread1.reportInterrupts();
		thread1.waitForSync();

		ConcurrencyTestCase.ThreadControl thread2 = executeControllableCommand(new ControllableCompleteTaskCommand(taskId));
		thread2.reportInterrupts();
		thread2.waitForSync();

		//set task variable, but not commit transaction
		thread1.makeContinue();
		thread1.waitForSync();

		//complete task -> task is removed, execution is removed
		thread2.makeContinue();
		thread2.waitForSync();

		//commit transaction with task variable
		thread1.makeContinue();
		thread1.waitUntilDone();

		//try to commit task completion
		thread2.makeContinue();
		thread2.waitUntilDone();

		//variable was persisted
		assertEquals(1, runtimeService.createVariableInstanceQuery().taskIdIn(taskId).count());

		//task was not removed
		assertNotNull(thread2.exception);
		assertEquals(1, taskService.createTaskQuery().taskId(taskId).count());

	  }
	}

}