using System;

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
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class GetCaseExecutionVariableTypedCmd : Command<TypedValue>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseExecutionId;
	  protected internal string variableName;
	  protected internal bool isLocal;
	  protected internal bool deserializeValue;

	  public GetCaseExecutionVariableTypedCmd(string caseExecutionId, string variableName, bool isLocal, bool deserializeValue)
	  {
		this.caseExecutionId = caseExecutionId;
		this.variableName = variableName;
		this.isLocal = isLocal;
		this.deserializeValue = deserializeValue;
	  }

	  public virtual TypedValue execute(CommandContext commandContext)
	  {
		ensureNotNull("caseExecutionId", caseExecutionId);
		ensureNotNull("variableName", variableName);

		CaseExecutionEntity caseExecution = commandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);

		ensureNotNull(typeof(CaseExecutionNotFoundException), "case execution " + caseExecutionId + " doesn't exist", "caseExecution", caseExecution);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadCaseInstance(caseExecution);
		}

		TypedValue value;

		if (isLocal)
		{
		  value = caseExecution.getVariableLocalTyped(variableName, deserializeValue);
		}
		else
		{
		  value = caseExecution.getVariableTyped(variableName, deserializeValue);
		}

		return value;
	  }

	}

}