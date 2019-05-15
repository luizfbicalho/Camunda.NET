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
namespace org.camunda.bpm.qa.performance.engine.steps
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PerfTestRunner = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunner;

	public class SignalTestRunListener : ExecutionListener
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(final org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void notify(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String runId = (String) execution.getVariable(PerfTestConstants.RUN_ID);
		string runId = (string) execution.getVariable(PerfTestConstants.RUN_ID);
		CommandContext commandContext = Context.CommandContext;
		if (!string.ReferenceEquals(runId, null) && commandContext != null)
		{
		  commandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, new TransactionListenerAnonymousInnerClass(this, runId, commandContext));
		}
	  }

	  private class TransactionListenerAnonymousInnerClass : TransactionListener
	  {
		  private readonly SignalTestRunListener outerInstance;

		  private string runId;
		  private CommandContext commandContext;

		  public TransactionListenerAnonymousInnerClass(SignalTestRunListener outerInstance, string runId, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.runId = runId;
			  this.commandContext = commandContext;
		  }

		  public void execute(CommandContext commandContext)
		  {
			// signal run after the transaction was committed
			PerfTestRunner.signalRun(runId);
		  }
	  }

	}

}