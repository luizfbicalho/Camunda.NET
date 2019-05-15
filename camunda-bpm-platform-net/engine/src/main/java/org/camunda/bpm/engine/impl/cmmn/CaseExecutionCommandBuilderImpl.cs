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
	using CaseExecutionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseExecutionNotFoundException;
	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using CaseExecutionVariableCmd = org.camunda.bpm.engine.impl.cmmn.cmd.CaseExecutionVariableCmd;
	using CloseCaseInstanceCmd = org.camunda.bpm.engine.impl.cmmn.cmd.CloseCaseInstanceCmd;
	using CompleteCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.CompleteCaseExecutionCmd;
	using DisableCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.DisableCaseExecutionCmd;
	using ManualStartCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.ManualStartCaseExecutionCmd;
	using ReenableCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.ReenableCaseExecutionCmd;
	using TerminateCaseExecutionCmd = org.camunda.bpm.engine.impl.cmmn.cmd.TerminateCaseExecutionCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CaseExecutionCommandBuilder = org.camunda.bpm.engine.runtime.CaseExecutionCommandBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionCommandBuilderImpl : CaseExecutionCommandBuilder
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandContext commandContext;

	  protected internal string caseExecutionId;
	  protected internal VariableMapImpl variables;
	  protected internal VariableMapImpl variablesLocal;
	  protected internal ICollection<string> variableDeletions;
	  protected internal ICollection<string> variableLocalDeletions;

	  public CaseExecutionCommandBuilderImpl(CommandExecutor commandExecutor, string caseExecutionId) : this(caseExecutionId)
	  {
		ensureNotNull("commandExecutor", commandExecutor);
		this.commandExecutor = commandExecutor;
	  }

	  public CaseExecutionCommandBuilderImpl(CommandContext commandContext, string caseExecutionId) : this(caseExecutionId)
	  {
		ensureNotNull("commandContext", commandContext);
		this.commandContext = commandContext;
	  }

	  private CaseExecutionCommandBuilderImpl(string caseExecutionId)
	  {
		this.caseExecutionId = caseExecutionId;
	  }

	  public virtual CaseExecutionCommandBuilder setVariable(string variableName, object variableValue)
	  {
		ensureNotNull(typeof(NotValidException), "variableName", variableName);
		ensureVariableShouldNotBeRemoved(variableName);
		ensureVariablesInitialized();
		variables.put(variableName, variableValue);
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder setVariables(IDictionary<string, object> variables)
	  {
		if (variables != null)
		{
		  ensureVariablesShouldNotBeRemoved(variables.Keys);
		  ensureVariablesInitialized();
		  this.variables.putAll(variables);
		}
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder setVariableLocal(string localVariableName, object localVariableValue)
	  {
		ensureNotNull(typeof(NotValidException), "localVariableName", localVariableName);
		ensureVariableShouldNotBeRemoved(localVariableName);
		ensureVariablesLocalInitialized();
		variablesLocal.put(localVariableName, localVariableValue);
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder setVariablesLocal(IDictionary<string, object> variablesLocal)
	  {
		if (variablesLocal != null)
		{
		  ensureVariablesShouldNotBeRemoved(variablesLocal.Keys);
		  ensureVariablesLocalInitialized();
		  this.variablesLocal.putAll(variablesLocal);
		}
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder removeVariable(string variableName)
	  {
		ensureNotNull(typeof(NotValidException), "variableName", variableName);
		ensureVariableShouldNotBeSet(variableName);
		ensureVariableDeletionsInitialized();
		variableDeletions.Add(variableName);
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder removeVariables(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  ensureVariablesShouldNotBeSet(variableNames);
		  ensureVariableDeletionsInitialized();
		  variableDeletions.addAll(variableNames);
		}
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder removeVariableLocal(string variableName)
	  {
		ensureNotNull(typeof(NotValidException), "localVariableName", variableName);
		ensureVariableShouldNotBeSet(variableName);
		ensureVariableDeletionsLocalInitialized();
		variableLocalDeletions.Add(variableName);
		return this;
	  }

	  public virtual CaseExecutionCommandBuilder removeVariablesLocal(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  ensureVariablesShouldNotBeSet(variableNames);
		  ensureVariableDeletionsLocalInitialized();
		  variableLocalDeletions.addAll(variableNames);
		}
		return this;
	  }

	  protected internal virtual void ensureVariablesShouldNotBeRemoved(ICollection<string> variableNames)
	  {
		foreach (string variableName in variableNames)
		{
		  ensureVariableShouldNotBeRemoved(variableName);
		}
	  }

	  protected internal virtual void ensureVariableShouldNotBeRemoved(string variableName)
	  {
		if ((variableDeletions != null && variableDeletions.Contains(variableName)) || (variableLocalDeletions != null && variableLocalDeletions.Contains(variableName)))
		{
		  throw new NotValidException("Cannot set and remove a variable with the same variable name: '" + variableName + "' within a command.");
		}
	  }

	  protected internal virtual void ensureVariablesShouldNotBeSet(ICollection<string> variableNames)
	  {
		foreach (string variableName in variableNames)
		{
		  ensureVariableShouldNotBeSet(variableName);
		}
	  }

	  protected internal virtual void ensureVariableShouldNotBeSet(string variableName)
	  {
		if ((variables != null && variables.Keys.Contains(variableName)) || (variablesLocal != null && variablesLocal.Keys.Contains(variableName)))
		{
		  throw new NotValidException("Cannot set and remove a variable with the same variable name: '" + variableName + "' within a command.");
		}
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		if (this.variables == null)
		{
		  this.variables = new VariableMapImpl();
		}
	  }

	  protected internal virtual void ensureVariablesLocalInitialized()
	  {
		if (this.variablesLocal == null)
		{
		  this.variablesLocal = new VariableMapImpl();
		}
	  }

	  protected internal virtual void ensureVariableDeletionsInitialized()
	  {
		if (variableDeletions == null)
		{
		  variableDeletions = new List<string>();
		}
	  }

	  protected internal virtual void ensureVariableDeletionsLocalInitialized()
	  {
		if (variableLocalDeletions == null)
		{
		  variableLocalDeletions = new List<string>();
		}
	  }


	  public virtual void execute()
	  {
		CaseExecutionVariableCmd command = new CaseExecutionVariableCmd(this);
		executeCommand(command);
	  }

	  public virtual void manualStart()
	  {
		ManualStartCaseExecutionCmd command = new ManualStartCaseExecutionCmd(this);
		executeCommand(command);
	  }

	  public virtual void disable()
	  {
		DisableCaseExecutionCmd command = new DisableCaseExecutionCmd(this);
		executeCommand(command);
	  }

	  public virtual void reenable()
	  {
		ReenableCaseExecutionCmd command = new ReenableCaseExecutionCmd(this);
		executeCommand(command);
	  }

	  public virtual void complete()
	  {
		CompleteCaseExecutionCmd command = new CompleteCaseExecutionCmd(this);
		executeCommand(command);
	  }

	  public virtual void close()
	  {
		CloseCaseInstanceCmd command = new CloseCaseInstanceCmd(this);
		executeCommand(command);
	  }

	  public virtual void terminate()
	  {
		TerminateCaseExecutionCmd command = new TerminateCaseExecutionCmd(this);
		executeCommand(command);
	  }

	  protected internal virtual void executeCommand<T1>(Command<T1> command)
	  {
		try
		{
		  if (commandExecutor != null)
		  {
			commandExecutor.execute(command);
		  }
		  else
		  {
			command.execute(commandContext);
		  }

		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);

		}
		catch (CaseExecutionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
		catch (CaseDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
		catch (CaseIllegalStateTransitionException e)
		{
		  throw new NotAllowedException(e.Message, e);

		}

	  }

	  // getters ////////////////////////////////////////////////////////////////////////////////

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual VariableMap getVariables()
	  {
		return variables;
	  }

	  public virtual VariableMap getVariablesLocal()
	  {
		return variablesLocal;
	  }

	  public virtual ICollection<string> VariableDeletions
	  {
		  get
		  {
			return variableDeletions;
		  }
	  }

	  public virtual ICollection<string> VariableLocalDeletions
	  {
		  get
		  {
			return variableLocalDeletions;
		  }
	  }

	}

}