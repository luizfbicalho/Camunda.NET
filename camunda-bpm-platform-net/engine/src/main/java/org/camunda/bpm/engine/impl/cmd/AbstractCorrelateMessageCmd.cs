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
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionVariableSnapshotObserver = org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using CorrelationHandlerResult = org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult;
	using MessageCorrelationResultImpl = org.camunda.bpm.engine.impl.runtime.MessageCorrelationResultImpl;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// @author Michael Scholz
	/// @author Christopher Zell
	/// </summary>
	public abstract class AbstractCorrelateMessageCmd
	{

	  protected internal readonly string messageName;

	  protected internal readonly MessageCorrelationBuilderImpl builder;

	  protected internal ExecutionVariableSnapshotObserver variablesListener;
	  protected internal bool variablesEnabled = false;
	  protected internal bool deserializeVariableValues = false;

	  /// <summary>
	  /// Initialize the command with a builder
	  /// </summary>
	  /// <param name="builder"> </param>
	  protected internal AbstractCorrelateMessageCmd(MessageCorrelationBuilderImpl builder)
	  {
		this.builder = builder;
		this.messageName = builder.MessageName;
	  }

	  protected internal AbstractCorrelateMessageCmd(MessageCorrelationBuilderImpl builder, bool variablesEnabled, bool deserializeVariableValues) : this(builder)
	  {
		this.variablesEnabled = variablesEnabled;
		this.deserializeVariableValues = deserializeVariableValues;
	  }

	  protected internal virtual void triggerExecution(CommandContext commandContext, CorrelationHandlerResult correlationResult)
	  {
		string executionId = correlationResult.ExecutionEntity.Id;

		MessageEventReceivedCmd command = new MessageEventReceivedCmd(messageName, executionId, builder.PayloadProcessInstanceVariables, builder.PayloadProcessInstanceVariablesLocal, builder.ExclusiveCorrelation);
		command.execute(commandContext);
	  }

	  protected internal virtual ProcessInstance instantiateProcess(CommandContext commandContext, CorrelationHandlerResult correlationResult)
	  {
		ProcessDefinitionEntity processDefinitionEntity = correlationResult.ProcessDefinitionEntity;

		ActivityImpl messageStartEvent = processDefinitionEntity.findActivity(correlationResult.StartEventActivityId);
		ExecutionEntity processInstance = processDefinitionEntity.createProcessInstance(builder.BusinessKey, messageStartEvent);

		if (variablesEnabled)
		{
		  variablesListener = new ExecutionVariableSnapshotObserver(processInstance, false, deserializeVariableValues);
		}

		processInstance.VariablesLocal = builder.PayloadProcessInstanceVariablesLocal;

		processInstance.start(builder.PayloadProcessInstanceVariables);

		return processInstance;
	  }

	  protected internal virtual void checkAuthorization(CorrelationHandlerResult correlation)
	  {
		CommandContext commandContext = Context.CommandContext;

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (MessageCorrelationResultType.Execution.Equals(correlation.ResultType))
		  {
			ExecutionEntity execution = correlation.ExecutionEntity;
			checker.checkUpdateProcessInstanceById(execution.ProcessInstanceId);

		  }
		  else
		  {
			ProcessDefinitionEntity definition = correlation.ProcessDefinitionEntity;

			checker.checkCreateProcessInstance(definition);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.runtime.MessageCorrelationResultImpl createMessageCorrelationResult(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, final org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult handlerResult)
	  protected internal virtual MessageCorrelationResultImpl createMessageCorrelationResult(CommandContext commandContext, CorrelationHandlerResult handlerResult)
	  {
		MessageCorrelationResultImpl resultWithVariables = new MessageCorrelationResultImpl(handlerResult);
		if (MessageCorrelationResultType.Execution.Equals(handlerResult.ResultType))
		{
		  ExecutionEntity execution = findProcessInstanceExecution(commandContext, handlerResult);
		  if (variablesEnabled && execution != null)
		  {
			variablesListener = new ExecutionVariableSnapshotObserver(execution, false, deserializeVariableValues);
		  }
		  triggerExecution(commandContext, handlerResult);
		}
		else
		{
		  ProcessInstance instance = instantiateProcess(commandContext, handlerResult);
		  resultWithVariables.ProcessInstance = instance;
		}

		if (variablesListener != null)
		{
		  resultWithVariables.Variables = variablesListener.Variables;
		}

		return resultWithVariables;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity findProcessInstanceExecution(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, final org.camunda.bpm.engine.impl.runtime.CorrelationHandlerResult handlerResult)
	  protected internal virtual ExecutionEntity findProcessInstanceExecution(CommandContext commandContext, CorrelationHandlerResult handlerResult)
	  {
		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(handlerResult.Execution.ProcessInstanceId);
		return execution;
	  }

	}

}