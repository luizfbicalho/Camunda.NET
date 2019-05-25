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
namespace org.camunda.bpm.engine.impl.cmmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using CreateCaseInstanceCmd = org.camunda.bpm.engine.impl.cmmn.cmd.CreateCaseInstanceCmd;
	using CmmnOperationLogger = org.camunda.bpm.engine.impl.cmmn.operation.CmmnOperationLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceBuilderImpl : CaseInstanceBuilder
	{

	  private static readonly CmmnOperationLogger LOG = ProcessEngineLogger.CMMN_OPERATION_LOGGER;

	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandContext commandContext;

	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string businessKey_Conflict;
	  protected internal VariableMap variables;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionTenantId_Conflict;
	  protected internal bool isTenantIdSet = false;

	  public CaseInstanceBuilderImpl(CommandExecutor commandExecutor, string caseDefinitionKey, string caseDefinitionId) : this(caseDefinitionKey, caseDefinitionId)
	  {
		ensureNotNull("commandExecutor", commandExecutor);
		this.commandExecutor = commandExecutor;
	  }

	  public CaseInstanceBuilderImpl(CommandContext commandContext, string caseDefinitionKey, string caseDefinitionId) : this(caseDefinitionKey, caseDefinitionId)
	  {
		ensureNotNull("commandContext", commandContext);
		this.commandContext = commandContext;
	  }

	  private CaseInstanceBuilderImpl(string caseDefinitionKey, string caseDefinitionId)
	  {
		this.caseDefinitionKey = caseDefinitionKey;
		this.caseDefinitionId = caseDefinitionId;
	  }

	  public virtual CaseInstanceBuilder businessKey(string businessKey)
	  {
		this.businessKey_Conflict = businessKey;
		return this;
	  }

	  public virtual CaseInstanceBuilder caseDefinitionTenantId(string tenantId)
	  {
		this.caseDefinitionTenantId_Conflict = tenantId;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseInstanceBuilder caseDefinitionWithoutTenantId()
	  {
		this.caseDefinitionTenantId_Conflict = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseInstanceBuilder setVariable(string variableName, object variableValue)
	  {
		ensureNotNull(typeof(NotValidException), "variableName", variableName);
		if (variables == null)
		{
		  variables = Variables.createVariables();
		}
		variables.putValue(variableName, variableValue);
		return this;
	  }

	  public virtual CaseInstanceBuilder setVariables(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  if (this.variables == null)
		  {
			this.variables = Variables.fromMap(variables);
		  }
		  else
		  {
			this.variables.putAll(variables);
		  }
		}
		return this;
	  }

	  public virtual CaseInstance create()
	  {
		if (isTenantIdSet && !string.ReferenceEquals(caseDefinitionId, null))
		{
		  throw LOG.exceptionCreateCaseInstanceByIdAndTenantId();
		}

		try
		{
		  CreateCaseInstanceCmd command = new CreateCaseInstanceCmd(this);
		  if (commandExecutor != null)
		  {
			return commandExecutor.execute(command);
		  }
		  else
		  {
			return command.execute(commandContext);
		  }

		}
		catch (CaseDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (CaseIllegalStateTransitionException e)
		{
		  throw new NotAllowedException(e.Message, e);

		}
	  }

	  // getters ////////////////////////////////////

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey_Conflict;
		  }
	  }

	  public virtual VariableMap getVariables()
	  {
		return variables;
	  }

	  public virtual string CaseDefinitionTenantId
	  {
		  get
		  {
			return caseDefinitionTenantId_Conflict;
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