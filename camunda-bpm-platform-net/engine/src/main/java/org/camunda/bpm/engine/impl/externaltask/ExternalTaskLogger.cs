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
namespace org.camunda.bpm.engine.impl.externaltask
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// Represents the logger for the external task.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ExternalTaskLogger : ProcessEngineLogger
	{

	  /// <summary>
	  /// Logs that the priority could not be determined in the given context.
	  /// </summary>
	  /// <param name="execution"> the context that is used for determining the priority </param>
	  /// <param name="value"> the default value </param>
	  /// <param name="e"> the exception which was catched </param>
	  public virtual void couldNotDeterminePriority(ExecutionEntity execution, object value, ProcessEngineException e)
	  {
		logWarn("001", "Could not determine priority for external task created in context of execution {}. Using default priority {}", execution, value, e);
	  }
	}

}