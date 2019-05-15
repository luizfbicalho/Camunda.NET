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
namespace org.camunda.bpm.engine.impl.cmd
{


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CorrelationHandler = org.camunda.bpm.engine.impl.runtime.CorrelationHandler;
	using CorrelationSet = org.camunda.bpm.engine.impl.runtime.CorrelationSet;
	using MessageCorrelationResultImpl = org.camunda.bpm.engine.impl.runtime.MessageCorrelationResultImpl;
	using CorrelationHandlerResult = org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureAtLeastOneNotNull;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// @author Michael Scholz
	/// </summary>
	public class CorrelateAllMessageCmd : AbstractCorrelateMessageCmd, Command<IList<MessageCorrelationResultImpl>>
	{

	  /// <summary>
	  /// Initialize the command with a builder
	  /// </summary>
	  /// <param name="messageCorrelationBuilderImpl"> </param>
	  public CorrelateAllMessageCmd(MessageCorrelationBuilderImpl messageCorrelationBuilderImpl, bool collectVariables, bool deserializeVariableValues) : base(messageCorrelationBuilderImpl, collectVariables, deserializeVariableValues)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.impl.runtime.MessageCorrelationResultImpl> execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<MessageCorrelationResultImpl> execute(CommandContext commandContext)
	  {
		ensureAtLeastOneNotNull("At least one of the following correlation criteria has to be present: " + "messageName, businessKey, correlationKeys, processInstanceId", messageName, builder.BusinessKey, builder.CorrelationProcessInstanceVariables, builder.ProcessInstanceId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.CorrelationHandler correlationHandler = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getCorrelationHandler();
		CorrelationHandler correlationHandler = Context.ProcessEngineConfiguration.CorrelationHandler;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.CorrelationSet correlationSet = new org.camunda.bpm.engine.impl.runtime.CorrelationSet(builder);
		CorrelationSet correlationSet = new CorrelationSet(builder);
		IList<CorrelationHandlerResult> correlationResults = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, correlationHandler, correlationSet));

		// check authorization
		foreach (CorrelationHandlerResult correlationResult in correlationResults)
		{
		  checkAuthorization(correlationResult);
		}

		IList<MessageCorrelationResultImpl> results = new List<MessageCorrelationResultImpl>();
		foreach (CorrelationHandlerResult correlationResult in correlationResults)
		{
		  results.Add(createMessageCorrelationResult(commandContext, correlationResult));
		}

		return results;
	  }

	  private class CallableAnonymousInnerClass : Callable<IList<CorrelationHandlerResult>>
	  {
		  private readonly CorrelateAllMessageCmd outerInstance;

		  private CommandContext commandContext;
		  private CorrelationHandler correlationHandler;
		  private CorrelationSet correlationSet;

		  public CallableAnonymousInnerClass(CorrelateAllMessageCmd outerInstance, CommandContext commandContext, CorrelationHandler correlationHandler, CorrelationSet correlationSet)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.correlationHandler = correlationHandler;
			  this.correlationSet = correlationSet;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult> call() throws Exception
		  public IList<CorrelationHandlerResult> call()
		  {
			return correlationHandler.correlateMessages(commandContext, outerInstance.messageName, correlationSet);
		  }
	  }
	}

}