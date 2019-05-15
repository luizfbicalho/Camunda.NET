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
namespace org.camunda.bpm.integrationtest.functional.transactions.beans
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class FailingTransactionListenerDelegate implements org.camunda.bpm.engine.delegate.JavaDelegate
	public class FailingTransactionListenerDelegate : JavaDelegate
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{

		Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTING, new TransactionListenerAnonymousInnerClass(this));
		}

	  private class TransactionListenerAnonymousInnerClass : TransactionListener
	  {
		  private readonly FailingTransactionListenerDelegate outerInstance;

		  public TransactionListenerAnonymousInnerClass(FailingTransactionListenerDelegate outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public void execute(CommandContext context)
		  {
			throw new Exception("exception in transaction listener");
		  }
	  }

	}

}