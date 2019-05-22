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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CorrelateAllMessageCmd = org.camunda.bpm.engine.impl.cmd.CorrelateAllMessageCmd;
	using CorrelateMessageCmd = org.camunda.bpm.engine.impl.cmd.CorrelateMessageCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Christopher Zell
	/// 
	/// </summary>
	public class MessageCorrelationBuilderImpl : MessageCorrelationBuilder
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandContext commandContext;

	  protected internal bool isExclusiveCorrelation = false;

	  protected internal string messageName;
	  protected internal string businessKey;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;

	  protected internal VariableMap correlationProcessInstanceVariables;
	  protected internal VariableMap correlationLocalVariables;
	  protected internal VariableMap payloadProcessInstanceVariables;
	  protected internal VariableMap payloadProcessInstanceVariablesLocal;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed = null;
	  protected internal bool isTenantIdSet = false;

	  protected internal bool startMessagesOnly = false;

	  public MessageCorrelationBuilderImpl(CommandExecutor commandExecutor, string messageName) : this(messageName)
	  {
		ensureNotNull("commandExecutor", commandExecutor);
		this.commandExecutor = commandExecutor;
	  }

	  public MessageCorrelationBuilderImpl(CommandContext commandContext, string messageName) : this(messageName)
	  {
		ensureNotNull("commandContext", commandContext);
		this.commandContext = commandContext;
	  }

	  private MessageCorrelationBuilderImpl(string messageName)
	  {
		this.messageName = messageName;
	  }

	  public virtual MessageCorrelationBuilder processInstanceBusinessKey(string businessKey)
	  {
		ensureNotNull("businessKey", businessKey);
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual MessageCorrelationBuilder processInstanceVariableEquals(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		ensureCorrelationProcessInstanceVariablesInitialized();

		correlationProcessInstanceVariables.put(variableName, variableValue);
		return this;
	  }

	  public virtual MessageCorrelationBuilder processInstanceVariablesEqual(IDictionary<string, object> variables)
	  {
		ensureNotNull("variables", variables);
		ensureCorrelationProcessInstanceVariablesInitialized();

		correlationProcessInstanceVariables.putAll(variables);
		return this;
	  }

	  public virtual MessageCorrelationBuilder localVariableEquals(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		ensureCorrelationLocalVariablesInitialized();

		correlationLocalVariables.put(variableName, variableValue);
		return this;
	  }

	  public virtual MessageCorrelationBuilder localVariablesEqual(IDictionary<string, object> variables)
	  {
		ensureNotNull("variables", variables);
		ensureCorrelationLocalVariablesInitialized();

		correlationLocalVariables.putAll(variables);
		return this;
	  }

	  protected internal virtual void ensureCorrelationProcessInstanceVariablesInitialized()
	  {
		if (correlationProcessInstanceVariables == null)
		{
		  correlationProcessInstanceVariables = new VariableMapImpl();
		}
	  }

	  protected internal virtual void ensureCorrelationLocalVariablesInitialized()
	  {
		if (correlationLocalVariables == null)
		{
		  correlationLocalVariables = new VariableMapImpl();
		}
	  }

	  public virtual MessageCorrelationBuilder processInstanceId(string id)
	  {
		ensureNotNull("processInstanceId", id);
		this.processInstanceId_Renamed = id;
		return this;
	  }

	  public virtual MessageCorrelationBuilder processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MessageCorrelationBuilder setVariable(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		ensurePayloadProcessInstanceVariablesInitialized();
		payloadProcessInstanceVariables.put(variableName, variableValue);
		return this;
	  }

	  public virtual MessageCorrelationBuilder setVariableLocal(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		ensurePayloadProcessInstanceVariablesLocalInitialized();
		payloadProcessInstanceVariablesLocal.put(variableName, variableValue);
		return this;
	  }

	  public virtual MessageCorrelationBuilder setVariables(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  ensurePayloadProcessInstanceVariablesInitialized();
		  payloadProcessInstanceVariables.putAll(variables);
		}
		return this;
	  }

	  public virtual MessageCorrelationBuilder setVariablesLocal(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  ensurePayloadProcessInstanceVariablesLocalInitialized();
		  payloadProcessInstanceVariablesLocal.putAll(variables);
		}
		return this;
	  }

	  protected internal virtual void ensurePayloadProcessInstanceVariablesInitialized()
	  {
		if (payloadProcessInstanceVariables == null)
		{
		  payloadProcessInstanceVariables = new VariableMapImpl();
		}
	  }

	  protected internal virtual void ensurePayloadProcessInstanceVariablesLocalInitialized()
	  {
		if (payloadProcessInstanceVariablesLocal == null)
		{
		  payloadProcessInstanceVariablesLocal = new VariableMapImpl();
		}
	  }

	  public virtual MessageCorrelationBuilder tenantId(string tenantId)
	  {
		ensureNotNull("The tenant-id cannot be null. Use 'withoutTenantId()' if you want to correlate the message to a process definition or an execution which has no tenant-id.", "tenantId", tenantId);

		isTenantIdSet = true;
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MessageCorrelationBuilder withoutTenantId()
	  {
		isTenantIdSet = true;
		tenantId_Renamed = null;
		return this;
	  }

	  public virtual MessageCorrelationBuilder startMessageOnly()
	  {
		startMessagesOnly = true;
		return this;
	  }

	  public virtual void correlate()
	  {
		correlateWithResult();
	  }

	  public virtual MessageCorrelationResult correlateWithResult()
	  {
		if (startMessagesOnly)
		{
		  ensureCorrelationVariablesNotSet();
		  ensureProcessDefinitionAndTenantIdNotSet();
		}
		else
		{
		  ensureProcessDefinitionIdNotSet();
		  ensureProcessInstanceAndTenantIdNotSet();
		}
		return execute(new CorrelateMessageCmd(this, false, false, startMessagesOnly));
	  }

	  public virtual MessageCorrelationResultWithVariables correlateWithResultAndVariables(bool deserializeValues)
	  {
		if (startMessagesOnly)
		{
		  ensureCorrelationVariablesNotSet();
		  ensureProcessDefinitionAndTenantIdNotSet();
		}
		else
		{
		  ensureProcessDefinitionIdNotSet();
		  ensureProcessInstanceAndTenantIdNotSet();
		}
		return execute(new CorrelateMessageCmd(this, true, deserializeValues, startMessagesOnly));
	  }

	  public virtual void correlateExclusively()
	  {
		isExclusiveCorrelation = true;

		correlate();
	  }

	  public virtual void correlateAll()
	  {
		correlateAllWithResult();
	  }

	  public virtual IList<MessageCorrelationResult> correlateAllWithResult()
	  {
		if (startMessagesOnly)
		{
		  ensureCorrelationVariablesNotSet();
		  ensureProcessDefinitionAndTenantIdNotSet();
		  // only one result can be expected
		  MessageCorrelationResult result = execute(new CorrelateMessageCmd(this, false, false, startMessagesOnly));
		  return Arrays.asList(result);
		}
		else
		{
		  ensureProcessDefinitionIdNotSet();
		  ensureProcessInstanceAndTenantIdNotSet();
		  return (System.Collections.IList) execute(new CorrelateAllMessageCmd(this, false, false));
		}
	  }

	  public virtual IList<MessageCorrelationResultWithVariables> correlateAllWithResultAndVariables(bool deserializeValues)
	  {
		if (startMessagesOnly)
		{
		  ensureCorrelationVariablesNotSet();
		  ensureProcessDefinitionAndTenantIdNotSet();
		  // only one result can be expected
		  MessageCorrelationResultWithVariables result = execute(new CorrelateMessageCmd(this, true, deserializeValues, startMessagesOnly));
		  return Arrays.asList(result);
		}
		else
		{
		  ensureProcessDefinitionIdNotSet();
		  ensureProcessInstanceAndTenantIdNotSet();
		  return (System.Collections.IList) execute(new CorrelateAllMessageCmd(this, true, deserializeValues));
		}
	  }

	  public virtual ProcessInstance correlateStartMessage()
	  {
		startMessageOnly();
		MessageCorrelationResult result = correlateWithResult();
		return result.ProcessInstance;
	  }

	  protected internal virtual void ensureProcessDefinitionIdNotSet()
	  {
		if (!string.ReferenceEquals(processDefinitionId_Renamed, null))
		{
		  throw LOG.exceptionCorrelateMessageWithProcessDefinitionId();
		}
	  }

	  protected internal virtual void ensureProcessInstanceAndTenantIdNotSet()
	  {
		if (!string.ReferenceEquals(processInstanceId_Renamed, null) && isTenantIdSet)
		{
		  throw LOG.exceptionCorrelateMessageWithProcessInstanceAndTenantId();
		}
	  }

	  protected internal virtual void ensureCorrelationVariablesNotSet()
	  {
		if (correlationProcessInstanceVariables != null || correlationLocalVariables != null)
		{
		  throw LOG.exceptionCorrelateStartMessageWithCorrelationVariables();
		}
	  }

	  protected internal virtual void ensureProcessDefinitionAndTenantIdNotSet()
	  {
		if (!string.ReferenceEquals(processDefinitionId_Renamed, null) && isTenantIdSet)
		{
		  throw LOG.exceptionCorrelateMessageWithProcessDefinitionAndTenantId();
		}
	  }

	  protected internal virtual T execute<T>(Command<T> command)
	  {
		if (commandExecutor != null)
		{
		  return commandExecutor.execute(command);
		}
		else
		{
		  return command.execute(commandContext);
		}
	  }

	  // getters //////////////////////////////////

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual CommandContext CommandContext
	  {
		  get
		  {
			return commandContext;
		  }
	  }

	  public virtual string MessageName
	  {
		  get
		  {
			return messageName;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual IDictionary<string, object> CorrelationProcessInstanceVariables
	  {
		  get
		  {
			return correlationProcessInstanceVariables;
		  }
	  }

	  public virtual IDictionary<string, object> CorrelationLocalVariables
	  {
		  get
		  {
			return correlationLocalVariables;
		  }
	  }

	  public virtual IDictionary<string, object> PayloadProcessInstanceVariables
	  {
		  get
		  {
			return payloadProcessInstanceVariables;
		  }
	  }

	  public virtual VariableMap PayloadProcessInstanceVariablesLocal
	  {
		  get
		  {
			return payloadProcessInstanceVariablesLocal;
		  }
	  }

	  public virtual bool ExclusiveCorrelation
	  {
		  get
		  {
			return isExclusiveCorrelation;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Renamed;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	}

}