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
namespace org.camunda.bpm.engine.impl.cfg.standalone
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using PersistenceSession = org.camunda.bpm.engine.impl.db.PersistenceSession;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class StandaloneTransactionContext : TransactionContext
	{

	  private static readonly TransactionLogger LOG = ProcessEngineLogger.TX_LOGGER;

	  protected internal CommandContext commandContext;
	  protected internal IDictionary<TransactionState, IList<TransactionListener>> stateTransactionListeners = null;
	  private TransactionState lastTransactionState;

	  public StandaloneTransactionContext(CommandContext commandContext)
	  {
		this.commandContext = commandContext;
	  }

	  public virtual void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener)
	  {
		if (stateTransactionListeners == null)
		{
		  stateTransactionListeners = new Dictionary<TransactionState, IList<TransactionListener>>();
		}
		IList<TransactionListener> transactionListeners = stateTransactionListeners[transactionState];
		if (transactionListeners == null)
		{
		  transactionListeners = new List<TransactionListener>();
		  stateTransactionListeners[transactionState] = transactionListeners;
		}
		transactionListeners.Add(transactionListener);
	  }

	  public virtual void commit()
	  {
		LOG.debugTransactionOperation("firing event committing...");

		fireTransactionEvent(TransactionState.COMMITTING);

		LOG.debugTransactionOperation("committing the persistence session...");

		PersistenceProvider.commit();

		LOG.debugTransactionOperation("firing event committed...");

		fireTransactionEvent(TransactionState.COMMITTED);
	  }

	  protected internal virtual void fireTransactionEvent(TransactionState transactionState)
	  {
		this.LastTransactionState = transactionState;
		if (stateTransactionListeners == null)
		{
		  return;
		}
		IList<TransactionListener> transactionListeners = stateTransactionListeners[transactionState];
		if (transactionListeners == null)
		{
		  return;
		}
		foreach (TransactionListener transactionListener in transactionListeners)
		{
		  transactionListener.execute(commandContext);
		}
	  }

	  protected internal virtual TransactionState LastTransactionState
	  {
		  set
		  {
			this.lastTransactionState = value;
		  }
	  }

	  private PersistenceSession PersistenceProvider
	  {
		  get
		  {
			return commandContext.getSession(typeof(PersistenceSession));
		  }
	  }

	  public virtual void rollback()
	  {
		try
		{
		  try
		  {
			LOG.debugTransactionOperation("firing event rollback...");
			fireTransactionEvent(TransactionState.ROLLINGBACK);

		  }
		  catch (Exception exception)
		  {
			LOG.exceptionWhileFiringEvent(TransactionState.ROLLINGBACK, exception);
			Context.CommandInvocationContext.trySetThrowable(exception);
		  }
		  finally
		  {
			LOG.debugTransactionOperation("rolling back the persistence session...");
			PersistenceProvider.rollback();
		  }

		}
		catch (Exception exception)
		{
		  LOG.exceptionWhileFiringEvent(TransactionState.ROLLINGBACK, exception);
		  Context.CommandInvocationContext.trySetThrowable(exception);
		}
		finally
		{
		  LOG.debugFiringEventRolledBack();
		  fireTransactionEvent(TransactionState.ROLLED_BACK);
		}
	  }

	  public virtual bool TransactionActive
	  {
		  get
		  {
			return !TransactionState.ROLLINGBACK.Equals(lastTransactionState) && !TransactionState.ROLLED_BACK.Equals(lastTransactionState);
		  }
	  }
	}

}