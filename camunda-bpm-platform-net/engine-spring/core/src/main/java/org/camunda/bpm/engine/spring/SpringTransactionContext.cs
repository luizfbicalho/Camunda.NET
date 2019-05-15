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
namespace org.camunda.bpm.engine.spring
{

	using TransactionContext = org.camunda.bpm.engine.impl.cfg.TransactionContext;
	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PlatformTransactionManager = org.springframework.transaction.PlatformTransactionManager;
	using TransactionSynchronization = org.springframework.transaction.support.TransactionSynchronization;
	using TransactionSynchronizationManager = org.springframework.transaction.support.TransactionSynchronizationManager;


	/// <summary>
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class SpringTransactionContext : TransactionContext
	{

	  protected internal PlatformTransactionManager transactionManager;
	  protected internal CommandContext commandContext;
	  protected internal TransactionState lastTransactionState = null;

	  public SpringTransactionContext(PlatformTransactionManager transactionManager, CommandContext commandContext)
	  {
		this.transactionManager = transactionManager;
		this.commandContext = commandContext;
		TransactionSynchronizationManager.registerSynchronization(new TransactionSynchronizationAdapterAnonymousInnerClass(this));
	  }

	  private class TransactionSynchronizationAdapterAnonymousInnerClass : TransactionSynchronizationAdapter
	  {
		  private readonly SpringTransactionContext outerInstance;

		  public TransactionSynchronizationAdapterAnonymousInnerClass(SpringTransactionContext outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override void beforeCommit(bool readOnly)
		  {
			outerInstance.lastTransactionState = TransactionState.COMMITTING;
		  }
		  public override void afterCommit()
		  {
			outerInstance.lastTransactionState = TransactionState.COMMITTED;
		  }
		  public override void beforeCompletion()
		  {
			outerInstance.lastTransactionState = TransactionState.ROLLINGBACK;
		  }
		  public override void afterCompletion(int status)
		  {
			if (TransactionSynchronization.STATUS_ROLLED_BACK == status)
			{
			  outerInstance.lastTransactionState = TransactionState.ROLLED_BACK;
			}
		  }
	  }

	  public virtual void commit()
	  {
		// Do nothing, transaction is managed by spring
	  }

	  public virtual void rollback()
	  {
		// Just in case the rollback isn't triggered by an
		// exception, we mark the current transaction rollBackOnly.
		transactionManager.getTransaction(null).setRollbackOnly();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void addTransactionListener(final org.camunda.bpm.engine.impl.cfg.TransactionState transactionState, final org.camunda.bpm.engine.impl.cfg.TransactionListener transactionListener)
	  public virtual void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener)
	  {
		if (transactionState.Equals(TransactionState.COMMITTING))
		{

		  TransactionSynchronizationManager.registerSynchronization(new TransactionSynchronizationAdapterAnonymousInnerClass2(this, transactionListener));

		}
		else if (transactionState.Equals(TransactionState.COMMITTED))
		{

		  TransactionSynchronizationManager.registerSynchronization(new TransactionSynchronizationAdapterAnonymousInnerClass3(this, transactionListener));

		}
		else if (transactionState.Equals(TransactionState.ROLLINGBACK))
		{

		  TransactionSynchronizationManager.registerSynchronization(new TransactionSynchronizationAdapterAnonymousInnerClass4(this, transactionListener));

		}
		else if (transactionState.Equals(TransactionState.ROLLED_BACK))
		{

		  TransactionSynchronizationManager.registerSynchronization(new TransactionSynchronizationAdapterAnonymousInnerClass5(this, transactionListener));

		}

	  }

	  private class TransactionSynchronizationAdapterAnonymousInnerClass2 : TransactionSynchronizationAdapter
	  {
		  private readonly SpringTransactionContext outerInstance;

		  private TransactionListener transactionListener;

		  public TransactionSynchronizationAdapterAnonymousInnerClass2(SpringTransactionContext outerInstance, TransactionListener transactionListener) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.transactionListener = transactionListener;
		  }

		  public override void beforeCommit(bool readOnly)
		  {
			transactionListener.execute(outerInstance.commandContext);
		  }
	  }

	  private class TransactionSynchronizationAdapterAnonymousInnerClass3 : TransactionSynchronizationAdapter
	  {
		  private readonly SpringTransactionContext outerInstance;

		  private TransactionListener transactionListener;

		  public TransactionSynchronizationAdapterAnonymousInnerClass3(SpringTransactionContext outerInstance, TransactionListener transactionListener) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.transactionListener = transactionListener;
		  }

		  public override void afterCommit()
		  {
			transactionListener.execute(outerInstance.commandContext);
		  }
	  }

	  private class TransactionSynchronizationAdapterAnonymousInnerClass4 : TransactionSynchronizationAdapter
	  {
		  private readonly SpringTransactionContext outerInstance;

		  private TransactionListener transactionListener;

		  public TransactionSynchronizationAdapterAnonymousInnerClass4(SpringTransactionContext outerInstance, TransactionListener transactionListener) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.transactionListener = transactionListener;
		  }

		  public override void beforeCompletion()
		  {
			transactionListener.execute(outerInstance.commandContext);
		  }
	  }

	  private class TransactionSynchronizationAdapterAnonymousInnerClass5 : TransactionSynchronizationAdapter
	  {
		  private readonly SpringTransactionContext outerInstance;

		  private TransactionListener transactionListener;

		  public TransactionSynchronizationAdapterAnonymousInnerClass5(SpringTransactionContext outerInstance, TransactionListener transactionListener) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.transactionListener = transactionListener;
		  }

		  public override void afterCompletion(int status)
		  {
			if (TransactionSynchronization.STATUS_ROLLED_BACK == status)
			{
			  transactionListener.execute(outerInstance.commandContext);
			}
		  }
	  }

	  public virtual bool TransactionActive
	  {
		  get
		  {
			return TransactionSynchronizationManager.ActualTransactionActive && !TransactionState.ROLLED_BACK.Equals(lastTransactionState) && !TransactionState.ROLLINGBACK.Equals(lastTransactionState);
		  }
	  }

	  protected internal abstract class TransactionSynchronizationAdapter : TransactionSynchronization
	  {
		  private readonly SpringTransactionContext outerInstance;

		  public TransactionSynchronizationAdapter(SpringTransactionContext outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual void suspend()
		{
		}

		public virtual void resume()
		{
		}

		public virtual void flush()
		{
		}

		public virtual void beforeCommit(bool readOnly)
		{
		}

		public virtual void beforeCompletion()
		{
		}

		public virtual void afterCommit()
		{
		}

		public virtual void afterCompletion(int status)
		{
		}

	  }

	}

}