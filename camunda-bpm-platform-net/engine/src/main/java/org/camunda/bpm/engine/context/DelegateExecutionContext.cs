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
namespace org.camunda.bpm.engine.context
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using BpmnExecutionContext = org.camunda.bpm.engine.impl.context.BpmnExecutionContext;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// Represents a delegation execution context which should return the current
	/// delegation execution.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DelegateExecutionContext
	{

	  /// <summary>
	  /// Returns the current delegation execution or null if the
	  /// execution is not available.
	  /// </summary>
	  /// <returns> the current delegation execution or null if not available </returns>
	  public static DelegateExecution CurrentDelegationExecution
	  {
		  get
		  {
			BpmnExecutionContext bpmnExecutionContext = Context.BpmnExecutionContext;
			ExecutionEntity executionEntity = null;
			if (bpmnExecutionContext != null)
			{
			  executionEntity = bpmnExecutionContext.Execution;
			}
			return executionEntity;
		  }
	  }
	}

}