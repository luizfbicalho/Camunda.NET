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
namespace org.camunda.bpm.engine.runtime
{
	/// <summary>
	/// <para>The result of a message correlation. A message may be correlated to either
	/// a waiting execution (BPMN receive message event) or a process definition
	/// (BPMN message start event). The type of the correlation (execution vs.
	/// processDefinition) can be obtained using <seealso cref="getResultType()"/></para>
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// @since 7.6
	/// </summary>
	public interface MessageCorrelationResult
	{

	  /// <summary>
	  /// Returns the execution entity on which the message was correlated to.
	  /// </summary>
	  /// <returns> the execution </returns>
	  Execution Execution {get;}

	  /// <summary>
	  /// Returns the process instance id on which the message was correlated to.
	  /// </summary>
	  /// <returns> the process instance id </returns>
	  ProcessInstance ProcessInstance {get;}

	  /// <summary>
	  /// Returns the result type of the message correlation result.
	  /// Indicates if either the message was correlated to a waiting execution
	  /// or to a process definition like a start event.
	  /// </summary>
	  /// <returns> the result type of the message correlation result </returns>
	  /// <seealso cref= <seealso cref="MessageCorrelationResultType"/> </seealso>
	  MessageCorrelationResultType ResultType {get;}
	}

}