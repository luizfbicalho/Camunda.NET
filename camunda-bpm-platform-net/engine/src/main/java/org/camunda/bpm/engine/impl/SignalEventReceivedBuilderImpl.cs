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
	using SignalEventReceivedCmd = org.camunda.bpm.engine.impl.cmd.SignalEventReceivedCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SignalEventReceivedBuilder = org.camunda.bpm.engine.runtime.SignalEventReceivedBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	public class SignalEventReceivedBuilderImpl : SignalEventReceivedBuilder
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal readonly CommandExecutor commandExecutor;
	  protected internal readonly string signalName;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict = null;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict = null;
	  protected internal bool isTenantIdSet = false;

	  protected internal VariableMap variables = null;

	  public SignalEventReceivedBuilderImpl(CommandExecutor commandExecutor, string signalName)
	  {
		this.commandExecutor = commandExecutor;
		this.signalName = signalName;
	  }

	  public virtual SignalEventReceivedBuilder setVariables(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{

		  if (this.variables == null)
		  {
			this.variables = new VariableMapImpl();
		  }
		  this.variables.putAll(variables);
		}
		return this;
	  }

	  public virtual SignalEventReceivedBuilder executionId(string executionId)
	  {
		ensureNotNull("executionId", executionId);
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual SignalEventReceivedBuilder tenantId(string tenantId)
	  {
		ensureNotNull("The tenant-id cannot be null. Use 'withoutTenantId()' if you want to send the signal to a process definition or an execution which has no tenant-id.", "tenantId", tenantId);

		this.tenantId_Conflict = tenantId;
		isTenantIdSet = true;

		return this;
	  }

	  public virtual SignalEventReceivedBuilder withoutTenantId()
	  {
		// tenant-id is null
		isTenantIdSet = true;
		return this;
	  }

	  public virtual void send()
	  {
		if (!string.ReferenceEquals(executionId_Conflict, null) && isTenantIdSet)
		{
		  throw LOG.exceptionDeliverSignalToSingleExecutionWithTenantId();
		}

		SignalEventReceivedCmd command = new SignalEventReceivedCmd(this);
		commandExecutor.execute(command);
	  }

	  public virtual string SignalName
	  {
		  get
		  {
			return signalName;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Conflict;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public virtual VariableMap getVariables()
	  {
		return variables;
	  }

	}

}