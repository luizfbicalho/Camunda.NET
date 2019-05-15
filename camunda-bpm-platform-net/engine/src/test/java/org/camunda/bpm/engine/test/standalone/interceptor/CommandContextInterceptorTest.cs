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
namespace org.camunda.bpm.engine.test.standalone.interceptor
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CommandContextInterceptorTest : PluggableProcessEngineTestCase
	{

	  public virtual void testCommandContextGetCurrentAfterException()
	  {
		try
		{
		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		  fail("expected exception");
		}
		catch (System.InvalidOperationException)
		{
		  // OK
		}

		assertNull(Context.CommandContext);
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly CommandContextInterceptorTest outerInstance;

		  public CommandAnonymousInnerClass(CommandContextInterceptorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			throw new System.InvalidOperationException("here i come!");
		  }
	  }

	  public virtual void testCommandContextNestedFailingCommands()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand1 = new ExceptionThrowingCmd(new IdentifiableRuntimeException(1));
		ExceptionThrowingCmd innerCommand1 = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand2 = new ExceptionThrowingCmd(new IdentifiableRuntimeException(2));
		ExceptionThrowingCmd innerCommand2 = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 2));

		try
		{
		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, innerCommand1, innerCommand2));

		  fail("Exception expected");
		}
		catch (IdentifiableRuntimeException e)
		{
		  assertEquals(1, e.id);
		}

		assertTrue(innerCommand1.executed);
		assertFalse(innerCommand2.executed);
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly CommandContextInterceptorTest outerInstance;

		  private org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand1;
		  private org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand2;

		  public CommandAnonymousInnerClass2(CommandContextInterceptorTest outerInstance, org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand1, org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand2)
		  {
			  this.outerInstance = outerInstance;
			  this.innerCommand1 = innerCommand1;
			  this.innerCommand2 = innerCommand2;
		  }

		  public object execute(CommandContext commandContext)
		  {
			CommandExecutor commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;

			commandExecutor.execute(innerCommand1);
			commandExecutor.execute(innerCommand2);

			return null;
		  }
	  }

	  public virtual void testCommandContextNestedTryCatch()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand = new ExceptionThrowingCmd(new IdentifiableRuntimeException(1));
		ExceptionThrowingCmd innerCommand = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 1));

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this, innerCommand));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private readonly CommandContextInterceptorTest outerInstance;

		  private org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand;

		  public CommandAnonymousInnerClass3(CommandContextInterceptorTest outerInstance, org.camunda.bpm.engine.test.standalone.interceptor.CommandContextInterceptorTest.ExceptionThrowingCmd innerCommand)
		  {
			  this.outerInstance = outerInstance;
			  this.innerCommand = innerCommand;
		  }

		  public object execute(CommandContext commandContext)
		  {
			CommandExecutor commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;

			try
			{
			  commandExecutor.execute(innerCommand);
			  fail("exception expected to pop up during execution of inner command");
			}
			catch (IdentifiableRuntimeException)
			{
			  // happy path
			  assertNull("the exception should not have been propagated to this command's context", Context.CommandInvocationContext.Throwable);
			}

			return null;
		  }
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testCommandContextNestedFailingCommandsNotExceptions()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess("processThrowingThrowable").startEvent().serviceTask().camundaClass(ThrowErrorJavaDelegate.class).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("processThrowingThrowable").startEvent().serviceTask().camundaClass(typeof(ThrowErrorJavaDelegate)).endEvent().done();

		deployment(modelInstance);

		bool errorThrown = false;
		try
		{
		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass4(this));
		  fail("Exception expected");
		}
		catch (StackOverflowError)
		{
		  //OK
		  errorThrown = true;
		}

		assertTrue(ThrowErrorJavaDelegate.executed);
		assertTrue(errorThrown);

		// Check data base consistency
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().count());
	  }

	  private class CommandAnonymousInnerClass4 : Command<object>
	  {
		  private readonly CommandContextInterceptorTest outerInstance;

		  public CommandAnonymousInnerClass4(CommandContextInterceptorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {

			outerInstance.runtimeService.startProcessInstanceByKey("processThrowingThrowable");
			return null;
		  }
	  }

	  protected internal class ExceptionThrowingCmd : Command<Void>
	  {
		  private readonly CommandContextInterceptorTest outerInstance;


		protected internal bool executed;

		protected internal Exception exceptionToThrow;

		public ExceptionThrowingCmd(CommandContextInterceptorTest outerInstance, Exception e)
		{
			this.outerInstance = outerInstance;
		  executed = false;
		  exceptionToThrow = e;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  executed = true;
		  throw exceptionToThrow;
		}

	  }

	  protected internal class IdentifiableRuntimeException : Exception
	  {
		  private readonly CommandContextInterceptorTest outerInstance;


		internal const long serialVersionUID = 1L;
		protected internal int id;
		public IdentifiableRuntimeException(CommandContextInterceptorTest outerInstance, int id)
		{
			this.outerInstance = outerInstance;
		  this.id = id;
		}
	  }

	}

}