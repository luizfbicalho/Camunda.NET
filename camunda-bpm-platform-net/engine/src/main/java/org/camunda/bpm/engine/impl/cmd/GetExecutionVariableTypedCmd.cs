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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class GetExecutionVariableTypedCmd<T> : Command<T> where T : org.camunda.bpm.engine.variable.value.TypedValue
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;
	  protected internal string variableName;
	  protected internal bool isLocal;
	  protected internal bool deserializeValue;

	  public GetExecutionVariableTypedCmd(string executionId, string variableName, bool isLocal, bool deserializeValue)
	  {
		this.executionId = executionId;
		this.variableName = variableName;
		this.isLocal = isLocal;
		this.deserializeValue = deserializeValue;
	  }

	  public virtual T execute(CommandContext commandContext)
	  {
		ensureNotNull("executionId", executionId);
		ensureNotNull("variableName", variableName);

		ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(executionId);

		ensureNotNull("execution " + executionId + " doesn't exist", "execution", execution);

		checkGetExecutionVariableTyped(execution, commandContext);

		T value;

		if (isLocal)
		{
		  value = execution.getVariableLocalTyped(variableName, deserializeValue);
		}
		else
		{
		  value = execution.getVariableTyped(variableName, deserializeValue);
		}

		return value;
	  }

	  public virtual void checkGetExecutionVariableTyped(ExecutionEntity execution, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessInstanceVariable(execution);
		}
	  }
	}

}