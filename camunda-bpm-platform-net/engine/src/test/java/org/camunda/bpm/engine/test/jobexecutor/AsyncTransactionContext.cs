using System.Threading;

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
namespace org.camunda.bpm.engine.test.jobexecutor
{
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using StandaloneTransactionContext = org.camunda.bpm.engine.impl.cfg.standalone.StandaloneTransactionContext;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AsyncTransactionContext : StandaloneTransactionContext
	{

	  public AsyncTransactionContext(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  // event is fired in new thread
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void fireTransactionEvent(final org.camunda.bpm.engine.impl.cfg.TransactionState transactionState)
	  protected internal override void fireTransactionEvent(TransactionState transactionState)
	  {
		Thread thread = new Thread(() =>
		{
	fireTransactionEventAsync(transactionState);
		});

		thread.Start();
		try
		{
		  thread.Join();
		}
		catch (InterruptedException e)
		{
		  throw new ProcessEngineException(e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void fireTransactionEventAsync(final org.camunda.bpm.engine.impl.cfg.TransactionState transactionState)
	  protected internal virtual void fireTransactionEventAsync(TransactionState transactionState)
	  {
		base.fireTransactionEvent(transactionState);
	  }



	}

}