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
	using CorrelationHandlerResult = org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	public class CorrelateStartMessageCmd : AbstractCorrelateMessageCmd, Command<ProcessInstance>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public CorrelateStartMessageCmd(MessageCorrelationBuilderImpl messageCorrelationBuilderImpl) : base(messageCorrelationBuilderImpl)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ProcessInstance execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual ProcessInstance execute(CommandContext commandContext)
	  {
		ensureNotNull("messageName", messageName);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.CorrelationHandler correlationHandler = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getCorrelationHandler();
		CorrelationHandler correlationHandler = Context.ProcessEngineConfiguration.CorrelationHandler;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.CorrelationSet correlationSet = new org.camunda.bpm.engine.impl.runtime.CorrelationSet(builder);
		CorrelationSet correlationSet = new CorrelationSet(builder);

		IList<CorrelationHandlerResult> correlationResults = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, correlationHandler, correlationSet));

		if (correlationResults.Count == 0)
		{
		  throw new MismatchingMessageCorrelationException(messageName, "No process definition matches the parameters");

		}
		else if (correlationResults.Count > 1)
		{
		  throw LOG.exceptionCorrelateMessageToSingleProcessDefinition(messageName, correlationResults.Count, correlationSet);

		}
		else
		{
		  CorrelationHandlerResult correlationResult = correlationResults[0];

		  checkAuthorization(correlationResult);

		  ProcessInstance processInstance = instantiateProcess(commandContext, correlationResult);
		  return processInstance;
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<IList<CorrelationHandlerResult>>
	  {
		  private readonly CorrelateStartMessageCmd outerInstance;

		  private CommandContext commandContext;
		  private CorrelationHandler correlationHandler;
		  private CorrelationSet correlationSet;

		  public CallableAnonymousInnerClass(CorrelateStartMessageCmd outerInstance, CommandContext commandContext, CorrelationHandler correlationHandler, CorrelationSet correlationSet)
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
			return correlationHandler.correlateStartMessages(commandContext, outerInstance.messageName, correlationSet);
		  }
	  }
	}

}