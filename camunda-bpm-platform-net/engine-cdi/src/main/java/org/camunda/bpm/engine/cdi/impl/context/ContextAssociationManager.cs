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
namespace org.camunda.bpm.engine.cdi.impl.context
{
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Represents a means for associating an execution with a context.
	/// <p />
	/// This enables activiti-cdi to provide contextual business process management
	/// services, without relying on a specific context like i.e. the conversation
	/// context.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface ContextAssociationManager
	{

	  /// <summary>
	  /// Disassociates the current process instance with a context / scope
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> if no process instance is currently associated </exception>
	  void disAssociate();

	  /// <returns> the id of the execution currently associated or null </returns>
	  string ExecutionId {get;}

	  /// <summary>
	  /// get the current execution
	  /// </summary>
	  Execution Execution {get;set;}


	  /// <summary>
	  /// set a current task
	  /// </summary>
	  Task Task {set;get;}


	  /// <summary>
	  /// set a process variable
	  /// </summary>
	  void setVariable(string variableName, object value);

	  /// <summary>
	  /// get a process variable
	  /// </summary>
	  TypedValue getVariable(string variableName);

	  /// <returns> a <seealso cref="VariableMap"/> of process variables cached between flushes </returns>
	  VariableMap CachedVariables {get;}

	  /// <summary>
	  /// set a local process variable
	  /// </summary>
	  void setVariableLocal(string variableName, object value);

	  /// <summary>
	  /// get a local process variable
	  /// </summary>
	  TypedValue getVariableLocal(string variableName);

	  /// <returns> a <seealso cref="VariableMap"/> of local process variables cached between flushes </returns>
	  VariableMap CachedLocalVariables {get;}

	  /// <summary>
	  /// allows to flush the cached variables.
	  /// </summary>
	  void flushVariableCache();

	}

}