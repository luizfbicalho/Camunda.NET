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
namespace org.camunda.bpm.engine.impl.scripting
{

	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// <para>Represents an executable script.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class ExecutableScript
	{

	  /// <summary>
	  /// The language of the script. Used to resolve the
	  /// <seealso cref="ScriptEngine"/>. 
	  /// </summary>
	  protected internal readonly string language;

	  protected internal ExecutableScript(string language)
	  {
		this.language = language;
	  }

	  /// <summary>
	  /// The language in which the script is written. </summary>
	  /// <returns> the language </returns>
	  public virtual string Language
	  {
		  get
		  {
			return language;
		  }
	  }

	  /// <summary>
	  /// <para>Evaluates the script using the provided engine and bindings</para>
	  /// </summary>
	  /// <param name="scriptEngine"> the script engine to use for evaluating the script. </param>
	  /// <param name="variableScope"> the variable scope of the execution </param>
	  /// <param name="bindings"> the bindings to use for evaluating the script. </param>
	  /// <exception cref="ProcessEngineException"> in case the script cannot be evaluated. </exception>
	  /// <returns> the result of the script evaluation </returns>
	  public virtual object execute(ScriptEngine scriptEngine, VariableScope variableScope, Bindings bindings)
	  {
		return evaluate(scriptEngine, variableScope, bindings);
	  }

	  protected internal virtual string getActivityIdExceptionMessage(VariableScope variableScope)
	  {
		string activityId = null;
		string definitionIdMessage = "";

		if (variableScope is DelegateExecution)
		{
		  activityId = ((DelegateExecution) variableScope).CurrentActivityId;
		  definitionIdMessage = " in the process definition with id '" + ((DelegateExecution) variableScope).ProcessDefinitionId + "'";
		}
		else if (variableScope is TaskEntity)
		{
		  TaskEntity task = (TaskEntity) variableScope;
		  if (task.getExecution() != null)
		  {
			activityId = task.getExecution().ActivityId;
			definitionIdMessage = " in the process definition with id '" + task.ProcessDefinitionId + "'";
		  }
		  if (task.getCaseExecution() != null)
		  {
			activityId = task.getCaseExecution().ActivityId;
			definitionIdMessage = " in the case definition with id '" + task.CaseDefinitionId + "'";
		  }
		}
		else if (variableScope is DelegateCaseExecution)
		{
		  activityId = ((DelegateCaseExecution) variableScope).ActivityId;
		  definitionIdMessage = " in the case definition with id '" + ((DelegateCaseExecution) variableScope).CaseDefinitionId + "'";
		}

		if (string.ReferenceEquals(activityId, null))
		{
		  return "";
		}
		else
		{
		  return " while executing activity '" + activityId + "'" + definitionIdMessage;
		}
	  }

	  protected internal abstract object evaluate(ScriptEngine scriptEngine, VariableScope variableScope, Bindings bindings);

	}

}