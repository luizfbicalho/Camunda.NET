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
namespace org.camunda.bpm.application.impl.context
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.spy;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using TestApplicationWithoutEngine = org.camunda.bpm.application.impl.embedded.TestApplicationWithoutEngine;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessApplicationContextTest : PluggableProcessEngineTestCase
	{

	  protected internal TestApplicationWithoutEngine pa;

	  public override void setUp()
	  {
		pa = new TestApplicationWithoutEngine();
		pa.deploy();
	  }

	  public override void tearDown()
	  {
		pa.undeploy();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetPAContextByName() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual void testSetPAContextByName()
	  {

		Assert.assertNull(Context.CurrentProcessApplication);

		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = pa.Name;

		  Assert.assertEquals(CurrentContextApplication.ProcessApplication, pa);
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testExecutionInPAContextByName() throws Exception
	  public virtual void testExecutionInPAContextByName()
	  {
		Assert.assertNull(Context.CurrentProcessApplication);

		ProcessApplicationReference contextPA = ProcessApplicationContext.withProcessApplicationContext(new CallableAnonymousInnerClass(this)
		   , pa.Name);

		Assert.assertEquals(contextPA.ProcessApplication, pa);

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

	  private class CallableAnonymousInnerClass : Callable<ProcessApplicationReference>
	  {
		  private readonly ProcessApplicationContextTest outerInstance;

		  public CallableAnonymousInnerClass(ProcessApplicationContextTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.application.ProcessApplicationReference call() throws Exception
		  public override ProcessApplicationReference call()
		  {
			return outerInstance.CurrentContextApplication;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetPAContextByReference() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual void testSetPAContextByReference()
	  {
		Assert.assertNull(Context.CurrentProcessApplication);

		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = pa.Reference;

		  Assert.assertEquals(CurrentContextApplication.ProcessApplication, pa);
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testExecutionInPAContextByReference() throws Exception
	  public virtual void testExecutionInPAContextByReference()
	  {
		Assert.assertNull(Context.CurrentProcessApplication);

		ProcessApplicationReference contextPA = ProcessApplicationContext.withProcessApplicationContext(new CallableAnonymousInnerClass2(this)
		   , pa.Reference);

		Assert.assertEquals(contextPA.ProcessApplication, pa);

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

	  private class CallableAnonymousInnerClass2 : Callable<ProcessApplicationReference>
	  {
		  private readonly ProcessApplicationContextTest outerInstance;

		  public CallableAnonymousInnerClass2(ProcessApplicationContextTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.application.ProcessApplicationReference call() throws Exception
		  public override ProcessApplicationReference call()
		  {
			return outerInstance.CurrentContextApplication;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSetPAContextByRawPA() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual void testSetPAContextByRawPA()
	  {
		Assert.assertNull(Context.CurrentProcessApplication);

		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = pa;

		  Assert.assertEquals(pa, CurrentContextApplication.ProcessApplication);
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testExecutionInPAContextbyRawPA() throws Exception
	  public virtual void testExecutionInPAContextbyRawPA()
	  {
		Assert.assertNull(Context.CurrentProcessApplication);

		ProcessApplicationReference contextPA = ProcessApplicationContext.withProcessApplicationContext(new CallableAnonymousInnerClass3(this)
		   , pa);

		Assert.assertEquals(contextPA.ProcessApplication, pa);

		Assert.assertNull(Context.CurrentProcessApplication);
	  }

	  private class CallableAnonymousInnerClass3 : Callable<ProcessApplicationReference>
	  {
		  private readonly ProcessApplicationContextTest outerInstance;

		  public CallableAnonymousInnerClass3(ProcessApplicationContextTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.application.ProcessApplicationReference call() throws Exception
		  public override ProcessApplicationReference call()
		  {
			return outerInstance.CurrentContextApplication;
		  }
	  }

	  public virtual void testCannotSetUnregisteredProcessApplicationName()
	  {

		string nonExistingName = pa.Name + pa.Name;

		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = nonExistingName;

		  try
		  {
			CurrentContextApplication;
			fail("should not succeed");

		  }
		  catch (ProcessEngineException e)
		  {
			assertTextPresent("A process application with name '" + nonExistingName + "' is not registered", e.Message);
		  }

		}
		finally
		{
		  ProcessApplicationContext.clear();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testCannotExecuteInUnregisteredPaContext() throws Exception
	  public virtual void testCannotExecuteInUnregisteredPaContext()
	  {
		string nonExistingName = pa.Name + pa.Name;

		try
		{
		  ProcessApplicationContext.withProcessApplicationContext(new CallableAnonymousInnerClass4(this)
		 , nonExistingName);
		  fail("should not succeed");

		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("A process application with name '" + nonExistingName + "' is not registered", e.Message);
		}

	  }

	  private class CallableAnonymousInnerClass4 : Callable<Void>
	  {
		  private readonly ProcessApplicationContextTest outerInstance;

		  public CallableAnonymousInnerClass4(ProcessApplicationContextTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			outerInstance.CurrentContextApplication;
			return null;
		  }

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void testExecuteWithInvocationContext() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testExecuteWithInvocationContext()
	  {
		// given a process application which extends the default one
		// - using a spy for verify the invocations
		TestApplicationWithoutEngine processApplication = spy(pa);
		ProcessApplicationReference processApplicationReference = mock(typeof(ProcessApplicationReference));
		when(processApplicationReference.ProcessApplication).thenReturn(processApplication);

		// when execute with context
		InvocationContext invocationContext = new InvocationContext(mock(typeof(BaseDelegateExecution)));
		Context.executeWithinProcessApplication(mock(typeof(Callable)), processApplicationReference, invocationContext);

		// then the execute method should be invoked with context
		verify(processApplication).execute(any(typeof(Callable)), eq(invocationContext));
		// and forward to call to the default execute method
		verify(processApplication).execute(any(typeof(Callable)));
	  }

	  protected internal virtual ProcessApplicationReference CurrentContextApplication
	  {
		  get
		  {
			ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
			return engineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		  }
	  }

	  private class CommandAnonymousInnerClass : Command<ProcessApplicationReference>
	  {
		  private readonly ProcessApplicationContextTest outerInstance;

		  public CommandAnonymousInnerClass(ProcessApplicationContextTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public ProcessApplicationReference execute(CommandContext commandContext)
		  {
			return Context.CurrentProcessApplication;
		  }
	  }

	}

}