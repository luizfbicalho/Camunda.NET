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
namespace org.camunda.bpm.engine.impl.cfg.jta
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class JtaTransactionContext : TransactionContext
	{

	  public static readonly TransactionLogger LOG = ProcessEngineLogger.TX_LOGGER;

	  protected internal readonly TransactionManager transactionManager;

	  public JtaTransactionContext(TransactionManager transactionManager)
	  {
		this.transactionManager = transactionManager;
	  }

	  public virtual void commit()
	  {
		// managed transaction, ignore
	  }

	  public virtual void rollback()
	  {
		// managed transaction, mark rollback-only if not done so already.
		try
		{
		  Transaction transaction = Transaction;
		  int status = transaction.Status;
		  if (status != Status.STATUS_NO_TRANSACTION && status != Status.STATUS_ROLLEDBACK)
		  {
			transaction.setRollbackOnly();
		  }
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInteractingWithTransaction("setting transaction rollback only", e);
		}
	  }

	  protected internal virtual Transaction Transaction
	  {
		  get
		  {
			try
			{
			  return transactionManager.Transaction;
			}
			catch (Exception e)
			{
			  throw LOG.exceptionWhileInteractingWithTransaction("getting transaction", e);
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void addTransactionListener(org.camunda.bpm.engine.impl.cfg.TransactionState transactionState, final org.camunda.bpm.engine.impl.cfg.TransactionListener transactionListener)
	  public virtual void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener)
	  {
		Transaction transaction = Transaction;
		CommandContext commandContext = Context.CommandContext;
		try
		{
		  transaction.registerSynchronization(new TransactionStateSynchronization(transactionState, transactionListener, commandContext));
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInteractingWithTransaction("registering synchronization", e);
		}
	  }

	  public class TransactionStateSynchronization : Synchronization
	  {

		protected internal readonly TransactionListener transactionListener;
		protected internal readonly TransactionState transactionState;
		internal readonly CommandContext commandContext;

		public TransactionStateSynchronization(TransactionState transactionState, TransactionListener transactionListener, CommandContext commandContext)
		{
		  this.transactionState = transactionState;
		  this.transactionListener = transactionListener;
		  this.commandContext = commandContext;
		}

		public virtual void beforeCompletion()
		{
		  if (TransactionState.COMMITTING.Equals(transactionState) || TransactionState.ROLLINGBACK.Equals(transactionState))
		  {
			transactionListener.execute(commandContext);
		  }
		}

		public virtual void afterCompletion(int status)
		{
		  if (Status.STATUS_ROLLEDBACK == status && TransactionState.ROLLED_BACK.Equals(transactionState))
		  {
			transactionListener.execute(commandContext);
		  }
		  else if (Status.STATUS_COMMITTED == status && TransactionState.COMMITTED.Equals(transactionState))
		  {
			transactionListener.execute(commandContext);
		  }
		}

	  }

	  public virtual bool TransactionActive
	  {
		  get
		  {
			try
			{
			  return transactionManager.Status != Status.STATUS_MARKED_ROLLBACK && transactionManager.Status != Status.STATUS_NO_TRANSACTION;
			}
			catch (SystemException e)
			{
			  throw LOG.exceptionWhileInteractingWithTransaction("getting transaction state", e);
			}
		  }
	  }

	}

}