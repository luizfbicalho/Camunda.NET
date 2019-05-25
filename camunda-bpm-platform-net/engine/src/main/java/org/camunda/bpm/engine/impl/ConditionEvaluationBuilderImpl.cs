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


	using EvaluateStartConditionCmd = org.camunda.bpm.engine.impl.cmd.EvaluateStartConditionCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ConditionEvaluationBuilder = org.camunda.bpm.engine.runtime.ConditionEvaluationBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	public class ConditionEvaluationBuilderImpl : ConditionEvaluationBuilder
	{
	  protected internal CommandExecutor commandExecutor;

	  protected internal string businessKey;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;

	  protected internal VariableMap variables = new VariableMapImpl();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict = null;
	  protected internal bool isTenantIdSet = false;

	  public ConditionEvaluationBuilderImpl(CommandExecutor commandExecutor)
	  {
		ensureNotNull("commandExecutor", commandExecutor);
		this.commandExecutor = commandExecutor;
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual VariableMap getVariables()
	  {
		return variables;
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

	  protected internal virtual T execute<T>(Command<T> command)
	  {
		return commandExecutor.execute(command);
	  }

	  public virtual ConditionEvaluationBuilder processInstanceBusinessKey(string businessKey)
	  {
		ensureNotNull("businessKey", businessKey);
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual ConditionEvaluationBuilder processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual ConditionEvaluationBuilder setVariable(string variableName, object variableValue)
	  {
		ensureNotNull("variableName", variableName);
		this.variables.put(variableName, variableValue);
		return this;
	  }

	  public virtual ConditionEvaluationBuilder setVariables(IDictionary<string, object> variables)
	  {
		ensureNotNull("variables", variables);
		if (variables != null)
		{
		  this.variables.putAll(variables);
		}
		return this;
	  }

	  public virtual ConditionEvaluationBuilder tenantId(string tenantId)
	  {
		ensureNotNull("The tenant-id cannot be null. Use 'withoutTenantId()' if you want to evaluate conditional start event with a process definition which has no tenant-id.", "tenantId", tenantId);

		isTenantIdSet = true;
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual ConditionEvaluationBuilder withoutTenantId()
	  {
		isTenantIdSet = true;
		tenantId_Conflict = null;
		return this;
	  }

	  public virtual IList<ProcessInstance> evaluateStartConditions()
	  {
		return execute(new EvaluateStartConditionCmd(this));
	  }

	}

}