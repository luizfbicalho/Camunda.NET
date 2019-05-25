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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using StartProcessInstanceAtActivitiesCmd = org.camunda.bpm.engine.impl.cmd.StartProcessInstanceAtActivitiesCmd;
	using StartProcessInstanceCmd = org.camunda.bpm.engine.impl.cmd.StartProcessInstanceCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;

	/// <summary>
	/// Simply wraps a modification builder because their API is equivalent.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ProcessInstantiationBuilderImpl : ProcessInstantiationBuilder
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal CommandExecutor commandExecutor;

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string businessKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionTenantId_Conflict;
	  protected internal bool isProcessDefinitionTenantIdSet = false;

	  protected internal ProcessInstanceModificationBuilderImpl modificationBuilder;

	  protected internal ProcessInstantiationBuilderImpl(CommandExecutor commandExecutor)
	  {
		modificationBuilder = new ProcessInstanceModificationBuilderImpl();

		this.commandExecutor = commandExecutor;
	  }

	  public virtual ProcessInstantiationBuilder startBeforeActivity(string activityId)
	  {
		modificationBuilder.startBeforeActivity(activityId);
		return this;
	  }

	  public virtual ProcessInstantiationBuilder startAfterActivity(string activityId)
	  {
		modificationBuilder.startAfterActivity(activityId);
		return this;
	  }

	  public virtual ProcessInstantiationBuilder startTransition(string transitionId)
	  {
		modificationBuilder.startTransition(transitionId);
		return this;
	  }

	  public virtual ProcessInstantiationBuilder setVariable(string name, object value)
	  {
		modificationBuilder.setVariable(name, value);
		return this;
	  }

	  public virtual ProcessInstantiationBuilder setVariableLocal(string name, object value)
	  {
		modificationBuilder.setVariableLocal(name, value);
		return this;
	  }

	  public virtual ProcessInstantiationBuilder setVariables(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  modificationBuilder.Variables = variables;
		}
		return this;
	  }

	  public virtual ProcessInstantiationBuilder setVariablesLocal(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  modificationBuilder.VariablesLocal = variables;
		}
		return this;
	  }

	  public virtual ProcessInstantiationBuilder businessKey(string businessKey)
	  {
		this.businessKey_Conflict = businessKey;
		return this;
	  }

	  public virtual ProcessInstantiationBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual ProcessInstantiationBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual ProcessInstantiationBuilder processDefinitionTenantId(string tenantId)
	  {
		this.processDefinitionTenantId_Conflict = tenantId;
		isProcessDefinitionTenantIdSet = true;
		return this;
	  }

	  public virtual ProcessInstantiationBuilder processDefinitionWithoutTenantId()
	  {
		this.processDefinitionTenantId_Conflict = null;
		isProcessDefinitionTenantIdSet = true;
		return this;
	  }

	  public virtual ProcessInstance execute()
	  {
		return execute(false, false);
	  }

	  public virtual ProcessInstance execute(bool skipCustomListeners, bool skipIoMappings)
	  {
		return executeWithVariablesInReturn(skipCustomListeners, skipIoMappings);
	  }

	  public virtual ProcessInstanceWithVariables executeWithVariablesInReturn()
	  {
		return executeWithVariablesInReturn(false, false);
	  }

	  public virtual ProcessInstanceWithVariables executeWithVariablesInReturn(bool skipCustomListeners, bool skipIoMappings)
	  {
		ensureOnlyOneNotNull("either process definition id or key must be set", processDefinitionId, processDefinitionKey);

		if (isProcessDefinitionTenantIdSet && !string.ReferenceEquals(processDefinitionId, null))
		{
		  throw LOG.exceptionStartProcessInstanceByIdAndTenantId();
		}

		Command<ProcessInstanceWithVariables> command;

		if (modificationBuilder.ModificationOperations.Count == 0)
		{

		  if (skipCustomListeners || skipIoMappings)
		  {
			throw LOG.exceptionStartProcessInstanceAtStartActivityAndSkipListenersOrMapping();
		  }
		  // start at the default start activity
		  command = new StartProcessInstanceCmd(this);

		}
		else
		{
		  // start at any activity using the instructions
		  modificationBuilder.SkipCustomListeners = skipCustomListeners;
		  modificationBuilder.SkipIoMappings = skipIoMappings;

		  command = new StartProcessInstanceAtActivitiesCmd(this);
		}

		return commandExecutor.execute(command);
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual ProcessInstanceModificationBuilderImpl ModificationBuilder
	  {
		  get
		  {
			return modificationBuilder;
		  }
		  set
		  {
			this.modificationBuilder = value;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return modificationBuilder.ProcessVariables;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionTenantId
	  {
		  get
		  {
			return processDefinitionTenantId_Conflict;
		  }
	  }

	  public virtual bool ProcessDefinitionTenantIdSet
	  {
		  get
		  {
			return isProcessDefinitionTenantIdSet;
		  }
	  }


	  public static ProcessInstantiationBuilder createProcessInstanceById(CommandExecutor commandExecutor, string processDefinitionId)
	  {
		ProcessInstantiationBuilderImpl builder = new ProcessInstantiationBuilderImpl(commandExecutor);
		builder.processDefinitionId = processDefinitionId;
		return builder;
	  }

	  public static ProcessInstantiationBuilder createProcessInstanceByKey(CommandExecutor commandExecutor, string processDefinitionKey)
	  {
		ProcessInstantiationBuilderImpl builder = new ProcessInstantiationBuilderImpl(commandExecutor);
		builder.processDefinitionKey = processDefinitionKey;
		return builder;
	  }

	}

}