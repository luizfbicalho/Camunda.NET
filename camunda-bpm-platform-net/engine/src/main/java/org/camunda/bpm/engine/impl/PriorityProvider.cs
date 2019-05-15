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
namespace org.camunda.bpm.engine.impl
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com> </summary>
	/// @param <T> the type of the extra param to determine the priority </param>
	public interface PriorityProvider<T>
	{

	  /// <param name="execution"> may be null when the job is not created in the context of a
	  ///   running process instance (e.g. a timer start event) </param>
	  /// <param name="param"> extra parameter to determine priority on </param>
	  /// <param name="jobDefinitionId"> the job definition id if related to a job </param>
	  /// <returns> the determined priority </returns>
	  long determinePriority(ExecutionEntity execution, T param, string jobDefinitionId);

	}

}