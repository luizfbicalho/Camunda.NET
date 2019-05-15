using System;
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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CaseExecutionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseExecutionNotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseExecutionVariableCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseExecutionId;
	  protected internal IDictionary<string, object> variables;
	  protected internal IDictionary<string, object> variablesLocal;

	  protected internal ICollection<string> variablesDeletions;
	  protected internal ICollection<string> variablesLocalDeletions;

	  protected internal CaseExecutionEntity caseExecution;

	  public CaseExecutionVariableCmd(string caseExecutionId, IDictionary<string, object> variables, IDictionary<string, object> variablesLocal, ICollection<string> variablesDeletions, ICollection<string> variablesLocalDeletions)
	  {
		this.caseExecutionId = caseExecutionId;
		this.variables = variables;
		this.variablesLocal = variablesLocal;
		this.variablesDeletions = variablesDeletions;
		this.variablesLocalDeletions = variablesLocalDeletions;

	  }

	  public CaseExecutionVariableCmd(CaseExecutionCommandBuilderImpl builder) : this(builder.CaseExecutionId, builder.getVariables(), builder.getVariablesLocal(), builder.VariableDeletions, builder.VariableLocalDeletions)
	  {
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("caseExecutionId", caseExecutionId);

		caseExecution = commandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);

		ensureNotNull(typeof(CaseExecutionNotFoundException), "There does not exist any case execution with id: '" + caseExecutionId + "'", "caseExecution", caseExecution);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateCaseInstance(caseExecution);
		}

		if (variablesDeletions != null && variablesDeletions.Count > 0)
		{
		  caseExecution.removeVariables(variablesDeletions);
		}

		if (variablesLocalDeletions != null && variablesLocalDeletions.Count > 0)
		{
		  caseExecution.removeVariablesLocal(variablesLocalDeletions);
		}

		if (variables != null && variables.Count > 0)
		{
		  caseExecution.Variables = variables;
		}

		if (variablesLocal != null && variablesLocal.Count > 0)
		{
		  caseExecution.VariablesLocal = variablesLocal;
		}

		return null;
	  }

	  public virtual CaseExecutionEntity CaseExecution
	  {
		  get
		  {
			return caseExecution;
		  }
	  }

	}

}