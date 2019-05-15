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
namespace org.camunda.bpm.engine.impl.interceptor
{

	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;

	/// <summary>
	/// @author Guillaume Nodet
	/// </summary>
	public class JtaTransactionInterceptor : CommandInterceptor
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  private readonly TransactionManager transactionManager;
	  private readonly bool requiresNew;

	  public JtaTransactionInterceptor(TransactionManager transactionManager, bool requiresNew)
	  {
		this.transactionManager = transactionManager;
		this.requiresNew = requiresNew;
	  }

	  public override T execute<T>(Command<T> command)
	  {
		Transaction oldTx = null;
		try
		{
		  bool existing = Existing;
		  bool isNew = !existing || requiresNew;
		  if (existing && requiresNew)
		  {
			oldTx = doSuspend();
		  }
		  if (isNew)
		  {
			doBegin();
		  }
		  T result;
		  try
		  {
			result = next.execute(command);
		  }
		  catch (Exception ex)
		  {
			doRollback(isNew);
			throw ex;
		  }
		  catch (Exception err)
		  {
			doRollback(isNew);
			throw err;
		  }
		  catch (Exception ex)
		  {
			doRollback(isNew);
			throw new UndeclaredThrowableException(ex, "TransactionCallback threw undeclared checked exception");
		  }
		  if (isNew)
		  {
			doCommit();
		  }
		  return result;
		}
		finally
		{
		  doResume(oldTx);
		}
	  }

	  private void doBegin()
	  {
		try
		{
		  transactionManager.begin();
		}
		catch (NotSupportedException e)
		{
		  throw new TransactionException("Unable to begin transaction", e);
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to begin transaction", e);
		}
	  }

	  private bool Existing
	  {
		  get
		  {
			try
			{
			  return transactionManager.Status != Status.STATUS_NO_TRANSACTION;
			}
			catch (SystemException e)
			{
			  throw new TransactionException("Unable to retrieve transaction status", e);
			}
		  }
	  }

	  private Transaction doSuspend()
	  {
		try
		{
		  return transactionManager.suspend();
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to suspend transaction", e);
		}
	  }

	  private void doResume(Transaction tx)
	  {
		if (tx != null)
		{
		  try
		  {
			transactionManager.resume(tx);
		  }
		  catch (SystemException e)
		  {
			throw new TransactionException("Unable to resume transaction", e);
		  }
		  catch (InvalidTransactionException e)
		  {
			throw new TransactionException("Unable to resume transaction", e);
		  }
		}
	  }

	  private void doCommit()
	  {
		try
		{
		  transactionManager.commit();
		}
		catch (HeuristicMixedException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (HeuristicRollbackException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (RollbackException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (Exception e)
		{
		  doRollback(true);
		  throw e;
		}
		catch (Exception e)
		{
		  doRollback(true);
		  throw e;
		}
	  }

	  private void doRollback(bool isNew)
	  {
		try
		{
		  if (isNew)
		  {
			transactionManager.rollback();
		  }
		  else
		  {
			transactionManager.setRollbackOnly();
		  }
		}
		catch (SystemException e)
		{
		  LOG.exceptionWhileRollingBackTransaction(e);
		}
	  }

	  private class TransactionException : Exception
	  {
		internal const long serialVersionUID = 1L;

		internal TransactionException()
		{
		}

		internal TransactionException(string s) : base(s)
		{
		}

		internal TransactionException(string s, Exception throwable) : base(s, throwable)
		{
		}

		internal TransactionException(Exception throwable) : base(throwable)
		{
		}
	  }
	}

}