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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// Provides access to the snapshot of latest variables of an execution.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ExecutionVariableSnapshotObserver : ExecutionObserver
	{

	  /// <summary>
	  /// The variables which are observed during the execution.
	  /// </summary>
	  protected internal VariableMap variableSnapshot;

	  protected internal ExecutionEntity execution;

	  protected internal bool localVariables = true;
	  protected internal bool deserializeValues = false;

	  public ExecutionVariableSnapshotObserver(ExecutionEntity executionEntity) : this(executionEntity, true, false)
	  {
	  }

	  public ExecutionVariableSnapshotObserver(ExecutionEntity executionEntity, bool localVariables, bool deserializeValues)
	  {
		this.execution = executionEntity;
		this.execution.addExecutionObserver(this);
		this.localVariables = localVariables;
		this.deserializeValues = deserializeValues;
	  }

	  public virtual void onClear(ExecutionEntity execution)
	  {
		if (variableSnapshot == null)
		{
		  variableSnapshot = getVariables(this.localVariables);
		}
	  }

	  public virtual VariableMap Variables
	  {
		  get
		  {
			if (variableSnapshot == null)
			{
			  return getVariables(this.localVariables);
			}
			else
			{
			  return variableSnapshot;
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.camunda.bpm.engine.variable.VariableMap getVariables(final boolean localVariables)
	  private VariableMap getVariables(bool localVariables)
	  {
		return this.localVariables ? execution.getVariablesLocalTyped(deserializeValues) : execution.getVariablesTyped(deserializeValues);
	  }
	}

}