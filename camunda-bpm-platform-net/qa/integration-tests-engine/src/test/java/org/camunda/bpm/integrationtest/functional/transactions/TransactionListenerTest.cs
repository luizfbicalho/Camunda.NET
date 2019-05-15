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
namespace org.camunda.bpm.integrationtest.functional.transactions
{
	using Assert = org.junit.Assert;

	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TransactionListenerTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TransactionListenerTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSynchronizationOnRollback()
	  public virtual void testSynchronizationOnRollback()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestTransactionListener rolledBackListener = new TestTransactionListener();
		TestTransactionListener rolledBackListener = new TestTransactionListener();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestTransactionListener committedListener = new TestTransactionListener();
		TestTransactionListener committedListener = new TestTransactionListener();
		Assert.assertFalse(rolledBackListener.Invoked);
		Assert.assertFalse(committedListener.Invoked);

		try
		{

		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, rolledBackListener, committedListener));

		}
		catch (Exception e)
		{
		  Assert.assertTrue(e.Message.contains("Rollback!"));
		}

		Assert.assertTrue(rolledBackListener.Invoked);
		Assert.assertFalse(committedListener.Invoked);

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly TransactionListenerTest outerInstance;

		  private org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener rolledBackListener;
		  private org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener committedListener;

		  public CommandAnonymousInnerClass(TransactionListenerTest outerInstance, org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener rolledBackListener, org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener committedListener)
		  {
			  this.outerInstance = outerInstance;
			  this.rolledBackListener = rolledBackListener;
			  this.committedListener = committedListener;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, rolledBackListener);
			commandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, committedListener);

			throw new Exception("Booum! Rollback!");
		  }

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSynchronizationOnCommitted()
	  public virtual void testSynchronizationOnCommitted()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestTransactionListener rolledBackListener = new TestTransactionListener();
		TestTransactionListener rolledBackListener = new TestTransactionListener();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestTransactionListener committedListener = new TestTransactionListener();
		TestTransactionListener committedListener = new TestTransactionListener();

		Assert.assertFalse(rolledBackListener.Invoked);
		Assert.assertFalse(committedListener.Invoked);

		try
		{

		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, rolledBackListener, committedListener));

		}
		catch (Exception e)
		{
		  Assert.assertTrue(e.Message.contains("Rollback!"));
		}

		Assert.assertFalse(rolledBackListener.Invoked);
		Assert.assertTrue(committedListener.Invoked);

	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly TransactionListenerTest outerInstance;

		  private org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener rolledBackListener;
		  private org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener committedListener;

		  public CommandAnonymousInnerClass2(TransactionListenerTest outerInstance, org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener rolledBackListener, org.camunda.bpm.integrationtest.functional.transactions.TransactionListenerTest.TestTransactionListener committedListener)
		  {
			  this.outerInstance = outerInstance;
			  this.rolledBackListener = rolledBackListener;
			  this.committedListener = committedListener;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, rolledBackListener);
			commandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, committedListener);
			return null;
		  }

	  }

	  protected internal class TestTransactionListener : TransactionListener
	  {

		protected internal volatile bool invoked = false;

		public virtual void execute(CommandContext commandContext)
		{
		  invoked = true;
		}

		public virtual bool Invoked
		{
			get
			{
			  return invoked;
			}
		}

	  }

	}

}