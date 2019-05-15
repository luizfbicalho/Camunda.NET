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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using PlatformTransactionManager = org.springframework.transaction.PlatformTransactionManager;
	using TransactionStatus = org.springframework.transaction.TransactionStatus;
	using TransactionCallback = org.springframework.transaction.support.TransactionCallback;
	using TransactionTemplate = org.springframework.transaction.support.TransactionTemplate;

	/// <summary>
	/// @author Dave Syer
	/// @author Tom Baeyens
	/// </summary>
	public class SpringTransactionInterceptor : CommandInterceptor
	{

	  protected internal PlatformTransactionManager transactionManager;
	  protected internal int transactionPropagation;

	  public SpringTransactionInterceptor(PlatformTransactionManager transactionManager, int transactionPropagation)
	  {
		this.transactionManager = transactionManager;
		this.transactionPropagation = transactionPropagation;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T execute(final org.camunda.bpm.engine.impl.interceptor.Command<T> command)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public override T execute<T>(Command<T> command)
	  {
		TransactionTemplate transactionTemplate = new TransactionTemplate(transactionManager);
		transactionTemplate.PropagationBehavior = transactionPropagation;
		T result = (T) transactionTemplate.execute(new TransactionCallbackAnonymousInnerClass(this, command));
		return result;
	  }

	  private class TransactionCallbackAnonymousInnerClass : TransactionCallback
	  {
		  private readonly SpringTransactionInterceptor outerInstance;

		  private Command<T> command;

		  public TransactionCallbackAnonymousInnerClass(SpringTransactionInterceptor outerInstance, Command<T> command)
		  {
			  this.outerInstance = outerInstance;
			  this.command = command;
		  }

		  public object doInTransaction(TransactionStatus status)
		  {
			return outerInstance.next.execute(command);
		  }
	  }
	}
}